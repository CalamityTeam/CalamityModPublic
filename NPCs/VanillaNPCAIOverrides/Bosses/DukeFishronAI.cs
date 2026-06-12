using System;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    public class DukeFishronAI : VanillaAIOverride
    {
        // Vanilla values
        public static float Phase2ContactDamageMult = 1.436f; // 201
        public static float Phase3ContactDamageMult = 1.315f; // 184

        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Variables
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool phase2 = lifeRatio < 0.7f;
            bool phase3 = lifeRatio < 0.4f;
            bool phase4 = lifeRatio < 0.2f;
            bool phase2AI = NPC.ai[0] > 4f;
            bool phase3AI = NPC.ai[0] > 9f;
            bool charging = NPC.ai[3] < 10f;

            // Adjust stats
            NPC.damage = NPC.defDamage;
            if (phase3AI)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * Phase3ContactDamageMult);
                NPC.defense = 0;
            }
            else if (phase2AI)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * Phase2ContactDamageMult);
                NPC.defense = (int)Math.Round(NPC.defDefense * 0.8);
            }
            else
                NPC.defense = NPC.defDefense;

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
                chargeVelocity *= 1.1f;
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

            int bubbleSpinPhaseTimer = death ? 90 : 120;
            int bubbleSpinPhaseDivisor = death ? 3 : 4;
            float bubbleSpinBubbleVelocity = death ? 8f : 7f;
            float bubbleSpinPhaseVelocity = 20f;
            float bubbleSpinPhaseRotation = MathHelper.TwoPi / (bubbleSpinPhaseTimer / 2);

            if (Main.getGoodWorld)
                bubbleSpinBubbleVelocity *= 1.5f;

            int spawnEffectPhaseTimer = 75;

            Player player = Main.player[NPC.target];

            // Get target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || player.dead || !player.active || Vector2.Distance(player.Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles)
            {
                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                player = Main.player[NPC.target];
                NPC.netUpdate = true;
            }

            // Despawn
            if (player.dead || Vector2.Distance(player.Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles)
            {
                NPC.velocity.Y -= 0.4f;

                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10;

                if (NPC.ai[0] > 4f)
                    NPC.ai[0] = 5f;
                else
                    NPC.ai[0] = 0f;

                NPC.ai[2] = 0f;
            }

            // Enrage variable
            bool enrage = !BossRushEvent.BossRushActive &&
                (player.position.Y < 800f || player.position.Y > Main.worldSurface * 16.0 ||
                (player.position.X > 6400f && player.position.X < (Main.maxTilesX * 16 - 6400)));

            calamityGlobalNPC.CurrentlyEnraged = enrage;

            // Make him always able to take damage
            NPC.dontTakeDamage = false;

            // Increased DR during phase transitions
            calamityGlobalNPC.DR = (NPC.ai[0] == -1f || NPC.ai[0] == 4f || NPC.ai[0] == 9f) ? 0.625f : 0.15f;
            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = NPC.ai[0] == -1f || NPC.ai[0] == 4f || NPC.ai[0] == 9f;

            // Enrage
            if (enrage)
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

                NPC.damage *= 2;
                NPC.defense = NPC.defDefense * 3;
            }

            if (death)
            {
                chargeTime -= 2;
                chargeVelocity += 1f;
            }

            if (Main.getGoodWorld)
                chargeTime += Main.rand.Next(5, 66);

            // Spawn cthulhunadoes in phase 3
            if (phase3AI && ((!phase4) || Main.getGoodWorld))
            {
                calamityGlobalNPC.newAI[0] += 1f;
                float timeGateValue = 600f;
                if (calamityGlobalNPC.newAI[0] >= timeGateValue)
                {
                    calamityGlobalNPC.newAI[0] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, NPC.target + 1, (enrage || death) ? 1 : 0);

                    NPC.netUpdate = true;
                }
            }

            // Set variables for spawn effects
            if (NPC.localAI[0] == 0f)
            {
                NPC.localAI[0] = 1f;
                NPC.alpha = 255;
                NPC.rotation = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[0] = -1f;
                    NPC.netUpdate = true;
                }
            }

            // Rotation
            float rateOfRotation = 0.04f;
            if (NPC.ai[0] == 1f || NPC.ai[0] == 6f || NPC.ai[0] == 7f)
                rateOfRotation = 0f;
            if (NPC.ai[0] == 3f || NPC.ai[0] == 4f || NPC.ai[0] == 8f)
                rateOfRotation = 0.01f;

            Vector2 rotationVector = player.Center - NPC.Center;

            float rotationSpeed = (float)Math.Atan2(rotationVector.Y, rotationVector.X);
            if (NPC.spriteDirection == 1)
                rotationSpeed += MathHelper.Pi;
            if (rotationSpeed < 0f)
                rotationSpeed += MathHelper.TwoPi;
            if (rotationSpeed > MathHelper.TwoPi)
                rotationSpeed -= MathHelper.TwoPi;
            if (NPC.ai[0] == -1f || NPC.ai[0] == 3f || NPC.ai[0] == 4f || NPC.ai[0] == 8f)
                rotationSpeed = 0f;

            if (rateOfRotation != 0f)
                NPC.rotation = NPC.rotation.AngleTowards(rotationSpeed, rateOfRotation);

            // Alpha adjustments
            if (NPC.ai[0] != -1f && NPC.ai[0] < 9f)
            {
                if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    NPC.alpha += 15;
                else
                    NPC.alpha -= 15;

                if (NPC.alpha < 0)
                    NPC.alpha = 0;
                if (NPC.alpha > 150)
                    NPC.alpha = 150;
            }

            // Spawn effects
            if (NPC.ai[0] == -1f)
            {
                // Disable contact damage while spawning
                NPC.damage = 0;

                // Velocity
                NPC.velocity *= 0.98f;

                // Direction
                int faceDirection = Math.Sign(player.Center.X - NPC.Center.X);
                if (faceDirection != 0)
                {
                    NPC.direction = faceDirection;
                    NPC.spriteDirection = -NPC.direction;
                }

                // Alpha
                if (NPC.ai[2] > 20f)
                {
                    NPC.velocity.Y = -2f;

                    NPC.alpha -= 5;
                    if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        NPC.alpha += 15;
                    if (NPC.alpha < 0)
                        NPC.alpha = 0;
                    if (NPC.alpha > 150)
                        NPC.alpha = 150;
                }

                // Spawn dust and play sound
                if (NPC.ai[2] == sharknadoPhaseTimer - 30)
                {
                    int dustAmt = 36;
                    for (int i = 0; i < dustAmt; i++)
                    {
                        Vector2 dust = (Vector2.Normalize(NPC.velocity) * new Vector2(NPC.width / 2f, NPC.height) * 0.75f * 0.5f).RotatedBy((i - (dustAmt / 2 - 1)) * MathHelper.TwoPi / dustAmt) + NPC.Center;
                        Vector2 sharknadoDustDirection = dust - NPC.Center;
                        int sharknadoDust = Dust.NewDust(dust + sharknadoDustDirection, 0, 0, DustID.DungeonWater, sharknadoDustDirection.X * 2f, sharknadoDustDirection.Y * 2f, 100, default, 1.4f);
                        Main.dust[sharknadoDust].noGravity = true;
                        Main.dust[sharknadoDust].noLight = true;
                        Main.dust[sharknadoDust].velocity = Vector2.Normalize(sharknadoDustDirection) * 3f;
                    }

                    SoundEngine.PlaySound(SoundID.Zombie20, NPC.Center);
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= spawnEffectPhaseTimer)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Phase 1
            else if (NPC.ai[0] == 0f && !player.dead)
            {
                // Velocity
                if (NPC.ai[1] == 0f)
                    NPC.ai[1] = 300 * Math.Sign((NPC.Center - player.Center).X);

                Vector2 idlePhaseDirection = Vector2.Normalize(player.Center + new Vector2(NPC.ai[1], -200f) - NPC.Center - NPC.velocity) * idlePhaseVelocity;
                NPC.SimpleFlyMovement(idlePhaseDirection, idlePhaseAcceleration);

                // Rotation and direction
                int playerFaceDirection = Math.Sign(player.Center.X - NPC.Center.X);
                if (playerFaceDirection != 0)
                {
                    if (NPC.ai[2] == 0f && playerFaceDirection != NPC.direction)
                        NPC.rotation += MathHelper.Pi;

                    NPC.direction = playerFaceDirection;

                    if (NPC.spriteDirection != -NPC.direction)
                        NPC.rotation += MathHelper.Pi;

                    NPC.spriteDirection = -NPC.direction;
                }

                // Phase switch
                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= idlePhaseTimer || Main.zenithWorld)
                {
                    int attackPicker = 0;
                    switch ((int)NPC.ai[3])
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
                            NPC.ai[3] = 1f;
                            attackPicker = 2;
                            break;
                        case 11:
                            NPC.ai[3] = 0f;
                            attackPicker = 3;
                            break;
                    }

                    if (enrage && attackPicker == 2)
                        attackPicker = 3;

                    if (phase2)
                        attackPicker = 4;

                    // Set velocity for charge
                    if (attackPicker == 1)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;

                        // Velocity
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * chargeVelocity;
                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                        // Direction
                        if (playerFaceDirection != 0)
                        {
                            NPC.direction = playerFaceDirection;

                            if (NPC.spriteDirection == 1)
                                NPC.rotation += MathHelper.Pi;

                            NPC.spriteDirection = -NPC.direction;
                        }
                    }

                    // Bubbles
                    else if (attackPicker == 2)
                    {
                        NPC.ai[0] = 2f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                    }

                    // Spawn sharknadoes
                    else if (attackPicker == 3)
                    {
                        NPC.ai[0] = 3f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        if (enrage)
                            NPC.ai[2] = sharknadoPhaseTimer - 40;
                        else if (death)
                            NPC.ai[2] = sharknadoPhaseTimer - 40;
                    }

                    // Go to phase 2
                    else if (attackPicker == 4)
                    {
                        NPC.ai[0] = 4f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                    }

                    NPC.netUpdate = true;
                }
            }

            // Charge
            else if (NPC.ai[0] == 1f)
            {
                // Accelerate
                NPC.velocity *= 1.01f;

                // Spawn dust
                int chargeDustAmt = 7;
                for (int j = 0; j < chargeDustAmt; j++)
                {
                    Vector2 arg_E1C_0 = (Vector2.Normalize(NPC.velocity) * new Vector2((NPC.width + 50) / 2f, NPC.height) * 0.75f).RotatedBy((j - (chargeDustAmt / 2 - 1)) * MathHelper.Pi / chargeDustAmt) + NPC.Center;
                    Vector2 chargeDustDirection = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int chargeDust = Dust.NewDust(arg_E1C_0 + chargeDustDirection, 0, 0, DustID.DungeonWater, chargeDustDirection.X * 2f, chargeDustDirection.Y * 2f, 100, default, 1.4f);
                    Main.dust[chargeDust].noGravity = true;
                    Main.dust[chargeDust].noLight = true;
                    Main.dust[chargeDust].velocity /= 4f;
                    Main.dust[chargeDust].velocity -= NPC.velocity;
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= chargeTime)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] += 2f;
                    NPC.netUpdate = true;
                }
            }

            // Bubble belch
            else if (NPC.ai[0] == 2f)
            {
                // Velocity
                if (NPC.ai[1] == 0f)
                    NPC.ai[1] = 300 * Math.Sign((NPC.Center - player.Center).X);

                Vector2 bubbleAttackDirection = Vector2.Normalize(player.Center + new Vector2(NPC.ai[1], -200f) - NPC.Center - NPC.velocity) * bubbleBelchPhaseVelocity;
                NPC.SimpleFlyMovement(bubbleAttackDirection, bubbleBelchPhaseAcceleration);

                // Play sounds and spawn bubbles
                if (NPC.ai[2] == 0f)
                    SoundEngine.PlaySound(SoundID.Zombie20, NPC.Center);

                if (NPC.ai[2] % bubbleBelchPhaseDivisor == 0f)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath19, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 bubbleSpawnDirection = Vector2.Normalize(player.Center - NPC.Center) * (NPC.width + 20) / 2f + NPC.Center;
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)bubbleSpawnDirection.X, (int)bubbleSpawnDirection.Y + 45, NPCID.DetonatingBubble);
                    }
                }

                // Direction
                int bubbleSpriteFaceDirection = Math.Sign(player.Center.X - NPC.Center.X);
                if (bubbleSpriteFaceDirection != 0)
                {
                    NPC.direction = bubbleSpriteFaceDirection;
                    if (NPC.spriteDirection != -NPC.direction)
                        NPC.rotation += MathHelper.Pi;
                    NPC.spriteDirection = -NPC.direction;
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= bubbleBelchPhaseTimer)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Sharknado spawn
            else if (NPC.ai[0] == 3f)
            {
                // Velocity
                NPC.velocity *= 0.98f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0f, 0.02f);

                // Play sound and spawn sharknadoes
                if (NPC.ai[2] == (sharknadoPhaseTimer - 30))
                    SoundEngine.PlaySound(SoundID.Zombie9, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] == sharknadoPhaseTimer - 30)
                {
                    Vector2 sharknadoSpawnerDirection = NPC.rotation.ToRotationVector2() * (Vector2.UnitX * NPC.direction) * (NPC.width + 20) / 2f + NPC.Center;
                    bool normal = Main.rand.NextBool();
                    float velocityY = normal ? 8f : -4f;
                    float ai1 = normal ? 0f : -1f;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), sharknadoSpawnerDirection.X, sharknadoSpawnerDirection.Y, NPC.direction * 3, velocityY, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 0f, ai1);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), sharknadoSpawnerDirection.X, sharknadoSpawnerDirection.Y, -(float)NPC.direction * 3, velocityY, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 0f, ai1);

                    velocityY = normal ? -4f : 8f;
                    ai1 = normal ? -1f : 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), sharknadoSpawnerDirection.X, sharknadoSpawnerDirection.Y, 0f, velocityY, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 0f, ai1);
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= sharknadoPhaseTimer)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Transition to phase 2
            else if (NPC.ai[0] == 4f)
            {
                // Velocity
                NPC.velocity *= 0.98f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0f, 0.02f);

                // Sound
                if (NPC.ai[2] == phaseTransitionTimer - 60)
                    SoundEngine.PlaySound(SoundID.Zombie20, NPC.Center);

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= phaseTransitionTimer)
                {
                    NPC.ai[0] = 5f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Phase 2
            else if (NPC.ai[0] == 5f && !player.dead)
            {
                // Velocity
                if (NPC.ai[1] == 0f)
                    NPC.ai[1] = 300 * Math.Sign((NPC.Center - player.Center).X);

                Vector2 phase2IdleDirection = Vector2.Normalize(player.Center + new Vector2(NPC.ai[1], -200f) - NPC.Center - NPC.velocity) * idlePhaseVelocity;
                NPC.SimpleFlyMovement(phase2IdleDirection, idlePhaseAcceleration);

                // Direction and rotation
                int phase2SpriteFaceDirection = Math.Sign(player.Center.X - NPC.Center.X);
                if (phase2SpriteFaceDirection != 0)
                {
                    if (NPC.ai[2] == 0f && phase2SpriteFaceDirection != NPC.direction)
                        NPC.rotation += MathHelper.Pi;

                    NPC.direction = phase2SpriteFaceDirection;

                    if (NPC.spriteDirection != -NPC.direction)
                        NPC.rotation += MathHelper.Pi;

                    NPC.spriteDirection = -NPC.direction;
                }

                // Phase switch
                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= idlePhaseTimer || Main.zenithWorld)
                {
                    int phase2AttackPicker = 0;
                    switch ((int)NPC.ai[3])
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
                            NPC.ai[3] = 1f;
                            phase2AttackPicker = 2;
                            break;
                        case 7:
                            NPC.ai[3] = 0f;
                            phase2AttackPicker = 3;
                            break;
                    }

                    if (enrage && phase2AttackPicker == 2)
                        phase2AttackPicker = 3;

                    if (phase3)
                        phase2AttackPicker = 4;

                    // Set velocity for charge
                    if (phase2AttackPicker == 1)
                    {
                        NPC.ai[0] = 6f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;

                        // Velocity and rotation
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * chargeVelocity;
                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                        // Direction
                        if (phase2SpriteFaceDirection != 0)
                        {
                            NPC.direction = phase2SpriteFaceDirection;

                            if (NPC.spriteDirection == 1)
                                NPC.rotation += MathHelper.Pi;

                            NPC.spriteDirection = -NPC.direction;
                        }
                    }

                    // Set velocity for spin
                    else if (phase2AttackPicker == 2)
                    {
                        // Velocity and rotation
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * bubbleSpinPhaseVelocity;
                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                        // Direction
                        if (phase2SpriteFaceDirection != 0)
                        {
                            NPC.direction = phase2SpriteFaceDirection;

                            if (NPC.spriteDirection == 1)
                                NPC.rotation += MathHelper.Pi;

                            NPC.spriteDirection = -NPC.direction;
                        }

                        NPC.ai[0] = 7f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                    }

                    // Spawn cthulhunado
                    else if (phase2AttackPicker == 3)
                    {
                        NPC.ai[0] = 8f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                    }

                    // Go to next phase
                    else if (phase2AttackPicker == 4)
                    {
                        NPC.ai[0] = 9f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                    }

                    NPC.netUpdate = true;
                }
            }

            // Charge
            else if (NPC.ai[0] == 6f)
            {
                // Accelerate
                NPC.velocity *= 1.01f;

                // Spawn dust
                int phase2ChargeDustAmt = 7;
                for (int k = 0; k < phase2ChargeDustAmt; k++)
                {
                    Vector2 arg_1A97_0 = (Vector2.Normalize(NPC.velocity) * new Vector2((NPC.width + 50) / 2f, NPC.height) * 0.75f).RotatedBy((k - (phase2ChargeDustAmt / 2 - 1)) * MathHelper.Pi / phase2ChargeDustAmt) + NPC.Center;
                    Vector2 phase2ChargeDustDirection = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int phase2ChargeDust = Dust.NewDust(arg_1A97_0 + phase2ChargeDustDirection, 0, 0, DustID.DungeonWater, phase2ChargeDustDirection.X * 2f, phase2ChargeDustDirection.Y * 2f, 100, default, 1.4f);
                    Main.dust[phase2ChargeDust].noGravity = true;
                    Main.dust[phase2ChargeDust].noLight = true;
                    Main.dust[phase2ChargeDust].velocity /= 4f;
                    Main.dust[phase2ChargeDust].velocity -= NPC.velocity;
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= chargeTime)
                {
                    NPC.ai[0] = 5f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] += 2f;
                    NPC.netUpdate = true;
                }
            }

            // Bubble spin
            else if (NPC.ai[0] == 7f)
            {
                // Play sounds and spawn bubbles
                if (NPC.ai[2] == 0f)
                    SoundEngine.PlaySound(SoundID.Zombie20, NPC.Center);

                if (NPC.ai[2] % bubbleSpinPhaseDivisor == 0f)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath19, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 phase2BubbleSharkronDirection = Vector2.Normalize(NPC.velocity) * (NPC.width + 20) / 2f + NPC.Center;
                        int phase2Bubbles = NPC.NewNPC(NPC.GetSource_FromAI(), (int)phase2BubbleSharkronDirection.X, (int)phase2BubbleSharkronDirection.Y + 45, NPCID.DetonatingBubble);
                        Main.npc[phase2Bubbles].target = NPC.target;
                        Main.npc[phase2Bubbles].velocity = Vector2.Normalize(NPC.velocity).RotatedBy(MathHelper.PiOver2 * NPC.direction) * bubbleSpinBubbleVelocity * (Main.getGoodWorld ? (Main.rand.NextFloat() + 0.5f) : 1f);
                        Main.npc[phase2Bubbles].netUpdate = true;
                        Main.npc[phase2Bubbles].ai[3] = Main.rand.Next(80, 121) / 100f;

                        if (NPC.ai[2] % (bubbleSpinPhaseDivisor * 5) == 0f)
                        {
                            int phase2BubbleSharkrons = NPC.NewNPC(NPC.GetSource_FromAI(), (int)phase2BubbleSharkronDirection.X, (int)phase2BubbleSharkronDirection.Y + 45, NPCID.Sharkron2);
                            Main.npc[phase2BubbleSharkrons].ai[1] = 89f;
                        }
                    }
                }

                // Velocity and rotation
                NPC.velocity = NPC.velocity.RotatedBy(-(double)bubbleSpinPhaseRotation * (float)NPC.direction);
                NPC.rotation -= bubbleSpinPhaseRotation * NPC.direction;

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= bubbleSpinPhaseTimer)
                {
                    NPC.ai[0] = 5f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Spawn cthulhunado
            else if (NPC.ai[0] == 8f)
            {
                // Velocity
                NPC.velocity *= 0.98f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0f, 0.02f);

                // Play sound and spawn cthulhunado
                if (NPC.ai[2] == sharknadoPhaseTimer - 30)
                    SoundEngine.PlaySound(SoundID.Zombie20, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] == sharknadoPhaseTimer - 30)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, NPC.target + 1, (enrage || death) ? 1 : 0);

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= sharknadoPhaseTimer)
                {
                    NPC.ai[0] = 5f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Transition to phase 3
            else if (NPC.ai[0] == 9f)
            {
                // Alpha adjustments
                if (NPC.ai[2] < phaseTransitionTimer - 90)
                {
                    if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        NPC.alpha += 15;
                    else
                        NPC.alpha -= 15;

                    if (NPC.alpha < 0)
                        NPC.alpha = 0;
                    if (NPC.alpha > 150)
                        NPC.alpha = 150;
                }
                else if (NPC.alpha < 255)
                {
                    NPC.alpha += 4;
                    if (NPC.alpha > 255)
                        NPC.alpha = 255;
                }

                // Velocity
                NPC.velocity *= 0.98f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0f, 0.02f);

                // Play sound
                if (NPC.ai[2] == phaseTransitionTimer - 60)
                    SoundEngine.PlaySound(SoundID.Zombie20, NPC.Center);

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= phaseTransitionTimer)
                {
                    NPC.ai[0] = 10f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Phase 3
            else if (NPC.ai[0] == 10f && !player.dead)
            {
                // Alpha
                if (NPC.alpha < 255)
                {
                    NPC.alpha += 25;
                    if (NPC.alpha > 255)
                        NPC.alpha = 255;
                }

                // Teleport location
                if (NPC.ai[1] == 0f)
                    NPC.ai[1] = 360 * Math.Sign((NPC.Center - player.Center).X);

                Vector2 desiredVelocity = Vector2.Normalize(player.Center + new Vector2(NPC.ai[1], -200f) - NPC.Center - NPC.velocity) * idlePhaseVelocity;
                NPC.SimpleFlyMovement(desiredVelocity, idlePhaseAcceleration);

                // Rotation and direction
                int phase3SpriteFaceDirection = Math.Sign(player.Center.X - NPC.Center.X);
                if (phase3SpriteFaceDirection != 0)
                {
                    if (NPC.ai[2] == 0f && phase3SpriteFaceDirection != NPC.direction)
                    {
                        NPC.rotation += MathHelper.Pi;
                        for (int l = 0; l < NPC.oldPos.Length; l++)
                            NPC.oldPos[l] = Vector2.Zero;
                    }

                    NPC.direction = phase3SpriteFaceDirection;

                    if (NPC.spriteDirection != -NPC.direction)
                        NPC.rotation += MathHelper.Pi;

                    NPC.spriteDirection = -NPC.direction;
                }

                // Phase switch
                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= idlePhaseTimer || Main.zenithWorld)
                {
                    int phase3AttackPicker = 0;
                    if (phase4)
                    {
                        switch ((int)NPC.ai[3])
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
                        switch ((int)NPC.ai[3])
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
                        NPC.ai[0] = 11f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;

                        // Velocity and rotation
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * chargeVelocity;
                        NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);

                        // Direction
                        if (phase3SpriteFaceDirection != 0)
                        {
                            NPC.direction = phase3SpriteFaceDirection;

                            if (NPC.spriteDirection == 1)
                                NPC.rotation += MathHelper.Pi;

                            NPC.spriteDirection = -NPC.direction;
                        }
                    }

                    // Pause
                    else if (phase3AttackPicker == 2)
                    {
                        NPC.ai[0] = 12f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                    }

                    // Go to next phase
                    else if (phase3AttackPicker == 3)
                    {
                        NPC.ai[0] = -1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                    }

                    NPC.netUpdate = true;
                }
            }

            // Charge
            else if (NPC.ai[0] == 11f)
            {
                // Accelerate
                NPC.velocity *= 1.01f;

                // Alpha
                NPC.alpha -= 25;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;

                // Spawn dust
                int phase3ChargeDustAmt = 7;
                for (int m = 0; m < phase3ChargeDustAmt; m++)
                {
                    Vector2 arg_2444_0 = (Vector2.Normalize(NPC.velocity) * new Vector2((NPC.width + 50) / 2f, NPC.height) * 0.75f).RotatedBy((m - (phase3ChargeDustAmt / 2 - 1)) * MathHelper.Pi / phase3ChargeDustAmt) + NPC.Center;
                    Vector2 phase3ChargeDustDirection = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int phase3ChargeDust = Dust.NewDust(arg_2444_0 + phase3ChargeDustDirection, 0, 0, DustID.DungeonWater, phase3ChargeDustDirection.X * 2f, phase3ChargeDustDirection.Y * 2f, 100, default, 1.4f);
                    Main.dust[phase3ChargeDust].noGravity = true;
                    Main.dust[phase3ChargeDust].noLight = true;
                    Main.dust[phase3ChargeDust].velocity /= 4f;
                    Main.dust[phase3ChargeDust].velocity -= NPC.velocity;
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= chargeTime)
                {
                    NPC.ai[0] = 10f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;

                    if (!phase4 || !death)
                        NPC.ai[3] += 1f;

                    NPC.netUpdate = true;
                }
            }

            // Pause before teleport
            else if (NPC.ai[0] == 12f)
            {
                // Disable contact damage during the teleporting phase
                NPC.damage = 0;

                // Alpha
                if (NPC.alpha < 255)
                {
                    NPC.alpha += 17;
                    if (NPC.alpha > 255)
                        NPC.alpha = 255;
                }

                // Velocity
                NPC.velocity *= 0.98f;
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0f, 0.02f);

                // Play sound
                if (NPC.ai[2] == teleportPhaseTimer / 2)
                    SoundEngine.PlaySound(SoundID.Zombie20, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[2] == teleportPhaseTimer / 2)
                {
                    // Teleport location
                    if (NPC.ai[1] == 0f)
                        NPC.ai[1] = 300 * Math.Sign((NPC.Center - player.Center).X);

                    // Rotation and direction
                    Vector2 center = player.Center + new Vector2(-NPC.ai[1], -200f);
                    NPC.Center = center;
                    int phase3PlayerDirection = Math.Sign(player.Center.X - NPC.Center.X);
                    if (phase3PlayerDirection != 0)
                    {
                        if (NPC.ai[2] == 0f && phase3PlayerDirection != NPC.direction)
                        {
                            NPC.rotation += MathHelper.Pi;
                            for (int n = 0; n < NPC.oldPos.Length; n++)
                                NPC.oldPos[n] = Vector2.Zero;
                        }

                        NPC.direction = phase3PlayerDirection;

                        if (NPC.spriteDirection != -NPC.direction)
                            NPC.rotation += MathHelper.Pi;

                        NPC.spriteDirection = -NPC.direction;
                    }
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] >= teleportPhaseTimer)
                {
                    NPC.ai[0] = 10f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;

                    NPC.ai[3] += 1f;
                    if (NPC.ai[3] >= 9f)
                        NPC.ai[3] = 0f;

                    NPC.netUpdate = true;
                }
            }

            return false;
        }

        public class DetonatingBubbleAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                bool driftUpward = NPC.ai[1] < 0f;
                NPC.damage = driftUpward ? 0 : NPC.defDamage;

                if (driftUpward)
                {
                    NPC.ai[1] += 1f;

                    if (NPC.velocity.Y > -2f)
                        NPC.velocity.Y -= 0.04f;

                    return false;
                }

                if (NPC.target == Main.maxPlayers)
                {
                    CalamityUtils.CalamityTargeting(NPC, default);
                    NPC.ai[3] = (float)Main.rand.Next(100, 151) / 100f;
                    float startingVelocity = (float)Main.rand.Next(250, 351) / 15f;
                    NPC.velocity = (Main.player[NPC.target].Center - NPC.Center + new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101))).SafeNormalize(Vector2.UnitY) * startingVelocity;
                    NPC.netUpdate = true;
                }

                bool pop = NPC.ai[0] == 1f;

                Vector2 velocityVector = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                float inertia = 30f;
                float velocity = 20f;
                NPC.velocity = (NPC.velocity * inertia + velocityVector * velocity) / (inertia + 1f);

                NPC.scale = NPC.ai[3];

                NPC.alpha -= 30;
                if (NPC.alpha < 50)
                    NPC.alpha = 50;
                NPC.alpha = 50;

                float inertia2 = inertia + 10f;
                NPC.velocity.X = (NPC.velocity.X * inertia2 + (float)Main.rand.Next(-10, 11) * 0.1f) / (inertia2 + 1f);
                NPC.velocity.Y = (NPC.velocity.Y * inertia2 + -0.25f + (float)Main.rand.Next(-10, 11) * 0.2f) / (inertia2 + 1f);
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y -= 0.04f;

                // Push Bubbles away from each other.
                float spreadOutStrength = (CalamityWorld.death || BossRushEvent.BossRushActive) ? -0.08f : -0.06f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != NPC.whoAmI && Main.npc[i].active && Main.npc[i].type == NPC.type)
                    {
                        Vector2 otherBubbleDist = Main.npc[i].Center - NPC.Center;
                        if (otherBubbleDist.Length() < (NPC.width + NPC.height))
                        {
                            otherBubbleDist = otherBubbleDist.SafeNormalize(Vector2.UnitY);
                            otherBubbleDist *= spreadOutStrength;
                            NPC.velocity += otherBubbleDist;
                            Main.npc[i].velocity -= otherBubbleDist;
                        }
                    }
                }

                if (NPC.ai[0] == 0f)
                {
                    int size = 40;
                    Rectangle rect = NPC.getRect();
                    rect.X -= size + NPC.width / 2;
                    rect.Y -= size + NPC.height / 2;
                    rect.Width += size * 2;
                    rect.Height += size * 2;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player player = Main.player[i];
                        if (player.active && !player.dead && rect.Intersects(player.getRect()))
                        {
                            NPC.ai[0] = 1f;
                            NPC.ai[1] = 4f;
                            NPC.netUpdate = true;
                            break;
                        }
                    }
                }

                if (NPC.ai[0] == 0f)
                {
                    NPC.ai[1] += 1f;
                    float timeBeforePopping = 300f;
                    if (NPC.ai[1] >= timeBeforePopping)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 4f;
                    }
                }

                if (pop)
                {
                    NPC.ai[1] -= 1f;
                    if (NPC.ai[1] <= 0f)
                    {
                        NPC.life = 0;
                        NPC.HitEffect();
                        NPC.active = false;
                        return false;
                    }
                }

                if (pop)
                {
                    NPC.position = NPC.Center;
                    NPC.width = NPC.height = 100;
                    NPC.position = new Vector2(NPC.position.X - (float)(NPC.width / 2), NPC.position.Y - (float)(NPC.height / 2));
                    NPC.EncourageDespawn(3);
                }

                return false;
            }
        }
    }
}
