using CalamityMod.Events;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Thanatos
{
	public class ThanatosHead : ModNPC
    {
		public static int normalIconIndex;
		public static int vulnerableIconIndex;

		internal static void LoadHeadIcons()
		{
			string normalIconPath = "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosNormalHead";
			string vulnerableIconPath = "CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosVulnerableHead";

			CalamityMod.Instance.AddBossHeadTexture(normalIconPath, -1);
			normalIconIndex = ModContent.GetModBossHeadSlot(normalIconPath);

			CalamityMod.Instance.AddBossHeadTexture(vulnerableIconPath, -1);
			vulnerableIconIndex = ModContent.GetModBossHeadSlot(vulnerableIconPath);
		}

		public enum Phase
		{
			Charge = 0,
			UndergroundLaserBarrage = 1,
			Deathray = 2
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

		public ThanatosSmokeParticleSet SmokeDrawer = new ThanatosSmokeParticleSet(-1, 3, 0f, 16f, 1.5f);

		// Invincibility time for the first 10 seconds
		public const float immunityTime = 600f;

		// Whether the head is venting heat or not, it is vulnerable to damage during venting
		private bool vulnerable = false;

		// Max time in vent phase
		public const float ventDuration = 180f;

		// Spawn rate for vent clouds
		public const int ventCloudSpawnRate = 5;

		// Default life ratio for the other mechs
		private const float defaultLifeRatio = 5f;

		// Base distance from the target for most attacks
		private const float baseDistance = 400f;

		// Base distance from target location in order to continue turning
		private const float baseTurnDistance = 160f;

		// Max distance from the target before they are unable to hear sound telegraphs
		private const float soundDistance = 2800f;

		// Length variables
		public const int minLength = 100;
        private const int maxLength = 101;

		// Variable used to stop the segment spawning loop
        private bool tailSpawned = false;

		// Used in the lerp to smoothly scale velocity up and down
		private float chargeVelocityScalar = 0f;

		// Total duration of the deathray telegraph
		private const float deathrayTelegraphDuration = 180f;

		// Total duration of the deathray
		private const float deathrayDuration = 180f;
		
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("XM-05 Thanatos");
			Main.npcFrameCount[npc.type] = 5;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 5f;
			npc.GetNPCDamage();
			npc.width = 164;
            npc.height = 164;
            npc.defense = 80;
			npc.DR_NERD(0.9999f);
			npc.Calamity().unbreakableDR = true;
			npc.LifeMaxNERB(1000000, 1150000, 500000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
            aiType = -1;
			npc.Opacity = 0f;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(3, 33, 0, 0);
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
			npc.boss = true;
			npc.chaseable = false;
			music = /*CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm") ??*/ MusicID.Boss3;
			bossBag = ModContent.ItemType<DraedonTreasureBag>();
		}

		public override void BossHeadSlot(ref int index)
		{
			if (SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune)
				index = -1;
			else if (vulnerable)
				index = vulnerableIconIndex;
			else
				index = normalIconIndex;
		}

		public override void BossHeadRotation(ref float rotation)
		{
			rotation = npc.rotation;
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(npc.chaseable);
            writer.Write(npc.dontTakeDamage);
			writer.Write(chargeVelocityScalar);
			writer.Write(vulnerable);
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
			chargeVelocityScalar = reader.ReadSingle();
			vulnerable = reader.ReadBoolean();
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

			CalamityGlobalNPC.draedonExoMechWorm = npc.whoAmI;

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Check if the other exo mechs are alive
			int otherExoMechsAlive = 0;
			bool exoPrimeAlive = false;
			bool exoTwinsAlive = false;
			if (CalamityGlobalNPC.draedonExoMechPrime != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
				{
					otherExoMechsAlive++;
					exoPrimeAlive = true;
				}
			}

			// There is no need in checking for the other twin because they have linked HP
			if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
				{
					otherExoMechsAlive++;
					exoTwinsAlive = true;
				}
			}

			// These are 5 by default to avoid triggering passive phases after the other mechs are dead
			float exoPrimeLifeRatio = defaultLifeRatio;
			float exoTwinsLifeRatio = defaultLifeRatio;
			if (exoPrimeAlive)
				exoPrimeLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].lifeMax;
			if (exoTwinsAlive)
				exoTwinsLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].lifeMax;
			float totalOtherExoMechLifeRatio = exoPrimeLifeRatio + exoTwinsLifeRatio;

			// Check if any of the other mechs are passive
			bool exoPrimePassive = false;
			bool exoTwinsPassive = false;
			if (exoPrimeAlive)
				exoPrimePassive = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Calamity().newAI[1] == (float)AresBody.SecondaryPhase.Passive;
			if (exoTwinsAlive)
				exoTwinsPassive = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[1] == (float)Apollo.Apollo.SecondaryPhase.Passive;
			bool anyOtherExoMechPassive = exoPrimePassive || exoTwinsPassive;

			// Check if any of the other mechs were spawned first
			bool exoPrimeWasFirst = false;
			bool exoTwinsWereFirst = false;
			if (exoPrimeAlive)
				exoPrimeWasFirst = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].ai[3] == 1f;
			if (exoTwinsAlive)
				exoTwinsWereFirst = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].ai[3] == 1f;
			bool otherExoMechWasFirst = exoPrimeWasFirst || exoTwinsWereFirst;

			// Prevent mechs from being respawned
			if (otherExoMechWasFirst)
				npc.ai[3] = 1f;

			// Phases
			bool spawnOtherExoMechs = lifeRatio < 0.7f && npc.ai[3] == 0f;
			bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
			bool lastMechAlive = berserk && otherExoMechsAlive == 0;

			// Set vulnerable to false by default
			vulnerable = false;

			// If Thanatos doesn't go berserk
			bool otherMechIsBerserk = exoPrimeLifeRatio < 0.4f || exoTwinsLifeRatio < 0.4f;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			// Target variable
			Player player = Main.player[npc.target];

			if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

			// Spawn segments
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!tailSpawned && npc.ai[0] == 0f)
				{
					int Previous = npc.whoAmI;
					for (int num36 = 0; num36 < maxLength; num36++)
					{
						int lol;
						if (num36 >= 0 && num36 < minLength)
						{
							if (num36 % 2 == 0)
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<ThanatosBody1>(), npc.whoAmI);
							else
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<ThanatosBody2>(), npc.whoAmI);
						}
						else
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<ThanatosTail>(), npc.whoAmI);

						Main.npc[lol].realLife = npc.whoAmI;
						Main.npc[lol].ai[2] = npc.whoAmI;
						Main.npc[lol].ai[1] = Previous;
						Main.npc[Previous].ai[0] = lol;
						NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
						Previous = lol;
					}
					tailSpawned = true;
				}
			}

			if (npc.life > Main.npc[(int)npc.ai[0]].life)
				npc.life = Main.npc[(int)npc.ai[0]].life;

			// General AI pattern
			// 0 - Fly towards the target for 7 seconds, gradually speeding up for the first 5 seconds and slowing down for the last 2 seconds, fire lasers from segments that are venting
			// 1 - Fly underneath the target and fire barrages of lasers
			// 2 - Fire deathray from mouth with a telegraph similar to the railgun from enter the gungeon, turn speed is very low during this to avoid cheap hits
			// 3 - Go passive and fly underneath the target while firing lasers
			// 4 - Go passive, immune and invisible; fly far underneath the target and do nothing until next phase

			// Attack patterns
			// If spawned first
			// Phase 1 - 0, 1, 0, 2
			// Phase 2 - 4
			// Phase 3 - 3

			// If berserk, this is the last phase of thanatos
			// Phase 4 - 0, 1, 0, 2

			// If not berserk
			// Phase 4 - 4
			// Phase 5 - 0, 1, 0, 2

			// If berserk, this is the last phase of thanatos
			// Phase 6 - 0, 1, 0, 2

			// If not berserk
			// Phase 6 - 4

			// Berserk, final phase of Thanatos
			// Phase 7 - 0, 1, 0, 2

			// Phase gate values
			float velocityAdjustTime = 20f;
			float speedUpTime = lastMechAlive ? 270f : berserk ? 360f : 480f;
			float slowDownTime = lastMechAlive ? 60f : berserk ? 90f : 120f;
			float chargePhaseGateValue = speedUpTime + slowDownTime;
			float laserBarrageDuration = lastMechAlive ? 285f : berserk ? 330f : 420f;

			// Adjust opacity
			bool invisiblePhase = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune;
			npc.dontTakeDamage = invisiblePhase;
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

			// Despawn if target is dead
			bool targetDead = false;
			if (player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (player.dead)
				{
					targetDead = true;
					npc.localAI[0] = 0f;
					npc.localAI[2] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					calamityGlobalNPC.newAI[3] = 0f;
					chargeVelocityScalar = 0f;
					npc.dontTakeDamage = true;

					npc.velocity.Y -= 1f;
					if ((double)npc.position.Y < Main.topWorld + 16f)
						npc.velocity.Y -= 1f;

					if ((double)npc.position.Y < Main.topWorld + 16f)
					{
						for (int a = 0; a < Main.maxNPCs; a++)
						{
							if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<Artemis.Artemis>() || Main.npc[a].type == ModContent.NPCType<AresBody>() ||
								Main.npc[a].type == ModContent.NPCType<AresLaserCannon>() || Main.npc[a].type == ModContent.NPCType<AresPlasmaFlamethrower>() ||
								Main.npc[a].type == ModContent.NPCType<AresTeslaCannon>() || Main.npc[a].type == ModContent.NPCType<AresGaussNuke>() ||
								Main.npc[a].type == ModContent.NPCType<Apollo.Apollo>() || Main.npc[a].type == ModContent.NPCType<ThanatosBody1>() ||
								Main.npc[a].type == ModContent.NPCType<ThanatosBody2>() || Main.npc[a].type == ModContent.NPCType<ThanatosTail>())
								Main.npc[a].active = false;
						}
					}
				}
			}

			// Rotation and direction
			npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
			int direction = npc.direction;
			npc.direction = npc.spriteDirection = (npc.velocity.X > 0f) ? 1 : (-1);
			if (direction != npc.direction)
				npc.netUpdate = true;

			// Default vector to fly to
			Vector2 destination = player.Center;

			// Charge variables
			float turnDistance = baseTurnDistance;
			float chargeLocationDistance = turnDistance * 0.2f;

			// Laser Barrage variables
			float laserBarrageLocationBaseDistance = SecondaryAIState == (int)SecondaryPhase.PassiveAndImmune ? baseDistance * 2f : baseDistance;
			Vector2 laserBarrageLocation = new Vector2(0f, laserBarrageLocationBaseDistance);
			float laserBarrageLocationDistance = turnDistance * 3f;

			// Velocity and turn speed values
			float baseVelocityMult = (berserk ? 0.5f : 0f) + (malice ? 1.3f : death ? 1.2f : revenge ? 1.15f : expertMode ? 1.1f : 1f);
			float baseVelocity = 9f * baseVelocityMult;
			float turnDegrees = baseVelocity * 0.11f * (berserk ? 1.5f : 1f);

			// Increase top velocity if target is dead
			if (targetDead)
				baseVelocity *= 4f;

			float turnSpeed = MathHelper.ToRadians(turnDegrees);
			float chargeVelocityMult = MathHelper.Lerp(1f, 1.5f, chargeVelocityScalar);
			float chargeTurnSpeedMult = MathHelper.Lerp(1f, 1.5f, chargeVelocityScalar);
			float laserBarragePhaseVelocityMult = MathHelper.Lerp(1f, 1.5f, chargeVelocityScalar);
			float laserBarragePhaseTurnSpeedMult = MathHelper.Lerp(1f, 3f, chargeVelocityScalar);
			float deathrayVelocityMult = MathHelper.Lerp(0.5f, 3f, chargeVelocityScalar);
			float deathrayTurnSpeedMult = MathHelper.Lerp(0.5f, 3f, chargeVelocityScalar);

			// Base scale on total time spent in phase
			float chargeVelocityScalarIncrement = 1f / speedUpTime;
			float chargeVelocityScalarDecrement = 1f / slowDownTime;

			// Scalar to use during deathray phase
			float deathrayVelocityScalarIncrement = 1f / deathrayDuration;

			// Scalar to use during laser barrage, passive and immune phases
			float laserBarrageVelocityScalarIncrement = lastMechAlive ? 0.025f : berserk ? 0.02f : 0.01f;
			float laserBarrageVelocityScalarDecrement = 1f / velocityAdjustTime;

			// Distance from target
			float distanceFromTarget = Vector2.Distance(npc.Center, player.Center);

			// Passive and Immune phases
			switch ((int)SecondaryAIState)
			{
				case (int)SecondaryPhase.Nothing:

					// Spawn the other mechs if Thanatos is first
					if (otherExoMechsAlive == 0)
					{
						if (spawnOtherExoMechs)
						{
							// Reset everything
							npc.ai[3] = 1f;
							SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
							npc.localAI[0] = 0f;
							npc.localAI[2] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;
							calamityGlobalNPC.newAI[3] = 0f;
							chargeVelocityScalar = 0f;
							npc.TargetClosest();

							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								// Spawn the fuckers
								NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<AresBody>());
								NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Artemis.Artemis>());
								NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Apollo.Apollo>());
							}
						}
					}
					else
					{
						// If not spawned first, go to passive state if any other mech is passive or if Thanatos is under 70% life
						// Do not run this if berserk
						// Do not run this if any exo mech is dead
						if ((anyOtherExoMechPassive || lifeRatio < 0.7f) && !berserk && totalOtherExoMechLifeRatio < 5f)
						{
							// Tells Thanatos to return to the battle in passive state and reset everything
							SecondaryAIState = (float)SecondaryPhase.Passive;
							npc.localAI[0] = 0f;
							npc.localAI[2] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;
							calamityGlobalNPC.newAI[3] = 0f;
							chargeVelocityScalar = 0f;
							npc.TargetClosest();
						}

						// Go passive and immune if one of the other mechs is berserk
						// This is only called if two exo mechs are alive
						if (otherMechIsBerserk)
						{
							// Reset everything
							SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
							npc.localAI[0] = 0f;
							npc.localAI[2] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;
							calamityGlobalNPC.newAI[3] = 0f;
							chargeVelocityScalar = 0f;
							npc.TargetClosest();
						}
					}

					break;

				// Fly underneath target and fire lasers
				case (int)SecondaryPhase.Passive:

					// Fire lasers while passive
					AIState = (float)Phase.UndergroundLaserBarrage;

					// Enter passive and invincible phase if one of the other exo mechs is berserk
					if (otherMechIsBerserk)
					{
						// Reset everything
						SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
						npc.localAI[0] = 0f;
						npc.localAI[2] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						chargeVelocityScalar = 0f;
						npc.TargetClosest();
					}

					// If Thanatos is the first mech to go berserk
					if (berserk)
					{
						// Reset everything
						AIState = (float)Phase.Charge;
						npc.localAI[0] = 0f;
						npc.localAI[2] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						chargeVelocityScalar = 0f;
						npc.TargetClosest();

						// Never be passive if berserk
						SecondaryAIState = (float)SecondaryPhase.Nothing;
					}

					break;

				// Fly underneath target and become immune
				case (int)SecondaryPhase.PassiveAndImmune:

					// Do nothing while immune
					AIState = (float)Phase.UndergroundLaserBarrage;

					// Enter the fight again if any of the other exo mechs is below 70% and the other mechs aren't berserk
					if ((exoPrimeLifeRatio < 0.7f || exoTwinsLifeRatio < 0.7f) && !otherMechIsBerserk)
					{
						// Tells Thanatos to return to the battle in passive state and reset everything
						// Return to normal phases if one or more mechs have been downed
						SecondaryAIState = totalOtherExoMechLifeRatio > 5f ? (float)SecondaryPhase.Nothing : (float)SecondaryPhase.Passive;
						npc.localAI[0] = 0f;
						npc.localAI[2] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						chargeVelocityScalar = 0f;
						npc.TargetClosest();
					}

					if (berserk)
					{
						// Reset everything
						AIState = (float)Phase.Charge;
						npc.localAI[0] = 0f;
						npc.localAI[2] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						chargeVelocityScalar = 0f;
						npc.TargetClosest();

						// Never be passive if berserk
						SecondaryAIState = (float)SecondaryPhase.Nothing;
					}

					break;
			}

			// Attacking phases
			switch ((int)AIState)
			{
				// Fly towards target and gain velocity over time
				case (int)Phase.Charge:

					// Use a lerp to smoothly scale up velocity and turn speed
					if (calamityGlobalNPC.newAI[3] == 0f)
					{
						chargeVelocityScalar += chargeVelocityScalarIncrement;
						if (chargeVelocityScalar >= 1f)
						{
							chargeVelocityScalar = 1f;
							calamityGlobalNPC.newAI[3] = 1f;
						}
					}
					else
					{
						chargeVelocityScalar -= chargeVelocityScalarDecrement;
						if (chargeVelocityScalar < 0f)
							chargeVelocityScalar = 0f;
					}

					baseVelocity *= chargeVelocityMult;
					turnSpeed *= chargeTurnSpeedMult;
					turnDistance = chargeLocationDistance;

					// Gradually turn slower if within 20 tiles of the target
					float turnSlowerDistanceGateValue = lastMechAlive ? 160f : 320f;
					if (distanceFromTarget < turnSlowerDistanceGateValue)
						turnSpeed *= distanceFromTarget / turnSlowerDistanceGateValue;

					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] >= chargePhaseGateValue)
					{
						AIState = npc.localAI[0] == 1f ? (float)Phase.Deathray : (float)Phase.UndergroundLaserBarrage;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						chargeVelocityScalar = 0f;
						npc.TargetClosest();
					}

					break;

				// Fly below and summon barrages of lasers
				case (int)Phase.UndergroundLaserBarrage:

					// Fly down
					destination += laserBarrageLocation;
					turnDistance = laserBarrageLocationDistance;

					// Use a lerp to smoothly scale up velocity and turn speed
					if (calamityGlobalNPC.newAI[3] == 0f)
					{
						chargeVelocityScalar += laserBarrageVelocityScalarIncrement;
						if (chargeVelocityScalar > 1f)
							chargeVelocityScalar = 1f;
					}

					baseVelocity *= laserBarragePhaseVelocityMult;
					turnSpeed *= laserBarragePhaseTurnSpeedMult;

					if ((destination - npc.Center).Length() < laserBarrageLocationDistance || calamityGlobalNPC.newAI[2] > 0f)
					{
						calamityGlobalNPC.newAI[2] += 1f;

						if (SecondaryAIState != (float)SecondaryPhase.Passive && SecondaryAIState != (float)SecondaryPhase.PassiveAndImmune)
						{
							if (calamityGlobalNPC.newAI[2] >= laserBarrageDuration)
							{
								// Use a lerp to smoothly scale down velocity and turn speed
								chargeVelocityScalar -= laserBarrageVelocityScalarDecrement;
								if (chargeVelocityScalar < 0f)
									chargeVelocityScalar = 0f;

								calamityGlobalNPC.newAI[3] += 1f;
								if (calamityGlobalNPC.newAI[3] >= velocityAdjustTime)
								{
									npc.localAI[0] = berserk ? 1f : 0f;
									AIState = (float)Phase.Charge;
									calamityGlobalNPC.newAI[2] = 0f;
									calamityGlobalNPC.newAI[3] = 0f;
									chargeVelocityScalar = 0f;
									npc.TargetClosest();
								}
							}
						}
					}

					break;

				// Move close to target, reduce velocity and turn speed when close enough, create telegraph beams, reduce turn speed, fire deathray
				case (int)Phase.Deathray:

					// Head is vulnerable while charging and firing deathray
					vulnerable = true;

					// If close enough to the target, prepare to fire deathray
					float slowDownDistance = lastMechAlive ? 640f : 800f;
					bool readyToFireDeathray = distanceFromTarget < slowDownDistance;
					if (readyToFireDeathray)
						npc.localAI[2] = 1f;

					// Use a lerp to smoothly scale up velocity and turn speed
					if (calamityGlobalNPC.newAI[3] == 0f)
					{
						chargeVelocityScalar += deathrayVelocityScalarIncrement;
						if (chargeVelocityScalar >= 1f)
						{
							chargeVelocityScalar = 1f;

							// If ready to fire deathray, start reducing the velocity scalar
							if (npc.localAI[2] == 1f)
								calamityGlobalNPC.newAI[3] = 1f;
						}
					}
					else
					{
						// Reduce velocity scalar very quickly
						chargeVelocityScalar -= deathrayVelocityScalarIncrement * 5f;
						if (chargeVelocityScalar < 0f)
							chargeVelocityScalar = 0f;
					}

					baseVelocity *= deathrayVelocityMult;
					turnSpeed *= deathrayTurnSpeedMult;
					turnDistance = chargeLocationDistance;

					// Gradually turn and move slower if within 50 tiles of the target
					if (npc.localAI[2] == 1f)
					{
						// Exponentially scale down velocity if close to the target
						float velocityScale = distanceFromTarget / slowDownDistance;
						if (velocityScale < 1f)
							velocityScale *= velocityScale;

						baseVelocity *= velocityScale;
						turnSpeed *= velocityScale;

						calamityGlobalNPC.newAI[2] += 1f;
						if (calamityGlobalNPC.newAI[2] < deathrayTelegraphDuration)
						{
							// Fire deathray telegraph beams
							if (calamityGlobalNPC.newAI[2] == 1f)
							{
								if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, npc.Center) < soundDistance)
								{
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/THanosLaser"),
										(int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
								}

								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									int type = ModContent.ProjectileType<ExoDestroyerBeamTelegraph>();
									for (int b = 0; b < 6; b++)
									{
										int beam = Projectile.NewProjectile(npc.Center, Vector2.Zero, type, 0, 0f, 255, npc.whoAmI);

										// Determine the initial offset angle of telegraph. It will be smoothened to give a "stretch" effect.
										if (Main.projectile.IndexInRange(beam))
										{
											float squishedRatio = (float)Math.Pow((float)Math.Sin(MathHelper.Pi * b / 6f), 2D);
											float smoothenedRatio = MathHelper.SmoothStep(0f, 1f, squishedRatio);
											Main.projectile[beam].ai[0] = npc.whoAmI;
											Main.projectile[beam].ai[1] = MathHelper.Lerp(-0.74f, 0.74f, smoothenedRatio);
										}
									}
									int beam2 = Projectile.NewProjectile(npc.Center, Vector2.Zero, type, 0, 0f, 255, npc.whoAmI);
									if (Main.projectile.IndexInRange(beam2))
										Main.projectile[beam2].ai[0] = npc.whoAmI;
								}
							}
						}
						else
						{
							// Fire deathray
							if (calamityGlobalNPC.newAI[2] == deathrayTelegraphDuration)
							{
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									int type = ModContent.ProjectileType<ExoDestroyerBeamStart>();
									int damage = npc.GetProjectileDamage(type);
									int laser = Projectile.NewProjectile(npc.Center, Vector2.Zero, type, damage, 0f, Main.myPlayer, npc.whoAmI);
									if (Main.projectile.IndexInRange(laser))
										Main.projectile[laser].ai[0] = npc.whoAmI;
								}
							}
						}

						if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration + deathrayDuration)
						{
							npc.localAI[0] = 0f;
							npc.localAI[2] = 0f;
							AIState = (float)Phase.Charge;
							calamityGlobalNPC.newAI[2] = 0f;
							calamityGlobalNPC.newAI[3] = 0f;
							chargeVelocityScalar = 0f;
							npc.TargetClosest();
						}
					}

					break;
			}

			if (npc.localAI[3] < immunityTime)
				npc.localAI[3] += 1f;

			// Homing only works if vulnerable is true
			npc.chaseable = vulnerable;

			// Adjust DR based on vulnerable
			npc.Calamity().DR = vulnerable ? 0f : 0.9999f;
			npc.Calamity().unbreakableDR = !vulnerable;

			// Increase overall damage taken while vulnerable
			npc.takenDamageMultiplier = vulnerable ? 1.1f : 1f;

			// Vent noise and steam
			SmokeDrawer.ParticleSpawnRate = 9999999;
			if (vulnerable)
			{
				// Light
				Lighting.AddLight(npc.Center, 0.35f * npc.Opacity, 0.05f * npc.Opacity, 0.05f * npc.Opacity);

				// Noise
				if (npc.localAI[1] == 0f)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ThanatosVent"), npc.Center);

				// Steam
				npc.localAI[1] += 1f;
				if (npc.localAI[1] < ventDuration)
				{
					SmokeDrawer.BaseMoveRotation = npc.rotation - MathHelper.PiOver2;
					SmokeDrawer.ParticleSpawnRate = ventCloudSpawnRate;
				}
			}
			else
			{
				// Light
				Lighting.AddLight(npc.Center, 0.05f * npc.Opacity, 0.2f * npc.Opacity, 0.2f * npc.Opacity);

				npc.localAI[1] = 0f;
			}

			SmokeDrawer.Update();

			if (!targetDead)
			{
				// Increase velocity if velocity is ever zero
				if (npc.velocity == Vector2.Zero)
					npc.velocity = Vector2.Normalize(player.Center - npc.Center).SafeNormalize(Vector2.Zero) * baseVelocity;

				// Acceleration
				if (!((destination - npc.Center).Length() < turnDistance))
				{
					float targetAngle = npc.AngleTo(destination);
					float f = npc.velocity.ToRotation().AngleTowards(targetAngle, turnSpeed);
					npc.velocity = f.ToRotationVector2() * baseVelocity;
				}
			}

			// Velocity upper limit
			if (npc.velocity.Length() > baseVelocity)
				npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * baseVelocity;
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

			return minDist <= 50f && npc.Opacity == 1f;
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			if (npc.localAI[3] < immunityTime)
				damage *= 0.01;

			return !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 1.5f;
			return null;
		}

		public override void FindFrame(int frameHeight) // 5 total frames
		{
			// Swap between venting and non-venting frames
			npc.frameCounter += 1D;
			if (AIState == (float)Phase.Charge || AIState == (float)Phase.UndergroundLaserBarrage)
			{
				if (npc.frameCounter >= 6D)
				{
					npc.frame.Y -= frameHeight;
					npc.frameCounter = 0D;
				}
				if (npc.frame.Y < 0)
					npc.frame.Y = 0;
			}
			else
			{
				if (npc.frameCounter >= 6D)
				{
					npc.frame.Y += frameHeight;
					npc.frameCounter = 0D;
				}
				int finalFrame = Main.npcFrameCount[npc.type] - 1;
				if (npc.frame.Y >= frameHeight * finalFrame)
					npc.frame.Y = frameHeight * finalFrame;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture = Main.npcTexture[npc.type];
			Vector2 vector = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);

			Vector2 center = npc.Center - Main.screenPosition;
			center -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			center += vector * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture, center, npc.frame, npc.GetAlpha(drawColor), npc.rotation, vector, npc.scale, spriteEffects, 0f);

			texture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosHeadGlow");
			spriteBatch.Draw(texture, center, npc.frame, Color.White * npc.Opacity, npc.rotation, vector, npc.scale, spriteEffects, 0f);

			SmokeDrawer.DrawSet(npc.Center);

			return false;
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ModContent.ItemType<OmegaHealingPotion>();
		}

		public override bool SpecialNPCLoot()
		{
			int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
				ModContent.NPCType<ThanatosHead>(),
				ModContent.NPCType<ThanatosBody1>(),
				ModContent.NPCType<ThanatosBody2>(),
				ModContent.NPCType<ThanatosTail>());
			npc.position = Main.npc[closestSegmentID].position;
			return false;
		}

		public override void NPCLoot()
        {
			// DropHelper.DropItemChance(npc, ModContent.ItemType<ThanatosTrophy>(), 10);

			// Check if the other exo mechs are alive
			bool otherExoMechsAlive = false;
			if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
					otherExoMechsAlive = true;
			}
			if (CalamityGlobalNPC.draedonExoMechPrime != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
					otherExoMechsAlive = true;
			}

			if (!otherExoMechsAlive)
				AresBody.DropExoMechLoot(npc);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.soundDelay == 0)
			{
				npc.soundDelay = 8;

				if (vulnerable)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/OtherworldlyHit"), npc.Center);
				else
					Main.PlaySound(SoundID.NPCHit4, npc.Center);
			}

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

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosHead"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosHead2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosHead3"), 1f);
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
