using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.Ravager
{
    [AutoloadBossHead]
    public class RavagerBody : ModNPC
    {
        private float velocityY = -16f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager");
            Main.npcFrameCount[NPC.type] = 7;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.npcSlots = 20f;
            NPC.aiStyle = -1;
            NPC.GetNPCDamage();
            NPC.width = 332;
            NPC.height = 214;
            NPC.defense = 55;
            NPC.value = Item.buyPrice(0, 75, 0, 0);
            NPC.DR_NERD(0.35f);
            NPC.LifeMaxNERB(44600, 53500, 460000);
            if (DownedBossSystem.downedProvidence && !BossRushEvent.BossRushActive)
            {
                NPC.damage = (int)(NPC.damage * 1.5);
                NPC.defense *= 2;
                NPC.lifeMax *= 4;
                NPC.value *= 1.5f;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            AIType = -1;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.alpha = 255;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath14;
            Music = CalamityMod.Instance.GetMusicFromMusicMod("Ravager") ?? MusicID.Boss4;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            writer.Write(velocityY);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            velocityY = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && malice;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Large fire light
            Lighting.AddLight((int)(NPC.Center.X - 110f) / 16, (int)(NPC.Center.Y - 30f) / 16, 0f, 0.5f, 2f);
            Lighting.AddLight((int)(NPC.Center.X + 110f) / 16, (int)(NPC.Center.Y - 30f) / 16, 0f, 0.5f, 2f);

            // Small fire light
            Lighting.AddLight((int)(NPC.Center.X - 40f) / 16, (int)(NPC.Center.Y - 60f) / 16, 0f, 0.25f, 1f);
            Lighting.AddLight((int)(NPC.Center.X + 40f) / 16, (int)(NPC.Center.Y - 60f) / 16, 0f, 0.25f, 1f);

            CalamityGlobalNPC.scavenger = NPC.whoAmI;

            if (NPC.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] = 1f;
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X - 70, (int)NPC.Center.Y + 88, ModContent.NPCType<RavagerLegLeft>());
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X + 70, (int)NPC.Center.Y + 88, ModContent.NPCType<RavagerLegRight>());
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X - 120, (int)NPC.Center.Y + 50, ModContent.NPCType<RavagerClawLeft>());
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X + 120, (int)NPC.Center.Y + 50, ModContent.NPCType<RavagerClawRight>());
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X + 1, (int)NPC.Center.Y - 20, ModContent.NPCType<RavagerHead>());
            }

            if (NPC.target >= 0 && Main.player[NPC.target].dead)
            {
                NPC.TargetClosest();
                if (Main.player[NPC.target].dead)
                    NPC.noTileCollide = true;
            }

            Player player = Main.player[NPC.target];

            if (NPC.alpha > 0)
            {
                NPC.alpha -= 10;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;

                NPC.ai[1] = 0f;
            }

            bool leftLegActive = false;
            bool rightLegActive = false;
            bool headActive = false;
            bool rightClawActive = false;
            bool leftClawActive = false;
            bool freeHeadActive = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RavagerHead>())
                    headActive = true;
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RavagerClawRight>())
                    rightClawActive = true;
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RavagerClawLeft>())
                    leftClawActive = true;
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RavagerLegRight>())
                    rightLegActive = true;
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RavagerLegLeft>())
                    leftLegActive = true;
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RavagerHead2>())
                    freeHeadActive = true;
            }

            bool anyHeadActive = headActive || freeHeadActive;
            bool immunePhase = headActive || rightClawActive || leftClawActive || rightLegActive || leftLegActive;
            bool finalPhase = !leftClawActive && !rightClawActive && !headActive && !leftLegActive && !rightLegActive && expertMode;
            bool phase2 = NPC.ai[0] == 2f;
            bool reduceFallSpeed = NPC.velocity.Y > 0f && NPC.Bottom.Y > player.Top.Y;

            if (immunePhase)
            {
                NPC.dontTakeDamage = true;
                if (malice)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && revenge)
                            Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<WeakPetrification>(), 2);
                    }
                }
            }
            else
            {
                NPC.dontTakeDamage = false;
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && revenge)
                        Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<WeakPetrification>(), 2);
                }
            }

            if (!headActive)
            {
                int rightDust = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y - 30f), 8, 8, DustID.Blood, 0f, 0f, 100, default, 2.5f);
                Main.dust[rightDust].alpha += Main.rand.Next(100);
                Main.dust[rightDust].velocity *= 0.2f;

                Dust rightDustExpr = Main.dust[rightDust];
                rightDustExpr.velocity.Y -= 3f + Main.rand.Next(10) * 0.1f;
                Main.dust[rightDust].fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    rightDust = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y - 30f), 8, 8, DustID.Torch, 0f, 0f, 0, default, 1.5f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[rightDust].noGravity = true;
                        Main.dust[rightDust].scale *= 1f + Main.rand.Next(10) * 0.1f;
                        Dust rightDustExpr2 = Main.dust[rightDust];
                        rightDustExpr2.velocity.Y -= 4f;
                    }
                }
            }

            if (!rightClawActive)
            {
                int rightDust = Dust.NewDust(new Vector2(NPC.Center.X + 80f, NPC.Center.Y + 45f), 8, 8, DustID.Blood, 0f, 0f, 100, default, 3f);
                Main.dust[rightDust].alpha += Main.rand.Next(100);
                Main.dust[rightDust].velocity *= 0.2f;

                Dust rightDustExpr = Main.dust[rightDust];
                rightDustExpr.velocity.X += 3f + Main.rand.Next(10) * 0.1f;
                Main.dust[rightDust].fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    rightDust = Dust.NewDust(new Vector2(NPC.Center.X + 80f, NPC.Center.Y + 45f), 8, 8, DustID.Torch, 0f, 0f, 0, default, 2f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[rightDust].noGravity = true;
                        Main.dust[rightDust].scale *= 1f + Main.rand.Next(10) * 0.1f;
                        Dust rightDustExpr2 = Main.dust[rightDust];
                        rightDustExpr2.velocity.X += 4f;
                    }
                }
            }

            if (!leftClawActive)
            {
                int leftDust = Dust.NewDust(new Vector2(NPC.Center.X - 80f, NPC.Center.Y + 45f), 8, 8, DustID.Blood, 0f, 0f, 100, default, 3f);
                Dust leftDustExpr = Main.dust[leftDust];
                leftDustExpr.alpha += Main.rand.Next(100);
                leftDustExpr.velocity *= 0.2f;
                leftDustExpr.velocity.X -= 3f + Main.rand.Next(10) * 0.1f;
                leftDustExpr.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    leftDust = Dust.NewDust(new Vector2(NPC.Center.X - 80f, NPC.Center.Y + 45f), 8, 8, DustID.Torch, 0f, 0f, 0, default, 2f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Dust leftDustExpr2 = Main.dust[leftDust];
                        leftDustExpr2.noGravity = true;
                        leftDustExpr2.scale *= 1f + Main.rand.Next(10) * 0.1f;
                        leftDustExpr2.velocity.X -= 4f;
                    }
                }
            }

            if (!rightLegActive)
            {
                int rightDust = Dust.NewDust(new Vector2(NPC.Center.X + 60f, NPC.Center.Y + 60f), 8, 8, DustID.Blood, 0f, 0f, 100, default, 2f);
                Dust rightDustExpr = Main.dust[rightDust];
                rightDustExpr.alpha += Main.rand.Next(100);
                rightDustExpr.velocity *= 0.2f;
                rightDustExpr.velocity.Y += 0.5f + Main.rand.Next(10) * 0.1f;
                rightDustExpr.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    rightDust = Dust.NewDust(new Vector2(NPC.Center.X + 60f, NPC.Center.Y + 60f), 8, 8, DustID.Torch, 0f, 0f, 0, default, 1.5f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Dust rightDustExpr2 = Main.dust[rightDust];
                        rightDustExpr2.noGravity = true;
                        rightDustExpr2.scale *= 1f + Main.rand.Next(10) * 0.1f;
                        rightDustExpr2.velocity.Y += 1f;
                    }
                }
            }

            if (!leftLegActive)
            {
                int leftDust = Dust.NewDust(new Vector2(NPC.Center.X - 60f, NPC.Center.Y + 60f), 8, 8, DustID.Blood, 0f, 0f, 100, default, 2f);
                Main.dust[leftDust].alpha += Main.rand.Next(100);
                Main.dust[leftDust].velocity *= 0.2f;

                Dust leftDustExpr = Main.dust[leftDust];
                leftDustExpr.velocity.Y += 0.5f + Main.rand.Next(10) * 0.1f;
                Main.dust[leftDust].fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    leftDust = Dust.NewDust(new Vector2(NPC.Center.X - 60f, NPC.Center.Y + 60f), 8, 8, DustID.Torch, 0f, 0f, 0, default, 1.5f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[leftDust].noGravity = true;
                        Main.dust[leftDust].scale *= 1f + Main.rand.Next(10) * 0.1f;
                        Dust leftDustExpr2 = Main.dust[leftDust];
                        leftDustExpr2.velocity.Y += 1f;
                    }
                }
            }

            if (NPC.noTileCollide && !player.dead)
            {
                if (NPC.velocity.Y > 0f && NPC.Bottom.Y > player.Top.Y)
                    NPC.noTileCollide = false;
                else if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    NPC.noTileCollide = false;
            }

            if (NPC.ai[0] == 0f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X *= 0.8f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 0f)
                    {
                        if (revenge)
                        {
                            if (calamityGlobalNPC.newAI[0] % 3f == 0f)
                                NPC.ai[1] += 1f;
                            else if (calamityGlobalNPC.newAI[0] % 2f == 0f)
                                NPC.ai[1] += 1f;
                        }

                        if (!rightClawActive && !leftClawActive)
                            NPC.ai[1] += 1f;
                        if (!headActive)
                            NPC.ai[1] += 1f;
                        if (!rightLegActive && !leftLegActive)
                            NPC.ai[1] += 1f;
                    }

                    float jumpGateValue = 180f;
                    if (NPC.ai[1] >= jumpGateValue)
                    {
                        NPC.ai[1] = -20f;
                    }
                    else if (NPC.ai[1] == -1f)
                    {
                        NPC.noTileCollide = true;

                        NPC.TargetClosest();

                        bool shouldFall = player.position.Y >= NPC.Bottom.Y;
                        float velocityXBoost = !anyHeadActive ? 6f : death ? 6f * (1f - lifeRatio) : 4f * (1f - lifeRatio);
                        float velocityX = 4f + velocityXBoost;
                        velocityY = -16f;

                        float distanceBelowTarget = NPC.position.Y - (player.position.Y + 80f);

                        if (revenge)
                        {
                            float multiplier = malice ? 0.003f : 0.0015f;
                            if (distanceBelowTarget > 0f)
                                calamityGlobalNPC.newAI[1] += 1f + distanceBelowTarget * multiplier;

                            float speedMultLimit = malice ? 3f : 2f;
                            if (calamityGlobalNPC.newAI[1] > speedMultLimit)
                                calamityGlobalNPC.newAI[1] = speedMultLimit;

                            if (calamityGlobalNPC.newAI[1] > 1f)
                                velocityY *= calamityGlobalNPC.newAI[1];
                        }

                        if (expertMode && !finalPhase)
                        {
                            if (shouldFall)
                                velocityY = 1f;

                            if (calamityGlobalNPC.newAI[0] % 3f == 0f)
                            {
                                velocityX *= 2f;
                                if (!shouldFall)
                                    velocityY *= 0.5f;
                            }
                            else if (calamityGlobalNPC.newAI[0] % 2f == 0f)
                            {
                                velocityX *= 1.5f;
                                if (!shouldFall)
                                    velocityY *= 0.75f;
                            }
                        }

                        if (finalPhase)
                            calamityGlobalNPC.newAI[2] = player.direction;

                        float playerLocation = NPC.Center.X - player.Center.X;
                        NPC.direction = playerLocation < 0 ? 1 : -1;

                        NPC.velocity.X = velocityX * NPC.direction;
                        NPC.velocity.Y = velocityY;

                        NPC.ai[0] = finalPhase ? 2f : 1f;
                        NPC.ai[1] = 0f;
                    }
                }

                CustomGravity();
            }
            else if (NPC.ai[0] >= 1f)
            {
                if (NPC.velocity.Y == 0f && (NPC.ai[1] == 31f || NPC.ai[0] == 1f))
                {
                    SoundEngine.PlaySound(SoundID.Item, (int)NPC.position.X, (int)NPC.position.Y, 14, 1.25f, -0.25f);

                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (expertMode)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].type == ModContent.NPCType<RockPillar>() && Main.npc[i].ai[0] == 0f)
                                {
                                    Main.npc[i].ai[1] = -1f;
                                    Main.npc[i].direction = NPC.direction;
                                    Main.npc[i].netUpdate = true;
                                }
                            }

                            int spawnDistance = 360;

                            if (!NPC.AnyNPCs(ModContent.NPCType<RockPillar>()))
                            {
                                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(player.Center.X - spawnDistance * 1.25f), (int)player.Center.Y - 100, ModContent.NPCType<RockPillar>());
                                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(player.Center.X + spawnDistance * 1.25f), (int)player.Center.Y - 100, ModContent.NPCType<RockPillar>());
                            }
                            else if (!NPC.AnyNPCs(ModContent.NPCType<FlamePillar>()))
                            {
                                float distanceMultiplier = finalPhase ? 2.5f : 2f;
                                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)player.Center.X - (int)(spawnDistance * distanceMultiplier), (int)player.Center.Y - 100, ModContent.NPCType<FlamePillar>());
                                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)player.Center.X + (int)(spawnDistance * distanceMultiplier), (int)player.Center.Y - 100, ModContent.NPCType<FlamePillar>());
                            }
                        }
                    }

                    if (revenge)
                        calamityGlobalNPC.newAI[0] += 1f;

                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;
                    NPC.TargetClosest();

                    for (int stompDustArea = (int)NPC.position.X - 30; stompDustArea < (int)NPC.position.X + NPC.width + 60; stompDustArea += 30)
                    {
                        for (int stompDustAmount = 0; stompDustAmount < 6; stompDustAmount++)
                        {
                            int stompDust = Dust.NewDust(new Vector2(NPC.position.X - 30f, NPC.position.Y + NPC.height), NPC.width + 30, 4, 31, 0f, 0f, 100, default, 1.5f);
                            Main.dust[stompDust].velocity *= 0.2f;
                        }

                        int stompGore = Gore.NewGore(new Vector2(stompDustArea - 30, NPC.position.Y + NPC.height - 12f), default, Main.rand.Next(61, 64), 1f);
                        Main.gore[stompGore].velocity *= 0.4f;
                    }
                }
                else
                {
                    Vector2 targetVector = player.position;
                    float aimY = targetVector.Y - 640f;
                    float distanceFromTargetPos = Math.Abs(NPC.Top.Y - aimY);
                    bool inRange = NPC.Top.Y <= aimY + 160f && NPC.Top.Y >= aimY - 16f;

                    if (phase2 && NPC.ai[1] == 0f)
                    {
                        NPC.noTileCollide = true;

                        calamityGlobalNPC.newAI[3] += 1f;

                        if (inRange)
                            NPC.velocity.Y = 0f;
                        else if (NPC.Top.Y > aimY)
                            NPC.velocity.Y -= 0.2f + distanceFromTargetPos * 0.001f;
                        else
                            NPC.velocity.Y += 0.2f + distanceFromTargetPos * 0.001f;

                        if (NPC.velocity.Y < velocityY)
                            NPC.velocity.Y = velocityY;
                        if (NPC.velocity.Y > -velocityY)
                            NPC.velocity.Y = -velocityY;
                    }

                    float maxOffsetScale = death ? 320f : 240f;
                    float maxOffset = maxOffsetScale * (1f - lifeRatio);
                    float offset = phase2 ? maxOffset * calamityGlobalNPC.newAI[2] : 0f;

                    // Set offset to 0 if the target stops moving
                    if (Math.Abs(player.velocity.X) < 0.5f)
                        calamityGlobalNPC.newAI[2] = 0f;
                    else
                        calamityGlobalNPC.newAI[2] = player.direction;

                    if ((NPC.position.X < targetVector.X + offset && NPC.position.X + NPC.width > targetVector.X + player.width + offset && (inRange || NPC.ai[0] != 2f)) || NPC.ai[1] > 0f || calamityGlobalNPC.newAI[3] >= 180f)
                    {
                        NPC.damage = NPC.defDamage;

                        if (phase2)
                        {
                            float stopBeforeFallTime = malice ? 25f : 30f;
                            if (!anyHeadActive)
                                stopBeforeFallTime -= 15f;
                            else if (expertMode)
                                stopBeforeFallTime -= death ? 15f * (1f - lifeRatio) : 10f * (1f - lifeRatio);

                            if (NPC.ai[1] < stopBeforeFallTime)
                            {
                                NPC.ai[1] += 1f;
                                NPC.velocity = Vector2.Zero;
                            }
                            else
                            {
                                float fallSpeedBoost = !anyHeadActive ? 1.8f : death ? 1.8f * (1f - lifeRatio) : 1.2f * (1f - lifeRatio);
                                float fallSpeed = (malice ? 1.8f : 1.2f) + fallSpeedBoost;

                                if (calamityGlobalNPC.newAI[1] > 1f)
                                    fallSpeed *= calamityGlobalNPC.newAI[1];

                                NPC.velocity.Y += fallSpeed;

                                NPC.ai[1] = 31f;
                            }
                        }
                        else
                        {
                            NPC.velocity.X *= 0.9f;

                            if (NPC.Bottom.Y < player.position.Y)
                            {
                                float fallSpeedBoost = !anyHeadActive ? 0.9f : death ? 0.9f * (1f - lifeRatio) : 0.6f * (1f - lifeRatio);
                                float fallSpeed = (malice ? 0.9f : 0.6f) + fallSpeedBoost;

                                if (calamityGlobalNPC.newAI[1] > 1f)
                                    fallSpeed *= calamityGlobalNPC.newAI[1];

                                NPC.velocity.Y += fallSpeed;
                            }
                        }
                    }
                    else
                    {
                        float velocityMult = malice ? 2f : 1.8f;
                        float velocityXChange = 0.2f + Math.Abs(NPC.Center.X - player.Center.X) * 0.001f;

                        float velocityXBoost = !anyHeadActive ? 6f : death ? 6f * (1f - lifeRatio) : 4f * (1f - lifeRatio);
                        float velocityX = 8f + velocityXBoost + Math.Abs(NPC.Center.X - player.Center.X) * 0.001f;

                        if (!rightClawActive)
                            velocityX += 1f;
                        if (!leftClawActive)
                            velocityX += 1f;
                        if (!headActive)
                            velocityX += 1f;
                        if (!rightLegActive)
                            velocityX += 1f;
                        if (!leftLegActive)
                            velocityX += 1f;

                        if (phase2)
                        {
                            NPC.damage = 0;
                            velocityXChange *= velocityMult;
                            velocityX *= velocityMult;
                        }

                        if (NPC.direction < 0)
                            NPC.velocity.X -= velocityXChange;
                        else if (NPC.direction > 0)
                            NPC.velocity.X += velocityXChange;

                        if (NPC.velocity.X < -velocityX)
                            NPC.velocity.X = -velocityX;
                        if (NPC.velocity.X > velocityX)
                            NPC.velocity.X = velocityX;
                    }

                    CustomGravity();
                }
            }

            void CustomGravity()
            {
                float gravity = phase2 ? 0f : 0.45f;
                float maxFallSpeed = reduceFallSpeed ? 12f : phase2 ? 24f : 15f;
                if (malice && !reduceFallSpeed)
                {
                    gravity *= 1.25f;
                    maxFallSpeed *= 1.25f;
                }

                if (calamityGlobalNPC.newAI[1] > 1f && !reduceFallSpeed)
                    maxFallSpeed *= calamityGlobalNPC.newAI[1];

                NPC.velocity.Y += gravity;
                if (NPC.velocity.Y > maxFallSpeed)
                    NPC.velocity.Y = maxFallSpeed;
            }

            player = Main.player[NPC.target];
            if (NPC.target <= 0 || NPC.target == 255 || player.dead || !player.active)
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];
            }

            int distanceFromTarget = player.dead ? 1600 : malice ? 8400 : 5600;
            if (Vector2.Distance(NPC.Center, player.Center) > distanceFromTarget)
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];

                if (Vector2.Distance(NPC.Center, player.Center) > distanceFromTarget)
                {
                    NPC.active = false;
                    NPC.netUpdate = true;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y);
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Vector2 vector = center - screenPos;
            vector -= new Vector2(ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerBodyGlow").Value.Width, ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerBodyGlow").Value.Height / Main.npcFrameCount[NPC.type]) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + NPC.gfxOffY);
            Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Blue);
            spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerBodyGlow").Value, vector,
                NPC.frame, color, NPC.rotation, vector11, 1f, spriteEffects, 0f);
            Color color2 = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
            spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerLegRight").Value, new Vector2(center.X - screenPos.X + 28f, center.Y - screenPos.Y + 20f), //72
                new Rectangle?(new Rectangle(0, 0, ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerLegRight").Value.Width, ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerLegRight").Value.Height)),
                color2, 0f, default, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerLegLeft").Value, new Vector2(center.X - screenPos.X - 112f, center.Y - screenPos.Y + 20f), //72
                new Rectangle?(new Rectangle(0, 0, ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerLegLeft").Value.Width, ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerLegLeft").Value.Height)),
                color2, 0f, default, 1f, SpriteEffects.None, 0f);
            if (NPC.AnyNPCs(ModContent.NPCType<RavagerHead>()))
            {
                spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerHead").Value, new Vector2(center.X - screenPos.X - 70f, center.Y - screenPos.Y - 75f),
                    new Rectangle?(new Rectangle(0, 0, ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerHead").Value.Width, ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/RavagerHead").Value.Height)),
                    color2, 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 2f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ScavengerGores/ScavengerBody").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ScavengerGores/ScavengerBody2").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ScavengerGores/ScavengerBody3").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ScavengerGores/ScavengerBody4").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ScavengerGores/ScavengerBody5").Type, 1f);
                }
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 5, hitDirection, -1f, 0, default, 2f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Vector2 npcCenter = NPC.Center;

            // NOTE: Right and left hitboxes are interchangeable, each hitbox is the same size and is located to the right or left of the center hitbox.
            // Width = 83, Height = 107
            Rectangle leftHitbox = new Rectangle((int)(npcCenter.X - (NPC.width / 2f) + 8f), (int)(npcCenter.Y - (NPC.height / 4f)), NPC.width / 4, NPC.height / 2);
            // Width = 166, Height = 214
            Rectangle bodyHitbox = new Rectangle((int)(npcCenter.X - (NPC.width / 4f)), (int)(npcCenter.Y - (NPC.height / 2f) + 8f), NPC.width / 2, NPC.height);
            // Width = 83, Height = 107
            Rectangle rightHitbox = new Rectangle((int)(npcCenter.X + (NPC.width / 4f) - 8f), (int)(npcCenter.Y - (NPC.height / 4f)), NPC.width / 4, NPC.height / 2);

            Vector2 leftHitboxCenter = new Vector2(leftHitbox.X + (leftHitbox.Width / 2), leftHitbox.Y + (leftHitbox.Height / 2));
            Vector2 bodyHitboxCenter = new Vector2(bodyHitbox.X + (bodyHitbox.Width / 2), bodyHitbox.Y + (bodyHitbox.Height / 2));
            Vector2 rightHitboxCenter = new Vector2(rightHitbox.X + (rightHitbox.Width / 2), rightHitbox.Y + (rightHitbox.Height / 2));

            Rectangle targetHitbox = target.Hitbox;

            float leftDist1 = Vector2.Distance(leftHitboxCenter, targetHitbox.TopLeft());
            float leftDist2 = Vector2.Distance(leftHitboxCenter, targetHitbox.TopRight());
            float leftDist3 = Vector2.Distance(leftHitboxCenter, targetHitbox.BottomLeft());
            float leftDist4 = Vector2.Distance(leftHitboxCenter, targetHitbox.BottomRight());

            float minLeftDist = leftDist1;
            if (leftDist2 < minLeftDist)
                minLeftDist = leftDist2;
            if (leftDist3 < minLeftDist)
                minLeftDist = leftDist3;
            if (leftDist4 < minLeftDist)
                minLeftDist = leftDist4;

            bool insideLeftHitbox = minLeftDist <= 55f;

            float bodyDist1 = Vector2.Distance(bodyHitboxCenter, targetHitbox.TopLeft());
            float bodyDist2 = Vector2.Distance(bodyHitboxCenter, targetHitbox.TopRight());
            float bodyDist3 = Vector2.Distance(bodyHitboxCenter, targetHitbox.BottomLeft());
            float bodyDist4 = Vector2.Distance(bodyHitboxCenter, targetHitbox.BottomRight());

            float minBodyDist = bodyDist1;
            if (bodyDist2 < minBodyDist)
                minBodyDist = bodyDist2;
            if (bodyDist3 < minBodyDist)
                minBodyDist = bodyDist3;
            if (bodyDist4 < minBodyDist)
                minBodyDist = bodyDist4;

            bool insideBodyHitbox = minBodyDist <= 110f;

            float rightDist1 = Vector2.Distance(rightHitboxCenter, targetHitbox.TopLeft());
            float rightDist2 = Vector2.Distance(rightHitboxCenter, targetHitbox.TopRight());
            float rightDist3 = Vector2.Distance(rightHitboxCenter, targetHitbox.BottomLeft());
            float rightDist4 = Vector2.Distance(rightHitboxCenter, targetHitbox.BottomRight());

            float minRightDist = rightDist1;
            if (rightDist2 < minRightDist)
                minRightDist = rightDist2;
            if (rightDist3 < minRightDist)
                minRightDist = rightDist3;
            if (rightDist4 < minRightDist)
                minRightDist = rightDist4;

            bool insideRightHitbox = minRightDist <= 55f;

            return insideLeftHitbox || insideBodyHitbox || insideRightHitbox;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300, true);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "Ravager";
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);


            // Mark Ravager as dead
            DownedBossSystem.downedRavager = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<RavagerBag>()));

            npcLoot.Add(ModContent.ItemType<RavagerTrophy>(), 10);
            npcLoot.AddIf(() => !DownedBossSystem.downedRavager, ModContent.ItemType<KnowledgeRavager>());

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<UltimusCleaver>(),
                    ModContent.ItemType<RealmRavager>(),
                    ModContent.ItemType<Hematemesis>(),
                    ModContent.ItemType<SpikecragStaff>(),
                    ModContent.ItemType<CraniumSmasher>(),
                };
                normalOnly.Add(ItemDropRule.OneFromOptions(DropHelper.NormalWeaponDropRateInt, weapons));

                // Materials.
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(() => !DownedBossSystem.downedProvidence), ModContent.ItemType<FleshyGeodeT1>()));
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(() => DownedBossSystem.downedProvidence), ModContent.ItemType<FleshyGeodeT2>()));

                // Equipment
                normalOnly.Add(ModContent.ItemType<BloodPact>(), 3);
                normalOnly.Add(ModContent.ItemType<FleshTotem>(), 3);
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.If(() => DownedBossSystem.downedProvidence), ModContent.ItemType<BloodflareCore>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<RavagerMask>(), 7);
            }

            npcLoot.AddIf(() => !Main.expertMode, ModContent.ItemType<Vesuvius>(), 10);
        }
    }
}
