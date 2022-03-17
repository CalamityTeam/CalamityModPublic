using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class DukeFishronAI
    {
        // Master Mode changes
        // 1 - Cycles between attacks faster, 
        // 2 - Moves faster, 
        // 3 - Bigger tornadoes
        public static bool BuffedDukeFishronAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;
			
            // Variables
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
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

            int bubbleBelchPhaseTimer = death ? 60 : 80;
            int bubbleBelchPhaseDivisor = death ? 3 : 4;
            float bubbleBelchPhaseAcceleration = death ? 0.35f : 0.3f;
            float bubbleBelchPhaseVelocity = death ? 5.5f : 5f;

            int sharknadoPhaseTimer = 90;

            int phaseTransitionTimer = 180;

            int teleportPhaseTimer = 30;

            int bubbleSpinPhaseTimer = malice ? 45 : death ? 90 : 120;
            int bubbleSpinPhaseDivisor = death ? 3 : 4;
            float bubbleSpinBubbleVelocity = death ? 8f : 7f;
            float bubbleSpinPhaseVelocity = 20f;
            float bubbleSpinPhaseRotation = MathHelper.TwoPi / (bubbleSpinPhaseTimer / 2);

            int spawnEffectPhaseTimer = 75;

            Vector2 vector = npc.Center;
            Player player = Main.player[npc.target];

            // Get target
            if (npc.target < 0 || npc.target == Main.maxPlayers || player.dead || !player.active)
            {
                npc.TargetClosest();
                player = Main.player[npc.target];
                npc.netUpdate = true;
            }

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(player.Center, vector) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Despawn
            if (player.dead || Vector2.Distance(player.Center, vector) > CalamityGlobalNPC.CatchUpDistance350Tiles)
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
            bool enrage = !BossRushEvent.BossRushActive &&
                (player.position.Y < 800f || player.position.Y > Main.worldSurface * 16.0 ||
                (player.position.X > 6400f && player.position.X < (Main.maxTilesX * 16 - 6400)));

            npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && enrage;

            // Enrage
            if (enrage || malice)
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

                if (!malice)
                {
                    npc.damage = npc.defDamage * 2;
                    npc.defense = npc.defDefense * 3;
                }
            }

            // Spawn cthulhunadoes in phase 3
            if (phase3AI && !phase4)
            {
                calamityGlobalNPC.newAI[0] += 1f;
                float timeGateValue = 600f;
                if (calamityGlobalNPC.newAI[0] >= timeGateValue)
                {
                    calamityGlobalNPC.newAI[0] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(vector, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, npc.target + 1);

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

            Vector2 rotationVector = player.Center - vector;
            if (!player.dead && malice && phase4)
            {
                // Rotate to show direction of predictive charge
                if (npc.ai[0] == 10f)
                {
                    rateOfRotation = 0.1f;
                    rotationVector = Vector2.Normalize(player.Center + player.velocity * 20f - vector) * chargeVelocity;
                }
            }

            float num17 = (float)Math.Atan2(rotationVector.Y, rotationVector.X);
            if (npc.spriteDirection == 1)
                num17 += MathHelper.Pi;
            if (num17 < 0f)
                num17 += MathHelper.TwoPi;
            if (num17 > MathHelper.TwoPi)
                num17 -= MathHelper.TwoPi;
            if (npc.ai[0] == -1f || npc.ai[0] == 3f || npc.ai[0] == 4f || npc.ai[0] == 8f)
                num17 = 0f;

            if (rateOfRotation != 0f)
                npc.rotation = npc.rotation.AngleTowards(num17, rateOfRotation);

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
                int num19 = Math.Sign(player.Center.X - vector.X);
                if (num19 != 0)
                {
                    npc.direction = num19;
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
                    int num20 = 36;
                    for (int i = 0; i < num20; i++)
                    {
                        Vector2 dust = (Vector2.Normalize(npc.velocity) * new Vector2(npc.width / 2f, npc.height) * 0.75f * 0.5f).RotatedBy((i - (num20 / 2 - 1)) * MathHelper.TwoPi / num20) + vector;
                        Vector2 vector2 = dust - vector;
                        int num21 = Dust.NewDust(dust + vector2, 0, 0, 172, vector2.X * 2f, vector2.Y * 2f, 100, default, 1.4f);
                        Main.dust[num21].noGravity = true;
                        Main.dust[num21].noLight = true;
                        Main.dust[num21].velocity = Vector2.Normalize(vector2) * 3f;
                    }

                    Main.PlaySound(SoundID.Zombie, (int)vector.X, (int)vector.Y, 20, 1f, 0f);
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
                    npc.ai[1] = 300 * Math.Sign((vector - player.Center).X);

                Vector2 vector3 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * idlePhaseVelocity;
                npc.SimpleFlyMovement(vector3, idlePhaseAcceleration);

                // Rotation and direction
                int num22 = Math.Sign(player.Center.X - vector.X);
                if (num22 != 0)
                {
                    if (npc.ai[2] == 0f && num22 != npc.direction)
                        npc.rotation += MathHelper.Pi;

                    npc.direction = num22;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += MathHelper.Pi;

                    npc.spriteDirection = -npc.direction;
                }

                // Phase switch
                npc.ai[2] += 1f;
                if (npc.ai[2] >= idlePhaseTimer)
                {
                    int num23 = 0;
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
                            num23 = 1;
                            break;
                        case 10:
                            npc.ai[3] = 1f;
                            num23 = 2;
                            break;
                        case 11:
                            npc.ai[3] = 0f;
                            num23 = 3;
                            break;
                    }

                    if (phase2)
                        num23 = 4;

                    // Set velocity for charge
                    if (num23 == 1)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;

                        // Velocity
                        npc.velocity = Vector2.Normalize(player.Center - vector) * chargeVelocity;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        // Direction
                        if (num22 != 0)
                        {
                            npc.direction = num22;

                            if (npc.spriteDirection == 1)
                                npc.rotation += MathHelper.Pi;

                            npc.spriteDirection = -npc.direction;
                        }
                    }

                    // Bubbles
                    else if (num23 == 2)
                    {
                        npc.ai[0] = 2f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    // Spawn sharknadoes
                    else if (num23 == 3)
                    {
                        npc.ai[0] = 3f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    // Go to phase 2
                    else if (num23 == 4)
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
                int num24 = 7;
                for (int j = 0; j < num24; j++)
                {
                    Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((j - (num24 / 2 - 1)) * MathHelper.Pi / num24) + vector;
                    Vector2 vector4 = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, 172, vector4.X * 2f, vector4.Y * 2f, 100, default, 1.4f);
                    Main.dust[num25].noGravity = true;
                    Main.dust[num25].noLight = true;
                    Main.dust[num25].velocity /= 4f;
                    Main.dust[num25].velocity -= npc.velocity;
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
                    npc.ai[1] = 300 * Math.Sign((vector - player.Center).X);

                Vector2 vector5 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * bubbleBelchPhaseVelocity;
                npc.SimpleFlyMovement(vector5, bubbleBelchPhaseAcceleration);

                // Play sounds and spawn bubbles
                if (npc.ai[2] == 0f)
                    Main.PlaySound(SoundID.Zombie, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

                if (npc.ai[2] % bubbleBelchPhaseDivisor == 0f)
                {
                    Main.PlaySound(SoundID.NPCKilled, (int)vector.X, (int)vector.Y, 19, 1f, 0f);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector6 = Vector2.Normalize(player.Center - vector) * (npc.width + 20) / 2f + vector;
                        NPC.NewNPC((int)vector6.X, (int)vector6.Y + 45, NPCID.DetonatingBubble);
                    }
                }

                // Direction
                int num26 = Math.Sign(player.Center.X - vector.X);
                if (num26 != 0)
                {
                    npc.direction = num26;
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
                    Main.PlaySound(SoundID.Zombie, (int)vector.X, (int)vector.Y, 9, 1f, 0f);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == sharknadoPhaseTimer - 30)
                {
                    Vector2 vector7 = npc.rotation.ToRotationVector2() * (Vector2.UnitX * npc.direction) * (npc.width + 20) / 2f + vector;
                    bool normal = Main.rand.NextBool();
                    float velocityY = normal ? 8f : -4f;
                    float ai1 = normal ? 0f : -1f;

                    Projectile.NewProjectile(vector7.X, vector7.Y, npc.direction * 3, velocityY, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 0f, ai1);
                    Projectile.NewProjectile(vector7.X, vector7.Y, -(float)npc.direction * 3, velocityY, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 0f, ai1);

                    velocityY = normal ? -4f : 8f;
                    ai1 = normal ? -1f : 0f;
                    Projectile.NewProjectile(vector7.X, vector7.Y, 0f, velocityY, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 0f, ai1);
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
                    Main.PlaySound(SoundID.Zombie, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

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
                    npc.ai[1] = 300 * Math.Sign((vector - player.Center).X);

                Vector2 vector8 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * idlePhaseVelocity;
                npc.SimpleFlyMovement(vector8, idlePhaseAcceleration);

                // Direction and rotation
                int num27 = Math.Sign(player.Center.X - vector.X);
                if (num27 != 0)
                {
                    if (npc.ai[2] == 0f && num27 != npc.direction)
                        npc.rotation += MathHelper.Pi;

                    npc.direction = num27;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += MathHelper.Pi;

                    npc.spriteDirection = -npc.direction;
                }

                // Phase switch
                npc.ai[2] += 1f;
                if (npc.ai[2] >= idlePhaseTimer)
                {
                    int num28 = 0;
                    switch ((int)npc.ai[3])
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            num28 = 1;
                            break;
                        case 6:
                            npc.ai[3] = 1f;
                            num28 = 2;
                            break;
                        case 7:
                            npc.ai[3] = 0f;
                            num28 = 3;
                            break;
                    }

                    if (phase3)
                        num28 = 4;

                    // Set velocity for charge
                    if (num28 == 1)
                    {
                        npc.ai[0] = 6f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;

                        // Velocity and rotation
                        npc.velocity = Vector2.Normalize(player.Center - vector) * chargeVelocity;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        // Direction
                        if (num27 != 0)
                        {
                            npc.direction = num27;

                            if (npc.spriteDirection == 1)
                                npc.rotation += MathHelper.Pi;

                            npc.spriteDirection = -npc.direction;
                        }
                    }

                    // Set velocity for spin
                    else if (num28 == 2)
                    {
                        // Velocity and rotation
                        npc.velocity = Vector2.Normalize(player.Center - vector) * bubbleSpinPhaseVelocity;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        // Direction
                        if (num27 != 0)
                        {
                            npc.direction = num27;

                            if (npc.spriteDirection == 1)
                                npc.rotation += MathHelper.Pi;

                            npc.spriteDirection = -npc.direction;
                        }

                        npc.ai[0] = 7f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    // Spawn cthulhunado
                    else if (num28 == 3)
                    {
                        npc.ai[0] = 8f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }

                    // Go to next phase
                    else if (num28 == 4)
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
                int num29 = 7;
                for (int k = 0; k < num29; k++)
                {
                    Vector2 arg_1A97_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((k - (num29 / 2 - 1)) * MathHelper.Pi / num29) + vector;
                    Vector2 vector9 = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int num30 = Dust.NewDust(arg_1A97_0 + vector9, 0, 0, 172, vector9.X * 2f, vector9.Y * 2f, 100, default, 1.4f);
                    Main.dust[num30].noGravity = true;
                    Main.dust[num30].noLight = true;
                    Main.dust[num30].velocity /= 4f;
                    Main.dust[num30].velocity -= npc.velocity;
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
                    Main.PlaySound(SoundID.Zombie, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

                if (npc.ai[2] % bubbleSpinPhaseDivisor == 0f)
                {
                    Main.PlaySound(SoundID.NPCKilled, (int)vector.X, (int)vector.Y, 19, 1f, 0f);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector10 = Vector2.Normalize(npc.velocity) * (npc.width + 20) / 2f + vector;
                        int num31 = NPC.NewNPC((int)vector10.X, (int)vector10.Y + 45, NPCID.DetonatingBubble);
                        Main.npc[num31].target = npc.target;
                        Main.npc[num31].velocity = Vector2.Normalize(npc.velocity).RotatedBy(MathHelper.PiOver2 * npc.direction) * bubbleSpinBubbleVelocity;
                        Main.npc[num31].netUpdate = true;
                        Main.npc[num31].ai[3] = Main.rand.Next(80, 121) / 100f;

                        if (npc.ai[2] % (bubbleSpinPhaseDivisor * 5) == 0f)
                        {
                            int npc2 = NPC.NewNPC((int)vector10.X, (int)vector10.Y + 45, NPCID.Sharkron2);
                            Main.npc[npc2].ai[1] = 89f;
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
                    Main.PlaySound(SoundID.Zombie, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == sharknadoPhaseTimer - 30)
                    Projectile.NewProjectile(vector.X, vector.Y, 0f, 0f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, npc.target + 1);

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
                    Main.PlaySound(SoundID.Zombie, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

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
                // Alpha
                if (npc.alpha < 255)
                {
                    npc.alpha += 25;
                    if (npc.alpha > 255)
                        npc.alpha = 255;
                }

                // Teleport location
                if (npc.ai[1] == 0f)
                    npc.ai[1] = 360 * Math.Sign((vector - player.Center).X);

                Vector2 desiredVelocity = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * idlePhaseVelocity;
                npc.SimpleFlyMovement(desiredVelocity, idlePhaseAcceleration);

                // Rotation and direction
                int num32 = Math.Sign(player.Center.X - vector.X);
                if (num32 != 0)
                {
                    if (npc.ai[2] == 0f && num32 != npc.direction)
                    {
                        npc.rotation += MathHelper.Pi;
                        for (int l = 0; l < npc.oldPos.Length; l++)
                            npc.oldPos[l] = Vector2.Zero;
                    }

                    npc.direction = num32;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += MathHelper.Pi;

                    npc.spriteDirection = -npc.direction;
                }

                // Phase switch
                npc.ai[2] += 1f;
                if (npc.ai[2] >= idlePhaseTimer)
                {
                    int num33 = 0;
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
                                num33 = 1;
                                break;
                            case 3:
                            case 8:
                                num33 = 2;
                                break;
                        }

                        if (death)
                            num33 = 1;
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
                                num33 = 1;
                                break;
                            case 1:
                            case 4:
                            case 8:
                                num33 = 2;
                                break;
                        }
                    }

                    // Set velocity for charge
                    if (num33 == 1)
                    {
                        npc.ai[0] = 11f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;

                        // Velocity and rotation
                        npc.velocity = Vector2.Normalize(player.Center + (malice && phase4 ? player.velocity * 20f : Vector2.Zero) - vector) * chargeVelocity;
                        npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                        // Direction
                        if (num32 != 0)
                        {
                            npc.direction = num32;

                            if (npc.spriteDirection == 1)
                                npc.rotation += MathHelper.Pi;

                            npc.spriteDirection = -npc.direction;
                        }
                    }

                    // Pause
                    else if (num33 == 2)
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
                int num34 = 7;
                for (int m = 0; m < num34; m++)
                {
                    Vector2 arg_2444_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((m - (num34 / 2 - 1)) * MathHelper.Pi / num34) + vector;
                    Vector2 vector11 = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int num35 = Dust.NewDust(arg_2444_0 + vector11, 0, 0, 172, vector11.X * 2f, vector11.Y * 2f, 100, default, 1.4f);
                    Main.dust[num35].noGravity = true;
                    Main.dust[num35].noLight = true;
                    Main.dust[num35].velocity /= 4f;
                    Main.dust[num35].velocity -= npc.velocity;
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
                    Main.PlaySound(SoundID.Zombie, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == teleportPhaseTimer / 2)
                {
                    // Teleport location
                    if (npc.ai[1] == 0f)
                        npc.ai[1] = 300 * Math.Sign((vector - player.Center).X);

                    // Rotation and direction
                    Vector2 center = player.Center + new Vector2(-npc.ai[1], -200f);
                    vector = npc.Center = center;
                    int num36 = Math.Sign(player.Center.X - vector.X);
                    if (num36 != 0)
                    {
                        if (npc.ai[2] == 0f && num36 != npc.direction)
                        {
                            npc.rotation += MathHelper.Pi;
                            for (int n = 0; n < npc.oldPos.Length; n++)
                                npc.oldPos[n] = Vector2.Zero;
                        }

                        npc.direction = num36;

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
