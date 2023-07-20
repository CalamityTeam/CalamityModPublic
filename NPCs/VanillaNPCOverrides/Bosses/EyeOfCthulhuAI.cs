using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class EyeOfCthulhuAI
    {
        public static bool BuffedEyeofCthulhuAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool phase2 = lifeRatio < 0.75f;
            bool phase3 = lifeRatio < 0.65f;
            bool phase4 = lifeRatio < 0.55f;
            bool phase5 = lifeRatio < 0.4f;
            float num5 = death ? 15f : 20f;

            float enrageScale = bossRush ? 1f : 0f;
            if (Main.dayTime || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            npc.reflectsProjectiles = false;

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
                    float hoverSpeed = 7f;
                    float hoverAcceleration = 0.15f;
                    hoverSpeed += 5f * enrageScale;
                    hoverAcceleration += 0.1f * enrageScale;

                    if (death)
                    {
                        hoverSpeed += 8f * (1f - lifeRatio);
                        hoverAcceleration += 0.17f * (1f - lifeRatio);
                    }

                    if (Main.getGoodWorld)
                    {
                        hoverSpeed += 1f;
                        hoverAcceleration += 0.05f;
                    }

                    Vector2 hoverDestination = Main.player[npc.target].Center - Vector2.UnitY * 320f;
                    Vector2 idealVelocity = npc.SafeDirectionTo(hoverDestination) * hoverSpeed;
                    npc.SimpleFlyMovement(idealVelocity, hoverAcceleration);

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
                    else if (npc.WithinRange(hoverDestination, 500f))
                    {
                        if (!Main.player[npc.target].dead)
                            npc.ai[3] += 1f;

                        float servantSpawnGateValue = 40f;
                        if (Main.getGoodWorld)
                            servantSpawnGateValue *= 0.8f;

                        if (npc.ai[3] >= servantSpawnGateValue)
                        {
                            npc.ai[3] = 0f;
                            npc.rotation = num8;

                            Vector2 servantSpawnVelocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * 6f;
                            Vector2 servantSpawnCenter = npc.Center + servantSpawnVelocity * 10f;
                            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(NPCID.ServantofCthulhu) < 12)
                            {
                                int eye = NPC.NewNPC(npc.GetSource_FromAI(), (int)servantSpawnCenter.X, (int)servantSpawnCenter.Y, NPCID.ServantofCthulhu);
                                Main.npc[eye].velocity = servantSpawnVelocity;

                                if (Main.netMode == NetmodeID.Server && eye < Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, eye);
                            }

                            SoundEngine.PlaySound(SoundID.NPCHit1, servantSpawnCenter);

                            int num;
                            for (int m = 0; m < 10; m = num + 1)
                            {
                                Dust.NewDust(servantSpawnCenter, 20, 20, 5, servantSpawnVelocity.X * 0.4f, servantSpawnVelocity.Y * 0.4f, 0, default, 1f);
                                num = m;
                            }
                        }
                    }
                }
                else if (npc.ai[1] == 1f)
                {
                    npc.rotation = num8;
                    float chargeSpeed = 8f;
                    chargeSpeed += 5f * enrageScale;
                    if (death)
                        chargeSpeed += 8.5f * (1f - lifeRatio);
                    if (Main.getGoodWorld)
                        chargeSpeed += 1f;

                    npc.velocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * chargeSpeed;

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
                        if (Main.getGoodWorld)
                            npc.velocity *= 0.99f;

                        if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                            npc.velocity.X = 0f;
                        if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                            npc.velocity.Y = 0f;
                    }
                    else
                        npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;

                    int num28 = 90;
                    if (death)
                        num28 -= (int)Math.Round(120f * (1f - lifeRatio));
                    if (Main.getGoodWorld)
                        num28 -= 15;

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
                if (Main.getGoodWorld)
                    npc.reflectsProjectiles = true;

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
                if (npc.ai[1] % (Main.getGoodWorld ? 10f : 20f) == 0f)
                {
                    Vector2 servantSpawnVelocity = Main.rand.NextVector2CircularEdge(5.65f, 5.65f);
                    if (Main.getGoodWorld)
                        servantSpawnVelocity *= 3f;

                    Vector2 servantSpawnCenter = npc.Center + servantSpawnVelocity * 10f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num34 = NPC.NewNPC(npc.GetSource_FromAI(), (int)servantSpawnCenter.X, (int)servantSpawnCenter.Y, NPCID.ServantofCthulhu, 0, 0f, 0f, 0f, 0f, 255);
                        Main.npc[num34].velocity.X = servantSpawnVelocity.X;
                        Main.npc[num34].velocity.Y = servantSpawnVelocity.Y;

                        if (Main.netMode == NetmodeID.Server && num34 < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num34, 0f, 0f, 0f, 0, 0, 0);

                        if (CalamityWorld.LegendaryMode)
                        {
                            int type = ProjectileID.BloodNautilusShot;
                            Vector2 projectileVelocity = Main.rand.NextVector2CircularEdge(10f, 10f);
                            int numProj = 3;
                            int spread = 20;
                            float rotation = MathHelper.ToRadians(spread);
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(perturbedSpeed) * 10f, perturbedSpeed, type, 15, 0f, Main.myPlayer);
                            }
                        }
                    }

                    int num;
                    for (int n = 0; n < 10; n = num + 1)
                    {
                        Dust.NewDust(servantSpawnCenter, 20, 20, 5, servantSpawnVelocity.X * 0.4f, servantSpawnVelocity.Y * 0.4f, 0, default, 1f);
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
                        SoundEngine.PlaySound(SoundID.NPCHit1, npc.position);

                        if (Main.netMode != NetmodeID.Server)
                        {
                            for (int num35 = 0; num35 < 2; num35++)
                            {
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 8, 1f);
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
                            }
                        }

                        for (int num36 = 0; num36 < 20; num36++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, 5, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
                        }

                        SoundEngine.PlaySound(SoundID.Roar, npc.position);
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
                    float hoverSpeed = 5.5f + 3.5f * (0.75f - lifeRatio);
                    float hoverAcceleration = 0.06f + 0.025f * (0.75f - lifeRatio);
                    hoverSpeed += 4f * enrageScale;
                    hoverAcceleration += 0.04f * enrageScale;

                    if (death)
                    {
                        hoverSpeed += 5f * (0.75f - lifeRatio);
                        hoverAcceleration += 0.04f * (0.75f - lifeRatio);
                    }

                    Vector2 hoverDestination = Main.player[npc.target].Center - Vector2.UnitY * 160f;
                    float distanceFromHoverDestination = npc.Distance(hoverDestination);

                    if (distanceFromHoverDestination > 400f)
                    {
                        hoverSpeed += 1.25f;
                        hoverAcceleration += 0.075f;
                        if (distanceFromHoverDestination > 600f)
                        {
                            hoverSpeed += 1.25f;
                            hoverAcceleration += 0.075f;
                            if (distanceFromHoverDestination > 800f)
                            {
                                hoverSpeed += 1.25f;
                                hoverAcceleration += 0.075f;
                            }
                        }
                    }

                    if (Main.getGoodWorld)
                    {
                        hoverSpeed += 1f;
                        hoverAcceleration += 0.1f;
                    }

                    Vector2 idealHoverVelocity = npc.SafeDirectionTo(hoverDestination) * hoverSpeed;
                    npc.SimpleFlyMovement(idealHoverVelocity, hoverAcceleration);

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
                    SoundEngine.PlaySound(SoundID.ForceRoar, npc.position);
                    npc.rotation = num8;

                    float chargeSpeed = 6.2f + 4f * (0.75f - lifeRatio);
                    chargeSpeed += 4f * enrageScale;
                    if (death)
                        chargeSpeed += 5.5f * (0.75f - lifeRatio);
                    if (npc.ai[3] == 1f)
                        chargeSpeed *= 1.15f;
                    if (npc.ai[3] == 2f)
                        chargeSpeed *= 1.3f;
                    if (Main.getGoodWorld)
                        chargeSpeed *= 1.2f;

                    npc.velocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * chargeSpeed;
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
                        npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;

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
                        SoundEngine.PlaySound(SoundID.ForceRoar, npc.position);

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
                        npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;

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
                    float offset = 600f;
                    float speedBoost = death ? 6f * (0.4f - lifeRatio) : 4f * (0.4f - lifeRatio);
                    float accelerationBoost = death ? 0.15f * (0.4f - lifeRatio) : 0.1f * (0.4f - lifeRatio);
                    float hoverSpeed = 8f + speedBoost;
                    float hoverAcceleration = 0.25f + accelerationBoost;

                    Vector2 vector11 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num65 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector11.X;
                    float num66 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) + offset - vector11.Y;
                    Vector2 hoverDestination = Main.player[npc.target].Center + Vector2.UnitY * offset;

                    bool horizontalCharge = calamityGlobalNPC.newAI[0] == 1f || calamityGlobalNPC.newAI[0] == 3f;
                    if (horizontalCharge)
                    {
                        offset = calamityGlobalNPC.newAI[0] == 1f ? -500f : 500f;
                        hoverSpeed *= 1.5f;
                        hoverAcceleration *= 1.5f;
                        hoverDestination = Main.player[npc.target].Center + Vector2.UnitX * offset;
                    }

                    Vector2 idealHoverVelocity = npc.SafeDirectionTo(hoverDestination) * hoverSpeed;
                    npc.SimpleFlyMovement(idealHoverVelocity, hoverAcceleration);

                    npc.ai[2] += 1f;

                    if (npc.ai[2] % 45f == 0f)
                    {
                        Vector2 servantSpawnVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 5f;
                        Vector2 servantSpawnCenter = npc.Center + servantSpawnVelocity * 10f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int eye = NPC.NewNPC(npc.GetSource_FromAI(), (int)servantSpawnCenter.X, (int)servantSpawnCenter.Y, NPCID.ServantofCthulhu);
                            Main.npc[eye].velocity.X = servantSpawnVelocity.X;
                            Main.npc[eye].velocity.Y = servantSpawnVelocity.Y;

                            if (Main.netMode == NetmodeID.Server && eye < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, eye);

                            if (CalamityWorld.LegendaryMode)
                            {
                                int type = ProjectileID.BloodNautilusShot;
                                Vector2 projectileVelocity = servantSpawnVelocity * 2f;
                                int numProj = 3;
                                int spread = 20;
                                float rotation = MathHelper.ToRadians(spread);
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(perturbedSpeed) * 10f, perturbedSpeed, type, 15, 0f, Main.myPlayer);
                                }
                            }
                        }

                        SoundEngine.PlaySound(SoundID.NPCDeath13, npc.position);

                        for (int m = 0; m < 10; m++)
                            Dust.NewDust(servantSpawnCenter, 20, 20, 5, servantSpawnVelocity.X * 0.4f, servantSpawnVelocity.Y * 0.4f, 0, default, 1f);
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
                        float chargeSpeed = 18f + speedBoost;
                        chargeSpeed += 10f * enrageScale;
                        npc.velocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * chargeSpeed;

                        npc.ai[1] = 7f;
                        npc.netUpdate = true;

                        if (npc.netSpam > 10)
                            npc.netSpam = 10;
                    }
                }

                else if (npc.ai[1] == 7f)
                {
                    if (npc.ai[2] == 0f)
                        SoundEngine.PlaySound(SoundID.Roar, npc.position);

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
                        npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;

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
