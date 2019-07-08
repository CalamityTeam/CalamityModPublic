using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
using CalamityMod.World;

namespace CalamityMod.NPCs.Cryogen
{
	[AutoloadBossHead]
	public class Cryogen : ModNPC
	{
		private int time = 0;
		private int iceShard = 0;
		private bool drawAltTexture = false;
		private int teleportLocationX = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cryogen");
		}

		public override void SetDefaults()
		{
			npc.npcSlots = 24f;
			npc.damage = 60;
			npc.width = 80; //324
			npc.height = 80; //216
			npc.defense = 10;
			npc.lifeMax = CalamityWorld.revenge ? 26300 : 17900;
			if (CalamityWorld.death)
			{
				npc.lifeMax = 39900;
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 3000000 : 2700000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			npc.aiStyle = -1; //new
			aiType = -1; //new
			animationType = 10; //new
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 12, 0, 0);
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.buffImmune[BuffID.Ichor] = false;
			npc.buffImmune[mod.BuffType("MarkedforDeath")] = false;
			npc.buffImmune[BuffID.OnFire] = false;
			npc.buffImmune[BuffID.CursedInferno] = false;
			npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
			npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
			npc.buffImmune[mod.BuffType("BrimstoneFlames")] = false;
			npc.buffImmune[mod.BuffType("DemonFlames")] = false;
			npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
			npc.buffImmune[mod.BuffType("HolyLight")] = false;
			npc.buffImmune[mod.BuffType("Nightwither")] = false;
			npc.buffImmune[mod.BuffType("Plague")] = false;
			npc.buffImmune[mod.BuffType("Shred")] = false;
			npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
			npc.buffImmune[mod.BuffType("SilvaStun")] = false;
			npc.boss = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit5;
			npc.DeathSound = SoundID.NPCDeath15;
			Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
			if (calamityModMusic != null)
				music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Cryogen");
			else
				music = MusicID.FrostMoon;
			bossBag = mod.ItemType("CryogenBag");
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(drawAltTexture);
			writer.Write(time);
			writer.Write(iceShard);
			writer.Write(teleportLocationX);
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.chaseable);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			drawAltTexture = reader.ReadBoolean();
			time = reader.ReadInt32();
			iceShard = reader.ReadInt32();
			teleportLocationX = reader.ReadInt32();
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.chaseable = reader.ReadBoolean();
		}

		public override void AI()
		{
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0f, 1f, 1f);
			Player player = Main.player[npc.target];
			bool isChill = player.ZoneSnow;
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			npc.TargetClosest(true);
			if (npc.ai[2] == 0f && npc.localAI[1] == 0f && Main.netMode != 1 && npc.ai[0] < 4f) //spawn shield for phase 0 1 2 3, not 4 5
			{
				int num6 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("CryogenIce"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				npc.ai[2] = (float)(num6 + 1);
				npc.localAI[1] = -1f;
				npc.netUpdate = true;
				Main.npc[num6].ai[0] = (float)npc.whoAmI;
				Main.npc[num6].netUpdate = true;
			}
			int num7 = (int)npc.ai[2] - 1;
			if (num7 != -1 && Main.npc[num7].active && Main.npc[num7].type == mod.NPCType("CryogenIce"))
			{
				npc.dontTakeDamage = true;
			}
			else
			{
				npc.dontTakeDamage = false;
				npc.ai[2] = 0f;
				if (npc.localAI[1] == -1f)
				{
					npc.localAI[1] = expertMode ? 720f : 1080f;
				}
				if (npc.localAI[1] > 0f)
				{
					npc.localAI[1] -= 1f;
				}
			}
			if (npc.ai[0] == 0f || npc.ai[0] == 2f || npc.ai[0] == 4f)
			{
				npc.rotation = npc.velocity.X * 0.1f;
			}
			else if (npc.ai[0] == 1f || npc.ai[0] == 3f)
			{
				npc.rotation = 0f;
			}
			if (!Main.raining && !CalamityWorld.bossRushActive)
			{
				RainStart();
			}
			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					npc.velocity = new Vector2(0f, -10f);
					if (npc.timeLeft > 150)
					{
						npc.timeLeft = 150;
					}
					return;
				}
			}
			else if (npc.timeLeft < 2400)
			{
				npc.timeLeft = 2400;
			}
			if (Main.netMode != 1 && expertMode)
			{
				time++;
				if (CalamityWorld.death || CalamityWorld.bossRushActive)
				{
					time++;
				}
				if (time >= 600)
				{
					Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float spread = 45f * 0.0174f;
					double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
					double deltaAngle = spread / 4f;
					double offsetAngle;
					int i;
					int num184 = 22;
					for (i = 0; i < 2; i++)
					{
						offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
						Projectile.NewProjectile(value9.X, value9.Y, (float)(Math.Sin(offsetAngle) * 8f), (float)(Math.Cos(offsetAngle) * 8f), mod.ProjectileType("IceBomb"), num184, 0f, Main.myPlayer, 0f, 0f);
						Projectile.NewProjectile(value9.X, value9.Y, (float)(-Math.Sin(offsetAngle) * 8f), (float)(-Math.Cos(offsetAngle) * 8f), mod.ProjectileType("IceBomb"), num184, 0f, Main.myPlayer, 0f, 0f);
					}
					time = 0;
				}
			}
			if (npc.ai[0] == 0f)
			{
				if (Main.netMode != 1)
				{
					npc.localAI[0] += 1f;
					if (npc.localAI[0] >= 120f)
					{
						npc.localAI[0] = 0f;
						npc.TargetClosest(true);
						npc.netUpdate = true;
						if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float spread = 45f * 0.0174f;
							double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
							double deltaAngle = spread / 8f;
							double offsetAngle;
							int num184 = expertMode ? 20 : 23;
							int i;
							for (i = 0; i < 8; i++)
							{
								offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
								Projectile.NewProjectile(value9.X, value9.Y, (float)(Math.Sin(offsetAngle) * 8f), (float)(Math.Cos(offsetAngle) * 8f), mod.ProjectileType("IceBlast"), num184, 0f, Main.myPlayer, 0f, 0f);
								Projectile.NewProjectile(value9.X, value9.Y, (float)(-Math.Sin(offsetAngle) * 8f), (float)(-Math.Cos(offsetAngle) * 8f), mod.ProjectileType("IceBlast"), num184, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}
				Vector2 vector142 = new Vector2(npc.Center.X, npc.Center.Y);
				float num1243 = player.Center.X - vector142.X;
				float num1244 = player.Center.Y - vector142.Y;
				float num1245 = (float)Math.Sqrt((double)(num1243 * num1243 + num1244 * num1244));
				float num1246 = isChill ? 3f : 5f;
				if (CalamityWorld.death)
				{
					num1246 = isChill ? 4f : 6f;
				}
				if (CalamityWorld.bossRushActive)
				{
					num1246 = 10f;
				}
				num1245 = num1246 / num1245;
				num1243 *= num1245;
				num1244 *= num1245;
				npc.velocity.X = (npc.velocity.X * 50f + num1243) / 51f;
				npc.velocity.Y = (npc.velocity.Y * 50f + num1244) / 51f;
				if ((double)npc.life < (double)npc.lifeMax * 0.83)
				{
					npc.ai[0] = 1f;
					npc.localAI[0] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 1f)
			{
				if (Main.netMode != 1)
				{
					npc.localAI[0] += 1f;
					if (npc.localAI[0] >= 120f)
					{
						npc.localAI[0] = 0f;
						npc.TargetClosest(true);
						if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float spread = 45f * 0.0174f;
							double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
							double deltaAngle = spread / 8f;
							double offsetAngle;
							int num184 = expertMode ? 20 : 23;
							int i;
							for (i = 0; i < 6; i++)
							{
								offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
								int ice = Projectile.NewProjectile(value9.X, value9.Y, (float)(Math.Sin(offsetAngle) * 8f), (float)(Math.Cos(offsetAngle) * 8f), mod.ProjectileType("IceBlast"), num184, 0f, Main.myPlayer, 0f, 0f);
								int ice2 = Projectile.NewProjectile(value9.X, value9.Y, (float)(-Math.Sin(offsetAngle) * 8f), (float)(-Math.Cos(offsetAngle) * 8f), mod.ProjectileType("IceBlast"), num184, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[ice].timeLeft = 300;
								Main.projectile[ice2].timeLeft = 300;
							}
						}
					}
				}
				float num1164 = isChill ? 4f : 6f;
				float num1165 = isChill ? 1f : 1.2f;
				if (CalamityWorld.death)
				{
					num1164 = isChill ? 5f : 7f;
				}
				if (CalamityWorld.bossRushActive)
				{
					num1164 = 12f;
				}
				Vector2 vector133 = new Vector2(npc.Center.X, npc.Center.Y);
				float num1166 = player.Center.X - vector133.X;
				float num1167 = player.Center.Y - vector133.Y - 400f;
				float num1168 = (float)Math.Sqrt((double)(num1166 * num1166 + num1167 * num1167));
				if (num1168 < 20f)
				{
					num1166 = npc.velocity.X;
					num1167 = npc.velocity.Y;
				}
				else
				{
					num1168 = num1164 / num1168;
					num1166 *= num1168;
					num1167 *= num1168;
				}
				if (npc.velocity.X < num1166)
				{
					npc.velocity.X = npc.velocity.X + num1165;
					if (npc.velocity.X < 0f && num1166 > 0f)
					{
						npc.velocity.X = npc.velocity.X + num1165 * 2f;
					}
				}
				else if (npc.velocity.X > num1166)
				{
					npc.velocity.X = npc.velocity.X - num1165;
					if (npc.velocity.X > 0f && num1166 < 0f)
					{
						npc.velocity.X = npc.velocity.X - num1165 * 2f;
					}
				}
				if (npc.velocity.Y < num1167)
				{
					npc.velocity.Y = npc.velocity.Y + num1165;
					if (npc.velocity.Y < 0f && num1167 > 0f)
					{
						npc.velocity.Y = npc.velocity.Y + num1165 * 2f;
					}
				}
				else if (npc.velocity.Y > num1167)
				{
					npc.velocity.Y = npc.velocity.Y - num1165;
					if (npc.velocity.Y > 0f && num1167 < 0f)
					{
						npc.velocity.Y = npc.velocity.Y - num1165 * 2f;
					}
				}
				if (npc.position.X + (float)npc.width > player.position.X && npc.position.X < player.position.X + (float)player.width && npc.position.Y + (float)npc.height < player.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height) && Main.netMode != 1)
				{
					iceShard += 4;
					if (iceShard > 8)
					{
						iceShard = 0;
						int num1169 = (int)(npc.position.X + 10f + (float)Main.rand.Next(npc.width - 20));
						int num1170 = (int)(npc.position.Y + (float)npc.height + 4f);
						int damage = expertMode ? 23 : 26;
						Projectile.NewProjectile((float)num1169, (float)num1170, 0f, 5f, mod.ProjectileType("IceRain"), damage, 0f, Main.myPlayer, 0f, 0f);
					}
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.66)
				{
					npc.ai[0] = 2f;
					npc.localAI[0] = 0f;
					iceShard = 0;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 2f)
			{
				if (Main.netMode != 1)
				{
					npc.localAI[0] += 1f;
					if (npc.localAI[0] >= 120f)
					{
						npc.localAI[0] = 0f;
						npc.TargetClosest(true);
						npc.netUpdate = true;
						if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							if (Main.rand.Next(2) == 0)
							{
								Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
								float spread = 45f * 0.0174f;
								double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
								double deltaAngle = spread / 8f;
								double offsetAngle;
								int num184 = expertMode ? 20 : 23;
								int i;
								for (i = 0; i < 6; i++)
								{
									offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
									int ice = Projectile.NewProjectile(value9.X, value9.Y, (float)(Math.Sin(offsetAngle) * 8f), (float)(Math.Cos(offsetAngle) * 8f), mod.ProjectileType("IceBlast"), num184, 0f, Main.myPlayer, 0f, 0f);
									int ice2 = Projectile.NewProjectile(value9.X, value9.Y, (float)(-Math.Sin(offsetAngle) * 8f), (float)(-Math.Cos(offsetAngle) * 8f), mod.ProjectileType("IceBlast"), num184, 0f, Main.myPlayer, 0f, 0f);
									Main.projectile[ice].timeLeft = 300;
									Main.projectile[ice2].timeLeft = 300;
								}
							}
							else
							{
								float num179 = revenge ? 9f : 7f;
								Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
								float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
								float num181 = Math.Abs(num180) * 0.1f;
								float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
								float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
								num183 = num179 / num183;
								num180 *= num183;
								num182 *= num183;
								int num184 = expertMode ? 23 : 26;
								int num185 = mod.ProjectileType("IceRain");
								value9.X += num180;
								value9.Y += num182;
								for (int num186 = 0; num186 < 15; num186++)
								{
									num180 = player.position.X + (float)player.width * 0.5f - value9.X;
									num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
									num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
									num183 = num179 / num183;
									num180 += (float)Main.rand.Next(-100, 101);
									num182 += (float)Main.rand.Next(-100, 101);
									num180 *= num183;
									num182 *= num183;
									Projectile.NewProjectile(value9.X, value9.Y, num180, -10f, num185, num184, 0f, Main.myPlayer, 0f, 0f);
								}
							}
						}
					}
				}
				Vector2 vector142 = new Vector2(npc.Center.X, npc.Center.Y);
				float num1243 = player.Center.X - vector142.X;
				float num1244 = player.Center.Y - vector142.Y;
				float num1245 = (float)Math.Sqrt((double)(num1243 * num1243 + num1244 * num1244));
				float num1246 = isChill ? 5f : 7f;
				if (CalamityWorld.death)
				{
					num1246 = isChill ? 6f : 8f;
				}
				if (CalamityWorld.bossRushActive)
				{
					num1246 = 14f;
				}
				num1245 = num1246 / num1245;
				num1243 *= num1245;
				num1244 *= num1245;
				npc.velocity.X = (npc.velocity.X * 50f + num1243) / 51f;
				npc.velocity.Y = (npc.velocity.Y * 50f + num1244) / 51f;
				if ((double)npc.life < (double)npc.lifeMax * 0.49)
				{
					npc.ai[0] = 3f;
					npc.localAI[0] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 3f)
			{
				if (Main.netMode != 1)
				{
					npc.localAI[0] += 1f;
					if (npc.localAI[0] >= 120f)
					{
						npc.localAI[0] = 0f;
						npc.TargetClosest(true);
						if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							float num179 = revenge ? 9f : 7f;
							Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
							float num181 = Math.Abs(num180) * 0.1f;
							float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
							float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
							npc.netUpdate = true;
							num183 = num179 / num183;
							num180 *= num183;
							num182 *= num183;
							int num184 = expertMode ? 23 : 26;
							int num185 = mod.ProjectileType("IceRain");
							value9.X += num180;
							value9.Y += num182;
							for (int num186 = 0; num186 < 15; num186++)
							{
								num180 = player.position.X + (float)player.width * 0.5f - value9.X;
								num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
								num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
								num183 = num179 / num183;
								num180 += (float)Main.rand.Next(-100, 101);
								num182 += (float)Main.rand.Next(-100, 101);
								num180 *= num183;
								num182 *= num183;
								Projectile.NewProjectile(value9.X, value9.Y, num180, -10f, num185, num184, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}
				float num1164 = isChill ? 4.5f : 6.5f;
				float num1165 = isChill ? 1.1f : 1.3f;
				if (CalamityWorld.death || CalamityWorld.bossRushActive)
				{
					num1164 = isChill ? 5.5f : 7.5f;
				}
				Vector2 vector133 = new Vector2(npc.Center.X, npc.Center.Y);
				float num1166 = player.Center.X - vector133.X;
				float num1167 = player.Center.Y - vector133.Y - 300f;
				float num1168 = (float)Math.Sqrt((double)(num1166 * num1166 + num1167 * num1167));
				if (num1168 < 20f)
				{
					num1166 = npc.velocity.X;
					num1167 = npc.velocity.Y;
				}
				else
				{
					num1168 = num1164 / num1168;
					num1166 *= num1168;
					num1167 *= num1168;
				}
				if (npc.velocity.X < num1166)
				{
					npc.velocity.X = npc.velocity.X + num1165;
					if (npc.velocity.X < 0f && num1166 > 0f)
					{
						npc.velocity.X = npc.velocity.X + num1165 * 2f;
					}
				}
				else if (npc.velocity.X > num1166)
				{
					npc.velocity.X = npc.velocity.X - num1165;
					if (npc.velocity.X > 0f && num1166 < 0f)
					{
						npc.velocity.X = npc.velocity.X - num1165 * 2f;
					}
				}
				if (npc.velocity.Y < num1167)
				{
					npc.velocity.Y = npc.velocity.Y + num1165;
					if (npc.velocity.Y < 0f && num1167 > 0f)
					{
						npc.velocity.Y = npc.velocity.Y + num1165 * 2f;
					}
				}
				else if (npc.velocity.Y > num1167)
				{
					npc.velocity.Y = npc.velocity.Y - num1165;
					if (npc.velocity.Y > 0f && num1167 < 0f)
					{
						npc.velocity.Y = npc.velocity.Y - num1165 * 2f;
					}
				}
				if (npc.position.X + (float)npc.width > player.position.X && npc.position.X < player.position.X + (float)player.width && npc.position.Y + (float)npc.height < player.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height) && Main.netMode != 1)
				{
					iceShard += 4;
					if (iceShard > 8)
					{
						iceShard = 0;
						int num1169 = (int)(npc.position.X + 10f + (float)Main.rand.Next(npc.width - 20));
						int num1170 = (int)(npc.position.Y + (float)npc.height + 4f);
						int damage = expertMode ? 23 : 26;
						Projectile.NewProjectile((float)num1169, (float)num1170, 0f, 5f, mod.ProjectileType("IceRain"), damage, 0f, Main.myPlayer, 0f, 0f);
						return;
					}
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.32)
				{
					npc.ai[0] = 4f;
					npc.localAI[0] = 0f;
					iceShard = 0;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 4f)
			{
				if (Main.netMode != 1)
				{
					npc.localAI[0] += 1f;
					if (npc.localAI[0] >= 60f && npc.alpha == 0)
					{
						npc.localAI[0] = 0f;
						npc.TargetClosest(true);
						if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float spread = 45f * 0.0174f;
							double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
							double deltaAngle = spread / 8f;
							double offsetAngle;
							int num184 = expertMode ? 20 : 23;
							int i;
							for (i = 0; i < 6; i++)
							{
								offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
								int ice = Projectile.NewProjectile(value9.X, value9.Y, (float)(Math.Sin(offsetAngle) * 9f), (float)(Math.Cos(offsetAngle) * 9f), mod.ProjectileType("IceBlast"), num184, 0f, Main.myPlayer, 0f, 0f);
								int ice2 = Projectile.NewProjectile(value9.X, value9.Y, (float)(-Math.Sin(offsetAngle) * 9f), (float)(-Math.Cos(offsetAngle) * 9f), mod.ProjectileType("IceBlast"), num184, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[ice].timeLeft = 300;
								Main.projectile[ice2].timeLeft = 300;
							}
						}
					}
				}
				npc.TargetClosest(true);
				Vector2 vector142 = new Vector2(npc.Center.X, npc.Center.Y);
				float num1243 = player.Center.X - vector142.X;
				float num1244 = player.Center.Y - vector142.Y;
				float num1245 = (float)Math.Sqrt((double)(num1243 * num1243 + num1244 * num1244));
				float speed = revenge ? 4.5f : 4f;
				if (CalamityWorld.bossRushActive)
				{
					speed = 9f;
				}
				num1245 = speed / num1245;
				num1243 *= num1245;
				num1244 *= num1245;
				npc.velocity.X = (npc.velocity.X * 50f + num1243) / 51f;
				npc.velocity.Y = (npc.velocity.Y * 50f + num1244) / 51f;
				if (npc.ai[1] == 0f)
				{
					npc.chaseable = true;
					npc.dontTakeDamage = false;
					if (Main.netMode != 1)
					{
						npc.localAI[2] += 1f;
						if (npc.localAI[2] >= (float)(120 + Main.rand.Next(200)))
						{
							npc.localAI[2] = 0f;
							npc.TargetClosest(true);
							int num1249 = 0;
							int num1250;
							int num1251;
							while (true)
							{
								num1249++;
								num1250 = (int)player.Center.X / 16;
								num1251 = (int)player.Center.Y / 16;
								num1250 += Main.rand.Next(-50, 51);
								num1251 += Main.rand.Next(-50, 51);
								if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2((float)(num1250 * 16), (float)(num1251 * 16)), 1, 1, player.position, player.width, player.height))
								{
									break;
								}
								if (num1249 > 100)
								{
									goto Block;
								}
							}
							npc.ai[1] = 1f;
							teleportLocationX = num1250;
							iceShard = num1251;
							npc.netUpdate = true;
						Block:;
						}
					}
				}
				else if (npc.ai[1] == 1f)
				{
					npc.dontTakeDamage = true;
					npc.chaseable = false;
					npc.alpha += 4;
					if (npc.alpha >= 255)
					{
						npc.alpha = 255;
						npc.position.X = (float)teleportLocationX * 16f - (float)(npc.width / 2);
						npc.position.Y = (float)iceShard * 16f - (float)(npc.height / 2);
						npc.ai[1] = 2f;
						npc.netUpdate = true;
					}
				}
				else if (npc.ai[1] == 2f)
				{
					npc.alpha -= 4;
					if (npc.alpha <= 0)
					{
						npc.dontTakeDamage = false;
						npc.chaseable = true;
						npc.alpha = 0;
						npc.ai[1] = 0f;
						npc.netUpdate = true;
					}
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.15)
				{
					Main.PlaySound(4, (int)npc.position.X, (int)npc.position.Y, 15);
					drawAltTexture = true;
					for (int num621 = 0; num621 < 40; num621++)
					{
						int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default(Color), 2f);
						Main.dust[num622].velocity *= 3f;
						if (Main.rand.Next(2) == 0)
						{
							Main.dust[num622].scale = 0.5f;
							Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
						}
					}
					for (int num623 = 0; num623 < 70; num623++)
					{
						int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default(Color), 3f);
						Main.dust[num624].noGravity = true;
						Main.dust[num624].velocity *= 5f;
						num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default(Color), 2f);
						Main.dust[num624].velocity *= 2f;
					}
					float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
					Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore1"), 1f);
					Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore2"), 1f);
					Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore3"), 1f);
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.localAI[0] = 0f;
					npc.localAI[2] = 0f;
					teleportLocationX = 0;
					iceShard = 0;
					npc.netUpdate = true;
					string key = "Mods.CalamityMod.CryogenBossText";
					Color messageColor = Color.Cyan;
					if (Main.netMode == 0)
					{
						Main.NewText(Language.GetTextValue(key), messageColor);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
					}
				}
			}
			else
			{
				npc.dontTakeDamage = false;
				npc.chaseable = true;
				float num1372 = isChill ? 16f : 22f;
				if (CalamityWorld.bossRushActive)
				{
					num1372 = 24f;
				}
				Vector2 vector167 = new Vector2(npc.Center.X + (float)(npc.direction * 20), npc.Center.Y + 6f);
				float num1373 = player.position.X + (float)player.width * 0.5f - vector167.X;
				float num1374 = player.Center.Y - vector167.Y;
				float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
				float num1376 = num1372 / num1375;
				num1373 *= num1376;
				num1374 *= num1376;
				iceShard--;
				if ((double)npc.life < (double)npc.lifeMax * 0.05 || CalamityWorld.death || CalamityWorld.bossRushActive)
				{
					if (num1375 < 170f || iceShard > 0)
					{
						if (num1375 < 170f)
						{
							iceShard = 17;
						}
						if (npc.velocity.X < 0f)
						{
							npc.direction = -1;
						}
						else
						{
							npc.direction = 1;
						}
						npc.rotation += (float)npc.direction * 0.5f;
						return;
					}
				}
				else if ((double)npc.life < (double)npc.lifeMax * 0.1)
				{
					if (num1375 < 190f || iceShard > 0)
					{
						if (num1375 < 190f)
						{
							iceShard = 19;
						}
						if (npc.velocity.X < 0f)
						{
							npc.direction = -1;
						}
						else
						{
							npc.direction = 1;
						}
						npc.rotation += (float)npc.direction * 0.35f;
						return;
					}
				}
				else
				{
					if (num1375 < 200f || iceShard > 0)
					{
						if (num1375 < 200f)
						{
							iceShard = 20;
						}
						if (npc.velocity.X < 0f)
						{
							npc.direction = -1;
						}
						else
						{
							npc.direction = 1;
						}
						npc.rotation += (float)npc.direction * 0.3f;
						return;
					}
				}
				npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
				npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
				if (num1375 < 350f)
				{
					npc.velocity.X = (npc.velocity.X * 10f + num1373) / 11f;
					npc.velocity.Y = (npc.velocity.Y * 10f + num1374) / 11f;
				}
				if (num1375 < 300f)
				{
					npc.velocity.X = (npc.velocity.X * 7f + num1373) / 8f;
					npc.velocity.Y = (npc.velocity.Y * 7f + num1374) / 8f;
				}
				npc.rotation = npc.velocity.X * 0.15f;
			}
			if (npc.ai[3] == 0f && npc.life > 0)
			{
				npc.ai[3] = (float)npc.lifeMax;
			}
			if (npc.life > 0)
			{
				if (Main.netMode != 1)
				{
					int num660 = (int)((double)npc.lifeMax * 0.05);
					if ((float)(npc.life + num660) < npc.ai[3])
					{
						npc.ai[3] = (float)npc.life;
						for (int num662 = 0; num662 < 2; num662++)
						{
							int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
							int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));
							int randomSpawn = Main.rand.Next(3);
							if (randomSpawn == 0)
							{
								randomSpawn = mod.NPCType("Cryocore");
							}
							else if (randomSpawn == 1)
							{
								randomSpawn = mod.NPCType("IceMass");
							}
							else
							{
								randomSpawn = mod.NPCType("Cryocore2");
							}
							int num664 = NPC.NewNPC(x, y, randomSpawn, 0, 0f, 0f, 0f, 0f, 255);
							if (Main.netMode == 2 && num664 < 200)
							{
								NetMessage.SendData(23, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
							}
						}
					}
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) //for alt textures
		{
			if (drawAltTexture)
			{
				Mod mod = ModLoader.GetMod("CalamityMod");
				Texture2D texture = mod.GetTexture("NPCs/Cryogen/Cryogen2");
				CalamityMod.DrawTexture(spriteBatch, texture, 0, npc, drawColor);
				return false;
			}
			return true;
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return npc.alpha == 0;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int num621 = 0; num621 < 40; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 70; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
				float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore8"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore9"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoGore10"), 1f);
			}
		}

		private void RainStart()
		{
			int num = 86400;
			int num2 = num / 24;
			Main.rainTime = Main.rand.Next(num2 * 8, num);
			if (Main.rand.Next(3) == 0)
			{
				Main.rainTime += Main.rand.Next(0, num2);
			}
			if (Main.rand.Next(4) == 0)
			{
				Main.rainTime += Main.rand.Next(0, num2 * 2);
			}
			if (Main.rand.Next(5) == 0)
			{
				Main.rainTime += Main.rand.Next(0, num2 * 2);
			}
			if (Main.rand.Next(6) == 0)
			{
				Main.rainTime += Main.rand.Next(0, num2 * 3);
			}
			if (Main.rand.Next(7) == 0)
			{
				Main.rainTime += Main.rand.Next(0, num2 * 4);
			}
			if (Main.rand.Next(8) == 0)
			{
				Main.rainTime += Main.rand.Next(0, num2 * 5);
			}
			float num3 = 1f;
			if (Main.rand.Next(2) == 0)
			{
				num3 += 0.05f;
			}
			if (Main.rand.Next(3) == 0)
			{
				num3 += 0.1f;
			}
			if (Main.rand.Next(4) == 0)
			{
				num3 += 0.15f;
			}
			if (Main.rand.Next(5) == 0)
			{
				num3 += 0.2f;
			}
			Main.rainTime = (int)((float)Main.rainTime * num3);
			Main.raining = true;
			if (Main.netMode == 2)
			{
				NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
			}
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.GreaterHealingPotion;
		}

		public override void NPCLoot()
		{
			int permadongo = NPC.FindFirstNPC(mod.NPCType("DILF"));
			if (permadongo == -1 && Main.netMode != 1)
			{
				NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("DILF"), 0, 0f, 0f, 0f, 0f, 255);
			}
			if (Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CryogenTrophy"));
			}
			if (CalamityWorld.armageddon)
			{
				for (int i = 0; i < 5; i++)
				{
					npc.DropBossBags();
				}
			}
			if (Main.expertMode)
			{
				npc.DropBossBags();
			}
			else
			{
				if (Main.rand.Next(5) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FrozenKey);
				}
				if (Main.rand.Next(10) == 0)
				{
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("CryoStone"), 1, true);
				}
				if (Main.rand.Next(7) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CryogenMask"));
				}
				if (Main.rand.Next(40) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Regenator"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BittercoldStaff"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EffluviumBow"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GlacialCrusher"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Icebreaker"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("IceStar"), Main.rand.Next(100, 151));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Avalanche"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SnowstormStaff"));
				}
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SoulofMight, Main.rand.Next(20, 41));
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CryoBar"), Main.rand.Next(15, 26));
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofEleum"), Main.rand.Next(4, 9));
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FrostCore);
			}
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.Frostburn, 120, true);
			player.AddBuff(BuffID.Chilled, 90, true);
		}
	}
}