using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Polterghast
{
	[AutoloadBossHead]
	public class PolterPhantom : ModNPC
    {
        private int despawnTimer = 600;
		private bool reachedChargingPoint = false;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polterghast");
            Main.npcFrameCount[npc.type] = 4;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.width = 90;
            npc.height = 120;
			npc.defense = 45;
			npc.DR_NERD(0.1f);
			npc.LifeMaxNERB(Main.expertMode ? 45000 : 35000, CalamityWorld.death ? 90000 : 75000, 60000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.Opacity = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit36;
            npc.DeathSound = SoundID.NPCDeath39;
			npc.Calamity().VulnerableToSickness = false;
		}

		public override void BossHeadRotation(ref float rotation)
		{
			bool polterHasTarget = CalamityGlobalNPC.ghostBoss.WithinBounds(Main.maxNPCs) && Main.npc[CalamityGlobalNPC.ghostBoss].active && Main.npc[CalamityGlobalNPC.ghostBoss].HasValidTarget;
			if (polterHasTarget && npc.Calamity().newAI[3] == 0f)
				rotation = (Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center - npc.Center).ToRotation() + MathHelper.PiOver2;
			else
				rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(despawnTimer);
			writer.Write(reachedChargingPoint);
			CalamityGlobalNPC cgn = npc.Calamity();
			writer.Write(cgn.newAI[0]);
			writer.Write(cgn.newAI[1]);
			writer.Write(cgn.newAI[2]);
			writer.Write(cgn.newAI[3]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            despawnTimer = reader.ReadInt32();
			reachedChargingPoint = reader.ReadBoolean();
			CalamityGlobalNPC cgn = npc.Calamity();
			cgn.newAI[0] = reader.ReadSingle();
			cgn.newAI[1] = reader.ReadSingle();
			cgn.newAI[2] = reader.ReadSingle();
			cgn.newAI[3] = reader.ReadSingle();
		}

        public override void AI()
        {
            CalamityGlobalNPC.ghostBossClone = npc.whoAmI;

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			if (CalamityGlobalNPC.ghostBoss < 0 || !Main.npc[CalamityGlobalNPC.ghostBoss].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.5f, 0.25f, 0.75f);

			Player player = Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target];

			// Percent life remaining, Polter
			float lifeRatio = Main.npc[CalamityGlobalNPC.ghostBoss].life / Main.npc[CalamityGlobalNPC.ghostBoss].lifeMax;

			Vector2 vector = npc.Center;

			// Scale multiplier based on nearby active tiles
			float tileEnrageMult = Main.npc[CalamityGlobalNPC.ghostBoss].ai[3];
			bool chargePhase = Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] >= 420f || npc.Calamity().newAI[3] == 1f;
			float chargeVelocity = 24f;
			float chargeAcceleration = 0.6f;
			float chargeDistance = 480f;

			bool speedBoost = false;
            bool despawnBoost = false;

            if (npc.timeLeft < 1500)
                npc.timeLeft = 1500;

            float velocity = 3f;
            float acceleration = 0.03f;
            if (!player.ZoneDungeon && !BossRushEvent.BossRushActive && player.position.Y < Main.worldSurface * 16.0)
            {
                despawnTimer--;
				if (despawnTimer <= 0)
				{
					despawnBoost = true;
					npc.ai[1] = 0f;
					npc.Calamity().newAI[0] = 0f;
					npc.Calamity().newAI[1] = 0f;
					npc.Calamity().newAI[2] = 0f;
					npc.Calamity().newAI[3] = 0f;
				}

                speedBoost = true;
				velocity += 8f;
				acceleration = 0.15f;
            }
            else
                despawnTimer++;

			if (Main.npc[CalamityGlobalNPC.ghostBoss].ai[2] < 300f)
            {
				velocity = 21f;
				acceleration = 0.13f;
            }

			if (expertMode)
			{
				chargeVelocity += revenge ? 4f : 2f;
				velocity += revenge ? 5f : 3.5f;
				acceleration += revenge ? 0.035f : 0.025f;
			}

			// Predictiveness
			Vector2 predictionVector = chargePhase && malice ? player.velocity * 20f : Vector2.Zero;
			Vector2 lookAt = player.Center + predictionVector;
			Vector2 rotationVector = lookAt - vector;

			// Rotation
			if (npc.Calamity().newAI[3] == 0f)
			{
				float num740 = player.Center.X + predictionVector.X - vector.X;
				float num741 = player.Center.Y + predictionVector.Y - vector.Y;
				npc.rotation = (float)Math.Atan2(num741, num740) + MathHelper.PiOver2;
			}
			else
				npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;

			npc.damage = npc.defDamage;

			if (!chargePhase)
			{
				// Set this here to avoid despawn issues
				reachedChargingPoint = false;

				npc.ai[0] = 0f;

				npc.Opacity += 0.02f;
				if (npc.Opacity > 0.8f)
					npc.Opacity = 0.8f;

				float movementLimitX = 0f;
				float movementLimitY = 0f;
				int numHooks = 4;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<PolterghastHook>())
					{
						movementLimitX += Main.npc[i].Center.X;
						movementLimitY += Main.npc[i].Center.Y;
					}
				}
				movementLimitX /= numHooks;
				movementLimitY /= numHooks;

				Vector2 vector91 = new Vector2(movementLimitX, movementLimitY);
				float num736 = player.Center.X - vector91.X;
				float num737 = player.Center.Y - vector91.Y;

				if (despawnBoost)
				{
					num737 *= -1f;
					num736 *= -1f;
					velocity += 8f;
				}

				float num738 = (float)Math.Sqrt(num736 * num736 + num737 * num737);
				float maxDistanceFromHooks = expertMode ? 650f : 500f;
				if (speedBoost || malice)
					maxDistanceFromHooks += 250f;
				if (death)
					maxDistanceFromHooks += maxDistanceFromHooks * 0.1f * (1f - lifeRatio);

				// Increase speed based on nearby active tiles
				velocity *= tileEnrageMult;
				acceleration *= tileEnrageMult;

				if (death)
				{
					velocity += velocity * 0.15f * (1f - lifeRatio);
					acceleration += acceleration * 0.15f * (1f - lifeRatio);
				}

				if (num738 >= maxDistanceFromHooks)
				{
					num738 = maxDistanceFromHooks / num738;
					num736 *= num738;
					num737 *= num738;
				}

				movementLimitX += num736;
				movementLimitY += num737;
				vector91 = vector;
				num736 = movementLimitX - vector91.X;
				num737 = movementLimitY - vector91.Y;
				num738 = (float)Math.Sqrt(num736 * num736 + num737 * num737);

				if (num738 < velocity)
				{
					num736 = npc.velocity.X;
					num737 = npc.velocity.Y;
				}
				else
				{
					num738 = velocity / num738;
					num736 *= num738;
					num737 *= num738;
				}

				if (npc.velocity.X < num736)
				{
					npc.velocity.X += acceleration;
					if (npc.velocity.X < 0f && num736 > 0f)
						npc.velocity.X += acceleration * 2f;
				}
				else if (npc.velocity.X > num736)
				{
					npc.velocity.X -= acceleration;
					if (npc.velocity.X > 0f && num736 < 0f)
						npc.velocity.X -= acceleration * 2f;
				}
				if (npc.velocity.Y < num737)
				{
					npc.velocity.Y += acceleration;
					if (npc.velocity.Y < 0f && num737 > 0f)
						npc.velocity.Y += acceleration * 2f;
				}
				else if (npc.velocity.Y > num737)
				{
					npc.velocity.Y -= acceleration;
					if (npc.velocity.Y > 0f && num737 < 0f)
						npc.velocity.Y -= acceleration * 2f;
				}
			}
			else
			{
				// Charge
				if (npc.Calamity().newAI[3] == 1f)
				{
					reachedChargingPoint = false;

					npc.Opacity += 0.06f;
					if (npc.Opacity > 0.8f)
						npc.Opacity = 0.8f;

					if (npc.Calamity().newAI[1] == 0f)
					{
						npc.velocity = Vector2.Normalize(rotationVector) * chargeVelocity;
						npc.Calamity().newAI[1] = 1f;
					}
					else
					{
						npc.Calamity().newAI[2] += 1f;

						// Slow down for a few frames
						float totalChargeTime = chargeDistance * 4f / chargeVelocity;
						float slowDownTime = chargeVelocity;
						if (npc.Calamity().newAI[2] >= totalChargeTime - slowDownTime)
							npc.velocity *= 0.9f;

						// Reset and either go back to normal or charge again
						if (npc.Calamity().newAI[2] >= totalChargeTime)
						{
							npc.Calamity().newAI[1] = 0f;
							npc.Calamity().newAI[2] = 0f;
							npc.Calamity().newAI[3] = 0f;
							npc.ai[0] = 0f;
							npc.ai[1] += 1f;

							if (npc.ai[1] >= 3f)
							{
								// Reset and return to normal movement
								npc.Calamity().newAI[0] = 0f;
								npc.ai[1] = 0f;
							}
						}
					}
				}
				else
				{
					// Random location choice
					if (npc.ai[0] == 0f)
					{
						npc.velocity = Vector2.Zero;
						npc.ai[0] = Main.rand.Next(2) + 1;
						npc.netUpdate = true;
					}

					// Pick a charging location
					// Set charge locations X
					if (Main.npc[CalamityGlobalNPC.ghostBoss].Center.X >= player.Center.X)
						npc.Calamity().newAI[1] = npc.ai[0] == 1f ? player.Center.X - chargeDistance : Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[1];
					else
						npc.Calamity().newAI[1] = npc.ai[0] == 1f ? player.Center.X + chargeDistance : Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[1];

					// Set charge locations Y
					if (Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y >= player.Center.Y)
						npc.Calamity().newAI[2] = npc.ai[0] == 2f ? player.Center.Y - chargeDistance : Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[2];
					else
						npc.Calamity().newAI[2] = npc.ai[0] == 2f ? player.Center.Y + chargeDistance : Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[2];

					// Do not deal damage during movement to avoid cheap bullshit hits
					npc.damage = 0;

					// Charge location
					Vector2 chargeVector = new Vector2(npc.Calamity().newAI[1], npc.Calamity().newAI[2]);
					Vector2 chargeLocationVelocity = Vector2.Normalize(chargeVector - vector) * chargeVelocity;

					// Line up a charge
					float chargeDistanceGateValue = 32f;

					if (Vector2.Distance(vector, chargeVector) <= chargeDistanceGateValue * 3f)
					{
						npc.Opacity += 0.06f;
						if (npc.Opacity > 0.8f)
							npc.Opacity = 0.8f;
					}
					else
					{
						npc.Opacity -= 0.06f;
						if (npc.Opacity < 0f)
							npc.Opacity = 0f;
					}

					if (Vector2.Distance(vector, chargeVector) <= chargeDistanceGateValue || reachedChargingPoint)
					{
						// Emit dust
						if (!reachedChargingPoint)
						{
							Main.PlaySound(SoundID.Item125, npc.position);
							for (int i = 0; i < 30; i++)
							{
								int dust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 3f);
								Main.dust[dust].noGravity = true;
								Main.dust[dust].velocity *= 5f;
							}
						}

						reachedChargingPoint = true;
						npc.velocity = Vector2.Zero;
						npc.Center = chargeVector;
					}
					else
					{
						// Reduce velocity and acceleration to allow for smoother movement inside this loop
						if (Vector2.Distance(vector, chargeVector) > 1200f)
							npc.velocity = chargeLocationVelocity;
						else
							npc.SimpleFlyMovement(chargeLocationVelocity, chargeAcceleration);
					}
				}

				npc.netUpdate = true;

				if (npc.netSpam > 10)
					npc.netSpam = 10;

				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
			}
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(200, 150, 255) * npc.Opacity;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
			Color color36 = Color.White;
			Color lightRed = new Color(255, 100, 100, 255) * npc.Opacity;
			float amount9 = 0.5f;
			int num153 = 7;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;

					if (Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] > 300f)
						color38 = Color.Lerp(color38, lightRed, MathHelper.Clamp((Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] - 300f) / 120f, 0f, 1f));

					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Color color = npc.GetAlpha(lightColor);

			if (Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] > 300f)
				color = Color.Lerp(color, lightRed, MathHelper.Clamp((Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] - 300f) / 120f, 0f, 1f));

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, color, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			Texture2D texture2D16 = ModContent.GetTexture("CalamityMod/NPCs/Polterghast/PolterPhantomGlow");
			Color color42 = Color.Lerp(Color.White, (Main.npc[CalamityGlobalNPC.ghostBoss].ai[2] < 300f || Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] > 300f) ? Color.Red : Color.Black, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Vector2 vector44 = npc.oldPos[num163] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					Color color43 = color42;
					color43 = Color.Lerp(color43, color36, amount9);
					color43 = npc.GetAlpha(color43);
					color43 *= (num153 - num163) / 15f;
					spriteBatch.Draw(texture2D16, vector44, npc.frame, color43, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture2D16, vector43, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 1.0;
            if (npc.frameCounter > 6.0)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = npc.frame.Y + frameHeight;
            }
            if (npc.frame.Y > frameHeight * 3)
            {
                npc.frame.Y = 0;
            }
        }

		public override bool CheckActive()
		{
			return false;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
        {
			player.AddBuff(BuffID.MoonLeech, 360, true);
		}

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(npc.position, npc.width, npc.height, 180, hitDirection, -1f, 0, default, 1f);
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 90;
                npc.height = 90;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
