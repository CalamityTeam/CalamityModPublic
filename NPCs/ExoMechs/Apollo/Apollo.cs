using CalamityMod.Events;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Skies;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

using ArtemisBoss = CalamityMod.NPCs.ExoMechs.Artemis.Artemis;

namespace CalamityMod.NPCs.ExoMechs.Apollo
{
    public class Apollo : ModNPC
    {
        public static int phase1IconIndex;
        public static int phase2IconIndex;

        internal static void LoadHeadIcons()
        {
            string phase1IconPath = "CalamityMod/NPCs/ExoMechs/Apollo/ApolloHead";
            string phase2IconPath = "CalamityMod/NPCs/ExoMechs/Apollo/ApolloPhase2Head";

            CalamityMod.Instance.AddBossHeadTexture(phase1IconPath, -1);
            phase1IconIndex = ModContent.GetModBossHeadSlot(phase1IconPath);

            CalamityMod.Instance.AddBossHeadTexture(phase2IconPath, -1);
            phase2IconIndex = ModContent.GetModBossHeadSlot(phase2IconPath);
        }

        public enum Phase
        {
            Normal = 0,
            RocketBarrage = 1,
            LineUpChargeCombo = 2,
            ChargeCombo = 3,
            PhaseTransition = 4
        }

        public float AIState
        {
            get => NPC.Calamity().newAI[0];
            set => NPC.Calamity().newAI[0] = value;
        }

        public enum SecondaryPhase
        {
            Nothing = 0,
            Passive = 1,
            PassiveAndImmune = 2
        }

        public float SecondaryAIState
        {
            get => NPC.Calamity().newAI[1];
            set => NPC.Calamity().newAI[1] = value;
        }

        // Variable used to scale up velocity if too far from destination
        private float velocityBoostMult = 0f;

        // Number of frames on the X and Y axis
        private const int maxFramesX = 10;
        private const int maxFramesY = 9;

        // Counters for frames on the X and Y axis
        private int frameX = 0;
        private int frameY = 0;

        // Frame limit per animation, these are the specific frames where each animation ends
        private const int normalFrameLimit_Phase1 = 9;
        private const int chargeUpFrameLimit_Phase1 = 19;
        private const int attackFrameLimit_Phase1 = 29;
        private const int phaseTransitionFrameLimit = 59; // The lens pops off on frame 37
        private const int normalFrameLimit_Phase2 = 69;
        private const int chargeUpFrameLimit_Phase2 = 79;
        private const int attackFrameLimit_Phase2 = 89;

        // Default life ratio for the other mechs
        private const float defaultLifeRatio = 5f;

        // Max distance from the target before they are unable to hear sound telegraphs
        private const float soundDistance = 2800f;

        // Normal animation duration
        private const float defaultAnimationDuration = 60f;

        // Total duration of the phase transition
        private const float phaseTransitionDuration = 180f;

        // Where the timer should be when the lens pops off
        private const float lensPopTime = 48f;

        // Variable to pick a different location after each attack
        private bool pickNewLocation = false;

        // Marks Apollo as a component of the Exo Mechdusa
        public bool exoMechdusa = false;

        // Charge locations during the charge combo
        private const int maxCharges = 4;
        public Vector2[] chargeLocations = new Vector2[maxCharges] { default, default, default, default };

        // Intensity of flash effects during the charge combo
        public float ChargeComboFlash;

        public static string NameToDisplay = "XS-03 Apollo";

        public static readonly SoundStyle MissileLaunchSound = new("CalamityMod/Sounds/Custom/ExoMechs/ApolloMissileLaunch") { Volume = 1.3f };

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitPositionXOverride = 50f,
                PortraitPositionYOverride = 40f,
                PortraitScale = 0.75f,
                Scale = 0.45f,
                Rotation = -MathHelper.PiOver4
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 5f;
            NPC.GetNPCDamage();
            NPC.width = 204;
            NPC.height = 226;
            NPC.defense = 100;
            NPC.DR_NERD(0.25f);
            NPC.LifeMaxNERB(1250000, 1495000, 650000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.Opacity = 0f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(15, 0, 0, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.DeathSound = CommonCalamitySounds.ExoDeathSound;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Apollo")
            });
        }

        public override void BossHeadSlot(ref int index)
        {
            if (SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune)
                index = -1;
            else if (NPC.life / (float)NPC.lifeMax < 0.6f)
                index = phase2IconIndex;
            else
                index = phase1IconIndex;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(velocityBoostMult);
            writer.Write(frameX);
            writer.Write(frameY);
            writer.Write(exoMechdusa);
            writer.Write(pickNewLocation);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
            {
                writer.Write(NPC.Calamity().newAI[i]);
                writer.WriteVector2(chargeLocations[i]);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            velocityBoostMult = reader.ReadSingle();
            frameX = reader.ReadInt32();
            frameY = reader.ReadInt32();
            pickNewLocation = reader.ReadBoolean();
            exoMechdusa = reader.ReadBoolean();
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
            {
                NPC.Calamity().newAI[i] = reader.ReadSingle();
                chargeLocations[i] = reader.ReadVector2();
            }
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            CalamityGlobalNPC.draedonExoMechTwinGreen = NPC.whoAmI;

            NPC.frame = new Rectangle(NPC.width * frameX, NPC.height * frameY, NPC.width, NPC.height);

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            // Target variable
            Player player = Main.player[NPC.target];

            // Check if the other exo mechs are alive
            int otherExoMechsAlive = 0;
            bool exoWormAlive = false;
            bool exoPrimeAlive = false;
            bool exoMechTwinRedAlive = false;
            bool artemisUsingDeathray = false;
            if (CalamityGlobalNPC.draedonExoMechTwinRed != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].active)
                {
                    exoMechTwinRedAlive = true;
                    artemisUsingDeathray = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[0] == 3f;

                    // Link the HP of both twins
                    if (NPC.life > Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].life)
                        NPC.life = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].life;
                }
            }
            if (CalamityGlobalNPC.draedonExoMechWorm != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].active)
                {
                    // Set target to Thanatos' target if Thanatos is alive
                    player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechWorm].target];

                    otherExoMechsAlive++;
                    exoWormAlive = true;
                }
            }
            if (CalamityGlobalNPC.draedonExoMechPrime != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
                {
                    // Set target to Ares' target if Ares is alive
                    player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechPrime].target];

                    otherExoMechsAlive++;
                    exoPrimeAlive = true;
                }
            }

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // These are 5 by default to avoid triggering passive phases after the other mechs are dead
            float exoWormLifeRatio = defaultLifeRatio;
            float exoPrimeLifeRatio = defaultLifeRatio;
            if (exoWormAlive)
                exoWormLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechWorm].lifeMax;
            if (exoPrimeAlive)
                exoPrimeLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].lifeMax;
            float totalOtherExoMechLifeRatio = exoWormLifeRatio + exoPrimeLifeRatio;

            // Check if any of the other mechs are passive
            bool exoWormPassive = false;
            bool exoPrimePassive = false;
            if (exoWormAlive)
                exoWormPassive = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].Calamity().newAI[1] == (float)ThanatosHead.SecondaryPhase.Passive;
            if (exoPrimeAlive)
                exoPrimePassive = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Calamity().newAI[1] == (float)AresBody.SecondaryPhase.Passive;
            bool anyOtherExoMechPassive = exoWormPassive || exoPrimePassive;

            // Used to nerf Artemis and Apollo if fighting alongside Ares, because otherwise it's too difficult
            bool nerfedAttacks = false;
            if (exoWormAlive && !nerfedAttacks)
                nerfedAttacks = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].Calamity().newAI[1] != (float)ThanatosHead.SecondaryPhase.PassiveAndImmune;
            if (exoPrimeAlive && !nerfedAttacks)
                nerfedAttacks = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Calamity().newAI[1] != (float)AresBody.SecondaryPhase.PassiveAndImmune;

            // Check if any of the other mechs were spawned first
            bool exoWormWasFirst = false;
            bool exoPrimeWasFirst = false;
            if (exoWormAlive)
                exoWormWasFirst = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].ai[3] == 1f;
            if (exoPrimeAlive)
                exoPrimeWasFirst = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].ai[3] == 1f;
            bool otherExoMechWasFirst = exoWormWasFirst || exoPrimeWasFirst;

            // Check for Draedon
            bool draedonAlive = false;
            if (CalamityGlobalNPC.draedon != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedon].active)
                    draedonAlive = true;
            }

            // Prevent mechs from being respawned
            if (otherExoMechWasFirst)
            {
                if (NPC.ai[3] < 1f)
                    NPC.ai[3] = 1f;
            }

            // Decrement the timer used to stop Artemis from firing lasers
            if (NPC.ai[3] > 1f)
                NPC.ai[3] -= 1f;

            // Phases
            bool phase2 = lifeRatio < 0.6f;
            bool spawnOtherExoMechs = lifeRatio < 0.7f && NPC.ai[3] == 0f;
            bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
            bool lastMechAlive = berserk && otherExoMechsAlive == 0;

            // If Artemis and Apollo don't go berserk
            bool otherMechIsBerserk = exoWormLifeRatio < 0.4f || exoPrimeLifeRatio < 0.4f;

            // Whether Artemis and Apollo should be buffed while in berserk phase
            bool shouldGetBuffedByBerserkPhase = berserk && !otherMechIsBerserk;

            // Spawn Artemis if it doesn't exist after the first 10 frames have passed
            if (NPC.ai[0] < 10f)
            {
                NPC.ai[0] += 1f;
                if (NPC.ai[0] == 10f && !NPC.AnyNPCs(ModContent.NPCType<Artemis.Artemis>()))
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Artemis.Artemis>());
            }
            else
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<Artemis.Artemis>()))
                {
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.active = false;
                    NPC.netUpdate = true;
                }
            }

            // General AI pattern
            // 0 - Fly to the right of the target and fire plasma when ready
            // 1 - Fly to the right of the target and fire homing rockets, the rockets home in until close to the target and then fly off in the direction they were moving in
            // 2 - Fly below the target, create several line telegraphs to show the dash pattern and then dash along those lines extremely quickly
            // 3 - Go passive and fly to the right of the target while firing less projectiles
            // 4 - Go passive, immune and invisible; fly far to the left of the target and do nothing until next phase

            // Attack patterns
            // If spawned first
            // Phase 1 - 0, 1
            // Phase 2 - 4
            // Phase 3 - 3

            // If berserk, this is the last phase of Artemis and Apollo
            // Phase 4 - 0, 1, 2

            // If not berserk
            // Phase 4 - 4
            // Phase 5 - 0, 1

            // If berserk, this is the last phase of Artemis and Apollo
            // Phase 6 - 0, 1, 2

            // If not berserk
            // Phase 6 - 4

            // Berserk, final phase of Artemis and Apollo
            // Phase 7 - 0, 1, 2

            // Gate values
            float reducedTimeForGateValue = bossRush ? 48f : death ? 32f : revenge ? 24f : expertMode ? 16f : 0f;
            float reducedTimeForGateValue_Berserk = reducedTimeForGateValue * 0.5f;
            float normalAttackTime = 360f - reducedTimeForGateValue;
            float berserkAttackTime = lastMechAlive ? 225f - reducedTimeForGateValue_Berserk : 270f - reducedTimeForGateValue_Berserk;
            float attackPhaseGateValue = shouldGetBuffedByBerserkPhase ? berserkAttackTime : normalAttackTime;
            float timeToLineUpAttack = 30f;
            float timeToLineUpCharge = bossRush ? 45f : death ? 60f : revenge ? 68f : expertMode ? 75f : 90f;

            if (Main.getGoodWorld)
            {
                timeToLineUpAttack *= 0.5f;
                timeToLineUpCharge *= 0.5f;
            }

            // Distance where Apollo stops moving
            float movementDistanceGateValue = 100f;
            float chargeLocationDistanceGateValue = 40f;

            // Velocity and acceleration values
            float baseVelocityMult = (shouldGetBuffedByBerserkPhase ? 0.25f : 0f) + (bossRush ? 1.15f : death ? 1.1f : revenge ? 1.075f : expertMode ? 1.05f : 1f);
            float baseVelocity = (AIState == (int)Phase.LineUpChargeCombo ? 40f : 20f) * baseVelocityMult;

            // Attack gate values
            bool lineUpAttack = calamityGlobalNPC.newAI[3] >= attackPhaseGateValue + 2f;
            bool doBigAttack = calamityGlobalNPC.newAI[3] >= attackPhaseGateValue + 2f + timeToLineUpAttack;

            // Charge velocity
            float chargeVelocity = bossRush ? 115f : death ? 105f : revenge ? 101.25f : expertMode ? 97.5f : 90f;

            if (Main.getGoodWorld)
            {
                baseVelocity *= 1.5f;
                chargeVelocity *= 1.15f;
            }

            // Charge phase variables
            double chargeDistance = Math.Sqrt(500D * 500D + 800D * 800D);
            float chargeTime = (float)chargeDistance / chargeVelocity;

            // Plasma and rocket projectile velocities
            float projectileVelocity = 12f;
            if (lastMechAlive)
                projectileVelocity *= 1.2f;
            else if (shouldGetBuffedByBerserkPhase)
                projectileVelocity *= 1.1f;

            // Rocket phase variables
            float rocketPhaseDuration = lastMechAlive ? 60f : 90f;
            int numRockets = nerfedAttacks ? 2 : 3;

            if (Main.getGoodWorld)
                numRockets += 3;

            // Default vector to fly to
            bool flyRight = NPC.ai[0] % 2f == 0f || NPC.ai[0] < 10f || !revenge;
            float destinationX = flyRight ? 750f : -750f;
            float destinationY = player.Center.Y;
            float chargeComboXOffset = flyRight ? -600f : 600f;
            float chargeComboYOffset = NPC.ai[2] % 2f == 0f ? 480f : -480f;
            Vector2 destination = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune ? new Vector2(player.Center.X + destinationX * 1.6f, destinationY) :
                SecondaryAIState == (float)SecondaryPhase.Passive ? new Vector2(player.Center.X + destinationX, destinationY + 360f) :
                AIState == (float)Phase.LineUpChargeCombo ? new Vector2(player.Center.X + destinationX * 1.2f, destinationY + chargeComboYOffset) :
                new Vector2(player.Center.X + destinationX, destinationY);

            // Add some random distance to the destination after certain attacks
            if (pickNewLocation)
            {
                pickNewLocation = false;

                int randomLocationVarianceX = shouldGetBuffedByBerserkPhase ? 50 : 20;
                int randomLocationVarianceY = shouldGetBuffedByBerserkPhase ? 250 : 100;

                if (Main.getGoodWorld)
                {
                    randomLocationVarianceX *= 2;
                    randomLocationVarianceY *= 2;
                }

                NPC.localAI[0] = Main.rand.Next(-randomLocationVarianceX, randomLocationVarianceX + 1);
                NPC.localAI[1] = Main.rand.Next(-randomLocationVarianceY, randomLocationVarianceY + 1);
                if (AIState == (float)Phase.RocketBarrage || SecondaryAIState == (float)SecondaryPhase.Passive)
                {
                    NPC.localAI[0] *= 0.5f;
                    NPC.localAI[1] *= 0.5f;
                }

                NPC.netUpdate = true;
            }

            // Add a bit of randomness to the destination, but only in specific phases where it's necessary
            if (AIState == (float)Phase.Normal || AIState == (float)Phase.RocketBarrage || AIState == (float)Phase.PhaseTransition)
            {
                destination.X += NPC.localAI[0];
                destination.Y += NPC.localAI[1];
            }

            // Scale up velocity over time if too far from destination
            Vector2 distanceFromDestination = destination - NPC.Center;
            if (distanceFromDestination.Length() > movementDistanceGateValue && AIState != (float)Phase.ChargeCombo)
            {
                if (velocityBoostMult < 1f)
                    velocityBoostMult += 0.004f;
            }
            else
            {
                if (velocityBoostMult > 0f)
                    velocityBoostMult -= 0.004f;
            }
            baseVelocity *= 1f + velocityBoostMult;

            // If Apollo can fire projectiles, cannot fire if too close to the target
            bool canFire = distanceFromDestination.Length() <= 320f;

            // Rotation
            Vector2 aimedVector = player.Center - NPC.Center;
            float rateOfRotation = 0.1f;
            Vector2 rotateTowards = player.Center - NPC.Center;
            bool readyToCharge = AIState == (float)Phase.LineUpChargeCombo && (Vector2.Distance(NPC.Center, destination) <= chargeLocationDistanceGateValue || calamityGlobalNPC.newAI[2] > 0f);
            if (AIState == (int)Phase.ChargeCombo)
            {
                rateOfRotation = 0f;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else if (AIState == (float)Phase.LineUpChargeCombo && chargeLocations[1] != default)
            {
                float x = chargeLocations[1].X - NPC.Center.X;
                float y = chargeLocations[1].Y - NPC.Center.Y;
                rotateTowards = Vector2.Normalize(new Vector2(x, y)) * baseVelocity;
            }
            else
            {
                float x = player.Center.X - NPC.Center.X;
                float y = player.Center.Y - NPC.Center.Y;
                rotateTowards = Vector2.Normalize(new Vector2(x, y)) * baseVelocity;
            }

            // Do not set this during charge or deathray phases
            if (rateOfRotation != 0f)
                NPC.rotation = NPC.rotation.AngleTowards((float)Math.Atan2(rotateTowards.Y, rotateTowards.X) + MathHelper.PiOver2, rateOfRotation);

            // Despawn if target is dead
            if (player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (player.dead)
                {
                    AIState = (float)Phase.Normal;
                    NPC.localAI[0] = 0f;
                    NPC.localAI[1] = 0f;
                    NPC.localAI[2] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;
                    for (int i = 0; i < maxCharges; i++)
                        chargeLocations[i] = default;

                    NPC.dontTakeDamage = true;

                    NPC.velocity.Y -= 1f;
                    if ((double)NPC.position.Y < Main.topWorld + 16f)
                        NPC.velocity.Y -= 1f;

                    if ((double)NPC.position.Y < Main.topWorld + 16f)
                    {
                        for (int a = 0; a < Main.maxNPCs; a++)
                        {
                            if (Main.npc[a].type == NPC.type || Main.npc[a].type == ModContent.NPCType<Artemis.Artemis>() || Main.npc[a].type == ModContent.NPCType<AresBody>() ||
                                Main.npc[a].type == ModContent.NPCType<AresLaserCannon>() || Main.npc[a].type == ModContent.NPCType<AresPlasmaFlamethrower>() ||
                                Main.npc[a].type == ModContent.NPCType<AresTeslaCannon>() || Main.npc[a].type == ModContent.NPCType<AresGaussNuke>() ||
                                Main.npc[a].type == ModContent.NPCType<ThanatosHead>() || Main.npc[a].type == ModContent.NPCType<ThanatosBody1>() ||
                                Main.npc[a].type == ModContent.NPCType<ThanatosBody2>() || Main.npc[a].type == ModContent.NPCType<ThanatosTail>())
                                Main.npc[a].active = false;
                        }
                    }

                    return;
                }
            }

            // Cause the charge visual effects to begin while performing or preparing for a combo
            if (AIState == (float)Phase.LineUpChargeCombo || AIState == (float)Phase.ChargeCombo)
                ChargeComboFlash = MathHelper.Clamp(ChargeComboFlash + 0.08f, 0f, 1f);

            // And have them go away afterwards
            else
                ChargeComboFlash = MathHelper.Clamp(ChargeComboFlash - 0.1f, 0f, 1f);

            // Set to transition to phase 2 if it hasn't happened yet
            if (phase2 && NPC.localAI[3] == 0f)
            {
                AIState = (float)Phase.PhaseTransition;
                NPC.localAI[3] = 1f;
                calamityGlobalNPC.newAI[2] = 0f;
                calamityGlobalNPC.newAI[3] = 0f;

                // Set frames to phase transition frames, which begin on frame 30
                // Reset the frame counter
                NPC.frameCounter = 0D;

                // X = 3 sets to frame 27
                frameX = 3;

                // Y = 3 sets to frame 30
                frameY = 3;
            }

            // Passive and Immune phases
            switch ((int)SecondaryAIState)
            {
                case (int)SecondaryPhase.Nothing:

                    // Spawn the other mechs if Artemis and Apollo are first
                    if (otherExoMechsAlive == 0 && !exoMechdusa)
                    {
                        if (spawnOtherExoMechs)
                        {
                            // Despawn projectile bullshit
                            KillProjectiles();

                            // Set Artemis variables
                            if (exoMechTwinRedAlive)
                            {
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[1] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[2] = 0f;
                                if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[3] < 1f)
                                    Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[3] = 1f;

                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[1] = (float)Artemis.Artemis.SecondaryPhase.PassiveAndImmune;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[0] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[1] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[2] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[2] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[3] = 0f;
                            }

                            // Reset everything
                            if (NPC.ai[0] < 10f)
                                NPC.ai[0] = 10f;
                            NPC.ai[0] += 1f;

                            if (NPC.ai[3] < 1f)
                                NPC.ai[3] = 1f;

                            SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
                            NPC.localAI[0] = 0f;
                            NPC.localAI[1] = 0f;
                            NPC.localAI[2] = 0f;
                            calamityGlobalNPC.newAI[2] = 0f;
                            calamityGlobalNPC.newAI[3] = 0f;
                            for (int i = 0; i < maxCharges; i++)
                                chargeLocations[i] = default;

                            NPC.TargetClosest();

                            // Draedon text for the start of phase 2
                            if (draedonAlive)
                            {
                                Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 1f;
                                Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                // Spawn the fuckers
                                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ThanatosHead>());
                                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<AresBody>());
                            }
                        }
                    }
                    else
                    {
                        // If not spawned first, go to passive state if any other mech is passive or if Artemis and Apollo are under 70% life
                        // Do not run this if berserk
                        // Do not run this if any exo mech is dead
                        if ((anyOtherExoMechPassive || lifeRatio < 0.7f) && !berserk && totalOtherExoMechLifeRatio < 5f)
                        {
                            // Set Artemis variables
                            if (exoMechTwinRedAlive)
                            {
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[1] = (float)Artemis.Artemis.SecondaryPhase.Passive;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[1] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[2] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[0] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[1] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[2] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[2] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[3] = 0f;
                            }

                            // Tells Apollo to return to the battle in passive state and reset everything
                            SecondaryAIState = (float)SecondaryPhase.Passive;
                            NPC.localAI[0] = 0f;
                            NPC.localAI[1] = 0f;
                            NPC.localAI[2] = 0f;
                            calamityGlobalNPC.newAI[2] = 0f;
                            calamityGlobalNPC.newAI[3] = 0f;
                            for (int i = 0; i < maxCharges; i++)
                                chargeLocations[i] = default;

                            NPC.TargetClosest();
                        }

                        // Go passive and immune if one of the other mechs is berserk
                        // This is only called if two exo mechs are alive in ideal scenarios
                        // This is not called if Artemis and Apollo and another one or two mechs are berserk
                        if (otherMechIsBerserk && !berserk && !exoMechdusa)
                        {
                            // Despawn projectile bullshit
                            KillProjectiles();

                            // Set Artemis variables
                            if (exoMechTwinRedAlive)
                            {
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[1] = (float)Artemis.Artemis.SecondaryPhase.PassiveAndImmune;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[1] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[2] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[0] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[1] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[2] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[2] = 0f;
                                Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[3] = 0f;
                            }

                            // Reset everything
                            if (NPC.ai[0] < 10f)
                                NPC.ai[0] = 10f;
                            NPC.ai[0] += 1f;

                            SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
                            NPC.localAI[0] = 0f;
                            NPC.localAI[1] = 0f;
                            NPC.localAI[2] = 0f;
                            calamityGlobalNPC.newAI[2] = 0f;
                            calamityGlobalNPC.newAI[3] = 0f;
                            for (int i = 0; i < maxCharges; i++)
                                chargeLocations[i] = default;

                            NPC.TargetClosest();

                            // Phase 6, when 1 mech goes berserk and the other one leaves
                            if (draedonAlive)
                            {
                                Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 5f;
                                Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                            }
                        }
                    }

                    break;

                // Fire projectiles less often, this happens when all 3 mechs are present and attacking
                case (int)SecondaryPhase.Passive:

                    // Artemis fires lasers while passive
                    if (exoMechTwinRedAlive)
                        Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[0] = (float)Artemis.Artemis.Phase.Normal;

                    // Fire plasma while passive
                    AIState = (float)Phase.Normal;

                    // Enter passive and invincible phase if one of the other exo mechs is berserk
                    if (otherMechIsBerserk && !exoMechdusa)
                    {
                        // Despawn projectile bullshit
                        KillProjectiles();

                        // Set Artemis variables
                        if (exoMechTwinRedAlive)
                        {
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[1] = (float)Artemis.Artemis.SecondaryPhase.PassiveAndImmune;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[1] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[2] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[0] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[1] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[2] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[2] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[3] = 0f;
                        }

                        // Reset everything
                        if (NPC.ai[0] < 10f)
                            NPC.ai[0] = 10f;
                        NPC.ai[0] += 1f;

                        SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
                        NPC.localAI[0] = 0f;
                        NPC.localAI[1] = 0f;
                        NPC.localAI[2] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        for (int i = 0; i < maxCharges; i++)
                            chargeLocations[i] = default;

                        NPC.TargetClosest();
                    }

                    // If Artemis and Apollo are the first mechs to go berserk
                    if (berserk)
                    {
                        // Set Artemis variables
                        if (exoMechTwinRedAlive)
                        {
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[1] = (float)Artemis.Artemis.SecondaryPhase.Nothing;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[1] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[2] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[0] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[1] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[2] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[2] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[3] = 0f;
                        }

                        // Reset everything
                        NPC.localAI[0] = 0f;
                        NPC.localAI[1] = 0f;
                        NPC.localAI[2] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        for (int i = 0; i < maxCharges; i++)
                            chargeLocations[i] = default;

                        NPC.TargetClosest();

                        // Never be passive if berserk
                        SecondaryAIState = (float)SecondaryPhase.Nothing;

                        // Phase 4, when 1 mech goes berserk and the other 2 leave
                        if (exoWormAlive && exoPrimeAlive)
                        {
                            if (draedonAlive)
                            {
                                Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 3f;
                                Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                            }
                        }
                    }

                    break;

                // Fly above target and become immune
                case (int)SecondaryPhase.PassiveAndImmune:

                    // Artemis does nothing while immune
                    if (exoMechTwinRedAlive)
                        Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[0] = (float)ArtemisBoss.Phase.Normal;

                    // Do nothing while immune
                    AIState = (float)Phase.Normal;

                    // Enter the fight again if any of the other exo mechs is below 70% and other mechs aren't berserk
                    if ((exoWormLifeRatio < 0.7f || exoPrimeLifeRatio < 0.7f) && !otherMechIsBerserk)
                    {
                        // Set Artemis variables
                        if (exoMechTwinRedAlive)
                        {
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[1] =
                                totalOtherExoMechLifeRatio > 5f ? (float)Artemis.Artemis.SecondaryPhase.Nothing : (float)Artemis.Artemis.SecondaryPhase.Passive;

                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[1] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[2] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[0] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[1] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[2] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[2] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[3] = 0f;
                        }

                        // Tells Artemis and Apollo to return to the battle in passive state and reset everything
                        // Return to normal phases if one or more mechs have been downed
                        SecondaryAIState = totalOtherExoMechLifeRatio > 5f ? (float)SecondaryPhase.Nothing : (float)SecondaryPhase.Passive;
                        NPC.localAI[0] = 0f;
                        NPC.localAI[1] = 0f;
                        NPC.localAI[2] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        for (int i = 0; i < maxCharges; i++)
                            chargeLocations[i] = default;

                        NPC.TargetClosest();

                        // Phase 3, when all 3 mechs attack at the same time
                        if (exoWormAlive && exoPrimeAlive)
                        {
                            if (draedonAlive)
                            {
                                Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 2f;
                                Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                            }
                        }
                    }

                    // This is here just in case
                    if (berserk)
                    {
                        // Set Artemis variables
                        if (exoMechTwinRedAlive)
                        {
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[1] = (float)Artemis.Artemis.SecondaryPhase.Nothing;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[1] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].ai[2] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[0] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[1] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].localAI[2] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[2] = 0f;
                            Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[3] = 0f;
                        }

                        // Reset everything
                        NPC.localAI[0] = 0f;
                        NPC.localAI[1] = 0f;
                        NPC.localAI[2] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        for (int i = 0; i < maxCharges; i++)
                            chargeLocations[i] = default;

                        NPC.TargetClosest();

                        // Never be passive if berserk
                        SecondaryAIState = (float)SecondaryPhase.Nothing;
                    }

                    break;
            }

            // Adjust opacity
            bool invisiblePhase = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune;
            NPC.dontTakeDamage = invisiblePhase || AIState == (float)Phase.PhaseTransition;
            if (!invisiblePhase)
            {
                NPC.Opacity += 0.2f;
                if (NPC.Opacity > 1f)
                    NPC.Opacity = 1f;
            }
            else
            {
                NPC.Opacity -= 0.05f;
                if (NPC.Opacity < 0f)
                    NPC.Opacity = 0f;
            }

            // Attacking phases
            switch ((int)AIState)
            {
                // Fly to the right of the target
                case (int)Phase.Normal:

                    // Smooth movement towards the location Apollo is meant to be at
                    CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);

                    // Default animation for 60 frames and then go to telegraph animation
                    // newAI[3] tells Apollo what animation state it's currently in
                    bool attacking = calamityGlobalNPC.newAI[3] >= 2f;
                    bool firingPlasma = attacking && calamityGlobalNPC.newAI[3] + 2f < attackPhaseGateValue;

                    // Only increase attack timer if not in immune phase
                    if (SecondaryAIState != (float)SecondaryPhase.PassiveAndImmune)
                        calamityGlobalNPC.newAI[2] += 1f;

                    if (calamityGlobalNPC.newAI[2] >= defaultAnimationDuration || attacking)
                    {
                        if (firingPlasma)
                        {
                            // Fire plasma.
                            float divisor = nerfedAttacks ? 60f : lastMechAlive ? 40f : 45f;
                            float plasmaTimer = calamityGlobalNPC.newAI[3] - 2f;
                            if (plasmaTimer % divisor == 0f && canFire)
                            {
                                pickNewLocation = true;
                                NPC.ai[2] += 1f;
                                SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaShootSound, NPC.Center);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = ModContent.ProjectileType<ApolloFireball>();
                                    int damage = NPC.GetProjectileDamage(type);
                                    Vector2 plasmaVelocity = Vector2.Normalize(aimedVector) * projectileVelocity;
                                    Vector2 offset = Vector2.Normalize(plasmaVelocity) * 70f;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, plasmaVelocity, type, damage, 0f, Main.myPlayer, player.Center.X, player.Center.Y);
                                }
                            }
                        }
                        else
                            calamityGlobalNPC.newAI[2] = 0f;

                        // Enter rocket phase after a certain time has passed
                        // Enter charge phase if in phase 2 and the localAI[2] variable is set to do so
                        calamityGlobalNPC.newAI[3] += 1f;
                        if (lineUpAttack)
                        {
                            // Return to normal plasma phase if in passive state
                            if (SecondaryAIState == (float)SecondaryPhase.Passive)
                            {
                                pickNewLocation = true;
                                calamityGlobalNPC.newAI[2] = 0f;
                                calamityGlobalNPC.newAI[3] = 0f;
                                for (int i = 0; i < maxCharges; i++)
                                    chargeLocations[i] = default;

                                NPC.TargetClosest();
                                PlayTargetingSound();
                            }
                            else if (doBigAttack)
                            {
                                pickNewLocation = NPC.localAI[2] == 0f;
                                calamityGlobalNPC.newAI[2] = 0f;
                                calamityGlobalNPC.newAI[3] = 0f;

                                if (phase2)
                                    AIState = (NPC.localAI[2] == 1f && (!artemisUsingDeathray || Main.zenithWorld)) ? (float)Phase.LineUpChargeCombo : (float)Phase.RocketBarrage;
                                else
                                    AIState = (float)Phase.RocketBarrage;
                            }
                        }
                    }

                    break;

                // Charge
                case (int)Phase.RocketBarrage:

                    // Smooth movement towards the location Apollo is meant to be at
                    CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);

                    if (canFire || calamityGlobalNPC.newAI[2] > 0f)
                        calamityGlobalNPC.newAI[2] += 1f;

                    if (calamityGlobalNPC.newAI[2] % (rocketPhaseDuration / numRockets) == 0f && canFire)
                    {
                        // Play a missile firing sound.
                        SoundEngine.PlaySound(MissileLaunchSound, NPC.Center);

                        pickNewLocation = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<ApolloRocket>();
                            int damage = NPC.GetProjectileDamage(type);
                            Vector2 rocketVelocity = Vector2.Normalize(aimedVector) * projectileVelocity * 1.2f;
                            Vector2 offset = Vector2.Normalize(rocketVelocity) * 70f;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, rocketVelocity, type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
                        }
                    }

                    // Reset phase and variables
                    if (calamityGlobalNPC.newAI[2] >= rocketPhaseDuration)
                    {
                        // Go back to normal phase
                        AIState = (float)Phase.Normal;
                        NPC.localAI[2] = shouldGetBuffedByBerserkPhase ? 1f : 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        PlayTargetingSound();

                        NPC.TargetClosest();
                    }

                    break;

                // Get in position for the charge combo, don't do anything else until in position
                // Fill up the charge location array with the positions Apollo will charge from
                // Create telegraph beams between the charge location array positions
                case (int)Phase.LineUpChargeCombo:

                    if (!readyToCharge)
                    {
                        // Smooth movement towards the location Apollo is meant to be at
                        CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);
                    }
                    else
                    {
                        // Save the charge locations and create telegraph beams
                        int type = ModContent.ProjectileType<ApolloChargeTelegraph>();

                        for (int i = 0; i < maxCharges; i++)
                        {
                            if (chargeLocations[i] == default)
                            {
                                switch (i)
                                {
                                    case 0:
                                        chargeLocations[i] = NPC.Center;
                                        break;
                                    case 1:
                                        chargeLocations[i] = chargeLocations[0] + new Vector2(chargeComboXOffset, -chargeComboYOffset * 2f);
                                        break;
                                    case 2:
                                        chargeLocations[i] = chargeLocations[1] + new Vector2(chargeComboXOffset, chargeComboYOffset * 2f);
                                        break;
                                    case 3:
                                        chargeLocations[i] = chargeLocations[2] + new Vector2(chargeComboXOffset, -chargeComboYOffset * 2f);
                                        break;
                                    default:
                                        break;
                                }

                                if (i == 0)
                                {
                                    // Draw telegraph beams
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int telegraph = Projectile.NewProjectile(NPC.GetSource_FromAI(), chargeLocations[0], Vector2.Zero, type, 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                        if (Main.projectile.IndexInRange(telegraph))
                                        {
                                            Main.projectile[telegraph].ModProjectile<ApolloChargeTelegraph>().ChargePositions = chargeLocations;
                                            Main.projectile[telegraph].netUpdate = true;
                                        }
                                    }

                                    // Play a charge sound
                                    SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound, NPC.Center);
                                }
                            }
                        }

                        // Don't move
                        NPC.velocity = Vector2.Zero;

                        // Play a sound to accompany the telegraph.
                        SoundEngine.PlaySound(ArtemisBoss.ChargeTelegraphSound with { Volume = 1.6f }, NPC.Center);

                        // Go to charge phase, create lightning bolts in the sky, and reset
                        calamityGlobalNPC.newAI[2] += 1f;
                        if (calamityGlobalNPC.newAI[2] >= timeToLineUpCharge)
                        {
                            ExoMechsSky.CreateLightningBolt(10);

                            AIState = (float)Phase.ChargeCombo;
                            NPC.localAI[2] = 0f;
                            calamityGlobalNPC.newAI[2] = 0f;
                        }
                    }

                    break;

                // Charge to several locations almost instantly (Apollo doesn't teleport here, he's just moving very fast :D)
                case (int)Phase.ChargeCombo:

                    // Tell Artemis to not fire lasers for a short time
                    NPC.ai[3] = 61f;

                    // Set charge velocity and fire halos of plasma bolts
                    if (NPC.localAI[2] == 0f)
                    {
                        // Play a charge sound.
                        SoundEngine.PlaySound(ArtemisBoss.ChargeSound, NPC.Center);

                        NPC.velocity = Vector2.Normalize(chargeLocations[(int)calamityGlobalNPC.newAI[2] + 1] - chargeLocations[(int)calamityGlobalNPC.newAI[2]]) * chargeVelocity;
                        NPC.localAI[2] = 1f;
                        NPC.netUpdate = true;
                        NPC.netSpam -= 5;

                        // Plasma bolts on charge
                        if (Main.netMode != NetmodeID.MultiplayerClient && (!(Main.zenithWorld && !exoMechdusa) || (CalamityWorld.LegendaryMode && revenge))) // I'm not that evil (you aren't, but I am - Fab)
                        {
                            int totalProjectiles = bossRush ? 16 : death ? 12 : 8;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = ModContent.ProjectileType<AresPlasmaBolt>();
                            int damage = (int)(NPC.GetProjectileDamage(ModContent.ProjectileType<ApolloFireball>()) * 0.8);
                            float velocity = 0.5f;
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity2, type, damage, 0f, Main.myPlayer);
                            }
                        }

                        // Dust rings
                        for (int i = 0; i < 200; i++)
                        {
                            float dustVelocity = 16f;
                            if (i < 150)
                                dustVelocity = 12f;
                            if (i < 100)
                                dustVelocity = 8f;
                            if (i < 50)
                                dustVelocity = 4f;

                            int dust1 = Dust.NewDust(NPC.Center, 6, 6, Main.rand.NextBool() ? 107 : 110, 0f, 0f, 100, default, 1f);
                            float dustVelX = Main.dust[dust1].velocity.X;
                            float dustVelY = Main.dust[dust1].velocity.Y;

                            if (dustVelX == 0f && dustVelY == 0f)
                                dustVelX = 1f;

                            float dustVelocity2 = (float)Math.Sqrt(dustVelX * dustVelX + dustVelY * dustVelY);
                            dustVelocity2 = dustVelocity / dustVelocity2;
                            dustVelX *= dustVelocity2;
                            dustVelY *= dustVelocity2;

                            float scale = 1f;
                            switch ((int)dustVelocity)
                            {
                                case 4:
                                    scale = 1.2f;
                                    break;
                                case 8:
                                    scale = 1.1f;
                                    break;
                                case 12:
                                    scale = 1f;
                                    break;
                                case 16:
                                    scale = 0.9f;
                                    break;
                                default:
                                    break;
                            }

                            Dust dust2 = Main.dust[dust1];
                            dust2.velocity *= 0.5f;
                            dust2.velocity.X += dustVelX;
                            dust2.velocity.Y += dustVelY;
                            dust2.scale = scale;
                            dust2.noGravity = true;
                        }
                    }

                    // Initiate next charge if close enough to next charge location
                    calamityGlobalNPC.newAI[3] += 1f;
                    if (calamityGlobalNPC.newAI[3] >= chargeTime)
                    {
                        // Set Apollo's location to the next charge location
                        NPC.Center = chargeLocations[(int)calamityGlobalNPC.newAI[2] + 1];

                        // Reset velocity to 0
                        NPC.velocity = Vector2.Zero;

                        // Increase newAI[2] whenever a charge ends
                        calamityGlobalNPC.newAI[2] += 1f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        NPC.localAI[2] = 0f;
                    }

                    // Reset phase and variables
                    if (calamityGlobalNPC.newAI[2] >= maxCharges - 1)
                    {
                        if (Main.zenithWorld && !exoMechdusa)
                        {
                            pickNewLocation = NPC.localAI[2] == 0f;
                            calamityGlobalNPC.newAI[3] = 0f;
                            AIState = (float)Phase.LineUpChargeCombo;
                        }
                        else
                        {
                            pickNewLocation = true;
                            AIState = (float)Phase.Normal;

                            // Tell Apollo and Artemis to swap positions
                            if (NPC.ai[0] < 10f)
                                NPC.ai[0] = 10f;
                            NPC.ai[0] += 1f;
                        }

                        NPC.localAI[2] = 0f;
                        for (int i = 0; i < maxCharges; i++)
                            chargeLocations[i] = default;
                        ChargeComboFlash = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;

                        // Change Y offset for the next charge combo
                        NPC.ai[2] = Main.rand.Next(2);

                        NPC.TargetClosest();
                        PlayTargetingSound();

                        NPC.netUpdate = true;
                    }

                    break;

                // Phase transition animation, that's all this exists for
                case (int)Phase.PhaseTransition:

                    // Smooth movement towards the location Apollo is meant to be at
                    CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);

                    // Shoot lens gore at the target at the proper time
                    if (calamityGlobalNPC.newAI[2] == lensPopTime)
                    {
                        SoundEngine.PlaySound(Artemis.Artemis.LensSound, NPC.Center);
                        Vector2 lensDirection = Vector2.Normalize(aimedVector);
                        Vector2 offset = lensDirection * 70f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, lensDirection * 24f, ModContent.ProjectileType<BrokenApolloLens>(), 0, 0f);
                    }

                    // Reset phase and variables
                    calamityGlobalNPC.newAI[2] += 1f;
                    if (calamityGlobalNPC.newAI[2] >= phaseTransitionDuration)
                    {
                        pickNewLocation = true;
                        AIState = (float)Phase.Normal;
                        NPC.localAI[0] = 0f;
                        NPC.localAI[1] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        calamityGlobalNPC.newAI[3] = 0f;
                        NPC.TargetClosest();
                        PlayTargetingSound();
                    }

                    break;
            }

            // Exo Mechdusa behavior
            if (exoMechdusa)
            {
                int twinoffset = 300;
                int extratwinoffset = 100;
                int twinheight = 300;
                if (CalamityGlobalNPC.draedonExoMechPrime != -1)
                {
                    if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].ModNPC<AresBody>().exoMechdusa)
                    {
                        NPC aresin = Main.npc[CalamityGlobalNPC.draedonExoMechPrime];
                        if (NPC.Calamity().newAI[0] != (float)Phase.ChargeCombo && NPC.Calamity().newAI[0] != (float)Phase.LineUpChargeCombo)
                        {
                            Vector2 pos = new Vector2(aresin.Center.X - twinoffset - extratwinoffset, aresin.Center.Y - twinheight);
                            NPC.position = pos;
                        }
                    }
                }
            }
        }

        // Plays the targeting sound from both Exo Twins, indicating that they're syncing up.
        public static void PlayTargetingSound()
        {
            // Play for Artemis.
            if (CalamityGlobalNPC.draedonExoMechTwinRed >= 0 && Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].active)
                SoundEngine.PlaySound(ArtemisBoss.AttackSelectionSound, Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Center);

            // Play for Apollo.
            if (CalamityGlobalNPC.draedonExoMechTwinGreen >= 0 && Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
                SoundEngine.PlaySound(ArtemisBoss.AttackSelectionSound, Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Center);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;

            Rectangle targetHitbox = target.Hitbox;

            float hitboxTopLeft = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float hitboxTopRight = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float hitboxBotLeft = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float hitboxBotRight = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = hitboxTopLeft;
            if (hitboxTopRight < minDist)
                minDist = hitboxTopRight;
            if (hitboxBotLeft < minDist)
                minDist = hitboxBotLeft;
            if (hitboxBotRight < minDist)
                minDist = hitboxBotRight;

            return minDist <= 100f && NPC.Opacity == 1f && AIState == (float)Phase.ChargeCombo;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 2f;
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            // Use telegraph frames before each attack
            bool phase2 = NPC.life / (float)NPC.lifeMax < 0.6f;
            NPC.frameCounter += 1D;
            if (AIState == (float)Phase.PhaseTransition)
            {
                if (NPC.frameCounter >= 6D)
                {
                    // Reset frame counter
                    NPC.frameCounter = 0D;

                    // Increment the Y frame
                    frameY++;

                    // Reset the Y frame if greater than 9
                    if (frameY == maxFramesY)
                    {
                        frameX++;
                        frameY = 0;
                    }
                }
            }
            else
            {
                if (AIState == (float)Phase.Normal)
                {
                    int frameLimit = phase2 ? (NPC.Calamity().newAI[3] == 0f ? normalFrameLimit_Phase2 : NPC.Calamity().newAI[3] == 1f ? chargeUpFrameLimit_Phase2 : attackFrameLimit_Phase2) :
                        (NPC.Calamity().newAI[3] == 0f ? normalFrameLimit_Phase1 : NPC.Calamity().newAI[3] == 1f ? chargeUpFrameLimit_Phase1 : attackFrameLimit_Phase1);

                    if (NPC.frameCounter >= 6D)
                    {
                        // Reset frame counter
                        NPC.frameCounter = 0D;

                        // Increment the Y frame
                        frameY++;

                        // Reset the Y frame if greater than 9
                        if (frameY == maxFramesY)
                        {
                            frameX++;
                            frameY = 0;
                        }

                        // Reset the frames
                        int currentFrame = (frameX * maxFramesY) + frameY;
                        if (currentFrame > frameLimit)
                            frameX = frameY = phase2 ? (NPC.Calamity().newAI[3] == 0f ? 6 : NPC.Calamity().newAI[3] == 1f ? 7 : 8) : (NPC.Calamity().newAI[3] == 0f ? 0 : NPC.Calamity().newAI[3] == 1f ? 1 : 2);
                    }
                }
                else if (AIState == (float)Phase.RocketBarrage || AIState == (float)Phase.LineUpChargeCombo || AIState == (float)Phase.ChargeCombo)
                {
                    int frameLimit = phase2 ? attackFrameLimit_Phase2 : attackFrameLimit_Phase1;
                    if (NPC.frameCounter >= 6D)
                    {
                        // Reset frame counter
                        NPC.frameCounter = 0D;

                        // Increment the Y frame
                        frameY++;

                        // Reset the Y frame if greater than 9
                        if (frameY == maxFramesY)
                        {
                            frameX++;
                            frameY = 0;
                        }

                        // Reset the frames
                        int currentFrame = (frameX * maxFramesY) + frameY;
                        if (currentFrame > frameLimit)
                            frameX = frameY = phase2 ? 8 : 2;
                    }
                }
            }
            NPC.frame = new Rectangle(NPC.width * frameX, NPC.height * frameY, NPC.width, NPC.height);
        }

        public float FlameTrailWidthFunction(float completionRatio) => MathHelper.SmoothStep(21f, 8f, completionRatio) * ChargeComboFlash;

        public float FlameTrailWidthFunctionBig(float completionRatio) => MathHelper.SmoothStep(34f, 12f, completionRatio) * ChargeComboFlash;

        public static float RibbonTrailWidthFunction(float completionRatio)
        {
            float baseWidth = Utils.GetLerpValue(1f, 0.54f, completionRatio, true) * 5f;
            float endTipWidth = CalamityUtils.Convert01To010(Utils.GetLerpValue(0.96f, 0.89f, completionRatio, true)) * 2.4f;
            return baseWidth + endTipWidth;
        }

        public Color FlameTrailColorFunction(float completionRatio)
        {
            float trailOpacity = Utils.GetLerpValue(0.8f, 0.27f, completionRatio, true) * Utils.GetLerpValue(0f, 0.067f, completionRatio, true);
            Color startingColor = Color.Lerp(Color.White, Color.Cyan, 0.27f);
            Color middleColor = Color.Lerp(Color.Orange, Color.ForestGreen, 0.74f);
            Color endColor = Color.Lime;
            return CalamityUtils.MulticolorLerp(completionRatio, startingColor, middleColor, endColor) * ChargeComboFlash * trailOpacity;
        }

        public Color FlameTrailColorFunctionBig(float completionRatio)
        {
            float trailOpacity = Utils.GetLerpValue(0.8f, 0.27f, completionRatio, true) * Utils.GetLerpValue(0f, 0.067f, completionRatio, true) * 0.56f;
            Color startingColor = Color.Lerp(Color.White, Color.Cyan, 0.25f);
            Color middleColor = Color.Lerp(Color.Blue, Color.White, 0.35f);
            Color endColor = Color.Lerp(Color.DarkBlue, Color.White, 0.47f);
            Color color = CalamityUtils.MulticolorLerp(completionRatio, startingColor, middleColor, endColor) * ChargeComboFlash * trailOpacity;
            color.A = 0;
            return color;
        }

        public Color RibbonTrailColorFunction(float completionRatio)
        {
            Color startingColor = new Color(34, 40, 48);
            Color endColor = new Color(40, 160, 32);
            return Color.Lerp(startingColor, endColor, (float)Math.Pow(completionRatio, 1.5D)) * NPC.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Prepare the flame trail shader with its map texture.
            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));

            int numAfterimages = ChargeComboFlash > 0f ? 0 : 5;
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Rectangle frame = new Rectangle(NPC.width * frameX, NPC.height * frameY, NPC.width, NPC.height);
            Vector2 origin = NPC.Size * 0.5f;
            Vector2 center = NPC.Center - screenPos;
            Color afterimageBaseColor = Color.White;

            // Draws a single instance of a regular, non-glowmask based Artemis.
            // This is created to allow easy duplication of them when drawing the charge.
            void drawInstance(Vector2 drawOffset, Color baseColor)
            {
                if (CalamityConfig.Instance.Afterimages && !NPC.IsABestiaryIconDummy)
                {
                    for (int i = 1; i < numAfterimages; i += 2)
                    {
                        Color afterimageColor = baseColor;
                        afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
                        afterimageColor = NPC.GetAlpha(afterimageColor);
                        afterimageColor *= (numAfterimages - i) / 15f;
                        Vector2 afterimageCenter = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        afterimageCenter -= new Vector2(texture.Width, texture.Height) / new Vector2(maxFramesX, maxFramesY) * NPC.scale / 2f;
                        afterimageCenter += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                        afterimageCenter += drawOffset;
                        spriteBatch.Draw(texture, afterimageCenter, NPC.frame, afterimageColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                    }
                }

                spriteBatch.Draw(texture, center + drawOffset, frame, NPC.GetAlpha(baseColor), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            }

            // Draw ribbons near the main thruster
            for (int direction = -1; direction <= 1; direction += 2)
            {
                if (NPC.IsABestiaryIconDummy)
                    break;

                Vector2 ribbonOffset = -Vector2.UnitY.RotatedBy(NPC.rotation) * 14f;
                ribbonOffset += Vector2.UnitX.RotatedBy(NPC.rotation) * direction * 26f;

                float currentSegmentRotation = NPC.rotation;
                List<Vector2> ribbonDrawPositions = new List<Vector2>();
                for (int i = 0; i < 12; i++)
                {
                    float ribbonCompletionRatio = i / 12f;
                    float wrappedAngularOffset = MathHelper.WrapAngle(NPC.oldRot[i + 1] - currentSegmentRotation) * 0.3f;
                    float segmentRotationOffset = MathHelper.Clamp(wrappedAngularOffset, -0.12f, 0.12f);

                    // Add a sinusoidal offset that goes based on time and completion ratio to create a waving-flag-like effect.
                    // This is dampened for the first few points to prevent weird offsets. It is also dampened by high velocity.
                    float sinusoidalRotationOffset = (float)Math.Sin(ribbonCompletionRatio * 2.22f + Main.GlobalTimeWrappedHourly * 3.4f) * 1.36f;
                    float sinusoidalRotationOffsetFactor = Utils.GetLerpValue(0f, 0.37f, ribbonCompletionRatio, true) * direction * 24f;
                    sinusoidalRotationOffsetFactor *= Utils.GetLerpValue(24f, 16f, NPC.velocity.Length(), true);

                    Vector2 sinusoidalOffset = Vector2.UnitY.RotatedBy(NPC.rotation + sinusoidalRotationOffset) * sinusoidalRotationOffsetFactor;
                    Vector2 ribbonSegmentOffset = Vector2.UnitY.RotatedBy(currentSegmentRotation) * ribbonCompletionRatio * 540f + sinusoidalOffset;
                    ribbonDrawPositions.Add(NPC.Center + ribbonSegmentOffset + ribbonOffset);

                    currentSegmentRotation += segmentRotationOffset;
                }
                PrimitiveSet.Prepare(ribbonDrawPositions, new(RibbonTrailWidthFunction, RibbonTrailColorFunction), 66);
            }

            int instanceCount = (int)MathHelper.Lerp(1f, 15f, ChargeComboFlash);
            Color baseInstanceColor = Color.Lerp(drawColor, Color.White, ChargeComboFlash);
            baseInstanceColor.A = (byte)(int)(255f - ChargeComboFlash * 255f);

            if (!NPC.IsABestiaryIconDummy)
                spriteBatch.EnterShaderRegion();

            drawInstance(Vector2.Zero, baseInstanceColor);
            if (instanceCount > 1)
            {
                baseInstanceColor *= 0.04f;
                float backAfterimageOffset = MathHelper.SmoothStep(0f, 2f, ChargeComboFlash);
                for (int i = 0; i < instanceCount; i++)
                {
                    Vector2 drawOffset = (MathHelper.TwoPi * i / instanceCount + Main.GlobalTimeWrappedHourly * 0.8f).ToRotationVector2() * backAfterimageOffset;
                    drawInstance(drawOffset, baseInstanceColor);
                }
            }

            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Apollo/ApolloGlow").Value;
            if (CalamityConfig.Instance.Afterimages && !NPC.IsABestiaryIconDummy)
            {
                for (int i = 1; i < numAfterimages; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (numAfterimages - i) / 15f;
                    Vector2 afterimageCenter = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimageCenter -= new Vector2(texture.Width, texture.Height) / new Vector2(maxFramesX, maxFramesY) * NPC.scale / 2f;
                    afterimageCenter += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, afterimageCenter, NPC.frame, afterimageColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.Draw(texture, center, frame, Color.White * NPC.Opacity, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

            if (!NPC.IsABestiaryIconDummy)
                spriteBatch.ExitShaderRegion();

            // Draw a flame trail on the thrusters if needed. This happens during charges.
            if (ChargeComboFlash > 0f && !NPC.IsABestiaryIconDummy)
            {
                for (int direction = -1; direction <= 1; direction++)
                {
                    Vector2 baseDrawOffset = new Vector2(0f, direction == 0f ? 18f : 60f).RotatedBy(NPC.rotation);
                    baseDrawOffset += new Vector2(direction * 64f, 0f).RotatedBy(NPC.rotation);

                    float backFlameLength = direction == 0f ? 700f : 190f;
                    Vector2 drawStart = NPC.Center + baseDrawOffset;
                    Vector2 drawEnd = drawStart - (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * ChargeComboFlash * backFlameLength;
                    Vector2[] drawPositions = new Vector2[]
                    {
                        drawStart,
                        drawEnd
                    };

                    if (direction == 0)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 drawOffset = (MathHelper.TwoPi * i / 4f).ToRotationVector2() * 8f;
                            PrimitiveSet.Prepare(drawPositions, new(FlameTrailWidthFunctionBig, FlameTrailColorFunctionBig, (_) => drawOffset, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"]), 70);
                        }
                    }
                    else
                        PrimitiveSet.Prepare(drawPositions, new(FlameTrailWidthFunction, FlameTrailColorFunction, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"]), 70);

                }
            }

            return false;
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<Artemis.Artemis>(),
                ModContent.NPCType<Apollo>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }

        public override void OnKill()
        {
            // Check if the other exo mechs are alive
            bool exoWormAlive = false;
            bool exoPrimeAlive = false;
            if (CalamityGlobalNPC.draedonExoMechWorm != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].active)
                    exoWormAlive = true;
            }
            if (CalamityGlobalNPC.draedonExoMechPrime != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
                    exoPrimeAlive = true;
            }

            // Check for Draedon
            bool draedonAlive = false;
            if (CalamityGlobalNPC.draedon != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedon].active)
                    draedonAlive = true;
            }

            // Phase 5, when 1 mech dies and the other 2 return to fight
            if (exoWormAlive && exoPrimeAlive)
            {
                if (draedonAlive)
                {
                    Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 4f;
                    Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                }
            }

            // Phase 7, when 1 mech dies and the final one returns to the fight
            else if (exoWormAlive || exoPrimeAlive)
            {
                if (draedonAlive)
                {
                    Main.npc[CalamityGlobalNPC.draedon].localAI[0] = 6f;
                    Main.npc[CalamityGlobalNPC.draedon].ai[0] = Draedon.ExoMechPhaseDialogueTime;
                }
            }
            else
                AresBody.DoMiscDeathEffects(NPC, AresBody.MechType.ArtemisAndApollo);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => AresBody.DefineExoMechLoot(NPC, npcLoot, (int)AresBody.MechType.ArtemisAndApollo);

        public override void ModifyTypeName(ref string typeName)
        {
            if (exoMechdusa)
            {
                typeName = NameToDisplay = this.GetLocalizedValue("HekateName");
            }
        }

        // Needs edits
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1f);

            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 3;
                SoundEngine.PlaySound(CommonCalamitySounds.ExoHitSound, NPC.Center);
            }

            if (NPC.life <= 0)
            {
                for (int num193 = 0; num193 < 2; num193++)
                {
                    Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                }
                for (int i = 0; i < 20; i++)
                {
                    int plasmaDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 107, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                    Main.dust[plasmaDust].noGravity = true;
                    Main.dust[plasmaDust].velocity *= 3f;
                    plasmaDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                    Main.dust[plasmaDust].velocity *= 2f;
                    Main.dust[plasmaDust].noGravity = true;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Apollo1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Apollo2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Apollo3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Apollo4").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Apollo5").Type, 1f);
                }
            }
        }

        public override bool CheckDead()
        {
            // Kill Artemis if he's still alive when Apollo dies
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.active && nPC.type == ModContent.NPCType<Artemis.Artemis>() && nPC.life > 0)
                {
                    nPC.life = 0;
                    nPC.HitEffect();
                    nPC.checkDead();
                    nPC.active = false;
                    nPC.netUpdate = true;
                }
            }

            // Despawn projectile bullshit
            KillProjectiles();

            return true;
        }

        private void KillProjectiles()
        {
            for (int x = 0; x < Main.maxProjectiles; x++)
            {
                Projectile projectile = Main.projectile[x];
                if (projectile.active)
                {
                    if (projectile.type == ModContent.ProjectileType<ArtemisLaser>() ||
                        projectile.type == ModContent.ProjectileType<ArtemisChargeTelegraph>() ||
                        projectile.type == ModContent.ProjectileType<ApolloFireball>() ||
                        projectile.type == ModContent.ProjectileType<ApolloRocket>())
                    {
                        projectile.ai[2] = -1f;
                        projectile.Kill();
                    }
                }
            }
        }

        public override bool CheckActive() => false;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }
    }
}
