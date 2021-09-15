using CalamityMod.Events;
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

namespace CalamityMod.NPCs.ExoMechs.Ares
{
	//[AutoloadBossHead]
	public class AresLaserCannon : ModNPC
    {
		public enum Phase
		{
			Nothing = 0,
			Deathray = 1
		}

		public float AIState
		{
			get => npc.Calamity().newAI[0];
			set => npc.Calamity().newAI[0] = value;
		}

		// Number of frames on the X and Y axis
		private const int maxFramesX = 6;
		private const int maxFramesY = 8;

		// Counters for frames on the X and Y axis
		private int frameX = 0;
		private int frameY = 0;

		// Frame limit per animation, these are the specific frames where each animation ends
		private const int normalFrameLimit = 11;
		private const int firstStageDeathrayChargeFrameLimit = 23;
		private const int secondStageDeathrayChargeFrameLimit = 35;
		private const int finalStageDeathrayChargeFrameLimit = 47;

		// Default life ratio for the other mechs
		private const float defaultLifeRatio = 5f;

		// Total duration of the deathray telegraph
		private const float deathrayTelegraphDuration = 144f;

		// Total duration of the deathray
		private const float deathrayDuration = 60f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("XF-09 Ares Laser Cannon");
		}

        public override void SetDefaults()
        {
			npc.npcSlots = 5f;
			npc.damage = 100;
			npc.width = 154;
            npc.height = 90;
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

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(frameX);
			writer.Write(frameY);
            writer.Write(npc.dontTakeDamage);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			frameX = reader.ReadInt32();
			frameY = reader.ReadInt32();
			npc.dontTakeDamage = reader.ReadBoolean();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			if (CalamityGlobalNPC.draedonExoMechPrime < 0 || !Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
			{
				npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.checkDead();
				npc.active = false;
				return;
			}

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

			// Percent life remaining
			float lifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].lifeMax;

			// Check if the other exo mechs are alive
			int otherExoMechsAlive = 0;
			if (CalamityGlobalNPC.draedonExoMechWorm != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechWorm].active)
					otherExoMechsAlive++;
			}
			if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
					otherExoMechsAlive++;
			}

			// Phases
			bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
			bool lastMechAlive = berserk && otherExoMechsAlive == 0;

			// Target variable
			Player player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechPrime].target];

			if (npc.ai[2] > 0f)
				npc.realLife = (int)npc.ai[2];

			if (npc.life > Main.npc[(int)npc.ai[1]].life)
				npc.life = Main.npc[(int)npc.ai[1]].life;

			CalamityGlobalNPC calamityGlobalNPC_Body = Main.npc[(int)npc.ai[2]].Calamity();

			// Passive phase check
			bool passivePhase = calamityGlobalNPC_Body.newAI[1] == (float)AresBody.SecondaryPhase.Passive;

			// Adjust opacity
			bool invisiblePhase = calamityGlobalNPC_Body.newAI[1] == (float)AresBody.SecondaryPhase.PassiveAndImmune;
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

			// Variable to fire normal lasers
			bool fireNormalLasers = calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays;

			// Rotate the cannon to look at the target while not firing the beam
			// Rotate the cannon to look in the direction it will fire only while it's charging or while it's firing
			// Rotation
			bool horizontalLaserSweep = calamityGlobalNPC.newAI[3] == 0f;
			float rateOfRotation = AIState == (int)Phase.Deathray ? 0.08f : 0.04f;
			Vector2 lookAt = AIState == (int)Phase.Deathray && !fireNormalLasers ? (horizontalLaserSweep ? new Vector2(npc.Center.X, npc.Center.Y + 1000f) : new Vector2(npc.Center.X + 1000f, npc.Center.Y)) : player.Center;

			float rotation = (float)Math.Atan2(lookAt.Y - npc.Center.Y, lookAt.X - npc.Center.X);
			if (npc.spriteDirection == 1)
				rotation += MathHelper.Pi;
			if (rotation < 0f)
				rotation += MathHelper.TwoPi;
			if (rotation > MathHelper.TwoPi)
				rotation -= MathHelper.TwoPi;

			npc.rotation = npc.rotation.AngleTowards(rotation, rateOfRotation);

			// Direction
			int direction = Math.Sign(((AIState == (int)Phase.Deathray && !horizontalLaserSweep) || fireNormalLasers ? lookAt.X : player.Center.X) - npc.Center.X);
			if (direction != 0)
			{
				npc.direction = direction;

				if (npc.spriteDirection != -npc.direction)
					npc.rotation += MathHelper.Pi;

				npc.spriteDirection = -npc.direction;
			}

			// Light
			Lighting.AddLight(npc.Center, 0.25f * npc.Opacity, 0.1f * npc.Opacity, 0.1f * npc.Opacity);

			// Despawn if target is dead
			if (player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (player.dead)
				{
					AIState = (float)Phase.Nothing;
					calamityGlobalNPC.newAI[1] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.dontTakeDamage = true;

					npc.velocity.Y -= 2f;
					if ((double)npc.position.Y < Main.topWorld + 16f)
						npc.velocity.Y -= 2f;

					if ((double)npc.position.Y < Main.topWorld + 16f)
					{
						for (int a = 0; a < Main.maxNPCs; a++)
						{
							if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<Artemis.Artemis>() || Main.npc[a].type == ModContent.NPCType<AresBody>() ||
								Main.npc[a].type == ModContent.NPCType<Apollo.Apollo>() || Main.npc[a].type == ModContent.NPCType<AresPlasmaFlamethrower>() ||
								Main.npc[a].type == ModContent.NPCType<AresTeslaCannon>() || Main.npc[a].type == ModContent.NPCType<AresGaussNuke>() ||
								Main.npc[a].type == ModContent.NPCType<ThanatosHead>() || Main.npc[a].type == ModContent.NPCType<ThanatosBody1>() ||
								Main.npc[a].type == ModContent.NPCType<ThanatosBody2>() || Main.npc[a].type == ModContent.NPCType<ThanatosTail>())
								Main.npc[a].active = false;
						}
					}

					return;
				}
			}

			// Default vector to fly to
			Vector2 destination = calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays ? new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.X - 540f, Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.Y - 540f) : new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.X - 560f, Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.Y);

			// Velocity and acceleration values
			float baseVelocityMult = malice ? 1.3f : death ? 1.2f : revenge ? 1.15f : expertMode ? 1.1f : 1f;
			float baseVelocity = 16f * baseVelocityMult;
			float baseAcceleration = 1f;
			if (berserk)
			{
				baseVelocity *= 1.5f;
				baseAcceleration *= 1.5f;
			}

			Vector2 distanceFromDestination = destination - npc.Center;
			Vector2 desiredVelocity = Vector2.Normalize(distanceFromDestination) * baseVelocity;

			// Whether Ares Laser Arm should move to its spot or not
			float movementDistanceGateValue = 50f;
			bool moveToLocation = Vector2.Distance(npc.Center, destination) > movementDistanceGateValue;

			// Gate values
			float deathrayPhaseGateValue = 450f;
			if (lastMechAlive)
				deathrayPhaseGateValue *= 0.7f;
			else if (berserk)
				deathrayPhaseGateValue *= 0.85f;

			float deathrayPhaseVelocity = (passivePhase ? 9f : 12f) * baseVelocityMult;
			if (lastMechAlive)
				deathrayPhaseVelocity *= 1.2f;
			else if (berserk)
				deathrayPhaseVelocity *= 1.1f;

			// Variable to disable deathray firing
			bool doNotFire = calamityGlobalNPC_Body.newAI[1] == (float)AresBody.SecondaryPhase.PassiveAndImmune || (calamityGlobalNPC_Body.newAI[2] >= AresBody.deathrayTelegraphDuration + AresBody.deathrayDuration - 1 && fireNormalLasers);
			if (doNotFire)
			{
				AIState = (float)Phase.Nothing;
				calamityGlobalNPC.newAI[1] = 0f;
				calamityGlobalNPC.newAI[2] = 0f;
			}

			// Attacking phases
			switch ((int)AIState)
			{
				// Do nothing, rotate to aim at the target and fly in place
				case (int)Phase.Nothing:

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

					calamityGlobalNPC.newAI[1] += 1f;
					if (calamityGlobalNPC.newAI[1] >= deathrayPhaseGateValue)
					{
						AIState = (float)Phase.Deathray;
						calamityGlobalNPC.newAI[1] = 0f;
					}

					break;

				// Move close to target, reduce velocity when close enough, create telegraph beams, fire deathrays
				case (int)Phase.Deathray:

					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] < deathrayTelegraphDuration)
					{
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

						// Set frames to deathray charge up frames, which begin on frame 12
						if (calamityGlobalNPC.newAI[2] == 1f)
						{
							// Reset the frame counter
							npc.frameCounter = 0D;

							// X = 1 sets to frame 8
							frameX = 1;

							// Y = 4 sets to frame 12
							frameY = 4;
						}
					}
					else
					{
						// Fire regular Thanatos lasers if Ares is in deathray phase, otherwise, fire deathray
						if (fireNormalLasers)
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

							// Fire a Thanatos laser
							int numLasers = lastMechAlive ? 3 : 2;
							float divisor = deathrayDuration / numLasers;

							if (calamityGlobalNPC.newAI[2] % divisor == 0f)
							{
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									int type = ModContent.ProjectileType<ExoDestroyerLaser>();
									int damage = npc.GetProjectileDamage(type);
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
									Vector2 laserVelocity = Vector2.Normalize(player.Center - npc.Center);
									Vector2 offset = laserVelocity * 70f + Vector2.UnitY * 16f;
									Projectile.NewProjectile(npc.Center + offset, player.Center, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
								}
							}
						}
						else
						{
							// Two possible variants: 1 - Horizontal, 2 - Vertical

							// Movement while firing deathray
							if (horizontalLaserSweep)
								desiredVelocity.X = 0f;
							else
								desiredVelocity.Y = 0f;

							npc.SimpleFlyMovement(desiredVelocity, baseAcceleration);
							npc.velocity = horizontalLaserSweep ? new Vector2(deathrayPhaseVelocity, npc.velocity.Y) : new Vector2(npc.velocity.X, deathrayPhaseVelocity);

							// Fire deathray
							if (calamityGlobalNPC.newAI[2] == deathrayTelegraphDuration)
							{
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									int type = ModContent.ProjectileType<AresLaserBeamStart>();
									int damage = npc.GetProjectileDamage(type);
									float offset = 84f;
									float offset2 = 16f;
									Vector2 source = horizontalLaserSweep ? new Vector2(npc.Center.X - offset2 * npc.direction, npc.Center.Y + offset) : new Vector2(npc.Center.X + offset * npc.direction, npc.Center.Y + offset2);
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), source);
									Vector2 laserVelocity = Vector2.Normalize(lookAt - source);
									if (laserVelocity.HasNaNs())
										laserVelocity = -Vector2.UnitY;

									Projectile.NewProjectile(source, laserVelocity, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
								}
							}
						}
					}

					if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration + deathrayDuration)
					{
						// Reset
						AIState = (float)Phase.Nothing;
						calamityGlobalNPC.newAI[2] = 0f;

						// Change deathray sweep type for next deathray phase
						calamityGlobalNPC.newAI[3] += 1f;
						if (calamityGlobalNPC.newAI[3] > 1f)
							calamityGlobalNPC.newAI[3] = 0f;

						npc.TargetClosest();
					}

					break;
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) => !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);

		public override void FindFrame(int frameHeight)
		{
			// Use telegraph frames when using deathrays
			npc.frameCounter += 1D;
			if (AIState == (float)Phase.Nothing)
			{
				if (npc.frameCounter >= 6D)
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

					// Reset the frames
					if ((frameX * maxFramesY) + frameY > normalFrameLimit)
						frameX = frameY = 0;
				}
			}
			else
			{
				if (npc.frameCounter >= 6D)
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
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

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
					spriteBatch.Draw(texture, afterimageCenter, npc.frame, afterimageColor, npc.rotation, vector, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 center = npc.Center - Main.screenPosition;
			spriteBatch.Draw(texture, center, frame, npc.GetAlpha(drawColor), npc.rotation, vector, npc.scale, spriteEffects, 0f);

			texture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Ares/AresLaserCannonGlow");

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
					spriteBatch.Draw(texture, afterimageCenter, npc.frame, afterimageColor, npc.rotation, vector, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture, center, frame, Color.White * npc.Opacity, npc.rotation, vector, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool PreNPCLoot() => false;

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

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresLaserCannon1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresLaserCannon2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresHandBase1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresHandBase2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresHandBase3"), 1f);
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
