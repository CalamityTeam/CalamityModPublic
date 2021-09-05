using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
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
    public class ThanatosBody1 : ModNPC
    {
		// Whether the body is venting heat or not, it is vulnerable to damage during venting
		private bool vulnerable = false;
		public ThanatosSmokeParticleSet SmokeDrawer = new ThanatosSmokeParticleSet(-1, 3, 0f, 16f, 1.5f);

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
			npc.width = 88;
            npc.height = 88;
            npc.defense = 100;
			npc.DR_NERD(0.9999f);
			npc.Calamity().unbreakableDR = true;
			npc.LifeMaxNERB(1000000, 1150000, 500000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
			npc.Opacity = 0f;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            npc.dontCountMe = true;
            npc.chaseable = false;
			npc.boss = true;
			music = /*CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm") ??*/ MusicID.Boss3;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.chaseable);
			writer.Write(npc.dontTakeDamage);
			writer.Write(vulnerable);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
			writer.Write(npc.localAI[2]);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.chaseable = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
			vulnerable = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
			npc.localAI[2] = reader.ReadSingle();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void AI()
        {
            if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

			if (npc.life > Main.npc[(int)npc.ai[1]].life)
				npc.life = Main.npc[(int)npc.ai[1]].life;

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

			// Check if other segments are still alive, if not, die
			bool shouldDespawn = true;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<ThanatosHead>())
				{
					shouldDespawn = false;
					break;
				}
			}
			if (!shouldDespawn)
			{
				if (npc.ai[1] <= 0f)
					shouldDespawn = true;
				else if (Main.npc[(int)npc.ai[1]].life <= 0)
					shouldDespawn = true;
			}
			if (shouldDespawn)
			{
				npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.checkDead();
				npc.active = false;
			}

			// Set vulnerable to false by default
			vulnerable = false;

			CalamityGlobalNPC calamityGlobalNPC_Head = Main.npc[(int)npc.ai[2]].Calamity();

			bool invisiblePhase = calamityGlobalNPC_Head.newAI[1] == (float)ThanatosHead.SecondaryPhase.PassiveAndImmune;
			npc.dontTakeDamage = invisiblePhase;
			if (!invisiblePhase)
			{
				if (Main.npc[(int)npc.ai[1]].Opacity > 0.5f)
				{
					npc.Opacity += 0.2f;
					if (npc.Opacity > 1f)
						npc.Opacity = 1f;
				}
			}
			else
			{
				npc.Opacity -= 0.05f;
				if (npc.Opacity < 0f)
					npc.Opacity = 0f;
			}

			// Number of body segments
			int numSegments = ThanatosHead.minLength;

			// Set timer to whoAmI so that segments don't all fire lasers at the same time
			if (npc.localAI[2] == 0f)
			{
				npc.localAI[2] = npc.ai[0];
				if (npc.localAI[2] > numSegments)
					npc.localAI[2] -= numSegments;
			}

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Check if the other exo mechs are alive
			int otherExoMechsAlive = 0;
			if (CalamityGlobalNPC.draedonExoMechPrime != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
					otherExoMechsAlive++;
			}
			if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
					otherExoMechsAlive++;
			}

			// Set the AI to become more aggressive if head is berserk
			bool berserk = lifeRatio < 0.4f || (otherExoMechsAlive == 0 && lifeRatio < 0.7f);

			bool shootLasers = (calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.Charge || calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.UndergroundLaserBarrage || berserk) && calamityGlobalNPC_Head.newAI[2] > 0f;
			if (shootLasers && !invisiblePhase)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					// Only charge up lasers if not venting or firing lasers
					if (npc.Calamity().newAI[0] == 0f)
						npc.ai[3] += 1f;

					double numSegmentsAbleToFire = malice ? 30D : death ? 20D : revenge ? 17.5 : expertMode ? 15D : 10D;
					if (calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.Charge && !berserk)
					{
						float divisor = 120f;
						float segmentDivisor = (float)Math.Round(numSegments / numSegmentsAbleToFire);
						if ((npc.ai[3] % divisor == 0f && npc.localAI[2] % segmentDivisor == 0f) || npc.Calamity().newAI[0] > 0f)
						{
							// Body is vulnerable while firing lasers
							vulnerable = true;

							if (npc.Calamity().newAI[1] == 0f)
							{
								npc.Calamity().newAI[0] += 1f;
								if (npc.Calamity().newAI[0] >= 72f)
								{
									npc.ai[3] = 0f;
									npc.Calamity().newAI[1] = 1f;
									if (Main.netMode != NetmodeID.MultiplayerClient)
									{
										int maxTargets = 3;
										int[] whoAmIArray = new int[maxTargets];
										Vector2[] targetCenterArray = new Vector2[maxTargets];
										int numProjectiles = 0;
										float maxDistance = 2400f;

										for (int i = 0; i < Main.maxPlayers; i++)
										{
											if (!Main.player[i].active || Main.player[i].dead)
												continue;

											Vector2 playerCenter = Main.player[i].Center;
											float distance = Vector2.Distance(playerCenter, npc.Center);
											if (distance < maxDistance)
											{
												whoAmIArray[numProjectiles] = i;
												targetCenterArray[numProjectiles] = playerCenter;
												int projectileLimit = numProjectiles + 1;
												numProjectiles = projectileLimit;
												if (projectileLimit >= targetCenterArray.Length)
													break;
											}
										}

										Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon").WithVolume(0.1f), npc.Center);
										for (int i = 0; i < numProjectiles; i++)
										{
											// Normal laser
											int type = ModContent.ProjectileType<ExoDestroyerLaser>();
											int damage = npc.GetProjectileDamage(type);
											Projectile.NewProjectile(npc.Center, targetCenterArray[i], type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
										}
									}
								}
							}
							else
							{
								npc.Calamity().newAI[0] -= 1f;
								if (npc.Calamity().newAI[0] <= 0f)
								{
									npc.Calamity().newAI[0] = 0f;
									npc.Calamity().newAI[1] = 0f;
								}
							}
						}
					}
					else
					{
						// This is only used in deathray phase to prevent laser spam
						float segmentDivisor = (float)Math.Round(numSegments / (berserk ? 5D : numSegmentsAbleToFire));
						float divisor = npc.localAI[2] * 3f; // Ranges from 3 to 300
						if ((npc.ai[3] == divisor && npc.localAI[2] % segmentDivisor == 0f) || npc.Calamity().newAI[0] > 0f)
						{
							// Body is vulnerable while firing lasers
							vulnerable = true;

							if (npc.Calamity().newAI[1] == 0f)
							{
								npc.Calamity().newAI[0] += 1f;
								if (npc.Calamity().newAI[0] >= 72f)
								{
									npc.ai[3] = 0f;
									npc.Calamity().newAI[1] = 1f;
									if (Main.netMode != NetmodeID.MultiplayerClient)
									{
										int maxTargets = 3;
										int[] whoAmIArray = new int[maxTargets];
										Vector2[] targetCenterArray = new Vector2[maxTargets];
										int numProjectiles = 0;
										float maxDistance = 2400f;

										for (int i = 0; i < Main.maxPlayers; i++)
										{
											if (!Main.player[i].active || Main.player[i].dead)
												continue;

											Vector2 playerCenter = Main.player[i].Center;
											float distance = Vector2.Distance(playerCenter, npc.Center);
											if (distance < maxDistance)
											{
												whoAmIArray[numProjectiles] = i;
												targetCenterArray[numProjectiles] = playerCenter;
												int projectileLimit = numProjectiles + 1;
												numProjectiles = projectileLimit;
												if (projectileLimit >= targetCenterArray.Length)
													break;
											}
										}

										float predictionAmt = malice ? 30f : death ? 25f : revenge ? 20f : expertMode ? 15f : 5f;
										int type = ModContent.ProjectileType<ExoDestroyerLaser>();
										int damage = npc.GetProjectileDamage(type);
										Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), npc.Center);
										for (int i = 0; i < numProjectiles; i++)
										{
											// Fire normal lasers if head is in passive state
											if (calamityGlobalNPC_Head.newAI[1] == (float)ThanatosHead.SecondaryPhase.Passive)
											{
												// Normal laser
												Projectile.NewProjectile(npc.Center, targetCenterArray[i], type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
											}
											else
											{
												// Normal laser
												if (malice && berserk)
													Projectile.NewProjectile(npc.Center, targetCenterArray[i], type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);

												// Predictive laser
												Vector2 projectileDestination = targetCenterArray[i] + Main.player[whoAmIArray[i]].velocity * predictionAmt;
												Projectile.NewProjectile(npc.Center, projectileDestination, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);

												// Opposite laser
												projectileDestination = targetCenterArray[i] - Main.player[whoAmIArray[i]].velocity * predictionAmt;
												Projectile.NewProjectile(npc.Center, projectileDestination, type, damage, 0f, Main.myPlayer, 0f, npc.whoAmI);
											}
										}
									}
								}
							}
							else
							{
								npc.Calamity().newAI[0] -= 1f;
								if (npc.Calamity().newAI[0] <= 0f)
								{
									npc.Calamity().newAI[0] = 0f;
									npc.Calamity().newAI[1] = 0f;
								}
							}
						}
					}
				}
			}
			else
			{
				if (npc.ai[3] > 0f)
					npc.ai[3] = 0f;

				// Set alternating laser-firing body segments every 3 seconds
				npc.localAI[1] += 1f;
				if (npc.localAI[1] >= 180f)
				{
					npc.localAI[1] = 0f;
					npc.localAI[2] += 1f;
					if (npc.localAI[2] > numSegments)
						npc.localAI[2] -= numSegments;
				}

				npc.Calamity().newAI[0] -= 1f;
				if (npc.Calamity().newAI[0] <= 0f)
				{
					npc.Calamity().newAI[0] = 0f;
					npc.Calamity().newAI[1] = 0f;
				}
				else
				{
					// Body is vulnerable while venting
					vulnerable = true;
				}
			}

			if (npc.Calamity().newAI[2] < ThanatosHead.immunityTime)
				npc.Calamity().newAI[2] += 1f;

			// Homing only works if vulnerable is true
			npc.chaseable = vulnerable;

			// Adjust DR based on vulnerable
			npc.Calamity().DR = vulnerable ? 0f : 0.9999f;
			npc.Calamity().unbreakableDR = !vulnerable;

			// Vent noise and steam
			SmokeDrawer.ParticleSpawnRate = 9999999;
			if (vulnerable)
			{
				// Light
				Lighting.AddLight(npc.Center, 0.35f, 0.05f, 0.05f);

				// Noise
				float volume = calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.Charge ? 0.1f : 1f;
				if (npc.localAI[0] == 0f)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ThanatosVent").WithVolume(volume), npc.Center);

				// Steam
				npc.localAI[0] += 1f;
				if (npc.localAI[0] < ThanatosHead.ventDuration)
				{
					SmokeDrawer.BaseMoveRotation = npc.rotation - MathHelper.PiOver2;
					SmokeDrawer.ParticleSpawnRate = ThanatosHead.ventCloudSpawnRate;
				}
			}
			else
			{
				// Light
				Lighting.AddLight(npc.Center, 0.05f, 0.2f, 0.2f);

				npc.localAI[0] = 0f;
			}

			SmokeDrawer.Update();

			Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float num191 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2);
            float num192 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2);
            num191 = (int)(num191 / 16f) * 16;
            num192 = (int)(num192 / 16f) * 16;
            vector18.X = (int)(vector18.X / 16f) * 16;
            vector18.Y = (int)(vector18.Y / 16f) * 16;
            num191 -= vector18.X;
            num192 -= vector18.Y;

            float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
            if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    num191 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
                } catch
                {
                }

                npc.rotation = (float)Math.Atan2(num192, num191) + MathHelper.PiOver2;
                num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
                int num194 = npc.width;
                num193 = (num193 - num194) / num193;
                num191 *= num193;
                num192 *= num193;
                npc.velocity = Vector2.Zero;
                npc.position.X = npc.position.X + num191;
                npc.position.Y = npc.position.Y + num192;

                if (num191 < 0f)
                    npc.spriteDirection = -1;
                else if (num191 > 0f)
                    npc.spriteDirection = 1;
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

			return minDist <= 50f && npc.Opacity == 1f;
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			if (npc.Calamity().newAI[2] < ThanatosHead.immunityTime)
				damage *= 0.01;

			return !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);
		}

		public override void FindFrame(int frameHeight) // 5 total frames
		{
			// Swap between venting and non-venting frames
			CalamityGlobalNPC calamityGlobalNPC_Head = Main.npc[(int)npc.ai[2]].Calamity();
			bool invisiblePhase = calamityGlobalNPC_Head.newAI[1] == (float)ThanatosHead.SecondaryPhase.PassiveAndImmune;
			bool shootLasers = (calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.Charge || calamityGlobalNPC_Head.newAI[0] == (float)ThanatosHead.Phase.UndergroundLaserBarrage) && calamityGlobalNPC_Head.newAI[2] > 0f;
			npc.frameCounter += 1D;
			if (shootLasers && !invisiblePhase)
			{
				if (npc.Calamity().newAI[1] == 0f && npc.Calamity().newAI[0] > 0f)
				{
					if (npc.frameCounter >= 10D)
					{
						npc.frame.Y += frameHeight;
						npc.frameCounter = 0D;
					}
					int finalFrame = Main.npcFrameCount[npc.type] - 1;
					if (npc.frame.Y > frameHeight * finalFrame)
						npc.frame.Y = frameHeight * finalFrame;
				}
				else
				{
					if (npc.frameCounter >= 10D)
					{
						npc.frame.Y -= frameHeight;
						npc.frameCounter = 0D;
					}
					if (npc.frame.Y < 0)
						npc.frame.Y = 0;
				}
			}
			else
			{
				if (npc.frameCounter >= 10D)
				{
					npc.frame.Y -= frameHeight;
					npc.frameCounter = 0D;
				}
				if (npc.frame.Y < 0)
					npc.frame.Y = 0;
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

			texture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosBody1Glow");
			spriteBatch.Draw(texture, center, npc.frame, Color.White * npc.Opacity, npc.rotation, vector, npc.scale, spriteEffects, 0f);

			SmokeDrawer.DrawSet(npc.Center);

			return false;
		}

		public override bool CheckActive() => false;

		public override bool PreNPCLoot() => false;

		public override void HitEffect(int hitDirection, double damage)
		{
			int baseDust = vulnerable ? 3 : 1;
			for (int k = 0; k < baseDust; k++)
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

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosBody1"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosBody1_2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosBody1_3"), 1f);
			}
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
		}
	}
}
