using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.Providence
{
	[AutoloadBossHead]
	public class Providence : ModNPC
	{
		private bool text = false;
		private bool useDefenseFrames = false;
		private float bossLife;
		private int biomeType = 0;
		private int flightPath = 0;
		private int phaseChange = 0;
		private int immuneTimer = 300;
		private int frameUsed = 0;
		private int healTimer = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Providence, the Profaned Goddess");
			Main.npcFrameCount[npc.type] = 3;
		}

		public override void SetDefaults()
		{
			npc.npcSlots = 36f;
			npc.damage = 100;
			npc.width = 600;
			npc.height = 450;
			npc.defense = 50;
			npc.lifeMax = CalamityWorld.revenge ? 500000 : 440000;
			if (CalamityWorld.death)
			{
				npc.lifeMax = 715000;
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 15000000 : 12500000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			npc.knockBackResist = 0f;
			npc.aiStyle = -1; //new
			aiType = -1; //new
			npc.value = Item.buyPrice(0, 50, 0, 0);
			npc.boss = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.buffImmune[BuffID.Ichor] = false;
			npc.buffImmune[BuffID.CursedInferno] = false;
			npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
			npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
			npc.buffImmune[mod.BuffType("DemonFlames")] = false;
			npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
			npc.buffImmune[mod.BuffType("Nightwither")] = false;
			npc.buffImmune[mod.BuffType("Shred")] = false;
			npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
			npc.buffImmune[mod.BuffType("SilvaStun")] = false;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.netAlways = true;
			npc.chaseable = true;
			npc.canGhostHeal = false;
			Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
			if (calamityModMusic != null)
				music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/ProvidenceTheme");
			else
				music = MusicID.LunarBoss;
			npc.HitSound = SoundID.NPCHit44;
			npc.DeathSound = SoundID.NPCDeath46;
			bossBag = mod.ItemType("ProvidenceBag");
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(text);
			writer.Write(useDefenseFrames);
			writer.Write(biomeType);
			writer.Write(phaseChange);
			writer.Write(immuneTimer);
			writer.Write(frameUsed);
			writer.Write(healTimer);
			writer.Write(flightPath);
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.chaseable);
			writer.Write(npc.canGhostHeal);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			text = reader.ReadBoolean();
			useDefenseFrames = reader.ReadBoolean();
			biomeType = reader.ReadInt32();
			phaseChange = reader.ReadInt32();
			immuneTimer = reader.ReadInt32();
			frameUsed = reader.ReadInt32();
			healTimer = reader.ReadInt32();
			flightPath = reader.ReadInt32();
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.chaseable = reader.ReadBoolean();
			npc.canGhostHeal = reader.ReadBoolean();
		}

		public override void AI()
		{
			npc.rotation = npc.velocity.X * 0.004f;
			CalamityGlobalNPC.holyBoss = npc.whoAmI;

			Player player = Main.player[npc.target];
			Vector2 vector = npc.Center;
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool isHoly = player.ZoneHoly;
			bool isHell = player.ZoneUnderworldHeight;
			bool normalAttackRate = true;
			bool ignoreGuardianAmt = (double)npc.life <= (double)npc.lifeMax * 0.15;
			bool phase2 = (double)npc.life <= (double)npc.lifeMax * 0.75;
			bool phase3 = (double)npc.life <= (double)npc.lifeMax * 0.5;

			if (Vector2.Distance(player.Center, vector) > 2800f)
			{
				if (!player.dead && player.active)
					player.AddBuff(mod.BuffType("HolyInferno"), 2);
			}

			int guardianAmt = 0;
			bool healerAlive = false;
			if (CalamityGlobalNPC.holyBossAttacker != -1)
			{
				if (Main.npc[CalamityGlobalNPC.holyBossAttacker].active)
					guardianAmt++;
			}
			if (CalamityGlobalNPC.holyBossDefender != -1)
			{
				if (Main.npc[CalamityGlobalNPC.holyBossDefender].active)
					guardianAmt++;
			}
			if (CalamityGlobalNPC.holyBossHealer != -1)
			{
				if (Main.npc[CalamityGlobalNPC.holyBossHealer].active)
				{
					guardianAmt++;
					healerAlive = true;
				}
			}
			if (guardianAmt > 0)
				normalAttackRate = ignoreGuardianAmt;
			npc.chaseable = normalAttackRate && npc.ai[0] != 2f && npc.ai[0] != 5f && npc.ai[0] != 7f;
			npc.canGhostHeal = npc.chaseable;

			CalamityMod.StopRain();

			if (biomeType == 0)
			{
				if (isHoly)
					biomeType = 1;
				else
					biomeType = 2;
			}

			if (!isHoly && !isHell && !CalamityWorld.bossRushActive)
			{
				if (immuneTimer > 0)
					immuneTimer--;
			}
			else
				immuneTimer = 300;

			npc.dontTakeDamage = (immuneTimer <= 0);

			if (healerAlive)
			{
				float heal = revenge ? 90f : 120f;
				if (CalamityWorld.death || CalamityWorld.bossRushActive)
					heal = 30f;

				switch (guardianAmt)
				{
					case 1:
						heal *= 2f;
						break;
					case 2:
						break;
					case 3:
						heal *= 0.5f;
						break;
					default:
						break;
				}

				healTimer++;
				if (healTimer >= heal)
				{
					healTimer = 0;
					if (Main.netMode != 1)
					{
						int healAmt = npc.lifeMax / 200;
						if (healAmt > npc.lifeMax - npc.life)
							healAmt = npc.lifeMax - npc.life;

						if (healAmt > 0)
						{
							npc.life += healAmt;
							npc.HealEffect(healAmt, true);
							npc.netUpdate = true;
						}
					}
				}
			}

			if ((!Main.dayTime && npc.ai[0] != 2f && npc.ai[0] != 5f && npc.ai[0] != 7f) || player.dead)
			{
				if (npc.timeLeft > 10)
					npc.timeLeft = 10;

				if (npc.velocity.X > 0f)
					npc.velocity.X = npc.velocity.X + 0.25f;
				else
					npc.velocity.X = npc.velocity.X - 0.25f;

				npc.velocity.Y = npc.velocity.Y - 0.25f;
			}
			else if (npc.timeLeft < 3600)
				npc.timeLeft = 3600;

			if (bossLife == 0f && npc.life > 0)
				bossLife = (float)npc.lifeMax;

			if (npc.life > 0)
			{
				if (Main.netMode != 1)
				{
					int num660 = (int)((double)npc.lifeMax * 0.66);
					if ((float)(npc.life + num660) < bossLife)
					{
						bossLife = (float)npc.life;
						int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
						int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));
						NPC.NewNPC(x - 100, y - 100, mod.NPCType("ProvSpawnDefense"), 0, 0f, 0f, 0f, 0f, 255);
						NPC.NewNPC(x + 100, y - 100, mod.NPCType("ProvSpawnHealer"), 0, 0f, 0f, 0f, 0f, 255);
						NPC.NewNPC(x, y + 100, mod.NPCType("ProvSpawnOffense"), 0, 0f, 0f, 0f, 0f, 255);
					}
				}
			}

			if (npc.ai[0] != 2f && npc.ai[0] != 5f)
			{
				bool firingLaser = npc.ai[0] == 7f;

				if (flightPath == 0)
				{
					npc.TargetClosest(true);
					if (npc.Center.X < player.Center.X)
						flightPath = 1;
					else
						flightPath = -1;
				}

				npc.TargetClosest(true);
				int num851 = 800;
				float num852 = Math.Abs(npc.Center.X - player.Center.X);

				if (npc.Center.X < player.Center.X && flightPath < 0 && num852 > (float)num851)
					flightPath = 0;
				if (npc.Center.X > player.Center.X && flightPath > 0 && num852 > (float)num851)
					flightPath = 0;

				float num853 = expertMode ? 1.1f : 1.05f;
				float num854 = expertMode ? 16f : 15f;
				if ((double)npc.life < (double)npc.lifeMax * 0.75)
				{
					num853 = expertMode ? 1.15f : 1.1f;
					num854 = expertMode ? 17f : 16f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.5)
				{
					num853 = expertMode ? 1.2f : 1.15f;
					num854 = expertMode ? 18f : 17f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.25)
				{
					num853 = expertMode ? 1.25f : 1.2f;
					num854 = expertMode ? 19f : 18f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
				{
					num853 = expertMode ? 1.3f : 1.25f;
					num854 = expertMode ? 20f : 19f;
				}
				if (firingLaser)
				{
					num854 *= normalAttackRate ? 0.5f : 0.25f;
					num853 *= normalAttackRate ? 0.5f : 0.25f;
				}

				npc.velocity.X = npc.velocity.X + (float)flightPath * num853;
				if (npc.velocity.X > num854)
					npc.velocity.X = num854;
				if (npc.velocity.X < -num854)
					npc.velocity.X = -num854;

				float num855 = player.position.Y - (npc.position.Y + (float)npc.height);
				if (num855 < (firingLaser ? 150f : 200f)) //150
					npc.velocity.Y = npc.velocity.Y - 0.2f;
				if (num855 > (firingLaser ? 200f : 250f)) //200
					npc.velocity.Y = npc.velocity.Y + 0.2f;

				float speedVariance = normalAttackRate ? 3f : 1.5f;
				if (npc.velocity.Y > (firingLaser ? speedVariance : 6f))
					npc.velocity.Y = firingLaser ? speedVariance : 6f;
				if (npc.velocity.Y < (firingLaser ? -speedVariance : -6f))
					npc.velocity.Y = firingLaser ? -speedVariance : -6f;
			}

			if (npc.ai[0] == -1f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				phaseChange++;
				if (phaseChange > 14)
					phaseChange = 0;

				int phase = 0; //0 = blasts 1 = holy fire 2 = shell heal 3 = molten blobs 4 = holy bombs 5 = shell spears 6 = crystal 7 = laser

				if (CalamityWorld.death || CalamityWorld.bossRushActive)
				{
					switch (phaseChange)
					{
						case 0: phase = 4; break; //1575 or 1500
						case 1: phase = 5; break; //1875 or 1800
						case 2: phase = 0; break; //2175 or 2100
						case 3: phase = phase2 ? 6 : 1; break;
						case 4: phase = 2; break; //600
						case 5: phase = 4; break; //900
						case 6: phase = 1; break; //1200
						case 7: phase = 5; break; //1500
						case 8:
							phase = phase3 ? 7 : 3; //1875 or 1800
							if (phase3)
							{
								npc.TargetClosest(false);
								Vector2 v3 = player.Center - npc.Center - new Vector2(0f, -22f);
								float num1219 = v3.Length() / 500f;
								if (num1219 > 1f)
									num1219 = 1f;
								num1219 = 1f - num1219;
								num1219 *= 2f;
								if (num1219 > 1f)
									num1219 = 1f;

								npc.localAI[0] = v3.ToRotation();
								npc.localAI[1] = num1219;
							}
							break;
						case 9: phase = 3; break; //2175 or 2100
						case 10: phase = phase2 ? 6 : 2; break;
						case 11: phase = 4; break; //300
						case 12:
							phase = phase3 ? 7 : 4; //675 or 600
							if (phase3)
							{
								npc.TargetClosest(false);
								Vector2 v3 = player.Center - npc.Center - new Vector2(0f, -22f);
								float num1219 = v3.Length() / 500f;
								if (num1219 > 1f)
									num1219 = 1f;
								num1219 = 1f - num1219;
								num1219 *= 2f;
								if (num1219 > 1f)
									num1219 = 1f;

								npc.localAI[0] = v3.ToRotation();
								npc.localAI[1] = num1219;
							}
							break;
						case 13: phase = 5; break; //975 or 900
						case 14: phase = 0; break; //1275 or 1200
						default: break;
					}
				}
				else
				{
					switch (phaseChange)
					{
						case 0: phase = 0; break; //3375 or 3300
						case 1:
							phase = phase3 ? 7 : 1; //3750 or 3600
							if (phase3)
							{
								npc.TargetClosest(false);
								Vector2 v3 = player.Center - npc.Center - new Vector2(0f, -22f);
								float num1219 = v3.Length() / 500f;
								if (num1219 > 1f)
									num1219 = 1f;
								num1219 = 1f - num1219;
								num1219 *= 2f;
								if (num1219 > 1f)
									num1219 = 1f;

								npc.localAI[0] = v3.ToRotation();
								npc.localAI[1] = num1219;
							}
							break;
						case 2: phase = 3; break; //4050 or 3900
						case 3: phase = 4; break; //4350 or 4200
						case 4: phase = 5; break; //4650 or 4500
						case 5: phase = phase2 ? 6 : 4; break;
						case 6: phase = 3; break; //300
						case 7: phase = 1; break; //600
						case 8: phase = 0; break; //900
						case 9: phase = 2; break; //1500
						case 10: phase = 4; break; //1800
						case 11:
							phase = phase3 ? 7 : 0; //2175 or 2100
							if (phase3)
							{
								npc.TargetClosest(false);
								Vector2 v3 = player.Center - npc.Center - new Vector2(0f, -22f);
								float num1219 = v3.Length() / 500f;
								if (num1219 > 1f)
									num1219 = 1f;
								num1219 = 1f - num1219;
								num1219 *= 2f;
								if (num1219 > 1f)
									num1219 = 1f;

								npc.localAI[0] = v3.ToRotation();
								npc.localAI[1] = num1219;
							}
							break;
						case 12: phase = 3; break; //2475 or 2400
						case 13: phase = 1; break; //2775 or 2700
						case 14: phase = 5; break; //3075 or 3000
						default: break;
					}
				}

				npc.TargetClosest(true);
				if (Math.Abs(npc.Center.X - player.Center.X) > 5600f)
					phase = 0;

				npc.ai[0] = (float)phase;
				npc.ai[1] = 0f;
				npc.ai[2] = 0f;
				npc.ai[3] = 0f;
			}
			else if (npc.ai[0] == 0f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				float num852 = Math.Abs(npc.Center.X - player.Center.X);
				if ((num852 < 500f || npc.ai[3] < 0f) && npc.position.Y < player.position.Y)
				{
					npc.ai[3] += 1f;
					int num856 = expertMode ? 25 : 26;
					if ((double)npc.life < (double)npc.lifeMax * 0.5)
						num856 = expertMode ? 23 : 24;
					if ((double)npc.life <= (double)npc.lifeMax * 0.1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
						num856 = expertMode ? 21 : 22;
					if (!normalAttackRate)
						num856 = expertMode ? 40 : 46;

					num856++;
					if (npc.ai[3] > (float)num856)
						npc.ai[3] = (float)(-(float)num856);

					if (npc.ai[3] == 0f && Main.netMode != 1)
					{
						Vector2 vector112 = new Vector2(npc.Center.X, npc.Center.Y);
						vector112.X += npc.velocity.X * 7f;
						float num857 = player.position.X + (float)player.width * 0.5f - vector112.X;
						float num858 = player.Center.Y - vector112.Y;
						float num859 = (float)Math.Sqrt((double)(num857 * num857 + num858 * num858));

						float num860 = expertMode ? 10.25f : 9f;
						if ((double)npc.life < (double)npc.lifeMax * 0.5)
							num860 = expertMode ? 11.5f : 10f;
						if ((double)npc.life <= (double)npc.lifeMax * 0.1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
							num860 = expertMode ? 12.75f : 11f;
						if (revenge)
							num860 *= 1.15f;

						num859 = num860 / num859;
						num857 *= num859;
						num858 *= num859;

						int holyDamage = expertMode ? 46 : 63;
						Projectile.NewProjectile(vector112.X, vector112.Y, num857, num858, mod.ProjectileType("HolyBlast"), holyDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				}
				else if (npc.ai[3] < 0f)
					npc.ai[3] += 1f;

				if (Main.netMode != 1)
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] > 300f)
						npc.ai[0] = -1f;
				}
			}
			else if (npc.ai[0] == 1f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				if (Main.netMode != 1)
				{
					npc.ai[3] += 1f;
					int num864 = expertMode ? 33 : 36;
					if ((double)npc.life < (double)npc.lifeMax * 0.5)
						num864 = expertMode ? 30 : 33;
					if ((double)npc.life <= (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
						num864 = expertMode ? 26 : 29;
					if (!normalAttackRate)
						num864 = expertMode ? 45 : 52;
					num864 += 3;

					if (npc.ai[3] >= (float)num864)
					{
						npc.ai[3] = 0f;
						Vector2 vector113 = new Vector2(npc.Center.X, npc.position.Y + (float)npc.height - 14f);
						float num865 = npc.velocity.Y;
						if (num865 < 0f)
							num865 = 0f;
						num865 += expertMode ? 4f : 3f;

						int fireDamage = expertMode ? 40 : 59; //260 200
						Projectile.NewProjectile(vector113.X, vector113.Y, npc.velocity.X * 0.25f, num865, mod.ProjectileType("HolyFire"), fireDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				}

				if (Main.netMode != 1)
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] > 300f)
						npc.ai[0] = -1f;
				}
			}
			else if (npc.ai[0] == 2f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;
				npc.TargetClosest(true);

				Vector2 vector114 = new Vector2(npc.Center.X, npc.Center.Y - 20f);
				float num866 = (float)Main.rand.Next(-1000, 1001);
				float num867 = (float)Main.rand.Next(-1000, 1001);
				float num868 = (float)Math.Sqrt((double)(num866 * num866 + num867 * num867));
				float num869 = 3f;
				npc.velocity *= 0.95f;

				num868 = num869 / num868;
				num866 *= num868;
				num867 *= num868;
				vector114.X += num866 * 4f;
				vector114.Y += num867 * 4f;

				npc.ai[3] += 1f;
				int num870 = expertMode ? 3 : 4;
				if (!normalAttackRate)
					num870 = expertMode ? 12 : 16;
				if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
					num870 -= 2;
				if ((double)npc.life <= (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
					num870 -= 2;

				if (npc.ai[3] > (float)num870)
				{
					npc.ai[3] = 0f;
					if (Main.netMode != 1)
					{
						if (Main.rand.Next(4) == 0 && !CalamityWorld.death && !CalamityWorld.bossRushActive)
							Projectile.NewProjectile(vector114.X, vector114.Y, num866, num867, mod.ProjectileType("HolyLight"), 0, 0f, Main.myPlayer, 0f, 0f);
						else
							Projectile.NewProjectile(vector114.X, vector114.Y, num866, num867, mod.ProjectileType("HolyBurnOrb"), 0, 0f, Main.myPlayer, 0f, 0f);
					}
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] > 450f && !text)
				{
					text = true;
					string key = "Mods.CalamityMod.ProfanedBossText";
					Color messageColor = Color.Orange;

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}

				if (npc.ai[1] > 600f)
				{
					if (Main.netMode != 2)
					{
						Player player2 = Main.player[Main.myPlayer];
						if (!player2.dead && player2.active && Vector2.Distance(player2.Center, vector) < 2800f)
						{
							Main.PlaySound(SoundID.Item20, player2.position);
							player2.AddBuff(mod.BuffType("ExtremeGravity"), 3000, true);

							for (int num621 = 0; num621 < 40; num621++)
							{
								int num622 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
									player2.width, player2.height, 244, 0f, 0f, 100, default(Color), 2f);
								Main.dust[num622].velocity *= 3f;
								if (Main.rand.Next(2) == 0)
								{
									Main.dust[num622].scale = 0.5f;
									Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
								}
							}

							for (int num623 = 0; num623 < 60; num623++)
							{
								int num624 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
									player2.width, player2.height, 244, 0f, 0f, 100, default(Color), 3f);
								Main.dust[num624].noGravity = true;
								Main.dust[num624].velocity *= 5f;
								num624 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
									player2.width, player2.height, 244, 0f, 0f, 100, default(Color), 2f);
								Main.dust[num624].velocity *= 2f;
							}
						}
					}

					text = false;
					npc.ai[0] = -1f;
				}
			}
			else if (npc.ai[0] == 3f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				float num852 = Math.Abs(npc.Center.X - player.Center.X);
				if ((num852 < 500f || npc.ai[3] < 0f) && npc.position.Y < player.position.Y)
				{
					npc.ai[3] += 1f;
					int num856 = expertMode ? 10 : 11;
					if ((double)npc.life < (double)npc.lifeMax * 0.5)
						num856 = expertMode ? 9 : 10;
					if ((double)npc.life <= (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
						num856 = expertMode ? 8 : 9;
					if (!normalAttackRate)
						num856 = expertMode ? 30 : 35;
					num856++;

					if (npc.ai[3] > (float)num856)
						npc.ai[3] = (float)(-(float)num856);

					if (npc.ai[3] == 0f && Main.netMode != 1)
					{
						Vector2 vector112 = new Vector2(npc.Center.X, npc.Center.Y);
						vector112.X += npc.velocity.X * 7f;
						float num857 = player.position.X + (float)player.width * 0.5f - vector112.X;
						float num858 = player.Center.Y - vector112.Y;
						float num859 = (float)Math.Sqrt((double)(num857 * num857 + num858 * num858));

						float num860 = expertMode ? 10.25f : 9f;
						if ((double)npc.life < (double)npc.lifeMax * 0.5)
							num860 = expertMode ? 11.5f : 10f;
						if ((double)npc.life <= (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
							num860 = expertMode ? 12.75f : 11f;
						if (revenge)
							num860 *= 1.15f;

						num859 = num860 / num859;
						num857 *= num859;
						num858 *= num859;

						int holyDamage = expertMode ? 39 : 55; //280 210
						Projectile.NewProjectile(vector112.X, vector112.Y, num857 * 0.1f, num858, mod.ProjectileType("MoltenBlast"), holyDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				}
				else if (npc.ai[3] < 0f)
					npc.ai[3] += 1f;

				if (Main.netMode != 1)
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] > 300f)
						npc.ai[0] = -1f;
				}
			}
			else if (npc.ai[0] == 4f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				if (Main.netMode != 1)
				{
					npc.ai[3] += 1f;
					int num864 = expertMode ? 70 : 74;
					if ((double)npc.life < (double)npc.lifeMax * 0.5)
						num864 = expertMode ? 64 : 70;
					if ((double)npc.life <= (double)npc.lifeMax * 0.1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
						num864 = expertMode ? 56 : 64;
					if (!normalAttackRate)
						num864 = expertMode ? 148 : 156;
					num864 += 3;

					if (npc.ai[3] >= (float)num864)
					{
						npc.ai[3] = 0f;
						Vector2 vector113 = new Vector2(npc.Center.X, npc.position.Y + (float)npc.height - 14f);
						int i2 = (int)(vector113.X / 16f);
						int j2 = (int)(vector113.Y / 16f);

						if (!WorldGen.SolidTile(i2, j2))
						{
							float num865 = npc.velocity.Y;
							if (num865 < 0f)
								num865 = 0f;
							num865 += expertMode ? 4f : 3f;

							int fireDamage = expertMode ? 44 : 60; //260 100
							Projectile.NewProjectile(vector113.X, vector113.Y, npc.velocity.X * 0.25f, num865, mod.ProjectileType("HolyBomb"), fireDamage, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}

				if (Main.netMode != 1)
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] > 300f)
						npc.ai[0] = -1f;
				}
			}
			else if (npc.ai[0] == 5f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;
				npc.TargetClosest(true);
				npc.velocity *= 0.95f;

				if (Main.netMode != 1)
				{
					npc.ai[2] += ((npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 2f : 1f);
					if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
						npc.ai[2] += 1f;
					if ((double)npc.life <= (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
						npc.ai[2] += 1f;

					if (npc.ai[2] > (normalAttackRate ? 24f : 60f))
					{
						npc.ai[2] = 0f;
						Vector2 vector93 = new Vector2(vector.X, vector.Y);
						float num742 = expertMode ? 12f : 10f;
						float num743 = player.position.X + (float)player.width * 0.5f - vector93.X;
						float num744 = player.position.Y + (float)player.height * 0.5f - vector93.Y;
						float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));

						num745 = num742 / num745;
						num743 *= num745;
						num744 *= num745;
						vector93.X += num743 * 3f;
						vector93.Y += num744 * 3f;

						int num746 = expertMode ? 48 : 65; //288 220
						int num747 = mod.ProjectileType("HolyShot");
						Projectile.NewProjectile(vector93.X, vector93.Y, num743, num744, num747, num746, 0f, Main.myPlayer, 0f, 0f);
						Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y + (float)Main.rand.Next(-100, 100), 0f, 0f, mod.ProjectileType("HolySpear"), num746, 0f, Main.myPlayer, (float)Main.rand.Next(2), 0f);
					}
				}

				if (Main.netMode != 1)
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] > 300f)
						npc.ai[0] = -1f;
				}
			}
			else if (npc.ai[0] == 6f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;
				npc.TargetClosest(true);
				npc.velocity *= 0.95f;

				if (Main.netMode != 1)
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] > 60f)
					{
						int damage = expertMode ? 52 : 70; //288 220
						Projectile.NewProjectile(player.Center.X, player.Center.Y - 360f, 0f, 0f, mod.ProjectileType("ProvidenceCrystal"), damage, 0f, player.whoAmI, 0f, 0f);
						npc.ai[0] = -1f;
					}
				}
			}
			else if (npc.ai[0] == 7f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;

				Vector2 value19 = new Vector2(27f, 59f);
				npc.ai[2] += 1f;
				if (npc.ai[2] < 180f)
				{
					npc.localAI[1] -= 0.05f;
					if (npc.localAI[1] < 0f)
						npc.localAI[1] = 0f;

					if (npc.ai[2] >= 60f)
					{
						Vector2 center20 = npc.Center;
						int num1220 = 0;
						if (npc.ai[2] >= 120f)
							num1220 = 1;

						int num;
						for (int num1221 = 0; num1221 < 1 + num1220; num1221 = num + 1)
						{
							int num1222 = 244;
							float num1223 = 1.2f;
							if (num1221 % 2 == 1)
								num1223 = 2.8f;

							Vector2 vector199 = center20 + ((float)Main.rand.NextDouble() * 6.28318548f).ToRotationVector2() * value19 / 2f;
							int num1224 = Dust.NewDust(vector199 - Vector2.One * 8f, 16, 16, num1222, npc.velocity.X / 2f, npc.velocity.Y / 2f, 0, default(Color), 1f);
							Main.dust[num1224].velocity = Vector2.Normalize(center20 - vector199) * 3.5f * (10f - (float)num1220 * 2f) / 10f;
							Main.dust[num1224].noGravity = true;
							Main.dust[num1224].scale = num1223;
							num = num1221;
						}
					}
				}
				else if (npc.ai[2] < 360f)
				{
					if (npc.ai[2] == 180f)
					{
						if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < 2800f)
							Main.PlaySound(29, (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y, 104, 1f, 0f);

						if (Main.netMode != 1)
						{
							npc.TargetClosest(false);
							Vector2 vector200 = player.Center - npc.Center;
							vector200.Normalize();
							float num1225 = -1f;
							if (vector200.X < 0f)
								num1225 = 1f;

							vector200 = vector200.RotatedBy((double)(-(double)num1225 * 6.28318548f / 6f), default(Vector2));
							Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 16f, vector200.X, vector200.Y, mod.ProjectileType("ProvidenceHolyRay"), 100, 0f, Main.myPlayer, num1225 * 6.28318548f / 450f, (float)npc.whoAmI);
							npc.ai[3] = (vector200.ToRotation() + 9.424778f) * num1225; //3.14159265f
							npc.netUpdate = true;
						}
					}

					npc.localAI[1] += 0.05f;
					if (npc.localAI[1] > 1f)
						npc.localAI[1] = 1f;

					float num1226 = (float)(npc.ai[3] >= 0f).ToDirectionInt();
					float num1227 = npc.ai[3];
					if (num1227 < 0f)
						num1227 *= -1f;
					num1227 += -9.424778f;
					num1227 += num1226 * 6.28318548f / 540f;
					npc.localAI[0] = num1227;
				}
				else
				{
					npc.localAI[1] -= 0.07f; //15
					if (npc.localAI[1] < 0f)
						npc.localAI[1] = 0f;
				}

				if (Main.netMode != 1)
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] > 375f)
						npc.ai[0] = -1f;
				}
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}

		public override void NPCLoot()
		{
			DropHelper.DropBags(npc);
			DropHelper.DropItemChance(npc, mod.ItemType("ProvidenceTrophy"), 10);
            DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge39"), true, !CalamityWorld.downedProvidence);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedProvidence, 5, 2, 1);

			DropHelper.DropItemCondition(npc, mod.ItemType("ElysianWings"), biomeType != 2);
			DropHelper.DropItemCondition(npc, mod.ItemType("ElysianAegis"), biomeType == 2);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
			{
                // Materials
                DropHelper.DropItemSpray(npc, mod.ItemType("UnholyEssence"), 20, 30);
                DropHelper.DropItemSpray(npc, mod.ItemType("DivineGeode"), 10, 15);

                // Weapons
                DropHelper.DropItemChance(npc, mod.ItemType("HolyCollider"), 4);
                DropHelper.DropItemChance(npc, mod.ItemType("SolarFlare"), 4);
                DropHelper.DropItemChance(npc, mod.ItemType("TelluricGlare"), 4);
                DropHelper.DropItemChance(npc, mod.ItemType("BlissfulBombardier"), 4);
                DropHelper.DropItemChance(npc, mod.ItemType("PurgeGuzzler"), 4);
                DropHelper.DropItemChance(npc, mod.ItemType("MoltenAmputator"), 4);

                // Equipment
                DropHelper.DropItemChance(npc, mod.ItemType("SamuraiBadge"), 40);

                // Vanity
                DropHelper.DropItemChance(npc, mod.ItemType("ProvidenceMask"), 7);

                // Other
                DropHelper.DropItem(npc, mod.ItemType("RuneofCos"));
			}

			if (Main.netMode != 1)
			{
				SpawnLootBox();
			}

            // If Providence has not been killed, notify players of Uelibloom Ore
            if (!CalamityWorld.downedProvidence)
            {
                string key2 = "Mods.CalamityMod.ProfanedBossText3";
                Color messageColor2 = Color.Orange;
                string key3 = "Mods.CalamityMod.TreeOreText";
                Color messageColor3 = Color.LightGreen;

                WorldGenerationMethods.SpawnOre(mod.TileType("UelibloomOre"), 15E-05, .4f, .8f);

                if (Main.netMode == 0)
                {
                    Main.NewText(Language.GetTextValue(key2), messageColor2);
                    Main.NewText(Language.GetTextValue(key3), messageColor3);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key3), messageColor3);
                }
            }

            // Mark Providence as dead
            CalamityWorld.downedProvidence = true;
            CalamityMod.UpdateServerBoolean();
        }

		private void SpawnLootBox()
		{
			int tileCenterX = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
			int tileCenterY = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
			int halfBox = npc.width / 2 / 16 + 1;
			for (int x = tileCenterX - halfBox; x <= tileCenterX + halfBox; x++)
			{
				for (int y = tileCenterY - halfBox; y <= tileCenterY + halfBox; y++)
				{
					if ((x == tileCenterX - halfBox || x == tileCenterX + halfBox || y == tileCenterY - halfBox || y == tileCenterY + halfBox)
                        && !Main.tile[x, y].active())
					{
						Main.tile[x, y].type = (ushort)mod.TileType("ProfanedRock");
						Main.tile[x, y].active(true);
					}
					Main.tile[x, y].lava(false);
					Main.tile[x, y].liquid = 0;

					if (Main.netMode == 2)
						NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
					else
						WorldGen.SquareTileFrame(x, y, true);
				}
			}
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.SuperHealingPotion;
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			double newDamage = (damage + (int)((double)defense * 0.25));
			float protection = (((npc.ichor || npc.onFire2) ? 0.2f : 0.25f) +
					((npc.ai[0] == 2f || npc.ai[0] == 5f || npc.ai[0] == 7f) ? 0.65f : 0f)); //0.85 or 0.9

			if (newDamage < 1.0)
				newDamage = 1.0;

			if (newDamage >= 1.0)
			{
				newDamage = (double)((int)((double)(1f - protection) * newDamage));

				if (newDamage < 1.0)
					newDamage = 1.0;
			}

			damage = newDamage;
			return true;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Mod mod = ModLoader.GetMod("CalamityMod");
			Texture2D texture = Main.npcTexture[npc.type];

			if (npc.ai[0] == 2f || npc.ai[0] == 5f)
			{
				if (!useDefenseFrames)
					texture = mod.GetTexture("NPCs/Providence/ProvidenceDefense");
				else
					texture = mod.GetTexture("NPCs/Providence/ProvidenceDefenseAlt");
			}
			else
			{
				if (frameUsed == 0)
					texture = Main.npcTexture[npc.type];
				else if (frameUsed == 1)
					texture = mod.GetTexture("NPCs/Providence/ProvidenceAlt");
				else if (frameUsed == 2)
					texture = mod.GetTexture("NPCs/Providence/ProvidenceAttack");
				else
					texture = mod.GetTexture("NPCs/Providence/ProvidenceAttackAlt");
			}

			CalamityMod.DrawTexture(spriteBatch, texture, 0, npc, drawColor);
			return false;
		}

		public override void FindFrame(int frameHeight) //9 total frames
		{
			if (npc.ai[0] == 2f || npc.ai[0] == 5f)
			{
				if (!useDefenseFrames)
				{
					npc.frameCounter += 1.0;
					if (npc.frameCounter > 5.0)
					{
						npc.frame.Y = npc.frame.Y + frameHeight;
						npc.frameCounter = 0.0;
					}
					if (npc.frame.Y >= frameHeight * 3)
					{
						npc.frame.Y = 0;
						useDefenseFrames = true;
					}
				}
				else
				{
					npc.frameCounter += 1.0;
					if (npc.frameCounter > 5.0)
					{
						npc.frame.Y = npc.frame.Y + frameHeight;
						npc.frameCounter = 0.0;
					}
					if (npc.frame.Y >= frameHeight * 2)
						npc.frame.Y = frameHeight * 2;
				}
			}
			else
			{
				if (useDefenseFrames)
					useDefenseFrames = false;

				npc.frameCounter += 1.0;
				if (npc.frameCounter > 5.0)
				{
					npc.frameCounter = 0.0;
					npc.frame.Y = npc.frame.Y + frameHeight;
				}
				if (npc.frame.Y >= frameHeight * 3) //6
				{
					npc.frame.Y = 0;
					frameUsed++;
				}
				if (frameUsed > 3)
					frameUsed = 0;
			}
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 2f;
			return null;
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 15; k++)
				Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default(Color), 1f);

			if (npc.life <= 0)
			{
				float randomSpread = (float)(Main.rand.Next(-50, 50) / 100);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence2"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence3"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence4"), 1f);
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 400;
				npc.height = 350;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 60; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 90; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
	}
}
