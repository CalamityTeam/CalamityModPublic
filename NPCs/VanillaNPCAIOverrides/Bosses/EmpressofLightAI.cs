using System;
using CalamityMod.Events;
using CalamityMod.World;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    public static class EmpressOfLightAIUtils
    {
        public static int CalculateDamageForEnrage(this int damage) => NPC.ShouldEmpressBeEnraged() ? 9999 : damage;
    }

    public class EmpressofLightAI : VanillaAIOverride
    {
        // NOTE: This is applied to all difficulties, in CalamityPlayerHitHurt
        public static float EverlastingRainbowTrailDamageMult = 0.75f;

        // Vanilla values
        public static float DashDamageMult = 1.5f; // 165
        public static int PrismaticBoltDamage = 30; // 120
        public static int EverlastingRainbowDamage = 30; // 120
        public static int EtherealLanceDamage = 30; // 120
        public static int SunDanceDamage = 35; // 140
        public static int Phase2PrismaticBoltDamage = 35; // 140
        public static int Phase2EverlastingRainbowDamage = 35; // 140
        public static int Phase2EtherealLanceDamage = 35; // 140
        public static int Phase2SunDanceDamage = 40; // 160

        // Vanilla sets her defDamage to 184 (80 * 2 * 1.15) in Expert Mode
        // However, some weird contraption in her Expert AI that knocks her base contact damage to the intended value of 110
        // This hasn't been found anywhere so our solution for now is avoid using defDamage altogether
        public static int ContactDamageCorrection = Main.masterMode ? 248 : 110;

        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Difficulty bools.
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Rotation
            NPC.rotation = NPC.velocity.X * 0.005f;

            // Reset DR every frame.
            calamityGlobalNPC.DR = 0.15f;

            // Percent life remaining.
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            float phase2LifeRatio = death ? 0.7f : 0.6f;
            float phase3LifeRatio = death ? 0.3f : 0.15f;
            bool phase2 = NPC.AI_120_HallowBoss_IsInPhase2();
            bool phase3 = lifeRatio <= phase3LifeRatio;

            int boltDamage = (phase2 ? Phase2PrismaticBoltDamage : PrismaticBoltDamage).CalculateDamageForEnrage();
            int rainbowDamage = (phase2 ? Phase2EverlastingRainbowDamage : EverlastingRainbowDamage).CalculateDamageForEnrage();
            int lanceDamage = (phase2 ? Phase2EtherealLanceDamage : EtherealLanceDamage).CalculateDamageForEnrage();
            int sunDanceDamage = (phase2 ? Phase2SunDanceDamage : SunDanceDamage).CalculateDamageForEnrage();

            bool shouldBeInPhase2ButIsStillInPhase1 = lifeRatio <= phase2LifeRatio && !phase2;
            if (shouldBeInPhase2ButIsStillInPhase1)
                calamityGlobalNPC.DR = 0.99f;

            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = shouldBeInPhase2ButIsStillInPhase1 || NPC.ai[0] == 6f;

            bool dayTimeEnrage = NPC.ShouldEmpressBeEnraged();
            if (NPC.life == NPC.lifeMax && dayTimeEnrage && !NPC.AI_120_HallowBoss_IsGenuinelyEnraged())
                NPC.ai[3] += 2f;

            NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && dayTimeEnrage;

            Vector2 rainbowStreakDistance = new Vector2(-150f, -250f);
            Vector2 everlastingRainbowDistance = new Vector2(0f, -350f);
            Vector2 etherealLanceDistance = new Vector2(0f, -350f);
            Vector2 sunDanceDistance = new Vector2(-80f, -500f);

            float acceleration = death ? 0.66f : 0.6f;
            float velocity = death ? 16.5f : 15f;
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

            // Variables for dust visuals on spawn and in phase 3
            float playSpawnSoundTime = 10f;
            float stopSpawningDustTime = 150f;
            float spawnTime = 180f;

            // Do visual stuff in phase 3
            float maxOpacity = phase3 ? 0.7f : 1f;
            int minAlpha = 255 - (int)(255 * maxOpacity);
            if (phase3)
            {
                if (calamityGlobalNPC.newAI[0] == playSpawnSoundTime)
                    SoundEngine.PlaySound(SoundID.Item161, NPC.Center);

                if (calamityGlobalNPC.newAI[0] > playSpawnSoundTime && calamityGlobalNPC.newAI[0] < stopSpawningDustTime)
                    CreateSpawnDust(NPC, false);

                calamityGlobalNPC.newAI[0] += 1f;
                if (calamityGlobalNPC.newAI[0] >= stopSpawningDustTime)
                {
                    calamityGlobalNPC.newAI[0] = playSpawnSoundTime + 1f;
                    NPC.SyncExtraAI();
                }
            }

            switch ((int)NPC.ai[0])
            {
                // Spawn animation.
                case 0:

                    // Avoid cheap bullshit.
                    NPC.damage = 0;

                    if (NPC.ai[1] == 0f)
                    {
                        NPC.velocity = new Vector2(0f, 5f);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0f, -80f), Vector2.Zero, ProjectileID.HallowBossDeathAurora, 0, 0f, Main.myPlayer);
                    }

                    if (NPC.ai[1] == playSpawnSoundTime)
                        SoundEngine.PlaySound(SoundID.Item161, NPC.Center);

                    NPC.velocity *= 0.95f;

                    if (NPC.ai[1] > playSpawnSoundTime && NPC.ai[1] < stopSpawningDustTime)
                        CreateSpawnDust(NPC);

                    NPC.ai[1] += 1f;
                    visible = false;
                    takeDamage = false;
                    NPC.Opacity = MathHelper.Clamp(NPC.ai[1] / spawnTime, 0f, 1f);

                    if (NPC.ai[1] >= spawnTime)
                    {
                        if (dayTimeEnrage && !NPC.AI_120_HallowBoss_IsGenuinelyEnraged())
                            NPC.ai[3] += 2f;

                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                        CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                    }

                    break;

                // Phase switch.
                case 1:

                    // Avoid cheap bullshit.
                    NPC.damage = 0;

                    float idleTimer = phase2 ? (death ? 10f : 15f) : (death ? 20f : 30f);
                    if (Main.getGoodWorld)
                        idleTimer *= 0.5f;
                    if (idleTimer < 10f)
                        idleTimer = 10f;

                    if (NPC.ai[1] <= 10f)
                    {
                        if (NPC.ai[1] == 0f)
                            CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                        // Despawn.
                        NPCAimedTarget targetData4 = NPC.GetTargetData();
                        if (targetData4.Invalid)
                        {
                            NPC.ai[0] = 13f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] += 1f;
                            NPC.velocity /= 4f;
                            NPC.netUpdate = true;
                            break;
                        }

                        Vector2 center = targetData4.Center;
                        center += new Vector2(0f, -400f);
                        if (NPC.Distance(center) > 200f)
                            center -= NPC.DirectionTo(center) * 100f;

                        Vector2 targetDirection = center - NPC.Center;
                        float lerpValue = Utils.GetLerpValue(100f, 600f, targetDirection.Length());
                        float targetDistance = targetDirection.Length();

                        float maxVelocity = death ? 24f : 21f;
                        if (targetDistance > maxVelocity)
                            targetDistance = maxVelocity;

                        NPC.velocity = Vector2.Lerp(targetDirection.SafeNormalize(Vector2.Zero) * targetDistance, targetDirection / 6f, lerpValue);
                        NPC.netUpdate = true;
                    }

                    NPC.velocity *= 0.92f;
                    NPC.ai[1] += 1f;
                    if (!(NPC.ai[1] >= idleTimer))
                        break;

                    int attackPatternLength = (int)NPC.ai[2];
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
                            attackType = 8;

                        if (attackPatternLength % attackIncrement == phase1Attack3)
                            attackType = 6;

                        if (attackPatternLength % attackIncrement == phase1Attack4)
                            attackType = 8;

                        if (attackPatternLength % attackIncrement == phase1Attack5)
                            attackType = 5;

                        if (attackPatternLength % attackIncrement == phase1Attack6)
                            attackType = 2;

                        if (attackPatternLength % attackIncrement == phase1Attack7)
                            attackType = 8;

                        if (attackPatternLength % attackIncrement == phase1Attack8)
                        {
                            attackType = 4;

                            // Adjust the upcoming Ethereal Lance attack depending on what random variable is chosen here.
                            calamityGlobalNPC.newAI[3] = Main.rand.Next(2);

                            // Sync the Calamity AI variables.
                            NPC.SyncExtraAI();
                        }

                        if (attackPatternLength % attackIncrement == phase1Attack9)
                            attackType = 8;

                        if (attackPatternLength % attackIncrement == phase1Attack10)
                            attackType = 5;

                        if (lifeRatio <= phase2LifeRatio)
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
                            NPC.SyncExtraAI();
                        }

                        if (attackPatternLength % attackIncrement == phase2Attack2)
                            attackType = phase3 ? 8 : 2;

                        if (attackPatternLength % attackIncrement == phase2Attack3)
                            attackType = 8;

                        if (attackPatternLength % attackIncrement == phase2Attack4)
                            attackType = 11;

                        if (attackPatternLength % attackIncrement == phase2Attack5)
                            attackType = 5;

                        if (attackPatternLength % attackIncrement == phase2Attack6)
                            attackType = 2;

                        if (attackPatternLength % attackIncrement == phase2Attack7)
                        {
                            if (phase3)
                            {
                                attackType = 4;

                                // Adjust the upcoming Ethereal Lance attack depending on what random variable is chosen here.
                                calamityGlobalNPC.newAI[3] = Main.rand.Next(2);

                                // Sync the Calamity AI variables.
                                NPC.SyncExtraAI();
                            }
                            else
                                attackType = 6;
                        }

                        if (attackPatternLength % attackIncrement == phase2Attack8)
                        {
                            attackType = 4;

                            // Adjust the upcoming Ethereal Lance attack depending on what random variable is chosen here.
                            calamityGlobalNPC.newAI[3] = Main.rand.Next(2);

                            // Sync the Calamity AI variables.
                            NPC.SyncExtraAI();
                        }

                        if (attackPatternLength % attackIncrement == phase2Attack9)
                            attackType = 8;

                        if (attackPatternLength % attackIncrement == phase2Attack10)
                            attackType = 12;
                    }

                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                    NPCAimedTarget targetData5 = NPC.GetTargetData();
                    bool despawnFlag = false;
                    if (NPC.AI_120_HallowBoss_IsGenuinelyEnraged() && !BossRushEvent.BossRushActive)
                    {
                        if (!Main.dayTime)
                            despawnFlag = true;

                        if (Main.dayTime && Main.time >= 53400D)
                            despawnFlag = true;
                    }

                    // Despawn.
                    if (targetData5.Invalid || NPC.Distance(targetData5.Center) > despawnDistanceGateValue || despawnFlag)
                        attackType = 13;

                    // Set charge direction.
                    if (attackType == 8 && targetData5.Center.X > NPC.Center.X)
                        attackType = 9;

                    if (attackType != 5 && attackType != 12)
                        NPC.velocity = NPC.DirectionFrom(targetData5.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2 * (targetData5.Center.X > NPC.Center.X).ToDirectionInt()) * 24f;

                    NPC.ai[0] = attackType;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] += 1f;
                    NPC.netUpdate = true;

                    break;

                // Spawn homing Rainbow Streaks.
                case 2:

                    // Avoid cheap bullshit.
                    NPC.damage = 0;

                    if (NPC.ai[1] == 0f)
                        SoundEngine.PlaySound(SoundID.Item164, NPC.Center);

                    Vector2 randomStreakOffset = new Vector2(-55f, -30f);
                    NPCAimedTarget targetData11 = NPC.GetTargetData();
                    Vector2 targetCenter = targetData11.Invalid ? NPC.Center : targetData11.Center;
                    if (NPC.Distance(targetCenter + rainbowStreakDistance) > movementDistanceGateValue)
                        NPC.SimpleFlyMovement(NPC.DirectionTo(targetCenter + rainbowStreakDistance).SafeNormalize(Vector2.Zero) * velocity, acceleration);

                    if (NPC.ai[1] < 60f)
                        AI_120_HallowBoss_DoMagicEffect(NPC.Center + randomStreakOffset, 1, Utils.GetLerpValue(0f, 60f, NPC.ai[1], clamped: true), NPC);

                    int streakSpawnFrequency = Main.getGoodWorld ? 1 : 2;
                    if (phase3)
                        streakSpawnFrequency *= 2;

                    if ((int)NPC.ai[1] % streakSpawnFrequency == 0 && NPC.ai[1] < 60f)
                    {
                        int projectileType = ProjectileID.HallowBossRainbowStreak;
                        int projectileDamage = boltDamage;

                        float ai3 = NPC.ai[1] / 60f;
                        Vector2 rainbowStreakVelocity = new Vector2(0f, death ? -10f : -8f).RotatedBy(MathHelper.PiOver2 * Main.rand.NextFloatDirection());
                        if (phase2)
                            rainbowStreakVelocity = new Vector2(0f, death ? -12f : -10f).RotatedBy(MathHelper.TwoPi * Main.rand.NextFloat());

                        if (dayTimeEnrage)
                            rainbowStreakVelocity *= MathHelper.Lerp(0.8f, 1.6f, ai3);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + randomStreakOffset, rainbowStreakVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, NPC.target, ai3);
                            if (phase3)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + randomStreakOffset, -rainbowStreakVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, NPC.target, 1f - ai3);
                            }
                        }

                        // Spawn extra homing Rainbow Streaks per player.
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int multiplayerStreakSpawnFrequency = (int)(NPC.ai[1] / streakSpawnFrequency);
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (NPC.Boss_CanShootExtraAt(i, multiplayerStreakSpawnFrequency % 3, 3, 2400f))
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + randomStreakOffset, rainbowStreakVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, i, ai3);
                            }
                        }
                    }

                    NPC.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? (death ? 30f : 36f) : (death ? 60f : 72f)) + 30f * lessTimeSpentPerPhaseMultiplier;
                    if (NPC.ai[1] >= 60f + extraPhaseTime)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
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
                    NPC.damage = 0;

                    if (NPC.ai[1] == 0f)
                        SoundEngine.PlaySound(SoundID.Item162, NPC.Center);

                    float lanceGateValue = death ? 75f : 100f;

                    if (NPC.ai[1] >= 6f && NPC.ai[1] < 54f)
                    {
                        AI_120_HallowBoss_DoMagicEffect(NPC.Center + new Vector2(-55f, -20f), 2, Utils.GetLerpValue(0f, lanceGateValue, NPC.ai[1], clamped: true), NPC);
                        AI_120_HallowBoss_DoMagicEffect(NPC.Center + new Vector2(55f, -20f), 4, Utils.GetLerpValue(0f, lanceGateValue, NPC.ai[1], clamped: true), NPC);
                    }

                    NPCAimedTarget targetData10 = NPC.GetTargetData();
                    targetCenter = targetData10.Invalid ? NPC.Center : targetData10.Center;
                    if (NPC.Distance(targetCenter + etherealLanceDistance) > movementDistanceGateValue)
                        NPC.SimpleFlyMovement(NPC.DirectionTo(targetCenter + etherealLanceDistance).SafeNormalize(Vector2.Zero) * velocity, acceleration);

                    int lanceRotation = death ? 10 : 8;
                    if (NPC.ai[1] % (dayTimeEnrage ? 2f : 3f) == 0f && NPC.ai[1] < lanceGateValue)
                    {
                        int lanceAmount = phase3 ? 2 : 1;
                        for (int i = 0; i < lanceAmount; i++)
                        {
                            int lanceFrequency = (int)(NPC.ai[1] / (dayTimeEnrage ? 2f : 3f));
                            lanceRotation += (death ? 5 : 4) * i;
                            Vector2 lanceDirection = Vector2.UnitX.RotatedBy((float)Math.PI / (lanceRotation * 2) + lanceFrequency * ((float)Math.PI / lanceRotation));
                            if (calamityGlobalNPC.newAI[3] == 0f)
                                lanceDirection.X += (lanceDirection.X > 0f) ? 0.5f : -0.5f;

                            lanceDirection = lanceDirection.SafeNormalize(Vector2.UnitY);
                            float spawnDistance = 600f;

                            Vector2 playerCenter = targetData10.Center;
                            if (NPC.Distance(playerCenter) > 2400f)
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

                                spawnLocation = playerCenter - lanceSpawnDirection.SafeNormalize(Vector2.UnitY) * spawnDistance;
                            }

                            int projectileType = ProjectileID.FairyQueenLance;
                            int projectileDamage = lanceDamage;

                            Vector2 v3 = targetHoverPos - spawnLocation;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnLocation, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v3.ToRotation(), NPC.ai[1] / lanceGateValue);

                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                continue;

                            // Spawn extra Ethereal Lances per player.
                            for (int j = 0; j < Main.maxPlayers; j++)
                            {
                                if (!NPC.Boss_CanShootExtraAt(j, lanceFrequency % 3, 3, 2400f))
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

                                    spawnLocation = playerCenter - extraPlayerSpawnDirection.SafeNormalize(Vector2.UnitY) * spawnDistance;
                                }

                                v3 = extraPlayerSpawnLocation - spawnLocation;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnLocation, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v3.ToRotation(), NPC.ai[1] / lanceGateValue);
                            }
                        }
                    }

                    NPC.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 24f : 48f) + 20f * lessTimeSpentPerPhaseMultiplier;
                    if (NPC.ai[1] >= lanceGateValue + extraPhaseTime)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        NPC.netUpdate = true;

                        // Sync the Calamity AI variables.
                        NPC.SyncExtraAI();
                    }

                    break;

                // Spawn Everlasting Rainbow spiral.
                case 5:

                    // Avoid cheap bullshit.
                    NPC.damage = 0;

                    if (NPC.ai[1] == 0f)
                        SoundEngine.PlaySound(SoundID.Item163, NPC.Center);

                    Vector2 magicSpawnOffset = new Vector2(55f, -30f);
                    Vector2 everlastingRainbowSpawn = NPC.Center + magicSpawnOffset;
                    if (NPC.ai[1] < 42f)
                        AI_120_HallowBoss_DoMagicEffect(NPC.Center + magicSpawnOffset, 3, Utils.GetLerpValue(0f, 42f, NPC.ai[1], clamped: true), NPC);

                    NPCAimedTarget targetData7 = NPC.GetTargetData();
                    targetCenter = targetData7.Invalid ? NPC.Center : targetData7.Center;
                    if (NPC.Distance(targetCenter + everlastingRainbowDistance) > movementDistanceGateValue)
                        NPC.SimpleFlyMovement(NPC.DirectionTo(targetCenter + everlastingRainbowDistance).SafeNormalize(Vector2.Zero) * velocity * 0.5f, acceleration * 0.75f);

                    if (NPC.ai[1] % 42f == 0f && NPC.ai[1] < 42f)
                    {
                        float projRotation = MathHelper.TwoPi * Main.rand.NextFloat();
                        float totalProjectiles = Main.getGoodWorld ? 30f : death ? (dayTimeEnrage ? 22f : 15f) : (dayTimeEnrage ? 18f : 13f);
                        int projIndex = 0;
                        bool inversePhase2SpreadPattern = Main.rand.NextBool();
                        for (float i = 0f; i < 1f; i += 1f / totalProjectiles)
                        {
                            int projectileType = ProjectileID.HallowBossLastingRainbow;
                            int projectileDamage = rainbowDamage;
                            int projectileType2 = ProjectileID.HallowBossRainbowStreak;
                            int projectileDamage2 = boltDamage;

                            float projRotationMultiplier = i;
                            Vector2 spinningpoint = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 + MathHelper.TwoPi * projRotationMultiplier + projRotation);

                            float initialVelocity = death ? 2f : 1.75f;
                            if (dayTimeEnrage && projIndex % 2 == 0)
                                initialVelocity *= 2f;
                            if (Main.getGoodWorld)
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
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), everlastingRainbowSpawn + spinningpoint.RotatedBy(-MathHelper.PiOver2) * 30f, spinningpoint * initialVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, 0f, projRotationMultiplier);

                                if (phase3)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), everlastingRainbowSpawn + spinningpoint.RotatedBy(-MathHelper.PiOver2) * 30f, spinningpoint * (death ? 3f : 2f) * initialVelocity, projectileType2, projectileDamage2, 0f, Main.myPlayer, NPC.target, projRotationMultiplier);
                                }
                            }

                            projIndex++;
                        }
                    }

                    NPC.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 36f : 72f) + 30f * lessTimeSpentPerPhaseMultiplier;
                    if (NPC.ai[1] >= 72f + extraPhaseTime)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Use Sun Dance.
                case 6:

                    // Avoid cheap bullshit.
                    NPC.damage = 0;

                    // Increase durability.
                    calamityGlobalNPC.DR = shouldBeInPhase2ButIsStillInPhase1 ? 0.99f : 0.575f;

                    int totalSunDances = phase2 ? 2 : 3;
                    float sunDanceGateValue = dayTimeEnrage ? 35f : death ? 40f : 50f;
                    float totalSunDancePhaseTime = totalSunDances * sunDanceGateValue;

                    Vector2 sunDanceHoverOffset = new Vector2(0f, -100f);
                    Vector2 position = NPC.Center + sunDanceHoverOffset;

                    NPCAimedTarget targetData2 = NPC.GetTargetData();
                    targetCenter = targetData2.Invalid ? NPC.Center : targetData2.Center;
                    if (NPC.Distance(targetCenter + sunDanceDistance) > movementDistanceGateValue)
                        NPC.SimpleFlyMovement(NPC.DirectionTo(targetCenter + sunDanceDistance).SafeNormalize(Vector2.Zero) * velocity * 0.3f, acceleration * 0.7f);

                    if (NPC.ai[1] % sunDanceGateValue == 0f && NPC.ai[1] < totalSunDancePhaseTime)
                    {
                        int projectileType = ProjectileID.FairyQueenSunDance;
                        int projectileDamage = sunDanceDamage;

                        int sunDanceExtension = (int)(NPC.ai[1] / sunDanceGateValue);
                        int targetFloatDirection = (targetData2.Center.X > NPC.Center.X) ? 1 : 0;
                        float projAmount = phase2 ? 8f : 6f;
                        float projRotation = 1f / projAmount;
                        for (float j = 0f; j < 1f; j += projRotation)
                        {
                            float projDirection = (j + projRotation * 0.5f + sunDanceExtension * projRotation * 0.5f) % 1f;
                            float ai = MathHelper.TwoPi * (projDirection + targetFloatDirection);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, ai, NPC.whoAmI);
                        }
                    }

                    NPC.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? (death ? 105f : 110f) : (death ? 140f : 150f)) + 30f * lessTimeSpentPerPhaseMultiplier; // 112.5 is too little
                    if (NPC.ai[1] >= totalSunDancePhaseTime + extraPhaseTime)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Spawn rows of Ethereal Lances.
                case 7:

                    // Avoid cheap bullshit.
                    NPC.damage = 0;

                    // Expert attack or not.
                    bool expertAttack = calamityGlobalNPC.newAI[2] == 0f;

                    int numLanceWalls = (death && !expertAttack) ? 9 : 6;
                    float lanceWallSpawnGateValue = expertAttack ? 50f : (death ? 32f : 45f);
                    if (dayTimeEnrage)
                        lanceWallSpawnGateValue -= expertAttack ? 4f : 6f;

                    float lanceWallPhaseTime = lanceWallSpawnGateValue * numLanceWalls;

                    NPCAimedTarget targetData9 = NPC.GetTargetData();
                    destination = targetData9.Invalid ? NPC.Center : targetData9.Center;
                    if (NPC.Distance(destination + etherealLanceDistance) > movementDistanceGateValue)
                        NPC.SimpleFlyMovement(NPC.DirectionTo(destination + etherealLanceDistance).SafeNormalize(Vector2.Zero) * velocity * 0.4f, acceleration);

                    if ((int)NPC.ai[1] % lanceWallSpawnGateValue == 0f && NPC.ai[1] < lanceWallPhaseTime)
                    {
                        SoundEngine.PlaySound(SoundID.Item162, NPC.Center);

                        float totalProjectiles = death ? 19f : 15f;
                        float lanceSpacing = death ? (expertAttack ? 150f : 200f) : 175f;
                        float lanceWallSize = totalProjectiles * lanceSpacing;

                        Vector2 lanceSpawnOffset = targetData9.Center;
                        if (NPC.Distance(lanceSpawnOffset) <= 3200f)
                        {
                            totalProjectiles += 5f;
                            lanceWallSize *= (death && expertAttack) ? 0.7f : 0.5f;

                            // Used to give the lance walls a random rotation.
                            calamityGlobalNPC.newAI[3] = Main.rand.NextFloat(MathHelper.TwoPi);
                            // Keeps track of the total number of lance walls used.
                            calamityGlobalNPC.newAI[1] += 1f;
                            // Sync the Calamity AI variables.
                            NPC.SyncExtraAI();

                            lanceSpawnOffset += -Vector2.UnitX.RotatedBy(calamityGlobalNPC.newAI[3]) * (lanceWallSize / 2f);
                            Vector2 lanceWallStartingPosition = Vector2.UnitY.RotatedBy(calamityGlobalNPC.newAI[3]) * lanceWallSize;
                            Vector2 lanceWallDirection = calamityGlobalNPC.newAI[3].ToRotationVector2();

                            int projectileType = ProjectileID.FairyQueenLance;
                            int projectileDamage = lanceDamage;

                            for (float i = 0f; i <= 1f; i += 1f / totalProjectiles)
                            {
                                Vector2 spawnLocation = lanceSpawnOffset + lanceWallStartingPosition * (i - 0.5f) * (expertAttack ? 1f : 2f);
                                Vector2 v2 = lanceWallDirection;
                                if (expertAttack)
                                {
                                    Vector2 lanceWallSpawnPredictiveness = targetData9.Velocity * 20f * i;
                                    Vector2 lanceWallSpawnLocation = spawnLocation.DirectionTo(targetData9.Center + lanceWallSpawnPredictiveness);
                                    v2 = Vector2.Lerp(lanceWallDirection, lanceWallSpawnLocation, 0.75f).SafeNormalize(Vector2.UnitY);
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnLocation, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v2.ToRotation(), i);
                            }
                        }
                    }

                    NPC.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 24f : 48f) + 20f * lessTimeSpentPerPhaseMultiplier;
                    if (NPC.ai[1] >= lanceWallPhaseTime + extraPhaseTime)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        calamityGlobalNPC.newAI[1] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        NPC.SyncExtraAI();
                        NPC.netUpdate = true;
                    }

                    break;

                // Charge either left or right.
                case 8:
                case 9:

                    int chargeDirection = (NPC.ai[0] != 8f) ? 1 : (-1);

                    AI_120_HallowBoss_DoMagicEffect(NPC.Center, 5, Utils.GetLerpValue(40f, 90f, NPC.ai[1], clamped: true), NPC);

                    float chargeGateValue = 40f;
                    float playChargeSoundTime = 20f;
                    float chargeDuration = phase3 ? 40f : 50f;
                    float totalPhaseTime = chargeGateValue + chargeDuration;
                    float chargeStartDistance = phase3 ? 1000f : 800f;
                    float chargeVelocity = phase3 ? 100f : 70f;
                    float chargeAcceleration = phase3 ? 0.1f : 0.07f;

                    if (NPC.ai[1] <= chargeGateValue)
                    {
                        // Avoid cheap bullshit.
                        NPC.damage = 0;

                        if (NPC.ai[1] == playChargeSoundTime)
                            SoundEngine.PlaySound(SoundID.Item160, NPC.Center);

                        NPCAimedTarget targetData3 = NPC.GetTargetData();
                        destination = (targetData3.Invalid ? NPC.Center : targetData3.Center) + new Vector2(chargeDirection * -chargeStartDistance, 0f);
                        NPC.SimpleFlyMovement(NPC.DirectionTo(destination).SafeNormalize(Vector2.Zero) * velocity, acceleration * 2f);

                        if (NPC.ai[1] == chargeGateValue)
                            NPC.velocity *= 0.3f;
                    }
                    else if (NPC.ai[1] <= chargeGateValue + chargeDuration)
                    {
                        // Spawn Rainbow Streaks during charge.
                        if (NPC.ai[1] == chargeGateValue + 1f)
                            SoundEngine.PlaySound(SoundID.Item164, NPC.Center);

                        float rainbowStreakGateValue = 2f;
                        if ((NPC.ai[1] - 1f) % rainbowStreakGateValue == 0f)
                        {
                            int projectileType = ProjectileID.HallowBossRainbowStreak;
                            int projectileDamage = boltDamage;

                            float ai3 = (NPC.ai[1] - chargeGateValue - 1f) / chargeDuration;
                            Vector2 rainbowStreakVelocity = new Vector2(0f, death ? -5f : -4f).RotatedBy(MathHelper.PiOver2 * Main.rand.NextFloatDirection());
                            if (phase2)
                                rainbowStreakVelocity = new Vector2(0f, death ? -6f : -5f).RotatedBy(MathHelper.TwoPi * Main.rand.NextFloat());

                            rainbowStreakVelocity.X *= 2f;
                            if (!phase2)
                                rainbowStreakVelocity.Y *= 0.5f;

                            if (dayTimeEnrage)
                                rainbowStreakVelocity *= MathHelper.Lerp(0.8f, 1.6f, ai3);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, rainbowStreakVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, NPC.target, ai3);
                            }

                            // Spawn extra homing Rainbow Streaks per player.
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int multiplayerStreakSpawnFrequency = (int)((NPC.ai[1] - chargeGateValue - 1f) / rainbowStreakGateValue);
                                for (int i = 0; i < Main.maxPlayers; i++)
                                {
                                    if (NPC.Boss_CanShootExtraAt(i, multiplayerStreakSpawnFrequency % 3, 3, 2400f))
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, rainbowStreakVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, i, ai3);
                                }
                            }
                        }

                        NPC.velocity = Vector2.Lerp(value2: new Vector2(chargeDirection * chargeVelocity, 0f), value1: NPC.velocity, amount: chargeAcceleration);

                        if (NPC.ai[1] == chargeGateValue + chargeDuration)
                            NPC.velocity *= 0.45f;

                        // Unlike other temporary damage boosts from bosses, vanilla Empress AI increases damage as defDamage while dashing, so we don't need to account for it
                        NPC.damage = (int)Math.Round(ContactDamageCorrection.CalculateDamageForEnrage() * DashDamageMult);
                    }
                    else
                    {
                        // Avoid cheap bullshit.
                        NPC.damage = 0;

                        NPC.velocity *= 0.92f;
                    }

                    NPC.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 60f : 120f) * (lessTimeSpentPerPhaseMultiplier < 1 ? lessTimeSpentPerPhaseMultiplier * 1.5f : lessTimeSpentPerPhaseMultiplier);
                    if (NPC.ai[1] >= totalPhaseTime && NPC.ai[1] <= totalPhaseTime + 10f)
                    {
                        Vector2 center = NPC.GetTargetData().Center;
                        center += new Vector2(0f, -200f);
                        if (NPC.Distance(center) > 200f)
                            center -= NPC.DirectionTo(center) * 100f;

                        Vector2 targetDirection = center - NPC.Center;
                        float lerpValue = Utils.GetLerpValue(100f, 600f, targetDirection.Length());
                        float targetDistance = targetDirection.Length();

                        float maxVelocity = death ? 24f : 21f;
                        if (targetDistance > maxVelocity)
                            targetDistance = maxVelocity;

                        NPC.velocity = Vector2.Lerp(targetDirection.SafeNormalize(Vector2.Zero) * targetDistance, targetDirection / 6f, lerpValue);
                        NPC.netUpdate = true;
                    }
                    if (NPC.ai[1] >= totalPhaseTime + extraPhaseTime)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Phase 2 animation.
                case 10:

                    // Avoid cheap bullshit.
                    NPC.damage = 0;

                    if (NPC.ai[1] == 0f)
                        SoundEngine.PlaySound(SoundID.Item161, NPC.Center);

                    takeDamage = !(NPC.ai[1] >= 30f) || !(NPC.ai[1] <= 170f);

                    NPC.velocity *= 0.95f;

                    if (NPC.ai[1] == 90f)
                    {
                        if (NPC.ai[3] == 0f)
                            NPC.ai[3] = 1f;

                        if (NPC.ai[3] == 2f)
                            NPC.ai[3] = 3f;

                        NPC.Center = NPC.GetTargetData().Center + new Vector2(0f, -250f);
                        NPC.netUpdate = true;
                    }

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= 180f)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Spawn Ethereal Lances ahead of the target.
                case 11:
                    // Avoid cheap bullshit.
                    NPC.damage = 0;

                    if (NPC.ai[1] == 0f)
                        SoundEngine.PlaySound(SoundID.Item162, NPC.Center);

                    float lanceGateValue2 = death ? 75f : 100f;

                    if (NPC.ai[1] >= 6f && NPC.ai[1] < 54f)
                    {
                        AI_120_HallowBoss_DoMagicEffect(NPC.Center + new Vector2(-55f, -20f), 2, Utils.GetLerpValue(0f, lanceGateValue2, NPC.ai[1], clamped: true), NPC);
                        AI_120_HallowBoss_DoMagicEffect(NPC.Center + new Vector2(55f, -20f), 4, Utils.GetLerpValue(0f, lanceGateValue2, NPC.ai[1], clamped: true), NPC);
                    }

                    NPCAimedTarget targetData6 = NPC.GetTargetData();
                    targetCenter = targetData6.Invalid ? NPC.Center : targetData6.Center;
                    if (NPC.Distance(targetCenter + etherealLanceDistance) > movementDistanceGateValue)
                        NPC.SimpleFlyMovement(NPC.DirectionTo(targetCenter + etherealLanceDistance).SafeNormalize(Vector2.Zero) * velocity, acceleration);

                    float etherealLanceGateValue = death ? 5f : 6f;
                    if (dayTimeEnrage)
                        etherealLanceGateValue -= 1f;

                    if (NPC.ai[1] % etherealLanceGateValue == 0f && NPC.ai[1] < lanceGateValue2)
                    {
                        int numLances = phase3 ? 4 : 3;
                        for (int i = 0; i < numLances; i++)
                        {
                            // Spawn another lance in the opposite location
                            bool oppositeLance = i % 2 == 0;

                            Vector2 inverseTargetVel = oppositeLance ? targetData6.Velocity : -targetData6.Velocity;
                            inverseTargetVel.SafeNormalize(-Vector2.UnitY);
                            float spawnDistance = 100f + (i * 100f);

                            targetCenter = targetData6.Center;
                            if (NPC.Distance(targetCenter) > 2400f)
                                continue;

                            Vector2 straightLanceSpawnPredict = targetCenter + (oppositeLance ? -targetData6.Velocity : targetData6.Velocity) * 90;
                            Vector2 straightLanceSpawnDirection = targetCenter + inverseTargetVel * spawnDistance;
                            if (straightLanceSpawnDirection.Distance(targetCenter) < spawnDistance)
                            {
                                Vector2 straightLanceSpawnLocation = targetCenter - straightLanceSpawnDirection;
                                if (straightLanceSpawnLocation == Vector2.Zero)
                                    straightLanceSpawnLocation = inverseTargetVel;

                                straightLanceSpawnDirection = targetCenter - straightLanceSpawnLocation.SafeNormalize(Vector2.UnitY) * spawnDistance;
                            }

                            int projectileType = ProjectileID.FairyQueenLance;
                            int projectileDamage = lanceDamage;

                            Vector2 v = straightLanceSpawnPredict - straightLanceSpawnDirection;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), straightLanceSpawnDirection, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v.ToRotation(), NPC.ai[1] / lanceGateValue2);

                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                continue;

                            int multiplayerExtraStraightLances = (int)(NPC.ai[1] / etherealLanceGateValue);
                            for (int l = 0; l < Main.maxPlayers; l++)
                            {
                                if (!NPC.Boss_CanShootExtraAt(l, multiplayerExtraStraightLances % 3, 3, 2400f))
                                    continue;

                                Player player = Main.player[l];
                                inverseTargetVel = oppositeLance ? player.velocity : -player.velocity;
                                inverseTargetVel.SafeNormalize(-Vector2.UnitY);
                                targetCenter = player.Center;
                                Vector2 extraPlayerLancePredict = targetCenter + (oppositeLance ? -player.velocity : player.velocity) * 90;
                                straightLanceSpawnDirection = targetCenter + inverseTargetVel * spawnDistance;
                                if (straightLanceSpawnDirection.Distance(targetCenter) < spawnDistance)
                                {
                                    Vector2 extraPlayerLanceSpawnLocation = targetCenter - straightLanceSpawnDirection;
                                    if (extraPlayerLanceSpawnLocation == Vector2.Zero)
                                        extraPlayerLanceSpawnLocation = inverseTargetVel;

                                    straightLanceSpawnDirection = targetCenter - extraPlayerLanceSpawnLocation.SafeNormalize(Vector2.UnitY) * spawnDistance;
                                }

                                v = extraPlayerLancePredict - straightLanceSpawnDirection;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), straightLanceSpawnDirection, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v.ToRotation(), NPC.ai[1] / lanceGateValue2);
                            }
                        }
                    }

                    NPC.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 24f : 48f) * lessTimeSpentPerPhaseMultiplier;
                    if (NPC.ai[1] >= lanceGateValue2 + extraPhaseTime)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Spawn homing Rainbow Streaks.
                case 12:

                    // Avoid cheap bullshit.
                    NPC.damage = 0;

                    Vector2 projRandomOffset = new Vector2(-55f, -30f);

                    if (NPC.ai[1] == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item165, NPC.Center);
                        NPC.velocity = new Vector2(0f, -12f);
                    }

                    NPC.velocity *= 0.95f;

                    bool shouldSpawnStreaks = NPC.ai[1] < 60f && NPC.ai[1] >= 10f;
                    if (shouldSpawnStreaks)
                        AI_120_HallowBoss_DoMagicEffect(NPC.Center + projRandomOffset, 1, Utils.GetLerpValue(0f, 60f, NPC.ai[1], clamped: true), NPC);

                    int stationaryStreakSpawnFrequency = 4;
                    if (dayTimeEnrage)
                        stationaryStreakSpawnFrequency -= 1;
                    if (phase3)
                        stationaryStreakSpawnFrequency *= 2;

                    float streakHomeTime = (NPC.ai[1] - 10f) / 50f;
                    if ((int)NPC.ai[1] % stationaryStreakSpawnFrequency == 0 && shouldSpawnStreaks)
                    {
                        int projectileType = ProjectileID.HallowBossRainbowStreak;
                        int projectileDamage = boltDamage;

                        Vector2 vector = new Vector2(0f, (death ? -24f : -22f) - (phase3 ? ((death ? 6f : 4f) * streakHomeTime) : 0f)).RotatedBy(MathHelper.TwoPi * streakHomeTime);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projRandomOffset, vector, projectileType, projectileDamage, 0f, Main.myPlayer, NPC.target, streakHomeTime);
                            if (phase3)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projRandomOffset, -vector, projectileType, projectileDamage, 0f, Main.myPlayer, NPC.target, 1f - streakHomeTime);
                            }
                        }

                        // Spawn extra homing Rainbow Streaks per player.
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int extraStationaryStreakSpawnFrequency = (int)(NPC.ai[1] % stationaryStreakSpawnFrequency);
                            for (int j = 0; j < Main.maxPlayers; j++)
                            {
                                if (NPC.Boss_CanShootExtraAt(j, extraStationaryStreakSpawnFrequency % 3, 3, 2400f))
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projRandomOffset, vector, projectileType, projectileDamage, 0f, Main.myPlayer, j, streakHomeTime);
                            }
                        }
                    }

                    NPC.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 36f : 72f) + 30f * lessTimeSpentPerPhaseMultiplier;
                    if (NPC.ai[1] >= (death ? 105f : 120f) + extraPhaseTime)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Despawn.
                case 13:

                    // Avoid cheap bullshit.
                    NPC.damage = 0;

                    if (NPC.ai[1] == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item165, NPC.Center);
                        NPC.velocity = new Vector2(0f, -7f);
                    }

                    NPC.velocity *= 0.95f;

                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                    NPCAimedTarget targetData = NPC.GetTargetData();

                    visible = false;

                    bool trueDespawnFlag = false;
                    bool shouldDespawn = false;
                    if (!trueDespawnFlag)
                    {
                        if (NPC.AI_120_HallowBoss_IsGenuinelyEnraged() && !BossRushEvent.BossRushActive)
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
                        bool hasNoTarget = targetData.Invalid || NPC.Distance(targetData.Center) > despawnDistanceGateValue;
                        trueDespawnFlag = trueDespawnFlag || hasNoTarget;
                    }

                    NPC.alpha = Utils.Clamp(NPC.alpha + trueDespawnFlag.ToDirectionInt() * 5, 0, 255);
                    bool alphaExtreme = NPC.alpha == 0 || NPC.alpha == 255;

                    int despawnDustAmt = 5;
                    for (int i = 0; i < despawnDustAmt; i++)
                    {
                        float despawnDustOpacity = MathHelper.Lerp(1.3f, 0.7f, NPC.Opacity);
                        Color newColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f);
                        int despawnRainbowDust = Dust.NewDust(NPC.position - NPC.Size * 0.5f, NPC.width * 2, NPC.height * 2, DustID.RainbowMk2, 0f, 0f, 0, newColor);
                        Main.dust[despawnRainbowDust].position = NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height);
                        Main.dust[despawnRainbowDust].velocity *= Main.rand.NextFloat() * 0.8f;
                        Main.dust[despawnRainbowDust].noGravity = true;
                        Main.dust[despawnRainbowDust].scale = 0.9f + Main.rand.NextFloat() * 1.2f;
                        Main.dust[despawnRainbowDust].fadeIn = 0.4f + Main.rand.NextFloat() * 1.2f * despawnDustOpacity;
                        Main.dust[despawnRainbowDust].velocity += Vector2.UnitY * -2f;
                        Main.dust[despawnRainbowDust].scale = 0.35f;
                        if (despawnRainbowDust != 6000)
                        {
                            Dust dust = Dust.BetterCloneDust(despawnRainbowDust);
                            dust.scale /= 2f;
                            dust.fadeIn *= 0.85f;
                            dust.color = new Color(255, 255, 255, 255);
                        }
                    }

                    NPC.ai[1] += 1f;
                    if (!(NPC.ai[1] >= 20f && alphaExtreme))
                        break;

                    if (NPC.alpha == 255)
                    {
                        NPC.active = false;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);

                        return false;
                    }

                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                    break;
            }

            NPC.dontTakeDamage = !takeDamage;

            if (phase3)
                NPC.defense = (int)Math.Round(NPC.defDefense * 0.8);
            else if (phase2)
                NPC.defense = (int)Math.Round(NPC.defDefense * 1.2);
            else
                NPC.defense = NPC.defDefense;

            if ((NPC.localAI[0] += 1f) >= 44f)
                NPC.localAI[0] = 0f;

            if (visible)
                NPC.alpha = Utils.Clamp(NPC.alpha - 5, 0, 255);

            Lighting.AddLight(NPC.Center, Vector3.One * NPC.Opacity);

            return false;
        }

        private static void CreateSpawnDust(NPC npc, bool useAI = true)
        {
            int spawnDustAmount = 2;
            float timer = useAI ? npc.ai[1] : npc.Calamity().newAI[0];
            float spawnTime = 180f;
            for (int i = 0; i < spawnDustAmount; i++)
            {
                float fadeInScalar = MathHelper.Lerp(1.3f, 0.7f, npc.Opacity) * Utils.GetLerpValue(0f, 120f, timer, clamped: true);
                Color newColor = Main.hslToRgb(timer / spawnTime, 1f, 0.5f);
                int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.RainbowMk2, 0f, 0f, 0, newColor);
                Main.dust[dust].position = npc.Center + Main.rand.NextVector2Circular((float)npc.width * 3f, (float)npc.height * 3f) + new Vector2(0f, -150f);
                Main.dust[dust].velocity *= Main.rand.NextFloat() * 0.8f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].fadeIn = 0.6f + Main.rand.NextFloat() * 0.7f * fadeInScalar;
                Main.dust[dust].velocity += Vector2.UnitY * 3f;
                Main.dust[dust].scale = 0.35f;
                if (dust != Main.maxDust)
                {
                    Dust dust2 = Dust.BetterCloneDust(dust);
                    dust2.scale /= 2f;
                    dust2.fadeIn *= 0.85f;
                    dust2.color = new Color(255, 255, 255, 255);
                }
            }
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
                            dust.velocity = (MathHelper.TwoPi * (i / 4f) + MathHelper.PiOver4).ToRotationVector2() * 8f * Utils.GetLerpValue(1f, 0f, progress, clamped: true);
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
