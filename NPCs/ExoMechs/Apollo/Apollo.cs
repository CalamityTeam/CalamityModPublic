using CalamityMod.Events;
using CalamityMod.Items.Potions;
using CalamityMod.NPCs.ExoMechs.Artemis;
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

namespace CalamityMod.NPCs.ExoMechs.Apollo
{
	//[AutoloadBossHead]
	public class Apollo : ModNPC
    {
		public enum Phase
		{
			Normal = 0,
			RocketBarrage = 1,
			ChargeCombo = 2,
			PhaseTransition = 3
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

		// Total duration of attack telegraphs
		public const float attackTelegraphDuration = 100f;

		// Total duration of the phase transition
		public const float phaseTransitionDuration = 300f;

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
            npc.value = Item.buyPrice(10, 0, 0, 0);
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
			npc.boss = true;
			music = /*CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm") ??*/ MusicID.Boss3;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(npc.chaseable);
            writer.Write(npc.dontTakeDamage);
			writer.Write(npc.localAI[0]);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			npc.chaseable = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			CalamityGlobalNPC.draedonExoMechTwinGreen = npc.whoAmI;

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

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
			if (CalamityGlobalNPC.draedonExoMechTwinRed != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].active)
				{
					// Link the HP of both twins
					if (npc.life > Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].life)
						npc.life = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].life;
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

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			// Target variable
			Player player = Main.player[npc.target];

			// Spawn Artemis if it doesn't exist after the first 10 frames have passed
			if (npc.ai[0] < 10f)
			{
				npc.ai[0] += 1f;
				if (npc.ai[0] == 10f && !NPC.AnyNPCs(ModContent.NPCType<Artemis.Artemis>()))
					NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Artemis.Artemis>());
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
					calamityGlobalNPC.newAI[2] = 0f;
					calamityGlobalNPC.newAI[3] = 0f;

					npc.velocity.Y -= 2f;
					if ((double)npc.position.Y < Main.topWorld + 16f)
						npc.velocity.Y -= 2f;

					if ((double)npc.position.Y < Main.topWorld + 16f)
					{
						for (int a = 0; a < Main.maxNPCs; a++)
						{
							if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<Artemis.Artemis>())
								Main.npc[a].active = false;
						}
					}
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

			// Predictiveness
			float predictionAmt = malice ? 16f : death ? 12f : revenge ? 10f : expertMode ? 8f : 4f;
			if (SecondaryAIState == (int)SecondaryPhase.Passive)
				predictionAmt *= 0.5f;

			Vector2 predictionVector = AIState == (int)Phase.RocketBarrage ? Vector2.Zero : player.velocity * predictionAmt;

			// Rotation
			if (AIState == (int)Phase.ChargeCombo)
			{
				npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
			}
			else
			{
				float x = player.Center.X + predictionVector.X - npc.Center.X;
				float y = player.Center.Y + predictionVector.Y - npc.Center.Y;
				npc.rotation = (float)Math.Atan2(y, x) + MathHelper.PiOver2;
			}

			// Light
			Lighting.AddLight(npc.Center, 0.05f, 0.25f, 0.15f);

			// Default vector to fly to
			Vector2 destination = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune ? new Vector2(player.Center.X + 1200f, player.Center.Y) : AIState == (float)Phase.ChargeCombo ? new Vector2(player.Center.X, player.Center.Y + 500f) : new Vector2(player.Center.X + 750f, player.Center.Y);

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

			// Distance where Apollo stops moving
			float movementDistanceGateValue = 100f;

			// Gate values
			float chargePhaseGateValue = lastMechAlive ? 300f : 480f;
			float deathrayPhaseGateValue = lastMechAlive ? 630f : 900f;

			// Passive and Immune phases
			switch ((int)SecondaryAIState)
			{
				case (int)SecondaryPhase.Nothing:

					// Spawn the other mechs if Artemis and Apollo are first
					if (otherExoMechsAlive == 0)
					{
						if (spawnOtherExoMechs)
						{
							// Reset everything
							SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
							npc.TargetClosest();

							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								// Spawn code here
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
							// Tells Apollo to return to the battle in passive state and reset everything
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

			// Needs edits
			// Attacking phases
			/*switch ((int)AIState)
			{
				// Fly above the target
				case (int)Phase.Normal:

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

					if (berserk)
					{
						calamityGlobalNPC.newAI[2] += 1f;
						if (calamityGlobalNPC.newAI[2] > deathrayPhaseGateValue)
						{
							calamityGlobalNPC.newAI[2] = 0f;
							AIState = (float)Phase.Deathrays;
						}
					}

					break;

				// Move close to target, reduce velocity when close enough, create telegraph beams, fire deathrays
				case (int)Phase.Deathrays:

					if (!targetDead)
					{
						if (distanceFromTarget > deathrayDistanceGateValue && calamityGlobalNPC.newAI[3] == 0f)
						{
							Vector2 desiredVelocity2 = Vector2.Normalize(distanceFromDestination) * baseVelocity;
							npc.SimpleFlyMovement(desiredVelocity2, baseAcceleration);
						}
						else
						{
							calamityGlobalNPC.newAI[3] = 1f;
							npc.velocity *= decelerationVelocityMult;

							int totalProjectiles = malice ? 12 : expertMode ? 10 : 8;
							float radians = MathHelper.TwoPi / totalProjectiles;
							Vector2 laserSpawnPoint = new Vector2(npc.Center.X, npc.Center.Y);
							bool normalLaserRotation = npc.localAI[0] % 2f == 0f;
							float velocity = 6f;
							double angleA = radians * 0.5;
							double angleB = MathHelper.ToRadians(90f) - angleA;
							float velocityX2 = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
							Vector2 spinningPoint = normalLaserRotation ? new Vector2(0f, -velocity) : new Vector2(-velocityX2, -velocity);
							spinningPoint.Normalize();

							calamityGlobalNPC.newAI[2] += 1f;
							if (calamityGlobalNPC.newAI[2] < deathrayTelegraphDuration)
							{
								// Fire deathray telegraph beams
								if (calamityGlobalNPC.newAI[2] == 1f)
								{
									// Set frames to deathray charge up frames, which begin on frame 12
									// Reset the frame counter
									npc.frameCounter = 0D;

									// X = 1 sets to frame 8
									frameX = 1;

									// Y = 4 sets to frame 12
									frameY = 4;

									if (Main.netMode != NetmodeID.MultiplayerClient)
									{
										Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
										int type = ModContent.ProjectileType<AresDeathBeamTelegraph>();
										Vector2 spawnPoint = npc.Center + new Vector2(-1f, 23f);
										for (int k = 0; k < totalProjectiles; k++)
										{
											Vector2 laserVelocity = spinningPoint.RotatedBy(radians * k);
											Projectile.NewProjectile(spawnPoint + Vector2.Normalize(laserVelocity) * 17f, laserVelocity, type, 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
										}
									}
								}
							}
							else
							{
								// Fire deathrays
								if (calamityGlobalNPC.newAI[2] == deathrayTelegraphDuration)
								{
									if (Main.netMode != NetmodeID.MultiplayerClient)
									{
										Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/TeslaCannonFire"), npc.Center);
										int type = ModContent.ProjectileType<AresDeathBeamStart>();
										int damage = npc.GetProjectileDamage(type);
										Vector2 spawnPoint = npc.Center + new Vector2(-1f, 23f);
										for (int k = 0; k < totalProjectiles; k++)
										{
											Vector2 laserVelocity = spinningPoint.RotatedBy(radians * k);
											Projectile.NewProjectile(spawnPoint + Vector2.Normalize(laserVelocity) * 35f, laserVelocity, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
										}
									}
								}
							}

							if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration + deathrayDuration)
							{
								AIState = (float)Phase.Normal;
								calamityGlobalNPC.newAI[2] = 0f;
								calamityGlobalNPC.newAI[3] = 0f;
								npc.localAI[0] += 1f;
								npc.TargetClosest();
							}
						}
					}

					break;
			}*/
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) => !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 2f;
			return null;
		}

		// Needs edits
		/*public override void FindFrame(int frameHeight)
		{
			// Use telegraph frames when using deathrays
			npc.frameCounter += 1D;
			if (AIState == (float)Phase.Normal || npc.Calamity().newAI[3] == 0f)
			{
				if (npc.frameCounter >= 10D)
				{
					// Reset frame counter
					npc.frameCounter = 0D;

					// Increment the Y frame
					frameY++;

					// Reset the Y frame if greater than 8
					if (frameY == maxFramesY)
					{
						frameX++;
						frameY = 0;
					}

					// Reset the frames to frame 0
					if ((frameX * maxFramesY) + frameY > normalFrameLimit)
						frameX = frameY = 0;
				}
			}
			else
			{
				if (npc.frameCounter >= 10D)
				{
					// Reset frame counter
					npc.frameCounter = 0D;

					// Increment the Y frame
					frameY++;

					// Reset the Y frame if greater than 8
					if (frameY == maxFramesY)
					{
						frameX++;
						frameY = 0;
					}

					// Reset the frames to frame 36, the start of the deathray firing animation loop
					if ((frameX * maxFramesY) + frameY > finalStageDeathrayChargeFrameLimit)
						frameX = frameY = 4;
				}
			}
		}*/

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Texture2D texture = Main.npcTexture[npc.type];
			Rectangle frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);
			Vector2 vector = new Vector2(npc.width / 2, npc.height / 2);
			Vector2 center = npc.Center - Main.screenPosition;
			spriteBatch.Draw(texture, center, frame, npc.GetAlpha(drawColor), npc.rotation, vector, npc.scale, SpriteEffects.None, 0f);

			texture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Apollo/ApolloGlow");
			spriteBatch.Draw(texture, center, frame, Color.White * npc.Opacity, npc.rotation, vector, npc.scale, SpriteEffects.None, 0f);

			return false;
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ModContent.ItemType<OmegaHealingPotion>();
		}

		// Needs edits
		public override void NPCLoot()
        {
            /*DropHelper.DropItem(npc, ModContent.ItemType<Voidstone>(), 80, 100);
            DropHelper.DropItem(npc, ModContent.ItemType<EidolicWail>());
            DropHelper.DropItem(npc, ModContent.ItemType<SoulEdge>());
            DropHelper.DropItem(npc, ModContent.ItemType<HalibutCannon>());

            DropHelper.DropItemCondition(npc, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas, 1, 50, 108);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas && Main.expertMode, 2, 15, 27);
            DropHelper.DropItemCondition(npc, ItemID.Ectoplasm, NPC.downedPlantBoss, 1, 21, 32);*/
        }

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
