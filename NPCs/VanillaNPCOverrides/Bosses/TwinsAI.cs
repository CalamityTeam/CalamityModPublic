using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class TwinsAI
    {
        // Only used for Normal and Expert Mode
        #region True Melee Retinazer Phase 2 AI
        public static bool TrueMeleeRetinazerPhase2AI(NPC npc)
        {
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
            }
            bool dead2 = Main.player[npc.target].dead;
            float num399 = npc.position.X + (npc.width / 2) - Main.player[npc.target].position.X - (Main.player[npc.target].width / 2);
            float num400 = npc.position.Y + npc.height - 59f - Main.player[npc.target].position.Y - (Main.player[npc.target].height / 2);
            float num401 = (float)Math.Atan2(num400, num399) + 1.57f;
            if (num401 < 0f)
            {
                num401 += MathHelper.TwoPi;
            }
            else if (num401 > MathHelper.TwoPi)
            {
                num401 -= MathHelper.TwoPi;
            }
            float num402 = 0.1f;
            if (npc.rotation < num401)
            {
                if ((double)(num401 - npc.rotation) > MathHelper.Pi)
                {
                    npc.rotation -= num402;
                }
                else
                {
                    npc.rotation += num402;
                }
            }
            else if (npc.rotation > num401)
            {
                if ((double)(npc.rotation - num401) > MathHelper.Pi)
                {
                    npc.rotation += num402;
                }
                else
                {
                    npc.rotation -= num402;
                }
            }
            if (npc.rotation > num401 - num402 && npc.rotation < num401 + num402)
            {
                npc.rotation = num401;
            }
            if (npc.rotation < 0f)
            {
                npc.rotation += MathHelper.TwoPi;
            }
            else if (npc.rotation > MathHelper.TwoPi)
            {
                npc.rotation -= MathHelper.TwoPi;
            }
            if (npc.rotation > num401 - num402 && npc.rotation < num401 + num402)
            {
                npc.rotation = num401;
            }
            if (Main.rand.Next(5) == 0)
            {
                int num403 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f), npc.width, (int)(npc.height * 0.5f), 5, npc.velocity.X, 2f);
                Main.dust[num403].velocity.X *= 0.5f;
                Main.dust[num403].velocity.Y *= 0.1f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && !Main.dayTime && !Main.player[npc.target].dead && npc.timeLeft < 10)
            {
                for (int num381 = 0; num381 < Main.maxNPCs; num381++)
                {
                    if (num381 != npc.whoAmI && Main.npc[num381].active && (Main.npc[num381].type == NPCID.Retinazer || Main.npc[num381].type == NPCID.Spazmatism) && Main.npc[num381].timeLeft - 1 > npc.timeLeft)
                        npc.timeLeft = Main.npc[num381].timeLeft - 1;

                }
            }
            if (Main.dayTime | Main.player[npc.target].dead)
            {
                npc.velocity.Y -= 0.04f;
                if (npc.timeLeft > 10)
                    npc.timeLeft = 10;
                return false;
            }
            if (npc.ai[0] == 3f)
            {
                npc.damage = (int)(npc.defDamage * 1.5);
                npc.defense = npc.defDefense + 10;
                npc.HitSound = SoundID.NPCHit4;
                if (npc.ai[1] == 0f)
                {
                    float num421 = 8f;
                    float num422 = 0.15f;
                    if (Main.expertMode)
                    {
                        num421 = 9.5f;
                        num422 = 0.175f;
                    }
                    // Reduce acceleration if target is holding a true melee weapon
                    if (Main.player[npc.target].HoldingTrueMeleeWeapon())
                    {
                        num421 *= 0.75f;
                        num422 *= 0.5f;
                    }
                    Vector2 vector39 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num423 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector39.X;
                    float num424 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 300f - vector39.Y;
                    float num425 = (float)Math.Sqrt(num423 * num423 + num424 * num424);
                    num425 = num421 / num425;
                    num423 *= num425;
                    num424 *= num425;
                    if (npc.velocity.X < num423)
                    {
                        npc.velocity.X += num422;
                        if (npc.velocity.X < 0f && num423 > 0f)
                        {
                            npc.velocity.X += num422;
                        }
                    }
                    else if (npc.velocity.X > num423)
                    {
                        npc.velocity.X -= num422;
                        if (npc.velocity.X > 0f && num423 < 0f)
                        {
                            npc.velocity.X -= num422;
                        }
                    }
                    if (npc.velocity.Y < num424)
                    {
                        npc.velocity.Y += num422;
                        if (npc.velocity.Y < 0f && num424 > 0f)
                        {
                            npc.velocity.Y += num422;
                        }
                    }
                    else if (npc.velocity.Y > num424)
                    {
                        npc.velocity.Y -= num422;
                        if (npc.velocity.Y > 0f && num424 < 0f)
                        {
                            npc.velocity.Y -= num422;
                        }
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 300f)
                    {
                        npc.ai[1] = 1f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }
                    vector39 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    num423 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector39.X;
                    num424 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector39.Y;
                    npc.rotation = (float)Math.Atan2(num424, num423) - 1.57f;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        return false;
                    }
                    npc.localAI[1] += 1f;
                    if (npc.life < npc.lifeMax * 0.75)
                    {
                        npc.localAI[1] += 1f;
                    }
                    if (npc.life < npc.lifeMax * 0.5)
                    {
                        npc.localAI[1] += 1f;
                    }
                    if (npc.life < npc.lifeMax * 0.25)
                    {
                        npc.localAI[1] += 1f;
                    }
                    if (npc.life < npc.lifeMax * 0.1)
                    {
                        npc.localAI[1] += 2f;
                    }
                    if (npc.localAI[1] > 180f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        npc.localAI[1] = 0f;
                        float num426 = 8.5f;
                        int attackDamage_ForProjectiles4 = Main.expertMode ? 23 : 25;
                        int num427 = 100;
                        if (Main.expertMode)
                        {
                            num426 = 10f;
                        }
                        num425 = (float)Math.Sqrt(num423 * num423 + num424 * num424);
                        num425 = num426 / num425;
                        num423 *= num425;
                        num424 *= num425;
                        vector39.X += num423 * 15f;
                        vector39.Y += num424 * 15f;
                        int num428 = Projectile.NewProjectile(npc.GetSource_FromAI(), vector39.X, vector39.Y, num423, num424, num427, attackDamage_ForProjectiles4, 0f, Main.myPlayer);
                    }
                    return false;
                }
                int num429 = 1;
                if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
                {
                    num429 = -1;
                }
                float num430 = 8f;
                float num431 = 0.2f;
                if (Main.expertMode)
                {
                    num430 = 9.5f;
                    num431 = 0.25f;
                }
                // Reduce acceleration if target is holding a true melee weapon
                if (Main.player[npc.target].HoldingTrueMeleeWeapon())
                {
                    num430 *= 0.75f;
                    num431 *= 0.5f;
                }
                Vector2 vector40 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num432 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (num429 * 340) - vector40.X;
                float num433 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector40.Y;
                float num434 = (float)Math.Sqrt(num432 * num432 + num433 * num433);
                num434 = num430 / num434;
                num432 *= num434;
                num433 *= num434;
                if (npc.velocity.X < num432)
                {
                    npc.velocity.X += num431;
                    if (npc.velocity.X < 0f && num432 > 0f)
                    {
                        npc.velocity.X += num431;
                    }
                }
                else if (npc.velocity.X > num432)
                {
                    npc.velocity.X -= num431;
                    if (npc.velocity.X > 0f && num432 < 0f)
                    {
                        npc.velocity.X -= num431;
                    }
                }
                if (npc.velocity.Y < num433)
                {
                    npc.velocity.Y += num431;
                    if (npc.velocity.Y < 0f && num433 > 0f)
                    {
                        npc.velocity.Y += num431;
                    }
                }
                else if (npc.velocity.Y > num433)
                {
                    npc.velocity.Y -= num431;
                    if (npc.velocity.Y > 0f && num433 < 0f)
                    {
                        npc.velocity.Y -= num431;
                    }
                }
                vector40 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                num432 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector40.X;
                num433 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector40.Y;
                npc.rotation = (float)Math.Atan2(num433, num432) - 1.57f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[1] += 1f;
                    if (npc.life < npc.lifeMax * 0.75)
                    {
                        npc.localAI[1] += 0.5f;
                    }
                    if (npc.life < npc.lifeMax * 0.5)
                    {
                        npc.localAI[1] += 0.75f;
                    }
                    if (npc.life < npc.lifeMax * 0.25)
                    {
                        npc.localAI[1] += 1f;
                    }
                    if (npc.life < npc.lifeMax * 0.1)
                    {
                        npc.localAI[1] += 1.5f;
                    }
                    if (Main.expertMode)
                    {
                        npc.localAI[1] += 1.5f;
                    }
                    if (npc.localAI[1] > 60f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        npc.localAI[1] = 0f;
                        float num435 = 9f;
                        int attackDamage_ForProjectiles5 = Main.expertMode ? 17 : 18;
                        int num436 = 100;
                        num434 = (float)Math.Sqrt(num432 * num432 + num433 * num433);
                        num434 = num435 / num434;
                        num432 *= num434;
                        num433 *= num434;
                        vector40.X += num432 * 15f;
                        vector40.Y += num433 * 15f;
                        int num437 = Projectile.NewProjectile(npc.GetSource_FromAI(), vector40.X, vector40.Y, num432, num433, num436, attackDamage_ForProjectiles5, 0f, Main.myPlayer);
                    }
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= 180f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }

                return false;
            }

            return true;
        }
        #endregion

        #region Twins AI
        public static bool BuffedRetinazerAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            float enrageScale = BossRushEvent.BossRushActive ? 0.5f : 0f;
            if (Main.dayTime || malice)
            {
                npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
                enrageScale += 1f;
            }

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Easier to send info to Spazmatism
            CalamityGlobalNPC.laserEye = npc.whoAmI;

            // Check for Spazmatism
            bool spazAlive = false;
            if (CalamityGlobalNPC.fireEye != -1)
                spazAlive = Main.npc[CalamityGlobalNPC.fireEye].active;

            bool enrage = lifeRatio < 0.25f;

            // I'm not commenting this entire fucking thing, already did spaz, I'm not doing ret
            float num376 = npc.position.X + (npc.width / 2) - Main.player[npc.target].position.X - (Main.player[npc.target].width / 2);
            float num377 = npc.position.Y + npc.height - 59f - Main.player[npc.target].position.Y - (Main.player[npc.target].height / 2);

            float num378 = (float)Math.Atan2(num377, num376) + MathHelper.PiOver2;
            if (num378 < 0f)
                num378 += MathHelper.TwoPi;
            else if (num378 > MathHelper.TwoPi)
                num378 -= MathHelper.TwoPi;

            float num379 = 0.1f;
            if (npc.rotation < num378)
            {
                if ((num378 - npc.rotation) > MathHelper.Pi)
                    npc.rotation -= num379;
                else
                    npc.rotation += num379;
            }
            else if (npc.rotation > num378)
            {
                if ((npc.rotation - num378) > MathHelper.Pi)
                    npc.rotation += num379;
                else
                    npc.rotation -= num379;
            }

            if (npc.rotation > num378 - num379 && npc.rotation < num378 + num379)
                npc.rotation = num378;

            if (npc.rotation < 0f)
                npc.rotation += MathHelper.TwoPi;
            else if (npc.rotation > MathHelper.TwoPi)
                npc.rotation -= MathHelper.TwoPi;

            if (npc.rotation > num378 - num379 && npc.rotation < num378 + num379)
                npc.rotation = num378;

            if (Main.rand.NextBool(5))
            {
                int num380 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f), npc.width, (int)(npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default, 1f);
                Dust dust = Main.dust[num380];
                dust.velocity.X *= 0.5f;
                dust.velocity.Y *= 0.1f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && !Main.player[npc.target].dead && npc.timeLeft < 10)
            {
                for (int num381 = 0; num381 < Main.maxNPCs; num381++)
                {
                    if (num381 != npc.whoAmI && Main.npc[num381].active && (Main.npc[num381].type == NPCID.Retinazer || Main.npc[num381].type == NPCID.Spazmatism) && Main.npc[num381].timeLeft - 1 > npc.timeLeft)
                        npc.timeLeft = Main.npc[num381].timeLeft - 1;

                }
            }

            if (Main.player[npc.target].dead)
            {
                npc.velocity.Y -= 0.04f;
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                    return false;
                }
            }

            else if (npc.ai[0] == 0f)
            {
                if (npc.ai[1] == 0f)
                {
                    float num382 = 8.25f;
                    float num383 = 0.115f;
                    num382 += 4f * enrageScale;
                    num383 += 0.05f * enrageScale;

                    if (death)
                    {
                        num382 += 8f * (1f - lifeRatio);
                        num383 += 0.1f * (1f - lifeRatio);
                    }

                    int num384 = 1;
                    if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
                        num384 = -1;

                    Vector2 vector40 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num385 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (num384 * 300) - vector40.X;
                    float num386 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 300f - vector40.Y;
                    float num387 = (float)Math.Sqrt(num385 * num385 + num386 * num386);
                    float num388 = num387;

                    num387 = num382 / num387;
                    num385 *= num387;
                    num386 *= num387;

                    if (npc.velocity.X < num385)
                    {
                        npc.velocity.X += num383;
                        if (npc.velocity.X < 0f && num385 > 0f)
                            npc.velocity.X += num383;
                    }
                    else if (npc.velocity.X > num385)
                    {
                        npc.velocity.X -= num383;
                        if (npc.velocity.X > 0f && num385 < 0f)
                            npc.velocity.X -= num383;
                    }
                    if (npc.velocity.Y < num386)
                    {
                        npc.velocity.Y += num383;
                        if (npc.velocity.Y < 0f && num386 > 0f)
                            npc.velocity.Y += num383;
                    }
                    else if (npc.velocity.Y > num386)
                    {
                        npc.velocity.Y -= num383;
                        if (npc.velocity.Y > 0f && num386 < 0f)
                            npc.velocity.Y -= num383;
                    }

                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 450f - (death ? 900f * (1f - lifeRatio) : 0f))
                    {
                        npc.ai[1] = 1f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }

                    else if (npc.position.Y + npc.height < Main.player[npc.target].position.Y && num388 < 400f)
                    {
                        if (!Main.player[npc.target].dead)
                            npc.ai[3] += 1f;

                        if (npc.ai[3] >= 30f)
                        {
                            npc.ai[3] = 0f;
                            vector40 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                            num385 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector40.X;
                            num386 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector40.Y;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float num389 = 10.5f;
                                num389 += 3f * enrageScale;
                                int type = ProjectileID.EyeLaser;
                                int damage = npc.GetProjectileDamage(type);

                                num387 = (float)Math.Sqrt(num385 * num385 + num386 * num386);
                                num387 = num389 / num387;
                                num385 *= num387;
                                num386 *= num387;
                                vector40.X += num385 * 15f;
                                vector40.Y += num386 * 15f;

                                Projectile.NewProjectile(npc.GetSource_FromAI(), vector40.X, vector40.Y, num385, num386, type, damage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }

                else if (npc.ai[1] == 1f)
                {
                    npc.rotation = num378;
                    float num393 = 15f;
                    num393 += 10f * enrageScale;
                    if (death)
                        num393 += 12f * (1f - lifeRatio);

                    Vector2 vector41 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num394 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector41.X;
                    float num395 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector41.Y;
                    float num396 = (float)Math.Sqrt(num394 * num394 + num395 * num395);
                    num396 = num393 / num396;
                    npc.velocity.X = num394 * num396;
                    npc.velocity.Y = num395 * num396;
                    npc.ai[1] = 2f;
                }
                else if (npc.ai[1] == 2f)
                {
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 25f)
                    {
                        npc.velocity *= 0.96f;
                        if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                        {
                            npc.velocity.X = 0f;
                        }
                        if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                        {
                            npc.velocity.Y = 0f;
                        }
                    }
                    else
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

                    if (npc.ai[2] >= 70f)
                    {
                        npc.ai[3] += 1f;
                        npc.ai[2] = 0f;
                        npc.TargetClosest();
                        npc.rotation = num378;
                        if (npc.ai[3] >= 3f)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[3] = 0f;
                        }
                        else
                            npc.ai[1] = 1f;
                    }
                }

                // Enter phase 2 earlier
                if (lifeRatio < 0.7f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            else if (npc.ai[0] == 1f || npc.ai[0] == 2f)
            {
                if (npc.ai[0] == 1f)
                {
                    npc.ai[2] += 0.005f;
                    if (npc.ai[2] > 0.5)
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
                if (npc.ai[1] == 100f)
                {
                    npc.ai[0] += 1f;
                    npc.ai[1] = 0f;
                    if (npc.ai[0] == 3f)
                        npc.ai[2] = 0f;
                    else
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit1, (int)npc.position.X, (int)npc.position.Y);

                        if (Main.netMode != NetmodeID.Server)
                        {
                            for (int num397 = 0; num397 < 2; num397++)
                            {
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 143, 1f);
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
                            }
                        }

                        for (int num398 = 0; num398 < 20; num398++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, 5, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
                        }

                        SoundEngine.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
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
                // If in phase 2 but Spaz isn't
                bool spazInPhase1 = false;
                if (CalamityGlobalNPC.fireEye != -1)
                {
                    if (Main.npc[CalamityGlobalNPC.fireEye].active)
                        spazInPhase1 = Main.npc[CalamityGlobalNPC.fireEye].ai[0] == 1f || Main.npc[CalamityGlobalNPC.fireEye].ai[0] == 2f || Main.npc[CalamityGlobalNPC.fireEye].ai[0] == 0f;
                }

                npc.chaseable = !spazInPhase1;

                npc.damage = (int)(npc.defDamage * 1.5);
                npc.defense = npc.defDefense + 10;
                calamityGlobalNPC.DR = spazInPhase1 ? 0.9999f : 0.2f;
                calamityGlobalNPC.unbreakableDR = spazInPhase1;
                calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = spazInPhase1;

                npc.HitSound = SoundID.NPCHit4;

                if (npc.ai[1] == 0f)
                {
                    float num399 = 9.5f + (death ? 3f * (0.7f - lifeRatio) : 0f);
                    float num400 = 0.175f + (death ? 0.05f * (0.7f - lifeRatio) : 0f);
                    num399 += 4.5f * enrageScale;
                    num400 += 0.075f * enrageScale;

                    // Reduce acceleration if target is holding a true melee weapon
                    if (Main.player[npc.target].HoldingTrueMeleeWeapon())
                    {
                        num399 *= 0.75f;
                        num400 *= 0.5f;
                    }

                    Vector2 vector42 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num401 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector42.X;
                    float num402 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 300f - vector42.Y;
                    float num403 = (float)Math.Sqrt(num401 * num401 + num402 * num402);
                    num403 = num399 / num403;
                    num401 *= num403;
                    num402 *= num403;

                    if (npc.velocity.X < num401)
                    {
                        npc.velocity.X += num400;
                        if (npc.velocity.X < 0f && num401 > 0f)
                            npc.velocity.X += num400;
                    }
                    else if (npc.velocity.X > num401)
                    {
                        npc.velocity.X -= num400;
                        if (npc.velocity.X > 0f && num401 < 0f)
                            npc.velocity.X -= num400;
                    }
                    if (npc.velocity.Y < num402)
                    {
                        npc.velocity.Y += num400;
                        if (npc.velocity.Y < 0f && num402 > 0f)
                            npc.velocity.Y += num400;
                    }
                    else if (npc.velocity.Y > num402)
                    {
                        npc.velocity.Y -= num400;
                        if (npc.velocity.Y > 0f && num402 < 0f)
                            npc.velocity.Y -= num400;
                    }

                    npc.ai[2] += spazAlive ? 1f : 1.25f;
                    if (npc.ai[2] >= 300f - (death ? 120f * (0.7f - lifeRatio) : 0f))
                    {
                        npc.ai[1] = 1f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }

                    vector42 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    num401 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector42.X;
                    num402 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector42.Y;
                    npc.rotation = (float)Math.Atan2(num402, num401) - MathHelper.PiOver2;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[1] += 1f + (death ? 0.7f - lifeRatio : 0f);
                        if (npc.localAI[1] >= (spazAlive ? 72f : 24f))
                        {
                            bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                            if (canHit || !spazAlive || enrage)
                            {
                                npc.localAI[1] = 0f;
                                float num404 = 10f;
                                num404 += enrageScale;
                                int type = ProjectileID.DeathLaser;
                                int damage = npc.GetProjectileDamage(type);

                                num403 = (float)Math.Sqrt(num401 * num401 + num402 * num402);
                                num403 = num404 / num403;
                                num401 *= num403;
                                num402 *= num403;
                                vector42.X += num401 * 15f;
                                vector42.Y += num402 * 15f;

                                if (canHit)
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), vector42.X, vector42.Y, num401, num402, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                else
                                {
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), vector42.X, vector42.Y, num401, num402, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                    Main.projectile[proj].tileCollide = false;
                                    Main.projectile[proj].timeLeft = 300;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (npc.ai[1] == 1f)
                    {
                        int num408 = 1;
                        if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
                            num408 = -1;

                        float num409 = 9.5f + (death ? 3f * (0.7f - lifeRatio) : 0f);
                        float num410 = 0.25f + (death ? 0.075f * (0.7f - lifeRatio) : 0f);
                        num409 += 4.5f * enrageScale;
                        num410 += 0.15f * enrageScale;

                        // Reduce acceleration if target is holding a true melee weapon
                        if (Main.player[npc.target].HoldingTrueMeleeWeapon())
                        {
                            num409 *= 0.75f;
                            num410 *= 0.5f;
                        }

                        Vector2 vector43 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float num411 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (num408 * 340) - vector43.X;
                        float num412 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector43.Y;
                        float num413 = (float)Math.Sqrt(num411 * num411 + num412 * num412);
                        num413 = num409 / num413;
                        num411 *= num413;
                        num412 *= num413;

                        if (npc.velocity.X < num411)
                        {
                            npc.velocity.X += num410;
                            if (npc.velocity.X < 0f && num411 > 0f)
                                npc.velocity.X += num410;
                        }
                        else if (npc.velocity.X > num411)
                        {
                            npc.velocity.X -= num410;
                            if (npc.velocity.X > 0f && num411 < 0f)
                                npc.velocity.X -= num410;
                        }
                        if (npc.velocity.Y < num412)
                        {
                            npc.velocity.Y += num410;
                            if (npc.velocity.Y < 0f && num412 > 0f)
                                npc.velocity.Y += num410;
                        }
                        else if (npc.velocity.Y > num412)
                        {
                            npc.velocity.Y -= num410;
                            if (npc.velocity.Y > 0f && num412 < 0f)
                                npc.velocity.Y -= num410;
                        }

                        vector43 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        num411 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector43.X;
                        num412 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector43.Y;
                        npc.rotation = (float)Math.Atan2(num412, num411) - MathHelper.PiOver2;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.localAI[1] += 1f + (death ? 0.7f - lifeRatio : 0f);
                            if (npc.localAI[1] > (spazAlive ? 24f : 8f))
                            {
                                bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                                if (canHit || !spazAlive || enrage)
                                {
                                    npc.localAI[1] = 0f;
                                    float num414 = 9f;
                                    int type = ProjectileID.DeathLaser;
                                    int damage = (int)Math.Round(npc.GetProjectileDamage(type) * 0.75);

                                    num413 = (float)Math.Sqrt(num411 * num411 + num412 * num412);
                                    num413 = num414 / num413;
                                    num411 *= num413;
                                    num412 *= num413;
                                    vector43.X += num411 * 15f;
                                    vector43.Y += num412 * 15f;

                                    if (canHit)
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector43.X, vector43.Y, num411, num412, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                    else
                                    {
                                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), vector43.X, vector43.Y, num411, num412, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                        Main.projectile[proj].tileCollide = false;
                                        Main.projectile[proj].timeLeft = 300;
                                    }
                                }
                            }
                        }

                        npc.ai[2] += spazAlive ? 1f : 1.5f;
                        if (npc.ai[2] >= 180f - (death ? 90f * (0.7f - lifeRatio) : 0f))
                        {
                            npc.ai[1] = (!spazAlive || enrage) ? 4f : 0f;
                            npc.ai[2] = 0f;
                            npc.ai[3] = 0f;
                            npc.TargetClosest();
                            npc.netUpdate = true;
                        }
                    }

                    // Charge
                    else if (npc.ai[1] == 2f)
                    {
                        // Set rotation and velocity
                        npc.rotation = num378;
                        float num450 = 22f + (death ? 8f * (0.7f - lifeRatio) : 0f);
                        num450 += 10f * enrageScale;

                        if (!spazAlive)
                            num450 += 2f;

                        Vector2 vector47 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float num451 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector47.X;
                        float num452 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector47.Y;
                        float num453 = (float)Math.Sqrt(num451 * num451 + num452 * num452);
                        num453 = num450 / num453;
                        npc.velocity.X = num451 * num453;
                        npc.velocity.Y = num452 * num453;
                        npc.ai[1] = 3f;
                    }

                    else if (npc.ai[1] == 3f)
                    {
                        npc.ai[2] += 1f;

                        float chargeTime = spazAlive ? 45f : 30f;
                        if (npc.ai[3] % 3f == 0f)
                            chargeTime = spazAlive ? 90f : 60f;
                        if (death)
                            chargeTime -= chargeTime * 0.25f * (0.7f - lifeRatio);
                        chargeTime -= chargeTime / 5 * enrageScale;

                        // Slow down
                        if (npc.ai[2] >= chargeTime)
                        {
                            npc.velocity *= 0.93f;
                            if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                                npc.velocity.X = 0f;
                            if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                                npc.velocity.Y = 0f;
                        }
                        else
                        {
                            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

                            if (npc.ai[3] % 3f == 0f)
                            {
                                float fireRate = spazAlive ? 13f : 9f;

                                if (npc.ai[2] % fireRate == 0f)
                                {
                                    Vector2 vector34 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                                    float num349 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector34.X;
                                    float num350 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector34.Y;
                                    float num351 = (float)Math.Sqrt(num349 * num349 + num350 * num350);

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        SoundEngine.PlaySound(SoundID.Item33, npc.position);

                                        float num353 = 6f;
                                        int type = ModContent.ProjectileType<ScavengerLaser>();
                                        int damage = npc.GetProjectileDamage(type);

                                        vector34 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                                        num349 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector34.X;
                                        num350 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector34.Y;
                                        num351 = (float)Math.Sqrt(num349 * num349 + num350 * num350);
                                        num351 = num353 / num351;
                                        num349 *= num351;
                                        num350 *= num351;
                                        vector34.X += num349;
                                        vector34.Y += num350;

                                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector34.X, vector34.Y, num349, num350, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                    }
                                }
                            }
                        }

                        // Charge four times
                        float chargeGateValue = 30f;
                        chargeGateValue -= chargeGateValue / 4 * enrageScale;
                        if (npc.ai[2] >= chargeTime + 30f)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[3] += 1f;
                            npc.TargetClosest();
                            npc.rotation = num378;
                            float maxChargeAmt = spazAlive ? 2f : 4f;
                            if (npc.ai[3] >= maxChargeAmt)
                            {
                                npc.ai[1] = 0f;
                                npc.ai[3] = 0f;
                            }
                            else
                                npc.ai[1] = 4f;
                        }
                    }

                    // Get in position for charge
                    else if (npc.ai[1] == 4f)
                    {
                        int num62 = spazAlive ? 600 : 500;
                        float num63 = 12f + (death ? 4f * (0.7f - lifeRatio) : 0f);
                        float num64 = 0.3f + (death ? 0.1f * (0.7f - lifeRatio) : 0f);

                        if (spazAlive)
                        {
                            num63 *= 0.75f;
                            num64 *= 0.75f;
                        }

                        int num408 = 1;
                        if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
                            num408 = -1;

                        Vector2 vector11 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float num65 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (num62 * num408) - vector11.X;
                        float num66 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector11.Y;
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

                        // Take 1.25 or 1 second to get in position, then charge
                        npc.ai[2] += 1f;
                        if (npc.ai[2] >= (spazAlive ? 75f : 60f) - (death ? 20f * (0.7f - lifeRatio) : 0f))
                        {
                            npc.TargetClosest();
                            npc.ai[1] = 2f;
                            npc.ai[2] = 0f;
                            npc.netUpdate = true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool BuffedSpazmatismAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            float enrageScale = BossRushEvent.BossRushActive ? 0.5f : 0f;
            if (Main.dayTime || malice)
            {
                npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
                enrageScale += 1f;
            }

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Easier to send info to Retinazer
            CalamityGlobalNPC.fireEye = npc.whoAmI;

            // Check for Retinazer
            bool retAlive = false;
            if (CalamityGlobalNPC.laserEye != -1)
                retAlive = Main.npc[CalamityGlobalNPC.laserEye].active;

            bool enrage = lifeRatio < 0.25f;

            // Rotation
            Vector2 npcCenter = new Vector2(npc.Center.X, npc.position.Y + npc.height - 59f);
            Vector2 lookAt = new Vector2(Main.player[npc.target].position.X - (Main.player[npc.target].width / 2), Main.player[npc.target].position.Y - (Main.player[npc.target].height / 2));
            Vector2 rotationVector = npcCenter - lookAt;

            // Malice Mode predictive charge rotation
            if (npc.ai[1] == 5f && !retAlive && malice)
            {
                // Velocity
                float chargeVelocity = 20f + (death ? 6f * (0.7f - lifeRatio) : 0f);
                chargeVelocity += 10f * enrageScale;
                if (npc.ai[2] == -1f || (!retAlive && npc.ai[3] == 4f))
                    chargeVelocity *= 1.3f;

                lookAt += Main.player[npc.target].velocity * 20f;
                rotationVector = Vector2.Normalize(npcCenter - lookAt) * chargeVelocity;
            }

            float num420 = (float)Math.Atan2(rotationVector.Y, rotationVector.X) + MathHelper.PiOver2;
            if (num420 < 0f)
                num420 += MathHelper.TwoPi;
            else if (num420 > MathHelper.TwoPi)
                num420 -= MathHelper.TwoPi;

            float num421 = 0.15f;
            if (npc.rotation < num420)
            {
                if ((num420 - npc.rotation) > MathHelper.Pi)
                    npc.rotation -= num421;
                else
                    npc.rotation += num421;
            }
            else if (npc.rotation > num420)
            {
                if ((npc.rotation - num420) > MathHelper.Pi)
                    npc.rotation += num421;
                else
                    npc.rotation -= num421;
            }

            if (npc.rotation > num420 - num421 && npc.rotation < num420 + num421)
                npc.rotation = num420;

            if (npc.rotation < 0f)
                npc.rotation += MathHelper.TwoPi;
            else if (npc.rotation > MathHelper.TwoPi)
                npc.rotation -= MathHelper.TwoPi;

            if (npc.rotation > num420 - num421 && npc.rotation < num420 + num421)
                npc.rotation = num420;

            // Dust
            if (Main.rand.NextBool(5))
            {
                int num422 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f), npc.width, (int)(npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default, 1f);
                Dust dust = Main.dust[num422];
                dust.velocity.X *= 0.5f;
                dust.velocity.Y *= 0.1f;
            }

            // Despawn Twins at the same time
            if (Main.netMode != NetmodeID.MultiplayerClient && !Main.player[npc.target].dead && npc.timeLeft < 10)
            {
                for (int num423 = 0; num423 < Main.maxNPCs; num423++)
                {
                    if (num423 != npc.whoAmI && Main.npc[num423].active && (Main.npc[num423].type == NPCID.Retinazer || Main.npc[num423].type == NPCID.Spazmatism) && Main.npc[num423].timeLeft - 1 > npc.timeLeft)
                        npc.timeLeft = Main.npc[num423].timeLeft - 1;

                }
            }

            // Despawn
            if (Main.player[npc.target].dead)
            {
                npc.velocity.Y -= 0.04f;
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                    return false;
                }
            }

            // Phase 1
            else if (npc.ai[0] == 0f)
            {
                // Cursed fireball phase
                if (npc.ai[1] == 0f)
                {
                    // Velocity
                    float num424 = 12f;
                    float num425 = 0.4f;
                    num424 += 6f * enrageScale;
                    num425 += 0.2f * enrageScale;

                    if (death)
                    {
                        num424 += 9f * (1f - lifeRatio);
                        num425 += 0.3f * (1f - lifeRatio);
                    }

                    int num426 = 1;
                    if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
                        num426 = -1;

                    Vector2 vector44 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num427 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (num426 * 400) - vector44.X;
                    float num428 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector44.Y;
                    float num429 = (float)Math.Sqrt(num427 * num427 + num428 * num428);

                    num429 = num424 / num429;
                    num427 *= num429;
                    num428 *= num429;

                    if (npc.velocity.X < num427)
                    {
                        npc.velocity.X += num425;
                        if (npc.velocity.X < 0f && num427 > 0f)
                            npc.velocity.X += num425;
                    }
                    else if (npc.velocity.X > num427)
                    {
                        npc.velocity.X -= num425;
                        if (npc.velocity.X > 0f && num427 < 0f)
                            npc.velocity.X -= num425;
                    }
                    if (npc.velocity.Y < num428)
                    {
                        npc.velocity.Y += num425;
                        if (npc.velocity.Y < 0f && num428 > 0f)
                            npc.velocity.Y += num425;
                    }
                    else if (npc.velocity.Y > num428)
                    {
                        npc.velocity.Y -= num425;
                        if (npc.velocity.Y > 0f && num428 < 0f)
                            npc.velocity.Y -= num425;
                    }

                    // Fire cursed flames for 5 seconds
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 300f - (death ? 600f * (1f - lifeRatio) : 0f))
                    {
                        // Reset AI array and go to charging phase
                        npc.ai[1] = 1f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }
                    else
                    {
                        // Fire cursed flame every half second
                        if (!Main.player[npc.target].dead)
                            npc.ai[3] += 1f;

                        if (npc.ai[3] >= 30f)
                        {
                            npc.ai[3] = 0f;
                            vector44 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                            num427 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector44.X;
                            num428 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector44.Y;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float num430 = 15f;
                                num430 += 3f * enrageScale;
                                int type = ProjectileID.CursedFlameHostile;
                                int damage = npc.GetProjectileDamage(type);

                                num429 = (float)Math.Sqrt(num427 * num427 + num428 * num428);
                                num429 = num430 / num429;
                                num427 *= num429;
                                num428 *= num429;
                                num427 += Main.rand.Next(-10, 11) * 0.05f;
                                num428 += Main.rand.Next(-10, 11) * 0.05f;
                                vector44.X += num427 * 4f;
                                vector44.Y += num428 * 4f;

                                Projectile.NewProjectile(npc.GetSource_FromAI(), vector44.X, vector44.Y, num427, num428, type, damage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }

                // Charging phase
                else if (npc.ai[1] == 1f)
                {
                    // Rotation and velocity
                    npc.rotation = num420;
                    float num434 = 16f;
                    num434 += 8f * enrageScale;
                    if (death)
                        num434 += 12f * (1f - lifeRatio);

                    Vector2 vector45 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num435 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector45.X;
                    float num436 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector45.Y;
                    float num437 = (float)Math.Sqrt(num435 * num435 + num436 * num436);
                    num437 = num434 / num437;
                    npc.velocity.X = num435 * num437;
                    npc.velocity.Y = num436 * num437;
                    npc.ai[1] = 2f;
                }
                else if (npc.ai[1] == 2f)
                {
                    npc.ai[2] += 1f;

                    float timeBeforeSlowDown = 12f;
                    if (npc.ai[2] >= timeBeforeSlowDown)
                    {
                        // Slow down
                        npc.velocity *= 0.9f;

                        if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                            npc.velocity.X = 0f;
                        if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                            npc.velocity.Y = 0f;
                    }
                    else
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

                    // Charge 8 times
                    float chargeTime = 38f;
                    if (npc.ai[2] >= chargeTime)
                    {
                        // Reset AI array and go to cursed fireball phase
                        npc.ai[3] += 1f;
                        npc.ai[2] = 0f;
                        npc.TargetClosest();
                        npc.rotation = num420;
                        float totalCharges = 8f;
                        if (death)
                            totalCharges -= (float)Math.Round(16f * (1f - lifeRatio));

                        if (npc.ai[3] >= totalCharges)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[3] = 0f;
                        }
                        else
                            npc.ai[1] = 1f;
                    }
                }

                // Enter phase 2 earlier
                if (lifeRatio < 0.7f)
                {
                    // Reset AI array and go to transition phase
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Transition phase
            else if (npc.ai[0] == 1f || npc.ai[0] == 2f)
            {
                // AI timer for rotation
                if (npc.ai[0] == 1f)
                {
                    npc.ai[2] += 0.005f;
                    if (npc.ai[2] > 0.5)
                        npc.ai[2] = 0.5f;
                }
                else
                {
                    npc.ai[2] -= 0.005f;
                    if (npc.ai[2] < 0f)
                        npc.ai[2] = 0f;
                }

                // Spin around like a moron while flinging blood and gore everywhere
                npc.rotation += npc.ai[2];
                npc.ai[1] += 1f;
                if (npc.ai[1] == 100f)
                {
                    npc.ai[0] += 1f;
                    npc.ai[1] = 0f;

                    if (npc.ai[0] == 3f)
                        npc.ai[2] = 0f;
                    else
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);

                        if (Main.netMode != NetmodeID.Server)
                        {
                            for (int num438 = 0; num438 < 2; num438++)
                            {
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 144, 1f);
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
                            }
                        }

                        for (int num439 = 0; num439 < 20; num439++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, 5, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
                        }

                        SoundEngine.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
                    }
                }

                Dust.NewDust(npc.position, npc.width, npc.height, 5, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

                npc.velocity *= 0.98f;

                if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                    npc.velocity.X = 0f;
                if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                    npc.velocity.Y = 0f;
            }

            // Phase 2
            else
            {
                // If in phase 2 but Ret isn't
                bool retInPhase1 = false;
                if (CalamityGlobalNPC.laserEye != -1)
                {
                    if (Main.npc[CalamityGlobalNPC.laserEye].active)
                        retInPhase1 = Main.npc[CalamityGlobalNPC.laserEye].ai[0] == 1f || Main.npc[CalamityGlobalNPC.laserEye].ai[0] == 2f || Main.npc[CalamityGlobalNPC.laserEye].ai[0] == 0f;
                }

                npc.chaseable = !retInPhase1;

                // Increase defense and damage
                npc.damage = (int)(npc.defDamage * 1.5);
                npc.defense = npc.defDefense + 18;
                calamityGlobalNPC.DR = retInPhase1 ? 0.9999f : 0.2f;
                calamityGlobalNPC.unbreakableDR = retInPhase1;
                calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = retInPhase1;

                // Change hit sound to metal
                npc.HitSound = SoundID.NPCHit4;

                // Shadowflamethrower phase
                if (npc.ai[1] == 0f)
                {
                    float num440 = 6.2f + (death ? 2f * (0.7f - lifeRatio) : 0f);
                    float num441 = 0.1f + (death ? 0.03f * (0.7f - lifeRatio) : 0f);
                    num440 += 4f * enrageScale;
                    num441 += 0.06f * enrageScale;

                    int num442 = 1;
                    if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
                        num442 = -1;

                    Vector2 vector46 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num443 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (num442 * 180) - vector46.X;
                    float num444 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector46.Y;
                    float num445 = (float)Math.Sqrt(num443 * num443 + num444 * num444);

                    // Boost speed if too far from target
                    if (num445 > 300f)
                        num440 += 0.5f;
                    if (num445 > 400f)
                        num440 += 0.5f;

                    num445 = num440 / num445;
                    num443 *= num445;
                    num444 *= num445;

                    if (npc.velocity.X < num443)
                    {
                        npc.velocity.X += num441;
                        if (npc.velocity.X < 0f && num443 > 0f)
                            npc.velocity.X += num441;
                    }
                    else if (npc.velocity.X > num443)
                    {
                        npc.velocity.X -= num441;
                        if (npc.velocity.X > 0f && num443 < 0f)
                            npc.velocity.X -= num441;
                    }
                    if (npc.velocity.Y < num444)
                    {
                        npc.velocity.Y += num441;
                        if (npc.velocity.Y < 0f && num444 > 0f)
                            npc.velocity.Y += num441;
                    }
                    else if (npc.velocity.Y > num444)
                    {
                        npc.velocity.Y -= num441;
                        if (npc.velocity.Y > 0f && num444 < 0f)
                            npc.velocity.Y -= num441;
                    }

                    // Fire flamethrower for x seconds
                    npc.ai[2] += retAlive ? 1f : 2f;
                    if (npc.ai[2] >= 180f - (death ? 60f * (0.7f - lifeRatio) : 0f))
                    {
                        npc.ai[1] = (!retAlive || enrage) ? 5f : 1f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }

                    // Fire fireballs and flamethrower
                    bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                    if (canHit || !retAlive || enrage)
                    {
                        // Play flame sound on timer
                        npc.localAI[2] += 1f;
                        if (npc.localAI[2] > 22f)
                        {
                            npc.localAI[2] = 0f;
                            SoundEngine.PlaySound(SoundID.Item34, npc.position);
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.localAI[1] += 1f;
                            if (npc.localAI[1] > 2f)
                            {
                                npc.localAI[1] = 0f;

                                float num446 = 6f;
                                num446 += 2f * enrageScale;
                                int type = ModContent.ProjectileType<Shadowflamethrower>();
                                int damage = npc.GetProjectileDamage(type);

                                vector46 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                                num443 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector46.X;
                                num444 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector46.Y;
                                num445 = (float)Math.Sqrt(num443 * num443 + num444 * num444);
                                num445 = num446 / num445;
                                num443 *= num445;
                                num444 *= num445;
                                num444 += Main.rand.Next(-10, 11) * 0.01f;
                                num443 += Main.rand.Next(-10, 11) * 0.01f;
                                num444 += npc.velocity.Y * 0.5f;
                                num443 += npc.velocity.X * 0.5f;
                                vector46.X -= num443 * 1f;
                                vector46.Y -= num444 * 1f;

                                if (canHit)
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), vector46.X, vector46.Y, num443, num444, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                else
                                {
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), vector46.X, vector46.Y, num443, num444, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                    Main.projectile[proj].tileCollide = false;
                                }
                            }
                        }
                    }
                }

                // Charging phase
                else
                {
                    // Charge
                    if (npc.ai[1] == 1f)
                    {
                        // Play charge sound
                        SoundEngine.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

                        // Set rotation and velocity
                        npc.rotation = num420;
                        float num450 = 16.75f + (death ? 5f * (0.7f - lifeRatio) : 0f);
                        num450 += 8f * enrageScale;

                        Vector2 distanceVector = Main.player[npc.target].Center - npc.Center;
                        npc.velocity = Vector2.Normalize(distanceVector) * num450;
                        npc.ai[1] = 2f;
                        return false;
                    }

                    if (npc.ai[1] == 2f)
                    {
                        npc.ai[2] += retAlive ? 1f : 1.25f;

                        float chargeTime = 50f - (death ? 12f * (0.7f - lifeRatio) : 0f);

                        // Slow down
                        if (npc.ai[2] >= chargeTime)
                        {
                            npc.velocity *= 0.93f;
                            if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                                npc.velocity.X = 0f;
                            if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                                npc.velocity.Y = 0f;
                        }
                        else
                            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

                        // Charges 5 times
                        if (npc.ai[2] >= chargeTime + 30f)
                        {
                            npc.ai[3] += 1f;
                            npc.ai[2] = 0f;
                            npc.TargetClosest();
                            npc.rotation = num420;
                            if (npc.ai[3] >= 5f)
                            {
                                npc.ai[1] = 0f;
                                npc.ai[3] = 0f;
                                return false;
                            }
                            npc.ai[1] = 1f;
                        }
                    }

                    // Crazy charge
                    else if (npc.ai[1] == 3f)
                    {
                        // Reset AI array and go to shadowflamethrower phase or fireball phase if ret is dead
                        float secondFastCharge = 4f;
                        if (npc.ai[3] >= (retAlive ? secondFastCharge : secondFastCharge + 1f))
                        {
                            npc.TargetClosest();
                            npc.ai[1] = retAlive ? 0f : 5f;
                            npc.ai[2] = 0f;
                            npc.ai[3] = 0f;

                            if (npc.ai[1] == 0f)
                                npc.localAI[1] = -20f;

                            npc.netUpdate = true;

                            if (npc.netSpam > 10)
                                npc.netSpam = 10;
                        }

                        // Set charging velocity
                        else if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            // Velocity
                            float num48 = 20f + (death ? 6f * (0.7f - lifeRatio) : 0f);
                            num48 += 10f * enrageScale;
                            if (npc.ai[2] == -1f || (!retAlive && npc.ai[3] == secondFastCharge))
                                num48 *= 1.3f;

                            Vector2 distanceVector = Main.player[npc.target].Center + (!retAlive && malice ? Main.player[npc.target].velocity * 20f : Vector2.Zero) - npc.Center;
                            npc.velocity = Vector2.Normalize(distanceVector) * num48;

                            if (retAlive)
                            {
                                Vector2 vector10 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                                float num49 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector10.X;
                                float num50 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector10.Y;
                                float num51 = (float)Math.Sqrt(num49 * num49 + num50 * num50);
                                float num52 = num51;

                                if (num52 < 100f)
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
                            }

                            npc.ai[1] = 4f;
                            npc.netUpdate = true;

                            if (npc.netSpam > 10)
                                npc.netSpam = 10;
                        }
                    }

                    // Crazy charge
                    else if (npc.ai[1] == 4f)
                    {
                        if (npc.ai[2] == 0f)
                            SoundEngine.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

                        float num60 = ((!retAlive && npc.ai[3] == 4f) ? 75f : 50f) - (float)Math.Round(death ? 14f * (0.7f - lifeRatio) : 0f);

                        npc.ai[2] += 1f;

                        if (npc.ai[2] == num60 && Vector2.Distance(npc.position, Main.player[npc.target].position) < (retAlive ? 200f : 150f))
                            npc.ai[2] -= 1f;

                        // Slow down
                        if (npc.ai[2] >= num60)
                        {
                            npc.velocity *= 0.93f;
                            if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                                npc.velocity.X = 0f;
                            if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                                npc.velocity.Y = 0f;
                        }
                        else
                            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

                        // Charge 3 times
                        float num61 = num60 + 25f;
                        if (npc.ai[2] >= num61)
                        {
                            npc.netUpdate = true;

                            if (npc.netSpam > 10)
                                npc.netSpam = 10;

                            npc.ai[3] += 1f;
                            npc.ai[2] = 0f;
                            npc.ai[1] = 3f;
                        }
                    }

                    // Get in position for charge
                    else if (npc.ai[1] == 5f)
                    {
                        float num62 = retAlive ? 600f : 500f;
                        float num63 = 16f + (death ? 5f * (0.7f - lifeRatio) : 0f);
                        float num64 = 0.4f + (death ? 0.1f * (0.7f - lifeRatio) : 0f);

                        if (retAlive)
                        {
                            num63 *= 0.75f;
                            num64 *= 0.75f;
                        }

                        Vector2 vector11 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float num65 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector11.X;
                        float num66 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) + num62 - vector11.Y;
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

                        // Fire shadowflames
                        float fireRate = retAlive ? 30f : 20f;
                        if (npc.ai[2] % fireRate == 0f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float velocity = 16f;
                                int type = ModContent.ProjectileType<ShadowflameFireball>();
                                int damage = npc.GetProjectileDamage(type);
                                Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center + (!retAlive && malice ? Main.player[npc.target].velocity * 20f : Vector2.Zero) - npc.Center) * velocity;
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(projectileVelocity) * 4f, projectileVelocity, type, damage, 0f, Main.myPlayer, 0f, retAlive ? 0f : 1f);
                            }
                        }

                        // Take 3 seconds to get in position, then charge
                        if (npc.ai[2] >= (retAlive ? 180f : 135f) - (death ? 45f * (0.7f - lifeRatio) : 0f))
                        {
                            npc.TargetClosest();
                            npc.ai[1] = 3f;
                            npc.ai[2] = -1f;
                        }

                        npc.netUpdate = true;

                        if (npc.netSpam > 10)
                            npc.netSpam = 10;
                    }
                }
            }

            return false;
        }
        #endregion
    }
}
