using System;
using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    public static class DestroyerAI
    {
        public const float DRIncreaseTime = 600f;
        public const float DeathModeLaserBreathGateValue = 600f;
        public const float LaserTelegraphTime = 120f;
        public const float SparkTelegraphTime = 30f;
        public const float FlightPhaseGateValue = 900f;
        public const float FlightPhaseResetGateValue = FlightPhaseGateValue * 2f;
        private const float Phase4FlightPhaseTimerSetValue = FlightPhaseGateValue * 0.5f;
        private const float Phase5FlightPhaseTimerSetValue = FlightPhaseGateValue;
        public const float PhaseTransitionTelegraphTime = 180f;
        public const float GroundTelegraphStartGateValue = FlightPhaseResetGateValue - PhaseTransitionTelegraphTime;
        public const float FlightTelegraphStartGateValue = FlightPhaseGateValue - PhaseTransitionTelegraphTime;
        private const int OneInXChanceToFireLaser = 200;

        public static bool BuffedDestroyerAI(NPC npc, Mod mod)
        {
            int mechdusaCurvedSpineSegmentIndex = 0;
            int mechdusaCurvedSpineSegments = 10;
            if (NPC.IsMechQueenUp && npc.type != NPCID.TheDestroyer)
            {
                int mechdusaIndex = (int)npc.ai[1];
                while (mechdusaIndex > 0 && mechdusaIndex < Main.maxNPCs)
                {
                    if (Main.npc[mechdusaIndex].active && Main.npc[mechdusaIndex].type >= NPCID.TheDestroyer && Main.npc[mechdusaIndex].type <= NPCID.TheDestroyerTail)
                    {
                        mechdusaCurvedSpineSegmentIndex++;
                        if (Main.npc[mechdusaIndex].type == NPCID.TheDestroyer)
                            break;

                        if (mechdusaCurvedSpineSegmentIndex >= mechdusaCurvedSpineSegments)
                        {
                            mechdusaCurvedSpineSegmentIndex = 0;
                            break;
                        }

                        mechdusaIndex = (int)Main.npc[mechdusaIndex].ai[1];
                        continue;
                    }

                    mechdusaCurvedSpineSegmentIndex = 0;
                    break;
                }
            }

            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool bossRush = BossRushEvent.BossRushActive;
            bool masterMode = Main.masterMode || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // 10 seconds of resistance to prevent spawn killing
            if (calamityGlobalNPC.newAI[1] < DRIncreaseTime)
                calamityGlobalNPC.newAI[1] += 1f;

            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = calamityGlobalNPC.newAI[1] < DRIncreaseTime;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases based on life percentage
            bool phase2 = lifeRatio < 0.85f || masterMode;
            bool phase3 = lifeRatio < 0.7f || masterMode;
            bool startFlightPhase = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < (death ? 0.4f : 0.25f);
            bool phase5 = lifeRatio < (death ? 0.2f : 0.1f);

            // Flight timer
            if (startFlightPhase)
                calamityGlobalNPC.newAI[3] += 1f;

            // Force the timer to be at a certain value in later phases
            float flightPhaseTimerSetValue = phase5 ? Phase5FlightPhaseTimerSetValue : phase4 ? Phase4FlightPhaseTimerSetValue : 0f;
            if (calamityGlobalNPC.newAI[3] < flightPhaseTimerSetValue)
                calamityGlobalNPC.newAI[3] = flightPhaseTimerSetValue;

            // Return to ground phase, with less time spent in later phases
            if (calamityGlobalNPC.newAI[3] >= FlightPhaseResetGateValue)
                calamityGlobalNPC.newAI[3] = flightPhaseTimerSetValue;

            // Spawn DR check
            bool hasSpawnDR = calamityGlobalNPC.newAI[1] < DRIncreaseTime && calamityGlobalNPC.newAI[1] > 60f;

            // Gradual color transition from ground to flight and vice versa
            // 0f = Red, 1f = Purple
            float phaseTransitionColorAmount = (hasSpawnDR || phase5) ? 1f : 0f;
            if (!hasSpawnDR && !phase5)
            {
                if (calamityGlobalNPC.newAI[3] >= GroundTelegraphStartGateValue)
                    phaseTransitionColorAmount = MathHelper.Clamp(1f - (calamityGlobalNPC.newAI[3] - GroundTelegraphStartGateValue) / PhaseTransitionTelegraphTime, 0f, 1f);
                else if (calamityGlobalNPC.newAI[3] >= FlightTelegraphStartGateValue)
                    phaseTransitionColorAmount = MathHelper.Clamp((calamityGlobalNPC.newAI[3] - FlightTelegraphStartGateValue) / PhaseTransitionTelegraphTime, 0f, 1f);
            }

            // Set worm variable for worms
            if (npc.ai[3] > 0f)
                npc.realLife = (int)npc.ai[3];

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            Player player = Main.player[npc.target];

            bool increaseSpeed = Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles;
            bool increaseSpeedMore = Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles;

            float enrageScale = bossRush ? 1f : 0f;
            if (Main.IsItDay() || bossRush)
            {
                calamityGlobalNPC.CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            // Phase for flying at the player
            bool flyAtTarget = (calamityGlobalNPC.newAI[3] >= FlightPhaseGateValue && startFlightPhase) || hasSpawnDR;

            // Dust on spawn and alpha effects
            if (npc.type == NPCID.TheDestroyer || (npc.type != NPCID.TheDestroyer && Main.npc[(int)npc.ai[1]].alpha < 128))
            {
                if (npc.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int spawnDust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.TheDestroyer, 0f, 0f, 100, default, 2f);
                        Main.dust[spawnDust].noGravity = true;
                        Main.dust[spawnDust].noLight = true;
                    }
                }

                npc.alpha -= 42;
                if (npc.alpha < 0)
                    npc.alpha = 0;
            }

            // Check if other segments are still alive, if not, die
            // Check for Oblivion too, since having a max power Destroyer during that fight would be turbo cancer
            bool oblivionAlive = false;
            if (npc.type > NPCID.TheDestroyer)
            {
                bool shouldDespawn = true;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == NPCID.TheDestroyer)
                    {
                        shouldDespawn = false;
                        break;
                    }
                }
                if (!shouldDespawn)
                {
                    if (npc.ai[1] <= 0f)
                        shouldDespawn = true;
                    else if (Main.npc[(int)npc.ai[1]].life <= 0)
                        shouldDespawn = true;
                }
                if (shouldDespawn)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.checkDead();
                    npc.active = false;
                }
            }
            else
            {
                if (masterMode && !bossRush && npc.localAI[3] != -1f)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && (Main.npc[i].type == ModContent.NPCType<SkeletronPrime2>() || Main.npc[i].type == NPCID.SkeletronPrime))
                        {
                            oblivionAlive = true;
                            break;
                        }
                    }
                }

                // Set variable to force despawn when Prime dies in Master Rev+
                // Set to -1f if Prime isn't alive when summoned
                if (npc.localAI[3] == 0f)
                {
                    if (oblivionAlive)
                        npc.localAI[3] = 1f;
                    else
                        npc.localAI[3] = -1f;

                    npc.SyncExtraAI();
                }
            }

            // Total segment variable
            int totalSegments = Main.getGoodWorld ? 100 : 80;

            // Calculate aggression based on how many broken segments there are
            float brokenSegmentAggressionMultiplier = 1f;
            if (npc.type == NPCID.TheDestroyer && !oblivionAlive)
            {
                int numProbeSegments = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == NPCID.TheDestroyerBody && Main.npc[i].ai[2] == 0f)
                        numProbeSegments++;
                }
                brokenSegmentAggressionMultiplier += (1f - MathHelper.Clamp(numProbeSegments / (float)totalSegments, 0f, 1f)) * 0.25f;
            }

            // Death Mode laser spit bool
            bool spitLaserSpreads = death;

            // Height of the box used to calculate whether The Destroyer should fly at its target or not
            int noFlyZoneBoxHeight = masterMode ? 1500 : 1800;

            // Speed and movement variables
            float speed = masterMode ? 0.2f : 0.1f;
            float turnSpeed = masterMode ? 0.3f : 0.15f;

            // Max velocity
            float segmentVelocity = flyAtTarget ? (masterMode ? 22.5f : 15f) : (masterMode ? 30f : 20f);

            // Increase velocity based on distance
            float velocityMultiplier = increaseSpeedMore ? 2f : increaseSpeed ? 1.5f : 1f;

            // If Oblivion is alive, don't fly, don't spit laser spreads, use the default vanilla no fly zone, reduce segment count to 60, use base speed and use base turn speed
            if (oblivionAlive)
            {
                calamityGlobalNPC.newAI[3] = 0f;
                totalSegments = Main.getGoodWorld ? 75 : 60;
                spitLaserSpreads = false;
                noFlyZoneBoxHeight = 2000;
            }
            else
            {
                noFlyZoneBoxHeight -= death ? 400 : (int)(400f * (1f - lifeRatio));

                float segmentVelocityBoost = death ? (flyAtTarget ? 4.5f : 6f) * (1f - lifeRatio) : (flyAtTarget ? 3f : 4f) * (1f - lifeRatio);
                float speedBoost = death ? (flyAtTarget ? 0.1125f : 0.15f) * (1f - lifeRatio) : (flyAtTarget ? 0.075f : 0.1f) * (1f - lifeRatio);
                float turnSpeedBoost = death ? 0.18f * (1f - lifeRatio) : 0.12f * (1f - lifeRatio);

                segmentVelocity += segmentVelocityBoost;
                speed += speedBoost;
                turnSpeed += turnSpeedBoost;

                segmentVelocity += 5f * enrageScale;
                speed += 0.05f * enrageScale;
                turnSpeed += 0.075f * enrageScale;

                if (flyAtTarget)
                {
                    float speedMultiplier = phase5 ? 1.8f : phase4 ? 1.65f : 1.5f;
                    speed *= speedMultiplier;
                }

                segmentVelocity *= velocityMultiplier;
                speed *= velocityMultiplier;
                turnSpeed *= velocityMultiplier;

                segmentVelocity *= brokenSegmentAggressionMultiplier;
                speed *= brokenSegmentAggressionMultiplier;
                turnSpeed *= brokenSegmentAggressionMultiplier;

                if (Main.getGoodWorld)
                {
                    segmentVelocity *= 1.2f;
                    speed *= 1.2f;
                    turnSpeed *= 1.2f;
                }
            }

            bool probeLaunched = npc.ai[2] == 1f;
            if (npc.type == NPCID.TheDestroyerBody)
            {
                // Enrage, fire more cyan lasers
                if (enrageScale > 0f && !bossRush)
                {
                    if (calamityGlobalNPC.newAI[2] < 480f)
                        calamityGlobalNPC.newAI[2] += 1f;
                }
                else
                {
                    if (calamityGlobalNPC.newAI[2] > 0f)
                        calamityGlobalNPC.newAI[2] -= 1f;
                }

                // Regenerate Probes in Master Mode if the number of Probes is less than 40 and the number of living NPCs is less than the segment count + 40 (this limit is here just in case)
                if (masterMode && probeLaunched)
                {
                    npc.localAI[2] += 1f;
                    if (npc.localAI[2] >= 600f)
                    {
                        int maxProbes = 40;
                        bool regenerateProbeSegment = NPC.CountNPCS(NPCID.Probe) < maxProbes;
                        if (regenerateProbeSegment)
                        {
                            int maxNPCs = totalSegments + maxProbes;
                            int numNPCs = 0;
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active)
                                {
                                    numNPCs++;
                                    if (numNPCs >= maxNPCs)
                                    {
                                        regenerateProbeSegment = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (regenerateProbeSegment)
                        {
                            npc.ai[2] = 0f;
                            npc.netUpdate = true;
                        }

                        npc.localAI[2] = 0f;
                        npc.SyncVanillaLocalAI();
                    }
                }
            }

            if (npc.type == NPCID.TheDestroyer)
            {
                // Spawn segments from head
                if (npc.ai[0] == 0f)
                {
                    npc.ai[3] = npc.whoAmI;
                    npc.realLife = npc.whoAmI;
                    int index = npc.whoAmI;
                    for (int j = 0; j <= totalSegments; j++)
                    {
                        int type = NPCID.TheDestroyerBody;
                        if (j == totalSegments)
                            type = NPCID.TheDestroyerTail;

                        int segment = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X), (int)(npc.position.Y + npc.height), type, npc.whoAmI);
                        Main.npc[segment].ai[3] = npc.whoAmI;
                        Main.npc[segment].realLife = npc.whoAmI;
                        Main.npc[segment].ai[1] = index;
                        Main.npc[index].ai[0] = segment;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, segment);
                        index = segment;
                    }
                }

                // Laser breath in Death Mode
                if (spitLaserSpreads)
                {
                    // Set laser color and type
                    if (calamityGlobalNPC.destroyerLaserColor == -1)
                    {
                        calamityGlobalNPC.destroyerLaserColor = phase3 ? 3 : phase2 ? 2 : 1;
                        npc.SyncDestroyerLaserColor();
                    }

                    float laserBreathGateValue = DeathModeLaserBreathGateValue;
                    if (calamityGlobalNPC.newAI[0] < laserBreathGateValue)
                        calamityGlobalNPC.newAI[0] += 1f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Sync newAI every 20 frames for the new telegraph
                        if (calamityGlobalNPC.newAI[0] % 20f == 10f)
                            npc.SyncExtraAI();
                    }

                    if ((player.Center - npc.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(npc.velocity.ToRotation(), MathHelper.PiOver4) == npc.velocity.ToRotation() &&
                        calamityGlobalNPC.newAI[0] >= laserBreathGateValue && Vector2.Distance(npc.Center, player.Center) > 480f &&
                        Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                    {
                        if (calamityGlobalNPC.newAI[0] % 30f == 0f)
                        {
                            float velocity = bossRush ? 6f : death ? 5.333f : 5f;
                            int type = ProjectileID.DeathLaser;
                            switch (calamityGlobalNPC.destroyerLaserColor)
                            {
                                default:
                                case 0:
                                    break;

                                case 1:
                                    type = ModContent.ProjectileType<DestroyerCursedLaser>();
                                    break;

                                case 2:
                                    type = ModContent.ProjectileType<DestroyerElectricLaser>();
                                    break;
                            }
                            int damage = npc.GetProjectileDamage(type);

                            // Reduce mech boss projectile damage depending on the new ore progression changes
                            if (CalamityServerConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                            {
                                double firstMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert;
                                double secondMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert;
                                if (!NPC.downedMechBossAny)
                                    damage = (int)(damage * firstMechMultiplier);
                                else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                    damage = (int)(damage * secondMechMultiplier);
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 projectileVelocity = (player.Center - npc.Center).SafeNormalize(Vector2.UnitY) * velocity;
                                int numProj = calamityGlobalNPC.newAI[0] % 60f == 0f ? (masterMode ? 9 : 7) : (masterMode ? 6 : 4);
                                int spread = masterMode ? 38 : 26;
                                float rotation = MathHelper.ToRadians(spread);
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + perturbedSpeed.SafeNormalize(Vector2.UnitY) * 100f, perturbedSpeed, type, damage, 0f, Main.myPlayer, 1f, 0f);
                                    Main.projectile[proj].timeLeft = 1200;
                                }
                            }
                        }

                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] > laserBreathGateValue + 60f)
                        {
                            calamityGlobalNPC.newAI[0] = 0f;
                            calamityGlobalNPC.destroyerLaserColor = -1;
                            npc.SyncDestroyerLaserColor();
                            npc.SyncExtraAI();
                        }
                    }
                }
            }

            // Fire lasers
            if (npc.type == NPCID.TheDestroyerBody)
            {
                bool ableToFireLaser = calamityGlobalNPC.destroyerLaserColor != -1;

                // Set laser color and type
                if (calamityGlobalNPC.destroyerLaserColor == -1 && !probeLaunched)
                {
                    if (Main.rand.NextBool(masterMode ? OneInXChanceToFireLaser / (phase5 ? 4 : phase4 ? 3 : 2) : OneInXChanceToFireLaser))
                    {
                        int random = phase3 ? 4 : phase2 ? 3 : 2;
                        switch (Main.rand.Next(random))
                        {
                            case 0:
                            case 1:
                                calamityGlobalNPC.destroyerLaserColor = 0;
                                break;
                            case 2:
                                calamityGlobalNPC.destroyerLaserColor = 1;
                                break;
                            case 3:
                                calamityGlobalNPC.destroyerLaserColor = 2;
                                break;
                        }

                        if (calamityGlobalNPC.newAI[2] > 0f || bossRush)
                            calamityGlobalNPC.destroyerLaserColor = 2;

                        npc.SyncDestroyerLaserColor();
                    }
                }

                if (probeLaunched && ableToFireLaser)
                {
                    calamityGlobalNPC.destroyerLaserColor = -1;
                    npc.SyncDestroyerLaserColor();
                }

                // Laser rate of fire
                float shootProjectileTime = death ? (masterMode ? (phase5 ? 120f : phase4 ? 150f : 180f) : 270f) : (masterMode ? (phase5 ? 150f : phase4 ? 210f : 270f) : 450f);
                float bodySegmentTime = npc.ai[0] * (masterMode ? 20f : 30f);
                float shootProjectileGateValue = bodySegmentTime + shootProjectileTime;
                float laserTimerIncrement = (calamityGlobalNPC.newAI[0] > shootProjectileGateValue - LaserTelegraphTime) ? 1f : 2f;
                if (ableToFireLaser)
                    calamityGlobalNPC.newAI[0] += laserTimerIncrement;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Sync newAI every 20 frames for the new telegraph
                    if (calamityGlobalNPC.newAI[0] % 20f == 10f && ableToFireLaser)
                        npc.SyncExtraAI();
                }

                Color telegraphColor = Color.Transparent;
                switch (calamityGlobalNPC.destroyerLaserColor)
                {
                    case 0:
                        telegraphColor = Color.Red;
                        break;
                    case 1:
                        telegraphColor = Color.Green;
                        break;
                    case 2:
                        telegraphColor = Color.Cyan;
                        break;
                }

                if (calamityGlobalNPC.newAI[0] == shootProjectileGateValue - LaserTelegraphTime)
                {
                    Particle telegraph = new DestroyerReticleTelegraph(
                        npc,
                        telegraphColor,
                        1.5f,
                        0.15f,
                        (int)LaserTelegraphTime);
                    GeneralParticleHandler.SpawnParticle(telegraph);
                }

                if (calamityGlobalNPC.newAI[0] == shootProjectileGateValue - SparkTelegraphTime)
                {
                    Particle spark = new DestroyerSparkTelegraph(
                        npc,
                        telegraphColor * 2f,
                        Color.White,
                        3f,
                        30,
                        Main.rand.NextFloat(MathHelper.ToRadians(3f)) * Main.rand.NextBool().ToDirectionInt());
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                // Shoot lasers
                // Shoot nothing if probe has been launched
                if (calamityGlobalNPC.newAI[0] >= shootProjectileGateValue && ableToFireLaser)
                {
                    if (!masterMode)
                    {
                        int numProbeSegments = 0;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].type == npc.type && Main.npc[i].ai[2] == 0f)
                                numProbeSegments++;
                        }
                        float lerpAmount = MathHelper.Clamp(numProbeSegments / (float)totalSegments, 0f, 1f);
                        float laserShootTimeBonus = (int)MathHelper.Lerp(0f, (shootProjectileTime + bodySegmentTime * lerpAmount) - LaserTelegraphTime, 1f - lerpAmount);
                        calamityGlobalNPC.newAI[0] = laserShootTimeBonus;
                        npc.SyncExtraAI();
                        npc.TargetClosest();
                    }

                    if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                    {
                        if (masterMode)
                        {
                            int numProbeSegments = 0;
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].type == npc.type && Main.npc[i].ai[2] == 0f)
                                    numProbeSegments++;
                            }
                            float lerpAmount = MathHelper.Clamp(numProbeSegments / (float)totalSegments, 0f, 1f);
                            float laserShootTimeBonus = (int)MathHelper.Lerp(0f, (shootProjectileTime + bodySegmentTime * lerpAmount) - LaserTelegraphTime, 1f - lerpAmount);
                            calamityGlobalNPC.newAI[0] = laserShootTimeBonus;
                            npc.SyncExtraAI();
                            npc.TargetClosest();
                        }

                        // Laser speed
                        float projectileSpeed = (masterMode ? 4.5f : 3.5f) + Main.rand.NextFloat() * 1.5f;
                        projectileSpeed += enrageScale;

                        // Set projectile damage and type
                        int projectileType = ProjectileID.DeathLaser;
                        switch (calamityGlobalNPC.destroyerLaserColor)
                        {
                            default:
                            case 0:
                                break;

                            case 1:
                                projectileType = ModContent.ProjectileType<DestroyerCursedLaser>();
                                break;

                            case 2:
                                projectileType = ModContent.ProjectileType<DestroyerElectricLaser>();
                                break;
                        }

                        // Get target vector
                        Vector2 projectileVelocity = (player.Center - npc.Center).SafeNormalize(Vector2.UnitY) * projectileSpeed;
                        Vector2 projectileSpawn = npc.Center + projectileVelocity.SafeNormalize(Vector2.UnitY) * 100f;

                        // Shoot projectile
                        int damage = npc.GetProjectileDamage(projectileType);

                        // Reduce mech boss projectile damage depending on the new ore progression changes
                        if (CalamityServerConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                        {
                            double firstMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert;
                            double secondMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert;
                            if (!NPC.downedMechBossAny)
                                damage = (int)(damage * firstMechMultiplier);
                            else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                damage = (int)(damage * secondMechMultiplier);
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, damage, 0f, Main.myPlayer, 1f, 0f);
                            Main.projectile[proj].timeLeft = 1200;
                        }

                        npc.netUpdate = true;

                        if (masterMode)
                        {
                            calamityGlobalNPC.destroyerLaserColor = -1;
                            npc.SyncDestroyerLaserColor();
                        }
                    }

                    if (!masterMode)
                    {
                        calamityGlobalNPC.destroyerLaserColor = -1;
                        npc.SyncDestroyerLaserColor();
                    }
                }
            }

            if (npc.type == NPCID.TheDestroyer)
            {
                if (npc.life > Main.npc[(int)npc.ai[0]].life)
                    npc.life = Main.npc[(int)npc.ai[0]].life;
            }
            else
            {
                if (npc.life > Main.npc[(int)npc.ai[1]].life)
                    npc.life = Main.npc[(int)npc.ai[1]].life;
            }

            int tilePosX = (int)(npc.position.X / 16f) - 1;
            int tileWidthPosX = (int)((npc.position.X + npc.width) / 16f) + 2;
            int tilePosY = (int)(npc.position.Y / 16f) - 1;
            int tileWidthPosY = (int)((npc.position.Y + npc.height) / 16f) + 2;

            if (tilePosX < 0)
                tilePosX = 0;
            if (tileWidthPosX > Main.maxTilesX)
                tileWidthPosX = Main.maxTilesX;
            if (tilePosY < 0)
                tilePosY = 0;
            if (tileWidthPosY > Main.maxTilesY)
                tileWidthPosY = Main.maxTilesY;

            // Fly or not
            bool shouldFly = flyAtTarget;
            if (!shouldFly)
            {
                for (int k = tilePosX; k < tileWidthPosX; k++)
                {
                    for (int l = tilePosY; l < tileWidthPosY; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[Main.tile[k, l].TileType] || (Main.tileSolidTop[Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            Vector2 tileConvertedPosition;
                            tileConvertedPosition.X = k * 16;
                            tileConvertedPosition.Y = l * 16;
                            if (npc.position.X + npc.width > tileConvertedPosition.X && npc.position.X < tileConvertedPosition.X + 16f && npc.position.Y + npc.height > tileConvertedPosition.Y && npc.position.Y < tileConvertedPosition.Y + 16f)
                            {
                                shouldFly = true;
                                break;
                            }
                        }
                    }
                }
            }

            // Start flying if target is not within a certain distance
            if (!shouldFly)
            {
                npc.localAI[1] = 1f;

                if (npc.type == NPCID.TheDestroyer)
                {
                    Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                    int noFlyZone = 1000;
                    bool outsideNoFlyZone = true;

                    if (npc.position.Y > player.position.Y)
                    {
                        for (int m = 0; m < Main.maxPlayers; m++)
                        {
                            if (Main.player[m].active)
                            {
                                Rectangle noFlyRectangle = new Rectangle((int)Main.player[m].position.X - noFlyZone, (int)Main.player[m].position.Y - noFlyZone, noFlyZone * 2, noFlyZoneBoxHeight);
                                if (rectangle.Intersects(noFlyRectangle))
                                {
                                    outsideNoFlyZone = false;
                                    break;
                                }
                            }
                        }

                        if (outsideNoFlyZone)
                            shouldFly = true;
                    }
                }
            }
            else
                npc.localAI[1] = 0f;

            if (npc.type != NPCID.TheDestroyerBody || !probeLaunched)
            {
                Vector3 lightColor = Color.Red.ToVector3();

                // Light colors
                Vector3 groundColor = new Vector3(0.3f, 0.1f, 0.05f);
                Vector3 flightColor = new Vector3(0.05f, 0.1f, 0.3f);
                Vector3 segmentColor = Vector3.Lerp(groundColor, flightColor, phaseTransitionColorAmount);
                Vector3 telegraphColor = groundColor;

                // Telegraph for the laser breath and body lasers
                float telegraphProgress = 0f;
                if (calamityGlobalNPC.destroyerLaserColor != -1)
                {
                    if (npc.type == NPCID.TheDestroyer && spitLaserSpreads)
                    {
                        float telegraphGateValue = DeathModeLaserBreathGateValue - LaserTelegraphTime;
                        if (calamityGlobalNPC.newAI[0] > telegraphGateValue)
                        {
                            switch (calamityGlobalNPC.destroyerLaserColor)
                            {
                                default:
                                case 0:
                                    break;

                                case 1:
                                    telegraphColor = new Vector3(0.1f, 0.3f, 0.05f);
                                    break;

                                case 2:
                                    telegraphColor = new Vector3(0.05f, 0.2f, 0.2f);
                                    break;
                            }
                            telegraphProgress = MathHelper.Clamp((calamityGlobalNPC.newAI[0] - telegraphGateValue) / LaserTelegraphTime, 0f, 1f);
                        }
                    }
                    else if (npc.type == NPCID.TheDestroyerBody)
                    {
                        float shootProjectileTime = (CalamityWorld.death || BossRushEvent.BossRushActive) ? 270f : 450f;
                        float bodySegmentTime = npc.ai[0] * 30f;
                        float shootProjectileGateValue = bodySegmentTime + shootProjectileTime;
                        float telegraphGateValue = shootProjectileGateValue - LaserTelegraphTime;
                        if (calamityGlobalNPC.newAI[0] > telegraphGateValue)
                        {
                            switch (calamityGlobalNPC.destroyerLaserColor)
                            {
                                default:
                                case 0:
                                    break;

                                case 1:
                                    telegraphColor = new Vector3(0.1f, 0.3f, 0.05f);
                                    break;

                                case 2:
                                    telegraphColor = new Vector3(0.05f, 0.2f, 0.2f);
                                    break;
                            }
                            telegraphProgress = MathHelper.Clamp((calamityGlobalNPC.newAI[0] - telegraphGateValue) / LaserTelegraphTime, 0f, 1f);
                        }
                    }
                }

                Lighting.AddLight(npc.Center, Vector3.Lerp(segmentColor, telegraphColor * 2f, telegraphProgress));
            }

            // Despawn
            bool oblivionWasAlive = npc.localAI[3] == 1f && !oblivionAlive;
            bool oblivionFightDespawn = (oblivionAlive && lifeRatio < 0.75f) || oblivionWasAlive;
            if (player.dead || oblivionFightDespawn)
            {
                shouldFly = false;
                npc.velocity.Y += 2f;

                if (npc.position.Y > Main.worldSurface * 16D)
                {
                    npc.velocity.Y += 2f;
                    segmentVelocity *= 2f;
                }

                if (npc.position.Y > Main.rockLayer * 16D)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        if (Main.npc[n].aiStyle == npc.aiStyle)
                            Main.npc[n].active = false;
                    }
                }
            }

            Vector2 npcCenter = npc.Center;
            float targetTilePosX = player.Center.X;
            float targetTilePosY = player.Center.Y;
            targetTilePosX = (int)(targetTilePosX / 16f) * 16;
            targetTilePosY = (int)(targetTilePosY / 16f) * 16;
            npcCenter.X = (int)(npcCenter.X / 16f) * 16;
            npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
            targetTilePosX -= npcCenter.X;
            targetTilePosY -= npcCenter.Y;
            float targetTileDist = (float)Math.Sqrt(targetTilePosX * targetTilePosX + targetTilePosY * targetTilePosY);

            if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
            {
                int mechdusaSegmentScale = (int)(44f * npc.scale);
                try
                {
                    npcCenter = npc.Center;
                    targetTilePosX = Main.npc[(int)npc.ai[1]].Center.X - npcCenter.X;
                    targetTilePosY = Main.npc[(int)npc.ai[1]].Center.Y - npcCenter.Y;
                }
                catch
                {
                }

                if (mechdusaCurvedSpineSegmentIndex > 0)
                {
                    float absoluteTilePosX = (float)mechdusaSegmentScale - (float)mechdusaSegmentScale * (((float)mechdusaCurvedSpineSegmentIndex - 1f) * 0.1f);
                    if (absoluteTilePosX < 0f)
                        absoluteTilePosX = 0f;

                    if (absoluteTilePosX > (float)mechdusaSegmentScale)
                        absoluteTilePosX = mechdusaSegmentScale;

                    targetTilePosY = Main.npc[(int)npc.ai[1]].Center.Y + absoluteTilePosX - npcCenter.Y;
                }

                npc.rotation = (float)Math.Atan2(targetTilePosY, targetTilePosX) + MathHelper.PiOver2;
                targetTileDist = (float)Math.Sqrt(targetTilePosX * targetTilePosX + targetTilePosY * targetTilePosY);
                if (mechdusaCurvedSpineSegmentIndex > 0)
                    mechdusaSegmentScale = mechdusaSegmentScale / mechdusaCurvedSpineSegments * mechdusaCurvedSpineSegmentIndex;

                targetTileDist = (targetTileDist - mechdusaSegmentScale) / targetTileDist;
                targetTilePosX *= targetTileDist;
                targetTilePosY *= targetTileDist;
                npc.velocity = Vector2.Zero;
                npc.position.X += targetTilePosX;
                npc.position.Y += targetTilePosY;
            }
            else
            {
                if (!shouldFly)
                {
                    npc.velocity.Y += 0.15f;
                    if (masterMode && npc.velocity.Y > 0f && Math.Abs(npc.Center.Y - player.Center.Y) > 360f)
                        npc.velocity.Y += 0.05f;

                    if (npc.velocity.Y > segmentVelocity)
                        npc.velocity.Y = segmentVelocity;

                    // This bool exists to stop the strange wiggle behavior when worms are falling down
                    bool slowXVelocity = Math.Abs(npc.velocity.X) > speed;
                    if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < segmentVelocity * 0.4)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X -= speed * 1.1f;
                        else
                            npc.velocity.X += speed * 1.1f;
                    }
                    else if (npc.velocity.Y == segmentVelocity)
                    {
                        if (slowXVelocity)
                        {
                            if (npc.velocity.X < targetTilePosX)
                                npc.velocity.X += speed;
                            else if (npc.velocity.X > targetTilePosX)
                                npc.velocity.X -= speed;
                        }
                        else
                            npc.velocity.X = 0f;
                    }
                    else if (npc.velocity.Y > 4f)
                    {
                        if (slowXVelocity)
                        {
                            if (npc.velocity.X < 0f)
                                npc.velocity.X += speed * 0.9f;
                            else
                                npc.velocity.X -= speed * 0.9f;
                        }
                        else
                            npc.velocity.X = 0f;
                    }
                }
                else
                {
                    if (npc.soundDelay == 0)
                    {
                        float soundDelay = targetTileDist / 40f;
                        if (soundDelay < 10f)
                            soundDelay = 10f;
                        if (soundDelay > 20f)
                            soundDelay = 20f;

                        npc.soundDelay = (int)soundDelay;
                        SoundEngine.PlaySound(SoundID.WormDig, npc.Center);
                    }

                    targetTileDist = (float)Math.Sqrt(targetTilePosX * targetTilePosX + targetTilePosY * targetTilePosY);
                    float absoluteTilePosX = Math.Abs(targetTilePosX);
                    float absoluteTilePosY = Math.Abs(targetTilePosY);
                    float tileToReachTarget = segmentVelocity / targetTileDist;
                    targetTilePosX *= tileToReachTarget;
                    targetTilePosY *= tileToReachTarget;

                    bool flyWyvernMovement = false;
                    if (flyAtTarget)
                    {
                        if (((npc.velocity.X > 0f && targetTilePosX < 0f) || (npc.velocity.X < 0f && targetTilePosX > 0f) || (npc.velocity.Y > 0f && targetTilePosY < 0f) || (npc.velocity.Y < 0f && targetTilePosY > 0f)) && Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) > speed / 2f && targetTileDist < 600f)
                        {
                            flyWyvernMovement = true;

                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < segmentVelocity)
                                npc.velocity *= 1.1f;
                        }

                        if (npc.position.Y > player.position.Y)
                        {
                            flyWyvernMovement = true;

                            if (Math.Abs(npc.velocity.X) < segmentVelocity / 2f)
                            {
                                if (npc.velocity.X == 0f)
                                    npc.velocity.X -= npc.direction;

                                npc.velocity.X *= 1.1f;
                            }
                            else if (npc.velocity.Y > -segmentVelocity)
                                npc.velocity.Y -= speed;
                        }
                    }

                    if (!flyWyvernMovement)
                    {
                        if (!flyAtTarget)
                        {
                            if (((npc.velocity.X > 0f && targetTilePosX > 0f) || (npc.velocity.X < 0f && targetTilePosX < 0f)) && ((npc.velocity.Y > 0f && targetTilePosY > 0f) || (npc.velocity.Y < 0f && targetTilePosY < 0f)))
                            {
                                if (npc.velocity.X < targetTilePosX)
                                    npc.velocity.X += turnSpeed;
                                else if (npc.velocity.X > targetTilePosX)
                                    npc.velocity.X -= turnSpeed;
                                if (npc.velocity.Y < targetTilePosY)
                                    npc.velocity.Y += turnSpeed;
                                else if (npc.velocity.Y > targetTilePosY)
                                    npc.velocity.Y -= turnSpeed;
                            }
                        }

                        if ((npc.velocity.X > 0f && targetTilePosX > 0f) || (npc.velocity.X < 0f && targetTilePosX < 0f) || (npc.velocity.Y > 0f && targetTilePosY > 0f) || (npc.velocity.Y < 0f && targetTilePosY < 0f))
                        {
                            if (npc.velocity.X < targetTilePosX)
                                npc.velocity.X += speed;
                            else if (npc.velocity.X > targetTilePosX)
                                npc.velocity.X -= speed;
                            if (npc.velocity.Y < targetTilePosY)
                                npc.velocity.Y += speed;
                            else if (npc.velocity.Y > targetTilePosY)
                                npc.velocity.Y -= speed;

                            if (Math.Abs(targetTilePosY) < segmentVelocity * 0.2 && ((npc.velocity.X > 0f && targetTilePosX < 0f) || (npc.velocity.X < 0f && targetTilePosX > 0f)))
                            {
                                if (npc.velocity.Y > 0f)
                                    npc.velocity.Y += speed * 2f;
                                else
                                    npc.velocity.Y -= speed * 2f;
                            }
                            if (Math.Abs(targetTilePosX) < segmentVelocity * 0.2 && ((npc.velocity.Y > 0f && targetTilePosY < 0f) || (npc.velocity.Y < 0f && targetTilePosY > 0f)))
                            {
                                if (npc.velocity.X > 0f)
                                    npc.velocity.X += speed * 2f;
                                else
                                    npc.velocity.X -= speed * 2f;
                            }
                        }
                        else if (absoluteTilePosX > absoluteTilePosY)
                        {
                            if (npc.velocity.X < targetTilePosX)
                                npc.velocity.X += speed * 1.1f;
                            else if (npc.velocity.X > targetTilePosX)
                                npc.velocity.X -= speed * 1.1f;

                            if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < segmentVelocity * 0.5)
                            {
                                if (npc.velocity.Y > 0f)
                                    npc.velocity.Y += speed;
                                else
                                    npc.velocity.Y -= speed;
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < targetTilePosY)
                                npc.velocity.Y += speed * 1.1f;
                            else if (npc.velocity.Y > targetTilePosY)
                                npc.velocity.Y -= speed * 1.1f;

                            if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < segmentVelocity * 0.5)
                            {
                                if (npc.velocity.X > 0f)
                                    npc.velocity.X += speed;
                                else
                                    npc.velocity.X -= speed;
                            }
                        }
                    }
                }

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;

                if (npc.type == NPCID.TheDestroyer)
                {
                    if (shouldFly)
                    {
                        if (npc.localAI[0] != 1f)
                            npc.netUpdate = true;

                        npc.localAI[0] = 1f;
                    }
                    else
                    {
                        if (npc.localAI[0] != 0f)
                            npc.netUpdate = true;

                        npc.localAI[0] = 0f;
                    }

                    if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                        npc.netUpdate = true;
                }
            }

            // Force the fucker to turn around in ground phase in Master
            // Turns slower if Oblivion is alive, for fairness
            if (npc.type == NPCID.TheDestroyer && masterMode && !flyAtTarget)
            {
                if (npc.Distance(player.Center) > 2000f)
                    npc.velocity += (player.Center - npc.Center).SafeNormalize(Vector2.UnitY) * (oblivionAlive ? speed : turnSpeed);
            }

            if (NPC.IsMechQueenUp && npc.type == NPCID.TheDestroyer)
            {
                NPC nPC = Main.npc[NPC.mechQueen];
                Vector2 mechQueenCenter = nPC.GetMechQueenCenter();
                Vector2 mechdusaSpinningVector = new Vector2(0f, 100f);
                Vector2 spinningpoint = mechQueenCenter + mechdusaSpinningVector;
                float mechdusaRotation = nPC.velocity.X * 0.025f;
                spinningpoint = spinningpoint.RotatedBy(mechdusaRotation, mechQueenCenter);
                npc.position = spinningpoint - npc.Size / 2f + nPC.velocity;
                npc.velocity.X = 0f;
                npc.velocity.Y = 0f;
                npc.rotation = mechdusaRotation * 0.75f + (float)Math.PI;
            }

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = segmentVelocity * 0.25f;
            float minimalDamageVelocity = segmentVelocity * 0.5f;
            if (npc.type == NPCID.TheDestroyer)
            {
                if (npc.velocity.Length() <= minimalContactDamageVelocity)
                {
                    npc.damage = (int)Math.Round(npc.defDamage * 0.5);
                }
                else
                {
                    float velocityDamageScalar = MathHelper.Clamp((npc.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                    npc.damage = (int)MathHelper.Lerp((float)Math.Round(npc.defDamage * 0.5), npc.defDamage, velocityDamageScalar);
                }
            }
            else
            {
                float bodyAndTailVelocity = (npc.position - npc.oldPosition).Length();
                if (bodyAndTailVelocity <= minimalContactDamageVelocity)
                {
                    npc.damage = 0;
                }
                else
                {
                    float velocityDamageScalar = MathHelper.Clamp((bodyAndTailVelocity - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                    npc.damage = (int)MathHelper.Lerp(0f, npc.defDamage, velocityDamageScalar);
                }
            }

            return false;
        }

        public static bool VanillaDestroyerAI(NPC npc, Mod mod)
        {
            int num = 0;
            int num2 = 10;
            if (NPC.IsMechQueenUp && npc.type != NPCID.TheDestroyer)
            {
                int num3 = (int)npc.ai[1];
                while (num3 > 0 && num3 < Main.maxNPCs)
                {
                    if (Main.npc[num3].active && Main.npc[num3].type >= NPCID.TheDestroyer && Main.npc[num3].type <= NPCID.TheDestroyerTail)
                    {
                        num++;
                        if (Main.npc[num3].type == NPCID.TheDestroyer)
                            break;

                        if (num >= num2)
                        {
                            num = 0;
                            break;
                        }

                        num3 = (int)Main.npc[num3].ai[1];
                        continue;
                    }

                    num = 0;
                    break;
                }
            }

            // 10 seconds of resistance to prevent spawn killing
            if (npc.Calamity().newAI[1] < DRIncreaseTime)
                npc.Calamity().newAI[1] += 1f;

            npc.Calamity().CurrentlyIncreasingDefenseOrDR = npc.Calamity().newAI[1] < DRIncreaseTime;

            if (npc.ai[3] > 0f)
                npc.realLife = (int)npc.ai[3];

            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            if (npc.type >= NPCID.TheDestroyer && npc.type <= NPCID.TheDestroyerTail)
            {
                if (npc.type == NPCID.TheDestroyer || (npc.type != NPCID.TheDestroyer && Main.npc[(int)npc.ai[1]].alpha < 128))
                {
                    if (npc.alpha != 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int num4 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.TheDestroyer, 0f, 0f, 100, default(Color), 2f);
                            Main.dust[num4].noGravity = true;
                            Main.dust[num4].noLight = true;
                        }
                    }

                    npc.alpha -= 42;
                    if (npc.alpha < 0)
                        npc.alpha = 0;
                }
            }

            if (npc.type > NPCID.TheDestroyer)
            {
                bool flag = false;
                if (npc.ai[1] <= 0f)
                    flag = true;
                else if (Main.npc[(int)npc.ai[1]].life <= 0)
                    flag = true;

                if (flag)
                {
                    npc.life = 0;
                    npc.HitEffect();
                    npc.checkDead();
                }
            }

            int destroyerSegmentsCount = NPC.GetDestroyerSegmentsCount();
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.ai[0] == 0f && npc.type == NPCID.TheDestroyer)
                {
                    npc.ai[3] = npc.whoAmI;
                    npc.realLife = npc.whoAmI;
                    int num5 = 0;
                    int num6 = npc.whoAmI;
                    for (int j = 0; j <= destroyerSegmentsCount; j++)
                    {
                        int num7 = NPCID.TheDestroyerBody;
                        if (j == destroyerSegmentsCount)
                            num7 = NPCID.TheDestroyerTail;

                        num5 = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X), (int)(npc.position.Y + (float)npc.height), num7, npc.whoAmI);
                        Main.npc[num5].ai[3] = npc.whoAmI;
                        Main.npc[num5].realLife = npc.whoAmI;
                        Main.npc[num5].ai[1] = num6;
                        Main.npc[num6].ai[0] = num5;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num5);
                        num6 = num5;
                    }
                }
            }

            if (npc.type == NPCID.TheDestroyerBody)
            {
                bool probeLaunched = npc.ai[2] == 1f;
                bool ableToFireLaser = npc.Calamity().destroyerLaserColor != -1;

                if (npc.Calamity().destroyerLaserColor == -1 && !probeLaunched && Main.rand.NextBool(OneInXChanceToFireLaser))
                {
                    npc.Calamity().destroyerLaserColor = 0;
                    npc.SyncDestroyerLaserColor();
                }

                if (probeLaunched && ableToFireLaser)
                {
                    npc.Calamity().destroyerLaserColor = -1;
                    npc.SyncDestroyerLaserColor();
                }

                float shootProjectileTime = Main.masterMode ? 500f : Main.expertMode ? 700f : 900f;
                float bodySegmentTime = npc.ai[0] * 30f;
                float shootProjectileGateValue = bodySegmentTime + shootProjectileTime;

                float laserTimerIncrement = (npc.localAI[0] > shootProjectileGateValue - LaserTelegraphTime) ? 1f : 2f;
                if (ableToFireLaser)
                    npc.localAI[0] += laserTimerIncrement;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Sync newAI every 20 frames for the new telegraph
                    if (npc.localAI[0] % 20f == 10f && ableToFireLaser)
                        npc.SyncVanillaLocalAI();
                }

                Color telegraphColor = Color.Transparent;
                switch (npc.Calamity().destroyerLaserColor)
                {
                    case 0:
                        telegraphColor = Color.Red;
                        break;
                    case 1:
                        telegraphColor = Color.Green;
                        break;
                    case 2:
                        telegraphColor = Color.Cyan;
                        break;
                }

                if (npc.localAI[0] == shootProjectileGateValue - LaserTelegraphTime)
                {
                    Particle telegraph = new DestroyerReticleTelegraph(
                        npc,
                        telegraphColor,
                        1.5f,
                        0.15f,
                        (int)LaserTelegraphTime);
                    GeneralParticleHandler.SpawnParticle(telegraph);
                }

                if (npc.localAI[0] == shootProjectileGateValue - SparkTelegraphTime)
                {
                    Particle spark = new DestroyerSparkTelegraph(
                        npc,
                        telegraphColor * 2f,
                        Color.White,
                        3f,
                        30,
                        Main.rand.NextFloat(MathHelper.ToRadians(3f)) * Main.rand.NextBool().ToDirectionInt());
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                if (npc.localAI[0] >= shootProjectileGateValue && ableToFireLaser)
                {
                    int numProbeSegments = 0;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].type == npc.type && Main.npc[i].ai[2] == 0f)
                            numProbeSegments++;
                    }
                    float lerpAmount = MathHelper.Clamp(numProbeSegments / (float)destroyerSegmentsCount, 0f, 1f);
                    float laserShootTimeBonus = (int)MathHelper.Lerp(0f, (shootProjectileTime + bodySegmentTime * lerpAmount) - LaserTelegraphTime, 1f - lerpAmount);
                    npc.localAI[0] = laserShootTimeBonus;
                    npc.SyncVanillaLocalAI();
                    npc.TargetClosest();
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float laserVelocity = (Main.masterMode ? 3.5f : Main.expertMode ? 3f : 2.5f) + Main.rand.NextFloat() * 1.5f;
                        Vector2 vector = npc.Center;
                        float num8 = Main.player[npc.target].Center.X - vector.X + (float)Main.rand.Next(-2, 3);
                        float num9 = Main.player[npc.target].Center.Y - vector.Y + (float)Main.rand.Next(-2, 3);
                        float num10 = (float)Math.Sqrt(num8 * num8 + num9 * num9);
                        num10 = laserVelocity / num10;
                        num8 *= num10;
                        num9 *= num10;
                        num8 += (float)Main.rand.Next(-2, 3) * 0.05f;
                        num9 += (float)Main.rand.Next(-2, 3) * 0.05f;

                        int type = ProjectileID.DeathLaser;
                        int damage = npc.GetProjectileDamage(type);

                        // Reduce mech boss projectile damage depending on the new ore progression changes
                        if (CalamityServerConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                        {
                            double firstMechMultiplier = Main.expertMode ? CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert : CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Classic;
                            double secondMechMultiplier = Main.expertMode ? CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert : CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Classic;
                            if (!NPC.downedMechBossAny)
                                damage = (int)(damage * firstMechMultiplier);
                            else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                damage = (int)(damage * secondMechMultiplier);
                        }

                        Vector2 laserVelocityActual = new Vector2(num8, num9);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int num12 = Projectile.NewProjectile(npc.GetSource_FromAI(), vector + laserVelocityActual.SafeNormalize(Vector2.UnitY) * 100f, laserVelocityActual, type, damage, 0f, Main.myPlayer, 1f, 0f);
                            Main.projectile[num12].timeLeft = 1200;
                        }

                        npc.netUpdate = true;
                    }

                    npc.Calamity().destroyerLaserColor = -1;
                    npc.SyncDestroyerLaserColor();
                }
            }

            if (npc.type == NPCID.TheDestroyer)
            {
                if (npc.life > Main.npc[(int)npc.ai[0]].life)
                    npc.life = Main.npc[(int)npc.ai[0]].life;
            }
            else
            {
                if (npc.life > Main.npc[(int)npc.ai[1]].life)
                    npc.life = Main.npc[(int)npc.ai[1]].life;
            }

            // Calculate aggression based on how many broken segments there are
            float brokenSegmentAggressionMultiplier = 1f;
            if (npc.type == NPCID.TheDestroyer)
            {
                int numProbeSegments = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == NPCID.TheDestroyerBody && Main.npc[i].ai[2] == 0f)
                        numProbeSegments++;
                }
                brokenSegmentAggressionMultiplier += (1f - MathHelper.Clamp(numProbeSegments / (float)destroyerSegmentsCount, 0f, 1f)) * 0.25f;
            }

            int num13 = (int)(npc.position.X / 16f) - 1;
            int num14 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
            int num15 = (int)(npc.position.Y / 16f) - 1;
            int num16 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
            if (num13 < 0)
                num13 = 0;

            if (num14 > Main.maxTilesX)
                num14 = Main.maxTilesX;

            if (num15 < 0)
                num15 = 0;

            if (num16 > Main.maxTilesY)
                num16 = Main.maxTilesY;

            bool flag2 = false;
            if (!flag2)
            {
                Vector2 vector2 = default(Vector2);
                for (int k = num13; k < num14; k++)
                {
                    for (int l = num15; l < num16; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[Main.tile[k, l].TileType] || (Main.tileSolidTop[Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            vector2.X = k * 16;
                            vector2.Y = l * 16;
                            if (npc.position.X + (float)npc.width > vector2.X && npc.position.X < vector2.X + 16f && npc.position.Y + (float)npc.height > vector2.Y && npc.position.Y < vector2.Y + 16f)
                            {
                                flag2 = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (!flag2)
            {
                if (npc.type != NPCID.TheDestroyerBody || npc.ai[2] != 1f)
                {
                    Vector3 lightColor = Color.Red.ToVector3();

                    // Light colors
                    Vector3 groundColor = new Vector3(0.3f, 0.1f, 0.05f);
                    Vector3 segmentColor = groundColor;
                    Vector3 telegraphColor = groundColor;

                    // Telegraph for the laser breath and body lasers
                    float telegraphProgress = 0f;
                    if (npc.Calamity().destroyerLaserColor != -1)
                    {
                        if (npc.type == NPCID.TheDestroyerBody)
                        {
                            float shootProjectileTime = Main.masterMode ? 500f : Main.expertMode ? 700f : 900f;
                            float bodySegmentTime = npc.ai[0] * 30f;
                            float shootProjectileGateValue = bodySegmentTime + shootProjectileTime;
                            float telegraphGateValue = shootProjectileGateValue - LaserTelegraphTime;
                            if (npc.localAI[0] > telegraphGateValue)
                                telegraphProgress = MathHelper.Clamp((npc.localAI[0] - telegraphGateValue) / LaserTelegraphTime, 0f, 1f);
                        }
                    }

                    Lighting.AddLight(npc.Center, Vector3.Lerp(segmentColor, telegraphColor * 2f, telegraphProgress));
                }

                npc.localAI[1] = 1f;
                if (npc.type == NPCID.TheDestroyer)
                {
                    Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                    int num17 = 1000;
                    int height = (num17 * 2) - (Main.masterMode ? 700 : Main.expertMode ? 400 : 0);
                    bool flag3 = true;
                    if (npc.position.Y > Main.player[npc.target].position.Y)
                    {
                        for (int m = 0; m < Main.maxPlayers; m++)
                        {
                            if (Main.player[m].active)
                            {
                                Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X - num17, (int)Main.player[m].position.Y - num17, num17 * 2, height);
                                if (rectangle.Intersects(rectangle2))
                                {
                                    flag3 = false;
                                    break;
                                }
                            }
                        }

                        if (flag3)
                            flag2 = true;
                    }
                }
            }
            else
                npc.localAI[1] = 0f;

            float num18 = (Main.masterMode ? 24f : Main.expertMode ? 20f : 16f) * brokenSegmentAggressionMultiplier;
            if (Main.IsItDay() || Main.player[npc.target].dead)
            {
                flag2 = false;
                npc.velocity.Y += 1f;
                if ((double)npc.position.Y > Main.worldSurface * 16D)
                {
                    npc.velocity.Y += 1f;
                    num18 *= 2f;
                }

                if ((double)npc.position.Y > Main.rockLayer * 16D)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        if (Main.npc[n].aiStyle == npc.aiStyle)
                            Main.npc[n].active = false;
                    }
                }
            }

            float num19 = 0.1f;
            float num20 = 0.15f;
            if (Main.expertMode)
            {
                num19 = Main.masterMode ? 0.2f : 0.15f;
                num20 = Main.masterMode ? 0.3f : 0.225f;
            }

            if (Main.getGoodWorld)
            {
                num19 *= 1.2f;
                num20 *= 1.2f;
            }

            num19 *= brokenSegmentAggressionMultiplier;
            num20 *= brokenSegmentAggressionMultiplier;

            Vector2 vector3 = npc.Center;
            float num21 = Main.player[npc.target].Center.X;
            float num22 = Main.player[npc.target].Center.Y;
            num21 = (int)(num21 / 16f) * 16;
            num22 = (int)(num22 / 16f) * 16;
            vector3.X = (int)(vector3.X / 16f) * 16;
            vector3.Y = (int)(vector3.Y / 16f) * 16;
            num21 -= vector3.X;
            num22 -= vector3.Y;
            float num23 = (float)Math.Sqrt(num21 * num21 + num22 * num22);
            if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
            {
                int num24 = (int)(44f * npc.scale);
                try
                {
                    vector3 = npc.Center;
                    num21 = Main.npc[(int)npc.ai[1]].Center.X - vector3.X;
                    num22 = Main.npc[(int)npc.ai[1]].Center.Y - vector3.Y;
                }
                catch
                {
                }

                if (num > 0)
                {
                    float num25 = (float)num24 - (float)num24 * (((float)num - 1f) * 0.1f);
                    if (num25 < 0f)
                        num25 = 0f;

                    if (num25 > (float)num24)
                        num25 = num24;

                    num22 = Main.npc[(int)npc.ai[1]].Center.Y + num25 - vector3.Y;
                }

                npc.rotation = (float)Math.Atan2(num22, num21) + MathHelper.PiOver2;
                num23 = (float)Math.Sqrt(num21 * num21 + num22 * num22);
                if (num > 0)
                    num24 = num24 / num2 * num;

                num23 = (num23 - (float)num24) / num23;
                num21 *= num23;
                num22 *= num23;
                npc.velocity = Vector2.Zero;
                npc.position.X += num21;
                npc.position.Y += num22;
                num21 = Main.npc[(int)npc.ai[1]].Center.X - vector3.X;
                num22 = Main.npc[(int)npc.ai[1]].Center.Y - vector3.Y;
                npc.rotation = (float)Math.Atan2(num22, num21) + MathHelper.PiOver2;
            }
            else
            {
                if (!flag2)
                {
                    npc.velocity.Y += 0.15f;
                    if (Main.masterMode && npc.velocity.Y > 0f)
                        npc.velocity.Y += 0.05f;

                    if (npc.velocity.Y > num18)
                        npc.velocity.Y = num18;

                    // This bool exists to stop the strange wiggle behavior when worms are falling down
                    bool slowXVelocity = Math.Abs(npc.velocity.X) > num19;
                    if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num18 * 0.4)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X -= num19 * 1.1f;
                        else
                            npc.velocity.X += num19 * 1.1f;
                    }
                    else if (npc.velocity.Y == num18)
                    {
                        if (slowXVelocity)
                        {
                            if (npc.velocity.X < num21)
                                npc.velocity.X += num19;
                            else if (npc.velocity.X > num21)
                                npc.velocity.X -= num19;
                        }
                        else
                            npc.velocity.X = 0f;
                    }
                    else if (npc.velocity.Y > 4f)
                    {
                        if (slowXVelocity)
                        {
                            if (npc.velocity.X < 0f)
                                npc.velocity.X += num19 * 0.9f;
                            else
                                npc.velocity.X -= num19 * 0.9f;
                        }
                        else
                            npc.velocity.X = 0f;
                    }
                }
                else
                {
                    if (npc.soundDelay == 0)
                    {
                        float num26 = num23 / 40f;
                        if (num26 < 10f)
                            num26 = 10f;

                        if (num26 > 20f)
                            num26 = 20f;

                        npc.soundDelay = (int)num26;
                        SoundEngine.PlaySound(SoundID.WormDig, npc.Center);
                    }

                    num23 = (float)Math.Sqrt(num21 * num21 + num22 * num22);
                    float num27 = Math.Abs(num21);
                    float num28 = Math.Abs(num22);
                    float num29 = num18 / num23;
                    num21 *= num29;
                    num22 *= num29;
                    if (((npc.velocity.X > 0f && num21 > 0f) || (npc.velocity.X < 0f && num21 < 0f)) && ((npc.velocity.Y > 0f && num22 > 0f) || (npc.velocity.Y < 0f && num22 < 0f)))
                    {
                        if (npc.velocity.X < num21)
                            npc.velocity.X += num20;
                        else if (npc.velocity.X > num21)
                            npc.velocity.X -= num20;

                        if (npc.velocity.Y < num22)
                            npc.velocity.Y += num20;
                        else if (npc.velocity.Y > num22)
                            npc.velocity.Y -= num20;
                    }

                    if ((npc.velocity.X > 0f && num21 > 0f) || (npc.velocity.X < 0f && num21 < 0f) || (npc.velocity.Y > 0f && num22 > 0f) || (npc.velocity.Y < 0f && num22 < 0f))
                    {
                        if (npc.velocity.X < num21)
                            npc.velocity.X += num19;
                        else if (npc.velocity.X > num21)
                            npc.velocity.X -= num19;

                        if (npc.velocity.Y < num22)
                            npc.velocity.Y += num19;
                        else if (npc.velocity.Y > num22)
                            npc.velocity.Y -= num19;

                        if ((double)Math.Abs(num22) < (double)num18 * 0.2 && ((npc.velocity.X > 0f && num21 < 0f) || (npc.velocity.X < 0f && num21 > 0f)))
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y += num19 * 2f;
                            else
                                npc.velocity.Y -= num19 * 2f;
                        }

                        if ((double)Math.Abs(num21) < (double)num18 * 0.2 && ((npc.velocity.Y > 0f && num22 < 0f) || (npc.velocity.Y < 0f && num22 > 0f)))
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X += num19 * 2f;
                            else
                                npc.velocity.X -= num19 * 2f;
                        }
                    }
                    else if (num27 > num28)
                    {
                        if (npc.velocity.X < num21)
                            npc.velocity.X += num19 * 1.1f;
                        else if (npc.velocity.X > num21)
                            npc.velocity.X -= num19 * 1.1f;

                        if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num18 * 0.5)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y += num19;
                            else
                                npc.velocity.Y -= num19;
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < num22)
                            npc.velocity.Y += num19 * 1.1f;
                        else if (npc.velocity.Y > num22)
                            npc.velocity.Y -= num19 * 1.1f;

                        if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num18 * 0.5)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X += num19;
                            else
                                npc.velocity.X -= num19;
                        }
                    }
                }

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;
                if (npc.type == NPCID.TheDestroyer)
                {
                    if (flag2)
                    {
                        if (npc.localAI[0] != 1f)
                            npc.netUpdate = true;

                        npc.localAI[0] = 1f;
                    }
                    else
                    {
                        if (npc.localAI[0] != 0f)
                            npc.netUpdate = true;

                        npc.localAI[0] = 0f;
                    }

                    if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                        npc.netUpdate = true;
                }
            }

            if (NPC.IsMechQueenUp && npc.type == NPCID.TheDestroyer)
            {
                NPC nPC = Main.npc[NPC.mechQueen];
                Vector2 mechQueenCenter = nPC.GetMechQueenCenter();
                Vector2 vector4 = new Vector2(0f, 100f);
                Vector2 spinningpoint = mechQueenCenter + vector4;
                float num30 = nPC.velocity.X * 0.025f;
                spinningpoint = spinningpoint.RotatedBy(num30, mechQueenCenter);
                npc.position = spinningpoint - npc.Size / 2f + nPC.velocity;
                npc.velocity.X = 0f;
                npc.velocity.Y = 0f;
                npc.rotation = num30 * 0.75f + (float)Math.PI;
            }

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = num18 * 0.25f;
            float minimalDamageVelocity = num18 * 0.5f;
            if (npc.type == NPCID.TheDestroyer)
            {
                if (npc.velocity.Length() <= minimalContactDamageVelocity)
                {
                    npc.damage = (int)Math.Round(npc.defDamage * 0.5);
                }
                else
                {
                    float velocityDamageScalar = MathHelper.Clamp((npc.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                    npc.damage = (int)MathHelper.Lerp((float)Math.Round(npc.defDamage * 0.5), npc.defDamage, velocityDamageScalar);
                }
            }
            else
            {
                float bodyAndTailVelocity = (npc.position - npc.oldPosition).Length();
                if (bodyAndTailVelocity <= minimalContactDamageVelocity)
                {
                    npc.damage = 0;
                }
                else
                {
                    float velocityDamageScalar = MathHelper.Clamp((bodyAndTailVelocity - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                    npc.damage = (int)MathHelper.Lerp(0f, npc.defDamage, velocityDamageScalar);
                }
            }

            return false;
        }

        public static bool BuffedProbeAI(NPC npc, Mod mod)
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool masterMode = Main.masterMode || bossRush;
            bool oblivionAlive = npc.ai[1] == 1f;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            NPCAimedTarget targetData = npc.GetTargetData();
            bool targetDead = false;
            if (targetData.Type == NPCTargetType.Player)
                targetDead = Main.player[npc.target].dead;

            float velocity = bossRush ? 12f : masterMode ? 8.4f : 7.2f;
            float acceleration = bossRush ? 0.1f : masterMode ? 0.07f : 0.06f;

            Vector2 probeCenter = npc.Center;
            float probeTargetX = targetData.Center.X;
            float probeTargetY = targetData.Center.Y;
            probeTargetX = (int)(probeTargetX / 8f) * 8;
            probeTargetY = (int)(probeTargetY / 8f) * 8;
            probeCenter.X = (int)(probeCenter.X / 8f) * 8;
            probeCenter.Y = (int)(probeCenter.Y / 8f) * 8;
            probeTargetX -= probeCenter.X;
            probeTargetY -= probeCenter.Y;
            float distanceFromTarget = (float)Math.Sqrt(probeTargetX * probeTargetX + probeTargetY * probeTargetY);
            float distance2 = distanceFromTarget;

            bool farAwayFromTarget = false;
            if (distanceFromTarget > 600f)
                farAwayFromTarget = true;

            if (distanceFromTarget == 0f)
            {
                probeTargetX = npc.velocity.X;
                probeTargetY = npc.velocity.Y;
            }
            else
            {
                distanceFromTarget = velocity / distanceFromTarget;
                probeTargetX *= distanceFromTarget;
                probeTargetY *= distanceFromTarget;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (masterMode && !bossRush && npc.ai[1] == 0f)
                {
                    if (Main.npc[i].active && (Main.npc[i].type == ModContent.NPCType<SkeletronPrime2>() || Main.npc[i].type == NPCID.SkeletronPrime))
                        npc.ai[1] = 1f;
                }

                if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type)
                {
                    Vector2 otherProbeDist = Main.npc[i].Center - npc.Center;
                    if (otherProbeDist.Length() < (npc.width + npc.height))
                    {
                        otherProbeDist = otherProbeDist.SafeNormalize(Vector2.UnitY);
                        otherProbeDist *= -0.1f;
                        npc.velocity += otherProbeDist;
                        Main.npc[i].velocity -= otherProbeDist;
                    }
                }
            }

            if (distance2 > 100f)
            {
                npc.ai[0] += 1f;
                if (npc.ai[0] > 0f)
                    npc.velocity.Y += 0.023f;
                else
                    npc.velocity.Y -= 0.023f;

                if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                    npc.velocity.X += 0.023f;
                else
                    npc.velocity.X -= 0.023f;

                if (npc.ai[0] > 200f)
                    npc.ai[0] = -200f;
            }

            if (targetDead)
            {
                probeTargetX = npc.direction * velocity / 2f;
                probeTargetY = -velocity / 2f;
            }

            if (npc.ai[3] != 0f)
            {
                if (NPC.IsMechQueenUp)
                {
                    NPC nPC = Main.npc[NPC.mechQueen];
                    Vector2 tileConvertedPosition = new Vector2(26f * npc.ai[3], 0f);
                    int mechdusaProbe = (int)npc.ai[2];
                    if (mechdusaProbe < 0 || mechdusaProbe >= Main.maxNPCs)
                    {
                        mechdusaProbe = NPC.FindFirstNPC(NPCID.TheDestroyer);
                        npc.ai[2] = mechdusaProbe;
                        npc.netUpdate = true;
                    }

                    if (mechdusaProbe > -1)
                    {
                        NPC nPC2 = Main.npc[mechdusaProbe];
                        if (!nPC2.active || nPC2.type != NPCID.TheDestroyer)
                        {
                            npc.dontTakeDamage = false;
                            if (npc.ai[3] > 0f)
                                npc.netUpdate = true;

                            npc.ai[3] = 0f;
                        }
                        else
                        {
                            Vector2 spinningpoint = nPC2.Center + tileConvertedPosition;
                            spinningpoint = spinningpoint.RotatedBy(nPC2.rotation, nPC2.Center);
                            npc.Center = spinningpoint;
                            npc.velocity = nPC.velocity;
                            npc.dontTakeDamage = true;
                        }
                    }
                    else
                    {
                        npc.dontTakeDamage = false;
                        if (npc.ai[3] > 0f)
                            npc.netUpdate = true;

                        npc.ai[3] = 0f;
                    }
                }
                else
                {
                    npc.dontTakeDamage = false;
                    if (npc.ai[3] > 0f)
                        npc.netUpdate = true;

                    npc.ai[3] = 0f;
                }
            }
            else
            {
                npc.dontTakeDamage = false;

                if (npc.velocity.X < probeTargetX)
                    npc.velocity.X += acceleration;
                else if (npc.velocity.X > probeTargetX)
                    npc.velocity.X -= acceleration;

                if (npc.velocity.Y < probeTargetY)
                    npc.velocity.Y += acceleration;
                else if (npc.velocity.Y > probeTargetY)
                    npc.velocity.Y -= acceleration;
            }

            npc.localAI[0] += 1f;
            if (npc.justHit && !masterMode)
                npc.localAI[0] = 0f;

            float laserGateValue = NPC.IsMechQueenUp ? 360f : bossRush ? 150f : 240f;
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= laserGateValue)
            {
                npc.localAI[0] = 0f;
                if (targetData.Type != 0 && Collision.CanHit(npc.position, npc.width, npc.height, targetData.Position, targetData.Width, targetData.Height))
                {
                    int type = ProjectileID.PinkLaser;
                    int damage = npc.GetProjectileDamage(type);

                    // Reduce mech boss projectile damage depending on the new ore progression changes
                    if (CalamityServerConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                    {
                        double firstMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert;
                        double secondMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert;
                        if (!NPC.downedMechBossAny)
                            damage = (int)(damage * firstMechMultiplier);
                        else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                            damage = (int)(damage * secondMechMultiplier);
                    }

                    int totalProjectiles = oblivionAlive ? 2 : (CalamityWorld.death || bossRush) ? 3 : 1;
                    Vector2 npcCenter = new Vector2(probeTargetX, probeTargetY);
                    if (NPC.IsMechQueenUp)
                    {
                        Vector2 v = targetData.Center - npc.Center - targetData.Velocity * 20f;
                        float projectileVelocity = 8f;
                        npcCenter = v.SafeNormalize(Vector2.UnitY) * projectileVelocity;
                    }
                    for (int i = 0; i < totalProjectiles; i++)
                    {
                        float velocityMultiplier = 1f;
                        switch (i)
                        {
                            case 0:
                                break;
                            case 1:
                                velocityMultiplier = 0.9f;
                                break;
                            case 2:
                                velocityMultiplier = 0.8f;
                                break;
                        }
                        Vector2 laserVelocity = npcCenter * velocityMultiplier;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), probeCenter + laserVelocity.SafeNormalize(Vector2.UnitY) * 50f, laserVelocity, type, damage, 0f, Main.myPlayer);
                    }

                    npc.netUpdate = true;
                }
            }

            int x = (int)npc.Center.X / 16;
            int y = (int)npc.Center.Y / 16;
            if (WorldGen.InWorld(x, y) && !WorldGen.SolidTile(x, y))
                Lighting.AddLight((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), 0.3f, 0.1f, 0.05f);

            if (probeTargetX > 0f)
            {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2(probeTargetY, probeTargetX);
            }
            if (probeTargetX < 0f)
            {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2(probeTargetY, probeTargetX) + MathHelper.Pi;
            }

            float tilePosX = -0.7f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * tilePosX;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    npc.velocity.X = 2f;
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    npc.velocity.X = -2f;
            }

            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * tilePosX;
                if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5)
                    npc.velocity.Y = 2f;
                if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5)
                    npc.velocity.Y = -2f;
            }

            if (farAwayFromTarget)
            {
                if ((npc.velocity.X > 0f && probeTargetX > 0f) || (npc.velocity.X < 0f && probeTargetX < 0f))
                {
                    if (Math.Abs(npc.velocity.X) < (NPC.IsMechQueenUp ? 5f : 12f))
                        npc.velocity.X *= 1.05f;
                }
                else
                    npc.velocity.X *= 0.9f;
            }

            if (NPC.IsMechQueenUp && npc.ai[2] == 0f)
            {
                Vector2 center = npc.GetTargetData().Center;
                Vector2 v2 = center - npc.Center;
                if (v2.Length() < 120f)
                    npc.Center = center - v2.SafeNormalize(Vector2.UnitY) * 120;
            }

            if (targetDead)
            {
                npc.velocity.Y -= acceleration * 2f;
                if (npc.timeLeft > 10)
                    npc.timeLeft = 10;
            }

            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                npc.netUpdate = true;

            return false;
        }

        public static bool VanillaProbeAI(NPC npc, Mod mod)
        {
            if (npc.target < 0 || npc.target <= Main.maxPlayers || Main.player[npc.target].dead)
                npc.TargetClosest();

            NPCAimedTarget targetData = npc.GetTargetData();
            bool flag = false;
            if (targetData.Type == NPCTargetType.Player)
                flag = Main.player[npc.target].dead;

            float num = Main.zenithWorld ? 3f : Main.expertMode ? 7.2f : 6f;
            float num2 = Main.expertMode ? 0.06f : 0.05f;

            Vector2 vector = npc.Center;
            float num4 = targetData.Center.X;
            float num5 = targetData.Center.Y;
            num4 = (int)(num4 / 8f) * 8;
            num5 = (int)(num5 / 8f) * 8;
            vector.X = (int)(vector.X / 8f) * 8;
            vector.Y = (int)(vector.Y / 8f) * 8;
            num4 -= vector.X;
            num5 -= vector.Y;
            float num6 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
            float num7 = num6;
            bool flag2 = false;
            if (num6 > 600f)
                flag2 = true;

            if (num6 == 0f)
            {
                num4 = npc.velocity.X;
                num5 = npc.velocity.Y;
            }
            else
            {
                num6 = num / num6;
                num4 *= num6;
                num5 *= num6;
            }

            if (num7 > 100f)
            {
                npc.ai[0] += 1f;
                if (npc.ai[0] > 0f)
                    npc.velocity.Y += 0.023f;
                else
                    npc.velocity.Y -= 0.023f;

                if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                    npc.velocity.X += 0.023f;
                else
                    npc.velocity.X -= 0.023f;

                if (npc.ai[0] > 200f)
                    npc.ai[0] = -200f;
            }

            if (flag)
            {
                num4 = (float)npc.direction * num / 2f;
                num5 = (0f - num) / 2f;
            }

            if (npc.ai[3] != 0f)
            {
                if (NPC.IsMechQueenUp)
                {
                    NPC nPC = Main.npc[NPC.mechQueen];
                    Vector2 vector2 = new Vector2(26f * npc.ai[3], 0f);
                    int num9 = (int)npc.ai[2];
                    if (num9 < 0 || num9 >= Main.maxNPCs)
                    {
                        num9 = NPC.FindFirstNPC(NPCID.TheDestroyer);
                        npc.ai[2] = num9;
                        npc.netUpdate = true;
                    }

                    if (num9 > -1)
                    {
                        NPC nPC2 = Main.npc[num9];
                        if (!nPC2.active || nPC2.type != NPCID.TheDestroyer)
                        {
                            npc.dontTakeDamage = false;
                            if (npc.ai[3] > 0f)
                                npc.netUpdate = true;

                            npc.ai[3] = 0f;
                        }
                        else
                        {
                            Vector2 spinningpoint = nPC2.Center + vector2;
                            spinningpoint = spinningpoint.RotatedBy(nPC2.rotation, nPC2.Center);
                            npc.Center = spinningpoint;
                            npc.velocity = nPC.velocity;
                            npc.dontTakeDamage = true;
                        }
                    }
                    else
                    {
                        npc.dontTakeDamage = false;
                        if (npc.ai[3] > 0f)
                            npc.netUpdate = true;

                        npc.ai[3] = 0f;
                    }
                }
                else
                {
                    npc.dontTakeDamage = false;
                    if (npc.ai[3] > 0f)
                        npc.netUpdate = true;

                    npc.ai[3] = 0f;
                }
            }
            else
            {
                npc.dontTakeDamage = false;

                if (npc.velocity.X < num4)
                    npc.velocity.X += num2;
                else if (npc.velocity.X > num4)
                    npc.velocity.X -= num2;

                if (npc.velocity.Y < num5)
                    npc.velocity.Y += num2;
                else if (npc.velocity.Y > num5)
                    npc.velocity.Y -= num2;
            }

            npc.localAI[0] += 1f;
            if (npc.ai[3] != 0f)
                npc.localAI[0] += 2f;

            if (npc.justHit && !Main.masterMode)
                npc.localAI[0] = 0f;

            float num10 = 120f;
            if (NPC.IsMechQueenUp)
                num10 = 360f;

            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= num10)
            {
                npc.localAI[0] = 0f;
                if (targetData.Type != 0 && Collision.CanHit(npc, targetData))
                {
                    int type = ProjectileID.PinkLaser;
                    int damage = npc.GetProjectileDamage(type);

                    // Reduce mech boss projectile damage depending on the new ore progression changes
                    if (CalamityServerConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                    {
                        double firstMechMultiplier = Main.expertMode ? CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert : CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Classic;
                        double secondMechMultiplier = Main.expertMode ? CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert : CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Classic;
                        if (!NPC.downedMechBossAny)
                            damage = (int)(damage * firstMechMultiplier);
                        else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                            damage = (int)(damage * secondMechMultiplier);
                    }

                    Vector2 vector3 = new Vector2(num4, num5);
                    if (NPC.IsMechQueenUp)
                    {
                        Vector2 v = targetData.Center - npc.Center - targetData.Velocity * 20f;
                        float num12 = 8f;
                        vector3 = v.SafeNormalize(Vector2.UnitY) * num12;
                    }

                    Projectile.NewProjectile(npc.GetSource_FromAI(), vector + vector3.SafeNormalize(Vector2.UnitY) * 50f, vector3, type, damage, 0f, Main.myPlayer);
                }
            }

            int num13 = (int)npc.Center.X;
            int num14 = (int)npc.Center.Y;
            num13 /= 16;
            num14 /= 16;
            if (WorldGen.InWorld(num13, num14) && !WorldGen.SolidTile(num13, num14))
                Lighting.AddLight((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), 0.3f, 0.1f, 0.05f);

            if (num4 > 0f)
            {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2(num5, num4);
            }

            if (num4 < 0f)
            {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2(num5, num4) + MathHelper.Pi;
            }

            float num15 = 0.7f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * (0f - num15);
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    npc.velocity.X = 2f;

                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    npc.velocity.X = -2f;
            }

            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * (0f - num15);
                if (npc.velocity.Y > 0f && (double)npc.velocity.Y < 1.5)
                    npc.velocity.Y = 2f;

                if (npc.velocity.Y < 0f && (double)npc.velocity.Y > -1.5)
                    npc.velocity.Y = -2f;
            }

            if (flag2)
            {
                if ((npc.velocity.X > 0f && num4 > 0f) || (npc.velocity.X < 0f && num4 < 0f))
                {
                    int num27 = 12;
                    if (NPC.IsMechQueenUp)
                        num27 = 5;

                    if (Math.Abs(npc.velocity.X) < (float)num27)
                        npc.velocity.X *= 1.05f;
                }
                else
                    npc.velocity.X *= 0.9f;
            }

            if (NPC.IsMechQueenUp && npc.ai[2] == 0f)
            {
                Vector2 center = npc.GetTargetData().Center;
                Vector2 v2 = center - npc.Center;
                int num28 = 120;
                if (v2.Length() < (float)num28)
                    npc.Center = center - v2.SafeNormalize(Vector2.UnitY) * num28;
            }

            if (Main.IsItDay() || flag)
            {
                npc.velocity.Y -= num2 * 2f;
                npc.EncourageDespawn(10);
            }

            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                npc.netUpdate = true;

            return false;
        }
    }
}
