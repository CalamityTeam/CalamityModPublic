using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Items.Materials;

namespace CalamityMod.NPCs.Abyss
{
    public class Flounder : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flounder");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.chaseable = false;
            npc.damage = 10;
            npc.width = 42;
            npc.height = 32;
            npc.defense = 15;
            npc.lifeMax = 40;
            npc.aiStyle = -1;
            aiType = -1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.value = Item.buyPrice(0, 0, 0, 80);
            npc.HitSound = SoundID.NPCHit50;
            npc.DeathSound = SoundID.NPCDeath53;
            npc.knockBackResist = 0.35f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<FlounderBanner>();
        }

        public override void AI()
        {
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            int num = 200;
            if (npc.ai[2] == 0f)
            {
                npc.alpha = num;
                npc.TargetClosest(true);
                if (!Main.player[npc.target].dead && (Main.player[npc.target].Center - npc.Center).Length() < 170f &&
                    Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.ai[2] = -16f;
                }
                if (npc.velocity.X != 0f || npc.velocity.Y < 0f || npc.velocity.Y > 2f || npc.justHit)
                {
                    npc.ai[2] = -16f;
                }
                return;
            }
            if (npc.ai[2] < 0f)
            {
                if (npc.alpha > 0)
                {
                    npc.alpha -= num / 16;
                    if (npc.alpha < 0)
                    {
                        npc.alpha = 0;
                    }
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] == 0f)
                {
                    npc.ai[2] = 1f;
                    npc.velocity.X = (float)(npc.direction * 2);
                }
                return;
            }
            npc.alpha = 0;
            if (npc.ai[2] == 1f)
            {
                npc.chaseable = true;
                npc.noGravity = true;
                if (npc.direction == 0)
                {
                    npc.TargetClosest(true);
                }
                if (npc.wet)
                {
                    bool flag14 = false;
                    npc.TargetClosest(false);
                    if (!Main.player[npc.target].dead)
                    {
                        flag14 = true;
                    }
                    if (!flag14)
                    {
                        if (npc.collideX)
                        {
                            npc.velocity.X = npc.velocity.X * -1f;
                            npc.direction *= -1;
                            npc.netUpdate = true;
                        }
                        if (npc.collideY)
                        {
                            npc.netUpdate = true;
                            if (npc.velocity.Y > 0f)
                            {
                                npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                                npc.directionY = -1;
                                npc.ai[0] = -1f;
                            }
                            else if (npc.velocity.Y < 0f)
                            {
                                npc.velocity.Y = Math.Abs(npc.velocity.Y);
                                npc.directionY = 1;
                                npc.ai[0] = 1f;
                            }
                        }
                    }
                    if (flag14)
                    {
                        npc.TargetClosest(true);
                        npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
                        npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * 0.1f;
                        if (npc.velocity.X > 2f)
                        {
                            npc.velocity.X = 2f;
                        }
                        if (npc.velocity.X < -2f)
                        {
                            npc.velocity.X = -2f;
                        }
                        if (npc.velocity.Y > 1f)
                        {
                            npc.velocity.Y = 1f;
                        }
                        if (npc.velocity.Y < -1f)
                        {
                            npc.velocity.Y = -1f;
                        }
                        if ((Main.player[npc.target].Center - npc.Center).Length() < 350f)
                        {
                            npc.localAI[0] += 1f;
                            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 180f)
                            {
                                npc.localAI[0] = 0f;
                                if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                                {
                                    float speed = 4f;
                                    Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                                    float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X + (float)Main.rand.Next(-20, 21);
                                    float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-20, 21);
                                    float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                                    num8 = speed / num8;
                                    num6 *= num8;
                                    num7 *= num8;
                                    int damage = 25;
                                    if (Main.expertMode)
                                    {
                                        damage = 19;
                                    }
                                    int beam = Projectile.NewProjectile(npc.Center.X + (npc.spriteDirection == 1 ? 10f : -10f), npc.Center.Y, num6, num7, ModContent.ProjectileType<SulphuricAcidMist>(), damage, 0f, Main.myPlayer, 0f, 0f);
                                }
                            }
                        }
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
                        if (npc.velocity.X < -1f || npc.velocity.X > 1f)
                        {
                            npc.velocity.X = npc.velocity.X * 0.95f;
                        }
                        if (npc.ai[0] == -1f)
                        {
                            npc.velocity.Y = npc.velocity.Y - 0.01f;
                            if ((double)npc.velocity.Y < -0.3)
                            {
                                npc.ai[0] = 1f;
                            }
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y + 0.01f;
                            if ((double)npc.velocity.Y > 0.3)
                            {
                                npc.ai[0] = -1f;
                            }
                        }
                    }
                    int num258 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                    int num259 = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                    if (Main.tile[num258, num259 - 1] == null)
                    {
                        Main.tile[num258, num259 - 1] = new Tile();
                    }
                    if (Main.tile[num258, num259 + 1] == null)
                    {
                        Main.tile[num258, num259 + 1] = new Tile();
                    }
                    if (Main.tile[num258, num259 + 2] == null)
                    {
                        Main.tile[num258, num259 + 2] = new Tile();
                    }
                    if (Main.tile[num258, num259 - 1].liquid > 128)
                    {
                        if (Main.tile[num258, num259 + 1].active())
                        {
                            npc.ai[0] = -1f;
                        }
                        else if (Main.tile[num258, num259 + 2].active())
                        {
                            npc.ai[0] = -1f;
                        }
                    }
                    if ((double)npc.velocity.Y > 0.4 || (double)npc.velocity.Y < -0.4)
                    {
                        npc.velocity.Y = npc.velocity.Y * 0.95f;
                    }
                }
                else
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.94f;
                        if ((double)npc.velocity.X > -0.2 && (double)npc.velocity.X < 0.2)
                        {
                            npc.velocity.X = 0f;
                        }
                    }
                    npc.velocity.Y = npc.velocity.Y + 0.2f;
                    if (npc.velocity.Y > 5f)
                    {
                        npc.velocity.Y = 5f;
                    }
                    npc.ai[0] = 1f;
                }
                npc.rotation = npc.velocity.Y * (float)npc.direction * 0.1f;
                if ((double)npc.rotation < -0.2)
                {
                    npc.rotation = -0.2f;
                }
                if ((double)npc.rotation > 0.2)
                {
                    npc.rotation = 0.2f;
                    return;
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Venom, 120, true);
        }

        public override void FindFrame(int frameHeight)
        {
            if (!npc.wet)
            {
                npc.frameCounter = 0.0;
                return;
            }
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe)
            {
                return 0f;
            }
            if (spawnInfo.player.Calamity().ZoneSulphur && spawnInfo.water)
            {
                return 0.2f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<CloakingGland>());
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
