using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    public static class SkeletronPrimeAI
    {
        public static bool BuffedSkeletronPrimeAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool bossRush = BossRushEvent.BossRushActive;
            bool masterMode = Main.masterMode || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            if (npc.ai[3] != 0f)
                NPC.mechQueen = npc.whoAmI;

            // Spawn arms
            if (calamityGlobalNPC.newAI[1] == 0f)
            {
                calamityGlobalNPC.newAI[1] = 1f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Spawn second head in Master Mode
                    // The main head owns the Saw and the Laser in Master Mode
                    if (masterMode)
                    {
                        int head = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<SkeletronPrime2>(), npc.whoAmI);
                        Main.npc[head].ai[0] = npc.whoAmI;
                        Main.npc[head].target = npc.target;
                        Main.npc[head].netUpdate = true;
                        npc.ai[0] = head;

                        int arm = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeSaw, npc.whoAmI);
                        Main.npc[arm].ai[0] = 1f;
                        Main.npc[arm].ai[1] = npc.whoAmI;
                        Main.npc[arm].target = npc.target;
                        Main.npc[arm].netUpdate = true;

                        arm = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeLaser, npc.whoAmI);
                        Main.npc[arm].ai[0] = 1f;
                        Main.npc[arm].ai[1] = npc.whoAmI;
                        Main.npc[arm].target = npc.target;
                        Main.npc[arm].netUpdate = true;
                        Main.npc[arm].ai[3] = 150f;
                    }
                    else
                    {
                        int arm = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeCannon, npc.whoAmI);
                        Main.npc[arm].ai[0] = -1f;
                        Main.npc[arm].ai[1] = npc.whoAmI;
                        Main.npc[arm].target = npc.target;
                        Main.npc[arm].netUpdate = true;

                        arm = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeSaw, npc.whoAmI);
                        Main.npc[arm].ai[0] = 1f;
                        Main.npc[arm].ai[1] = npc.whoAmI;
                        Main.npc[arm].target = npc.target;
                        Main.npc[arm].netUpdate = true;

                        arm = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeVice, npc.whoAmI);
                        Main.npc[arm].ai[0] = -1f;
                        Main.npc[arm].ai[1] = npc.whoAmI;
                        Main.npc[arm].target = npc.target;
                        Main.npc[arm].ai[3] = 150f;
                        Main.npc[arm].netUpdate = true;

                        arm = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeLaser, npc.whoAmI);
                        Main.npc[arm].ai[0] = 1f;
                        Main.npc[arm].ai[1] = npc.whoAmI;
                        Main.npc[arm].target = npc.target;
                        Main.npc[arm].netUpdate = true;
                        Main.npc[arm].ai[3] = 150f;
                    }
                }

                npc.netUpdate = true;
                npc.SyncExtraAI();
            }

            if (masterMode)
            {
                if (!Main.npc[(int)npc.ai[0]].active || Main.npc[(int)npc.ai[0]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
                {
                    npc.life = 0;
                    npc.HitEffect();
                    npc.active = false;
                    npc.netUpdate = true;
                }
                else
                {
                    // Link the HP of both heads
                    if (npc.life > Main.npc[(int)npc.ai[0]].life)
                        npc.life = Main.npc[(int)npc.ai[0]].life;

                    // Push away from the lead head if too close, pull closer if too far, if Mechdusa isn't real
                    if (!NPC.IsMechQueenUp)
                    {
                        float pushVelocity = 0.25f;
                        if (Vector2.Distance(npc.Center, Main.npc[(int)npc.ai[0]].Center) < 80f * npc.scale)
                        {
                            if (npc.position.X < Main.npc[(int)npc.ai[0]].position.X)
                                npc.velocity.X -= pushVelocity;
                            else
                                npc.velocity.X += pushVelocity;

                            if (npc.position.Y < Main.npc[(int)npc.ai[0]].position.Y)
                                npc.velocity.Y -= pushVelocity;
                            else
                                npc.velocity.Y += pushVelocity;
                        }
                        else if (Vector2.Distance(npc.Center, Main.npc[(int)npc.ai[0]].Center) > 240f * npc.scale)
                        {
                            if (npc.position.X < Main.npc[(int)npc.ai[0]].position.X)
                                npc.velocity.X += pushVelocity;
                            else
                                npc.velocity.X -= pushVelocity;

                            if (npc.position.Y < Main.npc[(int)npc.ai[0]].position.Y)
                                npc.velocity.Y += pushVelocity;
                            else
                                npc.velocity.Y -= pushVelocity;
                        }
                    }
                }
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

            npc.defense = npc.defDefense;

            // Phases
            bool phase2 = lifeRatio < 0.66f;
            bool spawnDestroyer = lifeRatio < 0.75f && masterMode && !bossRush && npc.localAI[2] == 0f;
            bool phase3 = lifeRatio < 0.33f;
            bool spawnRetinazer = lifeRatio < 0.5f && masterMode && !bossRush && npc.localAI[2] == 1f;

            // Spawn The Destroyer in Master Mode (just like Oblivion from Avalon)
            if (spawnDestroyer)
            {
                Player destroyerSpawnPlayer = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];
                SoundEngine.PlaySound(SoundID.Roar, destroyerSpawnPlayer.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(destroyerSpawnPlayer.whoAmI, NPCID.TheDestroyer);

                npc.localAI[2] = 1f;
                npc.SyncVanillaLocalAI();
            }

            // Spawn Retinazer in Master Mode (just like Oblivion from Avalon)
            if (spawnRetinazer)
            {
                Player retinazerSpawnPlayer = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];
                SoundEngine.PlaySound(SoundID.Roar, retinazerSpawnPlayer.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(retinazerSpawnPlayer.whoAmI, NPCID.Retinazer);

                npc.localAI[2] = 2f;
                npc.SyncVanillaLocalAI();
            }

            // Despawn
            if (npc.ai[1] != 3f)
            {
                int despawnDistanceInTiles = 500;
                if (Main.player[npc.target].dead || Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) / 16f > despawnDistanceInTiles)
                {
                    npc.TargetClosest();
                    if (Main.player[npc.target].dead || Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) / 16f > despawnDistanceInTiles)
                        npc.ai[1] = 3f;
                }
                else if (npc.timeLeft < 1800)
                    npc.timeLeft = 1800;
            }

            // Activate daytime enrage
            if (Main.IsItDay() && !bossRush && npc.ai[1] != 3f && npc.ai[1] != 2f)
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
                SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center);
            }

            // Adjust slowing debuff immunity
            bool immuneToSlowingDebuffs = npc.ai[1] == 5f;
            npc.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TimeDistortion>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<GalvanicCorrosion>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

            bool normalLaserRotation = npc.localAI[1] % 2f == 0f;

            // Prevents cheap hits
            bool canUseAttackInMaster = npc.position.Y < Main.player[npc.target].position.Y - 350f;

            // Float near player
            if (npc.ai[1] == 0f || npc.ai[1] == 4f)
            {
                // Avoid unfair bullshit
                npc.damage = 0;

                // Start other phases; if arms are dead, start with spin phase
                bool otherHeadChargingOrSpinning = Main.npc[(int)npc.ai[0]].ai[1] == 5f || Main.npc[(int)npc.ai[0]].ai[1] == 1f;
                if (phase2 || CalamityWorld.LegendaryMode || allArmsDead || masterMode)
                {
                    // Start spin phase after 1.5 seconds
                    npc.ai[2] += phase3 ? 1.5f : 1f;
                    if (npc.ai[2] >= (90f - (death ? (masterMode ? 15f : 60f) * (1f - lifeRatio) : 0f)) && (!otherHeadChargingOrSpinning || !masterMode || phase3) && (canUseAttackInMaster || !masterMode))
                    {
                        bool shouldSpinAround = npc.ai[1] == 4f && npc.position.Y < Main.player[npc.target].position.Y - 400f &&
                            Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 600f && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 400f;

                        bool shouldCharge = masterMode && !phase2 && !allArmsDead && !CalamityWorld.LegendaryMode;
                        if (shouldCharge)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[1] = 1f;
                            npc.netUpdate = true;
                        }
                        else if (shouldSpinAround || npc.ai[1] != 4f)
                        {
                            if (shouldSpinAround)
                            {
                                npc.localAI[3] = 300f;
                                npc.SyncVanillaLocalAI();
                            }

                            npc.ai[2] = 0f;
                            npc.ai[1] = shouldSpinAround ? 5f : 1f;
                            npc.netUpdate = true;
                        }
                    }
                }

                if (NPC.IsMechQueenUp)
                    npc.rotation = npc.rotation.AngleLerp(npc.velocity.X / 15f * 0.5f, 0.75f);
                else
                    npc.rotation = npc.velocity.X / 15f;

                float acceleration = (bossRush ? 0.2f : masterMode ? 0.125f : 0.1f) + (death ? 0.05f * (1f - lifeRatio) : 0f);
                float accelerationMult = 1f;
                if (!cannonAlive)
                {
                    acceleration += 0.025f;
                    accelerationMult += 0.5f;
                }
                if (!laserAlive)
                {
                    acceleration += 0.025f;
                    accelerationMult += 0.5f;
                }
                if (!viceAlive)
                    acceleration += 0.025f;
                if (!sawAlive)
                    acceleration += 0.025f;
                if (masterMode)
                    acceleration *= accelerationMult;

                float topVelocity = acceleration * 100f;
                float deceleration = masterMode ? 0.7f : 0.85f;

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

                if (npc.position.Y > Main.player[npc.target].position.Y - (400f + headDecelerationUpDist))
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y -= acceleration;

                    if (npc.velocity.Y > topVelocity)
                        npc.velocity.Y = topVelocity;
                }
                else if (npc.position.Y < Main.player[npc.target].position.Y - (450f + headDecelerationDownDist))
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y += acceleration;

                    if (npc.velocity.Y < -topVelocity)
                        npc.velocity.Y = -topVelocity;
                }

                if (npc.Center.X > Main.player[npc.target].Center.X + (400f + headDecelerationHorizontalDist))
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X -= acceleration;

                    if (npc.velocity.X > topVelocity)
                        npc.velocity.X = topVelocity;
                }
                if (npc.Center.X < Main.player[npc.target].Center.X - (400f + headDecelerationHorizontalDist))
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X += acceleration;

                    if (npc.velocity.X < -topVelocity)
                        npc.velocity.X = -topVelocity;
                }
            }

            else
            {
                // Spinning
                if (npc.ai[1] == 1f)
                {
                    npc.defense *= 2;
                    npc.damage = npc.defDamage * 2;

                    calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = true;

                    if (phase2 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[0] += 1f;
                        if (npc.localAI[0] >= 45f)
                        {
                            npc.localAI[0] = 0f;

                            int totalProjectiles = bossRush ? 24 : death ? (masterMode ? 15 : 18) : 12;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = ProjectileID.DeathLaser;
                            int damage = npc.GetProjectileDamage(type);

                            // Reduce mech boss projectile damage depending on the new ore progression changes
                            if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                            {
                                double firstMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert;
                                double secondMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert;
                                if (!NPC.downedMechBossAny)
                                    damage = (int)(damage * firstMechMultiplier);
                                else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                    damage = (int)(damage * secondMechMultiplier);
                            }

                            float velocity = 3f;
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 laserFireDirection = spinningPoint.RotatedBy(radians * k);
                                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + laserFireDirection.SafeNormalize(Vector2.UnitY) * 100f, laserFireDirection, type, damage, 0f, Main.myPlayer, 1f, 0f);
                                Main.projectile[proj].timeLeft = 900;
                            }
                            npc.localAI[1] += 1f;
                        }
                    }

                    npc.ai[2] += 1f;
                    if (npc.ai[2] == 2f)
                        SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center);

                    // Spin for 3 seconds then return to floating phase
                    float phaseTimer = 240f;
                    if (phase2 && !phase3)
                        phaseTimer += 60f;

                    if (npc.ai[2] >= (phaseTimer - (death ? 60f * (1f - lifeRatio) : 0f)))
                    {
                        npc.TargetClosest();
                        npc.ai[2] = 0f;
                        npc.ai[1] = 4f;
                        npc.localAI[0] = 0f;
                    }

                    if (NPC.IsMechQueenUp)
                        npc.rotation = npc.rotation.AngleLerp(npc.velocity.X / 15f * 0.5f, 0.75f);
                    else
                        npc.rotation += npc.direction * 0.3f;

                    Vector2 headPosition = npc.Center;
                    float headTargetX = Main.player[npc.target].Center.X - headPosition.X;
                    float headTargetY = Main.player[npc.target].Center.Y - headPosition.Y;
                    float headTargetDistance = (float)Math.Sqrt(headTargetX * headTargetX + headTargetY * headTargetY);

                    float speed = bossRush ? 12f : (masterMode ? 8f : 6f);
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

                    calamityGlobalNPC.CurrentlyEnraged = true;
                    calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = true;

                    if (NPC.IsMechQueenUp)
                        npc.rotation = npc.rotation.AngleLerp(npc.velocity.X / 15f * 0.5f, 0.75f);
                    else
                        npc.rotation += npc.direction * 0.3f;

                    Vector2 enragedHeadPosition = npc.Center;
                    float enragedHeadTargetX = Main.player[npc.target].Center.X - enragedHeadPosition.X;
                    float enragedHeadTargetY = Main.player[npc.target].Center.Y - enragedHeadPosition.Y;
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
                                float enragedHeadSkullTargetX = Main.player[npc.target].Center.X - headCenter.X + Main.rand.Next(-20, 21);
                                float enragedHeadSkullTargetY = Main.player[npc.target].Center.Y - headCenter.Y + Main.rand.Next(-20, 21);
                                float enragedHeadSkullTargetDist = (float)Math.Sqrt(enragedHeadSkullTargetX * enragedHeadSkullTargetX + enragedHeadSkullTargetY * enragedHeadSkullTargetY);
                                enragedHeadSkullTargetDist = enragedHeadSpeed / enragedHeadSkullTargetDist;
                                enragedHeadSkullTargetX *= enragedHeadSkullTargetDist;
                                enragedHeadSkullTargetY *= enragedHeadSkullTargetDist;

                                Vector2 value = new Vector2(enragedHeadSkullTargetX * 1f + Main.rand.Next(-50, 51) * 0.01f, enragedHeadSkullTargetY * 1f + Main.rand.Next(-50, 51) * 0.01f).SafeNormalize(Vector2.UnitY);
                                value *= enragedHeadSpeed;
                                value += npc.velocity;
                                enragedHeadSkullTargetX = value.X;
                                enragedHeadSkullTargetY = value.Y;

                                int type = ProjectileID.Skull;
                                headCenter += value * 5f;
                                int enragedSkulls = Projectile.NewProjectile(npc.GetSource_FromAI(), headCenter.X, headCenter.Y, enragedHeadSkullTargetX, enragedHeadSkullTargetY, type, 250, 0f, Main.myPlayer, -3f, 0f);
                                Main.projectile[enragedSkulls].timeLeft = 300;
                            }
                        }
                    }
                }

                // Despawning
                if (npc.ai[1] == 3f)
                {
                    // Avoid unfair bullshit
                    npc.damage = 0;

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

                // Fly around in a circle
                if (npc.ai[1] == 5f)
                {
                    // Avoid unfair bullshit
                    npc.damage = 0;

                    npc.ai[2] += 1f;

                    npc.rotation = npc.velocity.X / 50f;

                    float skullSpawnDivisor = bossRush ? 9f : death ? 15f - (float)Math.Round((masterMode ? 3f : 5f) * (1f - lifeRatio)) : 15f;
                    float totalSkulls = 12f;
                    int skullSpread = bossRush ? 250 : death ? (masterMode ? 125 : 150) : 100;

                    // Spin for about 3 seconds
                    float spinVelocity = 30f;
                    if (npc.ai[2] == 2f)
                    {
                        // Play angry noise
                        SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center);

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
                        npc.SyncExtraAI();
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
                            npc.localAI[0] += 1f;

                            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 64f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 headCenter = npc.Center;
                                    float enragedHeadSpeed = (masterMode ? 5f : 4f) + (death ? (masterMode ? 1f : 2f) * (1f - lifeRatio) : 0f);
                                    float enragedHeadSkullTargetX = Main.player[npc.target].Center.X - headCenter.X + Main.rand.Next(-20, 21);
                                    float enragedHeadSkullTargetY = Main.player[npc.target].Center.Y - headCenter.Y + Main.rand.Next(-20, 21);
                                    float enragedHeadSkullTargetDist = (float)Math.Sqrt(enragedHeadSkullTargetX * enragedHeadSkullTargetX + enragedHeadSkullTargetY * enragedHeadSkullTargetY);
                                    enragedHeadSkullTargetDist = enragedHeadSpeed / enragedHeadSkullTargetDist;
                                    enragedHeadSkullTargetX *= enragedHeadSkullTargetDist;
                                    enragedHeadSkullTargetY *= enragedHeadSkullTargetDist;

                                    Vector2 value = new Vector2(enragedHeadSkullTargetX + Main.rand.Next(-skullSpread, skullSpread + 1) * 0.01f, enragedHeadSkullTargetY + Main.rand.Next(-skullSpread, skullSpread + 1) * 0.01f).SafeNormalize(Vector2.UnitY);
                                    value *= enragedHeadSpeed;
                                    enragedHeadSkullTargetX = value.X;
                                    enragedHeadSkullTargetY = value.Y;

                                    int type = ProjectileID.Skull;
                                    int damage = npc.GetProjectileDamage(type);

                                    // Reduce mech boss projectile damage depending on the new ore progression changes
                                    if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                                    {
                                        double firstMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert;
                                        double secondMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert;
                                        if (!NPC.downedMechBossAny)
                                            damage = (int)(damage * firstMechMultiplier);
                                        else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                            damage = (int)(damage * secondMechMultiplier);
                                    }

                                    if (npc.localAI[0] % 3f == 0f)
                                    {
                                        int probeLimit = death ? (masterMode ? 3 : 4) : 2;
                                        if (NPC.CountNPCS(NPCID.Probe) < probeLimit)
                                            NPC.NewNPC(npc.GetSource_FromAI(), (int)headCenter.X, (int)headCenter.Y + 30, NPCID.Probe);
                                    }

                                    int enragedSkulls = Projectile.NewProjectile(npc.GetSource_FromAI(), headCenter.X, headCenter.Y + 30f, enragedHeadSkullTargetX, enragedHeadSkullTargetY, type, damage, 0f, Main.myPlayer, -3f, 0f);
                                    Main.projectile[enragedSkulls].timeLeft = 480;
                                    Main.projectile[enragedSkulls].tileCollide = false;
                                }
                            }

                            // Go to floating phase, or spinning phase if in phase 2
                            if (npc.localAI[0] >= totalSkulls)
                            {
                                npc.velocity = npc.velocity.SafeNormalize(Vector2.UnitY);

                                // Fly overhead and spit missiles if on low health
                                npc.ai[1] = phase3 ? 6f : 1f;
                                npc.ai[2] = 0f;
                                npc.localAI[3] = 0f;
                                npc.localAI[0] = 0f;
                                calamityGlobalNPC.newAI[0] = 0f;
                                npc.SyncVanillaLocalAI();
                                npc.SyncExtraAI();
                                npc.TargetClosest();
                                npc.netUpdate = true;
                            }
                        }
                    }
                }

                // Fly overhead and spit missiles
                if (npc.ai[1] == 6f)
                {
                    // Avoid unfair bullshit
                    npc.damage = 0;

                    npc.rotation = npc.velocity.X / 15f;

                    float flightVelocity = bossRush ? 28f : death ? 24f : 20f;
                    float flightAcceleration = bossRush ? 1.12f : death ? 0.96f : 0.8f;

                    if (masterMode)
                    {
                        flightVelocity += 4f;
                        flightAcceleration += 0.16f;
                    }

                    Vector2 destination = new Vector2(Main.player[npc.target].Center.X, Main.player[npc.target].Center.Y - 500f);
                    npc.SimpleFlyMovement((destination - npc.Center).SafeNormalize(Vector2.UnitY) * flightVelocity, flightAcceleration);

                    // Spit homing missiles and then go to floating phase
                    npc.localAI[3] += 1f;
                    if (Vector2.Distance(npc.Center, destination) < 80f || npc.ai[2] > 0f || npc.localAI[3] > 120f)
                    {
                        float missileSpawnDivisor = death ? 8f : (masterMode ? 10f : 12f);
                        float totalMissiles = masterMode ? 12f : 10f;
                        npc.ai[2] += 1f;
                        if (npc.ai[2] % missileSpawnDivisor == 0f)
                        {
                            npc.localAI[0] += 1f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 velocity = new Vector2(-1f * (float)Main.rand.NextDouble() * 5f, 1f);
                                velocity = velocity.RotatedBy((Main.rand.NextDouble() - 0.5) * MathHelper.PiOver4);
                                int type = ProjectileID.RocketSkeleton;
                                int damage = npc.GetProjectileDamage(type);

                                // Reduce mech boss projectile damage depending on the new ore progression changes
                                if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                                {
                                    double firstMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert;
                                    double secondMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert;
                                    if (!NPC.downedMechBossAny)
                                        damage = (int)(damage * firstMechMultiplier);
                                    else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                        damage = (int)(damage * secondMechMultiplier);
                                }

                                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + Main.rand.Next(npc.width / 2), npc.Center.Y + 30f, velocity.X, velocity.Y, type, damage, 0f, Main.myPlayer, npc.target, 1f);
                                Main.projectile[proj].timeLeft = 540;
                            }

                            SoundEngine.PlaySound(SoundID.Item62, npc.Center);

                            if (npc.localAI[0] >= totalMissiles)
                            {
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.localAI[3] = 0f;
                                calamityGlobalNPC.newAI[0] = 0f;
                                npc.localAI[0] = 0f;
                                npc.SyncVanillaLocalAI();
                                npc.SyncExtraAI();
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
            bool masterMode = Main.masterMode || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Set direction
            npc.spriteDirection = -(int)npc.ai[0];

            // Despawn if head is gone
            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
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
            float timeToNotAttack = 180f;
            bool dontAttack = npc.Calamity().newAI[2] < timeToNotAttack;
            if (dontAttack)
            {
                npc.Calamity().newAI[2] += 1f;
                if (npc.Calamity().newAI[2] >= timeToNotAttack)
                    npc.SyncExtraAI();
            }

            // Avoid cheap bullshit
            npc.damage = 0;

            bool normalLaserRotation = npc.localAI[1] % 2f == 0f;

            // Movement
            float acceleration = (bossRush ? 0.6f : death ? (masterMode ? 0.375f : 0.3f) : (masterMode ? 0.3125f : 0.25f));
            float accelerationMult = 1f;
            if (!cannonAlive)
            {
                acceleration += 0.025f;
                accelerationMult += 0.5f;
            }
            if (!viceAlive)
                acceleration += 0.025f;
            if (!sawAlive)
                acceleration += 0.025f;
            if (masterMode)
                acceleration *= accelerationMult;

            float topVelocity = acceleration * 100f;
            float deceleration = masterMode ? 0.6f : 0.8f;

            if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 80f)
            {
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= deceleration;

                npc.velocity.Y -= acceleration;

                if (npc.velocity.Y > topVelocity)
                    npc.velocity.Y = topVelocity;
            }
            else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 120f)
            {
                if (npc.velocity.Y < 0f)
                    npc.velocity.Y *= deceleration;

                npc.velocity.Y += acceleration;

                if (npc.velocity.Y < -topVelocity)
                    npc.velocity.Y = -topVelocity;
            }

            if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X - 160f * npc.ai[0])
            {
                if (npc.velocity.X > 0f)
                    npc.velocity.X *= deceleration;

                npc.velocity.X -= acceleration;

                if (npc.velocity.X > topVelocity)
                    npc.velocity.X = topVelocity;
            }
            if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0])
            {
                if (npc.velocity.X < 0f)
                    npc.velocity.X *= deceleration;

                npc.velocity.X += acceleration;

                if (npc.velocity.X < -topVelocity)
                    npc.velocity.X = -topVelocity;
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

                if (npc.ai[3] >= (masterMode ? 200f : 800f))
                {
                    npc.target = Main.npc[(int)npc.ai[1]].target;
                    npc.localAI[0] = 0f;
                    npc.ai[2] = 1f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }

                Vector2 laserArmPosition = npc.Center;
                float laserArmTargetX = Main.player[npc.target].Center.X - laserArmPosition.X;
                float laserArmTargetY = Main.player[npc.target].Center.Y - laserArmPosition.Y;
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

                    if (npc.localAI[0] >= 48f)
                    {
                        npc.localAI[0] = 0f;
                        float laserSpeed = bossRush ? 5f : 4f;
                        int type = ProjectileID.DeathLaser;
                        int damage = npc.GetProjectileDamage(type);

                        // Reduce mech boss projectile damage depending on the new ore progression changes
                        if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                        {
                            double firstMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert;
                            double secondMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert;
                            if (!NPC.downedMechBossAny)
                                damage = (int)(damage * firstMechMultiplier);
                            else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                damage = (int)(damage * secondMechMultiplier);
                        }

                        laserArmTargetDist = laserSpeed / laserArmTargetDist;
                        laserArmTargetX *= laserArmTargetDist;
                        laserArmTargetY *= laserArmTargetDist;
                        Vector2 laserVelocity = new Vector2(laserArmTargetX, laserArmTargetY);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), laserArmPosition + laserVelocity.SafeNormalize(Vector2.UnitY) * 100f, laserVelocity, type, damage, 0f, Main.myPlayer, 1f, 0f);
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
                    npc.target = Main.npc[(int)npc.ai[1]].target;
                    npc.localAI[0] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }

                Vector2 laserRingArmPosition = npc.Center;
                float laserRingTargetX = Main.player[npc.target].Center.X - laserRingArmPosition.X;
                float laserRingTargetY = Main.player[npc.target].Center.Y - laserRingArmPosition.Y;
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
                        int totalProjectiles = bossRush ? 32 : (masterMode ? 24 : 16);
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        int type = ProjectileID.DeathLaser;
                        int damage = npc.GetProjectileDamage(type);

                        // Reduce mech boss projectile damage depending on the new ore progression changes
                        if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                        {
                            double firstMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert;
                            double secondMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert;
                            if (!NPC.downedMechBossAny)
                                damage = (int)(damage * firstMechMultiplier);
                            else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                damage = (int)(damage * secondMechMultiplier);
                        }

                        float velocity = 3f;
                        double angleA = radians * 0.5;
                        double angleB = MathHelper.ToRadians(90f) - angleA;
                        float laserVelocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                        Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-laserVelocityX, -velocity);
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 laserFireDirection = spinningPoint.RotatedBy(radians * k);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + laserFireDirection.SafeNormalize(Vector2.UnitY) * 100f, laserFireDirection, type, damage, 0f, Main.myPlayer, 1f, 0f);
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
            bool masterMode = Main.masterMode || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            npc.spriteDirection = -(int)npc.ai[0];

            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
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
            float timeToNotAttack = 180f;
            bool dontAttack = npc.Calamity().newAI[2] < timeToNotAttack;
            if (dontAttack)
            {
                npc.Calamity().newAI[2] += 1f;
                if (npc.Calamity().newAI[2] >= timeToNotAttack)
                    npc.SyncExtraAI();
            }

            // Avoid cheap bullshit
            npc.damage = 0;

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

                    if (npc.ai[3] >= (masterMode ? 200f : 800f))
                    {
                        npc.target = Main.npc[(int)npc.ai[1]].target;
                        npc.localAI[0] = 0f;
                        npc.ai[2] = 1f;
                        fireSlower = false;
                        npc.ai[3] = 0f;
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
                        npc.target = Main.npc[(int)npc.ai[1]].target;
                        npc.localAI[0] = 0f;
                        npc.ai[2] = 0f;
                        fireSlower = true;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }
                }
            }

            // Movement
            float acceleration = (bossRush ? 0.6f : death ? (masterMode ? 0.375f : 0.3f) : (masterMode ? 0.3125f : 0.25f));
            float accelerationMult = 1f;
            if (!laserAlive)
            {
                acceleration += 0.025f;
                accelerationMult += 0.5f;
            }
            if (!viceAlive)
                acceleration += 0.025f;
            if (!sawAlive)
                acceleration += 0.025f;
            if (masterMode)
                acceleration *= accelerationMult;

            float topVelocity = acceleration * 100f;
            float deceleration = masterMode ? 0.6f : 0.8f;

            if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 130f)
            {
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= deceleration;

                npc.velocity.Y -= acceleration;

                if (npc.velocity.Y > topVelocity)
                    npc.velocity.Y = topVelocity;
            }
            else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 170f)
            {
                if (npc.velocity.Y < 0f)
                    npc.velocity.Y *= deceleration;

                npc.velocity.Y += acceleration;

                if (npc.velocity.Y < -topVelocity)
                    npc.velocity.Y = -topVelocity;
            }

            if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X + 160f)
            {
                if (npc.velocity.X > 0f)
                    npc.velocity.X *= deceleration;

                npc.velocity.X -= acceleration;

                if (npc.velocity.X > topVelocity)
                    npc.velocity.X = topVelocity;
            }
            if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X + 200f)
            {
                if (npc.velocity.X < 0f)
                    npc.velocity.X *= deceleration;

                npc.velocity.X += acceleration;

                if (npc.velocity.X < -topVelocity)
                    npc.velocity.X = -topVelocity;
            }

            if (fireSlower)
            {
                if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
                    npc.timeLeft = 10;

                Vector2 cannonArmPosition = npc.Center;
                float cannonArmTargetX = Main.player[npc.target].Center.X - cannonArmPosition.X;
                float cannonArmTargetY = Main.player[npc.target].Center.Y - cannonArmPosition.Y;
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
                        int type = ProjectileID.RocketSkeleton;
                        int damage = npc.GetProjectileDamage(type);

                        // Reduce mech boss projectile damage depending on the new ore progression changes
                        if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                        {
                            double firstMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert;
                            double secondMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert;
                            if (!NPC.downedMechBossAny)
                                damage = (int)(damage * firstMechMultiplier);
                            else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                damage = (int)(damage * secondMechMultiplier);
                        }

                        float rocketSpeed = 10f;
                        cannonArmTargetDist = rocketSpeed / cannonArmTargetDist;
                        cannonArmTargetX *= cannonArmTargetDist;
                        cannonArmTargetY *= cannonArmTargetDist;

                        Vector2 rocketVelocity = new Vector2(cannonArmTargetX, cannonArmTargetY);
                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), cannonArmPosition + rocketVelocity.SafeNormalize(Vector2.UnitY) * 40f, rocketVelocity, type, damage, 0f, Main.myPlayer, npc.target, 2f);
                        Main.projectile[proj].timeLeft = 600;
                    }
                }
            }
            else
            {
                Vector2 cannonSpreadArmPosition = npc.Center;
                float cannonSpreadArmTargetX = Main.player[npc.target].Center.X - cannonSpreadArmPosition.X;
                float cannonSpreadArmTargetY = Main.player[npc.target].Center.Y - cannonSpreadArmPosition.Y;
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
                        int type = ProjectileID.RocketSkeleton;
                        int damage = npc.GetProjectileDamage(type);

                        // Reduce mech boss projectile damage depending on the new ore progression changes
                        if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                        {
                            double firstMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert;
                            double secondMechMultiplier = CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert;
                            if (!NPC.downedMechBossAny)
                                damage = (int)(damage * firstMechMultiplier);
                            else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                damage = (int)(damage * secondMechMultiplier);
                        }

                        float rocketSpeed = 10f;
                        Vector2 cannonSpreadTargetDist = (Main.player[npc.target].Center - npc.Center).SafeNormalize(Vector2.UnitY) * rocketSpeed;
                        int numProj = bossRush ? 5 : 3;
                        float rotation = MathHelper.ToRadians(bossRush ? 15 : 9);
                        for (int i = 0; i < numProj; i++)
                        {
                            Vector2 perturbedSpeed = cannonSpreadTargetDist.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + perturbedSpeed.SafeNormalize(Vector2.UnitY) * 40f, perturbedSpeed, type, damage, 0f, Main.myPlayer, npc.target, 2f);
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
            bool masterMode = Main.masterMode || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Direction
            npc.spriteDirection = -(int)npc.ai[0];

            // Where the vice should be in relation to the head
            Vector2 viceArmPosition = npc.Center;
            float viceArmIdleXPos = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - viceArmPosition.X;
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
            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
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

            // Avoid cheap bullshit
            npc.damage = 0;

            // Return to the head
            if (npc.ai[2] == 99f)
            {
                float acceleration = (bossRush ? 0.6f : death ? (masterMode ? 0.375f : 0.3f) : (masterMode ? 0.3125f : 0.25f));
                float accelerationMult = 1f;
                if (!cannonAlive)
                {
                    acceleration += 0.025f;
                    accelerationMult += 0.5f;
                }
                if (!laserAlive)
                {
                    acceleration += 0.025f;
                    accelerationMult += 0.5f;
                }
                if (!sawAlive)
                    acceleration += 0.025f;
                if (masterMode)
                    acceleration *= accelerationMult;

                float topVelocity = acceleration * 100f;
                float deceleration = masterMode ? 0.6f : 0.8f;

                if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 20f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y -= acceleration;

                    if (npc.velocity.Y > topVelocity)
                        npc.velocity.Y = topVelocity;
                }
                else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 20f)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y += acceleration;

                    if (npc.velocity.Y < -topVelocity)
                        npc.velocity.Y = -topVelocity;
                }

                if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X + 20f)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X -= acceleration * 2f;

                    if (npc.velocity.X > topVelocity)
                        npc.velocity.X = topVelocity;
                }
                if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X - 20f)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X += acceleration * 2f;

                    if (npc.velocity.X < -topVelocity)
                        npc.velocity.X = -topVelocity;
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

                    if (npc.ai[3] >= (masterMode ? 150f : 600f))
                    {
                        npc.target = Main.npc[(int)npc.ai[1]].target;
                        npc.ai[2] += 1f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }

                    float acceleration = (bossRush ? 0.6f : death ? (masterMode ? 0.375f : 0.3f) : (masterMode ? 0.3125f : 0.25f));
                    float accelerationMult = 1f;
                    if (!cannonAlive)
                    {
                        acceleration += 0.025f;
                        accelerationMult += 0.5f;
                    }
                    if (!laserAlive)
                    {
                        acceleration += 0.025f;
                        accelerationMult += 0.5f;
                    }
                    if (!sawAlive)
                        acceleration += 0.025f;
                    if (masterMode)
                        acceleration *= accelerationMult;

                    float topVelocity = acceleration * 100f;
                    float deceleration = masterMode ? 0.6f : 0.8f;

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 290f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y -= acceleration;

                        if (npc.velocity.Y > topVelocity)
                            npc.velocity.Y = topVelocity;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 240f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y += acceleration;

                        if (npc.velocity.Y < -topVelocity)
                            npc.velocity.Y = -topVelocity;
                    }

                    if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X + 150f)
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X -= acceleration;

                        if (npc.velocity.X > topVelocity)
                            npc.velocity.X = topVelocity;
                    }
                    if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X + 100f)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X += acceleration;

                        if (npc.velocity.X < -topVelocity)
                            npc.velocity.X = -topVelocity;
                    }

                    Vector2 viceArmReelbackCurrentPos = npc.Center;
                    float viceArmReelbackXDest = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - viceArmReelbackCurrentPos.X;
                    float viceArmReelbackYDest = Main.npc[(int)npc.ai[1]].position.Y + 230f - viceArmReelbackCurrentPos.Y;
                    npc.rotation = (float)Math.Atan2(viceArmReelbackYDest, viceArmReelbackXDest) + MathHelper.PiOver2;
                    return false;
                }

                // Charge towards the player
                if (npc.ai[2] == 1f)
                {
                    float deceleration = masterMode ? 0.75f : 0.8f;
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= deceleration;

                    Vector2 viceArmChargePosition = npc.Center;
                    float viceArmChargeTargetX = Main.npc[(int)npc.ai[1]].Center.X - 280f * npc.ai[0] - viceArmChargePosition.X;
                    float viceArmChargeTargetY = Main.npc[(int)npc.ai[1]].position.Y + 230f - viceArmChargePosition.Y;
                    npc.rotation = (float)Math.Atan2(viceArmChargeTargetY, viceArmChargeTargetX) + MathHelper.PiOver2;

                    npc.velocity.X = (npc.velocity.X * 5f + Main.npc[(int)npc.ai[1]].velocity.X) / 6f;
                    npc.velocity.X += 0.5f;

                    npc.velocity.Y -= 0.5f;
                    if (npc.velocity.Y < -12f)
                        npc.velocity.Y = -12f;

                    if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 280f)
                    {
                        // Set damage
                        npc.damage = npc.defDamage;

                        float chargeVelocity = bossRush ? 20f : 16f;
                        if (!cannonAlive)
                            chargeVelocity += 1.5f;
                        if (!laserAlive)
                            chargeVelocity += 1.5f;
                        if (!sawAlive)
                            chargeVelocity += 1.5f;

                        npc.ai[2] = 2f;
                        viceArmChargePosition = npc.Center;
                        viceArmChargeTargetX = Main.player[npc.target].Center.X - viceArmChargePosition.X;
                        viceArmChargeTargetY = Main.player[npc.target].Center.Y - viceArmChargePosition.Y;
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
                    // Set damage
                    npc.damage = npc.defDamage;

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
                            return false;
                        }

                        npc.ai[2] = 1f;
                        npc.ai[3] += 1f;
                    }
                }

                // Different type of charge
                else if (npc.ai[2] == 4f)
                {
                    Vector2 viceArmOtherChargePosition = npc.Center;
                    float viceArmOtherChargeTargetX = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - viceArmOtherChargePosition.X;
                    float viceArmOtherChargeTargetY = Main.npc[(int)npc.ai[1]].position.Y + 230f - viceArmOtherChargePosition.Y;
                    npc.rotation = (float)Math.Atan2(viceArmOtherChargeTargetY, viceArmOtherChargeTargetX) + MathHelper.PiOver2;

                    npc.velocity.Y = (npc.velocity.Y * 5f + Main.npc[(int)npc.ai[1]].velocity.Y) / 6f;

                    npc.velocity.X += 0.5f;
                    if (npc.velocity.X > 12f)
                        npc.velocity.X = 12f;

                    if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X - 500f || npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X + 500f)
                    {
                        // Set damage
                        npc.damage = npc.defDamage;

                        float chargeVelocity = bossRush ? 17.5f : 14f;
                        if (!cannonAlive)
                            chargeVelocity += 1.15f;
                        if (!laserAlive)
                            chargeVelocity += 1.15f;
                        if (!sawAlive)
                            chargeVelocity += 1.15f;

                        npc.ai[2] = 5f;
                        viceArmOtherChargePosition = npc.Center;
                        viceArmOtherChargeTargetX = Main.player[npc.target].Center.X - viceArmOtherChargePosition.X;
                        viceArmOtherChargeTargetY = Main.player[npc.target].Center.Y - viceArmOtherChargePosition.Y;
                        float viceArmOtherChargeTargetDist = (float)Math.Sqrt(viceArmOtherChargeTargetX * viceArmOtherChargeTargetX + viceArmOtherChargeTargetY * viceArmOtherChargeTargetY);
                        viceArmOtherChargeTargetDist = chargeVelocity / viceArmOtherChargeTargetDist;
                        npc.velocity.X = viceArmOtherChargeTargetX * viceArmOtherChargeTargetDist;
                        npc.velocity.Y = viceArmOtherChargeTargetY * viceArmOtherChargeTargetDist;
                        npc.netUpdate = true;
                    }
                }

                // Charge 4 times (more if arms are dead)
                else if (npc.ai[2] == 5f && npc.Center.X < Main.player[npc.target].Center.X - 100f)
                {
                    // Set damage
                    npc.damage = npc.defDamage;

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
            bool masterMode = Main.masterMode || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            Vector2 sawArmLocation = npc.Center;
            float sawArmIdleXPos = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - sawArmLocation.X;
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

            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
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

            // Min saw damage
            int reducedSetDamage = (int)Math.Round(npc.defDamage * 0.5);

            // Avoid cheap bullshit
            npc.damage = reducedSetDamage;

            if (npc.ai[2] == 99f)
            {
                float acceleration = (bossRush ? 0.6f : death ? (masterMode ? 0.375f : 0.3f) : (masterMode ? 0.3125f : 0.25f));
                float accelerationMult = 1f;
                if (!cannonAlive)
                {
                    acceleration += 0.025f;
                    accelerationMult += 0.5f;
                }
                if (!laserAlive)
                {
                    acceleration += 0.025f;
                    accelerationMult += 0.5f;
                }
                if (!viceAlive)
                    acceleration += 0.025f;
                if (masterMode)
                    acceleration *= accelerationMult;

                float topVelocity = acceleration * 100f;
                float deceleration = masterMode ? 0.6f : 0.8f;

                if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 20f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y -= acceleration;

                    if (npc.velocity.Y > topVelocity)
                        npc.velocity.Y = topVelocity;
                }
                else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 20f)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y += acceleration;

                    if (npc.velocity.Y < -topVelocity)
                        npc.velocity.Y = -topVelocity;
                }

                if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X + 20f)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X -= acceleration * 2f;

                    if (npc.velocity.X > topVelocity)
                        npc.velocity.X = topVelocity;
                }
                if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X - 20f)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X += acceleration * 2f;

                    if (npc.velocity.X < -topVelocity)
                        npc.velocity.X = -topVelocity;
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

                    if (npc.ai[3] >= (masterMode ? 90f : 180f))
                    {
                        npc.target = Main.npc[(int)npc.ai[1]].target;
                        npc.ai[2] += 1f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }

                    float acceleration = (bossRush ? 0.6f : death ? (masterMode ? 0.375f : 0.3f) : (masterMode ? 0.3125f : 0.25f));
                    float accelerationMult = 1f;
                    if (!cannonAlive)
                    {
                        acceleration += 0.025f;
                        accelerationMult += 0.5f;
                    }
                    if (!laserAlive)
                    {
                        acceleration += 0.025f;
                        accelerationMult += 0.5f;
                    }
                    if (!viceAlive)
                        acceleration += 0.025f;
                    if (masterMode)
                        acceleration *= accelerationMult;

                    float topVelocity = acceleration * 100f;
                    float deceleration = masterMode ? 0.6f : 0.8f;

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 310f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y -= acceleration;

                        if (npc.velocity.Y > topVelocity)
                            npc.velocity.Y = topVelocity;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 270f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y += acceleration;

                        if (npc.velocity.Y < -topVelocity)
                            npc.velocity.Y = -topVelocity;
                    }

                    if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X - 100f)
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X -= acceleration * 1.5f;

                        if (npc.velocity.X > topVelocity)
                            npc.velocity.X = topVelocity;
                    }
                    if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X - 150f)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X += acceleration * 1.5f;

                        if (npc.velocity.X < -topVelocity)
                            npc.velocity.X = -topVelocity;
                    }

                    Vector2 sawArmReelbackCurrentPos = npc.Center;
                    float sawArmReelbackXDest = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - sawArmReelbackCurrentPos.X;
                    float sawArmReelbackYDest = Main.npc[(int)npc.ai[1]].position.Y + 230f - sawArmReelbackCurrentPos.Y;
                    npc.rotation = (float)Math.Atan2(sawArmReelbackYDest, sawArmReelbackXDest) + MathHelper.PiOver2;
                    return false;
                }

                if (npc.ai[2] == 1f)
                {
                    Vector2 sawArmChargePos = npc.Center;
                    float sawArmChargeTargetX = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - sawArmChargePos.X;
                    float sawArmChargeTargetY = Main.npc[(int)npc.ai[1]].position.Y + 230f - sawArmChargePos.Y;
                    npc.rotation = (float)Math.Atan2(sawArmChargeTargetY, sawArmChargeTargetX) + MathHelper.PiOver2;

                    float deceleration = masterMode ? 0.875f : 0.9f;
                    npc.velocity.X *= deceleration;
                    npc.velocity.Y -= 0.5f;
                    if (npc.velocity.Y < -12f)
                        npc.velocity.Y = -12f;

                    if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 200f)
                    {
                        // Set damage
                        npc.damage = npc.defDamage;

                        float chargeVelocity = bossRush ? 27.5f : 22f;
                        if (!cannonAlive)
                            chargeVelocity += 1.5f;
                        if (!laserAlive)
                            chargeVelocity += 1.5f;
                        if (!viceAlive)
                            chargeVelocity += 1.5f;

                        npc.ai[2] = 2f;
                        sawArmChargePos = npc.Center;
                        sawArmChargeTargetX = Main.player[npc.target].Center.X - sawArmChargePos.X;
                        sawArmChargeTargetY = Main.player[npc.target].Center.Y - sawArmChargePos.Y;
                        float sawArmChargeTargetDist = (float)Math.Sqrt(sawArmChargeTargetX * sawArmChargeTargetX + sawArmChargeTargetY * sawArmChargeTargetY);
                        sawArmChargeTargetDist = chargeVelocity / sawArmChargeTargetDist;
                        npc.velocity.X = sawArmChargeTargetX * sawArmChargeTargetDist;
                        npc.velocity.Y = sawArmChargeTargetY * sawArmChargeTargetDist;
                        npc.netUpdate = true;
                    }
                }

                else if (npc.ai[2] == 2f)
                {
                    // Set damage
                    npc.damage = npc.defDamage;

                    if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f)
                        npc.ai[2] = 3f;
                }

                else
                {
                    if (npc.ai[2] == 4f)
                    {
                        // Set damage
                        npc.damage = npc.defDamage;

                        float chargeVelocity = bossRush ? 13.5f : 11f;
                        if (!cannonAlive)
                            chargeVelocity += 1.5f;
                        if (!laserAlive)
                            chargeVelocity += 1.5f;
                        if (!viceAlive)
                            chargeVelocity += 1.5f;
                        if (masterMode)
                            chargeVelocity *= 1.25f;

                        Vector2 sawArmOtherChargePos = npc.Center;
                        float sawArmOtherChargeTargetX = Main.player[npc.target].Center.X - sawArmOtherChargePos.X;
                        float sawArmOtherChargeTargetY = Main.player[npc.target].Center.Y - sawArmOtherChargePos.Y;
                        float sawArmOtherChargeTargetDist = (float)Math.Sqrt(sawArmOtherChargeTargetX * sawArmOtherChargeTargetX + sawArmOtherChargeTargetY * sawArmOtherChargeTargetY);
                        sawArmOtherChargeTargetDist = chargeVelocity / sawArmOtherChargeTargetDist;
                        sawArmOtherChargeTargetX *= sawArmOtherChargeTargetDist;
                        sawArmOtherChargeTargetY *= sawArmOtherChargeTargetDist;

                        float acceleration = bossRush ? 0.3f : death ? 0.1f : 0.08f;
                        if (masterMode)
                            acceleration *= 1.25f;

                        float deceleration = masterMode ? 0.6f : 0.8f;

                        if (npc.velocity.X > sawArmOtherChargeTargetX)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X *= deceleration;

                            npc.velocity.X -= acceleration;
                        }
                        if (npc.velocity.X < sawArmOtherChargeTargetX)
                        {
                            if (npc.velocity.X < 0f)
                                npc.velocity.X *= deceleration;

                            npc.velocity.X += acceleration;
                        }
                        if (npc.velocity.Y > sawArmOtherChargeTargetY)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y *= deceleration;

                            npc.velocity.Y -= acceleration;
                        }
                        if (npc.velocity.Y < sawArmOtherChargeTargetY)
                        {
                            if (npc.velocity.Y < 0f)
                                npc.velocity.Y *= deceleration;

                            npc.velocity.Y += acceleration;
                        }

                        npc.ai[3] += 1f;
                        if (npc.justHit)
                            npc.ai[3] += 2f;

                        if (npc.ai[3] >= 600f)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[3] = 0f;
                            npc.netUpdate = true;
                        }

                        sawArmOtherChargePos = npc.Center;
                        sawArmOtherChargeTargetX = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - sawArmOtherChargePos.X;
                        sawArmOtherChargeTargetY = Main.npc[(int)npc.ai[1]].position.Y + 230f - sawArmOtherChargePos.Y;
                        npc.rotation = (float)Math.Atan2(sawArmOtherChargeTargetY, sawArmOtherChargeTargetX) + MathHelper.PiOver2;
                        return false;
                    }

                    if (npc.ai[2] == 5f && ((npc.velocity.X > 0f && npc.Center.X > Main.player[npc.target].Center.X) || (npc.velocity.X < 0f && npc.Center.X < Main.player[npc.target].Center.X)))
                        npc.ai[2] = 0f;
                }
            }

            return false;
        }

        public static bool VanillaSkeletronPrimeAI(NPC npc, Mod mod)
        {
            npc.defense = npc.defDefense;
            if (npc.ai[3] != 0f)
                NPC.mechQueen = npc.whoAmI;

            npc.reflectsProjectiles = false;
            if (npc.ai[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.TargetClosest();
                npc.ai[0] = 1f;
                int num495 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeCannon, npc.whoAmI);
                Main.npc[num495].ai[0] = -1f;
                Main.npc[num495].ai[1] = npc.whoAmI;
                Main.npc[num495].target = npc.target;
                Main.npc[num495].netUpdate = true;
                num495 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeSaw, npc.whoAmI);
                Main.npc[num495].ai[0] = 1f;
                Main.npc[num495].ai[1] = npc.whoAmI;
                Main.npc[num495].target = npc.target;
                Main.npc[num495].netUpdate = true;
                num495 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeVice, npc.whoAmI);
                Main.npc[num495].ai[0] = -1f;
                Main.npc[num495].ai[1] = npc.whoAmI;
                Main.npc[num495].target = npc.target;
                Main.npc[num495].ai[3] = 150f;
                Main.npc[num495].netUpdate = true;
                num495 = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.PrimeLaser, npc.whoAmI);
                Main.npc[num495].ai[0] = 1f;
                Main.npc[num495].ai[1] = npc.whoAmI;
                Main.npc[num495].target = npc.target;
                Main.npc[num495].netUpdate = true;
                Main.npc[num495].ai[3] = 150f;
            }

            if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 6000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 6000f)
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 6000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 6000f)
                    npc.ai[1] = 3f;
            }

            if (Main.IsItDay() && npc.ai[1] != 3f && npc.ai[1] != 2f)
            {
                npc.ai[1] = 2f;
                SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center);
            }

            if (npc.ai[1] == 0f)
            {
                npc.damage = 0;

                npc.ai[2] += 1f;
                if (npc.ai[2] >= (Main.masterMode ? 300f : 600f))
                {
                    npc.ai[2] = 0f;
                    npc.ai[1] = 1f;
                    npc.netUpdate = true;
                }

                if (NPC.IsMechQueenUp)
                    npc.rotation = npc.rotation.AngleLerp(npc.velocity.X / 15f * 0.5f, 0.75f);
                else
                    npc.rotation = npc.velocity.X / 15f;

                float num496 = 0.1f;
                float num497 = 2f;
                float num498 = 0.1f;
                float num499 = 8f;
                float deceleration = Main.masterMode ? 0.94f : Main.expertMode ? 0.96f : 0.98f;
                int num500 = 200;
                int num501 = 500;
                float num502 = 0f;
                int num503 = ((!(Main.player[npc.target].Center.X < npc.Center.X)) ? 1 : (-1));
                if (NPC.IsMechQueenUp)
                {
                    num502 = -450f * (float)num503;
                    num500 = 300;
                    num501 = 350;
                }

                if (Main.expertMode)
                {
                    num496 = Main.masterMode ? 0.05f : 0.03f;
                    num497 = Main.masterMode ? 5f : 4f;
                    num498 = Main.masterMode ? 0.15f : 0.12f;
                    num499 = Main.masterMode ? 11f : 9.5f;
                }

                if (npc.position.Y > Main.player[npc.target].position.Y - (float)num500)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y -= num496;
                    if (npc.velocity.Y > num497)
                        npc.velocity.Y = num497;
                }
                else if (npc.position.Y < Main.player[npc.target].position.Y - (float)num501)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y += num496;
                    if (npc.velocity.Y < -num497)
                        npc.velocity.Y = -num497;
                }

                if (npc.Center.X > Main.player[npc.target].Center.X + 100f + num502)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X -= num498;
                    if (npc.velocity.X > num499)
                        npc.velocity.X = num499;
                }

                if (npc.Center.X < Main.player[npc.target].Center.X - 100f + num502)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X += num498;
                    if (npc.velocity.X < 0f - num499)
                        npc.velocity.X = 0f - num499;
                }
            }
            else if (npc.ai[1] == 1f)
            {
                npc.defense *= 2;
                npc.damage = npc.defDamage * 2;

                npc.Calamity().CurrentlyIncreasingDefenseOrDR = true;

                npc.ai[2] += 1f;
                if (npc.ai[2] == 2f)
                    SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center);

                if (npc.ai[2] >= (Main.masterMode ? 300f : 400f))
                {
                    npc.ai[2] = 0f;
                    npc.ai[1] = 0f;
                    npc.TargetClosest();
                }

                if (NPC.IsMechQueenUp)
                    npc.rotation = npc.rotation.AngleLerp(npc.velocity.X / 15f * 0.5f, 0.75f);
                else
                    npc.rotation += (float)npc.direction * 0.3f;

                Vector2 vector54 = npc.Center;
                float num504 = Main.player[npc.target].Center.X - vector54.X;
                float num505 = Main.player[npc.target].Center.Y - vector54.Y;
                float num506 = (float)Math.Sqrt(num504 * num504 + num505 * num505);
                float num507 = 5f;
                if (Main.expertMode)
                {
                    num507 = Main.masterMode ? 7f : 6f;
                    if (num506 > 150f)
                        num507 *= (Main.masterMode ? 1.075f : 1.05f);

                    float additionalMultiplier = Main.masterMode ? 1.15f : 1.1f;
                    if (num506 > 200f)
                        num507 *= additionalMultiplier;

                    if (num506 > 250f)
                        num507 *= additionalMultiplier;

                    if (num506 > 300f)
                        num507 *= additionalMultiplier;

                    if (num506 > 350f)
                        num507 *= additionalMultiplier;

                    if (num506 > 400f)
                        num507 *= additionalMultiplier;

                    if (num506 > 450f)
                        num507 *= additionalMultiplier;

                    if (num506 > 500f)
                        num507 *= additionalMultiplier;

                    if (num506 > 550f)
                        num507 *= additionalMultiplier;

                    if (num506 > 600f)
                        num507 *= additionalMultiplier;
                }

                if (NPC.IsMechQueenUp)
                {
                    float num508 = (NPC.npcsFoundForCheckActive[NPCID.TheDestroyerBody] ? 0.6f : 0.75f);
                    num507 *= num508;
                }

                num506 = num507 / num506;
                npc.velocity.X = num504 * num506;
                npc.velocity.Y = num505 * num506;
                if (NPC.IsMechQueenUp)
                {
                    float num509 = Vector2.Distance(npc.Center, Main.player[npc.target].Center);
                    if (num509 < 0.1f)
                        num509 = 0f;

                    if (num509 < num507)
                        npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * num509;
                }
            }
            else if (npc.ai[1] == 2f)
            {
                npc.damage = 1000;
                npc.defense = 9999;

                npc.Calamity().CurrentlyEnraged = true;
                npc.Calamity().CurrentlyIncreasingDefenseOrDR = true;

                if (NPC.IsMechQueenUp)
                    npc.rotation = npc.rotation.AngleLerp(npc.velocity.X / 15f * 0.5f, 0.75f);
                else
                    npc.rotation += (float)npc.direction * 0.3f;

                Vector2 vector55 = npc.Center;
                float num510 = Main.player[npc.target].Center.X - vector55.X;
                float num511 = Main.player[npc.target].Center.Y - vector55.Y;
                float num512 = (float)Math.Sqrt(num510 * num510 + num511 * num511);
                float num513 = 10f;
                num513 += num512 / 100f;
                if (num513 < 8f)
                    num513 = 8f;

                if (num513 > 32f)
                    num513 = 32f;

                num512 = num513 / num512;
                npc.velocity.X = num510 * num512;
                npc.velocity.Y = num511 * num512;
            }
            else
            {
                if (npc.ai[1] != 3f)
                    return false;

                if (NPC.IsMechQueenUp)
                {
                    int num514 = NPC.FindFirstNPC(NPCID.Retinazer);
                    if (num514 >= 0)
                        Main.npc[num514].EncourageDespawn(5);

                    num514 = NPC.FindFirstNPC(NPCID.Spazmatism);
                    if (num514 >= 0)
                        Main.npc[num514].EncourageDespawn(5);

                    if (!NPC.AnyNPCs(NPCID.Retinazer) && !NPC.AnyNPCs(NPCID.Spazmatism))
                    {
                        num514 = NPC.FindFirstNPC(NPCID.TheDestroyer);
                        if (num514 >= 0)
                            Main.npc[num514].Transform(NPCID.TheDestroyerTail);

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
                    npc.EncourageDespawn(500);
                    npc.velocity.Y += 0.1f;
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.95f;

                    npc.velocity.X *= 0.95f;
                }
            }

            return false;
        }

        public static bool VanillaPrimeLaserAI(NPC npc, Mod mod)
        {
            npc.spriteDirection = -(int)npc.ai[0];
            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
            {
                npc.ai[2] += 10f;
                if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
                {
                    npc.life = -1;
                    npc.HitEffect();
                    npc.active = false;
                }
            }

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            npc.damage = 0;

            if (npc.ai[2] == 0f || npc.ai[2] == 3f)
            {
                if (Main.npc[(int)npc.ai[1]].ai[1] == 3f)
                    npc.EncourageDespawn(10);

                if (Main.npc[(int)npc.ai[1]].ai[1] != 0f)
                {
                    float num496 = 0.07f;
                    float num497 = 6f;
                    float num498 = 0.1f;
                    float num499 = 8f;
                    float deceleration = Main.masterMode ? 0.92f : Main.expertMode ? 0.94f : 0.96f;

                    if (Main.expertMode)
                    {
                        num496 = Main.masterMode ? 0.11f : 0.09f;
                        num497 = Main.masterMode ? 8f : 7f;
                        num498 = Main.masterMode ? 0.15f : 0.12f;
                        num499 = Main.masterMode ? 11f : 9.5f;
                    }

                    npc.localAI[0] += 3f;
                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 100f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y -= num496;
                        if (npc.velocity.Y > num497)
                            npc.velocity.Y = num497;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 100f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y += num496;
                        if (npc.velocity.Y < -num497)
                            npc.velocity.Y = -num497;
                    }

                    if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X - 120f * npc.ai[0])
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X -= num498;
                        if (npc.velocity.X > num499)
                            npc.velocity.X = num499;
                    }

                    if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X - 120f * npc.ai[0])
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X += num498;
                        if (npc.velocity.X < -num499)
                            npc.velocity.X = -num499;
                    }
                }
                else
                {
                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= (Main.masterMode ? 400f : Main.expertMode ? 600f : 800f))
                    {
                        npc.target = Main.npc[(int)npc.ai[1]].target;
                        npc.ai[2] += 1f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }

                    float num496 = 0.1f;
                    float num497 = 3f;
                    float num498 = 0.14f;
                    float num499 = 8f;
                    float deceleration = Main.masterMode ? 0.92f : Main.expertMode ? 0.94f : 0.96f;

                    if (Main.expertMode)
                    {
                        num496 = Main.masterMode ? 0.14f : 0.12f;
                        num497 = Main.masterMode ? 5f : 4f;
                        num498 = Main.masterMode ? 0.2f : 0.17f;
                        num499 = Main.masterMode ? 11f : 9.5f;
                    }

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 100f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y -= num496;
                        if (npc.velocity.Y > num497)
                            npc.velocity.Y = num497;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 100f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y += num496;
                        if (npc.velocity.Y < -num497)
                            npc.velocity.Y = -num497;
                    }

                    if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X - 180f * npc.ai[0])
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X -= num498;
                        if (npc.velocity.X > num499)
                            npc.velocity.X = num499;
                    }

                    if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X - 180f * npc.ai[0])
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X += num498;
                        if (npc.velocity.X < -num499)
                            npc.velocity.X = -num499;
                    }
                }

                Vector2 vector68 = npc.Center;
                float num559 = Main.player[npc.target].Center.X - vector68.X;
                float num560 = Main.player[npc.target].Center.Y - vector68.Y;
                float num561 = (float)Math.Sqrt(num559 * num559 + num560 * num560);
                npc.rotation = (float)Math.Atan2(num560, num559) - MathHelper.PiOver2;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] > (Main.masterMode ? 80f : Main.expertMode ? 140f : 200f))
                    {
                        npc.localAI[0] = 0f;
                        float num562 = 8f;

                        int type = ProjectileID.DeathLaser;
                        int damage = npc.GetProjectileDamage(type);

                        // Reduce mech boss projectile damage depending on the new ore progression changes
                        if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                        {
                            double firstMechMultiplier = Main.expertMode ? CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert : CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Classic;
                            double secondMechMultiplier = Main.expertMode ? CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert : CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Classic;
                            if (!NPC.downedMechBossAny)
                                damage = (int)(damage * firstMechMultiplier);
                            else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                damage = (int)(damage * secondMechMultiplier);
                        }

                        num561 = num562 / num561;
                        num559 *= num561;
                        num560 *= num561;
                        Vector2 laserVelocity = new Vector2(num559, num560);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector68 + laserVelocity.SafeNormalize(Vector2.UnitY) * 100f, laserVelocity, type, damage, 0f, Main.myPlayer);
                    }
                }
            }
            else
            {
                if (npc.ai[2] != 1f)
                    return false;

                npc.ai[3] += 1f;
                if (npc.ai[3] >= (Main.masterMode ? 150f : Main.expertMode ? 175f : 200f))
                {
                    npc.target = Main.npc[(int)npc.ai[1]].target;
                    npc.localAI[0] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }

                Vector2 vector69 = npc.Center;
                float num566 = Main.player[npc.target].Center.X - 350f - vector69.X;
                float num567 = Main.player[npc.target].Center.Y - 20f - vector69.Y;
                float num568 = (float)Math.Sqrt(num566 * num566 + num567 * num567);
                num568 = (Main.masterMode ? 9f : Main.expertMode ? 8f : 7f) / num568;
                num566 *= num568;
                num567 *= num568;

                float num496 = 0.1f;
                float num498 = 0.03f;
                float deceleration = Main.masterMode ? 0.8f : Main.expertMode ? 0.85f : 0.9f;

                if (Main.expertMode)
                {
                    num496 = Main.masterMode ? 0.14f : 0.12f;
                    num498 = Main.masterMode ? 0.05f : 0.04f;
                }

                if (npc.velocity.X > num566)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X -= num496;
                }

                if (npc.velocity.X < num566)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X += num496;
                }

                if (npc.velocity.Y > num567)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y -= num498;
                }

                if (npc.velocity.Y < num567)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y += num498;
                }

                vector69 = npc.Center;
                num566 = Main.player[npc.target].Center.X - vector69.X;
                num567 = Main.player[npc.target].Center.Y - vector69.Y;
                num568 = (float)Math.Sqrt(num566 * num566 + num567 * num567);
                npc.rotation = (float)Math.Atan2(num567, num566) - MathHelper.PiOver2;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] > (Main.masterMode ? 40f : Main.expertMode ? 60f : 80f))
                    {
                        npc.localAI[0] = 0f;
                        float num569 = 10f;

                        int type = ProjectileID.DeathLaser;
                        int damage = npc.GetProjectileDamage(type);

                        // Reduce mech boss projectile damage depending on the new ore progression changes
                        if (CalamityConfig.Instance.EarlyHardmodeProgressionRework && !BossRushEvent.BossRushActive)
                        {
                            double firstMechMultiplier = Main.expertMode ? CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Expert : CalamityGlobalNPC.EarlyHardmodeProgressionReworkFirstMechStatMultiplier_Classic;
                            double secondMechMultiplier = Main.expertMode ? CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Expert : CalamityGlobalNPC.EarlyHardmodeProgressionReworkSecondMechStatMultiplier_Classic;
                            if (!NPC.downedMechBossAny)
                                damage = (int)(damage * firstMechMultiplier);
                            else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
                                damage = (int)(damage * secondMechMultiplier);
                        }

                        num568 = num569 / num568;
                        num566 *= num568;
                        num567 *= num568;
                        Vector2 laserVelocity = new Vector2(num566, num567);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector69 + laserVelocity.SafeNormalize(Vector2.UnitY) * 100f, laserVelocity, type, damage, 0f, Main.myPlayer);
                    }
                }
            }

            return false;
        }

        public static bool VanillaPrimeCannonAI(NPC npc, Mod mod)
        {
            npc.spriteDirection = -(int)npc.ai[0];
            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
            {
                npc.ai[2] += 10f;
                if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
                {
                    npc.life = -1;
                    npc.HitEffect();
                    npc.active = false;
                }
            }

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            npc.damage = 0;

            if (npc.ai[2] == 0f)
            {
                if (Main.npc[(int)npc.ai[1]].ai[1] == 3f)
                    npc.EncourageDespawn(10);

                if (Main.npc[(int)npc.ai[1]].ai[1] != 0f)
                {
                    float num496 = 0.07f;
                    float num497 = 6f;
                    float num498 = 0.1f;
                    float num499 = 8f;
                    float deceleration = Main.masterMode ? 0.92f : Main.expertMode ? 0.94f : 0.96f;

                    if (Main.expertMode)
                    {
                        num496 = Main.masterMode ? 0.11f : 0.09f;
                        num497 = Main.masterMode ? 8f : 7f;
                        num498 = Main.masterMode ? 0.15f : 0.12f;
                        num499 = Main.masterMode ? 11f : 9.5f;
                    }

                    npc.localAI[0] += 2f;
                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 100f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y -= num496;
                        if (npc.velocity.Y > num497)
                            npc.velocity.Y = num497;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 100f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y += num496;
                        if (npc.velocity.Y < -num497)
                            npc.velocity.Y = -num497;
                    }

                    if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X - 120f * npc.ai[0])
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X -= num498;
                        if (npc.velocity.X > num499)
                            npc.velocity.X = num499;
                    }

                    if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X - 120f * npc.ai[0])
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X += num498;
                        if (npc.velocity.X < -num499)
                            npc.velocity.X = -num499;
                    }
                }
                else
                {
                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= (Main.masterMode ? 700f : Main.expertMode ? 900f : 1100f))
                    {
                        npc.target = Main.npc[(int)npc.ai[1]].target;
                        npc.localAI[0] = 0f;
                        npc.ai[2] = 1f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }

                    float num496 = 0.04f;
                    float num497 = 3f;
                    float num498 = 0.2f;
                    float num499 = 8f;
                    float deceleration = Main.masterMode ? 0.92f : Main.expertMode ? 0.94f : 0.96f;

                    if (Main.expertMode)
                    {
                        num496 = Main.masterMode ? 0.06f : 0.05f;
                        num497 = Main.masterMode ? 5f : 4f;
                        num498 = Main.masterMode ? 0.28f : 0.24f;
                        num499 = Main.masterMode ? 11f : 9.5f;
                    }

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 150f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y -= num496;
                        if (npc.velocity.Y > num497)
                            npc.velocity.Y = num497;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 150f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y += num496;
                        if (npc.velocity.Y < -num497)
                            npc.velocity.Y = -num497;
                    }

                    if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X + 200f)
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X -= num498;
                        if (npc.velocity.X > num499)
                            npc.velocity.X = num499;
                    }

                    if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X + 160f)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X += num498;
                        if (npc.velocity.X < -num499)
                            npc.velocity.X = -num499;
                    }
                }

                Vector2 vector66 = npc.Center;
                float num545 = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - vector66.X;
                float num546 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector66.Y;
                float num547 = (float)Math.Sqrt(num545 * num545 + num546 * num546);
                npc.rotation = (float)Math.Atan2(num546, num545) + MathHelper.PiOver2;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] > (Main.masterMode ? 60f : Main.expertMode ? 100f : 140f))
                    {
                        npc.localAI[0] = 0f;
                        float num548 = 12f;
                        int num549 = 0;
                        int num550 = ProjectileID.BombSkeletronPrime;
                        num547 = num548 / num547;
                        num545 = (0f - num545) * num547;
                        num546 = (0f - num546) * num547;
                        vector66.X += num545 * 4f;
                        vector66.Y += num546 * 4f;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector66.X, vector66.Y, num545, num546, num550, num549, 0f, Main.myPlayer);
                    }
                }
            }
            else
            {
                if (npc.ai[2] != 1f)
                    return false;

                npc.ai[3] += 1f;
                if (npc.ai[3] >= (Main.masterMode ? 180f : Main.expertMode ? 240f : 300f))
                {
                    npc.target = Main.npc[(int)npc.ai[1]].target;
                    npc.localAI[0] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }

                Vector2 vector67 = npc.Center;
                float num552 = Main.npc[(int)npc.ai[1]].Center.X - vector67.X;
                float num553 = Main.npc[(int)npc.ai[1]].position.Y - vector67.Y;
                num553 = Main.player[npc.target].Center.Y - 80f - vector67.Y;
                float num554 = (float)Math.Sqrt(num552 * num552 + num553 * num553);
                num554 = (Main.masterMode ? 8f : Main.expertMode ? 7f : 6f) / num554;
                num552 *= num554;
                num553 *= num554;

                float num496 = 0.04f;
                float num498 = 0.08f;
                float deceleration = Main.masterMode ? 0.8f : Main.expertMode ? 0.85f : 0.9f;

                if (Main.expertMode)
                {
                    num496 = Main.masterMode ? 0.08f : 0.06f;
                    num498 = Main.masterMode ? 0.12f : 0.1f;
                }

                if (npc.velocity.X > num552)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X -= num496;
                }

                if (npc.velocity.X < num552)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X += num496;
                }

                if (npc.velocity.Y > num553)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y -= num498;
                }

                if (npc.velocity.Y < num553)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y += num498;
                }

                vector67 = npc.Center;
                num552 = Main.player[npc.target].Center.X - vector67.X;
                num553 = Main.player[npc.target].Center.Y - vector67.Y;
                num554 = (float)Math.Sqrt(num552 * num552 + num553 * num553);
                npc.rotation = (float)Math.Atan2(num553, num552) - MathHelper.PiOver2;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] > (Main.masterMode ? 20f : Main.expertMode ? 30f : 40f))
                    {
                        npc.localAI[0] = 0f;
                        float num555 = 10f;
                        int num556 = 0;
                        int num557 = ProjectileID.BombSkeletronPrime;
                        num554 = num555 / num554;
                        num552 *= num554;
                        num553 *= num554;
                        vector67.X += num552 * 4f;
                        vector67.Y += num553 * 4f;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector67.X, vector67.Y, num552, num553, num557, num556, 0f, Main.myPlayer);
                    }
                }
            }

            return false;
        }

        public static bool VanillaPrimeViceAI(NPC npc, Mod mod)
        {
            npc.spriteDirection = -(int)npc.ai[0];
            Vector2 vector61 = npc.Center;
            float num530 = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - vector61.X;
            float num531 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector61.Y;
            float num532 = (float)Math.Sqrt(num530 * num530 + num531 * num531);
            if (npc.ai[2] != 99f)
            {
                if (num532 > 800f)
                    npc.ai[2] = 99f;
            }
            else if (num532 < 400f)
                npc.ai[2] = 0f;

            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
            {
                npc.ai[2] += 10f;
                if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
                {
                    npc.life = -1;
                    npc.HitEffect();
                    npc.active = false;
                }
            }

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Avoid cheap bullshit
            npc.damage = 0;

            if (npc.ai[2] == 99f)
            {
                float num496 = 0.1f;
                float num497 = 8f;
                float num498 = 0.5f;
                float num499 = 12f;
                float deceleration = Main.masterMode ? 0.92f : Main.expertMode ? 0.94f : 0.96f;

                if (Main.expertMode)
                {
                    num496 = Main.masterMode ? 0.14f : 0.12f;
                    num497 = Main.masterMode ? 11f : 9.5f;
                    num498 = Main.masterMode ? 0.6f : 0.55f;
                    num499 = Main.masterMode ? 16f : 14f;
                }

                if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y -= num496;
                    if (npc.velocity.Y > num497)
                        npc.velocity.Y = num497;
                }
                else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y += num496;
                    if (npc.velocity.Y < -num497)
                        npc.velocity.Y = -num497;
                }

                if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X -= num498;
                    if (npc.velocity.X > num499)
                        npc.velocity.X = num499;
                }

                if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X += num498;
                    if (npc.velocity.X < -num499)
                        npc.velocity.X = -num499;
                }
            }
            else if (npc.ai[2] == 0f || npc.ai[2] == 3f)
            {
                if (Main.npc[(int)npc.ai[1]].ai[1] == 3f)
                    npc.EncourageDespawn(10);

                if (Main.npc[(int)npc.ai[1]].ai[1] != 0f)
                {
                    if (Main.player[npc.target].dead)
                    {
                        npc.velocity.Y += 0.1f;
                        if (npc.velocity.Y > 16f)
                            npc.velocity.Y = 16f;
                    }
                    else
                    {
                        Vector2 vector62 = npc.Center;
                        float num533 = Main.player[npc.target].Center.X - vector62.X;
                        float num534 = Main.player[npc.target].Center.Y - vector62.Y;
                        float num535 = (float)Math.Sqrt(num533 * num533 + num534 * num534);
                        num535 = (Main.masterMode ? 16f : Main.expertMode ? 14f : 12f) / num535;
                        num533 *= num535;
                        num534 *= num535;
                        npc.rotation = (float)Math.Atan2(num534, num533) - MathHelper.PiOver2;
                        float deceleration = Main.masterMode ? 0.93f : Main.expertMode ? 0.95f : 0.97f;
                        if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 2f)
                        {
                            npc.rotation = (float)Math.Atan2(num534, num533) - MathHelper.PiOver2;
                            npc.velocity.X = num533;
                            npc.velocity.Y = num534;
                            npc.netUpdate = true;
                        }
                        else
                            npc.velocity *= deceleration;

                        npc.ai[3] += 1f;
                        if (npc.ai[3] >= (Main.masterMode ? 400f : Main.expertMode ? 500f : 600f))
                        {
                            npc.ai[2] = 0f;
                            npc.ai[3] = 0f;
                            npc.netUpdate = true;
                        }
                    }
                }
                else
                {
                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= (Main.masterMode ? 400f : Main.expertMode ? 500f : 600f))
                    {
                        npc.target = Main.npc[(int)npc.ai[1]].target;
                        npc.ai[2] += 1f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }

                    float num496 = 0.1f;
                    float num497 = 3f;
                    float num498 = 0.2f;
                    float num499 = 8f;
                    float deceleration = Main.masterMode ? 0.92f : Main.expertMode ? 0.94f : 0.96f;

                    if (Main.expertMode)
                    {
                        num496 = Main.masterMode ? 0.14f : 0.12f;
                        num497 = Main.masterMode ? 5f : 4f;
                        num498 = Main.masterMode ? 0.28f : 0.24f;
                        num499 = Main.masterMode ? 11f : 9.5f;
                    }

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 300f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y -= num496;
                        if (npc.velocity.Y > num497)
                            npc.velocity.Y = num497;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 230f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y += num496;
                        if (npc.velocity.Y < -num497)
                            npc.velocity.Y = -num497;
                    }

                    if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X + 250f)
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= (deceleration - 0.02f);

                        npc.velocity.X -= (num498 * 1.5f);
                        if (npc.velocity.X > num499 + 1f)
                            npc.velocity.X = num499 + 1f;
                    }

                    if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= (deceleration - 0.02f);

                        npc.velocity.X += num498;
                        if (npc.velocity.X < -num499)
                            npc.velocity.X = -num499;
                    }
                }

                Vector2 vector63 = npc.Center;
                float num536 = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - vector63.X;
                float num537 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector63.Y;
                float num538 = (float)Math.Sqrt(num536 * num536 + num537 * num537);
                npc.rotation = (float)Math.Atan2(num537, num536) + MathHelper.PiOver2;
            }
            else if (npc.ai[2] == 1f)
            {
                float deceleration = Main.masterMode ? 0.8f : Main.expertMode ? 0.85f : 0.9f;
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= deceleration;

                Vector2 vector64 = npc.Center;
                float num539 = Main.npc[(int)npc.ai[1]].Center.X - 280f * npc.ai[0] - vector64.X;
                float num540 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector64.Y;
                float num541 = (float)Math.Sqrt(num539 * num539 + num540 * num540);
                npc.rotation = (float)Math.Atan2(num540, num539) + MathHelper.PiOver2;
                npc.velocity.X = (npc.velocity.X * 5f + Main.npc[(int)npc.ai[1]].velocity.X) / 6f;
                npc.velocity.X += 0.5f;
                npc.velocity.Y -= 0.5f;
                if (npc.velocity.Y < -9f)
                    npc.velocity.Y = -9f;

                if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 280f)
                {
                    // Set damage
                    npc.damage = npc.defDamage;

                    npc.ai[2] = 2f;
                    vector64 = npc.Center;
                    num539 = Main.player[npc.target].Center.X - vector64.X;
                    num540 = Main.player[npc.target].Center.Y - vector64.Y;
                    num541 = (float)Math.Sqrt(num539 * num539 + num540 * num540);
                    num541 = (Main.masterMode ? 24f : Main.expertMode ? 22f : 20f) / num541;
                    npc.velocity.X = num539 * num541;
                    npc.velocity.Y = num540 * num541;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[2] == 2f)
            {
                // Set damage
                npc.damage = npc.defDamage;

                if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f)
                {
                    float numCharges = Main.masterMode ? 2f : Main.expertMode ? 3f : 4f;
                    if (npc.ai[3] >= numCharges)
                    {
                        npc.ai[2] = 3f;
                        npc.ai[3] = 0f;
                    }
                    else
                    {
                        npc.ai[2] = 1f;
                        npc.ai[3] += 1f;
                    }
                }
            }
            else if (npc.ai[2] == 4f)
            {
                Vector2 vector65 = npc.Center;
                float num542 = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - vector65.X;
                float num543 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector65.Y;
                float num544 = (float)Math.Sqrt(num542 * num542 + num543 * num543);
                npc.rotation = (float)Math.Atan2(num543, num542) + MathHelper.PiOver2;
                npc.velocity.Y = (npc.velocity.Y * 5f + Main.npc[(int)npc.ai[1]].velocity.Y) / 6f;
                npc.velocity.X += 0.5f;
                if (npc.velocity.X > 12f)
                    npc.velocity.X = 12f;

                if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X - 500f || npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X + 500f)
                {
                    // Set damage
                    npc.damage = npc.defDamage;

                    npc.ai[2] = 5f;
                    vector65 = npc.Center;
                    num542 = Main.player[npc.target].Center.X - vector65.X;
                    num543 = Main.player[npc.target].Center.Y - vector65.Y;
                    num544 = (float)Math.Sqrt(num542 * num542 + num543 * num543);
                    num544 = (Main.masterMode ? 20f : Main.expertMode ? 18.5f : 17f) / num544;
                    npc.velocity.X = num542 * num544;
                    npc.velocity.Y = num543 * num544;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[2] == 5f && npc.Center.X < Main.player[npc.target].Center.X - 100f)
            {
                // Set damage
                npc.damage = npc.defDamage;

                float numCharges = Main.masterMode ? 2f : Main.expertMode ? 3f : 4f;
                if (npc.ai[3] >= numCharges)
                {
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                }
                else
                {
                    npc.ai[2] = 4f;
                    npc.ai[3] += 1f;
                }
            }

            return false;
        }

        public static bool VanillaPrimeSawAI(NPC npc, Mod mod)
        {
            Vector2 vector56 = npc.Center;
            float num515 = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - vector56.X;
            float num516 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector56.Y;
            float num517 = (float)Math.Sqrt(num515 * num515 + num516 * num516);
            if (npc.ai[2] != 99f)
            {
                if (num517 > 800f)
                    npc.ai[2] = 99f;
            }
            else if (num517 < 400f)
                npc.ai[2] = 0f;

            npc.spriteDirection = -(int)npc.ai[0];
            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
            {
                npc.ai[2] += 10f;
                if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
                {
                    npc.life = -1;
                    npc.HitEffect();
                    npc.active = false;
                }
            }

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Min saw damage
            int reducedSetDamage = (int)Math.Round(npc.defDamage * 0.5);

            // Avoid cheap bullshit
            npc.damage = reducedSetDamage;

            if (npc.ai[2] == 99f)
            {
                float num496 = 0.1f;
                float num497 = 8f;
                float num498 = 0.5f;
                float num499 = 12f;
                float deceleration = Main.masterMode ? 0.92f : Main.expertMode ? 0.94f : 0.96f;

                if (Main.expertMode)
                {
                    num496 = Main.masterMode ? 0.14f : 0.12f;
                    num497 = Main.masterMode ? 11f : 9.5f;
                    num498 = Main.masterMode ? 0.6f : 0.55f;
                    num499 = Main.masterMode ? 16f : 14f;
                }

                if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y -= num496;
                    if (npc.velocity.Y > num497)
                        npc.velocity.Y = num497;
                }
                else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y += num496;
                    if (npc.velocity.Y < -num497)
                        npc.velocity.Y = -num497;
                }

                if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X -= num498;
                    if (npc.velocity.X > num499)
                        npc.velocity.X = num499;
                }

                if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X += num498;
                    if (npc.velocity.X < -num499)
                        npc.velocity.X = -num499;
                }
            }
            else if (npc.ai[2] == 0f || npc.ai[2] == 3f)
            {
                if (Main.npc[(int)npc.ai[1]].ai[1] == 3f)
                    npc.EncourageDespawn(10);

                if (Main.npc[(int)npc.ai[1]].ai[1] != 0f)
                {
                    if (Main.player[npc.target].dead)
                    {
                        npc.velocity.Y += 0.1f;
                        if (npc.velocity.Y > 16f)
                            npc.velocity.Y = 16f;
                    }
                    else
                    {
                        Vector2 vector57 = npc.Center;
                        float num518 = Main.player[npc.target].Center.X - vector57.X;
                        float num519 = Main.player[npc.target].Center.Y - vector57.Y;
                        float num520 = (float)Math.Sqrt(num518 * num518 + num519 * num519);
                        num520 = (Main.masterMode ? 9f : Main.expertMode ? 8f : 7f) / num520;
                        num518 *= num520;
                        num519 *= num520;
                        npc.rotation = (float)Math.Atan2(num519, num518) - MathHelper.PiOver2;

                        float deceleration = Main.masterMode ? 0.93f : Main.expertMode ? 0.95f : 0.97f;
                        float acceleration = Main.masterMode ? 0.07f : Main.expertMode ? 0.06f : 0.05f;
                        if (npc.velocity.X > num518)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X *= deceleration;

                            npc.velocity.X -= acceleration;
                        }

                        if (npc.velocity.X < num518)
                        {
                            if (npc.velocity.X < 0f)
                                npc.velocity.X *= deceleration;

                            npc.velocity.X += acceleration;
                        }

                        if (npc.velocity.Y > num519)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y *= deceleration;

                            npc.velocity.Y -= acceleration;
                        }

                        if (npc.velocity.Y < num519)
                        {
                            if (npc.velocity.Y < 0f)
                                npc.velocity.Y *= deceleration;

                            npc.velocity.Y += acceleration;
                        }
                    }

                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= (Main.masterMode ? 400f : Main.expertMode ? 500f : 600f))
                    {
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= (Main.masterMode ? 180f : Main.expertMode ? 240f : 300f))
                    {
                        npc.target = Main.npc[(int)npc.ai[1]].target;
                        npc.ai[2] += 1f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }

                    float num496 = 0.04f;
                    float num497 = 3f;
                    float num498 = 0.3f;
                    float num499 = 12f;
                    float deceleration = Main.masterMode ? 0.92f : Main.expertMode ? 0.94f : 0.96f;

                    if (Main.expertMode)
                    {
                        num496 = Main.masterMode ? 0.06f : 0.05f;
                        num497 = Main.masterMode ? 5f : 4f;
                        num498 = Main.masterMode ? 0.4f : 0.35f;
                        num499 = Main.masterMode ? 16f : 14f;
                    }

                    if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 320f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y -= num496;
                        if (npc.velocity.Y > num497)
                            npc.velocity.Y = num497;
                    }
                    else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 260f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y *= deceleration;

                        npc.velocity.Y += num496;
                        if (npc.velocity.Y < -num497)
                            npc.velocity.Y = -num497;
                    }

                    if (npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X)
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X -= num498;
                        if (npc.velocity.X > num499)
                            npc.velocity.X = num499;
                    }

                    if (npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X - 250f)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X *= deceleration;

                        npc.velocity.X += num498;
                        if (npc.velocity.X < -num499)
                            npc.velocity.X = -num499;
                    }
                }

                Vector2 vector58 = npc.Center;
                float num521 = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - vector58.X;
                float num522 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector58.Y;
                float num523 = (float)Math.Sqrt(num521 * num521 + num522 * num522);
                npc.rotation = (float)Math.Atan2(num522, num521) + MathHelper.PiOver2;
            }
            else if (npc.ai[2] == 1f)
            {
                Vector2 vector59 = npc.Center;
                float num524 = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - vector59.X;
                float num525 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector59.Y;
                float num526 = (float)Math.Sqrt(num524 * num524 + num525 * num525);
                npc.rotation = (float)Math.Atan2(num525, num524) + MathHelper.PiOver2;

                float deceleration = Main.masterMode ? 0.85f : Main.expertMode ? 0.9f : 0.95f;
                npc.velocity.X *= deceleration;
                npc.velocity.Y -= 0.1f;
                if (npc.velocity.Y < -8f)
                    npc.velocity.Y = -8f;

                if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 200f)
                {
                    // Set damage
                    npc.damage = npc.defDamage;

                    npc.ai[2] = 2f;
                    vector59 = npc.Center;
                    num524 = Main.player[npc.target].Center.X - vector59.X;
                    num525 = Main.player[npc.target].Center.Y - vector59.Y;
                    num526 = (float)Math.Sqrt(num524 * num524 + num525 * num525);
                    num526 = (Main.masterMode ? 26f : Main.expertMode ? 24f : 22f) / num526;
                    npc.velocity.X = num524 * num526;
                    npc.velocity.Y = num525 * num526;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[2] == 2f)
            {
                // Set damage
                npc.damage = npc.defDamage;

                if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f)
                    npc.ai[2] = 3f;
            }
            else if (npc.ai[2] == 4f)
            {
                // Set damage
                npc.damage = npc.defDamage;

                Vector2 vector60 = npc.Center;
                float num527 = Main.player[npc.target].Center.X - vector60.X;
                float num528 = Main.player[npc.target].Center.Y - vector60.Y;
                float num529 = (float)Math.Sqrt(num527 * num527 + num528 * num528);
                num529 = (Main.masterMode ? 9f : Main.expertMode ? 8f : 7f) / num529;
                num527 *= num529;
                num528 *= num529;

                float deceleration = Main.masterMode ? 0.93f : Main.expertMode ? 0.95f : 0.97f;
                float acceleration = Main.masterMode ? 0.07f : Main.expertMode ? 0.06f : 0.05f;

                if (npc.velocity.X > num527)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X -= acceleration;
                }

                if (npc.velocity.X < num527)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= deceleration;

                    npc.velocity.X += acceleration;
                }

                if (npc.velocity.Y > num528)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y -= acceleration;
                }

                if (npc.velocity.Y < num528)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= deceleration;

                    npc.velocity.Y += acceleration;
                }

                npc.ai[3] += 1f;
                if (npc.ai[3] >= (Main.masterMode ? 400f : Main.expertMode ? 500f : 600f))
                {
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }

                vector60 = npc.Center;
                num527 = Main.npc[(int)npc.ai[1]].Center.X - 200f * npc.ai[0] - vector60.X;
                num528 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector60.Y;
                num529 = (float)Math.Sqrt(num527 * num527 + num528 * num528);
                npc.rotation = (float)Math.Atan2(num528, num527) + MathHelper.PiOver2;
            }
            else if (npc.ai[2] == 5f && ((npc.velocity.X > 0f && npc.Center.X > Main.player[npc.target].Center.X) || (npc.velocity.X < 0f && npc.Center.X < Main.player[npc.target].Center.X)))
                npc.ai[2] = 0f;

            return false;
        }
    }
}
