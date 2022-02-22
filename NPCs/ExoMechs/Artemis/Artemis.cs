using CalamityMod.Events;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Skies;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Artemis
{
	public class Artemis : ModNPC
    {
		public static int phase1IconIndex;
		public static int phase2IconIndex;

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
			get => npc.Calamity().newAI[0];
			set => npc.Calamity().newAI[0] = value;
		}

		public enum SecondaryPhase
		{
			Nothing = 0,
			Passive = 1,
			PassiveAndImmune = 2
		}

		public float SecondaryAIState
		{
			get => npc.Calamity().newAI[1];
			set => npc.Calamity().newAI[1] = value;
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

		// Total duration of the deathray
		private const float deathrayDuration = 180f;

		// Variable to pick a different location after each attack
		private bool pickNewLocation = false;

		// The direction to spin in during spin phases
		private int rotationDirection = 0;

		// The point to spin around
		private Vector2 spinningPoint = default;

		// Velocity for the spin
		private Vector2 spinVelocity = default;

		// Intensity of flash effects during the charge combo
		public float ChargeFlash;

		// Primitive trail drawers for thrusters when charging
		public PrimitiveTrail ChargeFlameTrail = null;
		public PrimitiveTrail ChargeFlameTrailBig = null;

		// Primitive trail drawer for the ribbon things
		public PrimitiveTrail RibbonTrail = null;

		public const string NameToDisplay = "XS-01 Artemis";

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(NameToDisplay);
			NPCID.Sets.TrailingMode[npc.type] = 3;
			NPCID.Sets.TrailCacheLength[npc.type] = 15;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 5f;
			npc.GetNPCDamage();
			npc.width = 204;
            npc.height = 226;
            npc.defense = 80;
			npc.DR_NERD(0.25f);
			npc.LifeMaxNERB(1250000, 1495000, 500000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
            aiType = -1;
			npc.Opacity = 0f;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
			npc.boss = true;
			music = CalamityMod.Instance.GetMusicFromMusicMod("ExoMechs") ?? MusicID.Boss3;
			npc.Calamity().VulnerableToSickness = false;
			npc.Calamity().VulnerableToElectricity = true;
		}

		public override void BossHeadSlot(ref int index)
		{
			if (SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune)
				index = -1;
			else if (npc.life / (float)npc.lifeMax < 0.6f)
				index = phase2IconIndex;
			else
				index = phase1IconIndex;
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(velocityBoostMult);
			writer.WriteVector2(spinVelocity);
			writer.WriteVector2(chargeVelocityNormalized);
			writer.Write(frameX);
			writer.Write(frameY);
			writer.Write(pickNewLocation);
			writer.Write(rotationDirection);
			writer.WriteVector2(spinningPoint);
            writer.Write(npc.dontTakeDamage);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
			writer.Write(npc.localAI[2]);
			writer.Write(npc.localAI[3]);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			velocityBoostMult = reader.ReadSingle();
			spinVelocity = reader.ReadVector2();
			chargeVelocityNormalized = reader.ReadVector2();
			frameX = reader.ReadInt32();
			frameY = reader.ReadInt32();
			pickNewLocation = reader.ReadBoolean();
			rotationDirection = reader.ReadInt32();
			spinningPoint = reader.ReadVector2();
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
			npc.localAI[2] = reader.ReadSingle();
			npc.localAI[3] = reader.ReadSingle();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

        public override void AI()
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			CalamityGlobalNPC.draedonExoMechTwinRed = npc.whoAmI;

			npc.frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			// Target variable
			Player player = Main.player[npc.target];

			// Check if the other exo mechs are alive
			int otherExoMechsAlive = 0;
			bool exoTwinGreenAlive = false;
			bool exoWormAlive = false;
			bool exoPrimeAlive = false;
			if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
				{
					// Set target to Apollo's target if Apollo is alive
					player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].target];

					// Link the HP of both twins
					if (npc.life > Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life)
						npc.life = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life;

					exoTwinGreenAlive = true;
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
			float lifeRatio = npc.life / (float)npc.lifeMax;

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
				if (npc.ai[0] >= 10f)
					npc.ai[0] = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].ai[0];
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
				if (npc.ai[3] < 1f)
					npc.ai[3] = 1f;
			}

			// Phases
			bool phase2 = lifeRatio < 0.6f;
			bool spawnOtherExoMechs = lifeRatio < 0.7f && npc.ai[3] == 0f;
			bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
			bool lastMechAlive = berserk && otherExoMechsAlive == 0;

			// If Artemis and Apollo don't go berserk
			bool otherMechIsBerserk = exoWormLifeRatio < 0.4f || exoPrimeLifeRatio < 0.4f;

			// Whether Artemis and Apollo should be buffed while in berserk phase
			bool shouldGetBuffedByBerserkPhase = berserk && !otherMechIsBerserk;

			// Spawn Apollo if it doesn't exist after the first 10 frames have passed
			if (npc.ai[0] < 10f)
			{
				npc.ai[0] += 1f;
				if (npc.ai[0] == 10f && !NPC.AnyNPCs(ModContent.NPCType<Apollo.Apollo>()))
					NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Apollo.Apollo>());
			}
			else
			{
				if (!NPC.AnyNPCs(ModContent.NPCType<Apollo.Apollo>()))
				{
					npc.active = false;
					npc.netUpdate = true;
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

			// Predictiveness
			float predictionAmt = malice ? 21f : death ? 18f : revenge ? 16.5f : expertMode ? 15f : 12f;
			if (AIState == (float)Phase.LaserShotgun)
				predictionAmt *= 1.5f;
			if (nerfedAttacks)
				predictionAmt *= 0.5f;
			if (SecondaryAIState == (float)SecondaryPhase.Passive)
				predictionAmt *= 0.5f;

			// Gate values
			float reducedTimeForGateValue = malice ? 48f : death ? 32f : revenge ? 24f : expertMode ? 16f : 0f;
			float reducedTimeForGateValue_Berserk = reducedTimeForGateValue * 0.5f;
			float normalAttackTime = 360f - reducedTimeForGateValue;
			float berserkAttackTime = lastMechAlive ? 225f - reducedTimeForGateValue_Berserk : 270f - reducedTimeForGateValue_Berserk;
			float attackPhaseGateValue = shouldGetBuffedByBerserkPhase ? berserkAttackTime : normalAttackTime;
			float timeToLineUpAttack = phase2 ? 30f : 45f;

			// Spin variables
			float spinRadius = 500f;
			float spinLocationDistance = 50f;
			Vector2 spinLocation = player.Center;
			switch ((int)npc.ai[3])
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
			float chargeVelocity = nerfedAttacks ? 75f : malice ? 100f : death ? 90f : revenge ? 86.25f : expertMode ? 82.5f : 75f;
			float chargeDistance = 2000f;
			float chargeDuration = chargeDistance / chargeVelocity;
			bool lineUpAttack = calamityGlobalNPC.newAI[3] >= attackPhaseGateValue + 2f;
			bool doBigAttack = calamityGlobalNPC.newAI[3] >= attackPhaseGateValue + 2f + timeToLineUpAttack;

			// Velocity and acceleration values
			float baseVelocityMult = (shouldGetBuffedByBerserkPhase ? 0.25f : 0f) + (malice ? 1.15f : death ? 1.1f : revenge ? 1.075f : expertMode ? 1.05f : 1f);
			float baseVelocity = ((AIState == (int)Phase.Deathray || lineUpAttack || AIState == (int)Phase.LaserShotgun) ? 40f : 20f) * baseVelocityMult;
			float decelerationVelocityMult = 0.85f;

			// Laser shotgun variables
			float laserShotgunDuration = lastMechAlive ? 120f : 90f;

			// If Artemis can fire projectiles, cannot fire if too close to the target
			bool canFire = Vector2.Distance(npc.Center, player.Center) > 320f && canFireLasers;

			// Add some random distance to the destination after certain attacks
			if (pickNewLocation)
			{
				pickNewLocation = false;

				npc.localAI[0] = Main.rand.Next(-50, 51);
				npc.localAI[1] = Main.rand.Next(-250, 251);
				if (SecondaryAIState == (float)SecondaryPhase.Passive)
				{
					npc.localAI[0] *= 0.5f;
					npc.localAI[1] *= 0.5f;
				}

				npc.netUpdate = true;
			}

			// Default vector to fly to
			bool flyLeft = npc.ai[0] % 2f == 0f || npc.ai[0] < 10f || !revenge;
			float destinationX = flyLeft ? -750f : 750f;
			float destinationY = player.Center.Y;
			Vector2 destination = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune ? new Vector2(player.Center.X + destinationX * 1.6f, destinationY) :
				SecondaryAIState == (float)SecondaryPhase.Passive ? new Vector2(player.Center.X + destinationX, destinationY + 360f) :
				AIState == (float)Phase.Deathray ? spinLocation :
				new Vector2(player.Center.X + destinationX, destinationY);

			// Add a bit of randomness to the destination, but only in specific phases where it's necessary
			if (AIState == (float)Phase.Normal || AIState == (float)Phase.LaserShotgun || AIState == (float)Phase.PhaseTransition)
			{
				destination.X += npc.localAI[0];
				destination.Y += npc.localAI[1];
			}

			// Scale up velocity over time if too far from destination
			Vector2 distanceFromDestination = destination - npc.Center;
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

			// Rotation
			Vector2 predictionVector = AIState == (float)Phase.Deathray ? Vector2.Zero : player.velocity * predictionAmt;
			Vector2 aimedVector = player.Center + predictionVector - npc.Center;
			float rateOfRotation = 0.1f;
			Vector2 rotateTowards = player.Center - npc.Center;
			bool stopRotatingAndSlowDown = !phase2 && AIState == (float)Phase.Normal && lineUpAttack;
			if (!stopRotatingAndSlowDown)
			{
				if (AIState == (int)Phase.Charge)
				{
					rateOfRotation = 0f;
					npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
				}
				else if (spinningPoint != default)
				{
					rateOfRotation = 0f;
					float x = spinningPoint.X - npc.Center.X;
					float y = spinningPoint.Y - npc.Center.Y;
					npc.rotation = (float)Math.Atan2(y, x) + MathHelper.PiOver2;
				}
				else
				{
					float x = player.Center.X + predictionVector.X - npc.Center.X;
					float y = player.Center.Y + predictionVector.Y - npc.Center.Y;
					rotateTowards = Vector2.Normalize(new Vector2(x, y)) * baseVelocity;
				}

				// Do not set this during charge or deathray phases
				if (rateOfRotation != 0f)
					npc.rotation = npc.rotation.AngleTowards((float)Math.Atan2(rotateTowards.Y, rotateTowards.X) + MathHelper.PiOver2, rateOfRotation);
			}

			// Light
			Lighting.AddLight(npc.Center, 0.25f * npc.Opacity, 0.15f * npc.Opacity, 0.05f * npc.Opacity);

			// Despawn if target is dead
			if (player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (player.dead)
				{
					AIState = (float)Phase.Normal;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.localAI[0] = 0f;
					npc.localAI[1] = 0f;
					npc.localAI[2] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					calamityGlobalNPC.newAI[3] = 0f;
					rotationDirection = 0;
					chargeVelocityNormalized = default;
					spinningPoint = default;
					spinVelocity = default;
					npc.dontTakeDamage = true;

					npc.velocity.Y -= 1f;
					if ((double)npc.position.Y < Main.topWorld + 16f)
						npc.velocity.Y -= 1f;

					if ((double)npc.position.Y < Main.topWorld + 16f)
					{
						for (int a = 0; a < Main.maxNPCs; a++)
						{
							if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<Apollo.Apollo>() || Main.npc[a].type == ModContent.NPCType<AresBody>() ||
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

			// Destination variables
			Vector2 desiredVelocity = Vector2.Normalize(distanceFromDestination) * baseVelocity;

			// Duration of deathray spin to do a full circle
			// Normal = 120, Expert = 104, Rev = 96, Death = 88, Malice = 72
			float spinTime = 120f - 320f * (baseVelocityMult - 1.25f);

			// Set to transition to phase 2 if it hasn't happened yet
			if (phase2 && npc.localAI[3] == 0f)
			{
				AIState = (float)Phase.PhaseTransition;
				npc.localAI[3] = 1f;
				calamityGlobalNPC.newAI[2] = 0f;
				calamityGlobalNPC.newAI[3] = 0f;

				// Set frames to phase transition frames, which begin on frame 30
				// Reset the frame counter
				npc.frameCounter = 0D;

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
			npc.dontTakeDamage = invisiblePhase || AIState == (float)Phase.PhaseTransition;
			if (!invisiblePhase)
			{
				npc.Opacity += 0.2f;
				if (npc.Opacity > 1f)
					npc.Opacity = 1f;
			}
			else
			{
				npc.Opacity -= 0.05f;
				if (npc.Opacity < 0f)
					npc.Opacity = 0f;
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

						// Inverse lerp returns the percentage of progress between A and B
						float lerpValue2 = Utils.InverseLerp(movementDistanceGateValue, 2400f, distanceFromDestination.Length(), true);

						// Min velocity
						float minVelocity2 = distanceFromDestination.Length();
						float minVelocityCap2 = baseVelocity;
						if (minVelocity2 > minVelocityCap2)
							minVelocity2 = minVelocityCap2;

						// Max velocity
						Vector2 maxVelocity2 = distanceFromDestination / 24f;
						float maxVelocityCap2 = minVelocityCap2 * 3f;
						if (maxVelocity2.Length() > maxVelocityCap2)
							maxVelocity2 = distanceFromDestination.SafeNormalize(Vector2.Zero) * maxVelocityCap2;

						npc.velocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity2, maxVelocity2, lerpValue2);
					}
					else
					{
						// Save the normalized charge velocity for use in the charge phase
						if (chargeVelocityNormalized == default)
							chargeVelocityNormalized = Vector2.Normalize(aimedVector);

						// Decelerate
						npc.velocity *= decelerationVelocityMult;
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
							// Fire lasers
							float divisor = nerfedAttacks ? 60f : lastMechAlive ? 35f : 45f;
							float laserTimer = calamityGlobalNPC.newAI[3] - 2f;
							if (laserTimer % divisor == 0f && canFire)
							{
								pickNewLocation = true;
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									int type = ModContent.ProjectileType<ArtemisLaser>();
									int damage = npc.GetProjectileDamage(type);
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
									Vector2 laserVelocity = Vector2.Normalize(aimedVector);
									Vector2 projectileDestination = player.Center + predictionVector;
									Vector2 offset = laserVelocity * 70f;
									float setVelocityInAI = 6.5f;
									Projectile.NewProjectile(npc.Center + offset, projectileDestination, type, damage, 0f, Main.myPlayer, setVelocityInAI, npc.whoAmI);
								}
							}
						}
						else
							calamityGlobalNPC.newAI[2] = 0f;

						// Enter charge phase after a certain time has passed
						// This is replaced by a laser shotgun in phase 2
						// Enter deathray phase if in phase 2 and the localAI[2] variable is set to do so
						calamityGlobalNPC.newAI[3] += 1f;
						if (lineUpAttack)
						{
							// Return to normal laser phase if in passive state
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

									if (Main.netMode != NetmodeID.MultiplayerClient)
									{
										int type = ModContent.ProjectileType<ArtemisChargeTelegraph>();
										Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
										Vector2 laserVelocity = Vector2.Normalize(aimedVector);
										Vector2 offset = laserVelocity * 50f;
										Projectile.NewProjectile(npc.Center + offset, laserVelocity, type, 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
									}
								}

								// Fire a spread of projectiles in the direction of the charge
								if (!phase2 && calamityGlobalNPC.newAI[3] == attackPhaseGateValue + 2f + (timeToLineUpAttack - 30f))
								{
									if (Main.netMode != NetmodeID.MultiplayerClient)
									{
										int type = ModContent.ProjectileType<ArtemisLaser>();
										int damage = npc.GetProjectileDamage(type);
										Vector2 laserVelocity = chargeVelocityNormalized * 10f;
										int numLasersPerSpread = malice ? 10 : death ? 8 : expertMode ? 6 : 4;
										int spread = malice ? 30 : death ? 26 : expertMode ? 21 : 15;
										float rotation = MathHelper.ToRadians(spread);
										float distanceFromTarget = Vector2.Distance(npc.Center, npc.Center + chargeVelocityNormalized * chargeDistance);
										float setVelocityInAI = 6.5f;

										for (int i = 0; i < numLasersPerSpread + 1; i++)
										{
											Vector2 perturbedSpeed = laserVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numLasersPerSpread - 1)));
											Vector2 normalizedPerturbedSpeed = Vector2.Normalize(perturbedSpeed);

											Vector2 offset = normalizedPerturbedSpeed * 70f;
											Vector2 newCenter = npc.Center + offset;

											Projectile.NewProjectile(newCenter, newCenter + normalizedPerturbedSpeed * distanceFromTarget, type, damage, 0f, Main.myPlayer, setVelocityInAI, npc.whoAmI);
										}
									}
								}

								if (doBigAttack)
								{
									calamityGlobalNPC.newAI[3] = 0f;
									if (phase2)
									{
										AIState = npc.localAI[2] == 1f ? (float)Phase.Deathray : (float)Phase.LaserShotgun;
									}
									else
									{
										// Charge until a certain distance is reached and then return to normal phase
										Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/ELRFire"), npc.Center);
										AIState = (float)Phase.Charge;

										// Set charge velocity
										npc.velocity = chargeVelocityNormalized * chargeVelocity;
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
						npc.velocity *= decelerationVelocityMult;

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

					// Inverse lerp returns the percentage of progress between A and B
					float lerpValue = Utils.InverseLerp(movementDistanceGateValue, 2400f, distanceFromDestination.Length(), true);

					// Min velocity
					float minVelocity = distanceFromDestination.Length();
					float minVelocityCap = baseVelocity;
					if (minVelocity > minVelocityCap)
						minVelocity = minVelocityCap;

					// Max velocity
					Vector2 maxVelocity = distanceFromDestination / 24f;
					float maxVelocityCap = minVelocityCap * 3f;
					if (maxVelocity.Length() > maxVelocityCap)
						maxVelocity = distanceFromDestination.SafeNormalize(Vector2.Zero) * maxVelocityCap;

					npc.velocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity, maxVelocity, lerpValue);

					// Fire lasers
					int numSpreads = lastMechAlive ? 3 : 2;
					float divisor2 = laserShotgunDuration / numSpreads;
					if (calamityGlobalNPC.newAI[2] % divisor2 == 0f && canFire)
					{
						pickNewLocation = true;
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							int type = ModContent.ProjectileType<ArtemisLaser>();
							int damage = npc.GetProjectileDamage(type);
							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);

							Vector2 laserVelocity = Vector2.Normalize(aimedVector) * 10f;
							/* Spread:
							 * lastMechAlive = 20, 25, 30
							 * normal = 16, 20, 24
							 * nerfedAttacks = 12, 15, 18
							 */
							int numLasersAddedByDifficulty = malice ? 3 : death ? 2 : expertMode ? 1 : 0;
							int numLasersPerSpread = ((nerfedAttacks || nerfedLaserShotgun) ? 3 : lastMechAlive ? 7 : 5) + numLasersAddedByDifficulty;
							int baseSpread = ((nerfedAttacks || nerfedLaserShotgun) ? 9 : lastMechAlive ? 20 : 15) + numLasersAddedByDifficulty * 2;
							int spread = baseSpread + (int)(calamityGlobalNPC.newAI[2] / divisor2) * (baseSpread / 4);
							float rotation = MathHelper.ToRadians(spread);
							float distanceFromTarget = Vector2.Distance(npc.Center, player.Center + predictionVector);
							float setVelocityInAI = 6.5f;

							for (int i = 0; i < numLasersPerSpread + 1; i++)
							{
								Vector2 perturbedSpeed = laserVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numLasersPerSpread - 1)));
								Vector2 normalizedPerturbedSpeed = Vector2.Normalize(perturbedSpeed);

								Vector2 offset = normalizedPerturbedSpeed * 70f;
								Vector2 newCenter = npc.Center + offset;

								Projectile.NewProjectile(newCenter, newCenter + normalizedPerturbedSpeed * distanceFromTarget, type, damage, 0f, Main.myPlayer, setVelocityInAI, npc.whoAmI);
							}
						}
					}

					// Reset phase and variables
					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] >= laserShotgunDuration)
					{
						pickNewLocation = true;
						AIState = (float)Phase.Normal;
						npc.localAI[2] = shouldGetBuffedByBerserkPhase ? 1f : 0f;
						calamityGlobalNPC.newAI[2] = 0f;
					}

					break;

				// Fly above target, fire deathray and move in a circle around the target
				case (int)Phase.Deathray:

					// Fly above, stop doing this if in the proper position
					// Stop rotating and spin around a target point
					if ((destination - npc.Center).Length() < spinLocationDistance || calamityGlobalNPC.newAI[2] > 0f)
					{
						// Draw telegraph for deathray
						if (calamityGlobalNPC.newAI[2] == 0f)
						{
							npc.velocity = Vector2.Zero;

							switch ((int)npc.ai[3])
							{
								// Laser from top
								case 0:
								case 1:
									spinningPoint = npc.Center + Vector2.UnitY * spinRadius;
									break;

								// Laser from bottom
								case 2:
									spinningPoint = npc.Center - Vector2.UnitY * spinRadius;
									break;

								// Laser from left
								case 3:
									spinningPoint = npc.Center + Vector2.UnitX * spinRadius;
									break;

								// Laser from right
								case 4:
									spinningPoint = npc.Center - Vector2.UnitX * spinRadius;
									break;
							}

							npc.ai[1] = spinningPoint.X;
							npc.ai[2] = spinningPoint.Y;

							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								int type = ModContent.ProjectileType<ArtemisDeathrayTelegraph>();
								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
								Vector2 laserVelocity = Vector2.Normalize(spinningPoint - npc.Center);
								Vector2 offset = laserVelocity * 70f;
								Projectile.NewProjectile(npc.Center + offset, laserVelocity, type, 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
							}

							npc.netUpdate = true;
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
								switch ((int)npc.ai[3])
								{
									// Laser from top
									case 0:
									case 1:
										if (player.Center.X >= npc.Center.X)
											rotationDirection = 1;
										else
											rotationDirection = -1;
										break;

									// Laser from bottom
									case 2:
										if (player.Center.X >= npc.Center.X)
											rotationDirection = -1;
										else
											rotationDirection = 1;
										spinVelocity = -spinVelocity;
										break;

									// Laser from left
									case 3:
										if (player.Center.Y >= npc.Center.Y)
											rotationDirection = -1;
										else
											rotationDirection = 1;
										spinVelocity = spinVelocity.RotatedBy(-MathHelper.PiOver2);
										break;

									// Laser from right
									case 4:
										if (player.Center.Y >= npc.Center.Y)
											rotationDirection = 1;
										else
											rotationDirection = -1;
										spinVelocity = spinVelocity.RotatedBy(MathHelper.PiOver2);
										break;
								}

								spinVelocity *= -rotationDirection;
								npc.netUpdate = true;

								// Create a bunch of lightning bolts in the sky
								ExoMechsSky.CreateLightningBolt(12);

								// Fire deathray
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									int type = ModContent.ProjectileType<ArtemisLaserBeamStart>();
									Main.PlaySound(29, (int)npc.Center.X, (int)npc.Center.Y, 104);
									int damage = npc.GetProjectileDamage(type);
									int laser = Projectile.NewProjectile(npc.Center, Vector2.Zero, type, damage, 0f, Main.myPlayer, npc.whoAmI);
									if (Main.projectile.IndexInRange(laser))
									{
										Main.projectile[laser].ai[0] = npc.whoAmI;
										Main.projectile[laser].ai[1] = rotationDirection;
									}
								}
							}
							else
							{
								// This is used to adjust both the radians and the velocity of the spin moved per frame
								float rotationMult = (calamityGlobalNPC.newAI[2] - deathrayTelegraphDuration) / deathrayDuration * 4f;
								if (rotationMult > 1f)
								{
									// The radians moved per frame during the spin
									double radiansOfRotation = MathHelper.Pi / spinTime * -rotationDirection;
									npc.velocity = npc.velocity.RotatedBy(radiansOfRotation);
								}
								else
								{
									// The radians moved per frame during the spin
									// The radians moved are reduced early on to avoid spinning too fucking fast
									float decelerationMult = rotationMult * rotationMult;
									double radiansOfRotation = MathHelper.Pi / spinTime * -rotationDirection * decelerationMult;
									npc.velocity = (spinVelocity * decelerationMult).RotatedBy(radiansOfRotation);
								}
							}
						}
					}
					else
					{
						// Inverse lerp returns the percentage of progress between A and B
						float lerpValue3 = Utils.InverseLerp(movementDistanceGateValue, 2400f, distanceFromDestination.Length(), true);

						// Min velocity
						float minVelocity3 = distanceFromDestination.Length();
						float minVelocityCap3 = baseVelocity;
						if (minVelocity3 > minVelocityCap3)
							minVelocity3 = minVelocityCap3;

						// Max velocity
						Vector2 maxVelocity3 = distanceFromDestination / 24f;
						float maxVelocityCap3 = minVelocityCap3 * 3f;
						if (maxVelocity3.Length() > maxVelocityCap3)
							maxVelocity3 = distanceFromDestination.SafeNormalize(Vector2.Zero) * maxVelocityCap3;

						npc.velocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity3, maxVelocity3, lerpValue3);
					}

					// Reset phase and variables
					if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration + deathrayDuration)
					{
						spinVelocity = default;
						rotationDirection = 0;
						spinningPoint = default;
						pickNewLocation = true;
						AIState = (float)Phase.Normal;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						if (revenge)
						{
							// Can use all 4 types of lasers in rev+
							switch ((int)npc.ai[3])
							{
								case 0:
								case 1:
									npc.ai[3] = 3f + Main.rand.Next(2);
									break;
								case 2:
									npc.ai[3] = 3f + Main.rand.Next(2);
									break;
								case 3:
									npc.ai[3] = 1f + Main.rand.Next(2);
									break;
								case 4:
									npc.ai[3] = 1f + Main.rand.Next(2);
									break;
							}
						}
						else if (expertMode)
						{
							// Can only use the top and bottom lasers in expert
							npc.ai[3] += 1f;
							if (npc.ai[3] > 2f)
								npc.ai[3] = 1f;
						}
						npc.localAI[2] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						npc.netUpdate = true;
					}

					break;

				// Phase transition animation, that's all this exists for
				case (int)Phase.PhaseTransition:

					// Inverse lerp returns the percentage of progress between A and B
					float lerpValue4 = Utils.InverseLerp(movementDistanceGateValue, 2400f, distanceFromDestination.Length(), true);

					// Min velocity
					float minVelocity4 = distanceFromDestination.Length();
					float minVelocityCap4 = baseVelocity;
					if (minVelocity4 > minVelocityCap4)
						minVelocity4 = minVelocityCap4;

					// Max velocity
					Vector2 maxVelocity4 = distanceFromDestination / 24f;
					float maxVelocityCap4 = minVelocityCap4 * 3f;
					if (maxVelocity4.Length() > maxVelocityCap4)
						maxVelocity4 = distanceFromDestination.SafeNormalize(Vector2.Zero) * maxVelocityCap4;

					npc.velocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity4, maxVelocity4, lerpValue4);

					// Shoot lens gore at the target at the proper time
					if (calamityGlobalNPC.newAI[2] == lensPopTime)
					{
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire"), npc.Center);
						Vector2 lensDirection = Vector2.Normalize(aimedVector);
						Vector2 offset = lensDirection * 70f;

						if (Main.netMode != NetmodeID.MultiplayerClient)
							Projectile.NewProjectile(npc.Center + offset, lensDirection * 24f, ModContent.ProjectileType<BrokenArtemisLens>(), 0, 0f);
					}

					// Reset phase and variables
					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] >= phaseTransitionDuration)
					{
						pickNewLocation = true;
						AIState = (float)Phase.Normal;
						npc.localAI[0] = 0f;
						npc.localAI[1] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						chargeVelocityNormalized = default;
					}

					break;
			}

			// Update the charge flash variable
			ChargeFlash = MathHelper.Clamp(ChargeFlash + shouldDoChargeFlash.ToDirectionInt() * 0.08f, 0f, 1f);
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;

			Rectangle targetHitbox = target.Hitbox;

			float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist)
				minDist = dist2;
			if (dist3 < minDist)
				minDist = dist3;
			if (dist4 < minDist)
				minDist = dist4;

			return minDist <= 100f && npc.Opacity == 1f && AIState == (float)Phase.Charge;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 2f;
			return null;
		}

		public override void FindFrame(int frameHeight)
		{
			// Use telegraph frames before each attack
			bool phase2 = npc.life / (float)npc.lifeMax < 0.6f;
			npc.frameCounter += 1D;
			if (AIState == (float)Phase.PhaseTransition)
			{
				if (npc.frameCounter >= 6D)
				{
					// Reset frame counter
					npc.frameCounter = 0D;

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
					int frameLimit = phase2 ? (npc.Calamity().newAI[3] == 0f ? normalFrameLimit_Phase2 : npc.Calamity().newAI[3] == 1f ? chargeUpFrameLimit_Phase2 : attackFrameLimit_Phase2) :
						(npc.Calamity().newAI[3] == 0f ? normalFrameLimit_Phase1 : npc.Calamity().newAI[3] == 1f ? chargeUpFrameLimit_Phase1 : attackFrameLimit_Phase1);

					if (npc.frameCounter >= 6D)
					{
						// Reset frame counter
						npc.frameCounter = 0D;

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
							frameX = frameY = phase2 ? (npc.Calamity().newAI[3] == 0f ? 6 : npc.Calamity().newAI[3] == 1f ? 7 : 8) : (npc.Calamity().newAI[3] == 0f ? 0 : npc.Calamity().newAI[3] == 1f ? 1 : 2);
					}
				}
				else if (AIState == (float)Phase.Charge || AIState == (float)Phase.LaserShotgun || AIState == (float)Phase.Deathray)
				{
					int frameLimit = phase2 ? attackFrameLimit_Phase2 : attackFrameLimit_Phase1;
					if (npc.frameCounter >= 6D)
					{
						// Reset frame counter
						npc.frameCounter = 0D;

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
		}

		public float FlameTrailWidthFunction(float completionRatio) => MathHelper.SmoothStep(21f, 8f, completionRatio) * ChargeFlash;

		public float FlameTrailWidthFunctionBig(float completionRatio) => MathHelper.SmoothStep(34f, 12f, completionRatio) * ChargeFlash;

		public float RibbonTrailWidthFunction(float completionRatio)
		{
			float baseWidth = Utils.InverseLerp(1f, 0.54f, completionRatio, true) * 5f;
			float endTipWidth = CalamityUtils.Convert01To010(Utils.InverseLerp(0.96f, 0.89f, completionRatio, true)) * 2.4f;
			return baseWidth + endTipWidth;
		}

		public Color FlameTrailColorFunction(float completionRatio)
		{
			float trailOpacity = Utils.InverseLerp(0.8f, 0.27f, completionRatio, true) * Utils.InverseLerp(0f, 0.067f, completionRatio, true);
			Color startingColor = Color.Lerp(Color.White, Color.Cyan, 0.27f);
			Color middleColor = Color.Lerp(Color.Orange, Color.Yellow, 0.31f);
			Color endColor = Color.OrangeRed;
			return CalamityUtils.MulticolorLerp(completionRatio, startingColor, middleColor, endColor) * ChargeFlash * trailOpacity;
		}

		public Color FlameTrailColorFunctionBig(float completionRatio)
		{
			float trailOpacity = Utils.InverseLerp(0.8f, 0.27f, completionRatio, true) * Utils.InverseLerp(0f, 0.067f, completionRatio, true) * 0.56f;
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
			return Color.Lerp(startingColor, endColor, (float)Math.Pow(completionRatio, 1.5D)) * npc.Opacity;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			// Declare the trail drawers if they have yet to be defined.
			if (ChargeFlameTrail is null)
				ChargeFlameTrail = new PrimitiveTrail(FlameTrailWidthFunction, FlameTrailColorFunction, null, GameShaders.Misc["CalamityMod:ImpFlameTrail"]);

			if (ChargeFlameTrailBig is null)
				ChargeFlameTrailBig = new PrimitiveTrail(FlameTrailWidthFunctionBig, FlameTrailColorFunctionBig, null, GameShaders.Misc["CalamityMod:ImpFlameTrail"]);

			if (RibbonTrail is null)
				RibbonTrail = new PrimitiveTrail(RibbonTrailWidthFunction, RibbonTrailColorFunction);

			// Prepare the flame trail shader with its map texture.
			GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/ScarletDevilStreak"));

			int numAfterimages = ChargeFlash > 0f ? 0 : 5;
			Texture2D texture = Main.npcTexture[npc.type];
			Rectangle frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);
			Vector2 origin = npc.Size * 0.5f;
			Vector2 center = npc.Center - Main.screenPosition;
			Color afterimageBaseColor = Color.White;

			// Draws a single instance of a regular, non-glowmask based Artemis.
			// This is created to allow easy duplication of them when drawing the charge.
			void drawInstance(Vector2 drawOffset, Color baseColor)
			{
				if (CalamityConfig.Instance.Afterimages)
				{
					for (int i = 1; i < numAfterimages; i += 2)
					{
						Color afterimageColor = baseColor;
						afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
						afterimageColor = npc.GetAlpha(afterimageColor);
						afterimageColor *= (numAfterimages - i) / 15f;
						Vector2 afterimageCenter = npc.oldPos[i] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
						afterimageCenter -= new Vector2(texture.Width, texture.Height) / new Vector2(maxFramesX, maxFramesY) * npc.scale / 2f;
						afterimageCenter += origin * npc.scale + new Vector2(0f, npc.gfxOffY);
						afterimageCenter += drawOffset;
						spriteBatch.Draw(texture, afterimageCenter, npc.frame, afterimageColor, npc.oldRot[i], origin, npc.scale, SpriteEffects.None, 0f);
					}
				}

				spriteBatch.Draw(texture, center + drawOffset, frame, npc.GetAlpha(baseColor), npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
			}

			// Draw ribbons near the main thruster
			for (int direction = -1; direction <= 1; direction += 2)
			{
				Vector2 ribbonOffset = -Vector2.UnitY.RotatedBy(npc.rotation) * 14f;
				ribbonOffset += Vector2.UnitX.RotatedBy(npc.rotation) * direction * 26f;

				float currentSegmentRotation = npc.rotation;
				List<Vector2> ribbonDrawPositions = new List<Vector2>();
				for (int i = 0; i < 12; i++)
				{
					float ribbonCompletionRatio = i / 12f;
					float wrappedAngularOffset = MathHelper.WrapAngle(npc.oldRot[i + 1] - currentSegmentRotation) * 0.3f;
					float segmentRotationOffset = MathHelper.Clamp(wrappedAngularOffset, -0.12f, 0.12f);

					// Add a sinusoidal offset that goes based on time and completion ratio to create a waving-flag-like effect.
					// This is dampened for the first few points to prevent weird offsets. It is also dampened by high velocity.
					float sinusoidalRotationOffset = (float)Math.Sin(ribbonCompletionRatio * 2.22f + Main.GlobalTime * 3.4f) * 1.36f;
					float sinusoidalRotationOffsetFactor = Utils.InverseLerp(0f, 0.37f, ribbonCompletionRatio, true) * direction * 24f;
					sinusoidalRotationOffsetFactor *= Utils.InverseLerp(24f, 16f, npc.velocity.Length(), true);

					Vector2 sinusoidalOffset = Vector2.UnitY.RotatedBy(npc.rotation + sinusoidalRotationOffset) * sinusoidalRotationOffsetFactor;
					Vector2 ribbonSegmentOffset = Vector2.UnitY.RotatedBy(currentSegmentRotation) * ribbonCompletionRatio * 540f + sinusoidalOffset;
					ribbonDrawPositions.Add(npc.Center + ribbonSegmentOffset + ribbonOffset);

					currentSegmentRotation += segmentRotationOffset;
				}
				RibbonTrail.Draw(ribbonDrawPositions, -Main.screenPosition, 66);
			}

			int instanceCount = (int)MathHelper.Lerp(1f, 15f, ChargeFlash);
			Color baseInstanceColor = Color.Lerp(drawColor, Color.White, ChargeFlash);
			baseInstanceColor.A = (byte)(int)(255f - ChargeFlash * 255f);

			spriteBatch.EnterShaderRegion();

			drawInstance(Vector2.Zero, baseInstanceColor);
			if (instanceCount > 1)
			{
				baseInstanceColor *= 0.04f;
				float backAfterimageOffset = MathHelper.SmoothStep(0f, 2f, ChargeFlash);
				for (int i = 0; i < instanceCount; i++)
				{
					Vector2 drawOffset = (MathHelper.TwoPi * i / instanceCount + Main.GlobalTime * 0.8f).ToRotationVector2() * backAfterimageOffset;
					drawInstance(drawOffset, baseInstanceColor);
				}
			}

			texture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Artemis/ArtemisGlow");
			if (CalamityConfig.Instance.Afterimages)
			{
				for (int i = 1; i < numAfterimages; i += 2)
				{
					Color afterimageColor = drawColor;
					afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
					afterimageColor = npc.GetAlpha(afterimageColor);
					afterimageColor *= (numAfterimages - i) / 15f;
					Vector2 afterimageCenter = npc.oldPos[i] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					afterimageCenter -= new Vector2(texture.Width, texture.Height) / new Vector2(maxFramesX, maxFramesY) * npc.scale / 2f;
					afterimageCenter += origin * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture, afterimageCenter, npc.frame, afterimageColor, npc.oldRot[i], origin, npc.scale, SpriteEffects.None, 0f);
				}
			}

			spriteBatch.Draw(texture, center, frame, Color.White * npc.Opacity, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);

			spriteBatch.ExitShaderRegion();

			// Draw a flame trail on the thrusters if needed. This happens during charges.
			if (ChargeFlash > 0f)
			{
				for (int direction = -1; direction <= 1; direction++)
				{
					Vector2 baseDrawOffset = new Vector2(0f, direction == 0f ? 18f : 60f).RotatedBy(npc.rotation);
					baseDrawOffset += new Vector2(direction * 64f, 0f).RotatedBy(npc.rotation);

					float backFlameLength = direction == 0f ? 700f : 190f;
					Vector2 drawStart = npc.Center + baseDrawOffset;
					Vector2 drawEnd = drawStart - (npc.rotation - MathHelper.PiOver2).ToRotationVector2() * ChargeFlash * backFlameLength;
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
							ChargeFlameTrailBig.Draw(drawPositions, drawOffset - Main.screenPosition, 70);
						}
					}
					else
						ChargeFlameTrail.Draw(drawPositions, -Main.screenPosition, 70);
				}
			}

			return false;
		}

		public override bool PreNPCLoot() => false;

		// Needs edits
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
				Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1f);

			if (npc.life <= 0)
			{
				for (int num193 = 0; num193 < 2; num193++)
				{
					Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
				}
				for (int num194 = 0; num194 < 20; num194++)
				{
					int num195 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
					Main.dust[num195].noGravity = true;
					Main.dust[num195].velocity *= 3f;
					num195 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
					Main.dust[num195].velocity *= 2f;
					Main.dust[num195].noGravity = true;
				}

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Artemis/Artemis1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Artemis/Artemis2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Artemis/Artemis3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Artemis/Artemis4"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Artemis/Artemis5"), 1f);
			}
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
					nPC.HitEffect(0, 10.0);
					nPC.checkDead();
					nPC.active = false;
				}
			}
			return true;
		}

		public override bool CheckActive() => false;

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
		}
    }
}
