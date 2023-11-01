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
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
            }
            bool dead2 = Main.player[npc.target].dead;
            float trueMeleeRetiTargetXDist = npc.position.X + (npc.width / 2) - Main.player[npc.target].position.X - (Main.player[npc.target].width / 2);
            float trueMeleeRetiTargetYDist = npc.position.Y + npc.height - 59f - Main.player[npc.target].position.Y - (Main.player[npc.target].height / 2);
            float trueMeleeRetiTargetDistance = (float)Math.Atan2(trueMeleeRetiTargetYDist, trueMeleeRetiTargetXDist) + 1.57f;
            if (trueMeleeRetiTargetDistance < 0f)
            {
                trueMeleeRetiTargetDistance += MathHelper.TwoPi;
            }
            else if (trueMeleeRetiTargetDistance > MathHelper.TwoPi)
            {
                trueMeleeRetiTargetDistance -= MathHelper.TwoPi;
            }
            float trueMeleeRetiReducedAccel = 0.1f;
            if (npc.rotation < trueMeleeRetiTargetDistance)
            {
                if ((double)(trueMeleeRetiTargetDistance - npc.rotation) > MathHelper.Pi)
                {
                    npc.rotation -= trueMeleeRetiReducedAccel;
                }
                else
                {
                    npc.rotation += trueMeleeRetiReducedAccel;
                }
            }
            else if (npc.rotation > trueMeleeRetiTargetDistance)
            {
                if ((double)(npc.rotation - trueMeleeRetiTargetDistance) > MathHelper.Pi)
                {
                    npc.rotation += trueMeleeRetiReducedAccel;
                }
                else
                {
                    npc.rotation -= trueMeleeRetiReducedAccel;
                }
            }
            if (npc.rotation > trueMeleeRetiTargetDistance - trueMeleeRetiReducedAccel && npc.rotation < trueMeleeRetiTargetDistance + trueMeleeRetiReducedAccel)
            {
                npc.rotation = trueMeleeRetiTargetDistance;
            }
            if (npc.rotation < 0f)
            {
                npc.rotation += MathHelper.TwoPi;
            }
            else if (npc.rotation > MathHelper.TwoPi)
            {
                npc.rotation -= MathHelper.TwoPi;
            }
            if (npc.rotation > trueMeleeRetiTargetDistance - trueMeleeRetiReducedAccel && npc.rotation < trueMeleeRetiTargetDistance + trueMeleeRetiReducedAccel)
            {
                npc.rotation = trueMeleeRetiTargetDistance;
            }
            if (Main.rand.NextBool(5))
            {
                int retinazerDust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f), npc.width, (int)(npc.height * 0.5f), 5, npc.velocity.X, 2f);
                Main.dust[retinazerDust].velocity.X *= 0.5f;
                Main.dust[retinazerDust].velocity.Y *= 0.1f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && !Main.dayTime && !Main.player[npc.target].dead && npc.timeLeft < 10)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && (Main.npc[i].type == NPCID.Retinazer || Main.npc[i].type == NPCID.Spazmatism) && Main.npc[i].timeLeft - 1 > npc.timeLeft)
                        npc.timeLeft = Main.npc[i].timeLeft - 1;

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
                    float retinazerMaxSpeed = 8f;
                    float retinazerAccel = 0.15f;
                    if (Main.expertMode)
                    {
                        retinazerMaxSpeed = 9.5f;
                        retinazerAccel = 0.175f;
                    }
                    if (Main.getGoodWorld)
                    {
                        retinazerMaxSpeed *= 1.15f;
                        retinazerAccel *= 1.15f;
                    }
                    // Reduce acceleration if target is holding a true melee weapon
                    if (Main.player[npc.target].HoldingTrueMeleeWeapon())
                    {
                        retinazerMaxSpeed *= 0.75f;
                        retinazerAccel *= 0.5f;
                    }
                    Vector2 retinazerPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float retinazerTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - retinazerPosition.X;
                    float retinazerTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 300f - retinazerPosition.Y;
                    float retinazerTargetDist = (float)Math.Sqrt(retinazerTargetX * retinazerTargetX + retinazerTargetY * retinazerTargetY);
                    retinazerTargetDist = retinazerMaxSpeed / retinazerTargetDist;
                    retinazerTargetX *= retinazerTargetDist;
                    retinazerTargetY *= retinazerTargetDist;
                    if (npc.velocity.X < retinazerTargetX)
                    {
                        npc.velocity.X += retinazerAccel;
                        if (npc.velocity.X < 0f && retinazerTargetX > 0f)
                        {
                            npc.velocity.X += retinazerAccel;
                        }
                    }
                    else if (npc.velocity.X > retinazerTargetX)
                    {
                        npc.velocity.X -= retinazerAccel;
                        if (npc.velocity.X > 0f && retinazerTargetX < 0f)
                        {
                            npc.velocity.X -= retinazerAccel;
                        }
                    }
                    if (npc.velocity.Y < retinazerTargetY)
                    {
                        npc.velocity.Y += retinazerAccel;
                        if (npc.velocity.Y < 0f && retinazerTargetY > 0f)
                        {
                            npc.velocity.Y += retinazerAccel;
                        }
                    }
                    else if (npc.velocity.Y > retinazerTargetY)
                    {
                        npc.velocity.Y -= retinazerAccel;
                        if (npc.velocity.Y > 0f && retinazerTargetY < 0f)
                        {
                            npc.velocity.Y -= retinazerAccel;
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
                    retinazerPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    retinazerTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - retinazerPosition.X;
                    retinazerTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - retinazerPosition.Y;
                    npc.rotation = (float)Math.Atan2(retinazerTargetY, retinazerTargetX) - 1.57f;
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
                        float retinazerLaserSpeed = 8.5f;
                        int attackDamage_ForProjectiles4 = Main.expertMode ? 23 : 25;
                        if (Main.expertMode)
                        {
                            retinazerLaserSpeed = 10f;
                        }
                        retinazerTargetDist = (float)Math.Sqrt(retinazerTargetX * retinazerTargetX + retinazerTargetY * retinazerTargetY);
                        retinazerTargetDist = retinazerLaserSpeed / retinazerTargetDist;
                        retinazerTargetX *= retinazerTargetDist;
                        retinazerTargetY *= retinazerTargetDist;
                        retinazerPosition.X += retinazerTargetX * 15f;
                        retinazerPosition.Y += retinazerTargetY * 15f;
                        int retinazerLaser = Projectile.NewProjectile(npc.GetSource_FromAI(), retinazerPosition.X, retinazerPosition.Y, retinazerTargetX, retinazerTargetY, 100, attackDamage_ForProjectiles4, 0f, Main.myPlayer);
                    }
                    return false;
                }
                int retinazerFaceDirection = 1;
                if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
                {
                    retinazerFaceDirection = -1;
                }
                float retinazerSpeedPhase2 = 8f;
                float retinazerAccelPhase2 = 0.2f;
                if (Main.expertMode)
                {
                    retinazerSpeedPhase2 = 9.5f;
                    retinazerAccelPhase2 = 0.25f;
                }
                if (Main.getGoodWorld)
                {
                    retinazerSpeedPhase2 *= 1.15f;
                    retinazerAccelPhase2 *= 1.15f;
                }
                // Reduce acceleration if target is holding a true melee weapon
                if (Main.player[npc.target].HoldingTrueMeleeWeapon())
                {
                    retinazerSpeedPhase2 *= 0.75f;
                    retinazerAccelPhase2 *= 0.5f;
                }
                Vector2 retinazerPhase2Pos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float retinazerPhase2TargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (retinazerFaceDirection * 340) - retinazerPhase2Pos.X;
                float retinazerPhase2TargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - retinazerPhase2Pos.Y;
                float retinazerPhase2TargetDist = (float)Math.Sqrt(retinazerPhase2TargetX * retinazerPhase2TargetX + retinazerPhase2TargetY * retinazerPhase2TargetY);
                retinazerPhase2TargetDist = retinazerSpeedPhase2 / retinazerPhase2TargetDist;
                retinazerPhase2TargetX *= retinazerPhase2TargetDist;
                retinazerPhase2TargetY *= retinazerPhase2TargetDist;
                if (npc.velocity.X < retinazerPhase2TargetX)
                {
                    npc.velocity.X += retinazerAccelPhase2;
                    if (npc.velocity.X < 0f && retinazerPhase2TargetX > 0f)
                    {
                        npc.velocity.X += retinazerAccelPhase2;
                    }
                }
                else if (npc.velocity.X > retinazerPhase2TargetX)
                {
                    npc.velocity.X -= retinazerAccelPhase2;
                    if (npc.velocity.X > 0f && retinazerPhase2TargetX < 0f)
                    {
                        npc.velocity.X -= retinazerAccelPhase2;
                    }
                }
                if (npc.velocity.Y < retinazerPhase2TargetY)
                {
                    npc.velocity.Y += retinazerAccelPhase2;
                    if (npc.velocity.Y < 0f && retinazerPhase2TargetY > 0f)
                    {
                        npc.velocity.Y += retinazerAccelPhase2;
                    }
                }
                else if (npc.velocity.Y > retinazerPhase2TargetY)
                {
                    npc.velocity.Y -= retinazerAccelPhase2;
                    if (npc.velocity.Y > 0f && retinazerPhase2TargetY < 0f)
                    {
                        npc.velocity.Y -= retinazerAccelPhase2;
                    }
                }
                retinazerPhase2Pos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                retinazerPhase2TargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - retinazerPhase2Pos.X;
                retinazerPhase2TargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - retinazerPhase2Pos.Y;
                npc.rotation = (float)Math.Atan2(retinazerPhase2TargetY, retinazerPhase2TargetX) - 1.57f;
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
                        int attackDamage_ForProjectiles5 = Main.expertMode ? 17 : 18;
                        retinazerPhase2TargetDist = (float)Math.Sqrt(retinazerPhase2TargetX * retinazerPhase2TargetX + retinazerPhase2TargetY * retinazerPhase2TargetY);
                        retinazerPhase2TargetDist = 9f / retinazerPhase2TargetDist;
                        retinazerPhase2TargetX *= retinazerPhase2TargetDist;
                        retinazerPhase2TargetY *= retinazerPhase2TargetDist;
                        retinazerPhase2Pos.X += retinazerPhase2TargetX * 15f;
                        retinazerPhase2Pos.Y += retinazerPhase2TargetY * 15f;
                        int retinazerPhase2Laser = Projectile.NewProjectile(npc.GetSource_FromAI(), retinazerPhase2Pos.X, retinazerPhase2Pos.Y, retinazerPhase2TargetX, retinazerPhase2TargetY, 100, attackDamage_ForProjectiles5, 0f, Main.myPlayer);
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

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            float enrageScale = bossRush ? 0.5f : 0f;
            if (Main.dayTime || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
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
            float retinazerHoverXDest = npc.position.X + (npc.width / 2) - Main.player[npc.target].position.X - (Main.player[npc.target].width / 2);
            float retinazerHoverYDest = npc.position.Y + npc.height - 59f - Main.player[npc.target].position.Y - (Main.player[npc.target].height / 2);

            float retinazerHoverRotation = (float)Math.Atan2(retinazerHoverYDest, retinazerHoverXDest) + MathHelper.PiOver2;
            if (retinazerHoverRotation < 0f)
                retinazerHoverRotation += MathHelper.TwoPi;
            else if (retinazerHoverRotation > MathHelper.TwoPi)
                retinazerHoverRotation -= MathHelper.TwoPi;

            float retinazerRotationSpeed = 0.1f;
            if (npc.rotation < retinazerHoverRotation)
            {
                if ((retinazerHoverRotation - npc.rotation) > MathHelper.Pi)
                    npc.rotation -= retinazerRotationSpeed;
                else
                    npc.rotation += retinazerRotationSpeed;
            }
            else if (npc.rotation > retinazerHoverRotation)
            {
                if ((npc.rotation - retinazerHoverRotation) > MathHelper.Pi)
                    npc.rotation += retinazerRotationSpeed;
                else
                    npc.rotation -= retinazerRotationSpeed;
            }

            if (npc.rotation > retinazerHoverRotation - retinazerRotationSpeed && npc.rotation < retinazerHoverRotation + retinazerRotationSpeed)
                npc.rotation = retinazerHoverRotation;

            if (npc.rotation < 0f)
                npc.rotation += MathHelper.TwoPi;
            else if (npc.rotation > MathHelper.TwoPi)
                npc.rotation -= MathHelper.TwoPi;

            if (npc.rotation > retinazerHoverRotation - retinazerRotationSpeed && npc.rotation < retinazerHoverRotation + retinazerRotationSpeed)
                npc.rotation = retinazerHoverRotation;

            if (Main.rand.NextBool(5))
            {
                int retiDust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f), npc.width, (int)(npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default, 1f);
                Dust dust = Main.dust[retiDust];
                dust.velocity.X *= 0.5f;
                dust.velocity.Y *= 0.1f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && !Main.player[npc.target].dead && npc.timeLeft < 10)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && (Main.npc[i].type == NPCID.Retinazer || Main.npc[i].type == NPCID.Spazmatism) && Main.npc[i].timeLeft - 1 > npc.timeLeft)
                        npc.timeLeft = Main.npc[i].timeLeft - 1;

                }
            }

            Vector2 mechQueenSpacing = Vector2.Zero;
            if (NPC.IsMechQueenUp)
            {
                NPC nPC = Main.npc[NPC.mechQueen];
                Vector2 mechQueenCenter = nPC.GetMechQueenCenter();
                Vector2 eyePosition = new Vector2(-150f, -250f);
                eyePosition *= 0.75f;
                float mechdusaRotation = nPC.velocity.X * 0.025f;
                mechQueenSpacing = mechQueenCenter + eyePosition;
                mechQueenSpacing = mechQueenSpacing.RotatedBy(mechdusaRotation, mechQueenCenter);
            }

            npc.reflectsProjectiles = false;

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
                    float retinazerPhase1MaxSpeed = 8.25f;
                    float retinazerPhase1Acceleration = 0.115f;
                    retinazerPhase1MaxSpeed += 4f * enrageScale;
                    retinazerPhase1Acceleration += 0.05f * enrageScale;

                    if (death)
                    {
                        retinazerPhase1MaxSpeed += 8f * (1f - lifeRatio);
                        retinazerPhase1Acceleration += 0.1f * (1f - lifeRatio);
                    }

                    if (Main.getGoodWorld)
                    {
                        retinazerPhase1MaxSpeed *= 1.15f;
                        retinazerPhase1Acceleration *= 1.15f;
                    }

                    int retinazerFaceDirection = 1;
                    if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
                        retinazerFaceDirection = -1;

                    Vector2 retinazerPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float retinazerTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (retinazerFaceDirection * 300) - retinazerPosition.X;
                    float retinazerTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 300f - retinazerPosition.Y;

                    if (NPC.IsMechQueenUp)
                    {
                        retinazerPhase1MaxSpeed = 14f;
                        retinazerTargetX = mechQueenSpacing.X;
                        retinazerTargetY = mechQueenSpacing.Y;
                        retinazerTargetX -= retinazerPosition.X;
                        retinazerTargetY -= retinazerPosition.Y;
                    }

                    float retinazerTargetDist = (float)Math.Sqrt(retinazerTargetX * retinazerTargetX + retinazerTargetY * retinazerTargetY);
                    float retinazerTargetDistCopy = retinazerTargetDist;

                    if (NPC.IsMechQueenUp)
                    {
                        if (retinazerTargetDist > retinazerPhase1MaxSpeed)
                        {
                            retinazerTargetDist = retinazerPhase1MaxSpeed / retinazerTargetDist;
                            retinazerTargetX *= retinazerTargetDist;
                            retinazerTargetY *= retinazerTargetDist;
                        }

                        npc.velocity.X = (npc.velocity.X * 59f + retinazerTargetX) / 60f;
                        npc.velocity.Y = (npc.velocity.Y * 59f + retinazerTargetY) / 60f;
                    }
                    else
                    {
                        retinazerTargetDist = retinazerPhase1MaxSpeed / retinazerTargetDist;
                        retinazerTargetX *= retinazerTargetDist;
                        retinazerTargetY *= retinazerTargetDist;

                        if (npc.velocity.X < retinazerTargetX)
                        {
                            npc.velocity.X += retinazerPhase1Acceleration;
                            if (npc.velocity.X < 0f && retinazerTargetX > 0f)
                                npc.velocity.X += retinazerPhase1Acceleration;
                        }
                        else if (npc.velocity.X > retinazerTargetX)
                        {
                            npc.velocity.X -= retinazerPhase1Acceleration;
                            if (npc.velocity.X > 0f && retinazerTargetX < 0f)
                                npc.velocity.X -= retinazerPhase1Acceleration;
                        }
                        if (npc.velocity.Y < retinazerTargetY)
                        {
                            npc.velocity.Y += retinazerPhase1Acceleration;
                            if (npc.velocity.Y < 0f && retinazerTargetY > 0f)
                                npc.velocity.Y += retinazerPhase1Acceleration;
                        }
                        else if (npc.velocity.Y > retinazerTargetY)
                        {
                            npc.velocity.Y -= retinazerPhase1Acceleration;
                            if (npc.velocity.Y > 0f && retinazerTargetY < 0f)
                                npc.velocity.Y -= retinazerPhase1Acceleration;
                        }
                    }

                    npc.ai[2] += 1f;
                    float phaseGateValue = 450f - (death ? 900f * (1f - lifeRatio) : 0f);
                    float laserGateValue = 30f;
                    if (NPC.IsMechQueenUp)
                    {
                        phaseGateValue = 900f;
                        laserGateValue = ((!NPC.npcsFoundForCheckActive[NPCID.TheDestroyerBody]) ? 60f : 90f);
                    }
                    if (npc.ai[2] >= phaseGateValue)
                    {
                        npc.ai[1] = 1f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }

                    else if (npc.position.Y + npc.height < Main.player[npc.target].position.Y && retinazerTargetDistCopy < 400f)
                    {
                        if (!Main.player[npc.target].dead)
                        {
                            npc.ai[3] += 1f;
                            if (Main.getGoodWorld)
                                npc.ai[3] += 0.5f;
                        }

                        if (npc.ai[3] >= laserGateValue)
                        {
                            npc.ai[3] = 0f;
                            retinazerPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                            retinazerTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - retinazerPosition.X;
                            retinazerTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - retinazerPosition.Y;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float retinazerSpeed = 10.5f;
                                retinazerSpeed += 3f * enrageScale;
                                int type = ProjectileID.EyeLaser;
                                int damage = npc.GetProjectileDamage(type);

                                retinazerTargetDist = (float)Math.Sqrt(retinazerTargetX * retinazerTargetX + retinazerTargetY * retinazerTargetY);
                                retinazerTargetDist = retinazerSpeed / retinazerTargetDist;
                                retinazerTargetX *= retinazerTargetDist;
                                retinazerTargetY *= retinazerTargetDist;
                                retinazerPosition.X += retinazerTargetX * 15f;
                                retinazerPosition.Y += retinazerTargetY * 15f;

                                Projectile.NewProjectile(npc.GetSource_FromAI(), retinazerPosition.X, retinazerPosition.Y, retinazerTargetX, retinazerTargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }

                else if (npc.ai[1] == 1f)
                {
                    npc.rotation = retinazerHoverRotation;
                    float retinazerChargeSpeed = 15f;
                    retinazerChargeSpeed += 10f * enrageScale;
                    if (death)
                        retinazerChargeSpeed += 12f * (1f - lifeRatio);
                    if (Main.getGoodWorld)
                        retinazerChargeSpeed += 2f;

                    Vector2 retinazerChargePos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float retinazerChargeTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - retinazerChargePos.X;
                    float retinazerChargeTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - retinazerChargePos.Y;
                    float retinazerChargeTargetDist = (float)Math.Sqrt(retinazerChargeTargetX * retinazerChargeTargetX + retinazerChargeTargetY * retinazerChargeTargetY);
                    retinazerChargeTargetDist = retinazerChargeSpeed / retinazerChargeTargetDist;
                    npc.velocity.X = retinazerChargeTargetX * retinazerChargeTargetDist;
                    npc.velocity.Y = retinazerChargeTargetY * retinazerChargeTargetDist;
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
                        npc.rotation = retinazerHoverRotation;
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
                if (NPC.IsMechQueenUp)
                    npc.reflectsProjectiles = true;

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
                        SoundEngine.PlaySound(SoundID.NPCHit1, npc.position);

                        if (Main.netMode != NetmodeID.Server)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 143, 1f);
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
                            }
                        }

                        for (int j = 0; j < 20; j++)
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
                    float retinazerPhase2MaxSpeed = 9.5f + (death ? 3f * (0.7f - lifeRatio) : 0f);
                    float retinazerPhase2Accel = 0.175f + (death ? 0.05f * (0.7f - lifeRatio) : 0f);
                    retinazerPhase2MaxSpeed += 4.5f * enrageScale;
                    retinazerPhase2Accel += 0.075f * enrageScale;

                    if (Main.getGoodWorld)
                    {
                        retinazerPhase2MaxSpeed *= 1.15f;
                        retinazerPhase2Accel *= 1.15f;
                    }

                    // Reduce acceleration if target is holding a true melee weapon
                    if (Main.player[npc.target].HoldingTrueMeleeWeapon())
                    {
                        retinazerPhase2MaxSpeed *= 0.75f;
                        retinazerPhase2Accel *= 0.5f;
                    }

                    Vector2 eyePosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float retinazerPhase2TargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - eyePosition.X;
                    float retinazerPhase2TargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 300f - eyePosition.Y;

                    if (NPC.IsMechQueenUp)
                    {
                        retinazerPhase2MaxSpeed = 14f;
                        retinazerPhase2TargetX = mechQueenSpacing.X;
                        retinazerPhase2TargetY = mechQueenSpacing.Y;
                        retinazerPhase2TargetX -= eyePosition.X;
                        retinazerPhase2TargetY -= eyePosition.Y;
                    }

                    float retinazerPhase2TargetDist = (float)Math.Sqrt(retinazerPhase2TargetX * retinazerPhase2TargetX + retinazerPhase2TargetY * retinazerPhase2TargetY);

                    if (NPC.IsMechQueenUp)
                    {
                        if (retinazerPhase2TargetDist > retinazerPhase2MaxSpeed)
                        {
                            retinazerPhase2TargetDist = retinazerPhase2MaxSpeed / retinazerPhase2TargetDist;
                            retinazerPhase2TargetX *= retinazerPhase2TargetDist;
                            retinazerPhase2TargetY *= retinazerPhase2TargetDist;
                        }

                        npc.velocity.X = (npc.velocity.X * 4f + retinazerPhase2TargetX) / 5f;
                        npc.velocity.Y = (npc.velocity.Y * 4f + retinazerPhase2TargetY) / 5f;
                    }
                    else
                    {
                        retinazerPhase2TargetDist = retinazerPhase2MaxSpeed / retinazerPhase2TargetDist;
                        retinazerPhase2TargetX *= retinazerPhase2TargetDist;
                        retinazerPhase2TargetY *= retinazerPhase2TargetDist;

                        if (npc.velocity.X < retinazerPhase2TargetX)
                        {
                            npc.velocity.X += retinazerPhase2Accel;
                            if (npc.velocity.X < 0f && retinazerPhase2TargetX > 0f)
                                npc.velocity.X += retinazerPhase2Accel;
                        }
                        else if (npc.velocity.X > retinazerPhase2TargetX)
                        {
                            npc.velocity.X -= retinazerPhase2Accel;
                            if (npc.velocity.X > 0f && retinazerPhase2TargetX < 0f)
                                npc.velocity.X -= retinazerPhase2Accel;
                        }
                        if (npc.velocity.Y < retinazerPhase2TargetY)
                        {
                            npc.velocity.Y += retinazerPhase2Accel;
                            if (npc.velocity.Y < 0f && retinazerPhase2TargetY > 0f)
                                npc.velocity.Y += retinazerPhase2Accel;
                        }
                        else if (npc.velocity.Y > retinazerPhase2TargetY)
                        {
                            npc.velocity.Y -= retinazerPhase2Accel;
                            if (npc.velocity.Y > 0f && retinazerPhase2TargetY < 0f)
                                npc.velocity.Y -= retinazerPhase2Accel;
                        }
                    }

                    npc.ai[2] += spazAlive ? 1f : 1.25f;
                    float phaseGateValue = NPC.IsMechQueenUp ? 900f : 300f - (death ? 120f * (0.7f - lifeRatio) : 0f);
                    if (npc.ai[2] >= phaseGateValue)
                    {
                        npc.ai[1] = 1f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }

                    eyePosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    retinazerPhase2TargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - eyePosition.X;
                    retinazerPhase2TargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - eyePosition.Y;
                    npc.rotation = (float)Math.Atan2(retinazerPhase2TargetY, retinazerPhase2TargetX) - MathHelper.PiOver2;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[1] += 1f + (death ? 0.7f - lifeRatio : 0f);
                        if (npc.localAI[1] >= (spazAlive ? 72f : 24f))
                        {
                            bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                            if (canHit || !spazAlive || enrage)
                            {
                                npc.localAI[1] = 0f;
                                float retinazerPhase2LaserSpeed = 10f;
                                retinazerPhase2LaserSpeed += enrageScale;
                                int type = ProjectileID.DeathLaser;
                                int damage = npc.GetProjectileDamage(type);

                                retinazerPhase2TargetDist = (float)Math.Sqrt(retinazerPhase2TargetX * retinazerPhase2TargetX + retinazerPhase2TargetY * retinazerPhase2TargetY);
                                retinazerPhase2TargetDist = retinazerPhase2LaserSpeed / retinazerPhase2TargetDist;
                                retinazerPhase2TargetX *= retinazerPhase2TargetDist;
                                retinazerPhase2TargetY *= retinazerPhase2TargetDist;
                                eyePosition.X += retinazerPhase2TargetX * 15f;
                                eyePosition.Y += retinazerPhase2TargetY * 15f;

                                if (canHit)
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), eyePosition.X, eyePosition.Y, retinazerPhase2TargetX, retinazerPhase2TargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                else
                                {
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), eyePosition.X, eyePosition.Y, retinazerPhase2TargetX, retinazerPhase2TargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);
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
                        int retinazerPhase2FaceDirection = 1;
                        if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
                            retinazerPhase2FaceDirection = -1;

                        float retinazerPhase2RapidFireMaxSpeed = 9.5f + (death ? 3f * (0.7f - lifeRatio) : 0f);
                        float retinazerPhase2RapidFireAccel = 0.25f + (death ? 0.075f * (0.7f - lifeRatio) : 0f);
                        retinazerPhase2RapidFireMaxSpeed += 4.5f * enrageScale;
                        retinazerPhase2RapidFireAccel += 0.15f * enrageScale;

                        if (Main.getGoodWorld)
                        {
                            retinazerPhase2RapidFireMaxSpeed *= 1.15f;
                            retinazerPhase2RapidFireAccel *= 1.15f;
                        }

                        // Reduce acceleration if target is holding a true melee weapon
                        if (Main.player[npc.target].HoldingTrueMeleeWeapon())
                        {
                            retinazerPhase2RapidFireMaxSpeed *= 0.75f;
                            retinazerPhase2RapidFireAccel *= 0.5f;
                        }

                        Vector2 retinazerPhase2RapidFirePos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float retinazerPhase2RapidFireTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (retinazerPhase2FaceDirection * 340) - retinazerPhase2RapidFirePos.X;
                        float retinazerPhase2RapidFireTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - retinazerPhase2RapidFirePos.Y;
                        float retinazerPhase2RapidFireTargetDist = (float)Math.Sqrt(retinazerPhase2RapidFireTargetX * retinazerPhase2RapidFireTargetX + retinazerPhase2RapidFireTargetY * retinazerPhase2RapidFireTargetY);
                        retinazerPhase2RapidFireTargetDist = retinazerPhase2RapidFireMaxSpeed / retinazerPhase2RapidFireTargetDist;
                        retinazerPhase2RapidFireTargetX *= retinazerPhase2RapidFireTargetDist;
                        retinazerPhase2RapidFireTargetY *= retinazerPhase2RapidFireTargetDist;

                        if (npc.velocity.X < retinazerPhase2RapidFireTargetX)
                        {
                            npc.velocity.X += retinazerPhase2RapidFireAccel;
                            if (npc.velocity.X < 0f && retinazerPhase2RapidFireTargetX > 0f)
                                npc.velocity.X += retinazerPhase2RapidFireAccel;
                        }
                        else if (npc.velocity.X > retinazerPhase2RapidFireTargetX)
                        {
                            npc.velocity.X -= retinazerPhase2RapidFireAccel;
                            if (npc.velocity.X > 0f && retinazerPhase2RapidFireTargetX < 0f)
                                npc.velocity.X -= retinazerPhase2RapidFireAccel;
                        }
                        if (npc.velocity.Y < retinazerPhase2RapidFireTargetY)
                        {
                            npc.velocity.Y += retinazerPhase2RapidFireAccel;
                            if (npc.velocity.Y < 0f && retinazerPhase2RapidFireTargetY > 0f)
                                npc.velocity.Y += retinazerPhase2RapidFireAccel;
                        }
                        else if (npc.velocity.Y > retinazerPhase2RapidFireTargetY)
                        {
                            npc.velocity.Y -= retinazerPhase2RapidFireAccel;
                            if (npc.velocity.Y > 0f && retinazerPhase2RapidFireTargetY < 0f)
                                npc.velocity.Y -= retinazerPhase2RapidFireAccel;
                        }

                        retinazerPhase2RapidFirePos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        retinazerPhase2RapidFireTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - retinazerPhase2RapidFirePos.X;
                        retinazerPhase2RapidFireTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - retinazerPhase2RapidFirePos.Y;
                        npc.rotation = (float)Math.Atan2(retinazerPhase2RapidFireTargetY, retinazerPhase2RapidFireTargetX) - MathHelper.PiOver2;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.localAI[1] += 1f + (death ? 0.7f - lifeRatio : 0f);
                            if (npc.localAI[1] > (spazAlive ? 24f : 8f))
                            {
                                bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                                if (canHit || !spazAlive || enrage)
                                {
                                    npc.localAI[1] = 0f;
                                    int type = ProjectileID.DeathLaser;
                                    int damage = (int)Math.Round(npc.GetProjectileDamage(type) * 0.75);

                                    retinazerPhase2RapidFireTargetDist = (float)Math.Sqrt(retinazerPhase2RapidFireTargetX * retinazerPhase2RapidFireTargetX + retinazerPhase2RapidFireTargetY * retinazerPhase2RapidFireTargetY);
                                    retinazerPhase2RapidFireTargetDist = 9f / retinazerPhase2RapidFireTargetDist;
                                    retinazerPhase2RapidFireTargetX *= retinazerPhase2RapidFireTargetDist;
                                    retinazerPhase2RapidFireTargetY *= retinazerPhase2RapidFireTargetDist;
                                    retinazerPhase2RapidFirePos.X += retinazerPhase2RapidFireTargetX * 15f;
                                    retinazerPhase2RapidFirePos.Y += retinazerPhase2RapidFireTargetY * 15f;

                                    if (canHit)
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), retinazerPhase2RapidFirePos.X, retinazerPhase2RapidFirePos.Y, retinazerPhase2RapidFireTargetX, retinazerPhase2RapidFireTargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                    else
                                    {
                                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), retinazerPhase2RapidFirePos.X, retinazerPhase2RapidFirePos.Y, retinazerPhase2RapidFireTargetX, retinazerPhase2RapidFireTargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);
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
                        npc.rotation = retinazerHoverRotation;
                        float retinazerPhase3ChargeSpeed = 22f + (death ? 8f * (0.7f - lifeRatio) : 0f);
                        retinazerPhase3ChargeSpeed += 10f * enrageScale;

                        if (!spazAlive)
                            retinazerPhase3ChargeSpeed += 2f;

                        if (Main.getGoodWorld)
                            retinazerPhase3ChargeSpeed += 2f;

                        Vector2 retinazerPhase3ChargePos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float retinazerPhase3ChargeTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - retinazerPhase3ChargePos.X;
                        float retinazerPhase3ChargeTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - retinazerPhase3ChargePos.Y;
                        float retinazerPhase3ChargeTargetDist = (float)Math.Sqrt(retinazerPhase3ChargeTargetX * retinazerPhase3ChargeTargetX + retinazerPhase3ChargeTargetY * retinazerPhase3ChargeTargetY);
                        retinazerPhase3ChargeTargetDist = retinazerPhase3ChargeSpeed / retinazerPhase3ChargeTargetDist;
                        npc.velocity.X = retinazerPhase3ChargeTargetX * retinazerPhase3ChargeTargetDist;
                        npc.velocity.Y = retinazerPhase3ChargeTargetY * retinazerPhase3ChargeTargetDist;
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
                                    Vector2 retinazerPhase3ChargeLaserPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                                    float retinazerPhase3ChargeLaserTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - retinazerPhase3ChargeLaserPos.X;
                                    float retinazerPhase3ChargeLaserTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - retinazerPhase3ChargeLaserPos.Y;
                                    float retinazerPhase3ChargeLaserTargetDist = (float)Math.Sqrt(retinazerPhase3ChargeLaserTargetX * retinazerPhase3ChargeLaserTargetX + retinazerPhase3ChargeLaserTargetY * retinazerPhase3ChargeLaserTargetY);

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        SoundEngine.PlaySound(SoundID.Item33, npc.position);
                                        int type = ModContent.ProjectileType<ScavengerLaser>();
                                        int damage = npc.GetProjectileDamage(type);

                                        retinazerPhase3ChargeLaserPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                                        retinazerPhase3ChargeLaserTargetX = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - retinazerPhase3ChargeLaserPos.X;
                                        retinazerPhase3ChargeLaserTargetY = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - retinazerPhase3ChargeLaserPos.Y;
                                        retinazerPhase3ChargeLaserTargetDist = (float)Math.Sqrt(retinazerPhase3ChargeLaserTargetX * retinazerPhase3ChargeLaserTargetX + retinazerPhase3ChargeLaserTargetY * retinazerPhase3ChargeLaserTargetY);
                                        retinazerPhase3ChargeLaserTargetDist = 6f / retinazerPhase3ChargeLaserTargetDist;
                                        retinazerPhase3ChargeLaserTargetX *= retinazerPhase3ChargeLaserTargetDist;
                                        retinazerPhase3ChargeLaserTargetY *= retinazerPhase3ChargeLaserTargetDist;
                                        retinazerPhase3ChargeLaserPos.X += retinazerPhase3ChargeLaserTargetX;
                                        retinazerPhase3ChargeLaserPos.Y += retinazerPhase3ChargeLaserTargetY;

                                        Projectile.NewProjectile(npc.GetSource_FromAI(), retinazerPhase3ChargeLaserPos.X, retinazerPhase3ChargeLaserPos.Y, retinazerPhase3ChargeLaserTargetX, retinazerPhase3ChargeLaserTargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                    }
                                }
                            }
                        }

                        // Charge four times
                        float chargeGateValue = 30f;
                        chargeGateValue -= chargeGateValue / 4 * enrageScale;
                        if (npc.ai[2] >= chargeTime + chargeGateValue)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[3] += 1f;
                            npc.TargetClosest();
                            npc.rotation = retinazerHoverRotation;
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
                        int chargeLineUpDist = spazAlive ? 600 : 500;
                        float chargeSpeed = 12f + (death ? 4f * (0.7f - lifeRatio) : 0f);
                        float chargeAccel = 0.3f + (death ? 0.1f * (0.7f - lifeRatio) : 0f);

                        if (spazAlive)
                        {
                            chargeSpeed *= 0.75f;
                            chargeAccel *= 0.75f;
                        }

                        if (Main.getGoodWorld)
                        {
                            chargeSpeed *= 1.15f;
                            chargeAccel *= 1.15f;
                        }

                        int retinazerPhase2FaceDirection = 1;
                        if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
                            retinazerPhase2FaceDirection = -1;

                        Vector2 spazmatismRetDeadChargePos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float chargeTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (chargeLineUpDist * retinazerPhase2FaceDirection) - spazmatismRetDeadChargePos.X;
                        float chargeTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - spazmatismRetDeadChargePos.Y;
                        float chargeTargetDist = (float)Math.Sqrt(chargeTargetX * chargeTargetX + chargeTargetY * chargeTargetY);

                        chargeTargetDist = chargeSpeed / chargeTargetDist;
                        chargeTargetX *= chargeTargetDist;
                        chargeTargetY *= chargeTargetDist;

                        if (npc.velocity.X < chargeTargetX)
                        {
                            npc.velocity.X += chargeAccel;
                            if (npc.velocity.X < 0f && chargeTargetX > 0f)
                                npc.velocity.X += chargeAccel;
                        }
                        else if (npc.velocity.X > chargeTargetX)
                        {
                            npc.velocity.X -= chargeAccel;
                            if (npc.velocity.X > 0f && chargeTargetX < 0f)
                                npc.velocity.X -= chargeAccel;
                        }
                        if (npc.velocity.Y < chargeTargetY)
                        {
                            npc.velocity.Y += chargeAccel;
                            if (npc.velocity.Y < 0f && chargeTargetY > 0f)
                                npc.velocity.Y += chargeAccel;
                        }
                        else if (npc.velocity.Y > chargeTargetY)
                        {
                            npc.velocity.Y -= chargeAccel;
                            if (npc.velocity.Y > 0f && chargeTargetY < 0f)
                                npc.velocity.Y -= chargeAccel;
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

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            float enrageScale = bossRush ? 0.5f : 0f;
            if (Main.dayTime || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
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

            // Boss Rush predictive charge rotation
            if (npc.ai[1] == 5f && !retAlive && bossRush)
            {
                // Velocity
                float chargeVelocity = 20f + (death ? 6f * (0.7f - lifeRatio) : 0f);
                chargeVelocity += 10f * enrageScale;
                if (Main.getGoodWorld)
                    chargeVelocity += 2f;
                if (npc.ai[2] == -1f || (!retAlive && npc.ai[3] == 4f))
                    chargeVelocity *= 1.3f;

                lookAt += Main.player[npc.target].velocity * 20f;
                rotationVector = Vector2.Normalize(npcCenter - lookAt) * chargeVelocity;
            }

            float spazmatismRotation = (float)Math.Atan2(rotationVector.Y, rotationVector.X) + MathHelper.PiOver2;
            if (spazmatismRotation < 0f)
                spazmatismRotation += MathHelper.TwoPi;
            else if (spazmatismRotation > MathHelper.TwoPi)
                spazmatismRotation -= MathHelper.TwoPi;

            float spazmatismRotateSpeed = 0.15f;
            if (NPC.IsMechQueenUp && npc.ai[0] == 3f && npc.ai[1] == 0f)
                spazmatismRotateSpeed *= 0.25f;

            if (npc.rotation < spazmatismRotation)
            {
                if ((spazmatismRotation - npc.rotation) > MathHelper.Pi)
                    npc.rotation -= spazmatismRotateSpeed;
                else
                    npc.rotation += spazmatismRotateSpeed;
            }
            else if (npc.rotation > spazmatismRotation)
            {
                if ((npc.rotation - spazmatismRotation) > MathHelper.Pi)
                    npc.rotation += spazmatismRotateSpeed;
                else
                    npc.rotation -= spazmatismRotateSpeed;
            }

            if (npc.rotation > spazmatismRotation - spazmatismRotateSpeed && npc.rotation < spazmatismRotation + spazmatismRotateSpeed)
                npc.rotation = spazmatismRotation;

            if (npc.rotation < 0f)
                npc.rotation += MathHelper.TwoPi;
            else if (npc.rotation > MathHelper.TwoPi)
                npc.rotation -= MathHelper.TwoPi;

            if (npc.rotation > spazmatismRotation - spazmatismRotateSpeed && npc.rotation < spazmatismRotation + spazmatismRotateSpeed)
                npc.rotation = spazmatismRotation;

            // Dust
            if (Main.rand.NextBool(5))
            {
                int spazDust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f), npc.width, (int)(npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default, 1f);
                Dust dust = Main.dust[spazDust];
                dust.velocity.X *= 0.5f;
                dust.velocity.Y *= 0.1f;
            }

            // Despawn Twins at the same time
            if (Main.netMode != NetmodeID.MultiplayerClient && !Main.player[npc.target].dead && npc.timeLeft < 10)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && (Main.npc[i].type == NPCID.Retinazer || Main.npc[i].type == NPCID.Spazmatism) && Main.npc[i].timeLeft - 1 > npc.timeLeft)
                        npc.timeLeft = Main.npc[i].timeLeft - 1;

                }
            }

            Vector2 mechQueenSpacing = Vector2.Zero;
            if (NPC.IsMechQueenUp)
            {
                NPC nPC2 = Main.npc[NPC.mechQueen];
                Vector2 mechQueenCenter2 = nPC2.GetMechQueenCenter();
                Vector2 mechdusaSpacingVector = new Vector2(150f, -250f);
                mechdusaSpacingVector *= 0.75f;
                float mechdusaSpacingVel = nPC2.velocity.X * 0.025f;
                mechQueenSpacing = mechQueenCenter2 + mechdusaSpacingVector;
                mechQueenSpacing = mechQueenSpacing.RotatedBy(mechdusaSpacingVel, mechQueenCenter2);
            }

            npc.reflectsProjectiles = false;

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
                    float spazmatismFireballMaxSpeed = 12f;
                    float spazmatismFireballAccel = 0.4f;
                    spazmatismFireballMaxSpeed += 6f * enrageScale;
                    spazmatismFireballAccel += 0.2f * enrageScale;

                    if (death)
                    {
                        spazmatismFireballMaxSpeed += 9f * (1f - lifeRatio);
                        spazmatismFireballAccel += 0.3f * (1f - lifeRatio);
                    }

                    if (Main.getGoodWorld)
                    {
                        spazmatismFireballMaxSpeed *= 1.15f;
                        spazmatismFireballAccel *= 1.15f;
                    }

                    int spazmatismFireballFaceDirection = 1;
                    if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
                        spazmatismFireballFaceDirection = -1;

                    Vector2 spazmatismFireballPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float spazmatismFireballTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (spazmatismFireballFaceDirection * 400) - spazmatismFireballPos.X;
                    float spazmatismFireballTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - spazmatismFireballPos.Y;
                    if (NPC.IsMechQueenUp)
                    {
                        spazmatismFireballMaxSpeed = 14f;
                        spazmatismFireballTargetX = mechQueenSpacing.X;
                        spazmatismFireballTargetY = mechQueenSpacing.Y;
                        spazmatismFireballTargetX -= spazmatismFireballPos.X;
                        spazmatismFireballTargetY -= spazmatismFireballPos.Y;
                    }

                    float spazmatismFireballTargetDist = (float)Math.Sqrt(spazmatismFireballTargetX * spazmatismFireballTargetX + spazmatismFireballTargetY * spazmatismFireballTargetY);

                    if (NPC.IsMechQueenUp)
                    {
                        if (spazmatismFireballTargetDist > spazmatismFireballMaxSpeed)
                        {
                            spazmatismFireballTargetDist = spazmatismFireballMaxSpeed / spazmatismFireballTargetDist;
                            spazmatismFireballTargetX *= spazmatismFireballTargetDist;
                            spazmatismFireballTargetY *= spazmatismFireballTargetDist;
                        }

                        npc.velocity.X = (npc.velocity.X * 4f + spazmatismFireballTargetX) / 5f;
                        npc.velocity.Y = (npc.velocity.Y * 4f + spazmatismFireballTargetY) / 5f;
                    }
                    else
                    {
                        spazmatismFireballTargetDist = spazmatismFireballMaxSpeed / spazmatismFireballTargetDist;
                        spazmatismFireballTargetX *= spazmatismFireballTargetDist;
                        spazmatismFireballTargetY *= spazmatismFireballTargetDist;

                        if (npc.velocity.X < spazmatismFireballTargetX)
                        {
                            npc.velocity.X += spazmatismFireballAccel;
                            if (npc.velocity.X < 0f && spazmatismFireballTargetX > 0f)
                                npc.velocity.X += spazmatismFireballAccel;
                        }
                        else if (npc.velocity.X > spazmatismFireballTargetX)
                        {
                            npc.velocity.X -= spazmatismFireballAccel;
                            if (npc.velocity.X > 0f && spazmatismFireballTargetX < 0f)
                                npc.velocity.X -= spazmatismFireballAccel;
                        }
                        if (npc.velocity.Y < spazmatismFireballTargetY)
                        {
                            npc.velocity.Y += spazmatismFireballAccel;
                            if (npc.velocity.Y < 0f && spazmatismFireballTargetY > 0f)
                                npc.velocity.Y += spazmatismFireballAccel;
                        }
                        else if (npc.velocity.Y > spazmatismFireballTargetY)
                        {
                            npc.velocity.Y -= spazmatismFireballAccel;
                            if (npc.velocity.Y > 0f && spazmatismFireballTargetY < 0f)
                                npc.velocity.Y -= spazmatismFireballAccel;
                        }
                    }

                    // Fire cursed flames for 5 seconds
                    npc.ai[2] += 1f;
                    float phaseGateValue = NPC.IsMechQueenUp ? 900f : 300f - (death ? 600f * (1f - lifeRatio) : 0f);
                    if (npc.ai[2] >= phaseGateValue)
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
                        {
                            npc.ai[3] += 1f;
                            if (Main.getGoodWorld)
                                npc.ai[3] += 0.4f;
                        }

                        if (npc.ai[3] >= 30f)
                        {
                            npc.ai[3] = 0f;
                            spazmatismFireballPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                            spazmatismFireballTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - spazmatismFireballPos.X;
                            spazmatismFireballTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - spazmatismFireballPos.Y;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float cursedFireballSpeed = 15f;
                                cursedFireballSpeed += 3f * enrageScale;
                                int type = ProjectileID.CursedFlameHostile;
                                int damage = npc.GetProjectileDamage(type);

                                spazmatismFireballTargetDist = (float)Math.Sqrt(spazmatismFireballTargetX * spazmatismFireballTargetX + spazmatismFireballTargetY * spazmatismFireballTargetY);
                                spazmatismFireballTargetDist = cursedFireballSpeed / spazmatismFireballTargetDist;
                                spazmatismFireballTargetX *= spazmatismFireballTargetDist;
                                spazmatismFireballTargetY *= spazmatismFireballTargetDist;
                                spazmatismFireballTargetX += Main.rand.Next(-10, 11) * 0.05f;
                                spazmatismFireballTargetY += Main.rand.Next(-10, 11) * 0.05f;
                                spazmatismFireballPos.X += spazmatismFireballTargetX * 4f;
                                spazmatismFireballPos.Y += spazmatismFireballTargetY * 4f;

                                Projectile.NewProjectile(npc.GetSource_FromAI(), spazmatismFireballPos.X, spazmatismFireballPos.Y, spazmatismFireballTargetX, spazmatismFireballTargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }

                // Charging phase
                else if (npc.ai[1] == 1f)
                {
                    // Rotation and velocity
                    npc.rotation = spazmatismRotation;
                    float spazmatismPhase1ChargeSpeed = 16f;
                    spazmatismPhase1ChargeSpeed += 8f * enrageScale;
                    if (death)
                        spazmatismPhase1ChargeSpeed += 12f * (1f - lifeRatio);
                    if (Main.getGoodWorld)
                        spazmatismPhase1ChargeSpeed *= 1.2f;

                    Vector2 spazmatismPhase1ChargePos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float spazmatismPhase1ChargeTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - spazmatismPhase1ChargePos.X;
                    float spazmatismPhase1ChargeTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - spazmatismPhase1ChargePos.Y;
                    float spazmatismPhase1ChargeTargetDist = (float)Math.Sqrt(spazmatismPhase1ChargeTargetX * spazmatismPhase1ChargeTargetX + spazmatismPhase1ChargeTargetY * spazmatismPhase1ChargeTargetY);
                    spazmatismPhase1ChargeTargetDist = spazmatismPhase1ChargeSpeed / spazmatismPhase1ChargeTargetDist;
                    npc.velocity.X = spazmatismPhase1ChargeTargetX * spazmatismPhase1ChargeTargetDist;
                    npc.velocity.Y = spazmatismPhase1ChargeTargetY * spazmatismPhase1ChargeTargetDist;
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
                        npc.rotation = spazmatismRotation;
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
                if (NPC.IsMechQueenUp)
                    npc.reflectsProjectiles = true;

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
                        SoundEngine.PlaySound(SoundID.NPCHit1, npc.position);

                        if (Main.netMode != NetmodeID.Server)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 144, 1f);
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
                                Gore.NewGore(npc.GetSource_FromAI(), npc.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
                            }
                        }

                        for (int j = 0; j < 20; j++)
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
                    float spazmatismFlamethrowerMaxSpeed = 6.2f + (death ? 2f * (0.7f - lifeRatio) : 0f);
                    float spazmatismFlamethrowerAccel = 0.1f + (death ? 0.03f * (0.7f - lifeRatio) : 0f);
                    spazmatismFlamethrowerMaxSpeed += 4f * enrageScale;
                    spazmatismFlamethrowerAccel += 0.06f * enrageScale;

                    int spazmatismFlamethrowerFaceDirection = 1;
                    if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + Main.player[npc.target].width)
                        spazmatismFlamethrowerFaceDirection = -1;

                    Vector2 spazmatismFlamethrowerPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float spazmatismFlamethrowerTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (spazmatismFlamethrowerFaceDirection * 180) - spazmatismFlamethrowerPos.X;
                    float spazmatismFlamethrowerTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - spazmatismFlamethrowerPos.Y;
                    float spazmatismFlamethrowerTargetDist = (float)Math.Sqrt(spazmatismFlamethrowerTargetX * spazmatismFlamethrowerTargetX + spazmatismFlamethrowerTargetY * spazmatismFlamethrowerTargetY);

                    if (!NPC.IsMechQueenUp)
                    {
                        // Boost speed if too far from target
                        if (spazmatismFlamethrowerTargetDist > 300f)
                            spazmatismFlamethrowerMaxSpeed += 0.5f;
                        if (spazmatismFlamethrowerTargetDist > 400f)
                            spazmatismFlamethrowerMaxSpeed += 0.5f;

                        if (Main.getGoodWorld)
                        {
                            spazmatismFlamethrowerMaxSpeed *= 1.15f;
                            spazmatismFlamethrowerAccel *= 1.15f;
                        }

                        spazmatismFlamethrowerTargetDist = spazmatismFlamethrowerMaxSpeed / spazmatismFlamethrowerTargetDist;
                        spazmatismFlamethrowerTargetX *= spazmatismFlamethrowerTargetDist;
                        spazmatismFlamethrowerTargetY *= spazmatismFlamethrowerTargetDist;

                        if (npc.velocity.X < spazmatismFlamethrowerTargetX)
                        {
                            npc.velocity.X += spazmatismFlamethrowerAccel;
                            if (npc.velocity.X < 0f && spazmatismFlamethrowerTargetX > 0f)
                                npc.velocity.X += spazmatismFlamethrowerAccel;
                        }
                        else if (npc.velocity.X > spazmatismFlamethrowerTargetX)
                        {
                            npc.velocity.X -= spazmatismFlamethrowerAccel;
                            if (npc.velocity.X > 0f && spazmatismFlamethrowerTargetX < 0f)
                                npc.velocity.X -= spazmatismFlamethrowerAccel;
                        }
                        if (npc.velocity.Y < spazmatismFlamethrowerTargetY)
                        {
                            npc.velocity.Y += spazmatismFlamethrowerAccel;
                            if (npc.velocity.Y < 0f && spazmatismFlamethrowerTargetY > 0f)
                                npc.velocity.Y += spazmatismFlamethrowerAccel;
                        }
                        else if (npc.velocity.Y > spazmatismFlamethrowerTargetY)
                        {
                            npc.velocity.Y -= spazmatismFlamethrowerAccel;
                            if (npc.velocity.Y > 0f && spazmatismFlamethrowerTargetY < 0f)
                                npc.velocity.Y -= spazmatismFlamethrowerAccel;
                        }
                    }

                    // Fire flamethrower for x seconds
                    npc.ai[2] += retAlive ? 1f : 2f;
                    float phaseGateValue = NPC.IsMechQueenUp ? 900f : 180f - (death ? 60f * (0.7f - lifeRatio) : 0f);
                    if (npc.ai[2] >= phaseGateValue)
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

                                float spazmatismShadowFireballSpeed = 6f;
                                spazmatismShadowFireballSpeed += 2f * enrageScale;
                                int type = ModContent.ProjectileType<Shadowflamethrower>();
                                int damage = npc.GetProjectileDamage(type);

                                spazmatismFlamethrowerPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                                spazmatismFlamethrowerTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - spazmatismFlamethrowerPos.X;
                                spazmatismFlamethrowerTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - spazmatismFlamethrowerPos.Y;
                                spazmatismFlamethrowerTargetDist = (float)Math.Sqrt(spazmatismFlamethrowerTargetX * spazmatismFlamethrowerTargetX + spazmatismFlamethrowerTargetY * spazmatismFlamethrowerTargetY);
                                spazmatismFlamethrowerTargetDist = spazmatismShadowFireballSpeed / spazmatismFlamethrowerTargetDist;
                                spazmatismFlamethrowerTargetX *= spazmatismFlamethrowerTargetDist;
                                spazmatismFlamethrowerTargetY *= spazmatismFlamethrowerTargetDist;
                                spazmatismFlamethrowerTargetY += Main.rand.Next(-10, 11) * 0.01f;
                                spazmatismFlamethrowerTargetX += Main.rand.Next(-10, 11) * 0.01f;
                                spazmatismFlamethrowerTargetY += npc.velocity.Y * 0.5f;
                                spazmatismFlamethrowerTargetX += npc.velocity.X * 0.5f;
                                spazmatismFlamethrowerPos.X -= spazmatismFlamethrowerTargetX * 1f;
                                spazmatismFlamethrowerPos.Y -= spazmatismFlamethrowerTargetY * 1f;

                                if (NPC.IsMechQueenUp)
                                {
                                    Vector2 mechdusaSpazShadowFireballPos = (npc.rotation + (float)Math.PI / 2f).ToRotationVector2() * spazmatismShadowFireballSpeed + npc.velocity * 0.5f;
                                    spazmatismFlamethrowerTargetX = mechdusaSpazShadowFireballPos.X;
                                    spazmatismFlamethrowerTargetY = mechdusaSpazShadowFireballPos.Y;
                                    spazmatismFlamethrowerPos = npc.Center - mechdusaSpazShadowFireballPos * 3f;
                                }

                                if (canHit)
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), spazmatismFlamethrowerPos.X, spazmatismFlamethrowerPos.Y, spazmatismFlamethrowerTargetX, spazmatismFlamethrowerTargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                else
                                {
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), spazmatismFlamethrowerPos.X, spazmatismFlamethrowerPos.Y, spazmatismFlamethrowerTargetX, spazmatismFlamethrowerTargetY, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                    Main.projectile[proj].tileCollide = false;
                                }
                            }
                        }
                    }

                    if (NPC.IsMechQueenUp)
                    {
                        spazmatismFlamethrowerMaxSpeed = 14f;
                        spazmatismFlamethrowerTargetX = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - spazmatismFlamethrowerPos.X;
                        spazmatismFlamethrowerTargetY = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 300f - spazmatismFlamethrowerPos.Y;
                        spazmatismFlamethrowerTargetX = mechQueenSpacing.X;
                        spazmatismFlamethrowerTargetY = mechQueenSpacing.Y;
                        spazmatismFlamethrowerTargetX -= spazmatismFlamethrowerPos.X;
                        spazmatismFlamethrowerTargetY -= spazmatismFlamethrowerPos.Y;
                        spazmatismFlamethrowerTargetDist = (float)Math.Sqrt(spazmatismFlamethrowerTargetX * spazmatismFlamethrowerTargetX + spazmatismFlamethrowerTargetY * spazmatismFlamethrowerTargetY);
                        if (spazmatismFlamethrowerTargetDist > spazmatismFlamethrowerMaxSpeed)
                        {
                            spazmatismFlamethrowerTargetDist = spazmatismFlamethrowerMaxSpeed / spazmatismFlamethrowerTargetDist;
                            spazmatismFlamethrowerTargetX *= spazmatismFlamethrowerTargetDist;
                            spazmatismFlamethrowerTargetY *= spazmatismFlamethrowerTargetDist;
                        }

                        npc.velocity.X = (npc.velocity.X * 59f + spazmatismFlamethrowerTargetX) / 60f;
                        npc.velocity.Y = (npc.velocity.Y * 59f + spazmatismFlamethrowerTargetY) / 60f;
                    }
                }

                // Charging phase
                else
                {
                    // Charge
                    if (npc.ai[1] == 1f)
                    {
                        // Play charge sound
                        SoundEngine.PlaySound(SoundID.Roar, npc.position);

                        // Set rotation and velocity
                        npc.rotation = spazmatismRotation;
                        float spazmatismPhase2ChargeSpeed = 16.75f + (death ? 5f * (0.7f - lifeRatio) : 0f);
                        spazmatismPhase2ChargeSpeed += 8f * enrageScale;
                        if (Main.getGoodWorld)
                            spazmatismPhase2ChargeSpeed *= 1.2f;

                        Vector2 distanceVector = Main.player[npc.target].Center - npc.Center;
                        npc.velocity = Vector2.Normalize(distanceVector) * spazmatismPhase2ChargeSpeed;
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
                            npc.rotation = spazmatismRotation;
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
                            float spazmatismPhase3ChargeSpeed = 20f + (death ? 6f * (0.7f - lifeRatio) : 0f);
                            spazmatismPhase3ChargeSpeed += 10f * enrageScale;
                            if (npc.ai[2] == -1f || (!retAlive && npc.ai[3] == secondFastCharge))
                                spazmatismPhase3ChargeSpeed *= 1.3f;
                            if (Main.getGoodWorld)
                                spazmatismPhase3ChargeSpeed *= 1.2f;

                            Vector2 distanceVector = Main.player[npc.target].Center + (!retAlive && bossRush ? Main.player[npc.target].velocity * 20f : Vector2.Zero) - npc.Center;
                            npc.velocity = Vector2.Normalize(distanceVector) * spazmatismPhase3ChargeSpeed;

                            if (retAlive)
                            {
                                Vector2 spazmatismPhase3ChargePos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                                float spazmatismPhase3ChargeTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - spazmatismPhase3ChargePos.X;
                                float spazmatismPhase3ChargeTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - spazmatismPhase3ChargePos.Y;
                                float spazmatismPhase3ChargeTargetDist = (float)Math.Sqrt(spazmatismPhase3ChargeTargetX * spazmatismPhase3ChargeTargetX + spazmatismPhase3ChargeTargetY * spazmatismPhase3ChargeTargetY);
                                float spazmatismPhase3ChargeTargetDistCopy = spazmatismPhase3ChargeTargetDist;

                                if (spazmatismPhase3ChargeTargetDistCopy < 100f)
                                {
                                    if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
                                    {
                                        float absoluteSpazXVel = Math.Abs(npc.velocity.X);
                                        float absoluteSpazYVel = Math.Abs(npc.velocity.Y);

                                        if (npc.Center.X > Main.player[npc.target].Center.X)
                                            absoluteSpazYVel *= -1f;
                                        if (npc.Center.Y > Main.player[npc.target].Center.Y)
                                            absoluteSpazXVel *= -1f;

                                        npc.velocity.X = absoluteSpazYVel;
                                        npc.velocity.Y = absoluteSpazXVel;
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
                            SoundEngine.PlaySound(SoundID.Roar, npc.position);

                        float spazmatismRetDeadChargeSpeed = ((!retAlive && npc.ai[3] == 4f) ? 75f : 50f) - (float)Math.Round(death ? 14f * (0.7f - lifeRatio) : 0f);

                        npc.ai[2] += 1f;

                        if (npc.ai[2] == spazmatismRetDeadChargeSpeed && Vector2.Distance(npc.position, Main.player[npc.target].position) < (retAlive ? 200f : 150f))
                            npc.ai[2] -= 1f;

                        // Slow down
                        if (npc.ai[2] >= spazmatismRetDeadChargeSpeed)
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
                        float spazmatismRetDeadChargeTimer = spazmatismRetDeadChargeSpeed + 25f;
                        if (npc.ai[2] >= spazmatismRetDeadChargeTimer)
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
                        float chargeLineUpDist = retAlive ? 600f : 500f;
                        float chargeSpeed = 16f + (death ? 5f * (0.7f - lifeRatio) : 0f);
                        float chargeAccel = 0.4f + (death ? 0.1f * (0.7f - lifeRatio) : 0f);

                        if (retAlive)
                        {
                            chargeSpeed *= 0.75f;
                            chargeAccel *= 0.75f;
                        }

                        if (Main.getGoodWorld)
                        {
                            chargeSpeed *= 1.15f;
                            chargeAccel *= 1.15f;
                        }

                        Vector2 spazmatismRetDeadChargePos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float chargeTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - spazmatismRetDeadChargePos.X;
                        float chargeTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) + chargeLineUpDist - spazmatismRetDeadChargePos.Y;
                        float chargeTargetDist = (float)Math.Sqrt(chargeTargetX * chargeTargetX + chargeTargetY * chargeTargetY);

                        chargeTargetDist = chargeSpeed / chargeTargetDist;
                        chargeTargetX *= chargeTargetDist;
                        chargeTargetY *= chargeTargetDist;

                        if (npc.velocity.X < chargeTargetX)
                        {
                            npc.velocity.X += chargeAccel;
                            if (npc.velocity.X < 0f && chargeTargetX > 0f)
                                npc.velocity.X += chargeAccel;
                        }
                        else if (npc.velocity.X > chargeTargetX)
                        {
                            npc.velocity.X -= chargeAccel;
                            if (npc.velocity.X > 0f && chargeTargetX < 0f)
                                npc.velocity.X -= chargeAccel;
                        }
                        if (npc.velocity.Y < chargeTargetY)
                        {
                            npc.velocity.Y += chargeAccel;
                            if (npc.velocity.Y < 0f && chargeTargetY > 0f)
                                npc.velocity.Y += chargeAccel;
                        }
                        else if (npc.velocity.Y > chargeTargetY)
                        {
                            npc.velocity.Y -= chargeAccel;
                            if (npc.velocity.Y > 0f && chargeTargetY < 0f)
                                npc.velocity.Y -= chargeAccel;
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
                                Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center + (!retAlive && bossRush ? Main.player[npc.target].velocity * 20f : Vector2.Zero) - npc.Center) * velocity;
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
