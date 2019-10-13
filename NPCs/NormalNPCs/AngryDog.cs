using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class AngryDog : ModNPC
    {
        private bool reset = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angry Dog");
            Main.npcFrameCount[npc.type] = 9;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.damage = 10;
            npc.width = 56;
            npc.height = 56;
            npc.defense = 4;
            npc.lifeMax = 50;
            if (CalamityWorld.downedCryogen)
            {
                npc.damage = 84;
                npc.defense = 10;
                npc.lifeMax = 1000;
            }
            npc.knockBackResist = 0.3f;
            animationType = 329;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 3, 0);
            npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AngryDogHit");
            npc.DeathSound = SoundID.NPCDeath5;
            banner = npc.type;
            bannerItem = mod.ItemType("AngryDogBanner");
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(reset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            reset = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (Main.rand.NextBool(900))
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AngryDogGrowl"), (int)npc.position.X, (int)npc.position.Y);
            }
            bool phase2 = (double)npc.life <= (double)npc.lifeMax * 0.5;
            if (phase2)
            {
                if (!reset)
                {
                    npc.ai[0] = 0f;
                    npc.ai[3] = 0f;
                    reset = true;
                    npc.netUpdate = true;
                }
                if (npc.ai[1] < 7f)
                {
                    npc.ai[1] += 1f;
                }
                int num = 30;
                bool flag = false;
                bool flag2 = false;
                bool flag3 = false;
                if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
                {
                    flag2 = true;
                    npc.ai[3] += 1f;
                }
                int num2 = 4;
                bool flag4 = npc.velocity.Y == 0f;
                for (int i = 0; i < 200; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type && Math.Abs(npc.position.X - Main.npc[i].position.X) + Math.Abs(npc.position.Y - Main.npc[i].position.Y) < (float)npc.width)
                    {
                        if (npc.position.X < Main.npc[i].position.X)
                        {
                            npc.velocity.X = npc.velocity.X - 0.05f;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X + 0.05f;
                        }
                        if (npc.position.Y < Main.npc[i].position.Y)
                        {
                            npc.velocity.Y = npc.velocity.Y - 0.05f;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y + 0.05f;
                        }
                    }
                }
                if (flag4)
                {
                    npc.velocity.Y = 0f;
                }
                if (npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num || flag2)
                {
                    npc.ai[3] += 1f;
                    flag3 = true;
                }
                else if (npc.ai[3] > 0f)
                {
                    npc.ai[3] -= 1f;
                }
                if (npc.ai[3] > (float)(num * num2))
                {
                    npc.ai[3] = 0f;
                }
                if (npc.justHit)
                {
                    npc.ai[3] = 0f;
                }
                if (npc.ai[3] == (float)num)
                {
                    npc.netUpdate = true;
                }
                Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num3 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X;
                float num4 = Main.player[npc.target].position.Y - vector.Y;
                float num5 = (float)Math.Sqrt((double)(num3 * num3 + num4 * num4));
                if (num5 < 200f && !flag3)
                {
                    npc.ai[3] = 0f;
                }
                if (npc.velocity.Y == 0f && Math.Abs(npc.velocity.X) > 3f && ((npc.Center.X < Main.player[npc.target].Center.X && npc.velocity.X > 0f) || (npc.Center.X > Main.player[npc.target].Center.X && npc.velocity.X < 0f)))
                {
                    npc.velocity.Y = npc.velocity.Y - 4f;
                    for (int k = 0; k < 5; k++)
                    {
                        Dust.NewDust(npc.position, npc.width, npc.height, 33, 0f, -1f, 0, default, 1f);
                    }
                }
                if (npc.ai[3] < (float)num)
                {
                    npc.TargetClosest(true);
                }
                else
                {
                    if (npc.velocity.X == 0f)
                    {
                        if (npc.velocity.Y == 0f)
                        {
                            npc.ai[0] += 1f;
                            if (npc.ai[0] >= 2f)
                            {
                                npc.direction *= -1;
                                npc.spriteDirection = npc.direction;
                                npc.ai[0] = 0f;
                            }
                        }
                    }
                    else
                    {
                        npc.ai[0] = 0f;
                    }
                    npc.directionY = -1;
                    if (npc.direction == 0)
                    {
                        npc.direction = 1;
                    }
                }

                if (!flag && (npc.velocity.Y == 0f || npc.wet || (npc.velocity.X <= 0f && npc.direction < 0) || (npc.velocity.X >= 0f && npc.direction > 0)))
                {
                    if (Math.Sign(npc.velocity.X) != npc.direction)
                    {
                        npc.velocity.X = npc.velocity.X * 0.92f;
                    }
                    float num7 = 5f;
                    float num8 = 0.2f;
                    if (npc.velocity.X < -num7 || npc.velocity.X > num7)
                    {
                        if (npc.velocity.Y == 0f)
                        {
                            npc.velocity *= 0.8f;
                        }
                    }
                    else if (npc.velocity.X < num7 && npc.direction == 1)
                    {
                        npc.velocity.X = npc.velocity.X + num8;
                        if (npc.velocity.X > num7)
                        {
                            npc.velocity.X = num7;
                        }
                    }
                    else if (npc.velocity.X > -num7 && npc.direction == -1)
                    {
                        npc.velocity.X = npc.velocity.X - num8;
                        if (npc.velocity.X < -num7)
                        {
                            npc.velocity.X = -num7;
                        }
                    }
                }
                if (npc.velocity.Y >= 0f)
                {
                    int num10 = 0;
                    if (npc.velocity.X < 0f)
                    {
                        num10 = -1;
                    }
                    if (npc.velocity.X > 0f)
                    {
                        num10 = 1;
                    }
                    Vector2 position = npc.position;
                    position.X += npc.velocity.X;
                    int num11 = (int)((position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num10)) / 16f);
                    int num12 = (int)((position.Y + (float)npc.height - 1f) / 16f);
                    if (Main.tile[num11, num12] == null)
                    {
                        Main.tile[num11, num12] = new Tile();
                    }
                    if (Main.tile[num11, num12 - 1] == null)
                    {
                        Main.tile[num11, num12 - 1] = new Tile();
                    }
                    if (Main.tile[num11, num12 - 2] == null)
                    {
                        Main.tile[num11, num12 - 2] = new Tile();
                    }
                    if (Main.tile[num11, num12 - 3] == null)
                    {
                        Main.tile[num11, num12 - 3] = new Tile();
                    }
                    if (Main.tile[num11, num12 + 1] == null)
                    {
                        Main.tile[num11, num12 + 1] = new Tile();
                    }
                    if ((float)(num11 * 16) < position.X + (float)npc.width && (float)(num11 * 16 + 16) > position.X && ((Main.tile[num11, num12].nactive() && !Main.tile[num11, num12].topSlope() && !Main.tile[num11, num12 - 1].topSlope() && Main.tileSolid[(int)Main.tile[num11, num12].type] && !Main.tileSolidTop[(int)Main.tile[num11, num12].type]) || (Main.tile[num11, num12 - 1].halfBrick() && Main.tile[num11, num12 - 1].nactive())) && (!Main.tile[num11, num12 - 1].nactive() || !Main.tileSolid[(int)Main.tile[num11, num12 - 1].type] || Main.tileSolidTop[(int)Main.tile[num11, num12 - 1].type] || (Main.tile[num11, num12 - 1].halfBrick() && (!Main.tile[num11, num12 - 4].nactive() || !Main.tileSolid[(int)Main.tile[num11, num12 - 4].type] || Main.tileSolidTop[(int)Main.tile[num11, num12 - 4].type]))) && (!Main.tile[num11, num12 - 2].nactive() || !Main.tileSolid[(int)Main.tile[num11, num12 - 2].type] || Main.tileSolidTop[(int)Main.tile[num11, num12 - 2].type]) && (!Main.tile[num11, num12 - 3].nactive() || !Main.tileSolid[(int)Main.tile[num11, num12 - 3].type] || Main.tileSolidTop[(int)Main.tile[num11, num12 - 3].type]) && (!Main.tile[num11 - num10, num12 - 3].nactive() || !Main.tileSolid[(int)Main.tile[num11 - num10, num12 - 3].type]))
                    {
                        float num13 = (float)(num12 * 16);
                        if (Main.tile[num11, num12].halfBrick())
                        {
                            num13 += 8f;
                        }
                        if (Main.tile[num11, num12 - 1].halfBrick())
                        {
                            num13 -= 8f;
                        }
                        if (num13 < position.Y + (float)npc.height)
                        {
                            float num14 = position.Y + (float)npc.height - num13;
                            if ((double)num14 <= 16.1)
                            {
                                npc.gfxOffY += npc.position.Y + (float)npc.height - num13;
                                npc.position.Y = num13 - (float)npc.height;
                                if (num14 < 9f)
                                {
                                    npc.stepSpeed = 1f;
                                }
                                else
                                {
                                    npc.stepSpeed = 2f;
                                }
                            }
                        }
                    }
                }
                if (npc.velocity.Y == 0f)
                {
                    int num15 = (int)((npc.position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 2) * npc.direction) + npc.velocity.X * 5f) / 16f);
                    int num16 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);
                    if (Main.tile[num15, num16] == null)
                    {
                        Main.tile[num15, num16] = new Tile();
                    }
                    if (Main.tile[num15, num16 - 1] == null)
                    {
                        Main.tile[num15, num16 - 1] = new Tile();
                    }
                    if (Main.tile[num15, num16 - 2] == null)
                    {
                        Main.tile[num15, num16 - 2] = new Tile();
                    }
                    if (Main.tile[num15, num16 - 3] == null)
                    {
                        Main.tile[num15, num16 - 3] = new Tile();
                    }
                    if (Main.tile[num15, num16 + 1] == null)
                    {
                        Main.tile[num15, num16 + 1] = new Tile();
                    }
                    if (Main.tile[num15 + npc.direction, num16 - 1] == null)
                    {
                        Main.tile[num15 + npc.direction, num16 - 1] = new Tile();
                    }
                    if (Main.tile[num15 + npc.direction, num16 + 1] == null)
                    {
                        Main.tile[num15 + npc.direction, num16 + 1] = new Tile();
                    }
                    if (Main.tile[num15 - npc.direction, num16 + 1] == null)
                    {
                        Main.tile[num15 - npc.direction, num16 + 1] = new Tile();
                    }
                    int num17 = npc.spriteDirection;
                    num17 *= -1;
                    if ((npc.velocity.X < 0f && num17 == -1) || (npc.velocity.X > 0f && num17 == 1))
                    {
                        float num18 = 3f;
                        if (Main.tile[num15, num16 - 2].nactive() && Main.tileSolid[(int)Main.tile[num15, num16 - 2].type])
                        {
                            if (Main.tile[num15, num16 - 3].nactive() && Main.tileSolid[(int)Main.tile[num15, num16 - 3].type])
                            {
                                npc.velocity.Y = -8.5f;
                                npc.netUpdate = true;
                            }
                            else
                            {
                                npc.velocity.Y = -7.5f;
                                npc.netUpdate = true;
                            }
                        }
                        else if (Main.tile[num15, num16 - 1].nactive() && !Main.tile[num15, num16 - 1].topSlope() && Main.tileSolid[(int)Main.tile[num15, num16 - 1].type])
                        {
                            npc.velocity.Y = -7f;
                            npc.netUpdate = true;
                        }
                        else if (npc.position.Y + (float)npc.height - (float)(num16 * 16) > 20f && Main.tile[num15, num16].nactive() && !Main.tile[num15, num16].topSlope() && Main.tileSolid[(int)Main.tile[num15, num16].type])
                        {
                            npc.velocity.Y = -6f;
                            npc.netUpdate = true;
                        }
                        else if ((npc.directionY < 0 || Math.Abs(npc.velocity.X) > num18) && (!Main.tile[num15, num16 + 1].nactive() || !Main.tileSolid[(int)Main.tile[num15, num16 + 1].type]) && (!Main.tile[num15, num16 + 2].nactive() || !Main.tileSolid[(int)Main.tile[num15, num16 + 2].type]) && (!Main.tile[num15 + npc.direction, num16 + 3].nactive() || !Main.tileSolid[(int)Main.tile[num15 + npc.direction, num16 + 3].type]))
                        {
                            npc.velocity.Y = -8f;
                            npc.netUpdate = true;
                        }
                    }
                }
                npc.rotation += npc.velocity.X * 0.05f;
                npc.spriteDirection = -npc.direction;
                return;
            }
            int num19 = 30;
            int num20 = 10;
            bool flag19 = false;
            bool flag20 = false;
            bool flag30 = false;
            if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
            {
                flag20 = true;
                npc.ai[3] += 1f;
            }
            if ((npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num19) | flag20)
            {
                npc.ai[3] += 1f;
                flag30 = true;
            }
            else if (npc.ai[3] > 0f)
            {
                npc.ai[3] -= 1f;
            }
            if (npc.ai[3] > (float)(num19 * num20))
            {
                npc.ai[3] = 0f;
            }
            if (npc.justHit)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] == (float)num19)
            {
                npc.netUpdate = true;
            }
            Vector2 vector19 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float arg_31B_0 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector19.X;
            float num30 = Main.player[npc.target].position.Y - vector19.Y;
            float num40 = (float)Math.Sqrt((double)(arg_31B_0 * arg_31B_0 + num30 * num30));
            if (num40 < 200f && !flag30)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] < (float)num19)
            {
                npc.TargetClosest(true);
            }
            else
            {
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[0] += 1f;
                        if (npc.ai[0] >= 2f)
                        {
                            npc.direction *= -1;
                            npc.spriteDirection = npc.direction;
                            npc.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 0f;
                }
                npc.directionY = -1;
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }
            float num6 = 6f;
            float num70 = 0.07f;
            if (!flag19 && (npc.velocity.Y == 0f || npc.wet || (npc.velocity.X <= 0f && npc.direction < 0) || (npc.velocity.X >= 0f && npc.direction > 0)))
            {
                if (npc.velocity.X < -num6 || npc.velocity.X > num6)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < num6 && npc.direction == 1)
                {
                    npc.velocity.X = npc.velocity.X + num70;
                    if (npc.velocity.X > num6)
                    {
                        npc.velocity.X = num6;
                    }
                }
                else if (npc.velocity.X > -num6 && npc.direction == -1)
                {
                    npc.velocity.X = npc.velocity.X - num70;
                    if (npc.velocity.X < -num6)
                    {
                        npc.velocity.X = -num6;
                    }
                }
            }
            if (npc.velocity.Y >= 0f)
            {
                int num9 = 0;
                if (npc.velocity.X < 0f)
                {
                    num9 = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    num9 = 1;
                }
                Vector2 position = npc.position;
                position.X += npc.velocity.X;
                int num10 = (int)((position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num9)) / 16f);
                int num11 = (int)((position.Y + (float)npc.height - 1f) / 16f);
                if (Main.tile[num10, num11] == null)
                {
                    Main.tile[num10, num11] = new Tile();
                }
                if (Main.tile[num10, num11 - 1] == null)
                {
                    Main.tile[num10, num11 - 1] = new Tile();
                }
                if (Main.tile[num10, num11 - 2] == null)
                {
                    Main.tile[num10, num11 - 2] = new Tile();
                }
                if (Main.tile[num10, num11 - 3] == null)
                {
                    Main.tile[num10, num11 - 3] = new Tile();
                }
                if (Main.tile[num10, num11 + 1] == null)
                {
                    Main.tile[num10, num11 + 1] = new Tile();
                }
                if ((float)(num10 * 16) < position.X + (float)npc.width && (float)(num10 * 16 + 16) > position.X && ((Main.tile[num10, num11].nactive() && !Main.tile[num10, num11].topSlope() && !Main.tile[num10, num11 - 1].topSlope() && Main.tileSolid[(int)Main.tile[num10, num11].type] && !Main.tileSolidTop[(int)Main.tile[num10, num11].type]) || (Main.tile[num10, num11 - 1].halfBrick() && Main.tile[num10, num11 - 1].nactive())) && (!Main.tile[num10, num11 - 1].nactive() || !Main.tileSolid[(int)Main.tile[num10, num11 - 1].type] || Main.tileSolidTop[(int)Main.tile[num10, num11 - 1].type] || (Main.tile[num10, num11 - 1].halfBrick() && (!Main.tile[num10, num11 - 4].nactive() || !Main.tileSolid[(int)Main.tile[num10, num11 - 4].type] || Main.tileSolidTop[(int)Main.tile[num10, num11 - 4].type]))) && (!Main.tile[num10, num11 - 2].nactive() || !Main.tileSolid[(int)Main.tile[num10, num11 - 2].type] || Main.tileSolidTop[(int)Main.tile[num10, num11 - 2].type]) && (!Main.tile[num10, num11 - 3].nactive() || !Main.tileSolid[(int)Main.tile[num10, num11 - 3].type] || Main.tileSolidTop[(int)Main.tile[num10, num11 - 3].type]) && (!Main.tile[num10 - num9, num11 - 3].nactive() || !Main.tileSolid[(int)Main.tile[num10 - num9, num11 - 3].type]))
                {
                    float num12 = (float)(num11 * 16);
                    if (Main.tile[num10, num11].halfBrick())
                    {
                        num12 += 8f;
                    }
                    if (Main.tile[num10, num11 - 1].halfBrick())
                    {
                        num12 -= 8f;
                    }
                    if (num12 < position.Y + (float)npc.height)
                    {
                        float num13 = position.Y + (float)npc.height - num12;
                        if ((double)num13 <= 16.1)
                        {
                            npc.gfxOffY += npc.position.Y + (float)npc.height - num12;
                            npc.position.Y = num12 - (float)npc.height;
                            if (num13 < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
            if (npc.velocity.Y == 0f)
            {
                int num14 = (int)((npc.position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 2) * npc.direction) + npc.velocity.X * 5f) / 16f);
                int num15 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);
                if (Main.tile[num14, num15] == null)
                {
                    Main.tile[num14, num15] = new Tile();
                }
                if (Main.tile[num14, num15 - 1] == null)
                {
                    Main.tile[num14, num15 - 1] = new Tile();
                }
                if (Main.tile[num14, num15 - 2] == null)
                {
                    Main.tile[num14, num15 - 2] = new Tile();
                }
                if (Main.tile[num14, num15 - 3] == null)
                {
                    Main.tile[num14, num15 - 3] = new Tile();
                }
                if (Main.tile[num14, num15 + 1] == null)
                {
                    Main.tile[num14, num15 + 1] = new Tile();
                }
                if (Main.tile[num14 + npc.direction, num15 - 1] == null)
                {
                    Main.tile[num14 + npc.direction, num15 - 1] = new Tile();
                }
                if (Main.tile[num14 + npc.direction, num15 + 1] == null)
                {
                    Main.tile[num14 + npc.direction, num15 + 1] = new Tile();
                }
                if (Main.tile[num14 - npc.direction, num15 + 1] == null)
                {
                    Main.tile[num14 - npc.direction, num15 + 1] = new Tile();
                }
                int num16 = npc.spriteDirection;
                if ((npc.velocity.X < 0f && num16 == -1) || (npc.velocity.X > 0f && num16 == 1))
                {
                    float num17 = 3f;
                    if (Main.tile[num14, num15 - 2].nactive() && Main.tileSolid[(int)Main.tile[num14, num15 - 2].type])
                    {
                        if (Main.tile[num14, num15 - 3].nactive() && Main.tileSolid[(int)Main.tile[num14, num15 - 3].type])
                        {
                            npc.velocity.Y = -8.5f;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            npc.velocity.Y = -7.5f;
                            npc.netUpdate = true;
                        }
                    }
                    else if (Main.tile[num14, num15 - 1].nactive() && !Main.tile[num14, num15 - 1].topSlope() && Main.tileSolid[(int)Main.tile[num14, num15 - 1].type])
                    {
                        npc.velocity.Y = -7f;
                        npc.netUpdate = true;
                    }
                    else if (npc.position.Y + (float)npc.height - (float)(num15 * 16) > 20f && Main.tile[num14, num15].nactive() && !Main.tile[num14, num15].topSlope() && Main.tileSolid[(int)Main.tile[num14, num15].type])
                    {
                        npc.velocity.Y = -6f;
                        npc.netUpdate = true;
                    }
                    else if ((npc.directionY < 0 || Math.Abs(npc.velocity.X) > num17) && (!Main.tile[num14, num15 + 1].nactive() || !Main.tileSolid[(int)Main.tile[num14, num15 + 1].type]) && (!Main.tile[num14, num15 + 2].nactive() || !Main.tileSolid[(int)Main.tile[num14, num15 + 2].type]) && (!Main.tile[num14 + npc.direction, num15 + 3].nactive() || !Main.tileSolid[(int)Main.tile[num14 + npc.direction, num15 + 3].type]))
                    {
                        npc.velocity.Y = -8f;
                        npc.netUpdate = true;
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[1] < 7f && npc.ai[1] > 0f)
            {
                npc.frame.Y = frameHeight * 7;
                npc.frameCounter = 0.0;
                return;
            }
            if (npc.ai[1] >= 7f)
            {
                npc.frame.Y = frameHeight * 8;
                npc.frameCounter = 0.0;
                return;
            }
            if (npc.velocity.Y > 0f || npc.velocity.Y < 0f)
            {
                npc.frame.Y = frameHeight * 5;
                npc.frameCounter = 0.0;
            }
            else
            {
                npc.spriteDirection = npc.direction;
                npc.frameCounter += (double)(npc.velocity.Length() / 16f);
                if (npc.frameCounter > 12.0)
                {
                    npc.frame.Y = npc.frame.Y + frameHeight;
                    npc.frameCounter = 0.0;
                }
                if (npc.frame.Y >= frameHeight * 6)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.ZoneSnow &&
                !spawnInfo.player.ZoneTowerStardust &&
                !spawnInfo.player.ZoneTowerSolar &&
                !spawnInfo.player.ZoneTowerVortex &&
                !spawnInfo.player.ZoneTowerNebula &&
                !spawnInfo.player.ZoneDungeon &&
                !spawnInfo.player.Calamity().ZoneSunkenSea &&
                !spawnInfo.playerInTown && !spawnInfo.player.ZoneOldOneArmy && !Main.snowMoon && !Main.pumpkinMoon ? 0.012f : 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 180, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Leather, Main.rand.Next(1, 3));
            }
            if (Main.rand.NextBool(100))
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Cryophobia"));
            }
        }
    }
}
