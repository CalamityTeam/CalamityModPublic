using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.CalamityAIs.CalamityBossAIs
{
    public static class BumblebirbAI
    {
        public static void VanillaBumblebirbAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            Player player = Main.player[npc.target];

            // Variables
            float rotationMult = 3f;
            float rotationAmt = 0.03f;
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Adjust slowing debuff immunity
            bool immuneToSlowingDebuffs = npc.ai[0] == 3f || npc.ai[0] == 3.1f || npc.ai[0] == 3.2f;
            npc.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TimeDistortion>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<GalvanicCorrosion>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

            // If target is outside the jungle for more than 5 seconds, enrage
            if (!player.ZoneJungle)
            {
                if (npc.localAI[1] < CalamityGlobalNPC.biomeEnrageTimerMax)
                    npc.localAI[1] += 1f;
            }
            else
                npc.localAI[1] = 0f;

            // If dragonfolly is off screen, enrage for the next couple attacks
            if (Vector2.Distance(player.Center, npc.Center) > 1200f)
                npc.localAI[2] = 2f;

            // Enrage scale
            float enrageScale = death ? 1.5f : 1f;
            if (npc.localAI[1] >= CalamityGlobalNPC.biomeEnrageTimerMax || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }

            if (npc.localAI[2] > 0f || bossRush)
                enrageScale += 1f;

            if (Main.getGoodWorld)
                enrageScale += 0.5f;

            if (enrageScale > 3f)
                enrageScale = 3f;

            // Despawn
            if (!player.active || player.dead || Vector2.Distance(player.Center, npc.Center) > 5600f)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];

                if (!player.active || player.dead || Vector2.Distance(player.Center, npc.Center) > 5600f)
                {
                    npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

                    if (npc.velocity.Y > 3f)
                        npc.velocity.Y = 3f;
                    npc.velocity.Y -= 0.2f;
                    if (npc.velocity.Y < -12f)
                        npc.velocity.Y = -12f;

                    if (npc.timeLeft > 60)
                        npc.timeLeft = 60;

                    if (npc.ai[0] != 0f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;

                        npc.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (npc.netSpam >= 10)
                            npc.netSpam = 9;
                    }

                    return;
                }
            }
            else if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases
            bool phase2 = lifeRatio < (revenge ? 0.75f : 0.5f);
            bool phase3 = lifeRatio < (death ? 0.4f : revenge ? 0.25f : 0.1f) && expertMode;

            float birbSpawnPhaseTimer = 180f;
            float newPhaseTimer = 180f;
            bool phaseSwitchPhase = (phase2 && calamityGlobalNPC.newAI[0] < newPhaseTimer && calamityGlobalNPC.newAI[2] != 1f) ||
                (phase3 && calamityGlobalNPC.newAI[1] < newPhaseTimer && calamityGlobalNPC.newAI[3] != 1f);

            calamityGlobalNPC.DR = (phaseSwitchPhase || npc.ai[0] == 5f || (enrageScale == 3f && !bossRush)) ? (bossRush ? 0.99f : 0.55f) : 0.1f;
            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = phaseSwitchPhase || npc.ai[0] == 5f || (enrageScale == 3f && !bossRush);

            int reducedSetDamage = (int)Math.Round(npc.defDamage * 0.5);

            if (phaseSwitchPhase)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                if (npc.velocity.X < 0f)
                    npc.direction = -1;
                else if (npc.velocity.X > 0f)
                    npc.direction = 1;

                npc.spriteDirection = npc.direction;
                npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

                if (phase3)
                {
                    calamityGlobalNPC.newAI[1] += 1f;

                    // Sound
                    if (calamityGlobalNPC.newAI[1] == newPhaseTimer - 60f)
                    {
                        float squawkpitch = Main.zenithWorld ? 1.3f : 0.25f;
                        SoundEngine.PlaySound(SoundID.DD2_BetsyScream with { Pitch = squawkpitch }, npc.Center);

                        if (Main.zenithWorld)
                        {
                            int spacing = 20;
                            int amt = 5;
                            SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, npc.Center - Vector2.UnitY * 300f);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < amt; i++)
                                {
                                    Vector2 fireFrom = new Vector2(npc.Center.X + (spacing * i) - (spacing * amt / 2), npc.Center.Y - 900f);
                                    Vector2 ai0 = npc.Center - fireFrom;
                                    float ai = Main.rand.Next(100);
                                    Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), npc.defDamage, 0f, Main.myPlayer, ai0.ToRotation(), ai);
                                }
                            }
                        }
                    }

                    if (calamityGlobalNPC.newAI[1] >= newPhaseTimer)
                    {
                        calamityGlobalNPC.newAI[1] = 0f;
                        calamityGlobalNPC.newAI[2] = 1f;
                        calamityGlobalNPC.newAI[3] = 1f;
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.SyncExtraAI();

                        npc.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (npc.netSpam >= 10)
                            npc.netSpam = 9;
                    }
                }
                else
                {
                    calamityGlobalNPC.newAI[0] += 1f;

                    // Sound
                    if (calamityGlobalNPC.newAI[0] == newPhaseTimer - 60f)
                    {
                        float squawkpitch = Main.zenithWorld ? 1.3f : 0.25f;
                        SoundEngine.PlaySound(SoundID.DD2_BetsyScream with { Pitch = squawkpitch }, npc.Center);

                        if (Main.zenithWorld)
                        {
                            int spacing = 20;
                            int amt = 3;
                            SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, npc.Center - Vector2.UnitY * 300f);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < amt; i++)
                                {
                                    Vector2 fireFrom = new Vector2(npc.Center.X + (spacing * i) - (spacing * amt / 2), npc.Center.Y - 900f);
                                    Vector2 ai0 = npc.Center - fireFrom;
                                    float ai = Main.rand.Next(100);
                                    Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), npc.defDamage, 0f, Main.myPlayer, ai0.ToRotation(), ai);
                                }
                            }
                        }
                    }

                    if (calamityGlobalNPC.newAI[0] >= newPhaseTimer)
                    {
                        calamityGlobalNPC.newAI[0] = 0f;
                        calamityGlobalNPC.newAI[2] = 1f;
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.SyncExtraAI();

                        npc.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (npc.netSpam >= 10)
                            npc.netSpam = 9;
                    }
                }

                Vector2 follyTargetDirection = player.Center - npc.Center;
                float follyTargetDistance = 4f + follyTargetDirection.Length() / 100f;
                float follyVelocityMult = 25f;
                follyTargetDirection.Normalize();
                follyTargetDirection *= follyTargetDistance;
                npc.velocity = (npc.velocity * (follyVelocityMult - 1f) + follyTargetDirection) / follyVelocityMult;
                return;
            }

            // Max spawn amount
            int maxBirbs = (CalamityWorld.LegendaryMode && revenge) ? 12 : revenge ? 3 : 2;

            // Variable for charging
            float chargeDistance = 600f;
            if (phase2)
                chargeDistance -= 50f;
            if (phase3)
                chargeDistance -= 50f;
            chargeDistance -= (enrageScale - 1f) * 100f;

            // Phase switch
            if (npc.ai[0] == 0f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                if (npc.Center.X < player.Center.X - 2f)
                    npc.direction = 1;
                if (npc.Center.X > player.Center.X + 2f)
                    npc.direction = -1;

                // Direction and rotation
                npc.spriteDirection = npc.direction;
                npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt * 1.25f) / 10f;

                // Fly to target if target is too far away, otherwise get close to target and then slow down
                Vector2 follyFlyTargetDirection = player.Center - npc.Center;
                follyFlyTargetDirection.Y -= 200f;
                if (follyFlyTargetDirection.Length() > 2800f)
                {
                    npc.TargetClosest();
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                }
                else if (follyFlyTargetDirection.Length() > 240f)
                {
                    float follyFlySpeed = 12f + (enrageScale - 1f) * 6f;
                    float follyFlyVelocityMult = 30f;
                    follyFlyTargetDirection.Normalize();
                    follyFlyTargetDirection *= follyFlySpeed;
                    npc.velocity = (npc.velocity * (follyFlyVelocityMult - 1f) + follyFlyTargetDirection) / follyFlyVelocityMult;
                }
                else if (npc.velocity.Length() > 2f)
                    npc.velocity *= 0.95f;
                else if (npc.velocity.Length() < 1f)
                    npc.velocity *= 1.05f;

                // Phase switch
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 30f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;

                    while (npc.ai[0] == 0f)
                    {
                        if (phase2)
                            npc.localAI[0] += 1f;

                        if (npc.localAI[0] >= (phase3 ? 7 : 9))
                        {
                            npc.TargetClosest();
                            npc.ai[0] = 5f;
                            npc.localAI[0] = 0f;

                            // Decrease the feather variable, feathers can be used again if it's at 0
                            if (npc.ai[3] > 0f)
                                npc.ai[3] -= 1f;

                            // Decrease enraged attacks by 1
                            if (npc.localAI[2] > 0f)
                                npc.localAI[2] -= 1f;

                            // Decrease amount of attacks until able to spawn small birbs again
                            if (npc.localAI[3] > 0f)
                                npc.localAI[3] -= 1f;
                        }
                        else
                        {
                            int follyAttackPicker = phase2 ? Main.rand.Next(2) + 1 : Main.rand.Next(3);
                            if (phase3)
                                follyAttackPicker = 1;

                            float featherVelocity = 2f + (enrageScale - 1f);
                            int type = ModContent.ProjectileType<RedLightningFeather>();
                            int damage = npc.GetProjectileDamage(type);

                            if (follyAttackPicker == 0 && npc.localAI[3] == 0f)
                            {
                                npc.TargetClosest();
                                npc.ai[0] = 2f;

                                // Decrease the feather variable, feathers can be used again if it's at 0
                                if (npc.ai[3] > 0f)
                                    npc.ai[3] -= 1f;

                                // Decrease enraged attacks by 1
                                if (npc.localAI[2] > 0f)
                                    npc.localAI[2] -= 1f;

                                // Birb will do at least 1 different attack before entering this phase again
                                npc.localAI[3] = 1f;
                            }
                            else if (follyAttackPicker == 1)
                            {
                                npc.TargetClosest();
                                npc.ai[0] = 3f;

                                // Decrease enraged attacks by 1
                                if (npc.localAI[2] > 0f)
                                    npc.localAI[2] -= 1f;

                                // Decrease amount of attacks until able to use other attacks again
                                if (npc.localAI[3] > 0f)
                                    npc.localAI[3] -= 1f;

                                if (phase2 && npc.ai[3] == 0f)
                                {
                                    npc.ai[3] = 3f;
                                    SoundEngine.PlaySound(SoundID.Item102, player.Center);

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int totalProjectiles = 40;
                                        float radians = MathHelper.TwoPi / totalProjectiles;
                                        int distance = 800;
                                        bool spawnRight = player.velocity.X > 0f;
                                        for (int i = 0; i < totalProjectiles; i++)
                                        {
                                            if (Main.getGoodWorld)
                                            {
                                                if (i >= (int)(totalProjectiles * 0.125) && i <= (int)(totalProjectiles * 0.375))
                                                {
                                                    Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
                                                    Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
                                                    Projectile.NewProjectile(npc.GetSource_FromAI(), spawnVector, velocity, type, damage, 0f, Main.myPlayer);
                                                }
                                                if (i >= (int)(totalProjectiles * 0.625) && i <= (int)(totalProjectiles * 0.875))
                                                {
                                                    Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
                                                    Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
                                                    Projectile.NewProjectile(npc.GetSource_FromAI(), spawnVector, velocity, type, damage, 0f, Main.myPlayer);
                                                }
                                            }
                                            else
                                            {
                                                if (spawnRight)
                                                {
                                                    if (i >= (int)(totalProjectiles * 0.125) && i <= (int)(totalProjectiles * 0.375))
                                                    {
                                                        Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
                                                        Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
                                                        Projectile.NewProjectile(npc.GetSource_FromAI(), spawnVector, velocity, type, damage, 0f, Main.myPlayer);
                                                    }
                                                }
                                                else
                                                {
                                                    if (i >= (int)(totalProjectiles * 0.625) && i <= (int)(totalProjectiles * 0.875))
                                                    {
                                                        Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
                                                        Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
                                                        Projectile.NewProjectile(npc.GetSource_FromAI(), spawnVector, velocity, type, damage, 0f, Main.myPlayer);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Decrease the feather variable, feathers can be used again if it's at 0
                                    if (npc.ai[3] > 0f)
                                        npc.ai[3] -= 1f;
                                }
                            }
                            else if (NPC.CountNPCS(ModContent.NPCType<Bumblefuck2>()) < maxBirbs && npc.localAI[3] == 0f)
                            {
                                npc.TargetClosest();
                                npc.ai[0] = 4f;

                                // Birb will do at least 2 different attacks before entering this phase again
                                npc.localAI[3] = 2f;

                                // Decrease enraged attacks by 1
                                if (npc.localAI[2] > 0f)
                                    npc.localAI[2] -= 1f;

                                if (npc.ai[3] == 0f)
                                {
                                    npc.ai[3] = 3f;
                                    SoundEngine.PlaySound(SoundID.Item102, player.Center);

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int totalProjectiles = phase2 ? 40 : 48;

                                        if (Main.getGoodWorld)
                                            totalProjectiles *= 2;

                                        float radians = MathHelper.TwoPi / totalProjectiles;
                                        int distance = phase2 ? 1200 : 1320;

                                        if (Main.getGoodWorld)
                                            distance *= 2;

                                        bool spawnRight = player.velocity.X > 0f;
                                        for (int i = 0; i < totalProjectiles; i++)
                                        {
                                            if (spawnRight)
                                            {
                                                if (i >= totalProjectiles / 2)
                                                    break;

                                                Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
                                                Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
                                                Projectile.NewProjectile(npc.GetSource_FromAI(), spawnVector, velocity, type, damage, 0f, Main.myPlayer);
                                            }
                                            else
                                            {
                                                if (i >= totalProjectiles / 2)
                                                {
                                                    Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
                                                    Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
                                                    Projectile.NewProjectile(npc.GetSource_FromAI(), spawnVector, velocity, type, damage, 0f, Main.myPlayer);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Decrease the feather variable, feathers can be used again if it's at 0
                                    if (npc.ai[3] > 0f)
                                        npc.ai[3] -= 1f;
                                }
                            }
                        }
                    }

                    npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;

                    npc.SyncExtraAI();
                }
            }

            // Fly to target
            else if (npc.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                if (npc.velocity.X < 0f)
                    npc.direction = -1;
                else if (npc.velocity.X > 0f)
                    npc.direction = 1;

                npc.spriteDirection = npc.direction;
                npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

                Vector2 follyTargetDirection = player.Center - npc.Center;
                if (follyTargetDirection.Length() < 800f)
                {
                    npc.TargetClosest();
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;

                    npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;

                    npc.SyncExtraAI();
                }

                float velocity = 14f + (enrageScale - 1f) * 4f;
                float follyTargetDistance = velocity + follyTargetDirection.Length() / 100f;
                float follyVelocityMult = 25f;
                follyTargetDirection.Normalize();
                follyTargetDirection *= follyTargetDistance;
                npc.velocity = (npc.velocity * (follyVelocityMult - 1f) + follyTargetDirection) / follyVelocityMult;
            }

            // Fly towards target quickly
            else if (npc.ai[0] == 2f)
            {
                // Set reduced damage
                npc.damage = reducedSetDamage;

                if (npc.target < 0 || !player.active || player.dead)
                {
                    npc.TargetClosest();
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;

                    npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;

                    npc.SyncExtraAI();
                }

                if (player.Center.X - 10f < npc.Center.X)
                    npc.direction = -1;
                else if (player.Center.X + 10f > npc.Center.X)
                    npc.direction = 1;

                npc.spriteDirection = npc.direction;
                npc.rotation = (npc.rotation * rotationMult * 0.5f + npc.velocity.X * rotationAmt * 1.25f) / 5f;

                Vector2 follyQuickFlyTargetDirection = player.Center - npc.Center;
                follyQuickFlyTargetDirection.Y -= 20f;
                npc.ai[2] += 0.0222222228f;
                if (expertMode)
                    npc.ai[2] += 0.0166666675f;

                float velocity = 8f + (enrageScale - 1f) * 2f;
                float follyQuickFlySpeed = velocity + npc.ai[2] + follyQuickFlyTargetDirection.Length() / 120f;
                if (CalamityWorld.LegendaryMode && revenge)
                    follyQuickFlySpeed *= 2f;

                float follyQuickFlyVelMult = 20f;
                follyQuickFlyTargetDirection.Normalize();
                follyQuickFlyTargetDirection *= follyQuickFlySpeed;
                npc.velocity = (npc.velocity * (follyQuickFlyVelMult - 1f) + follyQuickFlyTargetDirection) / follyQuickFlyVelMult;

                npc.ai[1] += 1f;
                if (npc.ai[1] >= ((CalamityWorld.LegendaryMode && revenge) ? 90f : 180f))
                {
                    npc.TargetClosest();
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;

                    npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;

                    npc.SyncExtraAI();
                }
            }

            // Line up charge
            else if (npc.ai[0] == 3f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                if (npc.velocity.X < 0f)
                    npc.direction = -1;
                else
                    npc.direction = 1;

                npc.spriteDirection = npc.direction;
                npc.rotation = (npc.rotation * rotationMult * 0.5f + npc.velocity.X * rotationAmt * 0.85f) / 5f;

                Vector2 follyLineUpTargetDirection = player.Center - npc.Center;
                follyLineUpTargetDirection.Y -= 12f;
                if (npc.Center.X > player.Center.X)
                    follyLineUpTargetDirection.X += chargeDistance;
                else
                    follyLineUpTargetDirection.X -= chargeDistance;

                float verticalDistanceGateValue = (phase3 ? 100f : 20f) + (enrageScale - 1f) * 20f;
                if (Math.Abs(npc.Center.X - player.Center.X) > chargeDistance - 50f && Math.Abs(npc.Center.Y - player.Center.Y) < verticalDistanceGateValue)
                {
                    npc.ai[0] = 3.1f;
                    npc.ai[1] = 0f;

                    npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;

                    npc.SyncExtraAI();
                }

                npc.ai[1] += 0.0333333351f;
                float velocity = 16f + (enrageScale - 1f) * 4f;
                float follyLineUpSpeed = velocity + npc.ai[1];
                float follyLineUpVelMult = 4f;
                follyLineUpTargetDirection.Normalize();
                follyLineUpTargetDirection *= follyLineUpSpeed;
                npc.velocity = (npc.velocity * (follyLineUpVelMult - 1f) + follyLineUpTargetDirection) / follyLineUpVelMult;
            }

            // Prepare to charge
            else if (npc.ai[0] == 3.1f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                npc.rotation = (npc.rotation * rotationMult * 0.5f + npc.velocity.X * rotationAmt * 0.85f) / 5f;

                Vector2 follyChargePrepareTargetDirection = player.Center - npc.Center;
                follyChargePrepareTargetDirection.Y -= 12f;
                float follyChargePrepareSpeed = 28f + (enrageScale - 1f) * 4f;
                float follyChargePrepareVelMult = 8f;
                follyChargePrepareTargetDirection.Normalize();
                follyChargePrepareTargetDirection *= follyChargePrepareSpeed;
                npc.velocity = (npc.velocity * (follyChargePrepareVelMult - 1f) + follyChargePrepareTargetDirection) / follyChargePrepareVelMult;

                if (npc.velocity.X < 0f)
                    npc.direction = -1;
                else
                    npc.direction = 1;

                npc.spriteDirection = npc.direction;

                npc.ai[1] += 1f;
                if (npc.ai[1] > 10f)
                {
                    // Set damage
                    npc.damage = npc.defDamage;

                    npc.velocity = follyChargePrepareTargetDirection;

                    if (npc.velocity.X < 0f)
                        npc.direction = -1;
                    else
                        npc.direction = 1;

                    npc.ai[0] = 3.2f;
                    npc.ai[1] = npc.direction;

                    npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;

                    npc.SyncExtraAI();
                }
            }

            // Charge
            else if (npc.ai[0] == 3.2f)
            {
                // Set damage
                npc.damage = npc.defDamage;

                npc.ai[2] += 0.0333333351f;
                float velocity = 28f + (enrageScale - 1f) * 4f;
                npc.velocity.X = (velocity + npc.ai[2]) * npc.ai[1];

                if ((npc.ai[1] > 0f && npc.Center.X > player.Center.X + (chargeDistance - 140f)) || (npc.ai[1] < 0f && npc.Center.X < player.Center.X - (chargeDistance - 140f)))
                {
                    if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.TargetClosest();
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;

                        npc.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (npc.netSpam >= 10)
                            npc.netSpam = 9;
                    }
                    else if (Math.Abs(npc.Center.X - player.Center.X) > chargeDistance + 200f)
                    {
                        npc.TargetClosest();
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;

                        npc.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (npc.netSpam >= 10)
                            npc.netSpam = 9;
                    }
                }

                npc.rotation = (npc.rotation * rotationMult * 0.5f + npc.velocity.X * rotationAmt * 0.85f) / 5f;
            }

            // Birb spawn
            else if (npc.ai[0] == 4f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                if (npc.ai[1] == 0f)
                {
                    Vector2 destination2 = player.Center + new Vector2(0f, -200f);
                    Vector2 desiredVelocity2 = npc.SafeDirectionTo(destination2, -Vector2.UnitY) * 18f;
                    npc.SimpleFlyMovement(desiredVelocity2, 1.5f);

                    if (npc.velocity.X < 0f)
                        npc.direction = -1;
                    else
                        npc.direction = 1;

                    npc.spriteDirection = npc.direction;

                    npc.ai[2] += 1f;
                    if (npc.Distance(player.Center) < 600f || npc.ai[2] >= 180f)
                    {
                        npc.ai[1] = 1f;

                        npc.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (npc.netSpam >= 10)
                            npc.netSpam = 9;
                    }
                }
                else
                {
                    if (npc.ai[1] < 90f)
                        npc.velocity *= 0.95f;
                    else
                        npc.velocity *= 0.98f;

                    if (npc.ai[1] == 90f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y /= 3f;

                        npc.velocity.Y -= 3f;
                    }

                    // Sound
                    if (npc.ai[1] == birbSpawnPhaseTimer - 60f)
                    {
                        float squawkpitch = Main.zenithWorld ? 1.3f : 0.25f;
                        SoundEngine.PlaySound(SoundID.DD2_BetsyScream with { Pitch = squawkpitch }, npc.Center);

                        if (Main.zenithWorld)
                        {
                            int spacing = 30;
                            int amt = 3;
                            SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, npc.Center - Vector2.UnitY * 300f);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < amt; i++)
                                {
                                    Vector2 fireFrom = new Vector2(npc.Center.X + (spacing * i) - (spacing * amt / 2), npc.Center.Y - 900f);
                                    Vector2 ai0 = npc.Center - fireFrom;
                                    float ai = Main.rand.Next(100);
                                    Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), npc.damage, 0f, Main.myPlayer, ai0.ToRotation(), ai);
                                }
                            }
                        }
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        bool gfbSpawnFlag = CalamityWorld.LegendaryMode && revenge && (npc.ai[1] == 145f || npc.ai[1] == 150f || npc.ai[1] == 160f || npc.ai[1] == 165f);
                        bool spawnFlag = NPC.CountNPCS(ModContent.NPCType<Bumblefuck2>()) < maxBirbs && (npc.ai[1] == 140f || (revenge && npc.ai[1] == 155f) || npc.ai[1] == 170f || gfbSpawnFlag);
                        if (spawnFlag)
                        {
                            Vector2 follySpawnCenter = npc.Center + (MathHelper.TwoPi * Main.rand.NextFloat()).ToRotationVector2() * new Vector2(2f, 1f) * 50f * (0.6f + Main.rand.NextFloat() * 0.4f);
                            if (Vector2.Distance(follySpawnCenter, player.Center) > 150f)
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)follySpawnCenter.X, (int)follySpawnCenter.Y, ModContent.NPCType<Bumblefuck2>(), npc.whoAmI);

                            npc.netUpdate = true;

                            // Prevent netUpdate from being blocked by the spam counter.
                            if (npc.netSpam >= 10)
                                npc.netSpam = 9;
                        }
                    }

                    npc.ai[1] += 1f;
                }

                if (npc.ai[1] >= birbSpawnPhaseTimer)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();

                    npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;
                }
            }

            // Spit homing aura sphere
            else if (npc.ai[0] == 5f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Velocity
                npc.velocity *= 0.98f;
                npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

                // Play sound
                float aiGateValue = 120f;
                if (npc.ai[1] == aiGateValue - 30f)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, npc.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 follySpawnCenter = npc.rotation.ToRotationVector2() * (Vector2.UnitX * npc.direction) * (npc.width + 20) / 2f + npc.Center;
                        float ai0 = (phase3 ? 2f : 0f) + (enrageScale - 1f);
                        if (ai0 > 3f)
                            ai0 = 3f;

                        Projectile.NewProjectile(npc.GetSource_FromAI(), follySpawnCenter.X, follySpawnCenter.Y, 0f, 0f, ModContent.ProjectileType<BirbAuraFlare>(), 0, 0f, Main.myPlayer, ai0, npc.target + 1);

                        npc.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (npc.netSpam >= 10)
                            npc.netSpam = 9;
                    }

                    if (Main.zenithWorld)
                    {
                        int spacing = 30;
                        int amt = 3;
                        SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, npc.Center - Vector2.UnitY * 300f);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < amt; i++)
                            {
                                Vector2 fireFrom = new Vector2(npc.Center.X + (spacing * i) - (spacing * amt / 2), npc.Center.Y - 900f);
                                Vector2 ai0 = npc.Center - fireFrom;
                                float ai = Main.rand.Next(100);
                                Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                                Projectile.NewProjectile(npc.GetSource_FromAI(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), npc.damage, 0f, Main.myPlayer, ai0.ToRotation(), ai);
                            }
                        }
                    }
                }

                npc.ai[1] += 1f;
                if (npc.ai[1] >= aiGateValue)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;

                    npc.netUpdate = true;

                    // Prevent netUpdate from being blocked by the spam counter.
                    if (npc.netSpam >= 10)
                        npc.netSpam = 9;
                }
            }
        }

        public static void VanillaBumblebirb2AI(NPC npc, Mod mod, bool bossMinion)
        {
            Player player = Main.player[npc.target];

            float rotationMult = 4f;
            float rotationAmt = 0.04f;

            if (Vector2.Distance(player.Center, npc.Center) > 5600f)
            {
                if (npc.timeLeft > 5)
                    npc.timeLeft = 5;
            }

            npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt * 1.25f) / 10f;

            if (npc.ai[0] == 0f || npc.ai[0] == 1f)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type)
                    {
                        Vector2 otherSwarmerDirection = Main.npc[i].Center - npc.Center;
                        if (otherSwarmerDirection.Length() < (npc.width + npc.height))
                        {
                            otherSwarmerDirection.Normalize();
                            otherSwarmerDirection *= -0.1f;
                            npc.velocity += otherSwarmerDirection;
                            NPC nPC6 = Main.npc[i];
                            nPC6.velocity -= otherSwarmerDirection;
                        }
                    }
                }
            }

            if (npc.target < 0 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest(true);
                Vector2 swarmerTargetDist = Main.player[npc.target].Center - npc.Center;
                if (Main.player[npc.target].dead || swarmerTargetDist.Length() > (bossMinion ? 5600f : 2800f))
                    npc.ai[0] = -1f;
            }
            else
            {
                Vector2 swarmerCatchUpTargetDist = Main.player[npc.target].Center - npc.Center;
                if (npc.ai[0] > 1f && swarmerCatchUpTargetDist.Length() > 3600f)
                    npc.ai[0] = 1f;
            }

            if (npc.ai[0] == -1f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                Vector2 swarmerDespawnVelMult = new Vector2(0f, -8f);
                npc.velocity = (npc.velocity * 21f + swarmerDespawnVelMult) / 10f;
                return;
            }

            if (npc.ai[0] == 0f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                npc.TargetClosest(true);
                npc.spriteDirection = npc.direction;

                Vector2 swarmerIdleTargetDist = Main.player[npc.target].Center - npc.Center;
                if (swarmerIdleTargetDist.Length() > 2800f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                }
                else if (swarmerIdleTargetDist.Length() > 400f)
                {
                    float swarmerIdleSpeed = (bossMinion ? 9f : 7f) + swarmerIdleTargetDist.Length() / 100f + npc.ai[1] / 15f;
                    swarmerIdleTargetDist.Normalize();
                    swarmerIdleTargetDist *= swarmerIdleSpeed;
                    npc.velocity = (npc.velocity * 29f + swarmerIdleTargetDist) / 30f;
                    if (Main.getGoodWorld && !Main.zenithWorld)
                        npc.velocity *= 1.15f;
                }
                else if (npc.velocity.Length() > 2f)
                    npc.velocity *= 0.95f;
                else if (npc.velocity.Length() < 1f)
                    npc.velocity *= 1.05f;

                npc.ai[1] += 1f;
                if (npc.ai[1] >= (bossMinion ? 90f : 105f))
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 2f;
                }
            }
            else
            {
                if (npc.ai[0] == 1f)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    if (npc.target < 0 || !Main.player[npc.target].active || Main.player[npc.target].dead)
                        npc.TargetClosest(true);

                    if (npc.velocity.X < 0f)
                        npc.direction = -1;
                    else if (npc.velocity.X > 0f)
                        npc.direction = 1;

                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

                    Vector2 swarmerChargeTargetDist = Main.player[npc.target].Center - npc.Center;
                    if (swarmerChargeTargetDist.Length() < 800f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }

                    npc.ai[2] += 0.0166666675f;
                    float swarmerChargeSpeed = (bossMinion ? 12f : 9f) + npc.ai[2] + swarmerChargeTargetDist.Length() / 150f;
                    float swarmerChargeVelMult = 25f;
                    swarmerChargeTargetDist.Normalize();
                    swarmerChargeTargetDist *= swarmerChargeSpeed;
                    npc.velocity = (npc.velocity * (swarmerChargeVelMult - 1f) + swarmerChargeTargetDist) / swarmerChargeVelMult;
                    if (Main.getGoodWorld && !Main.zenithWorld)
                        npc.velocity *= 1.15f;

                    npc.netSpam = 5;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);

                    return;
                }

                if (npc.ai[0] == 2f)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    if (npc.velocity.X < 0f)
                        npc.direction = -1;
                    else if (npc.velocity.X > 0f)
                        npc.direction = 1;

                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * rotationMult * 0.75f + npc.velocity.X * rotationAmt * 1.25f) / 8f;

                    Vector2 swarmerDecelerateTargetDist = Main.player[npc.target].Center - npc.Center;
                    swarmerDecelerateTargetDist.Y -= 8f;
                    float swarmerDecelerateSpeed = bossMinion ? 18f : 14f;
                    float swarmerDecelerateVelMult = 8f;
                    swarmerDecelerateTargetDist.Normalize();
                    swarmerDecelerateTargetDist *= swarmerDecelerateSpeed;
                    npc.velocity = (npc.velocity * (swarmerDecelerateVelMult - 1f) + swarmerDecelerateTargetDist) / swarmerDecelerateVelMult;
                    if (Main.getGoodWorld && !Main.zenithWorld)
                        npc.velocity *= 1.15f;

                    if (npc.velocity.X < 0f)
                        npc.direction = -1;
                    else
                        npc.direction = 1;

                    npc.spriteDirection = npc.direction;

                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 10f)
                    {
                        // Set damage
                        npc.damage = npc.defDamage;

                        npc.velocity = swarmerDecelerateTargetDist;
                        if (Main.getGoodWorld && !Main.zenithWorld)
                            npc.velocity *= 1.15f;

                        if (npc.velocity.X < 0f)
                            npc.direction = -1;
                        else
                            npc.direction = 1;

                        npc.ai[0] = 2.1f;
                        npc.ai[1] = 0f;
                    }
                }
                else if (npc.ai[0] == 2.1f)
                {
                    // Set damage
                    npc.damage = npc.defDamage;

                    if (npc.velocity.X < 0f)
                        npc.direction = -1;
                    else if (npc.velocity.X > 0f)
                        npc.direction = 1;

                    npc.spriteDirection = npc.direction;

                    npc.velocity *= 1.01f;

                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 30f)
                    {
                        if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                        {
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            return;
                        }

                        if (npc.ai[1] > 60f)
                        {
                            npc.ai[0] = 1f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                    }
                }
            }
        }
    }
}
