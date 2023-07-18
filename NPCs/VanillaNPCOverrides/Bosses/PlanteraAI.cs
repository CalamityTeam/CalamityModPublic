using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
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
        public const float SeedGatlingGateValue = 900f;
        public const float SeedGatlingDuration = 300f;
        public const float SeedGatlingColorChangeDuration = 180f;
        public const float SeedGatlingStopValue = SeedGatlingGateValue + SeedGatlingDuration;
        public const float SeedGatlingColorChangeGateValue = SeedGatlingStopValue - SeedGatlingColorChangeDuration;

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
            bool addThornBallsToGatlingAttack = lifeRatio < 0.9f;
            bool addSporeGasBlastToGatlingAttack = lifeRatio < 0.75f;
            bool phase2 = lifeRatio <= 0.5f;
            bool phase3 = lifeRatio < 0.35f;
            bool phase4 = lifeRatio < 0.2f;

            // Variables and target
            bool enrage = bossRush;
            bool despawn = false;

            // Check for Jungle
            bool surface = !bossRush && Main.player[npc.target].position.Y < Main.worldSurface * 16.0;
            int maxTentacles = death ? 25 : 20;
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
                int num729 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasHook, npc.whoAmI);
                num729 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasHook, npc.whoAmI);
                num729 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasHook, npc.whoAmI);
            }

            // Find positions of hooks
            int[] array2 = new int[3];
            float num730 = 0f;
            float num731 = 0f;
            int num732 = 0;
            for (int num733 = 0; num733 < Main.maxNPCs; num733++)
            {
                if (Main.npc[num733].active && Main.npc[num733].aiStyle == 52)
                {
                    num730 += Main.npc[num733].Center.X;
                    num731 += Main.npc[num733].Center.Y;
                    array2[num732] = num733;

                    num732++;
                    if (num732 > 2)
                        break;
                }
            }
            num730 /= num732;
            num731 /= num732;

            // Velocity and acceleration
            float velocity = 4f;
            float acceleration = 0.04f;
            if (phase3)
            {
                velocity = 6.5f;
                acceleration = 0.06f;
                if (phase4)
                    velocity = 7f;
            }
            else if (phase2)
            {
                velocity = 6f;
                acceleration = 0.06f;
            }

            if (bossRush)
            {
                velocity += 2f;
                acceleration += 0.02f;
            }

            // Enrage if target is on the surface
            if (!bossRush && (surface || Main.player[npc.target].position.Y > ((Main.maxTilesY - 200) * 16)))
            {
                enrage = true;
                velocity += 8f;
                acceleration = 0.15f;
            }

            npc.Calamity().CurrentlyEnraged = !bossRush && enrage;

            // Movement relative to the target and hook positions
            Vector2 vector91 = new Vector2(num730, num731);
            float num736 = Main.player[npc.target].Center.X - vector91.X;
            float num737 = Main.player[npc.target].Center.Y - vector91.Y;
            if (despawn)
            {
                num737 *= -1f;
                num736 *= -1f;
                velocity += 8f;
            }
            float num738 = (float)Math.Sqrt(num736 * num736 + num737 * num737);

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
            if (!phase2)
            {
                npc.ai[1] += 1f;
                if (usingSeedGatling)
                {
                    float currentSeedGatlingTime = npc.ai[1] - SeedGatlingGateValue;

                    // Slow down more and more as gatling attack continues
                    velocity *= MathHelper.Lerp(0.25f, 1f, currentSeedGatlingTime / SeedGatlingDuration);

                    // Shoot projectiles
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
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
                            float projectileSpeed = 6f;
                            int projectileType = shootThornBall ? ProjectileID.ThornBall : shootPoisonSeed ? ProjectileID.PoisonSeedPlantera : ProjectileID.SeedPlantera;
                            int damage = npc.GetProjectileDamage(projectileType);
                            Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + projectileVelocity * 3f, projectileVelocity * projectileSpeed, projectileType, damage, 0f, Main.myPlayer);
                            if (projectileType == ProjectileID.ThornBall && (Main.rand.NextBool(2) || !Main.zenithWorld))
                                Main.projectile[proj].tileCollide = false;
                        }
                    }
                }

                if (npc.ai[1] >= SeedGatlingStopValue)
                {
                    // Vomit dense spread of spore gas at the end of the gatling attack
                    if (Main.netMode != NetmodeID.MultiplayerClient && addSporeGasBlastToGatlingAttack)
                    {
                        SoundEngine.PlaySound(SoundID.Item107, npc.Center);
                        int totalProjectiles = 36;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        int type = ModContent.ProjectileType<SporeGasPlantera>();
                        int damage = npc.GetProjectileDamage(type);
                        float velocity2 = CalamityWorld.LegendaryMode ? Main.rand.NextFloat(8f, 12f) : Main.rand.NextFloat(4f, 6f);
                        Vector2 spinningPoint = new Vector2(0f, -velocity2);
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(vector255) * 50f, vector255, type, damage, 0f, Main.myPlayer);
                        }
                    }

                    npc.ai[1] = -SeedGatlingDuration;
                }
            }
            else
                npc.ai[1] = 0f;

            // Move slowly for a bit after finishing gatling attack
            bool slowedAfterGatlingAttack = npc.ai[1] < 0f;
            if (slowedAfterGatlingAttack)
            {
                float absValueOfTimer = Math.Abs(npc.ai[1]);
                velocity *= MathHelper.Lerp(0.25f, 1f, absValueOfTimer / SeedGatlingDuration);

                // Shoot homing pink bulb projectiles that leave behind lingering pink clouds
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float shootBulbGateValue = death ? 60f : 120f;
                    if (lifeRatio < 0.75f)
                        shootBulbGateValue *= 0.5f;

                    if (absValueOfTimer % shootBulbGateValue == 0f)
                    {
                        float projectileSpeed = 12f;
                        int projectileType = ModContent.ProjectileType<HomingGasBulb>();
                        int damage = npc.GetProjectileDamage(projectileType);
                        Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);
                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + projectileVelocity * 3f, projectileVelocity * projectileSpeed, projectileType, damage, 0f, Main.myPlayer);
                    }
                }
            }

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

            if (num738 >= maxDistanceFromHooks)
            {
                num738 = maxDistanceFromHooks / num738;
                num736 *= num738;
                num737 *= num738;
            }

            num730 += num736;
            num731 += num737;
            vector91 = new Vector2(npc.Center.X, npc.Center.Y);
            num736 = num730 - vector91.X;
            num737 = num731 - vector91.Y;
            num738 = (float)Math.Sqrt(num736 * num736 + num737 * num737);

            if (num738 < velocity)
            {
                num736 = npc.velocity.X;
                num737 = npc.velocity.Y;
            }
            else
            {
                num738 = velocity / num738;
                num736 *= num738;
                num737 *= num738;
            }

            if (npc.velocity.X < num736)
            {
                npc.velocity.X += acceleration;
                if (npc.velocity.X < 0f && num736 > 0f)
                    npc.velocity.X += acceleration * 2f;
            }
            else if (npc.velocity.X > num736)
            {
                npc.velocity.X -= acceleration;
                if (npc.velocity.X > 0f && num736 < 0f)
                    npc.velocity.X -= acceleration * 2f;
            }
            if (npc.velocity.Y < num737)
            {
                npc.velocity.Y += acceleration;
                if (npc.velocity.Y < 0f && num737 > 0f)
                    npc.velocity.Y += acceleration * 2f;
            }
            else if (npc.velocity.Y > num737)
            {
                npc.velocity.Y -= acceleration;
                if (npc.velocity.Y > 0f && num737 < 0f)
                    npc.velocity.Y -= acceleration * 2f;
            }

            // Rotation
            float num740 = Main.player[npc.target].Center.X - npc.Center.X;
            float num741 = Main.player[npc.target].Center.Y - npc.Center.Y;
            npc.rotation = (float)Math.Atan2(num741, num740) + MathHelper.PiOver2;

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
                if (Main.netMode != NetmodeID.MultiplayerClient && !usingSeedGatling && !slowedAfterGatlingAttack)
                {
                    float shootBoost = 2f * (1f - lifeRatio);
                    npc.localAI[1] += 1f + shootBoost;

                    if (enrage)
                        npc.localAI[1] += 2f;

                    if (Main.getGoodWorld)
                        npc.localAI[1] += 1f;

                    // If hit, fire projectiles even if target is behind tiles
                    if (npc.justHit)
                        npc.localAI[3] = 1f;

                    float shootProjectileGateValue = death ? 40f : 60f;
                    if (npc.localAI[1] >= 60f)
                    {
                        npc.localAI[1] = 0f;
                        bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                        if (npc.localAI[3] > 0f)
                        {
                            canHit = true;
                            npc.localAI[3] = 0f;
                        }
                        if (canHit)
                        {
                            npc.TargetClosest();
                            int projectileType = (CalamityWorld.LegendaryMode || Main.rand.NextBool(4)) ? ProjectileID.PoisonSeedPlantera : ProjectileID.SeedPlantera;
                            float projectileSpeed = 9f;
                            int damage = npc.GetProjectileDamage(projectileType);
                            Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + projectileVelocity * 3f, projectileVelocity * projectileSpeed, projectileType, damage, 0f, Main.myPlayer);
                        }
                    }
                }
            }

            // Phase 2
            else
            {
                // Spore dust
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 44, 0f, 0f, 250, default, 0.4f);
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

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // If hit, fire projectiles even if target is behind tiles
                    if (npc.justHit)
                        calamityGlobalNPC.newAI[2] = 1f;

                    // Fire spread of poison seeds
                    npc.localAI[3] += 0.5f + (0.5f - lifeRatio) * 8f;

                    // If hit, fire projectiles even if target is behind tiles
                    if (npc.justHit)
                        calamityGlobalNPC.newAI[3] = 1f;

                    if (npc.localAI[3] >= 450f)
                    {
                        bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                        if (calamityGlobalNPC.newAI[3] > 0f)
                        {
                            canHit = true;
                            calamityGlobalNPC.newAI[3] = 0f;
                        }
                        if (canHit)
                        {
                            float projectileSpeed = 8f + ((0.5f - lifeRatio) * 8f); // 8f to 12f
                            if (bossRush)
                                projectileSpeed += 4f;

                            Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);

                            bool anyThornBalls = false;
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && Main.projectile[i].type == ProjectileID.ThornBall)
                                    anyThornBalls = true;

                                if (anyThornBalls)
                                    break;
                            }
                            if (!anyThornBalls && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                            {
                                int type = ProjectileID.ThornBall;
                                int damage = npc.GetProjectileDamage(type);

                                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + projectileVelocity * 3f, projectileVelocity * projectileSpeed, type, damage, 0f, Main.myPlayer);
                                Main.projectile[proj].tileCollide = false;

                                npc.localAI[3] = -240f;
                            }
                            else
                            {
                                int spread = 3 + (int)Math.Round((0.5f - lifeRatio) * 10f); // 3 to 8, wider spread is harder to avoid
                                int numProj = spread / 2;
                                int type = ProjectileID.PoisonSeedPlantera;
                                int damage = npc.GetProjectileDamage(type);

                                float rotation = MathHelper.ToRadians(spread);
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), perturbedSpeed * 3f, perturbedSpeed * projectileSpeed, type, damage, 0f, Main.myPlayer);
                                }

                                npc.localAI[3] = 0f;
                            }
                        }
                    }

                    // Fire spread of spore clouds
                    float shootBoost = 2f * (0.5f - lifeRatio);
                    calamityGlobalNPC.newAI[0] += 1f + shootBoost;

                    float sporeGasGateValue = CalamityWorld.LegendaryMode ? 150f : bossRush ? 300f : 720f;
                    if (calamityGlobalNPC.newAI[0] >= sporeGasGateValue)
                    {
                        npc.TargetClosest();
                        SoundEngine.PlaySound(SoundID.Item20, npc.position);

                        float projectileSpeed = 3f;
                        Vector2 projectileVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * projectileSpeed;
                        int type = ModContent.ProjectileType<SporeGasPlantera>();
                        int damage = npc.GetProjectileDamage(type);
                        int numProj = 4;
                        int spread = 30;
                        float rotation = MathHelper.ToRadians(spread);
                        float baseSpeed = (float)Math.Sqrt(projectileVelocity.X * projectileVelocity.X + projectileVelocity.Y * projectileVelocity.Y);
                        double startAngle = Math.Atan2(projectileVelocity.X, projectileVelocity.Y) - rotation / 2;
                        double deltaAngle = rotation / numProj;
                        double offsetAngle;

                        for (int i = 0; i < numProj; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * i;
                            float ai0 = Main.rand.Next(3);
                            projectileVelocity = new Vector2(baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle));
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + projectileVelocity, projectileVelocity, type, damage, 0f, Main.myPlayer, ai0, 0f);
                        }

                        calamityGlobalNPC.newAI[0] = 0f;
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
                    int healthInterval = death ? (int)(npc.lifeMax * 0.08) : (int)(npc.lifeMax * 0.1);
                    if ((npc.life + healthInterval) < npc.ai[0])
                    {
                        npc.ai[0] = npc.life;

                        if (NPC.CountNPCS(NPCID.PlanterasTentacle) < maxTentacles)
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasTentacle, npc.whoAmI);
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

            // Spore dust
            if (Main.rand.NextBool(10))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 44, 0f, 0f, 250, default, 0.2f);
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

            // Despawn if Plantera is gone
            if (NPC.plantBoss < 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    npc.StrikeInstantKill();

                return false;
            }

            // 3 seconds of resistance and no damage to prevent spawn killing and unfair hits
            if (calamityGlobalNPC.newAI[1] < 90f)
            {
                npc.damage = 0;
                calamityGlobalNPC.newAI[1] += 1f;
            }
            else
                npc.damage = npc.defDamage;

            // Set Plantera to a variable
            int num778 = NPC.plantBoss;

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
            float num781 = npc.ai[2];
            float num780 = 100f + (num781 * 75f);
            float deceleration = (death ? 0.5f : 0.8f) / (1f + num781);

            if (Main.getGoodWorld)
                num779 += 4f;

            // Despawn if Plantera is gone
            if (!Main.npc[num778].active || num778 < 0)
            {
                npc.active = false;
                return false;
            }

            // Movement
            Vector2 planteraCenter = Main.npc[num778].Center;
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

            float velocityLimit = 12f + 6f * num781;
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
