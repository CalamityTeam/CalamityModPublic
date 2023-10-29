using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class SkeletronPrimeAI
    {
        public static bool BuffedSkeletronPrimeAI(NPC npc, Mod mod)
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

            if (npc.ai[3] != 0f)
                NPC.mechQueen = npc.whoAmI;

            // Spawn arms
            if (npc.ai[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.ai[0] = 1f;

                int arm = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PrimeCannon, npc.whoAmI);
                Main.npc[arm].ai[0] = -1f;
                Main.npc[arm].ai[1] = npc.whoAmI;
                Main.npc[arm].target = npc.target;
                Main.npc[arm].netUpdate = true;

                arm = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PrimeSaw, npc.whoAmI);
                Main.npc[arm].ai[0] = 1f;
                Main.npc[arm].ai[1] = npc.whoAmI;
                Main.npc[arm].target = npc.target;
                Main.npc[arm].netUpdate = true;

                arm = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PrimeVice, npc.whoAmI);
                Main.npc[arm].ai[0] = -1f;
                Main.npc[arm].ai[1] = npc.whoAmI;
                Main.npc[arm].target = npc.target;
                Main.npc[arm].ai[3] = 150f;
                Main.npc[arm].netUpdate = true;

                arm = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PrimeLaser, npc.whoAmI);
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
            if (Main.dayTime && !bossRush && npc.ai[1] != 3f && npc.ai[1] != 2f)
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
                SoundEngine.PlaySound(SoundID.Roar, npc.position);
            }

            // Adjust slowing debuff immunity
            bool immuneToSlowingDebuffs = npc.ai[1] == 5f;
            npc.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<KamiFlu>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TimeDistortion>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<GalvanicCorrosion>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

            bool normalLaserRotation = calamityGlobalNPC.newAI[3] % 2f == 0f;

            // Float near player
            if (npc.ai[1] == 0f || npc.ai[1] == 4f)
            {
                // Start other phases if arms are dead, start with spin phase
                if (allArmsDead || CalamityWorld.LegendaryMode)
                {
                    // Start spin phase after 1.5 seconds
                    npc.ai[2] += phase3 ? 1.5f : 1f;
                    if (npc.ai[2] >= (90f - (death ? 60f * (1f - lifeRatio) : 0f)))
                    {
                        bool shouldSpinAroundTarget = npc.ai[1] == 4f && npc.position.Y < Main.player[npc.target].position.Y - 400f &&
                            Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 600f && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 400f;

                        if (shouldSpinAroundTarget || npc.ai[1] != 4f)
                        {
                            if (shouldSpinAroundTarget)
                            {
                                npc.localAI[3] = 300f;
                                npc.SyncVanillaLocalAI();
                            }

                            npc.ai[2] = 0f;
                            npc.ai[1] = shouldSpinAroundTarget ? 5f : 1f;
                            npc.TargetClosest();
                            npc.netUpdate = true;
                        }
                    }
                }

                if (NPC.IsMechQueenUp)
                    npc.rotation = npc.rotation.AngleLerp(npc.velocity.X / 15f * 0.5f, 0.75f);
                else
                    npc.rotation = npc.velocity.X / 15f;

                float velocityY = 3f - (death ? 1f - lifeRatio : 0f);
                float velocityX = 7f - (death ? 3.5f * (1f - lifeRatio) : 0f);
                float acceleration = 0.1f + (death ? 0.05f * (1f - lifeRatio) : 0f);

                float headDecelerationUpDist = 0f;
                float headDecelerationDownDist = 0f;
                float headDecelerationHorizontalDist = 0f;
                int headHorizontalDirection = ((!(Main.player[npc.target].Center.X < npc.Center.X)) ? 1 : (-1));
                if (NPC.IsMechQueenUp)
                {
                    headDecelerationHorizontalDist = -150f * (float)headHorizontalDirection;
                    headDecelerationUpDist = -100f;
                    headDecelerationDownDist = -100f;
                }

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

                if (bossRush)
                {
                    velocityY = 0.25f;
                    velocityX = 0.5f;
                    acceleration = 0.3f;
                }

                // Reduce acceleration if target is holding a true melee weapon
                if (Main.player[npc.target].HoldingTrueMeleeWeapon())
                    acceleration *= 0.5f;

                if (npc.position.Y > Main.player[npc.target].position.Y - (400f + headDecelerationUpDist))
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.9f;

                    npc.velocity.Y -= acceleration;

                    if (npc.velocity.Y > velocityY)
                        npc.velocity.Y = velocityY;
                }
                else if (npc.position.Y < Main.player[npc.target].position.Y - (450f + headDecelerationDownDist))
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.9f;

                    npc.velocity.Y += acceleration;

                    if (npc.velocity.Y < -velocityY)
                        npc.velocity.Y = -velocityY;
                }

                if (npc.position.X + (npc.width / 2) > Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) + (400f + headDecelerationHorizontalDist))
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.9f;

                    npc.velocity.X -= acceleration;

                    if (npc.velocity.X > velocityX)
                        npc.velocity.X = velocityX;
                }
                if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - (400f + headDecelerationHorizontalDist))
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.9f;

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

                            int totalProjectiles = bossRush ? 24 : 12;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = ProjectileID.DeathLaser;
                            int damage = npc.GetProjectileDamage(type);
                            float velocity = 3f;
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 laserFireDirection = spinningPoint.RotatedBy(radians * k);
                                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(laserFireDirection) * 30f, laserFireDirection, type, damage, 0f, Main.myPlayer, 1f, 0f);
                                Main.projectile[proj].timeLeft = 900;
                            }
                            calamityGlobalNPC.newAI[3] += 1f;
                        }
                    }

                    npc.ai[2] += 1f;
                    if (npc.ai[2] == 2f)
                        SoundEngine.PlaySound(SoundID.Roar, npc.position);

                    // Spin for 3 seconds then return to floating phase
                    float phaseTimer = 240f;
                    if (phase2 && !phase3)
                        phaseTimer += 60f;

                    if (npc.ai[2] >= (phaseTimer - (death ? 60f * (1f - lifeRatio) : 0f)))
                    {
                        npc.TargetClosest();
                        npc.ai[2] = 0f;
                        npc.ai[1] = 4f;
                        npc.localAI[1] = 0f;
                    }

                    if (NPC.IsMechQueenUp)
                        npc.rotation = npc.rotation.AngleLerp(npc.velocity.X / 15f * 0.5f, 0.75f);
                    else
                        npc.rotation += npc.direction * 0.3f;

                    Vector2 headPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float headTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - headPosition.X;
                    float headTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - headPosition.Y;
                    float headTargetDistance = (float)Math.Sqrt(headTargetX * headTargetX + headTargetY * headTargetY);

                    float speed = bossRush ? 9f : 6f;
                    if (phase2)
                        speed += 0.5f;
                    if (phase3)
                        speed += 0.5f;

                    if (headTargetDistance > 150f)
                    {
                        float baseDistanceVelocityMult = 1f + MathHelper.Clamp((headTargetDistance - 150f) * 0.0015f, 0.05f, 1.5f);
                        speed *= baseDistanceVelocityMult;
                    }

                    if (NPC.IsMechQueenUp)
                    {
                        float mechdusaSpeedMult = (NPC.npcsFoundForCheckActive[NPCID.TheDestroyerBody] ? 0.6f : 0.75f);
                        speed *= mechdusaSpeedMult;
                    }

                    headTargetDistance = speed / headTargetDistance;
                    npc.velocity.X = headTargetX * headTargetDistance;
                    npc.velocity.Y = headTargetY * headTargetDistance;

                    if (NPC.IsMechQueenUp)
                    {
                        float mechdusaAccelMult = Vector2.Distance(npc.Center, Main.player[npc.target].Center);
                        if (mechdusaAccelMult < 0.1f)
                            mechdusaAccelMult = 0f;

                        if (mechdusaAccelMult < speed)
                            npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * mechdusaAccelMult;
                    }
                }

                // Daytime enrage
                if (npc.ai[1] == 2f)
                {
                    npc.damage = 1000;
                    calamityGlobalNPC.DR = 0.9999f;
                    calamityGlobalNPC.unbreakableDR = true;

                    if (NPC.IsMechQueenUp)
                        npc.rotation = npc.rotation.AngleLerp(npc.velocity.X / 15f * 0.5f, 0.75f);
                    else
                        npc.rotation += npc.direction * 0.3f;

                    Vector2 enragedHeadPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float enragedHeadTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - enragedHeadPosition.X;
                    float enragedHeadTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - enragedHeadPosition.Y;
                    float enragedHeadTargetDist = (float)Math.Sqrt(enragedHeadTargetX * enragedHeadTargetX + enragedHeadTargetY * enragedHeadTargetY);

                    float enragedHeadSpeed = 10f;
                    enragedHeadSpeed += enragedHeadTargetDist / 100f;
                    if (enragedHeadSpeed < 8f)
                        enragedHeadSpeed = 8f;
                    if (enragedHeadSpeed > 32f)
                        enragedHeadSpeed = 32f;

                    enragedHeadTargetDist = enragedHeadSpeed / enragedHeadTargetDist;
                    npc.velocity.X = enragedHeadTargetX * enragedHeadTargetDist;
                    npc.velocity.Y = enragedHeadTargetY * enragedHeadTargetDist;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[0] += 1f;
                        if (npc.localAI[0] >= 60f)
                        {
                            npc.localAI[0] = 0f;
                            Vector2 headCenter = npc.Center;
                            if (Collision.CanHit(headCenter, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                            {
                                enragedHeadSpeed = 7f;
                                float enragedHeadSkullTargetX = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - headCenter.X + Main.rand.Next(-20, 21);
                                float enragedHeadSkullTargetY = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - headCenter.Y + Main.rand.Next(-20, 21);
                                float enragedHeadSkullTargetDist = (float)Math.Sqrt(enragedHeadSkullTargetX * enragedHeadSkullTargetX + enragedHeadSkullTargetY * enragedHeadSkullTargetY);
                                enragedHeadSkullTargetDist = enragedHeadSpeed / enragedHeadSkullTargetDist;
                                enragedHeadSkullTargetX *= enragedHeadSkullTargetDist;
                                enragedHeadSkullTargetY *= enragedHeadSkullTargetDist;

                                Vector2 value = new Vector2(enragedHeadSkullTargetX * 1f + Main.rand.Next(-50, 51) * 0.01f, enragedHeadSkullTargetY * 1f + Main.rand.Next(-50, 51) * 0.01f);
                                value.Normalize();
                                value *= enragedHeadSpeed;
                                value += npc.velocity;
                                enragedHeadSkullTargetX = value.X;
                                enragedHeadSkullTargetY = value.Y;

                                int type = ProjectileID.Skull;
                                headCenter += value * 5f;
                                int enragedSkulls = Projectile.NewProjectile(npc.GetSource_FromAI(), headCenter.X, headCenter.Y, enragedHeadSkullTargetX, enragedHeadSkullTargetY, type, 250, 0f, Main.myPlayer, -1f, 0f);
                                Main.projectile[enragedSkulls].timeLeft = 300;
                            }
                        }
                    }
                }

                // Despawning
                if (npc.ai[1] == 3f)
                {
                    if (NPC.IsMechQueenUp)
                    {
                        int mechdusaBossDespawning = NPC.FindFirstNPC(NPCID.Retinazer);
                        if (mechdusaBossDespawning >= 0)
                            Main.npc[mechdusaBossDespawning].EncourageDespawn(5);

                        mechdusaBossDespawning = NPC.FindFirstNPC(NPCID.Spazmatism);
                        if (mechdusaBossDespawning >= 0)
                            Main.npc[mechdusaBossDespawning].EncourageDespawn(5);

                        if (!NPC.AnyNPCs(NPCID.Retinazer) && !NPC.AnyNPCs(NPCID.Spazmatism))
                        {
                            mechdusaBossDespawning = NPC.FindFirstNPC(NPCID.TheDestroyer);
                            if (mechdusaBossDespawning >= 0)
                                Main.npc[mechdusaBossDespawning].Transform(NPCID.TheDestroyerTail);

                            npc.EncourageDespawn(5);
                        }

                        npc.velocity.Y += 0.1f;
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= 0.95f;

                        npc.velocity.X *= 0.95f;
                        if (npc.velocity.Y > 13f)
                            npc.velocity.Y = 13f;
                    }
                    else
                    {
                        npc.velocity.Y += 0.1f;
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= 0.9f;

                        npc.velocity.X *= 0.9f;

                        if (npc.timeLeft > 500)
                            npc.timeLeft = 500;
                    }
                }

                // Fly around target in a circle
                if (npc.ai[1] == 5f)
                {
                    npc.ai[2] += 1f;

                    npc.rotation = npc.velocity.X / 50f;

                    float skullSpawnDivisor = bossRush ? 9f : death ? 15f - (float)Math.Round(5f * (1f - lifeRatio)) : 15f;
                    float totalSkulls = 12f;
                    int skullSpread = bossRush ? 250 : death ? 150 : 100;

                    // Spin for about 3 seconds
                    float spinVelocity = 30f;
                    if (npc.ai[2] == 2f)
                    {
                        // Play angry noise
                        SoundEngine.PlaySound(SoundID.Roar, npc.position);

                        // Set spin direction
                        if (Main.player[npc.target].velocity.X > 0f)
                            calamityGlobalNPC.newAI[0] = 1f;
                        else if (Main.player[npc.target].velocity.X < 0f)
                            calamityGlobalNPC.newAI[0] = -1f;
                        else
                            calamityGlobalNPC.newAI[0] = Main.player[npc.target].direction;

                        // Set spin velocity
                        npc.velocity.X = MathHelper.Pi * npc.localAI[3] / spinVelocity;
                        npc.velocity *= -calamityGlobalNPC.newAI[0];
                        npc.netUpdate = true;
                    }

                    // Maintain velocity and spit skulls
                    else if (npc.ai[2] > 2f)
                    {
                        npc.velocity = npc.velocity.RotatedBy(MathHelper.Pi / spinVelocity * -calamityGlobalNPC.newAI[0]);
                        if (npc.ai[2] == 3f)
                            npc.velocity *= 0.6f;

                        if (npc.ai[2] % skullSpawnDivisor == 0f)
                        {
                            calamityGlobalNPC.newAI[1] += 1f;

                            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 64f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 headCenter = npc.Center;
                                    float enragedHeadSpeed = 4f + (death ? 2f * (1f - lifeRatio) : 0f);
                                    float enragedHeadSkullTargetX = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - headCenter.X + Main.rand.Next(-20, 21);
                                    float enragedHeadSkullTargetY = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - headCenter.Y + Main.rand.Next(-20, 21);
                                    float enragedHeadSkullTargetDist = (float)Math.Sqrt(enragedHeadSkullTargetX * enragedHeadSkullTargetX + enragedHeadSkullTargetY * enragedHeadSkullTargetY);
                                    enragedHeadSkullTargetDist = enragedHeadSpeed / enragedHeadSkullTargetDist;
                                    enragedHeadSkullTargetX *= enragedHeadSkullTargetDist;
                                    enragedHeadSkullTargetY *= enragedHeadSkullTargetDist;

                                    Vector2 value = new Vector2(enragedHeadSkullTargetX + Main.rand.Next(-skullSpread, skullSpread + 1) * 0.01f, enragedHeadSkullTargetY + Main.rand.Next(-skullSpread, skullSpread + 1) * 0.01f);
                                    value.Normalize();
                                    value *= enragedHeadSpeed;
                                    enragedHeadSkullTargetX = value.X;
                                    enragedHeadSkullTargetY = value.Y;

                                    int type = ProjectileID.Skull;
                                    int damage = npc.GetProjectileDamage(type);
                                    headCenter += value * 5f;
                                    int enragedSkulls = Projectile.NewProjectile(npc.GetSource_FromAI(), headCenter.X, headCenter.Y, enragedHeadSkullTargetX, enragedHeadSkullTargetY, type, damage, 0f, Main.myPlayer, -1f, 0f);
                                    Main.projectile[enragedSkulls].timeLeft = 480;
                                    Main.projectile[enragedSkulls].tileCollide = false;
                                }
                            }

                            // Go to floating phase, or spinning phase if in phase 2
                            if (calamityGlobalNPC.newAI[1] >= totalSkulls)
                            {
                                npc.velocity.Normalize();

                                // Fly overhead and spit missiles if on low health
                                npc.ai[1] = phase3 ? 6f : 1f;
                                npc.ai[2] = 0f;
                                npc.localAI[3] = 0f;
                                calamityGlobalNPC.newAI[1] = 0f;
                                calamityGlobalNPC.newAI[0] = 0f;
                                npc.SyncVanillaLocalAI();
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

                    float flightVelocity = bossRush ? 25f : death ? 20f : 15f;
                    float flightAcceleration = bossRush ? 0.25f : death ? 0.2f : 0.15f;

                    Vector2 destination = new Vector2(Main.player[npc.target].Center.X, Main.player[npc.target].Center.Y - 500f);
                    npc.SimpleFlyMovement(Vector2.Normalize(destination - npc.Center) * flightVelocity, flightAcceleration);

                    // Spit homing missiles and then go to floating phase
                    npc.localAI[3] += 1f;
                    if (Vector2.Distance(npc.Center, destination) < 160f || npc.ai[2] > 0f || npc.localAI[3] > 120f)
                    {
                        float missileSpawnDivisor = death ? 8f : 12f;
                        npc.ai[2] += 1f;
                        if (npc.ai[2] % missileSpawnDivisor == 0f)
                        {
                            calamityGlobalNPC.newAI[1] += 1f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 velocity = new Vector2(-1f * (float)Main.rand.NextDouble() * 3f, 1f);
                                velocity = velocity.RotatedBy((Main.rand.NextDouble() - 0.5) * MathHelper.PiOver4);
                                velocity *= 0.25f;
                                int type = ProjectileID.RocketSkeleton;
                                int damage = npc.GetProjectileDamage(type);
                                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + Main.rand.Next(npc.width / 2), npc.Center.Y + 4f, velocity.X, velocity.Y, type, damage, 0f, Main.myPlayer, npc.target, 1f);
                                Main.projectile[proj].timeLeft = 540;
                            }

                            SoundEngine.PlaySound(SoundID.Item62, npc.Center);

                            if (calamityGlobalNPC.newAI[1] >= 10f)
                            {
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.localAI[3] = 0f;
                                calamityGlobalNPC.newAI[0] = 0f;
                                calamityGlobalNPC.newAI[1] = 0f;
                                npc.SyncVanillaLocalAI();
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
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

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

            // Movement
            float velocityY = bossRush ? 0.5f : death ? 2f : 2.5f;
            float velocityX = bossRush ? 1f : death ? 5f : 7f;
            float acceleration = bossRush ? 0.6f : death ? 0.3f : 0.25f;

            if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 100f)
            {
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= 0.9f;

                npc.velocity.Y -= acceleration;

                if (npc.velocity.Y > velocityY)
                    npc.velocity.Y = velocityY;
            }
            else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 100f)
            {
                if (npc.velocity.Y < 0f)
                    npc.velocity.Y *= 0.9f;

                npc.velocity.Y += acceleration;

                if (npc.velocity.Y < -velocityY)
                    npc.velocity.Y = -velocityY;
            }

            if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 180f * npc.ai[0])
            {
                if (npc.velocity.X > 0f)
                    npc.velocity.X *= 0.9f;

                npc.velocity.X -= acceleration;

                if (npc.velocity.X > velocityX)
                    npc.velocity.X = velocityX;
            }
            if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 180f * npc.ai[0])
            {
                if (npc.velocity.X < 0f)
                    npc.velocity.X *= 0.9f;

                npc.velocity.X += acceleration;

                if (npc.velocity.X < -velocityX)
                    npc.velocity.X = -velocityX;
            }

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

                Vector2 laserArmPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float laserArmTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - laserArmPosition.X;
                float laserArmTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - laserArmPosition.Y;
                float laserArmTargetDist = (float)Math.Sqrt(laserArmTargetX * laserArmTargetX + laserArmTargetY * laserArmTargetY);
                npc.rotation = (float)Math.Atan2(laserArmTargetY, laserArmTargetX) - MathHelper.PiOver2;

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
                        float laserSpeed = bossRush ? 5f : 4f;
                        int type = ProjectileID.DeathLaser;
                        int damage = npc.GetProjectileDamage(type);
                        laserArmTargetDist = laserSpeed / laserArmTargetDist;
                        laserArmTargetX *= laserArmTargetDist;
                        laserArmTargetY *= laserArmTargetDist;
                        laserArmPosition.X += laserArmTargetX * 30f;
                        laserArmPosition.Y += laserArmTargetY * 30f;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), laserArmPosition.X, laserArmPosition.Y, laserArmTargetX, laserArmTargetY, type, damage, 0f, Main.myPlayer, 1f, 0f);
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

                Vector2 laserRingArmPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float laserRingTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - laserRingArmPosition.X;
                float laserRingTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - laserRingArmPosition.Y;
                npc.rotation = (float)Math.Atan2(laserRingTargetY, laserRingTargetX) - MathHelper.PiOver2;

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
                        int totalProjectiles = bossRush ? 24 : 12;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        int type = ProjectileID.DeathLaser;
                        int damage = npc.GetProjectileDamage(type);
                        float velocity = 3f;
                        double angleA = radians * 0.5;
                        double angleB = MathHelper.ToRadians(90f) - angleA;
                        float laserVelocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                        Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-laserVelocityX, -velocity);
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 laserFireDirection = spinningPoint.RotatedBy(radians * k);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(laserFireDirection) * 30f, laserFireDirection, type, damage, 0f, Main.myPlayer, 1f, 0f);
                            Main.projectile[proj].timeLeft = 900;
                        }
                        npc.localAI[1] += 1f;
                    }
                }
            }

            return false;
        }

        public static bool BuffedPrimeCannonAI(NPC npc, Mod mod)
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

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

            // Movement
            float velocityY = bossRush ? 0.5f : death ? 2f : 2.5f;
            float velocityX = bossRush ? 1f : death ? 5f : 7f;
            float acceleration = bossRush ? 0.6f : death ? 0.3f : 0.25f;

            if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 150f)
            {
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= 0.9f;

                npc.velocity.Y -= acceleration;

                if (npc.velocity.Y > velocityY)
                    npc.velocity.Y = velocityY;
            }
            else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 150f)
            {
                if (npc.velocity.Y < 0f)
                    npc.velocity.Y *= 0.9f;

                npc.velocity.Y += acceleration;

                if (npc.velocity.Y < -velocityY)
                    npc.velocity.Y = -velocityY;
            }

            if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) + 200f)
            {
                if (npc.velocity.X > 0f)
                    npc.velocity.X *= 0.9f;

                npc.velocity.X -= acceleration;

                if (npc.velocity.X > velocityX)
                    npc.velocity.X = velocityX;
            }
            if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) + 160f)
            {
                if (npc.velocity.X < 0f)
                    npc.velocity.X *= 0.9f;

                npc.velocity.X += acceleration;

                if (npc.velocity.X < -velocityX)
                    npc.velocity.X = -velocityX;
            }

            if (fireSlower)
            {
                if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
                    npc.timeLeft = 10;

                Vector2 cannonArmPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float cannonArmTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - cannonArmPosition.X;
                float cannonArmTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - cannonArmPosition.Y;
                float cannonArmTargetDist = (float)Math.Sqrt(cannonArmTargetX * cannonArmTargetX + cannonArmTargetY * cannonArmTargetY);
                npc.rotation = (float)Math.Atan2(cannonArmTargetY, cannonArmTargetX) - MathHelper.PiOver2;

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
                        SoundEngine.PlaySound(SoundID.Item62, npc.Center);
                        npc.localAI[0] = 0f;
                        npc.TargetClosest();
                        int type = ProjectileID.RocketSkeleton;
                        int damage = npc.GetProjectileDamage(type);
                        cannonArmTargetDist = 0.5f / cannonArmTargetDist;
                        cannonArmTargetX *= cannonArmTargetDist;
                        cannonArmTargetY *= cannonArmTargetDist;
                        cannonArmPosition.X += cannonArmTargetX * 30f;
                        cannonArmPosition.Y += cannonArmTargetY * 30f;
                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), cannonArmPosition.X, cannonArmPosition.Y, cannonArmTargetX, cannonArmTargetY, type, damage, 0f, Main.myPlayer, npc.target, 1f);
                        Main.projectile[proj].timeLeft = 600;
                    }
                }
            }
            else
            {
                Vector2 cannonSpreadArmPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float cannonSpreadArmTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - cannonSpreadArmPosition.X;
                float cannonSpreadArmTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - cannonSpreadArmPosition.Y;
                npc.rotation = (float)Math.Atan2(cannonSpreadArmTargetY, cannonSpreadArmTargetX) - MathHelper.PiOver2;

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
                        SoundEngine.PlaySound(SoundID.Item62, npc.Center);
                        npc.localAI[0] = 0f;
                        npc.TargetClosest();
                        int type = ProjectileID.RocketSkeleton;
                        int damage = npc.GetProjectileDamage(type);
                        Vector2 cannonSpreadTargetDist = Main.player[npc.target].Center - npc.Center;
                        cannonSpreadTargetDist.Normalize();
                        cannonSpreadTargetDist *= 0.5f;
                        int numProj = bossRush ? 5 : 3;
                        float rotation = MathHelper.ToRadians(bossRush ? 8 : 5);
                        for (int i = 0; i < numProj; i++)
                        {
                            Vector2 perturbedSpeed = cannonSpreadTargetDist.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(perturbedSpeed) * 30f, perturbedSpeed, type, damage, 0f, Main.myPlayer, npc.target, 1f);
                            Main.projectile[proj].timeLeft = 600;
                        }
                    }
                }
            }

            return false;
        }

        public static bool BuffedPrimeViceAI(NPC npc, Mod mod)
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Direction
            npc.spriteDirection = -(int)npc.ai[0];

            // Where the vice should be in relation to the head
            Vector2 viceArmPosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float viceArmIdleXPos = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - viceArmPosition.X;
            float viceArmIdleYPos = Main.npc[(int)npc.ai[1]].position.Y + 230f - viceArmPosition.Y;
            float viceArmIdleDistance = (float)Math.Sqrt(viceArmIdleXPos * viceArmIdleXPos + viceArmIdleYPos * viceArmIdleYPos);

            // Return the vice to its proper location in relation to the head if it's too far away
            if (npc.ai[2] != 99f)
            {
                if (viceArmIdleDistance > 800f)
                    npc.ai[2] = 99f;
            }
            else if (viceArmIdleDistance < 400f)
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
                float velocityY = bossRush ? 1f : death ? 5f : 7f;
                float velocityX = bossRush ? 1.5f : death ? 8f : 10f;
                float acceleration = bossRush ? 0.6f : death ? 0.3f : 0.25f;

                if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.9f;

                    npc.velocity.Y -= acceleration;

                    if (npc.velocity.Y > velocityY)
                        npc.velocity.Y = velocityY;
                }
                else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.9f;

                    npc.velocity.Y += acceleration;

                    if (npc.velocity.Y < -velocityY)
                        npc.velocity.Y = -velocityY;
                }

                if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2))
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.9f;

                    npc.velocity.X -= acceleration * 2f;

                    if (npc.velocity.X > velocityX)
                        npc.velocity.X = velocityX;
                }
                if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2))
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.9f;

                    npc.velocity.X += acceleration * 2f;

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

                    float velocityY = bossRush ? 0.5f : death ? 2f : 2.5f;
                    float velocityX = bossRush ? 1.5f : death ? 7f : 8f;
                    float velocityX2 = bossRush ? 1.25f : death ? 6f : 7f;
                    float acceleration = bossRush ? 0.6f : death ? 0.3f : 0.25f;

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 300f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= 0.9f;

                        npc.velocity.Y -= acceleration;

                        if (npc.velocity.Y > velocityY)
                            npc.velocity.Y = velocityY;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 230f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= 0.9f;

                        npc.velocity.Y += acceleration;

                        if (npc.velocity.Y < -velocityY)
                            npc.velocity.Y = -velocityY;
                    }

                    if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) + 250f)
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= 0.9f;

                        npc.velocity.X -= acceleration;

                        if (npc.velocity.X > velocityX)
                            npc.velocity.X = velocityX;
                    }
                    if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2))
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= 0.9f;

                        npc.velocity.X += acceleration;

                        if (npc.velocity.X < -velocityX2)
                            npc.velocity.X = -velocityX2;
                    }

                    Vector2 viceArmReelbackCurrentPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float viceArmReelbackXDest = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - viceArmReelbackCurrentPos.X;
                    float viceArmReelbackYDest = Main.npc[(int)npc.ai[1]].position.Y + 230f - viceArmReelbackCurrentPos.Y;
                    npc.rotation = (float)Math.Atan2(viceArmReelbackYDest, viceArmReelbackXDest) + MathHelper.PiOver2;
                    return false;
                }

                // Charge towards the player
                if (npc.ai[2] == 1f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.9f;

                    Vector2 viceArmChargePosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float viceArmChargeTargetX = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 280f * npc.ai[0] - viceArmChargePosition.X;
                    float viceArmChargeTargetY = Main.npc[(int)npc.ai[1]].position.Y + 230f - viceArmChargePosition.Y;
                    npc.rotation = (float)Math.Atan2(viceArmChargeTargetY, viceArmChargeTargetX) + MathHelper.PiOver2;

                    npc.velocity.X = (npc.velocity.X * 5f + Main.npc[(int)npc.ai[1]].velocity.X) / 6f;
                    npc.velocity.X += 0.5f;

                    npc.velocity.Y -= 0.5f;
                    if (npc.velocity.Y < -12f)
                        npc.velocity.Y = -12f;

                    if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 280f)
                    {
                        float chargeVelocity = bossRush ? 20f : 16f;
                        if (!cannonAlive)
                            chargeVelocity += 1.5f;
                        if (!laserAlive)
                            chargeVelocity += 1.5f;
                        if (!sawAlive)
                            chargeVelocity += 1.5f;

                        npc.ai[2] = 2f;
                        npc.TargetClosest();
                        viceArmChargePosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        viceArmChargeTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - viceArmChargePosition.X;
                        viceArmChargeTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - viceArmChargePosition.Y;
                        float viceArmChargeTargetDist = (float)Math.Sqrt(viceArmChargeTargetX * viceArmChargeTargetX + viceArmChargeTargetY * viceArmChargeTargetY);
                        viceArmChargeTargetDist = chargeVelocity / viceArmChargeTargetDist;
                        npc.velocity.X = viceArmChargeTargetX * viceArmChargeTargetDist;
                        npc.velocity.Y = viceArmChargeTargetY * viceArmChargeTargetDist;
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
                    Vector2 viceArmOtherChargePosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float viceArmOtherChargeTargetX = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - viceArmOtherChargePosition.X;
                    float viceArmOtherChargeTargetY = Main.npc[(int)npc.ai[1]].position.Y + 230f - viceArmOtherChargePosition.Y;
                    npc.rotation = (float)Math.Atan2(viceArmOtherChargeTargetY, viceArmOtherChargeTargetX) + MathHelper.PiOver2;

                    npc.velocity.Y = (npc.velocity.Y * 5f + Main.npc[(int)npc.ai[1]].velocity.Y) / 6f;

                    npc.velocity.X += 0.5f;
                    if (npc.velocity.X > 12f)
                        npc.velocity.X = 12f;

                    if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 500f || npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) + 500f)
                    {
                        float chargeVelocity = bossRush ? 17.5f : 14f;
                        if (!cannonAlive)
                            chargeVelocity += 1.15f;
                        if (!laserAlive)
                            chargeVelocity += 1.15f;
                        if (!sawAlive)
                            chargeVelocity += 1.15f;

                        npc.ai[2] = 5f;
                        npc.TargetClosest();
                        viceArmOtherChargePosition = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        viceArmOtherChargeTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - viceArmOtherChargePosition.X;
                        viceArmOtherChargeTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - viceArmOtherChargePosition.Y;
                        float viceArmOtherChargeTargetDist = (float)Math.Sqrt(viceArmOtherChargeTargetX * viceArmOtherChargeTargetX + viceArmOtherChargeTargetY * viceArmOtherChargeTargetY);
                        viceArmOtherChargeTargetDist = chargeVelocity / viceArmOtherChargeTargetDist;
                        npc.velocity.X = viceArmOtherChargeTargetX * viceArmOtherChargeTargetDist;
                        npc.velocity.Y = viceArmOtherChargeTargetY * viceArmOtherChargeTargetDist;
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
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            Vector2 sawArmLocation = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float sawArmIdleXPos = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - sawArmLocation.X;
            float sawArmIdleYPos = Main.npc[(int)npc.ai[1]].position.Y + 230f - sawArmLocation.Y;
            float sawArmIdleDistance = (float)Math.Sqrt(sawArmIdleXPos * sawArmIdleXPos + sawArmIdleYPos * sawArmIdleYPos);

            if (npc.ai[2] != 99f)
            {
                if (sawArmIdleDistance > 800f)
                    npc.ai[2] = 99f;
            }
            else if (sawArmIdleDistance < 400f)
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
                float velocityY = bossRush ? 1f : death ? 6f : 7f;
                float velocityX = bossRush ? 1.5f : death ? 8f : 10f;
                float acceleration = bossRush ? 0.6f : death ? 0.3f : 0.25f;

                if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.9f;

                    npc.velocity.Y -= acceleration;

                    if (npc.velocity.Y > velocityY)
                        npc.velocity.Y = velocityY;
                }
                else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.9f;

                    npc.velocity.Y += acceleration;

                    if (npc.velocity.Y < -velocityY)
                        npc.velocity.Y = -velocityY;
                }

                if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2))
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.9f;

                    npc.velocity.X -= acceleration * 2f;

                    if (npc.velocity.X > velocityX)
                        npc.velocity.X = velocityX;
                }
                if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2))
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.9f;

                    npc.velocity.X += acceleration * 2f;

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

                    float velocityY = bossRush ? 0.5f : death ? 2f : 2.5f;
                    float velocityX = bossRush ? 1.5f : death ? 8f : 10f;
                    float acceleration = bossRush ? 0.6f : death ? 0.3f : 0.25f;

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 320f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= 0.9f;

                        npc.velocity.Y -= acceleration;

                        if (npc.velocity.Y > velocityY)
                            npc.velocity.Y = velocityY;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 260f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= 0.9f;

                        npc.velocity.Y += acceleration;

                        if (npc.velocity.Y < -velocityY)
                            npc.velocity.Y = -velocityY;
                    }

                    if (npc.position.X + (npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2))
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= 0.9f;

                        npc.velocity.X -= acceleration * 1.5f;

                        if (npc.velocity.X > velocityX)
                            npc.velocity.X = velocityX;
                    }
                    if (npc.position.X + (npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 250f)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= 0.9f;

                        npc.velocity.X += acceleration * 1.5f;

                        if (npc.velocity.X < -velocityX)
                            npc.velocity.X = -velocityX;
                    }

                    Vector2 sawArmReelbackCurrentPos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float sawArmReelbackXDest = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - sawArmReelbackCurrentPos.X;
                    float sawArmReelbackYDest = Main.npc[(int)npc.ai[1]].position.Y + 230f - sawArmReelbackCurrentPos.Y;
                    npc.rotation = (float)Math.Atan2(sawArmReelbackYDest, sawArmReelbackXDest) + MathHelper.PiOver2;
                    return false;
                }

                if (npc.ai[2] == 1f)
                {
                    Vector2 sawArmChargePos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    float sawArmChargeTargetX = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - sawArmChargePos.X;
                    float sawArmChargeTargetY = Main.npc[(int)npc.ai[1]].position.Y + 230f - sawArmChargePos.Y;
                    npc.rotation = (float)Math.Atan2(sawArmChargeTargetY, sawArmChargeTargetX) + MathHelper.PiOver2;

                    npc.velocity.X *= 0.95f;
                    npc.velocity.Y -= 0.5f;
                    if (npc.velocity.Y < -12f)
                        npc.velocity.Y = -12f;

                    if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 200f)
                    {
                        float chargeVelocity = bossRush ? 27.5f : 22f;
                        if (!cannonAlive)
                            chargeVelocity += 1.5f;
                        if (!laserAlive)
                            chargeVelocity += 1.5f;
                        if (!viceAlive)
                            chargeVelocity += 1.5f;

                        npc.ai[2] = 2f;
                        npc.TargetClosest();
                        sawArmChargePos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        sawArmChargeTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - sawArmChargePos.X;
                        sawArmChargeTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - sawArmChargePos.Y;
                        float sawArmChargeTargetDist = (float)Math.Sqrt(sawArmChargeTargetX * sawArmChargeTargetX + sawArmChargeTargetY * sawArmChargeTargetY);
                        sawArmChargeTargetDist = chargeVelocity / sawArmChargeTargetDist;
                        npc.velocity.X = sawArmChargeTargetX * sawArmChargeTargetDist;
                        npc.velocity.Y = sawArmChargeTargetY * sawArmChargeTargetDist;
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
                        float chargeVelocity = bossRush ? 13.5f : 11f;
                        if (!cannonAlive)
                            chargeVelocity += 1.5f;
                        if (!laserAlive)
                            chargeVelocity += 1.5f;
                        if (!viceAlive)
                            chargeVelocity += 1.5f;

                        Vector2 sawArmOtherChargePos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float sawArmOtherChargeTargetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - sawArmOtherChargePos.X;
                        float sawArmOtherChargeTargetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - sawArmOtherChargePos.Y;
                        float sawArmOtherChargeTargetDist = (float)Math.Sqrt(sawArmOtherChargeTargetX * sawArmOtherChargeTargetX + sawArmOtherChargeTargetY * sawArmOtherChargeTargetY);
                        sawArmOtherChargeTargetDist = chargeVelocity / sawArmOtherChargeTargetDist;
                        sawArmOtherChargeTargetX *= sawArmOtherChargeTargetDist;
                        sawArmOtherChargeTargetY *= sawArmOtherChargeTargetDist;

                        float acceleration = bossRush ? 0.3f : death ? 0.1f : 0.08f;
                        if (npc.velocity.X > sawArmOtherChargeTargetX)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X *= 0.9f;

                            npc.velocity.X -= acceleration;
                        }
                        if (npc.velocity.X < sawArmOtherChargeTargetX)
                        {
                            if (npc.velocity.X < 0f)
                                npc.velocity.X *= 0.9f;

                            npc.velocity.X += acceleration;
                        }
                        if (npc.velocity.Y > sawArmOtherChargeTargetY)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y *= 0.9f;

                            npc.velocity.Y -= acceleration;
                        }
                        if (npc.velocity.Y < sawArmOtherChargeTargetY)
                        {
                            if (npc.velocity.Y < 0f)
                                npc.velocity.Y *= 0.9f;

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

                        sawArmOtherChargePos = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        sawArmOtherChargeTargetX = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - sawArmOtherChargePos.X;
                        sawArmOtherChargeTargetY = Main.npc[(int)npc.ai[1]].position.Y + 230f - sawArmOtherChargePos.Y;
                        npc.rotation = (float)Math.Atan2(sawArmOtherChargeTargetY, sawArmOtherChargeTargetX) + MathHelper.PiOver2;
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
