using System;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    public static class TwinsAI
    {
        // Vanilla values
        public static float RapidFireDamageMult = 0.75f; // 80 (buffed)
        public static float RetinazerPhase2ContactDamageMult = 1.5f; // 114
        public static float SpazmatismPhase2ContactDamageMult = 1.5f; // 127
        public static int FlamethrowerDamage = 27; // 108; Applies to both flamethrower types
        public static int FireballDamage = 22; // 88; Applies to both fireball types

        // Rev+ exclusive
        public static int LaserDamage = 21; // 84 (buffed)
        public static int RedLaserDamage = 27; // 108 (buffed)
        public static int HomingDartDamage = 27; // 108

        public static int CalculateMechDamage(this int damage)
        {
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
            return damage;
        }

        public class RetinazerAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                {
                    CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
                    options.aggroRatio = -1f;
                    CalamityUtils.CalamityTargeting(NPC, options);
                }

                // Percent life remaining
                float lifeRatio = NPC.life / (float)NPC.lifeMax;

                // Easier to send info to Spazmatism
                CalamityGlobalNPC.laserEye = NPC.whoAmI;

                // Check for Spazmatism
                bool spazAlive = false;
                if (CalamityGlobalNPC.fireEye != -1)
                    spazAlive = Main.npc[CalamityGlobalNPC.fireEye].active;

                // I'm not commenting this entire fucking thing, already did spaz, I'm not doing ret
                Vector2 hoverDestination = new Vector2(NPC.Center.X - Main.player[NPC.target].position.X - (Main.player[NPC.target].width / 2), NPC.position.Y + NPC.height - 59f - Main.player[NPC.target].position.Y - (Main.player[NPC.target].height / 2));
                int direction = (NPC.Center.X < Main.player[NPC.target].position.X + Main.player[NPC.target].width) ? -1 : 1;

                float hoverRotation = hoverDestination.ToRotation() + MathHelper.PiOver2;
                if (hoverRotation < 0f)
                    hoverRotation += MathHelper.TwoPi;
                else if (hoverRotation > MathHelper.TwoPi)
                    hoverRotation -= MathHelper.TwoPi;

                float rotationRate = 0.15f;
                NPC.rotation = NPC.rotation.AngleTowards(hoverRotation, rotationRate);

                if (Main.rand.NextBool(5))
                {
                    int retiDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + NPC.height * 0.25f), NPC.width, (int)(NPC.height * 0.5f), DustID.Blood, NPC.velocity.X, 2f, 0, default, 1f);
                    Dust dust = Main.dust[retiDust];
                    dust.velocity.X *= 0.5f;
                    dust.velocity.Y *= 0.1f;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && !Main.player[NPC.target].dead && NPC.timeLeft < 10)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (i != NPC.whoAmI && Main.npc[i].active && (Main.npc[i].type == NPCID.Retinazer || Main.npc[i].type == NPCID.Spazmatism) && Main.npc[i].timeLeft - 1 > NPC.timeLeft)
                            NPC.timeLeft = Main.npc[i].timeLeft - 1;
                    }
                }

                // Phase HP ratios
                float phase2LifeRatio = death ? 0.85f : 0.7f;
                float finalPhaseLifeRatio = death ? 0.4f : 0.25f;

                // Movement variables
                float phase1MaxSpeedIncrease = 1f;
                float phase1MaxChargeSpeedIncrease = 1.5f;

                // Phase duration variables
                float phase1MaxLaserPhaseDurationDecrease = death ? 120f : 300f;

                // Phase checks
                bool phase2 = lifeRatio < phase2LifeRatio;
                bool finalPhase = lifeRatio < finalPhaseLifeRatio;

                Vector2 mechQueenSpacing = Vector2.Zero;
                if (NPC.IsMechQueenUp)
                {
                    NPC nPC = Main.npc[NPC.mechQueen];
                    Vector2 mechQueenCenter = nPC.GetMechQueenCenter();
                    Vector2 eyePosition = new Vector2(-150f, -250f);
                    eyePosition *= 0.75f;
                    float mechdusaRotation = nPC.velocity.X * 0.025f;
                    mechQueenSpacing = mechQueenCenter + eyePosition;
                    mechQueenSpacing = mechQueenSpacing.RotatedBy(mechdusaRotation, mechQueenCenter);
                }

                NPC.reflectsProjectiles = false;

                // Despawn
                if ((Main.player[NPC.target].dead || Main.IsItDay()) && !BossRushEvent.BossRushActive)
                {
                    NPC.velocity.Y -= 0.04f;
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                        return false;
                    }
                }

                else if (NPC.ai[0] == 0f)
                {
                    if (NPC.ai[1] == 0f)
                    {
                        float maxVelocity = death ? 9.25f : 8.25f;
                        float acceleration = death ? 0.13f : 0.115f;

                        if (death)
                        {
                            maxVelocity += phase1MaxSpeedIncrease * ((1f - lifeRatio) / (1f - phase2LifeRatio));
                        }

                        if (Main.getGoodWorld)
                        {
                            maxVelocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        float distanceFromTarget = 300f;
                        Vector2 destination = Main.player[NPC.target].Center + Vector2.UnitX * distanceFromTarget * direction - Vector2.UnitY * distanceFromTarget;
                        float distanceFromDestination = (destination - NPC.Center).Length();
                        Vector2 idealVelocity = (destination - NPC.Center).SafeNormalize(Vector2.UnitX * direction);

                        if (NPC.IsMechQueenUp)
                        {
                            maxVelocity = 14f;

                            destination = mechQueenSpacing;
                            distanceFromDestination = (destination - NPC.Center).Length();
                            idealVelocity = (destination - NPC.Center).SafeNormalize(Vector2.UnitY);

                            if (distanceFromDestination > maxVelocity)
                                idealVelocity *= maxVelocity / distanceFromDestination;

                            float inertia = 60f;
                            NPC.velocity = (NPC.velocity * (inertia - 1f) + idealVelocity) / inertia;
                        }
                        else
                            NPC.SimpleFlyMovement(idealVelocity * maxVelocity, acceleration);

                        float phaseGateValue = death ? (300f - phase1MaxLaserPhaseDurationDecrease * ((1f - lifeRatio) / (1f - phase2LifeRatio))) : 450f;
                        float laserGateValue = 30f;
                        if (NPC.IsMechQueenUp)
                        {
                            phaseGateValue = 900f;
                            laserGateValue = ((!NPC.npcsFoundForCheckActive[NPCID.TheDestroyerBody]) ? 60f : 90f);
                        }

                        NPC.ai[2] += 1f;
                        if (NPC.ai[2] >= phaseGateValue)
                        {
                            NPC.ai[1] = 1f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;

                            CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
                            options.aggroRatio = -1f;
                            CalamityUtils.CalamityTargeting(NPC, options);

                            NPC.netUpdate = true;
                        }
                        else if (distanceFromDestination < (death ? 960f : 800f))
                        {
                            if (!Main.player[NPC.target].dead)
                            {
                                NPC.ai[3] += 1f;
                                if (Main.getGoodWorld)
                                    NPC.ai[3] += 0.5f;
                            }

                            if (NPC.ai[3] >= laserGateValue)
                            {
                                NPC.ai[3] = 0f;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float laserSpeed = death ? 11.25f : 10.5f;

                                    int type = ProjectileID.EyeLaser;
                                    int damage = LaserDamage.CalculateMechDamage();
                                    Vector2 laserVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * laserSpeed;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + laserVelocity.SafeNormalize(Vector2.UnitY) * 90f, laserVelocity, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    else if (NPC.ai[1] == 1f)
                    {
                        NPC.rotation = hoverRotation;

                        float chargeSpeed = death ? 17.5f : 15f;
                        if (death)
                            chargeSpeed += phase1MaxChargeSpeedIncrease * ((1f - lifeRatio) / (1f - phase2LifeRatio));
                        if (Main.getGoodWorld)
                            chargeSpeed += 2f;

                        NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitX * direction) * chargeSpeed;

                        NPC.ai[1] = 2f;
                    }
                    else if (NPC.ai[1] == 2f)
                    {
                        NPC.ai[2] += 1f;
                        float decelerateGateValue = (death ? 36f : 32f) + (death ? 4f * ((1f - lifeRatio) / (1f - phase2LifeRatio)) : 0f);
                        if (NPC.ai[2] >= decelerateGateValue)
                        {
                            float decelerationMultiplier = (death ? 0.84f : 0.92f) - (death ? 0.16f * ((1f - lifeRatio) / (1f - phase2LifeRatio)) : 0f);
                            NPC.velocity *= decelerationMultiplier;

                            if (Math.Abs(NPC.velocity.X) < 0.1)
                                NPC.velocity.X = 0f;
                            if (Math.Abs(NPC.velocity.Y) < 0.1)
                                NPC.velocity.Y = 0f;
                        }
                        else
                            NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

                        float delayBeforeChargingAgain = death ? 50f : 60f;
                        if (NPC.ai[2] >= delayBeforeChargingAgain)
                        {
                            NPC.ai[3] += 1f;
                            NPC.ai[2] = 0f;

                            NPC.rotation = hoverRotation;

                            float totalCharges = death ? 6f : 5f;

                            if (NPC.ai[3] >= totalCharges)
                            {
                                NPC.ai[1] = 0f;
                                NPC.ai[3] = 0f;

                                CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
                                options.aggroRatio = -1f;
                                CalamityUtils.CalamityTargeting(NPC, options);
                            }
                            else
                                NPC.ai[1] = 1f;
                        }
                    }

                    // Enter phase 2
                    if (phase2)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;

                        CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
                        options.aggroRatio = -1f;
                        CalamityUtils.CalamityTargeting(NPC, options);

                        NPC.netUpdate = true;
                    }
                }

                else if (NPC.ai[0] == 1f || NPC.ai[0] == 2f)
                {
                    if (NPC.IsMechQueenUp)
                        NPC.reflectsProjectiles = true;

                    if (NPC.ai[0] == 1f)
                    {
                        NPC.ai[2] += 0.005f;
                        if (NPC.ai[2] > 0.5)
                            NPC.ai[2] = 0.5f;
                    }
                    else
                    {
                        NPC.ai[2] -= 0.005f;
                        if (NPC.ai[2] < 0f)
                            NPC.ai[2] = 0f;
                    }

                    NPC.rotation += NPC.ai[2];

                    NPC.ai[1] += 1f;

                    if (NPC.ai[1] == 100f)
                    {
                        NPC.ai[0] += 1f;
                        NPC.ai[1] = 0f;
                        if (NPC.ai[0] == 3f)
                        {
                            NPC.ai[2] = 0f;
                        }
                        else
                        {
                            SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);

                            if (!Main.dedServ)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 143, 1f);
                                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
                                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
                                }
                            }

                            for (int j = 0; j < 20; j++)
                                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

                            SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                        }
                    }

                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

                    NPC.velocity *= 0.98f;
                    if (Math.Abs(NPC.velocity.X) < 0.1)
                        NPC.velocity.X = 0f;
                    if (Math.Abs(NPC.velocity.Y) < 0.1)
                        NPC.velocity.Y = 0f;
                }
                else
                {
                    // If in phase 2 but Spaz isn't
                    bool spazInPhase1 = false;
                    if (CalamityGlobalNPC.fireEye != -1)
                    {
                        if (Main.npc[CalamityGlobalNPC.fireEye].active)
                            spazInPhase1 = Main.npc[CalamityGlobalNPC.fireEye].ai[0] == 1f || Main.npc[CalamityGlobalNPC.fireEye].ai[0] == 2f || Main.npc[CalamityGlobalNPC.fireEye].ai[0] == 0f;
                    }

                    NPC.chaseable = !spazInPhase1;

                    NPC.damage = (int)Math.Round(NPC.defDamage * RetinazerPhase2ContactDamageMult);
                    NPC.defense = NPC.defDefense + 10;
                    calamityGlobalNPC.DR = spazInPhase1 ? 0.9999f : 0.2f;
                    calamityGlobalNPC.unbreakableDR = spazInPhase1;
                    calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = spazInPhase1;

                    NPC.HitSound = SoundID.NPCHit4;

                    if (NPC.ai[1] == 0f)
                    {
                        float maxVelocity = death ? 10.65f + (1.5f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio)) : 9.5f;
                        float acceleration = death ? 0.1975f : 0.175f;

                        if (Main.getGoodWorld)
                        {
                            maxVelocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        float distanceFromTarget = 420f;
                        Vector2 destination = Main.player[NPC.target].Center - Vector2.UnitY * distanceFromTarget;
                        float distanceFromDestination = (destination - NPC.Center).Length();
                        Vector2 idealVelocity = (destination - NPC.Center).SafeNormalize(Vector2.UnitX * direction);

                        if (NPC.IsMechQueenUp)
                        {
                            maxVelocity = 14f;

                            destination = mechQueenSpacing;
                            distanceFromDestination = (destination - NPC.Center).Length();
                            idealVelocity = (destination - NPC.Center).SafeNormalize(Vector2.UnitY);

                            if (distanceFromDestination > maxVelocity)
                                idealVelocity *= maxVelocity / distanceFromDestination;

                            float inertia = 5f;
                            NPC.velocity = (NPC.velocity * (inertia - 1f) + idealVelocity) / inertia;
                        }
                        else
                            NPC.SimpleFlyMovement(idealVelocity * maxVelocity, acceleration);

                        NPC.ai[2] += spazAlive ? 1f : 1.5f;
                        float phaseGateValue = NPC.IsMechQueenUp ? 900f : 300f - (death ? 80f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f);
                        if (NPC.ai[2] >= phaseGateValue)
                        {
                            NPC.ai[1] = 1f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;

                            CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
                            options.aggroRatio = -1f;
                            CalamityUtils.CalamityTargeting(NPC, options);

                            NPC.netUpdate = true;
                        }

                        NPC.rotation = (Main.player[NPC.target].Center - NPC.Center).ToRotation() - MathHelper.PiOver2;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.localAI[1] += 1f + (death ? ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) / 2 : 0f);
                            if (NPC.localAI[1] >= (spazAlive ? 52f : 26f))
                            {
                                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                                {
                                    NPC.localAI[1] = 0f;

                                    float laserSpeed = 10f;
                                    int type = ProjectileID.DeathLaser;

                                    Vector2 laserVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * laserSpeed;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + laserVelocity.SafeNormalize(Vector2.UnitY) * 120f, laserVelocity, type, RedLaserDamage.CalculateMechDamage(), 0f, Main.myPlayer);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (NPC.ai[1] == 1f)
                        {
                            float maxVelocity = death ? 10.65f + (1.5f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio)) : 9.5f;
                            float acceleration = death ? 0.295f : 0.25f;

                            if (Main.getGoodWorld)
                            {
                                maxVelocity *= 1.15f;
                                acceleration *= 1.15f;
                            }

                            float distanceFromTarget = 420f;
                            Vector2 destination = Main.player[NPC.target].Center + Vector2.UnitX * distanceFromTarget * direction;
                            float distanceFromDestination = (destination - NPC.Center).Length();
                            Vector2 idealVelocity = (destination - NPC.Center).SafeNormalize(Vector2.UnitX * direction);
                            NPC.SimpleFlyMovement(idealVelocity * maxVelocity, acceleration);

                            NPC.rotation = (Main.player[NPC.target].Center - NPC.Center).ToRotation() - MathHelper.PiOver2;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NPC.localAI[1] += 1f + (death ? ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) / 2 : 0f);
                                if (NPC.localAI[1] > (spazAlive ? 20f : 10f))
                                {
                                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                                    {
                                        NPC.localAI[1] = 0f;

                                        float laserSpeed = 9f;
                                        int type = ProjectileID.DeathLaser;
                                        int damage = (int)Math.Round(RedLaserDamage.CalculateMechDamage() * RapidFireDamageMult);

                                        Vector2 laserVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * laserSpeed;
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + laserVelocity.SafeNormalize(Vector2.UnitY) * 120f, laserVelocity, type, damage, 0f, Main.myPlayer);
                                    }
                                }
                            }

                            NPC.ai[2] += spazAlive ? 1f : 1.5f;
                            if (NPC.ai[2] >= (death ? 150f : 180f) - (death ? 25f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f))
                            {
                                NPC.ai[1] = finalPhase ? 4f : 0f;
                                NPC.ai[2] = 0f;
                                NPC.ai[3] = 0f;

                                CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
                                options.aggroRatio = -1f;
                                CalamityUtils.CalamityTargeting(NPC, options);

                                NPC.netUpdate = true;
                            }
                        }

                        // Charge
                        else if (NPC.ai[1] == 2f)
                        {
                            // Set rotation and velocity
                            NPC.rotation = hoverRotation;

                            float chargeSpeed = (death ? 25f : 22f) + (death ? 3f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f);
                            if (!spazAlive)
                                chargeSpeed += 2f;
                            if (Main.getGoodWorld)
                                chargeSpeed += 2f;

                            NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * chargeSpeed;

                            NPC.ai[1] = 3f;
                        }

                        else if (NPC.ai[1] == 3f)
                        {
                            NPC.ai[2] += 1f;

                            float chargeTime = spazAlive ? 45f : 30f;
                            if (NPC.ai[3] % 3f == 0f)
                                chargeTime = spazAlive ? 90f : 60f;
                            if (death)
                                chargeTime -= chargeTime * 0.1f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio);
                            chargeTime -= chargeTime * (death ? 0.06f : 0f);

                            // Slow down
                            if (NPC.ai[2] >= chargeTime)
                            {
                                NPC.velocity *= 0.93f;

                                if (Math.Abs(NPC.velocity.X) < 0.1)
                                    NPC.velocity.X = 0f;
                                if (Math.Abs(NPC.velocity.Y) < 0.1)
                                    NPC.velocity.Y = 0f;
                            }
                            else
                            {
                                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

                                if (NPC.ai[3] % 3f == 0f)
                                {
                                    float fireRate = spazAlive ? 13f : 9f;
                                    if (NPC.ai[2] % fireRate == 0f)
                                    {
                                        SoundEngine.PlaySound(SoundID.Item33, NPC.Center);
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            float laserDartSpeed = (death ? 7.25f : 6f) * (spazAlive ? 1f : 1.5f);
                                            int type = ModContent.ProjectileType<HomingLaserDart>();
                                            int damage = HomingDartDamage.CalculateMechDamage();

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

                                            Vector2 laserDartVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * laserDartSpeed;
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.velocity.SafeNormalize(Vector2.UnitY) * 60f, laserDartVelocity, type, damage, 0f, Main.myPlayer);
                                        }
                                    }
                                }
                            }

                            // Charge four times
                            float chargeGateValue = 30f;
                            chargeGateValue -= chargeGateValue * (death ? 0.07f : 0f);
                            if (NPC.ai[2] >= chargeTime + chargeGateValue)
                            {
                                NPC.ai[2] = 0f;

                                float chargeIncrement = 1f;
                                if (death && Main.rand.NextBool() && NPC.ai[3] < (spazAlive ? 1f : 3f))
                                {
                                    chargeIncrement = 2f;

                                    // Net update due to the randomness in Master Mode
                                    NPC.netUpdate = true;
                                }

                                NPC.rotation = hoverRotation;

                                NPC.ai[3] += chargeIncrement;
                                float maxChargeAmt = spazAlive ? 2f : 4f;
                                if (NPC.ai[3] >= maxChargeAmt)
                                {
                                    NPC.ai[1] = 0f;
                                    NPC.ai[3] = 0f;

                                    CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
                                    options.aggroRatio = -1f;
                                    CalamityUtils.CalamityTargeting(NPC, options);
                                }
                                else
                                    NPC.ai[1] = 4f;
                            }
                        }

                        // Get in position for charge
                        else if (NPC.ai[1] == 4f)
                        {
                            float chargeLineUpDistance = spazAlive ? 600f : 500f;
                            float chargeSpeed = death ? 5f : 3f;
                            float chargeAcceleration = death ? 0.495f : 0.45f;

                            if (spazAlive)
                            {
                                chargeSpeed *= 0.75f;
                                chargeAcceleration *= 0.75f;
                            }

                            if (Main.getGoodWorld)
                            {
                                chargeSpeed *= 1.15f;
                                chargeAcceleration *= 1.15f;
                            }

                            Vector2 destination = Main.player[NPC.target].Center + Vector2.UnitX * chargeLineUpDistance * direction;
                            Vector2 idealVelocity = (destination - NPC.Center).SafeNormalize(Vector2.UnitX * direction) * chargeSpeed;
                            NPC.SimpleFlyMovement(idealVelocity, chargeAcceleration);

                            // Take 1.25 or 1 second to get in position, then charge
                            NPC.ai[2] += 1f;
                            if (NPC.ai[2] >= (spazAlive ? 75f : 60f) - (death ? 5f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f))
                            {
                                NPC.ai[1] = 2f;
                                NPC.ai[2] = 0f;
                                NPC.netUpdate = true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        public class SpazmatismAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                {
                    CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
                    options.finishThemOff = true;
                    CalamityUtils.CalamityTargeting(NPC, options);
                }

                // Percent life remaining
                float lifeRatio = NPC.life / (float)NPC.lifeMax;

                // Easier to send info to Retinazer
                CalamityGlobalNPC.fireEye = NPC.whoAmI;

                // Check for Retinazer
                bool retAlive = false;
                if (CalamityGlobalNPC.laserEye != -1)
                    retAlive = Main.npc[CalamityGlobalNPC.laserEye].active;

                // Rotation
                Vector2 hoverDestination = new Vector2(NPC.Center.X - Main.player[NPC.target].position.X - (Main.player[NPC.target].width / 2), NPC.position.Y + NPC.height - 59f - Main.player[NPC.target].position.Y - (Main.player[NPC.target].height / 2));
                int direction = (NPC.Center.X < Main.player[NPC.target].position.X + Main.player[NPC.target].width) ? -1 : 1;

                float hoverRotation = hoverDestination.ToRotation() + MathHelper.PiOver2;
                if (hoverRotation < 0f)
                    hoverRotation += MathHelper.TwoPi;
                else if (hoverRotation > MathHelper.TwoPi)
                    hoverRotation -= MathHelper.TwoPi;

                float rotationRate = 0.15f;
                if (NPC.IsMechQueenUp && NPC.ai[0] == 3f && NPC.ai[1] == 0f)
                    rotationRate *= 0.25f;

                NPC.rotation = NPC.rotation.AngleTowards(hoverRotation, rotationRate);

                // Dust
                if (Main.rand.NextBool(5))
                {
                    int spazDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + NPC.height * 0.25f), NPC.width, (int)(NPC.height * 0.5f), DustID.Blood, NPC.velocity.X, 2f, 0, default, 1f);
                    Dust dust = Main.dust[spazDust];
                    dust.velocity.X *= 0.5f;
                    dust.velocity.Y *= 0.1f;
                }

                // Despawn Twins at the same time
                if (Main.netMode != NetmodeID.MultiplayerClient && !Main.player[NPC.target].dead && NPC.timeLeft < 10)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (i != NPC.whoAmI && Main.npc[i].active && (Main.npc[i].type == NPCID.Retinazer || Main.npc[i].type == NPCID.Spazmatism) && Main.npc[i].timeLeft - 1 > NPC.timeLeft)
                            NPC.timeLeft = Main.npc[i].timeLeft - 1;
                    }
                }

                // Phase HP ratios
                float phase2LifeRatio = death ? 0.85f : 0.7f;
                float finalPhaseLifeRatio = death ? 0.4f : 0.25f;

                // Movement variables
                float phase1MaxSpeedIncrease = 1.125f;
                float phase1MaxChargeSpeedIncrease = 1.4f;

                // Phase duration variables
                float phase1MaxCursedFlamePhaseDurationDecrease = death ? 80f : 200f;
                float phase1MaxChargesDecrease = 2f;

                // Phase checks
                bool phase2 = lifeRatio < phase2LifeRatio;
                bool finalPhase = lifeRatio < finalPhaseLifeRatio;

                Vector2 mechQueenSpacing = Vector2.Zero;
                if (NPC.IsMechQueenUp)
                {
                    NPC nPC2 = Main.npc[NPC.mechQueen];
                    Vector2 mechQueenCenter2 = nPC2.GetMechQueenCenter();
                    Vector2 mechdusaSpacingVector = new Vector2(150f, -250f);
                    mechdusaSpacingVector *= 0.75f;
                    float mechdusaSpacingVel = nPC2.velocity.X * 0.025f;
                    mechQueenSpacing = mechQueenCenter2 + mechdusaSpacingVector;
                    mechQueenSpacing = mechQueenSpacing.RotatedBy(mechdusaSpacingVel, mechQueenCenter2);
                }

                NPC.reflectsProjectiles = false;

                // Despawn
                if ((Main.player[NPC.target].dead || Main.IsItDay()) && !BossRushEvent.BossRushActive)
                {
                    NPC.velocity.Y -= 0.04f;
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                        return false;
                    }
                }

                // Phase 1
                else if (NPC.ai[0] == 0f)
                {
                    // Cursed fireball phase
                    if (NPC.ai[1] == 0f)
                    {
                        // Velocity
                        float maxVelocity = death ? 13.5f : 12f;
                        float acceleration = death ? 0.46f : 0.4f;

                        if (death)
                        {
                            maxVelocity += phase1MaxSpeedIncrease * ((1f - lifeRatio) / (1f - phase2LifeRatio));
                        }

                        if (Main.getGoodWorld)
                        {
                            maxVelocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        float distanceFromTarget = 400f;
                        Vector2 destination = Main.player[NPC.target].Center + Vector2.UnitX * distanceFromTarget * direction;
                        float distanceFromDestination = (destination - NPC.Center).Length();
                        Vector2 idealVelocity = (destination - NPC.Center).SafeNormalize(Vector2.UnitX * direction);

                        if (NPC.IsMechQueenUp)
                        {
                            maxVelocity = 14f;

                            destination = mechQueenSpacing;
                            distanceFromDestination = (destination - NPC.Center).Length();
                            idealVelocity = (destination - NPC.Center).SafeNormalize(Vector2.UnitY);

                            if (distanceFromDestination > maxVelocity)
                                idealVelocity *= maxVelocity / distanceFromDestination;

                            float inertia = 5f;
                            NPC.velocity = (NPC.velocity * (inertia - 1f) + idealVelocity) / inertia;
                        }
                        else
                            NPC.SimpleFlyMovement(idealVelocity * maxVelocity, acceleration);

                        // Fire cursed flames for 5 seconds
                        NPC.ai[2] += 1f;
                        float phaseGateValue = NPC.IsMechQueenUp ? 900f : 300f - (death ? phase1MaxCursedFlamePhaseDurationDecrease * ((1f - lifeRatio) / (1f - phase2LifeRatio)) : 0f);
                        if (NPC.ai[2] >= phaseGateValue)
                        {
                            // Reset AI array and go to charging phase
                            NPC.ai[1] = 1f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            // Fire cursed flame every half second
                            if (!Main.player[NPC.target].dead)
                            {
                                NPC.ai[3] += 1f;
                                if (Main.getGoodWorld)
                                    NPC.ai[3] += 0.4f;
                            }

                            if (NPC.ai[3] >= 30f)
                            {
                                NPC.ai[3] = 0f;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float cursedFireballSpeed = death ? 15.9f : 15f;
                                    int type = ProjectileID.CursedFlameHostile;

                                    Vector2 fireballVelocity = ((Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * cursedFireballSpeed) + new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)) * 0.05f;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + fireballVelocity.SafeNormalize(Vector2.UnitY) * 50f, fireballVelocity, type, FireballDamage.CalculateMechDamage(), 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    // Charging phase
                    else if (NPC.ai[1] == 1f)
                    {
                        // Rotation and velocity
                        NPC.rotation = hoverRotation;

                        float chargeSpeed = 16f;
                        if (death)
                            chargeSpeed += (phase1MaxChargeSpeedIncrease * ((1f - lifeRatio) / (1f - phase2LifeRatio)));
                        if (Main.getGoodWorld)
                            chargeSpeed *= 1.2f;

                        NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitX * direction) * chargeSpeed;

                        NPC.ai[1] = 2f;
                    }
                    else if (NPC.ai[1] == 2f)
                    {
                        NPC.ai[2] += 1f;

                        float timeBeforeSlowDown = death ? 14f : 10f;
                        if (NPC.ai[2] >= timeBeforeSlowDown)
                        {
                            // Slow down
                            NPC.velocity *= 0.8f;

                            if (Math.Abs(NPC.velocity.X) < 0.1)
                                NPC.velocity.X = 0f;
                            if (Math.Abs(NPC.velocity.Y) < 0.1)
                                NPC.velocity.Y = 0f;
                        }
                        else
                            NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

                        // Charge 8 times
                        float chargeTime = death ? 35f : 25f;
                        if (NPC.ai[2] >= chargeTime)
                        {
                            // Reset AI array and go to cursed fireball phase
                            NPC.ai[3] += 1f;
                            NPC.ai[2] = 0f;

                            NPC.rotation = hoverRotation;

                            float totalCharges = 8f;
                            if (death)
                                totalCharges -= (float)Math.Round(phase1MaxChargesDecrease * ((1f - lifeRatio) / (1f - phase2LifeRatio)));

                            if (NPC.ai[3] >= totalCharges)
                            {
                                NPC.ai[1] = 0f;
                                NPC.ai[3] = 0f;
                            }
                            else
                                NPC.ai[1] = 1f;
                        }
                    }

                    // Enter phase 2
                    if (phase2)
                    {
                        // Reset AI array and go to transition phase
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;

                        CalamityTargetingParameters options = CalamityTargetingParameters.BossDefaults;
                        options.finishThemOff = true;
                        CalamityUtils.CalamityTargeting(NPC, options);

                        NPC.netUpdate = true;
                    }
                }

                // Transition phase
                else if (NPC.ai[0] == 1f || NPC.ai[0] == 2f)
                {
                    if (NPC.IsMechQueenUp)
                        NPC.reflectsProjectiles = true;

                    // AI timer for rotation
                    if (NPC.ai[0] == 1f)
                    {
                        NPC.ai[2] += 0.005f;
                        if (NPC.ai[2] > 0.5)
                            NPC.ai[2] = 0.5f;
                    }
                    else
                    {
                        NPC.ai[2] -= 0.005f;
                        if (NPC.ai[2] < 0f)
                            NPC.ai[2] = 0f;
                    }

                    // Spin around like a moron while flinging blood and gore everywhere
                    NPC.rotation += NPC.ai[2];

                    NPC.ai[1] += 1f;

                    if (NPC.ai[1] == 100f)
                    {
                        NPC.ai[0] += 1f;
                        NPC.ai[1] = 0f;

                        if (NPC.ai[0] == 3f)
                        {
                            NPC.ai[2] = 0f;
                        }
                        else
                        {
                            SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);

                            if (!Main.dedServ)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 144, 1f);
                                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
                                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
                                }
                            }

                            for (int j = 0; j < 20; j++)
                                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

                            SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                        }
                    }

                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

                    NPC.velocity *= 0.98f;
                    if (Math.Abs(NPC.velocity.X) < 0.1)
                        NPC.velocity.X = 0f;
                    if (Math.Abs(NPC.velocity.Y) < 0.1)
                        NPC.velocity.Y = 0f;
                }

                // Phase 2
                else
                {
                    // If in phase 2 but Ret isn't
                    bool retInPhase1 = false;
                    if (CalamityGlobalNPC.laserEye != -1)
                    {
                        if (Main.npc[CalamityGlobalNPC.laserEye].active)
                            retInPhase1 = Main.npc[CalamityGlobalNPC.laserEye].ai[0] == 1f || Main.npc[CalamityGlobalNPC.laserEye].ai[0] == 2f || Main.npc[CalamityGlobalNPC.laserEye].ai[0] == 0f;
                    }

                    NPC.chaseable = !retInPhase1;

                    // Increase defense and damage
                    NPC.damage = (int)Math.Round(NPC.defDamage * SpazmatismPhase2ContactDamageMult);
                    NPC.defense = NPC.defDefense + 18;
                    calamityGlobalNPC.DR = retInPhase1 ? 0.9999f : 0.2f;
                    calamityGlobalNPC.unbreakableDR = retInPhase1;
                    calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = retInPhase1;

                    // Change hit sound to metal
                    NPC.HitSound = SoundID.NPCHit4;

                    // Shadowflamethrower phase
                    if (NPC.ai[1] == 0f)
                    {
                        float maxVelocity = 7.1f + (death ? 0.7f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f);
                        float acceleration = 0.118f + (death ? 0.0175f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f);

                        float distanceFromTarget = 180f;
                        Vector2 destination = Main.player[NPC.target].Center + Vector2.UnitX * distanceFromTarget * direction;
                        float distanceFromDestination = (destination - NPC.Center).Length();
                        Vector2 idealVelocity = (destination - NPC.Center).SafeNormalize(Vector2.UnitX * direction);

                        if (!NPC.IsMechQueenUp)
                        {
                            if (Main.getGoodWorld)
                            {
                                maxVelocity *= 1.15f;
                                acceleration *= 1.15f;
                            }

                            NPC.SimpleFlyMovement(idealVelocity * maxVelocity, acceleration);
                        }

                        // Fire flamethrower for x seconds
                        NPC.ai[2] += retAlive ? 1f : 2f;
                        float phaseGateValue = NPC.IsMechQueenUp ? 900f : 180f - (death ? 30f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f);
                        if (NPC.ai[2] >= phaseGateValue)
                        {
                            NPC.ai[1] = finalPhase ? 5f : 1f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                            NPC.netUpdate = true;
                        }

                        // Fire fireballs and flamethrower
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                        {
                            // Play flame sound on timer
                            NPC.localAI[2] += 1f;
                            if (NPC.localAI[2] > 22f)
                            {
                                NPC.localAI[2] = 0f;
                                SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NPC.localAI[1] += 1f;
                                if (NPC.localAI[1] > 2f)
                                {
                                    NPC.ai[3] += 1f;
                                    NPC.localAI[1] = 0f;

                                    float flamethrowerSpeed = death ? 6.9f : 6f;
                                    float timeForFlamethrowerToReachMaxVelocity = 60f;
                                    float flamethrowerSpeedScalar = MathHelper.Clamp(NPC.ai[2] / timeForFlamethrowerToReachMaxVelocity, 0f, 1f);
                                    flamethrowerSpeed = MathHelper.Lerp(0.1f, flamethrowerSpeed, flamethrowerSpeedScalar);
                                    int type = NPC.ai[3] % 2f == 0f ? ModContent.ProjectileType<CursedFire>() : ModContent.ProjectileType<Shadowflamethrower>();

                                    Vector2 flamethrowerVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * flamethrowerSpeed + NPC.velocity * 0.5f;

                                    if (NPC.IsMechQueenUp)
                                        flamethrowerVelocity = (NPC.rotation + MathHelper.PiOver2).ToRotationVector2() * flamethrowerSpeed + NPC.velocity * 0.5f;

                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + flamethrowerVelocity.SafeNormalize(Vector2.UnitY) * 25f, flamethrowerVelocity, type, FlamethrowerDamage.CalculateMechDamage(), 0f, Main.myPlayer);
                                    if (death && NPC.ai[3] % 30f == 0f)
                                    {
                                        type = NPC.ai[3] % 60f == 0f ? ModContent.ProjectileType<ShadowflameFireball>() : ProjectileID.CursedFlameHostile;
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + flamethrowerVelocity.SafeNormalize(Vector2.UnitY) * 50f, flamethrowerVelocity * 2f, type, FireballDamage.CalculateMechDamage(), 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }

                        if (NPC.IsMechQueenUp)
                        {
                            maxVelocity = 14f;

                            destination = mechQueenSpacing;
                            distanceFromDestination = (destination - NPC.Center).Length();
                            idealVelocity = (destination - NPC.Center).SafeNormalize(Vector2.UnitY);

                            if (distanceFromDestination > maxVelocity)
                                idealVelocity *= maxVelocity / distanceFromDestination;

                            float inertia = 60f;
                            NPC.velocity = (NPC.velocity * (inertia - 1f) + idealVelocity) / inertia;
                        }
                    }

                    // Charging phase
                    else
                    {
                        // Charge
                        if (NPC.ai[1] == 1f)
                        {
                            // Play charge sound
                            SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);

                            // Set rotation and velocity
                            NPC.rotation = hoverRotation;

                            float chargeSpeed = death ? 19.25f + (phase1MaxChargeSpeedIncrease * ((1f - lifeRatio) / (1f - phase2LifeRatio))) : 18f;
                            if (Main.getGoodWorld)
                                chargeSpeed *= 1.2f;

                            NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitX * direction) * chargeSpeed;

                            NPC.ai[1] = 2f;

                            return false;
                        }

                        if (NPC.ai[1] == 2f)
                        {
                            NPC.ai[2] += retAlive ? 1f : 1.25f;

                            float chargeTime = 30f - (death ? 3f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f);

                            // Slow down
                            if (NPC.ai[2] >= chargeTime)
                            {
                                float deceleration = 0.85f - (death ? 0.1f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f);
                                NPC.velocity *= deceleration;

                                if (Math.Abs(NPC.velocity.X) < 0.1)
                                    NPC.velocity.X = 0f;
                                if (Math.Abs(NPC.velocity.Y) < 0.1)
                                    NPC.velocity.Y = 0f;
                            }
                            else
                                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

                            // Charges 5 times
                            if (NPC.ai[2] >= chargeTime * 1.6f)
                            {
                                NPC.ai[3] += 1f;
                                NPC.ai[2] = 0f;

                                NPC.rotation = hoverRotation;

                                float totalCharges = 5f;
                                if (NPC.ai[3] >= totalCharges)
                                {
                                    NPC.ai[1] = 0f;
                                    NPC.ai[3] = 0f;
                                    return false;
                                }

                                NPC.ai[1] = 1f;
                            }
                        }

                        // Crazy charge
                        else if (NPC.ai[1] == 3f)
                        {
                            // Reset AI array and go to shadowflamethrower phase or fireball phase if ret is dead
                            float secondFastCharge = 4f;
                            if (NPC.ai[3] >= (retAlive ? secondFastCharge : secondFastCharge + 1f))
                            {
                                NPC.ai[1] = retAlive ? 0f : 5f;
                                NPC.ai[2] = 0f;
                                NPC.ai[3] = 0f;

                                if (NPC.ai[1] == 0f)
                                    NPC.localAI[1] = -20f;

                                NPC.ForceNetUpdate(false);
                            }

                            // Set charging velocity
                            else if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                // Velocity
                                float spazmatismPhase3ChargeSpeed = (death ? 22.5f : 20f) + (death ? 3f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f);
                                if (NPC.ai[2] == -1f || (!retAlive && NPC.ai[3] == secondFastCharge))
                                    spazmatismPhase3ChargeSpeed *= 1.3f;
                                if (Main.getGoodWorld)
                                    spazmatismPhase3ChargeSpeed *= 1.2f;

                                Vector2 distanceVector = Main.player[NPC.target].Center - NPC.Center;
                                NPC.velocity = distanceVector.SafeNormalize(Vector2.UnitY) * spazmatismPhase3ChargeSpeed;

                                if (retAlive)
                                {
                                    if (NPC.Distance(Main.player[NPC.target].Center) < 100f)
                                    {
                                        if (Math.Abs(NPC.velocity.X) > Math.Abs(NPC.velocity.Y))
                                        {
                                            float absoluteSpazXVel = Math.Abs(NPC.velocity.X);
                                            float absoluteSpazYVel = Math.Abs(NPC.velocity.Y);

                                            if (NPC.Center.X > Main.player[NPC.target].Center.X)
                                                absoluteSpazYVel *= -1f;
                                            if (NPC.Center.Y > Main.player[NPC.target].Center.Y)
                                                absoluteSpazXVel *= -1f;

                                            NPC.velocity = new Vector2(absoluteSpazYVel, absoluteSpazXVel);
                                        }
                                    }
                                }

                                if (death)
                                {
                                    float projectileSpeed = spazmatismPhase3ChargeSpeed * 0.5f;
                                    int type = (!retAlive && NPC.ai[3] % 2f == 0f) ? ModContent.ProjectileType<ShadowflameFireball>() : ProjectileID.CursedFlameHostile;

                                    Vector2 projectileVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * projectileSpeed;
                                    int numProj = 3;
                                    int spread = 15;
                                    float rotation = MathHelper.ToRadians(spread);

                                    for (int i = 0; i < numProj; i++)
                                    {
                                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + perturbedSpeed.SafeNormalize(Vector2.UnitY) * 25f, perturbedSpeed, type, FireballDamage.CalculateMechDamage(), 0f, Main.myPlayer);
                                    }
                                }

                                NPC.ai[1] = 4f;
                                NPC.ForceNetUpdate(false);
                            }
                        }

                        // Crazy charge
                        else if (NPC.ai[1] == 4f)
                        {
                            if (NPC.ai[2] == 0f)
                                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);

                            float spazmatismRetDeadChargeSpeed = ((!retAlive && NPC.ai[3] == 4f) ? 75f : 50f) - (float)Math.Round(death ? ((!retAlive && NPC.ai[3] == 4f) ? 15f : 10f) * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f);

                            NPC.ai[2] += 1f;

                            if (NPC.ai[2] == spazmatismRetDeadChargeSpeed && Vector2.Distance(NPC.position, Main.player[NPC.target].position) < (retAlive ? 200f : 150f))
                                NPC.ai[2] -= 1f;

                            // Slow down
                            if (NPC.ai[2] >= spazmatismRetDeadChargeSpeed)
                            {
                                NPC.velocity *= 0.93f;
                                if (Math.Abs(NPC.velocity.X) < 0.1)
                                    NPC.velocity.X = 0f;
                                if (Math.Abs(NPC.velocity.Y) < 0.1)
                                    NPC.velocity.Y = 0f;
                            }
                            else
                                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

                            // Charge 3 times
                            float spazmatismRetDeadChargeTimer = spazmatismRetDeadChargeSpeed + 25f;
                            if (NPC.ai[2] >= spazmatismRetDeadChargeTimer)
                            {
                                NPC.ForceNetUpdate(false);

                                float chargeIncrement = 1f;
                                if (death && Main.rand.NextBool() && NPC.ai[3] < (retAlive ? 2f : 3f))
                                    chargeIncrement = 2f;

                                NPC.ai[3] += chargeIncrement;
                                NPC.ai[2] = 0f;
                                NPC.ai[1] = 3f;
                            }
                        }

                        // Get in position for charge
                        else if (NPC.ai[1] == 5f)
                        {
                            float chargeLineUpDistance = retAlive ? 600f : 500f;
                            float chargeSpeed = (death ? 17.5f : 16f) + (death ? 5f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f);
                            float chargeAcceleration = (death ? 0.44f : 0.4f) + (death ? 0.1f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f);

                            if (retAlive)
                            {
                                chargeSpeed *= 0.75f;
                                chargeAcceleration *= 0.75f;
                            }

                            if (Main.getGoodWorld)
                            {
                                chargeSpeed *= 1.15f;
                                chargeAcceleration *= 1.15f;
                            }

                            Vector2 destination = Main.player[NPC.target].Center + Vector2.UnitY * chargeLineUpDistance;
                            Vector2 idealVelocity = (destination - NPC.Center).SafeNormalize(Vector2.UnitX * direction) * chargeSpeed;
                            NPC.SimpleFlyMovement(idealVelocity, chargeAcceleration);

                            NPC.ai[2] += 1f;

                            // Fire shadowflames and cursed fireballs
                            float fireRate = retAlive ? 30f : 20f;
                            if (NPC.ai[2] % fireRate == 0f)
                            {
                                NPC.ai[3] += 1f;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float projectileSpeed = 16f;
                                    int type = NPC.ai[3] % 2f == 0f ? ProjectileID.CursedFlameHostile : ModContent.ProjectileType<ShadowflameFireball>();

                                    Vector2 projectileVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * projectileSpeed;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projectileVelocity.SafeNormalize(Vector2.UnitY) * 25f, projectileVelocity, type, FireballDamage.CalculateMechDamage(), 0f, Main.myPlayer, 0f, retAlive ? 0f : 1f);
                                }
                            }

                            // Take 3 seconds to get in position, then charge
                            if (NPC.ai[2] >= (retAlive ? 180f : 135f) - (death ? 45f * ((phase2LifeRatio - lifeRatio) / phase2LifeRatio) : 0f))
                            {
                                NPC.ai[1] = 3f;
                                NPC.ai[2] = -1f;
                                NPC.ai[3] = 0f;
                            }

                            NPC.ForceNetUpdate(false);
                        }
                    }
                }

                return false;
            }
        }
    }
}
