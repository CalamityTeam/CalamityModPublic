using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AbyssNPCs
{
    public class Frogfish : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frogfish");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.chaseable = false;
            npc.damage = 25;
            npc.width = 60;
            npc.height = 50;
            npc.defense = 10;
            npc.lifeMax = 80;
            npc.aiStyle = -1;
            aiType = -1;
            npc.buffImmune[mod.BuffType("CrushDepth")] = true;
            npc.value = Item.buyPrice(0, 0, 0, 80);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.5f;
            banner = npc.type;
            bannerItem = mod.ItemType("FrogfishBanner");
        }

        public override void AI()
        {
            npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
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
                    if (Main.player[npc.target].wet && !Main.player[npc.target].dead)
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
                        npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.15f;
                        npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * 0.15f;
                        if (npc.velocity.X > 3.5f)
                        {
                            npc.velocity.X = 3.5f;
                        }
                        if (npc.velocity.X < -3.5f)
                        {
                            npc.velocity.X = -3.5f;
                        }
                        if (npc.velocity.Y > 1.5f)
                        {
                            npc.velocity.Y = 1.5f;
                        }
                        if (npc.velocity.Y < -1.5f)
                        {
                            npc.velocity.Y = -1.5f;
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
            player.AddBuff(BuffID.Venom, 180, true);
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
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            return SpawnCondition.OceanMonster.Chance * 0.2f;
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CloakingGland"));
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
