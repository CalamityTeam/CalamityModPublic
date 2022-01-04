using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public partial class CalamityGlobalAI
    {
        // Master Mode changes
        /* 1 - Spawns a clone of itself that copies every movement of the main eye but inverted (if main is on top and to the left, the mirror is on bottom and to the right)
           2 - Damaging either eye causes damage to both
           3 - Horizontal dashes are far more common
           4 - The delay between dashes and horizontal dashes is reduced*/
        public static bool BuffedEyeofCthulhuAI(NPC npc, bool enraged, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Increase aggression if player is taking a long time to kill the boss
            if (lifeRatio > calamityGlobalNPC.killTimeRatio_IncreasedAggression)
                lifeRatio = calamityGlobalNPC.killTimeRatio_IncreasedAggression;

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive || enraged;
            bool death = CalamityWorld.death || malice;
            bool phase2 = lifeRatio < 0.75f;
            bool phase3 = lifeRatio < 0.65f;
            bool phase4 = lifeRatio < 0.55f;
            bool phase5 = lifeRatio < 0.4f;
            float num5 = (float)(death ? 20 - Math.Round(5f * (1f - lifeRatio)) : 20);

            float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
            if (Main.dayTime || malice)
            {
                npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive || enraged;
                enrageScale += 2f;
            }

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            bool dead = Main.player[npc.target].dead;
            float num6 = npc.position.X + (npc.width / 2) - Main.player[npc.target].position.X - (Main.player[npc.target].width / 2);
            float num7 = npc.position.Y + npc.height - 59f - Main.player[npc.target].position.Y - (Main.player[npc.target].height / 2);
            float num8 = (float)Math.Atan2(num7, num6) + MathHelper.PiOver2;

            if (num8 < 0f)
                num8 += MathHelper.TwoPi;
            else if (num8 > MathHelper.TwoPi)
                num8 -= MathHelper.TwoPi;

            float num9 = 0f;
            if (npc.ai[0] == 0f && npc.ai[1] == 0f)
                num9 = 0.02f;
            if (npc.ai[0] == 0f && npc.ai[1] == 2f && npc.ai[2] > 40f)
                num9 = 0.05f;
            if (npc.ai[0] == 3f && npc.ai[1] == 0f)
                num9 = 0.05f;
            if (npc.ai[0] == 3f && npc.ai[1] == 2f && npc.ai[2] > 40f)
                num9 = 0.08f;
            if (npc.ai[0] == 3f && npc.ai[1] == 4f && npc.ai[2] > num5)
                num9 = 0.15f;
            if (npc.ai[0] == 3f && npc.ai[1] == 5f)
                num9 = 0.05f;
            num9 *= 1.5f;

            if (npc.rotation < num8)
            {
                if ((num8 - npc.rotation) > MathHelper.Pi)
                    npc.rotation -= num9;
                else
                    npc.rotation += num9;
            }
            else if (npc.rotation > num8)
            {
                if ((npc.rotation - num8) > MathHelper.Pi)
                    npc.rotation += num9;
                else
                    npc.rotation -= num9;
            }

            if (npc.rotation > num8 - num9 && npc.rotation < num8 + num9)
                npc.rotation = num8;
            if (npc.rotation < 0f)
                npc.rotation += MathHelper.TwoPi;
            else if (npc.rotation > MathHelper.TwoPi)
                npc.rotation -= MathHelper.TwoPi;
            if (npc.rotation > num8 - num9 && npc.rotation < num8 + num9)
                npc.rotation = num8;

            if (Main.rand.NextBool(5))
            {
                int num10 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f), npc.width, (int)(npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default, 1f);
                Dust dust = Main.dust[num10];
                dust.velocity.X *= 0.5f;
                dust.velocity.Y *= 0.1f;
            }

            if (dead)
            {
                npc.velocity.Y -= 0.04f;

                if (npc.timeLeft > 10)
                    npc.timeLeft = 10;
            }

            else if (npc.ai[0] == 0f)
            {
                if (npc.ai[1] == 0f)
                {
                    float num11 = 7f;
                    float num12 = 0.15f;
                    num11 += 5f * enrageScale;
                    num12 += 0.1f * enrageScale;

                    if (death)
                    {
                        num11 += 8f * (1f - lifeRatio);
                        num12 += 0.17f * (1f - lifeRatio);
                    }

                    Vector2 vector = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num13 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector.X;
                    float num14 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 200f - vector.Y;
                    float num15 = (float)Math.Sqrt(num13 * num13 + num14 * num14);
                    float num16 = num15;

                    num15 = num11 / num15;
                    num13 *= num15;
                    num14 *= num15;

                    if (npc.velocity.X < num13)
                    {
                        npc.velocity.X += num12;
                        if (npc.velocity.X < 0f && num13 > 0f)
                            npc.velocity.X += num12;
                    }
                    else if (npc.velocity.X > num13)
                    {
                        npc.velocity.X -= num12;
                        if (npc.velocity.X > 0f && num13 < 0f)
                            npc.velocity.X -= num12;
                    }
                    if (npc.velocity.Y < num14)
                    {
                        npc.velocity.Y += num12;
                        if (npc.velocity.Y < 0f && num14 > 0f)
                            npc.velocity.Y += num12;
                    }
                    else if (npc.velocity.Y > num14)
                    {
                        npc.velocity.Y -= num12;
                        if (npc.velocity.Y > 0f && num14 < 0f)
                            npc.velocity.Y -= num12;
                    }

                    npc.ai[2] += 1f;
                    float num17 = 180f - (death ? 200f * (1f - lifeRatio) : 0f);
                    if (npc.ai[2] >= num17)
                    {
                        npc.ai[1] = 1f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }
                    else if (num16 < 500f)
                    {
                        if (!Main.player[npc.target].dead)
                            npc.ai[3] += 1f;

                        if (npc.ai[3] >= 40f)
                        {
                            npc.ai[3] = 0f;
                            npc.rotation = num8;

                            float num19 = 6f;
                            float num20 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector.X;
                            float num21 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector.Y;
                            float num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);

                            num22 = num19 / num22;
                            Vector2 vector2 = vector;
                            Vector2 vector3;
                            vector3.X = num20 * num22;
                            vector3.Y = num21 * num22;
                            vector2.X += vector3.X * 10f;
                            vector2.Y += vector3.Y * 10f;

                            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(NPCID.ServantofCthulhu) < 12)
                            {
                                int num23 = NPC.NewNPC((int)vector2.X, (int)vector2.Y, NPCID.ServantofCthulhu, 0, 0f, 0f, 0f, 0f, 255);
                                Main.npc[num23].velocity.X = vector3.X;
                                Main.npc[num23].velocity.Y = vector3.Y;

                                if (Main.netMode == NetmodeID.Server && num23 < Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num23, 0f, 0f, 0f, 0, 0, 0);
                            }

                            Main.PlaySound(SoundID.NPCHit1, (int)vector2.X, (int)vector2.Y);

                            int num;
                            for (int m = 0; m < 10; m = num + 1)
                            {
                                Dust.NewDust(vector2, 20, 20, 5, vector3.X * 0.4f, vector3.Y * 0.4f, 0, default, 1f);
                                num = m;
                            }
                        }
                    }
                }
                else if (npc.ai[1] == 1f)
                {
                    npc.rotation = num8;
                    float num24 = 7.25f;
                    num24 += 5f * enrageScale;
                    if (death)
                        num24 += 8.5f * (1f - lifeRatio);

                    Vector2 vector4 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num25 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector4.X;
                    float num26 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector4.Y;
                    float num27 = (float)Math.Sqrt(num25 * num25 + num26 * num26);

                    num27 = num24 / num27;
                    npc.velocity.X = num25 * num27;
                    npc.velocity.Y = num26 * num27;

                    npc.ai[1] = 2f;
                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }
                else if (npc.ai[1] == 2f)
                {
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 40f)
                    {
                        npc.velocity *= 0.975f;
                        if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                            npc.velocity.X = 0f;
                        if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                            npc.velocity.Y = 0f;
                    }
                    else
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

                    int num28 = 90;
                    if (death)
                        num28 -= (int)Math.Round(120f * (1f - lifeRatio));

                    if (npc.ai[2] >= num28)
                    {
                        npc.ai[3] += 1f;
                        npc.ai[2] = 0f;
                        npc.TargetClosest();
                        npc.rotation = num8;

                        if (npc.ai[3] >= 3f)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[3] = 0f;
                        }
                        else
                            npc.ai[1] = 1f;
                    }
                }

                if (phase2)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }
            }

            else if (npc.ai[0] == 1f || npc.ai[0] == 2f)
            {
                if (npc.ai[0] == 1f)
                {
                    npc.ai[2] += 0.005f;
                    if (npc.ai[2] > 0.5f)
                        npc.ai[2] = 0.5f;
                }
                else
                {
                    npc.ai[2] -= 0.005f;
                    if (npc.ai[2] < 0f)
                        npc.ai[2] = 0f;
                }

                npc.rotation += npc.ai[2];
                npc.ai[1] += 1f;
                if (npc.ai[1] % 20f == 0f)
                {
                    Vector2 vector5 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num31 = Main.rand.Next(-200, 200);
                    float num32 = Main.rand.Next(-200, 200);
                    float num33 = (float)Math.Sqrt(num31 * num31 + num32 * num32);

                    num33 = 8f / num33;
                    Vector2 vector6 = vector5;
                    Vector2 vector7;
                    vector7.X = num31 * num33;
                    vector7.Y = num32 * num33;
                    vector6.X += vector7.X * 10f;
                    vector6.Y += vector7.Y * 10f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num34 = NPC.NewNPC((int)vector6.X, (int)vector6.Y, NPCID.ServantofCthulhu, 0, 0f, 0f, 0f, 0f, 255);
                        Main.npc[num34].velocity.X = vector7.X;
                        Main.npc[num34].velocity.Y = vector7.Y;

                        if (Main.netMode == NetmodeID.Server && num34 < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num34, 0f, 0f, 0f, 0, 0, 0);
                    }

                    int num;
                    for (int n = 0; n < 10; n = num + 1)
                    {
                        Dust.NewDust(vector6, 20, 20, 5, vector7.X * 0.4f, vector7.Y * 0.4f, 0, default, 1f);
                        num = n;
                    }
                }

                if (npc.ai[1] == 100f)
                {
                    npc.ai[0] += 1f;
                    npc.ai[1] = 0f;

                    if (npc.ai[0] == 3f)
                        npc.ai[2] = 0f;
                    else
                    {
                        Main.PlaySound(SoundID.NPCHit1, (int)npc.position.X, (int)npc.position.Y);

                        int num;
                        for (int num35 = 0; num35 < 2; num35 = num + 1)
                        {
                            Gore.NewGore(npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 8, 1f);
                            Gore.NewGore(npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
                            Gore.NewGore(npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
                            num = num35;
                        }

                        for (int num36 = 0; num36 < 20; num36 = num + 1)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, 5, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
                            num = num36;
                        }

                        Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
                    }
                }

                Dust.NewDust(npc.position, npc.width, npc.height, 5, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
                npc.velocity *= 0.98f;

                if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                    npc.velocity.X = 0f;
                if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                    npc.velocity.Y = 0f;
            }

            else
            {
                npc.defense = 0;
                npc.damage = (int)(npc.defDamage * (phase5 ? 1.4f : 1.2f));

                if (npc.ai[1] == 0f & phase5)
                    npc.ai[1] = 5f;

                if (npc.ai[1] == 0f)
                {
                    float num37 = 5.5f + 3.5f * (0.75f - lifeRatio);
                    float num38 = 0.06f + 0.025f * (0.75f - lifeRatio);
                    num37 += 4f * enrageScale;
                    num38 += 0.04f * enrageScale;

                    if (death)
                    {
                        num37 += 5f * (0.75f - lifeRatio);
                        num38 += 0.04f * (0.75f - lifeRatio);
                    }

                    Vector2 vector8 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num39 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector8.X;
                    float num40 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 120f - vector8.Y;
                    float num41 = (float)Math.Sqrt(num39 * num39 + num40 * num40);

                    if (num41 > 400f)
                    {
                        num37 += 1.25f;
                        num38 += 0.075f;
                        if (num41 > 600f)
                        {
                            num37 += 1.25f;
                            num38 += 0.075f;
                            if (num41 > 800f)
                            {
                                num37 += 1.25f;
                                num38 += 0.075f;
                            }
                        }
                    }

                    num41 = num37 / num41;
                    num39 *= num41;
                    num40 *= num41;

                    if (npc.velocity.X < num39)
                    {
                        npc.velocity.X += num38;
                        if (npc.velocity.X < 0f && num39 > 0f)
                            npc.velocity.X += num38;
                    }
                    else if (npc.velocity.X > num39)
                    {
                        npc.velocity.X -= num38;
                        if (npc.velocity.X > 0f && num39 < 0f)
                            npc.velocity.X -= num38;
                    }
                    if (npc.velocity.Y < num40)
                    {
                        npc.velocity.Y += num38;
                        if (npc.velocity.Y < 0f && num40 > 0f)
                            npc.velocity.Y += num38;
                    }
                    else if (npc.velocity.Y > num40)
                    {
                        npc.velocity.Y -= num38;
                        if (npc.velocity.Y > 0f && num40 < 0f)
                            npc.velocity.Y -= num38;
                    }

                    npc.ai[2] += 1f;
                    float phaseLimit = 200f - (death ? 200f * (0.75f - lifeRatio) : 0f);
                    if (npc.ai[2] >= phaseLimit)
                    {
                        npc.ai[1] = 1f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;

                        if (phase4)
                            npc.ai[1] = 3f;

                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }
                }

                else if (npc.ai[1] == 1f)
                {
                    Main.PlaySound(SoundID.ForceRoar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
                    npc.rotation = num8;

                    float num42 = 6.2f + 4f * (0.75f - lifeRatio);
                    num42 += 4f * enrageScale;
                    if (death)
                        num42 += 5.5f * (0.75f - lifeRatio);
                    if (npc.ai[3] == 1f)
                        num42 *= 1.15f;
                    if (npc.ai[3] == 2f)
                        num42 *= 1.3f;

                    Vector2 vector9 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num43 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector9.X;
                    float num44 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector9.Y;
                    float num45 = (float)Math.Sqrt(num43 * num43 + num44 * num44);

                    num45 = num42 / num45;
                    npc.velocity.X = num43 * num45;
                    npc.velocity.Y = num44 * num45;
                    npc.ai[1] = 2f;
                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }

                else if (npc.ai[1] == 2f)
                {
                    float num46 = 60f;

                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= num46)
                    {
                        npc.velocity *= 0.96f;
                        if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                            npc.velocity.X = 0f;
                        if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                            npc.velocity.Y = 0f;
                    }
                    else
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

                    int num47 = 80;
                    if (death)
                        num47 -= (int)Math.Round(40f * (0.75f - lifeRatio));

                    if (npc.ai[2] >= num47)
                    {
                        npc.ai[3] += 1f;
                        npc.ai[2] = 0f;
                        npc.TargetClosest();
                        npc.rotation = num8;

                        if (npc.ai[3] >= 3f)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[3] = 0f;
                            if (Main.netMode != NetmodeID.MultiplayerClient && phase3)
                            {
                                npc.ai[1] = 3f;
                                npc.ai[3] += Main.rand.Next(1, 4);
                            }
                            npc.netUpdate = true;

                            if (npc.netSpam > 10)
                                npc.netSpam = 10;
                        }
                        else
                            npc.ai[1] = 1f;
                    }
                }

                else if (npc.ai[1] == 3f)
                {
                    if ((npc.ai[3] == 4f & phase5) && npc.Center.Y > Main.player[npc.target].Center.Y)
                    {
                        npc.TargetClosest();
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;

                        if (npc.netSpam > 10)
                            npc.netSpam = 10;
                    }
                    else if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speedBoost = death ? 5f * (0.65f - lifeRatio) : 3.5f * (0.65f - lifeRatio);
                        float num48 = 18f + speedBoost;
                        num48 += 10f * enrageScale;

                        Vector2 vector10 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float num49 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector10.X;
                        float num50 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector10.Y;
                        float num51 = Math.Abs(Main.player[npc.target].velocity.X) + Math.Abs(Main.player[npc.target].velocity.Y) / 4f;
                        num51 += 10f - num51;

                        if (num51 < 5f)
                            num51 = 5f;
                        if (num51 > 15f)
                            num51 = 15f;

                        if (npc.ai[2] == -1f)
                        {
                            num51 *= 4f;
                            num48 *= 1.3f;
                        }

                        num49 -= Main.player[npc.target].velocity.X * num51;
                        num50 -= Main.player[npc.target].velocity.Y * num51 / 4f;
                        num49 *= 1f + Main.rand.Next(-10, 11) * 0.01f;
                        num50 *= 1f + Main.rand.Next(-10, 11) * 0.01f;

                        float num52 = (float)Math.Sqrt(num49 * num49 + num50 * num50);
                        float num53 = num52;

                        num52 = num48 / num52;
                        npc.velocity.X = num49 * num52;
                        npc.velocity.Y = num50 * num52;
                        npc.velocity.X += Main.rand.Next(-20, 21) * 0.1f;
                        npc.velocity.Y += Main.rand.Next(-20, 21) * 0.1f;

                        if (num53 < 100f)
                        {
                            if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
                            {
                                float num56 = Math.Abs(npc.velocity.X);
                                float num57 = Math.Abs(npc.velocity.Y);

                                if (npc.Center.X > Main.player[npc.target].Center.X)
                                    num57 *= -1f;
                                if (npc.Center.Y > Main.player[npc.target].Center.Y)
                                    num56 *= -1f;

                                npc.velocity.X = num57;
                                npc.velocity.Y = num56;
                            }
                        }
                        else if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
                        {
                            float num58 = (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) / 2f;
                            float num59 = num58;

                            if (npc.Center.X > Main.player[npc.target].Center.X)
                                num59 *= -1f;
                            if (npc.Center.Y > Main.player[npc.target].Center.Y)
                                num58 *= -1f;

                            npc.velocity.X = num59;
                            npc.velocity.Y = num58;
                        }

                        npc.ai[1] = 4f;
                        npc.netUpdate = true;

                        if (npc.netSpam > 10)
                            npc.netSpam = 10;
                    }
                }

                else if (npc.ai[1] == 4f)
                {
                    if (npc.ai[2] == 0f)
                        Main.PlaySound(SoundID.ForceRoar, (int)npc.position.X, (int)npc.position.Y, -1, 1f, 0f);

                    float num60 = num5;
                    npc.ai[2] += 1f;

                    if (npc.ai[2] == num60 && Vector2.Distance(npc.position, Main.player[npc.target].position) < 200f)
                        npc.ai[2] -= 1f;

                    if (npc.ai[2] >= num60)
                    {
                        npc.velocity *= 0.95f;
                        if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                            npc.velocity.X = 0f;
                        if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                            npc.velocity.Y = 0f;
                    }
                    else
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

                    float num61 = num60 + 13f;
                    if (npc.ai[2] >= num61)
                    {
                        npc.netUpdate = true;

                        if (npc.netSpam > 10)
                            npc.netSpam = 10;

                        npc.ai[3] += 1f;
                        npc.ai[2] = 0f;

                        float maxCharges = death ? (lifeRatio < 0.05f ? 1f : lifeRatio < 0.1f ? 2f : lifeRatio < 0.15f ? 3f : lifeRatio < 0.25f ? 4f : 5f) : lifeRatio < 0.2f ? 4f : 5f;
                        if (npc.ai[3] >= maxCharges)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[3] = 0f;
                        }
                        else
                            npc.ai[1] = 3f;
                    }
                }

                else if (npc.ai[1] == 5f)
                {
                    float num62 = 600f;
                    float speedBoost = death ? 6f * (0.4f - lifeRatio) : 4f * (0.4f - lifeRatio);
                    float speedBoost2 = death ? 0.15f * (0.4f - lifeRatio) : 0.1f * (0.4f - lifeRatio);
                    float num63 = 8f + speedBoost;
                    float num64 = 0.25f + speedBoost2;

                    Vector2 vector11 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num65 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector11.X;
                    float num66 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) + num62 - vector11.Y;

                    bool horizontalCharge = calamityGlobalNPC.newAI[0] == 1f || calamityGlobalNPC.newAI[0] == 3f;
                    if (horizontalCharge)
                    {
                        num62 = calamityGlobalNPC.newAI[0] == 1f ? -500f : 500f;
                        num63 *= 1.5f;
                        num64 *= 1.5f;

                        num65 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + num62 - vector11.X;
                        num66 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector11.Y;
                    }

                    float num67 = (float)Math.Sqrt(num65 * num65 + num66 * num66);
                    num67 = num63 / num67;
                    num65 *= num67;
                    num66 *= num67;

                    if (npc.velocity.X < num65)
                    {
                        npc.velocity.X += num64;
                        if (npc.velocity.X < 0f && num65 > 0f)
                            npc.velocity.X += num64;
                    }
                    else if (npc.velocity.X > num65)
                    {
                        npc.velocity.X -= num64;
                        if (npc.velocity.X > 0f && num65 < 0f)
                            npc.velocity.X -= num64;
                    }
                    if (npc.velocity.Y < num66)
                    {
                        npc.velocity.Y += num64;
                        if (npc.velocity.Y < 0f && num66 > 0f)
                            npc.velocity.Y += num64;
                    }
                    else if (npc.velocity.Y > num66)
                    {
                        npc.velocity.Y -= num64;
                        if (npc.velocity.Y > 0f && num66 < 0f)
                            npc.velocity.Y -= num64;
                    }

                    npc.ai[2] += 1f;

                    if (npc.ai[2] % 45f == 0f)
                    {
                        float num19 = 6f;
                        Vector2 vector = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float num20 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector.X;
                        float num21 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector.Y;
                        float num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);

                        num22 = num19 / num22;
                        Vector2 vector2 = vector;
                        Vector2 vector3;
                        vector3.X = num20 * num22;
                        vector3.Y = num21 * num22;
                        vector2.X += vector3.X * 10f;
                        vector2.Y += vector3.Y * 10f;

                        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(NPCID.ServantofCthulhu) < 4)
                        {
                            int num23 = NPC.NewNPC((int)vector2.X, (int)vector2.Y, NPCID.ServantofCthulhu);
                            Main.npc[num23].velocity.X = vector3.X;
                            Main.npc[num23].velocity.Y = vector3.Y;

                            if (Main.netMode == NetmodeID.Server && num23 < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num23, 0f, 0f, 0f, 0, 0, 0);
                        }

                        Main.PlaySound(SoundID.NPCDeath13, npc.position);

                        for (int m = 0; m < 10; m++)
                            Dust.NewDust(vector2, 20, 20, 5, vector3.X * 0.4f, vector3.Y * 0.4f, 0, default, 1f);
                    }

                    float timeGateValue = horizontalCharge ? (100f - (death ? 20f * (0.4f - lifeRatio) : 0f)) : (70f - (death ? 15f * (0.4f - lifeRatio) : 0f));
                    if (npc.ai[2] >= timeGateValue)
                    {
                        switch ((int)calamityGlobalNPC.newAI[0])
                        {
                            case 0: // Normal Eye behavior
                                npc.ai[1] = 3f;
                                npc.ai[2] = -1f;
                                npc.ai[3] = -1f;
                                break;
                            case 1: // Charge from the left
                                npc.ai[1] = 6f;
                                npc.ai[2] = 0f;
                                break;
                            case 2: // Normal Eye behavior
                                npc.ai[1] = 3f;
                                npc.ai[2] = -1f;
                                break;
                            case 3: // Charge from the right
                                npc.ai[1] = 6f;
                                npc.ai[2] = 0f;
                                break;
                            default:
                                break;
                        }

                        npc.TargetClosest();
                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] > 3f)
                            calamityGlobalNPC.newAI[0] = 0f;

                        npc.SyncExtraAI();
                    }

                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }

                else if (npc.ai[1] == 6f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float speedBoost = death ? 6f * (0.4f - lifeRatio) : 4f * (0.4f - lifeRatio);
                        float num48 = 18f + speedBoost;
                        num48 += 10f * enrageScale;

                        Vector2 vector10 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float num49 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector10.X;
                        float num50 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector10.Y;
                        float num51 = (float)Math.Sqrt(num49 * num49 + num50 * num50);

                        num51 = num48 / num51;
                        npc.velocity.X = num49 * num51;
                        npc.velocity.Y = num50 * num51;

                        npc.ai[1] = 7f;
                        npc.netUpdate = true;

                        if (npc.netSpam > 10)
                            npc.netSpam = 10;
                    }
                }

                else if (npc.ai[1] == 7f)
                {
                    if (npc.ai[2] == 0f)
                        Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

                    float num60 = (float)Math.Round(num5 * 2.5f);
                    npc.ai[2] += 1f;

                    if (npc.ai[2] == num60 && Vector2.Distance(npc.position, Main.player[npc.target].position) < 200f)
                        npc.ai[2] -= 1f;

                    if (npc.ai[2] >= num60)
                    {
                        npc.velocity *= 0.95f;
                        if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                            npc.velocity.X = 0f;
                        if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                            npc.velocity.Y = 0f;
                    }
                    else
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

                    float num61 = num60 + 13f;
                    if (npc.ai[2] >= num61)
                    {
                        npc.netUpdate = true;

                        if (npc.netSpam > 10)
                            npc.netSpam = 10;

                        npc.ai[2] = 0f;
                        npc.ai[1] = 0f;
                    }
                }
            }

            return false;
        }
    }
}
