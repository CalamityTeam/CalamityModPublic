using CalamityMod.Events;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Potions;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Skies;
using CalamityMod.Sounds;
using CalamityMod.UI.VanillaBossBars;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
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

namespace CalamityMod.NPCs.ExoMechs.Artemis
{
    public class Artemis : ModNPC
    {
        public static int phase1IconIndex;
        public static int phase2IconIndex;

        public static readonly SoundStyle AttackSelectionSound = new("CalamityMod/Sounds/Custom/ExoMechs/ApolloArtemisTargetSelection") { Volume = 1.3f };

        public static readonly SoundStyle ChargeSound = new("CalamityMod/Sounds/Custom/ExoMechs/ArtemisApolloDash") { Volume = 1.2f };

        public static readonly SoundStyle ChargeTelegraphSound = new("CalamityMod/Sounds/Custom/ExoMechs/ArtemisApolloDashTelegraph") { Volume = 1.2f };

        public static readonly SoundStyle LensSound = new("CalamityMod/Sounds/Custom/ExoMechs/ExoTwinsEject") { Volume = 1.2f };

        public static readonly SoundStyle LaserShotgunSound = new("CalamityMod/Sounds/Custom/ExoMechs/ArtemisShotgunLaser") { Volume = 1.2f };

        public static readonly SoundStyle SpinLaserbeamSound = new("CalamityMod/Sounds/Custom/ExoMechs/ArtemisSpinLaserbeam") { Volume = 1.3f };

        internal static void LoadHeadIcons()
        {
            string phase1IconPath = "CalamityMod/NPCs/ExoMechs/Artemis/ArtemisHead";
            string phase2IconPath = "CalamityMod/NPCs/ExoMechs/Artemis/ArtemisPhase2Head";

            CalamityMod.Instance.AddBossHeadTexture(phase1IconPath, -1);
            phase1IconIndex = ModContent.GetModBossHeadSlot(phase1IconPath);

            CalamityMod.Instance.AddBossHeadTexture(phase2IconPath, -1);
            phase2IconIndex = ModContent.GetModBossHeadSlot(phase2IconPath);
        }

        public enum Phase
        {
            Normal = 0,
            Charge = 1,
            LaserShotgun = 2,
            Deathray = 3,
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

        // The vector used for charging
        private Vector2 chargeVelocityNormalized = default;

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

        // Total duration of the deathray telegraph
        private const float deathrayTelegraphDuration = 60f;

        // The amount of time Artemis pauses for before shooting
        private const float PauseDurationBeforeLaserActuallyFires = ArtemisLaser.TelegraphTotalTime;

        // Vector to look at after a laser has been fired
        private Vector2 pointToLookAt = default;

        // Total duration of the deathray
        private const float deathrayDuration = 180f;

        // Variable to pick a different location after each attack
        private bool pickNewLocation = false;

        // Mark Artemis as a component of the Exo Mechdusa
        public bool exoMechdusa = false;

        // The direction to spin in during spin phases
        private int rotationDirection = 0;

        // The point to spin around
        private Vector2 spinningPoint = default;

        // Velocity for the spin
        private Vector2 spinVelocity = default;

        // Intensity of flash effects during the charge combo
        public float ChargeFlash;

        //This stores the sound slot of the ML laser sound it makes, so it may be properly updated in terms of position.
        private SlotId DeathraySoundSlot;

        public static string NameToDisplay = "XS-01 Artemis";

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitPositionXOverride = -50f,
                PortraitPositionYOverride = -40f,
                PortraitScale = 0.75f,
                Scale = 0.45f,
                Rotation = MathHelper.Pi - MathHelper.PiOver4
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
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.DeathSound = CommonCalamitySounds.ExoDeathSound;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.BossBar = ModContent.GetInstance<ExoMechsBossBar>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Artemis")
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
            writer.WriteVector2(pointToLookAt);
            writer.WriteVector2(spinVelocity);
            writer.WriteVector2(chargeVelocityNormalized);
            writer.Write(frameX);
            writer.Write(frameY);
            writer.Write(pickNewLocation);
            writer.Write(exoMechdusa);
            writer.Write(rotationDirection);
            writer.WriteVector2(spinningPoint);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            velocityBoostMult = reader.ReadSingle();
            pointToLookAt = reader.ReadVector2();
            spinVelocity = reader.ReadVector2();
            chargeVelocityNormalized = reader.ReadVector2();
            frameX = reader.ReadInt32();
            frameY = reader.ReadInt32();
            pickNewLocation = reader.ReadBoolean();
            exoMechdusa = reader.ReadBoolean();
            rotationDirection = reader.ReadInt32();
            spinningPoint = reader.ReadVector2();
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            CalamityGlobalNPC.draedonExoMechTwinRed = NPC.whoAmI;

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
            bool exoTwinGreenAlive = false;
            bool exoWormAlive = false;
            bool exoPrimeAlive = false;
            bool apolloUsingChargeCombo = false;
            if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
                {
                    // Set target to Apollo's target if Apollo is alive
                    player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].target];

                    // Link the HP of both twins
                    if (NPC.life > Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life)
                        NPC.life = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life;

                    exoTwinGreenAlive = true;
                    apolloUsingChargeCombo = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[0] == 2f || Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[0] == 3f;
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

            // Used to nerf Artemis laser shotgun if Apollo is in charging phase
            bool nerfedLaserShotgun = false;
            bool canFireLasers = true;
            if (exoTwinGreenAlive)
            {
                nerfedLaserShotgun = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[0] == (float)Apollo.Apollo.Phase.LineUpChargeCombo ||
                    Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[0] == (float)Apollo.Apollo.Phase.ChargeCombo;

                // Can only fire lasers if cooldown is less than or equal to 1
                canFireLasers = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].ai[3] <= 1f;

                // Set movement according to Apollo
                if (NPC.ai[0] >= 10f)
                    NPC.ai[0] = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].ai[0];
            }

            // Check if any of the other mechs were spawned first
            bool exoWormWasFirst = false;
            bool exoPrimeWasFirst = false;
            if (exoWormAlive)
                exoWormWasFirst = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].ai[3] == 1f;
            if (exoPrimeAlive)
                exoPrimeWasFirst = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].ai[3] == 1f;
            bool otherExoMechWasFirst = exoWormWasFirst || exoPrimeWasFirst;

            // Prevent mechs from being respawned
            if (otherExoMechWasFirst)
            {
                if (NPC.ai[3] < 1f)
                    NPC.ai[3] = 1f;
            }

            // Phases
            bool phase2 = lifeRatio < 0.6f;
            bool spawnOtherExoMechs = lifeRatio < 0.7f && NPC.ai[3] == 0f;
            bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
            bool lastMechAlive = berserk && otherExoMechsAlive == 0;

            // If Artemis and Apollo don't go berserk
            bool otherMechIsBerserk = exoWormLifeRatio < 0.4f || exoPrimeLifeRatio < 0.4f;

            // Whether Artemis and Apollo should be buffed while in berserk phase
            bool shouldGetBuffedByBerserkPhase = berserk && !otherMechIsBerserk;

            // Spawn Apollo if it doesn't exist after the first 10 frames have passed
            if (NPC.ai[0] < 10f)
            {
                NPC.ai[0] += 1f;
                if (NPC.ai[0] == 10f && !NPC.AnyNPCs(ModContent.NPCType<Apollo.Apollo>()))
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Apollo.Apollo>());
            }
            else
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<Apollo.Apollo>()))
                {
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.active = false;
                    NPC.netUpdate = true;
                }
            }

            // General AI pattern
            // 0 - Fly to the left of the target and fire lasers when ready
            // 1 - Create a laser telegraph and charge extremely fast, this attack is replaced by a spread pattern of lasers in phase 2
            // 2 - Fly above the target, charge up and fire a deathray while moving in a clockwise or counterclockwise circle around the target's original position when the attack started
            // 3 - Go passive and fly to the left of the target while firing less projectiles
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
            float timeToLineUpAttack = phase2 ? 30f : 45f;

            if (Main.getGoodWorld)
                timeToLineUpAttack *= 0.5f;

            // Spin variables
            float spinRadius = 500f;
            float spinLocationDistance = 50f;
            Vector2 spinLocation = player.Center;
            switch ((int)NPC.ai[3])
            {
                // Laser from top
                case 0:
                case 1:
                    spinLocation.Y -= spinRadius;
                    break;

                // Laser from bottom
                case 2:
                    spinLocation.Y += spinRadius;
                    break;

                // Laser from left
                case 3:
                    spinRadius *= 1.7f;
                    spinLocation.X -= spinRadius;
                    break;

                // Laser from right
                case 4:
                    spinRadius *= 1.7f;
                    spinLocation.X += spinRadius;
                    break;
            }

            // Distance where Artemis stops moving
            float movementDistanceGateValue = 100f;

            // Charge variables
            float chargeVelocity = nerfedAttacks ? 60f : bossRush ? 81f : death ? 74f : revenge ? 70.5f : expertMode ? 67f : 60f;

            if (Main.getGoodWorld)
                chargeVelocity *= 1.15f;

            float chargeDistance = 2000f;
            float chargeDuration = chargeDistance / chargeVelocity;
            bool aimTowardsChargeTarget = calamityGlobalNPC.newAI[3] >= (attackPhaseGateValue - 30f + 2f) && !phase2 && AIState == (float)Phase.Normal;
            bool lineUpAttack = calamityGlobalNPC.newAI[3] >= attackPhaseGateValue + 2f;
            bool doBigAttack = calamityGlobalNPC.newAI[3] >= attackPhaseGateValue + 2f + timeToLineUpAttack;

            // Predictiveness
            float predictionAmt = 20f;
            if (aimTowardsChargeTarget)
                predictionAmt *= 2f;
            if (AIState == (float)Phase.LaserShotgun)
                predictionAmt *= 1.5f;
            if (nerfedAttacks)
                predictionAmt *= 0.5f;
            if (SecondaryAIState == (float)SecondaryPhase.Passive)
                predictionAmt *= 0.5f;

            // Velocity and acceleration values
            float baseVelocityMult = (shouldGetBuffedByBerserkPhase ? 0.25f : 0f) + (bossRush ? 1.15f : death ? 1.1f : revenge ? 1.075f : expertMode ? 1.05f : 1f);
            float baseVelocity = ((AIState == (int)Phase.Deathray || lineUpAttack || AIState == (int)Phase.LaserShotgun) ? 40f : 20f) * baseVelocityMult;
            float decelerationVelocityMult = 0.85f;

            if (Main.getGoodWorld)
                baseVelocity *= 1.5f;

            // Laser shotgun variables
            float laserShotgunDuration = lastMechAlive ? 120f : 90f;

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
                if (SecondaryAIState == (float)SecondaryPhase.Passive)
                {
                    NPC.localAI[0] *= 0.5f;
                    NPC.localAI[1] *= 0.5f;
                }

                NPC.netUpdate = true;
            }

            // Default vector to fly to
            bool flyLeft = NPC.ai[0] % 2f == 0f || NPC.ai[0] < 10f || !revenge;
            float destinationX = flyLeft ? -750f : 750f;
            float destinationY = player.Center.Y;
            Vector2 destination = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune ? new Vector2(player.Center.X + destinationX * 1.6f, destinationY) :
                SecondaryAIState == (float)SecondaryPhase.Passive ? new Vector2(player.Center.X + destinationX, destinationY + 360f) :
                AIState == (float)Phase.Deathray ? spinLocation :
                new Vector2(player.Center.X + destinationX, destinationY);

            // Add a bit of randomness to the destination, but only in specific phases where it's necessary
            if (AIState == (float)Phase.Normal || AIState == (float)Phase.LaserShotgun || AIState == (float)Phase.PhaseTransition)
            {
                destination.X += NPC.localAI[0];
                destination.Y += NPC.localAI[1];
            }

            // Scale up velocity over time if too far from destination
            Vector2 distanceFromDestination = destination - NPC.Center;
            if (distanceFromDestination.Length() > movementDistanceGateValue && AIState != (float)Phase.Charge)
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

            // If Artemis can fire projectiles, cannot fire if too close to the target
            bool canFire = distanceFromDestination.Length() <= 320f && canFireLasers;

            // Rotation
            Vector2 predictionVector = AIState == (float)Phase.Deathray ? Vector2.Zero : player.velocity * predictionAmt;
            Vector2 aimedVector = player.Center + predictionVector - NPC.Center;
            float rateOfRotation = 0.1f;
            Vector2 rotateTowards = player.Center - NPC.Center;
            bool stopRotatingAndSlowDown = !phase2 && AIState == (float)Phase.Normal && lineUpAttack;
            if (!stopRotatingAndSlowDown)
            {
                if (AIState == (int)Phase.Charge)
                {
                    rateOfRotation = 0f;
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                }
                else if (spinningPoint != default)
                {
                    rateOfRotation = 0f;
                    float x = spinningPoint.X - NPC.Center.X;
                    float y = spinningPoint.Y - NPC.Center.Y;
                    NPC.rotation = (float)Math.Atan2(y, x) + MathHelper.PiOver2;
                }
                else if (pointToLookAt != default)
                {
                    rateOfRotation = 0f;
                    float x = pointToLookAt.X - NPC.Center.X;
                    float y = pointToLookAt.Y - NPC.Center.Y;
                    NPC.rotation = (float)Math.Atan2(y, x) + MathHelper.PiOver2;
                }
                else
                {
                    float x = player.Center.X + predictionVector.X - NPC.Center.X;
                    float y = player.Center.Y + predictionVector.Y - NPC.Center.Y;
                    rotateTowards = Vector2.Normalize(new Vector2(x, y)) * baseVelocity;
                }

                // Do not set this during charge or deathray phases
                if (rateOfRotation != 0f)
                    NPC.rotation = NPC.rotation.AngleTowards((float)Math.Atan2(rotateTowards.Y, rotateTowards.X) + MathHelper.PiOver2, rateOfRotation);
            }

            // Despawn if target is dead
            if (player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (player.dead)
                {
                    AIState = (float)Phase.Normal;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.localAI[0] = 0f;
                    NPC.localAI[1] = 0f;
                    NPC.localAI[2] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;
                    rotationDirection = 0;
                    chargeVelocityNormalized = default;
                    spinningPoint = default;
                    spinVelocity = default;
                    NPC.dontTakeDamage = true;

                    NPC.velocity.Y -= 1f;
                    if ((double)NPC.position.Y < Main.topWorld + 16f)
                        NPC.velocity.Y -= 1f;

                    if ((double)NPC.position.Y < Main.topWorld + 16f)
                    {
                        for (int a = 0; a < Main.maxNPCs; a++)
                        {
                            if (Main.npc[a].type == NPC.type || Main.npc[a].type == ModContent.NPCType<Apollo.Apollo>() || Main.npc[a].type == ModContent.NPCType<AresBody>() ||
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

            // Duration of deathray spin to do a full circle
            // Normal = 120, Expert = 104, Rev = 96, Death = 88, Boss Rush = 72
            float spinTime = 120f - 320f * (baseVelocityMult - 1.25f);

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

            // Variable for charge flash. Changed in parts of the AI below
            bool shouldDoChargeFlash = false;

            // Passive and Immune phases
            switch ((int)SecondaryAIState)
            {
                case (int)SecondaryPhase.Nothing:
                    break;

                // Fire projectiles less often
                case (int)SecondaryPhase.Passive:
                    break;

                // Fly above target and become immune
                case (int)SecondaryPhase.PassiveAndImmune:
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
                // Fly to the left of the target
                case (int)Phase.Normal:

                    if (!stopRotatingAndSlowDown)
                    {
                        // Set charge variable to default
                        chargeVelocityNormalized = default;

                        // Smooth movement towards the location Artemis is meant to be at
                        CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);
                    }
                    else
                    {
                        // Save the normalized charge velocity for use in the charge phase
                        if (chargeVelocityNormalized == default)
                            chargeVelocityNormalized = Vector2.Normalize(aimedVector);

                        // Decelerate
                        NPC.velocity *= decelerationVelocityMult;
                    }

                    // Default animation for 60 frames and then go to telegraph animation
                    // newAI[3] tells Artemis what animation state it's currently in
                    bool attacking = calamityGlobalNPC.newAI[3] >= 2f;
                    bool firingLasers = attacking && calamityGlobalNPC.newAI[3] + 2f < attackPhaseGateValue;

                    // Only increase attack timer if not in immune phase
                    if (SecondaryAIState != (float)SecondaryPhase.PassiveAndImmune)
                        calamityGlobalNPC.newAI[2] += 1f;

                    if (calamityGlobalNPC.newAI[2] >= defaultAnimationDuration || attacking)
                    {
                        if (firingLasers)
                        {
                            // Fire lasers or swap to a new location
                            float divisor = nerfedAttacks ? 45f : lastMechAlive ? 25f : 30f;
                            float laserTimer = calamityGlobalNPC.newAI[3] - 2f;
                            if (laserTimer % divisor == 0f && canFire)
                            {
                                if (laserTimer % (divisor * 2f) == 0f)
                                {
                                    pointToLookAt = default;
                                    pickNewLocation = true;
                                }
                                else
                                {
                                    Vector2 laserVelocity = Vector2.Normalize(aimedVector);
                                    Vector2 projectileDestination = player.Center + predictionVector;
                                    pointToLookAt = projectileDestination;
                                    SoundEngine.PlaySound(CommonCalamitySounds.ExoLaserShootSound, NPC.Center);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int type = ModContent.ProjectileType<ArtemisLaser>();
                                        int damage = NPC.GetProjectileDamage(type);
                                        Vector2 offset = laserVelocity * 70f;
                                        float setVelocityInAI = 7.5f;
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, projectileDestination, type, damage, 0f, Main.myPlayer, setVelocityInAI, NPC.whoAmI);
                                    }
                                }
                            }
                        }
                        else
                        {
                            pointToLookAt = default;
                            calamityGlobalNPC.newAI[2] = 0f;
                        }

                        // Enter charge phase after a certain time has passed
                        // This is replaced by a laser shotgun in phase 2
                        // Enter deathray phase if in phase 2 and the localAI[2] variable is set to do so
                        calamityGlobalNPC.newAI[3] += 1f;
                        if (lineUpAttack)
                        {
                            // Return to normal laser phase if in passive state
                            pointToLookAt = default;
                            if (SecondaryAIState == (float)SecondaryPhase.Passive)
                            {
                                pickNewLocation = true;
                                calamityGlobalNPC.newAI[2] = 0f;
                                calamityGlobalNPC.newAI[3] = 0f;
                                chargeVelocityNormalized = default;
                            }
                            else
                            {
                                // Draw a large laser telegraph for the charge
                                if (!phase2 && calamityGlobalNPC.newAI[3] == attackPhaseGateValue + 5f)
                                {
                                    // And allow the charge flash effect
                                    shouldDoChargeFlash = true;

                                    // Play a sound to accompany the telegraph.
                                    SoundEngine.PlaySound(ChargeTelegraphSound, NPC.Center);

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int type = ModContent.ProjectileType<ArtemisChargeTelegraph>();
                                        Vector2 laserVelocity = Vector2.Normalize(aimedVector);
                                        Vector2 offset = laserVelocity * 50f;
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, laserVelocity, type, 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                    }
                                }

                                // Fire a spread of projectiles in the direction of the charge
                                if (!phase2 && calamityGlobalNPC.newAI[3] == attackPhaseGateValue + 2f + (timeToLineUpAttack - 30f))
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int type = ModContent.ProjectileType<ArtemisLaser>();
                                        int damage = NPC.GetProjectileDamage(type);
                                        Vector2 laserVelocity = chargeVelocityNormalized * 10f;
                                        int numLasersPerSpread = bossRush ? 10 : death ? 8 : expertMode ? 6 : 4;
                                        int spread = bossRush ? 30 : death ? 26 : expertMode ? 21 : 15;
                                        float rotation = MathHelper.ToRadians(spread);
                                        float distanceFromTarget = Vector2.Distance(NPC.Center, NPC.Center + chargeVelocityNormalized * chargeDistance);
                                        float setVelocityInAI = death ? 7f : revenge ? 6.75f : expertMode ? 6.5f : 6f;

                                        for (int i = 0; i < numLasersPerSpread + 1; i++)
                                        {
                                            Vector2 perturbedSpeed = laserVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numLasersPerSpread - 1)));
                                            Vector2 normalizedPerturbedSpeed = Vector2.Normalize(perturbedSpeed);

                                            Vector2 offset = normalizedPerturbedSpeed * 70f;
                                            Vector2 newCenter = NPC.Center + offset;

                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), newCenter, newCenter + normalizedPerturbedSpeed * distanceFromTarget, type, damage, 0f, Main.myPlayer, setVelocityInAI, NPC.whoAmI);
                                        }
                                    }
                                }

                                if (doBigAttack)
                                {
                                    calamityGlobalNPC.newAI[3] = 0f;
                                    if (phase2)
                                    {
                                        AIState = (NPC.localAI[2] == 1f && (!apolloUsingChargeCombo || Main.zenithWorld)) ? (float)Phase.Deathray : (float)Phase.LaserShotgun;
                                    }
                                    else
                                    {
                                        // Charge until a certain distance is reached and then return to normal phase
                                        SoundEngine.PlaySound(ChargeSound, NPC.Center);
                                        AIState = (float)Phase.Charge;

                                        // Set charge velocity
                                        NPC.velocity = chargeVelocityNormalized * chargeVelocity;
                                        chargeVelocityNormalized = default;
                                    }
                                }
                            }
                        }
                    }

                    break;

                // Charge
                case (int)Phase.Charge:

                    // Allow the charge flash to happen
                    shouldDoChargeFlash = true;

                    // Reset phase and variables
                    calamityGlobalNPC.newAI[2] += 1f;
                    if (calamityGlobalNPC.newAI[2] >= chargeDuration)
                    {
                        // Decelerate
                        NPC.velocity *= decelerationVelocityMult;

                        // Go back to normal phase
                        if (calamityGlobalNPC.newAI[2] >= chargeDuration + 10f)
                        {
                            pickNewLocation = true;
                            AIState = (float)Phase.Normal;
                            calamityGlobalNPC.newAI[2] = 0f;
                        }
                    }

                    break;

                // Laser shotgun barrage
                case (int)Phase.LaserShotgun:

                    // Smooth movement towards the location Artemis is meant to be at
                    CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);

                    // Fire lasers
                    int numSpreads = lastMechAlive ? 3 : 2;
                    float divisor2 = laserShotgunDuration / numSpreads;
                    if (calamityGlobalNPC.newAI[2] % divisor2 == 0f && canFire && calamityGlobalNPC.newAI[2] < laserShotgunDuration)
                    {
                        SoundEngine.PlaySound(LaserShotgunSound, NPC.Center);

                        Vector2 laserVelocity = Vector2.Normalize(aimedVector) * 10f;

                        int type = ModContent.ProjectileType<ArtemisLaser>();
                        int damage = NPC.GetProjectileDamage(type);

                        /* Spread:
                         * lastMechAlive = 20, 25, 30
                         * normal = 16, 20, 24
                         * nerfedAttacks = 12, 15, 18
                         */
                        int numLasersAddedByDifficulty = bossRush ? 3 : death ? 2 : expertMode ? 1 : 0;
                        int numLasersPerSpread = ((nerfedAttacks || nerfedLaserShotgun) ? 3 : lastMechAlive ? 7 : 5) + numLasersAddedByDifficulty;
                        int baseSpread = ((nerfedAttacks || nerfedLaserShotgun) ? 9 : lastMechAlive ? 20 : 15) + numLasersAddedByDifficulty * 2;
                        int spread = baseSpread + (int)(calamityGlobalNPC.newAI[2] / divisor2) * (baseSpread / 4);
                        float rotation = MathHelper.ToRadians(spread);
                        float distanceFromTarget = Vector2.Distance(NPC.Center, player.Center + predictionVector);
                        float setVelocityInAI = death ? 6.5f : revenge ? 6.25f : expertMode ? 6f : 5.5f;
                        pointToLookAt = player.Center + predictionVector;

                        for (int i = 0; i < numLasersPerSpread + 1; i++)
                        {
                            Vector2 perturbedSpeed = laserVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numLasersPerSpread - 1)));
                            Vector2 normalizedPerturbedSpeed = Vector2.Normalize(perturbedSpeed);

                            Vector2 offset = normalizedPerturbedSpeed * 70f;
                            Vector2 newCenter = NPC.Center + offset;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), newCenter, newCenter + normalizedPerturbedSpeed * distanceFromTarget, type, damage, 0f, Main.myPlayer, setVelocityInAI, NPC.whoAmI);
                        }
                    }

                    // Reset phase and variables
                    if (canFire || calamityGlobalNPC.newAI[2] > 0f)
                        calamityGlobalNPC.newAI[2] += 1f;

                    if (calamityGlobalNPC.newAI[2] >= (laserShotgunDuration + PauseDurationBeforeLaserActuallyFires))
                    {
                        pointToLookAt = default;
                        pickNewLocation = true;
                        AIState = (float)Phase.Normal;
                        NPC.localAI[2] = shouldGetBuffedByBerserkPhase ? 1f : 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                    }

                    break;

                // Fly above target, fire deathray and move in a circle around the target
                case (int)Phase.Deathray:

                    // Fly above, stop doing this if in the proper position
                    // Stop rotating and spin around a target point
                    if ((destination - NPC.Center).Length() < spinLocationDistance || calamityGlobalNPC.newAI[2] > 0f)
                    {
                        // Draw telegraph for deathray
                        if (calamityGlobalNPC.newAI[2] == 0f)
                        {
                            NPC.velocity = Vector2.Zero;

                            switch ((int)NPC.ai[3])
                            {
                                // Laser from top
                                case 0:
                                case 1:
                                    spinningPoint = NPC.Center + Vector2.UnitY * spinRadius;
                                    break;

                                // Laser from bottom
                                case 2:
                                    spinningPoint = NPC.Center - Vector2.UnitY * spinRadius;
                                    break;

                                // Laser from left
                                case 3:
                                    spinningPoint = NPC.Center + Vector2.UnitX * spinRadius;
                                    break;

                                // Laser from right
                                case 4:
                                    spinningPoint = NPC.Center - Vector2.UnitX * spinRadius;
                                    break;
                            }

                            NPC.ai[1] = spinningPoint.X;
                            NPC.ai[2] = spinningPoint.Y;

                            SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound, NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ModContent.ProjectileType<ArtemisDeathrayTelegraph>();
                                Vector2 laserVelocity = Vector2.Normalize(spinningPoint - NPC.Center);
                                Vector2 offset = laserVelocity * 70f;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, laserVelocity, type, 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                            }

                            NPC.netUpdate = true;
                        }

                        // Fire deathray and spin
                        calamityGlobalNPC.newAI[2] += 1f;
                        if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration)
                        {
                            if (rotationDirection == 0)
                            {
                                // Set spin velocity
                                spinVelocity.X = MathHelper.Pi * spinRadius / spinTime;

                                // Set spin direction
                                switch ((int)NPC.ai[3])
                                {
                                    // Laser from top
                                    case 0:
                                    case 1:
                                        if (player.Center.X >= NPC.Center.X)
                                            rotationDirection = 1;
                                        else
                                            rotationDirection = -1;
                                        break;

                                    // Laser from bottom
                                    case 2:
                                        if (player.Center.X >= NPC.Center.X)
                                            rotationDirection = -1;
                                        else
                                            rotationDirection = 1;
                                        spinVelocity = -spinVelocity;
                                        break;

                                    // Laser from left
                                    case 3:
                                        if (player.Center.Y >= NPC.Center.Y)
                                            rotationDirection = -1;
                                        else
                                            rotationDirection = 1;
                                        spinVelocity = spinVelocity.RotatedBy(-MathHelper.PiOver2);
                                        break;

                                    // Laser from right
                                    case 4:
                                        if (player.Center.Y >= NPC.Center.Y)
                                            rotationDirection = 1;
                                        else
                                            rotationDirection = -1;
                                        spinVelocity = spinVelocity.RotatedBy(MathHelper.PiOver2);
                                        break;
                                }

                                spinVelocity *= -rotationDirection;
                                NPC.netUpdate = true;

                                // Create a bunch of lightning bolts in the sky
                                ExoMechsSky.CreateLightningBolt(12);

                                // Fire deathray
                                DeathraySoundSlot = SoundEngine.PlaySound(SpinLaserbeamSound, NPC.Center);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = ModContent.ProjectileType<ArtemisSpinLaserbeam>();
                                    int damage = NPC.GetProjectileDamage(type);
                                    int laser = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, type, damage, 0f, Main.myPlayer, NPC.whoAmI);
                                    if (Main.projectile.IndexInRange(laser))
                                    {
                                        Main.projectile[laser].ai[0] = NPC.whoAmI;
                                        Main.projectile[laser].ai[1] = rotationDirection;
                                    }
                                }
                            }
                            else
                            {
                                // This first variable is used to adjust how long it takes for the rotation rate to reach max
                                float rotationSpeedMult = bossRush ? 6.66f : death ? 4f : revenge ? 3f : expertMode ? 2.5f : 2f;

                                // This is used to adjust both the radians and the velocity of the spin moved per frame
                                // At 15% progress it will be at max rotation in Boss Rush
                                // At 25% progress it will be at max rotation in Death Mode
                                // At 33% progress it will be at max rotation in Rev Mode
                                // At 40% progress it will be at max rotation in Expert Mode
                                // At 50% progress it will be at max rotation in Normal Mode
                                float rotationMult = (calamityGlobalNPC.newAI[2] - deathrayTelegraphDuration) / deathrayDuration * rotationSpeedMult;

                                // Max rotation
                                if (rotationMult > 1f)
                                {
                                    // The radians moved per frame during the spin
                                    double radiansOfRotation = MathHelper.Pi / spinTime * -rotationDirection;
                                    NPC.velocity = NPC.velocity.RotatedBy(radiansOfRotation);
                                }
                                else
                                {
                                    // The radians moved per frame during the spin
                                    // The radians moved are reduced early on to avoid spinning too fucking fast
                                    float decelerationMult = rotationMult * rotationMult;
                                    double radiansOfRotation = MathHelper.Pi / spinTime * -rotationDirection * decelerationMult;
                                    NPC.velocity = (spinVelocity * decelerationMult).RotatedBy(radiansOfRotation);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Smooth movement towards the location Artemis is meant to be at
                        CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);
                    }

                    // Reset phase and variables
                    if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration + deathrayDuration)
                    {
                        if (Main.zenithWorld && !exoMechdusa)
                        {
                            calamityGlobalNPC.newAI[3] = 0f;
                            AIState = (float)Phase.Deathray;
                            for (int i = 0; i < Main.maxProjectiles; i ++)
                            {
                                if (Main.projectile[i].type == ModContent.ProjectileType<ArtemisSpinLaserbeam>())
                                {
                                    Main.projectile[i].active = false;
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            AIState = (float)Phase.Normal;
                        }
                        spinVelocity = default;
                        rotationDirection = 0;
                        spinningPoint = default;
                        pickNewLocation = true;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        if (revenge)
                        {
                            // Can use all 4 types of lasers in rev+
                            switch ((int)NPC.ai[3])
                            {
                                case 0:
                                case 1:
                                    NPC.ai[3] = 3f + Main.rand.Next(2);
                                    break;
                                case 2:
                                    NPC.ai[3] = 3f + Main.rand.Next(2);
                                    break;
                                case 3:
                                    NPC.ai[3] = 1f + Main.rand.Next(2);
                                    break;
                                case 4:
                                    NPC.ai[3] = 1f + Main.rand.Next(2);
                                    break;
                            }
                        }
                        else if (expertMode)
                        {
                            // Can only use the top and bottom lasers in expert
                            NPC.ai[3] += 1f;
                            if (NPC.ai[3] > 2f)
                                NPC.ai[3] = 1f;
                        }
                        NPC.localAI[2] = 0f;
                        calamityGlobalNPC.newAI[2] = 0f;
                        NPC.netUpdate = true;
                    }

                    break;

                // Phase transition animation, that's all this exists for
                case (int)Phase.PhaseTransition:

                    // Smooth movement towards the location Artemis is meant to be at
                    CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, 0f, false);

                    // Shoot lens gore at the target at the proper time
                    if (calamityGlobalNPC.newAI[2] == lensPopTime)
                    {
                        SoundEngine.PlaySound(LensSound, NPC.Center);
                        Vector2 lensDirection = Vector2.Normalize(aimedVector);
                        Vector2 offset = lensDirection * 70f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, lensDirection * 24f, ModContent.ProjectileType<BrokenArtemisLens>(), 0, 0f);
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
                        chargeVelocityNormalized = default;
                    }

                    break;
            }

            // Update the charge flash variable
            ChargeFlash = MathHelper.Clamp(ChargeFlash + shouldDoChargeFlash.ToDirectionInt() * 0.08f, 0f, 1f);

            // Update the deathray sound position if it's being played.
            if (SoundEngine.TryGetActiveSound(DeathraySoundSlot, out var deathraySound) && deathraySound.IsPlaying)
                deathraySound.Position = NPC.Center;

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
                        if (NPC.Calamity().newAI[0] != (float)Phase.Charge && NPC.Calamity().newAI[0] != (float)Phase.Deathray)
                        {
                            Vector2 pos = new Vector2(aresin.Center.X + twinoffset - extratwinoffset, aresin.Center.Y - twinheight);
                            NPC.position = pos;
                        }
                    }
                }
            }
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

            return minDist <= 100f && NPC.Opacity == 1f && AIState == (float)Phase.Charge;
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
                else if (AIState == (float)Phase.Charge || AIState == (float)Phase.LaserShotgun || AIState == (float)Phase.Deathray)
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

        public float FlameTrailWidthFunction(float completionRatio) => MathHelper.SmoothStep(21f, 8f, completionRatio) * ChargeFlash;

        public float FlameTrailWidthFunctionBig(float completionRatio) => MathHelper.SmoothStep(34f, 12f, completionRatio) * ChargeFlash;

        public float RibbonTrailWidthFunction(float completionRatio)
        {
            float baseWidth = Utils.GetLerpValue(1f, 0.54f, completionRatio, true) * 5f;
            float endTipWidth = CalamityUtils.Convert01To010(Utils.GetLerpValue(0.96f, 0.89f, completionRatio, true)) * 2.4f;
            return baseWidth + endTipWidth;
        }

        public Color FlameTrailColorFunction(float completionRatio)
        {
            float trailOpacity = Utils.GetLerpValue(0.8f, 0.27f, completionRatio, true) * Utils.GetLerpValue(0f, 0.067f, completionRatio, true);
            Color startingColor = Color.Lerp(Color.White, Color.Cyan, 0.27f);
            Color middleColor = Color.Lerp(Color.Orange, Color.Yellow, 0.31f);
            Color endColor = Color.OrangeRed;
            return CalamityUtils.MulticolorLerp(completionRatio, startingColor, middleColor, endColor) * ChargeFlash * trailOpacity;
        }

        public Color FlameTrailColorFunctionBig(float completionRatio)
        {
            float trailOpacity = Utils.GetLerpValue(0.8f, 0.27f, completionRatio, true) * Utils.GetLerpValue(0f, 0.067f, completionRatio, true) * 0.56f;
            Color startingColor = Color.Lerp(Color.White, Color.Cyan, 0.25f);
            Color middleColor = Color.Lerp(Color.Blue, Color.White, 0.35f);
            Color endColor = Color.Lerp(Color.DarkBlue, Color.White, 0.47f);
            Color color = CalamityUtils.MulticolorLerp(completionRatio, startingColor, middleColor, endColor) * ChargeFlash * trailOpacity;
            color.A = 0;
            return color;
        }

        public Color RibbonTrailColorFunction(float completionRatio)
        {
            Color startingColor = new Color(34, 40, 48);
            Color endColor = new Color(219, 82, 28);
            return Color.Lerp(startingColor, endColor, (float)Math.Pow(completionRatio, 1.5D)) * NPC.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Prepare the flame trail shader with its map texture.
            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));

            int numAfterimages = ChargeFlash > 0f ? 0 : 5;
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
                        spriteBatch.Draw(texture, afterimageCenter, NPC.frame, afterimageColor, NPC.oldRot[i], origin, NPC.scale, SpriteEffects.None, 0f);
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

            int instanceCount = (int)MathHelper.Lerp(1f, 15f, ChargeFlash);
            Color baseInstanceColor = Color.Lerp(drawColor, Color.White, ChargeFlash);
            baseInstanceColor.A = (byte)(int)(255f - ChargeFlash * 255f);

            if (!NPC.IsABestiaryIconDummy)
                spriteBatch.EnterShaderRegion();

            drawInstance(Vector2.Zero, baseInstanceColor);
            if (instanceCount > 1)
            {
                baseInstanceColor *= 0.04f;
                float backAfterimageOffset = MathHelper.SmoothStep(0f, 2f, ChargeFlash);
                for (int i = 0; i < instanceCount; i++)
                {
                    Vector2 drawOffset = (MathHelper.TwoPi * i / instanceCount + Main.GlobalTimeWrappedHourly * 0.8f).ToRotationVector2() * backAfterimageOffset;
                    drawInstance(drawOffset, baseInstanceColor);
                }
            }

            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ExoMechs/Artemis/ArtemisGlow").Value;
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
                    spriteBatch.Draw(texture, afterimageCenter, NPC.frame, afterimageColor, NPC.oldRot[i], origin, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.Draw(texture, center, frame, Color.White * NPC.Opacity, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

            if (!NPC.IsABestiaryIconDummy)
                spriteBatch.ExitShaderRegion();

            // Draw a flame trail on the thrusters if needed. This happens during charges.
            if (ChargeFlash > 0f)
            {
                for (int direction = -1; direction <= 1; direction++)
                {
                    Vector2 baseDrawOffset = new Vector2(0f, direction == 0f ? 18f : 60f).RotatedBy(NPC.rotation);
                    baseDrawOffset += new Vector2(direction * 64f, 0f).RotatedBy(NPC.rotation);

                    float backFlameLength = direction == 0f ? 700f : 190f;
                    Vector2 drawStart = NPC.Center + baseDrawOffset;
                    Vector2 drawEnd = drawStart - (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * ChargeFlash * backFlameLength;
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
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                }
                for (int j = 0; j < 20; j++)
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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Artemis1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Artemis2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Artemis3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Artemis4").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Artemis5").Type, 1f);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }

        public override bool CheckDead()
        {
            // Kill Apollo if he's still alive when Artemis dies
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.active && nPC.type == ModContent.NPCType<Apollo.Apollo>() && nPC.life > 0)
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

        public override bool CheckActive() => false;

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }
    }
}
