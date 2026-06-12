using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    public class SkeletronPrimeAI : VanillaAIOverride
    {
        // Vanilla values
        public static int SpinDamageMult = 2; // 158
        public static int LaserDamage = 25; // 100

        // Rev+ exclusive
        public static int SkullDamage = 22; // 88
        public static int RocketDamage = 30; // 120

        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            if (NPC.ai[3] != 0f)
                NPC.mechQueen = NPC.whoAmI;

            // Spawn arms
            if (calamityGlobalNPC.newAI[1] == 0f)
            {
                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                calamityGlobalNPC.newAI[1] = 1f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int arm = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PrimeCannon, NPC.whoAmI);
                    Main.npc[arm].ai[0] = -1f;
                    Main.npc[arm].ai[1] = NPC.whoAmI;
                    Main.npc[arm].target = NPC.target;
                    Main.npc[arm].netUpdate = true;

                    arm = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PrimeSaw, NPC.whoAmI);
                    Main.npc[arm].ai[0] = 1f;
                    Main.npc[arm].ai[1] = NPC.whoAmI;
                    Main.npc[arm].target = NPC.target;
                    Main.npc[arm].netUpdate = true;

                    arm = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PrimeVice, NPC.whoAmI);
                    Main.npc[arm].ai[0] = -1f;
                    Main.npc[arm].ai[1] = NPC.whoAmI;
                    Main.npc[arm].target = NPC.target;
                    Main.npc[arm].ai[3] = 150f;
                    Main.npc[arm].netUpdate = true;

                    arm = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PrimeLaser, NPC.whoAmI);
                    Main.npc[arm].ai[0] = 1f;
                    Main.npc[arm].ai[1] = NPC.whoAmI;
                    Main.npc[arm].target = NPC.target;
                    Main.npc[arm].netUpdate = true;
                    Main.npc[arm].ai[3] = 150f;
                }

                NPC.netUpdate = true;
                NPC.SyncExtraAI();
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
            NPC.chaseable = allArmsDead;

            NPC.defense = NPC.defDefense;
            NPC.damage = NPC.defDamage;

            // Phases
            bool phase2 = lifeRatio < 0.66f;
            bool phase3 = lifeRatio < 0.33f;

            // Despawn
            if (NPC.ai[1] != 3f)
            {
                int despawnDistanceInTiles = 500;
                if (Main.player[NPC.target].dead || Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) / 16f > despawnDistanceInTiles)
                {
                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                    if (Main.player[NPC.target].dead || Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) / 16f > despawnDistanceInTiles)
                        NPC.ai[1] = 3f;
                }
                else if (NPC.timeLeft < 1800)
                    NPC.timeLeft = 1800;
            }

            // Activate daytime enrage
            if (Main.IsItDay() && !BossRushEvent.BossRushActive && NPC.ai[1] != 3f && NPC.ai[1] != 2f)
            {
                // Heal
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int healAmt = NPC.life - 300;
                    if (healAmt < 0)
                    {
                        int absHeal = Math.Abs(healAmt);
                        NPC.life += absHeal;
                        NPC.HealEffect(absHeal, true);
                        NPC.netUpdate = true;
                    }
                }

                NPC.ai[1] = 2f;
                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
            }

            bool normalLaserRotation = NPC.localAI[1] % 2f == 0f;

            // Float near player
            if (NPC.ai[1] == 0f || NPC.ai[1] == 4f)
            {
                // Start other phases; if arms are dead, start with spin phase
                if (phase2 || Main.getGoodWorld || allArmsDead)
                {
                    // Start spin phase after 1.5 seconds if close enough; forced after 4.5 seconds
                    NPC.ai[2] += phase3 ? 1.5f : 1f;
                    if (NPC.ai[2] >= (90f - (death ? 15f * (1f - lifeRatio) : 0f)))
                    {
                        bool shouldSpinAround = NPC.ai[1] == 4f && ((NPC.position.Y < Main.player[NPC.target].position.Y - 320f &&
                            Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) < 600f && Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 400f) ||
                            NPC.ai[2] >= (270f - (death ? 15f * (1f - lifeRatio) : 0f)));

                        if (shouldSpinAround || NPC.ai[1] != 4f)
                        {
                            if (shouldSpinAround)
                            {
                                NPC.localAI[3] = 200f;
                                NPC.localAI[1] = 0;
                                NPC.SyncVanillaLocalAI();
                            }

                            NPC.ai[2] = 0f;
                            NPC.ai[1] = shouldSpinAround ? 5f : 1f;
                            CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                            NPC.netUpdate = true;
                        }
                    }
                }

                if (NPC.IsMechQueenUp)
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.X / 15f * 0.5f, 0.75f);
                else
                    NPC.rotation = NPC.velocity.X / 15f;

                float acceleration = death ? (0.125f + 0.05f * (1f - lifeRatio)) : 0.1f;
                float accelerationMult = 1f;
                if (!cannonAlive)
                {
                    acceleration += 0.0125f;
                    accelerationMult += 0.25f;
                }
                if (!laserAlive)
                {
                    acceleration += 0.0125f;
                    accelerationMult += 0.25f;
                }
                if (!viceAlive)
                    acceleration += 0.0125f;
                if (!sawAlive)
                    acceleration += 0.0125f;
                if (death)
                    acceleration *= accelerationMult;

                float topVelocity = acceleration * 100f;
                float deceleration = death ? 0.7f : 0.85f;

                float headDecelerationUpDist = 0f;
                float headDecelerationDownDist = 0f;
                float headDecelerationHorizontalDist = 0f;
                int headHorizontalDirection = ((!(Main.player[NPC.target].Center.X < NPC.Center.X)) ? 1 : (-1));
                if (NPC.IsMechQueenUp)
                {
                    headDecelerationHorizontalDist = -150f * (float)headHorizontalDirection;
                    headDecelerationUpDist = -100f;
                    headDecelerationDownDist = -100f;
                }

                if (NPC.position.Y > Main.player[NPC.target].position.Y - (320f + headDecelerationUpDist))
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= deceleration;

                    NPC.velocity.Y -= acceleration;

                    if (NPC.velocity.Y > topVelocity)
                        NPC.velocity.Y = topVelocity;
                }
                else if (NPC.position.Y < Main.player[NPC.target].position.Y - (360f + headDecelerationDownDist))
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= deceleration;

                    NPC.velocity.Y += acceleration;

                    if (NPC.velocity.Y < -topVelocity)
                        NPC.velocity.Y = -topVelocity;
                }

                if (NPC.Center.X > Main.player[NPC.target].Center.X + (400f + headDecelerationHorizontalDist))
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X *= deceleration;

                    NPC.velocity.X -= acceleration;

                    if (NPC.velocity.X > topVelocity)
                        NPC.velocity.X = topVelocity;
                }
                if (NPC.Center.X < Main.player[NPC.target].Center.X - (400f + headDecelerationHorizontalDist))
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X *= deceleration;

                    NPC.velocity.X += acceleration;

                    if (NPC.velocity.X < -topVelocity)
                        NPC.velocity.X = -topVelocity;
                }
            }

            else
            {
                // Spinning
                if (NPC.ai[1] == 1f)
                {
                    NPC.defense = NPC.defDefense * 2;
                    NPC.damage = NPC.defDamage * SpinDamageMult;

                    calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = true;

                    if (phase2 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[0] += 1f;
                        if (NPC.localAI[0] >= 45f)
                        {
                            NPC.localAI[0] = 0f;

                            int totalProjectiles = death ? 15 : 12;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = ProjectileID.DeathLaser;

                            float velocity = 3f;
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 laserFireDirection = spinningPoint.RotatedBy(radians * k);
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + laserFireDirection.SafeNormalize(Vector2.UnitY) * 100f, laserFireDirection, type, LaserDamage.CalculateMechDamage(), 0f, Main.myPlayer, 1f, 0f);
                                Main.projectile[proj].timeLeft = 900;
                            }
                            NPC.localAI[1] += 1f;
                        }
                    }

                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] == 2f)
                        SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);

                    // Spin for 3 seconds then return to floating phase
                    float phaseTimer = 240f;
                    if (phase2 && !phase3)
                        phaseTimer += 60f;

                    if (NPC.ai[2] >= (phaseTimer - (death ? 60f * (1f - lifeRatio) : 0f)))
                    {
                        CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                        NPC.ai[2] = 0f;
                        // Fly overhead and spit missiles if on low health
                        NPC.ai[1] = phase3 ? 6f : 4f;
                        NPC.localAI[0] = 0f;
                    }

                    if (NPC.IsMechQueenUp)
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.X / 15f * 0.5f, 0.75f);
                    else
                        NPC.rotation += NPC.direction * 0.3f;

                    Vector2 headPosition = NPC.Center;
                    float headTargetX = Main.player[NPC.target].Center.X - headPosition.X;
                    float headTargetY = Main.player[NPC.target].Center.Y - headPosition.Y;
                    float headTargetDistance = (float)Math.Sqrt(headTargetX * headTargetX + headTargetY * headTargetY);

                    float speed = death ? 8f : 6f;
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
                    NPC.velocity.X = headTargetX * headTargetDistance;
                    NPC.velocity.Y = headTargetY * headTargetDistance;

                    if (NPC.IsMechQueenUp)
                    {
                        float mechdusaAccelMult = Vector2.Distance(NPC.Center, Main.player[NPC.target].Center);
                        if (mechdusaAccelMult < 0.1f)
                            mechdusaAccelMult = 0f;

                        if (mechdusaAccelMult < speed)
                            NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * mechdusaAccelMult;
                    }
                }

                // Daytime enrage
                if (NPC.ai[1] == 2f)
                {
                    NPC.damage = 1000;
                    calamityGlobalNPC.DR = 0.9999f;
                    calamityGlobalNPC.unbreakableDR = true;

                    calamityGlobalNPC.CurrentlyEnraged = true;
                    calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = true;

                    if (NPC.IsMechQueenUp)
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.X / 15f * 0.5f, 0.75f);
                    else
                        NPC.rotation += NPC.direction * 0.3f;

                    Vector2 enragedHeadPosition = NPC.Center;
                    float enragedHeadTargetX = Main.player[NPC.target].Center.X - enragedHeadPosition.X;
                    float enragedHeadTargetY = Main.player[NPC.target].Center.Y - enragedHeadPosition.Y;
                    float enragedHeadTargetDist = (float)Math.Sqrt(enragedHeadTargetX * enragedHeadTargetX + enragedHeadTargetY * enragedHeadTargetY);

                    float enragedHeadSpeed = 10f;
                    enragedHeadSpeed += enragedHeadTargetDist / 100f;
                    if (enragedHeadSpeed < 8f)
                        enragedHeadSpeed = 8f;
                    if (enragedHeadSpeed > 32f)
                        enragedHeadSpeed = 32f;

                    enragedHeadTargetDist = enragedHeadSpeed / enragedHeadTargetDist;
                    NPC.velocity.X = enragedHeadTargetX * enragedHeadTargetDist;
                    NPC.velocity.Y = enragedHeadTargetY * enragedHeadTargetDist;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[0] += 1f;
                        if (NPC.localAI[0] >= 60f)
                        {
                            NPC.localAI[0] = 0f;
                            Vector2 headCenter = NPC.Center;
                            if (Collision.CanHit(headCenter, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                            {
                                enragedHeadSpeed = 7f;
                                float enragedHeadSkullTargetX = Main.player[NPC.target].Center.X - headCenter.X + Main.rand.Next(-20, 21);
                                float enragedHeadSkullTargetY = Main.player[NPC.target].Center.Y - headCenter.Y + Main.rand.Next(-20, 21);
                                float enragedHeadSkullTargetDist = (float)Math.Sqrt(enragedHeadSkullTargetX * enragedHeadSkullTargetX + enragedHeadSkullTargetY * enragedHeadSkullTargetY);
                                enragedHeadSkullTargetDist = enragedHeadSpeed / enragedHeadSkullTargetDist;
                                enragedHeadSkullTargetX *= enragedHeadSkullTargetDist;
                                enragedHeadSkullTargetY *= enragedHeadSkullTargetDist;

                                Vector2 value = new Vector2(enragedHeadSkullTargetX * 1f + Main.rand.Next(-50, 51) * 0.01f, enragedHeadSkullTargetY * 1f + Main.rand.Next(-50, 51) * 0.01f).SafeNormalize(Vector2.UnitY);
                                value *= enragedHeadSpeed;
                                value += NPC.velocity;
                                enragedHeadSkullTargetX = value.X;
                                enragedHeadSkullTargetY = value.Y;

                                int type = ProjectileID.Skull;
                                headCenter += value * 5f;
                                int enragedSkulls = Projectile.NewProjectile(NPC.GetSource_FromAI(), headCenter.X, headCenter.Y, enragedHeadSkullTargetX, enragedHeadSkullTargetY, type, 250, 0f, Main.myPlayer, -3f, 0f);
                                Main.projectile[enragedSkulls].timeLeft = 300;
                            }
                        }
                    }
                }

                // Despawning
                if (NPC.ai[1] == 3f)
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

                            NPC.EncourageDespawn(5);
                        }

                        NPC.velocity.Y += 0.1f;
                        if (NPC.velocity.Y < 0f)
                            NPC.velocity.Y *= 0.95f;

                        NPC.velocity.X *= 0.95f;
                        if (NPC.velocity.Y > 13f)
                            NPC.velocity.Y = 13f;
                    }
                    else
                    {
                        NPC.velocity.Y += 0.1f;
                        if (NPC.velocity.Y < 0f)
                            NPC.velocity.Y *= 0.9f;

                        NPC.velocity.X *= 0.9f;

                        if (NPC.timeLeft > 500)
                            NPC.timeLeft = 500;
                    }
                }

                // Fly around in a circle
                if (NPC.ai[1] == 5f)
                {
                    NPC.ai[2] += 1f;

                    NPC.rotation = NPC.velocity.X / 50f;

                    float skullSpawnDivisor = death ? 15f - (float)Math.Round(3f * (1f - lifeRatio)) : 15f;
                    float totalSkulls = 12f;
                    int skullSpread = death ? 125 : 100;

                    // Spin for about 3 seconds
                    // Decreasing this number will INCREASE how fast he moves while spinning
                    float spinVelocity = 25f;
                    if (NPC.ai[2] == 2f)
                    {
                        // Play angry noise
                        SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);

                        // Set spin direction
                        if (Main.player[NPC.target].velocity.X > 0f)
                            calamityGlobalNPC.newAI[0] = 1f;
                        else if (Main.player[NPC.target].velocity.X < 0f)
                            calamityGlobalNPC.newAI[0] = -1f;
                        else
                            calamityGlobalNPC.newAI[0] = Main.player[NPC.target].direction;

                        // Set spin velocity
                        NPC.velocity.X = MathHelper.Pi * NPC.localAI[3] / spinVelocity;
                        NPC.velocity *= -calamityGlobalNPC.newAI[0];
                        NPC.SyncExtraAI();
                        NPC.netUpdate = true;
                    }

                    // Maintain velocity and spit skulls
                    else if (NPC.ai[2] > 2f)
                    {
                        NPC.velocity = NPC.velocity.RotatedBy(MathHelper.Pi / spinVelocity * -calamityGlobalNPC.newAI[0]);
                        if (NPC.ai[2] == 3f)
                            NPC.velocity *= 0.6f;

                        if (NPC.ai[2] % skullSpawnDivisor == 0f)
                        {
                            NPC.localAI[0] += 1f;

                            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 96f)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 headCenter = NPC.Center;
                                    float enragedHeadSpeed = death ? (5f + (1f - lifeRatio)) : 4f;
                                    float enragedHeadSkullTargetX = Main.player[NPC.target].Center.X - headCenter.X + Main.rand.Next(-20, 21);
                                    float enragedHeadSkullTargetY = Main.player[NPC.target].Center.Y - headCenter.Y + Main.rand.Next(-20, 21);
                                    float enragedHeadSkullTargetDist = (float)Math.Sqrt(enragedHeadSkullTargetX * enragedHeadSkullTargetX + enragedHeadSkullTargetY * enragedHeadSkullTargetY);
                                    enragedHeadSkullTargetDist = enragedHeadSpeed / enragedHeadSkullTargetDist;
                                    enragedHeadSkullTargetX *= enragedHeadSkullTargetDist;
                                    enragedHeadSkullTargetY *= enragedHeadSkullTargetDist;

                                    Vector2 value = new Vector2(enragedHeadSkullTargetX + Main.rand.Next(-skullSpread, skullSpread + 1) * 0.01f, enragedHeadSkullTargetY + Main.rand.Next(-skullSpread, skullSpread + 1) * 0.01f).SafeNormalize(Vector2.UnitY);
                                    value *= enragedHeadSpeed;
                                    enragedHeadSkullTargetX = value.X;
                                    enragedHeadSkullTargetY = value.Y;

                                    int type = ProjectileID.Skull;

                                    int enragedSkulls = Projectile.NewProjectile(NPC.GetSource_FromAI(), headCenter.X, headCenter.Y + 30f, enragedHeadSkullTargetX, enragedHeadSkullTargetY, type, SkullDamage.CalculateMechDamage(), 0f, Main.myPlayer, -3f, 0f);
                                    Main.projectile[enragedSkulls].timeLeft = 480;
                                    Main.projectile[enragedSkulls].tileCollide = false;
                                }
                            }

                            // Go to floating phase, or spinning phase if in phase 2
                            // NetMode check here fixes the strange teleporting issue prime had. The issue was that this attack was ending on the client
                            // and then some time later ending a second time on the server, causing major de-sync and the issues of prime seeming to teleport.
                            if (NPC.localAI[0] >= totalSkulls && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY);

                                NPC.ai[1] = 1f;
                                NPC.ai[2] = 0f;
                                NPC.localAI[3] = 0f;
                                NPC.localAI[0] = 0f;
                                calamityGlobalNPC.newAI[0] = 0f;
                                NPC.SyncVanillaLocalAI();
                                NPC.SyncExtraAI();
                                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                                NPC.netUpdate = true;
                            }
                        }
                    }
                }

                // Fly overhead and spit missiles
                if (NPC.ai[1] == 6f)
                {
                    NPC.rotation = NPC.velocity.X / 15f;

                    float flightVelocity = death ? 25f : 18f;
                    float flightAcceleration = death ? 0.96f : 0.6f;

                    Vector2 destination = new Vector2(Main.player[NPC.target].Center.X, Main.player[NPC.target].Center.Y - 420f);
                    NPC.SimpleFlyMovement((destination - NPC.Center).SafeNormalize(Vector2.UnitY) * flightVelocity, flightAcceleration);

                    // Spit homing missiles and then go to floating phase
                    NPC.localAI[3] += 1f;
                    if (Vector2.Distance(NPC.Center, destination) < 80f || NPC.ai[2] > 0f || NPC.localAI[3] > 120f)
                    {
                        float missileSpawnDivisor = 12f;
                        float totalMissiles = 10f;
                        NPC.ai[2] += 1f;
                        if (NPC.ai[2] % missileSpawnDivisor == 0f)
                        {
                            NPC.localAI[0] += 1f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 velocity = (-Vector2.UnitY * 3f).RotatedByRandom(MathHelper.Pi / 8f);
                                int type = ProjectileID.RocketSkeleton;

                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(NPC.width / 2), NPC.Center.Y + 30f, velocity.X, velocity.Y, type, RocketDamage.CalculateMechDamage(), 0f, Main.myPlayer, NPC.target, 1f);
                                Main.projectile[proj].timeLeft = 540;
                            }

                            SoundEngine.PlaySound(SoundID.Item62, NPC.Center);

                            if (NPC.localAI[0] >= totalMissiles)
                            {
                                NPC.ai[1] = 4f;
                                NPC.ai[2] = -60f;
                                NPC.localAI[3] = 0f;
                                calamityGlobalNPC.newAI[0] = 0f;
                                NPC.localAI[0] = 0f;
                                NPC.SyncVanillaLocalAI();
                                NPC.SyncExtraAI();
                                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                                NPC.netUpdate = true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public override bool PreDraw(Mod mod, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsMechQueenUp)
                return true;

            // Allows correct frames to draw in Rev+ phases
            // GFB can rot for all I care
            var calNPC = NPC.GetGlobalNPC<CalamityGlobalNPC>();
            int frameHeight = TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type];
            if (NPC.ai[1] == 0f || NPC.ai[1] == 4f)
            {
                calNPC.newAI[2] += 1f;
                if (calNPC.newAI[2] >= 12f)
                {
                    calNPC.newAI[2] = 0f;
                    calNPC.newAI[3] += frameHeight;

                    if (calNPC.newAI[3] / frameHeight >= 2f)
                        calNPC.newAI[3] = 0f;
                }
            }

            // Spinning probe spawn or fly over phase
            else if (NPC.ai[1] == 5f || NPC.ai[1] == 6f)
            {
                calNPC.newAI[2] = 0f;
                calNPC.newAI[3] = frameHeight;
            }

            // Spinning phase
            else
            {
                calNPC.newAI[2] = 0f;
                calNPC.newAI[3] = frameHeight * 2;
            }

            NPC.frame.Y = (int)calNPC.newAI[3];
            return true;
        }

        public class PrimeLaserAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                // Set direction
                NPC.spriteDirection = -(int)NPC.ai[0];

                // Despawn if head is gone
                if (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)NPC.ai[1]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
                {
                    NPC.ai[2] += 10f;
                    if (NPC.ai[2] > 50f || !Main.dedServ)
                    {
                        NPC.life = -1;
                        NPC.HitEffect(0, 10.0);
                        NPC.active = false;
                    }
                }

                CalamityGlobalNPC.primeLaser = NPC.whoAmI;

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
                bool dontAttack = NPC.Calamity().newAI[2] < timeToNotAttack;
                if (dontAttack)
                {
                    NPC.Calamity().newAI[2] += 1f;
                    if (NPC.Calamity().newAI[2] >= timeToNotAttack)
                        NPC.SyncExtraAI();
                }

                bool normalLaserRotation = NPC.localAI[1] % 2f == 0f;

                // Movement
                float acceleration = death ? 0.385f : 0.25f;
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
                if (death)
                    acceleration *= accelerationMult;

                float topVelocity = acceleration * 100f;
                float deceleration = death ? 0.6f : 0.8f;

                if (NPC.position.Y > Main.npc[(int)NPC.ai[1]].position.Y - 70f)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= deceleration;

                    NPC.velocity.Y -= acceleration;

                    if (NPC.velocity.Y > topVelocity)
                        NPC.velocity.Y = topVelocity;
                }
                else if (NPC.position.Y < Main.npc[(int)NPC.ai[1]].position.Y - 100f)
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= deceleration;

                    NPC.velocity.Y += acceleration;

                    if (NPC.velocity.Y < -topVelocity)
                        NPC.velocity.Y = -topVelocity;
                }

                if (NPC.Center.X > Main.npc[(int)NPC.ai[1]].Center.X - 130f * NPC.ai[0])
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X *= deceleration;

                    NPC.velocity.X -= acceleration;

                    if (NPC.velocity.X > topVelocity)
                        NPC.velocity.X = topVelocity;
                }
                if (NPC.Center.X < Main.npc[(int)NPC.ai[1]].Center.X - 160f * NPC.ai[0])
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X *= deceleration;

                    NPC.velocity.X += acceleration;

                    if (NPC.velocity.X < -topVelocity)
                        NPC.velocity.X = -topVelocity;
                }

                // Phase 1
                if (NPC.ai[2] == 0f)
                {
                    // Despawn if head is despawning
                    if (Main.npc[(int)NPC.ai[1]].ai[1] == 3f && NPC.timeLeft > 10)
                        NPC.timeLeft = 10;

                    // Go to other phase after 13.3 seconds (change this as each arm dies)
                    NPC.ai[3] += 1f;
                    if (!cannonAlive)
                        NPC.ai[3] += 1f;
                    if (!viceAlive)
                        NPC.ai[3] += 1f;
                    if (!sawAlive)
                        NPC.ai[3] += 1f;

                    if (NPC.ai[3] >= (death ? 200f : 800f))
                    {
                        NPC.target = Main.npc[(int)NPC.ai[1]].target;
                        NPC.localAI[0] = 0f;
                        NPC.ai[2] = 1f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }

                    Vector2 laserArmPosition = NPC.Center;
                    float laserArmTargetX = Main.player[NPC.target].Center.X - laserArmPosition.X;
                    float laserArmTargetY = Main.player[NPC.target].Center.Y - laserArmPosition.Y;
                    float laserArmTargetDist = (float)Math.Sqrt(laserArmTargetX * laserArmTargetX + laserArmTargetY * laserArmTargetY);
                    NPC.rotation = (float)Math.Atan2(laserArmTargetY, laserArmTargetX) - MathHelper.PiOver2;

                    if (Main.netMode != NetmodeID.MultiplayerClient && !dontAttack)
                    {
                        // Fire laser every 0.8 seconds (change this as each arm dies to fire more aggressively)
                        NPC.localAI[0] += 1f;
                        if (!cannonAlive)
                            NPC.localAI[0] += 1f;
                        if (!viceAlive)
                            NPC.localAI[0] += 1f;
                        if (!sawAlive)
                            NPC.localAI[0] += 1f;

                        if (NPC.localAI[0] >= 48f)
                        {
                            NPC.localAI[0] = 0f;
                            float laserSpeed = 4f;
                            int type = ProjectileID.DeathLaser;

                            laserArmTargetDist = laserSpeed / laserArmTargetDist;
                            laserArmTargetX *= laserArmTargetDist;
                            laserArmTargetY *= laserArmTargetDist;
                            Vector2 laserVelocity = new Vector2(laserArmTargetX, laserArmTargetY);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), laserArmPosition + laserVelocity.SafeNormalize(Vector2.UnitY) * 100f, laserVelocity, type, LaserDamage.CalculateMechDamage(), 0f, Main.myPlayer, 1f, 0f);
                        }
                    }
                }

                // Other phase, get closer to the player and fire ring of lasers
                else if (NPC.ai[2] == 1f)
                {
                    // Go to phase 1 after 2 seconds (change this as each arm dies to stay in this phase for longer)
                    NPC.ai[3] += 1f;

                    float timeLimit = 135f;
                    float timeMult = 1.882075f;
                    if (!cannonAlive)
                        timeLimit *= timeMult;
                    if (!viceAlive)
                        timeLimit *= timeMult;
                    if (!sawAlive)
                        timeLimit *= timeMult;

                    if (NPC.ai[3] >= timeLimit)
                    {
                        NPC.target = Main.npc[(int)NPC.ai[1]].target;
                        NPC.localAI[0] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }

                    Vector2 laserRingArmPosition = NPC.Center;
                    float laserRingTargetX = Main.player[NPC.target].Center.X - laserRingArmPosition.X;
                    float laserRingTargetY = Main.player[NPC.target].Center.Y - laserRingArmPosition.Y;
                    NPC.rotation = (float)Math.Atan2(laserRingTargetY, laserRingTargetX) - MathHelper.PiOver2;

                    if (Main.netMode != NetmodeID.MultiplayerClient && !dontAttack)
                    {
                        // Fire laser every 1.5 seconds (change this as each arm dies to fire more aggressively)
                        NPC.localAI[0] += 1f;
                        if (!cannonAlive)
                            NPC.localAI[0] += 0.5f;
                        if (!viceAlive)
                            NPC.localAI[0] += 0.5f;
                        if (!sawAlive)
                            NPC.localAI[0] += 0.5f;

                        if (NPC.localAI[0] >= 120f)
                        {
                            NPC.localAI[0] = 0f;
                            int totalProjectiles = death ? 24 : 16;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = ProjectileID.DeathLaser;

                            float velocity = 3f;
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float laserVelocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-laserVelocityX, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 laserFireDirection = spinningPoint.RotatedBy(radians * k);
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + laserFireDirection.SafeNormalize(Vector2.UnitY) * 100f, laserFireDirection, type, LaserDamage.CalculateMechDamage(), 0f, Main.myPlayer, 1f, 0f);
                                Main.projectile[proj].timeLeft = 900;
                            }
                            NPC.localAI[1] += 1f;
                        }
                    }
                }

                return false;
            }
        }

        public class PrimeCannonAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                NPC.spriteDirection = -(int)NPC.ai[0];

                if (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)NPC.ai[1]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
                {
                    NPC.ai[2] += 10f;
                    if (NPC.ai[2] > 50f || !Main.dedServ)
                    {
                        NPC.life = -1;
                        NPC.HitEffect(0, 10.0);
                        NPC.active = false;
                    }
                }

                CalamityGlobalNPC.primeCannon = NPC.whoAmI;

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
                bool dontAttack = NPC.Calamity().newAI[2] < timeToNotAttack;
                if (dontAttack)
                {
                    NPC.Calamity().newAI[2] += 1f;
                    if (NPC.Calamity().newAI[2] >= timeToNotAttack)
                        NPC.SyncExtraAI();
                }

                bool fireSlower = false;
                if (laserAlive)
                {
                    // If laser is firing ring of lasers
                    if (Main.npc[CalamityGlobalNPC.primeLaser].ai[2] == 1f)
                        fireSlower = true;
                }
                else
                {
                    fireSlower = NPC.ai[2] == 0f;

                    if (fireSlower)
                    {
                        // Go to other phase after 13.33 seconds (change this as each arm dies)
                        NPC.ai[3] += 1f;
                        if (!laserAlive)
                            NPC.ai[3] += 1f;
                        if (!viceAlive)
                            NPC.ai[3] += 1f;
                        if (!sawAlive)
                            NPC.ai[3] += 1f;

                        if (NPC.ai[3] >= (death ? 200f : 800f))
                        {
                            NPC.target = Main.npc[(int)NPC.ai[1]].target;
                            NPC.localAI[0] = 0f;
                            NPC.ai[2] = 1f;
                            fireSlower = false;
                            NPC.ai[3] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        // Go to phase 1 after 2 seconds (change this as each arm dies to stay in this phase for longer)
                        NPC.ai[3] += 1f;

                        float timeLimit = 120f;
                        float timeMult = 1.882075f;
                        if (!laserAlive)
                            timeLimit *= timeMult;
                        if (!viceAlive)
                            timeLimit *= timeMult;
                        if (!sawAlive)
                            timeLimit *= timeMult;

                        if (NPC.ai[3] >= timeLimit)
                        {
                            NPC.target = Main.npc[(int)NPC.ai[1]].target;
                            NPC.localAI[0] = 0f;
                            NPC.ai[2] = 0f;
                            fireSlower = true;
                            NPC.ai[3] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                }

                // Movement
                float acceleration = death ? 0.385f : 0.25f;
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
                if (death)
                    acceleration *= accelerationMult;

                float topVelocity = acceleration * 100f;
                float deceleration = death ? 0.6f : 0.8f;

                if (NPC.position.Y > Main.npc[(int)NPC.ai[1]].position.Y - 70f)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= deceleration;

                    NPC.velocity.Y -= acceleration;

                    if (NPC.velocity.Y > topVelocity)
                        NPC.velocity.Y = topVelocity;
                }
                else if (NPC.position.Y < Main.npc[(int)NPC.ai[1]].position.Y - 100f)
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= deceleration;

                    NPC.velocity.Y += acceleration;

                    if (NPC.velocity.Y < -topVelocity)
                        NPC.velocity.Y = -topVelocity;
                }

                if (NPC.Center.X > Main.npc[(int)NPC.ai[1]].Center.X + 130f)
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X *= deceleration;

                    NPC.velocity.X -= acceleration;

                    if (NPC.velocity.X > topVelocity)
                        NPC.velocity.X = topVelocity;
                }
                if (NPC.Center.X < Main.npc[(int)NPC.ai[1]].Center.X + 160f)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X *= deceleration;

                    NPC.velocity.X += acceleration;

                    if (NPC.velocity.X < -topVelocity)
                        NPC.velocity.X = -topVelocity;
                }

                if (fireSlower)
                {
                    if (Main.npc[(int)NPC.ai[1]].ai[1] == 3f && NPC.timeLeft > 10)
                        NPC.timeLeft = 10;

                    Vector2 cannonArmPosition = NPC.Center;
                    float cannonArmTargetX = Main.player[NPC.target].Center.X - cannonArmPosition.X;
                    float cannonArmTargetY = Main.player[NPC.target].Center.Y - cannonArmPosition.Y;
                    float cannonArmTargetDist = (float)Math.Sqrt(cannonArmTargetX * cannonArmTargetX + cannonArmTargetY * cannonArmTargetY);
                    NPC.rotation = (float)Math.Atan2(cannonArmTargetY, cannonArmTargetX) - MathHelper.PiOver2;

                    if (Main.netMode != NetmodeID.MultiplayerClient && !dontAttack)
                    {
                        // Fire rocket every 2 seconds (change this as each arm dies to fire more aggressively)
                        NPC.localAI[0] += 1f;
                        if (!laserAlive)
                            NPC.localAI[0] += 1f;
                        if (!viceAlive)
                            NPC.localAI[0] += 1f;
                        if (!sawAlive)
                            NPC.localAI[0] += 1f;

                        if (NPC.localAI[0] >= 120f)
                        {
                            SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                            NPC.localAI[0] = 0f;
                            int type = ProjectileID.RocketSkeleton;

                            float rocketSpeed = 10f;
                            cannonArmTargetDist = rocketSpeed / cannonArmTargetDist;
                            cannonArmTargetX *= cannonArmTargetDist;
                            cannonArmTargetY *= cannonArmTargetDist;

                            Vector2 rocketVelocity = new Vector2(cannonArmTargetX, cannonArmTargetY);
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), cannonArmPosition + rocketVelocity.SafeNormalize(Vector2.UnitY) * 40f, rocketVelocity, type, RocketDamage.CalculateMechDamage(), 0f, Main.myPlayer, NPC.target, 2f);
                            Main.projectile[proj].timeLeft = 540;
                        }
                    }
                }
                else
                {
                    Vector2 cannonSpreadArmPosition = NPC.Center;
                    float cannonSpreadArmTargetX = Main.player[NPC.target].Center.X - cannonSpreadArmPosition.X;
                    float cannonSpreadArmTargetY = Main.player[NPC.target].Center.Y - cannonSpreadArmPosition.Y;
                    NPC.rotation = (float)Math.Atan2(cannonSpreadArmTargetY, cannonSpreadArmTargetX) - MathHelper.PiOver2;

                    if (Main.netMode != NetmodeID.MultiplayerClient && !dontAttack)
                    {
                        // Fire rockets every 2 seconds (change this as each arm dies to fire more aggressively)
                        NPC.localAI[0] += 1f;
                        if (!laserAlive)
                            NPC.localAI[0] += 0.5f;
                        if (!viceAlive)
                            NPC.localAI[0] += 0.5f;
                        if (!sawAlive)
                            NPC.localAI[0] += 0.5f;

                        if (NPC.localAI[0] >= 180f)
                        {
                            SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                            NPC.localAI[0] = 0f;
                            int type = ProjectileID.RocketSkeleton;

                            float rocketSpeed = 10f;
                            Vector2 cannonSpreadTargetDist = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * rocketSpeed;
                            int numProj = 3;
                            float rotation = MathHelper.ToRadians(9);
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = cannonSpreadTargetDist.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + perturbedSpeed.SafeNormalize(Vector2.UnitY) * 40f, perturbedSpeed, type, RocketDamage.CalculateMechDamage(), 0f, Main.myPlayer, NPC.target, 2f);
                                Main.projectile[proj].timeLeft = 600;
                            }
                        }
                    }
                }

                return false;
            }
        }

        public class PrimeViceAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                // Direction
                NPC.spriteDirection = -(int)NPC.ai[0];

                // Where the vice should be in relation to the head
                Vector2 viceArmPosition = NPC.Center;
                float viceArmIdleXPos = Main.npc[(int)NPC.ai[1]].Center.X - 200f * NPC.ai[0] - viceArmPosition.X;
                float viceArmIdleYPos = Main.npc[(int)NPC.ai[1]].Center.Y + 230f - viceArmPosition.Y;
                float viceArmIdleDistance = (float)Math.Sqrt(viceArmIdleXPos * viceArmIdleXPos + viceArmIdleYPos * viceArmIdleYPos);

                // Return the vice to its proper location in relation to the head if it's too far away
                if (NPC.ai[2] != 99f)
                {
                    if (viceArmIdleDistance > 800f)
                        NPC.ai[2] = 99f;
                }
                else if (viceArmIdleDistance < 400f)
                    NPC.ai[2] = 0f;

                // Despawn if head is gone
                if (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)NPC.ai[1]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
                {
                    NPC.ai[2] += 10f;
                    if (NPC.ai[2] > 50f || !Main.dedServ)
                    {
                        NPC.life = -1;
                        NPC.HitEffect(0, 10.0);
                        NPC.active = false;
                    }
                }

                CalamityGlobalNPC.primeVice = NPC.whoAmI;

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

                // Return to the head
                if (NPC.ai[2] == 99f)
                {
                    float acceleration = death ? 0.385f : 0.25f;
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
                    if (death)
                        acceleration *= accelerationMult;

                    float topVelocity = acceleration * 100f;
                    float deceleration = death ? 0.6f : 0.8f;

                    if (NPC.position.Y > Main.npc[(int)NPC.ai[1]].position.Y + 20f)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y *= deceleration;

                        NPC.velocity.Y -= acceleration;

                        if (NPC.velocity.Y > topVelocity)
                            NPC.velocity.Y = topVelocity;
                    }
                    else if (NPC.position.Y < Main.npc[(int)NPC.ai[1]].position.Y - 20f)
                    {
                        if (NPC.velocity.Y < 0f)
                            NPC.velocity.Y *= deceleration;

                        NPC.velocity.Y += acceleration;

                        if (NPC.velocity.Y < -topVelocity)
                            NPC.velocity.Y = -topVelocity;
                    }

                    if (NPC.Center.X > Main.npc[(int)NPC.ai[1]].Center.X + 20f)
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X *= deceleration;

                        NPC.velocity.X -= acceleration * 2f;

                        if (NPC.velocity.X > topVelocity)
                            NPC.velocity.X = topVelocity;
                    }
                    if (NPC.Center.X < Main.npc[(int)NPC.ai[1]].Center.X - 20f)
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.velocity.X *= deceleration;

                        NPC.velocity.X += acceleration * 2f;

                        if (NPC.velocity.X < -topVelocity)
                            NPC.velocity.X = -topVelocity;
                    }
                }

                // Other phases
                else
                {
                    // Stay near the head
                    if (NPC.ai[2] == 0f || NPC.ai[2] == 3f)
                    {
                        // Despawn if head is despawning
                        if (Main.npc[(int)NPC.ai[1]].ai[1] == 3f && NPC.timeLeft > 10)
                            NPC.timeLeft = 10;

                        // Start charging after 10 seconds (change this as each arm dies)
                        NPC.ai[3] += 1f;
                        if (!cannonAlive)
                            NPC.ai[3] += 1f;
                        if (!laserAlive)
                            NPC.ai[3] += 1f;
                        if (!sawAlive)
                            NPC.ai[3] += 1f;

                        if (NPC.ai[3] >= (death ? 150f : 600f))
                        {
                            NPC.target = Main.npc[(int)NPC.ai[1]].target;
                            NPC.ai[2] += 1f;
                            NPC.ai[3] = 0f;
                            NPC.netUpdate = true;
                        }

                        float acceleration = death ? 0.385f : 0.25f;
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
                        if (death)
                            acceleration *= accelerationMult;

                        float topVelocity = acceleration * 100f;
                        float deceleration = death ? 0.6f : 0.8f;

                        if (NPC.position.Y > Main.npc[(int)NPC.ai[1]].position.Y + 100f)
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y *= deceleration;

                            NPC.velocity.Y -= acceleration;

                            if (NPC.velocity.Y > topVelocity)
                                NPC.velocity.Y = topVelocity;
                        }
                        else if (NPC.position.Y < Main.npc[(int)NPC.ai[1]].position.Y + 70f)
                        {
                            if (NPC.velocity.Y < 0f)
                                NPC.velocity.Y *= deceleration;

                            NPC.velocity.Y += acceleration;

                            if (NPC.velocity.Y < -topVelocity)
                                NPC.velocity.Y = -topVelocity;
                        }

                        if (NPC.Center.X > Main.npc[(int)NPC.ai[1]].Center.X + 160f)
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X *= deceleration;

                            NPC.velocity.X -= acceleration;

                            if (NPC.velocity.X > topVelocity)
                                NPC.velocity.X = topVelocity;
                        }
                        if (NPC.Center.X < Main.npc[(int)NPC.ai[1]].Center.X + 130f)
                        {
                            if (NPC.velocity.X < 0f)
                                NPC.velocity.X *= deceleration;

                            NPC.velocity.X += acceleration;

                            if (NPC.velocity.X < -topVelocity)
                                NPC.velocity.X = -topVelocity;
                        }

                        Vector2 viceArmReelbackCurrentPos = NPC.Center;
                        float viceArmReelbackXDest = Main.npc[(int)NPC.ai[1]].Center.X - 200f * NPC.ai[0] - viceArmReelbackCurrentPos.X;
                        float viceArmReelbackYDest = Main.npc[(int)NPC.ai[1]].position.Y + 230f - viceArmReelbackCurrentPos.Y;
                        NPC.rotation = (float)Math.Atan2(viceArmReelbackYDest, viceArmReelbackXDest) + MathHelper.PiOver2;
                        return false;
                    }

                    // Charge towards the player
                    if (NPC.ai[2] == 1f)
                    {
                        float deceleration = death ? 0.75f : 0.8f;
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y *= deceleration;

                        Vector2 viceArmChargePosition = NPC.Center;
                        float viceArmChargeTargetX = Main.npc[(int)NPC.ai[1]].Center.X - 280f * NPC.ai[0] - viceArmChargePosition.X;
                        float viceArmChargeTargetY = Main.npc[(int)NPC.ai[1]].position.Y + 230f - viceArmChargePosition.Y;
                        NPC.rotation = (float)Math.Atan2(viceArmChargeTargetY, viceArmChargeTargetX) + MathHelper.PiOver2;

                        NPC.velocity.X = (NPC.velocity.X * 5f + Main.npc[(int)NPC.ai[1]].velocity.X) / 6f;
                        NPC.velocity.X += 0.5f;

                        NPC.velocity.Y -= 0.5f;
                        if (NPC.velocity.Y < -12f)
                            NPC.velocity.Y = -12f;

                        if (NPC.position.Y < Main.npc[(int)NPC.ai[1]].position.Y - 280f)
                        {
                            float chargeVelocity = 16f;
                            if (!cannonAlive)
                                chargeVelocity += 1.5f;
                            if (!laserAlive)
                                chargeVelocity += 1.5f;
                            if (!sawAlive)
                                chargeVelocity += 1.5f;

                            NPC.ai[2] = 2f;
                            viceArmChargePosition = NPC.Center;
                            viceArmChargeTargetX = Main.player[NPC.target].Center.X - viceArmChargePosition.X;
                            viceArmChargeTargetY = Main.player[NPC.target].Center.Y - viceArmChargePosition.Y;
                            float viceArmChargeTargetDist = (float)Math.Sqrt(viceArmChargeTargetX * viceArmChargeTargetX + viceArmChargeTargetY * viceArmChargeTargetY);
                            viceArmChargeTargetDist = chargeVelocity / viceArmChargeTargetDist;
                            NPC.velocity.X = viceArmChargeTargetX * viceArmChargeTargetDist;
                            NPC.velocity.Y = viceArmChargeTargetY * viceArmChargeTargetDist;
                            NPC.netUpdate = true;
                        }
                    }

                    // Charge 4 times (more if arms are dead)
                    else if (NPC.ai[2] == 2f)
                    {
                        if (NPC.position.Y > Main.player[NPC.target].position.Y || NPC.velocity.Y < 0f)
                        {
                            float chargeAmt = 4f;
                            if (!cannonAlive)
                                chargeAmt += 1f;
                            if (!laserAlive)
                                chargeAmt += 1f;
                            if (!sawAlive)
                                chargeAmt += 1f;

                            if (NPC.ai[3] >= chargeAmt)
                            {
                                // Return to head
                                NPC.ai[2] = 3f;
                                NPC.ai[3] = 0f;
                                return false;
                            }

                            NPC.ai[2] = 1f;
                            NPC.ai[3] += 1f;
                        }
                    }

                    // Different type of charge
                    else if (NPC.ai[2] == 4f)
                    {
                        Vector2 viceArmOtherChargePosition = NPC.Center;
                        float viceArmOtherChargeTargetX = Main.npc[(int)NPC.ai[1]].Center.X - 200f * NPC.ai[0] - viceArmOtherChargePosition.X;
                        float viceArmOtherChargeTargetY = Main.npc[(int)NPC.ai[1]].position.Y + 230f - viceArmOtherChargePosition.Y;
                        NPC.rotation = (float)Math.Atan2(viceArmOtherChargeTargetY, viceArmOtherChargeTargetX) + MathHelper.PiOver2;

                        NPC.velocity.Y = (NPC.velocity.Y * 5f + Main.npc[(int)NPC.ai[1]].velocity.Y) / 6f;

                        NPC.velocity.X += 0.5f;
                        if (NPC.velocity.X > 12f)
                            NPC.velocity.X = 12f;

                        if (NPC.Center.X < Main.npc[(int)NPC.ai[1]].Center.X - 500f || NPC.Center.X > Main.npc[(int)NPC.ai[1]].Center.X + 500f)
                        {
                            float chargeVelocity = 14f;
                            if (!cannonAlive)
                                chargeVelocity += 1f;
                            if (!laserAlive)
                                chargeVelocity += 1f;
                            if (!sawAlive)
                                chargeVelocity += 1f;

                            NPC.ai[2] = 5f;
                            viceArmOtherChargePosition = NPC.Center;
                            viceArmOtherChargeTargetX = Main.player[NPC.target].Center.X - viceArmOtherChargePosition.X;
                            viceArmOtherChargeTargetY = Main.player[NPC.target].Center.Y - viceArmOtherChargePosition.Y;
                            float viceArmOtherChargeTargetDist = (float)Math.Sqrt(viceArmOtherChargeTargetX * viceArmOtherChargeTargetX + viceArmOtherChargeTargetY * viceArmOtherChargeTargetY);
                            viceArmOtherChargeTargetDist = chargeVelocity / viceArmOtherChargeTargetDist;
                            NPC.velocity.X = viceArmOtherChargeTargetX * viceArmOtherChargeTargetDist;
                            NPC.velocity.Y = viceArmOtherChargeTargetY * viceArmOtherChargeTargetDist;
                            NPC.netUpdate = true;
                        }
                    }

                    // Charge 4 times (more if arms are dead)
                    else if (NPC.ai[2] == 5f && NPC.Center.X < Main.player[NPC.target].Center.X - 100f)
                    {
                        float chargeAmt = 4f;
                        if (!cannonAlive)
                            chargeAmt += 1f;
                        if (!laserAlive)
                            chargeAmt += 1f;
                        if (!sawAlive)
                            chargeAmt += 1f;

                        if (NPC.ai[3] >= chargeAmt)
                        {
                            // Return to head
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                            return false;
                        }

                        NPC.ai[2] = 4f;
                        NPC.ai[3] += 1f;
                    }
                }

                return false;
            }
        }

        public class PrimeSawAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                // Get a target
                if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                Vector2 sawArmLocation = NPC.Center;
                float sawArmIdleXPos = Main.npc[(int)NPC.ai[1]].Center.X - 200f * NPC.ai[0] - sawArmLocation.X;
                float sawArmIdleYPos = Main.npc[(int)NPC.ai[1]].Center.Y + 230f - sawArmLocation.Y;
                float sawArmIdleDistance = (float)Math.Sqrt(sawArmIdleXPos * sawArmIdleXPos + sawArmIdleYPos * sawArmIdleYPos);

                if (NPC.ai[2] != 99f)
                {
                    if (sawArmIdleDistance > 800f)
                        NPC.ai[2] = 99f;
                }
                else if (sawArmIdleDistance < 400f)
                    NPC.ai[2] = 0f;

                NPC.spriteDirection = -(int)NPC.ai[0];

                if (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)NPC.ai[1]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
                {
                    NPC.ai[2] += 10f;
                    if (NPC.ai[2] > 50f || !Main.dedServ)
                    {
                        NPC.life = -1;
                        NPC.HitEffect(0, 10.0);
                        NPC.active = false;
                    }
                }

                CalamityGlobalNPC.primeSaw = NPC.whoAmI;

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

                if (NPC.ai[2] == 99f)
                {
                    float acceleration = death ? 0.385f : 0.25f;
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
                    if (death)
                        acceleration *= accelerationMult;

                    float topVelocity = acceleration * 100f;
                    float deceleration = death ? 0.6f : 0.8f;

                    if (NPC.position.Y > Main.npc[(int)NPC.ai[1]].position.Y + 20f)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y *= deceleration;

                        NPC.velocity.Y -= acceleration;

                        if (NPC.velocity.Y > topVelocity)
                            NPC.velocity.Y = topVelocity;
                    }
                    else if (NPC.position.Y < Main.npc[(int)NPC.ai[1]].position.Y - 20f)
                    {
                        if (NPC.velocity.Y < 0f)
                            NPC.velocity.Y *= deceleration;

                        NPC.velocity.Y += acceleration;

                        if (NPC.velocity.Y < -topVelocity)
                            NPC.velocity.Y = -topVelocity;
                    }

                    if (NPC.Center.X > Main.npc[(int)NPC.ai[1]].Center.X + 20f)
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X *= deceleration;

                        NPC.velocity.X -= acceleration * 2f;

                        if (NPC.velocity.X > topVelocity)
                            NPC.velocity.X = topVelocity;
                    }
                    if (NPC.Center.X < Main.npc[(int)NPC.ai[1]].Center.X - 20f)
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.velocity.X *= deceleration;

                        NPC.velocity.X += acceleration * 2f;

                        if (NPC.velocity.X < -topVelocity)
                            NPC.velocity.X = -topVelocity;
                    }
                }
                else
                {
                    if (NPC.ai[2] == 0f || NPC.ai[2] == 3f)
                    {
                        if (Main.npc[(int)NPC.ai[1]].ai[1] == 3f && NPC.timeLeft > 10)
                            NPC.timeLeft = 10;

                        // Start charging after 3 seconds (change this as each arm dies)
                        NPC.ai[3] += 1f;
                        if (!cannonAlive)
                            NPC.ai[3] += 1f;
                        if (!laserAlive)
                            NPC.ai[3] += 1f;
                        if (!viceAlive)
                            NPC.ai[3] += 1f;

                        if (NPC.ai[3] >= (death ? 90f : 180f))
                        {
                            NPC.target = Main.npc[(int)NPC.ai[1]].target;
                            NPC.ai[2] += 1f;
                            NPC.ai[3] = 0f;
                            NPC.netUpdate = true;
                        }

                        float acceleration = death ? 0.385f : 0.25f;
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
                        if (death)
                            acceleration *= accelerationMult;

                        float topVelocity = acceleration * 100f;
                        float deceleration = death ? 0.6f : 0.8f;

                        if (NPC.position.Y > Main.npc[(int)NPC.ai[1]].position.Y + 100f)
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y *= deceleration;

                            NPC.velocity.Y -= acceleration;

                            if (NPC.velocity.Y > topVelocity)
                                NPC.velocity.Y = topVelocity;
                        }
                        else if (NPC.position.Y < Main.npc[(int)NPC.ai[1]].position.Y + 70f)
                        {
                            if (NPC.velocity.Y < 0f)
                                NPC.velocity.Y *= deceleration;

                            NPC.velocity.Y += acceleration;

                            if (NPC.velocity.Y < -topVelocity)
                                NPC.velocity.Y = -topVelocity;
                        }

                        if (NPC.Center.X > Main.npc[(int)NPC.ai[1]].Center.X - 130f)
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X *= deceleration;

                            NPC.velocity.X -= acceleration * 1.5f;

                            if (NPC.velocity.X > topVelocity)
                                NPC.velocity.X = topVelocity;
                        }
                        if (NPC.Center.X < Main.npc[(int)NPC.ai[1]].Center.X - 160f)
                        {
                            if (NPC.velocity.X < 0f)
                                NPC.velocity.X *= deceleration;

                            NPC.velocity.X += acceleration * 1.5f;

                            if (NPC.velocity.X < -topVelocity)
                                NPC.velocity.X = -topVelocity;
                        }

                        Vector2 sawArmReelbackCurrentPos = NPC.Center;
                        float sawArmReelbackXDest = Main.npc[(int)NPC.ai[1]].Center.X - 200f * NPC.ai[0] - sawArmReelbackCurrentPos.X;
                        float sawArmReelbackYDest = Main.npc[(int)NPC.ai[1]].position.Y + 230f - sawArmReelbackCurrentPos.Y;
                        NPC.rotation = (float)Math.Atan2(sawArmReelbackYDest, sawArmReelbackXDest) + MathHelper.PiOver2;
                        return false;
                    }

                    if (NPC.ai[2] == 1f)
                    {
                        Vector2 sawArmChargePos = NPC.Center;
                        float sawArmChargeTargetX = Main.npc[(int)NPC.ai[1]].Center.X - 200f * NPC.ai[0] - sawArmChargePos.X;
                        float sawArmChargeTargetY = Main.npc[(int)NPC.ai[1]].position.Y + 230f - sawArmChargePos.Y;
                        NPC.rotation = (float)Math.Atan2(sawArmChargeTargetY, sawArmChargeTargetX) + MathHelper.PiOver2;

                        float deceleration = death ? 0.875f : 0.9f;
                        NPC.velocity.X *= deceleration;
                        NPC.velocity.Y -= 0.5f;
                        if (NPC.velocity.Y < -12f)
                            NPC.velocity.Y = -12f;

                        if (NPC.position.Y < Main.npc[(int)NPC.ai[1]].position.Y - 200f)
                        {
                            float chargeVelocity = 22f;
                            if (!cannonAlive)
                                chargeVelocity += 1.5f;
                            if (!laserAlive)
                                chargeVelocity += 1.5f;
                            if (!viceAlive)
                                chargeVelocity += 1.5f;

                            NPC.ai[2] = 2f;
                            sawArmChargePos = NPC.Center;
                            sawArmChargeTargetX = Main.player[NPC.target].Center.X - sawArmChargePos.X;
                            sawArmChargeTargetY = Main.player[NPC.target].Center.Y - sawArmChargePos.Y;
                            float sawArmChargeTargetDist = (float)Math.Sqrt(sawArmChargeTargetX * sawArmChargeTargetX + sawArmChargeTargetY * sawArmChargeTargetY);
                            sawArmChargeTargetDist = chargeVelocity / sawArmChargeTargetDist;
                            NPC.velocity.X = sawArmChargeTargetX * sawArmChargeTargetDist;
                            NPC.velocity.Y = sawArmChargeTargetY * sawArmChargeTargetDist;
                            NPC.netUpdate = true;
                        }
                    }

                    else if (NPC.ai[2] == 2f)
                    {
                        if (NPC.position.Y > Main.player[NPC.target].position.Y || NPC.velocity.Y < 0f)
                            NPC.ai[2] = 3f;
                    }

                    else
                    {
                        if (NPC.ai[2] == 4f)
                        {
                            float chargeVelocity = 11f;
                            if (!cannonAlive)
                                chargeVelocity += 1.5f;
                            if (!laserAlive)
                                chargeVelocity += 1.5f;
                            if (!viceAlive)
                                chargeVelocity += 1.5f;
                            if (death)
                                chargeVelocity *= 1.25f;

                            Vector2 sawArmOtherChargePos = NPC.Center;
                            float sawArmOtherChargeTargetX = Main.player[NPC.target].Center.X - sawArmOtherChargePos.X;
                            float sawArmOtherChargeTargetY = Main.player[NPC.target].Center.Y - sawArmOtherChargePos.Y;
                            float sawArmOtherChargeTargetDist = (float)Math.Sqrt(sawArmOtherChargeTargetX * sawArmOtherChargeTargetX + sawArmOtherChargeTargetY * sawArmOtherChargeTargetY);
                            sawArmOtherChargeTargetDist = chargeVelocity / sawArmOtherChargeTargetDist;
                            sawArmOtherChargeTargetX *= sawArmOtherChargeTargetDist;
                            sawArmOtherChargeTargetY *= sawArmOtherChargeTargetDist;

                            float acceleration = death ? 0.125f : 0.08f;
                            float deceleration = death ? 0.6f : 0.8f;

                            if (NPC.velocity.X > sawArmOtherChargeTargetX)
                            {
                                if (NPC.velocity.X > 0f)
                                    NPC.velocity.X *= deceleration;

                                NPC.velocity.X -= acceleration;
                            }
                            if (NPC.velocity.X < sawArmOtherChargeTargetX)
                            {
                                if (NPC.velocity.X < 0f)
                                    NPC.velocity.X *= deceleration;

                                NPC.velocity.X += acceleration;
                            }
                            if (NPC.velocity.Y > sawArmOtherChargeTargetY)
                            {
                                if (NPC.velocity.Y > 0f)
                                    NPC.velocity.Y *= deceleration;

                                NPC.velocity.Y -= acceleration;
                            }
                            if (NPC.velocity.Y < sawArmOtherChargeTargetY)
                            {
                                if (NPC.velocity.Y < 0f)
                                    NPC.velocity.Y *= deceleration;

                                NPC.velocity.Y += acceleration;
                            }

                            NPC.ai[3] += 1f;
                            if (NPC.justHit)
                                NPC.ai[3] += 2f;

                            if (NPC.ai[3] >= 600f)
                            {
                                NPC.ai[2] = 0f;
                                NPC.ai[3] = 0f;
                                NPC.netUpdate = true;
                            }

                            sawArmOtherChargePos = NPC.Center;
                            sawArmOtherChargeTargetX = Main.npc[(int)NPC.ai[1]].Center.X - 200f * NPC.ai[0] - sawArmOtherChargePos.X;
                            sawArmOtherChargeTargetY = Main.npc[(int)NPC.ai[1]].position.Y + 230f - sawArmOtherChargePos.Y;
                            NPC.rotation = (float)Math.Atan2(sawArmOtherChargeTargetY, sawArmOtherChargeTargetX) + MathHelper.PiOver2;
                            return false;
                        }

                        if (NPC.ai[2] == 5f && ((NPC.velocity.X > 0f && NPC.Center.X > Main.player[NPC.target].Center.X) || (NPC.velocity.X < 0f && NPC.Center.X < Main.player[NPC.target].Center.X)))
                            NPC.ai[2] = 0f;
                    }
                }

                return false;
            }
        }
    }
}
