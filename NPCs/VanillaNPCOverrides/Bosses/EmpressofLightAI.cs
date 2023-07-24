using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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

            float acceleration = death ? 0.7f : 0.6f;
            float velocity = death ? 18f : 15f;
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
                        int num67 = 2;
                        for (int m = 0; m < num67; m++)
                        {
                            float num68 = MathHelper.Lerp(1.3f, 0.7f, npc.Opacity) * Utils.GetLerpValue(0f, 120f, npc.ai[1], clamped: true);
                            Color newColor2 = Main.hslToRgb(npc.ai[1] / 180f, 1f, 0.5f);
                            int num69 = Dust.NewDust(npc.position, npc.width, npc.height, 267, 0f, 0f, 0, newColor2);
                            Main.dust[num69].position = npc.Center + Main.rand.NextVector2Circular(npc.width * 3f, npc.height * 3f) + new Vector2(0f, -150f);
                            Main.dust[num69].velocity *= Main.rand.NextFloat() * 0.8f;
                            Main.dust[num69].noGravity = true;
                            Main.dust[num69].fadeIn = 0.6f + Main.rand.NextFloat() * 0.7f * num68;
                            Main.dust[num69].velocity += Vector2.UnitY * 3f;
                            Main.dust[num69].scale = 0.35f;
                            if (num69 != 6000)
                            {
                                Dust dust2 = Dust.CloneDust(num69);
                                dust2.scale /= 2f;
                                dust2.fadeIn *= 0.85f;
                                dust2.color = new Color(255, 255, 255, 255);
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

                    float num33 = phase2 ? (death ? 10f : 15f) : (death ? 20f : 30f);
                    if (Main.getGoodWorld)
                        num33 *= 0.5f;
                    if (num33 < 10f)
                        num33 = 10f;

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

                        Vector2 vector2 = center - npc.Center;
                        float lerpValue = Utils.GetLerpValue(100f, 600f, vector2.Length());
                        float num34 = vector2.Length();

                        float maxVelocity = death ? 24f : 21f;
                        if (num34 > maxVelocity)
                            num34 = maxVelocity;

                        npc.velocity = Vector2.Lerp(vector2.SafeNormalize(Vector2.Zero) * num34, vector2 / 6f, lerpValue);
                        npc.netUpdate = true;
                    }

                    npc.velocity *= 0.92f;
                    npc.ai[1] += 1f;
                    if (!(npc.ai[1] >= num33))
                        break;

                    int num35 = (int)npc.ai[2];
                    int num36 = 2;
                    int num37 = 0;

                    if (!phase2)
                    {
                        int num38 = num37++;
                        int num39 = num37++;
                        int num40 = num37++;
                        int num41 = num37++;
                        int num42 = num37++;
                        int num43 = num37++;
                        int num44 = num37++;
                        int num45 = num37++;
                        int num46 = num37++;
                        int num47 = num37++;

                        if (num35 % num37 == num38)
                            num36 = 2;

                        if (num35 % num37 == num39)
                            num36 = 6;

                        if (num35 % num37 == num40)
                            num36 = 8;

                        if (num35 % num37 == num41)
                        {
                            num36 = 4;

                            // Adjust the upcoming Ethereal Lance attack depending on what random variable is chosen here.
                            calamityGlobalNPC.newAI[0] = Main.rand.Next(2);

                            // Sync the Calamity AI variables.
                            npc.SyncExtraAI();
                        }

                        if (num35 % num37 == num42)
                            num36 = 5;

                        if (num35 % num37 == num43)
                            num36 = 8;

                        if (num35 % num37 == num44)
                            num36 = 2;

                        if (num35 % num37 == num45)
                        {
                            num36 = 4;

                            // Adjust the upcoming Ethereal Lance attack depending on what random variable is chosen here.
                            calamityGlobalNPC.newAI[0] = Main.rand.Next(2);

                            // Sync the Calamity AI variables.
                            npc.SyncExtraAI();
                        }

                        if (num35 % num37 == num46)
                            num36 = 8;

                        if (num35 % num37 == num47)
                            num36 = 5;

                        if (lifeRatio <= 0.5f)
                            num36 = 10;
                    }

                    if (phase2)
                    {
                        int num48 = num37++;
                        int num49 = num37++;
                        int num50 = num37++;
                        int num51 = num37++;
                        int num52 = num37++;
                        int num53 = num37++;
                        int num54 = num37++;
                        int num55 = num37++;
                        int num56 = num37++;
                        int num57 = num37++;

                        if (num35 % num37 == num48)
                        {
                            num36 = 7;

                            // Adjust the upcoming Ethereal Lance attack depending on what random variable is chosen here.
                            calamityGlobalNPC.newAI[2] = Main.rand.Next(2);

                            // Sync the Calamity AI variables.
                            npc.SyncExtraAI();
                        }

                        if (num35 % num37 == num49)
                            num36 = 2;

                        if (num35 % num37 == num50)
                            num36 = 8;

                        if (num35 % num37 == num52)
                            num36 = 5;

                        if (num35 % num37 == num53)
                            num36 = 2;

                        if (num35 % num37 == num54)
                            num36 = 6;

                        if (num35 % num37 == num54)
                            num36 = 6;

                        if (num35 % num37 == num55)
                        {
                            num36 = 4;

                            // Adjust the upcoming Ethereal Lance attack depending on what random variable is chosen here.
                            calamityGlobalNPC.newAI[0] = Main.rand.Next(2);

                            // Sync the Calamity AI variables.
                            npc.SyncExtraAI();
                        }

                        if (num35 % num37 == num56)
                            num36 = 8;

                        if (num35 % num37 == num51)
                            num36 = 11;

                        if (num35 % num37 == num57)
                            num36 = 12;
                    }

                    npc.TargetClosest();
                    NPCAimedTarget targetData5 = npc.GetTargetData();
                    bool flag12 = false;
                    if (npc.AI_120_HallowBoss_IsGenuinelyEnraged() && !bossRush)
                    {
                        if (!Main.dayTime)
                            flag12 = true;

                        if (Main.dayTime && Main.time >= 53400.0)
                            flag12 = true;
                    }

                    // Despawn.
                    if (targetData5.Invalid || npc.Distance(targetData5.Center) > despawnDistanceGateValue || flag12)
                        num36 = 13;

                    // Set charge direction.
                    if (num36 == 8 && targetData5.Center.X > npc.Center.X)
                        num36 = 9;

                    if (num36 != 5 && num36 != 12)
                        npc.velocity = npc.DirectionFrom(targetData5.Center).SafeNormalize(Vector2.Zero).RotatedBy((float)Math.PI / 2f * (targetData5.Center.X > npc.Center.X).ToDirectionInt()) * 24f;

                    npc.ai[0] = num36;
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

                    Vector2 value29 = new Vector2(-55f, -30f);
                    NPCAimedTarget targetData11 = npc.GetTargetData();
                    Vector2 value30 = targetData11.Invalid ? npc.Center : targetData11.Center;
                    if (npc.Distance(value30 + rainbowStreakDistance) > movementDistanceGateValue)
                        npc.SimpleFlyMovement(npc.DirectionTo(value30 + rainbowStreakDistance).SafeNormalize(Vector2.Zero) * velocity, acceleration);

                    if (npc.ai[1] < 60f)
                        AI_120_HallowBoss_DoMagicEffect(npc.Center + value29, 1, Utils.GetLerpValue(0f, 60f, npc.ai[1], clamped: true), npc);

                    int num91 = CalamityWorld.LegendaryMode ? 1 : 2;
                    if ((int)npc.ai[1] % num91 == 0 && npc.ai[1] < 60f)
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
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + value29, rainbowStreakVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, npc.target, ai3);
                            if (Main.rand.NextBool(60) && CalamityWorld.LegendaryMode)
                            {
                                Main.projectile[proj].extraUpdates += 1;
                                Main.projectile[proj].netUpdate = true;
                            }
                        }

                        // Spawn extra homing Rainbow Streaks per player.
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int num92 = (int)(npc.ai[1] / num91);
                            for (int num93 = 0; num93 < Main.maxPlayers; num93++)
                            {
                                if (npc.Boss_CanShootExtraAt(num93, num92 % 3, 3, 2400f))
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + value29, rainbowStreakVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, num93, ai3);
                            }
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 36f : 45f) + 30f * lessTimeSpentPerPhaseMultiplier;
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
                        Vector2 value19 = targetData8.Invalid ? npc.Center : targetData8.Center;
                        if (npc.Distance(value19 + phase2AnimationDistance) > num2)
                            npc.SimpleFlyMovement(npc.DirectionTo(value19 + phase2AnimationDistance).SafeNormalize(Vector2.Zero) * scaleFactor, num);

                        if ((int)npc.ai[1] % 180 == 0)
                        {
                            Vector2 value20 = new Vector2(0f, -100f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), targetData8.Center + value20, Vector2.Zero, ProjectileID.HallowBossDeathAurora, num4, 0f, Main.myPlayer);
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
                    Vector2 value24 = targetData10.Invalid ? npc.Center : targetData10.Center;
                    if (npc.Distance(value24 + etherealLanceDistance) > movementDistanceGateValue)
                        npc.SimpleFlyMovement(npc.DirectionTo(value24 + etherealLanceDistance).SafeNormalize(Vector2.Zero) * velocity, acceleration);

                    int num82 = death ? 8 : 12;
                    if (npc.ai[1] % (dayTimeEnrage ? 2f : 3f) == 0f && npc.ai[1] < 100f)
                    {
                        int num83 = 1;
                        for (int n = 0; n < num83; n++)
                        {
                            int num85 = (int)(npc.ai[1] / (dayTimeEnrage ? 2f : 3f));
                            Vector2 vector8 = Vector2.UnitX.RotatedBy((float)Math.PI / (num82 * 2) + num85 * ((float)Math.PI / num82));
                            if (calamityGlobalNPC.newAI[0] == 0f)
                                vector8.X += (vector8.X > 0f) ? 0.5f : -0.5f;

                            vector8.Normalize();
                            float spawnDistance = 600f;

                            Vector2 center4 = targetData10.Center;
                            if (npc.Distance(center4) > 2400f)
                                continue;

                            if (Vector2.Dot(targetData10.Velocity.SafeNormalize(Vector2.UnitY), vector8) > 0f)
                                vector8 *= -1f;

                            int num87 = 90;
                            Vector2 value25 = center4 + targetData10.Velocity * num87;
                            Vector2 spawnLocation = center4 + vector8 * spawnDistance - targetData10.Velocity * 30f;
                            if (spawnLocation.Distance(center4) < spawnDistance)
                            {
                                Vector2 value26 = center4 - spawnLocation;
                                if (value26 == Vector2.Zero)
                                    value26 = vector8;

                                spawnLocation = center4 - Vector2.Normalize(value26) * spawnDistance;
                            }

                            int projectileType = ProjectileID.FairyQueenLance;
                            int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                            Vector2 v3 = value25 - spawnLocation;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromAI(), spawnLocation, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v3.ToRotation(), npc.ai[1] / 100f);

                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                continue;

                            // Spawn extra Ethereal Lances per player.
                            for (int num89 = 0; num89 < Main.maxPlayers; num89++)
                            {
                                if (!npc.Boss_CanShootExtraAt(num89, num85 % 3, 3, 2400f))
                                    continue;

                                Player player2 = Main.player[num89];
                                center4 = player2.Center;
                                if (Vector2.Dot(player2.velocity.SafeNormalize(Vector2.UnitY), vector8) > 0f)
                                    vector8 *= -1f;

                                Vector2 value27 = center4 + player2.velocity * num87;
                                spawnLocation = center4 + vector8 * spawnDistance - player2.velocity * 30f;
                                if (spawnLocation.Distance(center4) < spawnDistance)
                                {
                                    Vector2 value28 = center4 - spawnLocation;
                                    if (value28 == Vector2.Zero)
                                        value28 = vector8;

                                    spawnLocation = center4 - Vector2.Normalize(value28) * spawnDistance;
                                }

                                v3 = value27 - spawnLocation;
                                Projectile.NewProjectile(npc.GetSource_FromAI(), spawnLocation, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v3.ToRotation(), npc.ai[1] / 100f);
                            }
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 24f : 30f) + 20f * lessTimeSpentPerPhaseMultiplier;
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

                    Vector2 value16 = new Vector2(55f, -30f);
                    Vector2 value17 = npc.Center + value16;
                    if (npc.ai[1] < 42f)
                        AI_120_HallowBoss_DoMagicEffect(npc.Center + value16, 3, Utils.GetLerpValue(0f, 42f, npc.ai[1], clamped: true), npc);

                    NPCAimedTarget targetData7 = npc.GetTargetData();
                    Vector2 value18 = targetData7.Invalid ? npc.Center : targetData7.Center;
                    if (npc.Distance(value18 + everlastingRainbowDistance) > movementDistanceGateValue)
                        npc.SimpleFlyMovement(npc.DirectionTo(value18 + everlastingRainbowDistance).SafeNormalize(Vector2.Zero) * velocity, acceleration);

                    if (npc.ai[1] % 42f == 0f && npc.ai[1] < 42f)
                    {
                        float num64 = (float)Math.PI * 2f * Main.rand.NextFloat();
                        float totalProjectiles = CalamityWorld.LegendaryMode ? 30f : death ? (dayTimeEnrage ? 22f : 15f) : (dayTimeEnrage ? 18f : 13f);
                        int projIndex = 0;
                        bool inversePhase2SpreadPattern = Main.rand.NextBool();
                        for (float i = 0f; i < 1f; i += 1f / totalProjectiles)
                        {
                            int projectileType = ProjectileID.HallowBossLastingRainbow;
                            int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                            float num66 = i;
                            Vector2 spinningpoint = Vector2.UnitY.RotatedBy((float)Math.PI / 2f + (float)Math.PI * 2f * num66 + num64);

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
                                Projectile.NewProjectile(npc.GetSource_FromAI(), value17 + spinningpoint.RotatedBy(-MathHelper.PiOver2) * 30f, spinningpoint * initialVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, 0f, num66);

                            projIndex++;
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 36f : 45f) + 30f * lessTimeSpentPerPhaseMultiplier;
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

                    Vector2 value7 = new Vector2(0f, -100f);
                    Vector2 position = npc.Center + value7;

                    NPCAimedTarget targetData2 = npc.GetTargetData();
                    Vector2 value8 = targetData2.Invalid ? npc.Center : targetData2.Center;
                    if (npc.Distance(value8 + sunDanceDistance) > movementDistanceGateValue)
                        npc.SimpleFlyMovement(npc.DirectionTo(value8 + sunDanceDistance).SafeNormalize(Vector2.Zero) * velocity * 0.3f, acceleration * 0.7f);

                    if (npc.ai[1] % sunDanceGateValue == 0f && npc.ai[1] < totalSunDancePhaseTime)
                    {
                        int projectileType = ProjectileID.FairyQueenSunDance;
                        int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                        int num25 = (int)(npc.ai[1] / sunDanceGateValue);
                        int num26 = (targetData2.Center.X > npc.Center.X) ? 1 : 0;
                        float num27 = phase2 ? 8f : 6f;
                        float num28 = 1f / num27;
                        for (float num29 = 0f; num29 < 1f; num29 += num28)
                        {
                            float num30 = (num29 + num28 * 0.5f + num25 * num28 * 0.5f) % 1f;
                            float ai = (float)Math.PI * 2f * (num30 + num26);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromAI(), position, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, ai, npc.whoAmI);
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = 110f + 30f * lessTimeSpentPerPhaseMultiplier; // 112.5 is too little
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
                            Vector2 vector6 = Vector2.UnitY;
                            float num77 = 0.4f;
                            float num78 = 1.4f;
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
                                    vector6 = Vector2.UnitX;
                                    break;

                                case 1:
                                    center3 += new Vector2(lanceWallSize / 2f, lanceSpacing / 2f);
                                    lanceWallStartingPosition = new Vector2(0f, lanceWallSize);
                                    vector6 = -Vector2.UnitX;
                                    break;

                                case 2:
                                    center3 += new Vector2(0f - lanceWallSize, 0f - lanceWallSize) * num77;
                                    lanceWallStartingPosition = new Vector2(lanceWallSize * num78, 0f);
                                    vector6 = new Vector2(1f, 1f);
                                    break;

                                case 3:
                                    center3 += new Vector2(lanceWallSize * num77 + lanceSpacing / 2f, (0f - lanceWallSize) * num77);
                                    lanceWallStartingPosition = new Vector2((0f - lanceWallSize) * num78, 0f);
                                    vector6 = new Vector2(-1f, 1f);
                                    break;

                                case 4:
                                    center3 += new Vector2(0f - lanceWallSize, lanceWallSize) * num77;
                                    lanceWallStartingPosition = new Vector2(lanceWallSize * num78, 0f);
                                    vector6 = center3.DirectionTo(targetData9.Center);
                                    break;

                                case 5:
                                    center3 += new Vector2(lanceWallSize * num77 + lanceSpacing / 2f, lanceWallSize * num77);
                                    lanceWallStartingPosition = new Vector2((0f - lanceWallSize) * num78, 0f);
                                    vector6 = center3.DirectionTo(targetData9.Center);
                                    break;
                            }

                            int projectileType = ProjectileID.FairyQueenLance;
                            int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                            for (float i = 0f; i <= 1f; i += 1f / totalProjectiles)
                            {
                                Vector2 spawnLocation = center3 + lanceWallStartingPosition * (i - 0.5f) * (expertAttack ? 1f : 2f);
                                Vector2 v2 = vector6;
                                if (expertAttack)
                                {
                                    Vector2 value22 = targetData9.Velocity * 20f * i;
                                    Vector2 value23 = spawnLocation.DirectionTo(targetData9.Center + value22);
                                    v2 = Vector2.Lerp(vector6, value23, 0.75f).SafeNormalize(Vector2.UnitY);
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
                    extraPhaseTime = (dayTimeEnrage ? 24f : 30f) + 20f * lessTimeSpentPerPhaseMultiplier;
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

                    int num32 = (npc.ai[0] != 8f) ? 1 : (-1);

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
                        destination = (targetData3.Invalid ? npc.Center : targetData3.Center) + new Vector2(num32 * -800f, 0f);
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
                                int num92 = (int)((npc.ai[1] - chargeGateValue - 1f) / rainbowStreakGateValue);
                                for (int num93 = 0; num93 < Main.maxPlayers; num93++)
                                {
                                    if (npc.Boss_CanShootExtraAt(num93, num92 % 3, 3, 2400f))
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, rainbowStreakVelocity, projectileType, projectileDamage, 0f, Main.myPlayer, num93, ai3);
                                }
                            }
                        }

                        npc.velocity = Vector2.Lerp(value2: new Vector2(num32 * 70f, 0f), value1: npc.velocity, amount: 0.05f);

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
                    extraPhaseTime = (dayTimeEnrage ? 48f : 60f) * lessTimeSpentPerPhaseMultiplier;
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
                    Vector2 value11 = targetData6.Invalid ? npc.Center : targetData6.Center;
                    if (npc.Distance(value11 + etherealLanceDistance) > movementDistanceGateValue)
                        npc.SimpleFlyMovement(npc.DirectionTo(value11 + etherealLanceDistance).SafeNormalize(Vector2.Zero) * velocity, acceleration);

                    float etherealLanceGateValue = death ? 5f : 6f;
                    if (dayTimeEnrage)
                        etherealLanceGateValue -= 1f;

                    if (npc.ai[1] % etherealLanceGateValue == 0f && npc.ai[1] < 100f)
                    {
                        int num59 = 3;
                        for (int k = 0; k < num59; k++)
                        {
                            Vector2 vector3 = -targetData6.Velocity;
                            vector3.SafeNormalize(-Vector2.UnitY);
                            float spawnDistance = 100f + (k * 100f);

                            Vector2 center2 = targetData6.Center;
                            if (npc.Distance(center2) > 2400f)
                                continue;

                            int num61 = 90;
                            Vector2 value12 = center2 + targetData6.Velocity * num61;
                            Vector2 vector4 = center2 + vector3 * spawnDistance;
                            if (vector4.Distance(center2) < spawnDistance)
                            {
                                Vector2 value13 = center2 - vector4;
                                if (value13 == Vector2.Zero)
                                    value13 = vector3;

                                vector4 = center2 - Vector2.Normalize(value13) * spawnDistance;
                            }

                            int projectileType = ProjectileID.FairyQueenLance;
                            int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                            Vector2 v = value12 - vector4;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(npc.GetSource_FromAI(), vector4, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v.ToRotation(), npc.ai[1] / 100f);

                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                continue;

                            int num62 = (int)(npc.ai[1] / etherealLanceGateValue);
                            for (int l = 0; l < Main.maxPlayers; l++)
                            {
                                if (!npc.Boss_CanShootExtraAt(l, num62 % 3, 3, 2400f))
                                    continue;

                                Player player = Main.player[l];
                                vector3 = -player.velocity;
                                vector3.SafeNormalize(-Vector2.UnitY);
                                center2 = player.Center;
                                num61 = 90;
                                Vector2 value14 = center2 + player.velocity * num61;
                                vector4 = center2 + vector3 * spawnDistance;
                                if (vector4.Distance(center2) < spawnDistance)
                                {
                                    Vector2 value15 = center2 - vector4;
                                    if (value15 == Vector2.Zero)
                                        value15 = vector3;

                                    vector4 = center2 - Vector2.Normalize(value15) * spawnDistance;
                                }

                                v = value14 - vector4;
                                Projectile.NewProjectile(npc.GetSource_FromAI(), vector4, Vector2.Zero, projectileType, projectileDamage, 0f, Main.myPlayer, v.ToRotation(), npc.ai[1] / 100f);
                            }
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 24f : 30f) * lessTimeSpentPerPhaseMultiplier;
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

                    Vector2 value6 = new Vector2(-55f, -30f);

                    if (npc.ai[1] == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item165, npc.Center);
                        npc.velocity = new Vector2(0f, -12f);
                    }

                    npc.velocity *= 0.95f;

                    bool flag11 = npc.ai[1] < 60f && npc.ai[1] >= 10f;
                    if (flag11)
                        AI_120_HallowBoss_DoMagicEffect(npc.Center + value6, 1, Utils.GetLerpValue(0f, 60f, npc.ai[1], clamped: true), npc);

                    int num21 = 4;
                    if (dayTimeEnrage)
                        num21 -= 1;

                    float num22 = (npc.ai[1] - 10f) / 50f;
                    if ((int)npc.ai[1] % num21 == 0 && flag11)
                    {
                        int projectileType = ProjectileID.HallowBossRainbowStreak;
                        int projectileDamage = npc.GetProjectileDamage(projectileType) * projectileDamageMultiplier;

                        Vector2 vector = new Vector2(0f, death ? -24f : -22f).RotatedBy((float)Math.PI * 2f * num22);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + value6, vector, projectileType, projectileDamage, 0f, Main.myPlayer, npc.target, num22);
                            if (Main.rand.NextBool(15) && CalamityWorld.LegendaryMode)
                            {
                                Main.projectile[proj].extraUpdates += 1;
                                Main.projectile[proj].netUpdate = true;
                            }
                        }

                        // Spawn extra homing Rainbow Streaks per player.
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int num23 = (int)(npc.ai[1] % num21);
                            for (int j = 0; j < Main.maxPlayers; j++)
                            {
                                if (npc.Boss_CanShootExtraAt(j, num23 % 3, 3, 2400f))
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + value6, vector, projectileType, projectileDamage, 0f, Main.myPlayer, j, num22);
                            }
                        }
                    }

                    npc.ai[1] += 1f;
                    extraPhaseTime = (dayTimeEnrage ? 36f : 45f) + 30f * lessTimeSpentPerPhaseMultiplier;
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

                    bool flag7 = false;
                    bool flag8 = false;
                    if (!flag7)
                    {
                        if (npc.AI_120_HallowBoss_IsGenuinelyEnraged() && !bossRush)
                        {
                            if (!Main.dayTime)
                                flag8 = true;

                            if (Main.dayTime && Main.time >= 53400.0)
                                flag8 = true;
                        }

                        flag7 = flag7 || flag8;
                    }

                    if (!flag7)
                    {
                        bool flag9 = targetData.Invalid || npc.Distance(targetData.Center) > despawnDistanceGateValue;
                        flag7 = flag7 || flag9;
                    }

                    npc.alpha = Utils.Clamp(npc.alpha + flag7.ToDirectionInt() * 5, 0, 255);
                    bool flag10 = npc.alpha == 0 || npc.alpha == 255;

                    int num17 = 5;
                    for (int i = 0; i < num17; i++)
                    {
                        float num18 = MathHelper.Lerp(1.3f, 0.7f, npc.Opacity);
                        Color newColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f);
                        int num19 = Dust.NewDust(npc.position - npc.Size * 0.5f, npc.width * 2, npc.height * 2, 267, 0f, 0f, 0, newColor);
                        Main.dust[num19].position = npc.Center + Main.rand.NextVector2Circular(npc.width, npc.height);
                        Main.dust[num19].velocity *= Main.rand.NextFloat() * 0.8f;
                        Main.dust[num19].noGravity = true;
                        Main.dust[num19].scale = 0.9f + Main.rand.NextFloat() * 1.2f;
                        Main.dust[num19].fadeIn = 0.4f + Main.rand.NextFloat() * 1.2f * num18;
                        Main.dust[num19].velocity += Vector2.UnitY * -2f;
                        Main.dust[num19].scale = 0.35f;
                        if (num19 != 6000)
                        {
                            Dust dust = Dust.CloneDust(num19);
                            dust.scale /= 2f;
                            dust.fadeIn *= 0.85f;
                            dust.color = new Color(255, 255, 255, 255);
                        }
                    }

                    npc.ai[1] += 1f;
                    if (!(npc.ai[1] >= 20f && flag10))
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
            float num = 4f;
            float num2 = 1f;
            float fadeIn = 0f;
            float num3 = 0.5f;
            int num4 = 2;
            int num5 = 267;
            switch (effectType)
            {
                case 1:
                    num2 = 0.5f;
                    fadeIn = 2f;
                    num3 = 0f;
                    break;
                case 2:
                case 4:
                    num = 50f;
                    num2 = 0.5f;
                    fadeIn = 0f;
                    num3 = 0f;
                    num4 = 4;
                    break;
                case 3:
                    num = 30f;
                    num2 = 0.1f;
                    fadeIn = 2.5f;
                    num3 = 0f;
                    break;
                case 5:
                    if (progress == 0f)
                    {
                        num4 = 0;
                    }
                    else
                    {
                        num4 = 5;
                        num5 = Main.rand.Next(86, 92);
                    }
                    if (progress >= 1f)
                        num4 = 0;
                    break;
            }

            for (int i = 0; i < num4; i++)
            {
                Dust dust = Dust.NewDustPerfect(spot, num5, Main.rand.NextVector2CircularEdge(num, num) * (Main.rand.NextFloat() * (1f - num3) + num3), 0, Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f), (Main.rand.NextFloat() * 2f + 2f) * num2);
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
                            float num6 = 0f;
                            if (effectType == 4)
                                num6 = 0.5f;

                            dust.color = Main.hslToRgb((i / 5f + num6 + progress * 0.5f) % 1f, 1f, 0.5f);
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
