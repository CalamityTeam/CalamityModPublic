using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class SkeletronPrimeAI
    {
        // Master Mode changes
        // 1 - Charges far quicker, 
        // 2 - Arms and head accelerate extremely fast, 
        // 3 - Arms remain closer to the head and resist piercing
        public static bool BuffedSkeletronPrimeAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && malice;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Spawn arms
            if (npc.ai[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.ai[0] = 1f;

                int arm = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PrimeCannon, npc.whoAmI);
                Main.npc[arm].ai[0] = -1f;
                Main.npc[arm].ai[1] = npc.whoAmI;
                Main.npc[arm].target = npc.target;
                Main.npc[arm].netUpdate = true;

                arm = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PrimeSaw, npc.whoAmI);
                Main.npc[arm].ai[0] = 1f;
                Main.npc[arm].ai[1] = npc.whoAmI;
                Main.npc[arm].target = npc.target;
                Main.npc[arm].netUpdate = true;

                arm = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PrimeVice, npc.whoAmI);
                Main.npc[arm].ai[0] = -1f;
                Main.npc[arm].ai[1] = npc.whoAmI;
                Main.npc[arm].target = npc.target;
                Main.npc[arm].ai[3] = 150f;
                Main.npc[arm].netUpdate = true;

                arm = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PrimeLaser, npc.whoAmI);
                Main.npc[arm].ai[0] = 1f;
                Main.npc[arm].ai[1] = npc.whoAmI;
                Main.npc[arm].target = npc.target;
                Main.npc[arm].netUpdate = true;
                Main.npc[arm].ai[3] = 150f;
            }

            // Check if arms are alive
            bool cannonAlive = false;
            bool laserAlive = false;
            bool viceAlive = false;
            bool sawAlive = false;
            if (CalamityGlobalNPC.primeCannon != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeCannon].active)
                    cannonAlive = true;
            }
            if (CalamityGlobalNPC.primeLaser != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeLaser].active)
                    laserAlive = true;
            }
            if (CalamityGlobalNPC.primeVice != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeVice].active)
                    viceAlive = true;
            }
            if (CalamityGlobalNPC.primeSaw != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeSaw].active)
                    sawAlive = true;
            }
            bool allArmsDead = !cannonAlive && !laserAlive && !viceAlive && !sawAlive;
            npc.chaseable = allArmsDead;

            // Inflict 0 damage for 3 seconds after spawning
            if (calamityGlobalNPC.newAI[2] < 180f)
            {
                calamityGlobalNPC.newAI[2] += 1f;
                npc.damage = 0;
            }

            // Set stats
            if (npc.ai[1] == 5f)
                npc.damage = 0;
            else if (allArmsDead)
                npc.damage = npc.defDamage;

            npc.defense = npc.defDefense;

            // Phases
            bool phase2 = lifeRatio < 0.5f;
            bool phase3 = lifeRatio < 0.25f;

            // Kill all arms if Prime Head enters phase 2
            if (phase2 && !allArmsDead)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc2 = Main.npc[i];
                    if (npc2.type == NPCID.PrimeCannon || npc2.type == NPCID.PrimeLaser || npc2.type == NPCID.PrimeSaw || npc2.type == NPCID.PrimeVice)
                    {
                        npc2.life = -1;
                        npc2.HitEffect(0, 10.0);
                        npc2.active = false;
                        npc2.netUpdate = true;
                    }
                }
            }

            // Despawn
            if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 6000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 6000f)
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 6000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 6000f)
                    npc.ai[1] = 3f;
            }

            // Activate daytime enrage
            if (Main.dayTime && !BossRushEvent.BossRushActive && npc.ai[1] != 3f && npc.ai[1] != 2f)
            {
                // Heal
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int healAmt = npc.life - 300;
                    if (healAmt < 0)
                    {
                        int absHeal = Math.Abs(healAmt);
                        npc.life += absHeal;
                        npc.HealEffect(absHeal, true);
                        npc.netUpdate = true;
                    }
                }

                npc.ai[1] = 2f;
                Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
            }

            // Adjust slowing debuff immunity
            bool immuneToSlowingDebuffs = npc.ai[1] == 5f;
            npc.buffImmune[ModContent.BuffType<ExoFreeze>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<KamiDebuff>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TimeSlow>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TeslaFreeze>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

            bool normalLaserRotation = calamityGlobalNPC.newAI[3] % 2f == 0f;

            // Float near player
            if (npc.ai[1] == 0f || npc.ai[1] == 4f)
            {
                // Start other phases if arms are dead, start with spinning phase
                if (allArmsDead)
                {
                    // Start spin phase after 5 seconds
                    npc.ai[2] += phase3 ? 1.25f : 1f;
                    if (npc.ai[2] >= (300f - (death ? 100f * (1f - lifeRatio) : 0f)))
                    {
                        bool shouldSpinAroundTarget = npc.ai[1] == 4f && npc.position.Y < Main.player[npc.target].position.Y - 400f &&
                            Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 600f && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 400f;

                        if (shouldSpinAroundTarget || npc.ai[1] != 4f)
                        {
                            if (shouldSpinAroundTarget)
                                npc.ai[3] = Vector2.Distance(Main.player[npc.target].Center, npc.Center);

                            npc.ai[2] = 0f;
                            npc.ai[1] = shouldSpinAroundTarget ? 5f : 1f;
                            npc.TargetClosest();
                            npc.netUpdate = true;
                        }
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Ring of lasers and rocket spread if all arms are dead
                    if (allArmsDead)
                    {
                        npc.localAI[1] += 1f;
                        if (phase3)
                            npc.localAI[1] += 0.5f;

                        if (npc.localAI[1] >= 240f)
                        {
                            npc.localAI[1] = 0f;

                            int totalProjectiles = malice ? 24 : 12;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = ProjectileID.DeathLaser;
                            int damage = npc.GetProjectileDamage(type);
                            float velocity = 4f;
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                int proj = Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer);
                                Main.projectile[proj].timeLeft = 480;
                            }
                            calamityGlobalNPC.newAI[3] += 1f;
                        }

                        npc.localAI[2] += 1f;
                        if (phase3)
                            npc.localAI[2] += 0.25f;

                        if (npc.localAI[2] >= 200f)
                        {
                            npc.localAI[2] = 0f;
                            float num502 = 0.5f;
                            int type = ProjectileID.RocketSkeleton;
                            int damage = npc.GetProjectileDamage(type);
                            Vector2 value19 = Main.player[npc.target].Center - npc.Center;
                            value19.Normalize();
                            value19 *= num502;
                            int numProj = 2;
                            float rotation = MathHelper.ToRadians(5);
                            if (malice)
                            {
                                rotation = MathHelper.ToRadians(8);
                                for (int i = 0; i < numProj; i++)
                                {
                                    Projectile.NewProjectile(npc.Center, value19.RotatedBy(-rotation * (i + 1)), type, damage, 0f, Main.myPlayer, 0f, 1f);
                                    Projectile.NewProjectile(npc.Center, value19.RotatedBy(+rotation * (i + 1)), type, damage, 0f, Main.myPlayer, 0f, 1f);
                                }
                                Projectile.NewProjectile(npc.Center, value19, type, damage, 0f, Main.myPlayer, 0f, 1f);
                            }
                            else
                            {
                                for (int i = 0; i < numProj + 1; i++)
                                {
                                    Vector2 perturbedSpeed = value19.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                                    Projectile.NewProjectile(npc.Center, perturbedSpeed, type, damage, 0f, Main.myPlayer, 0f, 1f);
                                }
                            }
                        }
                    }
                }

                npc.rotation = npc.velocity.X / 15f;

                float velocityY = 3f - (death ? 1f - lifeRatio : 0f);
                float velocityX = 7f - (death ? 3.5f * (1f - lifeRatio) : 0f);
                float acceleration = 0.1f + (death ? 0.05f * (1f - lifeRatio) : 0f);

                if (!cannonAlive)
                {
                    velocityY -= 0.35f;
                    acceleration += 0.025f;
                }
                if (!laserAlive)
                {
                    velocityY -= 0.35f;
                    acceleration += 0.025f;
                }
                if (!viceAlive)
                {
                    velocityY -= 0.35f;
                    acceleration += 0.025f;
                }
                if (!sawAlive)
                {
                    velocityY -= 0.35f;
                    acceleration += 0.025f;
                }

                if (malice)
                {
                    velocityY = 0.25f;
                    velocityX = 0.5f;
                    acceleration = 0.3f;
                }

                // Reduce acceleration if target is holding a true melee weapon
                Item targetSelectedItem = Main.player[npc.target].inventory[Main.player[npc.target].selectedItem];
                if (targetSelectedItem.melee && (targetSelectedItem.shoot == ProjectileID.None || targetSelectedItem.Calamity().trueMelee))
                {
                    acceleration *= 0.5f;
                }

                if (npc.position.Y > Main.player[npc.target].position.Y - 350f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.98f;

                    npc.velocity.Y -= acceleration;

                    if (npc.velocity.Y > velocityY)
                        npc.velocity.Y = velocityY;
                }
                else if (npc.position.Y < Main.player[npc.target].position.Y - 500f)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.98f;

                    npc.velocity.Y += acceleration;

                    if (npc.velocity.Y < -velocityY)
                        npc.velocity.Y = -velocityY;
                }

                if (npc.position.X + (npc.width / 2) > Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + 150f)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.98f;

                    npc.velocity.X -= acceleration;

                    if (npc.velocity.X > velocityX)
                        npc.velocity.X = velocityX;
                }
                if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - 150f)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.98f;

                    npc.velocity.X += acceleration;

                    if (npc.velocity.X < -velocityX)
                        npc.velocity.X = -velocityX;
                }
            }

            else
            {
                // Spinning
                if (npc.ai[1] == 1f)
                {
                    npc.defense *= 2;
                    npc.damage *= 2;

                    if (phase2 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[1] += 1f;
                        if (npc.localAI[1] >= 45f)
                        {
                            npc.localAI[1] = 0f;

                            int totalProjectiles = malice ? 24 : 12;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = ProjectileID.DeathLaser;
                            int damage = npc.GetProjectileDamage(type);
                            float velocity = 5f;
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                int proj = Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer);
                                Main.projectile[proj].timeLeft = 480;
                            }
                            calamityGlobalNPC.newAI[3] += 1f;
                        }
                    }

                    npc.ai[2] += 1f;
                    if (npc.ai[2] == 2f)
                        Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

                    // Spin for 3 seconds then return to floating phase
                    float phaseTimer = 240f;
                    if (phase2 && !phase3)
                        phaseTimer += 60f;

                    if (npc.ai[2] >= (phaseTimer - (death ? 60f * (1f - lifeRatio) : 0f)))
                    {
                        npc.TargetClosest();
                        npc.ai[2] = 0f;
                        npc.ai[1] = 4f;
                    }

                    npc.rotation += npc.direction * 0.3f;
                    Vector2 vector48 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num455 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector48.X;
                    float num456 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector48.Y;
                    float num457 = (float)Math.Sqrt(num455 * num455 + num456 * num456);

                    float speed = BossRushEvent.BossRushActive ? 9f : 6f;
                    if (phase2)
                        speed += 0.5f;
                    if (phase3)
                        speed += 0.5f;

                    if (num457 > 150f)
                    {
                        float baseDistanceVelocityMult = 1f + MathHelper.Clamp((num457 - 150f) * 0.0015f, 0.05f, 1.5f);
                        speed *= baseDistanceVelocityMult;
                    }

                    num457 = speed / num457;
                    npc.velocity.X = num455 * num457;
                    npc.velocity.Y = num456 * num457;
                }

                // Daytime enrage
                if (npc.ai[1] == 2f)
                {
                    npc.damage = 1000;
                    calamityGlobalNPC.DR = 0.9999f;
                    calamityGlobalNPC.unbreakableDR = true;

                    npc.rotation += npc.direction * 0.3f;

                    Vector2 vector49 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num458 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector49.X;
                    float num459 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector49.Y;
                    float num460 = (float)Math.Sqrt(num458 * num458 + num459 * num459);

                    float num461 = 10f;
                    num461 += num460 / 100f;
                    if (num461 < 8f)
                        num461 = 8f;
                    if (num461 > 32f)
                        num461 = 32f;

                    num460 = num461 / num460;
                    npc.velocity.X = num458 * num460;
                    npc.velocity.Y = num459 * num460;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[0] += 1f;
                        if (npc.localAI[0] >= 60f)
                        {
                            npc.localAI[0] = 0f;
                            Vector2 vector16 = npc.Center;
                            if (Collision.CanHit(vector16, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                            {
                                float num159 = 7f;
                                float num160 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector16.X + Main.rand.Next(-20, 21);
                                float num161 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector16.Y + Main.rand.Next(-20, 21);
                                float num162 = (float)Math.Sqrt(num160 * num160 + num161 * num161);
                                num162 = num159 / num162;
                                num160 *= num162;
                                num161 *= num162;

                                Vector2 value = new Vector2(num160 * 1f + Main.rand.Next(-50, 51) * 0.01f, num161 * 1f + Main.rand.Next(-50, 51) * 0.01f);
                                value.Normalize();
                                value *= num159;
                                value += npc.velocity;
                                num160 = value.X;
                                num161 = value.Y;

                                int type = ProjectileID.Skull;
                                vector16 += value * 5f;
                                int num165 = Projectile.NewProjectile(vector16.X, vector16.Y, num160, num161, type, 250, 0f, Main.myPlayer, -1f, 0f);
                                Main.projectile[num165].timeLeft = 300;
                            }
                        }
                    }
                }

                // Despawning
                if (npc.ai[1] == 3f)
                {
                    npc.velocity.Y += 0.1f;
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.95f;

                    npc.velocity.X *= 0.95f;

                    if (npc.timeLeft > 500)
                        npc.timeLeft = 500;
                }

                // Fly around target in a circle
                if (npc.ai[1] == 5f)
                {
                    npc.ai[2] += 1f;

                    npc.rotation = npc.velocity.X / 30f;

                    // Spin for 3 seconds
                    float spinVelocity = 45f;
                    if (npc.ai[2] == 2f)
                    {
                        // Play angry noise
                        Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

                        // Set spin direction
                        if (Main.player[npc.target].velocity.X > 0f)
                            calamityGlobalNPC.newAI[0] = 1f;
                        else if (Main.player[npc.target].velocity.X < 0f)
                            calamityGlobalNPC.newAI[0] = -1f;
                        else
                            calamityGlobalNPC.newAI[0] = Main.player[npc.target].direction;

                        // Set spin velocity
                        npc.velocity.X = MathHelper.Pi * npc.ai[3] / spinVelocity;
                        npc.velocity *= -calamityGlobalNPC.newAI[0];
                        npc.netUpdate = true;
                    }

                    // Maintain velocity and spit skulls
                    else
                    {
                        npc.velocity = npc.velocity.RotatedBy(MathHelper.Pi / spinVelocity * -calamityGlobalNPC.newAI[0]);
                        float skullSpawnDivisor = BossRushEvent.BossRushActive ? 15f : malice ? 20f : death ? 30f - (float)Math.Round(10f * (1f - lifeRatio)) : 30f;
                        if (npc.ai[2] % skullSpawnDivisor == 0f)
                        {
                            calamityGlobalNPC.newAI[1] += 1f;

                            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 64f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 vector16 = npc.Center;
                                    float num159 = 4f + (death ? 2f * (1f - lifeRatio) : 0f);
                                    float num160 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector16.X + Main.rand.Next(-20, 21);
                                    float num161 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector16.Y + Main.rand.Next(-20, 21);
                                    float num162 = (float)Math.Sqrt(num160 * num160 + num161 * num161);
                                    num162 = num159 / num162;
                                    num160 *= num162;
                                    num161 *= num162;

                                    Vector2 value = new Vector2(num160 * 1f + Main.rand.Next(-50, 51) * 0.01f, num161 * 1f + Main.rand.Next(-50, 51) * 0.01f);
                                    value.Normalize();
                                    value *= num159;
                                    num160 = value.X;
                                    num161 = value.Y;

                                    int type = ProjectileID.Skull;
                                    int damage = npc.GetProjectileDamage(type);
                                    vector16 += value * 5f;
                                    int num165 = Projectile.NewProjectile(vector16.X, vector16.Y, num160, num161, type, damage, 0f, Main.myPlayer, -1f, 0f);
                                    Main.projectile[num165].timeLeft = 480;
                                    Main.projectile[num165].tileCollide = false;
                                }
                            }

                            // Go to floating phase, or spinning phase if in phase 2
                            if (calamityGlobalNPC.newAI[1] >= 6f)
                            {
                                npc.velocity.Normalize();

                                // Fly overhead and spit missiles if on low health
                                npc.ai[1] = phase3 ? 6f : 1f;
                                npc.ai[2] = 0f;
                                npc.ai[3] = 0f;
                                calamityGlobalNPC.newAI[1] = 0f;
                                calamityGlobalNPC.newAI[0] = 0f;
                                npc.TargetClosest();
                                npc.netUpdate = true;
                            }
                        }
                    }
                }

                // Fly overhead and spit missiles
                if (npc.ai[1] == 6f)
                {
                    npc.damage = 0;

                    npc.rotation = npc.velocity.X / 15f;

                    float flightVelocity = malice ? 25f : 15f + (death ? 5f * (1f - lifeRatio) : 0f);
                    float flightAcceleration = malice ? 0.25f : 0.15f + (death ? 0.5f * (1f - lifeRatio) : 0f);

                    Vector2 destination = new Vector2(Main.player[npc.target].Center.X, Main.player[npc.target].Center.Y - 500f);
                    npc.SimpleFlyMovement(Vector2.Normalize(destination - npc.Center) * flightVelocity, flightAcceleration);

                    // Spit homing missiles and then go to floating phase
                    if (Vector2.Distance(npc.Center, destination) < 160f || npc.ai[2] > 0f)
                    {
                        float missileSpawnDivisor = death ? 12f - (float)Math.Round(4f * (1f - lifeRatio)) : 12f;
                        npc.ai[2] += 1f;
                        if (npc.ai[2] % missileSpawnDivisor == 0f)
                        {
                            calamityGlobalNPC.newAI[1] += 1f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 velocity = new Vector2(-1f * (float)Main.rand.NextDouble() * 3f, 1f);
                                velocity = velocity.RotatedBy((Main.rand.NextDouble() - 0.5) * MathHelper.PiOver4);
                                velocity *= 5f;
                                int type = ProjectileID.SaucerMissile;
                                int damage = npc.GetProjectileDamage(type);
                                float delayBeforeHoming = malice ? 25f : 45f;
                                Projectile.NewProjectile(npc.Center.X + Main.rand.Next(npc.width / 2), npc.Center.Y + 4f, velocity.X, velocity.Y, type, damage, 0f, Main.myPlayer, 0f, delayBeforeHoming);
                            }

                            Main.PlaySound(SoundID.Item39, npc.Center);

                            if (calamityGlobalNPC.newAI[1] >= 10f)
                            {
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                calamityGlobalNPC.newAI[0] = 0f;
                                calamityGlobalNPC.newAI[1] = 0f;
                                npc.localAI[1] = -90f;
                                npc.localAI[2] = -90f;
                                npc.TargetClosest();
                                npc.netUpdate = true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static bool BuffedPrimeLaserAI(NPC npc, Mod mod)
        {
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Set direction
            npc.spriteDirection = -(int)npc.ai[0];

            // Despawn if head is gone
            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 32)
            {
                npc.ai[2] += 10f;
                if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
                {
                    npc.life = -1;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                }
            }

            CalamityGlobalNPC.primeLaser = npc.whoAmI;

            // Check if arms are alive
            bool cannonAlive = false;
            bool viceAlive = false;
            bool sawAlive = false;
            if (CalamityGlobalNPC.primeCannon != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeCannon].active)
                    cannonAlive = true;
            }
            if (CalamityGlobalNPC.primeVice != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeVice].active)
                    viceAlive = true;
            }
            if (CalamityGlobalNPC.primeSaw != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeSaw].active)
                    sawAlive = true;
            }

            // Inflict 0 damage for 3 seconds after spawning
            bool dontAttack = npc.Calamity().newAI[2] < 180f;
            if (dontAttack)
            {
                npc.Calamity().newAI[2] += 1f;
                npc.damage = 0;
            }
            else
                npc.damage = npc.defDamage;

            bool normalLaserRotation = npc.localAI[1] % 2f == 0f;

            // Phase 1
            if (npc.ai[2] == 0f)
            {
                // Despawn if head is despawning
                if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
                    npc.timeLeft = 10;

                // Go to other phase after 13.3 seconds (change this as each arm dies)
                npc.ai[3] += 1f;
                if (!cannonAlive)
                    npc.ai[3] += 1f;
                if (!viceAlive)
                    npc.ai[3] += 1f;
                if (!sawAlive)
                    npc.ai[3] += 1f;

                if (npc.ai[3] >= 800f)
                {
                    npc.localAI[0] = 0f;
                    npc.ai[2] = 1f;
                    npc.ai[3] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }

                float velocityY = malice ? 0.5f : death ? 2f : 2.5f;
                float velocityX = malice ? 1f : death ? 5f : 7f;

                if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 100f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.96f;

                    npc.velocity.Y -= malice ? 0.3f : death ? 0.12f : 0.1f;

                    if (npc.velocity.Y > velocityY)
                        npc.velocity.Y = velocityY;
                }
                else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 100f)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.96f;

                    npc.velocity.Y += malice ? 0.3f : death ? 0.12f : 0.1f;

                    if (npc.velocity.Y < -velocityY)
                        npc.velocity.Y = -velocityY;
                }

                if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 180f * npc.ai[0])
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.96f;

                    npc.velocity.X -= malice ? 0.3f : death ? 0.16f : 0.14f;

                    if (npc.velocity.X > velocityX)
                        npc.velocity.X = velocityX;
                }
                if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 180f * npc.ai[0])
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.96f;

                    npc.velocity.X += malice ? 0.3f : death ? 0.16f : 0.14f;

                    if (npc.velocity.X < -velocityX)
                        npc.velocity.X = -velocityX;
                }

                Vector2 vector62 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num506 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector62.X;
                float num507 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector62.Y;
                float num508 = (float)Math.Sqrt(num506 * num506 + num507 * num507);
                npc.rotation = (float)Math.Atan2(num507, num506) - MathHelper.PiOver2;

                if (Main.netMode != NetmodeID.MultiplayerClient && !dontAttack)
                {
                    // Fire laser every 1.5 seconds (change this as each arm dies to fire more aggressively)
                    npc.localAI[0] += 1f;
                    if (!cannonAlive)
                        npc.localAI[0] += 1f;
                    if (!viceAlive)
                        npc.localAI[0] += 1f;
                    if (!sawAlive)
                        npc.localAI[0] += 1f;

                    if (npc.localAI[0] >= 90f)
                    {
                        npc.localAI[0] = 0f;
                        npc.TargetClosest();
                        float num509 = malice ? 13f : 11f;
                        int type = ProjectileID.DeathLaser;
                        int damage = npc.GetProjectileDamage(type);
                        num508 = num509 / num508;
                        num506 *= num508;
                        num507 *= num508;
                        vector62.X += num506 * 8f;
                        vector62.Y += num507 * 8f;
                        Projectile.NewProjectile(vector62.X, vector62.Y, num506, num507, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }

            // Other phase, get closer to the player and fire ring of lasers
            else if (npc.ai[2] == 1f)
            {
                // Go to phase 1 after 2 seconds (change this as each arm dies to stay in this phase for longer)
                npc.ai[3] += 1f;

                float timeLimit = 135f;
                float timeMult = 1.882075f;
                if (!cannonAlive)
                    timeLimit *= timeMult;
                if (!viceAlive)
                    timeLimit *= timeMult;
                if (!sawAlive)
                    timeLimit *= timeMult;

                if (npc.ai[3] >= timeLimit)
                {
                    npc.localAI[0] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }

                Vector2 vector63 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num513 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - 320f - vector63.X;
                float num514 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 160f - vector63.Y;
                float num515 = (float)Math.Sqrt(num513 * num513 + num514 * num514);
                num515 = 9f / num515;
                num513 *= num515;
                num514 *= num515;

                if (npc.velocity.X > num513)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.9f;
                    npc.velocity.X -= malice ? 0.3f : death ? 0.12f : 0.1f;
                }
                if (npc.velocity.X < num513)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.9f;
                    npc.velocity.X += malice ? 0.3f : death ? 0.12f : 0.1f;
                }
                if (npc.velocity.Y > num514)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.9f;
                    npc.velocity.Y -= malice ? 0.3f : death ? 0.08f : 0.06f;
                }
                if (npc.velocity.Y < num514)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.9f;
                    npc.velocity.Y += malice ? 0.3f : death ? 0.08f : 0.06f;
                }

                vector63 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                num513 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector63.X;
                num514 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector63.Y;
                npc.rotation = (float)Math.Atan2(num514, num513) - MathHelper.PiOver2;

                if (Main.netMode != NetmodeID.MultiplayerClient && !dontAttack)
                {
                    // Fire laser every 1.5 seconds (change this as each arm dies to fire more aggressively)
                    npc.localAI[0] += 1f;
                    if (!cannonAlive)
                        npc.localAI[0] += 0.5f;
                    if (!viceAlive)
                        npc.localAI[0] += 0.5f;
                    if (!sawAlive)
                        npc.localAI[0] += 0.5f;

                    if (npc.localAI[0] >= 120f)
                    {
                        npc.localAI[0] = 0f;
                        npc.TargetClosest();
                        int totalProjectiles = malice ? 24 : 12;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        int type = ProjectileID.DeathLaser;
                        int damage = npc.GetProjectileDamage(type);
                        float velocity = 5f;
                        double angleA = radians * 0.5;
                        double angleB = MathHelper.ToRadians(90f) - angleA;
                        float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                        Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                            int proj = Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer);
                            Main.projectile[proj].timeLeft = 480;
                        }
                        npc.localAI[1] += 1f;
                    }
                }
            }

            return false;
        }

        public static bool BuffedPrimeCannonAI(NPC npc, Mod mod)
        {
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            npc.spriteDirection = -(int)npc.ai[0];

            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 32)
            {
                npc.ai[2] += 10f;
                if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
                {
                    npc.life = -1;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                }
            }

            CalamityGlobalNPC.primeCannon = npc.whoAmI;

            // Check if arms are alive
            bool laserAlive = false;
            bool viceAlive = false;
            bool sawAlive = false;
            if (CalamityGlobalNPC.primeLaser != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeLaser].active)
                    laserAlive = true;
            }
            if (CalamityGlobalNPC.primeVice != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeVice].active)
                    viceAlive = true;
            }
            if (CalamityGlobalNPC.primeSaw != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeSaw].active)
                    sawAlive = true;
            }

            // Inflict 0 damage for 3 seconds after spawning
            bool dontAttack = npc.Calamity().newAI[2] < 180f;
            if (dontAttack)
            {
                npc.Calamity().newAI[2] += 1f;
                npc.damage = 0;
            }
            else
                npc.damage = npc.defDamage;

            bool fireSlower = false;
            if (laserAlive)
            {
                // If laser is firing ring of lasers
                if (Main.npc[CalamityGlobalNPC.primeLaser].ai[2] == 1f)
                    fireSlower = true;
            }
            else
            {
                fireSlower = npc.ai[2] == 0f;

                if (fireSlower)
                {
                    // Go to other phase after 13.33 seconds (change this as each arm dies)
                    npc.ai[3] += 1f;
                    if (!laserAlive)
                        npc.ai[3] += 1f;
                    if (!viceAlive)
                        npc.ai[3] += 1f;
                    if (!sawAlive)
                        npc.ai[3] += 1f;

                    if (npc.ai[3] >= 800f)
                    {
                        npc.localAI[0] = 0f;
                        npc.ai[2] = 1f;
                        fireSlower = false;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    // Go to phase 1 after 2 seconds (change this as each arm dies to stay in this phase for longer)
                    npc.ai[3] += 1f;

                    float timeLimit = 120f;
                    float timeMult = 1.882075f;
                    if (!laserAlive)
                        timeLimit *= timeMult;
                    if (!viceAlive)
                        timeLimit *= timeMult;
                    if (!sawAlive)
                        timeLimit *= timeMult;

                    if (npc.ai[3] >= timeLimit)
                    {
                        npc.localAI[0] = 0f;
                        npc.ai[2] = 0f;
                        fireSlower = true;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }
                }
            }

            if (fireSlower)
            {
                if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
                    npc.timeLeft = 10;

                float velocityY = malice ? 0.5f : death ? 2f : 2.5f;
                float velocityX = malice ? 1f : death ? 5f : 7f;

                if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 150f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.96f;

                    npc.velocity.Y -= malice ? 0.275f : death ? 0.1f : 0.08f;

                    if (npc.velocity.Y > velocityY)
                        npc.velocity.Y = velocityY;
                }
                else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 150f)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.96f;

                    npc.velocity.Y += malice ? 0.275f : death ? 0.1f : 0.08f;

                    if (npc.velocity.Y < -velocityY)
                        npc.velocity.Y = -velocityY;
                }

                if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) + 200f)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.96f;

                    npc.velocity.X -= malice ? 0.275f : death ? 0.22f : 0.2f;

                    if (npc.velocity.X > velocityX)
                        npc.velocity.X = velocityX;
                }
                if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) + 160f)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.96f;

                    npc.velocity.X += malice ? 0.275f : death ? 0.22f : 0.2f;

                    if (npc.velocity.X < -velocityX)
                        npc.velocity.X = -velocityX;
                }

                Vector2 vector60 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num492 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector60.X;
                float num493 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector60.Y;
                float num494 = (float)Math.Sqrt(num492 * num492 + num493 * num493);
                npc.rotation = (float)Math.Atan2(num493, num492) - MathHelper.PiOver2;

                if (Main.netMode != NetmodeID.MultiplayerClient && !dontAttack)
                {
                    // Fire rocket every 2 seconds (change this as each arm dies to fire more aggressively)
                    npc.localAI[0] += 1f;
                    if (!laserAlive)
                        npc.localAI[0] += 1f;
                    if (!viceAlive)
                        npc.localAI[0] += 1f;
                    if (!sawAlive)
                        npc.localAI[0] += 1f;

                    if (npc.localAI[0] >= 120f)
                    {
                        npc.localAI[0] = 0f;
                        npc.TargetClosest();
                        float num495 = 1f;
                        int type = ProjectileID.RocketSkeleton;
                        int damage = npc.GetProjectileDamage(type);
                        num494 = num495 / num494;
                        num492 *= num494;
                        num493 *= num494;
                        vector60.X += num492 * 5f;
                        vector60.Y += num493 * 5f;
                        Projectile.NewProjectile(vector60.X, vector60.Y, num492, num493, type, damage, 0f, Main.myPlayer, 0f, 1f);
                    }
                }
            }

            else
            {
                Vector2 vector61 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num499 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - vector61.X;
                float num500 = Main.npc[(int)npc.ai[1]].position.Y - vector61.Y;
                num500 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 160f - vector61.Y;
                float num501 = (float)Math.Sqrt(num499 * num499 + num500 * num500);
                num501 = 9f / num501;
                num499 *= num501;
                num500 *= num501;

                if (npc.velocity.X > num499)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.9f;
                    npc.velocity.X -= malice ? 0.275f : death ? 0.1f : 0.08f;
                }
                if (npc.velocity.X < num499)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.9f;
                    npc.velocity.X += malice ? 0.275f : death ? 0.1f : 0.08f;
                }
                if (npc.velocity.Y > num500)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.9f;
                    npc.velocity.Y -= malice ? 0.275f : death ? 0.1f : 0.08f;
                }
                if (npc.velocity.Y < num500)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.9f;
                    npc.velocity.Y += malice ? 0.275f : death ? 0.1f : 0.08f;
                }

                vector61 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                num499 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector61.X;
                num500 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector61.Y;
                npc.rotation = (float)Math.Atan2(num500, num499) - MathHelper.PiOver2;

                if (Main.netMode != NetmodeID.MultiplayerClient && !dontAttack)
                {
                    // Fire rockets every 2 seconds (change this as each arm dies to fire more aggressively)
                    npc.localAI[0] += 1f;
                    if (!laserAlive)
                        npc.localAI[0] += 0.5f;
                    if (!viceAlive)
                        npc.localAI[0] += 0.5f;
                    if (!sawAlive)
                        npc.localAI[0] += 0.5f;

                    if (npc.localAI[0] >= 180f)
                    {
                        npc.localAI[0] = 0f;
                        npc.TargetClosest();
                        float num502 = 1f;
                        int type = ProjectileID.RocketSkeleton;
                        int damage = npc.GetProjectileDamage(type);
                        Vector2 value19 = Main.player[npc.target].Center - npc.Center;
                        value19.Normalize();
                        value19 *= num502;
                        int numProj = 2;
                        float rotation = MathHelper.ToRadians(5);
                        if (malice)
                        {
                            rotation = MathHelper.ToRadians(8);
                            for (int i = 0; i < numProj; i++)
                            {
                                Projectile.NewProjectile(npc.Center, value19.RotatedBy(-rotation * (i + 1)), type, damage, 0f, Main.myPlayer, 0f, 1f);
                                Projectile.NewProjectile(npc.Center, value19.RotatedBy(+rotation * (i + 1)), type, damage, 0f, Main.myPlayer, 0f, 1f);
                            }
                            Projectile.NewProjectile(npc.Center, value19, type, damage, 0f, Main.myPlayer, 0f, 1f);
                        }
                        else
                        {
                            for (int i = 0; i < numProj + 1; i++)
                            {
                                Vector2 perturbedSpeed = value19.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 0f, Main.myPlayer, 0f, 1f);
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static bool BuffedPrimeViceAI(NPC npc, Mod mod)
        {
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Direction
            npc.spriteDirection = -(int)npc.ai[0];

            // Where the vice should be in relation to the head
            Vector2 vector55 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float num477 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector55.X;
            float num478 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector55.Y;
            float num479 = (float)Math.Sqrt(num477 * num477 + num478 * num478);

            // Return the vice to its proper location in relation to the head if it's too far away
            if (npc.ai[2] != 99f)
            {
                if (num479 > 800f)
                    npc.ai[2] = 99f;
            }
            else if (num479 < 400f)
                npc.ai[2] = 0f;

            // Despawn if head is gone
            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 32)
            {
                npc.ai[2] += 10f;
                if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
                {
                    npc.life = -1;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                }
            }

            CalamityGlobalNPC.primeVice = npc.whoAmI;

            // Check if arms are alive
            bool cannonAlive = false;
            bool laserAlive = false;
            bool sawAlive = false;
            if (CalamityGlobalNPC.primeCannon != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeCannon].active)
                    cannonAlive = true;
            }
            if (CalamityGlobalNPC.primeLaser != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeLaser].active)
                    laserAlive = true;
            }
            if (CalamityGlobalNPC.primeSaw != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeSaw].active)
                    sawAlive = true;
            }

            // Inflict 0 damage for 3 seconds after spawning
            if (npc.Calamity().newAI[2] < 180f)
            {
                npc.Calamity().newAI[2] += 1f;
                npc.damage = 0;
            }
            else
                npc.damage = npc.defDamage;

            // Return to the head
            if (npc.ai[2] == 99f)
            {
                float velocityY = malice ? 1f : death ? 5f : 7f;
                float velocityX = malice ? 1.5f : death ? 8f : 10f;

                if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.96f;

                    npc.velocity.Y -= malice ? 0.3f : death ? 0.12f : 0.1f;

                    if (npc.velocity.Y > velocityY)
                        npc.velocity.Y = velocityY;
                }
                else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.96f;

                    npc.velocity.Y += malice ? 0.3f : death ? 0.12f : 0.1f;

                    if (npc.velocity.Y < -velocityY)
                        npc.velocity.Y = -velocityY;
                }

                if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2))
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.96f;

                    npc.velocity.X -= malice ? 1f : death ? 0.55f : 0.5f;

                    if (npc.velocity.X > velocityX)
                        npc.velocity.X = velocityX;
                }
                if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2))
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.96f;

                    npc.velocity.X += malice ? 1f : death ? 0.55f : 0.5f;

                    if (npc.velocity.X < -velocityX)
                        npc.velocity.X = -velocityX;
                }
            }

            // Other phases
            else
            {
                // Stay near the head
                if (npc.ai[2] == 0f || npc.ai[2] == 3f)
                {
                    // Despawn if head is despawning
                    if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
                        npc.timeLeft = 10;

                    // Start charging after 10 seconds (change this as each arm dies)
                    npc.ai[3] += 1f;
                    if (!cannonAlive)
                        npc.ai[3] += 1f;
                    if (!laserAlive)
                        npc.ai[3] += 1f;
                    if (!sawAlive)
                        npc.ai[3] += 1f;

                    if (npc.ai[3] >= 600f)
                    {
                        npc.ai[2] += 1f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }

                    float velocityY = malice ? 0.5f : death ? 2f : 2.5f;
                    float velocityX = malice ? 1.5f : death ? 7f : 8f;
                    float velocityX2 = malice ? 1.25f : death ? 6f : 7f;

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 300f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= 0.96f;

                        npc.velocity.Y -= malice ? 0.3f : death ? 0.12f : 0.1f;

                        if (npc.velocity.Y > velocityY)
                            npc.velocity.Y = velocityY;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 230f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= 0.96f;

                        npc.velocity.Y += malice ? 0.3f : death ? 0.12f : 0.1f;

                        if (npc.velocity.Y < -velocityY)
                            npc.velocity.Y = -velocityY;
                    }

                    if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) + 250f)
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= 0.94f;

                        npc.velocity.X -= malice ? 0.8f : death ? 0.33f : 0.3f;

                        if (npc.velocity.X > velocityX)
                            npc.velocity.X = velocityX;
                    }
                    if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2))
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= 0.94f;

                        npc.velocity.X += malice ? 0.8f : death ? 0.22f : 0.2f;

                        if (npc.velocity.X < -velocityX2)
                            npc.velocity.X = -velocityX2;
                    }

                    Vector2 vector57 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num483 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector57.X;
                    float num484 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector57.Y;
                    npc.rotation = (float)Math.Atan2(num484, num483) + MathHelper.PiOver2;
                    return false;
                }

                // Charge towards the player
                if (npc.ai[2] == 1f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.9f;

                    Vector2 vector58 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num486 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 280f * npc.ai[0] - vector58.X;
                    float num487 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector58.Y;
                    npc.rotation = (float)Math.Atan2(num487, num486) + MathHelper.PiOver2;

                    npc.velocity.X = (npc.velocity.X * 5f + Main.npc[(int)npc.ai[1]].velocity.X) / 6f;
                    npc.velocity.X += 0.5f;

                    npc.velocity.Y -= 0.5f;
                    if (npc.velocity.Y < -9f)
                        npc.velocity.Y = -9f;

                    if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 280f)
                    {
                        float chargeVelocity = malice ? 20f : 16f;
                        if (!cannonAlive)
                            chargeVelocity += 1.5f;
                        if (!laserAlive)
                            chargeVelocity += 1.5f;
                        if (!sawAlive)
                            chargeVelocity += 1.5f;

                        npc.ai[2] = 2f;
                        npc.TargetClosest();
                        vector58 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        num486 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector58.X;
                        num487 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector58.Y;
                        float num488 = (float)Math.Sqrt(num486 * num486 + num487 * num487);
                        num488 = chargeVelocity / num488;
                        npc.velocity.X = num486 * num488;
                        npc.velocity.Y = num487 * num488;
                        npc.netUpdate = true;
                    }
                }

                // Charge 4 times (more if arms are dead)
                else if (npc.ai[2] == 2f)
                {
                    if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f)
                    {
                        float chargeAmt = 4f;
                        if (!cannonAlive)
                            chargeAmt += 1f;
                        if (!laserAlive)
                            chargeAmt += 1f;
                        if (!sawAlive)
                            chargeAmt += 1f;

                        if (npc.ai[3] >= chargeAmt)
                        {
                            // Return to head
                            npc.ai[2] = 3f;
                            npc.ai[3] = 0f;
                            npc.TargetClosest();
                            return false;
                        }

                        npc.ai[2] = 1f;
                        npc.ai[3] += 1f;
                    }
                }

                // Different type of charge
                else if (npc.ai[2] == 4f)
                {
                    Vector2 vector59 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num489 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector59.X;
                    float num490 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector59.Y;
                    npc.rotation = (float)Math.Atan2(num490, num489) + MathHelper.PiOver2;

                    npc.velocity.Y = (npc.velocity.Y * 5f + Main.npc[(int)npc.ai[1]].velocity.Y) / 6f;

                    npc.velocity.X += 0.5f;
                    if (npc.velocity.X > 12f)
                        npc.velocity.X = 12f;

                    if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 500f || npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) + 500f)
                    {
                        float chargeVelocity = malice ? 17.5f : 14f;
                        if (!cannonAlive)
                            chargeVelocity += 1.15f;
                        if (!laserAlive)
                            chargeVelocity += 1.15f;
                        if (!sawAlive)
                            chargeVelocity += 1.15f;

                        npc.ai[2] = 5f;
                        npc.TargetClosest();
                        vector59 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        num489 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector59.X;
                        num490 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector59.Y;
                        float num491 = (float)Math.Sqrt(num489 * num489 + num490 * num490);
                        num491 = chargeVelocity / num491;
                        npc.velocity.X = num489 * num491;
                        npc.velocity.Y = num490 * num491;
                        npc.netUpdate = true;
                    }
                }

                // Charge 4 times (more if arms are dead)
                else if (npc.ai[2] == 5f && npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - 100f)
                {
                    float chargeAmt = 4f;
                    if (!cannonAlive)
                        chargeAmt += 1f;
                    if (!laserAlive)
                        chargeAmt += 1f;
                    if (!sawAlive)
                        chargeAmt += 1f;

                    if (npc.ai[3] >= chargeAmt)
                    {
                        // Return to head
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        return false;
                    }

                    npc.ai[2] = 4f;
                    npc.ai[3] += 1f;
                }
            }

            return false;
        }

        public static bool BuffedPrimeSawAI(NPC npc, Mod mod)
        {
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            Vector2 vector50 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float num462 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector50.X;
            float num463 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector50.Y;
            float num464 = (float)Math.Sqrt(num462 * num462 + num463 * num463);

            if (npc.ai[2] != 99f)
            {
                if (num464 > 800f)
                    npc.ai[2] = 99f;
            }
            else if (num464 < 400f)
                npc.ai[2] = 0f;

            npc.spriteDirection = -(int)npc.ai[0];

            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 32)
            {
                npc.ai[2] += 10f;
                if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
                {
                    npc.life = -1;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                }
            }

            CalamityGlobalNPC.primeSaw = npc.whoAmI;

            // Check if arms are alive
            bool cannonAlive = false;
            bool laserAlive = false;
            bool viceAlive = false;
            if (CalamityGlobalNPC.primeCannon != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeCannon].active)
                    cannonAlive = true;
            }
            if (CalamityGlobalNPC.primeLaser != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeLaser].active)
                    laserAlive = true;
            }
            if (CalamityGlobalNPC.primeVice != -1)
            {
                if (Main.npc[CalamityGlobalNPC.primeVice].active)
                    viceAlive = true;
            }

            // Inflict 0 damage for 3 seconds after spawning
            if (npc.Calamity().newAI[2] < 180f)
            {
                npc.Calamity().newAI[2] += 1f;
                npc.damage = 0;
            }
            else
                npc.damage = npc.defDamage;

            if (npc.ai[2] == 99f)
            {
                float velocityY = malice ? 1f : death ? 6f : 7f;
                float velocityX = malice ? 1.5f : death ? 8f : 10f;

                if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.96f;

                    npc.velocity.Y -= malice ? 0.3f : death ? 0.12f : 0.1f;

                    if (npc.velocity.Y > velocityY)
                        npc.velocity.Y = velocityY;
                }
                else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.96f;

                    npc.velocity.Y += malice ? 0.3f : death ? 0.12f : 0.1f;

                    if (npc.velocity.Y < -velocityY)
                        npc.velocity.Y = -velocityY;
                }

                if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2))
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.96f;

                    npc.velocity.X -= malice ? 1.2f : death ? 0.55f : 0.5f;

                    if (npc.velocity.X > velocityX)
                        npc.velocity.X = velocityX;
                }
                if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2))
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.96f;

                    npc.velocity.X += malice ? 1.2f : death ? 0.55f : 0.5f;

                    if (npc.velocity.X < -velocityX)
                        npc.velocity.X = -velocityX;
                }
            }
            else
            {
                if (npc.ai[2] == 0f || npc.ai[2] == 3f)
                {
                    if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
                        npc.timeLeft = 10;

                    // Start charging after 3 seconds (change this as each arm dies)
                    npc.ai[3] += 1f;
                    if (!cannonAlive)
                        npc.ai[3] += 1f;
                    if (!laserAlive)
                        npc.ai[3] += 1f;
                    if (!viceAlive)
                        npc.ai[3] += 1f;

                    if (npc.ai[3] >= 180f)
                    {
                        npc.ai[2] += 1f;
                        npc.ai[3] = 0f;
                        npc.TargetClosest();
                        npc.netUpdate = true;
                    }

                    float velocityY = malice ? 0.5f : death ? 2f : 2.5f;
                    float velocityX = malice ? 1.5f : death ? 8f : 10f;

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 320f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= 0.96f;

                        npc.velocity.Y -= malice ? 0.15f : death ? 0.05f : 0.04f;

                        if (npc.velocity.Y > velocityY)
                            npc.velocity.Y = velocityY;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 260f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= 0.96f;

                        npc.velocity.Y += malice ? 0.15f : death ? 0.05f : 0.04f;

                        if (npc.velocity.Y < -velocityY)
                            npc.velocity.Y = -velocityY;
                    }

                    if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2))
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= 0.96f;

                        npc.velocity.X -= malice ? 0.8f : death ? 0.33f : 0.3f;

                        if (npc.velocity.X > velocityX)
                            npc.velocity.X = velocityX;
                    }
                    if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 250f)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= 0.96f;

                        npc.velocity.X += malice ? 0.8f : death ? 0.33f : 0.3f;

                        if (npc.velocity.X < -velocityX)
                            npc.velocity.X = -velocityX;
                    }

                    Vector2 vector52 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num468 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector52.X;
                    float num469 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector52.Y;
                    npc.rotation = (float)Math.Atan2(num469, num468) + MathHelper.PiOver2;
                    return false;
                }

                if (npc.ai[2] == 1f)
                {
                    Vector2 vector53 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float num471 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector53.X;
                    float num472 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector53.Y;
                    npc.rotation = (float)Math.Atan2(num472, num471) + MathHelper.PiOver2;

                    npc.velocity.X *= 0.95f;
                    npc.velocity.Y -= 0.3f;
                    if (npc.velocity.Y < -8f)
                        npc.velocity.Y = -8f;

                    if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 200f)
                    {
                        float chargeVelocity = malice ? 27.5f : 22f;
                        if (!cannonAlive)
                            chargeVelocity += 1.5f;
                        if (!laserAlive)
                            chargeVelocity += 1.5f;
                        if (!viceAlive)
                            chargeVelocity += 1.5f;

                        npc.ai[2] = 2f;
                        npc.TargetClosest();
                        vector53 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        num471 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector53.X;
                        num472 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector53.Y;
                        float num473 = (float)Math.Sqrt(num471 * num471 + num472 * num472);
                        num473 = chargeVelocity / num473;
                        npc.velocity.X = num471 * num473;
                        npc.velocity.Y = num472 * num473;
                        npc.netUpdate = true;
                    }
                }

                else if (npc.ai[2] == 2f)
                {
                    if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f)
                        npc.ai[2] = 3f;
                }

                else
                {
                    if (npc.ai[2] == 4f)
                    {
                        float chargeVelocity = malice ? 13.5f : 11f;
                        if (!cannonAlive)
                            chargeVelocity += 1.5f;
                        if (!laserAlive)
                            chargeVelocity += 1.5f;
                        if (!viceAlive)
                            chargeVelocity += 1.5f;

                        Vector2 vector54 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float num474 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector54.X;
                        float num475 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector54.Y;
                        float num476 = (float)Math.Sqrt(num474 * num474 + num475 * num475);
                        num476 = chargeVelocity / num476;
                        num474 *= num476;
                        num475 *= num476;

                        float acceleration = malice ? 0.15f : death ? 0.06f : 0.05f;
                        if (npc.velocity.X > num474)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X *= 0.97f;
                            npc.velocity.X -= acceleration;
                        }
                        if (npc.velocity.X < num474)
                        {
                            if (npc.velocity.X < 0f)
                                npc.velocity.X *= 0.97f;
                            npc.velocity.X += acceleration;
                        }
                        if (npc.velocity.Y > num475)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y *= 0.97f;
                            npc.velocity.Y -= acceleration;
                        }
                        if (npc.velocity.Y < num475)
                        {
                            if (npc.velocity.Y < 0f)
                                npc.velocity.Y *= 0.97f;
                            npc.velocity.Y += acceleration;
                        }

                        npc.ai[3] += 1f;
                        if (npc.justHit)
                            npc.ai[3] += 2f;

                        if (npc.ai[3] >= 600f)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[3] = 0f;
                            npc.TargetClosest();
                            npc.netUpdate = true;
                        }

                        vector54 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        num474 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector54.X;
                        num475 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector54.Y;
                        npc.rotation = (float)Math.Atan2(num475, num474) + MathHelper.PiOver2;
                        return false;
                    }

                    if (npc.ai[2] == 5f && ((npc.velocity.X > 0f && npc.position.X + (npc.width / 2) > Main.player[npc.target].position.X + (Main.player[npc.target].width / 2)) || (npc.velocity.X < 0f && npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))))
                        npc.ai[2] = 0f;
                }
            }

            return false;
        }
    }
}
