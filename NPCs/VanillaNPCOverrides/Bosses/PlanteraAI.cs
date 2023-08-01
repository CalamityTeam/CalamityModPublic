using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class PlanteraAI
    {
        public const float SeedGatlingGateValue = 600f;
        public const float SeedGatlingDuration = 300f;
        public const float SeedGatlingColorChangeDuration = 180f;
        public const float SeedGatlingStopValue = SeedGatlingGateValue + SeedGatlingDuration;
        public const float SeedGatlingColorChangeGateValue = SeedGatlingStopValue - SeedGatlingColorChangeDuration;
        public const float TentaclePhaseSlowDuration = 1200f;
        public const float ChargePhaseGateValue = 900f;
        public const float ReduceSpeedForChargeDistance = 480f;
        public const float BeginChargeGateValue = -120f;
        public const float BeginChargeSlowDownGateValue = BeginChargeGateValue - 45f;
        public const float StopChargeGateValue = BeginChargeSlowDownGateValue - 30f;
        public const float MovementVelocityMultiplierForSlowAttacks = 0.5f;

        public static bool BuffedPlanteraAI(NPC npc, Mod mod)
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

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases based on HP
            bool addThornBallsToGatlingAttack = lifeRatio < 0.85f;
            bool addSporeGasBlastToGatlingAttack = lifeRatio < 0.75f;
            bool phase2 = lifeRatio <= 0.5f;
            bool phase3 = lifeRatio < 0.35f;
            bool phase4 = lifeRatio < 0.2f;

            // Variables and target
            bool enrage = bossRush;
            bool despawn = false;

            // Check for Jungle
            bool surface = !bossRush && Main.player[npc.target].position.Y < Main.worldSurface * 16.0;
            int maxTentaclesAfterFirstTentaclePhase = death ? 4 : 2;
            int maxFreeTentaclesAfterFirstTentaclePhase = maxTentaclesAfterFirstTentaclePhase * 2;
            float speedUpDistance = 480f;
            bool speedUp = Vector2.Distance(Main.player[npc.target].Center, npc.Center) > speedUpDistance; // 30 or 40 tile distance

            // Despawn
            if (Main.player[npc.target].dead)
            {
                despawn = true;
                enrage = true;
            }

            // Despawn if too far from target
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 6000f)
                {
                    npc.active = false;
                    npc.life = 0;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                }
            }

            // Set whoAmI variable and spawn hooks
            NPC.plantBoss = npc.whoAmI;
            if (npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] = 1f;
                NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasHook, npc.whoAmI);
                NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasHook, npc.whoAmI);
                NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasHook, npc.whoAmI);
            }

            // Find positions of hooks
            int maxHooks = 3;
            int[] hookArray = new int[maxHooks];
            float hookPositionX = 0f;
            float hookPositionY = 0f;
            int numHooksSpawned = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].aiStyle == NPCAIStyleID.PlanteraHook)
                {
                    hookPositionX += Main.npc[i].Center.X;
                    hookPositionY += Main.npc[i].Center.Y;
                    hookArray[numHooksSpawned] = i;

                    numHooksSpawned++;
                    if (numHooksSpawned >= maxHooks)
                        break;
                }
            }
            hookPositionX /= numHooksSpawned;
            hookPositionY /= numHooksSpawned;

            // Velocity and acceleration
            float velocity = bossRush ? 12f : phase4 ? 7f : phase3 ? 6.5f : phase2 ? 6f : 4f;
            float acceleration = bossRush ? 0.12f : phase3 ? 0.06f : 0.04f;
            float chargeLineUpVelocity = bossRush ? 20f : phase4 ? 12f : phase3 ? 10f : 8f;
            float chargeLineUpAcceleration = bossRush ? 0.8f : phase4 ? 0.6f : phase3 ? 0.5f : 0.4f;
            float chargeVelocity = bossRush ? 30f : phase4 ? 22f : phase3 ? 20f : 18f;
            float chargeDeceleration = bossRush ? 0.85f : phase4 ? 0.92f : phase3 ? 0.95f : 0.96f;

            // Enrage if target is on the surface
            if (!bossRush && (surface || Main.player[npc.target].position.Y > ((Main.maxTilesY - 200) * 16)))
            {
                enrage = true;
                velocity += 8f;
                acceleration = 0.15f;
            }

            npc.Calamity().CurrentlyEnraged = !bossRush && enrage;

            // Movement relative to the target and hook positions
            Vector2 npcCenterAccountingForHooks = new Vector2(hookPositionX, hookPositionY);
            float maxVelocityX = Main.player[npc.target].Center.X - npcCenterAccountingForHooks.X;
            float maxVelocityY = Main.player[npc.target].Center.Y - npcCenterAccountingForHooks.Y;
            if (despawn)
            {
                maxVelocityY *= -1f;
                maxVelocityX *= -1f;
                velocity += 8f;
            }
            float distanceFromTarget = (float)Math.Sqrt(maxVelocityX * maxVelocityX + maxVelocityY * maxVelocityY);

            if (death)
            {
                velocity += velocity * 0.25f * (1f - lifeRatio);
                acceleration += acceleration * 0.25f * (1f - lifeRatio);
            }

            if (Main.getGoodWorld)
            {
                velocity *= 1.15f;
                acceleration *= 1.15f;
            }

            // Slow down and fire a gatling of projectiles
            // These projectiles are slower than normal
            // Glow gradually more green the closer the gatling attack is to ending
            bool usingSeedGatling = npc.ai[1] > SeedGatlingGateValue;
            bool slowedDuringTentaclePhase = npc.ai[2] > 0f;
            bool doneWithTentaclePhase = npc.ai[2] == -1f;
            bool charging = npc.ai[3] <= -2f;
            if (!phase2)
            {
                npc.ai[1] += 1f;
                if (usingSeedGatling)
                {
                    float currentSeedGatlingTime = npc.ai[1] - SeedGatlingGateValue;

                    // Slow down more and more as gatling attack continues
                    velocity *= MathHelper.Lerp(MovementVelocityMultiplierForSlowAttacks, 1f, (float)Math.Pow(currentSeedGatlingTime / SeedGatlingDuration, 2D));

                    // Shoot projectiles
                    float shootProjectileGateValue = 30f;
                    if (currentSeedGatlingTime >= 240f)
                        shootProjectileGateValue = 3f;
                    else if (currentSeedGatlingTime >= 180f)
                        shootProjectileGateValue = 5f;
                    else if (currentSeedGatlingTime >= 120f)
                        shootProjectileGateValue = 9f;
                    else if (currentSeedGatlingTime >= 60f)
                        shootProjectileGateValue = 15f;

                    if (npc.ai[1] % shootProjectileGateValue == 0f)
                    {
                        bool shootThornBall = npc.ai[1] % 90f == 0f && addThornBallsToGatlingAttack;
                        bool shootPoisonSeed = npc.ai[1] % 9f == 0f;
                        float projectileSpeed = 14f;
                        int projectileType = shootThornBall ? ProjectileID.ThornBall : shootPoisonSeed ? ProjectileID.PoisonSeedPlantera : ProjectileID.SeedPlantera;
                        int damage = npc.GetProjectileDamage(projectileType);
                        Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);
                        Vector2 spawnOffset = npc.Center + projectileVelocity * 50f;

                        int dustType = shootPoisonSeed ? 74 : 73;
                        int dustSpawnBoxSize = shootThornBall ? 38 : 14;
                        int dustAmount = shootThornBall ? 15 : 5;
                        Vector2 dustVelocity = projectileVelocity * projectileSpeed;
                        for (int k = 0; k < dustAmount; k++)
                        {
                            int dust = Dust.NewDust(spawnOffset, dustSpawnBoxSize, dustSpawnBoxSize, dustType, dustVelocity.X, dustVelocity.Y);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].scale = 1.4f;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), spawnOffset, projectileVelocity * projectileSpeed, projectileType, damage, 0f, Main.myPlayer);
                            if (projectileType == ProjectileID.ThornBall && (Main.rand.NextBool(2) || !Main.zenithWorld))
                                Main.projectile[proj].tileCollide = false;
                        }
                    }
                }

                // Spore Gas vomit color telegraph
                if (addSporeGasBlastToGatlingAttack)
                {
                    bool startEmittingDust = npc.ai[1] > SeedGatlingColorChangeGateValue;
                    if (startEmittingDust)
                    {
                        float dustEmitAmount = npc.ai[1] - SeedGatlingColorChangeGateValue;
                        int dustInXChanceMin = 2;
                        int dustInXChanceMax = 8;
                        int dustChance = (int)Math.Round(MathHelper.Lerp(dustInXChanceMin, dustInXChanceMax, 1f - dustEmitAmount / SeedGatlingColorChangeDuration));
                        if (Main.rand.NextBool(dustChance))
                        {
                            int dust = Dust.NewDust(npc.position, npc.width, npc.height, 74, 0f, 0f, 0, default, 1.4f);
                            Vector2 vector = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                            vector.Normalize();
                            vector *= Main.rand.Next(50, 100) * 0.04f;
                            Main.dust[dust].velocity = vector;
                            vector.Normalize();
                            vector *= 86f;
                            Main.dust[dust].position = npc.Center - vector;
                        }
                    }
                }

                if (npc.ai[1] >= SeedGatlingStopValue)
                {
                    // Vomit dense spread of spore gas at the end of the gatling attack
                    if (addSporeGasBlastToGatlingAttack)
                    {
                        SoundEngine.PlaySound(SoundID.Item74, npc.Center);
                        int totalProjectiles = 30;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        int type = ModContent.ProjectileType<SporeGasPlantera>();
                        int damage = npc.GetProjectileDamage(type);
                        float velocity2 = CalamityWorld.LegendaryMode ? 10f : 5f;
                        Vector2 spinningPoint = new Vector2(0f, -velocity2);
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 projectileVelocity = spinningPoint.RotatedBy(radians * k);
                            Vector2 spawnOffset = npc.Center + Vector2.Normalize(projectileVelocity) * 50f;
                            float randomSpeed = Main.rand.NextFloat(0.8f, 1.2f);

                            int dustType = 74;
                            Vector2 dustVelocity = projectileVelocity * randomSpeed;
                            for (int l = 0; l < 5; l++)
                            {
                                int dust = Dust.NewDust(spawnOffset, 32, 32, dustType, dustVelocity.X, dustVelocity.Y);
                                Main.dust[dust].scale = 1.4f;
                            }

                            float ai0 = Main.rand.Next(3);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromAI(), spawnOffset, projectileVelocity * randomSpeed, type, damage, 0f, Main.myPlayer, ai0);
                        }
                    }

                    npc.ai[1] = -SeedGatlingDuration;
                }
            }
            else
            {
                npc.ai[1] = 0f;

                // Slow down for a while after tentacles are spawned
                if (slowedDuringTentaclePhase)
                    velocity *= MathHelper.Lerp(MovementVelocityMultiplierForSlowAttacks, 1f, (float)Math.Pow(1f - npc.ai[2] / TentaclePhaseSlowDuration, 2D));

                // Prepare to charge
                // More charges are used in a row at lower HP
                if (doneWithTentaclePhase && !charging)
                {
                    float timeToChargeIncrement = phase4 ? 2f : phase3 ? 1.5f : 1f;
                    npc.ai[3] += timeToChargeIncrement;
                    if (npc.ai[3] >= ChargePhaseGateValue)
                        npc.ai[3] = -2f;
                }
            }

            // Move slowly for a bit after finishing gatling attack
            bool slowedAfterGatlingAttack = npc.ai[1] < 0f && !phase2;
            if (slowedAfterGatlingAttack)
            {
                float absValueOfTimer = Math.Abs(npc.ai[1]);
                velocity *= MathHelper.Lerp(MovementVelocityMultiplierForSlowAttacks, 1f, (float)Math.Pow(absValueOfTimer / SeedGatlingDuration, 2D));

                // Shoot homing pink bulb projectiles that leave behind lingering pink clouds
                float shootBulbGateValue = death ? 90f : 120f;
                if (addSporeGasBlastToGatlingAttack)
                    shootBulbGateValue *= 0.8f;

                if (absValueOfTimer % shootBulbGateValue == 0f)
                {
                    float projectileSpeed = 9f;
                    int projectileType = ModContent.ProjectileType<HomingGasBulb>();
                    int damage = npc.GetProjectileDamage(projectileType);
                    Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);
                    Vector2 spawnOffset = npc.Center + projectileVelocity * 50f;

                    int dustType = 73;
                    Vector2 dustVelocity = projectileVelocity * projectileSpeed;
                    for (int k = 0; k < 5; k++)
                    {
                        int dust = Dust.NewDust(spawnOffset, 18, 18, dustType, dustVelocity.X, dustVelocity.Y);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale = 1.4f;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), spawnOffset, projectileVelocity * projectileSpeed, projectileType, damage, 0f, Main.myPlayer);
                }
            }

            if (charging)
            {
                // Slow down and return to normal behavior
                if (npc.ai[3] <= BeginChargeSlowDownGateValue)
                {
                    npc.velocity *= chargeDeceleration;
                    float timeToDecelerateDecrement = bossRush ? 2f : phase4 ? 1.5f : 1f;
                    npc.ai[3] -= timeToDecelerateDecrement;
                    if (npc.ai[3] <= StopChargeGateValue)
                    {
                        npc.ai[3] = 0f;

                        // Spawn a few tentacles
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            // If the most likely loop condition to be false isn't met, don't run the second one, this is more efficient
                            if (NPC.CountNPCS(NPCID.PlanterasTentacle) < maxTentaclesAfterFirstTentaclePhase)
                            {
                                if (NPC.CountNPCS(ModContent.NPCType<PlanterasFreeTentacle>()) < maxFreeTentaclesAfterFirstTentaclePhase)
                                {
                                    for (int i = 0; i < maxTentaclesAfterFirstTentaclePhase; i++)
                                        NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasTentacle, npc.whoAmI, 0f, 0f, 1f, 0f);
                                }
                            }
                        }
                    }
                }

                // Maintain charge velocity
                // Emit spore gas in phase 3
                else if (npc.ai[3] <= BeginChargeGateValue)
                {
                    float sporeGasDashGateValue = death ? 4f : 6f;
                    if (phase3 && npc.ai[3] % sporeGasDashGateValue == 0f)
                    {
                        int projectileType = ModContent.ProjectileType<SporeGasPlantera>();
                        int damage = npc.GetProjectileDamage(projectileType);
                        Vector2 projectileVelocity = npc.velocity * Main.rand.NextVector2CircularEdge(0.2f, 0.2f);
                        Vector2 spawnOffset = npc.Center + Vector2.Normalize(projectileVelocity) * 30f;

                        int dustType = 74;
                        Vector2 dustVelocity = projectileVelocity;
                        for (int k = 0; k < 5; k++)
                        {
                            int dust = Dust.NewDust(spawnOffset, 32, 32, dustType, dustVelocity.X, dustVelocity.Y);
                            Main.dust[dust].scale = 1.4f;
                        }

                        float ai0 = Main.rand.Next(3);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_FromAI(), spawnOffset, projectileVelocity, projectileType, damage, 0f, Main.myPlayer, ai0);
                    }

                    npc.ai[3] -= 1f;
                    if (npc.ai[3] <= BeginChargeSlowDownGateValue)
                        npc.ai[3] = BeginChargeSlowDownGateValue;
                }

                // Move a specified distance away from the target and charge once that distance is reached
                else
                {
                    // Line up before charging
                    if (npc.Calamity().newAI[0] == 0f)
                    {
                        npc.Calamity().newAI[0] = Math.Sign((npc.Center - Main.player[npc.target].Center).X);
                        npc.SyncExtraAI();
                    }

                    Vector2 destination = Main.player[npc.target].Center + new Vector2(npc.Calamity().newAI[0], 0);
                    Vector2 distanceFromDestination = destination - npc.Center;
                    Vector2 desiredVelocity = Vector2.Normalize(distanceFromDestination - npc.velocity) * chargeLineUpVelocity;

                    if (Vector2.Distance(npc.Center, destination) > ReduceSpeedForChargeDistance)
                        npc.SimpleFlyMovement(desiredVelocity, chargeLineUpAcceleration);
                    else
                        npc.velocity *= 0.98f;

                    // Emit dust to show that a spore and charge attack are about to happen
                    float dustEmitAmount = Math.Abs(BeginChargeGateValue) - Math.Abs(npc.ai[3]);
                    int dustInXChanceMin = 2;
                    int dustInXChanceMax = 8;
                    int dustChance = (int)Math.Round(MathHelper.Lerp(dustInXChanceMin, dustInXChanceMax, 1f - dustEmitAmount / Math.Abs(BeginChargeGateValue)));
                    if (Main.rand.NextBool(dustChance))
                    {
                        int dust = Dust.NewDust(npc.position, npc.width, npc.height, 74, 0f, 0f, 0, default, 1.4f);
                        Vector2 vector = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                        vector.Normalize();
                        vector *= Main.rand.Next(50, 100) * 0.04f;
                        Main.dust[dust].velocity = vector;
                        vector.Normalize();
                        vector *= 86f;
                        Main.dust[dust].position = npc.Center - vector;
                    }

                    float timeToLineUpChargeDecrement = phase4 ? 2f : 1f;
                    npc.ai[3] -= timeToLineUpChargeDecrement;
                    if (npc.ai[3] <= BeginChargeGateValue)
                    {
                        // Charge
                        npc.ai[3] = BeginChargeGateValue;
                        npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * chargeVelocity;
                        SoundEngine.PlaySound(SoundID.Item74, npc.Center);

                        // Spore dust cloud
                        Vector2 dustVelocity = npc.velocity * -0.25f;
                        for (int k = 0; k < 30; k++)
                        {
                            Dust dust = Dust.NewDustDirect(npc.Center, npc.width, npc.height, 44, dustVelocity.X, dustVelocity.Y, 250, default, 0.8f);
                            dust.fadeIn = 0.7f;
                        }

                        // Vomit spread of spore gas
                        int totalProjectiles = 12;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        int type = ModContent.ProjectileType<SporeGasPlantera>();
                        int damage = npc.GetProjectileDamage(type);
                        float velocity2 = CalamityWorld.LegendaryMode ? 10f : 5f;
                        Vector2 spinningPoint = new Vector2(0f, -velocity2);
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 projectileVelocity = spinningPoint.RotatedBy(radians * k);
                            Vector2 spawnOffset = npc.Center + Vector2.Normalize(projectileVelocity) * 50f;
                            float randomSpeed = Main.rand.NextFloat(0.8f, 1.2f);

                            int dustType = 74;
                            Vector2 dustVelocity2 = projectileVelocity * randomSpeed;
                            for (int l = 0; l < 5; l++)
                            {
                                int dust = Dust.NewDust(spawnOffset, 32, 32, dustType, dustVelocity2.X, dustVelocity2.Y);
                                Main.dust[dust].scale = 1.4f;
                            }

                            float ai0 = Main.rand.Next(3);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromAI(), spawnOffset, projectileVelocity * randomSpeed, type, damage, 0f, Main.myPlayer, ai0);
                        }
                    }

                    // Rotation
                    float rotationX = Main.player[npc.target].Center.X - npc.Center.X;
                    float rotationY = Main.player[npc.target].Center.Y - npc.Center.Y;
                    npc.rotation = (float)Math.Atan2(rotationY, rotationX) + MathHelper.PiOver2;
                }
            }
            else
            {
                // Velocity ranges from 4 to 7.2, Acceleration ranges from 0.04 to 0.072, non-enraged phase 1
                // Velocity ranges from 7 to 12.6, Acceleration ranges from 0.07 to 0.126, non-enraged phase 2
                // Velocity ranges from 9 to 16.2, Acceleration ranges from 0.07 to 0.126, non-enraged phase 3
                // Velocity ranges from 17 to 30.6, Acceleration ranges from 0.15 to 0.27, enraged phase 3

                // Distance Plantera can travel from her hooks
                float maxDistanceFromHooks = enrage ? 1000f : 600f;
                if (phase3)
                    maxDistanceFromHooks += 150f;
                if (death)
                    maxDistanceFromHooks += maxDistanceFromHooks * 0.2f * (1f - lifeRatio);

                if (distanceFromTarget >= maxDistanceFromHooks)
                {
                    distanceFromTarget = maxDistanceFromHooks / distanceFromTarget;
                    maxVelocityX *= distanceFromTarget;
                    maxVelocityY *= distanceFromTarget;
                }

                hookPositionX += maxVelocityX;
                hookPositionY += maxVelocityY;
                npcCenterAccountingForHooks = npc.Center;
                maxVelocityX = hookPositionX - npcCenterAccountingForHooks.X;
                maxVelocityY = hookPositionY - npcCenterAccountingForHooks.Y;
                distanceFromTarget = (float)Math.Sqrt(maxVelocityX * maxVelocityX + maxVelocityY * maxVelocityY);

                if (distanceFromTarget < velocity)
                {
                    maxVelocityX = npc.velocity.X;
                    maxVelocityY = npc.velocity.Y;
                }
                else
                {
                    distanceFromTarget = velocity / distanceFromTarget;
                    maxVelocityX *= distanceFromTarget;
                    maxVelocityY *= distanceFromTarget;
                }

                if (npc.velocity.X < maxVelocityX)
                {
                    npc.velocity.X += acceleration;
                    if (npc.velocity.X < 0f && maxVelocityX > 0f)
                        npc.velocity.X += acceleration * 2f;
                }
                else if (npc.velocity.X > maxVelocityX)
                {
                    npc.velocity.X -= acceleration;
                    if (npc.velocity.X > 0f && maxVelocityX < 0f)
                        npc.velocity.X -= acceleration * 2f;
                }
                if (npc.velocity.Y < maxVelocityY)
                {
                    npc.velocity.Y += acceleration;
                    if (npc.velocity.Y < 0f && maxVelocityY > 0f)
                        npc.velocity.Y += acceleration * 2f;
                }
                else if (npc.velocity.Y > maxVelocityY)
                {
                    npc.velocity.Y -= acceleration;
                    if (npc.velocity.Y > 0f && maxVelocityY < 0f)
                        npc.velocity.Y -= acceleration * 2f;
                }

                // Rotation
                float rotationX = Main.player[npc.target].Center.X - npc.Center.X;
                float rotationY = Main.player[npc.target].Center.Y - npc.Center.Y;
                npc.rotation = (float)Math.Atan2(rotationY, rotationX) + MathHelper.PiOver2;
            }

            // Phase 1
            if (!phase2)
            {
                // Emit light
                Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.8f, 0.2f, 0.4f);

                // Adjust stats
                calamityGlobalNPC.DR = 0.15f;
                calamityGlobalNPC.unbreakableDR = false;
                npc.defense = 32;
                npc.damage = npc.defDamage;

                // Fire projectiles
                if (!usingSeedGatling && !slowedAfterGatlingAttack)
                {
                    float shootBoost = 2f * (1f - lifeRatio);
                    npc.localAI[1] += 1f + shootBoost;

                    if (enrage)
                        npc.localAI[1] += 2f;

                    if (Main.getGoodWorld)
                        npc.localAI[1] += 1f;

                    float shootProjectileGateValue = death ? 40f : 60f;
                    if (npc.localAI[1] >= shootProjectileGateValue)
                    {
                        npc.localAI[1] = 0f;
                        npc.TargetClosest();
                        bool shootPoisonSeed = CalamityWorld.LegendaryMode || Main.rand.NextBool(4);
                        int projectileType = shootPoisonSeed ? ProjectileID.PoisonSeedPlantera : ProjectileID.SeedPlantera;
                        float projectileSpeed = 14f;
                        int damage = npc.GetProjectileDamage(projectileType);
                        Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);
                        Vector2 spawnOffset = npc.Center + projectileVelocity * 50f;

                        int dustType = shootPoisonSeed ? 74 : 73;
                        Vector2 dustVelocity = projectileVelocity * projectileSpeed;
                        for (int k = 0; k < 5; k++)
                        {
                            int dust = Dust.NewDust(spawnOffset, 14, 14, dustType, dustVelocity.X, dustVelocity.Y);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].scale = 1.4f;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_FromAI(), spawnOffset, projectileVelocity * projectileSpeed, projectileType, damage, 0f, Main.myPlayer);
                    }
                }
            }

            // Phase 2
            else
            {
                // Emit light
                Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.4f, 0.8f, 0.2f);

                // Spore dust
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 44, 0f, 0f, 250, default, 0.6f);
                    dust.fadeIn = 0.7f;
                }

                // Adjust stats
                calamityGlobalNPC.DR = 0.15f;
                calamityGlobalNPC.unbreakableDR = false;
                npc.defense = 10;
                npc.damage = (int)(npc.defDamage * 1.4f);

                // Spawn tentacles
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.localAI[0] == 1f)
                    {
                        npc.localAI[0] = 2f;
                        int totalTentacles = death ? 10 : 8;
                        if (Main.getGoodWorld)
                            totalTentacles += 6;
                        if (CalamityWorld.LegendaryMode)
                            totalTentacles *= 2;

                        for (int i = 0; i < totalTentacles; i++)
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasTentacle, npc.whoAmI);

                        if (Main.getGoodWorld)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].aiStyle == NPCAIStyleID.PlanteraHook)
                                {
                                    for (int j = 0; j < totalTentacles / 2 - 1; j++)
                                    {
                                        int hookIndex = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasTentacle, npc.whoAmI);
                                        Main.npc[hookIndex].ai[3] = i + 1;
                                    }
                                }
                            }
                        }
                    }
                }

                // Slow down for 20 seconds after transitioning to phase 2
                // This gives players time to handle the tentacles before Plantera starts attack again
                // Decrement the timer far faster if there aren't any tentacles alive
                if (npc.ai[2] == 0f)
                    npc.ai[2] = TentaclePhaseSlowDuration;

                if (slowedDuringTentaclePhase)
                {
                    bool noAttachedTentacles = !NPC.AnyNPCs(NPCID.PlanterasTentacle);
                    bool noFreeTentacles = !NPC.AnyNPCs(ModContent.NPCType<PlanterasFreeTentacle>());
                    float tentacleIdleTimerDecrement = (noAttachedTentacles && noFreeTentacles) ? 4f : noAttachedTentacles ? 2f : 1f;
                    npc.ai[2] -= tentacleIdleTimerDecrement;
                    if (npc.ai[2] <= 0f)
                        npc.ai[2] = -1f;
                }

                // Spawn gore
                if (npc.localAI[2] == 0f)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(npc.GetSource_FromAI(), new Vector2(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height)), npc.velocity, 378, npc.scale);
                        Gore.NewGore(npc.GetSource_FromAI(), new Vector2(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height)), npc.velocity, 379, npc.scale);
                        Gore.NewGore(npc.GetSource_FromAI(), new Vector2(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height)), npc.velocity, 380, npc.scale);
                    }
                    npc.localAI[2] = 1f;
                }

                if (!charging)
                {
                    // Fire spreads of poison seeds
                    npc.localAI[3] += 1f;
                    float shootProjectileGateValue = slowedDuringTentaclePhase ? 120f : 90f;
                    if (npc.localAI[3] >= shootProjectileGateValue)
                    {
                        float projectileSpeed = 14f;
                        if (bossRush)
                            projectileSpeed += 4f;

                        Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);

                        int spread = 8 + (int)Math.Round((0.5f - lifeRatio) * 16f); // 8 to 16, wider spread is harder to avoid
                        int numProj = spread / 2;

                        // Always an odd number of projectiles
                        if (numProj % 2 == 0)
                            numProj++;

                        int type = ProjectileID.PoisonSeedPlantera;
                        int damage = npc.GetProjectileDamage(type);
                        float rotation = MathHelper.ToRadians(spread);

                        for (int i = 0; i < numProj; i++)
                        {
                            bool shootPinkSeed = i % 2 == 0;
                            if (shootPinkSeed)
                                type = ProjectileID.SeedPlantera;
                            else
                                type = ProjectileID.PoisonSeedPlantera;

                            Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                            Vector2 spawnOffset = npc.Center + perturbedSpeed * 50f;

                            int dustType = shootPinkSeed ? 73 : 74;
                            Vector2 dustVelocity = perturbedSpeed * projectileSpeed;
                            for (int k = 0; k < 5; k++)
                            {
                                int dust = Dust.NewDust(spawnOffset, 14, 14, dustType, dustVelocity.X, dustVelocity.Y);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].scale = 1.4f;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromAI(), spawnOffset, perturbedSpeed * projectileSpeed, type, damage, 0f, Main.myPlayer);
                        }

                        npc.localAI[3] = 0f;
                    }
                }
            }

            // Heal if on surface
            if (surface)
            {
                if (Main.rand.NextBool(Main.dayTime ? 3 : 6))
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, 55, 0f, 0f, 200, default, 0.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].fadeIn = 1.3f;
                    Vector2 vector = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                    vector.Normalize();
                    vector *= Main.rand.Next(50, 100) * 0.04f;
                    Main.dust[dust].velocity = vector;
                    vector.Normalize();
                    vector *= 86f;
                    Main.dust[dust].position = npc.Center - vector;
                }

                // Heal, 100 (50 during daytime) seconds to reach full HP from 0
                calamityGlobalNPC.newAI[1] += 1f;
                if (calamityGlobalNPC.newAI[1] >= (Main.dayTime ? 30f : 60f))
                {
                    calamityGlobalNPC.newAI[1] = 0f;
                    npc.SyncExtraAI();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int healAmt = npc.lifeMax / 100;
                        if (healAmt > npc.lifeMax - npc.life)
                            healAmt = npc.lifeMax - npc.life;

                        if (healAmt > 0)
                        {
                            npc.life += healAmt;
                            npc.HealEffect(healAmt, true);
                            npc.netUpdate = true;
                        }
                    }
                }
            }

            if (npc.ai[0] == 0f && npc.life > 0)
                npc.ai[0] = npc.lifeMax;

            if (npc.life > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int healthInterval = death ? (int)(npc.lifeMax * 0.03) : (int)(npc.lifeMax * 0.04);
                    if ((npc.life + healthInterval) < npc.ai[0])
                    {
                        npc.ai[0] = npc.life;

                        if (phase2)
                        {
                            int spore = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.Spore, npc.whoAmI);
                            float sporeSpeed = death ? 9f : 6f;
                            Vector2 sporeVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * sporeSpeed;
                            Main.npc[spore].velocity.X = sporeVelocity.X;
                            Main.npc[spore].velocity.Y = sporeVelocity.Y;
                            Main.npc[spore].netUpdate = true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool BuffedPlanterasHookAI(NPC npc, Mod mod)
        {
            // Variables
            bool enrage = BossRushEvent.BossRushActive;
            bool despawn = false;
            bool death = CalamityWorld.death || enrage;

            // Despawn if Plantera is gone
            if (NPC.plantBoss < 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    npc.StrikeInstantKill();

                return false;
            }

            // Percent life remaining, Plantera
            float lifeRatio = Main.npc[NPC.plantBoss].life / (float)Main.npc[NPC.plantBoss].lifeMax;

            // Despawn if Plantera's target is dead
            if (Main.player[Main.npc[NPC.plantBoss].target].dead && !enrage)
                despawn = true;

            // Enrage if Plantera's target is on the surface
            if (!enrage && ((Main.player[Main.npc[NPC.plantBoss].target].position.Y < Main.worldSurface * 16.0 || Main.player[Main.npc[NPC.plantBoss].target].position.Y > ((Main.maxTilesY - 200) * 16)) | despawn))
            {
                npc.localAI[0] -= 4f;
                enrage = true;
            }

            // Set centers for movement
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (npc.ai[0] == 0f)
                    npc.ai[0] = (int)(npc.Center.X / 16f);
                if (npc.ai[1] == 0f)
                    npc.ai[1] = (int)(npc.Center.X / 16f);
            }

            // Find new spot to move to after set time has passed
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Move immediately
                if (npc.ai[0] == 0f || npc.ai[1] == 0f)
                    npc.localAI[0] = 0f;

                // Timer dictating whether to pick a new location or not
                float moveBoost = death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
                npc.localAI[0] -= 1f + moveBoost;
                if (enrage)
                    npc.localAI[0] -= 6f;

                // Set timer to new amount if a different hook is currently moving
                if (!despawn && npc.localAI[0] <= 0f && npc.ai[0] != 0f)
                {
                    for (int num763 = 0; num763 < Main.maxNPCs; num763++)
                    {
                        if (num763 != npc.whoAmI && Main.npc[num763].active && Main.npc[num763].type == npc.type && (Main.npc[num763].velocity.X != 0f || Main.npc[num763].velocity.Y != 0f))
                            npc.localAI[0] = Main.rand.Next(60, 301);
                    }
                }

                // Pick a location to move to
                if (npc.localAI[0] <= 0f)
                {
                    // Reset timer
                    npc.localAI[0] = Main.rand.Next(300, 601);

                    // Pick location
                    bool flag50 = false;
                    int num764 = 0;
                    while (!flag50 && num764 <= 1000)
                    {
                        num764++;

                        int num765 = (int)(Main.player[Main.npc[NPC.plantBoss].target].Center.X / 16f);
                        int num766 = (int)(Main.player[Main.npc[NPC.plantBoss].target].Center.Y / 16f);

                        if (npc.ai[0] == 0f)
                        {
                            num765 = (int)((Main.player[Main.npc[NPC.plantBoss].target].Center.X + Main.npc[NPC.plantBoss].Center.X) / 32f);
                            num766 = (int)((Main.player[Main.npc[NPC.plantBoss].target].Center.Y + Main.npc[NPC.plantBoss].Center.Y) / 32f);
                        }

                        if (despawn)
                        {
                            num765 = (int)Main.npc[NPC.plantBoss].position.X / 16;
                            num766 = (int)(Main.npc[NPC.plantBoss].position.Y + 400f) / 16;
                        }

                        int num767 = 20;
                        num767 += (int)(100f * (num764 / 1000f));
                        int num768 = num765 + Main.rand.Next(-num767, num767 + 1);
                        int num769 = num766 + Main.rand.Next(-num767, num767 + 1);

                        try
                        {
                            if (WorldGen.SolidTile(num768, num769) || (Main.tile[num768, num769].WallType > 0 && (num764 > 500 || lifeRatio < 0.5f)))
                            {
                                flag50 = true;
                                npc.ai[0] = num768;
                                npc.ai[1] = num769;
                                npc.netUpdate = true;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            // Movement to new location
            if (npc.ai[0] > 0f && npc.ai[1] > 0f)
            {
                // Hook movement velocity
                float velocityBoost = death ? 6f * (1f - lifeRatio) : 3f * (1f - lifeRatio);
                float velocity = 7f + velocityBoost;
                if (enrage)
                    velocity *= 2f;
                if (despawn)
                    velocity *= 2f;

                // Moving to new location
                Vector2 vector95 = new Vector2(npc.Center.X, npc.Center.Y);
                float num773 = npc.ai[0] * 16f - 8f - vector95.X;
                float num774 = npc.ai[1] * 16f - 8f - vector95.Y;
                float num775 = (float)Math.Sqrt(num773 * num773 + num774 * num774);
                if (num775 < 12f + velocity)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld && npc.localAI[3] == 1f)
                    {
                        npc.localAI[3] = 0f;
                        WorldGen.SpawnPlanteraThorns(npc.Center);
                    }

                    npc.velocity.X = num773;
                    npc.velocity.Y = num774;
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld)
                        npc.localAI[3] = 1f;

                    num775 = velocity / num775;
                    npc.velocity.X = num773 * num775;
                    npc.velocity.Y = num774 * num775;
                }

                // Rotation
                Vector2 vector96 = new Vector2(npc.Center.X, npc.Center.Y);
                float num776 = Main.npc[NPC.plantBoss].Center.X - vector96.X;
                float num777 = Main.npc[NPC.plantBoss].Center.Y - vector96.Y;
                npc.rotation = (float)Math.Atan2(num777, num776) - MathHelper.PiOver2;
            }

            return false;
        }

        public static bool BuffedPlanterasTentacleAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Emit light
            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.2f, 0.4f, 0.1f);

            // Spore dust
            if (Main.rand.NextBool(10))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 44, 0f, 0f, 250, default, 0.4f);
                dust.fadeIn = 0.7f;
            }

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            if (Main.getGoodWorld)
            {
                if (Main.rand.NextBool(5))
                    npc.reflectsProjectiles = true;
                else
                    npc.reflectsProjectiles = false;
            }

            // Die if Plantera is gone
            if (NPC.plantBoss < 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    npc.StrikeInstantKill();

                return false;
            }

            // Set Plantera to a variable
            int plantBoss = NPC.plantBoss;

            // Become free if Plantera gets sick of your shit
            if (Main.npc[plantBoss].ai[2] == -1f && npc.ai[2] != 1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    npc.StrikeInstantKill();

                return false;
            }

            // 3 seconds of resistance and no damage to prevent spawn killing and unfair hits
            if (npc.localAI[0] < 90f)
            {
                npc.damage = 0;
                npc.localAI[0] += 1f;
            }
            else
                npc.damage = npc.defDamage;

            // Movement variables
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.ai[0] == 0f || npc.ai[1] == 0f)
                {
                    npc.ai[0] = Main.rand.Next(-100, 101);
                    npc.ai[1] = Main.rand.Next(-100, 101);
                    npc.netUpdate = true;
                }
            }

            // Velocity and acceleration
            float num779 = death ? 2.4f : 1.6f;
            float extendedDistanceFromPlantera = (1f - lifeRatio) * 2f;
            float num780 = 100f + (extendedDistanceFromPlantera * 300f);
            float deceleration = (death ? 0.5f : 0.8f) / (1f + extendedDistanceFromPlantera);

            if (Main.getGoodWorld)
                num779 += 4f;

            // Despawn if Plantera is gone
            if (!Main.npc[plantBoss].active)
            {
                npc.active = false;
                return false;
            }

            // Movement
            Vector2 planteraCenter = Main.npc[plantBoss].Center;
            float num784 = planteraCenter.X + npc.ai[0];
            float num785 = planteraCenter.Y + npc.ai[1];
            float num786 = num784 - planteraCenter.X;
            float num787 = num785 - planteraCenter.Y;
            float num788 = (float)Math.Sqrt(num786 * num786 + num787 * num787);
            num788 = num780 / num788;
            num786 *= num788;
            num787 *= num788;

            if (npc.position.X < planteraCenter.X + num786)
            {
                npc.velocity.X += num779;
                if (npc.velocity.X < 0f && num786 > 0f)
                    npc.velocity.X *= deceleration;
            }
            else if (npc.position.X > planteraCenter.X + num786)
            {
                npc.velocity.X -= num779;
                if (npc.velocity.X > 0f && num786 < 0f)
                    npc.velocity.X *= deceleration;
            }
            if (npc.position.Y < planteraCenter.Y + num787)
            {
                npc.velocity.Y += num779;
                if (npc.velocity.Y < 0f && num787 > 0f)
                    npc.velocity.Y *= deceleration;
            }
            else if (npc.position.Y > planteraCenter.Y + num787)
            {
                npc.velocity.Y -= num779;
                if (npc.velocity.Y > 0f && num787 < 0f)
                    npc.velocity.Y *= deceleration;
            }

            float velocityLimit = 12f + 6f * extendedDistanceFromPlantera;
            if (npc.velocity.X > velocityLimit)
                npc.velocity.X = velocityLimit;
            if (npc.velocity.X < -velocityLimit)
                npc.velocity.X = -velocityLimit;
            if (npc.velocity.Y > velocityLimit)
                npc.velocity.Y = velocityLimit;
            if (npc.velocity.Y < -velocityLimit)
                npc.velocity.Y = -velocityLimit;

            // Direction and rotation
            if (num786 > 0f)
            {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2(num787, num786);
            }
            if (num786 < 0f)
            {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2(num787, num786) + MathHelper.Pi;
            }

            return false;
        }
    }
}
