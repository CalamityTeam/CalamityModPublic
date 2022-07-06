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
            bool phase1phase2 = lifeRatio < 0.75f;
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

            // Detect active tiles around Plantera
            int radius = 20; // 20 tile radius
            int diameter = radius * 2;
            int npcCenterX = (int)(npc.Center.X / 16f);
            int npcCenterY = (int)(npc.Center.Y / 16f);
            Rectangle area = new Rectangle(npcCenterX - radius, npcCenterY - radius, diameter, diameter);
            int nearbyActiveTiles = 0; // 0 to 1600
            for (int x = area.Left; x < area.Right; x++)
            {
                for (int y = area.Top; y < area.Bottom; y++)
                {
                    if (Main.tile[x, y] != null)
                    {
                        if (Main.tile[x, y].HasUnactuatedTile && Main.tileSolid[Main.tile[x, y].TileType] && !Main.tileSolidTop[Main.tile[x, y].TileType] && !TileID.Sets.Platforms[Main.tile[x, y].TileType])
                            nearbyActiveTiles++;
                    }
                }
            }

            // Scale multiplier based on nearby active tiles
            float tileEnrageMult = 1f;
            if (nearbyActiveTiles < 800)
                tileEnrageMult += (800 - nearbyActiveTiles) * 0.00075f; // Ranges from 1f to 1.6f

            if (bossRush)
                tileEnrageMult = 1.6f;

            // Let hooks and tentacles know how enraged plantera is
            npc.ai[3] = tileEnrageMult;
            float tentacleEnrageMult = 1f - lifeRatio + (tileEnrageMult - 1f);

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

            // Increase speed based on nearby active tiles
            velocity *= tileEnrageMult;
            acceleration *= tileEnrageMult;

            if (Main.getGoodWorld)
            {
                velocity *= 1.15f;
                acceleration *= 1.15f;
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

            // Slow down considerably if near player
            if (!speedUp && nearbyActiveTiles > 800 && !Collision.SolidCollision(npc.position, npc.width, npc.height) && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
            {
                if (npc.velocity.Length() > 4f)
                    npc.velocity *= 0.98f;
            }

            // Rotation
            float num740 = Main.player[npc.target].Center.X - npc.Center.X;
            float num741 = Main.player[npc.target].Center.Y - npc.Center.Y;
            npc.rotation = (float)Math.Atan2(num741, num740) + MathHelper.PiOver2;

            // Phase 1
            if (!phase2)
            {
                if (phase1phase2 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.localAI[0] == 1f)
                    {
                        npc.localAI[0] = 2f;
                        int baseTentacles = death ? 3 : 2;
                        int totalTentacles = (int)(baseTentacles * tileEnrageMult);
                        for (int i = 0; i < totalTentacles; i++)
                        {
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasTentacle, npc.whoAmI, 0f, 0f, tentacleEnrageMult);
                        }
                    }
                }

                // Adjust stats
                calamityGlobalNPC.DR = 0.15f;
                calamityGlobalNPC.unbreakableDR = false;
                npc.defense = 32;
                npc.damage = npc.defDamage;

                // Fire projectiles
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float shootBoost = 8f * (1f - lifeRatio);
                    npc.localAI[1] += 1f + shootBoost;

                    if (enrage)
                        npc.localAI[1] += 2f;

                    if (Main.getGoodWorld)
                        npc.localAI[1] += 1f;

                    // If hit, fire projectiles even if target is behind tiles
                    if (npc.justHit)
                        npc.localAI[3] = 1f;

                    if (npc.localAI[1] >= 75f)
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
                            int projectileType = ProjectileID.SeedPlantera;
                            float projectileVelocity = 17f;
                            int chance = bossRush ? 2 : 6;
                            if (Main.rand.NextBool(chance))
                            {
                                projectileType = ModContent.ProjectileType<SporeGasPlantera>();
                                projectileVelocity = 3f;
                            }
                            if (lifeRatio < 0.9f && Main.rand.NextBool(3))
                            {
                                npc.localAI[1] = -30f;
                                projectileType = ProjectileID.PoisonSeedPlantera;
                            }
                            else if (lifeRatio < 0.8f && Main.rand.NextBool(4))
                            {
                                int thornBallCount = 0;
                                for (int i = 0; i < Main.maxProjectiles; i++)
                                {
                                    if (Main.projectile[i].active && Main.projectile[i].type == ProjectileID.ThornBall)
                                        thornBallCount++;

                                    if (thornBallCount > 1)
                                        break;
                                }
                                if (thornBallCount < 2)
                                {
                                    npc.localAI[1] = -120f;
                                    projectileType = ProjectileID.ThornBall;
                                }
                            }

                            int damage = npc.GetProjectileDamage(projectileType);

                            Vector2 vector93 = npc.Center;
                            float num743 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector93.X;
                            float num744 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector93.Y;
                            float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);
                            num745 = projectileVelocity / num745;
                            num743 *= num745;
                            num744 *= num745;

                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), vector93.X, vector93.Y, num743, num744, projectileType, damage, 0f, Main.myPlayer, 0f, projectileType == ProjectileID.ThornBall ? tileEnrageMult : 0f);
                        }
                    }
                }
            }

            // Phase 2
            else
            {
                // Adjust stats
                calamityGlobalNPC.DR = 0.15f;
                calamityGlobalNPC.unbreakableDR = false;
                npc.defense = 10;
                npc.damage = (int)(npc.defDamage * 1.4f);

                // Spawn tentacles
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.localAI[0] == 2f)
                    {
                        npc.localAI[0] = 3f;
                        int baseTentacles = death ? 10 : 8;
                        int totalTentacles = (int)(baseTentacles * tileEnrageMult);
                        if (Main.getGoodWorld)
                            totalTentacles += 6;

                        for (int i = 0; i < totalTentacles; i++)
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasTentacle, npc.whoAmI, 0f, 0f, tentacleEnrageMult);

                        if (Main.getGoodWorld)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].aiStyle == 52)
                                {
                                    for (int j = 0; j < totalTentacles / 2 - 1; j++)
                                    {
                                        int num800 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasTentacle, npc.whoAmI, 0f, 0f, tentacleEnrageMult);
                                        Main.npc[num800].ai[3] = i + 1;
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
                    // Spawn spores
                    float spawnBoost = 8f * (0.5f - lifeRatio);
                    npc.localAI[1] += 1f + spawnBoost;

                    // If hit, fire projectiles even if target is behind tiles
                    if (npc.justHit)
                        calamityGlobalNPC.newAI[2] = 1f;

                    if (npc.localAI[1] >= 360f)
                    {
                        npc.localAI[1] = 0f;
                        bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                        if (calamityGlobalNPC.newAI[2] > 0f)
                        {
                            canHit = true;
                            calamityGlobalNPC.newAI[2] = 0f;
                        }
                        if (canHit)
                        {
                            float num757 = 4f;
                            Vector2 vector94 = npc.Center;
                            float num758 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector94.X + Main.rand.Next(-10, 11);
                            float num759 = Math.Abs(num758 * 0.2f);

                            float num760 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector94.Y + Main.rand.Next(-10, 11);
                            if (num760 > 0f)
                                num759 = 0f;

                            num760 -= num759;
                            float num761 = (float)Math.Sqrt(num758 * num758 + num760 * num760);
                            num761 = num757 / num761;
                            num758 *= num761;
                            num760 *= num761;

                            int num762 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.Spore);
                            Main.npc[num762].velocity.X = num758;
                            Main.npc[num762].velocity.Y = num760;
                            Main.npc[num762].dontTakeDamage = bossRush;
                            Main.npc[num762].netUpdate = true;
                        }
                    }

                    // Fire spread of poison seeds
                    if (nearbyActiveTiles > 600)
                        npc.localAI[3] += 0.5f + (0.5f - lifeRatio) * 8f;
                    else
                        npc.localAI[3] += (nearbyActiveTiles > 300 ? 1f : 5f) + (0.5f - lifeRatio) * 8f;

                    // If hit, fire projectiles even if target is behind tiles
                    if (npc.justHit)
                        calamityGlobalNPC.newAI[3] = 1f;

                    if (npc.localAI[3] >= 360f)
                    {
                        bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                        if (calamityGlobalNPC.newAI[3] > 0f)
                        {
                            canHit = true;
                            calamityGlobalNPC.newAI[3] = 0f;
                        }
                        if (canHit)
                        {
                            Vector2 vector93 = npc.Center;

                            float num742 = 8f + ((0.5f - lifeRatio) * 8f); // 8f to 12f
                            if (nearbyActiveTiles < 300)
                                num742 = 12f;
                            if (bossRush)
                                num742 += 4f;

                            float num743 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector93.X;
                            float num744 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector93.Y;
                            float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);
                            num745 = num742 / num745;
                            num743 *= num745;
                            num744 *= num745;
                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;

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

                                Projectile.NewProjectile(npc.GetSource_FromAI(), vector93.X, vector93.Y, num743, num744, type, damage, 0f, Main.myPlayer, 0f, tileEnrageMult);

                                npc.localAI[3] = -240f;
                            }
                            else
                            {
                                int spread = 3 + (int)Math.Round((0.5f - lifeRatio) * 10f); // 3 to 8, wider spread is harder to avoid
                                if (nearbyActiveTiles < 300)
                                    spread = Main.rand.Next(3, 7) + (int)Math.Round((0.5f - lifeRatio) * 8f);

                                int numProj = spread / 2;
                                int type = ProjectileID.PoisonSeedPlantera;
                                int damage = npc.GetProjectileDamage(type);

                                float rotation = MathHelper.ToRadians(spread);
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 perturbedSpeed = new Vector2(num743, num744).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), vector93, perturbedSpeed, type, damage, 0f, Main.myPlayer, 0f, 0f);
                                }

                                npc.localAI[3] = 0f;
                            }
                        }
                    }

                    // Fire spread of spore clouds
                    float shootBoost = 2f * (0.5f - lifeRatio);
                    calamityGlobalNPC.newAI[0] += 1f + shootBoost;

                    float sporeGasGateValue = bossRush ? 300f : 600f;
                    if (calamityGlobalNPC.newAI[0] >= sporeGasGateValue)
                    {
                        npc.TargetClosest();
                        SoundEngine.PlaySound(SoundID.Item20, npc.position);

                        Vector2 vector93 = npc.Center;
                        float num742 = 3f;
                        float num743 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector93.X;
                        float num744 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector93.Y;
                        float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);
                        num745 = num742 / num745;
                        num743 *= num745;
                        num744 *= num745;
                        vector93.X += num743 * 3f;
                        vector93.Y += num744 * 3f;

                        int type = ModContent.ProjectileType<SporeGasPlantera>();
                        int damage = npc.GetProjectileDamage(type);

                        int numProj = 4;
                        int spread = 30;
                        if (nearbyActiveTiles <= 300)
                            spread = Main.rand.NextBool(2) ? 45 : 60;

                        float rotation = MathHelper.ToRadians(spread);
                        float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
                        double startAngle = Math.Atan2(num743, num744) - rotation / 2;
                        double deltaAngle = rotation / numProj;
                        double offsetAngle;

                        for (int i = 0; i < numProj; i++)
                        {
                            offsetAngle = startAngle + deltaAngle * i;
                            float ai0 = Main.rand.Next(3);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, ai0, 0f);
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
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasTentacle, npc.whoAmI, 0f, 0f, tentacleEnrageMult);
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
                npc.StrikeNPCNoInteraction(9999, 0f, 0, false, false, false);
                npc.netUpdate = true;
                return false;
            }

            // Percent life remaining, Plantera
            float lifeRatio = Main.npc[NPC.plantBoss].life / (float)Main.npc[NPC.plantBoss].lifeMax;

            // Despawn if Plantera's target is dead
            if (Main.player[Main.npc[NPC.plantBoss].target].dead && !enrage)
                despawn = true;

            // Tile enrage
            float tileEnrageMult = Main.npc[NPC.plantBoss].ai[3];

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
                npc.localAI[0] -= (1f + moveBoost) * MathHelper.Lerp(1f, 3.6f, (tileEnrageMult - 1f) * 1.25f);
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
                float velocity = (7f + velocityBoost) * tileEnrageMult;
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
                    npc.velocity.X = num773;
                    npc.velocity.Y = num774;
                }
                else
                {
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

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Despawn if Plantera is gone
            if (NPC.plantBoss < 0)
            {
                npc.StrikeNPCNoInteraction(9999, 0f, 0, false, false, false);
                npc.netUpdate = true;
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
            float tileEnrageMult = Main.npc[num778].ai[3];

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
            float num779 = (death ? 2.4f : 1.6f) * tileEnrageMult;
            float num781 = npc.ai[2];
            float num780 = 100f + (num781 * 150f);
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
