using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Perforator
{
    [AutoloadBossHead]
    [HasPierceResist]
    [LongDistanceNetSync]
    public class PerforatorHeadLarge : ModNPC
    {
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/PerfLargeHit", 3);
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/PerfLargeDeath");

        public static Asset<Texture2D> GlowTexture;
        private bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.75f,
                PortraitScale = 0.75f,
                PortraitPositionXOverride = 40,
                PortraitPositionYOverride = 40
            };
            value.Position.X += 70;
            value.Position.Y += 40;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        // GFB exclusive
        public static int LaserWallDamage = 10; // 40

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 40; // 80
            NPC.npcSlots = 5f;
            NPC.width = 70;
            NPC.height = 84;
            NPC.defense = 4;

            NPC.LifeMaxNERB(2000, 2600, 80000);
            if (Main.zenithWorld)
                NPC.lifeMax *= 4;

            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.netAlways = true;

            if (CalamityWorld.death || BossRushEvent.BossRushActive)
                NPC.scale *= 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.15f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<PerforatorHive>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCrimson,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Perforator")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            float speed = 0.1f;
            float turnSpeed = 0.07f;

            if (expertMode)
            {
                float velocityScale = death ? 0.1f : 0.07f;
                speed += velocityScale * (1f - lifeRatio);
                float accelerationScale = death ? 0.07f : 0.05f;
                turnSpeed += accelerationScale * (1f - lifeRatio);
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Spit attack in Rev
            // Spit ichor blobs in Death
            float spitDistance = 960f;
            float tooCloseToSpitDistance = 320f;
            bool isInRangeToSpit = NPC.Distance(player.Center) <= spitDistance && NPC.Distance(player.Center) > tooCloseToSpitDistance;
            bool headIsTurnedTowardsTarget = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation();
            bool canHitTarget = Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
            bool alwaysAbleToSpit = NPC.Calamity().newAI[1] == 1f;
            bool canSpit = (isInRangeToSpit && headIsTurnedTowardsTarget && canHitTarget) || alwaysAbleToSpit;

            if (canSpit)
            {
                float spitGateValue = 120f;
                if (NPC.Calamity().newAI[0] < spitGateValue)
                    NPC.Calamity().newAI[0] += 1f;

                // Only spit if all the conditions are met, in order to make the attack actually dangerous
                bool spit = NPC.Calamity().newAI[0] >= spitGateValue && isInRangeToSpit && headIsTurnedTowardsTarget && canHitTarget;

                // Telegraph for half a second, or for however long it takes for the spit conditions to be met
                float telegraphSpitGateValue = spitGateValue - 30f;
                bool telegraphSpit = NPC.Calamity().newAI[0] >= telegraphSpitGateValue;

                // Spit from the mouth hole thing...yeah
                Vector2 spitLocation = NPC.Center + NPC.velocity.SafeNormalize(Vector2.UnitY) * 20f;
                if (telegraphSpit)
                {
                    NPC.Calamity().newAI[1] = 1f;
                    int dustType = Main.rand.NextBool() ? DustID.Ichor : DustID.Blood;
                    for (int k = 0; k < 10; k++)
                    {
                        int dust = Dust.NewDust(spitLocation, 1, 1, dustType);
                        Main.dust[dust].position = spitLocation + Main.rand.NextVector2CircularEdge(25f, 25f);
                        Main.dust[dust].velocity = (spitLocation - Main.dust[dust].position).SafeNormalize(Vector2.UnitY) * 2f;
                        Main.dust[dust].scale = dustType == DustID.Ichor ? 1f : 2f;
                        Main.dust[dust].noGravity = true;
                    }
                }

                if (spit)
                {
                    NPC.Calamity().newAI[0] = 0f;
                    NPC.Calamity().newAI[1] = 0f;

                    SoundEngine.PlaySound(SoundID.NPCDeath13, spitLocation);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int spitProjectileAmount = 8;
                        float spitProjectileBaseVelocity = 16f;
                        float spitProjectileRandomVelocityLimit = 3f;
                        for (int i = 0; i < spitProjectileAmount; i++)
                        {
                            int type = Main.rand.NextBool() ? ModContent.ProjectileType<IchorShot>() : ModContent.ProjectileType<BloodGeyser>();
                            int damage = type == ModContent.ProjectileType<IchorShot>() ? PerforatorHive.IchorShotDamage : PerforatorHive.BloodGeyserDamage;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(),
                                spitLocation + Main.rand.NextVector2CircularEdge(8f, 8f),
                                NPC.velocity.SafeNormalize(Vector2.UnitY) * spitProjectileBaseVelocity + Main.rand.NextVector2CircularEdge(spitProjectileRandomVelocityLimit, spitProjectileRandomVelocityLimit),
                                type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
                        }

                        // Spit blobs
                        if (death)
                        {
                            int spitBlobAmount = 3;
                            float spitBlobBaseVelocity = 8f;
                            float spitBlobRandomVelocityLimit = 2f;
                            for (int i = 0; i < spitBlobAmount; i++)
                            {
                                int type = ModContent.ProjectileType<IchorBlob>();
                                Projectile.NewProjectile(NPC.GetSource_FromAI(),
                                    spitLocation + Main.rand.NextVector2CircularEdge(8f, 8f),
                                    NPC.velocity.SafeNormalize(Vector2.UnitY) * spitBlobBaseVelocity + Main.rand.NextVector2CircularEdge(spitBlobRandomVelocityLimit, spitBlobRandomVelocityLimit),
                                    type, PerforatorHive.IchorShotDamage, 0f, Main.myPlayer, 0f, player.Center.Y);
                            }
                        }
                    }
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned)
                {
                    int Previous = NPC.whoAmI;
                    int maxLength = death ? 27 : expertMode ? 21 : 15;
                    for (int segments = 0; segments < maxLength; segments++)
                    {
                        int lol;
                        if (segments >= 0 && segments < maxLength - 1)
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorBodyLarge>(), NPC.whoAmI);
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorTailLarge>(), NPC.whoAmI);

                        if (segments % 2 == 0)
                            Main.npc[lol].localAI[3] = 1f;

                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }

                    TailSpawned = true;
                }
            }

            int tilePositionX = (int)(NPC.position.X / 16f) - 1;
            int tileWidthPosX = (int)((NPC.position.X + (float)NPC.width) / 16f) + 2;
            int tilePositionY = (int)(NPC.position.Y / 16f) - 1;
            int tileWidthPosY = (int)((NPC.position.Y + (float)NPC.height) / 16f) + 2;
            if (tilePositionX < 0)
                tilePositionX = 0;
            if (tileWidthPosX > Main.maxTilesX)
                tileWidthPosX = Main.maxTilesX;
            if (tilePositionY < 0)
                tilePositionY = 0;
            if (tileWidthPosY > Main.maxTilesY)
                tileWidthPosY = Main.maxTilesY;

            bool shouldFly = false;
            if (!shouldFly)
            {
                for (int k = tilePositionX; k < tileWidthPosX; k++)
                {
                    for (int l = tilePositionY; l < tileWidthPosY; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[k, l].TileType] || (Main.tileSolidTop[(int)Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            Vector2 vector2;
                            vector2.X = (float)(k * 16);
                            vector2.Y = (float)(l * 16);
                            if (NPC.position.X + (float)NPC.width > vector2.X && NPC.position.X < vector2.X + 16f && NPC.position.Y + (float)NPC.height > vector2.Y && NPC.position.Y < vector2.Y + 16f)
                            {
                                shouldFly = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (!shouldFly)
            {
                NPC.localAI[1] = 1f;
                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int stopFlyingRadius = death ? 160 : revenge ? 200 : expertMode ? 240 : 300;
                bool outsideFlyingRadius = true;
                if (NPC.position.Y > player.position.Y)
                {
                    foreach (Player plr in Main.ActivePlayers)
                    {
                        Rectangle rectangle2 = new Rectangle((int)plr.position.X - stopFlyingRadius, (int)plr.position.Y - stopFlyingRadius, stopFlyingRadius * 2, stopFlyingRadius * 2);
                        if (rectangle.Intersects(rectangle2))
                        {
                            outsideFlyingRadius = false;
                            break;
                        }
                    }
                    if (outsideFlyingRadius)
                        shouldFly = true;
                }
            }
            else
                NPC.localAI[1] = 0f;

            float maxChargeSpeed = 16f;
            if (player.dead || CalamityGlobalNPC.perfHive < 0 || !Main.npc[CalamityGlobalNPC.perfHive].active)
            {
                shouldFly = false;
                NPC.velocity.Y += 1f;
                if ((double)NPC.position.Y > Main.worldSurface * 16D)
                {
                    NPC.velocity.Y += 1f;
                    maxChargeSpeed *= 2f;
                }

                if ((double)NPC.position.Y > Main.rockLayer * 16D)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == ModContent.NPCType<PerforatorHeadLarge>() || Main.npc[a].type == ModContent.NPCType<PerforatorBodyLarge>() || Main.npc[a].type == ModContent.NPCType<PerforatorTailLarge>())
                            Main.npc[a].active = false;
                    }
                }
            }

            // This is possibly the best or worst idea ever conceived
            if (Main.zenithWorld)
            {
                NPC.Calamity().newAI[3]++;
                if (NPC.Calamity().newAI[3] > 180f) // Effectively 10 seconds but give a little headstart in case players kill it too fast
                {
                    if (NPC.Calamity().newAI[3] % 60 == 59)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + Main.rand.NextVector2CircularEdge(600, 600), Vector2.Zero, ModContent.ProjectileType<DoGLaserWalls>(), LaserWallDamage, 0, Main.myPlayer, 0.45f, 180, Main.rand.Next(5));

                    if (NPC.Calamity().newAI[3] >= 300f)
                        NPC.Calamity().newAI[3] = -300f;
                }
            }

            float speedCopy = speed;
            float turnSpeedCopy = turnSpeed;
            Vector2 npcCenter = NPC.Center;
            float playerX = player.Center.X;
            float targettingPosition = player.Center.Y;
            playerX = (float)((int)(playerX / 16f) * 16);
            targettingPosition = (float)((int)(targettingPosition / 16f) * 16);
            npcCenter.X = (float)((int)(npcCenter.X / 16f) * 16);
            npcCenter.Y = (float)((int)(npcCenter.Y / 16f) * 16);
            playerX -= npcCenter.X;
            targettingPosition -= npcCenter.Y;
            float targetDistance = (float)Math.Sqrt((double)(playerX * playerX + targettingPosition * targettingPosition));

            if (!shouldFly)
            {
                NPC.TargetClosest(true);
                NPC.velocity.Y += 0.15f;
                if (NPC.velocity.Y > maxChargeSpeed)
                    NPC.velocity.Y = maxChargeSpeed;

                // This bool exists to stop the strange wiggle behavior when worms are falling down
                bool slowXVelocity = Math.Abs(NPC.velocity.X) > speedCopy;
                if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChargeSpeed * 0.4)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X -= speedCopy * 1.1f;
                    else
                        NPC.velocity.X += speedCopy * 1.1f;
                }
                else if (NPC.velocity.Y == maxChargeSpeed)
                {
                    if (slowXVelocity)
                    {
                        if (NPC.velocity.X < playerX)
                            NPC.velocity.X += speedCopy;
                        else if (NPC.velocity.X > playerX)
                            NPC.velocity.X -= speedCopy;
                    }
                    else
                        NPC.velocity.X = 0f;
                }
                else if (NPC.velocity.Y > 4f)
                {
                    if (slowXVelocity)
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.velocity.X += speedCopy * 0.9f;
                        else
                            NPC.velocity.X -= speedCopy * 0.9f;
                    }
                    else
                        NPC.velocity.X = 0f;
                }
            }
            else
            {
                if (NPC.soundDelay == 0)
                {
                    float soundDelay = targetDistance / 40f;
                    if (soundDelay < 10f)
                        soundDelay = 10f;
                    if (soundDelay > 20f)
                        soundDelay = 20f;

                    NPC.soundDelay = (int)soundDelay;
                    SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
                }

                targetDistance = (float)Math.Sqrt((double)(playerX * playerX + targettingPosition * targettingPosition));
                float absoluteTargetX = Math.Abs(playerX);
                float absoluteTargetPos = Math.Abs(targettingPosition);
                float timeToReachTarget = maxChargeSpeed / targetDistance;
                playerX *= timeToReachTarget;
                targettingPosition *= timeToReachTarget;
                if (((NPC.velocity.X > 0f && playerX > 0f) || (NPC.velocity.X < 0f && playerX < 0f)) && ((NPC.velocity.Y > 0f && targettingPosition > 0f) || (NPC.velocity.Y < 0f && targettingPosition < 0f)))
                {
                    if (NPC.velocity.X < playerX)
                        NPC.velocity.X = NPC.velocity.X + turnSpeedCopy;
                    else if (NPC.velocity.X > playerX)
                        NPC.velocity.X = NPC.velocity.X - turnSpeedCopy;

                    if (NPC.velocity.Y < targettingPosition)
                        NPC.velocity.Y = NPC.velocity.Y + turnSpeedCopy;
                    else if (NPC.velocity.Y > targettingPosition)
                        NPC.velocity.Y = NPC.velocity.Y - turnSpeedCopy;
                }
                if ((NPC.velocity.X > 0f && playerX > 0f) || (NPC.velocity.X < 0f && playerX < 0f) || (NPC.velocity.Y > 0f && targettingPosition > 0f) || (NPC.velocity.Y < 0f && targettingPosition < 0f))
                {
                    if (NPC.velocity.X < playerX)
                        NPC.velocity.X = NPC.velocity.X + speedCopy;
                    else if (NPC.velocity.X > playerX)
                        NPC.velocity.X = NPC.velocity.X - speedCopy;

                    if (NPC.velocity.Y < targettingPosition)
                        NPC.velocity.Y = NPC.velocity.Y + speedCopy;
                    else if (NPC.velocity.Y > targettingPosition)
                        NPC.velocity.Y = NPC.velocity.Y - speedCopy;

                    if ((double)Math.Abs(targettingPosition) < (double)maxChargeSpeed * 0.2 && ((NPC.velocity.X > 0f && playerX < 0f) || (NPC.velocity.X < 0f && playerX > 0f)))
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y = NPC.velocity.Y + speedCopy * 2f;
                        else
                            NPC.velocity.Y = NPC.velocity.Y - speedCopy * 2f;
                    }

                    if ((double)Math.Abs(playerX) < (double)maxChargeSpeed * 0.2 && ((NPC.velocity.Y > 0f && targettingPosition < 0f) || (NPC.velocity.Y < 0f && targettingPosition > 0f)))
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X = NPC.velocity.X + speedCopy * 2f;
                        else
                            NPC.velocity.X = NPC.velocity.X - speedCopy * 2f;
                    }
                }
                else if (absoluteTargetX > absoluteTargetPos)
                {
                    if (NPC.velocity.X < playerX)
                        NPC.velocity.X = NPC.velocity.X + speedCopy * 1.1f;
                    else if (NPC.velocity.X > playerX)
                        NPC.velocity.X = NPC.velocity.X - speedCopy * 1.1f;

                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChargeSpeed * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y = NPC.velocity.Y + speedCopy;
                        else
                            NPC.velocity.Y = NPC.velocity.Y - speedCopy;
                    }
                }
                else
                {
                    if (NPC.velocity.Y < targettingPosition)
                        NPC.velocity.Y = NPC.velocity.Y + speedCopy * 1.1f;
                    else if (NPC.velocity.Y > targettingPosition)
                        NPC.velocity.Y = NPC.velocity.Y - speedCopy * 1.1f;

                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChargeSpeed * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X = NPC.velocity.X + speedCopy;
                        else
                            NPC.velocity.X = NPC.velocity.X - speedCopy;
                    }
                }
            }

            if (NPC.Distance(player.Center) > 1120f)
                NPC.velocity += (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * turnSpeed;

            NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + MathHelper.PiOver2;

            if (shouldFly)
            {
                if (NPC.localAI[0] != 1f)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 1f;
            }
            else
            {
                if (NPC.localAI[0] != 0f)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 0f;
            }

            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                NPC.netUpdate = true;

            if (NPC.alpha > 0 && NPC.life > 0)
            {
                for (int dustIndex = 0; dustIndex < 2; dustIndex++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].noLight = true;
                }
            }

            if ((NPC.position - NPC.oldPosition).Length() > 2f)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.Opacity = 1f;
                return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, NPC, drawColor, TextureAssets.Npc[Type].Value, TextureAssets.Npc[ModContent.NPCType<PerforatorBodyLarge>()].Value, PerforatorBodyLarge.AltTexture.Value, 3, 40, 0.3f, Vector2.Zero, 3, 10);
            }

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[Type].Value.Width / 2), (float)(TextureAssets.Npc[Type].Value.Height / 2));

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = GlowTexture.Value;
            Color glowmaskColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, glowmaskColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("LargePerf").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("LargePerf2").Type, NPC.scale);
                }
            }
        }

        public override LocalizedText DeathMessage => CalamityUtils.GetText("NPCs.PerforatorLarge");
        public override void BossLoot(ref int potionType) => potionType = ItemID.HealingPotion;

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<PerforatorHeadLarge>(),
                ModContent.NPCType<PerforatorBodyLarge>(),
                ModContent.NPCType<PerforatorTailLarge>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void OnKill()
        {
            int heartAmt = Main.rand.Next(3) + 3;
            for (int i = 0; i < heartAmt; i++)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BurningBlood>(), 300);
        }
    }
}
