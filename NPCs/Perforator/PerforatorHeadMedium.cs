using System;
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
    public class PerforatorHeadMedium : ModNPC
    {
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/PerfMediumHit", 3);
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/PerfMediumDeath");

        public static Asset<Texture2D> GlowTexture;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.7f,
                PortraitScale = 0.7f,
                PortraitPositionXOverride = 40,
                PortraitPositionYOverride = 40
            };
            value.Position.X += 60;
            value.Position.Y += 40;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 24; // 48
            NPC.npcSlots = 5f;
            NPC.width = 58;
            NPC.height = 68;
            NPC.defense = 2;

            NPC.LifeMaxNERB(120, 150, 7000);
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

            NPC.Calamity().SplittingWorm = true;

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

        public override void AI()
        {
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Total body segments
            float totalSegments = GetMediumPerforatorSegmentsCount();

            // Count body segments remaining
            float segmentCount = NPC.CountNPCS(ModContent.NPCType<PerforatorBodyMedium>());

            // Percent body segments remaining
            float lifeRatio = MathHelper.Clamp(segmentCount / totalSegments, 0f, 1f);

            float speed = 0.125f;
            float turnSpeed = 0.085f;

            if (expertMode)
            {
                float velocityScale = death ? 0.125f : 0.085f;
                speed += velocityScale * (1f - lifeRatio);
                float accelerationScale = death ? 0.085f : 0.06f;
                turnSpeed += accelerationScale * (1f - lifeRatio);
            }

            NPC.realLife = -1;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f)
                {
                    NPC.ai[2] = totalSegments;
                    NPC.ai[0] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (NPC.width / 2)), (int)(NPC.position.Y + NPC.height), ModContent.NPCType<PerforatorBodyMedium>(), NPC.whoAmI);
                    Main.npc[(int)NPC.ai[0]].ai[1] = NPC.whoAmI;
                    Main.npc[(int)NPC.ai[0]].ai[2] = NPC.ai[2] - 1f;
                    NPC.netUpdate = true;
                }

                // Splitting effect
                bool spawnedBlob = false;
                if (!Main.npc[(int)NPC.ai[1]].active && !Main.npc[(int)NPC.ai[0]].active)
                {
                    if (death)
                    {
                        spawnedBlob = true;
                        int type = ModContent.ProjectileType<IchorBlob>();
                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, NPC.velocity + Main.rand.NextVector2CircularEdge(3f, 3f), type, PerforatorHive.IchorBlobDamage, 0f, Main.myPlayer, 0f, NPC.Center.Y);
                    }

                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
                if (!Main.npc[(int)NPC.ai[0]].active)
                {
                    if (death && !spawnedBlob)
                    {
                        int type = ModContent.ProjectileType<IchorBlob>();
                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, NPC.velocity + Main.rand.NextVector2CircularEdge(3f, 3f), type, PerforatorHive.IchorBlobDamage, 0f, Main.myPlayer, 0f, NPC.Center.Y);
                    }

                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }

                if (!NPC.active && Main.dedServ)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
            }

            // Movement
            int tilePositionX = (int)(NPC.position.X / 16f) - 1;
            int tileWidthPosX = (int)((NPC.position.X + NPC.width) / 16f) + 2;
            int tilePositionY = (int)(NPC.position.Y / 16f) - 1;
            int tileWidthPosY = (int)((NPC.position.Y + NPC.height) / 16f) + 2;
            if (tilePositionX < 0)
                tilePositionX = 0;
            if (tileWidthPosX > Main.maxTilesX)
                tileWidthPosX = Main.maxTilesX;
            if (tilePositionY < 0)
                tilePositionY = 0;
            if (tileWidthPosY > Main.maxTilesY)
                tileWidthPosY = Main.maxTilesY;

            // Fly or not
            bool shouldFly = false;
            if (!shouldFly)
            {
                for (int i = tilePositionX; i < tileWidthPosX; i++)
                {
                    for (int j = tilePositionY; j < tileWidthPosY; j++)
                    {
                        if (Main.tile[i, j] != null && ((Main.tile[i, j].HasUnactuatedTile && (Main.tileSolid[Main.tile[i, j].TileType] || (Main.tileSolidTop[Main.tile[i, j].TileType] && Main.tile[i, j].TileFrameY == 0))) || Main.tile[i, j].LiquidAmount > 64))
                        {
                            Vector2 vector;
                            vector.X = i * 16;
                            vector.Y = j * 16;
                            if (NPC.position.X + NPC.width > vector.X && NPC.position.X < vector.X + 16f && NPC.position.Y + NPC.height > vector.Y && NPC.position.Y < vector.Y + 16f)
                            {
                                shouldFly = true;
                                if (Main.rand.NextBool(100) && Main.tile[i, j].HasUnactuatedTile)
                                    WorldGen.KillTile(i, j, true, true, false);
                            }
                        }
                    }
                }
            }

            if (!shouldFly)
            {
                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int stopFlyingRadius = death ? 320 : revenge ? 400 : expertMode ? 480 : 600;
                bool outsideFlyRadius = true;
                for (int k = 0; k < Main.maxPlayers; k++)
                {
                    if (Main.player[k].active)
                    {
                        Rectangle rectangle2 = new Rectangle((int)Main.player[k].position.X - stopFlyingRadius, (int)Main.player[k].position.Y - stopFlyingRadius, stopFlyingRadius * 2, stopFlyingRadius * 2);
                        if (rectangle.Intersects(rectangle2))
                        {
                            outsideFlyRadius = false;
                            break;
                        }
                    }
                }

                if (outsideFlyRadius)
                    shouldFly = true;
            }

            float maxChargeSpeed = 16f;
            if (player.dead || CalamityGlobalNPC.perfHive < 0 || !Main.npc[CalamityGlobalNPC.perfHive].active)
            {
                NPC.TargetClosest(false);
                shouldFly = false;
                NPC.velocity.Y += 1f;
                if (NPC.position.Y > Main.worldSurface * 16D)
                {
                    NPC.velocity.Y += 1f;
                    maxChargeSpeed *= 2f;
                }

                if (NPC.position.Y > Main.rockLayer * 16D)
                {
                    for (int p = 0; p < Main.maxNPCs; p++)
                    {
                        if (Main.npc[p].type == NPC.type || Main.npc[p].type == ModContent.NPCType<PerforatorBodyMedium>() || Main.npc[p].type == ModContent.NPCType<PerforatorTailMedium>())
                            Main.npc[p].active = false;
                    }
                }
            }

            // Velocity and acceleration
            float speedCopy = speed;
            float turnSpeedCopy = turnSpeed;

            Vector2 vector2 = NPC.Center;
            float targetX = player.Center.X;
            float targetY = player.Center.Y;

            targetX = (int)(targetX / 16f) * 16;
            targetY = (int)(targetY / 16f) * 16;
            vector2.X = (int)(vector2.X / 16f) * 16;
            vector2.Y = (int)(vector2.Y / 16f) * 16;
            targetX -= vector2.X;
            targetY -= vector2.Y;
            float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);

            // Prevent new heads from being slowed when they spawn
            if (NPC.Calamity().newAI[1] < 3f)
            {
                NPC.Calamity().newAI[1] += 1f;

                // Set velocity for when a new head spawns
                NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * (speedCopy * (death ? 0.3f : 0.2f));
            }

            if (!shouldFly)
            {
                NPC.velocity.Y += 0.15f;
                if (NPC.velocity.Y > maxChargeSpeed)
                    NPC.velocity.Y = maxChargeSpeed;

                // This bool exists to stop the strange wiggle behavior when worms are falling down
                bool slowXVelocity = Math.Abs(NPC.velocity.X) > turnSpeedCopy;
                if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < maxChargeSpeed * 0.4)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X -= turnSpeedCopy * 1.1f;
                    else
                        NPC.velocity.X += turnSpeedCopy * 1.1f;
                }
                else if (NPC.velocity.Y == maxChargeSpeed)
                {
                    if (slowXVelocity)
                    {
                        if (NPC.velocity.X < targetX)
                            NPC.velocity.X += turnSpeedCopy;
                        else if (NPC.velocity.X > targetX)
                            NPC.velocity.X -= turnSpeedCopy;
                    }
                    else
                        NPC.velocity.X = 0f;
                }
                else if (NPC.velocity.Y > 4f)
                {
                    if (slowXVelocity)
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.velocity.X += turnSpeedCopy * 0.9f;
                        else
                            NPC.velocity.X -= turnSpeedCopy * 0.9f;
                    }
                    else
                        NPC.velocity.X = 0f;
                }
            }
            else
            {
                // Sound
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

                targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
                float absoluteTargetX = Math.Abs(targetX);
                float absoluteTargetY = Math.Abs(targetY);
                float timeToReachTarget = maxChargeSpeed / targetDistance;
                targetX *= timeToReachTarget;
                targetY *= timeToReachTarget;

                if (((NPC.velocity.X > 0f && targetX > 0f) || (NPC.velocity.X < 0f && targetX < 0f)) && ((NPC.velocity.Y > 0f && targetY > 0f) || (NPC.velocity.Y < 0f && targetY < 0f)))
                {
                    if (NPC.velocity.X < targetX)
                        NPC.velocity.X += turnSpeedCopy;
                    else if (NPC.velocity.X > targetX)
                        NPC.velocity.X -= turnSpeedCopy;

                    if (NPC.velocity.Y < targetY)
                        NPC.velocity.Y += turnSpeedCopy;
                    else if (NPC.velocity.Y > targetY)
                        NPC.velocity.Y -= turnSpeedCopy;
                }

                if ((NPC.velocity.X > 0f && targetX > 0f) || (NPC.velocity.X < 0f && targetX < 0f) || (NPC.velocity.Y > 0f && targetY > 0f) || (NPC.velocity.Y < 0f && targetY < 0f))
                {
                    if (NPC.velocity.X < targetX)
                        NPC.velocity.X += speedCopy;
                    else if (NPC.velocity.X > targetX)
                        NPC.velocity.X -= speedCopy;

                    if (NPC.velocity.Y < targetY)
                        NPC.velocity.Y += speedCopy;
                    else if (NPC.velocity.Y > targetY)
                        NPC.velocity.Y -= speedCopy;

                    if (Math.Abs(targetY) < maxChargeSpeed * 0.2 && ((NPC.velocity.X > 0f && targetX < 0f) || (NPC.velocity.X < 0f && targetX > 0f)))
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y += speedCopy * 2f;
                        else
                            NPC.velocity.Y -= speedCopy * 2f;
                    }

                    if (Math.Abs(targetX) < maxChargeSpeed * 0.2 && ((NPC.velocity.Y > 0f && targetY < 0f) || (NPC.velocity.Y < 0f && targetY > 0f)))
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X += speedCopy * 2f;
                        else
                            NPC.velocity.X -= speedCopy * 2f;
                    }
                }
                else if (absoluteTargetX > absoluteTargetY)
                {
                    if (NPC.velocity.X < targetX)
                        NPC.velocity.X += speedCopy * 1.1f;
                    else if (NPC.velocity.X > targetX)
                        NPC.velocity.X -= speedCopy * 1.1f;

                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < maxChargeSpeed * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y += speedCopy;
                        else
                            NPC.velocity.Y -= speedCopy;
                    }
                }
                else
                {
                    if (NPC.velocity.Y < targetY)
                        NPC.velocity.Y += speedCopy * 1.1f;
                    else if (NPC.velocity.Y > targetY)
                        NPC.velocity.Y -= speedCopy * 1.1f;

                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < maxChargeSpeed * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X += speedCopy;
                        else
                            NPC.velocity.X -= speedCopy;
                    }
                }
            }

            if (NPC.Distance(player.Center) > 1120f)
                NPC.velocity += (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * turnSpeed;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

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
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, Main.rand.NextBool() ? DustID.Ichor : DustID.Blood, 0f, 0f, 100, default, 2f);
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

        public static int GetMediumPerforatorSegmentsCount()
        {
            return Main.getGoodWorld ? 20 : (CalamityWorld.death || BossRushEvent.BossRushActive) ? 14 : Main.expertMode ? 12 : 10;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.Opacity = 1f;
                return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, NPC, drawColor, TextureAssets.Npc[Type].Value, TextureAssets.Npc[ModContent.NPCType<PerforatorBodyMedium>()].Value, 5, 34, 0.3f, Vector2.Zero, 5, 6);
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

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("MediumPerf").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("MediumPerf2").Type, NPC.scale);
                }
            }
        }

        public override void OnKill()
        {
            int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
            if (Main.rand.NextBool(4) && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);

            if (Main.netMode != NetmodeID.MultiplayerClient && Main.zenithWorld)
            {
                int type = ModContent.ProjectileType<IchorBlob>();
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.UnitY, type, PerforatorHive.IchorBlobDamage, 0f, Main.myPlayer);

                for (int i = -1; i < 2; i++) //releases 3 Ichor Shots
                {
                    int type2 = ModContent.ProjectileType<IchorShot>();
                    Vector2 baseVelocity = Vector2.UnitY * Main.rand.NextFloat(-12.5f, -5f);
                    int spread = Main.rand.Next(16, 36);
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, baseVelocity.RotatedBy(MathHelper.ToRadians(spread * i)), type2, PerforatorHive.IchorShotDamage, 0f, Main.myPlayer);
                }
            }
        }

        public override LocalizedText DeathMessage => CalamityUtils.GetText("NPCs.PerforatorMedium");
        public override void BossLoot(ref int potionType) => potionType = ItemID.HealingPotion;

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                NPC.type,
                ModContent.NPCType<PerforatorBodyMedium>(),
                ModContent.NPCType<PerforatorTailMedium>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(BuffID.Ichor, 360);
            }
        }
    }
}
