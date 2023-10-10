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

            // Projectile damage
            int shadowHandDamage = 10;

            int num = 15;

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
            npc.localAI[3] = MathHelper.Lerp(0f, 1f, resistDamageAmount);
            calamityGlobalNPC.DR = MathHelper.Lerp(0.05f, 0.9f, npc.localAI[3]);
            if (npc.localAI[3] > 0f)
            {
                float invincibleDustAmount = Main.rand.NextFloat() * npc.localAI[3] * 3f;
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
                SpawnPassiveShadowHands(npc, lifeRatio, shadowHandDamage);

            // AI states
            switch ((int)npc.ai[0])
            {
                // This case is never used
                case -1:

                    npc.localAI[3] = 0f;
                    
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
                    Vector2 vector = npc.Bottom + new Vector2(0f, -32f);
                    Vector2 vector2 = targetData.Hitbox.ClosestPointInRect(vector);
                    Vector2 vector3 = vector2 - vector;
                    (vector2 - npc.Center).Length();
                    float num15 = 0.6f;
                    bool flag4 = Math.Abs(vector3.X) >= Math.Abs(vector3.Y) * num15 || vector3.Length() < 48f;
                    bool flag5 = vector3.Y <= (float)(100 + targetData.Height) && vector3.Y >= -200f;
                    if (Math.Abs(vector3.X) < 120f && flag5 && npc.velocity.Y == 0f && npc.localAI[1] >= 2f)
                    {
                        npc.velocity.X = 0f;
                        npc.ai[0] = 4f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] = 0f;
                        npc.netUpdate = true;
                        break;
                    }

                    if (Math.Abs(vector3.X) < 120f && flag5 && npc.velocity.Y == 0f && flag4)
                    {
                        npc.velocity.X = 0f;
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] += 1f;
                        npc.netUpdate = true;
                        break;
                    }

                    bool flag6 = npc.ai[1] >= 240f;
                    if (npc.velocity.Y == 0f && npc.velocity.X != 0f && flag6)
                    {
                        npc.velocity.X = 0f;
                        npc.ai[0] = 2f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] = 0f;
                        npc.netUpdate = true;
                        break;
                    }

                    bool flag7 = npc.ai[1] >= 90f;
                    if (npc.velocity.Y == 0f && npc.velocity.X == 0f && flag7)
                    {
                        npc.velocity.X = 0f;
                        npc.ai[0] = 5f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] = 0f;
                        npc.netUpdate = true;
                        break;
                    }

                    bool flag8 = npc.ai[1] >= 120f;
                    int num16 = 32;
                    bool flag9 = targetData.Type == NPCTargetType.Player && !Main.player[npc.target].buffImmune[num16] && Main.player[npc.target].FindBuffIndex(num16) == -1;
                    if (npc.velocity.Y == 0f && flag8 && flag9 && Math.Abs(vector3.X) > 100f)
                    {
                        npc.velocity.X = 0f;
                        npc.ai[0] = 3f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                case 1:

                    npc.ai[1] += 1f;
                    haltMovement = true;
                    MakeSpikesForward(npc, 1, targetData);

                    if (npc.ai[1] >= 80f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                case 2:

                    int num8 = 4;
                    int num9 = 8 * num8;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] == (float)(num9 - 20))
                        SoundEngine.PlaySound(SoundID.DeerclopsScream, npc.Center);

                    if (npc.ai[1] == (float)num9)
                        SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, npc.Center);

                    haltMovement = true;
                    if (Main.netMode != 1 && npc.ai[1] >= (float)num9)
                    {
                        Point sourceTileCoords = npc.Top.ToTileCoordinates();
                        int num10 = 20;
                        int distancedByThisManyTiles = 1;
                        float upBiasPerSpike = 200f;
                        sourceTileCoords.X += npc.direction * 3;
                        sourceTileCoords.Y -= 10;
                        int num11 = (int)npc.ai[1] - num9;
                        if (num11 == 0)
                        {
                            PunchCameraModifier modifier4 = new PunchCameraModifier(npc.Center, new Vector2(0f, -1f), 20f, 6f, 30, 1000f, "Deerclops");
                            Main.instance.CameraModifiers.Add(modifier4);
                        }

                        int num12 = 1;
                        int num13 = num11 / num12 * num12;
                        int num14 = num13 + num12;
                        if (num11 % num12 != 0)
                            num14 = num13;

                        for (int j = num13; j < num14 && j < num10; j++)
                            ShootRubbleUp(npc, ref targetData, ref sourceTileCoords, num10, distancedByThisManyTiles, upBiasPerSpike, j);
                    }

                    if (npc.ai[1] >= 60f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                case 3:

                    if (npc.ai[1] == 30f)
                        SoundEngine.PlaySound(SoundID.DeerclopsScream, npc.Center);

                    npc.ai[1] += 1f;
                    haltMovement = true;
                    if ((int)npc.ai[1] % 4 == 0 && npc.ai[1] >= 28f)
                    {
                        PunchCameraModifier modifier5 = new PunchCameraModifier(npc.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, "Deerclops");
                        Main.instance.CameraModifiers.Add(modifier5);
                        if (Main.netMode != 2)
                        {
                            Player player = Main.player[Main.myPlayer];
                            _ = Main.myPlayer;
                            int num17 = 32;
                            int timeToAdd = 720;
                            if (!player.dead && player.active && player.FindBuffIndex(num17) == -1 && (player.Center - npc.Center).Length() < 800f && !player.creativeGodMode)
                                player.AddBuff(num17, timeToAdd);
                        }
                    }

                    if (npc.ai[1] == 30f)
                        npc.TargetClosest();

                    if (npc.ai[1] >= 60f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                case 4:

                    npc.ai[1] += 1f;
                    haltMovement = true;
                    npc.TargetClosest();
                    MakeSpikesBothSides(npc, 1, targetData);

                    if (npc.ai[1] >= 90f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

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
                        if (Main.netMode != 1)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                RandomizeInsanityShadowFor(Main.player[npc.target], isHostile: true, out var spawnposition, out var spawnvelocity, out var ai, out var ai2);
                                Projectile.NewProjectile(npc.GetSource_FromAI(), spawnposition, spawnvelocity, 965, num, 0f, Main.myPlayer, ai, ai2);
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

                case 6:

                    npc.TargetClosest(faceTarget: false);
                    targetData = npc.GetTargetData();

                    if (Main.netMode != 1)
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
                    Vector2 other = new Vector2(npc.homeTileX * 16, npc.homeTileY * 16);
                    bool flag2 = npc.Top.Y > other.Y + 1600f;
                    bool num4 = npc.Distance(other) < 1020f;
                    npc.Distance(targetData.Center);
                    float num5 = npc.ai[1] % 600f;
                    if (num4 && num5 < 420f)
                        haltMovement = true;

                    bool flag3 = false;
                    int num6 = 300;
                    if (flag2 && npc.ai[1] >= (float)num6)
                        flag3 = true;

                    int num7 = 1500;
                    if (!num4 && npc.ai[1] >= (float)num7)
                        flag3 = true;

                    if (flag3)
                    {
                        npc.ai[0] = 7f;
                        npc.ai[1] = 0f;
                        npc.localAI[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

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
                        if (Main.netMode != 1)
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
                        if (Main.netMode != 1)
                            NetMessage.SendData(28, -1, -1, null, npc.whoAmI, -1f);

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

        private static void SpawnPassiveShadowHands(NPC npc, float lifeRatio, int shadowHandDamage)
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

                    Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPosition, spawnVelocity, ProjectileID.InsanityShadowHostile, shadowHandDamage, 0f, Main.myPlayer, ai, ai2);
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

        private static void ShootRubbleUp(NPC npc, ref NPCAimedTarget targetData, ref Point sourceTileCoords, int howMany, int distancedByThisManyTiles, float upBiasPerSpike, int whichOne)
        {
            int num = 18;
            int num2 = whichOne * distancedByThisManyTiles;
            for (int i = 0; i < 35; i++)
            {
                int num3 = sourceTileCoords.X + num2 * npc.direction;
                int num4 = sourceTileCoords.Y + i;
                if (WorldGen.SolidTile(num3, num4))
                {
                    Vector2 vector = targetData.Center + new Vector2(num2 * npc.direction * 20, (0f - upBiasPerSpike) * (float)howMany + (float)num2 * upBiasPerSpike / (float)distancedByThisManyTiles);
                    Vector2 vector2 = new Vector2(num3 * 16 + 8, num4 * 16 + 8);
                    Vector2 vector3 = (vector - vector2).SafeNormalize(-Vector2.UnitY);
                    vector3 = new Vector2(0f, -1f).RotatedBy((float)(whichOne * npc.direction) * 0.7f * ((float)Math.PI / 4f / (float)howMany));
                    int num5 = Main.rand.Next(Main.projFrames[962] * 4);
                    num5 = 6 + Main.rand.Next(6);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), new Vector2(num3 * 16 + 8, num4 * 16 - 8), vector3 * (8f + Main.rand.NextFloat() * 8f), 962, num, 0f, Main.myPlayer, 0f, num5);
                    break;
                }
            }
        }

        private static void MakeSpikesForward(NPC npc, int AISLOT_PhaseCounter, NPCAimedTarget targetData)
        {
            int num = 9;
            int num2 = 4;
            if (Main.netMode == 1)
                return;

            int num3 = num * num2;
            if (!(npc.ai[AISLOT_PhaseCounter] < (float)num3))
            {
                Point sourceTileCoords = npc.Bottom.ToTileCoordinates();
                int num4 = 20;
                int num5 = 1;
                sourceTileCoords.X += npc.direction * 3;
                int num6 = (int)npc.ai[AISLOT_PhaseCounter] - num3;
                if (num6 == 0)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, new Vector2(0f, 1f), 20f, 6f, 30, 1000f, "Deerclops");
                    Main.instance.CameraModifiers.Add(modifier);
                }

                int num7 = 4;
                int num8 = num6 / num7 * num7;
                int num9 = num8 + num7;
                if (num6 % num7 != 0)
                    num9 = num8;

                for (int i = num8; i < num9 && i < num4; i++)
                {
                    int xOffset = i * num5;
                    TryMakingSpike(npc, ref sourceTileCoords, npc.direction, num4, i, xOffset);
                }
            }
        }

        private static void MakeSpikesBothSides(NPC npc, int AISLOT_PhaseCounter, NPCAimedTarget targetData)
        {
            if (Main.netMode == 1)
                return;

            int num = 56;
            if (!(npc.ai[AISLOT_PhaseCounter] < (float)num))
            {
                Point sourceTileCoords = npc.Bottom.ToTileCoordinates();
                int num2 = 15;
                int num3 = 1;
                int num4 = (int)npc.ai[AISLOT_PhaseCounter] - num;
                if (num4 == 0)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(npc.Center, new Vector2(0f, 1f), 20f, 6f, 30, 1000f, "Deerclops");
                    Main.instance.CameraModifiers.Add(modifier);
                }

                int num5 = 2;
                int num6 = num4 / num5 * num5;
                int num7 = num6 + num5;
                if (num4 % num5 != 0)
                    num7 = num6;

                for (int i = num6; i >= 0 && i < num7 && i < num2; i++)
                {
                    int xOffset = i * num3;
                    TryMakingSpike(npc, ref sourceTileCoords, npc.direction, num2, -i, xOffset);
                    TryMakingSpike(npc, ref sourceTileCoords, -npc.direction, num2, -i, xOffset);
                }
            }
        }

        private static void FindSpotToSpawnSpike(NPC npc, int howMany, int whichOne, ref int x, ref int y)
        {
            if (WorldGen.ActiveAndWalkableTile(x, y))
                return;

            Rectangle rectangle = npc.targetRect;
            int num = rectangle.Center.X / 16;
            int num2 = (rectangle.Bottom - 16) / 16;
            int num3 = ((num2 - y > 0) ? 1 : (-1));
            int num4 = y;
            for (int i = 1; i <= 10; i++)
            {
                int num5 = y + num3 * i;
                if (num5 >= 20 && num5 <= Main.maxTilesY - 20 && WorldGen.ActiveAndWalkableTile(x, num5))
                    num4 = num5;
            }

            if (num4 != y)
            {
                y = num4;
                return;
            }

            y = (int)MathHelper.Lerp(num2, y, (float)Math.Abs(num - x) * 0.1f);
            for (int j = 0; j < 4; j++)
            {
                int num6 = y + j;
                if (num6 >= 20 && num6 <= Main.maxTilesY - 20 && WorldGen.ActiveAndWalkableTile(x, num6))
                {
                    y = num6;
                    break;
                }
            }
        }

        private static void TryMakingSpike(NPC npc, ref Point sourceTileCoords, int dir, int howMany, int whichOne, int xOffset)
        {
            int num = 13;
            int num2 = sourceTileCoords.X + xOffset * dir;
            int num3 = FindBestY(npc, ref sourceTileCoords, num2);
            if (WorldGen.ActiveAndWalkableTile(num2, num3))
            {
                Vector2 vector = new Vector2(num2 * 16 + 8, num3 * 16 - 8);
                Vector2 vector2 = new Vector2(0f, -1f).RotatedBy((float)(whichOne * dir) * 0.7f * ((float)Math.PI / 4f / (float)howMany));
                Projectile.NewProjectile(npc.GetSource_FromAI(), vector, vector2, 961, num, 0f, Main.myPlayer, 0f, 0.1f + Main.rand.NextFloat() * 0.1f + (float)xOffset * 1.1f / (float)howMany);
            }
        }

        private static int FindBestY(NPC npc, ref Point sourceTileCoords, int x)
        {
            int num = sourceTileCoords.Y;
            NPCAimedTarget targetData = npc.GetTargetData();
            if (!targetData.Invalid)
            {
                Rectangle hitbox = targetData.Hitbox;
                Vector2 vector = new Vector2(hitbox.Center.X, hitbox.Bottom);
                int num2 = (int)(vector.Y / 16f);
                int num3 = Math.Sign(num2 - num);
                int num4 = num2 + num3 * 15;
                int? num5 = null;
                float num6 = float.PositiveInfinity;
                for (int i = num; i != num4; i += num3)
                {
                    if (WorldGen.ActiveAndWalkableTile(x, i))
                    {
                        float num7 = new Point(x, i).ToWorldCoordinates().Distance(vector);
                        if (!num5.HasValue || !(num7 >= num6))
                        {
                            num5 = i;
                            num6 = num7;
                        }
                    }
                }

                if (num5.HasValue)
                    num = num5.Value;
            }

            for (int j = 0; j < 20; j++)
            {
                if (num < 10)
                    break;

                if (!WorldGen.SolidTile(x, num))
                    break;

                num--;
            }

            for (int k = 0; k < 20; k++)
            {
                if (num > Main.maxTilesY - 10)
                    break;

                if (WorldGen.ActiveAndWalkableTile(x, num))
                    break;

                num++;
            }

            return num;
        }

        private static void Movement(NPC npc, float lifeRatio, bool haltMovement, bool goHome)
        {
            float num = lifeRatio;
            float num2 = 1f - num;
            float num3 = 3.5f + 1f * num2;
            float num4 = 4f;
            float num5 = -0.4f;
            float min = -8f;
            float num6 = 0.4f;
            Rectangle rectangle = npc.GetTargetData().Hitbox;
            if (goHome)
            {
                rectangle = new Rectangle(npc.homeTileX * 16, npc.homeTileY * 16, 16, 16);
                if (npc.Distance(rectangle.Center.ToVector2()) < 240f)
                    rectangle.X = (int)(npc.Center.X + (float)(160 * npc.direction));
            }

            float num7 = (float)rectangle.Center.X - npc.Center.X;
            float num8 = Math.Abs(num7);
            if (goHome && num7 != 0f)
                npc.direction = (npc.spriteDirection = Math.Sign(num7));

            bool flag = num8 < 80f;
            bool flag2 = flag || haltMovement;
            if (npc.ai[0] == -1f)
            {
                num7 = 5f;
                num3 = 5.35f;
                flag2 = false;
            }

            if (flag2)
            {
                npc.velocity.X *= 0.9f;
                if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                    npc.velocity.X = 0f;
            }
            else
            {
                int num9 = Math.Sign(num7);
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (float)num9 * num3, 1f / num4);
            }

            int num10 = 40;
            int num11 = 20;
            int num12 = 0;
            Vector2 vector = new Vector2(npc.Center.X - (float)(num10 / 2), npc.position.Y + (float)npc.height - (float)num11 + (float)num12);
            bool num13 = vector.X < (float)rectangle.X && vector.X + (float)npc.width > (float)(rectangle.X + rectangle.Width);
            bool flag3 = vector.Y + (float)num11 < (float)(rectangle.Y + rectangle.Height - 16);
            bool acceptTopSurfaces = npc.Bottom.Y >= (float)rectangle.Top;
            bool flag4 = Collision.SolidCollision(vector, num10, num11, acceptTopSurfaces);
            bool flag5 = Collision.SolidCollision(vector, num10, num11 - 4, acceptTopSurfaces);
            bool flag6 = !Collision.SolidCollision(vector + new Vector2(num10 * npc.direction, 0f), 16, 80, acceptTopSurfaces);
            float num14 = 8f;
            if (flag4 || flag5)
                npc.localAI[0] = 0f;

            if ((num13 || flag) && flag3)
            {
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + num6 * 2f, 0.001f, 16f);
            }
            else if (flag4 && !flag5)
            {
                npc.velocity.Y = 0f;
            }
            else if (flag4)
            {
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + num5, min, 0f);
            }
            else if (npc.velocity.Y == 0f && flag6)
            {
                npc.velocity.Y = 0f - num14;
                npc.localAI[0] = 1f;
            }
            else
            {
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + num6, 0f - num14, 16f);
            }
        }
    }
}
