using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public class CalamityAI
    {
		#region Aquatic Scourge
		public static void AquaticScourgeAI(NPC npc, Mod mod, bool head)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			// Adjust hostility and stats
			if (npc.justHit || npc.life <= npc.lifeMax * 0.999 || BossRushEvent.BossRushActive)
			{
				if (calamityGlobalNPC.newAI[0] == 0f)
				{
					// Kiss my motherfucking ass you piece of shit game
					npc.timeLeft *= 20;
					CalamityMod.bossKillTimes.TryGetValue(npc.type, out int revKillTime);
					calamityGlobalNPC.KillTime = revKillTime;
					calamityGlobalNPC.newAI[0] = 1f;
					npc.boss = head;
					npc.chaseable = true;
					npc.netUpdate = true;
				}
				npc.damage = npc.defDamage;
			}
			else
				npc.damage = 0;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Phases
			bool phase2 = lifeRatio < 0.75f;
			bool phase3 = lifeRatio < 0.5f;
			bool phase4 = lifeRatio < 0.25f;

			// Set worm variable
			if (npc.ai[2] > 0f)
				npc.realLife = (int)npc.ai[2];

			if (!head)
			{
				if (npc.life > Main.npc[(int)npc.ai[1]].life)
					npc.life = Main.npc[(int)npc.ai[1]].life;
			}

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			bool notOcean = player.position.Y < 400f ||
				player.position.Y > Main.worldSurface * 16.0 ||
				(player.position.X > 7680f && player.position.X < (Main.maxTilesX * 16 - 7680));

			// Enrage
			if (head)
			{
				if (notOcean && !player.Calamity().ZoneSulphur && !BossRushEvent.BossRushActive)
				{
					if (npc.localAI[2] > 0f)
						npc.localAI[2] -= 1f;
				}
				else
					npc.localAI[2] = CalamityGlobalNPC.biomeEnrageTimerMax;
			}

			bool biomeEnraged = npc.localAI[2] <= 0f || malice;

			float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
			if (biomeEnraged)
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 2f;
			}

			Vector2 vectorCenter = npc.Center;

			// Circular movement
			bool doSpiral = false;
			if (head && calamityGlobalNPC.newAI[0] == 1f && calamityGlobalNPC.newAI[2] == 1f && revenge)
			{
				bool doNotSpiral = !Collision.CanHit(npc.Center, 1, 1, player.position, player.width, player.height);
				float ai3 = 660f;
				calamityGlobalNPC.newAI[3] += 1f;
				doSpiral = calamityGlobalNPC.newAI[1] == 0f && calamityGlobalNPC.newAI[3] >= ai3;
				if (doSpiral)
				{
					// Barf
					npc.localAI[3] = 90f;
					float barfDivisor = malice ? 30f : expertMode ? 40f : 60f;
					if (calamityGlobalNPC.newAI[3] % barfDivisor == 0f)
					{
						Main.PlaySound(SoundID.NPCKilled, (int)npc.position.X, (int)npc.position.Y, 13);

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							if (phase3)
							{
								// Circle of toxic clouds
								int type = ModContent.ProjectileType<ToxicCloud>();
								int damage = npc.GetProjectileDamage(type);
								int totalProjectiles = (phase4 ? 6 : 9) + (int)((calamityGlobalNPC.newAI[3] - ai3) / barfDivisor) * (phase4 ? 2 : 3);
								float radians = MathHelper.TwoPi / totalProjectiles;
								float cloudVelocity = 1f + enrageScale;
								Vector2 spinningPoint = new Vector2(0f, -cloudVelocity);
								for (int k = 0; k < totalProjectiles; k++)
								{
									Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
									Projectile.NewProjectile(vectorCenter, vector255, type, damage, 0f, Main.myPlayer);
								}
							}
							else
							{
								// Spread of directed sand blasts
								float num742 = death ? 10f : 8f;
								float num743 = player.Center.X - vectorCenter.X;
								float num744 = player.Center.Y - vectorCenter.Y;
								float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

								num745 = num742 / num745;
								num743 *= num745;
								num744 *= num745;

								int type = ModContent.ProjectileType<SandBlast>();
								int damage = npc.GetProjectileDamage(type);
								int numProj = death ? 5 : 3;
								int spread = death ? 60 : 36;
								float rotation = MathHelper.ToRadians(spread);
								float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
								double startAngle = Math.Atan2(num743, num744) - rotation / 2;
								double deltaAngle = rotation / numProj;
								double offsetAngle;

								for (int i = 0; i < numProj; i++)
								{
									offsetAngle = startAngle + deltaAngle * i;
									int proj = Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
									Main.projectile[proj].tileCollide = false;
								}
							}
						}
					}

					// Velocity boost
					if (calamityGlobalNPC.newAI[3] == ai3)
					{
						npc.velocity.Normalize();
						npc.velocity *= 24f;
					}

					// Spin velocity
					float velocity = (float)(Math.PI * 2D) / 120f;
					npc.velocity = npc.velocity.RotatedBy(-(double)velocity * npc.localAI[1]);
					npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;

					// Reset and charge at target
					if (calamityGlobalNPC.newAI[3] >= ai3 + 120f)
					{
						calamityGlobalNPC.newAI[3] = 0f;
						float chargeVelocity = (death ? 16f : 12f) + 3f * enrageScale;
						npc.velocity = Vector2.Normalize(player.Center - npc.Center) * chargeVelocity;
						npc.TargetClosest();
					}
				}
				else
				{
					if (doNotSpiral && calamityGlobalNPC.newAI[3] > 0f)
						calamityGlobalNPC.newAI[3] -= 2f;

					if (npc.localAI[3] > 0f)
						npc.localAI[3] -= 1f;

					npc.localAI[1] = npc.Center.X - player.Center.X < 0 ? 1f : -1f;
				}
			}

			// Adjust slowing debuff immunity
			bool immuneToSlowingDebuffs = doSpiral;
			npc.buffImmune[ModContent.BuffType<ExoFreeze>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<KamiDebuff>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<TimeSlow>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<TeslaFreeze>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
			npc.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
			npc.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (head)
				{
					// Spawn segments
					if (calamityGlobalNPC.newAI[2] == 0f && npc.ai[0] == 0f)
					{
						int maxLength = death ? 80 : revenge ? 40 : expertMode ? 35 : 30;
						int Previous = npc.whoAmI;
						for (int num36 = 0; num36 < maxLength; num36++)
						{
							int lol;
							if (num36 >= 0 && num36 < maxLength - 1)
							{
								if (num36 % 2 == 0)
									lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AquaticScourgeBodyAlt>(), npc.whoAmI);
								else
									lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AquaticScourgeBody>(), npc.whoAmI);
							}
							else
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<AquaticScourgeTail>(), npc.whoAmI);

							Main.npc[lol].realLife = npc.whoAmI;
							Main.npc[lol].ai[2] = npc.whoAmI;
							Main.npc[lol].ai[1] = Previous;
							Main.npc[Previous].ai[0] = lol;
							NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
							Previous = lol;
						}
						calamityGlobalNPC.newAI[2] = 1f;
					}

					// Big barf attack
					if (calamityGlobalNPC.newAI[0] == 1f && !doSpiral && phase2)
					{
						npc.localAI[0] += 1f;
						if (npc.localAI[0] >= (revenge ? 360f : 420f))
						{
							if (Vector2.Distance(player.Center, npc.Center) > 320f)
							{
								npc.localAI[0] = 0f;
								npc.netUpdate = true;
								Main.PlaySound(SoundID.NPCKilled, (int)npc.position.X, (int)npc.position.Y, 13);

								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									float velocity = death ? 9.5f : revenge ? 7.5f : 6.5f;
									int totalProjectiles = malice ? 40 : expertMode ? 30 : 20;
									float radians = MathHelper.TwoPi / totalProjectiles;
									int type = ModContent.ProjectileType<SandBlast>();
									int damage = npc.GetProjectileDamage(type);
									float homing = phase4 ? 1f : 0f;
									for (int i = 0; i < totalProjectiles; i++)
									{
										Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * i);
										Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer, homing, 0f);
									}

									if (phase3)
									{
										int num320 = malice ? 21 : expertMode ? 14 : 7;
										type = ModContent.ProjectileType<SandPoisonCloud>();
										damage = npc.GetProjectileDamage(type);
										for (int num321 = 0; num321 < num320; num321++)
										{
											Vector2 vector15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
											vector15.Normalize();
											vector15 *= Main.rand.Next(death ? 200 : 50, 401) * 0.01f;

											float maximumVelocityMult = death ? 0.35f : 0.25f;
											if (expertMode)
												vector15 *= 1f + (maximumVelocityMult * (0.5f - lifeRatio) * 2f);

											Projectile.NewProjectile(npc.Center, vector15, type, damage, 0f, Main.myPlayer, 0f, Main.rand.Next(-45, 1));
										}
									}
								}
							}
						}
					}
				}

				// Fire sand blasts or teeth depending on body type
				else
				{
					if (calamityGlobalNPC.newAI[0] == 1f && (!phase3 || phase4))
					{
						npc.localAI[0] += 1f;
						float shootProjectile = 300;
						float timer = npc.ai[0] + 15f;
						float divisor = timer + shootProjectile;

						if (npc.type == ModContent.NPCType<AquaticScourgeBody>())
						{
							if (npc.localAI[0] % divisor == 0f && (npc.ai[0] % 3f == 0f || !death))
							{
								npc.TargetClosest();
								if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
								{
									Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 17);
									Vector2 vector104 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + (npc.height / 2));
									float num942 = player.position.X + player.width * 0.5f - vector104.X;
									float num943 = player.position.Y + player.height * 0.5f - vector104.Y;
									float num944 = (float)Math.Sqrt(num942 * num942 + num943 * num943);
									int type = ModContent.ProjectileType<SandTooth>();
									int damage = npc.GetProjectileDamage(type);
									float num941 = death ? 9f : 8f;
									num944 = num941 / num944;
									num942 *= num944;
									num943 *= num944;
									vector104.X += num942 * 5f;
									vector104.Y += num943 * 5f;
									float accelerate = phase4 ? 1f : 0f;
									Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, type, damage, 0f, Main.myPlayer, accelerate, 0f);
									npc.netUpdate = true;
								}
							}
						}
						else if (npc.type == ModContent.NPCType<AquaticScourgeBodyAlt>())
						{
							if (npc.localAI[0] % divisor == 0f && (npc.ai[0] % 3f == 0f || !death))
							{
								npc.TargetClosest();
								if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
								{
									Vector2 vector104 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + (npc.height / 2));
									float num942 = player.position.X + player.width * 0.5f - vector104.X;
									float num943 = player.position.Y + player.height * 0.5f - vector104.Y;
									float num944 = (float)Math.Sqrt(num942 * num942 + num943 * num943);
									int type = ModContent.ProjectileType<SandBlast>();
									int damage = npc.GetProjectileDamage(type);
									float num941 = death ? 15f : 12f;
									num944 = num941 / num944;
									num942 *= num944;
									num943 *= num944;
									vector104.X += num942 * 5f;
									vector104.Y += num943 * 5f;
									float homing = phase4 ? 1f : 0f;
									Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, type, damage, 0f, Main.myPlayer, homing, 0f);
									npc.netUpdate = true;
								}
							}
						}
					}
				}
			}

			// Kill body and tail
			if (!head)
			{
				bool shouldDespawn = true;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<AquaticScourgeHead>())
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
			}
			else
			{
				if (npc.life > Main.npc[(int)npc.ai[0]].life)
					npc.life = Main.npc[(int)npc.ai[0]].life;
			}

			float maxDistance = calamityGlobalNPC.newAI[0] == 1f ? 12800f : 6400f;
			if (player.dead || Vector2.Distance(npc.Center, player.Center) > maxDistance)
			{
				calamityGlobalNPC.newAI[1] = 1f;
				npc.TargetClosest(false);
				npc.velocity.Y += 2f;

				if (npc.position.Y > Main.worldSurface * 16.0)
					npc.velocity.Y += 2f;

				if (npc.position.Y > Main.worldSurface * 16.0)
				{
					for (int a = 0; a < Main.npc.Length; a++)
					{
						int type = Main.npc[a].type;
						if (type == ModContent.NPCType<AquaticScourgeHead>() || type == ModContent.NPCType<AquaticScourgeBody>() ||
							type == ModContent.NPCType<AquaticScourgeBodyAlt>() || type == ModContent.NPCType<AquaticScourgeTail>())
						{
							Main.npc[a].active = false;
						}
					}
				}
			}
			else
				calamityGlobalNPC.newAI[1] = 0f;

			// Change direction
			if (npc.velocity.X < 0f)
				npc.spriteDirection = -1;
			else if (npc.velocity.X > 0f)
				npc.spriteDirection = 1;

			// Alpha changes
			if (head || Main.npc[(int)npc.ai[1]].alpha < 128)
			{
				npc.alpha -= 42;
				if (npc.alpha < 0)
					npc.alpha = 0;
			}

			// Velocity and movement
			float num188 = 5f;
			float num189 = 0.08f;
			Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
			float num191 = player.position.X + (player.width / 2);
			float num192 = player.position.Y + (player.height / 2);

			if (head && !doSpiral)
			{
				if (calamityGlobalNPC.newAI[0] != 1f)
				{
					num192 += 400;
					if (Math.Abs(npc.Center.X - player.Center.X) < 500f)
					{
						if (npc.velocity.X > 0f)
							num191 = player.Center.X + 600f;
						else
							num191 = player.Center.X - 600f;
					}
				}

				if (calamityGlobalNPC.newAI[0] == 1f)
				{
					num188 = revenge ? 9f : 7f;
					num189 = revenge ? 0.12f : 0.1f;
					if (expertMode)
					{
						num188 += 8f * (1f - lifeRatio);
						num189 += 0.06f * (1f - lifeRatio);
					}
					num188 += 3f * enrageScale;
					num189 += 0.06f * enrageScale;
					if (death)
					{
						num188 += 5f;
						num189 -= 0.03f;
						num188 += Vector2.Distance(player.Center, npc.Center) * 0.0015f;
						num189 += Vector2.Distance(player.Center, npc.Center) * 0.00003f;
					}
				}

				float num48 = num188 * 1.3f;
				float num49 = num188 * 0.7f;
				float num50 = npc.velocity.Length();
				if (num50 > 0f)
				{
					if (num50 > num48)
					{
						npc.velocity.Normalize();
						npc.velocity *= num48;
					}
					else if (num50 < num49)
					{
						npc.velocity.Normalize();
						npc.velocity *= num49;
					}
				}
			}

			num191 = (int)(num191 / 16f) * 16;
			num192 = (int)(num192 / 16f) * 16;
			vector18.X = (int)(vector18.X / 16f) * 16;
			vector18.Y = (int)(vector18.Y / 16f) * 16;
			num191 -= vector18.X;
			num192 -= vector18.Y;
			float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);

			if (!head)
			{
				if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
				{
					try
					{
						vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
						num191 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
						num192 = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
					}
					catch
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
					{
						npc.spriteDirection = -1;
					}
					else if (num191 > 0f)
					{
						npc.spriteDirection = 1;
					}
				}
			}
			else if (!doSpiral)
			{
				float num196 = Math.Abs(num191);
				float num197 = Math.Abs(num192);
				float num198 = num188 / num193;
				num191 *= num198;
				num192 *= num198;

				if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
				{
					if (npc.velocity.X < num191)
					{
						npc.velocity.X += num189;
					}
					else
					{
						if (npc.velocity.X > num191)
							npc.velocity.X -= num189;
					}
					if (npc.velocity.Y < num192)
					{
						npc.velocity.Y += num189;
					}
					else
					{
						if (npc.velocity.Y > num192)
							npc.velocity.Y -= num189;
					}
					if (Math.Abs(num192) < num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y += num189 * 2f;
						else
							npc.velocity.Y -= num189 * 2f;
					}
					if (Math.Abs(num191) < num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X += num189 * 2f;
						else
							npc.velocity.X -= num189 * 2f;
					}
				}
				else
				{
					if (num196 > num197)
					{
						if (npc.velocity.X < num191)
							npc.velocity.X += num189 * 1.1f;
						else if (npc.velocity.X > num191)
							npc.velocity.X -= num189 * 1.1f;

						if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
						{
							if (npc.velocity.Y > 0f)
								npc.velocity.Y += num189;
							else
								npc.velocity.Y -= num189;
						}
					}
					else
					{
						if (npc.velocity.Y < num192)
							npc.velocity.Y += num189 * 1.1f;
						else if (npc.velocity.Y > num192)
							npc.velocity.Y -= num189 * 1.1f;

						if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
						{
							if (npc.velocity.X > 0f)
								npc.velocity.X += num189;
							else
								npc.velocity.X -= num189;
						}
					}
				}
				npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;
			}
		}
		#endregion

		#region Brimstone Elemental
		public static void BrimstoneElementalAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Used for Brimling AI states
			CalamityGlobalNPC.brimstoneElemental = npc.whoAmI;

			// Emit light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 1.2f, 0f, 0f);

			// Center
			Vector2 vectorCenter = npc.Center;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			bool despawnDistance = Vector2.Distance(player.Center, vectorCenter) > CalamityGlobalNPC.CatchUpDistance350Tiles;

			if (!player.active || player.dead || despawnDistance)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || despawnDistance)
				{
					npc.rotation = npc.velocity.X * 0.04f;

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.ai[0] != 0f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.localAI[0] = 0f;
						npc.localAI[1] = 0f;
						npc.netUpdate = true;
					}
					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			CalamityPlayer modPlayer = player.Calamity();

			// Reset defense
			npc.defense = npc.defDefense;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Variables for buffing the AI
			bool provy = CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive;
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			bool phase2 = lifeRatio < 0.5f && revenge;
			bool phase3 = lifeRatio < 0.33f;

			// Enrage
			if ((!player.ZoneUnderworldHeight || !modPlayer.ZoneCalamity) && !BossRushEvent.BossRushActive)
			{
				if (calamityGlobalNPC.newAI[3] > 0f)
					calamityGlobalNPC.newAI[3] -= 1f;
			}
			else
				calamityGlobalNPC.newAI[3] = CalamityGlobalNPC.biomeEnrageTimerMax;

			bool biomeEnraged = calamityGlobalNPC.newAI[3] <= 0f || malice;

			float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
			if (biomeEnraged && (!player.ZoneUnderworldHeight || malice))
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 1f;
			}
			if (biomeEnraged && (!modPlayer.ZoneCalamity || malice))
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 1f;
			}

			npc.Calamity().DR = npc.ai[0] == 4f ? 0.6f : 0.15f;

			// Emit dust
			int dustAmt = (npc.ai[0] == 2f) ? 2 : 1;
			int size = (npc.ai[0] == 2f) ? 50 : 35;
			if (npc.ai[0] != 1f)
			{
				for (int num1011 = 0; num1011 < 2; num1011++)
				{
					if (Main.rand.Next(3) < dustAmt)
					{
						int dust = Dust.NewDust(vectorCenter - new Vector2(size), size * 2, size * 2, 235, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 1.5f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 0.2f;
						Main.dust[dust].fadeIn = 1f;
					}
				}
			}

			// Distance from destination where Brimmy stops moving
			float movementDistanceGateValue = 100f;

			// How fast Brimmy moves to the destination
			float baseVelocity = (death ? 6f : revenge ? 5.5f : expertMode ? 5f : 4.5f) * (npc.ai[0] == 5f ? 0.05f : npc.ai[0] == 3f ? 1.5f : 1f);
			if (expertMode)
				baseVelocity += death ? 3f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
			baseVelocity += 5f * enrageScale;

			float baseAcceleration = (death ? 0.12f : 0.1f) * (npc.ai[0] == 5f ? 0.5f : npc.ai[0] == 3f ? 1.5f : 1f);
			baseAcceleration += 0.1f * enrageScale;
			if (expertMode)
			{
				baseAcceleration += 0.03f * (1f - lifeRatio);
			}
			if (provy)
			{
				baseVelocity *= 1.25f;
				baseAcceleration *= 1.25f;
			}

			// This is where Brimmy should be
			Vector2 destination = npc.ai[0] != 3f ? player.Center : new Vector2(player.Center.X, player.Center.Y - 300f);

			// How far Brimmy is from where she's supposed to be
			Vector2 distanceFromDestination = destination - npc.Center;

			// Movement
			if (npc.ai[0] != 4f)
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

				Vector2 desiredVelocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity, maxVelocity, lerpValue);
				npc.SimpleFlyMovement(desiredVelocity, baseAcceleration);
			}

			// Rotation and direction
			if (npc.ai[0] <= 2f || npc.ai[0] == 5f)
			{
				npc.rotation = npc.velocity.X * 0.04f;
				if (npc.ai[0] != 5 || (npc.ai[1] < 180f && npc.ai[0] == 5f))
				{
					float playerLocation = vectorCenter.X - player.Center.X;
					npc.direction = playerLocation < 0f ? 1 : -1;
					npc.spriteDirection = npc.direction;
				}
			}

			if (npc.ai[0] == -1f)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int phase;
					int random = phase2 ? 6 : 5;
					do phase = Main.rand.Next(random);
					while (phase == npc.ai[1] || (phase == 0 && phase3 && revenge) || phase == 1 || phase == 2 || (phase == 4 && npc.localAI[3] != 0f));

					npc.ai[0] = phase;
					npc.ai[1] = 0f;

					// Cocoon phase cooldown
					if (npc.localAI[3] > 0f)
						npc.localAI[3] -= 1f;
					else if (phase == 4)
						npc.localAI[3] = 3f;

					npc.netUpdate = true;

					// Prevent netUpdate from being blocked by the spam counter.
					// A phase switch sync is a critical operation that must be synced.
					if (npc.netSpam >= 10)
						npc.netSpam = 9;
				}
			}

			// Pick a location to teleport to
			else if (npc.ai[0] == 0f)
			{
				npc.chaseable = true;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[1] += 1f;
					if (npc.localAI[1] >= (malice ? 5f : death ? 30f : 180f))
					{
						npc.TargetClosest();
						npc.localAI[1] = 0f;
						int timer = 0;
						int playerPosX;
						int playerPosY;
						while (true)
						{
							timer++;
							playerPosX = (int)player.Center.X / 16;
							playerPosY = (int)player.Center.Y / 16;

							int min = 12;
							int max = 16;

							if (Main.rand.NextBool(2))
								playerPosX += Main.rand.Next(min, max);
							else
								playerPosX -= Main.rand.Next(min, max);

							if (Main.rand.NextBool(2))
								playerPosY += Main.rand.Next(min, max);
							else
								playerPosY -= Main.rand.Next(min, max);

							if (!WorldGen.SolidTile(playerPosX, playerPosY))
								break;

							if (timer > 100)
								return;
						}
						npc.ai[0] = 1f;
						npc.ai[1] = playerPosX;
						npc.ai[2] = playerPosY;
						npc.netUpdate = true;
					}
				}
			}

			// Teleport to location
			else if (npc.ai[0] == 1f)
			{
				npc.chaseable = true;
				Vector2 position = new Vector2(npc.ai[1] * 16f - (npc.width / 2), npc.ai[2] * 16f - (npc.height / 2));
				for (int m = 0; m < 5; m++)
				{
					int dust = Dust.NewDust(position, npc.width, npc.height, 235, 0f, -1f, 90, default, 2f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].fadeIn = 1f;
				}
				npc.alpha += malice ? 4 : death ? 3 : 2;
				if (npc.alpha >= 255)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(ModContent.NPCType<Brimling>()) < (death ? 1 : 2) && revenge)
					{
						NPC.NewNPC((int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<Brimling>());
					}
					Main.PlaySound(SoundID.Item8, vectorCenter);
					npc.alpha = 255;
					npc.position = position;
					for (int n = 0; n < 15; n++)
					{
						int warpDust = Dust.NewDust(npc.position, npc.width, npc.height, 235, 0f, -1f, 90, default, 3f);
						Main.dust[warpDust].noGravity = true;
					}
					npc.ai[0] = 2f;
					npc.netUpdate = true;
				}
			}

			// Either teleport again or go to next AI state
			else if (npc.ai[0] == 2f)
			{
				npc.alpha -= 50;
				if (npc.alpha <= 0)
				{
					npc.chaseable = true;
					npc.ai[3] += 1f;
					npc.alpha = 0;
					if (npc.ai[3] >= 2f || phase2)
					{
						npc.ai[0] = -1f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
					else
					{
						npc.ai[0] = 0f;
					}
					npc.netUpdate = true;
				}
			}

			// Float above target and fire projectiles
			else if (npc.ai[0] == 3f)
			{
				npc.chaseable = true;
				npc.rotation = npc.velocity.X * 0.04f;

				float playerLocation = vectorCenter.X - player.Center.X;
				npc.direction = playerLocation < 0f ? 1 : -1;
				npc.spriteDirection = npc.direction;

				npc.ai[1] += 1f;
				float divisor = expertMode ? (death ? 80f : revenge ? 45f : 50f) - (float)Math.Ceiling(10f * (1f - lifeRatio)) : 50f;
				divisor -= 5f * enrageScale;
				float divisor2 = divisor * 2f;

				if (npc.ai[1] % divisor == divisor - 1f)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						float num742 = death ? 5f : 4f;
						num742 += 2f * enrageScale;
						float num743 = player.position.X + player.width * 0.5f - vectorCenter.X;
						float num744 = player.position.Y + player.height * 0.5f - vectorCenter.Y;
						float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

						num745 = num742 / num745;
						num743 *= num745;
						num744 *= num745;
						vectorCenter.X += num743 * 3f;
						vectorCenter.Y += num744 * 3f;

						int type = ModContent.ProjectileType<BrimstoneBarrage>();
						int damage = npc.GetProjectileDamage(type);
						int numProj = death ? 8 : 4;
						int spread = death ? 90 : 45;
						float rotation = MathHelper.ToRadians(spread);
						float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
						double startAngle = Math.Atan2(num743, num744) - rotation / 2;
						double deltaAngle = rotation / numProj;
						double offsetAngle;

						if (npc.ai[1] % divisor2 == divisor2 - 1f)
						{
							for (int i = 0; i < numProj; i++)
							{
								offsetAngle = startAngle + deltaAngle * i;
								Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
							}
						}

						float projectileSpeed = death ? 6f : 5f;
						if (revenge)
							projectileSpeed += 1f;
						projectileSpeed += 2f * enrageScale;

						if (expertMode)
							projectileSpeed += 3f * (1f - lifeRatio);

						vectorCenter = npc.Center;
						float relativeSpeedX = player.position.X + player.width * 0.5f - vectorCenter.X;
						float relativeSpeedY = player.position.Y + player.height * 0.5f - vectorCenter.Y;
						float totalRelativeSpeed = (float)Math.Sqrt(relativeSpeedX * relativeSpeedX + relativeSpeedY * relativeSpeedY);
						totalRelativeSpeed = projectileSpeed / totalRelativeSpeed;
						relativeSpeedX *= totalRelativeSpeed;
						relativeSpeedY *= totalRelativeSpeed;
						vectorCenter.X += relativeSpeedX * 3f;
						vectorCenter.Y += relativeSpeedY * 3f;
						type = ModContent.ProjectileType<BrimstoneHellfireball>();
						damage = npc.GetProjectileDamage(type);
						int projectileShot = Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, relativeSpeedX, relativeSpeedY, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[projectileShot].timeLeft = 240;
					}
				}

				if (npc.ai[1] >= divisor * (death ? 5f : 10f))
				{
					npc.TargetClosest();
					npc.ai[0] = -1f;
					npc.ai[1] = 3f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}

			// Cocoon bullet hell
			else if (npc.ai[0] == 4f)
			{
				npc.defense = npc.defDefense * 4;

				npc.chaseable = false;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[0] += 1f;
					if (expertMode)
						npc.localAI[0] += 1f - lifeRatio;
					npc.localAI[0] += 1f * enrageScale;

					if (npc.localAI[0] >= 120f)
					{
						npc.localAI[0] = 0f;

						float projectileSpeed = death ? 9f : revenge ? 8f : 6f;
						projectileSpeed += 3f * enrageScale;

						vectorCenter = player.Center - vectorCenter;

						float radialOffset = 0.2f;
						float diameter = 80f;

						vectorCenter = Vector2.Normalize(vectorCenter) * projectileSpeed;

						Vector2 velocity = vectorCenter;
						velocity.Normalize();
						velocity *= diameter;

						int totalProjectiles = 6;
						float offsetAngle = (float)Math.PI * radialOffset;
						int type = ModContent.ProjectileType<BrimstoneHellblast>();
						int damage = npc.GetProjectileDamage(type);
						for (int j = 0; j < totalProjectiles; j++)
						{
							float radians = j - (totalProjectiles - 1f) / 2f;
							Vector2 offset = velocity.RotatedBy(offsetAngle * radians);
							int proj = Projectile.NewProjectile(npc.Center + offset, vectorCenter, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
							Main.projectile[proj].timeLeft = 300;
							Main.projectile[proj].tileCollide = false;
						}

						totalProjectiles = death ? 16 : 12;
						float radians2 = MathHelper.TwoPi / totalProjectiles;
						type = ModContent.ProjectileType<BrimstoneBarrage>();
						damage = npc.GetProjectileDamage(type);
						double angleA = radians2 * 0.5;
						double angleB = MathHelper.ToRadians(90f) - angleA;
						float velocityX = (float)(projectileSpeed * Math.Sin(angleA) / Math.Sin(angleB));
						Vector2 spinningPoint = npc.localAI[2] % 2f == 0f ? new Vector2(0f, -projectileSpeed) : new Vector2(-velocityX, -projectileSpeed);
						for (int k = 0; k < totalProjectiles; k++)
						{
							Vector2 vector255 = spinningPoint.RotatedBy(radians2 * k);
							Projectile.NewProjectile(npc.Center, vector255, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
						}

						npc.localAI[2] += 1f;
					}
				}

				npc.velocity *= 0.95f;
				npc.rotation = npc.velocity.X * 0.04f;
				float playerLocation = vectorCenter.X - player.Center.X;
				npc.direction = playerLocation < 0f ? 1 : -1;
				npc.spriteDirection = npc.direction;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= (death ? 240f : 300f))
				{
					npc.TargetClosest();
					npc.ai[0] = -1f;
					npc.ai[1] = 4f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.localAI[0] = 0f;
					npc.netUpdate = true;
				}
			}

			// Laser beam attack
			else if (npc.ai[0] == 5f)
			{
				npc.chaseable = true;

				npc.defense = npc.defDefense * 2;

				Vector2 source = new Vector2(vectorCenter.X + (npc.spriteDirection > 0 ? 34f : -34f), vectorCenter.Y - 74f);
				Vector2 aimAt = player.Center + player.velocity * 20f;
				float aimResponsiveness = malice ? 0.05f : (npc.ai[2] == 1f || death) ? 0.1f : 0.25f;

				Vector2 aimVector = Vector2.Normalize(aimAt - source);
				if (aimVector.HasNaNs())
					aimVector = -Vector2.UnitY;
				aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(npc.velocity), aimResponsiveness));
				aimVector *= 6f;

				Vector2 laserVelocity = Vector2.Normalize(aimVector);
				if (laserVelocity.HasNaNs())
					laserVelocity = -Vector2.UnitY;

				calamityGlobalNPC.newAI[1] = laserVelocity.X;
				calamityGlobalNPC.newAI[2] = laserVelocity.Y;

				// Rev = 190 + 165 = 355
				// Death = 165

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 240f)
				{
					npc.TargetClosest();
					npc.ai[2] += 1f;
					npc.localAI[0] = 0f;
					npc.localAI[1] = 0f;
					if (npc.ai[2] >= (death ? 1f : 2f))
					{
						npc.ai[0] = -1f;
						npc.ai[1] = 5f;
						npc.ai[2] = 0f;
						calamityGlobalNPC.newAI[0] = 0f;
					}
					else
					{
						npc.ai[1] = 0f;
						calamityGlobalNPC.newAI[0] = 0f;
					}
				}
				else if (npc.ai[1] >= 180f)
				{
					npc.velocity *= 0.95f;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (npc.ai[1] == 180f)
						{
							Main.PlaySound(SoundID.Item68, source);
							Vector2 laserVelocity2 = new Vector2(npc.localAI[0], npc.localAI[1]);
							laserVelocity2.Normalize();
							int type = ModContent.ProjectileType<BrimstoneRay>();
							int damage = npc.GetProjectileDamage(type);
							Projectile.NewProjectile(source, laserVelocity2, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, npc.whoAmI);
						}
					}
				}
				else
				{
					float playSoundTimer = 30f;
					if (npc.ai[1] < 150f)
					{
						switch ((int)npc.ai[2])
						{
							case 0:
								npc.ai[1] += 0.5f;
								break;
							case 1:
								npc.ai[1] += 1f;
								playSoundTimer = 40f;
								break;
						}
						if (death)
						{
							npc.ai[1] += 0.5f;
							playSoundTimer += 10f;
						}
					}

					if (npc.ai[1] % playSoundTimer == 0f)
						Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 20);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (npc.ai[1] < 150f && calamityGlobalNPC.newAI[0] == 0f)
						{
							Projectile.NewProjectile(source, laserVelocity, ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
							calamityGlobalNPC.newAI[0] = 1f;
						}
						else
						{
							if (npc.ai[1] == 150f)
							{
								npc.localAI[0] = laserVelocity.X;
								npc.localAI[1] = laserVelocity.Y;
								Projectile.NewProjectile(source.X, source.Y, npc.localAI[0], npc.localAI[1], ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 1f, npc.whoAmI);
							}
						}
					}
				}
			}
		}
		#endregion

		#region Calamitas Clone
		public static void CalamitasCloneAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Emit light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 1f, 0f, 0f);

			// Variables for increasing difficulty
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool provy = CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Phases
			bool phase3 = lifeRatio < 0.7f || death;
			bool phase4 = lifeRatio < 0.35f;
			bool phase5 = lifeRatio <= 0.1f && revenge;

			// Don't take damage during bullet hells
			npc.dontTakeDamage = calamityGlobalNPC.newAI[2] > 0f;

			// Variable for live brothers
			bool brotherAlive = false;

			// For seekers
			CalamityGlobalNPC.calamitas = npc.whoAmI;

			// Seeker ring
			if (calamityGlobalNPC.newAI[1] == 0f && phase4 && expertMode)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 74);
					int seekerAmt = death ? 10 : 5;
					int seekerSpread = 360 / seekerAmt;
					int seekerDistance = death ? 180 : 150;
					for (int i = 0; i < seekerAmt; i++)
					{
						int spawn = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(i * seekerSpread) * seekerDistance)), (int)(npc.Center.Y + (Math.Cos(i * seekerSpread) * seekerDistance)), ModContent.NPCType<SoulSeeker>(), npc.whoAmI, 0, 0, 0, -1);
						Main.npc[spawn].ai[0] = i * seekerSpread;
					}
				}

				string key = "Mods.CalamityMod.CalamitasBossText3";
				Color messageColor = Color.Orange;
				CalamityUtils.DisplayLocalizedText(key, messageColor);

				calamityGlobalNPC.newAI[1] = 1f;
			}

			// Do bullet hell or spawn brothers
			if (calamityGlobalNPC.newAI[0] == 0f && npc.life > 0)
				calamityGlobalNPC.newAI[0] = npc.lifeMax;

			// Bullet hells at 70% and 10%, brothers at 40%
			if (npc.life > 0)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int num660 = (int)(npc.lifeMax * 0.3);
					if ((npc.life + num660) < calamityGlobalNPC.newAI[0])
					{
						calamityGlobalNPC.newAI[0] = npc.life;
						if (calamityGlobalNPC.newAI[0] <= (float)npc.lifeMax * 0.1)
						{
							Main.PlaySound(SoundID.Item109, npc.Center);
							calamityGlobalNPC.newAI[2] = 2f;
							SpawnDust();
							npc.alpha = 255;
						}
						else if (calamityGlobalNPC.newAI[0] <= (float)npc.lifeMax * 0.4)
						{
							NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<CalamitasRun>(), npc.whoAmI);
							NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<CalamitasRun2>(), npc.whoAmI);

							string key = "Mods.CalamityMod.CalamitasBossText2";
							Color messageColor = Color.Orange;
							CalamityUtils.DisplayLocalizedText(key, messageColor);
						}
						else
						{
							Main.PlaySound(SoundID.Item109, npc.Center);
							calamityGlobalNPC.newAI[2] = 1f;
							SpawnDust();
							npc.alpha = 255;
						}
					}
				}
			}

			// Huge DR boost if brothers are alive
			if (expertMode)
			{
				if (CalamityGlobalNPC.cataclysm != -1)
				{
					if (Main.npc[CalamityGlobalNPC.cataclysm].active)
						brotherAlive = true;
				}
				if (CalamityGlobalNPC.catastrophe != -1)
				{
					if (Main.npc[CalamityGlobalNPC.catastrophe].active)
						brotherAlive = true;
				}
				if (brotherAlive)
				{
					calamityGlobalNPC.DR = 0.9999f;
					calamityGlobalNPC.unbreakableDR = true;
				}
				else
				{
					calamityGlobalNPC.DR = death ? 0.075f : 0.15f;
					calamityGlobalNPC.unbreakableDR = false;
				}
			}

			// Disable homing if brothers are alive
			npc.chaseable = !brotherAlive;

			void SpawnDust()
			{
				int dustAmt = 50;
				int random = 3;

				for (int j = 0; j < 10; j++)
				{
					random += j;
					int dustAmtSpawned = 0;
					int scale = random * 6;
					float dustPositionX = npc.Center.X - (scale / 2);
					float dustPositionY = npc.Center.Y - (scale / 2);
					while (dustAmtSpawned < dustAmt)
					{
						float dustVelocityX = Main.rand.Next(-random, random);
						float dustVelocityY = Main.rand.Next(-random, random);
						float dustVelocityScalar = random * 2f;
						float dustVelocity = (float)Math.Sqrt(dustVelocityX * dustVelocityX + dustVelocityY * dustVelocityY);
						dustVelocity = dustVelocityScalar / dustVelocity;
						dustVelocityX *= dustVelocity;
						dustVelocityY *= dustVelocity;
						int dust = Dust.NewDust(new Vector2(dustPositionX, dustPositionY), scale, scale, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].position.X = npc.Center.X;
						Main.dust[dust].position.Y = npc.Center.Y;
						Main.dust[dust].position.X += Main.rand.Next(-10, 11);
						Main.dust[dust].position.Y += Main.rand.Next(-10, 11);
						Main.dust[dust].velocity.X = dustVelocityX;
						Main.dust[dust].velocity.Y = dustVelocityY;
						dustAmtSpawned++;
					}
				}
			}

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			// Target variable
			Player player = Main.player[npc.target];

			float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
			if (Main.dayTime || malice)
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 2f;
			}

			// Rotation
			Vector2 npcCenter = new Vector2(npc.Center.X, npc.position.Y + npc.height - 59f);
			Vector2 lookAt = new Vector2(player.position.X - (player.width / 2), player.position.Y - (player.height / 2));
			Vector2 rotationVector = npcCenter - lookAt;

			// Malice Mode predictive charge rotation
			if (npc.ai[1] == 4f && phase5 && malice)
			{
				// Velocity
				float chargeVelocity = 30f;
				chargeVelocity += 5f * enrageScale;
				if (provy)
					chargeVelocity *= 1.25f;

				lookAt += Main.player[npc.target].velocity * 20f;
				rotationVector = Vector2.Normalize(npcCenter - lookAt) * chargeVelocity;
			}

			float rotation = (float)Math.Atan2(rotationVector.Y, rotationVector.X) + MathHelper.PiOver2;
			if (rotation < 0f)
				rotation += MathHelper.TwoPi;
			else if (rotation > MathHelper.TwoPi)
				rotation -= MathHelper.TwoPi;

			float rotationAmt = 0.1f;
			if (npc.rotation < rotation)
			{
				if ((rotation - npc.rotation) > MathHelper.Pi)
					npc.rotation -= rotationAmt;
				else
					npc.rotation += rotationAmt;
			}
			else if (npc.rotation > rotation)
			{
				if ((npc.rotation - rotation) > MathHelper.Pi)
					npc.rotation += rotationAmt;
				else
					npc.rotation -= rotationAmt;
			}

			if (npc.rotation > rotation - rotationAmt && npc.rotation < rotation + rotationAmt)
				npc.rotation = rotation;
			if (npc.rotation < 0f)
				npc.rotation += MathHelper.TwoPi;
			else if (npc.rotation > MathHelper.TwoPi)
				npc.rotation -= MathHelper.TwoPi;
			if (npc.rotation > rotation - rotationAmt && npc.rotation < rotation + rotationAmt)
				npc.rotation = rotation;

			// Despawn
			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.ai[1] != 0f)
					{
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						npc.alpha = 0;
						npc.netUpdate = true;
					}
					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			// Reset damage from bullet hell phases
			npc.damage = npc.defDamage;

			// Distance from destination where Cal Clone stops moving
			float movementDistanceGateValue = 100f;

			// How fast Cal Clone moves to the destination
			float baseVelocity = (expertMode ? 10f : 8.5f) * (npc.ai[1] == 4f ? 1.4f : 1f);
			float baseAcceleration = (expertMode ? 0.18f : 0.155f) * (npc.ai[1] == 4f ? 1.4f : 1f);
			baseVelocity += 4f * enrageScale;
			baseAcceleration += 0.1f * enrageScale;
			if (death)
			{
				baseVelocity += 2f * (1f - lifeRatio);
				baseAcceleration += 0.04f * (1f - lifeRatio);
			}
			if (provy)
			{
				baseVelocity *= 1.25f;
				baseAcceleration *= 1.25f;
			}

			// Reduce acceleration if target is holding a true melee weapon
			Item targetSelectedItem = player.inventory[player.selectedItem];
			if (targetSelectedItem.melee && (targetSelectedItem.shoot == ProjectileID.None || targetSelectedItem.Calamity().trueMelee))
				baseAcceleration *= 0.5f;

			// What side Cal Clone should be on, relative to the target
			int xPos = 1;
			if (npc.Center.X < player.Center.X)
				xPos = -1;

			// How far Cal Clone should be from the target
			float averageDistance = 500f;
			float chargeDistance = phase5 ? 300f : 400f;

			// This is where Cal Clone should be
			Vector2 destination = (calamityGlobalNPC.newAI[2] > 0f || npc.ai[1] == 0f) ? new Vector2(player.Center.X, player.Center.Y - averageDistance) :
				npc.ai[1] == 1f ? new Vector2(player.Center.X + averageDistance * xPos, player.Center.Y) :
				new Vector2(player.Center.X + chargeDistance * xPos, player.Center.Y);

			// Add some random distance to the destination after certain attacks
			if (npc.localAI[0] == 1f)
			{
				npc.localAI[0] = 0f;
				npc.localAI[2] = Main.rand.Next(-50, 51);
				npc.localAI[3] = Main.rand.Next(-300, 301);
				npc.netUpdate = true;
			}

			// Add a bit of randomness to the destination
			if (death)
			{
				destination.X += npc.ai[1] == 0f ? npc.localAI[3] : npc.localAI[2];
				destination.Y += npc.ai[1] == 0f ? npc.localAI[2] : npc.localAI[3];
			}

			// How far Cal Clone is from where she's supposed to be
			Vector2 distanceFromDestination = destination - npc.Center;

			// Movement
			if (npc.ai[1] == 0f || npc.ai[1] == 1f || npc.ai[1] == 4f || calamityGlobalNPC.newAI[2] > 0f)
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

				Vector2 desiredVelocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity, maxVelocity, lerpValue);
				npc.SimpleFlyMovement(desiredVelocity, baseAcceleration);
			}

			// Bullet hell phase
			if (calamityGlobalNPC.newAI[2] > 0f)
			{
				if (calamityGlobalNPC.newAI[3] < 900f)
				{
					calamityGlobalNPC.newAI[3] += 1f;
					npc.damage = 0;
					npc.chaseable = false;
					npc.dontTakeDamage = true;

					float rotX = player.Center.X - npc.Center.X;
					float rotY = player.Center.Y - npc.Center.Y;
					npc.rotation = (float)Math.Atan2(rotY, rotX) - MathHelper.PiOver2;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (calamityGlobalNPC.newAI[2] == 2f)
						{
							int type = ModContent.ProjectileType<BrimstoneGigaBlast>();
							int damage = npc.GetProjectileDamage(type) + (provy ? 30 : 0);
							float gigaBlastFrequency = (expertMode ? 180f : 240f) - enrageScale * 15f;
							float projSpeed = malice ? 6.25f : 5f;
							if (calamityGlobalNPC.newAI[3] <= 300f)
							{
								if (calamityGlobalNPC.newAI[3] % gigaBlastFrequency == 0f) // Blasts from top
									Projectile.NewProjectile(player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, projSpeed, type, damage, 0f, Main.myPlayer);
							}
							else if (calamityGlobalNPC.newAI[3] <= 600f && calamityGlobalNPC.newAI[3] > 300f)
							{
								if (calamityGlobalNPC.newAI[3] % gigaBlastFrequency == 0f) // Blasts from right
									Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -projSpeed, 0f, type, damage, 0f, Main.myPlayer);
							}
							else if (calamityGlobalNPC.newAI[3] > 600f)
							{
								if (calamityGlobalNPC.newAI[3] % gigaBlastFrequency == 0f) // Blasts from top
									Projectile.NewProjectile(player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, projSpeed, type, damage, 0f, Main.myPlayer);
							}
						}

						npc.ai[0] += 1f;
						float hellblastGateValue = (expertMode ? 12f : 16f) - enrageScale;
						if (npc.ai[0] >= hellblastGateValue)
						{
							npc.ai[0] = 0f;
							int type = ModContent.ProjectileType<BrimstoneHellblast2>();
							int damage = npc.GetProjectileDamage(type) + (provy ? 30 : 0);
							float projSpeed = malice ? 5f : 4f;
							if (calamityGlobalNPC.newAI[3] % (hellblastGateValue * 6f) == 0f)
							{
								float distance = Main.rand.NextBool() ? -1000f : 1000f;
								float velocity = distance == -1000f ? projSpeed : -projSpeed;
								Projectile.NewProjectile(player.position.X + distance, player.position.Y, velocity, 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
							}
							if (calamityGlobalNPC.newAI[3] < 300f) // Blasts from above
							{
								Projectile.NewProjectile(player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, projSpeed, type, damage, 0f, Main.myPlayer, 2f, 0f);
							}
							else if (calamityGlobalNPC.newAI[3] < 600f) // Blasts from left and right
							{
								Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -(projSpeed - 0.5f), 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
								Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), projSpeed - 0.5f, 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
							}
							else // Blasts from above, left, and right
							{
								Projectile.NewProjectile(player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, projSpeed - 1f, type, damage, 0f, Main.myPlayer, 2f, 0f);
								Projectile.NewProjectile(player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -(projSpeed - 1f), 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
								Projectile.NewProjectile(player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), projSpeed - 1f, 0f, type, damage, 0f, Main.myPlayer, 2f, 0f);
							}
						}
					}
				}
				else
				{
					npc.ai[0] = 0f;
					npc.ai[3] = 0f;
					npc.localAI[1] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					calamityGlobalNPC.newAI[3] = 0f;
					npc.alpha = 0;

					// Prevent bullshit charge hits when second bullet hell ends.
					if (phase5)
					{
						npc.ai[1] = 4f;
						npc.ai[2] = -105f;
						npc.TargetClosest();
					}
					else
					{
						if (death)
						{
							int AIState = Main.rand.Next(3);
							switch (AIState)
							{
								case 0:
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									break;
								case 1:
									npc.ai[1] = 1f;
									npc.ai[2] = 0f;
									break;
								case 2:
									npc.ai[1] = 4f;
									npc.ai[2] = -105f;
									npc.TargetClosest();
									break;
							}
						}
						else
						{
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						if (death)
							npc.localAI[0] = 1f;
					}

					npc.netUpdate = true;

					for (int x = 0; x < Main.maxProjectiles; x++)
					{
						Projectile projectile = Main.projectile[x];
						if (projectile.active)
						{
							if (projectile.type == ModContent.ProjectileType<BrimstoneHellblast2>() || projectile.type == ModContent.ProjectileType<BrimstoneBarrage>())
							{
								if (projectile.timeLeft > 60)
									projectile.timeLeft = 60;
							}
							else if (projectile.type == ModContent.ProjectileType<BrimstoneGigaBlast>())
							{
								projectile.ai[1] = 1f;

								if (projectile.timeLeft > 60)
									projectile.timeLeft = 60;
							}
						}
					}
				}

				return;
			}

			// Float above target and fire lasers or fireballs
			if (npc.ai[1] == 0f)
			{
				npc.ai[2] += 1f;
				float phaseTimer = 400f - (death ? 120f * (1f - lifeRatio) : 0f);
				if (npc.ai[2] >= phaseTimer || phase5)
				{
					if (death && !phase5 && Main.rand.Next(2) == 0 && !brotherAlive)
						npc.ai[1] = 4f;
					else
						npc.ai[1] = 1f;

					npc.ai[2] = 0f;
					if (death)
						npc.localAI[0] = 1f;

					npc.TargetClosest();
					npc.netUpdate = true;
				}

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[1] += 1f;
					if (!brotherAlive)
					{
						if (expertMode)
							npc.localAI[1] += death ? 2f * (1f - lifeRatio) : 1f - lifeRatio;
						if (revenge)
							npc.localAI[1] += 0.5f;
					}

					if (npc.localAI[1] >= (brotherAlive ? 180f : 120f))
					{
						npc.localAI[1] = 0f;

						float projectileVelocity = expertMode ? 14f : 12.5f;
						projectileVelocity += 3f * enrageScale;
						int type = ModContent.ProjectileType<BrimstoneHellfireball>();
						int damage = npc.GetProjectileDamage(type);
						Vector2 fireballVelocity = Vector2.Normalize(player.Center - npc.Center) * projectileVelocity;
						Vector2 offset = Vector2.Normalize(fireballVelocity) * 40f;

						if (!Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							int proj = Projectile.NewProjectile(npc.Center + offset, fireballVelocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, player.Center.X, player.Center.Y);
							Main.projectile[proj].tileCollide = false;
						}
						else
							Projectile.NewProjectile(npc.Center + offset, fireballVelocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer);
					}
				}
			}

			// Float to the side of the target and fire
			else if (npc.ai[1] == 1f)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[1] += 1f;
					if (!brotherAlive)
					{
						if (revenge)
							npc.localAI[1] += 0.5f;
						if (expertMode)
							npc.localAI[1] += 0.5f;
					}

					if (npc.localAI[1] >= (brotherAlive ? 75f : 50f) && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
					{
						npc.localAI[1] = 0f;

						float projectileVelocity = expertMode ? 12.5f : 11f;
						projectileVelocity += 3f * enrageScale;
						int type = brotherAlive ? ModContent.ProjectileType<BrimstoneHellfireball>() : ModContent.ProjectileType<BrimstoneHellblast>();
						int damage = npc.GetProjectileDamage(type);
						Vector2 fireballVelocity = Vector2.Normalize(player.Center - npc.Center) * projectileVelocity;
						Vector2 offset = Vector2.Normalize(fireballVelocity) * 40f;

						if (!Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							type = ModContent.ProjectileType<BrimstoneHellfireball>();
							damage = npc.GetProjectileDamage(type);
							int proj = Projectile.NewProjectile(npc.Center + offset, fireballVelocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, player.Center.X, player.Center.Y);
							Main.projectile[proj].tileCollide = false;
						}
						else
						{
							float ai0 = type == ModContent.ProjectileType<BrimstoneHellblast>() ? 1f : 0f;
							Projectile.NewProjectile(npc.Center + offset, fireballVelocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, ai0, 0f);
						}
					}
				}

				npc.ai[2] += 1f;
				float phaseTimer = 240f - (death ? 60f * (1f - lifeRatio) : 0f);
				if (npc.ai[2] >= phaseTimer || phase5)
				{
					if (death && !phase5 && Main.rand.Next(2) == 0 && !brotherAlive)
						npc.ai[1] = 0f;
					else
						npc.ai[1] = !brotherAlive && phase3 && revenge ? 4f : 0f;

					npc.ai[2] = 0f;
					if (death)
						npc.localAI[0] = 1f;

					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[1] == 2f)
			{
				npc.rotation = rotation;

				float chargeVelocity = phase5 ? 30f : (25f + (death ? 4f * (1f - lifeRatio) : 0f));
				chargeVelocity += 5f * enrageScale;

				if (provy)
					chargeVelocity *= 1.25f;

				Vector2 vector = Vector2.Normalize(player.Center + (phase5 && malice ? player.velocity * 20f : Vector2.Zero) - npc.Center);
				npc.velocity = vector * chargeVelocity;

				npc.ai[1] = 3f;
				npc.netUpdate = true;
			}
			else if (npc.ai[1] == 3f)
			{
				npc.ai[2] += 1f;

				float chargeTime = phase5 ? 35f : (45f - (death ? 6f * (1f - lifeRatio) : 0f));
				if (npc.ai[2] >= chargeTime)
				{
					npc.velocity *= 0.9f;
					if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
						npc.velocity.X = 0f;
					if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
						npc.velocity.Y = 0f;
				}
				else
				{
					npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

					// Leave behind slow hellblasts in Death Mode
					if (Main.netMode != NetmodeID.MultiplayerClient && death && phase4 && npc.ai[2] % (phase5 ? 6f : 10f) == 0f)
					{
						int type = ModContent.ProjectileType<BrimstoneHellblast>();
						int damage = npc.GetProjectileDamage(type);
						Vector2 fireballVelocity = npc.velocity * 0.01f;
						Projectile.NewProjectile(npc.Center, fireballVelocity, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
					}
				}

				if (npc.ai[2] >= chargeTime + 10f)
				{
					if (!phase5)
						npc.ai[3] += 1f;

					npc.ai[2] = 0f;

					npc.rotation = rotation;
					npc.TargetClosest();
					npc.netUpdate = true;

					if (npc.ai[3] >= 2f)
					{
						npc.ai[1] = 0f;
						npc.ai[3] = 0f;
						return;
					}

					npc.ai[1] = 4f;
				}
			}
			else
			{
				npc.ai[2] += 1f;
				if (npc.ai[2] >= (phase5 ? 15f : 30f))
				{
					npc.TargetClosest();

					npc.ai[1] = 2f;
					npc.ai[2] = 0f;
					if (death)
						npc.localAI[0] = 1f;

					npc.netUpdate = true;
				}
			}
		}

		public static void CataclysmAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Emit light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 1f, 0f, 0f);

			CalamityGlobalNPC.cataclysm = npc.whoAmI;

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool provy = CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
			if (Main.dayTime || malice)
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 2f;
			}

			float num840 = npc.position.X + (npc.width / 2) - player.position.X - (player.width / 2);
			float num841 = npc.position.Y + npc.height - 59f - player.position.Y - (player.height / 2);
			float num842 = (float)Math.Atan2(num841, num840) + MathHelper.PiOver2;
			if (num842 < 0f)
				num842 += MathHelper.TwoPi;
			else if (num842 > MathHelper.TwoPi)
				num842 -= MathHelper.TwoPi;

			float num843 = 0.15f;
			if (npc.rotation < num842)
			{
				if ((num842 - npc.rotation) > MathHelper.Pi)
					npc.rotation -= num843;
				else
					npc.rotation += num843;
			}
			else if (npc.rotation > num842)
			{
				if ((npc.rotation - num842) > MathHelper.Pi)
					npc.rotation += num843;
				else
					npc.rotation -= num843;
			}

			if (npc.rotation > num842 - num843 && npc.rotation < num842 + num843)
				npc.rotation = num842;
			if (npc.rotation < 0f)
				npc.rotation += MathHelper.TwoPi;
			else if (npc.rotation > MathHelper.TwoPi)
				npc.rotation -= MathHelper.TwoPi;
			if (npc.rotation > num842 - num843 && npc.rotation < num842 + num843)
				npc.rotation = num842;

			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					calamityGlobalNPC.newAI[0] = 1f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.ai[1] != 0f)
					{
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					return;
				}
			}
			else
				calamityGlobalNPC.newAI[0] = 0f;

			if (npc.ai[1] == 0f)
			{
				float num861 = 5f;
				float num862 = 0.1f;
				num861 += 2f * enrageScale;
				num862 += 0.06f * enrageScale;

				if (provy)
				{
					num861 *= 1.25f;
					num862 *= 1.25f;
				}

				int num863 = 1;
				if (npc.position.X + (npc.width / 2) < player.position.X + player.width)
					num863 = -1;

				Vector2 vector86 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
				float num864 = player.position.X + (player.width / 2) + (num863 * 180) - vector86.X;
				float num865 = player.position.Y + (player.height / 2) - vector86.Y;
				float num866 = (float)Math.Sqrt(num864 * num864 + num865 * num865);

				if (expertMode || provy)
				{
					if (num866 > 300f)
						num861 += 0.5f;
					if (num866 > 400f)
						num861 += 0.5f;
					if (num866 > 500f)
						num861 += 0.55f;
					if (num866 > 600f)
						num861 += 0.55f;
					if (num866 > 700f)
						num861 += 0.6f;
					if (num866 > 800f)
						num861 += 0.6f;
				}

				num866 = num861 / num866;
				num864 *= num866;
				num865 *= num866;

				if (npc.velocity.X < num864)
				{
					npc.velocity.X += num862;
					if (npc.velocity.X < 0f && num864 > 0f)
						npc.velocity.X += num862;
				}
				else if (npc.velocity.X > num864)
				{
					npc.velocity.X -= num862;
					if (npc.velocity.X > 0f && num864 < 0f)
						npc.velocity.X -= num862;
				}
				if (npc.velocity.Y < num865)
				{
					npc.velocity.Y += num862;
					if (npc.velocity.Y < 0f && num865 > 0f)
						npc.velocity.Y += num862;
				}
				else if (npc.velocity.Y > num865)
				{
					npc.velocity.Y -= num862;
					if (npc.velocity.Y > 0f && num865 < 0f)
						npc.velocity.Y -= num862;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (240f - (death ? 120f * (1f - lifeRatio) : 0f)))
				{
					npc.TargetClosest();
					npc.ai[1] = 1f;
					npc.ai[2] = 0f;
					npc.target = 255;
					npc.netUpdate = true;
				}

				bool fireDelay = npc.ai[2] > 120f || npc.life < npc.lifeMax * 0.9;
				if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height) && fireDelay)
				{
					npc.localAI[2] += 1f;
					if (npc.localAI[2] > 22f)
					{
						npc.localAI[2] = 0f;
						Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 34);
					}

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.localAI[1] += 1f;
						if (revenge)
							npc.localAI[1] += 0.5f;

						if (npc.localAI[1] > 12f)
						{
							npc.localAI[1] = 0f;
							float num867 = NPC.AnyNPCs(ModContent.NPCType<CalamitasRun2>()) ? 4f : 6f;
							num867 += enrageScale;
							int type = ModContent.ProjectileType<BrimstoneFire>();
							int damage = npc.GetProjectileDamage(type);
							vector86 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
							num864 = player.position.X + (player.width / 2) - vector86.X;
							num865 = player.position.Y + (player.height / 2) - vector86.Y;
							num866 = (float)Math.Sqrt(num864 * num864 + num865 * num865);
							num866 = num867 / num866;
							num864 *= num866;
							num865 *= num866;
							num865 += npc.velocity.Y * 0.5f;
							num864 += npc.velocity.X * 0.5f;
							vector86.X -= num864;
							vector86.Y -= num865;
							Projectile.NewProjectile(vector86.X, vector86.Y, num864, num865, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}
			else
			{
				if (npc.ai[1] == 1f)
				{
					Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0);
					npc.rotation = num842;

					float num870 = 14f + (death ? 4f * (1f - lifeRatio) : 0f);
					num870 += 3f * enrageScale;
					if (expertMode)
						num870 += 2f;
					if (revenge)
						num870 += 2f;
					if (provy)
						num870 *= 1.15f;

					Vector2 vector87 = npc.Center;
					float num871 = player.Center.X - vector87.X;
					float num872 = player.Center.Y - vector87.Y;
					float num873 = (float)Math.Sqrt(num871 * num871 + num872 * num872);
					num873 = num870 / num873;
					npc.velocity.X = num871 * num873;
					npc.velocity.Y = num872 * num873;
					npc.ai[1] = 2f;
					return;
				}

				if (npc.ai[1] == 2f)
				{
					npc.ai[2] += 1f + (death ? 0.5f * (1f - lifeRatio) : 0f);
					if (expertMode)
						npc.ai[2] += 0.25f;
					if (revenge)
						npc.ai[2] += 0.25f;

					if (npc.ai[2] >= 75f)
					{
						npc.velocity.X *= 0.93f;
						npc.velocity.Y *= 0.93f;

						if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
							npc.velocity.X = 0f;
						if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
							npc.velocity.Y = 0f;
					}
					else
						npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

					if (npc.ai[2] >= 105f)
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = num842;
						npc.TargetClosest();
						if (npc.ai[3] >= 3f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
							return;
						}
						npc.ai[1] = 1f;
					}
				}
			}
		}

		public static void CatastropheAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Emit light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 1f, 0f, 0f);

			CalamityGlobalNPC.catastrophe = npc.whoAmI;

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool provy = CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
			if (Main.dayTime || malice)
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 2f;
			}

			float num840 = npc.position.X + (npc.width / 2) - player.position.X - (player.width / 2);
			float num841 = npc.position.Y + npc.height - 59f - player.position.Y - (player.height / 2);
			float num842 = (float)Math.Atan2(num841, num840) + MathHelper.PiOver2;
			if (num842 < 0f)
				num842 += MathHelper.TwoPi;
			else if (num842 > MathHelper.TwoPi)
				num842 -= MathHelper.TwoPi;

			float num843 = 0.15f;
			if (npc.rotation < num842)
			{
				if ((num842 - npc.rotation) > MathHelper.Pi)
					npc.rotation -= num843;
				else
					npc.rotation += num843;
			}
			else if (npc.rotation > num842)
			{
				if ((npc.rotation - num842) > MathHelper.Pi)
					npc.rotation += num843;
				else
					npc.rotation -= num843;
			}

			if (npc.rotation > num842 - num843 && npc.rotation < num842 + num843)
				npc.rotation = num842;
			if (npc.rotation < 0f)
				npc.rotation += MathHelper.TwoPi;
			else if (npc.rotation > MathHelper.TwoPi)
				npc.rotation -= MathHelper.TwoPi;
			if (npc.rotation > num842 - num843 && npc.rotation < num842 + num843)
				npc.rotation = num842;

			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					calamityGlobalNPC.newAI[0] = 1f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.ai[1] != 0f)
					{
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					return;
				}
			}
			else
				calamityGlobalNPC.newAI[0] = 0f;

			if (npc.ai[1] == 0f)
			{
				float num861 = 4.5f;
				float num862 = 0.2f;
				num861 += 2f * enrageScale;
				num862 += 0.1f * enrageScale;

				if (provy)
				{
					num861 *= 1.25f;
					num862 *= 1.25f;
				}

				int num863 = 1;
				if (npc.Center.X < player.Center.X)
					num863 = -1;

				Vector2 vector86 = npc.Center;
				float num864 = player.Center.X + (num863 * 180) - vector86.X;
				float num865 = player.Center.Y - vector86.Y;
				float num866 = (float)Math.Sqrt(num864 * num864 + num865 * num865);

				if (expertMode || provy)
				{
					if (num866 > 300f)
						num861 += 0.5f;
					if (num866 > 400f)
						num861 += 0.5f;
					if (num866 > 500f)
						num861 += 0.55f;
					if (num866 > 600f)
						num861 += 0.55f;
					if (num866 > 700f)
						num861 += 0.6f;
					if (num866 > 800f)
						num861 += 0.6f;
				}

				num866 = num861 / num866;
				num864 *= num866;
				num865 *= num866;

				if (npc.velocity.X < num864)
				{
					npc.velocity.X += num862;
					if (npc.velocity.X < 0f && num864 > 0f)
						npc.velocity.X += num862;
				}
				else if (npc.velocity.X > num864)
				{
					npc.velocity.X -= num862;
					if (npc.velocity.X > 0f && num864 < 0f)
						npc.velocity.X -= num862;
				}
				if (npc.velocity.Y < num865)
				{
					npc.velocity.Y += num862;
					if (npc.velocity.Y < 0f && num865 > 0f)
						npc.velocity.Y += num862;
				}
				else if (npc.velocity.Y > num865)
				{
					npc.velocity.Y -= num862;
					if (npc.velocity.Y > 0f && num865 < 0f)
						npc.velocity.Y -= num862;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (180f - (death ? 90f * (1f - lifeRatio) : 0f)))
				{
					npc.TargetClosest();
					npc.ai[1] = 1f;
					npc.ai[2] = 0f;
					npc.target = 255;
					npc.netUpdate = true;
				}

				bool fireDelay = npc.ai[2] > 120f || lifeRatio < 0.9f;
				if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height) && fireDelay)
				{
					npc.localAI[2] += 1f;
					if (npc.localAI[2] > 36f)
					{
						npc.localAI[2] = 0f;
						Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 34);
					}

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.localAI[1] += 1f;
						if (revenge)
							npc.localAI[1] += 0.5f;

						if (npc.localAI[1] > 50f)
						{
							npc.localAI[1] = 0f;
							float num867 = death ? 14f : 12f;
							num867 += 3f * enrageScale;
							int type = ModContent.ProjectileType<BrimstoneBall>();
							int damage = npc.GetProjectileDamage(type);
							vector86 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
							num864 = player.position.X + (player.width / 2) - vector86.X;
							num865 = player.position.Y + (player.height / 2) - vector86.Y;
							num866 = (float)Math.Sqrt(num864 * num864 + num865 * num865);
							num866 = num867 / num866;
							num864 *= num866;
							num865 *= num866;
							num865 += npc.velocity.Y * 0.5f;
							num864 += npc.velocity.X * 0.5f;
							vector86.X -= num864;
							vector86.Y -= num865;
							Projectile.NewProjectile(vector86.X, vector86.Y, num864, num865, type, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}
			else
			{
				if (npc.ai[1] == 1f)
				{
					Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0);
					npc.rotation = num842;

					float num870 = (NPC.AnyNPCs(ModContent.NPCType<CalamitasRun>()) ? 12f : 16f) + (death ? 4f * (1f - lifeRatio) : 0f);
					num870 += 4f * enrageScale;
					if (expertMode)
						num870 += 2f;
					if (revenge)
						num870 += 2f;
					if (provy)
						num870 *= 1.15f;

					Vector2 vector87 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
					float num871 = player.position.X + (player.width / 2) - vector87.X;
					float num872 = player.position.Y + (player.height / 2) - vector87.Y;
					float num873 = (float)Math.Sqrt(num871 * num871 + num872 * num872);
					num873 = num870 / num873;
					npc.velocity.X = num871 * num873;
					npc.velocity.Y = num872 * num873;
					npc.ai[1] = 2f;
					return;
				}

				if (npc.ai[1] == 2f)
				{
					npc.ai[2] += 1f + (death ? 0.5f * (1f - lifeRatio) : 0f);
					if (expertMode)
						npc.ai[2] += 0.25f;
					if (revenge)
						npc.ai[2] += 0.25f;

					if (npc.ai[2] >= 60f) //50
					{
						npc.velocity.X *= 0.93f;
						npc.velocity.Y *= 0.93f;

						if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
							npc.velocity.X = 0f;
						if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
							npc.velocity.Y = 0f;
					}
					else
						npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - MathHelper.PiOver2;

					if (npc.ai[2] >= 90f) //80
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.TargetClosest();
						npc.rotation = num842;
						if (npc.ai[3] >= 4f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
							return;
						}
						npc.ai[1] = 1f;
					}
				}
			}
		}
		#endregion

		#region Astrum Aureus
		public static void AstrumAureusAI(NPC npc, Mod mod)
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			bool postMoonLordBuff = NPC.downedMoonlord && !BossRushEvent.BossRushActive;
			npc.damage = postMoonLordBuff ? npc.defDamage * 2 : npc.defDamage;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Variables
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			float shootTimer = 1f;
			if (expertMode)
				shootTimer += death ? (float)Math.Round(3f * (1f - lifeRatio)) : (float)Math.Round(2f * (1f - lifeRatio));

			float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
			if (Main.dayTime || malice)
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 2f;
			}

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			// Direction
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;

			// Phases
			bool phase2 = lifeRatio < (revenge ? 0.85f : expertMode ? 0.8f : 0.75f);
			bool phase3 = lifeRatio < (revenge ? 0.7f : expertMode ? 0.6f : 0.5f);
			bool phase4 = lifeRatio < (revenge ? 0.5f : 0.4f) && expertMode;
			bool phase5 = lifeRatio < 0.3f && revenge;
			bool reduceFallSpeed = npc.velocity.Y > 0f && npc.Bottom.Y > player.Top.Y && npc.ai[0] == 4f;

			// Despawn
			bool despawnDistance = Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles;
			if (!player.active || player.dead || despawnDistance)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead || despawnDistance)
                {
                    npc.noTileCollide = true;

					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
					npc.velocity.Y += 0.1f;
					if (npc.velocity.Y > 12f)
						npc.velocity.Y = 12f;

					if (npc.timeLeft > 60)
                        npc.timeLeft = 60;

					if (npc.ai[0] != 1f)
					{
						npc.ai[0] = 1f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					return;
                }
            }
            else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			// Emit light when not Idle
			if (npc.ai[0] != 1f)
                Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 1.3f, 0.5f, 0f);

            // Fire projectiles while walking, teleporting, or falling
            if (npc.ai[0] == 2f || npc.ai[0] >= 5f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += npc.ai[0] == 2f ? 4f : shootTimer;

					float astralFlameGateValue = phase4 ? 30f : 60f;
					if (npc.localAI[0] >= astralFlameGateValue)
					{
						// Fire astral flames while teleporting
						if (npc.ai[0] >= 5f && npc.ai[0] != 7)
						{
							npc.localAI[0] = 0f;
							Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 109);

							float velocity = death ? (8f + npc.localAI[2] * 0.025f) : 7f;
							int type = ModContent.ProjectileType<AstralFlame>();
							int damage = npc.GetProjectileDamage(type);
							if (postMoonLordBuff)
								damage *= 2;

							float spreadLimit = (phase4 ? 100f : 50f) + enrageScale * 50f;
							float randomSpread = (Main.rand.NextFloat() - 0.5f) * spreadLimit;
							Vector2 spawnVector = new Vector2(npc.Center.X, npc.Center.Y - 80f);
							Vector2 destination = new Vector2(spawnVector.X + randomSpread, spawnVector.Y - 100f);
							Projectile.NewProjectile(npc.Center, Vector2.Normalize(destination - npc.Center) * velocity, type, damage, 0f, Main.myPlayer);
						}

						npc.netUpdate = true;
					}

                    if (npc.localAI[0] >= 180f)
                    {
                        // Fire astral lasers while walking
                        if (npc.ai[0] == 2f)
                        {
							npc.localAI[0] = 0f;
							Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 33);

							float num179 = death ? 20f : 18.5f;
							int maxProjectiles = malice ? 8 : death ? 6 : 4;
							int spread = malice ? 80 : death ? 60 : 45;

							if (!phase2)
							{
								maxProjectiles /= 2;
								spread /= 2;
							}

                            Vector2 value9 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                            float num180 = player.position.X + player.width * 0.5f - value9.X;
                            float num181 = Math.Abs(num180) * 0.1f;
                            float num182 = player.position.Y + player.height * 0.5f - value9.Y - num181;
							float num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                            num183 = num179 / num183;
                            num180 *= num183;
                            num182 *= num183;
							value9.X += num180;
							value9.Y += num182;

							int type = ModContent.ProjectileType<AstralLaser>();
							int damage = npc.GetProjectileDamage(type);
							if (postMoonLordBuff)
								damage *= 2;

                            for (int i = 0; i < maxProjectiles; i++)
                            {
                                num180 = player.position.X + player.width * 0.5f - value9.X;
                                num182 = player.position.Y + player.height * 0.5f - value9.Y;
                                num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
                                num183 = num179 / num183;
                                num180 += Main.rand.Next(-spread, spread + 1);
                                num182 += Main.rand.Next(-spread, spread + 1);
                                num180 *= num183;
                                num182 *= num183;
                                Projectile.NewProjectile(value9.X, value9.Y, num180, num182, type, damage, 0f, Main.myPlayer);
                            }

							npc.netUpdate = true;
                        }
                    }
                }
            }
			else
				npc.localAI[0] = 0f;

			// Start up
			if (npc.ai[0] == 0f)
            {
                npc.ai[0] = 1f;
                npc.netUpdate = true;
				CustomGravity();
			}

            // Idle
            else if (npc.ai[0] == 1f)
            {
                // Slow down
                npc.velocity.X *= 0.9f;

                // Stay vulnerable for a maximum of 1.5 or 2.5 seconds
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 120f || malice)
                {
					// Stop colliding with tiles
					npc.noTileCollide = true;

					// Set AI to next phase (Walk) and reset other AI
					npc.TargetClosest();
					npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
				else
					CustomGravity();
			}

            // Walk
            else if (npc.ai[0] == 2f)
            {
				// Set walking speed
                float num823 = 6f;
				num823 += 4f * enrageScale;
				if (expertMode)
					num823 += death ? 3f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
				if (revenge)
					num823 += Math.Abs(npc.Center.X - player.Center.X) * 0.0025f;

				// Set walking direction
				if (Math.Abs(npc.Center.X - player.Center.X) < 200f)
                {
                    npc.velocity.X *= 0.9f;
                    if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                        npc.velocity.X = 0f;
                }
                else
                {
                    float playerLocation = npc.Center.X - player.Center.X;
                    npc.direction = playerLocation < 0 ? 1 : -1;

                    if (npc.direction > 0)
                        npc.velocity.X = (npc.velocity.X * 20f + num823) / 21f;
                    if (npc.direction < 0)
                        npc.velocity.X = (npc.velocity.X * 20f - num823) / 21f;
                }

				if (Collision.CanHit(npc.position, npc.width, npc.height, player.Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height) && player.position.Y <= npc.position.Y + npc.height)
				{
					CustomGravity();
					npc.noTileCollide = false;
				}
				else
				{
					npc.noTileCollide = true;
					// Walk through tiles if colliding with tiles and player is out of reach
					int num854 = 80;
					int num855 = 20;
					Vector2 position2 = new Vector2(npc.Center.X - (num854 / 2), npc.position.Y + npc.height - num855);

					bool flag52 = false;
					if (npc.position.X < player.position.X && npc.position.X + npc.width > player.position.X + player.width && npc.position.Y + npc.height < player.position.Y + player.height - 16f)
						flag52 = true;

					if (flag52)
					{
						npc.velocity.Y += 0.5f;
					}
					else if (Collision.SolidCollision(position2, num854, num855))
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y = 0f;

						if (npc.velocity.Y > -0.2)
							npc.velocity.Y -= 0.025f;
						else
							npc.velocity.Y -= 0.2f;

						if (npc.velocity.Y < -4f)
							npc.velocity.Y = -4f;
					}
					else
					{
						if (npc.velocity.Y < 0f)
							npc.velocity.Y = 0f;

						if (npc.velocity.Y < 0.1)
							npc.velocity.Y += 0.025f;
						else
							npc.velocity.Y += 0.5f;
					}
				}

                // Walk for a maximum of 6 seconds
                npc.ai[1] += 1f;
                if (npc.ai[1] >= ((malice ? 270f : 360f) - (death ? 90f * (1f - lifeRatio) : 0f)))
                {
                    // Collide with tiles again
                    npc.noTileCollide = false;

					// Set AI to next phase (Jump) and reset other AI
					npc.TargetClosest();
					npc.ai[0] = 3f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }

                // Limit downward velocity
                if (npc.velocity.Y > 10f)
                    npc.velocity.Y = 10f;
            }

            // Jump
            else if (npc.ai[0] == 3f)
            {
				npc.noTileCollide = false;

                if (npc.velocity.Y == 0f)
                {
                    // Slow down
                    npc.velocity.X *= 0.8f;

                    // Half second delay before jumping
                    npc.ai[1] += 1f;
					if (npc.ai[1] >= 30f)
					{
						npc.ai[1] = -20f;
					}
					else if (npc.ai[1] == -1f)
					{
						// Set jump velocity, reset and set AI to next phase (Stomp)
						float velocityX = 9f;
						velocityX += 6f * enrageScale;
						if (expertMode)
							velocityX += death ? 4.5f * (1f - lifeRatio) : 3f * (1f - lifeRatio);

						float playerLocation = npc.Center.X - player.Center.X;
						npc.direction = playerLocation < 0 ? 1 : -1;

						npc.velocity.X = velocityX * npc.direction;

						float distanceBelowTarget = npc.position.Y - (player.position.Y + 80f);

						if (revenge)
						{
							float multiplier = 0.001f + 0.0005f * enrageScale;
							if (distanceBelowTarget > 0f)
								calamityGlobalNPC.newAI[0] = 1f + distanceBelowTarget * multiplier;

							float speedMultLimit = 2f + 1f * enrageScale;
							if (calamityGlobalNPC.newAI[0] > speedMultLimit)
								calamityGlobalNPC.newAI[0] = speedMultLimit;
						}

						if (expertMode)
						{
							if (player.position.Y < npc.Bottom.Y)
								npc.velocity.Y = (phase5 && (npc.ai[2] < 2f || npc.ai[2] == 3f)) ? -1f : -14.5f;
							else
								npc.velocity.Y = 1f;

							npc.noTileCollide = true;
						}
						else
							npc.velocity.Y = (phase5 && (npc.ai[2] < 2f || npc.ai[2] == 3f)) ? -1f : -14.5f;

						if (calamityGlobalNPC.newAI[0] > 1f)
							npc.velocity.Y *= calamityGlobalNPC.newAI[0];

						npc.ai[0] = 4f;
						npc.ai[1] = 0f;

						npc.netUpdate = true;
					}
                }

				CustomGravity();
			}

            // Stomp
            else if (npc.ai[0] == 4f)
            {
                if (npc.velocity.Y == 0f)
                {
                    // Play stomp sound
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LegStomp"), (int)npc.position.X, (int)npc.position.Y);

					// Stomp and jump again, if stomped twice then reset and set AI to next phase (Teleport or Idle)
					npc.TargetClosest();
					npc.ai[2] += 1f;
					float maxStompAmt = phase5 ? 5f : 3f;
					if (npc.ai[2] >= maxStompAmt)
					{
						npc.ai[0] = phase2 ? 5f : 1f;
						npc.ai[2] = 0f;
						npc.netUpdate = true;
					}
					else
					{
						float playerLocation = npc.Center.X - player.Center.X;
						npc.direction = playerLocation < 0 ? 1 : -1;

						npc.ai[0] = 3f;
						npc.netUpdate = true;
					}

					calamityGlobalNPC.newAI[0] = 0f;

					// Spawn dust for visual effect
					for (int num622 = (int)npc.position.X - 20; num622 < (int)npc.position.X + npc.width + 40; num622 += 20)
                    {
                        for (int num623 = 0; num623 < 4; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(npc.position.X - 20f, npc.position.Y + npc.height), npc.width + 20, 4, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                            Main.dust[num624].velocity *= 0.2f;
                        }
                    }

					// Fire lasers on stomp
					Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 33);
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						float num179 = death ? 20f : 18.5f;
						int maxProjectiles = malice ? 12 : death ? 6 : 4;
						int spread = malice ? 120 : death ? 60 : 45;
						if (phase5)
						{
							maxProjectiles = (int)(maxProjectiles * 1.5);
							spread = (int)Math.Round(spread * 1.5);
						}

						Vector2 value9 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
						float num180 = player.position.X + player.width * 0.5f - value9.X;
						float num181 = Math.Abs(num180) * 0.1f;
						float num182 = player.position.Y + player.height * 0.5f - value9.Y - num181;
						float num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
						num183 = num179 / num183;
						num180 *= num183;
						num182 *= num183;
						int type = ModContent.ProjectileType<AstralLaser>();
						int damage = npc.GetProjectileDamage(type);
						if (postMoonLordBuff)
							damage *= 2;

						value9.X += num180;
						value9.Y += num182;
						for (int i = 0; i < maxProjectiles; i++)
						{
							num180 = player.position.X + player.width * 0.5f - value9.X;
							num182 = player.position.Y + player.height * 0.5f - value9.Y;
							num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
							num183 = num179 / num183;
							num180 += Main.rand.Next(-spread, spread + 1);
							num182 += Main.rand.Next(-spread, spread + 1);
							num180 *= num183;
							num182 *= num183;
							Projectile.NewProjectile(value9.X, value9.Y, num180, num182, type, damage, 0f, Main.myPlayer, 0f, 0f);
						}

						npc.netUpdate = true;
					}
                }
                else
                {
					// Set velocities while falling, this happens before the stomp
					// Fall through
					if (!player.dead && expertMode)
					{
						if ((player.position.Y > npc.Bottom.Y && npc.velocity.Y > 0f) || (player.position.Y < npc.Bottom.Y && npc.velocity.Y < 0f))
							npc.noTileCollide = true;
						else if ((npc.velocity.Y > 0f && npc.Bottom.Y > Main.player[npc.target].Top.Y) || (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height)))
							npc.noTileCollide = false;
					}

					if (npc.position.X < player.position.X && npc.position.X + npc.width > player.position.X + player.width)
                    {
                        npc.velocity.X *= 0.9f;

						if (npc.Bottom.Y < player.position.Y)
						{
                            float fallSpeed = 1.2f;
							fallSpeed += 0.8f * enrageScale;
							if (expertMode)
								fallSpeed += death ? 0.6f * (1f - lifeRatio) : 0.4f * (1f - lifeRatio);

							if (calamityGlobalNPC.newAI[0] > 1f)
								fallSpeed *= calamityGlobalNPC.newAI[0];

							npc.velocity.Y += fallSpeed;
						}
                    }
                    else
                    {
						float velocityXChange = 0.2f + Math.Abs(npc.Center.X - player.Center.X) * 0.0001f;
						velocityXChange += 0.1f * enrageScale;

						if (npc.direction < 0)
                            npc.velocity.X -= velocityXChange;
                        else if (npc.direction > 0)
                            npc.velocity.X += velocityXChange;

                        float num626 = 12f;
						num626 += 8f * enrageScale;
						if (expertMode)
							num626 += death ? 4.5f * (1f - lifeRatio) : 3f * (1f - lifeRatio);

                        if (npc.velocity.X < -num626)
                            npc.velocity.X = -num626;
                        if (npc.velocity.X > num626)
                            npc.velocity.X = num626;
                    }

					CustomGravity();
				}
            }

            // Teleport
            else if (npc.ai[0] == 5f)
            {
				// Don't deal damage
				npc.damage = 0;

				// Slow down
				npc.velocity.X *= 0.9f;

                // Spawn slimes and start teleport
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[1] += 1f;
					if (death)
						npc.localAI[2] += 1f + 0.33f * (1f - lifeRatio);
					
					if (phase3)
					{
						npc.localAI[1] += 1f;
						if (death)
							npc.localAI[2] += 1f + 0.33f * (1f - lifeRatio);
					}

                    if (npc.localAI[1] >= ((malice ? 120f : 240f) - (death ? 60f * (1f - lifeRatio) : 0f)))
                    {
                        // Spawn slimes
                        bool spawnFlag = revenge;
                        if (NPC.CountNPCS(ModContent.NPCType<AureusSpawn>()) > 1)
                            spawnFlag = false;

						if (spawnFlag && Main.netMode != NetmodeID.MultiplayerClient)
						{
							NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 25, ModContent.NPCType<AureusSpawn>());

							if (death)
							{
								int damageAmt = npc.lifeMax / 100;
								npc.life -= damageAmt;
								if (npc.life < 1)
									npc.life = 1;

								npc.HealEffect(-damageAmt, true);
								npc.netUpdate = true;
							}
						}

						// Reset localAI and find a teleport destination
						npc.TargetClosest();
						npc.localAI[1] = 0f;
                        int num1249 = 0;
                        int num1250;
                        int num1251;

                        while (true)
                        {
                            num1249++;
                            num1250 = (int)player.Center.X / 16;
                            num1251 = (int)player.Center.Y / 16;
                            num1250 += Main.rand.Next(-30, 31);
                            num1251 += Main.rand.Next(-30, 31);

                            if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2(num1250 * 16, num1251 * 16), 1, 1, player.position, player.width, player.height))
                                break;

                            if (num1249 > 100)
                                goto Block;
                        }

                        // Set AI to next phase (Mid-teleport), set AI 2 and 3 to teleport coordinates X and Y respectively
                        npc.ai[0] = 6f;
                        npc.ai[2] = num1250;
                        npc.ai[3] = num1251;
                        npc.netUpdate = true;
                        Block:
                        ;
                    }
                }
            }

            // Mid-teleport
            else if (npc.ai[0] == 6f)
            {
                // Become immune
                npc.chaseable = false;
                npc.dontTakeDamage = true;

				if (death)
					npc.localAI[2] += 1f + 0.33f * (1f - lifeRatio);

				if (phase3)
				{
					if (death)
						npc.localAI[2] += 1f + 0.33f * (1f - lifeRatio);
				}

				// Turn invisible
				npc.alpha += 10;
                if (npc.alpha >= 255)
                {
                    // Set position to teleport destination
                    npc.position.X = npc.ai[2] * 16f - (npc.width / 2);
                    npc.position.Y = npc.ai[3] * 16f - (npc.height / 2);

                    // Reset alpha and set AI to next phase (End of teleport)
                    npc.alpha = 255;
                    npc.ai[0] = 7f;
					npc.localAI[2] = 0f;
                    npc.netUpdate = true;
                }

                // Play sound for cool effect
                if (npc.soundDelay == 0)
                {
                    npc.soundDelay = 15;
                    Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 109);
                }

                // Emit dust to make the teleport pretty
                for (int num245 = 0; num245 < 10; num245++)
                {
                    int num244 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), npc.velocity.X, npc.velocity.Y, 255, default, 2f);
                    Main.dust[num244].noGravity = true;
                    Main.dust[num244].velocity *= 0.5f;
                }
            }

            // End of teleport
            else if (npc.ai[0] == 7f)
            {
                // Turn visible
                npc.alpha -= 10;
                if (npc.alpha <= 0)
                {
                    // Spawn slimes
                    bool spawnFlag = phase4;
                    if (NPC.CountNPCS(ModContent.NPCType<AureusSpawn>()) > 1)
                        spawnFlag = false;

					if (spawnFlag && Main.netMode != NetmodeID.MultiplayerClient)
					{
						NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 25, ModContent.NPCType<AureusSpawn>());

						if (death)
						{
							int damageAmt = npc.lifeMax / 100;
							npc.life -= damageAmt;
							if (npc.life < 1)
								npc.life = 1;

							npc.HealEffect(-damageAmt, true);
							npc.netUpdate = true;
						}
					}

                    // Become vulnerable
                    npc.chaseable = true;
                    npc.dontTakeDamage = false;

                    // Reset alpha and set AI to next phase (Idle)
                    npc.alpha = 0;
                    npc.ai[0] = 1f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }

                // Play sound at teleport destination for cool effect
                if (npc.soundDelay == 0)
                {
                    npc.soundDelay = 15;
                    Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 109);
                }

                // Emit dust to make the teleport pretty
                for (int num245 = 0; num245 < 10; num245++)
                {
                    int num244 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), npc.velocity.X, npc.velocity.Y, 255, default, 2f);
                    Main.dust[num244].noGravity = true;
                    Main.dust[num244].velocity *= 0.5f;
                }
            }

			void CustomGravity()
			{
				float gravity = 0.36f + 0.12f * enrageScale;
				float maxFallSpeed = reduceFallSpeed ? 12f : 12f + 4f * enrageScale;

				if (calamityGlobalNPC.newAI[0] > 1f && !reduceFallSpeed)
					maxFallSpeed *= calamityGlobalNPC.newAI[0];

				npc.velocity.Y += gravity;
				if (npc.velocity.Y > maxFallSpeed)
					npc.velocity.Y = maxFallSpeed;
			}
        }
		#endregion

		#region Astrum Deus
		public static void AstrumDeusAI(NPC npc, Mod mod, bool head)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Difficulty variables
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			float enrageScale = BossRushEvent.BossRushActive ? 0.5f : 0f;
			if (Main.dayTime || malice)
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 1.5f;
			}

			// Deus cannot hit for 3 seconds or while invulnerable
			if (calamityGlobalNPC.newAI[1] < 180f || npc.dontTakeDamage)
				npc.damage = 0;
			else
				npc.damage = npc.defDamage;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			bool increaseSpeed = Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles;
			bool increaseSpeedMore = Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles;

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (increaseSpeedMore && head)
				npc.TargetClosest();

			// Inflict Extreme Gravity to nearby players
			if (revenge)
			{
				if (Main.netMode != NetmodeID.Server)
				{
					if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, npc.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
						Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<ExtremeGrav>(), 2);
				}
			}

			// Life
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Phases based on life percentage
			bool halfHealth = lifeRatio < 0.5f;
			bool doubleWormPhase = calamityGlobalNPC.newAI[0] != 0f;
			bool startFlightPhase = lifeRatio < 0.8f || death || doubleWormPhase;
			bool phase2 = lifeRatio < 0.5f && doubleWormPhase && expertMode;
			bool phase3 = lifeRatio < 0.2f && doubleWormPhase && expertMode;
			bool doubleLasersAndSplittingMines = lifeRatio < 0.7f;
			bool movingMines = lifeRatio < 0.3f && doubleWormPhase && expertMode;
			bool deathModeEnragePhase_Head = calamityGlobalNPC.newAI[0] == 3f;
			bool deathModeEnragePhase_BodyAndTail = false;

			// 5 seconds of resistance in phase 2, 10 seconds in phase 1, to prevent spawn killing
			float resistanceTime = doubleWormPhase ? 300f : 600f;
			if (calamityGlobalNPC.newAI[1] < resistanceTime)
				calamityGlobalNPC.newAI[1] += 1f;

			// Flight timer
			float aiSwitchTimer = doubleWormPhase ? 1200f : 1800f;
			calamityGlobalNPC.newAI[3] += 1f;
			if (calamityGlobalNPC.newAI[3] >= aiSwitchTimer)
				calamityGlobalNPC.newAI[3] = 0f;

			// Phase for flying at the player
			bool flyAtTarget = calamityGlobalNPC.newAI[3] >= (aiSwitchTimer * 0.5f) && startFlightPhase;

			// Length of worms
			int phase1Length = death ? 80 : revenge ? 70 : expertMode ? 60 : 50;
			int phase2Length = phase1Length / 2;
			int maxLength = doubleWormPhase ? phase2Length : phase1Length;

			// Split into two worms
			if (head)
			{
				float splitAnimationTime = 180f;
				bool oneWormAlive = NPC.CountNPCS(ModContent.NPCType<AstrumDeusHeadSpectral>()) < 2;
				bool deathModeFinalWormEnrage = death && doubleWormPhase && oneWormAlive && calamityGlobalNPC.newAI[1] >= resistanceTime;
				if (deathModeFinalWormEnrage)
				{
					if (calamityGlobalNPC.newAI[0] != 3f)
					{
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AstrumDeusSplit"), player.Center);
						calamityGlobalNPC.newAI[0] = 3f;
						npc.defense = 0;
						calamityGlobalNPC.DR = 0f;

						// Despawns the other deus worm segments
						int bodyID = ModContent.NPCType<AstrumDeusBodySpectral>();
						int tailID = ModContent.NPCType<AstrumDeusTailSpectral>();
						for (int i = 0; i < Main.maxNPCs; i++)
						{
							NPC wormseg = Main.npc[i];
							if (!wormseg.active)
								continue;

							if ((wormseg.type == bodyID || wormseg.type == tailID) && Main.npc[(int)wormseg.ai[2]].Calamity().newAI[0] != 3f)
							{
								wormseg.life = 0;
								wormseg.active = false;
							}
						}
					}

					calamityGlobalNPC.newAI[2] += 10f;
					if (calamityGlobalNPC.newAI[2] > 162f)
						calamityGlobalNPC.newAI[2] = 162f;

					npc.Opacity = MathHelper.Clamp(1f - (calamityGlobalNPC.newAI[2] / splitAnimationTime), 0f, 1f);
				}

				bool despawnRemainingWorm = doubleWormPhase && oneWormAlive && !death;
				if ((halfHealth && calamityGlobalNPC.newAI[0] == 0f) || despawnRemainingWorm)
				{
					npc.dontTakeDamage = true;

					calamityGlobalNPC.newAI[2] += despawnRemainingWorm ? 10f : 1f;
					npc.Opacity = MathHelper.Clamp(1f - (calamityGlobalNPC.newAI[2] / splitAnimationTime), 0f, 1f);

					bool despawning = calamityGlobalNPC.newAI[2] == splitAnimationTime;

					//
					// CODE TWEAKED BY: OZZATRON
					// September 20th, 2020
					// reason: fixing Astrum Deus death mode NPC cap bug
					//

					// Despawn the unsplit Astrum Deus and spawn two half-size Astrum Deus worms.
					if (despawning)
					{
						// If this is already a split worm (newAI[0] != 0f) then don't do anything. At all.
						if (doubleWormPhase)
							return;

						// Mark all existing body and tail segments as inactive. This instantly frees up their NPC slots for the freshly spawned worms.
						int bodyID = ModContent.NPCType<AstrumDeusBodySpectral>();
						int tailID = ModContent.NPCType<AstrumDeusTailSpectral>();
						for (int i = 0; i < Main.maxNPCs; i++)
						{
							NPC wormseg = Main.npc[i];
							if (!wormseg.active)
								continue;
							if (wormseg.type == bodyID || wormseg.type == tailID)
							{
								wormseg.life = 0;
								wormseg.active = false;
							}
						}

						// The unsplit Astrum Deus worm head will die next frame.
						npc.life = 0;

						// Do not spawn worms client side. The server handles this.
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							// Now that the original worm doesn't exist, startCount can be zero.
							// int startCount = npc.whoAmI + phase1Length + 1;
							int startIndexHeadOne = 1;
							int headOneID = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, npc.type, startIndexHeadOne);
							Main.npc[headOneID].Calamity().newAI[0] = 1f;
							Main.npc[headOneID].velocity = Vector2.Normalize(player.Center - Main.npc[headOneID].Center) * 16f;
							Main.npc[headOneID].timeLeft *= 20;
							Main.npc[headOneID].netUpdate = true;

							// On server, immediately send the correct extra AI of this head to clients.
							if (Main.netMode == NetmodeID.Server)
							{
								var netMessage = mod.GetPacket();
								netMessage.Write((byte)CalamityModMessageType.SyncCalamityNPCAIArray);
								netMessage.Write((byte)headOneID);
								netMessage.Write(Main.npc[headOneID].Calamity().newAI[0]);
								netMessage.Write(Main.npc[headOneID].Calamity().newAI[1]);
								netMessage.Write(Main.npc[headOneID].Calamity().newAI[2]);
								netMessage.Write(Main.npc[headOneID].Calamity().newAI[3]);
								netMessage.Send();
							}

							// Make sure the second split worm is also contiguous.
							int startIndexHeadTwo = startIndexHeadOne + phase2Length + 1;
							int headTwoID = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, npc.type, startIndexHeadTwo);
							Main.npc[headTwoID].Calamity().newAI[0] = 2f;
							Main.npc[headTwoID].Calamity().newAI[3] = 600f;
							Main.npc[headTwoID].velocity = Vector2.Normalize(player.Center - Main.npc[headTwoID].Center) * 16f;
							Main.npc[headTwoID].timeLeft *= 20;
							Main.npc[headTwoID].netUpdate = true;

							// On server, immediately send the correct extra AI of this head to clients.
							if (Main.netMode == NetmodeID.Server)
							{
								var netMessage = mod.GetPacket();
								netMessage.Write((byte)CalamityModMessageType.SyncCalamityNPCAIArray);
								netMessage.Write((byte)headTwoID);
								netMessage.Write(Main.npc[headTwoID].Calamity().newAI[0]);
								netMessage.Write(Main.npc[headTwoID].Calamity().newAI[1]);
								netMessage.Write(Main.npc[headTwoID].Calamity().newAI[2]);
								netMessage.Write(Main.npc[headTwoID].Calamity().newAI[3]);
								netMessage.Send();
							}

							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AstrumDeusSplit"), player.Center);
						}
						return;
					}
				}
			}

			// Copy dontTakeDamage and Opacity from head
			else
			{
				npc.dontTakeDamage = Main.npc[(int)npc.ai[2]].dontTakeDamage;
				npc.Opacity = Main.npc[(int)npc.ai[2]].Opacity;
				deathModeEnragePhase_BodyAndTail = Main.npc[(int)npc.ai[2]].Calamity().newAI[0] == 3f;

				if (deathModeEnragePhase_BodyAndTail)
				{
					npc.defense = 0;
					calamityGlobalNPC.DR = 0f;
				}
			}

			// Set worm variable
			if (npc.ai[2] > 0f)
				npc.realLife = (int)npc.ai[2];

			// Emit light
			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

			// Dust and alpha effects
			if ((head || Main.npc[(int)npc.ai[1]].alpha < 128) && !npc.dontTakeDamage)
			{
				// Alpha changes
				npc.alpha -= 42;
				if (npc.alpha < 0)
					npc.alpha = 0;
			}

			// Check if other segments are still alive, if not, die
			if (npc.type != ModContent.NPCType<AstrumDeusHeadSpectral>())
			{
				bool shouldDespawn = true;
				int headType = ModContent.NPCType<AstrumDeusHeadSpectral>();
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i].type != headType || !Main.npc[i].active)
						continue;
					shouldDespawn = false;
					break;
				}
				if (shouldDespawn)
				{
					if (Main.npc.IndexInRange((int)npc.ai[1]) && Main.npc[(int)npc.ai[1]].active && Main.npc[(int)npc.ai[1]].life > 0)
						shouldDespawn = false;
				}
				if (shouldDespawn)
				{
					npc.life = 0;
					npc.HitEffect(0, 10.0);
					npc.checkDead();
					npc.active = false;
					npc.netUpdate = true;
				}
			}

			// Direction
			if (npc.velocity.X < 0f)
				npc.spriteDirection = -1;
			else if (npc.velocity.X > 0f)
				npc.spriteDirection = 1;

			// Head code
			if (head)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					if (npc.ai[0] == 0f)
					{
						int Previous = npc.whoAmI;
						int bodyType = ModContent.NPCType<AstrumDeusBodySpectral>();
						int tailType = ModContent.NPCType<AstrumDeusTailSpectral>();
						for (int num36 = 0; num36 < maxLength; num36++)
						{
							int lol;
							if (num36 >= 0 && num36 < maxLength - 1)
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), bodyType, npc.whoAmI);
							else
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), tailType, npc.whoAmI);

							if (num36 % 2 == 0)
								Main.npc[lol].localAI[3] = 1f;

							Main.npc[lol].realLife = npc.whoAmI;
							Main.npc[lol].Calamity().newAI[0] = Main.npc[Previous].Calamity().newAI[0];
							Main.npc[lol].Calamity().newAI[3] = Main.npc[Previous].Calamity().newAI[3];
							if (Main.netMode == NetmodeID.Server)
							{
								var netMessage = mod.GetPacket();
								netMessage.Write((byte)CalamityModMessageType.SyncCalamityNPCAIArray);
								netMessage.Write((byte)lol);
								netMessage.Write(Main.npc[lol].Calamity().newAI[0]);
								netMessage.Write(Main.npc[lol].Calamity().newAI[1]);
								netMessage.Write(Main.npc[lol].Calamity().newAI[2]);
								netMessage.Write(Main.npc[lol].Calamity().newAI[3]);
								netMessage.Send();
							}
							Main.npc[lol].ai[2] = npc.whoAmI;
							Main.npc[lol].ai[1] = Previous;
							Main.npc[Previous].ai[0] = lol;
							NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
							Previous = lol;
						}
					}
				}
			}

			if (head)
			{
				int num12 = (int)(npc.position.X / 16f) - 1;
				int num13 = (int)((npc.position.X + npc.width) / 16f) + 2;
				int num14 = (int)(npc.position.Y / 16f) - 1;
				int num15 = (int)((npc.position.Y + npc.height) / 16f) + 2;

				if (num12 < 0)
					num12 = 0;
				if (num13 > Main.maxTilesX)
					num13 = Main.maxTilesX;
				if (num14 < 0)
					num14 = 0;
				if (num15 > Main.maxTilesY)
					num15 = Main.maxTilesY;

				// Fly or not
				bool flag2 = flyAtTarget;
				if (!flag2)
				{
					for (int k = num12; k < num13; k++)
					{
						for (int l = num14; l < num15; l++)
						{
							if (Main.tile[k, l] != null && ((Main.tile[k, l].nactive() && (Main.tileSolid[Main.tile[k, l].type] || (Main.tileSolidTop[Main.tile[k, l].type] && Main.tile[k, l].frameY == 0))) || Main.tile[k, l].liquid > 64))
							{
								Vector2 vector2;
								vector2.X = k * 16;
								vector2.Y = l * 16;
								if (npc.position.X + npc.width > vector2.X && npc.position.X < vector2.X + 16f && npc.position.Y + npc.height > vector2.Y && npc.position.Y < vector2.Y + 16f)
								{
									flag2 = true;
									break;
								}
							}
						}
					}
				}

				// Start flying if target is not within a certain distance
				if (!flag2)
				{
					npc.localAI[1] = 1f;

					if (head)
					{
						Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
						int num16 = 200;
						int heightReduction = death ? 130 : (int)(130f * (1f - lifeRatio));
						int height = 400 - heightReduction;
						bool flag3 = true;

						if (npc.position.Y > player.position.Y)
						{
							for (int m = 0; m < Main.maxPlayers; m++)
							{
								if (Main.player[m].active)
								{
									Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X - num16, (int)Main.player[m].position.Y - num16, num16 * 2, height);
									if (rectangle.Intersects(rectangle2))
									{
										flag3 = false;
										break;
									}
								}
							}
							if (flag3)
								flag2 = true;
						}
					}
				}
				else
					npc.localAI[1] = 0f;

				// Despawn
				float fallSpeed = deathModeEnragePhase_Head ? 22f : death ? 17.5f : 16f;
				if (player.dead)
				{
					flag2 = false;
					float velocity = 2f;
					npc.velocity.Y -= velocity;
					if ((double)npc.position.Y < Main.topWorld + 16f)
					{
						fallSpeed = deathModeEnragePhase_Head ? 44f : death ? 35f : 32f;
						npc.velocity.Y -= velocity;
					}

					int headType = ModContent.NPCType<AstrumDeusHeadSpectral>();
					int bodyType = ModContent.NPCType<AstrumDeusBodySpectral>();
					int tailType = ModContent.NPCType<AstrumDeusBodySpectral>();
					if ((double)npc.position.Y < Main.topWorld + 16f)
					{
						for (int num957 = 0; num957 < Main.maxNPCs; num957++)
						{
							if (Main.npc[num957].type == headType || Main.npc[num957].type == bodyType || Main.npc[num957].type == tailType)
							{
								Main.npc[num957].active = false;
								Main.npc[num957].netUpdate = true;
							}
						}
					}
				}

				float fallSpeedBoost = 5f * (1f - lifeRatio);
				fallSpeed += fallSpeedBoost;
				fallSpeed += 4f * enrageScale;

				// Speed and movement
				float speedBoost = death ? (0.1f * (1f - lifeRatio)) : (0.13f * (1f - lifeRatio));
				float turnSpeedBoost = death ? (0.18f * (1f - lifeRatio)) : (0.2f * (1f - lifeRatio));
				float speed = (deathModeEnragePhase_Head ? 0.23f : death ? 0.18f : 0.13f) + speedBoost;
				float turnSpeed = (deathModeEnragePhase_Head ? 0.3f : death ? 0.25f : 0.2f) + turnSpeedBoost;
				speed += 0.05f * enrageScale;
				turnSpeed += 0.08f * enrageScale;

				if (flyAtTarget)
				{
					float speedMultiplier = deathModeEnragePhase_Head ? 1.25f : doubleWormPhase ? (phase3 ? 1.25f : phase2 ? 1.2f : 1.15f) : 1f;
					speed *= speedMultiplier;
				}

				if (revenge)
				{
					float revMultiplier = 1.1f;
					speed *= revMultiplier;
					turnSpeed *= revMultiplier;
					fallSpeed *= revMultiplier;
				}

				speed *= increaseSpeedMore ? 4f : increaseSpeed ? 2f : 1f;
				turnSpeed *= increaseSpeedMore ? 4f : increaseSpeed ? 2f : 1f;

				Vector2 vector3 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
				float num20 = player.position.X + (player.width / 2);
				float num21 = player.position.Y + (player.height / 2);
				num20 = (int)(num20 / 16f) * 16;
				num21 = (int)(num21 / 16f) * 16;
				vector3.X = (int)(vector3.X / 16f) * 16;
				vector3.Y = (int)(vector3.Y / 16f) * 16;
				num20 -= vector3.X;
				num21 -= vector3.Y;

				if (!flag2)
				{
					npc.velocity.Y += 0.15f;
					if (npc.velocity.Y > fallSpeed)
						npc.velocity.Y = fallSpeed;

					if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * 0.4)
					{
						if (npc.velocity.X < 0f)
							npc.velocity.X -= speed * 1.1f;
						else
							npc.velocity.X += speed * 1.1f;
					}
					else if (npc.velocity.Y == fallSpeed)
					{
						if (npc.velocity.X < num20)
							npc.velocity.X += speed;
						else if (npc.velocity.X > num20)
							npc.velocity.X -= speed;
					}
					else if (npc.velocity.Y > 4f)
					{
						if (npc.velocity.X < 0f)
							npc.velocity.X += speed * 0.9f;
						else
							npc.velocity.X -= speed * 0.9f;
					}
				}
				else
				{
					float num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);
					float num25 = Math.Abs(num20);
					float num26 = Math.Abs(num21);
					float num27 = fallSpeed / num22;
					num20 *= num27;
					num21 *= num27;

					bool flag6 = false;
					if (flyAtTarget)
					{
						if (((npc.velocity.X > 0f && num20 < 0f) || (npc.velocity.X < 0f && num20 > 0f) || (npc.velocity.Y > 0f && num21 < 0f) || (npc.velocity.Y < 0f && num21 > 0f)) && Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) > speed / 2f && num22 < 400f)
						{
							flag6 = true;

							if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < fallSpeed)
								npc.velocity *= 1.1f;
						}

						if (npc.position.Y > player.position.Y)
						{
							flag6 = true;

							if (Math.Abs(npc.velocity.X) < fallSpeed / 2f)
							{
								if (npc.velocity.X == 0f)
									npc.velocity.X -= npc.direction;

								npc.velocity.X *= 1.1f;
							}
							else if (npc.velocity.Y > -fallSpeed)
								npc.velocity.Y -= speed;
						}
					}

					if (!flag6)
					{
						if (!flyAtTarget)
						{
							if (((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f)) && ((npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f)))
							{
								if (npc.velocity.X < num20)
									npc.velocity.X += turnSpeed;
								else if (npc.velocity.X > num20)
									npc.velocity.X -= turnSpeed;
								if (npc.velocity.Y < num21)
									npc.velocity.Y += turnSpeed;
								else if (npc.velocity.Y > num21)
									npc.velocity.Y -= turnSpeed;
							}
						}

						if ((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f) || (npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f))
						{
							if (npc.velocity.X < num20)
								npc.velocity.X += speed;
							else if (npc.velocity.X > num20)
								npc.velocity.X -= speed;
							if (npc.velocity.Y < num21)
								npc.velocity.Y += speed;
							else if (npc.velocity.Y > num21)
								npc.velocity.Y -= speed;

							if (Math.Abs(num21) < fallSpeed * 0.2 && ((npc.velocity.X > 0f && num20 < 0f) || (npc.velocity.X < 0f && num20 > 0f)))
							{
								if (npc.velocity.Y > 0f)
									npc.velocity.Y += speed * 2f;
								else
									npc.velocity.Y -= speed * 2f;
							}
							if (Math.Abs(num20) < fallSpeed * 0.2 && ((npc.velocity.Y > 0f && num21 < 0f) || (npc.velocity.Y < 0f && num21 > 0f)))
							{
								if (npc.velocity.X > 0f)
									npc.velocity.X += speed * 2f;
								else
									npc.velocity.X -= speed * 2f;
							}
						}
						else if (num25 > num26)
						{
							if (npc.velocity.X < num20)
								npc.velocity.X += speed * 1.1f;
							else if (npc.velocity.X > num20)
								npc.velocity.X -= speed * 1.1f;

							if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * 0.5)
							{
								if (npc.velocity.Y > 0f)
									npc.velocity.Y += speed;
								else
									npc.velocity.Y -= speed;
							}
						}
						else
						{
							if (npc.velocity.Y < num21)
								npc.velocity.Y += speed * 1.1f;
							else if (npc.velocity.Y > num21)
								npc.velocity.Y -= speed * 1.1f;

							if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * 0.5)
							{
								if (npc.velocity.X > 0f)
									npc.velocity.X += speed;
								else
									npc.velocity.X -= speed;
							}
						}
					}
				}

				if (flag2)
				{
					if (npc.localAI[0] != 1f)
						npc.netUpdate = true;

					npc.localAI[0] = 1f;
				}
				else
				{
					if (npc.localAI[0] != 0f)
						npc.netUpdate = true;

					npc.localAI[0] = 0f;
				}

				if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
					npc.netUpdate = true;

				npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;
			}

			// Only the main worm body and tail use this code
			else
			{
				// Shoot lasers
				if (npc.type == ModContent.NPCType<AstrumDeusBodySpectral>() && Main.netMode != NetmodeID.MultiplayerClient)
				{
					int shootTime = (doubleWormPhase && expertMode) ? 2 : 1;
					npc.localAI[0] += 1f;
					float shootProjectile = 400 / shootTime;
					float timer = npc.ai[0] + 15f;
					float divisor = timer + shootProjectile;
					if (!flyAtTarget || deathModeEnragePhase_BodyAndTail)
					{
						if (npc.localAI[0] % divisor == 0f && npc.ai[0] % 2f == 0f)
						{
							npc.TargetClosest();

							if (deathModeEnragePhase_BodyAndTail)
							{
								Vector2 velocity = Vector2.Zero;
								if (movingMines)
								{
									Vector2 vector15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
									vector15.Normalize();
									vector15 *= Main.rand.Next(90, 121) * 0.01f;
									velocity = vector15;
								}
								int type = ModContent.ProjectileType<DeusMine>();
								int damage = npc.GetProjectileDamage(type);
								float split = (doubleLasersAndSplittingMines && npc.ai[0] % 4f == 0f) ? 1f : 0f;
								Projectile.NewProjectile(npc.Center, velocity, type, damage, 0f, Main.myPlayer, split, 0f);
							}

							if (Vector2.Distance(player.Center, npc.Center) > 80f)
							{
								Main.PlaySound((phase2 || deathModeEnragePhase_BodyAndTail) ? SoundID.Item91 : SoundID.Item12, npc.Center);
								float num941 = (death ? 16f : revenge ? 14f : 13f) + enrageScale * 4f;
								Vector2 vector104 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + (npc.height / 2));
								float num942 = player.position.X + player.width * 0.5f - vector104.X;
								float num943 = player.position.Y + player.height * 0.5f - vector104.Y;
								float num944 = (float)Math.Sqrt(num942 * num942 + num943 * num943);
								num944 = num941 / num944;
								num942 *= num944;
								num943 *= num944;
								vector104.X += num942 * 5f;
								vector104.Y += num943 * 5f;
								Vector2 shootDirection = new Vector2(num942, num943).SafeNormalize(Vector2.UnitY);
								Vector2 laserVelocity = shootDirection * num941;
								int type = (phase2 || deathModeEnragePhase_BodyAndTail) ? ModContent.ProjectileType<AstralGodRay>() : ModContent.ProjectileType<AstralShot2>();
								int damage = npc.GetProjectileDamage(type);
								if (phase2 || deathModeEnragePhase_BodyAndTail)
								{
									// Waving beams need to start offset so they cross each other neatly.
									float waveSideOffset = Main.rand.NextFloat(9f, 14f);
									Vector2 perp = shootDirection.RotatedBy(-MathHelper.PiOver2) * waveSideOffset;

									for (int i = -1; i <= 1; i += 2)
									{
										Vector2 laserStartPos = vector104 + i * perp + Main.rand.NextVector2CircularEdge(6f, 6f);
										Projectile godRay = Projectile.NewProjectileDirect(laserStartPos, laserVelocity * 0.9f, type, damage, 0f, Main.myPlayer, player.Center.X, player.Center.Y);

										// Tell this Phased God Ray exactly which way it should be waving.
										godRay.localAI[1] = i * 0.5f;
									}
								}
								else
								{
									Projectile.NewProjectile(vector104, laserVelocity, type, damage, 0f, Main.myPlayer, player.Center.X, player.Center.Y);
									if (doubleLasersAndSplittingMines && npc.ai[0] % 4f == 0f)
										Projectile.NewProjectile(vector104, laserVelocity * 0.9f, type, damage, 0f, Main.myPlayer, player.Center.X, player.Center.Y);
								}
							}
						}
					}
					else
					{
						if (npc.localAI[0] % divisor == 0f && npc.ai[0] % 2f == 0f)
						{
							Vector2 velocity = Vector2.Zero;
							if (movingMines)
							{
								Vector2 vector15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
								vector15.Normalize();
								vector15 *= Main.rand.Next(30, 121) * 0.01f;
								velocity = vector15;
							}
							int type = ModContent.ProjectileType<DeusMine>();
							int damage = npc.GetProjectileDamage(type);
							float split = (doubleLasersAndSplittingMines && npc.ai[0] % 4f == 0f) ? 1f : 0f;
							Projectile.NewProjectile(npc.Center, velocity, type, damage, 0f, Main.myPlayer, split, 0f);
						}
					}
				}

				// Follow the head
				Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
				float num191 = player.position.X + (player.width / 2);
				float num192 = player.position.Y + (player.height / 2);
				num191 = (int)(num191 / 16f) * 16;
				num192 = (int)(num192 / 16f) * 16;
				vector18.X = (int)(vector18.X / 16f) * 16;
				vector18.Y = (int)(vector18.Y / 16f) * 16;
				num191 -= vector18.X;
				num192 -= vector18.Y;

				if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
				{
					try
					{
						vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
						num191 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
						num192 = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
					}
					catch
					{
					}

					npc.rotation = (float)Math.Atan2(num192, num191) + MathHelper.PiOver2;
					float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
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
		}
		#endregion

		#region Ceaseless Void
		public static void CeaselessVoidAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			CalamityGlobalNPC.voidBoss = npc.whoAmI;

			Vector2 vector = npc.Center;

			// Percent life remaining
			double lifeRatio = npc.life / (double)npc.lifeMax;

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			// Phases
			bool phase2 = lifeRatio <= 0.7;
			bool phase3 = lifeRatio <= 0.4;
			bool phase4 = lifeRatio <= 0.1;
			bool theBigSucc = npc.life / (double)npc.lifeMax <= 0.1;
			bool succSoHardThatYouDie = npc.life / (double)npc.lifeMax <= 0.005;

			// Spawn Dark Energies
			int darkEnergyAmt = death ? 6 : revenge ? 5 : expertMode ? 4 : 3;
			if (phase2)
				darkEnergyAmt += 1;
			if (phase3)
				darkEnergyAmt += 1;
			if (phase4)
				darkEnergyAmt += 1;

			// Spawn a few Dark Energies as soon as the fight starts
			int spacing = 360 / darkEnergyAmt;
			int distance2 = 10;
			if (npc.ai[2] == 0f)
			{
				npc.ai[2] = 1f;
				for (int i = 0; i < darkEnergyAmt; i++)
				{
					NPC.NewNPC((int)(vector.X + (Math.Sin(i * spacing) * distance2)), (int)(vector.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 0f, 0f, 0f);
					NPC.NewNPC((int)(vector.X + (Math.Sin(i * spacing) * distance2)), (int)(vector.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 1f, 0f, 0f);
					NPC.NewNPC((int)(vector.X + (Math.Sin(i * spacing) * distance2)), (int)(vector.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 2f, 0f, 0f);
				}
				NPC.NewNPC((int)(vector.X + (Math.Sin(0) * distance2)), (int)(vector.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 0.5f, 0f, 0f);
				NPC.NewNPC((int)(vector.X + (Math.Sin(0) * distance2)), (int)(vector.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 1.5f, 0f, 0f);
			}

			// If there are any Dark Energies alive, change AI and don't take damage
			bool anyDarkEnergies = NPC.AnyNPCs(ModContent.NPCType<DarkEnergy>());
			bool movingDuringSuccPhase = npc.ai[3] == 0f;
			npc.dontTakeDamage = anyDarkEnergies || theBigSucc || movingDuringSuccPhase;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, vector) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			// Speed enrage
			bool moveVeryFast = Vector2.Distance(npc.Center, player.Center) > 960f || (!player.ZoneDungeon && !BossRushEvent.BossRushActive && player.position.Y < Main.worldSurface * 16.0);

			// Despawn
			if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
				{
					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft < 10)
					{
						CalamityWorld.DoGSecondStageCountdown = 0;
						if (Main.netMode == NetmodeID.Server)
						{
							var netMessage = mod.GetPacket();
							netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
							netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
							netMessage.Send();
						}
					}

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			// Scale multiplier based on nearby active tiles
			float tileEnrageMult = BossRushEvent.BossRushActive ? 1.375f : malice  ? 1.25f : 1f;
			npc.Calamity().CurrentlyEnraged = tileEnrageMult > 1f && !BossRushEvent.BossRushActive;

			// Set AI variable to be used by Dark Energies
			npc.ai[1] = tileEnrageMult;

			// Increase projectile fire rate based on number of nearby active tiles
			float projectileFireRateMultiplier = MathHelper.Lerp(0.5f, 1.5f, 1f - ((tileEnrageMult - 1f) / 0.5f));

			// Succ attack
			if (!anyDarkEnergies)
			{
				// This is here because it's used in multiple places
				float suckDistance = malice ? 1920f : death ? 1600f : revenge ? 1440f : expertMode ? 1280f : 1040f;

				// Move closer to the target before trying to succ
				if (movingDuringSuccPhase)
				{
					if (Vector2.Distance(vector, player.Center) > 320f || !Collision.CanHit(vector, 1, 1, player.Center, 1, 1))
						Movement(true);
					else
						npc.ai[3] = 1f;
				}
				else
				{
					// Use this to generate more and more dust in final phase
					float finalPhaseDustRatio = 1f;
					if (succSoHardThatYouDie)
					{
						finalPhaseDustRatio = 5f;
					}
					else if (theBigSucc)
					{
						float amount = (10f - (float)(npc.life / (double)npc.lifeMax) * 100f) / 10f;
						finalPhaseDustRatio += MathHelper.Lerp(0f, 2f, amount);
					}

					// Slow down
					if (npc.velocity.Length() > 0.5f)
						npc.velocity *= 0.8f;
					else
						npc.velocity = Vector2.Zero;

					// Move towards target again if they get outside the succ radius
					float moveCloserGateValue = suckDistance * 0.75f;
					if (Vector2.Distance(vector, player.Center) > moveCloserGateValue)
						npc.ai[3] = 0f;

					// Ceaseless Void sucks in dark energy in different patterns
					// This attack also sucks in all players that are within reach of the succ
					int dustRings = 3;
					for (int h = 0; h < dustRings; h++)
					{
						float distanceDivisor = h + 1f;
						float dustDistance = suckDistance / distanceDivisor;
						int numDust = (int)(0.1f * MathHelper.TwoPi * dustDistance);
						float angleIncrement = MathHelper.TwoPi / (float)numDust;
						Vector2 dustOffset = new Vector2(dustDistance, 0f);
						dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);

						int var = (int)(dustDistance / finalPhaseDustRatio);
						float dustVelocity = 24f / distanceDivisor * finalPhaseDustRatio;
						for (int i = 0; i < numDust; i++)
						{
							if (Main.rand.NextBool(var))
							{
								dustOffset = dustOffset.RotatedBy(angleIncrement);
								int dust = Dust.NewDust(vector, 1, 1, ModContent.DustType<CeaselessDust>());
								Main.dust[dust].position = vector + dustOffset;
								Main.dust[dust].fadeIn = 1f;
								Main.dust[dust].velocity = Vector2.Normalize(vector - Main.dust[dust].position) * dustVelocity;
								Main.dust[dust].scale = 3f - h;
							}
						}
					}

					float succPower = 0.1f + finalPhaseDustRatio * 0.1f;
					for (int i = 0; i < Main.maxPlayers; i++)
					{
						float distance = Vector2.Distance(Main.player[i].Center, vector);
						if (distance < suckDistance && Main.player[i].grappling[0] == -1)
						{
							if (Collision.CanHit(vector, 1, 1, Main.player[i].Center, 1, 1))
							{
								float distanceRatio = distance / suckDistance;
								float multiplier = 1f - distanceRatio;

								if (Main.player[i].Center.X < vector.X)
									Main.player[i].velocity.X += succPower * multiplier;
								else
									Main.player[i].velocity.X -= succPower * multiplier;

								if (Main.player[i].Center.Y < vector.Y)
									Main.player[i].velocity.Y += succPower * multiplier;
								else
									Main.player[i].velocity.Y -= succPower * multiplier;
							}
						}
					}

					// Projectiles
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						// Slowly die in final phase and then implode
						// This phase lasts 20 seconds
						if (theBigSucc && calamityGlobalNPC.newAI[1] % 60f == 0f)
						{
							int damageAmt = npc.lifeMax / 200;
							npc.life -= damageAmt;
							npc.HealEffect(-damageAmt, true);

							if (npc.life <= 0)
							{
								npc.life = 0;
								npc.HitEffect();
								npc.checkDead();
							}

							npc.netUpdate = true;
						}

						// Beam Portals
						if (calamityGlobalNPC.newAI[1] == 0f)
						{
							int numBeamPortals = malice ? 3 : revenge ? 2 : 1;
							float degrees = 360 / numBeamPortals;
							float beamPortalDistance = malice ? 800f : death ? 960f : revenge ? 1000f : expertMode ? 1040f : 1080f;
							int type = ModContent.ProjectileType<DoGBeamPortal>();
							int damage = npc.GetProjectileDamage(type);
							for (int i = 0; i < numBeamPortals; i++)
							{
								float ai1 = i * degrees;
								Projectile.NewProjectile(player.Center.X + (float)(Math.Sin(i * degrees) * beamPortalDistance), player.Center.Y + (float)(Math.Cos(i * degrees) * beamPortalDistance), 0f, 0f, type, damage, 0f, player.whoAmI, ai1, 0f);
							}
						}

						// Use this timer to lessen Dark Energy projectile rate of fire while Beam Portals are active
						float beamPortalTimeLeft = 600f;
						bool summonLessDarkEnergies = false;
						if (calamityGlobalNPC.newAI[1] < beamPortalTimeLeft)
						{
							calamityGlobalNPC.newAI[1] += 1f;
							summonLessDarkEnergies = true;
						}
						else if (theBigSucc)
							calamityGlobalNPC.newAI[1] += 1f;

						// Suck in Dark Energy projectiles from far away
						calamityGlobalNPC.newAI[3] += 1f;
						float darkEnergySpiralGateValue = (summonLessDarkEnergies ? 36f : 12f) * projectileFireRateMultiplier;
						if (calamityGlobalNPC.newAI[3] >= darkEnergySpiralGateValue)
						{
							calamityGlobalNPC.newAI[3] = 0f;
							int type = ModContent.ProjectileType<DarkEnergyBall>();
							int damage = npc.GetProjectileDamage(type);
							bool normalSpread = npc.localAI[0] % 2f == 0f;
							float speed = 0.5f;
							int totalProjectiles = 4;
							float radians = MathHelper.TwoPi / totalProjectiles;
							double angleA = radians * 0.5;
							double angleB = MathHelper.ToRadians(90f) - angleA;
							float velocityX = (float)(speed * Math.Sin(angleA) / Math.Sin(angleB));
							Vector2 spinningPoint = normalSpread ? new Vector2(0f, -speed) : new Vector2(-velocityX, -speed);
							float radialOffset = MathHelper.ToRadians(npc.localAI[1]);
							for (int i = 0; i < totalProjectiles; i++)
							{
								Vector2 spawnVector = vector + Vector2.Normalize(spinningPoint.RotatedBy(radians * i + radialOffset)) * suckDistance;
								Vector2 velocity = Vector2.Normalize(vector - spawnVector) * speed;
								Projectile.NewProjectile(spawnVector, velocity, type, damage, 0f, Main.myPlayer);
							}
							npc.localAI[1] += 10f;
						}

						// Summon some extra projectiles in Expert Mode
						if (phase2 && expertMode && !summonLessDarkEnergies)
						{
							npc.localAI[2] += 1f;
							if (npc.localAI[2] >= 60f * projectileFireRateMultiplier)
							{
								npc.localAI[2] = 0f;
								int type = ModContent.ProjectileType<DarkEnergyBall2>();
								int damage = npc.GetProjectileDamage(type);
								bool normalSpread = npc.localAI[0] % 2f != 0f;
								float speed = 2f;
								int totalProjectiles = 2;
								float radians = MathHelper.TwoPi / totalProjectiles;
								double angleA = radians * 0.5;
								double angleB = MathHelper.ToRadians(90f) - angleA;
								float velocityX = (float)(speed * Math.Sin(angleA) / Math.Sin(angleB));
								Vector2 spinningPoint = normalSpread ? new Vector2(0f, -speed) : new Vector2(-velocityX, -speed);
								float radialOffset = MathHelper.ToRadians(npc.localAI[1] * 0.25f);
								for (int i = 0; i < totalProjectiles; i++)
								{
									Vector2 spawnVector = vector + Vector2.Normalize(spinningPoint.RotatedBy(radians * i + radialOffset)) * suckDistance;
									Vector2 velocity = Vector2.Normalize(vector - spawnVector) * speed;
									Projectile.NewProjectile(spawnVector, velocity, type, damage, 0f, Main.myPlayer);
								}
							}
						}

						// Summon some extra projectiles in Revengeance Mode
						if (phase4 && revenge && !summonLessDarkEnergies)
						{
							npc.localAI[3] += 1f;
							if (npc.localAI[3] >= 90f * projectileFireRateMultiplier)
							{
								npc.localAI[3] = 0f;
								int type = ModContent.ProjectileType<DarkEnergyBall2>();
								int damage = npc.GetProjectileDamage(type);
								bool normalSpread = npc.localAI[0] % 2f == 0f;
								float speed = 4f;
								int totalProjectiles = 2;
								float radians = MathHelper.TwoPi / totalProjectiles;
								double angleA = radians * 0.5;
								double angleB = MathHelper.ToRadians(90f) - angleA;
								float velocityX = (float)(speed * Math.Sin(angleA) / Math.Sin(angleB));
								Vector2 spinningPoint = normalSpread ? new Vector2(0f, -speed) : new Vector2(-velocityX, -speed);
								float radialOffset = MathHelper.ToRadians(npc.localAI[1] * 0.25f);
								for (int i = 0; i < totalProjectiles; i++)
								{
									Vector2 spawnVector = vector + Vector2.Normalize(spinningPoint.RotatedBy(radians * i + radialOffset)) * suckDistance;
									Vector2 velocity = Vector2.Normalize(vector - spawnVector) * speed;
									Projectile.NewProjectile(spawnVector, velocity, type, damage, 0f, Main.myPlayer);
								}
							}
						}
					}
				}
			}
			else
			{
				Movement(false);

				// Count up all Dark Energy HP values
				int totalDarkEnergyHP = 0;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC darkEnergy = Main.npc[i];
					if (darkEnergy.active && darkEnergy.type == ModContent.NPCType<DarkEnergy>())
						totalDarkEnergyHP += darkEnergy.life;
				}

				// Destroy all Dark Energies if their total HP is below 20%
				int darkEnergyMaxHP = BossRushEvent.BossRushActive ? 44000 : (CalamityWorld.DoGSecondStageCountdown <= 0 || !CalamityWorld.downedSentinel1) ? 12000 : 3000;
				int totalDarkEnergiesSpawned = darkEnergyAmt * 3 + 2;
				int totalDarkEnergyMaxHP = darkEnergyMaxHP * totalDarkEnergiesSpawned;
				int succPhaseGateValue = (int)(totalDarkEnergyMaxHP * 0.2);
				if (totalDarkEnergyHP < succPhaseGateValue)
				{
					Main.PlaySound(SoundID.NPCDeath44, vector);

					// Kill all Dark Energies
					for (int i = 0; i < Main.maxNPCs; i++)
					{
						NPC darkEnergy = Main.npc[i];
						if (darkEnergy.active && darkEnergy.type == ModContent.NPCType<DarkEnergy>())
						{
							darkEnergy.HitEffect();
							darkEnergy.active = false;
							darkEnergy.netUpdate = true;
						}
					}

					// Generate a dust explosion
					int dustAmt = 30;
					int random = 3;
					for (int j = 0; j < 10; j++)
					{
						random += j * 2;
						int dustAmtSpawned = 0;
						int scale = random * 13;
						float dustPositionX = vector.X - (scale / 2);
						float dustPositionY = vector.Y - (scale / 2);
						while (dustAmtSpawned < dustAmt)
						{
							float dustVelocityX = Main.rand.Next(-random, random);
							float dustVelocityY = Main.rand.Next(-random, random);
							float dustVelocityScalar = random * 2f;
							float dustVelocity = (float)Math.Sqrt(dustVelocityX * dustVelocityX + dustVelocityY * dustVelocityY);
							dustVelocity = dustVelocityScalar / dustVelocity;
							dustVelocityX *= dustVelocity;
							dustVelocityY *= dustVelocity;
							int dust = Dust.NewDust(new Vector2(dustPositionX, dustPositionY), scale, scale, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 5f);
							Main.dust[dust].noGravity = true;
							Main.dust[dust].position.X = vector.X;
							Main.dust[dust].position.Y = vector.Y;
							Main.dust[dust].position.X += Main.rand.Next(-10, 11);
							Main.dust[dust].position.Y += Main.rand.Next(-10, 11);
							Main.dust[dust].velocity.X = dustVelocityX;
							Main.dust[dust].velocity.Y = dustVelocityY;
							dustAmtSpawned++;
						}
					}
				}
			}

			// Basic movement towards a location
			void Movement(bool succ)
			{
				float velocity = moveVeryFast ? 25f : BossRushEvent.BossRushActive ? 15f : ((expertMode ? 7.5f : 6f) + (float)(death ? 2f * (1D - lifeRatio) : 0f)) * tileEnrageMult;
				float acceleration = moveVeryFast ? 0.75f : malice ? 0.3f : death ? 0.2f : expertMode ? 0.16f : 0.12f;

				// Increase speed dramatically in succ phase
				if (succ)
				{
					velocity *= 2f;
					acceleration *= 2f;
				}

				Vector2 moveTo = new Vector2(player.Center.X - vector.X, player.Center.Y - vector.Y);

				// Move between 8 different positions around the player, in order
				float maxDistance = 320f;
				Vector2 moveToOffset = succ ? Vector2.Zero : new Vector2(0f, -maxDistance);
				if (!succ)
				{
					// Move to a new location every few seconds
					calamityGlobalNPC.newAI[2] += 1f;
					float newPositionGateValue = malice ? 180f : death ? 270f : revenge ? 300f : expertMode ? 360f : 480f;
					if (calamityGlobalNPC.newAI[2] > newPositionGateValue)
					{
						calamityGlobalNPC.newAI[2] = 0f;

						npc.ai[0] += 1f;
						if (npc.ai[0] > 7f)
							npc.ai[0] = 0f;
					}

					switch ((int)npc.ai[0])
					{
						case 0:
							break;
						case 1:
							moveToOffset.X = -maxDistance;
							break;
						case 2:
							moveToOffset.X = -maxDistance;
							moveToOffset.Y = 0f;
							break;
						case 3:
							moveToOffset.X = -maxDistance;
							moveToOffset.Y = maxDistance;
							break;
						case 4:
							moveToOffset.Y = maxDistance;
							break;
						case 5:
							moveToOffset.X = maxDistance;
							moveToOffset.Y = maxDistance;
							break;
						case 6:
							moveToOffset.X = maxDistance;
							moveToOffset.Y = 0f;
							break;
						case 7:
							moveToOffset.X = maxDistance;
							break;
					}
				}

				moveTo += moveToOffset;

				float distance = (float)Math.Sqrt(moveTo.X * moveTo.X + moveTo.Y * moveTo.Y);

				distance = velocity / distance;
				moveTo.X *= distance;
				moveTo.Y *= distance;

				if (npc.velocity.X < moveTo.X)
				{
					npc.velocity.X += acceleration;
					if (npc.velocity.X < 0f && moveTo.X > 0f)
						npc.velocity.X += acceleration;
				}
				else if (npc.velocity.X > moveTo.X)
				{
					npc.velocity.X -= acceleration;
					if (npc.velocity.X > 0f && moveTo.X < 0f)
						npc.velocity.X -= acceleration;
				}
				if (npc.velocity.Y < moveTo.Y)
				{
					npc.velocity.Y += acceleration;
					if (npc.velocity.Y < 0f && moveTo.Y > 0f)
						npc.velocity.Y += acceleration;
				}
				else if (npc.velocity.Y > moveTo.Y)
				{
					npc.velocity.Y -= acceleration;
					if (npc.velocity.Y > 0f && moveTo.Y < 0f)
						npc.velocity.Y -= acceleration;
				}
			}

			// Spawn more Dark Energies as the fight progresses
			if (calamityGlobalNPC.newAI[0] == 0f && npc.life > 0)
				calamityGlobalNPC.newAI[0] = npc.lifeMax;

			if (npc.life > 0)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int healthGateValue = (int)(npc.lifeMax * 0.3);
					if ((npc.life + healthGateValue) < calamityGlobalNPC.newAI[0])
					{
						npc.TargetClosest();
						calamityGlobalNPC.newAI[0] = npc.life;
						calamityGlobalNPC.newAI[1] = 0f;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						npc.ai[3] = 0f;
						npc.localAI[0] += 1f;
						npc.localAI[1] = 0f;
						npc.localAI[2] = 0f;
						npc.localAI[3] = 0f;

						if (phase4)
						{
							for (int i = 0; i < darkEnergyAmt; i++)
							{
								NPC.NewNPC((int)(vector.X + (Math.Sin(i * spacing) * distance2)), (int)(vector.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 0f, 0f, 0f);
								NPC.NewNPC((int)(vector.X + (Math.Sin(i * spacing) * distance2)), (int)(vector.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 2f, 0f, 0f);
								NPC.NewNPC((int)(vector.X + (Math.Sin(i * spacing) * distance2)), (int)(vector.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 4f, 0f, 0f);
							}
							NPC.NewNPC((int)(vector.X + (Math.Sin(0) * distance2)), (int)(vector.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 1f, 0f, 0f);
							NPC.NewNPC((int)(vector.X + (Math.Sin(0) * distance2)), (int)(vector.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 3f, 0f, 0f);
						}
						else if (phase3)
						{
							for (int i = 0; i < darkEnergyAmt; i++)
							{
								NPC.NewNPC((int)(vector.X + (Math.Sin(i * spacing) * distance2)), (int)(vector.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 0f, 0f, 0f);
								NPC.NewNPC((int)(vector.X + (Math.Sin(i * spacing) * distance2)), (int)(vector.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 1.5f, 0f, 0f);
								NPC.NewNPC((int)(vector.X + (Math.Sin(i * spacing) * distance2)), (int)(vector.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 3f, 0f, 0f);
							}
							NPC.NewNPC((int)(vector.X + (Math.Sin(0) * distance2)), (int)(vector.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 0.5f, 0f, 0f);
							NPC.NewNPC((int)(vector.X + (Math.Sin(0) * distance2)), (int)(vector.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 2f, 0f, 0f);
						}
						else
						{
							for (int i = 0; i < darkEnergyAmt; i++)
							{
								NPC.NewNPC((int)(vector.X + (Math.Sin(i * spacing) * distance2)), (int)(vector.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 0f, 0f, 0f);
								NPC.NewNPC((int)(vector.X + (Math.Sin(i * spacing) * distance2)), (int)(vector.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 1f, 0f, 0f);
								NPC.NewNPC((int)(vector.X + (Math.Sin(i * spacing) * distance2)), (int)(vector.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, i * spacing, 2f, 0f, 0f);
							}
							NPC.NewNPC((int)(vector.X + (Math.Sin(0) * distance2)), (int)(vector.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 1.5f, 0f, 0f);
							NPC.NewNPC((int)(vector.X + (Math.Sin(0) * distance2)), (int)(vector.Y + (Math.Cos(0) * distance2)), ModContent.NPCType<DarkEnergy>(), npc.whoAmI, 0f, 2.5f, 0f, 0f);
						}

						// Despawn potentially hazardous projectiles when entering a new phase
						for (int i = 0; i < Main.maxProjectiles; i++)
						{
							Projectile projectile = Main.projectile[i];
							if (projectile.active)
							{
								if (projectile.type == ModContent.ProjectileType<DoGBeamPortal>() || projectile.type == ModContent.ProjectileType<DoGBeam>() ||
									projectile.type == ModContent.ProjectileType<DarkEnergyBall>() || projectile.type == ModContent.ProjectileType<DarkEnergyBall2>())
								{
									if (projectile.timeLeft > 30)
										projectile.timeLeft = 30;
								}
							}
						}
					}
				}
			}
		}
		#endregion

		#region Bumblebirb
		public static void BumblebirbAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			// Variables
			float rotationMult = 3f;
			float rotationAmt = 0.03f;
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			Vector2 vector = npc.Center;

			// Adjust slowing debuff immunity
			bool immuneToSlowingDebuffs = npc.ai[0] == 3f || npc.ai[0] == 3.1f || npc.ai[0] == 3.2f;
			npc.buffImmune[ModContent.BuffType<ExoFreeze>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<KamiDebuff>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<TimeSlow>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<TeslaFreeze>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
			npc.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
			npc.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

			// If target is outside the jungle for more than 5 seconds, enrage
			if (!player.ZoneJungle)
			{
				if (npc.localAI[1] < CalamityGlobalNPC.biomeEnrageTimerMax)
					npc.localAI[1] += 1f;
			}
			else
				npc.localAI[1] = 0f;

			// If dragonfolly is off screen, enrage for the next couple attacks
			if (Vector2.Distance(player.Center, vector) > 1200f)
				npc.localAI[2] = 2f;

			// Enrage scale
			float enrageScale = death ? 1.5f : 1f;
			if (npc.localAI[1] >= CalamityGlobalNPC.biomeEnrageTimerMax || malice)
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 1f;
			}
			if (npc.localAI[2] > 0f || malice)
				enrageScale += 1f;

			if (enrageScale > 3f)
				enrageScale = 3f;

			// Despawn
			if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];

				if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
				{
					npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.2f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.ai[0] != 0f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.netUpdate = true;
					}

					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Phases
			bool phase2 = lifeRatio < (revenge ? 0.75f : 0.5f);
			bool phase3 = lifeRatio < (death ? 0.4f : revenge ? 0.25f : 0.1f) && expertMode;

			float birbSpawnPhaseTimer = 180f;
			float newPhaseTimer = 180f;
			bool phaseSwitchPhase = (phase2 && calamityGlobalNPC.newAI[0] < newPhaseTimer && calamityGlobalNPC.newAI[2] != 1f) ||
				(phase3 && calamityGlobalNPC.newAI[1] < newPhaseTimer && calamityGlobalNPC.newAI[3] != 1f);

			npc.dontTakeDamage = phaseSwitchPhase;

			calamityGlobalNPC.DR = (npc.ai[0] == 5f || (enrageScale == 3f && !malice)) ? 0.75f : 0.1f;

			if (phaseSwitchPhase)
			{
				if (npc.velocity.X < 0f)
					npc.direction = -1;
				else if (npc.velocity.X > 0f)
					npc.direction = 1;

				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

				if (phase3)
				{
					calamityGlobalNPC.newAI[1] += 1f;

					// Sound
					if (calamityGlobalNPC.newAI[1] == newPhaseTimer - 60f)
					{
						SoundEffectInstance sound = Main.PlaySound(SoundID.DD2_BetsyScream, (int)npc.position.X, (int)npc.position.Y);
						if (sound != null)
						{
							sound.Pitch = 0.25f;
						}
					}

					if (calamityGlobalNPC.newAI[1] >= newPhaseTimer)
					{
						calamityGlobalNPC.newAI[1] = 0f;
						calamityGlobalNPC.newAI[2] = 1f;
						calamityGlobalNPC.newAI[3] = 1f;
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.SyncExtraAI();
						npc.netUpdate = true;
					}
				}
				else
				{
					calamityGlobalNPC.newAI[0] += 1f;

					// Sound
					if (calamityGlobalNPC.newAI[0] == newPhaseTimer - 60f)
					{
						SoundEffectInstance sound = Main.PlaySound(SoundID.DD2_BetsyScream, (int)npc.position.X, (int)npc.position.Y);
						if (sound != null)
						{
							sound.Pitch = 0.25f;
						}
					}

					if (calamityGlobalNPC.newAI[0] >= newPhaseTimer)
					{
						calamityGlobalNPC.newAI[0] = 0f;
						calamityGlobalNPC.newAI[2] = 1f;
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.SyncExtraAI();
						npc.netUpdate = true;
					}
				}

				Vector2 value52 = player.Center - npc.Center;
				float scaleFactor16 = 4f + value52.Length() / 100f;
				float num1308 = 25f;
				value52.Normalize();
				value52 *= scaleFactor16;
				npc.velocity = (npc.velocity * (num1308 - 1f) + value52) / num1308;
				return;
			}

			// Max spawn amount
			int maxBirbs = revenge ? 3 : 2;

			// Variable for charging
			float chargeDistance = 600f;
			if (phase2)
				chargeDistance -= 50f;
			if (phase3)
				chargeDistance -= 50f;
			chargeDistance -= (enrageScale - 1f) * 100f;

			// Phase switch
			if (npc.ai[0] == 0f)
			{
				if (npc.Center.X < player.Center.X - 2f)
					npc.direction = 1;
				if (npc.Center.X > player.Center.X + 2f)
					npc.direction = -1;

				// Direction and rotation
				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt * 1.25f) / 10f;

				// Fly to target if target is too far away, otherwise get close to target and then slow down
				Vector2 value51 = player.Center - npc.Center;
				value51.Y -= 200f;
				if (value51.Length() > 2800f)
				{
					npc.TargetClosest();
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
				}
				else if (value51.Length() > 240f)
				{
					float scaleFactor15 = 12f + (enrageScale - 1f) * 6f;
					float num1306 = 30f;
					value51.Normalize();
					value51 *= scaleFactor15;
					npc.velocity = (npc.velocity * (num1306 - 1f) + value51) / num1306;
				}
				else if (npc.velocity.Length() > 2f)
					npc.velocity *= 0.95f;
				else if (npc.velocity.Length() < 1f)
					npc.velocity *= 1.05f;

				// Phase switch
				npc.ai[1] += 1f;
				if (npc.ai[1] >= 30f && Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;

					while (npc.ai[0] == 0f)
					{
						if (phase2)
							npc.localAI[0] += 1f;

						bool canHit = Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1);

						if (npc.localAI[0] >= (phase3 ? 7 : 9) && canHit)
						{
							npc.TargetClosest();
							npc.ai[0] = 5f;
							npc.localAI[0] = 0f;

							// Decrease the feather variable, feathers can be used again if it's at 0
							if (npc.ai[3] > 0f)
								npc.ai[3] -= 1f;

							// Decrease enraged attacks by 1
							if (npc.localAI[2] > 0f)
								npc.localAI[2] -= 1f;

							// Decrease amount of attacks until able to spawn small birbs again
							if (npc.localAI[3] > 0f)
								npc.localAI[3] -= 1f;
						}
						else
						{
							int num1307 = phase2 ? Main.rand.Next(2) + 1 : Main.rand.Next(3);
							if (phase3)
								num1307 = 1;

							float featherVelocity = 2f + (enrageScale - 1f);
							int type = ModContent.ProjectileType<RedLightningFeather>();
							int damage = npc.GetProjectileDamage(type);

							if (num1307 == 0 && canHit && npc.localAI[3] == 0f)
							{
								npc.TargetClosest();
								npc.ai[0] = 2f;

								// Decrease the feather variable, feathers can be used again if it's at 0
								if (npc.ai[3] > 0f)
									npc.ai[3] -= 1f;

								// Decrease enraged attacks by 1
								if (npc.localAI[2] > 0f)
									npc.localAI[2] -= 1f;

								// Birb will do at least 1 different attack before entering this phase again
								npc.localAI[3] = 1f;
							}
							else if (num1307 == 1)
							{
								npc.TargetClosest();
								npc.ai[0] = 3f;

								// Decrease enraged attacks by 1
								if (npc.localAI[2] > 0f)
									npc.localAI[2] -= 1f;

								// Decrease amount of attacks until able to use other attacks again
								if (npc.localAI[3] > 0f)
									npc.localAI[3] -= 1f;

								if (phase2 && npc.ai[3] == 0f)
								{
									npc.ai[3] = 3f;
									Main.PlaySound(SoundID.Item102, (int)player.position.X, (int)player.position.Y);
									int totalProjectiles = 40;
									float radians = MathHelper.TwoPi / totalProjectiles;
									int distance = 800;
									bool spawnRight = player.velocity.X > 0f;
									for (int i = 0; i < totalProjectiles; i++)
									{
										if (spawnRight)
										{
											if (i >= (int)(totalProjectiles * 0.125) && i <= (int)(totalProjectiles * 0.375))
											{
												Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
												Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
												Projectile.NewProjectile(spawnVector, velocity, type, damage, 0f, Main.myPlayer);
											}
										}
										else
										{
											if (i >= (int)(totalProjectiles * 0.625) && i <= (int)(totalProjectiles * 0.875))
											{
												Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
												Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
												Projectile.NewProjectile(spawnVector, velocity, type, damage, 0f, Main.myPlayer);
											}
										}
									}
								}
								else
								{
									// Decrease the feather variable, feathers can be used again if it's at 0
									if (npc.ai[3] > 0f)
										npc.ai[3] -= 1f;
								}
							}
							else if (NPC.CountNPCS(ModContent.NPCType<Bumblefuck2>()) < maxBirbs && npc.localAI[3] == 0f)
							{
								npc.TargetClosest();
								npc.ai[0] = 4f;

								// Birb will do at least 2 different attacks before entering this phase again
								npc.localAI[3] = 2f;

								// Decrease enraged attacks by 1
								if (npc.localAI[2] > 0f)
									npc.localAI[2] -= 1f;

								if (npc.ai[3] == 0f)
								{
									npc.ai[3] = 3f;
									Main.PlaySound(SoundID.Item102, (int)player.position.X, (int)player.position.Y);
									int totalProjectiles = phase2 ? 40 : 48;
									float radians = MathHelper.TwoPi / totalProjectiles;
									int distance = phase2 ? 1200 : 1320;
									bool spawnRight = player.velocity.X > 0f;
									for (int i = 0; i < totalProjectiles; i++)
									{
										if (spawnRight)
										{
											if (i >= totalProjectiles / 2)
												break;

											Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
											Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
											Projectile.NewProjectile(spawnVector, velocity, type, damage, 0f, Main.myPlayer);
										}
										else
										{
											if (i >= totalProjectiles / 2)
											{
												Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
												Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
												Projectile.NewProjectile(spawnVector, velocity, type, damage, 0f, Main.myPlayer);
											}
										}
									}
								}
								else
								{
									// Decrease the feather variable, feathers can be used again if it's at 0
									if (npc.ai[3] > 0f)
										npc.ai[3] -= 1f;
								}
							}
						}
					}

					npc.netUpdate = true;
					npc.SyncExtraAI();
				}
			}

			// Fly to target
			else if (npc.ai[0] == 1f)
			{
				if (npc.velocity.X < 0f)
					npc.direction = -1;
				else if (npc.velocity.X > 0f)
					npc.direction = 1;

				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

				Vector2 value52 = player.Center - npc.Center;
				if (value52.Length() < 800f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
				{
					npc.TargetClosest();
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
					npc.SyncExtraAI();
				}

				float velocity = 14f + (enrageScale - 1f) * 4f;
				float scaleFactor16 = velocity + value52.Length() / 100f;
				float num1308 = 25f;
				value52.Normalize();
				value52 *= scaleFactor16;
				npc.velocity = (npc.velocity * (num1308 - 1f) + value52) / num1308;
			}

			// Fly towards target quickly
			else if (npc.ai[0] == 2f)
			{
				if (npc.target < 0 || !player.active || player.dead)
				{
					npc.TargetClosest();
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
					npc.SyncExtraAI();
				}

				if (player.Center.X - 10f < npc.Center.X)
					npc.direction = -1;
				else if (player.Center.X + 10f > npc.Center.X)
					npc.direction = 1;

				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * rotationMult * 0.5f + npc.velocity.X * rotationAmt * 1.25f) / 5f;

				Vector2 value53 = player.Center - npc.Center;
				value53.Y -= 20f;
				npc.ai[2] += 0.0222222228f;
				if (expertMode)
					npc.ai[2] += 0.0166666675f;

				float velocity = 8f + (enrageScale - 1f) * 2f;
				float scaleFactor17 = velocity + npc.ai[2] + value53.Length() / 120f;
				float num1309 = 20f;
				value53.Normalize();
				value53 *= scaleFactor17;
				npc.velocity = (npc.velocity * (num1309 - 1f) + value53) / num1309;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 120f || !Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
				{
					npc.TargetClosest();
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
					npc.SyncExtraAI();
				}
			}

			// Line up charge
			else if (npc.ai[0] == 3f)
			{
				if (npc.velocity.X < 0f)
					npc.direction = -1;
				else
					npc.direction = 1;

				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * rotationMult * 0.5f + npc.velocity.X * rotationAmt * 0.85f) / 5f;

				Vector2 value54 = player.Center - npc.Center;
				value54.Y -= 12f;
				if (npc.Center.X > player.Center.X)
					value54.X += chargeDistance;
				else
					value54.X -= chargeDistance;

				float verticalDistanceGateValue = (phase3 ? 100f : 20f) + (enrageScale - 1f) * 20f;
				if (Math.Abs(npc.Center.X - player.Center.X) > chargeDistance - 50f && Math.Abs(npc.Center.Y - player.Center.Y) < verticalDistanceGateValue)
				{
					npc.ai[0] = 3.1f;
					npc.ai[1] = 0f;
					npc.netUpdate = true;
					npc.SyncExtraAI();
				}

				npc.ai[1] += 0.0333333351f;
				float velocity = 16f + (enrageScale - 1f) * 4f;
				float scaleFactor18 = velocity + npc.ai[1];
				float num1310 = 4f;
				value54.Normalize();
				value54 *= scaleFactor18;
				npc.velocity = (npc.velocity * (num1310 - 1f) + value54) / num1310;
			}

			// Prepare to charge
			else if (npc.ai[0] == 3.1f)
			{
				npc.rotation = (npc.rotation * rotationMult * 0.5f + npc.velocity.X * rotationAmt * 0.85f) / 5f;

				Vector2 vector206 = player.Center - npc.Center;
				vector206.Y -= 12f;
				float scaleFactor19 = 28f + (enrageScale - 1f) * 4f;
				float num1311 = 8f;
				vector206.Normalize();
				vector206 *= scaleFactor19;
				npc.velocity = (npc.velocity * (num1311 - 1f) + vector206) / num1311;

				if (npc.velocity.X < 0f)
					npc.direction = -1;
				else
					npc.direction = 1;

				npc.spriteDirection = npc.direction;

				npc.ai[1] += 1f;
				if (npc.ai[1] > 10f)
				{
					npc.velocity = vector206;

					if (npc.velocity.X < 0f)
						npc.direction = -1;
					else
						npc.direction = 1;

					npc.ai[0] = 3.2f;
					npc.ai[1] = npc.direction;
					npc.netUpdate = true;
					npc.SyncExtraAI();
				}
			}

			// Charge
			else if (npc.ai[0] == 3.2f)
			{
				npc.ai[2] += 0.0333333351f;
				float velocity = 28f + (enrageScale - 1f) * 4f;
				npc.velocity.X = (velocity + npc.ai[2]) * npc.ai[1];

				if ((npc.ai[1] > 0f && npc.Center.X > player.Center.X + (chargeDistance - 140f)) || (npc.ai[1] < 0f && npc.Center.X < player.Center.X - (chargeDistance - 140f)))
				{
					if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
					{
						npc.TargetClosest();
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.netUpdate = true;
					}
					else if (Math.Abs(npc.Center.X - player.Center.X) > chargeDistance + 200f)
					{
						npc.TargetClosest();
						npc.ai[0] = 1f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.netUpdate = true;
					}
				}

				npc.rotation = (npc.rotation * rotationMult * 0.5f + npc.velocity.X * rotationAmt * 0.85f) / 5f;
			}

			// Birb spawn
			else if (npc.ai[0] == 4f)
			{
				if (npc.ai[1] == 0f)
				{
					Vector2 destination2 = player.Center + new Vector2(0f, -200f);
					Vector2 desiredVelocity2 = npc.SafeDirectionTo(destination2, -Vector2.UnitY) * 18f;
					npc.SimpleFlyMovement(desiredVelocity2, 1.5f);

					if (npc.velocity.X < 0f)
						npc.direction = -1;
					else
						npc.direction = 1;

					npc.spriteDirection = npc.direction;

					npc.ai[2] += 1f;
					if (npc.Distance(player.Center) < 600f || npc.ai[2] >= 180f)
					{
						npc.ai[1] = 1f;
						npc.netUpdate = true;
					}
				}
				else
				{
					if (npc.ai[1] < 90f)
						npc.velocity *= 0.95f;
					else
						npc.velocity *= 0.98f;

					if (npc.ai[1] == 90f)
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y /= 3f;

						npc.velocity.Y -= 3f;
					}

					// Sound
					if (npc.ai[1] == birbSpawnPhaseTimer - 60f)
					{
						SoundEffectInstance sound = Main.PlaySound(SoundID.DD2_BetsyScream, (int)npc.position.X, (int)npc.position.Y);
						if (sound != null)
						{
							sound.Pitch = 0.25f;
						}
					}

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						bool spawnFlag = NPC.CountNPCS(ModContent.NPCType<Bumblefuck2>()) < maxBirbs && (npc.ai[1] == 140f || (revenge && npc.ai[1] == 155f) || npc.ai[1] == 170f);
						if (spawnFlag)
						{
							Vector2 vector7 = npc.Center + (MathHelper.TwoPi * Main.rand.NextFloat()).ToRotationVector2() * new Vector2(2f, 1f) * 50f * (0.6f + Main.rand.NextFloat() * 0.4f);
							if (Vector2.Distance(vector7, player.Center) > 150f)
								NPC.NewNPC((int)vector7.X, (int)vector7.Y, ModContent.NPCType<Bumblefuck2>(), npc.whoAmI);
							npc.netUpdate = true;
						}
					}

					npc.ai[1] += 1f;
				}

				if (npc.ai[1] >= birbSpawnPhaseTimer)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}

			// Spit homing aura sphere
			else if (npc.ai[0] == 5f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;

				// Play sound
				float aiGateValue = 120f;
				if (npc.ai[1] == aiGateValue - 30f)
				{
					Main.PlaySound(SoundID.DD2_BetsyFireballShot, (int)npc.position.X, (int)npc.position.Y);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector7 = npc.rotation.ToRotationVector2() * (Vector2.UnitX * npc.direction) * (npc.width + 20) / 2f + vector;
						float ai0 = (phase3 ? 2f : 0f) + (enrageScale - 1f);
						if (ai0 > 3f)
							ai0 = 3f;
						Projectile.NewProjectile(vector7.X, vector7.Y, 0f, 0f, ModContent.ProjectileType<BirbAuraFlare>(), 0, 0f, Main.myPlayer, ai0, npc.target + 1);
						npc.netUpdate = true;
					}
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= aiGateValue)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.netUpdate = true;
				}
			}
		}

		public static void Bumblebirb2AI(NPC npc, Mod mod, bool bossMinion)
		{
            Player player = Main.player[npc.target];
            Vector2 vector = npc.Center;

			float rotationMult = 4f;
			float rotationAmt = 0.04f;

			if (Vector2.Distance(player.Center, vector) > 5600f)
            {
                if (npc.timeLeft > 5)
                {
                    npc.timeLeft = 5;
                }
            }

            npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt * 1.25f) / 10f;

            if (npc.ai[0] == 0f || npc.ai[0] == 1f)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type)
                    {
                        Vector2 value42 = Main.npc[i].Center - npc.Center;
                        if (value42.Length() < (npc.width + npc.height))
                        {
                            value42.Normalize();
                            value42 *= -0.1f;
                            npc.velocity += value42;
                            NPC nPC6 = Main.npc[i];
                            nPC6.velocity -= value42;
                        }
                    }
                }
            }

            if (npc.target < 0 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest(true);
                Vector2 vector240 = Main.player[npc.target].Center - npc.Center;
                if (Main.player[npc.target].dead || vector240.Length() > (bossMinion ? 5600f : 2800f))
                {
                    npc.ai[0] = -1f;
                }
            }
            else
            {
                Vector2 vector241 = Main.player[npc.target].Center - npc.Center;
                if (npc.ai[0] > 1f && vector241.Length() > 3600f)
                {
                    npc.ai[0] = 1f;
                }
            }

            if (npc.ai[0] == -1f)
            {
                Vector2 value43 = new Vector2(0f, -8f);
                npc.velocity = (npc.velocity * 21f + value43) / 10f;
                npc.dontTakeDamage = true;
                return;
            }

            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(true);
                npc.spriteDirection = npc.direction;
                Vector2 value44 = Main.player[npc.target].Center - npc.Center;
                if (value44.Length() > 2800f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                }
                else if (value44.Length() > 400f)
                {
                    float scaleFactor20 = (bossMinion ? 9f : 7f) + value44.Length() / 100f + npc.ai[1] / 15f;
                    float num1377 = 30f;
                    value44.Normalize();
                    value44 *= scaleFactor20;
                    npc.velocity = (npc.velocity * (num1377 - 1f) + value44) / num1377;
                }
                else if (npc.velocity.Length() > 2f)
                {
                    npc.velocity *= 0.95f;
                }
                else if (npc.velocity.Length() < 1f)
                {
                    npc.velocity *= 1.05f;
                }
                npc.ai[1] += 1f;
                if (npc.ai[1] >= (bossMinion ? 90f : 105f))
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 2f;
                }
            }
            else
            {
                if (npc.ai[0] == 1f)
                {
                    if (npc.target < 0 || !Main.player[npc.target].active || Main.player[npc.target].dead)
                    {
                        npc.TargetClosest(true);
                    }
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * rotationMult + npc.velocity.X * rotationAmt) / 10f;
                    Vector2 value45 = Main.player[npc.target].Center - npc.Center;
                    if (value45.Length() < 800f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }
                    npc.ai[2] += 0.0166666675f;
                    float scaleFactor21 = (bossMinion ? 12f : 9f) + npc.ai[2] + value45.Length() / 150f;
                    float num1378 = 25f;
                    value45.Normalize();
                    value45 *= scaleFactor21;
                    npc.velocity = (npc.velocity * (num1378 - 1f) + value45) / num1378;
                    npc.netSpam = 5;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }
                    return;
                }
                if (npc.ai[0] == 2f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * rotationMult * 0.75f + npc.velocity.X * rotationAmt * 1.25f) / 8f;
                    Vector2 vector242 = Main.player[npc.target].Center - npc.Center;
                    vector242.Y -= 8f;
                    float scaleFactor22 = bossMinion ? 18f : 14f;
                    float num1379 = 8f;
                    vector242.Normalize();
                    vector242 *= scaleFactor22;
                    npc.velocity = (npc.velocity * (num1379 - 1f) + vector242) / num1379;
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 10f)
                    {
                        npc.velocity = vector242;
                        if (npc.velocity.X < 0f)
                        {
                            npc.direction = -1;
                        }
                        else
                        {
                            npc.direction = 1;
                        }
                        npc.ai[0] = 2.1f;
                        npc.ai[1] = 0f;
                    }
                }
                else if (npc.ai[0] == 2.1f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.velocity *= 1.01f;
                    npc.ai[1] += 1f;
                    int num1380 = 30;
                    if (npc.ai[1] > num1380)
                    {
                        if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                        {
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            return;
                        }
                        if (npc.ai[1] > (num1380 * 2))
                        {
                            npc.ai[0] = 1f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                    }
                }
            }
		}
		#endregion

		#region Old Duke
		public static void OldDukeAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			npc.Calamity().canBreakPlayerDefense = true;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Variables
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			bool phase2 = lifeRatio <= (death ? 0.8f : revenge ? 0.7f : 0.5f);
			bool phase3 = lifeRatio <= (death ? 0.5f : (revenge ? 0.35f : 0.2f)) && expertMode;
			bool phase2AI = npc.ai[0] > 4f;
			bool phase3AI = npc.ai[0] > 9f;
			bool charging = npc.ai[3] < 10f;
			float pie = (float)Math.PI;

			if (calamityGlobalNPC.newAI[0] >= 300f)
				calamityGlobalNPC.newAI[1] = 1f;

			// Adjust stats
			calamityGlobalNPC.DR = calamityGlobalNPC.newAI[1] == 1f ? 0f : 0.5f;
			npc.defense = calamityGlobalNPC.newAI[1] == 1f ? 0 : npc.defDefense;
			if (phase3AI)
			{
				npc.damage = (int)(npc.defDamage * 1.3f);
				npc.defense = calamityGlobalNPC.newAI[1] == 1f ? 0 : npc.defDefense - 40;
			}
			else if (phase2AI)
			{
				npc.damage = (int)(npc.defDamage * 1.2f);
				npc.defense = calamityGlobalNPC.newAI[1] == 1f ? 0 : npc.defDefense - 20;
			}
			else
			{
				npc.damage = npc.defDamage;
			}

			int idlePhaseTimer = expertMode ? 55 : 60;
			float idlePhaseAcceleration = expertMode ? 0.75f : 0.7f;
			float idlePhaseVelocity = expertMode ? 14f : 13f;
			if (phase3AI)
			{
				idlePhaseAcceleration = expertMode ? 0.6f : 0.55f;
				idlePhaseVelocity = expertMode ? 12f : 11f;
			}
			else if (phase2AI & charging)
			{
				idlePhaseAcceleration = expertMode ? 0.8f : 0.75f;
				idlePhaseVelocity = expertMode ? 15f : 14f;
			}

			int chargeTime = expertMode ? 34 : 36;
			float chargeVelocity = expertMode ? 20f : 19f;
			if (phase3AI)
			{
				chargeTime = expertMode ? 28 : 30;
				chargeVelocity = expertMode ? 26f : 25f;
			}
			else if (charging & phase2AI)
			{
				chargeTime = expertMode ? 31 : 33;
				chargeVelocity = expertMode ? 24f : 23f;
			}

			if (malice)
			{
				idlePhaseTimer = 35;
				idlePhaseAcceleration *= 1.25f;
				idlePhaseVelocity *= 1.2f;
				chargeTime -= 3;
				chargeVelocity *= 1.25f;
			}
			else if (death)
			{
				idlePhaseTimer = 51;
				idlePhaseAcceleration *= 1.05f;
				idlePhaseVelocity *= 1.05f;
				chargeTime -= 2;
				chargeVelocity *= 1.1f;
			}
			else if (revenge)
			{
				idlePhaseTimer = 53;
				idlePhaseAcceleration *= 1.025f;
				idlePhaseVelocity *= 1.025f;
				chargeTime -= 1;
				chargeVelocity *= 1.05f;
			}

			if (calamityGlobalNPC.newAI[1] == 1f)
				idlePhaseVelocity *= 0.25f;

			// Variables
			int toothBallBelchPhaseTimer = malice ? 35 : death ? 90 : 120;
			int toothBallBelchPhaseDivisor = malice ? 7 : death ? 18 : 24;
			float toothBallBelchPhaseAcceleration = malice ? 0.95f : death ? 0.6f : 0.55f;
			float toothBallBelchPhaseVelocity = malice ? 14f : death ? 10f : 9f;
			float goreVelocityX = death ? 8f : revenge ? 7.5f : expertMode ? 7f : 6f;
			float goreVelocityY = death ? 10.5f : revenge ? 10f : expertMode ? 9.5f : 8f;
			float sharkronVelocity = malice ? 18f : death ? 16f : revenge ? 15f : expertMode ? 14f : 12f;
			int num9 = 120;
			int num10 = 180;
			int num12 = 30;
			int num13 = malice ? 35 : death ? 90 : 120;
			int toothBallSpinPhaseDivisor = malice ? 7 : death ? 18 : 24;
			float spinTime = num13 / 2;
			float toothBallSpinToothBallVelocity = malice ? 14f : death ? 9.5f : 9f;
			float scaleFactor4 = 22f;
			float num15 = MathHelper.TwoPi / spinTime;
			int num16 = 75;

			Vector2 vector = npc.Center;
			Player player = Main.player[npc.target];

			// Get target
			if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
			{
				npc.TargetClosest();
				player = Main.player[npc.target];
				npc.netUpdate = true;
			}

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(player.Center, vector) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			// Despawn
			if (player.dead || Vector2.Distance(player.Center, vector) > 8800f)
			{
				npc.TargetClosest();

				npc.velocity.Y -= 0.4f;

				if (npc.timeLeft > 10)
					npc.timeLeft = 10;

				if (npc.timeLeft == 1)
				{
					CalamityWorld.acidRainPoints = 0;
					CalamityWorld.triedToSummonOldDuke = false;
					AcidRainEvent.UpdateInvasion(false);
					npc.timeLeft = 0;
				}

				if (npc.ai[0] > 4f)
					npc.ai[0] = 5f;
				else
					npc.ai[0] = 0f;

				npc.ai[2] = 0f;
			}

			if (calamityGlobalNPC.newAI[1] == 1f)
			{
				npc.Calamity().canBreakPlayerDefense = false;

				npc.damage /= 4;

				// Play tired sound
				if (calamityGlobalNPC.newAI[0] % 60f == 0f)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeHuff"), player.Center);

				calamityGlobalNPC.newAI[0] -= malice ? 1.5f : 1f;
				if (calamityGlobalNPC.newAI[0] <= 0f)
					calamityGlobalNPC.newAI[1] = 0f;
			}

			// Enrage variable
			bool enrage = !BossRushEvent.BossRushActive &&
				(player.position.Y < 300f || player.position.Y > Main.worldSurface * 16.0 ||
				(player.position.X > 8000f && player.position.X < (Main.maxTilesX * 16 - 8000)));

			// Enrage
			if (enrage)
			{
				if (npc.localAI[1] > 0f)
					npc.localAI[1] -= 1f;
			}
			else
				npc.localAI[1] = CalamityGlobalNPC.biomeEnrageTimerMax;

			bool biomeEnraged = npc.localAI[1] <= 0f;

			npc.Calamity().CurrentlyEnraged = biomeEnraged;

			// If the player isn't in the ocean biome or Old Duke is transitioning between phases, become immune
			if (!phase3AI)
				npc.dontTakeDamage = npc.ai[0] == -1f || npc.ai[0] == 4f || npc.ai[0] == 9f;

			// Enrage
			if (biomeEnraged)
			{
				toothBallBelchPhaseTimer = 30;
				toothBallBelchPhaseDivisor = 6;
				toothBallBelchPhaseAcceleration = 1f;
				toothBallBelchPhaseVelocity = 15f;
				goreVelocityX = 12f;
				goreVelocityY = 16f;
				sharkronVelocity = 20f;
				idlePhaseTimer = 20;
				idlePhaseAcceleration = 1.2f;
				idlePhaseVelocity = 20f;
				chargeTime = 25;
				chargeVelocity += 8f;
				toothBallSpinPhaseDivisor = 6;
				toothBallSpinToothBallVelocity = 15f;
				npc.damage = npc.defDamage * 2;
				npc.defense = npc.defDefense * 3;
			}

			// Set variables for spawn effects
			if (npc.localAI[0] == 0f)
			{
				npc.localAI[0] = 1f;
				npc.alpha = 255;
				npc.rotation = 0f;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[0] = -1f;
					npc.netUpdate = true;
				}
			}

			// Rotation
			float rateOfRotation = 0.04f;
			if (npc.ai[0] == 1f || npc.ai[0] == 6f || npc.ai[0] == 7f || npc.ai[0] == 14f)
				rateOfRotation = 0f;
			if (npc.ai[0] == 3f || npc.ai[0] == 4f)
				rateOfRotation = 0.01f;
			if (npc.ai[0] == 8f || npc.ai[0] == 13f)
				rateOfRotation = 0.05f;

			Vector2 rotationVector = player.Center - vector;
			if (calamityGlobalNPC.newAI[1] != 1f && !player.dead)
			{
				// Rotate to show direction of predictive charge
				if (npc.ai[0] == 0f && !phase2)
				{
					if (npc.ai[3] < 6f)
					{
						rateOfRotation = 0.1f;
						rotationVector = Vector2.Normalize(player.Center + player.velocity * 20f - vector) * chargeVelocity;
					}
				}
				else if (npc.ai[0] == 5f && !phase3)
				{
					if (npc.ai[3] < 4f)
					{
						rateOfRotation = 0.1f;
						rotationVector = Vector2.Normalize(player.Center + player.velocity * 20f - vector) * chargeVelocity;
					}
				}
				else if (npc.ai[0] == 10f)
				{
					if (npc.ai[3] < 8f && npc.ai[3] != 1f && npc.ai[3] != 4f)
					{
						rateOfRotation = 0.1f;
						rotationVector = Vector2.Normalize(player.Center + player.velocity * 20f - vector) * chargeVelocity;
					}
				}
			}

			float num17 = (float)Math.Atan2(rotationVector.Y, rotationVector.X);
			if (npc.spriteDirection == 1)
				num17 += pie;
			if (num17 < 0f)
				num17 += MathHelper.TwoPi;
			if (num17 > MathHelper.TwoPi)
				num17 -= MathHelper.TwoPi;
			if (npc.ai[0] == -1f || npc.ai[0] == 3f || npc.ai[0] == 4f)
				num17 = 0f;
			if (npc.ai[0] == 8f || npc.ai[0] == 13f)
				num17 = pie * 0.1666666667f * npc.spriteDirection;

			if (npc.rotation < num17)
			{
				if (num17 - npc.rotation > pie)
					npc.rotation -= rateOfRotation;
				else
					npc.rotation += rateOfRotation;
			}
			if (npc.rotation > num17)
			{
				if (npc.rotation - num17 > pie)
					npc.rotation += rateOfRotation;
				else
					npc.rotation -= rateOfRotation;
			}

			if ((npc.ai[0] != 8f && npc.ai[0] != 13f) || npc.spriteDirection == 1)
			{
				if (npc.rotation > num17 - rateOfRotation && npc.rotation < num17 + rateOfRotation)
					npc.rotation = num17;
				if (npc.rotation < 0f)
					npc.rotation += MathHelper.TwoPi;
				if (npc.rotation > MathHelper.TwoPi)
					npc.rotation -= MathHelper.TwoPi;
				if (npc.rotation > num17 - rateOfRotation && npc.rotation < num17 + rateOfRotation)
					npc.rotation = num17;
			}

			// Alpha adjustments
			if (npc.ai[0] != -1f && (npc.ai[0] < 9f || npc.ai[0] > 12f))
			{
				if (Collision.SolidCollision(npc.position, npc.width, npc.height))
					npc.alpha += 15;
				else
					npc.alpha -= 15;

				if (npc.alpha < 0)
					npc.alpha = 0;
				if (npc.alpha > 150)
					npc.alpha = 150;
			}

			// Spawn effects
			if (npc.ai[0] == -1f)
			{
				// Velocity
				if (npc.Calamity().newAI[3] == 0f)
					npc.velocity *= 0.98f;

				// Direction
				int num19 = Math.Sign(player.Center.X - vector.X);
				if (num19 != 0)
				{
					npc.direction = num19;
					npc.spriteDirection = -npc.direction;
				}

				// Alpha
				if (npc.ai[2] > 20f)
				{
					if (npc.Calamity().newAI[3] == 0f)
						npc.velocity.Y = -2f;

					npc.alpha -= 5;
					if (Collision.SolidCollision(npc.position, npc.width, npc.height))
						npc.alpha += 15;
					if (npc.alpha < 0)
						npc.alpha = 0;
					if (npc.alpha > 150)
						npc.alpha = 150;
				}

				// Spawn dust and play sound
				if (npc.ai[2] == num9 - 30)
				{
					int num20 = 36;
					for (int i = 0; i < num20; i++)
					{
						Vector2 dust = (Vector2.Normalize(npc.velocity) * new Vector2(npc.width / 2f, npc.height) * 0.75f * 0.5f).RotatedBy((i - (num20 / 2 - 1)) * MathHelper.TwoPi / num20) + npc.Center;
						Vector2 vector2 = dust - npc.Center;
						int num21 = Dust.NewDust(dust + vector2, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, vector2.X * 2f, vector2.Y * 2f, 100, default, 1.4f);
						Main.dust[num21].noGravity = true;
						Main.dust[num21].noLight = true;
						Main.dust[num21].velocity = Vector2.Normalize(vector2) * 3f;
					}

					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num16)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}

			// Phase 1
			else if (npc.ai[0] == 0f && !player.dead)
			{
				// Velocity
				if (npc.ai[1] == 0f)
					npc.ai[1] = 500 * Math.Sign((vector - player.Center).X);

				Vector2 vector3 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -300f) - vector - npc.velocity) * idlePhaseVelocity;
				npc.SimpleFlyMovement(vector3, idlePhaseAcceleration);

				// Rotation and direction
				int num22 = Math.Sign(player.Center.X - vector.X);
				if (num22 != 0)
				{
					if (npc.ai[2] == 0f && num22 != npc.direction)
						npc.rotation += pie;

					npc.direction = num22;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += pie;

					npc.spriteDirection = -npc.direction;
				}

				// Phase switch
				if (calamityGlobalNPC.newAI[1] != 1f || (phase2 && !phase2AI))
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= idlePhaseTimer || (phase2 && !phase2AI))
					{
						int num23 = 0;
						switch ((int)npc.ai[3])
						{
							case 0:
							case 1:
							case 2:
							case 3:
							case 4:
							case 5:
								num23 = 1;
								break;
							case 6:
								npc.ai[3] = 1f;
								num23 = 2;
								break;
							case 7:
								npc.ai[3] = 0f;
								num23 = 3;
								break;
						}

						if (phase2)
							num23 = 4;

						// Set velocity for charge
						if (num23 == 1)
						{
							npc.ai[0] = 1f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;

							// Velocity
							Vector2 distanceVector = player.Center + player.velocity * 20f - vector;
							npc.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
							npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

							// Direction
							if (num22 != 0)
							{
								npc.direction = num22;

								if (npc.spriteDirection == 1)
									npc.rotation += pie;

								npc.spriteDirection = -npc.direction;
							}
						}

						// Tooth Balls
						else if (num23 == 2)
						{
							npc.ai[0] = 2f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						// Call sharks from the sides of the screen
						else if (num23 == 3)
						{
							npc.ai[0] = 3f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						// Go to phase 2
						else if (num23 == 4)
						{
							npc.ai[0] = 4f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						npc.netUpdate = true;
					}
				}
			}

			// Charge
			else if (npc.ai[0] == 1f)
			{
				// Accelerate
				npc.velocity *= 1.01f;

				// Spawn dust
				int num24 = 7;
				for (int j = 0; j < num24; j++)
				{
					Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((j - (num24 / 2 - 1)) * pie / num24) + vector;
					Vector2 vector4 = ((float)(Main.rand.NextDouble() * pie) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
					int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, vector4.X * 2f, vector4.Y * 2f, 100, default, 1.4f);
					Main.dust[num25].noGravity = true;
					Main.dust[num25].noLight = true;
					Main.dust[num25].velocity /= 4f;
					Main.dust[num25].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= chargeTime)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 2f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}

			// Tooth Ball belch
			else if (npc.ai[0] == 2f)
			{
				// Velocity
				if (npc.ai[1] == 0f)
					npc.ai[1] = 500 * Math.Sign((vector - player.Center).X);

				Vector2 vector5 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -300f) - vector - npc.velocity) * toothBallBelchPhaseVelocity;
				npc.SimpleFlyMovement(vector5, toothBallBelchPhaseAcceleration);

				// Play sounds and spawn Tooth Balls
				if (npc.ai[2] == 0f)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);

				if (npc.ai[2] % toothBallBelchPhaseDivisor == 0f)
				{
					if (npc.ai[2] % 40f == 0f && npc.ai[2] != 0f)
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVomit"), (int)npc.position.X, (int)npc.position.Y);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector6 = Vector2.Normalize(player.Center - vector) * (npc.width + 20) / 2f + vector;
						NPC.NewNPC((int)vector6.X, (int)vector6.Y + 45, ModContent.NPCType<OldDukeToothBall>());
					}
				}

				// Direction
				int num26 = Math.Sign(player.Center.X - vector.X);
				if (num26 != 0)
				{
					npc.direction = num26;
					if (npc.spriteDirection != -npc.direction)
						npc.rotation += pie;
					npc.spriteDirection = -npc.direction;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= toothBallBelchPhaseTimer)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}

			// Call sharks from the sides of the screen
			else if (npc.ai[0] == 3f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Play sound and spawn sharks
				if (npc.ai[2] == num9 - 30)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVomit"), (int)npc.position.X, (int)npc.position.Y);

				if (npc.ai[2] >= num9 - 90)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] % 18f == 0f)
					{
						calamityGlobalNPC.newAI[2] += 100f;
						NPC.NewNPC((int)(vector.X + 900f), (int)(vector.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
						NPC.NewNPC((int)(vector.X - 900f), (int)(vector.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num9)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}

			// Transition to phase 2 and call sharks from below
			else if (npc.ai[0] == 4f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Sound
				if (npc.ai[2] == num10 - 60)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);

				if (npc.ai[2] >= num10 - 60)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] % 18f == 0f)
					{
						calamityGlobalNPC.newAI[2] += 150f;
						NPC.NewNPC((int)(vector.X + 50f + calamityGlobalNPC.newAI[2]), (int)(vector.Y + 540f), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, 1f, -sharkronVelocity, 255);
						NPC.NewNPC((int)(vector.X - 50f - calamityGlobalNPC.newAI[2]), (int)(vector.Y + 540f), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, -1f, -sharkronVelocity, 255);
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num10)
				{
					calamityGlobalNPC.newAI[0] = 0f;
					calamityGlobalNPC.newAI[1] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}

			// Phase 2
			else if (npc.ai[0] == 5f && !player.dead)
			{
				// Velocity
				if (npc.ai[1] == 0f)
					npc.ai[1] = 500 * Math.Sign((vector - player.Center).X);

				Vector2 vector8 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -300f) - vector - npc.velocity) * idlePhaseVelocity;
				npc.SimpleFlyMovement(vector8, idlePhaseAcceleration);

				// Direction and rotation
				int num27 = Math.Sign(player.Center.X - vector.X);
				if (num27 != 0)
				{
					if (npc.ai[2] == 0f && num27 != npc.direction)
						npc.rotation += pie;

					npc.direction = num27;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += pie;

					npc.spriteDirection = -npc.direction;
				}

				// Phase switch
				if (calamityGlobalNPC.newAI[1] != 1f || (phase3 && !phase3AI))
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= idlePhaseTimer || (phase3 && !phase3AI))
					{
						int num28 = 0;
						switch ((int)npc.ai[3])
						{
							case 0:
							case 1:
							case 2:
							case 3:
								num28 = 1;
								break;
							case 4:
								npc.ai[3] = 1f;
								num28 = 2;
								break;
							case 5:
								npc.ai[3] = 0f;
								num28 = 3;
								break;
						}

						if (phase3)
							num28 = 4;

						// Set velocity for charge
						if (num28 == 1)
						{
							npc.ai[0] = 6f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;

							// Velocity and rotation
							Vector2 distanceVector = player.Center + player.velocity * 20f - vector;
							npc.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
							npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

							// Direction
							if (num27 != 0)
							{
								npc.direction = num27;

								if (npc.spriteDirection == 1)
									npc.rotation += pie;

								npc.spriteDirection = -npc.direction;
							}
						}

						// Set velocity for spin
						else if (num28 == 2)
						{
							// Velocity and rotation
							npc.velocity = Vector2.Normalize(player.Center - vector) * scaleFactor4;
							npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

							// Direction
							if (num27 != 0)
							{
								npc.direction = num27;

								if (npc.spriteDirection == 1)
									npc.rotation += pie;

								npc.spriteDirection = -npc.direction;
							}

							npc.ai[0] = 7f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						else if (num28 == 3)
						{
							npc.ai[0] = 8f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						// Go to next phase
						else if (num28 == 4)
						{
							npc.ai[0] = 9f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						npc.netUpdate = true;
					}
				}
			}

			// Charge
			else if (npc.ai[0] == 6f)
			{
				// Accelerate
				npc.velocity *= 1.01f;

				// Spawn dust
				int num29 = 7;
				for (int k = 0; k < num29; k++)
				{
					Vector2 arg_1A97_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((k - (num29 / 2 - 1)) * pie / num29) + vector;
					Vector2 vector9 = ((float)(Main.rand.NextDouble() * pie) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
					int num30 = Dust.NewDust(arg_1A97_0 + vector9, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, vector9.X * 2f, vector9.Y * 2f, 100, default, 1.4f);
					Main.dust[num30].noGravity = true;
					Main.dust[num30].noLight = true;
					Main.dust[num30].velocity /= 4f;
					Main.dust[num30].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= chargeTime)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 2f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}

			// Tooth Ball and Vortex spin
			else if (npc.ai[0] == 7f)
			{
				// Play sounds and spawn Tooth Balls and a Vortex
				if (npc.ai[2] == 0f)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);

					int type = ModContent.ProjectileType<OldDukeVortex>();
					int damage = npc.GetProjectileDamage(type);
					Vector2 vortexSpawn = vector + npc.velocity.RotatedBy(MathHelper.PiOver2 * -npc.direction) * spinTime / MathHelper.TwoPi;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(vortexSpawn, Vector2.Zero, type, damage, 0f, Main.myPlayer, vortexSpawn.X, vortexSpawn.Y);
					}
				}

				if (npc.ai[2] % toothBallSpinPhaseDivisor == 0f)
				{
					if (npc.ai[2] % 45f == 0f && npc.ai[2] != 0f)
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVomit"), (int)npc.position.X, (int)npc.position.Y);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector10 = Vector2.Normalize(npc.velocity) * (npc.width + 20) / 2f + vector;
						int num31 = NPC.NewNPC((int)vector10.X, (int)vector10.Y + 45, ModContent.NPCType<OldDukeToothBall>());
						Main.npc[num31].target = npc.target;
						Main.npc[num31].velocity = Vector2.Normalize(npc.velocity).RotatedBy(MathHelper.PiOver2 * npc.direction) * toothBallSpinToothBallVelocity;
						Main.npc[num31].netUpdate = true;
						Main.npc[num31].ai[3] = Main.rand.Next(30, 181);
					}
				}

				// Velocity and rotation
				npc.velocity = npc.velocity.RotatedBy(-(double)num15 * (float)npc.direction);
				npc.rotation -= num15 * npc.direction;

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num13)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}

			// Vomit a huge amount of gore into the sky and call sharks from the sides of the screen
			else if (npc.ai[0] == 8f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Play sound
				if (npc.ai[2] == num9 - 30)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVomit"), (int)npc.position.X, (int)npc.position.Y);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector7 = npc.rotation.ToRotationVector2() * (Vector2.UnitX * npc.direction) * (npc.width + 20) / 2f + vector;
						int type = ModContent.ProjectileType<OldDukeGore>();
						int damage = npc.GetProjectileDamage(type);
						for (int i = 0; i < 20; i++)
						{
							float velocityX = npc.direction * goreVelocityX * (Main.rand.NextFloat() + 0.5f);
							float velocityY = goreVelocityY * (Main.rand.NextFloat() + 0.5f);
							Projectile.NewProjectile(vector7.X, vector7.Y, velocityX, -velocityY, type, damage, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}

				if (npc.ai[2] >= num9 - 90)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] % 18f == 0f)
					{
						calamityGlobalNPC.newAI[2] += 100f;
						float x = 900f - calamityGlobalNPC.newAI[2];
						NPC.NewNPC((int)(vector.X + x), (int)(vector.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
						NPC.NewNPC((int)(vector.X - x), (int)(vector.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num9)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}

			// Transition to phase 3 and summon sharks from above
			else if (npc.ai[0] == 9f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Sound
				if (npc.ai[2] == num10 - 60)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);

				if (npc.ai[2] >= num10 - 60)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] % 18f == 0f)
					{
						calamityGlobalNPC.newAI[2] += 200f;
						NPC.NewNPC((int)(vector.X + 50f + calamityGlobalNPC.newAI[2]), (int)(vector.Y - 540f), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, 1f, sharkronVelocity, 255);
						NPC.NewNPC((int)(vector.X - 50f - calamityGlobalNPC.newAI[2]), (int)(vector.Y - 540f), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, -1f, sharkronVelocity, 255);
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num10)
				{
					calamityGlobalNPC.newAI[0] = 0f;
					calamityGlobalNPC.newAI[1] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}

			// Phase 3
			else if (npc.ai[0] == 10f && !player.dead)
			{
				// Alpha
				npc.alpha -= 25;
				if (npc.alpha < 0)
					npc.alpha = 0;

				// Movement location
				if (npc.ai[1] == 0f)
					npc.ai[1] = 500 * Math.Sign((vector - player.Center).X);

				Vector2 desiredVelocity = Vector2.Normalize(player.Center + new Vector2(-npc.ai[1], -300f) - vector - npc.velocity) * idlePhaseVelocity;
				npc.SimpleFlyMovement(desiredVelocity, idlePhaseAcceleration);

				// Rotation and direction
				int num32 = Math.Sign(player.Center.X - vector.X);
				if (num32 != 0)
				{
					if (npc.ai[2] == 0f && num32 != npc.direction)
					{
						npc.rotation += pie;
						for (int l = 0; l < npc.oldPos.Length; l++)
							npc.oldPos[l] = Vector2.Zero;
					}

					npc.direction = num32;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += pie;

					npc.spriteDirection = -npc.direction;
				}

				// Phase switch
				if (calamityGlobalNPC.newAI[1] != 1f)
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= idlePhaseTimer)
					{
						int num33 = 0;
						switch ((int)npc.ai[3])
						{
							case 0:
							case 2:
							case 3:
							case 5:
							case 6:
							case 7:
								num33 = 1;
								break;
							case 1:
							case 8:
								num33 = 2;
								break;
							case 4:
								npc.ai[3] = 1f;
								num33 = 3;
								break;
							case 9:
								npc.ai[3] = 6f;
								num33 = 4;
								break;
						}

						// Set velocity for charge
						if (num33 == 1)
						{
							npc.ai[0] = 11f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;

							// Velocity and rotation
							Vector2 distanceVector = player.Center + player.velocity * 20f - vector;
							npc.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
							npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

							// Direction
							if (num32 != 0)
							{
								npc.direction = num32;

								if (npc.spriteDirection == 1)
									npc.rotation += pie;

								npc.spriteDirection = -npc.direction;
							}
						}

						// Pause
						else if (num33 == 2)
						{
							npc.ai[0] = 12f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						else if (num33 == 3)
						{
							npc.ai[0] = 13f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						// Set velocity for spin
						else if (num33 == 4)
						{
							// Velocity and rotation
							npc.velocity = Vector2.Normalize(player.Center - vector) * scaleFactor4;
							npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

							// Direction
							if (num32 != 0)
							{
								npc.direction = num32;

								if (npc.spriteDirection == 1)
									npc.rotation += pie;

								npc.spriteDirection = -npc.direction;
							}

							npc.ai[0] = 14f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
						}

						npc.netUpdate = true;
					}
				}
			}

			// Charge
			else if (npc.ai[0] == 11f)
			{
				// Accelerate
				npc.velocity *= 1.01f;

				// Spawn dust
				int num34 = 7;
				for (int m = 0; m < num34; m++)
				{
					Vector2 arg_2444_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((m - (num34 / 2 - 1)) * pie / num34) + vector;
					Vector2 vector11 = ((float)(Main.rand.NextDouble() * pie) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
					int num35 = Dust.NewDust(arg_2444_0 + vector11, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, vector11.X * 2f, vector11.Y * 2f, 100, default, 1.4f);
					Main.dust[num35].noGravity = true;
					Main.dust[num35].noLight = true;
					Main.dust[num35].velocity /= 4f;
					Main.dust[num35].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= chargeTime)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 2f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}

			// Pause before teleport
			else if (npc.ai[0] == 12f)
			{
				npc.dontTakeDamage = true;

				// Alpha
				if (npc.alpha < 255 && npc.ai[2] >= num12 - 15f)
				{
					npc.alpha += 17;
					if (npc.alpha > 255)
						npc.alpha = 255;
				}

				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Play sound
				if (npc.ai[2] == num12 / 2)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);

				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == num12 / 2)
				{
					// Teleport location
					if (npc.ai[1] == 0f)
						npc.ai[1] = 600 * Math.Sign((vector - player.Center).X);

					// Rotation and direction
					Vector2 center = player.Center + new Vector2(npc.ai[1], -300f);
					vector = npc.Center = center;
					int num36 = Math.Sign(player.Center.X - vector.X);
					if (num36 != 0)
					{
						if (npc.ai[2] == 0f && num36 != npc.direction)
						{
							npc.rotation += pie;
							for (int n = 0; n < npc.oldPos.Length; n++)
								npc.oldPos[n] = Vector2.Zero;
						}

						npc.direction = num36;

						if (npc.spriteDirection != -npc.direction)
							npc.rotation += pie;

						npc.spriteDirection = -npc.direction;
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num12)
				{
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;

					npc.ai[3] += 2f;
					if (npc.ai[3] >= 9f)
						npc.ai[3] = 0f;

					npc.dontTakeDamage = false;

					npc.netUpdate = true;
				}
			}

			// Vomit a huge amount of gore into the sky and call sharks from the sides of the screen
			else if (npc.ai[0] == 13f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Play sound
				if (npc.ai[2] == num9 - 30)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVomit"), (int)npc.position.X, (int)npc.position.Y);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector7 = npc.rotation.ToRotationVector2() * (Vector2.UnitX * npc.direction) * (npc.width + 20) / 2f + vector;
						int type = ModContent.ProjectileType<OldDukeGore>();
						int damage = npc.GetProjectileDamage(type);
						for (int i = 0; i < 20; i++)
						{
							float velocityX = npc.direction * goreVelocityX * (Main.rand.NextFloat() + 0.5f);
							float velocityY = goreVelocityY * (Main.rand.NextFloat() + 0.5f);
							Projectile.NewProjectile(vector7.X, vector7.Y, velocityX, -velocityY, type, damage, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}

				if (npc.ai[2] >= num9 - 90)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] % 18f == 0f)
					{
						calamityGlobalNPC.newAI[2] += 150f;
						float x = 900f - calamityGlobalNPC.newAI[2];
						NPC.NewNPC((int)(vector.X + x), (int)(vector.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
						NPC.NewNPC((int)(vector.X - x), (int)(vector.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<OldDukeSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num9)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}

			// Tooth Ball and Vortex spin
			else if (npc.ai[0] == 14f)
			{
				// Play sounds and spawn Tooth Balls and a Vortex
				if (npc.ai[2] == 0f)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeRoar"), (int)npc.position.X, (int)npc.position.Y);

					int type = ModContent.ProjectileType<OldDukeVortex>();
					int damage = npc.GetProjectileDamage(type);
					Vector2 vortexSpawn = vector + npc.velocity.RotatedBy(MathHelper.PiOver2 * -npc.direction) * spinTime / MathHelper.TwoPi;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.NewProjectile(vortexSpawn, Vector2.Zero, type, damage, 0f, Main.myPlayer, vortexSpawn.X, vortexSpawn.Y);
					}
				}

				if (npc.ai[2] % toothBallSpinPhaseDivisor == 0f)
				{
					if (npc.ai[2] % 45f == 0f && npc.ai[2] != 0f)
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/OldDukeVomit"), (int)npc.position.X, (int)npc.position.Y);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector10 = Vector2.Normalize(npc.velocity) * (npc.width + 20) / 2f + vector;
						int num31 = NPC.NewNPC((int)vector10.X, (int)vector10.Y + 45, ModContent.NPCType<OldDukeToothBall>());
						Main.npc[num31].target = npc.target;
						Main.npc[num31].velocity = Vector2.Normalize(npc.velocity).RotatedBy(MathHelper.PiOver2 * npc.direction) * toothBallSpinToothBallVelocity;
						Main.npc[num31].netUpdate = true;
						Main.npc[num31].ai[3] = Main.rand.Next(30, 361);
					}
				}

				// Velocity and rotation
				npc.velocity = npc.velocity.RotatedBy(-(double)num15 * (float)npc.direction);
				npc.rotation -= num15 * npc.direction;

				npc.ai[2] += 1f;
				if (npc.ai[2] >= num13)
				{
					calamityGlobalNPC.newAI[0] += 30f;
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
			}
		}
		#endregion

		// This AI is for testing purposes, don't remove it
		#region Brimstone Elemental2
		public static void BrimstoneElementalAI2(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Used for Brimling AI states
			CalamityGlobalNPC.brimstoneElemental = npc.whoAmI;

			// Center
			Vector2 vectorCenter = npc.Center;

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];
			if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 5600f)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 5600f)
				{
					npc.rotation = npc.velocity.X * 0.04f;

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					if (npc.ai[0] != 0f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.localAI[0] = 0f;
						npc.localAI[1] = 0f;
						npc.netUpdate = true;
					}
					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			CalamityPlayer modPlayer = player.Calamity();

			// Reset defense
			npc.defense = npc.defDefense;

			// Hide the sprite
			npc.alpha = 255;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Variables for buffing the AI
			bool provy = CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool calamity = modPlayer.ZoneCalamity;
			bool phase2 = (lifeRatio < 0.5f && revenge) || death;

			// Speed while moving in phase 1
			float speed = expertMode ? 5f : 4.5f;
			if (BossRushEvent.BossRushActive)
				speed = 12f;
			else if (!calamity)
				speed = 7f;
			else if (death)
				speed = 6f;
			else if (revenge)
				speed = 5.5f;
			float speedBoost = death ? 2f : 2f * (1f - lifeRatio);
			speed += speedBoost;

			int damage = expertMode ? 125 : 200;
			float radialOffset = 32f;
			float xDistance = player.Center.X - vectorCenter.X;
			float yDistance = player.Center.Y - vectorCenter.Y;
			float totalDistance = (float)Math.Sqrt(xDistance * xDistance + yDistance * yDistance);

			// Static movement towards target
			if (npc.ai[0] == 5f)
			{
				npc.rotation = npc.velocity.X * 0.04f;
				if (npc.ai[1] < 180f)
				{
					float playerLocation = vectorCenter.X - player.Center.X;
					npc.direction = playerLocation < 0f ? 1 : -1;
					npc.spriteDirection = npc.direction;
					totalDistance = speed * 0.15f / totalDistance;
					xDistance *= totalDistance;
					yDistance *= totalDistance;
					npc.velocity.X = (npc.velocity.X * 50f + xDistance) / 51f;
					npc.velocity.Y = (npc.velocity.Y * 50f + yDistance) / 51f;
				}
			}

			if (npc.ai[0] == 0f)
			{
				npc.chaseable = true;
				npc.rotation = npc.velocity.X * 0.04f;
				float playerLocation = vectorCenter.X - player.Center.X;
				npc.direction = playerLocation < 0f ? 1 : -1;
				npc.spriteDirection = npc.direction;
				npc.ai[1] += 1f;

				bool shootProjectile = false;
				float fireRate = 60f;
				if (npc.ai[1] % fireRate == fireRate - 1f)
					shootProjectile = true;

				if (shootProjectile)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						float num742 = 16f;
						float num743 = player.position.X + player.width * 0.5f - vectorCenter.X;
						float num744 = player.position.Y + player.height * 0.5f - vectorCenter.Y;
						float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

						num745 = num742 / num745;
						num743 *= num745;
						num744 *= num745;

						vectorCenter.X += num743 * 3f;
						vectorCenter.Y += num744 * 3f;

						float radians = MathHelper.TwoPi / 3f;

						for (int i = 0; i < 3; i++)
						{
							Vector2 vector = new Vector2(0f, -3f).RotatedBy(radians * i);
							int proj = Projectile.NewProjectile(vectorCenter, vector, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 8f, 0f);
							Main.projectile[proj].scale = 4f;
						}

						//Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, num743, num744, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 4f, 0f);

						int numProj = 4;
						int spread = 45;
						float rotation = MathHelper.ToRadians(spread);
						float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
						double startAngle = Math.Atan2(num743, num744) - rotation / 2;
						double deltaAngle = rotation / numProj;
						double offsetAngle;

						for (int i = 0; i < numProj; i++)
						{
							offsetAngle = startAngle + deltaAngle * i;
							Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 10f, 0f);
						}
					}
				}

				if (npc.position.Y > player.position.Y - 150f) //200
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y -= BossRushEvent.BossRushActive ? 0.15f : 0.1f;

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
				}
				else if (npc.position.Y < player.position.Y - 350f) //500
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y += BossRushEvent.BossRushActive ? 0.15f : 0.1f;

					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
				}
				if (npc.position.X + (npc.width / 2) > player.position.X + (player.width / 2) + 150f) //100
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X *= 0.985f;

					npc.velocity.X -= BossRushEvent.BossRushActive ? 0.15f : 0.1f;

					if (npc.velocity.X > 8f)
						npc.velocity.X = 8f;
				}
				if (npc.position.X + (npc.width / 2) < player.position.X + (player.width / 2) - 150f) //100
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X *= 0.985f;

					npc.velocity.X += (BossRushEvent.BossRushActive ? 0.15f : 0.1f);

					if (npc.velocity.X < -8f)
						npc.velocity.X = -8f;
				}

				if (npc.ai[1] >= 300f)
				{
					npc.TargetClosest(true);
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}

			// Cocoon bullet hell
			else if (npc.ai[0] == 1f)
			{
				npc.defense = 99999;
				npc.chaseable = false;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[0] += 1f;

					float projectileSpeed = revenge ? 8f : 6f;
					if (BossRushEvent.BossRushActive)
						projectileSpeed = 12f;

					Vector2 targetVector = player.Center - vectorCenter;
					float offsetAngle = 0f;
					int totalProjectiles = 0;
					//int chains = 0;

					// Parabolic Arcs
					if (npc.localAI[0] % 60f == 0f)
					{
						radialOffset = 0.1f;
						float diameter = 90f;

						// Offset the arc so the central projectile(s) point towards the target
						targetVector.Y -= Math.Abs(targetVector.X) * radialOffset;

						// Velocity of the arc moving towards the target
						targetVector = Vector2.Normalize(targetVector) * projectileSpeed;

						// Offset the arc
						Vector2 velocity = targetVector;
						velocity.Normalize();
						velocity *= diameter;

						totalProjectiles = 16;
						int half = totalProjectiles / 2;
						float scalar = 0.2f;
						float velocityMult = (totalProjectiles - 1f) / 2f;
						offsetAngle = (float)Math.PI * radialOffset;

						for (int j = 0; j < totalProjectiles; j++)
						{
							float radians = j - velocityMult;
							Vector2 offset = velocity.RotatedBy(offsetAngle * radians, default);
							int proj = Projectile.NewProjectile(vectorCenter + offset, targetVector * (1f - (Math.Abs(radians) / half * scalar)), ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, -1f, 0f);
						}
					}

					// Arcs
					/*if (npc.localAI[0] % 60f == 0f)
					{
						radialOffset = 0.1f;
						float diameter = 120f;

						targetVector.Y -= Math.Abs(targetVector.X) * radialOffset;
						targetVector = Vector2.Normalize(targetVector) * projectileSpeed;

						Vector2 velocity = targetVector;
						velocity.Normalize();
						velocity *= diameter;

						totalProjectiles = 8;
						offsetAngle = (float)Math.PI * radialOffset;

						for (int j = 0; j < totalProjectiles; j++)
						{
							float radians = j - (totalProjectiles - 1f) / 2f;
							Vector2 offset = velocity.RotatedBy(offsetAngle * radians, default);
							int proj = Projectile.NewProjectile(vectorCenter + offset, targetVector, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, -1f, 0f);
						}
					}

					if (npc.localAI[0] % 120f == 0f)
					{
						totalProjectiles = 24;
						offsetAngle = 360 / totalProjectiles;
						for (int i = 0; i < totalProjectiles; i++)
						{
							Projectile.NewProjectile(vectorCenter, Vector2.Zero, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 15f, i * offsetAngle);
						}
						
						totalProjectiles = 16;
						chains = 4;
						float velocityTimer = 90f / (totalProjectiles / chains);
						double radians = MathHelper.TwoPi / totalProjectiles;
						int j = 0;

						for (int i = 0; i < totalProjectiles; i++)
						{
							Vector2 vector = new Vector2(0f, -3f).RotatedBy(radians * i, default);
							Projectile.NewProjectile(vectorCenter, vector, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 16f, j * velocityTimer);

							j++;
							if (j % 4 == 0)
								j = 0;
						}
					}

					totalProjectiles = 36;
					float divisor = 6f;
					chains = 3;
					float interval = totalProjectiles / chains * divisor;
					double patternInterval = Math.Floor(npc.localAI[0] / interval);

					if (patternInterval % 2 == 0)
					{
						if (npc.localAI[0] % divisor == 0f)
						{
							double radians = (patternInterval % 4 == 0 ? MathHelper.TwoPi + MathHelper.Pi / 3 : MathHelper.TwoPi) / chains;
							radialOffset = MathHelper.ToRadians(npc.ai[2]);

							for (int i = 0; i < chains; i++)
							{
								Vector2 vector = new Vector2(0f, -3f).RotatedBy(radians * i + radialOffset, default);
								Projectile.NewProjectile(vectorCenter, vector, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 3f, 0f);
							}

							// Radial offset
							npc.ai[2] += 5f;
						}
					}
					else
						npc.ai[2] = 0f;*/

					/*if (npc.localAI[0] % 180f == 0f)
					{
						totalProjectiles = 12;
						offsetAngle = 360 / totalProjectiles;

						for (int i = 0; i < totalProjectiles; i++)
						{
							Projectile.NewProjectile(vectorCenter, Vector2.Zero, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 0f, i * offsetAngle);
							Projectile.NewProjectile(vectorCenter, Vector2.Zero, ModContent.ProjectileType<DarkOrb>(), damage, 0f, Main.myPlayer, 12f, i * offsetAngle);
						}
					}*/
				}

				if (npc.velocity.Length() > 0f)
					npc.velocity = Vector2.Zero;

				npc.rotation = npc.velocity.X * 0.04f;

				float playerLocation = vectorCenter.X - player.Center.X;
				npc.direction = playerLocation < 0f ? 1 : -1;
				npc.spriteDirection = npc.direction;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 600f)
				{
					npc.TargetClosest(true);
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.localAI[0] = 0f;
					npc.netUpdate = true;
				}
			}

			// Laser beam attack
			else if (npc.ai[0] == 2f)
			{
				npc.defense = npc.defDefense * 3;

				Vector2 source = new Vector2(vectorCenter.X + (npc.spriteDirection > 0 ? 34f : -34f), vectorCenter.Y - 74f);
				Vector2 aimAt = player.Center + player.velocity * 20f;
				float aimResponsiveness = 0.4f;

				switch ((int)npc.ai[2])
				{
					case 0:
						break;
					case 1:
						aimResponsiveness = 0.25f;
						break;
					case 2:
						aimResponsiveness = 0.1f;
						break;
				}

				Vector2 aimVector = Vector2.Normalize(aimAt - source);
				if (aimVector.HasNaNs())
					aimVector = -Vector2.UnitY;
				aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(npc.velocity), aimResponsiveness));
				aimVector *= 6f;

				Vector2 laserVelocity = Vector2.Normalize(aimVector);
				if (laserVelocity.HasNaNs())
					laserVelocity = -Vector2.UnitY;

				calamityGlobalNPC.newAI[1] = laserVelocity.X;
				calamityGlobalNPC.newAI[2] = laserVelocity.Y;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 300f)
				{
					npc.TargetClosest(true);
					npc.ai[2] += 1f;
					npc.localAI[0] = 0f;
					npc.localAI[1] = 0f;
					if (npc.ai[2] >= 3f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						calamityGlobalNPC.newAI[0] = 0f;
					}
					else
					{
						npc.ai[1] = 0f;
						calamityGlobalNPC.newAI[0] = 0f;
					}
				}
				else if (npc.ai[1] >= 180f)
				{
					npc.velocity *= 0.95f;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (npc.ai[1] == 180f)
						{
							Main.PlaySound(SoundID.Zombie, (int)npc.position.X, (int)npc.position.Y, 104);
							Vector2 laserVelocity2 = new Vector2(npc.localAI[0], npc.localAI[1]);
							laserVelocity2.Normalize();

							int totalProjectiles = 4;
							float spread = MathHelper.ToRadians(90); // 30 degrees in radians = 0.523599
							double startAngle = Math.Atan2(laserVelocity2.X, laserVelocity2.Y) - spread / 2; // Where the projectiles start spawning at, don't change this
							double deltaAngle = spread / totalProjectiles; // Angle between each projectile, 0.04363325
							double offsetAngle;

							int i;
							for (i = 0; i < 2; i++)
							{
								offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + radialOffset * i;
								Projectile.NewProjectile(source.X, source.Y, (float)Math.Sin(offsetAngle), (float)Math.Cos(offsetAngle), ModContent.ProjectileType<BrimstoneRay>(), 40, 0f, Main.myPlayer, 1f, npc.whoAmI);
								Projectile.NewProjectile(source.X, source.Y, (float)-Math.Sin(offsetAngle), (float)-Math.Cos(offsetAngle), ModContent.ProjectileType<BrimstoneRay>(), 40, 0f, Main.myPlayer, 1f, npc.whoAmI);
							}
						}
					}
				}
				else
				{
					float playSoundTimer = 20f;
					if (npc.ai[1] < 150f)
					{
						switch ((int)npc.ai[2])
						{
							case 0:
								break;
							case 1:
								npc.ai[1] += 0.5f;
								playSoundTimer = 30f;
								break;
							case 2:
								npc.ai[1] += 1f;
								playSoundTimer = 40f;
								break;
						}
					}

					if (npc.ai[1] % playSoundTimer == 0f)
						Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 20);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (npc.ai[1] < 150f && calamityGlobalNPC.newAI[0] == 0f)
						{
							Projectile.NewProjectile(source, laserVelocity, ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 0f, npc.whoAmI);
							calamityGlobalNPC.newAI[0] = 1f;
						}
						else
						{
							if (npc.ai[1] == 150f)
							{
								npc.localAI[0] = laserVelocity.X;
								npc.localAI[1] = laserVelocity.Y;

								Vector2 laserVelocity2 = new Vector2(npc.localAI[0], npc.localAI[1]);
								laserVelocity2.Normalize();

								int totalProjectiles = 4;
								float spread = MathHelper.ToRadians(90); // 30 degrees in radians = 0.523599
								double startAngle = Math.Atan2(laserVelocity2.X, laserVelocity2.Y) - spread / 2; // Where the projectiles start spawning at, don't change this
								double deltaAngle = spread / totalProjectiles; // Angle between each projectile, 0.04363325
								double offsetAngle;

								int i;
								for (i = 0; i < 2; i++)
								{
									offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + radialOffset * i;
									Projectile.NewProjectile(source.X, source.Y, (float)Math.Sin(offsetAngle), (float)Math.Cos(offsetAngle), ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 1f, npc.whoAmI);
									Projectile.NewProjectile(source.X, source.Y, (float)-Math.Sin(offsetAngle), (float)-Math.Cos(offsetAngle), ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 1f, npc.whoAmI);
								}
							}
						}
					}
				}
			}
		}
		#endregion

		#region Gem Crawler AI
		public static void GemCrawlerAI(NPC npc, Mod mod, float speedDetect, float speedAdditive)
        {
            int num19 = 30;
            int num20 = 10;
            bool flag19 = false;
            bool flag20 = false;
            bool flag30 = false;
            if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction > 0) || (npc.velocity.X < 0f && npc.direction < 0)))
            {
                flag20 = true;
                npc.ai[3] += 1f;
            }
            if ((npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num19) | flag20)
            {
                npc.ai[3] += 1f;
                flag30 = true;
            }
            else if (npc.ai[3] > 0f)
            {
                npc.ai[3] -= 1f;
            }
            if (npc.ai[3] > (float)(num19 * num20))
            {
                npc.ai[3] = 0f;
            }
            if (npc.justHit)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] == (float)num19)
            {
                npc.netUpdate = true;
            }
            Vector2 npcPos = new Vector2(npc.Center.X, npc.Center.Y);
            float xDist = Main.player[npc.target].Center.X - npcPos.X;
            float yDist = Main.player[npc.target].Center.Y - npcPos.Y;
            float targetDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            if (targetDist < 200f && !flag30)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] < (float)num19)
            {
                npc.TargetClosest(true);
            }
            else
            {
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[0] += 1f;
                        if (npc.ai[0] >= 2f)
                        {
                            npc.direction *= -1;
                            npc.spriteDirection = -npc.direction;
                            npc.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 0f;
                }
                npc.directionY = -1;
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }
            float num6 = speedDetect; //5
            float num70 = speedAdditive; //0.05
            if (!flag19 && (npc.velocity.Y == 0f || npc.wet || (npc.velocity.X <= 0f && npc.direction > 0) || (npc.velocity.X >= 0f && npc.direction < 0)))
            {
                if (npc.velocity.X < -num6 || npc.velocity.X > num6)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < num6 && npc.direction == -1)
                {
                    npc.velocity.X = npc.velocity.X + num70;
                    if (npc.velocity.X > num6)
                    {
                        npc.velocity.X = num6;
                    }
                }
                else if (npc.velocity.X > -num6 && npc.direction == 1)
                {
                    npc.velocity.X = npc.velocity.X - num70;
                    if (npc.velocity.X < -num6)
                    {
                        npc.velocity.X = -num6;
                    }
                }
            }
            if (npc.velocity.Y >= 0f)
            {
                int num9 = 0;
                if (npc.velocity.X < 0f)
                {
                    num9 = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    num9 = 1;
                }
                Vector2 position = npc.position;
                position.X += npc.velocity.X;
                int x = (int)((position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num9)) / 16f);
                int y = (int)((position.Y + (float)npc.height - 1f) / 16f);
                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                }
                if (Main.tile[x, y - 1] == null)
                {
                    Main.tile[x, y - 1] = new Tile();
                }
                if (Main.tile[x, y - 2] == null)
                {
                    Main.tile[x, y - 2] = new Tile();
                }
                if (Main.tile[x, y - 3] == null)
                {
                    Main.tile[x, y - 3] = new Tile();
                }
                if (Main.tile[x, y + 1] == null)
                {
                    Main.tile[x, y + 1] = new Tile();
                }
                if ((float)(x * 16) < position.X + (float)npc.width && (float)(x * 16 + 16) > position.X && ((Main.tile[x, y].nactive() && !Main.tile[x, y].topSlope() && !Main.tile[x, y - 1].topSlope() && Main.tileSolid[(int)Main.tile[x, y].type] && !Main.tileSolidTop[(int)Main.tile[x, y].type]) || (Main.tile[x, y - 1].halfBrick() && Main.tile[x, y - 1].nactive())) && (!Main.tile[x, y - 1].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 1].type] || Main.tileSolidTop[(int)Main.tile[x, y - 1].type] || (Main.tile[x, y - 1].halfBrick() && (!Main.tile[x, y - 4].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 4].type] || Main.tileSolidTop[(int)Main.tile[x, y - 4].type]))) && (!Main.tile[x, y - 2].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 2].type] || Main.tileSolidTop[(int)Main.tile[x, y - 2].type]) && (!Main.tile[x, y - 3].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 3].type] || Main.tileSolidTop[(int)Main.tile[x, y - 3].type]) && (!Main.tile[x - num9, y - 3].nactive() || !Main.tileSolid[(int)Main.tile[x - num9, y - 3].type]))
                {
                    float npcBottom = (float)(y * 16);
                    if (Main.tile[x, y].halfBrick())
                    {
                        npcBottom += 8f;
                    }
                    if (Main.tile[x, y - 1].halfBrick())
                    {
                        npcBottom -= 8f;
                    }
                    if (npcBottom < position.Y + (float)npc.height)
                    {
                        float num13 = position.Y + (float)npc.height - npcBottom;
                        if (num13 <= 16.1f)
                        {
                            npc.gfxOffY += npc.position.Y + (float)npc.height - npcBottom;
                            npc.position.Y = npcBottom - (float)npc.height;
                            if (num13 < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
		}
		#endregion

		#region Dungeon Spirit AI
		public static void DungeonSpiritAI(NPC npc, Mod mod, float speed, float rotation, bool lantern = false)
        {
            npc.TargetClosest(true);
            Vector2 npcPos = new Vector2(npc.Center.X, npc.Center.Y);
            float xDist = Main.player[npc.target].Center.X - npcPos.X;
            float yDist = Main.player[npc.target].Center.Y - npcPos.Y;
            float targetDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            float homingSpeed = speed;

			if (lantern)
			{
				if (npc.localAI[0] < 85f)
				{
					homingSpeed = 0.1f;
					targetDist = homingSpeed / targetDist;
					xDist *= targetDist;
					yDist *= targetDist;
					npc.velocity = (npc.velocity * 100f + new Vector2(xDist, yDist)) / 101f;
					npc.localAI[0] += 1f;
					return;
				}

				npc.dontTakeDamage = false;
				npc.chaseable = true;
			}

            targetDist = homingSpeed / targetDist;
            xDist *= targetDist;
            yDist *= targetDist;
            npc.velocity.X = (npc.velocity.X * 100f + xDist) / 101f;
            npc.velocity.Y = (npc.velocity.Y * 100f + yDist) / 101f;

			if (lantern)
			{
				npc.rotation = npc.velocity.X * 0.08f;
				npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
			}
			else
				npc.rotation = (float)Math.Atan2((double)yDist, (double)xDist) + rotation;
        }
		#endregion

		#region Unicorn AI
		public static void UnicornAI(NPC npc, Mod mod, bool spin, float bounciness, float speedDetect, float speedAdditive, float bouncy1 = -8.5f, float bouncy2 = -7.5f, float bouncy3 = -7f, float bouncy4 = -6f, float bouncy5 = -8f)
        {
            bool DogPhase1 = npc.type == ModContent.NPCType<AngryDog>() && npc.life > npc.lifeMax * (CalamityWorld.death ? 0.9 : 0.5);
            bool DogPhase2 = npc.type == ModContent.NPCType<AngryDog>() && npc.life <= npc.lifeMax * (CalamityWorld.death ? 0.9 : 0.5);
            int num = 30;
            bool flag2 = false;
            bool flag3 = false;
            if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
            {
                flag2 = true;
                npc.ai[3] += 1f;
            }
            int num2 = DogPhase1 ? 10 : 4;
			if (!DogPhase1)
			{
				bool flag4 = npc.velocity.Y == 0f;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type && Math.Abs(npc.position.X - Main.npc[i].position.X) + Math.Abs(npc.position.Y - Main.npc[i].position.Y) < (float)npc.width)
					{
						if (npc.position.X < Main.npc[i].position.X)
						{
							npc.velocity.X -= 0.05f;
						}
						else
						{
							npc.velocity.X += 0.05f;
						}
						if (npc.position.Y < Main.npc[i].position.Y)
						{
							npc.velocity.Y -= 0.05f;
						}
						else
						{
							npc.velocity.Y += 0.05f;
						}
					}
				}
				if (flag4)
				{
					npc.velocity.Y = 0f;
				}
			}
            if (npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num || flag2)
            {
                npc.ai[3] += 1f;
                flag3 = true;
            }
            else if (npc.ai[3] > 0f)
            {
                npc.ai[3] -= 1f;
            }
            if (npc.ai[3] > (float)(num * num2))
            {
                npc.ai[3] = 0f;
            }
            if (npc.justHit)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] == (float)num)
            {
                npc.netUpdate = true;
            }
            Vector2 npcPos = new Vector2(npc.Center.X, npc.Center.Y);
            float xDist = Main.player[npc.target].Center.X - npcPos.X;
            float yDist = Main.player[npc.target].Center.Y - npcPos.Y;
            float targetDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
            if (targetDist < 200f && !flag3)
            {
                npc.ai[3] = 0f;
            }
			if (!DogPhase1)
			{
				if (npc.velocity.Y == 0f && Math.Abs(npc.velocity.X) > 3f && ((npc.Center.X < Main.player[npc.target].Center.X && npc.velocity.X > 0f) || (npc.Center.X > Main.player[npc.target].Center.X && npc.velocity.X < 0f)))
				{
					npc.velocity.Y -= bounciness;
					if (npc.type == ModContent.NPCType<DespairStone>())
					{
						Main.PlaySound(SoundID.Item, npc.Center, 14);
						for (int k = 0; k < 10; k++)
						{
							Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, -1f, 0, default, 1f);
						}
					}
					if (npc.type == ModContent.NPCType<Bohldohr>())
					{
						Main.PlaySound(SoundID.NPCHit, npc.Center, 7);
					}
					if (DogPhase2)
					{
						for (int k = 0; k < 5; k++)
						{
							Dust.NewDust(npc.position, npc.width, npc.height, 33, 0f, -1f, 0, default, 1f);
						}
					}
					if (npc.type == ModContent.NPCType<AquaticUrchin>() || npc.type == ModContent.NPCType<SeaUrchin>())
					{
						for (int k = 0; k < 5; k++)
						{
							Dust.NewDust(npc.position, npc.width, npc.height, 33, 0f, -1f, 0, default, 1f);
						}
					}
				}
			}
            if (npc.ai[3] < (float)num)
            {
                npc.TargetClosest(true);
            }
            else
            {
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[0] += 1f;
                        if (npc.ai[0] >= 2f)
                        {
                            npc.direction *= -1;
                            npc.spriteDirection = npc.direction;
                            npc.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 0f;
                }
                npc.directionY = -1;
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }

            if (npc.velocity.Y == 0f || npc.wet || (npc.velocity.X <= 0f && npc.direction < 0) || (npc.velocity.X >= 0f && npc.direction > 0))
            {
                if (Math.Sign(npc.velocity.X) != npc.direction && !DogPhase1)
                {
                    npc.velocity.X *= 0.92f;
                }
                float num9 = MathHelper.Lerp(0.6f, 1f, Math.Abs(Main.windSpeedSet)) * (float)Math.Sign(Main.windSpeedSet);
                if (!Main.player[npc.target].ZoneSandstorm)
                {
                    num9 = 0f;
                }
                float num7 = speedDetect;
                float num8 = speedAdditive;
                if (npc.velocity.X < -num7 || npc.velocity.X > num7)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < num7 && npc.direction == 1)
                {
                    npc.velocity.X += num8;
                    if (npc.velocity.X > num7)
                    {
                        npc.velocity.X = num7;
                    }
                }
                else if (npc.velocity.X > -num7 && npc.direction == -1)
                {
                    npc.velocity.X -= num8;
                    if (npc.velocity.X < -num7)
                    {
                        npc.velocity.X = -num7;
                    }
                }
            }
            if (npc.velocity.Y >= 0f)
            {
                int num10 = 0;
                if (npc.velocity.X < 0f)
                {
                    num10 = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    num10 = 1;
                }
                Vector2 position = npc.position;
                position.X += npc.velocity.X;
                int x = (int)((position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num10)) / 16f);
                int y = (int)((position.Y + (float)npc.height - 1f) / 16f);
                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                }
                if (Main.tile[x, y - 1] == null)
                {
                    Main.tile[x, y - 1] = new Tile();
                }
                if (Main.tile[x, y - 2] == null)
                {
                    Main.tile[x, y - 2] = new Tile();
                }
                if (Main.tile[x, y - 3] == null)
                {
                    Main.tile[x, y - 3] = new Tile();
                }
                if (Main.tile[x, y + 1] == null)
                {
                    Main.tile[x, y + 1] = new Tile();
                }
                if ((float)(x * 16) < position.X + (float)npc.width && (float)(x * 16 + 16) > position.X && ((Main.tile[x, y].nactive() && !Main.tile[x, y].topSlope() && !Main.tile[x, y - 1].topSlope() && Main.tileSolid[(int)Main.tile[x, y].type] && !Main.tileSolidTop[(int)Main.tile[x, y].type]) || (Main.tile[x, y - 1].halfBrick() && Main.tile[x, y - 1].nactive())) && (!Main.tile[x, y - 1].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 1].type] || Main.tileSolidTop[(int)Main.tile[x, y - 1].type] || (Main.tile[x, y - 1].halfBrick() && (!Main.tile[x, y - 4].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 4].type] || Main.tileSolidTop[(int)Main.tile[x, y - 4].type]))) && (!Main.tile[x, y - 2].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 2].type] || Main.tileSolidTop[(int)Main.tile[x, y - 2].type]) && (!Main.tile[x, y - 3].nactive() || !Main.tileSolid[(int)Main.tile[x, y - 3].type] || Main.tileSolidTop[(int)Main.tile[x, y - 3].type]) && (!Main.tile[x - num10, y - 3].nactive() || !Main.tileSolid[(int)Main.tile[x - num10, y - 3].type]))
                {
                    float num13 = (float)(y * 16);
                    if (Main.tile[x, y].halfBrick())
                    {
                        num13 += 8f;
                    }
                    if (Main.tile[x, y - 1].halfBrick())
                    {
                        num13 -= 8f;
                    }
                    if (num13 < position.Y + (float)npc.height)
                    {
                        float num14 = position.Y + (float)npc.height - num13;
                        if ((double)num14 <= 16.1)
                        {
                            npc.gfxOffY += npc.position.Y + (float)npc.height - num13;
                            npc.position.Y = num13 - (float)npc.height;
                            if (num14 < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
            if (npc.velocity.Y == 0f)
            {
                int num15 = (int)((npc.position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 2) * npc.direction) + npc.velocity.X * 5f) / 16f);
                int num16 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);
                if (Main.tile[num15, num16] == null)
                {
                    Main.tile[num15, num16] = new Tile();
                }
                if (Main.tile[num15, num16 - 1] == null)
                {
                    Main.tile[num15, num16 - 1] = new Tile();
                }
                if (Main.tile[num15, num16 - 2] == null)
                {
                    Main.tile[num15, num16 - 2] = new Tile();
                }
                if (Main.tile[num15, num16 - 3] == null)
                {
                    Main.tile[num15, num16 - 3] = new Tile();
                }
                if (Main.tile[num15, num16 + 1] == null)
                {
                    Main.tile[num15, num16 + 1] = new Tile();
                }
                if (Main.tile[num15 + npc.direction, num16 - 1] == null)
                {
                    Main.tile[num15 + npc.direction, num16 - 1] = new Tile();
                }
                if (Main.tile[num15 + npc.direction, num16 + 1] == null)
                {
                    Main.tile[num15 + npc.direction, num16 + 1] = new Tile();
                }
                if (Main.tile[num15 - npc.direction, num16 + 1] == null)
                {
                    Main.tile[num15 - npc.direction, num16 + 1] = new Tile();
                }
                int num17 = npc.spriteDirection;
                num17 *= -1;
                if ((npc.velocity.X < 0f && num17 == -1) || (npc.velocity.X > 0f && num17 == 1))
                {
                    float num18 = 3f;
                    if (Main.tile[num15, num16 - 2].nactive() && Main.tileSolid[(int)Main.tile[num15, num16 - 2].type])
                    {
                        if (Main.tile[num15, num16 - 3].nactive() && Main.tileSolid[(int)Main.tile[num15, num16 - 3].type])
                        {
                            npc.velocity.Y = bouncy1;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            npc.velocity.Y = bouncy2;
                            npc.netUpdate = true;
                        }
                    }
                    else if (Main.tile[num15, num16 - 1].nactive() && !Main.tile[num15, num16 - 1].topSlope() && Main.tileSolid[(int)Main.tile[num15, num16 - 1].type])
                    {
                        npc.velocity.Y = bouncy3;
                        npc.netUpdate = true;
                    }
                    else if (npc.position.Y + (float)npc.height - (float)(num16 * 16) > 20f && Main.tile[num15, num16].nactive() && !Main.tile[num15, num16].topSlope() && Main.tileSolid[(int)Main.tile[num15, num16].type])
                    {
                        npc.velocity.Y = bouncy4;
                        npc.netUpdate = true;
                    }
                    else if ((npc.directionY < 0 || Math.Abs(npc.velocity.X) > num18) && (!Main.tile[num15, num16 + 1].nactive() || !Main.tileSolid[(int)Main.tile[num15, num16 + 1].type]) && (!Main.tile[num15, num16 + 2].nactive() || !Main.tileSolid[(int)Main.tile[num15, num16 + 2].type]) && (!Main.tile[num15 + npc.direction, num16 + 3].nactive() || !Main.tileSolid[(int)Main.tile[num15 + npc.direction, num16 + 3].type]))
                    {
                        npc.velocity.Y = bouncy5;
                        npc.netUpdate = true;
                    }
                }
            }
			if (spin)
			{
				npc.rotation += npc.velocity.X * 0.05f;
				npc.spriteDirection = -npc.direction;
			}
		}
		#endregion

		#region Swimming AI
		public static void PassiveSwimmingAI(NPC npc, Mod mod, int passiveness, float detectRange, float xSpeed, float ySpeed, float speedLimitX, float speedLimitY, float rotation, bool spriteFacesLeft = true)
        {
			if (spriteFacesLeft)
				npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
			else
				npc.spriteDirection = (npc.direction > 0) ? -1 : 1;

            npc.noGravity = true;
            if (npc.direction == 0)
            {
                npc.TargetClosest(true);
            }
            if (npc.justHit && passiveness != 3)
            {
                npc.chaseable = true;
            }
            if (npc.wet)
            {
                bool flag14 = npc.chaseable;
                npc.TargetClosest(false);
				if (passiveness != 2)
				{
					if (npc.type == ModContent.NPCType<Frogfish>())
					{
						if (Main.player[npc.target].wet && !Main.player[npc.target].dead)
						{
							flag14 = true;
							npc.chaseable = true; //once the enemy has detected the player, let minions fuck it up
						}
					}
					if (npc.type == ModContent.NPCType<Flounder>())
					{
						if (!Main.player[npc.target].dead)
						{
							flag14 = true;
							npc.chaseable = true; //once the enemy has detected the player, let minions fuck it up
						}
					}
					else if (Main.player[npc.target].wet && !Main.player[npc.target].dead &&
						(Main.player[npc.target].Center - npc.Center).Length() < detectRange &&
						Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						flag14 = true;
						npc.chaseable = true; //once the enemy has detected the player, let minions fuck it up
					}
					else
					{
						if (passiveness == 1)
						{
							flag14 = false;
						}
					}
				}
                if ((Main.player[npc.target].dead || !Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height)) && flag14)
                {
                    flag14 = false;
                }
                if (!flag14 || passiveness == 0)
                {
					if (passiveness == 0)
						npc.TargetClosest(true);
                    if (npc.collideX)
                    {
                        npc.velocity.X = npc.velocity.X * -1f;
                        npc.direction *= -1;
                        npc.netUpdate = true;
                    }
                    if (npc.collideY)
                    {
                        npc.netUpdate = true;
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                            npc.directionY = -1;
                            npc.ai[0] = -1f;
                        }
                        else if (npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y);
                            npc.directionY = 1;
                            npc.ai[0] = 1f;
                        }
                    }
                }
                if (flag14 && passiveness != 2)
                {
                    npc.TargetClosest(true);
					if (passiveness == 3)
					{
						npc.velocity.X = npc.velocity.X - (float)npc.direction * xSpeed;
						npc.velocity.Y = npc.velocity.Y - (float)npc.directionY * ySpeed;
					}
					else
					{
						npc.velocity.X = npc.velocity.X + (float)npc.direction * (CalamityWorld.death ? 2f * xSpeed : xSpeed);
						npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * (CalamityWorld.death ? 2f * ySpeed : ySpeed);
					}
					float velocityCapX = CalamityWorld.death && passiveness != 3 ? 2f * speedLimitX : speedLimitX;
					float velocityCapY = CalamityWorld.death && passiveness != 3 ? 2f * speedLimitY : speedLimitY;
                    npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -velocityCapX, velocityCapX);
                    npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -velocityCapY, velocityCapY);
					if (npc.type == ModContent.NPCType<Laserfish>())
					{
						npc.localAI[0] += (CalamityWorld.death ? 2f : 1f);
						if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 120f)
						{
							npc.localAI[0] = 0f;
							if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
							{
								float speed = 5f;
								Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
								float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X + (float)Main.rand.Next(-20, 21);
								float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-20, 21);
								float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
								num8 = speed / num8;
								num6 *= num8;
								num7 *= num8;
								int damage = 50;
								if (Main.expertMode)
								{
									damage = 40;
								}
								int beam = Projectile.NewProjectile(npc.Center.X + (npc.spriteDirection == 1 ? 25f : -25f), npc.Center.Y + (Main.player[npc.target].position.Y > npc.Center.Y ? 5f : -5f), num6, num7, ProjectileID.EyeBeam, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[beam].tileCollide = true;
							}
						}
					}
					if (npc.type == ModContent.NPCType<Flounder>())
					{
                        if ((Main.player[npc.target].Center - npc.Center).Length() < 350f)
                        {
                            npc.localAI[0] += (CalamityWorld.death ? 3f : 1f);
                            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 180f)
                            {
                                npc.localAI[0] = 0f;
                                if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                                {
                                    float speed = 4f;
                                    Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                                    float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X + (float)Main.rand.Next(-20, 21);
                                    float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-20, 21);
                                    float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                                    num8 = speed / num8;
                                    num6 *= num8;
                                    num7 *= num8;
                                    int damage = 35;
                                    if (Main.expertMode)
                                    {
                                        damage = 25;
                                    }
                                    int beam = Projectile.NewProjectile(npc.Center.X + (npc.spriteDirection == 1 ? 10f : -10f), npc.Center.Y, num6, num7, ModContent.ProjectileType<SulphuricAcidMist>(), damage, 0f, Main.myPlayer, 0f, 0f);
                                }
                            }
                        }
					}
					if (npc.type == ModContent.NPCType<SeaMinnow>())
					{
						npc.direction *= -1;
					}
                }
                else
                {
                    npc.velocity.X += (float)npc.direction * 0.1f;
                    if (npc.velocity.X < -2.5f || npc.velocity.X > 2.5f)
                    {
                        npc.velocity.X *= 0.95f;
                    }
                    if (npc.ai[0] == -1f)
                    {
                        npc.velocity.Y -= 0.01f;
                        if (npc.velocity.Y < -0.3f)
                        {
                            npc.ai[0] = 1f;
                        }
                    }
                    else
                    {
                        npc.velocity.Y += 0.01f;
                        if (npc.velocity.Y > 0.3f)
                        {
                            npc.ai[0] = -1f;
                        }
                    }
                }
                int num258 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                int num259 = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                if (Main.tile[num258, num259 - 1] == null)
                {
                    Main.tile[num258, num259 - 1] = new Tile();
                }
                if (Main.tile[num258, num259 + 1] == null)
                {
                    Main.tile[num258, num259 + 1] = new Tile();
                }
                if (Main.tile[num258, num259 + 2] == null)
                {
                    Main.tile[num258, num259 + 2] = new Tile();
                }
                if (Main.tile[num258, num259 - 1].liquid > 128)
                {
                    if (Main.tile[num258, num259 + 1].active())
                    {
                        npc.ai[0] = -1f;
                    }
                    else if (Main.tile[num258, num259 + 2].active())
                    {
                        npc.ai[0] = -1f;
                    }
                }
                if (npc.velocity.Y > 0.4f || npc.velocity.Y < -0.4f)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.95f;
                }
            }
            else
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.94f;
                    if (npc.velocity.X > -0.2f && npc.velocity.X < 0.2f)
                    {
                        npc.velocity.X = 0f;
                    }
                }
                npc.velocity.Y = npc.velocity.Y + 0.4f;
                if (npc.velocity.Y > 12f)
                {
                    npc.velocity.Y = 12f;
                }
                npc.ai[0] = 1f;
            }
            npc.rotation = npc.velocity.Y * (float)npc.direction * rotation;
			float rotationLimit = 2f * rotation;
			npc.rotation = MathHelper.Clamp(npc.rotation, -rotationLimit, rotationLimit);
		}
		#endregion
	}
}
