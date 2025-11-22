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
    public static class CeaselessVoidAI
    {
        public static void VanillaCeaselessVoidAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            CalamityGlobalNPC.voidBoss = npc.whoAmI;

            // Percent life remaining
            double lifeRatio = npc.life / (double)npc.lifeMax;

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Phases
            bool phase2 = lifeRatio <= 0.7;
            bool phase3 = lifeRatio <= 0.4;
            bool phase4 = lifeRatio <= 0.1;
            bool theBigSucc = npc.life / (double)npc.lifeMax <= 0.1;
            bool succSoHardThatYouDie = npc.life / (double)npc.lifeMax <= 0.005;

            // Spawn Dark Energies
            int darkEnergyAmt = death ? 6 : revenge ? 5 : expertMode ? 4 : 3;
            if (phase2)
                darkEnergyAmt += 1;
            if (phase3)
                darkEnergyAmt += 1;
            if (phase4)
                darkEnergyAmt += 1;

            if (Main.getGoodWorld)
                darkEnergyAmt *= 2;

            // Spawn a few Dark Energies as soon as the fight starts
            int spacing = 360 / darkEnergyAmt;
            int distance2 = 10;
            if (npc.ai[2] == 0f)
            {
                npc.ai[2] = 1f;
                for (int i = 0; i < darkEnergyAmt; i++)
                {
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(npc.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 0f, 0f, 0f);
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(npc.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 1f, 0f, 0f);
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(npc.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 2f, 0f, 0f);
                }
                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(0) * distance2)), (int)(npc.Center.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 0.5f, 0f, 0f);
                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(0) * distance2)), (int)(npc.Center.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 1.5f, 0f, 0f);
            }

            // If there are any Dark Energies alive, change AI and don't take damage
            bool anyDarkEnergies = NPC.AnyNPCs(ModContent.NPCType<DarkEnergy>());
            bool movingDuringSuccPhase = npc.ai[3] == 0f;
            npc.dontTakeDamage = anyDarkEnergies || theBigSucc || movingDuringSuccPhase;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            Player player = Main.player[npc.target];

            // Speed enrage
            bool moveVeryFast = Vector2.Distance(npc.Center, player.Center) > 960f || (!player.ZoneDungeon && !bossRush && player.position.Y < Main.worldSurface * 16.0);

            // Despawn
            if (!player.active || player.dead || Vector2.Distance(player.Center, npc.Center) > 5600f)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead || Vector2.Distance(player.Center, npc.Center) > 5600f)
                {
                    if (npc.velocity.Y > 3f)
                        npc.velocity.Y = 3f;
                    npc.velocity.Y -= 0.1f;
                    if (npc.velocity.Y < -12f)
                        npc.velocity.Y = -12f;

                    if (npc.timeLeft > 60)
                        npc.timeLeft = 60;

                    return;
                }
            }
            else if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            float tileEnrageMult = (CalamityWorld.LegendaryMode && revenge) ? 1.5f : bossRush ? 1.375f : 1f;
            npc.Calamity().CurrentlyEnraged = tileEnrageMult > 1f && !bossRush;

            // Set AI variable to be used by Dark Energies
            npc.ai[1] = tileEnrageMult;

            // Increase projectile fire rate based on number of nearby active tiles
            float projectileFireRateMultiplier = MathHelper.Lerp(0.5f, 1.5f, 1f - ((tileEnrageMult - 1f) / 0.5f));

            // Succ attack
            if (!anyDarkEnergies)
            {
                // This is here because it's used in multiple places
                float suckDistance = (CalamityWorld.LegendaryMode && revenge) ? 2400f : bossRush ? 1920f : death ? 1600f : revenge ? 1440f : expertMode ? 1280f : 1040f;

                // Move closer to the target before trying to succ
                if (movingDuringSuccPhase)
                {
                    // Avoid cheap bullshit
                    npc.damage = 0;

                    if (Vector2.Distance(npc.Center, player.Center) > 320f || !Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
                        Movement(true);
                    else
                        npc.ai[3] = 1f;
                }
                else
                {
                    // Set damage
                    npc.damage = npc.defDamage;

                    // Use this to generate more and more dust in final phase
                    float finalPhaseDustRatio = 1f;
                    if (succSoHardThatYouDie)
                    {
                        finalPhaseDustRatio = 5f;
                    }
                    else if (theBigSucc)
                    {
                        float amount = (10f - (float)(npc.life / (double)npc.lifeMax) * 100f) / 10f;
                        finalPhaseDustRatio += MathHelper.Lerp(0f, 2f, amount);
                    }

                    // Slow down
                    if (npc.velocity.Length() > 0.5f)
                        npc.velocity *= 0.8f;
                    else
                        npc.velocity = Vector2.Zero;

                    // Move towards target again if they get outside the succ radius
                    float moveCloserGateValue = suckDistance * 0.75f;
                    if (Vector2.Distance(npc.Center, player.Center) > moveCloserGateValue)
                        npc.ai[3] = 0f;

                    // Ceaseless Void sucks in dark energy in different patterns
                    // This attack also sucks in all players that are within reach of the succ
                    int dustRings = 3;
                    for (int h = 0; h < dustRings; h++)
                    {
                        float distanceDivisor = h + 1f;
                        float dustDistance = suckDistance / distanceDivisor;
                        int numDust = (int)(0.1f * MathHelper.TwoPi * dustDistance);
                        float angleIncrement = MathHelper.TwoPi / numDust;
                        Vector2 dustOffset = new Vector2(dustDistance, 0f);
                        dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);

                        int var = (int)(dustDistance / finalPhaseDustRatio);
                        float dustVelocity = 24f / distanceDivisor * finalPhaseDustRatio;
                        for (int i = 0; i < numDust; i++)
                        {
                            if (Main.rand.NextBool(var))
                            {
                                dustOffset = dustOffset.RotatedBy(angleIncrement);
                                int dust = Dust.NewDust(npc.Center, 1, 1, ModContent.DustType<CeaselessDust>());
                                Main.dust[dust].position = npc.Center + dustOffset;
                                Main.dust[dust].fadeIn = 1f;
                                Main.dust[dust].velocity = Vector2.Normalize(npc.Center - Main.dust[dust].position) * dustVelocity;
                                Main.dust[dust].scale = 3f - h;
                            }
                        }
                    }

                    float succPower = 0.125f + finalPhaseDustRatio * 0.125f;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        float distance = Vector2.Distance(Main.player[i].Center, npc.Center);
                        if (distance < suckDistance && Main.player[i].grappling[0] == -1)
                        {
                            if (Collision.CanHit(npc.Center, 1, 1, Main.player[i].Center, 1, 1))
                            {
                                float distanceRatio = distance / suckDistance;
                                float multiplier = 1f - distanceRatio;

                                if (Main.player[i].Center.X < npc.Center.X)
                                    Main.player[i].velocity.X += succPower * multiplier;
                                else
                                    Main.player[i].velocity.X -= succPower * multiplier;
                            }
                        }
                    }

                    // Slowly die in final phase and then implode
                    // This phase lasts 20 seconds
                    if (theBigSucc && calamityGlobalNPC.newAI[1] % 60f == 0f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int damageAmt = npc.lifeMax / 200;
                            npc.life -= damageAmt;
                            npc.HealEffect(-damageAmt, true);
                        }

                        if (npc.life <= ((npc.lifeMax / 200) * 5) && !npc.ModNPC<CeaselessVoid.CeaselessVoid>().playedbuildsound)
                        {
                            SoundEngine.PlaySound(CeaselessVoid.CeaselessVoid.BuildupSound, npc.Center);
                            npc.ModNPC<CeaselessVoid.CeaselessVoid>().playedbuildsound = true;
                        }

                        if (npc.life <= 0)
                        {
                            npc.life = 0;
                            npc.HitEffect();
                            npc.checkDead();
                        }

                        npc.netUpdate = true;
                    }

                    // Beam Portals
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (calamityGlobalNPC.newAI[1] == 0f)
                        {
                            int numBeamPortals = bossRush ? 4 : revenge ? 3 : 2;
                            float degrees = 360 / numBeamPortals;
                            float beamPortalDistance = bossRush ? 360f : death ? 400f : revenge ? 420f : expertMode ? 440f : 480f;
                            int type = ModContent.ProjectileType<DoGBeamPortal>();
                            int damage = npc.GetProjectileDamage(type);
                            for (int i = 0; i < numBeamPortals; i++)
                            {
                                float ai1 = i * degrees;
                                Projectile.NewProjectile(npc.GetSource_FromAI(), player.Center.X + (float)(Math.Sin(i * degrees) * beamPortalDistance), player.Center.Y + (float)(Math.Cos(i * degrees) * beamPortalDistance), 0f, 0f, type, damage, 0f, Main.myPlayer, ai1, 0f);
                            }
                        }
                    }

                    // Use this timer to lessen Dark Energy projectile rate of fire while Beam Portals are active
                    float beamPortalTimeLeft = 600f;
                    bool summonLessDarkEnergies = false;
                    if (calamityGlobalNPC.newAI[1] < beamPortalTimeLeft)
                    {
                        calamityGlobalNPC.newAI[1] += 1f;
                        summonLessDarkEnergies = true;
                    }
                    else if (theBigSucc)
                        calamityGlobalNPC.newAI[1] += 1f;

                    // Suck in Dark Energy projectiles from far away
                    calamityGlobalNPC.newAI[3] += 1f;
                    float darkEnergySpiralGateValue = (summonLessDarkEnergies ? 24f : 12f) * projectileFireRateMultiplier;
                    if (calamityGlobalNPC.newAI[3] >= darkEnergySpiralGateValue)
                    {
                        calamityGlobalNPC.newAI[3] = 0f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<DarkEnergyBall>();
                            int damage = npc.GetProjectileDamage(type);
                            bool normalSpread = npc.localAI[0] % 2f == 0f;
                            float speed = 0.5f;
                            int totalProjectiles = 4;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX = (float)(speed * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = normalSpread ? new Vector2(0f, -speed) : new Vector2(-velocityX, -speed);
                            float radialOffset = MathHelper.ToRadians(npc.localAI[1]);
                            for (int i = 0; i < totalProjectiles; i++)
                            {
                                Vector2 spawnVector = npc.Center + Vector2.Normalize(spinningPoint.RotatedBy(radians * i + radialOffset)) * suckDistance;
                                Vector2 velocity = Vector2.Normalize(npc.Center - spawnVector) * speed;
                                Projectile.NewProjectile(npc.GetSource_FromAI(), spawnVector, velocity, type, damage, 0f, Main.myPlayer);
                            }
                        }

                        npc.localAI[1] += 10f;
                    }

                    // Summon some extra projectiles in Expert Mode
                    if (phase2 && expertMode)
                    {
                        npc.localAI[2] += 1f;
                        if (npc.localAI[2] >= 60f * projectileFireRateMultiplier)
                        {
                            npc.localAI[2] = 0f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ModContent.ProjectileType<DarkEnergyBall2>();
                                int damage = npc.GetProjectileDamage(type);
                                bool normalSpread = npc.localAI[0] % 2f != 0f;
                                float speed = 2f;
                                int totalProjectiles = 2;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                double angleA = radians * 0.5;
                                double angleB = MathHelper.ToRadians(90f) - angleA;
                                float velocityX = (float)(speed * Math.Sin(angleA) / Math.Sin(angleB));
                                Vector2 spinningPoint = normalSpread ? new Vector2(0f, -speed) : new Vector2(-velocityX, -speed);
                                float radialOffset = MathHelper.ToRadians(npc.localAI[1] * 0.25f);
                                for (int i = 0; i < totalProjectiles; i++)
                                {
                                    Vector2 spawnVector = npc.Center + Vector2.Normalize(spinningPoint.RotatedBy(radians * i + radialOffset)) * suckDistance;
                                    Vector2 velocity = Vector2.Normalize(npc.Center - spawnVector) * speed;
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), spawnVector, velocity, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    // Summon some extra projectiles in Revengeance Mode
                    if (phase4 && revenge)
                    {
                        npc.localAI[3] += 1f;
                        if (npc.localAI[3] >= 90f * projectileFireRateMultiplier)
                        {
                            npc.localAI[3] = 0f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ModContent.ProjectileType<DarkEnergyBall2>();
                                int damage = npc.GetProjectileDamage(type);
                                bool normalSpread = npc.localAI[0] % 2f == 0f;
                                float speed = 4f;
                                int totalProjectiles = 2;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                double angleA = radians * 0.5;
                                double angleB = MathHelper.ToRadians(90f) - angleA;
                                float velocityX = (float)(speed * Math.Sin(angleA) / Math.Sin(angleB));
                                Vector2 spinningPoint = normalSpread ? new Vector2(0f, -speed) : new Vector2(-velocityX, -speed);
                                float radialOffset = MathHelper.ToRadians(npc.localAI[1] * 0.25f);
                                for (int i = 0; i < totalProjectiles; i++)
                                {
                                    Vector2 spawnVector = npc.Center + Vector2.Normalize(spinningPoint.RotatedBy(radians * i + radialOffset)) * suckDistance;
                                    Vector2 velocity = Vector2.Normalize(npc.Center - spawnVector) * speed;
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), spawnVector, velocity, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                Movement(false);

                // Count up all Dark Energy HP values
                int totalDarkEnergyHP = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC darkEnergy = Main.npc[i];
                    if (darkEnergy.active && darkEnergy.type == ModContent.NPCType<DarkEnergy>())
                        totalDarkEnergyHP += darkEnergy.life;
                }

                // Destroy all Dark Energies if their total HP is below 20%
                int darkEnergyMaxHP = bossRush ? DarkEnergy.MaxBossRushHP : DarkEnergy.MaxHP;
                double HPBoost = CalamityServerConfig.Instance.BossHealthBoost * 0.01;
                darkEnergyMaxHP += (int)(darkEnergyMaxHP * HPBoost);
                int totalDarkEnergiesSpawned = darkEnergyAmt * 3 + 2;
                int totalDarkEnergyMaxHP = darkEnergyMaxHP * totalDarkEnergiesSpawned;
                int succPhaseGateValue = (int)(totalDarkEnergyMaxHP * 0.2);
                if (totalDarkEnergyHP < succPhaseGateValue)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath44, npc.Center);

                    // Kill all Dark Energies
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC darkEnergy = Main.npc[i];
                        if (darkEnergy.active && darkEnergy.type == ModContent.NPCType<DarkEnergy>())
                        {
                            darkEnergy.HitEffect();
                            darkEnergy.active = false;
                            darkEnergy.netUpdate = true;
                        }
                    }

                    // Generate a dust explosion
                    int dustAmt = 30;
                    int random = 3;
                    for (int j = 0; j < 10; j++)
                    {
                        random += j * 2;
                        int dustAmtSpawned = 0;
                        int scale = random * 13;
                        float dustPositionX = npc.Center.X - (scale / 2);
                        float dustPositionY = npc.Center.Y - (scale / 2);
                        while (dustAmtSpawned < dustAmt)
                        {
                            float dustVelocityX = Main.rand.Next(-random, random);
                            float dustVelocityY = Main.rand.Next(-random, random);
                            float dustVelocityScalar = random * 2f;
                            float dustVelocity = (float)Math.Sqrt(dustVelocityX * dustVelocityX + dustVelocityY * dustVelocityY);
                            dustVelocity = dustVelocityScalar / dustVelocity;
                            dustVelocityX *= dustVelocity;
                            dustVelocityY *= dustVelocity;
                            int dust = Dust.NewDust(new Vector2(dustPositionX, dustPositionY), scale, scale, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 5f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].position.X = npc.Center.X;
                            Main.dust[dust].position.Y = npc.Center.Y;
                            Main.dust[dust].position.X += Main.rand.Next(-10, 11);
                            Main.dust[dust].position.Y += Main.rand.Next(-10, 11);
                            Main.dust[dust].velocity.X = dustVelocityX;
                            Main.dust[dust].velocity.Y = dustVelocityY;
                            dustAmtSpawned++;
                        }
                    }
                }
            }

            // Basic movement towards a location
            void Movement(bool succ)
            {
                float velocity = moveVeryFast ? 25f : bossRush ? 15f : ((expertMode ? 7.5f : 6f) + (float)(death ? 2f * (1D - lifeRatio) : 0f)) * tileEnrageMult;
                float acceleration = (moveVeryFast ? 0.75f : bossRush ? 0.3f : death ? 0.2f : expertMode ? 0.16f : 0.12f) + (float)(death ? 0.04f * (1D - lifeRatio) : 0f) * tileEnrageMult;

                // Increase speed dramatically in succ phase
                if (succ)
                {
                    velocity *= 2f;
                    acceleration *= 2f;
                }

                if (Main.getGoodWorld)
                {
                    velocity *= 1.15f;
                    acceleration *= 1.15f;
                }

                Vector2 destination = player.Center;

                // Move between 8 different positions around the player, in order
                float maxDistance = 320f;
                Vector2 moveToOffset = succ ? Vector2.Zero : new Vector2(0f, -maxDistance);
                if (!succ)
                {
                    // Move to a new location every few seconds
                    calamityGlobalNPC.newAI[2] += 1f;
                    float newPositionGateValue = bossRush ? 120f : death ? 180f : revenge ? 210f : expertMode ? 240f : 300f;
                    if (calamityGlobalNPC.newAI[2] > newPositionGateValue)
                    {
                        calamityGlobalNPC.newAI[2] = 0f;

                        npc.ai[0] += 1f;
                        if (npc.ai[0] > 7f)
                            npc.ai[0] = 0f;
                    }

                    switch ((int)npc.ai[0])
                    {
                        case 0:
                            break;
                        case 1:
                            moveToOffset.X = -maxDistance;
                            break;
                        case 2:
                            moveToOffset.X = -maxDistance;
                            moveToOffset.Y = 0f;
                            break;
                        case 3:
                            moveToOffset.X = -maxDistance;
                            moveToOffset.Y = maxDistance;
                            break;
                        case 4:
                            moveToOffset.Y = maxDistance;
                            break;
                        case 5:
                            moveToOffset.X = maxDistance;
                            moveToOffset.Y = maxDistance;
                            break;
                        case 6:
                            moveToOffset.X = maxDistance;
                            moveToOffset.Y = 0f;
                            break;
                        case 7:
                            moveToOffset.X = maxDistance;
                            break;
                    }
                }

                destination += moveToOffset;

                // How far Ceaseless Void is from where it's supposed to be
                Vector2 distanceFromDestination = destination - npc.Center;

                // Movement
                if (npc.Distance(destination) > maxDistance || succ)
                {
                    CalamityUtils.SmoothMovement(npc, 0f, distanceFromDestination, velocity, acceleration, true);
                }
                else
                {
                    // Slow down
                    if (npc.velocity.Length() > 0.5f)
                        npc.velocity *= 0.8f;
                    else
                        npc.velocity = Vector2.Zero;
                }
            }

            // Spawn more Dark Energies as the fight progresses
            if (calamityGlobalNPC.newAI[0] == 0f && npc.life > 0)
                calamityGlobalNPC.newAI[0] = npc.lifeMax;

            if (npc.life > 0)
            {
                int healthGateValue = (int)(npc.lifeMax * 0.3);
                if ((npc.life + healthGateValue) < calamityGlobalNPC.newAI[0])
                {
                    npc.TargetClosest();
                    calamityGlobalNPC.newAI[0] = npc.life;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;
                    npc.ai[3] = 0f;
                    npc.localAI[0] += 1f;
                    npc.localAI[1] = 0f;
                    npc.localAI[2] = 0f;
                    npc.localAI[3] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (phase4)
                        {
                            for (int i = 0; i < darkEnergyAmt; i++)
                            {
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(npc.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 0f, 0f, 0f);
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(npc.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 2f, 0f, 0f);
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(npc.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 4f, 0f, 0f);
                            }
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(0) * distance2)), (int)(npc.Center.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 1f, 0f, 0f);
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(0) * distance2)), (int)(npc.Center.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 3f, 0f, 0f);
                        }
                        else if (phase3)
                        {
                            for (int i = 0; i < darkEnergyAmt; i++)
                            {
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(npc.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 0f, 0f, 0f);
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(npc.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 1.5f, 0f, 0f);
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(npc.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 3f, 0f, 0f);
                            }
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(0) * distance2)), (int)(npc.Center.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 0.5f, 0f, 0f);
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(0) * distance2)), (int)(npc.Center.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 2f, 0f, 0f);
                        }
                        else
                        {
                            for (int i = 0; i < darkEnergyAmt; i++)
                            {
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(npc.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 0f, 0f, 0f);
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(npc.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 1f, 0f, 0f);
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(npc.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 2f, 0f, 0f);
                            }
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(0) * distance2)), (int)(npc.Center.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 1.5f, 0f, 0f);
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + (Math.Sin(0) * distance2)), (int)(npc.Center.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 2.5f, 0f, 0f);
                        }
                    }

                    // Despawn potentially hazardous projectiles when entering a new phase
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.active)
                        {
                            if (projectile.type == ModContent.ProjectileType<DoGBeamPortal>() || projectile.type == ModContent.ProjectileType<DoGBeam>() ||
                                projectile.type == ModContent.ProjectileType<DarkEnergyBall>() || projectile.type == ModContent.ProjectileType<DarkEnergyBall2>())
                            {
                                if (projectile.timeLeft > 30)
                                    projectile.timeLeft = 30;
                            }
                        }
                    }
                }
            }
        }
    }
}
