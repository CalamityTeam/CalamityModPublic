using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Projectiles.Boss;
using CalamityMod.NPCs.TownNPCs;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class DeerclopsAI
    {
        public static bool BuffedDeerclopsAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            NPC.deerclopsBoss = npc.whoAmI;

            // Percent life remaining
            float lifeRatio = (float)npc.life / (float)npc.lifeMax;

            // Difficulty bools
            bool expertMode = Main.expertMode;

            // Projectile types and damage
            int rubble = ProjectileID.DeerclopsRangedProjectile;
            int rubbleDamage = npc.GetProjectileDamage(rubble);
            int iceSpike = ProjectileID.DeerclopsIceSpike;
            int iceSpikeDamage = npc.GetProjectileDamage(iceSpike);
            int shadowHand = ProjectileID.InsanityShadowHostile;
            int shadowHandDamage = npc.GetProjectileDamage(shadowHand);

            // Target data
            NPCAimedTarget targetData = npc.GetTargetData();

            // Movement variables
            bool haltMovement = false;
            bool goHome = false;

            // Damage resistance based on distance from target
            float increaseDRTriggerDistance = 450f;
            float maxDRIncreaseDistance = 900f;
            float distanceFromTarget = npc.Distance(targetData.Center);
            bool triggerDRIncrease = distanceFromTarget >= increaseDRTriggerDistance;
            float resistDamageAmount = MathHelper.Clamp((distanceFromTarget - increaseDRTriggerDistance) / (maxDRIncreaseDistance - increaseDRTriggerDistance), 0f, 1f);
            npc.localAI[3] = MathHelper.Lerp(0f, 30f, resistDamageAmount);
            float dustAndDRScalar = Utils.Remap(npc.localAI[3], 0f, 30f, 0f, 1f);
            calamityGlobalNPC.DR = MathHelper.Lerp(0.05f, 0.9f, dustAndDRScalar);
            if (dustAndDRScalar > 0f)
            {
                float invincibleDustAmount = Main.rand.NextFloat() * dustAndDRScalar * 3f;
                while (invincibleDustAmount > 0f)
                {
                    invincibleDustAmount -= 1f;
                    Dust.NewDustDirect(npc.position, npc.width, npc.height, 109, 0f, -3f, 0, default(Color), 1.4f).noGravity = true;
                }
            }

            // Spawn settings
            if (npc.homeTileX == -1 && npc.homeTileY == -1)
            {
                Point point = npc.Bottom.ToTileCoordinates();
                npc.homeTileX = point.X;
                npc.homeTileY = point.Y;
                npc.ai[2] = npc.homeTileX;
                npc.ai[3] = npc.homeTileY;
                npc.netUpdate = true;
                npc.timeLeft = 86400;
            }

            // Decrease time left based on actual world updates
            npc.timeLeft -= Main.worldEventUpdates;
            if (npc.timeLeft < 0)
                npc.timeLeft = 0;

            // Set home tile so Deerclops knows where to return to
            npc.homeTileX = (int)npc.ai[2];
            npc.homeTileY = (int)npc.ai[3];

            // Spawn Shadow Hands
            if (Main.netMode != NetmodeID.MultiplayerClient)
                SpawnPassiveShadowHands(npc, lifeRatio, shadowHand, shadowHandDamage);

            // AI states
            switch ((int)npc.ai[0])
            {
                // This case is never used
                case -1:

                    npc.localAI[3] = -10f;
                    
                    break;

                // Choose an attack to use
                case 0:

                    npc.TargetClosest();
                    targetData = npc.GetTargetData();
                    if (ShouldRunAway(npc, ref targetData, isChasing: true))
                    {
                        npc.ai[0] = 6f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] = 0f;
                        npc.netUpdate = true;
                        break;
                    }

                    npc.ai[1] += 1f;
                    Vector2 relativeCenter = npc.Bottom + new Vector2(0f, -32f);
                    Vector2 closestTargetPoint = targetData.Hitbox.ClosestPointInRect(relativeCenter);
                    Vector2 distanceFromTarget2 = closestTargetPoint - relativeCenter;
                    (closestTargetPoint - npc.Center).Length();
                    float distanceCheckMultiplier = 0.6f;
                    bool useFrontIceSpikeAttack = Math.Abs(distanceFromTarget2.X) >= Math.Abs(distanceFromTarget2.Y) * distanceCheckMultiplier || distanceFromTarget2.Length() < 48f;
                    bool useEitherIceSpikeAttack = distanceFromTarget2.Y <= (float)(100 + targetData.Height) && distanceFromTarget2.Y >= -200f;
                    if (Math.Abs(distanceFromTarget2.X) < 120f && useEitherIceSpikeAttack && npc.velocity.Y == 0f && npc.localAI[1] >= 2f)
                    {
                        npc.velocity.X = 0f;
                        npc.ai[0] = 4f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] = 0f;
                        calamityGlobalNPC.newAI[0] -= 1f;
                        npc.SyncExtraAI();
                        npc.netUpdate = true;
                        break;
                    }

                    if (Math.Abs(distanceFromTarget2.X) < 120f && useEitherIceSpikeAttack && npc.velocity.Y == 0f && useFrontIceSpikeAttack)
                    {
                        npc.velocity.X = 0f;
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] += 1f;
                        calamityGlobalNPC.newAI[0] -= 1f;
                        npc.SyncExtraAI();
                        npc.netUpdate = true;
                        break;
                    }

                    bool useRubbleAttack = npc.ai[1] >= 240f;
                    if (npc.velocity.Y == 0f && npc.velocity.X != 0f && useRubbleAttack)
                    {
                        npc.velocity.X = 0f;
                        npc.ai[0] = 2f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] = 0f;
                        calamityGlobalNPC.newAI[0] -= 1f;
                        npc.SyncExtraAI();
                        npc.netUpdate = true;
                        break;
                    }

                    bool useShadowHandAttack = npc.ai[1] >= 90f;
                    if (npc.velocity.Y == 0f && npc.velocity.X == 0f && useShadowHandAttack)
                    {
                        npc.velocity.X = 0f;
                        npc.ai[0] = 5f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] = 0f;
                        calamityGlobalNPC.newAI[0] -= 1f;
                        npc.SyncExtraAI();
                        npc.netUpdate = true;
                        break;
                    }

                    // This replaced the slow debuff infliction
                    bool useSecondShadowHandAttack = npc.ai[1] >= 120f;
                    if (npc.velocity.Y == 0f && useSecondShadowHandAttack && Math.Abs(distanceFromTarget2.X) > 100f && calamityGlobalNPC.newAI[0] <= 0f)
                    {
                        npc.velocity.X = 0f;
                        npc.ai[0] = 3f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] = 0f;
                        calamityGlobalNPC.newAI[0] = 5f;
                        npc.SyncExtraAI();
                        npc.netUpdate = true;
                    }

                    break;

                // Create spikes in front of Deerclops
                case 1:

                    npc.ai[1] += 1f;
                    haltMovement = true;
                    MakeSpikesForward(npc, 1, targetData, iceSpike, iceSpikeDamage);

                    if (npc.ai[1] >= 80f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Scoop up rubble
                case 2:

                    int scoopRubbleGateValue = 32;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] == (float)(scoopRubbleGateValue - 20))
                        SoundEngine.PlaySound(SoundID.DeerclopsScream, npc.Center);

                    if (npc.ai[1] == (float)scoopRubbleGateValue)
                        SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, npc.Center);

                    haltMovement = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] >= (float)scoopRubbleGateValue)
                    {
                        Point sourceTileCoords = npc.Top.ToTileCoordinates();
                        int numRubble = 20;
                        int distancedByThisManyTiles = 5;
                        float upBiasPerRubble = 200f;
                        sourceTileCoords.X += npc.direction * 3;
                        sourceTileCoords.Y -= 10;
                        int screenShakeGateValue = (int)npc.ai[1] - scoopRubbleGateValue;
                        if (screenShakeGateValue == 0)
                        {
                            PunchCameraModifier modifier4 = new PunchCameraModifier(npc.Center, new Vector2(0f, -1f), 20f, 6f, 30, 1000f, "Deerclops");
                            Main.instance.CameraModifiers.Add(modifier4);
                        }

                        int rubbleStart = screenShakeGateValue;
                        int rubbleLimit = rubbleStart + 1;
                        if (screenShakeGateValue % 1 != 0)
                            rubbleLimit = rubbleStart;

                        for (int rubbleIndex = rubbleStart; rubbleIndex < rubbleLimit && rubbleIndex < numRubble; rubbleIndex++)
                            ShootRubbleUp(npc, ref targetData, ref sourceTileCoords, numRubble, distancedByThisManyTiles, upBiasPerRubble, rubbleIndex, rubble, rubbleDamage);
                    }

                    if (npc.ai[1] >= 60f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Spawn shadow hands around the target with differing velocities
                case 3:

                    if (npc.ai[1] == 30f)
                        SoundEngine.PlaySound(SoundID.DeerclopsScream, npc.Center);

                    npc.ai[1] += 1f;
                    haltMovement = true;
                    if ((int)npc.ai[1] % 4 == 0 && npc.ai[1] >= 28f)
                    {
                        PunchCameraModifier modifier5 = new PunchCameraModifier(npc.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, "Deerclops");
                        Main.instance.CameraModifiers.Add(modifier5);
                    }

                    if (npc.ai[1] == 30f)
                    {
                        npc.TargetClosest();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int totalProjectiles = 9;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            float velocity = 9f;
                            Vector2 spinningPoint = new Vector2(0f, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 actualVelocity = spinningPoint.RotatedBy(radians * k);
                                float velocityMultiplier = 1f - k * 0.1f;
                                Projectile.NewProjectile(npc.GetSource_FromAI(), Main.player[npc.target].Center + Vector2.Normalize(actualVelocity) * 450f, actualVelocity * velocityMultiplier * -1f, shadowHand, shadowHandDamage, 0f, Main.myPlayer);
                            }
                        }
                    }

                    if (npc.ai[1] >= 60f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Spawn ice spikes on both sides
                case 4:

                    npc.ai[1] += 1f;
                    haltMovement = true;
                    npc.TargetClosest();
                    MakeSpikesBothSides(npc, 1, targetData, iceSpike, iceSpikeDamage);

                    if (npc.ai[1] >= 90f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Spawn shadow hands around the target with randomized AI
                case 5:

                    if (npc.ai[1] == 30f)
                        SoundEngine.PlaySound(SoundID.DeerclopsScream, npc.Center);

                    npc.ai[1] += 1f;
                    haltMovement = true;
                    if ((int)npc.ai[1] % 4 == 0 && npc.ai[1] >= 28f)
                    {
                        PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, "Deerclops");
                        Main.instance.CameraModifiers.Add(modifier);
                    }

                    if (npc.ai[1] == 30f)
                    {
                        npc.TargetClosest();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                RandomizeInsanityShadowFor(Main.player[npc.target], isHostile: true, out var spawnposition, out var spawnvelocity, out var ai, out var ai2);
                                Projectile.NewProjectile(npc.GetSource_FromAI(), spawnposition, spawnvelocity, shadowHand, shadowHandDamage, 0f, Main.myPlayer, ai, ai2);
                            }
                        }
                    }

                    if (npc.ai[1] >= 60f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Try to go home
                case 6:

                    npc.TargetClosest(faceTarget: false);
                    targetData = npc.GetTargetData();

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (!ShouldRunAway(npc, ref targetData, isChasing: false))
                        {
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            npc.localAI[1] = 0f;
                            npc.netUpdate = true;
                            break;
                        }

                        if (npc.timeLeft <= 0)
                        {
                            npc.ai[0] = 8f;
                            npc.ai[1] = 0f;
                            npc.localAI[1] = 0f;
                            npc.netUpdate = true;
                            break;
                        }
                    }

                    if (npc.direction != npc.oldDirection)
                        npc.netUpdate = true;

                    goHome = true;
                    npc.ai[1] += 1f;
                    Vector2 homeVector = new Vector2(npc.homeTileX * 16, npc.homeTileY * 16);
                    bool farBelowHome = npc.Top.Y > homeVector.Y + 1600f;
                    bool closeToHome = npc.Distance(homeVector) < 1020f;
                    npc.Distance(targetData.Center);
                    float stopMovingGateValue = npc.ai[1] % 600f;
                    if (closeToHome && stopMovingGateValue < 420f)
                        haltMovement = true;

                    bool returnHome = false;
                    int returnHomeDueToBelowHomeGateValue = 300;
                    if (farBelowHome && npc.ai[1] >= (float)returnHomeDueToBelowHomeGateValue)
                        returnHome = true;

                    int returnHomeDueToFarFromHomeGateValue = 1500;
                    if (!closeToHome && npc.ai[1] >= (float)returnHomeDueToFarFromHomeGateValue)
                        returnHome = true;

                    if (returnHome)
                    {
                        npc.ai[0] = 7f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Return home
                case 7:

                    if (npc.ai[1] == 30f)
                        SoundEngine.PlaySound(SoundID.DeerclopsScream, npc.Center);

                    npc.ai[1] += 1f;
                    haltMovement = true;
                    if ((int)npc.ai[1] % 4 == 0 && npc.ai[1] >= 28f)
                    {
                        PunchCameraModifier modifier3 = new PunchCameraModifier(npc.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, "Deerclops");
                        Main.instance.CameraModifiers.Add(modifier3);
                    }

                    if (npc.ai[1] == 40f)
                    {
                        npc.TargetClosest();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.netUpdate = true;
                            npc.Bottom = new Vector2(npc.homeTileX * 16, npc.homeTileY * 16);
                        }
                    }

                    if (npc.ai[1] >= 60f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Despawn
                case 8:

                    if (npc.ai[1] == 30f)
                        SoundEngine.PlaySound(SoundID.DeerclopsScream, npc.Center);

                    npc.ai[1] += 1f;
                    haltMovement = true;
                    if ((int)npc.ai[1] % 4 == 0 && npc.ai[1] >= 28f)
                    {
                        PunchCameraModifier modifier2 = new PunchCameraModifier(npc.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, "Deerclops");
                        Main.instance.CameraModifiers.Add(modifier2);
                    }

                    if (npc.ai[1] >= 40f)
                    {
                        npc.life = -1;
                        npc.HitEffect();
                        npc.active = false;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, -1f);

                        return false;
                    }

                    break;
            }

            // Movement
            Movement(npc, lifeRatio, haltMovement, goHome);

            return false;
        }

        private static bool ShouldRunAway(NPC npc, ref NPCAimedTarget targetData, bool isChasing)
        {
            // Run away if the target is far enough away from Deerclops' spawn point and not in the snow biome, or if the target is dead, or if the target is 2400 or more units away
            if (targetData.Type == NPCTargetType.Player)
            {
                Player player = Main.player[npc.target];
                bool zoneSnow = player.ZoneSnow;
                Vector2 other = new Vector2(npc.homeTileX * 16, npc.homeTileY * 16);
                float distanceToTriggerRunAway = 480f;
                zoneSnow |= player.Distance(other) <= distanceToTriggerRunAway;
                return (player.dead || (!isChasing && !zoneSnow)) | (npc.Distance(player.Center) >= 2400f);
            }

            if (targetData.Type == NPCTargetType.None)
                return true;

            return false;
        }

        private static void SpawnPassiveShadowHands(NPC npc, float lifeRatio, int shadowHand, int shadowHandDamage)
        {
            int shadowHandSpawnRate = (int)Utils.Remap(lifeRatio, 1f, 0f, 80f, 40f);
            npc.localAI[2] += 1f;
            int shadowHandTimer = (int)npc.localAI[2];
            if (shadowHandTimer % shadowHandSpawnRate != 0)
                return;

            int rotation = shadowHandTimer / shadowHandSpawnRate;
            if (shadowHandTimer / shadowHandSpawnRate >= 3)
                npc.localAI[2] = 0f;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (npc.Boss_CanShootExtraAt(i, rotation % 3, 3, 1200f, alwaysSkipMainTarget: false))
                {
                    // Normal shadow hand spawn behavior
                    // Modified to make the shadow hand spawns fair
                    RandomizeInsanityShadowFor(Main.player[i], isHostile: true, out var spawnPosition, out var spawnVelocity, out var ai, out var ai2);

                    // Spawn hands to cut the player off and force them back towards Deerclops
                    // This only happens if the player is triggering Deerclops' increased DR
                    float minShadowHandSpawnDistanceFromPlayer = 450f;
                    float playerDistanceFromDeerclops = Vector2.Distance(npc.Center, Main.player[i].Center);
                    if (playerDistanceFromDeerclops >= 450f)
                    {
                        spawnPosition = (Main.player[i].Center - npc.Center).SafeNormalize(Vector2.UnitY) * (playerDistanceFromDeerclops + minShadowHandSpawnDistanceFromPlayer);
                        spawnVelocity = (Main.player[i].Center - spawnPosition).SafeNormalize(Vector2.UnitY) * spawnVelocity.Length();
                    }

                    Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPosition, spawnVelocity, shadowHand, shadowHandDamage, 0f, Main.myPlayer, ai, ai2);
                }
            }
        }

        private static void RandomizeInsanityShadowFor(Entity targetEntity, bool isHostile, out Vector2 spawnPosition, out Vector2 spawnVelocity, out float ai0, out float ai1)
        {
            int spawnDirection = Main.rand.Next(2) * 2 - 1;
            int shadowHandType = Main.rand.Next(4);
            float spawnDistance = (isHostile ? 450f : 225f);
            float velocityDivisor = (isHostile ? 30 : 20);
            float velocityOffset = (isHostile ? 30 : 0);
            float randomRotation = Main.rand.NextFloatDirection() * (float)Math.PI * 0.125f;
            if (isHostile && targetEntity.velocity.X * (float)spawnDirection > 0f)
                spawnDirection *= -1;

            if (shadowHandType == 0 && isHostile)
                velocityDivisor += 10f;

            spawnPosition = targetEntity.Center + targetEntity.velocity * velocityOffset + new Vector2((float)spawnDirection * (0f - spawnDistance), 0f).RotatedBy(randomRotation);
            spawnVelocity = new Vector2((float)spawnDirection * spawnDistance / velocityDivisor, 0f).RotatedBy(randomRotation);
            ai0 = 0f;
            ai1 = 0f;

            if (shadowHandType == 1)
            {
                float rotation = (float)Math.PI * 2f * Main.rand.NextFloat();
                spawnPosition = targetEntity.Center - rotation.ToRotationVector2() * spawnDistance;
                ai0 = 180f;
                ai1 = rotation - (float)Math.PI / 2f;
                spawnVelocity = rotation.ToRotationVector2() * (isHostile ? 4 : 2);
            }
            else if (shadowHandType == 2)
            {
                float rotation = (float)Math.PI * 2f * Main.rand.NextFloat();
                spawnPosition = targetEntity.Center - rotation.ToRotationVector2() * spawnDistance;
                ai0 = 300f;
                ai1 = rotation;
                spawnVelocity = rotation.ToRotationVector2() * (isHostile ? 4 : 2);
            }
            else if (shadowHandType == 3)
            {
                float rotation = (float)Math.PI * 2f * Main.rand.NextFloat();
                float distance = (isHostile ? 150 : 75);
                float spawnVelocityRotation = (float)Math.PI / 2f / distance * Main.rand.NextFloatDirection();
                spawnPosition = targetEntity.Center + targetEntity.velocity * distance;
                if (Vector2.Distance(spawnPosition, targetEntity.Center) < spawnDistance)
                    spawnPosition = (spawnPosition - targetEntity.Center).SafeNormalize(Vector2.UnitY) * spawnDistance;

                Vector2 vector = rotation.ToRotationVector2() * (isHostile ? 8 : 3);
                for (int i = 0; (float)i < distance; i++)
                {
                    spawnPosition -= vector;
                    vector = vector.RotatedBy(0f - spawnVelocityRotation);
                }

                spawnVelocity = vector;
                ai0 = 390f;
                ai1 = spawnVelocityRotation;
            }
        }

        private static void ShootRubbleUp(NPC npc, ref NPCAimedTarget targetData, ref Point sourceTileCoords, int howMany, int distancedByThisManyTiles, float upBiasPerRubble, int whichOne, int rubble, int rubbleDamage)
        {
            // Loop to spawn rubble
            // The rubble attempts are used to offset the Y coordinates of the rubble spawns to make sure they can spawn in non-solid tiles
            int rubbleSpawnLocation = whichOne * distancedByThisManyTiles;
            int maxRubbleSpawnAttempts = 35;
            for (int rubbleSpawnAttempts = 0; rubbleSpawnAttempts < maxRubbleSpawnAttempts; rubbleSpawnAttempts++)
            {
                int posX = sourceTileCoords.X + rubbleSpawnLocation * npc.direction;
                int posY = sourceTileCoords.Y + rubbleSpawnAttempts;
                if (WorldGen.SolidTile(posX, posY))
                {
                    Vector2 vector = targetData.Center + new Vector2(rubbleSpawnLocation * npc.direction * 20, (0f - upBiasPerRubble) * (float)howMany + (float)rubbleSpawnLocation * upBiasPerRubble / (float)distancedByThisManyTiles);
                    Vector2 vector2 = new Vector2(posX * 16 + 8, posY * 16 + 8);
                    Vector2 rubbleVelocity = (vector - vector2).SafeNormalize(-Vector2.UnitY);
                    rubbleVelocity = new Vector2(0f, -1f).RotatedBy((float)(whichOne * npc.direction) * 0.7f * ((float)Math.PI / 4f / (float)howMany));
                    int ai1_FrameToUse = Main.rand.Next(Main.projFrames[rubble] * 4);
                    ai1_FrameToUse = 6 + Main.rand.Next(6);
                    float ai2_DelayBeforeGoingUp = whichOne * 20f;
                    Projectile.NewProjectile(npc.GetSource_FromAI(), new Vector2(posX * 16 + 8, posY * 16 - 8), rubbleVelocity * 0.01f, rubble, rubbleDamage, 0f, Main.myPlayer, 0f, ai1_FrameToUse, ai2_DelayBeforeGoingUp);
                    break;
                }
            }
        }

        private static void MakeSpikesForward(NPC npc, int AISLOT_PhaseCounter, NPCAimedTarget targetData, int iceSpike, int iceSpikeDamage)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int iceSpikeGateValue = 36;
            if (!(npc.ai[AISLOT_PhaseCounter] < (float)iceSpikeGateValue))
            {
                Point sourceTileCoords = npc.Bottom.ToTileCoordinates();
                int numIceSpikes = 20;
                int xOffsetMult = 1;
                sourceTileCoords.X += npc.direction * 3;
                int screenShakeGateValue = (int)npc.ai[AISLOT_PhaseCounter] - iceSpikeGateValue;
                if (screenShakeGateValue == 0)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, new Vector2(0f, 1f), 20f, 6f, 30, 1000f, "Deerclops");
                    Main.instance.CameraModifiers.Add(modifier);
                }

                int iceSpikeStart = screenShakeGateValue / 4 * 4;
                int iceSpikeLimit = iceSpikeStart + 4;
                if (screenShakeGateValue % 4 != 0)
                    iceSpikeLimit = iceSpikeStart;

                for (int i = iceSpikeStart; i < iceSpikeLimit && i < numIceSpikes; i++)
                {
                    int xOffset = i * xOffsetMult;
                    TryMakingSpike(npc, ref sourceTileCoords, npc.direction, numIceSpikes, i, xOffset, iceSpike, iceSpikeDamage);
                }
            }
        }

        private static void MakeSpikesBothSides(NPC npc, int AISLOT_PhaseCounter, NPCAimedTarget targetData, int iceSpike, int iceSpikeDamage)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int iceSpikeGateValue = 56;
            if (!(npc.ai[AISLOT_PhaseCounter] < (float)iceSpikeGateValue))
            {
                Point sourceTileCoords = npc.Bottom.ToTileCoordinates();
                int numIceSpikes = 15;
                int xOffsetMult = 1;
                int screenShakeGateValue = (int)npc.ai[AISLOT_PhaseCounter] - iceSpikeGateValue;
                if (screenShakeGateValue == 0)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, new Vector2(0f, 1f), 20f, 6f, 30, 1000f, "Deerclops");
                    Main.instance.CameraModifiers.Add(modifier);
                }

                int iceSpikeStart = screenShakeGateValue / 2 * 2;
                int iceSpikeLimit = iceSpikeStart + 2;
                if (screenShakeGateValue % 2 != 0)
                    iceSpikeLimit = iceSpikeStart;

                for (int iceSpikeIndex = iceSpikeStart; iceSpikeIndex >= 0 && iceSpikeIndex < iceSpikeLimit && iceSpikeIndex < numIceSpikes; iceSpikeIndex++)
                {
                    int xOffset = iceSpikeIndex * xOffsetMult;
                    TryMakingSpike(npc, ref sourceTileCoords, npc.direction, numIceSpikes, -iceSpikeIndex, xOffset, iceSpike, iceSpikeDamage);
                    TryMakingSpike(npc, ref sourceTileCoords, -npc.direction, numIceSpikes, -iceSpikeIndex, xOffset, iceSpike, iceSpikeDamage);
                }
            }
        }

        private static void TryMakingSpike(NPC npc, ref Point sourceTileCoords, int dir, int howMany, int whichOne, int xOffset, int iceSpike, int iceSpikeDamage)
        {
            int posX = sourceTileCoords.X + xOffset * dir;
            int posY = FindBestY(npc, ref sourceTileCoords, posX);
            if (WorldGen.ActiveAndWalkableTile(posX, posY))
            {
                Vector2 iceSpikeSpawnPos = new Vector2(posX * 16 + 8, posY * 16 - 8);
                Vector2 iceSpikeVelocity = new Vector2(0f, -1f).RotatedBy((float)(whichOne * dir) * 0.7f * ((float)Math.PI / 4f / (float)howMany));
                Projectile.NewProjectile(npc.GetSource_FromAI(), iceSpikeSpawnPos, iceSpikeVelocity, iceSpike, iceSpikeDamage, 0f, Main.myPlayer, 0f, 0.1f + Main.rand.NextFloat() * 0.1f + (float)xOffset * 1.1f / (float)howMany);
            }
        }

        private static int FindBestY(NPC npc, ref Point sourceTileCoords, int x)
        {
            int bestY = sourceTileCoords.Y;
            NPCAimedTarget targetData = npc.GetTargetData();
            if (!targetData.Invalid)
            {
                Rectangle hitbox = targetData.Hitbox;
                Vector2 vector = new Vector2(hitbox.Center.X, hitbox.Bottom);
                int y = (int)(vector.Y / 16f);
                int sign = Math.Sign(y - bestY);
                int y2 = y + sign * 15;
                int? potentialBestY = null;
                float yLimit = float.PositiveInfinity;
                for (int i = bestY; i != y2; i += sign)
                {
                    if (WorldGen.ActiveAndWalkableTile(x, i))
                    {
                        float newYLimit = new Point(x, i).ToWorldCoordinates().Distance(vector);
                        if (!potentialBestY.HasValue || !(newYLimit >= yLimit))
                        {
                            potentialBestY = i;
                            yLimit = newYLimit;
                        }
                    }
                }

                if (potentialBestY.HasValue)
                    bestY = potentialBestY.Value;
            }

            for (int j = 0; j < 20; j++)
            {
                if (bestY < 10)
                    break;

                if (!WorldGen.SolidTile(x, bestY))
                    break;

                bestY--;
            }

            for (int k = 0; k < 20; k++)
            {
                if (bestY > Main.maxTilesY - 10)
                    break;

                if (WorldGen.ActiveAndWalkableTile(x, bestY))
                    break;

                bestY++;
            }

            return bestY;
        }

        private static void Movement(NPC npc, float lifeRatio, bool haltMovement, bool goHome)
        {
            float moveSpeedMultiplier = 1f - lifeRatio;
            float moveSpeed = 3.5f + 1f * moveSpeedMultiplier;
            float moveSpeedDivisor = 4f;
            float yVelocityIncrease = -0.4f;
            float yVelocityMin = -8f;
            float yVelocityIncrease2 = 0.4f;
            Rectangle targetHitbox = npc.GetTargetData().Hitbox;

            if (goHome)
            {
                targetHitbox = new Rectangle(npc.homeTileX * 16, npc.homeTileY * 16, 16, 16);
                if (npc.Distance(targetHitbox.Center.ToVector2()) < 240f)
                    targetHitbox.X = (int)(npc.Center.X + (float)(160 * npc.direction));
            }

            float distanceFromTargetX = (float)targetHitbox.Center.X - npc.Center.X;
            float absoluteDistanceFromTargetX = Math.Abs(distanceFromTargetX);
            if (goHome && distanceFromTargetX != 0f)
                npc.direction = (npc.spriteDirection = Math.Sign(distanceFromTargetX));

            bool closeToTarget = absoluteDistanceFromTargetX < 80f;
            bool stopMoving = closeToTarget || haltMovement;
            if (npc.ai[0] == -1f)
            {
                distanceFromTargetX = 5f;
                moveSpeed = 5.35f;
                stopMoving = false;
            }

            if (stopMoving)
            {
                npc.velocity.X *= 0.9f;
                if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                    npc.velocity.X = 0f;
            }
            else
            {
                int moveDirection = Math.Sign(distanceFromTargetX);
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (float)moveDirection * moveSpeed, 1f / moveSpeedDivisor);
            }

            int npcCenterXOffset = 40;
            int npcCenterYOffset = 20;
            int gfxOffsetY = 0;
            Vector2 npcCenter = new Vector2(npc.Center.X - (float)(npcCenterXOffset / 2), npc.position.Y + (float)npc.height - (float)npcCenterYOffset + (float)gfxOffsetY);
            bool moveDown = npcCenter.X < (float)targetHitbox.X && npcCenter.X + (float)npc.width > (float)(targetHitbox.X + targetHitbox.Width);
            bool aboveTarget = npcCenter.Y + (float)npcCenterYOffset < (float)(targetHitbox.Y + targetHitbox.Height - 16);
            bool acceptTopSurfaces = npc.Bottom.Y >= (float)targetHitbox.Top;
            bool insideTiles = Collision.SolidCollision(npcCenter, npcCenterXOffset, npcCenterYOffset, acceptTopSurfaces);
            bool insideTiles2 = Collision.SolidCollision(npcCenter, npcCenterXOffset, npcCenterYOffset - 4, acceptTopSurfaces);
            bool moveUp = !Collision.SolidCollision(npcCenter + new Vector2(npcCenterXOffset * npc.direction, 0f), 16, 80, acceptTopSurfaces);
            float yVelocity = -8f;

            if (insideTiles || insideTiles2)
                npc.localAI[0] = 0f;

            if ((moveDown || closeToTarget) && aboveTarget)
            {
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + yVelocityIncrease2 * 2f, 0.001f, 16f);
            }
            else if (insideTiles && !insideTiles2)
            {
                npc.velocity.Y = 0f;
            }
            else if (insideTiles)
            {
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + yVelocityIncrease, yVelocityMin, 0f);
            }
            else if (npc.velocity.Y == 0f && moveUp)
            {
                npc.velocity.Y = yVelocity;
                npc.localAI[0] = 1f;
            }
            else
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + yVelocityIncrease2, yVelocity, 16f);
        }
    }
}
