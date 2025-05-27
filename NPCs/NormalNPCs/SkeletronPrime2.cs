using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.DataStructures;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class SkeletronPrime2 : ModNPC
    {
        public override string BossHeadTexture => $"Terraria/Images/NPC_Head_Boss_18";

        public static Asset<Texture2D> EyeTexture;

        public const int BombTimeLeft = 600;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
            if (!Main.dedServ)
            {
                EyeTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SkeletronPrime2HeadGlow");
            }
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.aiStyle = NPCAIStyleID.SkeletronPrimeHead;
            NPC.GetNPCDamage();
            NPC.DR_NERD(0.2f);

            NPC.width = 80;
            NPC.height = 102;
            if (Main.tenthAnniversaryWorld)
                NPC.scale *= 0.5f;
            if (Main.getGoodWorld)
                NPC.scale *= 1.1f;

            NPC.defense = 24;

            NPC.lifeMax = 28000;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);

            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.value = 200000f;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToSickness = false;
            AnimationType = NPCID.SkeletronPrime;
            Music = MusicID.Boss3;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = (NPC.ai[1] == 1f || NPC.ai[1] == 2f) ? NPC.rotation : 0f;
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Spawn arms
            if (calamityGlobalNPC.newAI[1] == 0f)
            {
                calamityGlobalNPC.newAI[1] = 1f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // This head owns the Cannon and the Vice in Master Mode
                    int arm = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PrimeCannon, NPC.whoAmI);
                    Main.npc[arm].ai[0] = -1f;
                    Main.npc[arm].ai[1] = NPC.whoAmI;
                    Main.npc[arm].target = NPC.target;
                    Main.npc[arm].netUpdate = true;

                    arm = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.PrimeVice, NPC.whoAmI);
                    Main.npc[arm].ai[0] = -1f;
                    Main.npc[arm].ai[1] = NPC.whoAmI;
                    Main.npc[arm].target = NPC.target;
                    Main.npc[arm].ai[3] = 150f;
                    Main.npc[arm].netUpdate = true;
                }

                NPC.SyncExtraAI();
            }

            if (!Main.npc[(int)NPC.ai[0]].active || Main.npc[(int)NPC.ai[0]].aiStyle != NPCAIStyleID.SkeletronPrimeHead)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
            }
            else
            {
                // Link the HP of both heads
                if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                    NPC.life = Main.npc[(int)NPC.ai[0]].life;

                // Push away from the lead head if too close, pull closer if too far, if Mechdusa isn't real
                if (!NPC.IsMechQueenUp)
                {
                    float pushVelocity = 0.25f;
                    if (Vector2.Distance(NPC.Center, Main.npc[(int)NPC.ai[0]].Center) < 80f * NPC.scale)
                    {
                        if (NPC.position.X < Main.npc[(int)NPC.ai[0]].position.X)
                            NPC.velocity.X -= pushVelocity;
                        else
                            NPC.velocity.X += pushVelocity;

                        if (NPC.position.Y < Main.npc[(int)NPC.ai[0]].position.Y)
                            NPC.velocity.Y -= pushVelocity;
                        else
                            NPC.velocity.Y += pushVelocity;
                    }
                    else if (Vector2.Distance(NPC.Center, Main.npc[(int)NPC.ai[0]].Center) > 240f * NPC.scale)
                    {
                        if (NPC.position.X < Main.npc[(int)NPC.ai[0]].position.X)
                            NPC.velocity.X += pushVelocity;
                        else
                            NPC.velocity.X -= pushVelocity;

                        if (NPC.position.Y < Main.npc[(int)NPC.ai[0]].position.Y)
                            NPC.velocity.Y += pushVelocity;
                        else
                            NPC.velocity.Y -= pushVelocity;
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
            NPC.chaseable = allArmsDead;

            NPC.defense = NPC.defDefense;

            // Phases
            bool phase2 = lifeRatio < 0.66f;
            bool phase3 = lifeRatio < 0.33f;
            bool spawnSpazmatism = lifeRatio < 0.5f && !bossRush && NPC.localAI[2] == 0f;

            // Spawn Spazmatism in Master Mode (just like Oblivion from Avalon)
            if (spawnSpazmatism)
            {
                Player spazmatismSpawnPlayer = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];
                SoundEngine.PlaySound(SoundID.Roar, spazmatismSpawnPlayer.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(spazmatismSpawnPlayer.whoAmI, NPCID.Spazmatism);

                NPC.localAI[2] = 1f;
                NPC.SyncVanillaLocalAI();
            }

            // Despawn
            if (Main.npc[(int)NPC.ai[0]].ai[1] == 3f)
                NPC.ai[1] = 3f;

            // Activate daytime enrage
            if (Main.IsItDay() && !bossRush && NPC.ai[1] != 3f && NPC.ai[1] != 2f)
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

            // Adjust slowing debuff immunity
            bool immuneToSlowingDebuffs = NPC.ai[1] == 5f;
            NPC.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<TimeDistortion>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<GalvanicCorrosion>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
            NPC.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
            NPC.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

            bool normalLaserRotation = NPC.localAI[1] % 2f == 0f;

            // Prevents cheap hits
            bool canUseAttackInMaster = NPC.position.Y < Main.player[Main.npc[(int)NPC.ai[0]].target].position.Y - 350f;

            // Float near player
            if (NPC.ai[1] == 0f || NPC.ai[1] == 4f)
            {
                // Avoid unfair bullshit
                NPC.damage = 0;

                // Start other phases; if arms are dead, start with spin phase
                bool otherHeadChargingOrSpinning = Main.npc[(int)NPC.ai[0]].ai[1] == 5f || Main.npc[(int)NPC.ai[0]].ai[1] == 1f;

                // Start spin phase after 1.875 seconds
                NPC.ai[2] += phase3 ? 1.2f : 0.8f;
                if (NPC.ai[2] >= (90f - (death ? 15f * (1f - lifeRatio) : 0f)) && (!otherHeadChargingOrSpinning || phase3) && canUseAttackInMaster)
                {
                    bool shouldSpinAround = NPC.ai[1] == 4f && NPC.position.Y < Main.player[Main.npc[(int)NPC.ai[0]].target].position.Y - 400f &&
                        Vector2.Distance(Main.player[Main.npc[(int)NPC.ai[0]].target].Center, NPC.Center) < 600f && Vector2.Distance(Main.player[Main.npc[(int)NPC.ai[0]].target].Center, NPC.Center) > 400f;

                    bool shouldCharge = !phase2 && !allArmsDead && !CalamityWorld.LegendaryMode;
                    if (shouldCharge)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[1] = 1f;
                        NPC.netUpdate = true;
                    }
                    else if (shouldSpinAround || NPC.ai[1] != 4f)
                    {
                        if (shouldSpinAround)
                        {
                            NPC.localAI[3] = 300f;
                            NPC.SyncVanillaLocalAI();
                        }

                        NPC.ai[2] = 0f;
                        NPC.ai[1] = shouldSpinAround ? 5f : 1f;
                        NPC.netUpdate = true;
                    }
                }

                if (NPC.IsMechQueenUp)
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.X / 15f * 0.5f, 0.75f);
                else
                    NPC.rotation = NPC.velocity.X / 15f;

                float acceleration = (bossRush ? 0.2f : 0.125f) + (death ? 0.05f * (1f - lifeRatio) : 0f);
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
                acceleration *= accelerationMult;

                float topVelocity = acceleration * 100f;
                float deceleration = 0.7f;

                float headDecelerationUpDist = 0f;
                float headDecelerationDownDist = 0f;
                float headDecelerationHorizontalDist = 0f;
                int headHorizontalDirection = ((!(Main.player[Main.npc[(int)NPC.ai[0]].target].Center.X < NPC.Center.X)) ? 1 : (-1));
                if (NPC.IsMechQueenUp)
                {
                    headDecelerationHorizontalDist = -150f * (float)headHorizontalDirection;
                    headDecelerationUpDist = 50f;
                    headDecelerationDownDist = 50f;
                }

                if (NPC.position.Y > Main.player[Main.npc[(int)NPC.ai[0]].target].position.Y - (400f + headDecelerationUpDist))
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= deceleration;

                    NPC.velocity.Y -= acceleration;

                    if (NPC.velocity.Y > topVelocity)
                        NPC.velocity.Y = topVelocity;
                }
                else if (NPC.position.Y < Main.player[Main.npc[(int)NPC.ai[0]].target].position.Y - (450f + headDecelerationDownDist))
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= deceleration;

                    NPC.velocity.Y += acceleration;

                    if (NPC.velocity.Y < -topVelocity)
                        NPC.velocity.Y = -topVelocity;
                }

                if (NPC.Center.X > Main.player[Main.npc[(int)NPC.ai[0]].target].Center.X + (400f + headDecelerationHorizontalDist))
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X *= deceleration;

                    NPC.velocity.X -= acceleration;

                    if (NPC.velocity.X > topVelocity)
                        NPC.velocity.X = topVelocity;
                }
                if (NPC.Center.X < Main.player[Main.npc[(int)NPC.ai[0]].target].Center.X - (400f + headDecelerationHorizontalDist))
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
                    NPC.defense *= 2;
                    NPC.damage = NPC.defDamage * 2;

                    calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = true;

                    if (phase2 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[0] += 1f;
                        if (NPC.localAI[0] >= 60f)
                        {
                            NPC.localAI[0] = 0f;

                            int totalProjectiles = bossRush ? 20 : death ? 12 : 10;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = ProjectileID.FrostBeam;
                            int damage = NPC.GetProjectileDamage(type);

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

                            float velocity = 4.5f;
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 laserFireDirection = spinningPoint.RotatedBy(radians * k);
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + laserFireDirection.SafeNormalize(Vector2.UnitY) * 140f, laserFireDirection, type, damage, 0f, Main.myPlayer, 1f, 0f);
                                Main.projectile[proj].timeLeft = 600;
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
                        NPC.ai[2] = 0f;
                        NPC.ai[1] = 4f;
                        NPC.localAI[0] = 0f;
                    }

                    if (NPC.IsMechQueenUp)
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.X / 15f * 0.5f, 0.75f);
                    else
                        NPC.rotation += NPC.direction * 0.3f;

                    Vector2 headPosition = NPC.Center;
                    float headTargetX = Main.player[Main.npc[(int)NPC.ai[0]].target].Center.X - headPosition.X;
                    float headTargetY = Main.player[Main.npc[(int)NPC.ai[0]].target].Center.Y - headPosition.Y;
                    float headTargetDistance = (float)Math.Sqrt(headTargetX * headTargetX + headTargetY * headTargetY);

                    float speed = bossRush ? 12f : 8f;
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
                        float mechdusaAccelMult = Vector2.Distance(NPC.Center, Main.player[Main.npc[(int)NPC.ai[0]].target].Center);
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
                    float enragedHeadTargetX = Main.player[Main.npc[(int)NPC.ai[0]].target].Center.X - enragedHeadPosition.X;
                    float enragedHeadTargetY = Main.player[Main.npc[(int)NPC.ai[0]].target].Center.Y - enragedHeadPosition.Y;
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
                            if (Collision.CanHit(headCenter, 1, 1, Main.player[Main.npc[(int)NPC.ai[0]].target].position, Main.player[Main.npc[(int)NPC.ai[0]].target].width, Main.player[Main.npc[(int)NPC.ai[0]].target].height))
                            {
                                enragedHeadSpeed = 7f;
                                float enragedHeadSkullTargetX = Main.player[Main.npc[(int)NPC.ai[0]].target].Center.X - headCenter.X + Main.rand.Next(-20, 21);
                                float enragedHeadSkullTargetY = Main.player[Main.npc[(int)NPC.ai[0]].target].Center.Y - headCenter.Y + Main.rand.Next(-20, 21);
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
                                int enragedSkulls = Projectile.NewProjectile(NPC.GetSource_FromAI(), headCenter.X, headCenter.Y, enragedHeadSkullTargetX, enragedHeadSkullTargetY, type, 250, 0f, Main.myPlayer, -1f, 0f);
                                Main.projectile[enragedSkulls].timeLeft = 300;
                            }
                        }
                    }
                }

                // Despawning
                if (NPC.ai[1] == 3f)
                {
                    // Avoid unfair bullshit
                    NPC.damage = 0;

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
                    // Avoid unfair bullshit
                    NPC.damage = 0;

                    NPC.ai[2] += 1f;

                    NPC.rotation = NPC.velocity.X / 50f;

                    float bombSpawnDivisor = bossRush ? 14f : death ? 22f - (float)Math.Round(5f * (1f - lifeRatio)) : 22f;
                    float totalBombs = 6f;
                    int bombSpread = bossRush ? 250 : death ? 125 : 100;

                    // Spin for about 3 seconds
                    float spinVelocity = 24f;
                    if (NPC.ai[2] == 2f)
                    {
                        // Play angry noise
                        SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);

                        // Set spin direction
                        if (Main.player[Main.npc[(int)NPC.ai[0]].target].velocity.X > 0f)
                            calamityGlobalNPC.newAI[0] = 1f;
                        else if (Main.player[Main.npc[(int)NPC.ai[0]].target].velocity.X < 0f)
                            calamityGlobalNPC.newAI[0] = -1f;
                        else
                            calamityGlobalNPC.newAI[0] = Main.player[Main.npc[(int)NPC.ai[0]].target].direction;

                        // Set spin velocity
                        NPC.velocity.X = MathHelper.Pi * NPC.localAI[3] / spinVelocity;
                        NPC.velocity *= -calamityGlobalNPC.newAI[0];
                        NPC.SyncExtraAI();
                        NPC.netUpdate = true;
                    }

                    // Maintain velocity and spit homing bombs
                    else if (NPC.ai[2] > 2f)
                    {
                        NPC.velocity = NPC.velocity.RotatedBy(MathHelper.Pi / spinVelocity * -calamityGlobalNPC.newAI[0]);
                        if (NPC.ai[2] == 3f)
                            NPC.velocity *= 0.6f;

                        if (NPC.ai[2] % bombSpawnDivisor == 0f)
                        {
                            NPC.localAI[0] += 1f;

                            if (Vector2.Distance(Main.player[Main.npc[(int)NPC.ai[0]].target].Center, NPC.Center) > 64f)
                            {
                                SoundEngine.PlaySound(SoundID.Item61, NPC.Center);
                                CreateParticles(NPC, new Vector2(NPC.Center.X + Main.rand.Next(NPC.width / 2), NPC.Center.Y + 4f));

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 headCenter = NPC.Center;
                                    float enragedHeadSpeed = 6f + (death ? 2f * (1f - lifeRatio) : 0f);
                                    float enragedHeadBombTargetX = Main.player[Main.npc[(int)NPC.ai[0]].target].Center.X - headCenter.X + Main.rand.Next(-20, 21);
                                    float enragedHeadBombTargetY = Main.player[Main.npc[(int)NPC.ai[0]].target].Center.Y - headCenter.Y + Main.rand.Next(-20, 21);
                                    float enragedHeadBombTargetDist = (float)Math.Sqrt(enragedHeadBombTargetX * enragedHeadBombTargetX + enragedHeadBombTargetY * enragedHeadBombTargetY);
                                    enragedHeadBombTargetDist = enragedHeadSpeed / enragedHeadBombTargetDist;
                                    enragedHeadBombTargetX *= enragedHeadBombTargetDist;
                                    enragedHeadBombTargetY *= enragedHeadBombTargetDist;

                                    Vector2 value = new Vector2(enragedHeadBombTargetX + Main.rand.Next(-bombSpread, bombSpread + 1) * 0.01f, enragedHeadBombTargetY + Main.rand.Next(-bombSpread, bombSpread + 1) * 0.01f).SafeNormalize(Vector2.UnitY);
                                    value *= enragedHeadSpeed;
                                    enragedHeadBombTargetX = value.X;
                                    enragedHeadBombTargetY = value.Y;

                                    int type = ProjectileID.BombSkeletronPrime;
                                    int damage = NPC.GetProjectileDamage(type);

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

                                    int enragedBombs = Projectile.NewProjectile(NPC.GetSource_FromAI(), headCenter.X, headCenter.Y + 30f, enragedHeadBombTargetX, enragedHeadBombTargetY, type, damage, 0f, Main.myPlayer, -1f);
                                    Main.projectile[enragedBombs].timeLeft = BombTimeLeft;
                                    Main.projectile[enragedBombs].tileCollide = false;
                                }
                            }

                            // Go to floating phase, or spinning phase if in phase 2
                            if (NPC.localAI[0] >= totalBombs)
                            {
                                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY);

                                // Fly overhead and spit spreads of bombs into the air if on low health
                                NPC.ai[1] = phase3 ? 6f : 1f;
                                NPC.ai[2] = 0f;
                                NPC.localAI[3] = 0f;
                                NPC.localAI[0] = 0f;
                                calamityGlobalNPC.newAI[0] = 0f;
                                NPC.SyncVanillaLocalAI();
                                NPC.SyncExtraAI();
                                NPC.netUpdate = true;
                            }
                        }
                    }
                }

                // Fly overhead and spit bombs
                if (NPC.ai[1] == 6f)
                {
                    // Avoid unfair bullshit
                    NPC.damage = 0;

                    NPC.rotation = NPC.velocity.X / 15f;

                    float flightVelocity = bossRush ? 32f : death ? 28f : 24f;
                    float flightAcceleration = bossRush ? 1.28f : death ? 1.12f : 0.96f;

                    Vector2 destination = new Vector2(Main.player[Main.npc[(int)NPC.ai[0]].target].Center.X, Main.player[Main.npc[(int)NPC.ai[0]].target].Center.Y - 500f);
                    NPC.SimpleFlyMovement((destination - NPC.Center).SafeNormalize(Vector2.UnitY) * flightVelocity, flightAcceleration);

                    // Spit bombs and then go to floating phase
                    NPC.localAI[3] += 1f;
                    if (Vector2.Distance(NPC.Center, destination) < 160f || NPC.ai[2] > 0f || NPC.localAI[3] > 120f)
                    {
                        float bombSpawnDivisor = death ? 50f : 60f;
                        float totalBombSpreads = 2f;
                        NPC.ai[2] += 1f;
                        if (NPC.ai[2] % bombSpawnDivisor == 0f)
                        {
                            NPC.localAI[0] += 1f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int totalProjectiles = bossRush ? 24 : 12;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                int type = ProjectileID.BombSkeletronPrime;
                                int damage = NPC.GetProjectileDamage(type);

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

                                float velocity = 12f;
                                double angleA = radians * 0.5;
                                double angleB = MathHelper.ToRadians(90f) - angleA;
                                float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                                Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                                Vector2 upwardVelocity = Vector2.UnitY * velocity;
                                for (int k = 0; k < totalProjectiles; k++)
                                {
                                    Vector2 bombVelocity = spinningPoint.RotatedBy(radians * k);
                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitY * 30f + bombVelocity.SafeNormalize(Vector2.UnitY) * 15f, bombVelocity - upwardVelocity, type, damage, 0f, Main.myPlayer, -2f);
                                    Main.projectile[proj].timeLeft = BombTimeLeft;
                                    Main.projectile[proj].tileCollide = false;
                                }
                                NPC.localAI[1] += 1f;
                            }

                            SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                            CreateParticles(NPC, new Vector2(NPC.Center.X, NPC.Center.Y + 4f), 5);

                            if (NPC.localAI[0] >= totalBombSpreads)
                            {
                                NPC.ai[1] = 0f;
                                NPC.ai[2] = 0f;
                                NPC.localAI[3] = 0f;
                                calamityGlobalNPC.newAI[0] = 0f;
                                NPC.localAI[0] = 0f;
                                NPC.SyncVanillaLocalAI();
                                NPC.SyncExtraAI();
                                NPC.netUpdate = true;
                            }
                        }
                    }
                }
            }
        }

        private void CreateParticles(NPC npc, Vector2 position, int amountMultiplier = 1)
        {
            int firstDustCloudParticleAmount = 30 * amountMultiplier;
            int secondDustCloudParticleAmount = 20 * amountMultiplier;
            int goreAmount = 2 * amountMultiplier;

            for (int dustIndex = 0; dustIndex < firstDustCloudParticleAmount; dustIndex++)
            {
                int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int dustIndex = 0; dustIndex < secondDustCloudParticleAmount; dustIndex++)
            {
                int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            for (int goreIndex = 0; goreIndex < goreAmount; goreIndex++)
            {
                float goreVelocityMultiplier = 0.4f;
                if (goreIndex >= goreAmount / 2)
                    goreVelocityMultiplier = 0.8f;

                int gore = Gore.NewGore(npc.GetSource_FromAI(), npc.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[gore].velocity *= goreVelocityMultiplier;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
                gore = Gore.NewGore(npc.GetSource_FromAI(), npc.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[gore].velocity *= goreVelocityMultiplier;
                Main.gore[gore].velocity.X -= 1f;
                Main.gore[gore].velocity.Y += 1f;
                gore = Gore.NewGore(npc.GetSource_FromAI(), npc.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[gore].velocity *= goreVelocityMultiplier;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y -= 1f;
                gore = Gore.NewGore(npc.GetSource_FromAI(), npc.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[gore].velocity *= goreVelocityMultiplier;
                Main.gore[gore].velocity.X -= 1f;
                Main.gore[gore].velocity.Y -= 1f;
            }
        }

        public override void BossLoot(ref string name, ref int potionType) => potionType = ItemID.GreaterHealingPotion;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override bool CheckActive() => false;

        public override bool CheckDead()
        {
            // Kill the lead head if he's still alive when this head dies
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.active && nPC.type == NPCID.SkeletronPrime && nPC.life > 0)
                {
                    nPC.life = 0;
                    nPC.HitEffect();
                    nPC.checkDead();
                    nPC.active = false;
                    nPC.netUpdate = true;
                }
            }

            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 149);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, 150);

                    int num802 = Gore.NewGore(NPC.GetSource_Death(), NPC.position, default(Vector2), Main.rand.Next(61, 64));
                    Gore gore2 = Main.gore[num802];
                    gore2.velocity *= 0.4f;
                    Main.gore[num802].velocity.X += 1f;
                    Main.gore[num802].velocity.Y += 1f;

                    num802 = Gore.NewGore(NPC.GetSource_Death(), NPC.position, default(Vector2), Main.rand.Next(61, 64));
                    gore2 = Main.gore[num802];
                    gore2.velocity *= 0.4f;
                    Main.gore[num802].velocity.X -= 1f;
                    Main.gore[num802].velocity.Y += 1f;

                    num802 = Gore.NewGore(NPC.GetSource_Death(), NPC.position, default(Vector2), Main.rand.Next(61, 64));
                    gore2 = Main.gore[num802];
                    gore2.velocity *= 0.4f;
                    Main.gore[num802].velocity.X += 1f;
                    Main.gore[num802].velocity.Y -= 1f;

                    num802 = Gore.NewGore(NPC.GetSource_Death(), NPC.position, default(Vector2), Main.rand.Next(61, 64));
                    gore2 = Main.gore[num802];
                    gore2.velocity *= 0.4f;
                    Main.gore[num802].velocity.X -= 1f;
                    Main.gore[num802].velocity.Y -= 1f;
                }

                for (int num798 = 0; num798 < 10; num798++)
                {
                    int num799 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                    Dust dust = Main.dust[num799];
                    dust.velocity *= 1.4f;
                }

                for (int num800 = 0; num800 < 5; num800++)
                {
                    int num801 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default(Color), 2.5f);
                    Main.dust[num801].noGravity = true;
                    Dust dust = Main.dust[num801];
                    dust.velocity *= 5f;
                    num801 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 100, default(Color), 1.5f);
                    dust = Main.dust[num801];
                    dust.velocity *= 3f;
                }
            }
        }
    }
}
