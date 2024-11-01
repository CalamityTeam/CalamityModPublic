using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Perforator
{
    [AutoloadBossHead]
    [LongDistanceNetSync]
    public class PerforatorHeadSmall : ModNPC
    {
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/PerfSmallHit", 3);
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/PerfSmallDeath");

        public static Asset<Texture2D> GlowTexture;

        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private const int MsgType = 23;
        private bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.8f,
                PortraitScale = 0.8f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/PerforatorSmall_Bestiary",
                PortraitPositionXOverride = 40,
                PortraitPositionYOverride = 60
            };
            value.Position.X += 60;
            value.Position.Y += 50;
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
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 42;
            NPC.height = 62;

            NPC.LifeMaxNERB(1200, 1440, 50000);
            if (Main.zenithWorld)
                NPC.lifeMax *= 4;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
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

            if (BossRushEvent.BossRushActive)
                NPC.scale *= 1.25f;
            else if (CalamityWorld.death)
                NPC.scale *= 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.15f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;

            // Scale stats in Expert and Master
            CalamityGlobalNPC.AdjustExpertModeStatScaling(NPC);
            CalamityGlobalNPC.AdjustMasterModeStatScaling(NPC);
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
            writer.Write(biomeEnrageTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
        }

        public override void AI()
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Enrage
            if ((!Main.player[NPC.target].ZoneCrimson || (NPC.position.Y / 16f) < Main.worldSurface) && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || bossRush;

            float enrageScale = bossRush ? 1f : 0f;
            if (biomeEnraged && (!Main.player[NPC.target].ZoneCrimson || bossRush))
                enrageScale += 1f;
            if (biomeEnraged && ((NPC.position.Y / 16f) < Main.worldSurface || bossRush))
                enrageScale += 1f;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            float speed = 0.15f;
            float turnSpeed = 0.1f;

            if (expertMode)
            {
                float velocityScale = (death ? 0.15f : 0.1f) * enrageScale;
                speed += velocityScale * (1f - lifeRatio);
                float accelerationScale = (death ? 0.12f : 0.1f) * enrageScale;
                turnSpeed += accelerationScale * (1f - lifeRatio);
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            NPC.alpha -= 42;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned)
                {
                    int Previous = NPC.whoAmI;
                    int maxLength = death ? 9 : revenge ? 8 : expertMode ? 7 : 5;
                    for (int segments = 0; segments < maxLength; segments++)
                    {
                        int lol;
                        if (segments >= 0 && segments < maxLength - 1)
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorBodySmall>(), NPC.whoAmI);
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorTailSmall>(), NPC.whoAmI);

                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        NetMessage.SendData(MsgType, -1, -1, null, lol, 0f, 0f, 0f, 0);
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
                int stopFlyingRadius = death ? 320 : revenge ? 400 : expertMode ? 480 : 600;
                bool outsideFlyRadius = true;
                if (NPC.position.Y > player.position.Y)
                {
                    foreach (Player plr in Main.ActivePlayers)
                    {
                        Rectangle rectangle2 = new Rectangle((int)plr.position.X - stopFlyingRadius, (int)plr.position.Y - stopFlyingRadius, stopFlyingRadius * 2, stopFlyingRadius * 2);
                        if (rectangle.Intersects(rectangle2))
                        {
                            outsideFlyRadius = false;
                            break;
                        }
                    }
                    if (outsideFlyRadius)
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
                        if (Main.npc[a].type == ModContent.NPCType<PerforatorHeadSmall>() || Main.npc[a].type == ModContent.NPCType<PerforatorBodySmall>() || Main.npc[a].type == ModContent.NPCType<PerforatorTailSmall>())
                            Main.npc[a].active = false;
                    }
                }
            }

            if (Main.zenithWorld && CalamityGlobalNPC.perfHive >= 0)
                NPC.ai[0]++;

            // GFB movement
            if (NPC.ai[0] >= 300f && CalamityGlobalNPC.perfHive >= 0)
            {
                shouldFly = false;

                NPC Hive = Main.npc[CalamityGlobalNPC.perfHive];
                Vector2 targetDistance = Hive.Center - NPC.Center;
                float velocity = 16f;
                float acceleration = 0.15f;

                // Move towards the hive really quickly
                if (targetDistance.Length() > 32f)
                {
                    CalamityUtils.SmoothMovement(NPC, 0f, targetDistance, velocity, acceleration, true);
                }
                else
                {
                    // Go and succ it
                    NPC.Center = Hive.Center + (Vector2.UnitY * 4f).RotatedBy(NPC.rotation);
                    Hive.localAI[1] += 1f;
                }

                if (NPC.ai[0] >= 600f)
                {
                    NPC.ai[0] = 0f;
                    Hive.localAI[1] = 0f;
                }
            }
            // Normal movement
            else
            {
                float speedCopy = speed;
                float turnSpeedCopy = turnSpeed;
                Vector2 npcCenter = NPC.Center;
                float targetX = player.Center.X;
                float targetY = player.Center.Y;
                targetX = (float)((int)(targetX / 16f) * 16);
                targetY = (float)((int)(targetY / 16f) * 16);
                npcCenter.X = (float)((int)(npcCenter.X / 16f) * 16);
                npcCenter.Y = (float)((int)(npcCenter.Y / 16f) * 16);
                targetX -= npcCenter.X;
                targetY -= npcCenter.Y;
                float targetDistance = (float)Math.Sqrt((double)(targetX * targetX + targetY * targetY));
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
                            if (NPC.velocity.X < targetX)
                                NPC.velocity.X += speedCopy;
                            else if (NPC.velocity.X > targetX)
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

                    targetDistance = (float)Math.Sqrt((double)(targetX * targetX + targetY * targetY));
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

                        if ((double)Math.Abs(targetY) < (double)maxChargeSpeed * 0.2 && ((NPC.velocity.X > 0f && targetX < 0f) || (NPC.velocity.X < 0f && targetX > 0f)))
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y += speedCopy * 2f;
                            else
                                NPC.velocity.Y -= speedCopy * 2f;
                        }

                        if ((double)Math.Abs(targetX) < (double)maxChargeSpeed * 0.2 && ((NPC.velocity.Y > 0f && targetY < 0f) || (NPC.velocity.Y < 0f && targetY > 0f)))
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

                        if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChargeSpeed * 0.5)
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

                        if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChargeSpeed * 0.5)
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X += speedCopy;
                            else
                                NPC.velocity.X -= speedCopy;
                        }
                    }
                }
            }

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = maxChargeSpeed * 0.25f;
            float minimalDamageVelocity = maxChargeSpeed * 0.5f;
            if (NPC.velocity.Length() <= minimalContactDamageVelocity)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * 0.5);
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((NPC.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp((float)Math.Round(NPC.defDamage * 0.5), NPC.defDamage, velocityDamageScalar);
            }

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
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / 2));

            Vector2 glowmaskDrawLocation = NPC.Center - screenPos;
            glowmaskDrawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * NPC.scale / 2f;
            glowmaskDrawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, glowmaskDrawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = GlowTexture.Value;
            Color glowmaskColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            spriteBatch.Draw(texture2D15, glowmaskDrawLocation, NPC.frame, glowmaskColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

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
                for (int k = 0; k < 5; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SmallPerf").Type, NPC.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = CalamityUtils.GetTextValue("NPCs.PerforatorSmall");
            potionType = ItemID.HealingPotion;
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<PerforatorHeadSmall>(),
                ModContent.NPCType<PerforatorBodySmall>(),
                ModContent.NPCType<PerforatorTailSmall>());
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
                target.AddBuff(ModContent.BuffType<BurningBlood>(), 180, true);
        }
    }
}
