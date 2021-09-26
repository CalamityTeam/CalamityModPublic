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
		}

        public override void SetDefaults()
        {
			npc.npcSlots = 5f;
			npc.damage = 100;
			npc.width = 152;
            npc.height = 90;
            npc.defense = 80;
			npc.DR_NERD(0.25f);
			npc.LifeMaxNERB(1300000, 1495000, 500000);
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

			// Predictiveness
			float predictionAmt = malice ? 20f : death ? 15f : revenge ? 12.5f : expertMode ? 10f : 5f;
			if (passivePhase)
				predictionAmt *= 0.5f;

			Vector2 predictionVector = player.velocity * predictionAmt;
			Vector2 rotationVector = player.Center + predictionVector - npc.Center;

			float projectileVelocity = passivePhase ? 9.6f : 12f;
			if (lastMechAlive)
				projectileVelocity *= 1.2f;
			else if (berserk)
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
			Lighting.AddLight(npc.Center, 0.1f * npc.Opacity, 0.25f * npc.Opacity, 0.05f * npc.Opacity);

			// Despawn if target is dead
			if (player.dead)
			{
				player = Main.player[Main.npc[CalamityGlobalNPC.draedonExoMechPrime].target];
				if (player.dead)
				{
					AIState = (float)Phase.Nothing;
					calamityGlobalNPC.newAI[1] = 0f;
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
			Vector2 destination = calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays ? new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.X + 540f, Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.Y - 540f) : new Vector2(Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.X + 375f, Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Center.Y + 160f);

			// Velocity and acceleration values
			float baseVelocityMult = (berserk ? 0.25f : 0f) + (malice ? 1.3f : death ? 1.2f : revenge ? 1.15f : expertMode ? 1.1f : 1f);
			float baseVelocity = 22f * baseVelocityMult;

			Vector2 distanceFromDestination = destination - npc.Center;

			// Distance where Ares Plasma Arm stops moving
			float movementDistanceGateValue = 50f;

			// Gate values
			bool fireMoreBolts = calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays;
			float plasmaBoltPhaseGateValue = fireMoreBolts ? 120f : 270f;
			if (lastMechAlive)
				plasmaBoltPhaseGateValue *= 0.7f;
			else if (berserk)
				plasmaBoltPhaseGateValue *= 0.85f;

			// Variable to cancel plasma bolt firing
			bool doNotFire = calamityGlobalNPC_Body.newAI[1] == (float)AresBody.SecondaryPhase.PassiveAndImmune || (calamityGlobalNPC_Body.newAI[2] >= AresBody.deathrayTelegraphDuration + AresBody.deathrayDuration - 1 && calamityGlobalNPC_Body.newAI[0] == (float)AresBody.Phase.Deathrays);
			if (doNotFire)
			{
				AIState = (float)Phase.Nothing;
				calamityGlobalNPC.newAI[1] = 0f;
				calamityGlobalNPC.newAI[2] = 0f;
			}

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
					}
					else
					{
						// Fire plasma bolts
						int numPlasmaBolts = lastMechAlive ? 3 : 2;
						float divisor = plasmaBoltDuration / numPlasmaBolts;

						if (calamityGlobalNPC.newAI[2] % divisor == 0f)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaCasterFire"), npc.Center);
								Vector2 plasmaBoltVelocity = Vector2.Normalize(rotationVector) * projectileVelocity;
								int type = ModContent.ProjectileType<AresPlasmaFireball>();
								int damage = npc.GetProjectileDamage(type);
								Vector2 offset = Vector2.Normalize(plasmaBoltVelocity) * 40f + Vector2.UnitY * 16f;
								Projectile.NewProjectile(npc.Center + offset, plasmaBoltVelocity, type, damage, 0f, Main.myPlayer, player.Center.X, player.Center.Y);
							}
						}
					}

					if (calamityGlobalNPC.newAI[2] >= plasmaBoltTelegraphDuration + plasmaBoltDuration)
					{
						AIState = (float)Phase.Nothing;
						calamityGlobalNPC.newAI[2] = 0f;
					}

					break;
			}

			// Movement
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

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) => !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);

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

			texture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Ares/AresPlasmaFlamethrowerGlow");

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
