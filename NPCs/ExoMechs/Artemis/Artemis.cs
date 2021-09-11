using CalamityMod.Events;
using CalamityMod.Items.Potions;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
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
		private const float defaultAnimationDuration = 100f;

		// Total duration of attack telegraphs
		private const float attackTelegraphDuration = 100f;

		// Total duration of the phase transition
		private const float phaseTransitionDuration = 300f;

		// Where the timer should be when the lens pops off
		private const float lensPopTime = 80f;

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

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("XS-01 Artemis");
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
			npc.LifeMaxNERB(1000000, 1150000, 500000);
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
			music = /*CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm") ??*/ MusicID.Boss3;
		}

		public override void BossHeadSlot(ref int index)
		{
			if (npc.life / (float)npc.lifeMax < 0.6f)
				index = phase2IconIndex;
			else
				index = phase1IconIndex;
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(npc.chaseable);
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
			npc.chaseable = reader.ReadBoolean();
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

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

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
			bool exoWormAlive = false;
			bool exoPrimeAlive = false;
			if (CalamityGlobalNPC.draedonExoMechWorm != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].active)
				{
					otherExoMechsAlive++;
					exoWormAlive = true;
				}
			}
			if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
				{
					// Set target to Apollo's target if Apollo is alive
					player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].target];

					// Link the HP of both twins
					if (npc.life > Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life)
						npc.life = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life;
				}
			}
			if (CalamityGlobalNPC.draedonExoMechPrime != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
				{
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
				exoPrimePassive = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[1] == (float)AresBody.SecondaryPhase.Passive;
			bool anyOtherExoMechPassive = exoWormPassive || exoPrimePassive;

			// Phases
			bool phase2 = lifeRatio < 0.6f;
			bool spawnOtherExoMechs = lifeRatio > 0.4f && otherExoMechsAlive == 0 && lifeRatio < 0.7f;
			bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
			bool lastMechAlive = berserk && otherExoMechsAlive == 0;

			// If Artemis and Apollo don't go berserk
			bool otherMechIsBerserk = exoWormLifeRatio < 0.4f || exoPrimeLifeRatio < 0.4f;

			// Spawn Apollo if it doesn't exist after the first 10 frames have passed
			if (npc.ai[0] < 10f)
			{
				npc.ai[0] += 1f;
				if (npc.ai[0] == 10f && !NPC.AnyNPCs(ModContent.NPCType<Apollo.Apollo>()))
					NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Apollo.Apollo>());
			}

			// Despawn if target is dead
			bool targetDead = false;
			if (player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (player.dead)
				{
					targetDead = true;

					AIState = (float)Phase.Normal;
					npc.localAI[0] = 0f;
					npc.localAI[1] = 0f;
					npc.localAI[2] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					calamityGlobalNPC.newAI[3] = 0f;
					rotationDirection = 0;

					npc.velocity.Y -= 2f;
					if ((double)npc.position.Y < Main.topWorld + 16f)
						npc.velocity.Y -= 2f;

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

			// Predictiveness
			float predictionAmt = malice ? 20f : death ? 15f : revenge ? 12.5f : expertMode ? 10f : 5f;
			if (SecondaryAIState == (float)SecondaryPhase.Passive)
				predictionAmt *= 0.5f;

			Vector2 predictionVector = AIState == (float)Phase.Deathray ? Vector2.Zero : player.velocity * predictionAmt;
			Vector2 rotationVector = player.Center + predictionVector - npc.Center;

			// Gate values
			float attackPhaseGateValue = lastMechAlive ? 300f : 480f;
			float timeToLineUpAttack = lastMechAlive ? 60f : 90f;
			float deathrayPhaseGateValue = lastMechAlive ? 630f : 900f;

			// Spin variables
			float spinRadius = 500f;
			float spinLocationDistance = 50f;

			// Distance where Artemis stops moving
			float movementDistanceGateValue = 100f;

			// Charge velocity
			float chargeVelocity = malice ? 60f : death ? 50f : revenge ? 45f : expertMode ? 40f : 30f;
			float chargeDistance = 1800f;
			float chargeDuration = chargeDistance / chargeVelocity;

			// Laser shotgun variables
			float laserShotgunDuration = lastMechAlive ? 45f : 60f;

			// Rotation
			bool stopRotatingAndSlowDown = !phase2 && AIState == (float)Phase.Normal && calamityGlobalNPC.newAI[3] >= attackPhaseGateValue + 2f;
			if (!stopRotatingAndSlowDown)
			{
				if (AIState == (int)Phase.Charge)
				{
					npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
				}
				else if (spinningPoint != default)
				{
					float x = spinningPoint.X - npc.Center.X;
					float y = spinningPoint.Y - npc.Center.Y;
					npc.rotation = (float)Math.Atan2(y, x) + MathHelper.PiOver2;
				}
				else
				{
					float x = player.Center.X + predictionVector.X - npc.Center.X;
					float y = player.Center.Y + predictionVector.Y - npc.Center.Y;
					npc.rotation = (float)Math.Atan2(y, x) + MathHelper.PiOver2;
				}
			}

			// Light
			Lighting.AddLight(npc.Center, 0.25f, 0.15f, 0.05f);

			// Add some random distance to the destination after certain attacks
			if (pickNewLocation)
			{
				pickNewLocation = false;
				npc.localAI[0] = Main.rand.Next(-50, 51);
				npc.localAI[1] = Main.rand.Next(-250, 251);
				npc.netUpdate = true;
			}

			// Default vector to fly to
			Vector2 destination = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune ? new Vector2(player.Center.X - 1200f, player.Center.Y) : AIState == (float)Phase.Deathray ? new Vector2(player.Center.X, player.Center.Y - spinRadius) : new Vector2(player.Center.X - 750f, player.Center.Y);

			// Add a bit of randomness to the destination, but only in specific phases where it's necessary
			if (AIState == (float)Phase.Normal || AIState == (float)Phase.LaserShotgun || AIState == (float)Phase.PhaseTransition)
			{
				destination.X += npc.localAI[0];
				destination.Y += npc.localAI[1];
			}

			// Velocity and acceleration values
			float baseVelocityMult = malice ? 1.3f : death ? 1.2f : revenge ? 1.15f : expertMode ? 1.1f : 1f;
			float baseVelocity = 14f * baseVelocityMult;
			float baseAcceleration = 1f;
			float decelerationVelocityMult = 0.85f;
			if (berserk)
			{
				baseVelocity *= 1.5f;
				baseAcceleration *= 1.5f;
			}
			Vector2 distanceFromDestination = destination - npc.Center;
			Vector2 desiredVelocity = Vector2.Normalize(distanceFromDestination) * baseVelocity;

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

			// Passive and Immune phases
			switch ((int)SecondaryAIState)
			{
				case (int)SecondaryPhase.Nothing:

					// Spawn the other mechs if Artemis and Apollo are first
					// Note: Only Apollo spawns the mechs because the twins HP values are linked
					if (otherExoMechsAlive == 0)
					{
						if (spawnOtherExoMechs)
						{
							// Reset everything
							SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
							npc.TargetClosest();
						}
					}
					else
					{
						// If not spawned first, go to passive state if any other mech is passive or if Artemis and Apollo are under 70% life
						// Do not run this if berserk
						// Do not run this if any exo mech is dead
						if ((anyOtherExoMechPassive || lifeRatio < 0.7f) && !berserk && totalOtherExoMechLifeRatio < 5f)
						{
							// Tells Artemis to return to the battle in passive state and reset everything
							SecondaryAIState = (float)SecondaryPhase.Passive;
							npc.TargetClosest();
						}

						// Go passive and immune if one of the other mechs is berserk
						// This is only called if two exo mechs are alive
						if (otherMechIsBerserk)
						{
							// Reset everything
							SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
							npc.TargetClosest();
						}
					}

					break;

				// Fire projectiles less often
				case (int)SecondaryPhase.Passive:

					// Enter passive and invincible phase if one of the other exo mechs is berserk
					if (otherMechIsBerserk)
					{
						// Reset everything
						SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
						npc.TargetClosest();
					}

					// If Artemis and Apollo are the first mechs to go berserk
					if (berserk)
					{
						// Reset everything
						npc.TargetClosest();

						// Never be passive if berserk
						SecondaryAIState = (float)SecondaryPhase.Nothing;
					}

					break;

				// Fly above target and become immune
				case (int)SecondaryPhase.PassiveAndImmune:

					// Enter the fight again if any of the other exo mechs is below 70% and the other mechs aren't berserk
					if ((exoWormLifeRatio < 0.7f || exoPrimeLifeRatio < 0.7f) && !otherMechIsBerserk)
					{
						// Tells Artemis and Apollo to return to the battle in passive state and reset everything
						// Return to normal phases if one or more mechs have been downed
						SecondaryAIState = totalOtherExoMechLifeRatio > 5f ? (float)SecondaryPhase.Nothing : (float)SecondaryPhase.Passive;
						npc.TargetClosest();
					}

					if (berserk)
					{
						// Reset everything
						npc.TargetClosest();

						// Never be passive if berserk
						SecondaryAIState = (float)SecondaryPhase.Nothing;
					}

					break;
			}

			// Attacking phases
			switch ((int)AIState)
			{
				// Fly to the left of the target
				case (int)Phase.Normal:

					if (!targetDead)
					{
						if (!stopRotatingAndSlowDown)
						{
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
						}
						else
						{
							if (chargeVelocityNormalized == default)
								chargeVelocityNormalized = Vector2.Normalize(npc.velocity);

							npc.velocity *= decelerationVelocityMult;
						}
					}

					// Default animation for 100 frames and then go to telegraph animation
					// newAI[3] tells Artemis what animation state it's currently in
					bool attacking = calamityGlobalNPC.newAI[3] >= 2f;
					bool firingLasers = attacking && calamityGlobalNPC.newAI[3] + 2f < attackPhaseGateValue;
					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] >= defaultAnimationDuration || attacking)
					{
						if (firingLasers)
						{
							// Fire lasers
							int numLasers = lastMechAlive ? 15 : 12;
							float divisor = attackPhaseGateValue / numLasers;
							float laserTimer = calamityGlobalNPC.newAI[3] - 2f;
							if (laserTimer % divisor == 0f)
							{
								pickNewLocation = true;
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									int type = ModContent.ProjectileType<ExoDestroyerLaser>();
									int damage = npc.GetProjectileDamage(type);
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
									Vector2 laserVelocity = Vector2.Normalize(rotationVector);
									Vector2 projectileDestination = player.Center + player.velocity * predictionAmt;
									Vector2 offset = laserVelocity * 70f;
									Projectile.NewProjectile(npc.Center + offset, projectileDestination, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
								}
							}
						}
						else
							calamityGlobalNPC.newAI[2] = 0f;

						// Enter charge phase after a certain time has passed
						// This is replaced by a laser shotgun in phase 2
						// Enter deathray phase if in phase 2 and the localAI[2] variable is set to do so
						calamityGlobalNPC.newAI[3] += 1f;
						bool lineUpAttack = calamityGlobalNPC.newAI[3] >= attackPhaseGateValue + 2f;
						if (lineUpAttack)
						{
							// Draw a large laser telegraph for the charge
							if (!phase2 && calamityGlobalNPC.newAI[3] == attackPhaseGateValue + 2f)
							{
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									/*int type = ModContent.ProjectileType<ArtemisChargeTelegraph>();
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
									Vector2 laserVelocity = Vector2.Normalize(rotationVector);
									Vector2 projectileDestination = player.Center + player.velocity * predictionAmt;
									Vector2 offset = laserVelocity * 70f;
									Projectile.NewProjectile(npc.Center + offset, projectileDestination, type, 0, 0f, Main.myPlayer, 0f, npc.whoAmI);*/
								}
							}

							bool doAttack = calamityGlobalNPC.newAI[3] >= attackPhaseGateValue + 2f + timeToLineUpAttack;
							if (doAttack)
							{
								if (phase2)
								{
									AIState = npc.localAI[2] == 1f ? (float)Phase.Deathray : (float)Phase.LaserShotgun;
								}
								else
								{
									// Charge until a certain distance is reached and then return to normal phase
									AIState = (float)Phase.Charge;
									npc.velocity = chargeVelocityNormalized * chargeVelocity;
								}
							}
						}
					}

					break;

				// Charge
				case (int)Phase.Charge:

					// Reset phase and variables
					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] >= chargeDuration)
					{
						npc.velocity *= decelerationVelocityMult;

						if (calamityGlobalNPC.newAI[2] >= chargeDuration + 20f)
						{
							pickNewLocation = true;
							AIState = (float)Phase.Normal;
							calamityGlobalNPC.newAI[2] = 0f;
							calamityGlobalNPC.newAI[3] = 0f;
							npc.TargetClosest();
						}
					}

					break;

				// Laser shotgun barrage
				case (int)Phase.LaserShotgun:

					if (!targetDead)
					{
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
					}

					// Fire lasers
					int numSpreads = lastMechAlive ? 3 : 2;
					int numLasersPerSpread = lastMechAlive ? 8 : 6;
					float divisor2 = laserShotgunDuration / numSpreads;
					if (calamityGlobalNPC.newAI[2] % divisor2 == 0f)
					{
						pickNewLocation = true;
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							int type = ModContent.ProjectileType<ExoDestroyerLaser>();
							int damage = npc.GetProjectileDamage(type);
							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
							Vector2 laserVelocity = Vector2.Normalize(rotationVector);
							int spread = 3 + (int)(calamityGlobalNPC.newAI[2] / divisor2) * 3;
							float rotation = MathHelper.ToRadians(spread);
							for (int i = 0; i < numLasersPerSpread + 1; i++)
							{
								Vector2 perturbedSpeed = laserVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numLasersPerSpread - 1)));
								Vector2 offset = Vector2.Normalize(perturbedSpeed) * 70f;
								Projectile.NewProjectile(npc.Center + offset, player.Center + perturbedSpeed, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
							}
						}
					}

					// Reset phase and variables
					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] >= laserShotgunDuration)
					{
						pickNewLocation = true;
						AIState = (float)Phase.Normal;
						npc.localAI[2] = berserk ? 1f : 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						npc.TargetClosest();
					}

					break;

				// Fly above target, fire deathray and move in a circle around the target
				case (int)Phase.Deathray:

					// Fly above, stop doing this if in the proper position
					if (!targetDead)
					{
						// Stop rotating and spin around a target point
						if ((destination - npc.Center).Length() < spinLocationDistance || calamityGlobalNPC.newAI[2] > 0f)
						{
							// Draw telegraph for deathray
							if (calamityGlobalNPC.newAI[2] == 0f)
							{
								npc.velocity = Vector2.Zero;
								spinningPoint = npc.Center + Vector2.UnitY * spinRadius;
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									/*int type = ModContent.ProjectileType<ArtemisChargeTelegraph>();
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
									Vector2 laserVelocity = Vector2.Normalize(spinningPoint - npc.Center);
									Vector2 offset = laserVelocity * 70f;
									Projectile.NewProjectile(npc.Center + offset, spinningPoint, type, 0, 0f, Main.myPlayer, 0f, npc.whoAmI);*/
								}
								npc.netUpdate = true;
							}

							// Fire deathray and spin
							calamityGlobalNPC.newAI[2] += 1f;
							if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration)
							{
								if (rotationDirection == 0)
								{
									// Set spin direction
									if (Main.player[npc.target].velocity.X > 0f)
										rotationDirection = 1;
									else if (Main.player[npc.target].velocity.X < 0f)
										rotationDirection = -1;
									else
										rotationDirection = player.direction;

									// Set spin velocity
									npc.velocity.X = MathHelper.Pi * spinRadius / baseVelocity;
									npc.velocity *= -rotationDirection;
									npc.netUpdate = true;

									// Fire deathray
									if (Main.netMode != NetmodeID.MultiplayerClient)
									{
										/*int type = ModContent.ProjectileType<ArtemisLaserBeamStart>();
										int damage = npc.GetProjectileDamage(type);
										int laser = Projectile.NewProjectile(npc.Center, Vector2.Zero, type, damage, 0f, Main.myPlayer, npc.whoAmI);
										if (Main.projectile.IndexInRange(laser))
											Main.projectile[laser].ai[0] = npc.whoAmI;*/
									}
								}
								else
									npc.velocity = npc.velocity.RotatedBy(MathHelper.Pi / baseVelocity * -rotationDirection);
							}
						}
						else
						{
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
						}
					}

					// Reset phase and variables
					if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration + deathrayDuration)
					{
						rotationDirection = 0;
						spinningPoint = default;
						pickNewLocation = true;
						AIState = (float)Phase.Normal;
						npc.localAI[2] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						npc.TargetClosest();
					}

					break;

				// Phase transition animation, that's all this exists for
				case (int)Phase.PhaseTransition:

					if (!targetDead)
					{
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
					}

					// Shoot lens gore at the target at the proper time
					if (calamityGlobalNPC.newAI[2] == lensPopTime)
					{
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeWeaponFire"), npc.Center);
						Vector2 goreVelocity = Vector2.Normalize(rotationVector);
						Vector2 offset = goreVelocity * 70f;
						Gore.NewGore(npc.Center + offset, goreVelocity * 24f, mod.GetGoreSlot("Gores/Artemis/ArtemisTransitionGore"), 1f);
					}

					// Reset phase and variables
					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] >= phaseTransitionDuration)
					{
						pickNewLocation = true;
						AIState = (float)Phase.Normal;
						calamityGlobalNPC.newAI[2] = 0f;
						npc.TargetClosest();
					}

					break;
			}
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

			return minDist <= 100f && npc.Opacity == 1f;
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) => !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);

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
				if (npc.frameCounter >= 10D)
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

					if (npc.frameCounter >= 10D)
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
					if (npc.frameCounter >= 10D)
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

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Texture2D texture = Main.npcTexture[npc.type];
			Rectangle frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);
			Vector2 vector = new Vector2(npc.width / 2, npc.height / 2);
			Color afterimageBaseColor = Color.White;
			int numAfterimages = 5;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int i = 1; i < numAfterimages; i += 2)
				{
					Color afterimageColor = drawColor;
					afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
					afterimageColor = npc.GetAlpha(afterimageColor);
					afterimageColor *= (numAfterimages - i) / 15f;
					Vector2 afterimageCenter = npc.oldPos[i] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					afterimageCenter -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					afterimageCenter += vector * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture, afterimageCenter, npc.frame, afterimageColor, npc.rotation, vector, npc.scale, SpriteEffects.None, 0f);
				}
			}

			Vector2 center = npc.Center - Main.screenPosition;
			spriteBatch.Draw(texture, center, frame, npc.GetAlpha(drawColor), npc.rotation, vector, npc.scale, SpriteEffects.None, 0f);

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
					afterimageCenter -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					afterimageCenter += vector * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture, afterimageCenter, npc.frame, afterimageColor, npc.rotation, vector, npc.scale, SpriteEffects.None, 0f);
				}
			}

			spriteBatch.Draw(texture, center, frame, Color.White * npc.Opacity, npc.rotation, vector, npc.scale, SpriteEffects.None, 0f);

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

				/*Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody4"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody5"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody6"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody7"), 1f);*/
			}
		}

		public override bool CheckActive() => false;

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
		}
    }
}