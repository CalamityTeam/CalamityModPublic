using System;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class EmpressofLightAI
    {
        public static bool BuffedEmpressofLightAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Difficulty bools.
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Rotation
            npc.rotation = npc.velocity.X * 0.005f;

            // Reset damage every frame.
            npc.damage = npc.defDamage;

            // Reset DR every frame.
            calamityGlobalNPC.DR = 0.15f;

            // Percent life remaining.
            float lifeRatio = npc.life / (float)npc.lifeMax;

            bool phase2 = npc.AI_120_HallowBoss_IsInPhase2();
            bool shouldBeInPhase2ButIsStillInPhase1 = lifeRatio <= 0.5f && !phase2;
            if (shouldBeInPhase2ButIsStillInPhase1)
                calamityGlobalNPC.DR = 0.99f;

            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = shouldBeInPhase2ButIsStillInPhase1 || npc.ai[0] == 6f;

            bool dayTimeEnrage = NPC.ShouldEmpressBeEnraged();
            if (npc.life == npc.lifeMax && dayTimeEnrage && !npc.AI_120_HallowBoss_IsGenuinelyEnraged())
                npc.ai[3] += 2f;

            npc.Calamity().CurrentlyEnraged = !bossRush && dayTimeEnrage;

            int projectileDamageMultiplier = dayTimeEnrage ? 2 : 1;

            Vector2 rainbowStreakDistance = new Vector2(-250f, -350f);
            Vector2 everlastingRainbowDistance = new Vector2(0f, -450f);
            Vector2 etherealLanceDistance = new Vector2(0f, -450f);
            Vector2 sunDanceDistance = new Vector2(-80f, -500f);

            float acceleration = death ? 0.55f : 0.48f;
            float velocity = death ? 14f : 12f;
            float movementDistanceGateValue = 40f;
            float despawnDistanceGateValue = 6400f;

            if (dayTimeEnrage)
            {
                float enragedDistanceMultiplier = 1.1f;
                rainbowStreakDistance *= enragedDistanceMultiplier;
                everlastingRainbowDistance *= enragedDistanceMultiplier;
                etherealLanceDistance *= enragedDistanceMultiplier;

                float enragedVelocityMultiplier = 1.2f;
                acceleration *= enragedVelocityMultiplier;
                velocity *= enragedVelocityMultiplier;
            }

            bool visible = true;
            bool takeDamage = true;
            float lessTimeSpentPerPhaseMultiplier = phase2 ? (death ? 0.375f : 0.5f) : (death ? 0.75f : 1f);
            if (Main.getGoodWorld)
                lessTimeSpentPerPhaseMultiplier *= 0.2f;

            float extraPhaseTime;
            Vector2 destination;

            switch ((int)npc.ai[0])
            {
                // Spawn animation.
                case 0:

                    // Avoid cheap bullshit.
                    npc.damage = 0;

                    if (npc.ai[1] == 0f)
                    {
                        npc.velocity = new Vector2(0f, 5f);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + new Vector2(0f, -80f), Vector2.Zero, ProjectileID.HallowBossDeathAurora, 0, 0f, Main.myPlayer);
                    }

                    if (npc.ai[1] == 10f)
                        SoundEngine.PlaySound(SoundID.Item161, npc.Center);

                    npc.velocity *= 0.95f;

                    if (npc.ai[1] > 10f && npc.ai[1] < 150f)
                    {
                        for (int m = 0; m < 2; m++)
                        {
                            float dustOpacity = MathHelper.Lerp(1.3f, 0.7f, npc.Opacity) * Utils.GetLerpValue(0f, 120f, npc.ai[1], clamped: true);
                            Color newColor2 = Main.hslToRgb(npc.ai[1] / 180f, 1f, 0.5f);
                            int rainbow = Dust.NewDust(npc.position, npc.width, npc.height, 267, 0f, 0f, 0, newColor2);
                            Main.dust[rainbow].position = npc.Center + Main.rand.NextVector2Circular(npc.width * 3f, npc.height * 3f) + new Vector2(0f, -150f);
                            Main.dust[rainbow].velocity *= Main.rand.NextFloat() * 0.8f;
                            Main.dust[rainbow].noGravity = true;
                            Main.dust[rainbow].fadeIn = 0.6f + Main.rand.NextFloat() * 0.7f * dustOpacity;
                            Main.dust[rainbow].velocity += Vector2.UnitY * 3f;
                            Main.dust[rainbow].scale = 0.35f;
                            if (rainbow != 6000)
                            {
                                Dust rainbowClone = Dust.CloneDust(rainbow);
                                rainbowClone.scale /= 2f;
                                rainbowClone.fadeIn *= 0.85f;
                                rainbowClone.color = new Color(255, 255, 255, 255);
                            }
                        }
                    }

                    npc.ai[1] += 1f;
                    visible = false;
                    takeDamage = false;
                    npc.Opacity = MathHelper.Clamp(npc.ai[1] / 180f, 0f, 1f);

                    if (npc.ai[1] >= 180f)
                    {
                        if (dayTimeEnrage && !npc.AI_120_HallowBoss_IsGenuinelyEnraged())
                            npc.ai[3] += 2f;

                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                        npc.TargetClosest();
                    }

                    break;

                // Phase switch.
                case 1:

                    // Avoid cheap bullshit.
                    npc.damage = 0;

                    float idleTimer = phase2 ? (death ? 10f : 15f) : (death ? 20f : 30f);
                    if (Main.getGoodWorld)
                        idleTimer *= 0.5f;
                    if (idleTimer < 10f)
                        idleTimer = 10f;

                    if (npc.ai[1] <= 10f)
                    {
                        if (npc.ai[1] == 0f)
                            npc.TargetClosest();

                        // Despawn.
                        NPCAimedTarget targetData4 = npc.GetTargetData();
                        if (targetData4.Invalid)
                        {
                            npc.ai[0] = 13f;
                            npc.ai[1] = 0f;
                            npc.ai[2] += 1f;
                            npc.velocity /= 4f;
                            npc.netUpdate = true;
                            break;
                        }

                        Vector2 center = targetData4.Center;
                        center += new Vector2(0f, -400f);
                        if (npc.Distance(center) > 200f)
                            center -= npc.DirectionTo(center) * 100f;

                        Vector2 targetDirection = center - npc.Center;
                        float lerpValue = Utils.GetLerpValue(100f, 600f, targetDirection.Length());
                        float targetDistance = targetDirection.Length();

                        float maxVelocity = death ? 24f : 21f;
                        if (targetDistance > maxVelocity)
                            targetDistance = maxVelocity;

                        npc.velocity = Vector2.Lerp(targetDirection.SafeNormalize(Vector2.Zero) * targetDistance, targetDirection / 6f, lerpValue);
                        npc.netUpdate = true;
                    }

                    npc.velocity *= 0.92f;
                    npc.ai[1] += 1f;
                    if (!(npc.ai[1] >= idleTimer))
                        break;

                    int attackPatternLength = (int)npc.ai[2];
                    int attackType = 2;
                    int attackIncrement = 0;

                    if (!phase2)
                    {
                        int phase1Attack1 = attackIncrement++;
                        int phase1Attack2 = attackIncrement++;
                        int phase1Attack3 = attackIncrement++;
                        int phase1Attack4 = attackIncrement++;
                        int phase1Attack5 = attackIncrement++;
                        int phase1Attack6 = attackIncrement++;
                        int phase1Attack7 = attackIncrement++;
                        int phase1Attack8 = attackIncrement++;
                        int phase1Attack9 = attackIncrement++;
                        int phase1Attack10 = attackIncrement++;

                        if (attackPatternLength % attackIncrement == phase1Attack1)
                            attackType = 2;

                        if (attackPatternLength % attackIncrement == phase1Attack2)
                            attackType = 6;

                        if (attackPatternLength % attackIncrement == phase1Attack3)
                            attackType = 8;

                        if (attackPatternLength % attackIncrement == phase1Attack4)
                        {
                            attackType = 4;

                            // Adjust the upcoming Ethereal Lance attack depending on what random variable is chosen here.
                            calamityGlobalNPC.newAI[0] = Main.rand.Next(2);

                            // Sync the Calamity AI variables.
                            npc.SyncExtraAI();
                        }

                        if (attackPatternLength % attackIncrement == phase1Attack5)
                            attackType = 5;

                        if (attackPatternLength % attackIncrement == phase1Attack6)
                            attackType = 8;

                        if (attackPatternLength % attackIncrement == phase1Attack7)
                            attackType = 2;

                        if (attackPatternLength % attackIncrement == phase1Attack8)
                        {
                            attackType = 4;

                            // Adjust the upcoming Ethereal Lance attack depending on what random variable is chosen here.
                            calamityGlobalNPC.newAI[0] = Main.rand.Next(2);

                            // Sync the Calamity AI variables.
                            npc.SyncExtraAI();
                        }

                        if (attackPatternLength % attackIncrement == phase1Attack9)
                            attackType = 8;

                        if (attackPatternLength % attackIncrement == phase1Attack10)
                            attackType = 5;

                        if (lifeRatio <= 0.5f)
                            attackType = 10;
                    }

                    if (phase2)
                    {
                        int phase2Attack1 = attackIncrement++;
                        int phase2Attack2 = attackIncrement++;
                        int phase2Attack3 = attackIncrement++;
                        int phase2Attack4 = attackIncrement++;
                        int phase2Attack5 = attackIncrement++;
                        int phase2Attack6 = attackIncrement++;
                        int phase2Attack7 = attackIncrement++;
                        int phase2Attack8 = attackIncrement++;
                        int phase2Attack9 = attackIncrement++;
                        int phase2Attack10 = attackIncrement++;

                        if (attackPatternLength % attackIncrement == phase2Attack1)
                        {
                            attackType = 7;

                            // Adjust the upcoming Ethereal Lance attack depending on what random variable is chosen here.
                            calamityGlobalNPC.newAI[2] = Main.rand.Next(2);

                            // Sync the Calamity AI variables.
                            npc.SyncExtraAI();
                        }

                        if (attackPatternLength % attackIncrement == phase2Attack2)
                            attackType = 2;

                        if (attackPatternLength % attackIncrement == phase2Attack3)
                            attackType = 8;

                        if (attackPatternLength % attackIncrement == phase2Attack5)
                            attackType = 5;

                        if (attackPatternLength % attackIncrement == phase2Attack6)
                            attackType = 2;

                        if (attackPatternLength % attackIncrement == phase2Attack7)
                            attackType = 6;

                        if (attackPatternLength % attackIncrement == phase2Attack7)
                            attackType = 6;

                        if (attackPatternLength % attackIncrement == phase2Attack8)
                        {
                            attackType = 4;

                            // Adjust the upcoming Ethereal Lance attack depending on what random variable is chosen here.
                            calamityGlobalNPC.newAI[0] = Main.rand.Next(2);

                            // Sync the Calamity AI variables.
                            npc.SyncExtraAI();
                        }

                        if (attackPatternLength % attackIncrement == phase2Attack9)
                            attackType = 8;

                        if (attackPatternLength % attackIncrement == phase2Attack4)
                            attackType = 11;

                        if (attackPatternLength % attackIncrement == phase2Attack10)
                            attackType = 12;
                    }

                    npc.TargetClosest();
                    NPCAimedTarget targetData5 = npc.GetTargetData();
                    bool despawnFlag = false;
                    if (npc.AI_120_HallowBoss_IsGenuinelyEnraged() && !bossRush)
                    {
                        if (!Main.dayTime)
                            despawnFlag = true;

                        if (Main.dayTime && Main.time >= 53400.0)
                            despawnFlag = true;
                    }

                    // Despawn.
                    if (targetData5.Invalid || npc.Distance(targetData5.Center) > despawnDistanceGateValue || despawnFlag)
                        attackType = 13;

                    // Set charge direction.
                    if (attackType == 8 && targetData5.Center.X > npc.Center.X)
                        attackType = 9;

                    if (attackType != 5 && attackType != 12)
                        npc.velocity = npc.DirectionFrom(targetData5.Center).SafeNormalize(Vector2.Zero).RotatedBy((float)Math.PI / 2f * (targetData5.Center.X > npc.Center.X).ToDirectionInt()) * 24f;

                    npc.ai[0] = attackType;
                    npc.ai[1] = 0f;
                    npc.ai[2] += Main.rand.Next(2) + 1f;
                    npc.netUpdate = true;

                    break;

                // Spawn homing Rainbow Streaks.
                case 2:

                    // Avoid cheap bullshit.
                    npc.damage = 0;

                    if (npc.ai[1] == 0f)
                        SoundEngine.PlaySound(SoundID.Item164, npc.Center);

                    Vector2 randomStreakOffset = new Vector2(-55f, -30f);
                    NPCAimedTarget targetData11 = npc.GetTargetData();
                    Vector2 targetCenter = targetData11.Invalid ? npc.Center : targetData11.Center;
                    if (npc.Distance(targetCenter + rainbowStreakDistance) > movementDistanceGateValue)
                        npc.SimpleFlyMovement(npc.DirectionTo(targetCenter + rainbowStreakDistance).SafeNormalize(Vector2.Zero) * velocity, acceleration);

                    if (npc.ai[1] < 60f)
                        AI_120_HallowBoss_DoMagicEffect(npc.Center + randomStreakOffset, 1, Utils.GetLerpValue(0f, 60f, npc.ai[1], clamped: true), npc);

                    int streakSpawnFrequency = CalamityWorld.LegendaryMode ? 1 : 2;
                    if ((int)npc.ai[1] % streakSpawnFrequency == 0 && npc.ai[1] < 60f)
                    {
                        int projectileType = ProjectileID.HallowBossRainbowStreak;
                        int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                        float ai3 = npc.ai[1] / 60f;
                        Vector2 rainbowStreakVelocity = new Vector2(0f, death ? -10f : -8f).RotatedBy((float)Math.PI / 2f * Main.rand.NextFloatDirection());
                        if (phase2)
                            rainbowStreakVelocity = new Vector2(0f, death ? -12f : -10f).RotatedBy((float)Math.PI * 2f * Main.rand.NextFloat());

                        if (dayTimeEnrage)
                            rainbowStreakVelocity *= MathHelper.Lerp(0.8f, 1.6f, ai3);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + randomStreakOffset, rainbowStreakVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, npc.target, ai3);
                            if (Main.rand.NextBool(60) && CalamityWorld.LegendaryMode)
                            {
                                Main.projectile[proj].extraUpdates += 1;
                                Main.projectile[proj].netUpdate = true;
                            }
                        }

                        // Spawn extra homing Rainbow Streaks per player.
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int multiplayerStreakSpawnFrequency = (int)(npc.ai[1] / streakSpawnFrequency);
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (npc.Boss_CanShootExtraAt(i, multiplayerStreakSpawnFrequency % 3, 3, 2400f))
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + randomStreakOffset, rainbowStreakVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, i, ai3);
                            }
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 36f : 72f) + 30f * lessTimeSpentPerPhaseMultiplier;
                    if (npc.ai[1] >= 60f + extraPhaseTime)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // This is never fucking used.
                /*case 3:
                    {
                        npc.ai[1] += 1f;
                        NPCAimedTarget targetData8 = npc.GetTargetData();
                        Vector2 targetCenter = targetData8.Invalid ? npc.Center : targetData8.Center;
                        if (npc.Distance(targetCenter + phase2AnimationDistance) > 0.5f)
                            npc.SimpleFlyMovement(npc.DirectionTo(targetCenter + phase2AnimationDistance).SafeNormalize(Vector2.Zero) * scaleFactor, 4f);

                        if ((int)npc.ai[1] % 180 == 0)
                        {
                            Vector2 auroraVector = new Vector2(0f, -100f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), targetData8.Center + auroraVector, Vector2.Zero, ProjectileID.HallowBossDeathAurora, magicAmt, 0f, Main.myPlayer);
                        }

                        if (npc.ai[1] >= 120f)
                        {
                            npc.ai[0] = 1f;
                            npc.ai[1] = 0f;
                            npc.netUpdate = true;
                        }

                        break;
                    }*/

                // Spawn Ethereal Lances around the target in seemingly random places (they will be made slower to make this easier to deal with).
                case 4:

                    // Avoid cheap bullshit.
                    npc.damage = 0;

                    if (npc.ai[1] == 0f)
                        SoundEngine.PlaySound(SoundID.Item162, npc.Center);

                    if (npc.ai[1] >= 6f && npc.ai[1] < 54f)
                    {
                        AI_120_HallowBoss_DoMagicEffect(npc.Center + new Vector2(-55f, -20f), 2, Utils.GetLerpValue(0f, 100f, npc.ai[1], clamped: true), npc);
                        AI_120_HallowBoss_DoMagicEffect(npc.Center + new Vector2(55f, -20f), 4, Utils.GetLerpValue(0f, 100f, npc.ai[1], clamped: true), npc);
                    }

                    NPCAimedTarget targetData10 = npc.GetTargetData();
                    targetCenter = targetData10.Invalid ? npc.Center : targetData10.Center;
                    if (npc.Distance(targetCenter + etherealLanceDistance) > movementDistanceGateValue)
                        npc.SimpleFlyMovement(npc.DirectionTo(targetCenter + etherealLanceDistance).SafeNormalize(Vector2.Zero) * velocity, acceleration);

                    int lanceRotation = death ? 8 : 12;
                    if (npc.ai[1] % (dayTimeEnrage ? 2f : 3f) == 0f && npc.ai[1] < 100f)
                    {
                        for (int n = 0; n < 1; n++)
                        {
                            int lanceFrequency = (int)(npc.ai[1] / (dayTimeEnrage ? 2f : 3f));
                            Vector2 lanceDirection = Vector2.UnitX.RotatedBy((float)Math.PI / (lanceRotation * 2) + lanceFrequency * ((float)Math.PI / lanceRotation));
                            if (calamityGlobalNPC.newAI[0] == 0f)
                                lanceDirection.X += (lanceDirection.X > 0f) ? 0.5f : -0.5f;

                            lanceDirection.Normalize();
                            float spawnDistance = 600f;

                            Vector2 playerCenter = targetData10.Center;
                            if (npc.Distance(playerCenter) > 2400f)
                                continue;

                            if (Vector2.Dot(targetData10.Velocity.SafeNormalize(Vector2.UnitY), lanceDirection) > 0f)
                                lanceDirection *= -1f;

                            Vector2 targetHoverPos = playerCenter + targetData10.Velocity * 90;
                            Vector2 spawnLocation = playerCenter + lanceDirection * spawnDistance - targetData10.Velocity * 30f;
                            if (spawnLocation.Distance(playerCenter) < spawnDistance)
                            {
                                Vector2 lanceSpawnDirection = playerCenter - spawnLocation;
                                if (lanceSpawnDirection == Vector2.Zero)
                                    lanceSpawnDirection = lanceDirection;

                                spawnLocation = playerCenter - Vector2.Normalize(lanceSpawnDirection) * spawnDistance;
                            }

                            int projectileType = ProjectileID.FairyQueenLance;
                            int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                            Vector2 v3 = targetHoverPos - spawnLocation;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromAI(), spawnLocation, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v3.ToRotation(), npc.ai[1] / 100f);

                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                continue;

                            // Spawn extra Ethereal Lances per player.
                            for (int j = 0; j < Main.maxPlayers; j++)
                            {
                                if (!npc.Boss_CanShootExtraAt(j, lanceFrequency % 3, 3, 2400f))
                                    continue;

                                Player extraPlayer = Main.player[j];
                                playerCenter = extraPlayer.Center;
                                if (Vector2.Dot(extraPlayer.velocity.SafeNormalize(Vector2.UnitY), lanceDirection) > 0f)
                                    lanceDirection *= -1f;

                                Vector2 extraPlayerSpawnLocation = playerCenter + extraPlayer.velocity * 90;
                                spawnLocation = playerCenter + lanceDirection * spawnDistance - extraPlayer.velocity * 30f;
                                if (spawnLocation.Distance(playerCenter) < spawnDistance)
                                {
                                    Vector2 extraPlayerSpawnDirection = playerCenter - spawnLocation;
                                    if (extraPlayerSpawnDirection == Vector2.Zero)
                                        extraPlayerSpawnDirection = lanceDirection;

                                    spawnLocation = playerCenter - Vector2.Normalize(extraPlayerSpawnDirection) * spawnDistance;
                                }

                                v3 = extraPlayerSpawnLocation - spawnLocation;
                                Projectile.NewProjectile(npc.GetSource_FromAI(), spawnLocation, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v3.ToRotation(), npc.ai[1] / 100f);
                            }
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 24f : 48f) + 20f * lessTimeSpentPerPhaseMultiplier;
                    if (npc.ai[1] >= 100f + extraPhaseTime)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        calamityGlobalNPC.newAI[0] = 0f;
                        npc.netUpdate = true;

                        // Sync the Calamity AI variables.
                        npc.SyncExtraAI();
                    }

                    break;

                // Spawn Everlasting Rainbow spiral.
                case 5:

                    // Avoid cheap bullshit.
                    npc.damage = 0;

                    if (npc.ai[1] == 0f)
                        SoundEngine.PlaySound(SoundID.Item163, npc.Center);

                    Vector2 magicSpawnOffset = new Vector2(55f, -30f);
                    Vector2 everlastingRainbowSpawn = npc.Center + magicSpawnOffset;
                    if (npc.ai[1] < 42f)
                        AI_120_HallowBoss_DoMagicEffect(npc.Center + magicSpawnOffset, 3, Utils.GetLerpValue(0f, 42f, npc.ai[1], clamped: true), npc);

                    NPCAimedTarget targetData7 = npc.GetTargetData();
                    targetCenter = targetData7.Invalid ? npc.Center : targetData7.Center;
                    if (npc.Distance(targetCenter + everlastingRainbowDistance) > movementDistanceGateValue)
                        npc.SimpleFlyMovement(npc.DirectionTo(targetCenter + everlastingRainbowDistance).SafeNormalize(Vector2.Zero) * velocity, acceleration);

                    if (npc.ai[1] % 42f == 0f && npc.ai[1] < 42f)
                    {
                        float projRotation = (float)Math.PI * 2f * Main.rand.NextFloat();
                        float totalProjectiles = CalamityWorld.LegendaryMode ? 30f : death ? (dayTimeEnrage ? 22f : 15f) : (dayTimeEnrage ? 18f : 13f);
                        int projIndex = 0;
                        bool inversePhase2SpreadPattern = Main.rand.NextBool();
                        for (float i = 0f; i < 1f; i += 1f / totalProjectiles)
                        {
                            int projectileType = ProjectileID.HallowBossLastingRainbow;
                            int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                            float projRotationMultiplier = i;
                            Vector2 spinningpoint = Vector2.UnitY.RotatedBy((float)Math.PI / 2f + (float)Math.PI * 2f * projRotationMultiplier + projRotation);

                            float initialVelocity = death ? 2f : 1.75f;
                            if (dayTimeEnrage && projIndex % 2 == 0)
                                initialVelocity *= 2f;
                            if (CalamityWorld.LegendaryMode)
                                initialVelocity *= 1.5f;

                            // Given that maxAddedVelocity = 2
                            // Before inverse: index 0 = 2, index 0.25 = 0, index 0.5 = 2, index 0.75 = 0, index 1 = 2
                            // After inverse: index 0 = 0, index 0.25 = 2, index 0.5 = 0, index 0.75 = 2, index 1 = 0
                            if (phase2)
                            {
                                float maxAddedVelocity = initialVelocity;
                                float addedVelocity = inversePhase2SpreadPattern ? Math.Abs(maxAddedVelocity - Math.Abs(MathHelper.Lerp(-maxAddedVelocity, maxAddedVelocity, Math.Abs(i - 0.5f) * 2f))) : Math.Abs(MathHelper.Lerp(-maxAddedVelocity, maxAddedVelocity, Math.Abs(i - 0.5f) * 2f));
                                initialVelocity += addedVelocity;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromAI(), everlastingRainbowSpawn + spinningpoint.RotatedBy(-MathHelper.PiOver2) * 30f, spinningpoint * initialVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, 0f, projRotationMultiplier);

                            projIndex++;
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 36f : 72f) + 30f * lessTimeSpentPerPhaseMultiplier;
                    if (npc.ai[1] >= 72f + extraPhaseTime)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Use Sun Dance.
                case 6:

                    // Avoid cheap bullshit.
                    npc.damage = 0;

                    // Increase durability.
                    calamityGlobalNPC.DR = shouldBeInPhase2ButIsStillInPhase1 ? 0.99f : (bossRush ? 0.99f : 0.575f);

                    int totalSunDances = phase2 ? 2 : 3;
                    float sunDanceGateValue = dayTimeEnrage ? 35f : death ? 40f : 50f;
                    float totalSunDancePhaseTime = totalSunDances * sunDanceGateValue;

                    Vector2 sunDanceHoverOffset = new Vector2(0f, -100f);
                    Vector2 position = npc.Center + sunDanceHoverOffset;

                    NPCAimedTarget targetData2 = npc.GetTargetData();
                    targetCenter = targetData2.Invalid ? npc.Center : targetData2.Center;
                    if (npc.Distance(targetCenter + sunDanceDistance) > movementDistanceGateValue)
                        npc.SimpleFlyMovement(npc.DirectionTo(targetCenter + sunDanceDistance).SafeNormalize(Vector2.Zero) * velocity * 0.3f, acceleration * 0.7f);

                    if (npc.ai[1] % sunDanceGateValue == 0f && npc.ai[1] < totalSunDancePhaseTime)
                    {
                        int projectileType = ProjectileID.FairyQueenSunDance;
                        int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                        int sunDanceExtension = (int)(npc.ai[1] / sunDanceGateValue);
                        int targetFloatDirection = (targetData2.Center.X > npc.Center.X) ? 1 : 0;
                        float projAmount = phase2 ? 8f : 6f;
                        float projRotation = 1f / projAmount;
                        for (float j = 0f; j < 1f; j += projRotation)
                        {
                            float projDirection = (j + projRotation * 0.5f + sunDanceExtension * projRotation * 0.5f) % 1f;
                            float ai = (float)Math.PI * 2f * (projDirection + targetFloatDirection);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromAI(), position, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, ai, npc.whoAmI);
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 110f : 150f) + 30f * lessTimeSpentPerPhaseMultiplier; // 112.5 is too little
                    if (npc.ai[1] >= totalSunDancePhaseTime + extraPhaseTime)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Spawn rows of Ethereal Lances.
                case 7:

                    // Avoid cheap bullshit.
                    npc.damage = 0;

                    // Expert attack or not.
                    bool expertAttack = calamityGlobalNPC.newAI[2] == 0f;

                    int numLanceWalls = expertAttack ? 6 : 4;
                    float lanceWallSpawnGateValue = expertAttack ? 36f : 54f;
                    if (dayTimeEnrage)
                        lanceWallSpawnGateValue -= expertAttack ? 4f : 6f;

                    float lanceWallPhaseTime = lanceWallSpawnGateValue * numLanceWalls;

                    NPCAimedTarget targetData9 = npc.GetTargetData();
                    destination = targetData9.Invalid ? npc.Center : targetData9.Center;
                    if (npc.Distance(destination + etherealLanceDistance) > movementDistanceGateValue)
                        npc.SimpleFlyMovement(npc.DirectionTo(destination + etherealLanceDistance).SafeNormalize(Vector2.Zero) * velocity * 0.4f, acceleration);

                    if ((int)npc.ai[1] % lanceWallSpawnGateValue == 0f && npc.ai[1] < lanceWallPhaseTime)
                    {
                        SoundEngine.PlaySound(SoundID.Item162, npc.Center);

                        float totalProjectiles = 15f;
                        float lanceSpacing = 175f;
                        float lanceWallSize = totalProjectiles * lanceSpacing;

                        Vector2 center3 = targetData9.Center;
                        if (npc.Distance(center3) <= 3200f)
                        {
                            Vector2 lanceWallStartingPosition = Vector2.Zero;
                            Vector2 lanceWallDirection = Vector2.UnitY;
                            float lanceWallConvergence = 0.4f;
                            float lanceWallSizeMult = 1.4f;
                            totalProjectiles += 5f;
                            lanceSpacing += 50f;
                            lanceWallSize *= 0.5f;

                            int randomLanceWallType;
                            do randomLanceWallType = Main.rand.Next(numLanceWalls);
                            while (randomLanceWallType == calamityGlobalNPC.newAI[0]);

                            // This is set so that Empress doesn't use the same wall type twice in a row.
                            calamityGlobalNPC.newAI[0] = randomLanceWallType;

                            // Keeps track of the total number of lance walls used.
                            calamityGlobalNPC.newAI[1] += 1f;

                            // Sync the Calamity AI variables.
                            npc.SyncExtraAI();

                            switch (randomLanceWallType)
                            {
                                case 0:
                                    center3 += new Vector2((0f - lanceWallSize) / 2f, 0f);
                                    lanceWallStartingPosition = new Vector2(0f, lanceWallSize);
                                    lanceWallDirection = Vector2.UnitX;
                                    break;

                                case 1:
                                    center3 += new Vector2(lanceWallSize / 2f, lanceSpacing / 2f);
                                    lanceWallStartingPosition = new Vector2(0f, lanceWallSize);
                                    lanceWallDirection = -Vector2.UnitX;
                                    break;

                                case 2:
                                    center3 += new Vector2(0f - lanceWallSize, 0f - lanceWallSize) * lanceWallConvergence;
                                    lanceWallStartingPosition = new Vector2(lanceWallSize * lanceWallSizeMult, 0f);
                                    lanceWallDirection = new Vector2(1f, 1f);
                                    break;

                                case 3:
                                    center3 += new Vector2(lanceWallSize * lanceWallConvergence + lanceSpacing / 2f, (0f - lanceWallSize) * lanceWallConvergence);
                                    lanceWallStartingPosition = new Vector2((0f - lanceWallSize) * lanceWallSizeMult, 0f);
                                    lanceWallDirection = new Vector2(-1f, 1f);
                                    break;

                                case 4:
                                    center3 += new Vector2(0f - lanceWallSize, lanceWallSize) * lanceWallConvergence;
                                    lanceWallStartingPosition = new Vector2(lanceWallSize * lanceWallSizeMult, 0f);
                                    lanceWallDirection = center3.DirectionTo(targetData9.Center);
                                    break;

                                case 5:
                                    center3 += new Vector2(lanceWallSize * lanceWallConvergence + lanceSpacing / 2f, lanceWallSize * lanceWallConvergence);
                                    lanceWallStartingPosition = new Vector2((0f - lanceWallSize) * lanceWallSizeMult, 0f);
                                    lanceWallDirection = center3.DirectionTo(targetData9.Center);
                                    break;
                            }

                            int projectileType = ProjectileID.FairyQueenLance;
                            int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                            for (float i = 0f; i <= 1f; i += 1f / totalProjectiles)
                            {
                                Vector2 spawnLocation = center3 + lanceWallStartingPosition * (i - 0.5f) * (expertAttack ? 1f : 2f);
                                Vector2 v2 = lanceWallDirection;
                                if (expertAttack)
                                {
                                    Vector2 lanceWallSpawnPredictiveness = targetData9.Velocity * 20f * i;
                                    Vector2 lanceWallSpawnLocation = spawnLocation.DirectionTo(targetData9.Center + lanceWallSpawnPredictiveness);
                                    v2 = Vector2.Lerp(lanceWallDirection, lanceWallSpawnLocation, 0.75f).SafeNormalize(Vector2.UnitY);
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), spawnLocation, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v2.ToRotation(), i);
                            }
                        }

                        // Chance to stop using the lance walls and switch to a different attack after 3 lance walls are used.
                        if (Main.rand.NextBool(5 - ((int)calamityGlobalNPC.newAI[1] - 2)) && calamityGlobalNPC.newAI[1] >= 2f)
                        {
                            npc.ai[1] = lanceWallPhaseTime;
                            npc.netUpdate = true;
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 24f : 48f) + 20f * lessTimeSpentPerPhaseMultiplier;
                    if (npc.ai[1] >= lanceWallPhaseTime + extraPhaseTime)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        calamityGlobalNPC.newAI[0] = 0f;
                        calamityGlobalNPC.newAI[1] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        npc.SyncExtraAI();
                        npc.netUpdate = true;
                    }

                    break;

                // Charge either left or right.
                case 8:
                case 9:

                    takeDamage = !(npc.ai[1] >= 6f) || !(npc.ai[1] <= 40f);

                    int chargeDirection = (npc.ai[0] != 8f) ? 1 : (-1);

                    AI_120_HallowBoss_DoMagicEffect(npc.Center, 5, Utils.GetLerpValue(40f, 90f, npc.ai[1], clamped: true), npc);

                    float chargeGateValue = 40f;
                    float chargeDuration = 50f;
                    if (npc.ai[1] <= chargeGateValue)
                    {
                        // Avoid cheap bullshit.
                        npc.damage = 0;

                        if (npc.ai[1] == 20f)
                            SoundEngine.PlaySound(SoundID.Item160, npc.Center);

                        NPCAimedTarget targetData3 = npc.GetTargetData();
                        destination = (targetData3.Invalid ? npc.Center : targetData3.Center) + new Vector2(chargeDirection * -800f, 0f);
                        npc.SimpleFlyMovement(npc.DirectionTo(destination).SafeNormalize(Vector2.Zero) * velocity, acceleration * 2f);

                        if (npc.ai[1] == chargeGateValue)
                            npc.velocity *= 0.3f;
                    }
                    else if (npc.ai[1] <= chargeGateValue + chargeDuration)
                    {
                        // Spawn Rainbow Streaks during charge.
                        if (npc.ai[1] == chargeGateValue + 1f)
                            SoundEngine.PlaySound(SoundID.Item164, npc.Center);

                        float rainbowStreakGateValue = 2f;
                        if ((npc.ai[1] - 1f) % rainbowStreakGateValue == 0f)
                        {
                            int projectileType = ProjectileID.HallowBossRainbowStreak;
                            int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                            float ai3 = (npc.ai[1] - chargeGateValue - 1f) / chargeDuration;
                            Vector2 rainbowStreakVelocity = new Vector2(0f, death ? -5f : -4f).RotatedBy((float)Math.PI / 2f * Main.rand.NextFloatDirection());
                            if (phase2)
                                rainbowStreakVelocity = new Vector2(0f, death ? -6f : -5f).RotatedBy((float)Math.PI * 2f * Main.rand.NextFloat());

                            rainbowStreakVelocity.X *= 2f;
                            if (!phase2)
                                rainbowStreakVelocity.Y *= 0.5f;

                            if (dayTimeEnrage)
                                rainbowStreakVelocity *= MathHelper.Lerp(0.8f, 1.6f, ai3);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, rainbowStreakVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, npc.target, ai3);
                                if (Main.rand.NextBool(30) && CalamityWorld.LegendaryMode)
                                {
                                    Main.projectile[proj].extraUpdates += 1;
                                    Main.projectile[proj].netUpdate = true;
                                }
                            }

                            // Spawn extra homing Rainbow Streaks per player.
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int multiplayerStreakSpawnFrequency = (int)((npc.ai[1] - chargeGateValue - 1f) / rainbowStreakGateValue);
                                for (int i = 0; i < Main.maxPlayers; i++)
                                {
                                    if (npc.Boss_CanShootExtraAt(i, multiplayerStreakSpawnFrequency % 3, 3, 2400f))
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, rainbowStreakVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, i, ai3);
                                }
                            }
                        }

                        npc.velocity = Vector2.Lerp(value2: new Vector2(chargeDirection * 70f, 0f), value1: npc.velocity, amount: 0.05f);

                        if (npc.ai[1] == chargeGateValue + chargeDuration)
                            npc.velocity *= 0.45f;

                        npc.damage = (int)(npc.defDamage * (dayTimeEnrage ? 3f : 1.5f));
                    }
                    else
                    {
                        // Avoid cheap bullshit.
                        npc.damage = 0;

                        npc.velocity *= 0.92f;
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 48f : 96f) * lessTimeSpentPerPhaseMultiplier;
                    if (npc.ai[1] >= 150f + extraPhaseTime)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Phase 2 animation.
                case 10:

                    // Avoid cheap bullshit.
                    npc.damage = 0;

                    if (npc.ai[1] == 0f)
                        SoundEngine.PlaySound(SoundID.Item161, npc.Center);

                    takeDamage = !(npc.ai[1] >= 30f) || !(npc.ai[1] <= 170f);

                    npc.velocity *= 0.95f;

                    if (npc.ai[1] == 90f)
                    {
                        if (npc.ai[3] == 0f)
                            npc.ai[3] = 1f;

                        if (npc.ai[3] == 2f)
                            npc.ai[3] = 3f;

                        npc.Center = npc.GetTargetData().Center + new Vector2(0f, -250f);
                        npc.netUpdate = true;
                    }

                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= 180f)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Spawn Ethereal Lances around the target in seemingly random places (they will be made slower to make this easier to deal with).
                case 11:

                    // Avoid cheap bullshit.
                    npc.damage = 0;

                    if (npc.ai[1] == 0f)
                        SoundEngine.PlaySound(SoundID.Item162, npc.Center);

                    if (npc.ai[1] >= 6f && npc.ai[1] < 54f)
                    {
                        AI_120_HallowBoss_DoMagicEffect(npc.Center + new Vector2(-55f, -20f), 2, Utils.GetLerpValue(0f, 100f, npc.ai[1], clamped: true), npc);
                        AI_120_HallowBoss_DoMagicEffect(npc.Center + new Vector2(55f, -20f), 4, Utils.GetLerpValue(0f, 100f, npc.ai[1], clamped: true), npc);
                    }

                    NPCAimedTarget targetData6 = npc.GetTargetData();
                    targetCenter = targetData6.Invalid ? npc.Center : targetData6.Center;
                    if (npc.Distance(targetCenter + etherealLanceDistance) > movementDistanceGateValue)
                        npc.SimpleFlyMovement(npc.DirectionTo(targetCenter + etherealLanceDistance).SafeNormalize(Vector2.Zero) * velocity, acceleration);

                    float etherealLanceGateValue = death ? 5f : 6f;
                    if (dayTimeEnrage)
                        etherealLanceGateValue -= 1f;

                    if (npc.ai[1] % etherealLanceGateValue == 0f && npc.ai[1] < 100f)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            Vector2 inverseTargetVel = -targetData6.Velocity;
                            inverseTargetVel.SafeNormalize(-Vector2.UnitY);
                            float spawnDistance = 100f + (k * 100f);

                            targetCenter = targetData6.Center;
                            if (npc.Distance(targetCenter) > 2400f)
                                continue;

                            Vector2 straightLanceSpawnPredict = targetCenter + targetData6.Velocity * 90;
                            Vector2 straightLanceSpawnDirection = targetCenter + inverseTargetVel * spawnDistance;
                            if (straightLanceSpawnDirection.Distance(targetCenter) < spawnDistance)
                            {
                                Vector2 straightLanceSpawnLocation = targetCenter - straightLanceSpawnDirection;
                                if (straightLanceSpawnLocation == Vector2.Zero)
                                    straightLanceSpawnLocation = inverseTargetVel;

                                straightLanceSpawnDirection = targetCenter - Vector2.Normalize(straightLanceSpawnLocation) * spawnDistance;
                            }

                            int projectileType = ProjectileID.FairyQueenLance;
                            int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                            Vector2 v = straightLanceSpawnPredict - straightLanceSpawnDirection;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromAI(), straightLanceSpawnDirection, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v.ToRotation(), npc.ai[1] / 100f);

                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                continue;

                            int multiplayerExtraStraightLances = (int)(npc.ai[1] / etherealLanceGateValue);
                            for (int l = 0; l < Main.maxPlayers; l++)
                            {
                                if (!npc.Boss_CanShootExtraAt(l, multiplayerExtraStraightLances % 3, 3, 2400f))
                                    continue;

                                Player player = Main.player[l];
                                inverseTargetVel = -player.velocity;
                                inverseTargetVel.SafeNormalize(-Vector2.UnitY);
                                targetCenter = player.Center;
                                Vector2 extraPlayerLancePredict = targetCenter + player.velocity * 90;
                                straightLanceSpawnDirection = targetCenter + inverseTargetVel * spawnDistance;
                                if (straightLanceSpawnDirection.Distance(targetCenter) < spawnDistance)
                                {
                                    Vector2 extraPlayerLanceSpawnLocation = targetCenter - straightLanceSpawnDirection;
                                    if (extraPlayerLanceSpawnLocation == Vector2.Zero)
                                        extraPlayerLanceSpawnLocation = inverseTargetVel;

                                    straightLanceSpawnDirection = targetCenter - Vector2.Normalize(extraPlayerLanceSpawnLocation) * spawnDistance;
                                }

                                v = extraPlayerLancePredict - straightLanceSpawnDirection;
                                Projectile.NewProjectile(npc.GetSource_FromAI(), straightLanceSpawnDirection, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v.ToRotation(), npc.ai[1] / 100f);
                            }
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 24f : 48f) * lessTimeSpentPerPhaseMultiplier;
                    if (npc.ai[1] >= 100f + extraPhaseTime)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Spawn homing Rainbow Streaks.
                case 12:

                    // Avoid cheap bullshit.
                    npc.damage = 0;

                    Vector2 projRandomOffset = new Vector2(-55f, -30f);

                    if (npc.ai[1] == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item165, npc.Center);
                        npc.velocity = new Vector2(0f, -12f);
                    }

                    npc.velocity *= 0.95f;

                    bool shouldSpawnStreaks = npc.ai[1] < 60f && npc.ai[1] >= 10f;
                    if (shouldSpawnStreaks)
                        AI_120_HallowBoss_DoMagicEffect(npc.Center + projRandomOffset, 1, Utils.GetLerpValue(0f, 60f, npc.ai[1], clamped: true), npc);

                    int stationaryStreakSpawnFrequency = 4;
                    if (dayTimeEnrage)
                        stationaryStreakSpawnFrequency -= 1;

                    float streakHomeTime = (npc.ai[1] - 10f) / 50f;
                    if ((int)npc.ai[1] % stationaryStreakSpawnFrequency == 0 && shouldSpawnStreaks)
                    {
                        int projectileType = ProjectileID.HallowBossRainbowStreak;
                        int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                        Vector2 vector = new Vector2(0f, death ? -24f : -22f).RotatedBy((float)Math.PI * 2f * streakHomeTime);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + projRandomOffset, vector, projectileType, projectileDamage, 0f, Main.myPlayer, npc.target, streakHomeTime);
                            if (Main.rand.NextBool(15) && CalamityWorld.LegendaryMode)
                            {
                                Main.projectile[proj].extraUpdates += 1;
                                Main.projectile[proj].netUpdate = true;
                            }
                        }

                        // Spawn extra homing Rainbow Streaks per player.
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int extraStationaryStreakSpawnFrequency = (int)(npc.ai[1] % stationaryStreakSpawnFrequency);
                            for (int j = 0; j < Main.maxPlayers; j++)
                            {
                                if (npc.Boss_CanShootExtraAt(j, extraStationaryStreakSpawnFrequency % 3, 3, 2400f))
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + projRandomOffset, vector, projectileType, projectileDamage, 0f, Main.myPlayer, j, streakHomeTime);
                            }
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 36f : 72f) + 30f * lessTimeSpentPerPhaseMultiplier;
                    if (npc.ai[1] >= 120f + extraPhaseTime)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }

                    break;

                // Despawn.
                case 13:

                    // Avoid cheap bullshit.
                    npc.damage = 0;

                    if (npc.ai[1] == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item165, npc.Center);
                        npc.velocity = new Vector2(0f, -7f);
                    }

                    npc.velocity *= 0.95f;

                    npc.TargetClosest();
                    NPCAimedTarget targetData = npc.GetTargetData();

                    visible = false;

                    bool trueDespawnFlag = false;
                    bool shouldDespawn = false;
                    if (!trueDespawnFlag)
                    {
                        if (npc.AI_120_HallowBoss_IsGenuinelyEnraged() && !bossRush)
                        {
                            if (!Main.dayTime)
                                shouldDespawn = true;

                            if (Main.dayTime && Main.time >= 53400.0)
                                shouldDespawn = true;
                        }

                        trueDespawnFlag = trueDespawnFlag || shouldDespawn;
                    }

                    if (!trueDespawnFlag)
                    {
                        bool hasNoTarget = targetData.Invalid || npc.Distance(targetData.Center) > despawnDistanceGateValue;
                        trueDespawnFlag = trueDespawnFlag || hasNoTarget;
                    }

                    npc.alpha = Utils.Clamp(npc.alpha + trueDespawnFlag.ToDirectionInt() * 5, 0, 255);
                    bool alphaExtreme = npc.alpha == 0 || npc.alpha == 255;

                    int despawnDustAmt = 5;
                    for (int i = 0; i < despawnDustAmt; i++)
                    {
                        float despawnDustOpacity = MathHelper.Lerp(1.3f, 0.7f, npc.Opacity);
                        Color newColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f);
                        int despawnRainbowDust = Dust.NewDust(npc.position - npc.Size * 0.5f, npc.width * 2, npc.height * 2, 267, 0f, 0f, 0, newColor);
                        Main.dust[despawnRainbowDust].position = npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height);
                        Main.dust[despawnRainbowDust].velocity *= Main.rand.NextFloat() * 0.8f;
                        Main.dust[despawnRainbowDust].noGravity = true;
                        Main.dust[despawnRainbowDust].scale = 0.9f + Main.rand.NextFloat() * 1.2f;
                        Main.dust[despawnRainbowDust].fadeIn = 0.4f + Main.rand.NextFloat() * 1.2f * despawnDustOpacity;
                        Main.dust[despawnRainbowDust].velocity += Vector2.UnitY * -2f;
                        Main.dust[despawnRainbowDust].scale = 0.35f;
                        if (despawnRainbowDust != 6000)
                        {
                            Dust dust = Dust.CloneDust(despawnRainbowDust);
                            dust.scale /= 2f;
                            dust.fadeIn *= 0.85f;
                            dust.color = new Color(255, 255, 255, 255);
                        }
                    }

                    npc.ai[1] += 1f;
                    if (!(npc.ai[1] >= 20f && alphaExtreme))
                        break;

                    if (npc.alpha == 255)
                    {
                        npc.active = false;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);

                        return false;
                    }

                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                    break;
            }

            npc.dontTakeDamage = !takeDamage;

            if (phase2)
                npc.defense = (int)(npc.defDefense * 1.2f);
            else
                npc.defense = npc.defDefense;

            if ((npc.localAI[0] += 1f) >= 44f)
                npc.localAI[0] = 0f;

            if (visible)
                npc.alpha = Utils.Clamp(npc.alpha - 5, 0, 255);

            Lighting.AddLight(npc.Center, Vector3.One * npc.Opacity);

            return false;
        }

        private static void AI_120_HallowBoss_DoMagicEffect(Vector2 spot, int effectType, float progress, NPC npc)
        {
            float magicDustSpawnArea = 4f;
            float magicDustColorMult = 1f;
            float fadeIn = 0f;
            float magicDustPosChange = 0.5f;
            int magicAmt = 2;
            int magicDustType = 267;
            switch (effectType)
            {
                case 1:
                    magicDustColorMult = 0.5f;
                    fadeIn = 2f;
                    magicDustPosChange = 0f;
                    break;
                case 2:
                case 4:
                    magicDustSpawnArea = 50f;
                    magicDustColorMult = 0.5f;
                    fadeIn = 0f;
                    magicDustPosChange = 0f;
                    magicAmt = 4;
                    break;
                case 3:
                    magicDustSpawnArea = 30f;
                    magicDustColorMult = 0.1f;
                    fadeIn = 2.5f;
                    magicDustPosChange = 0f;
                    break;
                case 5:
                    if (progress == 0f)
                    {
                        magicAmt = 0;
                    }
                    else
                    {
                        magicAmt = 5;
                        magicDustType = Main.rand.Next(86, 92);
                    }
                    if (progress >= 1f)
                        magicAmt = 0;
                    break;
            }

            for (int i = 0; i < magicAmt; i++)
            {
                Dust dust = Dust.NewDustPerfect(spot, magicDustType, Main.rand.NextVector2CircularEdge(magicDustSpawnArea, magicDustSpawnArea) * (Main.rand.NextFloat() * (1f - magicDustPosChange) + magicDustPosChange), 0, Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f), (Main.rand.NextFloat() * 2f + 2f) * magicDustColorMult);
                dust.fadeIn = fadeIn;
                dust.noGravity = true;
                switch (effectType)
                {
                    case 2:
                    case 4:
                        {
                            dust.velocity *= 0.005f;
                            dust.scale = 3f * Utils.GetLerpValue(0.7f, 0f, progress, clamped: true) * Utils.GetLerpValue(0f, 0.3f, progress, clamped: true);
                            dust.velocity = ((float)Math.PI * 2f * (i / 4f) + (float)Math.PI / 4f).ToRotationVector2() * 8f * Utils.GetLerpValue(1f, 0f, progress, clamped: true);
                            dust.velocity += npc.velocity * 0.3f;
                            float magicDustColorChange = 0f;
                            if (effectType == 4)
                                magicDustColorChange = 0.5f;

                            dust.color = Main.hslToRgb((i / 5f + magicDustColorChange + progress * 0.5f) % 1f, 1f, 0.5f);
                            dust.color.A /= 2;
                            dust.alpha = 127;
                            break;
                        }
                    case 5:
                        if (progress == 0f)
                        {
                            dust.customData = npc;
                            dust.scale = 1.5f;
                            dust.fadeIn = 0f;
                            dust.velocity = new Vector2(0f, -1f) + Main.rand.NextVector2Circular(1f, 1f);
                            dust.color = new Color(255, 255, 255, 80) * 0.3f;
                        }
                        else
                        {
                            dust.color = Main.hslToRgb(progress * 2f % 1f, 1f, 0.5f);
                            dust.alpha = 0;
                            dust.scale = 1f;
                            dust.fadeIn = 1.3f;
                            dust.velocity *= 3f;
                            dust.velocity.X *= 0.1f;
                            dust.velocity += npc.velocity * 1f;
                        }
                        break;
                }
            }
        }
    }
}
