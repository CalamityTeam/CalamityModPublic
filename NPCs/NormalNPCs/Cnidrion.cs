using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class Cnidrion : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cnidrion");
            Main.npcFrameCount[npc.type] = 10;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 3f;
            npc.aiStyle = -1;
            npc.damage = 20;
            npc.width = 160;
            npc.height = 80;
            npc.defense = 6;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.DR_NERD(0.05f);
            npc.lifeMax = 280;
            npc.knockBackResist = 0.05f;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 25, 0);
            npc.HitSound = SoundID.NPCHit12;
            npc.DeathSound = SoundID.NPCDeath18;
            npc.rarity = 2;
            banner = npc.type;
            bannerItem = ModContent.ItemType<CnidrionBanner>();
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.PillarZone() ||
                spawnInfo.player.InAstral() ||
                spawnInfo.player.ZoneCorrupt ||
                spawnInfo.player.ZoneCrimson ||
                spawnInfo.player.ZoneOldOneArmy ||
                spawnInfo.player.ZoneSkyHeight ||
                spawnInfo.playerSafe ||
                !spawnInfo.player.ZoneDesert ||
                !spawnInfo.player.ZoneOverworldHeight ||
                Main.eclipse ||
                Main.snowMoon ||
                Main.pumpkinMoon ||
                NPC.AnyNPCs(ModContent.NPCType<Cnidrion>()))
            {
                return 0f;
            }
            return 0.05f;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.1f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];
            bool expertMode = Main.expertMode;
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            float num823 = 1f;
            npc.TargetClosest(true);
            bool flag51 = false;
            int offsetX = 80;
            int projectileDamage = expertMode ? 9 : 12;
            if (npc.life < npc.lifeMax * 0.33 || (CalamityWorld.death && npc.life < npc.lifeMax * 0.6))
            {
                num823 = 2f;
            }
            if (npc.life < npc.lifeMax * 0.1)
            {
                num823 = 4f;
            }
            if (npc.ai[0] == 0f)
            {
                npc.ai[1] += 1f;
                if (npc.life < npc.lifeMax * 0.33 || (CalamityWorld.death && npc.life < npc.lifeMax * 0.6))
                {
                    npc.ai[1] += 1f;
                }
                if (npc.life < npc.lifeMax * 0.1)
                {
                    npc.ai[1] += 1f;
                }
                if (npc.ai[1] >= 300f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[1] = 0f;
                    if (npc.life < npc.lifeMax * 0.25)
                    {
                        npc.ai[0] = Main.rand.Next(3, 5);
                    }
                    else if (CalamityWorld.death && npc.life < npc.lifeMax * 0.6)
                    {
                        npc.ai[0] = Main.rand.Next(1, 5);
                    }
                    else
                    {
                        npc.ai[0] = Main.rand.Next(1, 3);
                    }
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                flag51 = true;
                npc.ai[1] += 1f;
                if (npc.ai[1] % 30f == 0f)
                {
                    Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + 20f);
                    vector18.X += offsetX * npc.direction;
                    float num829 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector18.X;
                    float num830 = Main.player[npc.target].position.Y - vector18.Y;
                    float num831 = (float)Math.Sqrt(num829 * num829 + num830 * num830);
                    float num832 = 6f;
                    num831 = num832 / num831;
                    num829 *= num831;
                    num830 *= num831;
                    num829 *= 1f + Main.rand.Next(-10, 11) * 0.01f;
                    num830 *= 1f + Main.rand.Next(-10, 11) * 0.01f;
                    Projectile.NewProjectile(vector18.X, vector18.Y, num829, num830, ModContent.ProjectileType<HorsWaterBlast>(), projectileDamage, 0f, Main.myPlayer);
                }
                if (npc.ai[1] >= 120f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 0f;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                flag51 = true;
                npc.ai[1] += 1f;
                if (npc.ai[1] > 60f && npc.ai[1] < 240f && npc.ai[1] % 16f == 0f)
                {
                    Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + 20f);
                    vector18.X += offsetX * npc.direction;
                    float num829 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector18.X;
                    float num830 = Main.player[npc.target].position.Y - vector18.Y;
                    float num831 = (float)Math.Sqrt(num829 * num829 + num830 * num830);
                    float num832 = 8f;
                    num831 = num832 / num831;
                    num829 *= num831;
                    num830 *= num831;
                    num829 *= 1f + Main.rand.Next(-15, 16) * 0.01f;
                    num830 *= 1f + Main.rand.Next(-15, 16) * 0.01f;
                    Projectile.NewProjectile(vector18.X, vector18.Y, num829, num830, ModContent.ProjectileType<HorsWaterBlast>(), projectileDamage, 0f, Main.myPlayer);
                }
                if (npc.ai[1] >= 300f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 0f;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                num823 = 4f;
                npc.ai[1] += 1f;
                if (npc.ai[1] % 30f == 0f)
                {
                    Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + 20f);
                    vector18.X += offsetX * npc.direction;
                    float num844 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector18.X;
                    float num845 = Main.player[npc.target].position.Y - vector18.Y;
                    float num846 = (float)Math.Sqrt(num844 * num844 + num845 * num845);
                    float num847 = 10f;
                    num846 = num847 / num846;
                    num844 *= num846;
                    num845 *= num846;
                    num844 *= 1f + Main.rand.Next(-10, 11) * 0.001f;
                    num845 *= 1f + Main.rand.Next(-10, 11) * 0.001f;
                    int num848 = Projectile.NewProjectile(vector18.X, vector18.Y, num844, num845, ModContent.ProjectileType<HorsWaterBlast>(), projectileDamage, 0f, Main.myPlayer);
                }
                if (npc.ai[1] >= 120f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 0f;
                }
            }
            else if (npc.ai[0] == 4f)
            {
                num823 = 4f;
                npc.ai[1] += 1f;
                if (npc.ai[1] % 20f == 0f)
                {
                    Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + 20f);
                    vector18.X += offsetX * npc.direction;
                    float num829 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector18.X;
                    float num830 = Main.player[npc.target].position.Y - vector18.Y;
                    float num831 = (float)Math.Sqrt(num829 * num829 + num830 * num830);
                    float num832 = 11f;
                    num831 = num832 / num831;
                    num829 *= num831;
                    num830 *= num831;
                    num829 *= 1f + Main.rand.Next(-5, 6) * 0.01f;
                    num830 *= 1f + Main.rand.Next(-5, 6) * 0.01f;
                    int num833 = Projectile.NewProjectile(vector18.X, vector18.Y, num829, num830, ModContent.ProjectileType<HorsWaterBlast>(), projectileDamage, 0f, Main.myPlayer);
                }
                if (npc.ai[1] >= 240f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 0f;
                }
            }

            if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 50f)
                flag51 = true;

            if (flag51)
            {
                npc.velocity.X = npc.velocity.X * 0.9f;
                if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                    npc.velocity.X = 0f;
            }
            else
            {
                if (npc.direction > 0)
                    npc.velocity.X = (npc.velocity.X * 20f + num823) / 21f;

                if (npc.direction < 0)
                    npc.velocity.X = (npc.velocity.X * 20f - num823) / 21f;
            }

            int num854 = 80;
            int num855 = 20;
            Vector2 position2 = new Vector2(npc.Center.X - (num854 / 2), npc.position.Y + npc.height - num855);
            bool flag52 = false;
            if (npc.position.X < Main.player[npc.target].position.X && npc.position.X + npc.width > Main.player[npc.target].position.X + Main.player[npc.target].width && npc.position.Y + npc.height < Main.player[npc.target].position.Y + Main.player[npc.target].height - 16f)
                flag52 = true;

            if (flag52)
            {
                npc.velocity.Y += 0.5f;
                if (npc.velocity.Y > 10f)
                    npc.velocity.Y = 10f;
            }
            else if (Collision.SolidCollision(position2, num854, num855))
            {
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y = 0f;

                if (npc.velocity.Y > -0.2f)
                    npc.velocity.Y -= 0.025f;
                else
                    npc.velocity.Y -= 0.2f;

                if (npc.velocity.Y < -2f)
                    npc.velocity.Y = -2f;
            }
            else
            {
                if (npc.velocity.Y < 0f)
                    npc.velocity.Y = 0f;

                if (npc.velocity.Y < 0.5)
                    npc.velocity.Y += 0.025f;
                else
                    npc.velocity.Y += 0.25f;

                if (npc.velocity.Y > 2f)
                    npc.velocity.Y = 2f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 2f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<AmidiasSpark>(), 4);
        }
    }
}
