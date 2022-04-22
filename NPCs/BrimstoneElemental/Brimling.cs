using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.BrimstoneElemental
{
    public class Brimling : ModNPC
    {
        private bool boostDR = false;
        public static float normalDR = 0.15f;
        public static float boostedDR = 0.6f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimling");
            Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 0;
            NPC.width = 60;
            NPC.height = 60;
            NPC.defense = 0;
            NPC.DR_NERD(normalDR);
            NPC.lifeMax = 2000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit23;
            NPC.DeathSound = SoundID.NPCDeath39;
            if (DownedBossSystem.downedProvidence)
            {
                NPC.lifeMax = 26000;
            }
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 10000;
            }
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(boostDR);
            writer.Write(NPC.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            boostDR = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (float)(NPC.height / 2)) / 16f), 1f, 0f, 0f);
            if (CalamityGlobalNPC.brimstoneElemental < 0 || !Main.npc[CalamityGlobalNPC.brimstoneElemental].active)
            {
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            bool goIntoShell = NPC.life <= NPC.lifeMax * 0.25;
            bool provy = DownedBossSystem.downedProvidence && !BossRushEvent.BossRushActive;
            if (goIntoShell || Main.npc[CalamityGlobalNPC.brimstoneElemental].ai[0] == 4f)
            {
                boostDR = true;
                NPC.chaseable = false;
            }
            else
            {
                boostDR = false;
                NPC.chaseable = true;
            }

            // Set DR based on boost status
            NPC.Calamity().DR = boostDR ? boostedDR : normalDR;

            float num1446 = goIntoShell ? 1f : 6f;
            int num1447 = 480;
            if (NPC.localAI[1] == 1f)
            {
                NPC.localAI[1] = 0f;
                if (Main.rand.NextBool(4))
                {
                    NPC.ai[0] = (float)num1447;
                }
            }
            NPC.TargetClosest(true);
            NPC.rotation = Math.Abs(NPC.velocity.X) * (float)NPC.direction * 0.1f;
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            Vector2 value53 = NPC.Center + new Vector2((float)(NPC.direction * 20), 6f);
            Vector2 vector251 = Main.player[NPC.target].Center - value53;
            bool flag104 = Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1);
            NPC.localAI[0] += 1f;
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] >= ((CalamityWorld.death || BossRushEvent.BossRushActive) ? 180f : 360f) && Main.npc[CalamityGlobalNPC.brimstoneElemental].ai[0] != 4f)
            {
                NPC.localAI[0] = 0f;
                float speed = 5f;
                Vector2 vector = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)(NPC.height / 2));
                float num6 = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - vector.X;
                float num7 = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - vector.Y;
                float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                num8 = speed / num8;
                num6 *= num8;
                num7 *= num8;
                int type = ModContent.ProjectileType<BrimstoneHellfireball>();
                int damage = NPC.GetProjectileDamage(type);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, num6, num7, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, Main.player[NPC.target].Center.X, Main.player[NPC.target].Center.Y);
            }
            if (vector251.Length() > 400f || !flag104)
            {
                Vector2 value54 = vector251;
                if (value54.Length() > num1446)
                {
                    value54.Normalize();
                    value54 *= num1446;
                }
                int num1448 = 30;
                NPC.velocity = (NPC.velocity * (float)(num1448 - 1) + value54) / (float)num1448;
            }
            else
            {
                NPC.velocity *= 0.98f;
            }
            if (NPC.ai[2] != 0f && NPC.ai[3] != 0f)
            {
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                for (int num1449 = 0; num1449 < 20; num1449++)
                {
                    int num1450 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 235, 0f, 0f, 100, Color.Transparent, 1f);
                    Dust dust = Main.dust[num1450];
                    dust.velocity *= 3f;
                    Main.dust[num1450].noGravity = true;
                    Main.dust[num1450].scale = 2.5f;
                }
                NPC.Center = new Vector2(NPC.ai[2] * 16f, NPC.ai[3] * 16f);
                NPC.velocity = Vector2.Zero;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                if (NPC.localAI[0] > 240f) //Lower firing cooldown to prevent firing so quickly after a teleport
                    NPC.localAI[0] = 240f;
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                for (int num1451 = 0; num1451 < 20; num1451++)
                {
                    int num1452 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 235, 0f, 0f, 100, Color.Transparent, 1f);
                    Dust dust = Main.dust[num1452];
                    dust.velocity *= 3f;
                    Main.dust[num1452].noGravity = true;
                    Main.dust[num1452].scale = 2.5f;
                }
            }
            NPC.ai[0] += 1f;
            if (NPC.ai[0] >= (float)num1447 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.localAI[0] > 260f)
                {
                    NPC.localAI[0] -= 60f;
                }
                NPC.ai[0] = 0f;
                Point point12 = NPC.Center.ToTileCoordinates();
                Point point13 = Main.player[NPC.target].Center.ToTileCoordinates();
                int num1453 = 20;
                int num1454 = 3;
                int num1455 = 10;
                int num1456 = 1;
                int num1457 = 0;
                bool flag106 = false;
                if (vector251.Length() > 2000f)
                {
                    flag106 = true;
                }
                while (!flag106 && num1457 < 100)
                {
                    num1457++;
                    int num1458 = Main.rand.Next(point13.X - num1453, point13.X + num1453 + 1);
                    int num1459 = Main.rand.Next(point13.Y - num1453, point13.Y + num1453 + 1);
                    if ((num1459 < point13.Y - num1455 || num1459 > point13.Y + num1455 || num1458 < point13.X - num1455 || num1458 > point13.X + num1455) && (num1459 < point12.Y - num1454 || num1459 > point12.Y + num1454 || num1458 < point12.X - num1454 || num1458 > point12.X + num1454) && !Main.tile[num1458, num1459].HasUnactuatedTile)
                    {
                        bool flag107 = true;
                        if (flag107 && Main.tile[num1458, num1459].LiquidType == LiquidID.Lava)
                        {
                            flag107 = false;
                        }
                        if (flag107 && Collision.SolidTiles(num1458 - num1456, num1458 + num1456, num1459 - num1456, num1459 + num1456))
                        {
                            flag107 = false;
                        }
                        if (flag107)
                        {
                            NPC.ai[2] = (float)num1458;
                            NPC.ai[3] = (float)num1459;
                            break;
                        }
                    }
                }
                NPC.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1.0;
            if (!boostDR)
            {
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0;
                }
            }
            else
            {
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y < frameHeight * 4)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
            }
        }

        public override void OnKill()
        {
            if (!CalamityWorld.revenge)
            {
                int closestPlayer = Player.FindClosest(NPC.Center, 1, 1);
                if (Main.rand.Next(4) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
                    Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
