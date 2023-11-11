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
    public static class DukeFishronAI
    {
        public static bool BuffedDukeFishronAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Variables
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool phase2 = lifeRatio < 0.7f;
            bool phase3 = lifeRatio < 0.4f;
            bool phase4 = lifeRatio < 0.2f;
            bool phase2AI = npc.ai[0] > 4f;
            bool phase3AI = npc.ai[0] > 9f;
            bool charging = npc.ai[3] < 10f;

            // Adjust stats
            if (phase3AI)
            {
                npc.damage = (int)(npc.defDamage * 1.32f);
                npc.defense = 0;
            }
            else if (phase2AI)
            {
                npc.damage = (int)(npc.defDamage * 1.44f);
                npc.defense = (int)(npc.defDefense * 0.8f);
            }
            else
            {
                npc.damage = npc.defDamage;
                npc.defense = npc.defDefense;
            }

            int idlePhaseTimer = 30;
            float idlePhaseAcceleration = 0.55f;
            float idlePhaseVelocity = 8.5f;
            if (phase3AI)
            {
                idlePhaseAcceleration = 0.7f;
                idlePhaseVelocity = 12f;
            }
            else if (phase2AI & charging)
            {
                idlePhaseAcceleration = 0.6f;
                idlePhaseVelocity = 10f;
            }

            if (Main.getGoodWorld)
            {
                idlePhaseAcceleration *= 1.15f;
                idlePhaseVelocity *= 1.15f;
            }

            int chargeTime = 28;
            float chargeVelocity = 17f;
            if (phase3AI)
            {
                chargeTime = 25;
                chargeVelocity = 27f;
            }
            else if (charging & phase2AI)
            {
                chargeTime = 27;
                chargeVelocity = 21f;
            }

            if (death)
            {
                idlePhaseTimer = 28;
                idlePhaseAcceleration *= 1.05f;
                idlePhaseVelocity *= 1.08f;
                chargeTime -= 1;
                chargeVelocity *= 1.13f;
            }

            if (Main.getGoodWorld)
                chargeVelocity *= 1.15f;

            int bubbleBelchPhaseTimer = death ? 60 : 80;
            int bubbleBelchPhaseDivisor = death ? 3 : 4;
            float bubbleBelchPhaseAcceleration = death ? 0.35f : 0.3f;
            float bubbleBelchPhaseVelocity = death ? 5.5f : 5f;

            if (Main.getGoodWorld)
            {
                bubbleBelchPhaseAcceleration *= 1.5f;
                bubbleBelchPhaseVelocity *= 1.5f;
            }

            int sharknadoPhaseTimer = 90;

            int phaseTransitionTimer = 180;

            int teleportPhaseTimer = 30;

            int bubbleSpinPhaseTimer = bossRush ? 45 : death ? 90 : 120;
            int bubbleSpinPhaseDivisor = death ? 3 : 4;
            float bubbleSpinBubbleVelocity = death ? 8f : 7f;
            float bubbleSpinPhaseVelocity = 20f;
            float bubbleSpinPhaseRotation = MathHelper.TwoPi / (bubbleSpinPhaseTimer / 2);

            if (Main.getGoodWorld)
                bubbleSpinBubbleVelocity *= 1.5f;

            int spawnEffectPhaseTimer = 75;

            Player player = Main.player[npc.target];

            // Get target
            if (npc.target < 0 || npc.target == Main.maxPlayers || player.dead || !player.active)
            {
                npc.TargetClosest();
                player = Main.player[npc.target];
                npc.netUpdate = true;
            }

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Despawn
            if (player.dead || Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles)
            {
                npc.TargetClosest();

                npc.velocity.Y -= 0.4f;

                if (npc.timeLeft > 10)
                    npc.timeLeft = 10;

                if (npc.ai[0] > 4f)
                    npc.ai[0] = 5f;
                else
                    npc.ai[0] = 0f;

                npc.ai[2] = 0f;
            }

            // Enrage variable
            bool enrage = !bossRush &&
                (player.position.Y < 800f || player.position.Y > Main.worldSurface * 16.0 ||
                (player.position.X > 6400f && player.position.X < (Main.maxTilesX * 16 - 6400)));

            npc.Calamity().CurrentlyEnraged = !bossRush && enrage;

            // Make him always able to take damage
            npc.dontTakeDamage = false;

            // Increased DR during phase transitions
            calamityGlobalNPC.DR = (npc.ai[0] == -1f || npc.ai[0] == 4f || npc.ai[0] == 9f) ? (bossRush ? 0.99f : 0.625f) : 0.15f;
            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = npc.ai[0] == -1f || npc.ai[0] == 4f || npc.ai[0] == 9f;

            // Enrage
            if (enrage || bossRush)
            {
                bubbleBelchPhaseTimer = 20;
                bubbleBelchPhaseDivisor = 1;
                bubbleBelchPhaseAcceleration = 0.65f;
                bubbleBelchPhaseVelocity = 10f;
                idlePhaseTimer = 20;
                idlePhaseAcceleration = 1f;
                idlePhaseVelocity = 15f;
                chargeTime = 24;
                chargeVelocity += 5f;
                bubbleSpinPhaseDivisor = 1;
                bubbleSpinBubbleVelocity = 15f;

                if (!bossRush)
                {
                    npc.damage = npc.defDamage * 2;
                    npc.defense = npc.defDefense * 3;
                }
            }

            if (CalamityWorld.LegendaryMode)
                chargeTime += Main.rand.Next(5, 66);

            // Spawn cthulhunadoes in phase 3
            if (phase3AI && (!phase4 || Main.getGoodWorld))
            {
                calamityGlobalNPC.newAI[0] += 1f;
                float timeGateValue = 600f;
                if (calamityGlobalNPC.newAI[0] >= timeGateValue)
                {
                    calamityGlobalNPC.newAI[0] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, npc.target + 1);

                    npc.netUpdate = true;
                }
            }

            // Set variables for spawn effects
            if (npc.localAI[0] == 0f)
            {
                npc.localAI[0] = 1f;
                npc.alpha = 255;
                npc.rotation = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[0] = -1f;
                    npc.netUpdate = true;
                }
            }

            // Rotation
            float rateOfRotation = 0.04f;
            if (npc.ai[0] == 1f || npc.ai[0] == 6f || npc.ai[0] == 7f)
                rateOfRotation = 0f;
            if (npc.ai[0] == 3f || npc.ai[0] == 4f || npc.ai[0] == 8f)
                rateOfRotation = 0.01f;

            Vector2 rotationVector = player.Center - npc.Center;
            if (!player.dead && bossRush && phase4)
            {
                // Rotate to show direction of predictive charge
                if (npc.ai[0] == 10f)
                {
                    rateOfRotation = 0.1f;
                    rotationVector = Vector2.Normalize(player.Center + player.velocity * 20f - npc.Center) * chargeVelocity;
                }
            }

            float rotationSpeed = (float)Math.Atan2(rotationVector.Y, rotationVector.X);
            if (npc.spriteDirection == 1)
                rotationSpeed += MathHelper.Pi;
            if (rotationSpeed < 0f)
                rotationSpeed += MathHelper.TwoPi;
            if (rotationSpeed > MathHelper.TwoPi)
                rotationSpeed -= MathHelper.TwoPi;
            if (npc.ai[0] == -1f || npc.ai[0] == 3f || npc.ai[0] == 4f || npc.ai[0] == 8f)
                rotationSpeed = 0f;

            if (rateOfRotation != 0f)
                npc.rotation = npc.rotation.AngleTowards(rotationSpeed, rateOfRotation);

            // Alpha adjustments
            if (npc.ai[0] != -1f && npc.ai[0] < 9f)
            {
                if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                    npc.alpha += 15;
                else
                    npc.alpha -= 15;

                if (npc.alpha < 0)
                    npc.alpha = 0;
                if (npc.alpha > 150)
                    npc.alpha = 150;
            }

            // Spawn effects
            if (npc.ai[0] == -1f)
            {
                // Velocity
                npc.velocity *= 0.98f;

                // Direction
                int faceDirection = Math.Sign(player.Center.X - npc.Center.X);
                if (faceDirection != 0)
                {
                    npc.direction = faceDirection;
                    npc.spriteDirection = -npc.direction;
                }

                // Alpha
                if (npc.ai[2] > 20f)
                {
                    npc.velocity.Y = -2f;

                    npc.alpha -= 5;
                    if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                        npc.alpha += 15;
                    if (npc.alpha < 0)
                        npc.alpha = 0;
                    if (npc.alpha > 150)
                        npc.alpha = 150;
                }

                // Spawn dust and play sound
                if (npc.ai[2] == sharknadoPhaseTimer - 30)
                {
                    int dustAmt = 36;
                    for (int i = 0; i < dustAmt; i++)
                    {
                        Vector2 dust = (Vector2.Normalize(npc.velocity) * new Vector2(npc.width / 2f, npc.height) * 0.75f * 0.5f).RotatedBy((i - (dustAmt / 2 - 1)) * MathHelper.TwoPi / dustAmt) + npc.Center;
                        Vector2 sharknadoDustDirection = dust - npc.Center;
                        int sharknadoDust = Dust.NewDust(dust + sharknadoDustDirection, 0, 0, 172, sharknadoDustDirection.X * 2f, sharknadoDustDirection.Y * 2f, 100, default, 1.4f);
                        Main.dust[sharknadoDust].noGravity = true;
                        Main.dust[sharknadoDust].noLight = true;
                        Main.dust[sharknadoDust].velocity = Vector2.Normalize(sharknadoDustDirection) * 3f;
                    }

                    SoundEngine.PlaySound(SoundID.Zombie20,npc.Center);
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= spawnEffectPhaseTimer)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Phase 1
            else if (npc.ai[0] == 0f && !player.dead)
            {
                // Velocity
                if (npc.ai[1] == 0f)
                    npc.ai[1] = 300 * Math.Sign((npc.Center - player.Center).X);

                Vector2 idlePhaseDirection = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - npc.Center - npc.velocity) * idlePhaseVelocity;
                npc.SimpleFlyMovement(idlePhaseDirection, idlePhaseAcceleration);

                // Rotation and direction
                int playerFaceDirection = Math.Sign(player.Center.X - npc.Center.X);
                if (playerFaceDirection != 0)
                {
                    if (npc.ai[2] == 0f && playerFaceDirection != npc.direction)
                        npc.rotation += MathHelper.Pi;

                    npc.direction = playerFaceDirection;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += MathHelper.Pi;

                    npc.spriteDirection = -npc.direction;
                }

                // Phase switch
                npc.ai[2] += 1f;
                if (npc.ai[2] >= idlePhaseTimer || CalamityWorld.LegendaryMode)
                {
                    int attackPicker = 0;
                    switch ((int)npc.ai[3])
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                            attackPicker = 1;
                            break;
                        case 10:
                            npc.ai[3] = 1f;
                            attackPicker = 2;
                            break;
                        case 11:
                            npc.ai[3] = 0f;
                            attackPicker = 3;
                            break;
                    }

                    if (phase2)
                        attackPicker = 4;

                    // Set velocity for charge
                    if (attackPicker == 1)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;

                        // Velocity
                        npc.velocity = Vector2.Normalize(player.Center - npc.Center) * chargeVelocity;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        // Direction
                        if (playerFaceDirection != 0)
                        {
                            npc.direction = playerFaceDirection;

                            if (npc.spriteDirection == 1)
                                npc.rotation += MathHelper.Pi;

                            npc.spriteDirection = -npc.direction;
                        }
                    }

                    // Bubbles
                    else if (attackPicker == 2)
                    {
                        npc.ai[0] = 2f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    // Spawn sharknadoes
                    else if (attackPicker == 3)
                    {
                        npc.ai[0] = 3f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    // Go to phase 2
                    else if (attackPicker == 4)
                    {
                        npc.ai[0] = 4f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    npc.netUpdate = true;
                }
            }

            // Charge
            else if (npc.ai[0] == 1f)
            {
                // Accelerate
                npc.velocity *= 1.01f;

                // Spawn dust
                int chargeDustAmt = 7;
                for (int j = 0; j < chargeDustAmt; j++)
                {
                    Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((j - (chargeDustAmt / 2 - 1)) * MathHelper.Pi / chargeDustAmt) + npc.Center;
                    Vector2 chargeDustDirection = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int chargeDust = Dust.NewDust(arg_E1C_0 + chargeDustDirection, 0, 0, 172, chargeDustDirection.X * 2f, chargeDustDirection.Y * 2f, 100, default, 1.4f);
                    Main.dust[chargeDust].noGravity = true;
                    Main.dust[chargeDust].noLight = true;
                    Main.dust[chargeDust].velocity /= 4f;
                    Main.dust[chargeDust].velocity -= npc.velocity;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= chargeTime)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Bubble belch
            else if (npc.ai[0] == 2f)
            {
                // Velocity
                if (npc.ai[1] == 0f)
                    npc.ai[1] = 300 * Math.Sign((npc.Center - player.Center).X);

                Vector2 bubbleAttackDirection = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - npc.Center - npc.velocity) * bubbleBelchPhaseVelocity;
                npc.SimpleFlyMovement(bubbleAttackDirection, bubbleBelchPhaseAcceleration);

                // Play sounds and spawn bubbles
                if (npc.ai[2] == 0f)
                    SoundEngine.PlaySound(SoundID.Zombie20, npc.Center);

                if (npc.ai[2] % bubbleBelchPhaseDivisor == 0f)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath19, npc.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 bubbleSpawnDirection = Vector2.Normalize(player.Center - npc.Center) * (npc.width + 20) / 2f + npc.Center;
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)bubbleSpawnDirection.X, (int)bubbleSpawnDirection.Y + 45, NPCID.DetonatingBubble);
                    }
                }

                // Direction
                int bubbleSpriteFaceDirection = Math.Sign(player.Center.X - npc.Center.X);
                if (bubbleSpriteFaceDirection != 0)
                {
                    npc.direction = bubbleSpriteFaceDirection;
                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += MathHelper.Pi;
                    npc.spriteDirection = -npc.direction;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= bubbleBelchPhaseTimer)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Sharknado spawn
            else if (npc.ai[0] == 3f)
            {
                // Velocity
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                // Play sound and spawn sharknadoes
                if (npc.ai[2] == (sharknadoPhaseTimer - 30))
                    SoundEngine.PlaySound(SoundID.Zombie9, npc.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == sharknadoPhaseTimer - 30)
                {
                    Vector2 sharknadoSpawnerDirection = npc.rotation.ToRotationVector2() * (Vector2.UnitX * npc.direction) * (npc.width + 20) / 2f + npc.Center;
                    bool normal = Main.rand.NextBool();
                    float velocityY = normal ? 8f : -4f;
                    float ai1 = normal ? 0f : -1f;

                    Projectile.NewProjectile(npc.GetSource_FromAI(), sharknadoSpawnerDirection.X, sharknadoSpawnerDirection.Y, npc.direction * 3, velocityY, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 0f, ai1);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), sharknadoSpawnerDirection.X, sharknadoSpawnerDirection.Y, -(float)npc.direction * 3, velocityY, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 0f, ai1);

                    velocityY = normal ? -4f : 8f;
                    ai1 = normal ? -1f : 0f;
                    Projectile.NewProjectile(npc.GetSource_FromAI(), sharknadoSpawnerDirection.X, sharknadoSpawnerDirection.Y, 0f, velocityY, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 0f, ai1);
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= sharknadoPhaseTimer)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Transition to phase 2
            else if (npc.ai[0] == 4f)
            {
                // Velocity
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                // Sound
                if (npc.ai[2] == phaseTransitionTimer - 60)
                    SoundEngine.PlaySound(SoundID.Zombie20, npc.Center);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= phaseTransitionTimer)
                {
                    npc.ai[0] = 5f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Phase 2
            else if (npc.ai[0] == 5f && !player.dead)
            {
                // Velocity
                if (npc.ai[1] == 0f)
                    npc.ai[1] = 300 * Math.Sign((npc.Center - player.Center).X);

                Vector2 phase2IdleDirection = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - npc.Center - npc.velocity) * idlePhaseVelocity;
                npc.SimpleFlyMovement(phase2IdleDirection, idlePhaseAcceleration);

                // Direction and rotation
                int phase2SpriteFaceDirection = Math.Sign(player.Center.X - npc.Center.X);
                if (phase2SpriteFaceDirection != 0)
                {
                    if (npc.ai[2] == 0f && phase2SpriteFaceDirection != npc.direction)
                        npc.rotation += MathHelper.Pi;

                    npc.direction = phase2SpriteFaceDirection;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += MathHelper.Pi;

                    npc.spriteDirection = -npc.direction;
                }

                // Phase switch
                npc.ai[2] += 1f;
                if (npc.ai[2] >= idlePhaseTimer || CalamityWorld.LegendaryMode)
                {
                    int phase2AttackPicker = 0;
                    switch ((int)npc.ai[3])
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            phase2AttackPicker = 1;
                            break;
                        case 6:
                            npc.ai[3] = 1f;
                            phase2AttackPicker = 2;
                            break;
                        case 7:
                            npc.ai[3] = 0f;
                            phase2AttackPicker = 3;
                            break;
                    }

                    if (phase3)
                        phase2AttackPicker = 4;

                    // Set velocity for charge
                    if (phase2AttackPicker == 1)
                    {
                        npc.ai[0] = 6f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;

                        // Velocity and rotation
                        npc.velocity = Vector2.Normalize(player.Center - npc.Center) * chargeVelocity;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        // Direction
                        if (phase2SpriteFaceDirection != 0)
                        {
                            npc.direction = phase2SpriteFaceDirection;

                            if (npc.spriteDirection == 1)
                                npc.rotation += MathHelper.Pi;

                            npc.spriteDirection = -npc.direction;
                        }
                    }

                    // Set velocity for spin
                    else if (phase2AttackPicker == 2)
                    {
                        // Velocity and rotation
                        npc.velocity = Vector2.Normalize(player.Center - npc.Center) * bubbleSpinPhaseVelocity;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        // Direction
                        if (phase2SpriteFaceDirection != 0)
                        {
                            npc.direction = phase2SpriteFaceDirection;

                            if (npc.spriteDirection == 1)
                                npc.rotation += MathHelper.Pi;

                            npc.spriteDirection = -npc.direction;
                        }

                        npc.ai[0] = 7f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    // Spawn cthulhunado
                    else if (phase2AttackPicker == 3)
                    {
                        npc.ai[0] = 8f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    // Go to next phase
                    else if (phase2AttackPicker == 4)
                    {
                        npc.ai[0] = 9f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    npc.netUpdate = true;
                }
            }

            // Charge
            else if (npc.ai[0] == 6f)
            {
                // Accelerate
                npc.velocity *= 1.01f;

                // Spawn dust
                int phase2ChargeDustAmt = 7;
                for (int k = 0; k < phase2ChargeDustAmt; k++)
                {
                    Vector2 arg_1A97_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((k - (phase2ChargeDustAmt / 2 - 1)) * MathHelper.Pi / phase2ChargeDustAmt) + npc.Center;
                    Vector2 phase2ChargeDustDirection = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int phase2ChargeDust = Dust.NewDust(arg_1A97_0 + phase2ChargeDustDirection, 0, 0, 172, phase2ChargeDustDirection.X * 2f, phase2ChargeDustDirection.Y * 2f, 100, default, 1.4f);
                    Main.dust[phase2ChargeDust].noGravity = true;
                    Main.dust[phase2ChargeDust].noLight = true;
                    Main.dust[phase2ChargeDust].velocity /= 4f;
                    Main.dust[phase2ChargeDust].velocity -= npc.velocity;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= chargeTime)
                {
                    npc.ai[0] = 5f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Bubble spin
            else if (npc.ai[0] == 7f)
            {
                // Play sounds and spawn bubbles
                if (npc.ai[2] == 0f)
                    SoundEngine.PlaySound(SoundID.Zombie20, npc.Center);

                if (npc.ai[2] % bubbleSpinPhaseDivisor == 0f)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath19, npc.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 phase2BubbleSharkronDirection = Vector2.Normalize(npc.velocity) * (npc.width + 20) / 2f + npc.Center;
                        int phase2Bubbles = NPC.NewNPC(npc.GetSource_FromAI(), (int)phase2BubbleSharkronDirection.X, (int)phase2BubbleSharkronDirection.Y + 45, NPCID.DetonatingBubble);
                        Main.npc[phase2Bubbles].target = npc.target;
                        Main.npc[phase2Bubbles].velocity = Vector2.Normalize(npc.velocity).RotatedBy(MathHelper.PiOver2 * npc.direction) * bubbleSpinBubbleVelocity * (CalamityWorld.LegendaryMode ? (Main.rand.NextFloat() + 0.5f) : 1f);
                        Main.npc[phase2Bubbles].netUpdate = true;
                        Main.npc[phase2Bubbles].ai[3] = Main.rand.Next(80, 121) / 100f;

                        if (npc.ai[2] % (bubbleSpinPhaseDivisor * 5) == 0f)
                        {
                            int phase2BubbleSharkrons = NPC.NewNPC(npc.GetSource_FromAI(), (int)phase2BubbleSharkronDirection.X, (int)phase2BubbleSharkronDirection.Y + 45, NPCID.Sharkron2);
                            Main.npc[phase2BubbleSharkrons].ai[1] = 89f;
                        }
                    }
                }

                // Velocity and rotation
                npc.velocity = npc.velocity.RotatedBy(-(double)bubbleSpinPhaseRotation * (float)npc.direction);
                npc.rotation -= bubbleSpinPhaseRotation * npc.direction;

                npc.ai[2] += 1f;
                if (npc.ai[2] >= bubbleSpinPhaseTimer)
                {
                    npc.ai[0] = 5f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Spawn cthulhunado
            else if (npc.ai[0] == 8f)
            {
                // Velocity
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                // Play sound and spawn cthulhunado
                if (npc.ai[2] == sharknadoPhaseTimer - 30)
                    SoundEngine.PlaySound(SoundID.Zombie20, npc.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == sharknadoPhaseTimer - 30)
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, npc.target + 1);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= sharknadoPhaseTimer)
                {
                    npc.ai[0] = 5f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Transition to phase 3
            else if (npc.ai[0] == 9f)
            {
                // Alpha adjustments
                if (npc.ai[2] < phaseTransitionTimer - 90)
                {
                    if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                        npc.alpha += 15;
                    else
                        npc.alpha -= 15;

                    if (npc.alpha < 0)
                        npc.alpha = 0;
                    if (npc.alpha > 150)
                        npc.alpha = 150;
                }
                else if (npc.alpha < 255)
                {
                    npc.alpha += 4;
                    if (npc.alpha > 255)
                        npc.alpha = 255;
                }

                // Velocity
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                // Play sound
                if (npc.ai[2] == phaseTransitionTimer - 60)
                    SoundEngine.PlaySound(SoundID.Zombie20, npc.Center);

                npc.ai[2] += 1f;
                if (npc.ai[2] >= phaseTransitionTimer)
                {
                    npc.ai[0] = 10f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Phase 3
            else if (npc.ai[0] == 10f && !player.dead)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Alpha
                if (npc.alpha < 255)
                {
                    npc.alpha += 25;
                    if (npc.alpha > 255)
                        npc.alpha = 255;
                }

                // Teleport location
                if (npc.ai[1] == 0f)
                    npc.ai[1] = 360 * Math.Sign((npc.Center - player.Center).X);

                Vector2 desiredVelocity = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - npc.Center - npc.velocity) * idlePhaseVelocity;
                npc.SimpleFlyMovement(desiredVelocity, idlePhaseAcceleration);

                // Rotation and direction
                int phase3SpriteFaceDirection = Math.Sign(player.Center.X - npc.Center.X);
                if (phase3SpriteFaceDirection != 0)
                {
                    if (npc.ai[2] == 0f && phase3SpriteFaceDirection != npc.direction)
                    {
                        npc.rotation += MathHelper.Pi;
                        for (int l = 0; l < npc.oldPos.Length; l++)
                            npc.oldPos[l] = Vector2.Zero;
                    }

                    npc.direction = phase3SpriteFaceDirection;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += MathHelper.Pi;

                    npc.spriteDirection = -npc.direction;
                }

                // Phase switch
                npc.ai[2] += 1f;
                if (npc.ai[2] >= idlePhaseTimer || CalamityWorld.LegendaryMode)
                {
                    int phase3AttackPicker = 0;
                    if (phase4)
                    {
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                                phase3AttackPicker = 1;
                                break;
                            case 3:
                            case 8:
                                phase3AttackPicker = 2;
                                break;
                        }

                        if (death)
                            phase3AttackPicker = 1;
                    }
                    else
                    {
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                            case 2:
                            case 3:
                            case 5:
                            case 6:
                            case 7:
                                phase3AttackPicker = 1;
                                break;
                            case 1:
                            case 4:
                            case 8:
                                phase3AttackPicker = 2;
                                break;
                        }
                    }

                    // Set velocity for charge
                    if (phase3AttackPicker == 1)
                    {
                        npc.ai[0] = 11f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;

                        // Velocity and rotation
                        npc.velocity = Vector2.Normalize(player.Center + (bossRush && phase4 ? player.velocity * 20f : Vector2.Zero) - npc.Center) * chargeVelocity;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        // Direction
                        if (phase3SpriteFaceDirection != 0)
                        {
                            npc.direction = phase3SpriteFaceDirection;

                            if (npc.spriteDirection == 1)
                                npc.rotation += MathHelper.Pi;

                            npc.spriteDirection = -npc.direction;
                        }
                    }

                    // Pause
                    else if (phase3AttackPicker == 2)
                    {
                        npc.ai[0] = 12f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    npc.netUpdate = true;
                }
            }

            // Charge
            else if (npc.ai[0] == 11f)
            {
                // Accelerate
                npc.velocity *= 1.01f;

                // Alpha
                npc.alpha -= 25;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                // Spawn dust
                int phase3ChargeDustAmt = 7;
                for (int m = 0; m < phase3ChargeDustAmt; m++)
                {
                    Vector2 arg_2444_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((m - (phase3ChargeDustAmt / 2 - 1)) * MathHelper.Pi / phase3ChargeDustAmt) + npc.Center;
                    Vector2 phase3ChargeDustDirection = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int phase3ChargeDust = Dust.NewDust(arg_2444_0 + phase3ChargeDustDirection, 0, 0, 172, phase3ChargeDustDirection.X * 2f, phase3ChargeDustDirection.Y * 2f, 100, default, 1.4f);
                    Main.dust[phase3ChargeDust].noGravity = true;
                    Main.dust[phase3ChargeDust].noLight = true;
                    Main.dust[phase3ChargeDust].velocity /= 4f;
                    Main.dust[phase3ChargeDust].velocity -= npc.velocity;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= chargeTime)
                {
                    npc.ai[0] = 10f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;

                    if (!phase4 || !death)
                        npc.ai[3] += 1f;

                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Pause before teleport
            else if (npc.ai[0] == 12f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Alpha
                if (npc.alpha < 255)
                {
                    npc.alpha += 17;
                    if (npc.alpha > 255)
                        npc.alpha = 255;
                }

                // Velocity
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                // Play sound
                if (npc.ai[2] == teleportPhaseTimer / 2)
                    SoundEngine.PlaySound(SoundID.Zombie20, npc.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == teleportPhaseTimer / 2)
                {
                    // Teleport location
                    if (npc.ai[1] == 0f)
                        npc.ai[1] = 300 * Math.Sign((npc.Center - player.Center).X);

                    // Rotation and direction
                    Vector2 center = player.Center + new Vector2(-npc.ai[1], -200f);
                    npc.Center = center;
                    int phase3PlayerDirection = Math.Sign(player.Center.X - npc.Center.X);
                    if (phase3PlayerDirection != 0)
                    {
                        if (npc.ai[2] == 0f && phase3PlayerDirection != npc.direction)
                        {
                            npc.rotation += MathHelper.Pi;
                            for (int n = 0; n < npc.oldPos.Length; n++)
                                npc.oldPos[n] = Vector2.Zero;
                        }

                        npc.direction = phase3PlayerDirection;

                        if (npc.spriteDirection != -npc.direction)
                            npc.rotation += MathHelper.Pi;

                        npc.spriteDirection = -npc.direction;
                    }
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= teleportPhaseTimer)
                {
                    npc.ai[0] = 10f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;

                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= 9f)
                        npc.ai[3] = 0f;

                    npc.netUpdate = true;
                }
            }

            return false;
        }
    }
}
