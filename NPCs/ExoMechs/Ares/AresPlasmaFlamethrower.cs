using CalamityMod.Events;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Ares
{
	public class AresPlasmaFlamethrower : ModNPC
    {
		public enum Phase
		{
			Nothing = 0,
			PlasmaBolts = 1
		}

		public float AIState
		{
			get => npc.Calamity().newAI[0];
			set => npc.Calamity().newAI[0] = value;
		}

		public ThanatosSmokeParticleSet SmokeDrawer = new ThanatosSmokeParticleSet(-1, 3, 0f, 16f, 1.5f);
		public AresCannonChargeParticleSet EnergyDrawer = new AresCannonChargeParticleSet(-1, 15, 40f, Color.GreenYellow);
		public Vector2 CoreSpritePosition => npc.Center + npc.spriteDirection * npc.rotation.ToRotationVector2() * 30f + (npc.rotation + MathHelper.PiOver2).ToRotationVector2() * 15f;

		// Number of frames on the X and Y axis
		private const int maxFramesX = 6;
		private const int maxFramesY = 8;

		// Counters for frames on the X and Y axis
		private int frameX = 0;
		private int frameY = 0;

		// Frame limit per animation, these are the specific frames where each animation ends
		private const int normalFrameLimit = 11;
		private const int firstStagePlasmaBoltChargeFrameLimit = 23;
		private const int secondStagePlasmaBoltChargeFrameLimit = 35;
		private const int finalStagePlasmaBoltChargeFrameLimit = 47;

		// Default life ratio for the other mechs
		private const float defaultLifeRatio = 5f;

		// Total duration of the plasma bolt telegraph
		private const float plasmaBoltTelegraphDuration = 144f;

		// Total duration of the plasma bolt firing phase
		private const float plasmaBoltDuration = 120f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("XF-09 Ares Plasma Cannon");
			NPCID.Sets.TrailingMode[npc.type] = 3;
			NPCID.Sets.TrailCacheLength[npc.type] = npc.oldPos.Length;
		}

        public override void SetDefaults()
        {
			npc.npcSlots = 5f;
			npc.damage = 100;
			npc.width = 152;
            npc.height = 90;
            npc.defense = 80;
			npc.DR_NERD(0.35f);
			npc.LifeMaxNERB(1250000, 1495000, 500000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
            aiType = -1;
			npc.Opacity = 0f;
            npc.knockBackResist = 0f;
			npc.canGhostHeal = false;
			npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
			npc.boss = true;
			npc.hide = true;
			music = CalamityMod.Instance.GetMusicFromMusicMod("ExoMechs") ?? MusicID.Boss3;
			npc.Calamity().VulnerableToSickness = false;
			npc.Calamity().VulnerableToElectricity = true;
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

			CalamityGlobalNPC.draedonExoMechPrimePlasmaCannon = npc.whoAmI;

			npc.frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);

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
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			// Percent life remaining
			float lifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].lifeMax;

			// Check if the other exo mechs are alive
			int otherExoMechsAlive = 0;
			bool exoWormAlive = false;
			bool exoTwinsAlive = false;
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
					exoTwinsAlive = true;
				}
			}

			// Used to nerf Ares if fighting alongside Artemis and Apollo, because otherwise it's too difficult
			bool nerfedAttacks = false;
			if (exoTwinsAlive)
				nerfedAttacks = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Calamity().newAI[1] != (float)Apollo.Apollo.SecondaryPhase.PassiveAndImmune;

			// Phases
			bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);
			bool lastMechAlive = berserk && otherExoMechsAlive == 0;

			// These are 5 by default to avoid triggering passive phases after the other mechs are dead
			float exoWormLifeRatio = defaultLifeRatio;
			float exoTwinsLifeRatio = defaultLifeRatio;
			if (exoWormAlive)
				exoWormLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechWorm].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechWorm].lifeMax;
			if (exoTwinsAlive)
				exoTwinsLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].lifeMax;

			// If Ares doesn't go berserk
			bool otherMechIsBerserk = exoWormLifeRatio < 0.4f || exoTwinsLifeRatio < 0.4f;

			// Whether Ares should be buffed while in berserk phase
			bool shouldGetBuffedByBerserkPhase = berserk && !otherMechIsBerserk;

			// Target variable
			Player player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechPrime].target];

			if (npc.ai[2] > 0f)
				npc.realLife = (int)npc.ai[2];

			if (npc.life > Main.npc[(int)npc.ai[1]].life)
				npc.life = Main.npc[(int)npc.ai[1]].life;

			CalamityGlobalNPC calamityGlobalNPC_Body = Main.npc[(int)npc.ai[2]].Calamity();

			// Passive phase check
			bool passivePhase = calamityGlobalNPC_Body.newAI[1] == (float)AresBody.SecondaryPhase.Passive;

			// Enrage check
			bool enraged = Main.npc[(int)npc.ai[2]].localAI[1] == (float)AresBody.Enraged.Yes;

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

			// Predictiveness
			float predictionAmt = malice ? 20f : death ? 15f : revenge ? 13.75f : expertMode ? 12.5f : 10f;
			if (nerfedAttacks)
				predictionAmt *= 0.5f;
			if (passivePhase)
				predictionAmt *= 0.5f;

			Vector2 predictionVector = player.velocity * predictionAmt;
			Vector2 rotationVector = player.Center + predictionVector - npc.Center;

			float projectileVelocity = passivePhase ? 9.6f : 12f;
			if (lastMechAlive)
				projectileVelocity *= 1.2f;
			else if (shouldGetBuffedByBerserkPhase)
				projectileVelocity *= 1.1f;

			float rateOfRotation = AIState == (int)Phase.PlasmaBolts ? 0.08f : 0.04f;
			Vector2 lookAt = Vector2.Normalize(rotationVector) * projectileVelocity;

			float rotation = (float)Math.Atan2(lookAt.Y, lookAt.X);
			if (npc.spriteDirection == 1)
				rotation += MathHelper.Pi;
			if (rotation < 0f)
				rotation += MathHelper.TwoPi;
			if (rotation > MathHelper.TwoPi)
				rotation -= MathHelper.TwoPi;

			npc.rotation = npc.rotation.AngleTowards(rotation, rateOfRotation);

			// Direction
			int direction = Math.Sign(player.Center.X - npc.Center.X);
			if (direction != 0)
			{
				npc.direction = direction;

				if (npc.spriteDirection != -npc.direction)
					npc.rotation += MathHelper.Pi;

				npc.spriteDirection = -npc.direction;
			}

			// Light
			if (enraged)
				Lighting.AddLight(npc.Center, 0.5f * npc.Opacity, 0f, 0f);
			else
				Lighting.AddLight(npc.Center, 0.1f * npc.Opacity, 0.25f * npc.Opacity, 0.05f * npc.Opacity);

			// Gate values
			bool fireMoreBolts = calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays;
			float plasmaBoltPhaseGateValue = fireMoreBolts ? 120f : 270f;
			if (enraged)
				plasmaBoltPhaseGateValue *= 0.1f;
			else if (lastMechAlive)
				plasmaBoltPhaseGateValue *= 0.4f;
			else if (shouldGetBuffedByBerserkPhase)
				plasmaBoltPhaseGateValue *= 0.7f;

			// Set attack timer to this when despawning or when Ares is coming out of deathray phase
			float setTimerTo = (int)(plasmaBoltPhaseGateValue * 0.95f) - 1;

			// Despawn if target is dead
			if (player.dead)
			{
				player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechPrime].target];
				if (player.dead)
				{
					AIState = (float)Phase.Nothing;
					calamityGlobalNPC.newAI[1] = setTimerTo;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.dontTakeDamage = true;

					npc.velocity.Y -= 1f;
					if ((double)npc.position.Y < Main.topWorld + 16f)
						npc.velocity.Y -= 1f;

					if ((double)npc.position.Y < Main.topWorld + 16f)
					{
						for (int a = 0; a < Main.maxNPCs; a++)
						{
							if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<Artemis.Artemis>() || Main.npc[a].type == ModContent.NPCType<AresBody>() ||
								Main.npc[a].type == ModContent.NPCType<AresLaserCannon>() || Main.npc[a].type == ModContent.NPCType<Apollo.Apollo>() ||
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
			float offsetX = 375f;
			float offsetY = 160f;
			float offsetX2 = 540f;
			float offsetY2 = -540f;
			switch ((int)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].ai[3])
			{
				case 0:
				case 1:
				case 4:
				case 5:
					break;

				case 2:
				case 3:
					offsetX *= -1f;
					offsetX2 *= -1f;
					offsetY2 *= -1f;
					break;
			}
			Vector2 destination = calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays ? new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.X + offsetX2, Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.Y + offsetY2) : new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.X + offsetX, Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.Y + offsetY);

			// Velocity and acceleration values
			float baseVelocityMult = (shouldGetBuffedByBerserkPhase ? 0.25f : 0f) + (malice ? 1.15f : death ? 1.1f : revenge ? 1.075f : expertMode ? 1.05f : 1f);
			float baseVelocity = (enraged ? 38f : 30f) * baseVelocityMult;
			baseVelocity *= 1f + Main.npc[(int)npc.ai[2]].localAI[2];

			Vector2 distanceFromDestination = destination - npc.Center;

			// Distance where Ares Plasma Arm stops moving
			float movementDistanceGateValue = 50f;

			// Make plasma bolts split less if in deathray spiral phase and not the last mech alive, but fire more if in deathray spiral phase
			bool boltsSplitLess = fireMoreBolts && !lastMechAlive;

			// If Plasma Cannon can fire projectiles, cannot fire if too close to the target and in deathray spiral phase
			bool canFire = Vector2.Distance(npc.Center, player.Center) > 320f || calamityGlobalNPC_Body.newAI[0] != (float)AresBody.Phase.Deathrays;

			// Telegraph duration for deathray spiral
			float deathrayTelegraphDuration = malice ? AresBody.deathrayTelegraphDuration_Malice : death ? AresBody.deathrayTelegraphDuration_Death :
				revenge ? AresBody.deathrayTelegraphDuration_Rev : expertMode ? AresBody.deathrayTelegraphDuration_Expert : AresBody.deathrayTelegraphDuration_Normal;

			// Variable to cancel plasma bolt firing
			bool doNotFire = calamityGlobalNPC_Body.newAI[1] == (float)AresBody.SecondaryPhase.PassiveAndImmune || 
				(calamityGlobalNPC_Body.newAI[2] >= deathrayTelegraphDuration + AresBody.deathrayDuration - 10 && calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays) ||
				(calamityGlobalNPC_Body.newAI[3] == 0f && calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays);

			if (doNotFire)
			{
				AIState = (float)Phase.Nothing;
				calamityGlobalNPC.newAI[1] = setTimerTo;
				calamityGlobalNPC.newAI[2] = 0f;
			}

			// Emit steam while enraged
			SmokeDrawer.ParticleSpawnRate = 9999999;
			if (enraged)
			{
				SmokeDrawer.ParticleSpawnRate = AresBody.ventCloudSpawnRate;
				SmokeDrawer.BaseMoveRotation = npc.rotation + MathHelper.Pi;
				SmokeDrawer.SpawnAreaCompactness = 40f;

				// Increase DR during enrage
				npc.Calamity().DR = 0.85f;
			}
			else
				npc.Calamity().DR = 0.35f;

			SmokeDrawer.Update();


			EnergyDrawer.ParticleSpawnRate = 9999999;
			// Attacking phases
			switch ((int)AIState)
			{
				// Do nothing and fly in place
				case (int)Phase.Nothing:

					calamityGlobalNPC.newAI[1] += 1f;
					if (calamityGlobalNPC.newAI[1] >= plasmaBoltPhaseGateValue)
					{
						AIState = (float)Phase.PlasmaBolts;
						calamityGlobalNPC.newAI[1] = 0f;
					}

					break;

				// Fire plasma bolts in a double burst that burst into a halo of smaller bolts
				case (int)Phase.PlasmaBolts:

					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] < plasmaBoltTelegraphDuration)
					{
						// Set frames to plasma orb charge up frames, which begin on frame 12
						if (calamityGlobalNPC.newAI[2] == 1f)
						{
							// Reset the frame counter
							npc.frameCounter = 0D;

							// X = 1 sets to frame 8
							frameX = 1;

							// Y = 4 sets to frame 12
							frameY = 4;
						}

						EnergyDrawer.ParticleSpawnRate = AresBody.telegraphParticlesSpawnRate;
						EnergyDrawer.SpawnAreaCompactness = 100f;
						EnergyDrawer.chargeProgress = calamityGlobalNPC.newAI[2] / plasmaBoltTelegraphDuration;
					}
					else if (calamityGlobalNPC.newAI[2] < plasmaBoltTelegraphDuration + plasmaBoltDuration)
					{
						// Fire plasma bolts
						int numPlasmaBolts = lastMechAlive ? 3 : 2;
						float divisor = plasmaBoltDuration / numPlasmaBolts;

						if ((calamityGlobalNPC.newAI[2] - plasmaBoltTelegraphDuration) % divisor == 0f && canFire)
						{
							npc.ai[3] += 1f;
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaCasterFire"), npc.Center);
								Vector2 plasmaBoltVelocity = Vector2.Normalize(rotationVector) * projectileVelocity;
								int type = ModContent.ProjectileType<AresPlasmaFireball>();
								int damage = npc.GetProjectileDamage(type);
								Vector2 offset = Vector2.Normalize(plasmaBoltVelocity) * 40f + Vector2.UnitY * 16f;

								if (boltsSplitLess)
									Projectile.NewProjectile(npc.Center + offset, plasmaBoltVelocity, type, damage, 0f, Main.myPlayer, -1f);
								else
									Projectile.NewProjectile(npc.Center + offset, plasmaBoltVelocity, type, damage, 0f, Main.myPlayer, player.Center.X, player.Center.Y);
							}
						}
					}

					if (calamityGlobalNPC.newAI[2] % (float)Math.Floor(plasmaBoltTelegraphDuration / 5f) == (float)Math.Floor(plasmaBoltTelegraphDuration / 5f) - 1 && calamityGlobalNPC.newAI[2] <= plasmaBoltTelegraphDuration)
					{
						float pulseCounter = (float)Math.Floor(calamityGlobalNPC.newAI[2] / (plasmaBoltTelegraphDuration / 5f)) + 1;
						EnergyDrawer.AddPulse(pulseCounter);
					}

					if (calamityGlobalNPC.newAI[2] >= plasmaBoltTelegraphDuration + plasmaBoltDuration)
					{
						AIState = (float)Phase.Nothing;
						calamityGlobalNPC.newAI[2] = 0f;
					}

					break;
			}

			EnergyDrawer.Update();

			// Smooth movement towards the location Ares Plasma Flamethrower is meant to be at
			CalamityGlobalNPC.SmoothMovement(npc, movementDistanceGateValue, distanceFromDestination, baseVelocity);
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

		public override void FindFrame(int frameHeight)
		{
			// Use telegraph frames when using plasma bolts
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

					// Reset the frames to frame 36, the start of the plasma bolt firing animation loop
					if ((frameX * maxFramesY) + frameY > finalStagePlasmaBoltChargeFrameLimit)
						frameX = frameY = 4;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			// Draw the enrage smoke behind Ares
			SmokeDrawer.DrawSet(npc.Center);

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture = Main.npcTexture[npc.type];
			Rectangle frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);
			Vector2 vector = new Vector2(npc.width / 2, npc.height / 2);
			Color afterimageBaseColor = Main.npc[(int)npc.ai[2]].localAI[1] == (float)AresBody.Enraged.Yes ? Color.Red : Color.White;
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
					afterimageCenter -= new Vector2(texture.Width, texture.Height) / new Vector2(maxFramesX, maxFramesY) * npc.scale / 2f;
					afterimageCenter += vector * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture, afterimageCenter, npc.frame, afterimageColor, npc.oldRot[i], vector, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 center = npc.Center - Main.screenPosition;

			//Draw an outline to the arm when it charges up
			if ((npc.Calamity().newAI[2] < plasmaBoltTelegraphDuration) && AIState == (float)Phase.PlasmaBolts)
			{
				CalamityUtils.EnterShaderRegion(spriteBatch);
				Color outlineColor = Color.Lerp(Color.GreenYellow, Color.White, npc.Calamity().newAI[2] / plasmaBoltTelegraphDuration);
				Vector3 outlineHSL = Main.rgbToHsl(outlineColor); //BasicTint uses the opposite hue i guess? or smth is fucked with the way shaders get their colors. anyways, we invert it
				float outlineThickness = MathHelper.Clamp(npc.Calamity().newAI[2] / plasmaBoltTelegraphDuration * 4f, 0f, 3f);

				GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(1f);
				GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1 - outlineHSL.X, outlineHSL.Y, outlineHSL.Z));
				GameShaders.Misc["CalamityMod:BasicTint"].Apply();

				for (float i = 0; i < 1; i += 0.125f)
				{
					spriteBatch.Draw(texture, center + (i * MathHelper.TwoPi + npc.rotation).ToRotationVector2() * outlineThickness, frame, outlineColor, npc.rotation, vector, npc.scale, spriteEffects, 0f);
				}
				CalamityUtils.ExitShaderRegion(spriteBatch);
			}

			spriteBatch.Draw(texture, center, frame, npc.GetAlpha(drawColor), npc.rotation, vector, npc.scale, spriteEffects, 0f);

			Texture2D glowTexture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Ares/AresPlasmaFlamethrowerGlow");

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int i = 1; i < numAfterimages; i += 2)
				{
					Color afterimageColor = drawColor;
					afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
					afterimageColor = npc.GetAlpha(afterimageColor);
					afterimageColor *= (numAfterimages - i) / 15f;
					Vector2 afterimageCenter = npc.oldPos[i] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					afterimageCenter -= new Vector2(glowTexture.Width, glowTexture.Height) / new Vector2(maxFramesX, maxFramesY) * npc.scale / 2f;
					afterimageCenter += vector * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(glowTexture, afterimageCenter, npc.frame, afterimageColor, npc.oldRot[i], vector, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(glowTexture, center, frame, afterimageBaseColor * npc.Opacity, npc.rotation, vector, npc.scale, spriteEffects, 0f);


			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
			//Update the parameters

			//Draw a pulsing version of the cannon above the real one
			if ((npc.Calamity().newAI[2] < plasmaBoltTelegraphDuration) && AIState == (float)Phase.PlasmaBolts)
			{

				float pulseRatio = (npc.Calamity().newAI[2] % (plasmaBoltTelegraphDuration / 5f)) / (plasmaBoltTelegraphDuration / 5f);
				float pulseSize = MathHelper.Lerp(0.1f, 0.6f, (float)Math.Floor(npc.Calamity().newAI[2] / (plasmaBoltTelegraphDuration / 5f)) / 4f);
				float pulseOpacity = MathHelper.Clamp((float)Math.Floor(npc.Calamity().newAI[2] / (plasmaBoltTelegraphDuration / 5f)) * 0.3f, 1f, 2f);
				spriteBatch.Draw(texture, center, frame, Color.GreenYellow * MathHelper.Lerp(1f, 0f, pulseRatio) * pulseOpacity, npc.rotation, vector, npc.scale + pulseRatio * pulseSize, spriteEffects, 0f);

				//Draw the bloom
				EnergyDrawer.DrawBloom(CoreSpritePosition);
			}

			EnergyDrawer.DrawPulses(CoreSpritePosition);
			EnergyDrawer.DrawSet(CoreSpritePosition);

			//Back to normal
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

			return false;
		}

		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCProjectiles.Add(index);
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

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresPlasmaFlamethrower1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Ares/AresPlasmaFlamethrower2"), 1f);
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
