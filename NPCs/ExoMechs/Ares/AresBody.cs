using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Potions;
using CalamityMod.NPCs.ExoMechs.Thanatos;
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

namespace CalamityMod.NPCs.ExoMechs.Ares
{
	//[AutoloadBossHead]
	public class AresBody : ModNPC
    {
		public enum Phase
		{
			Normal = 0,
			Deathrays = 1
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
		private const int maxFramesX = 6;
		private const int maxFramesY = 8;

		// Counters for frames on the X and Y axis
		private int frameX = 0;
		private int frameY = 0;

		// The exact frame the animation is currently on
		private int exactFrame = 0;

		// Frame limit per animation, these are the specific frames where each animation ends
		private const int normalFrameLimit = 11;
		private const int firstStageDeathrayChargeFrameLimit = 23;
		private const int secondStageDeathrayChargeFrameLimit = 35;
		private const int finalStageDeathrayChargeFrameLimit = 47;

		// Default life ratio for the other mechs
		private const float defaultLifeRatio = 5f;

		// Max distance from the target before they are unable to hear sound telegraphs
		private const float soundDistance = 2800f;

		// Total duration of the deathray telegraph
		private const float deathrayTelegraphDuration = 240f;

		// Total duration of the deathrays
		private const float deathrayDuration = 600f;

		// Whether or not the other exo mechs have been spawned
		private bool otherExoMechsSpawned = false;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("XF-09 Ares");
		}

        public override void SetDefaults()
        {
			npc.npcSlots = 5f;
			npc.damage = 100;
			npc.width = 220;
            npc.height = 252;
            npc.defense = 100;
			npc.DR_NERD(0.35f);
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
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			npc.chaseable = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			CalamityGlobalNPC.draedonExoMechPrime = npc.whoAmI;

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

			// Spawn arms
			if (npc.ai[0] == 0f)
			{
				int totalArms = 4;
				for (int i = 0; i < totalArms; i++)
				{
					int lol = 0;
					switch (i)
					{
						case 0:
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AresLaserCannon>(), npc.whoAmI);
							break;
						case 1:
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AresPlasmaFlamethrower>(), npc.whoAmI);
							break;
						case 2:
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AresTeslaCannon>(), npc.whoAmI);
							break;
						case 3:
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AresGaussNuke>(), npc.whoAmI);
							break;
						default:
							break;
					}

					Main.npc[lol].realLife = npc.whoAmI;
					Main.npc[lol].ai[0] = npc.whoAmI;
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
				}
				npc.ai[0] = 1f;
			}

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Check if the other exo mechs are alive
			int otherExoMechsAlive = 0;
			bool exoWormAlive = false;
			bool exoSpazAlive = false;
			bool exoRetAlive = false;
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
					otherExoMechsAlive++;
					exoSpazAlive = true;
				}
			}
			if (CalamityGlobalNPC.draedonExoMechTwinRed != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].active)
				{
					otherExoMechsAlive++;
					exoRetAlive = true;
				}
			}

			// These are 5 by default to avoid triggering passive phases after the other mechs are dead
			float exoWormLifeRatio = defaultLifeRatio;
			float exoSpazLifeRatio = defaultLifeRatio;
			float exoRetLifeRatio = defaultLifeRatio;
			if (exoWormAlive)
				exoWormLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechWorm].lifeMax;
			if (exoSpazAlive)
				exoSpazLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].lifeMax;
			if (exoRetAlive)
				exoRetLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].lifeMax;
			float totalOtherExoMechLifeRatio = exoWormLifeRatio + exoSpazLifeRatio + exoRetLifeRatio;

			// Check if any of the other mechs are passive
			bool exoWormPassive = false;
			bool exoSpazPassive = false;
			bool exoRetPassive = false;
			if (exoWormAlive)
				exoWormPassive = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].Calamity().newAI[1] == (float)ThanatosHead.SecondaryPhase.Passive;
			/*if (exoSpazAlive)
				exoSpazPassive = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[1] == (float)Apollo.SecondaryPhase.Passive;
			if (exoRetAlive)
				exoRetPassive = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Calamity().newAI[1] == (float)Artemis.SecondaryPhase.Passive;*/
			bool anyOtherExoMechPassive = exoWormPassive || exoSpazPassive || exoRetPassive;

			// Phases
			bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
			bool spawnOtherExoMechs = lifeRatio > 0.4f && otherExoMechsAlive == 0 && lifeRatio < 0.7f;

			// If Ares doesn't go berserk
			bool otherMechIsBerserk = exoWormLifeRatio < 0.4f || exoSpazLifeRatio < 0.4f || exoRetLifeRatio < 0.4f;

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

					/*if ((double)npc.position.Y < Main.topWorld + 16f)
					{
						for (int a = 0; a < Main.maxNPCs; a++)
						{
							if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<AresLaserCannon>() || Main.npc[a].type == ModContent.NPCType<AresGaussNuke>() || Main.npc[a].type == ModContent.NPCType<AresPlasmaFlamethrower>() || Main.npc[a].type == ModContent.NPCType<AresTeslaCannon>())
								Main.npc[a].active = false;
						}
					}*/
				}
			}

			// General AI pattern
			// 0 - Fly above target
			// 1 - Fly towards the target, slow down when close enough
			// 2 - Fire deathrays from telegraph locations to avoid cheap hits and rotate them around for 10 seconds while the plasma and tesla arms fire projectiles to make dodging difficult
			// 3 - Go passive and fly above the target while firing less projectiles
			// 4 - Go passive, immune and invisible; fly above the target and do nothing until next phase

			// Attack patterns
			// If spawned first
			// Phase 1 - 0
			// Phase 2 - 4
			// Phase 3 - 3

			// If berserk, this is the last phase of Ares
			// Phase 4 - 1, 2

			// If not berserk
			// Phase 4 - 4
			// Phase 5 - 0

			// If berserk, this is the last phase of Ares
			// Phase 6 - 1, 2

			// If not berserk
			// Phase 6 - 4
			// Phase 7 - 1, 2

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

			// Rotation
			npc.rotation = npc.velocity.X * 0.003f;

			// Default vector to fly to
			Vector2 destination = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune ? new Vector2(player.Center.X, player.Center.Y - 800f) : AIState != (float)Phase.Deathrays ? new Vector2(player.Center.X, player.Center.Y - 500f) : player.Center;

			// Velocity and acceleration values
			float baseVelocityMult = malice ? 1.3f : death ? 1.2f : revenge ? 1.15f : expertMode ? 1.1f : 1f;
			float baseVelocity = 12f * baseVelocityMult;
			float baseAcceleration = 1f;
			if (berserk)
			{
				baseVelocity *= 1.25f;
				baseAcceleration *= 1.25f;
			}
			Vector2 desiredVelocity = Vector2.Normalize(destination - npc.Center) * baseVelocity;

			// Distance from target
			float distanceFromTarget = Vector2.Distance(npc.Center, player.Center);

			// Gate values
			float deathrayPhaseGateValue = 900f;
			float deathrayDistanceGateValue = 480f;
			float deathrayPhaseVelocityMult = 0.95f;

			// Passive and Immune phases
			switch ((int)SecondaryAIState)
			{
				case (int)SecondaryPhase.Nothing:

					// Spawn the other mechs if Ares is first
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
								// NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Artemis>());
								// NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Apollo>());
							}
						}
					}
					else
					{
						// If not spawned first, go to passive state if any other mech is passive or if Ares is under 70% life
						// Do not run this if berserk
						// Do not run this if any exo mech is dead
						if ((anyOtherExoMechPassive || lifeRatio < 0.7f) && !berserk && totalOtherExoMechLifeRatio < 5f)
						{
							// Tells Ares to return to the battle in passive state and reset everything
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

					// If Ares is the first mech to go berserk
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
					if ((exoWormLifeRatio < 0.7f || exoSpazLifeRatio < 0.7f || exoRetLifeRatio < 0.7f) && !otherMechIsBerserk)
					{
						// Tells Ares to return to the battle in passive state and reset everything
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
				// Fly above the target
				case (int)Phase.Normal:

					if (!targetDead)
						npc.SimpleFlyMovement(desiredVelocity, baseAcceleration);

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
							Vector2 desiredVelocity2 = Vector2.Normalize(destination - npc.Center) * baseVelocity;
							npc.SimpleFlyMovement(desiredVelocity2, baseAcceleration);
						}
						else
						{
							calamityGlobalNPC.newAI[3] = 1f;
							npc.velocity *= deathrayPhaseVelocityMult;

							int totalProjectiles = 8;
							float radians = MathHelper.TwoPi / totalProjectiles;
							Vector2 spinningPoint = new Vector2(0f, -1f);
							Vector2 laserSpawnPoint = new Vector2(npc.Center.X, npc.Center.Y);

							calamityGlobalNPC.newAI[2] += 1f;
							if (calamityGlobalNPC.newAI[2] < deathrayTelegraphDuration)
							{
								// Fire deathray telegraph beams
								if (calamityGlobalNPC.newAI[2] == 1f)
								{
									// Set frames to deathray charge up frames
									npc.frameCounter = 0D;
									frameX = 1;
									frameY = 5;
									exactFrame = 12;

									if (Main.netMode != NetmodeID.MultiplayerClient)
									{
										/*int type = ModContent.ProjectileType<AresDeathBeamTelegraph>();
										for (int k = 0; k < totalProjectiles; k++)
										{
											Vector2 velocity = spinningPoint.RotatedBy(radians * k);
											Projectile.NewProjectile(laserSpawnPoint, velocity, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
										}*/
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
										/*int type = ModContent.ProjectileType<AresDeathBeamStart>();
										int damage = npc.GetProjectileDamage(type);
										for (int k = 0; k < totalProjectiles; k++)
										{
											Vector2 velocity = spinningPoint.RotatedBy(radians * k);
											Projectile.NewProjectile(laserSpawnPoint, velocity, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
										}*/
									}
								}
							}

							if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration + deathrayDuration)
							{
								AIState = (float)Phase.Normal;
								calamityGlobalNPC.newAI[2] = 0f;
								calamityGlobalNPC.newAI[3] = 0f;
								npc.TargetClosest();
							}
						}
					}

					break;
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) => !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 2f;
			return null;
		}

		public override void FindFrame(int frameHeight)
		{
			// Use telegraph frames when using deathrays
			npc.frameCounter += 1D;
			if (AIState == (float)Phase.Normal || npc.Calamity().newAI[3] == 0f)
			{
				if (npc.frameCounter >= 10D)
				{
					npc.frameCounter = 0D;
					frameY++;
					exactFrame++;
					if (frameY == maxFramesY)
					{
						frameX++;
						frameY = 0;
					}
					if (exactFrame > normalFrameLimit)
						frameX = frameY = exactFrame = 0;
				}
			}
			else
			{
				if (npc.frameCounter >= 10D)
				{
					npc.frameCounter = 0D;
					frameY++;
					exactFrame++;
					if (frameY == maxFramesY)
					{
						frameX++;
						frameY = 0;
					}
					if (exactFrame > finalStageDeathrayChargeFrameLimit)
					{
						frameX = 4;
						frameY = 5;
						exactFrame = secondStageDeathrayChargeFrameLimit + 1;
					}
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Texture2D texture = Main.npcTexture[npc.type];
			Rectangle frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);
			Vector2 vector = new Vector2(npc.width / 2, npc.height / 2);
			Vector2 center = npc.Center - Main.screenPosition;
			spriteBatch.Draw(texture, center, frame, npc.GetAlpha(drawColor), npc.rotation, vector, npc.scale, SpriteEffects.None, 0f);

			texture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Ares/AresBodyGlow");
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

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody4"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody5"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody6"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresBody7"), 1f);
			}
		}

		public override bool CheckActive() => false;

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
    }
}
