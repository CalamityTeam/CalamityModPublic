using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.NPCs
{
    public class CalamityGlobalAI
	{
		#region Queen Bee Lore AI Changes
		public static void QueenBeeLoreEffect(NPC npc)
		{
			npc.damage = 0;

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
				npc.TargetClosest(true);

			float num = 5f;
			float num2 = 0.1f;
			npc.localAI[0] += 1f;

			float num3 = (npc.localAI[0] - 60f) / 60f;
			if (num3 > 1f)
				num3 = 1f;
			else
			{
				if (npc.velocity.X > 6f)
					npc.velocity.X = 6f;
				if (npc.velocity.X < -6f)
					npc.velocity.X = -6f;
				if (npc.velocity.Y > 6f)
					npc.velocity.Y = 6f;
				if (npc.velocity.Y < -6f)
					npc.velocity.Y = -6f;
			}

			num2 *= num3;
			Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num4 = (float)npc.direction * num / 2f;
			float num5 = -num / 2f;

			if (npc.velocity.X < num4)
			{
				npc.velocity.X = npc.velocity.X + num2;
				if (npc.velocity.X < 0f && num4 > 0f)
					npc.velocity.X = npc.velocity.X + num2;
			}
			else if (npc.velocity.X > num4)
			{
				npc.velocity.X = npc.velocity.X - num2;
				if (npc.velocity.X > 0f && num4 < 0f)
					npc.velocity.X = npc.velocity.X - num2;
			}
			if (npc.velocity.Y < num5)
			{
				npc.velocity.Y = npc.velocity.Y + num2;
				if (npc.velocity.Y < 0f && num5 > 0f)
					npc.velocity.Y = npc.velocity.Y + num2;
			}
			else if (npc.velocity.Y > num5)
			{
				npc.velocity.Y = npc.velocity.Y - num2;
				if (npc.velocity.Y > 0f && num5 < 0f)
					npc.velocity.Y = npc.velocity.Y - num2;
			}

			if (npc.type != NPCID.Bee && npc.type != NPCID.BeeSmall)
			{
				if (npc.velocity.X > 0f)
					npc.spriteDirection = 1;
				if (npc.velocity.X < 0f)
					npc.spriteDirection = -1;

				npc.rotation = npc.velocity.X * 0.1f;
			}
			else
				npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

			float num11 = 0.7f;
			if (npc.collideX)
			{
				npc.netUpdate = true;
				npc.velocity.X = npc.oldVelocity.X * -num11;

				if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
					npc.velocity.X = 2f;
				if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
					npc.velocity.X = -2f;
			}
			if (npc.collideY)
			{
				npc.netUpdate = true;
				npc.velocity.Y = npc.oldVelocity.Y * -num11;

				if (npc.velocity.Y > 0f && (double)npc.velocity.Y < 1.5)
					npc.velocity.Y = 2f;
				if (npc.velocity.Y < 0f && (double)npc.velocity.Y > -1.5)
					npc.velocity.Y = -2f;
			}

			if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
				npc.netUpdate = true;
		}
		#endregion

		#region Boss Rush King Slime AI
		public static bool BossRushKingSlimeAI(NPC npc, bool enraged)
		{
			float num234 = 1f;
			bool flag8 = false;
			bool flag9 = false;
			npc.aiAction = 0;

			if (npc.ai[3] == 0f && npc.life > 0)
				npc.ai[3] = (float)npc.lifeMax;

			if (npc.localAI[3] == 0f && Main.netMode != 1)
			{
				npc.ai[0] = -100f;
				npc.localAI[3] = 1f;
				npc.TargetClosest(true);
				npc.netUpdate = true;
			}

			if (Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
				if (Main.player[npc.target].dead)
				{
					npc.timeLeft = 0;

					if (Main.player[npc.target].Center.X < npc.Center.X)
						npc.direction = 1;
					else
						npc.direction = -1;
				}
			}

			if (!Main.player[npc.target].dead && npc.ai[2] >= 300f && npc.ai[1] < 5f && npc.velocity.Y == 0f)
			{
				npc.ai[2] = 0f;
				npc.ai[0] = 0f;
				npc.ai[1] = 5f;

				if (Main.netMode != 1)
				{
					npc.TargetClosest(false);
					Vector2 vector30 = Main.player[npc.target].Center - npc.Center;

					int num238 = 0;
					bool flag10 = false;
					if (vector30.Length() > ((enraged || Config.BossRushXerocCurse) ? 1000f : 2000f))
					{
						flag10 = true;
						num238 = 100;
					}

					Point point3 = npc.Center.ToTileCoordinates();
					Point point4 = Main.player[npc.target].Center.ToTileCoordinates();
					int num235 = 10;
					int num236 = 0;
					int num237 = 7;

					while (!flag10 && num238 < 100)
					{
						num238++;
						int num239 = Main.rand.Next(point4.X - num235, point4.X + num235 + 1);
						int num240 = Main.rand.Next(point4.Y - num235, point4.Y + 1);

						if ((num240 < point4.Y - num237 || num240 > point4.Y + num237 || num239 < point4.X - num237 || num239 > point4.X + num237) && (num240 < point3.Y - num236 || num240 > point3.Y + num236 || num239 < point3.X - num236 || num239 > point3.X + num236) && !Main.tile[num239, num240].nactive())
						{
							int num241 = num240;
							int num242 = 0;

							bool flag11 = Main.tile[num239, num241].nactive() && Main.tileSolid[(int)Main.tile[num239, num241].type] && !Main.tileSolidTop[(int)Main.tile[num239, num241].type];
							if (flag11)
								num242 = 1;
							else
							{
								while (num242 < 150 && num241 + num242 < Main.maxTilesY)
								{
									int num243 = num241 + num242;
									bool flag12 = Main.tile[num239, num243].nactive() && Main.tileSolid[(int)Main.tile[num239, num243].type] && !Main.tileSolidTop[(int)Main.tile[num239, num243].type];
									if (flag12)
									{
										num242--;
										break;
									}
									int num = num242;
									num242 = num + 1;
								}
							}
							num240 += num242;

							bool flag13 = true;
							if (flag13 && Main.tile[num239, num240].lava())
								flag13 = false;
							if (flag13 && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
								flag13 = false;

							if (flag13)
							{
								npc.localAI[1] = (float)(num239 * 16 + 8);
								npc.localAI[2] = (float)(num240 * 16 + 16);
								break;
							}
						}
					}
					if (num238 >= 100)
					{
						Vector2 bottom = Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].Bottom;
						npc.localAI[1] = bottom.X;
						npc.localAI[2] = bottom.Y;
					}
				}
			}

			if (!Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
				npc.ai[2] += 1f;
			if (Math.Abs(npc.Top.Y - Main.player[npc.target].Bottom.Y) > 320f)
				npc.ai[2] += 1f;

			Dust dust;
			if (npc.ai[1] == 5f)
			{
				flag8 = true;
				npc.aiAction = 1;
				npc.ai[0] += ((enraged || Config.BossRushXerocCurse) ? 2f : 1f);
				num234 = MathHelper.Clamp((60f - npc.ai[0]) / 60f, 0f, 1f);
				num234 = 0.5f + num234 * 0.5f;

				if (npc.ai[0] >= 60f)
					flag9 = true;

				if (!flag9)
				{
					int num;
					for (int num245 = 0; num245 < 10; num245 = num + 1)
					{
						int num246 = Dust.NewDust(npc.position + Vector2.UnitX * -20f, npc.width + 40, npc.height, 4, npc.velocity.X, npc.velocity.Y, 150, new Color(78, 136, 255, 80), 2f);
						Main.dust[num246].noGravity = true;
						dust = Main.dust[num246];
						dust.velocity *= 0.5f;
						num = num245;
					}
				}

				if (npc.ai[0] == 60f)
					Gore.NewGore(npc.Center + new Vector2(-40f, (float)(-(float)npc.height / 2)), npc.velocity, 734, 1f);

				if (npc.ai[0] >= 60f && Main.netMode != 1)
				{
					npc.Bottom = new Vector2(npc.localAI[1], npc.localAI[2]);
					npc.ai[1] = 6f;
					npc.ai[0] = 0f;
					npc.netUpdate = true;
				}

				if (Main.netMode == 1 && npc.ai[0] >= 120f)
				{
					npc.ai[1] = 6f;
					npc.ai[0] = 0f;
				}
			}
			else if (npc.ai[1] == 6f)
			{
				flag8 = true;
				npc.aiAction = 0;
				npc.ai[0] += ((enraged || Config.BossRushXerocCurse) ? 2f : 1f);
				num234 = MathHelper.Clamp(npc.ai[0] / 30f, 0f, 1f);
				num234 = 0.5f + num234 * 0.5f;

				if (npc.ai[0] >= 30f && Main.netMode != 1)
				{
					npc.ai[1] = 0f;
					npc.ai[0] = 0f;
					npc.netUpdate = true;
					npc.TargetClosest(true);
				}

				if (Main.netMode == 1 && npc.ai[0] >= 60f)
				{
					npc.ai[1] = 0f;
					npc.ai[0] = 0f;
					npc.TargetClosest(true);
				}

				int num;
				for (int num247 = 0; num247 < 10; num247 = num + 1)
				{
					int num248 = Dust.NewDust(npc.position + Vector2.UnitX * -20f, npc.width + 40, npc.height, 4, npc.velocity.X, npc.velocity.Y, 150, new Color(78, 136, 255, 80), 2f);
					Main.dust[num248].noGravity = true;
					dust = Main.dust[num248];
					dust.velocity *= 2f;
					num = num247;
				}
			}

			npc.dontTakeDamage = (npc.hide = flag9);

			if (npc.velocity.Y == 0f)
			{
				npc.velocity.X = npc.velocity.X * 0.8f;

				if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
					npc.velocity.X = 0f;

				if (!flag8)
				{
					npc.ai[0] += (enraged || Config.BossRushXerocCurse) ? 4f : 2f;
					if ((double)npc.life < (double)npc.lifeMax * 0.8)
						npc.ai[0] += 1f;
					if ((double)npc.life < (double)npc.lifeMax * 0.6)
						npc.ai[0] += 1f;
					if ((double)npc.life < (double)npc.lifeMax * 0.4)
						npc.ai[0] += 2f;
					if ((double)npc.life < (double)npc.lifeMax * 0.2)
						npc.ai[0] += 3f;
					if ((double)npc.life < (double)npc.lifeMax * 0.1)
						npc.ai[0] += 4f;

					if (npc.ai[0] >= 0f)
					{
						npc.netUpdate = true;
						npc.TargetClosest(true);

						if (npc.ai[1] == 3f)
						{
							npc.velocity.Y = -26f;
							npc.velocity.X = npc.velocity.X + 7f * (float)npc.direction;
							npc.ai[0] = -200f;
							npc.ai[1] = 0f;
						}
						else if (npc.ai[1] == 2f)
						{
							npc.velocity.Y = -12f;
							npc.velocity.X = npc.velocity.X + 9f * (float)npc.direction;
							npc.ai[0] = -120f;
							npc.ai[1] += 1f;
						}
						else
						{
							npc.velocity.Y = -16f;
							npc.velocity.X = npc.velocity.X + 8f * (float)npc.direction;
							npc.ai[0] = -120f;
							npc.ai[1] += 1f;
						}
					}
					else if (npc.ai[0] >= -30f)
						npc.aiAction = 1;
				}
			}
			else if (npc.target < 255 && ((npc.direction == 1 && npc.velocity.X < 3f) || (npc.direction == -1 && npc.velocity.X > -3f)))
			{
				if ((npc.direction == -1 && (double)npc.velocity.X < 0.1) || (npc.direction == 1 && (double)npc.velocity.X > -0.1))
					npc.velocity.X = npc.velocity.X + 0.2f * (float)npc.direction;
				else
					npc.velocity.X = npc.velocity.X * 0.93f;
			}

			int num249 = Dust.NewDust(npc.position, npc.width, npc.height, 4, npc.velocity.X, npc.velocity.Y, 255, new Color(0, 80, 255, 80), npc.scale * 1.2f);
			Main.dust[num249].noGravity = true;
			dust = Main.dust[num249];
			dust.velocity *= 0.5f;

			if (npc.life > 0)
			{
				float num250 = (float)npc.life / (float)npc.lifeMax;
				num250 = num250 * 0.5f + 0.75f;
				num250 *= num234;
				if (num250 != npc.scale)
				{
					npc.position.X = npc.position.X + (float)(npc.width / 2);
					npc.position.Y = npc.position.Y + (float)npc.height;
					npc.scale = num250;
					npc.width = (int)(98f * npc.scale);
					npc.height = (int)(92f * npc.scale);
					npc.position.X = npc.position.X - (float)(npc.width / 2);
					npc.position.Y = npc.position.Y - (float)npc.height;
				}

				if (Main.netMode != 1)
				{
					int num251 = (int)((double)npc.lifeMax * 0.05);
					if ((float)(npc.life + num251) < npc.ai[3])
					{
						npc.ai[3] = (float)npc.life;
						int num252 = Main.rand.Next(1, 4);
						int num;
						for (int num253 = 0; num253 < num252; num253 = num + 1)
						{
							int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
							int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));

							int num254 = 1;
							if (Main.rand.Next(4) == 0)
								num254 = 535;

							int num255 = NPC.NewNPC(x, y, num254, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num255].SetDefaults(num254, -1f);
							Main.npc[num255].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
							Main.npc[num255].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
							Main.npc[num255].ai[0] = (float)(-1000 * Main.rand.Next(3));
							Main.npc[num255].ai[1] = 0f;

							if (Main.netMode == 2 && num255 < 200)
								NetMessage.SendData(23, -1, -1, null, num255, 0f, 0f, 0f, 0, 0, 0);

							num = num253;
						}
					}
				}
			}
			return false;
		}
		#endregion

		#region Boss Rush Brain of Cthulhu AI
		public static bool BossRushBrainofCthulhuAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);
			NPC.crimsonBoss = npc.whoAmI;
			npc.dontTakeDamage = false;

			if (Main.netMode != 1 && npc.localAI[0] == 0f)
			{
				npc.localAI[0] = 1f;
				int num;
				for (int num789 = 0; num789 < 20; num789 = num + 1)
				{
					float num790 = npc.Center.X;
					float num791 = npc.Center.Y;
					num790 += (float)Main.rand.Next(-npc.width, npc.width);
					num791 += (float)Main.rand.Next(-npc.height, npc.height);
					int num792 = NPC.NewNPC((int)num790, (int)num791, 267, 0, 0f, 0f, 0f, 0f, 255);
					Main.npc[num792].velocity = new Vector2((float)Main.rand.Next(-30, 31) * 0.1f, (float)Main.rand.Next(-30, 31) * 0.1f);
					Main.npc[num792].netUpdate = true;
					num = num789;
				}
			}

			if (npc.ai[0] < 0f)
			{
				if (npc.localAI[2] == 0f)
				{
					Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);
					npc.localAI[2] = 1f;

					Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 392, 1f);
					Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 393, 1f);
					Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 394, 1f);
					Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 395, 1f);

					int num;
					for (int num794 = 0; num794 < 20; num794 = num + 1)
					{
						Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);
						num = num794;
					}

					Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
				}

				calamityGlobalNPC.newAI[0] += 1f;
				if (calamityGlobalNPC.newAI[0] >= 60f)
				{
					calamityGlobalNPC.newAI[0] = 0f;
					if (Main.netMode != 1 && NPC.CountNPCS(NPCID.Creeper) < 15)
					{
						int creeper = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2) + npc.velocity.X), (int)(npc.position.Y + (float)(npc.height / 2) + npc.velocity.Y), 267, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[creeper].netUpdate = true;
					}
					npc.netUpdate = true;
				}

				npc.knockBackResist = 0f;
				npc.TargetClosest(true);

				Vector2 vector98 = new Vector2(npc.Center.X, npc.Center.Y);
				float num795 = Main.player[npc.target].Center.X - vector98.X;
				float num796 = Main.player[npc.target].Center.Y - vector98.Y;
				float num797 = (float)Math.Sqrt((double)(num795 * num795 + num796 * num796));
				float num798 = ((enraged || Config.BossRushXerocCurse) ? 21f : 14f);

				num797 = num798 / num797;
				num795 *= num797;
				num796 *= num797;

				npc.velocity.X = (npc.velocity.X * 50f + num795) / 51f;
				npc.velocity.Y = (npc.velocity.Y * 50f + num796) / 51f;

				if (npc.ai[0] == -1f)
				{
					if (Main.netMode != 1)
					{
						npc.localAI[1] += 1f;
						if (npc.justHit)
							npc.localAI[1] -= (float)Main.rand.Next(5);

						int num799 = 60 + Main.rand.Next(120);
						if (Main.netMode != 0)
							num799 += Main.rand.Next(30, 90);

						if (npc.localAI[1] >= (float)num799)
						{
							npc.localAI[1] = 0f;
							npc.TargetClosest(true);

							int num800 = 0;
							int num801;
							int num802;

							while (true)
							{
								num800++;
								num801 = (int)Main.player[npc.target].Center.X / 16;
								num802 = (int)Main.player[npc.target].Center.Y / 16;

								if (Main.rand.Next(2) == 0)
									num801 += Main.rand.Next(7, 13);
								else
									num801 -= Main.rand.Next(7, 13);

								if (Main.rand.Next(2) == 0)
									num802 += Main.rand.Next(7, 13);
								else
									num802 -= Main.rand.Next(7, 13);

								if (!WorldGen.SolidTile(num801, num802))
									break;
								if (num800 > 100)
									goto Block_2784;
							}

							npc.ai[3] = 0f;
							npc.ai[0] = -2f;
							npc.ai[1] = (float)num801;
							npc.ai[2] = (float)num802;

							npc.netUpdate = true;
							npc.netSpam = 0;
							Block_2784:;
						}
					}
				}
				else if (npc.ai[0] == -2f)
				{
					npc.velocity *= 0.9f;

					if (Main.netMode != 0)
						npc.ai[3] += 15f;
					else
						npc.ai[3] += 25f;

					if (npc.ai[3] >= 255f)
					{
						npc.ai[3] = 255f;
						npc.ai[0] = -3f;

						npc.position.X = npc.ai[1] * 16f - (float)(npc.width / 2);
						npc.position.Y = npc.ai[2] * 16f - (float)(npc.height / 2);

						Main.PlaySound(SoundID.Item8, npc.Center);
						npc.netUpdate = true;
						npc.netSpam = 0;
					}
					npc.alpha = (int)npc.ai[3];
				}
				else if (npc.ai[0] == -3f)
				{
					if (Main.netMode != 0)
						npc.ai[3] -= 15f;
					else
						npc.ai[3] -= 25f;

					if (npc.ai[3] <= 0f)
					{
						npc.ai[3] = 0f;
						npc.ai[0] = -1f;
						npc.netUpdate = true;
						npc.netSpam = 0;
					}
					npc.alpha = (int)npc.ai[3];
				}
			}
			else
			{
				npc.TargetClosest(true);

				Vector2 vector99 = new Vector2(npc.Center.X, npc.Center.Y);
				float num803 = Main.player[npc.target].Center.X - vector99.X;
				float num804 = Main.player[npc.target].Center.Y - vector99.Y;
				float num805 = (float)Math.Sqrt((double)(num803 * num803 + num804 * num804));
				float num806 = ((enraged || Config.BossRushXerocCurse) ? 3f : 2f);

				if (num805 < num806)
				{
					npc.velocity.X = num803;
					npc.velocity.Y = num804;
				}
				else
				{
					num805 = num806 / num805;
					npc.velocity.X = num803 * num805;
					npc.velocity.Y = num804 * num805;
				}

				if (npc.ai[0] == 0f)
				{
					if (Main.netMode != 1)
					{
						int num807 = 0;
						int num;
						for (int num808 = 0; num808 < 200; num808 = num + 1)
						{
							if (Main.npc[num808].active && Main.npc[num808].type == 267)
								num807++;

							num = num808;
						}
						if (num807 == 0)
						{
							npc.ai[0] = -1f;
							npc.localAI[1] = 0f;
							npc.alpha = 0;
							npc.netUpdate = true;
						}

						npc.localAI[1] += 1f;
						if (npc.localAI[1] >= (float)(120 + Main.rand.Next(120)))
						{
							npc.localAI[1] = 0f;
							npc.TargetClosest(true);

							int num809 = 0;
							int num810;
							int num811;

							while (true)
							{
								num809++;
								num810 = (int)Main.player[npc.target].Center.X / 16;
								num811 = (int)Main.player[npc.target].Center.Y / 16;
								num810 += Main.rand.Next(-50, 51);
								num811 += Main.rand.Next(-50, 51);

								if (!WorldGen.SolidTile(num810, num811) && Collision.CanHit(new Vector2((float)(num810 * 16), (float)(num811 * 16)), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
									break;
								if (num809 > 100)
									goto Block_2801;
							}

							npc.ai[0] = 1f;
							npc.ai[1] = (float)num810;
							npc.ai[2] = (float)num811;

							npc.netUpdate = true;
							Block_2801:;
						}
					}
				}
				else if (npc.ai[0] == 1f)
				{
					npc.alpha += 5;
					if (npc.alpha >= 255)
					{
						Main.PlaySound(SoundID.Item8, npc.Center);
						npc.alpha = 255;
						npc.position.X = npc.ai[1] * 16f - (float)(npc.width / 2);
						npc.position.Y = npc.ai[2] * 16f - (float)(npc.height / 2);
						npc.ai[0] = 2f;
					}
				}
				else if (npc.ai[0] == 2f)
				{
					npc.alpha -= 5;
					if (npc.alpha <= 0)
					{
						npc.alpha = 0;
						npc.ai[0] = 0f;
					}
				}
			}

			if (Main.player[npc.target].dead)
			{
				if (npc.localAI[3] < 120f)
					npc.localAI[3] += 1f;

				if (npc.localAI[3] > 60f)
					npc.velocity.Y = npc.velocity.Y + (npc.localAI[3] - 60f) * 0.25f;

				npc.ai[0] = 2f;
				npc.alpha = 10;
				return false;
			}
			if (npc.localAI[3] > 0f)
				npc.localAI[3] -= 1f;

			return false;
		}
		#endregion

		#region Boss Rush Eater of Worlds AI
		public static bool BossRushEaterofWorldsAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);
			Main.player[npc.target].ZoneCorrupt = true;
			npc.reflectingProjectiles = true;

			if (!Main.player[npc.target].dead)
				calamityGlobalNPC.newAI[0] += ((enraged || Config.BossRushXerocCurse) ? 6f : 3f);
			if (calamityGlobalNPC.newAI[0] >= 180f)
			{
				calamityGlobalNPC.newAI[0] = 0f;
				if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num349 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
					float num350 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
					float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
					num349 *= num351;
					num350 *= num351;

					if (Main.netMode != 1)
					{
						float num418 = ((enraged || Config.BossRushXerocCurse) ? 18f : 12f);
						int num419 = 12;
						int num420 = 96;
						num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
						num351 = num418 / num351;
						num349 *= num351;
						num350 *= num351;
						num349 += (float)Main.rand.Next(-40, 41) * 0.05f;
						num350 += (float)Main.rand.Next(-40, 41) * 0.05f;
						vector34.X += num349 * 4f;
						vector34.Y += num350 * 4f;
						Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350, num420, num419, 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}
			return false;
		}
		#endregion

		#region Boss Rush Queen Bee AI
		public static bool BossRushQueenBeeAI(NPC npc, bool enraged)
		{
			int num592 = 0;
			int variable;
			for (int num593 = 0; num593 < 255; num593 = variable + 1)
			{
				if (Main.player[num593].active && !Main.player[num593].dead && (npc.Center - Main.player[num593].Center).Length() < 1000f)
					num592++;

				variable = num593;
			}

			int num594 = (int)(20f * (1f - (float)npc.life / (float)npc.lifeMax));
			npc.defense = npc.defDefense + num594;

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			bool dead4 = Main.player[npc.target].dead;
			if (dead4)
			{
				if ((double)npc.position.Y < Main.worldSurface * 16.0 + 2000.0)
					npc.velocity.Y = npc.velocity.Y + 0.04f;

				if (npc.position.X < (float)(Main.maxTilesX * 8))
					npc.velocity.X = npc.velocity.X - 0.04f;
				else
					npc.velocity.X = npc.velocity.X + 0.04f;

				if (npc.timeLeft > 10)
					npc.timeLeft = 10;
			}
			else if (npc.ai[0] == -1f)
			{
				if (Main.netMode != 1)
				{
					float num595 = npc.ai[1];
					int num596;
					do
					{
						num596 = Main.rand.Next(3);
						if (num596 == 1)
							num596 = 2;
						else if (num596 == 2)
							num596 = 3;
					}
					while ((float)num596 == num595);

					npc.ai[0] = (float)num596;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
				}
			}
			else if (npc.ai[0] == 0f)
			{
				if (npc.ai[1] > 10f && npc.ai[1] % 2f == 0f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
					return false;
				}

				if (npc.ai[1] % 2f == 0f)
				{
					npc.TargetClosest(true);

					if (Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))) < 20f)
					{
						npc.localAI[0] = 1f;
						npc.ai[1] += 1f;
						npc.ai[2] = 0f;

						float num598 = 24f;
						Vector2 vector74 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num599 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector74.X;
						float num600 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector74.Y;
						float num601 = (float)Math.Sqrt((double)(num599 * num599 + num600 * num600));

						num601 = num598 / num601;
						npc.velocity.X = num599 * num601;
						npc.velocity.Y = num600 * num601;

						npc.spriteDirection = npc.direction;
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
						return false;
					}

					npc.localAI[0] = 0f;
					float num602 = 18f;
					float num603 = 0.4f;

					if (npc.position.Y + (float)(npc.height / 2) < Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))
						npc.velocity.Y = npc.velocity.Y + num603;
					else
						npc.velocity.Y = npc.velocity.Y - num603;

					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -num602;
					if (npc.velocity.Y > 12f)
						npc.velocity.Y = num602;

					if (Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) > 600f)
						npc.velocity.X = npc.velocity.X + 0.15f * (float)npc.direction;
					else if (Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) < 300f)
						npc.velocity.X = npc.velocity.X - 0.15f * (float)npc.direction;
					else
						npc.velocity.X = npc.velocity.X * 0.8f;

					if (npc.velocity.X < -16f)
						npc.velocity.X = -16f;
					if (npc.velocity.X > 16f)
						npc.velocity.X = 16f;

					npc.spriteDirection = npc.direction;
				}
				else
				{
					if (npc.velocity.X < 0f)
						npc.direction = -1;
					else
						npc.direction = 1;
					npc.spriteDirection = npc.direction;

					int num605 = 1;
					if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))
						num605 = -1;

					int num604 = ((enraged || Config.BossRushXerocCurse) ? 150 : 250);
					if (npc.direction == num605 && Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) > (float)num604)
						npc.ai[2] = 1f;

					if (npc.ai[2] != 1f)
					{
						npc.localAI[0] = 1f;
						return false;
					}

					npc.TargetClosest(true);
					npc.spriteDirection = npc.direction;
					npc.localAI[0] = 0f;

					npc.velocity *= 0.9f;
					if (npc.life < npc.lifeMax / 2)
						npc.velocity *= 0.9f;
					if (npc.life < npc.lifeMax / 3)
						npc.velocity *= 0.9f;
					if (npc.life < npc.lifeMax / 5)
						npc.velocity *= 0.9f;

					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 0.25f)
					{
						npc.ai[2] = 0f;
						npc.ai[1] += 1f;
					}
				}
			}
			else if (npc.ai[0] == 2f)
			{
				npc.TargetClosest(true);
				npc.spriteDirection = npc.direction;

				float num607 = 12f;
				float num608 = 0.1f;
				Vector2 vector75 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num609 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector75.X;
				float num610 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 200f - vector75.Y;

				float num611 = (float)Math.Sqrt((double)(num609 * num609 + num610 * num610));
				if (num611 < 200f)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.netUpdate = true;
					return false;
				}
				num611 = num607 / num611;

				if (npc.velocity.X < num609)
				{
					npc.velocity.X = npc.velocity.X + num608;
					if (npc.velocity.X < 0f && num609 > 0f)
						npc.velocity.X = npc.velocity.X + num608;
				}
				else if (npc.velocity.X > num609)
				{
					npc.velocity.X = npc.velocity.X - num608;
					if (npc.velocity.X > 0f && num609 < 0f)
						npc.velocity.X = npc.velocity.X - num608;
				}
				if (npc.velocity.Y < num610)
				{
					npc.velocity.Y = npc.velocity.Y + num608;
					if (npc.velocity.Y < 0f && num610 > 0f)
						npc.velocity.Y = npc.velocity.Y + num608;
				}
				else if (npc.velocity.Y > num610)
				{
					npc.velocity.Y = npc.velocity.Y - num608;
					if (npc.velocity.Y > 0f && num610 < 0f)
						npc.velocity.Y = npc.velocity.Y - num608;
				}
			}
			else if (npc.ai[0] == 1f)
			{
				npc.localAI[0] = 0f;
				npc.TargetClosest(true);

				Vector2 vector76 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				Vector2 vector77 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num612 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector77.X;
				float num613 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector77.Y;
				float num614 = (float)Math.Sqrt((double)(num612 * num612 + num613 * num613));

				npc.ai[1] += 1f;
				npc.ai[1] += (float)(num592 / 2);
				bool flag38 = false;

				if (npc.ai[1] > 10f)
				{
					npc.ai[1] = 0f;
					npc.ai[2] += 1f;
					flag38 = true;
				}

				if (Collision.CanHit(vector76, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && flag38)
				{
					Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);
					if (Main.netMode != 1)
					{
						int num615 = Main.rand.Next(210, 212);
						int num616 = NPC.NewNPC((int)vector76.X, (int)vector76.Y, num615, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[num616].velocity.X = (float)Main.rand.Next(-200, 201) * 0.002f;
						Main.npc[num616].velocity.Y = (float)Main.rand.Next(-200, 201) * 0.002f;
						Main.npc[num616].localAI[0] = 60f;
						Main.npc[num616].netUpdate = true;
					}
				}

				if (num614 > 400f || !Collision.CanHit(new Vector2(vector76.X, vector76.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					float num617 = 14f;
					float num618 = 0.1f;
					vector77 = vector76;

					num612 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector77.X;
					num613 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector77.Y;
					num614 = (float)Math.Sqrt((double)(num612 * num612 + num613 * num613));
					num614 = num617 / num614;

					if (npc.velocity.X < num612)
					{
						npc.velocity.X = npc.velocity.X + num618;
						if (npc.velocity.X < 0f && num612 > 0f)
							npc.velocity.X = npc.velocity.X + num618;
					}
					else if (npc.velocity.X > num612)
					{
						npc.velocity.X = npc.velocity.X - num618;
						if (npc.velocity.X > 0f && num612 < 0f)
							npc.velocity.X = npc.velocity.X - num618;
					}
					if (npc.velocity.Y < num613)
					{
						npc.velocity.Y = npc.velocity.Y + num618;
						if (npc.velocity.Y < 0f && num613 > 0f)
							npc.velocity.Y = npc.velocity.Y + num618;
					}
					else if (npc.velocity.Y > num613)
					{
						npc.velocity.Y = npc.velocity.Y - num618;
						if (npc.velocity.Y > 0f && num613 < 0f)
							npc.velocity.Y = npc.velocity.Y - num618;
					}
				}
				else
					npc.velocity *= 0.9f;

				npc.spriteDirection = npc.direction;

				if (npc.ai[2] > 5f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 1f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 3f)
			{
				float num619 = 6f;
				float num620 = 0.075f;

				Vector2 vector78 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				Vector2 vector79 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num621 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector79.X;
				float num622 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 300f - vector79.Y;
				float num623 = (float)Math.Sqrt((double)(num621 * num621 + num622 * num622));

				npc.ai[1] += 1f;
				bool flag39 = false;
				if (npc.ai[1] % 15f == 14f)
					flag39 = true;

				if (flag39 && npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y && Collision.CanHit(vector78, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					Main.PlaySound(SoundID.Item17, npc.position);
					if (Main.netMode != 1)
					{
						float num624 = ((enraged || Config.BossRushXerocCurse) ? 24f : 16f);
						float num625 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector78.X + (float)Main.rand.Next(-80, 81);
						float num626 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector78.Y + (float)Main.rand.Next(-40, 41);
						float num627 = (float)Math.Sqrt((double)(num625 * num625 + num626 * num626));

						num627 = num624 / num627;
						num625 *= num627;
						num626 *= num627;

						int num628 = 11;
						int num629 = 55;
						int num630 = Projectile.NewProjectile(vector78.X, vector78.Y, num625, num626, num629, num628, 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[num630].timeLeft = 300;
					}
				}

				if (!Collision.CanHit(new Vector2(vector78.X, vector78.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					num619 = 14f;
					num620 = 0.1f;
					vector79 = vector78;

					num621 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector79.X;
					num622 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector79.Y;
					num623 = (float)Math.Sqrt((double)(num621 * num621 + num622 * num622));
					num623 = num619 / num623;

					if (npc.velocity.X < num621)
					{
						npc.velocity.X = npc.velocity.X + num620;
						if (npc.velocity.X < 0f && num621 > 0f)
							npc.velocity.X = npc.velocity.X + num620;
					}
					else if (npc.velocity.X > num621)
					{
						npc.velocity.X = npc.velocity.X - num620;
						if (npc.velocity.X > 0f && num621 < 0f)
							npc.velocity.X = npc.velocity.X - num620;
					}
					if (npc.velocity.Y < num622)
					{
						npc.velocity.Y = npc.velocity.Y + num620;
						if (npc.velocity.Y < 0f && num622 > 0f)
							npc.velocity.Y = npc.velocity.Y + num620;
					}
					else if (npc.velocity.Y > num622)
					{
						npc.velocity.Y = npc.velocity.Y - num620;
						if (npc.velocity.Y > 0f && num622 < 0f)
							npc.velocity.Y = npc.velocity.Y - num620;
					}
				}
				else if (num623 > 100f)
				{
					npc.TargetClosest(true);
					npc.spriteDirection = npc.direction;

					num623 = num619 / num623;
					if (npc.velocity.X < num621)
					{
						npc.velocity.X = npc.velocity.X + num620;
						if (npc.velocity.X < 0f && num621 > 0f)
							npc.velocity.X = npc.velocity.X + num620 * 2f;
					}
					else if (npc.velocity.X > num621)
					{
						npc.velocity.X = npc.velocity.X - num620;
						if (npc.velocity.X > 0f && num621 < 0f)
							npc.velocity.X = npc.velocity.X - num620 * 2f;
					}
					if (npc.velocity.Y < num622)
					{
						npc.velocity.Y = npc.velocity.Y + num620;
						if (npc.velocity.Y < 0f && num622 > 0f)
							npc.velocity.Y = npc.velocity.Y + num620 * 2f;
					}
					else if (npc.velocity.Y > num622)
					{
						npc.velocity.Y = npc.velocity.Y - num620;
						if (npc.velocity.Y > 0f && num622 < 0f)
							npc.velocity.Y = npc.velocity.Y - num620 * 2f;
					}
				}

				if (npc.ai[1] > 400f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 3f;
					npc.netUpdate = true;
				}
			}
			return false;
		}
		#endregion

		#region Boss Rush Duke Fishron AI
		public static bool BossRushDukeFishronAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);

			if (npc.ai[0] == -1f || npc.ai[0] == 4f || npc.ai[0] == 9f)
				npc.dontTakeDamage = true;
			else if (npc.ai[0] <= 8f)
				npc.dontTakeDamage = false;

			float newDamage = (0.6f * Main.damageMultiplier);
			bool flag = (double)npc.life <= (double)npc.lifeMax * 0.99;
			bool flag2 = (double)npc.life <= (double)npc.lifeMax * 0.15;
			bool flag3 = npc.ai[0] > 4f;
			bool flag4 = npc.ai[0] > 9f;
			bool flag5 = npc.ai[3] < 10f;
			Vector2 vector = npc.Center;

			if (flag4)
			{
				calamityGlobalNPC.newAI[0] += 1f;
				if (calamityGlobalNPC.newAI[0] >= 480f)
				{
					calamityGlobalNPC.newAI[0] = 0f;

					if (Main.netMode != 1)
						Projectile.NewProjectile(vector.X, vector.Y, 0f, 0f, 385, 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));

					npc.netUpdate = true;
				}
				npc.damage = (int)((float)npc.defDamage * 1.5f * newDamage);
				npc.defense = 38;
			}
			else if (flag3)
			{
				npc.damage = (int)((float)npc.defDamage * 2.2f * newDamage);
				npc.defense = (int)((float)npc.defDefense * 0.8f);
			}
			else
			{
				npc.damage = npc.defDamage;
				npc.defense = npc.defDefense;
			}

			int num2 = 35;
			float num3 = 0.65f;
			float scaleFactor = 9.5f;
			if (flag4)
			{
				num3 = ((enraged || Config.BossRushXerocCurse) ? 0.9f : 0.8f);
				scaleFactor = ((enraged || Config.BossRushXerocCurse) ? 15f : 13f);
				num2 = ((enraged || Config.BossRushXerocCurse) ? 22 : 25);
			}
			else if (flag3 & flag5)
			{
				num3 = ((enraged || Config.BossRushXerocCurse) ? 0.8f : 0.7f);
				scaleFactor = ((enraged || Config.BossRushXerocCurse) ? 13f : 11f);
				num2 = ((enraged || Config.BossRushXerocCurse) ? 30 : 35);
			}
			else if (flag5 && !flag3 && !flag4)
				num2 = 25;

			int num4 = 24;
			float num5 = 18f;
			if (flag4)
			{
				num4 = ((enraged || Config.BossRushXerocCurse) ? 18 : 20);
				num5 = ((enraged || Config.BossRushXerocCurse) ? 30f : 28f);
			}
			else if (flag5 & flag3)
			{
				num4 = ((enraged || Config.BossRushXerocCurse) ? 21 : 23);
				num5 = ((enraged || Config.BossRushXerocCurse) ? 25f : 22f);
			}

			int num6 = 80;
			int num7 = 4;
			float num8 = 0.3f;
			float scaleFactor2 = 5f;
			int num9 = 90;
			int num10 = 180;
			int num11 = 180;
			int num12 = 30;
			int num13 = 120;
			int num14 = 4;
			float scaleFactor3 = 6f;
			float scaleFactor4 = 20f;
			float num15 = 6.28318548f / (float)(num13 / 2);
			int num16 = 75;

			Player player = Main.player[npc.target];
			if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
			{
				npc.TargetClosest(true);
				player = Main.player[npc.target];
				npc.netUpdate = true;
			}

			if (player.dead || Vector2.Distance(player.Center, vector) > 5600f)
			{
				npc.velocity.Y = npc.velocity.Y - 0.4f;

				if (npc.timeLeft > 10)
					npc.timeLeft = 10;

				if (npc.ai[0] > 4f)
					npc.ai[0] = 5f;
				else
					npc.ai[0] = 0f;

				npc.ai[2] = 0f;
			}

			if (player.position.Y < 800f || (double)player.position.Y > Main.worldSurface * 16.0 || (player.position.X > 6400f && player.position.X < (float)(Main.maxTilesX * 16 - 6400)))
			{
				num2 = 20;
				num5 += 6f;
			}

			if (npc.localAI[0] == 0f)
			{
				npc.localAI[0] = 1f;
				npc.alpha = 255;
				npc.rotation = 0f;
				if (Main.netMode != 1)
				{
					npc.ai[0] = -1f;
					npc.netUpdate = true;
				}
			}

			float num17 = (float)Math.Atan2((double)(player.Center.Y - vector.Y), (double)(player.Center.X - vector.X));
			if (npc.spriteDirection == 1)
				num17 += 3.14159274f;
			if (num17 < 0f)
				num17 += 6.28318548f;
			if (num17 > 6.28318548f)
				num17 -= 6.28318548f;
			if (npc.ai[0] == -1f)
				num17 = 0f;
			if (npc.ai[0] == 3f)
				num17 = 0f;
			if (npc.ai[0] == 4f)
				num17 = 0f;
			if (npc.ai[0] == 8f)
				num17 = 0f;

			float num18 = 0.04f;
			if (npc.ai[0] == 1f || npc.ai[0] == 6f)
				num18 = 0f;
			if (npc.ai[0] == 7f)
				num18 = 0f;
			if (npc.ai[0] == 3f)
				num18 = 0.01f;
			if (npc.ai[0] == 4f)
				num18 = 0.01f;
			if (npc.ai[0] == 8f)
				num18 = 0.01f;

			if (npc.rotation < num17)
			{
				if ((double)(num17 - npc.rotation) > 3.1415926535897931)
					npc.rotation -= num18;
				else
					npc.rotation += num18;
			}

			if (npc.rotation > num17)
			{
				if ((double)(npc.rotation - num17) > 3.1415926535897931)
					npc.rotation += num18;
				else
					npc.rotation -= num18;
			}

			if (npc.rotation > num17 - num18 && npc.rotation < num17 + num18)
				npc.rotation = num17;
			if (npc.rotation < 0f)
				npc.rotation += 6.28318548f;
			if (npc.rotation > 6.28318548f)
				npc.rotation -= 6.28318548f;
			if (npc.rotation > num17 - num18 && npc.rotation < num17 + num18)
				npc.rotation = num17;

			if (npc.ai[0] != -1f && npc.ai[0] < 9f)
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
			if (npc.ai[0] == -1f)
			{
				npc.velocity *= 0.98f;

				int num19 = Math.Sign(player.Center.X - vector.X);
				if (num19 != 0)
				{
					npc.direction = num19;
					npc.spriteDirection = -npc.direction;
				}

				if (npc.ai[2] > 20f)
				{
					npc.velocity.Y = -2f;
					npc.alpha -= 5;

					if (Collision.SolidCollision(npc.position, npc.width, npc.height))
						npc.alpha += 15;
					if (npc.alpha < 0)
						npc.alpha = 0;
					if (npc.alpha > 150)
						npc.alpha = 150;
				}

				if (npc.ai[2] == (float)(num9 - 30))
				{
					int num20 = 36;
					for (int i = 0; i < num20; i++)
					{
						Vector2 expr_80F = (Vector2.Normalize(npc.velocity) * new Vector2((float)npc.width / 2f, (float)npc.height) * 0.75f * 0.5f).RotatedBy((double)((float)(i - (num20 / 2 - 1)) * 6.28318548f / (float)num20), default(Vector2)) + npc.Center;
						Vector2 vector2 = expr_80F - npc.Center;
						int num21 = Dust.NewDust(expr_80F + vector2, 0, 0, 172, vector2.X * 2f, vector2.Y * 2f, 100, default(Color), 1.4f);
						Main.dust[num21].noGravity = true;
						Main.dust[num21].noLight = true;
						Main.dust[num21].velocity = Vector2.Normalize(vector2) * 3f;
					}
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num16)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 0f && !player.dead)
			{
				if (npc.ai[1] == 0f)
					npc.ai[1] = (float)(300 * Math.Sign((vector - player.Center).X));

				Vector2 vector3 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * scaleFactor;
				if (npc.velocity.X < vector3.X)
				{
					npc.velocity.X = npc.velocity.X + num3;
					if (npc.velocity.X < 0f && vector3.X > 0f)
						npc.velocity.X = npc.velocity.X + num3;
				}
				else if (npc.velocity.X > vector3.X)
				{
					npc.velocity.X = npc.velocity.X - num3;
					if (npc.velocity.X > 0f && vector3.X < 0f)
						npc.velocity.X = npc.velocity.X - num3;
				}
				if (npc.velocity.Y < vector3.Y)
				{
					npc.velocity.Y = npc.velocity.Y + num3;
					if (npc.velocity.Y < 0f && vector3.Y > 0f)
						npc.velocity.Y = npc.velocity.Y + num3;
				}
				else if (npc.velocity.Y > vector3.Y)
				{
					npc.velocity.Y = npc.velocity.Y - num3;
					if (npc.velocity.Y > 0f && vector3.Y < 0f)
						npc.velocity.Y = npc.velocity.Y - num3;
				}

				int num22 = Math.Sign(player.Center.X - vector.X);
				if (num22 != 0)
				{
					if (npc.ai[2] == 0f && num22 != npc.direction)
						npc.rotation += 3.14159274f;

					npc.direction = num22;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += 3.14159274f;

					npc.spriteDirection = -npc.direction;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num2)
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
						case 6:
						case 7:
						case 8:
						case 9:
							num23 = 1;
							break;
						case 10:
							npc.ai[3] = 1f;
							num23 = 2;
							break;
						case 11:
							npc.ai[3] = 0f;
							num23 = 3;
							break;
					}

					if (flag)
						num23 = 4;

					if (num23 == 1)
					{
						npc.ai[0] = 1f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.velocity = Vector2.Normalize(player.Center - vector) * num5;
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

						if (num22 != 0)
						{
							npc.direction = num22;

							if (npc.spriteDirection == 1)
								npc.rotation += 3.14159274f;

							npc.spriteDirection = -npc.direction;
						}
					}
					else if (num23 == 2)
					{
						npc.ai[0] = 2f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					else if (num23 == 3)
					{
						npc.ai[0] = 3f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					else if (num23 == 4)
					{
						npc.ai[0] = 4f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 1f)
			{
				int num24 = 7;
				for (int j = 0; j < num24; j++)
				{
					Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(j - (num24 / 2 - 1)) * 3.1415926535897931 / (double)((float)num24), default(Vector2)) + vector;
					Vector2 vector4 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, 172, vector4.X * 2f, vector4.Y * 2f, 100, default(Color), 1.4f);
					Main.dust[num25].noGravity = true;
					Main.dust[num25].noLight = true;
					Main.dust[num25].velocity /= 4f;
					Main.dust[num25].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num4)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 2f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 2f)
			{
				if (npc.ai[1] == 0f)
					npc.ai[1] = (float)(300 * Math.Sign((vector - player.Center).X));

				Vector2 vector5 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * scaleFactor2;
				if (npc.velocity.X < vector5.X)
				{
					npc.velocity.X = npc.velocity.X + num8;
					if (npc.velocity.X < 0f && vector5.X > 0f)
						npc.velocity.X = npc.velocity.X + num8;
				}
				else if (npc.velocity.X > vector5.X)
				{
					npc.velocity.X = npc.velocity.X - num8;
					if (npc.velocity.X > 0f && vector5.X < 0f)
						npc.velocity.X = npc.velocity.X - num8;
				}
				if (npc.velocity.Y < vector5.Y)
				{
					npc.velocity.Y = npc.velocity.Y + num8;
					if (npc.velocity.Y < 0f && vector5.Y > 0f)
						npc.velocity.Y = npc.velocity.Y + num8;
				}
				else if (npc.velocity.Y > vector5.Y)
				{
					npc.velocity.Y = npc.velocity.Y - num8;
					if (npc.velocity.Y > 0f && vector5.Y < 0f)
						npc.velocity.Y = npc.velocity.Y - num8;
				}

				if (npc.ai[2] == 0f)
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				if (npc.ai[2] % (float)num7 == 0f)
				{
					Main.PlaySound(4, (int)npc.Center.X, (int)npc.Center.Y, 19, 1f, 0f);
					if (Main.netMode != 1)
					{
						Vector2 vector6 = Vector2.Normalize(player.Center - vector) * (float)(npc.width + 20) / 2f + vector;
						NPC.NewNPC((int)vector6.X, (int)vector6.Y + 45, 371, 0, 0f, 0f, 0f, 0f, 255);
					}
				}

				int num26 = Math.Sign(player.Center.X - vector.X);
				if (num26 != 0)
				{
					npc.direction = num26;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += 3.14159274f;

					npc.spriteDirection = -npc.direction;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num6)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 3f)
			{
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				if (npc.ai[2] == (float)(num9 - 30))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 9, 1f, 0f);

				if (Main.netMode != 1 && npc.ai[2] == (float)(num9 - 30))
				{
					Vector2 vector7 = npc.rotation.ToRotationVector2() * (Vector2.UnitX * (float)npc.direction) * (float)(npc.width + 20) / 2f + vector;
					Projectile.NewProjectile(vector7.X, vector7.Y, (float)(npc.direction * 2), 8f, 385, 0, 0f, Main.myPlayer, 0f, 0f);
					Projectile.NewProjectile(vector7.X, vector7.Y, (float)(-(float)npc.direction * 2), 8f, 385, 0, 0f, Main.myPlayer, 0f, 0f);
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num9)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 4f)
			{
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				if (npc.ai[2] == (float)(num10 - 60))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num10)
				{
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 5f && !player.dead)
			{
				if (npc.ai[1] == 0f)
					npc.ai[1] = (float)(300 * Math.Sign((vector - player.Center).X));

				Vector2 vector8 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * scaleFactor;
				if (npc.velocity.X < vector8.X)
				{
					npc.velocity.X = npc.velocity.X + num3;
					if (npc.velocity.X < 0f && vector8.X > 0f)
						npc.velocity.X = npc.velocity.X + num3;
				}
				else if (npc.velocity.X > vector8.X)
				{
					npc.velocity.X = npc.velocity.X - num3;
					if (npc.velocity.X > 0f && vector8.X < 0f)
						npc.velocity.X = npc.velocity.X - num3;
				}
				if (npc.velocity.Y < vector8.Y)
				{
					npc.velocity.Y = npc.velocity.Y + num3;
					if (npc.velocity.Y < 0f && vector8.Y > 0f)
						npc.velocity.Y = npc.velocity.Y + num3;
				}
				else if (npc.velocity.Y > vector8.Y)
				{
					npc.velocity.Y = npc.velocity.Y - num3;
					if (npc.velocity.Y > 0f && vector8.Y < 0f)
						npc.velocity.Y = npc.velocity.Y - num3;
				}

				int num27 = Math.Sign(player.Center.X - vector.X);
				if (num27 != 0)
				{
					if (npc.ai[2] == 0f && num27 != npc.direction)
						npc.rotation += 3.14159274f;

					npc.direction = num27;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += 3.14159274f;

					npc.spriteDirection = -npc.direction;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num2)
				{
					int num28 = 0;
					switch ((int)npc.ai[3])
					{
						case 0:
						case 1:
						case 2:
						case 3:
						case 4:
						case 5:
							num28 = 1;
							break;
						case 6:
							npc.ai[3] = 1f;
							num28 = 2;
							break;
						case 7:
							npc.ai[3] = 0f;
							num28 = 3;
							break;
					}

					if (flag2)
						num28 = 4;

					if (num28 == 1)
					{
						npc.ai[0] = 6f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.velocity = Vector2.Normalize(player.Center - vector) * num5;
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

						if (num27 != 0)
						{
							npc.direction = num27;

							if (npc.spriteDirection == 1)
								npc.rotation += 3.14159274f;

							npc.spriteDirection = -npc.direction;
						}
					}
					else if (num28 == 2)
					{
						npc.velocity = Vector2.Normalize(player.Center - vector) * scaleFactor4;
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

						if (num27 != 0)
						{
							npc.direction = num27;

							if (npc.spriteDirection == 1)
								npc.rotation += 3.14159274f;

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
					else if (num28 == 4)
					{
						npc.ai[0] = 9f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 6f)
			{
				int num29 = 7;
				for (int k = 0; k < num29; k++)
				{
					Vector2 arg_1A97_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(k - (num29 / 2 - 1)) * 3.1415926535897931 / (double)((float)num29), default(Vector2)) + vector;
					Vector2 vector9 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num30 = Dust.NewDust(arg_1A97_0 + vector9, 0, 0, 172, vector9.X * 2f, vector9.Y * 2f, 100, default(Color), 1.4f);
					Main.dust[num30].noGravity = true;
					Main.dust[num30].noLight = true;
					Main.dust[num30].velocity /= 4f;
					Main.dust[num30].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num4)
				{
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 2f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 7f)
			{
				if (npc.ai[2] == 0f)
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				if (npc.ai[2] % (float)num14 == 0f)
				{
					Main.PlaySound(4, (int)npc.Center.X, (int)npc.Center.Y, 19, 1f, 0f);
					if (Main.netMode != 1)
					{
						Vector2 vector10 = Vector2.Normalize(npc.velocity) * (float)(npc.width + 20) / 2f + vector;
						int num31 = NPC.NewNPC((int)vector10.X, (int)vector10.Y + 45, 371, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[num31].target = npc.target;
						Main.npc[num31].velocity = Vector2.Normalize(npc.velocity).RotatedBy((double)(1.57079637f * (float)npc.direction), default(Vector2)) * scaleFactor3;
						Main.npc[num31].netUpdate = true;
						Main.npc[num31].ai[3] = (float)Main.rand.Next(80, 121) / 100f;
					}
				}

				npc.velocity = npc.velocity.RotatedBy((double)(-(double)num15 * (float)npc.direction), default(Vector2));
				npc.rotation -= num15 * (float)npc.direction;

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num13)
				{
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 8f)
			{
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				if (npc.ai[2] == (float)(num9 - 30))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				if (Main.netMode != 1 && npc.ai[2] == (float)(num9 - 30))
					Projectile.NewProjectile(vector.X, vector.Y, 0f, 0f, 385, 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num9)
				{
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 9f)
			{
				if (npc.ai[2] < (float)(num11 - 90))
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
				else if (npc.alpha < 255)
				{
					npc.alpha += 4;
					if (npc.alpha > 255)
						npc.alpha = 255;
				}

				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				if (npc.ai[2] == (float)(num11 - 60))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num11)
				{
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 10f && !player.dead)
			{
				npc.dontTakeDamage = false;
				npc.chaseable = false;

				if (npc.alpha < 255)
				{
					npc.alpha += 25;
					if (npc.alpha > 255)
						npc.alpha = 255;
				}

				if (npc.ai[1] == 0f)
					npc.ai[1] = (float)(360 * Math.Sign((vector - player.Center).X));

				Vector2 desiredVelocity = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * scaleFactor;
				npc.SimpleFlyMovement(desiredVelocity, num3);

				int num32 = Math.Sign(player.Center.X - vector.X);
				if (num32 != 0)
				{
					if (npc.ai[2] == 0f && num32 != npc.direction)
					{
						npc.rotation += 3.14159274f;

						for (int l = 0; l < npc.oldPos.Length; l++)
							npc.oldPos[l] = Vector2.Zero;
					}

					npc.direction = num32;

					if (npc.spriteDirection != -npc.direction)
						npc.rotation += 3.14159274f;

					npc.spriteDirection = -npc.direction;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num2)
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
						case 4:
						case 8:
							num33 = 2;
							break;
					}
					if (num33 == 1)
					{
						npc.ai[0] = 11f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.velocity = Vector2.Normalize(player.Center - vector) * num5;
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

						if (num32 != 0)
						{
							npc.direction = num32;

							if (npc.spriteDirection == 1)
								npc.rotation += 3.14159274f;

							npc.spriteDirection = -npc.direction;
						}
					}
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
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 11f)
			{
				npc.dontTakeDamage = false;
				npc.chaseable = true;

				npc.alpha -= 25;
				if (npc.alpha < 0)
					npc.alpha = 0;

				int num34 = 7;
				for (int m = 0; m < num34; m++)
				{
					Vector2 arg_2444_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(m - (num34 / 2 - 1)) * 3.1415926535897931 / (double)((float)num34), default(Vector2)) + vector;
					Vector2 vector11 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num35 = Dust.NewDust(arg_2444_0 + vector11, 0, 0, 172, vector11.X * 2f, vector11.Y * 2f, 100, default(Color), 1.4f);
					Main.dust[num35].noGravity = true;
					Main.dust[num35].noLight = true;
					Main.dust[num35].velocity /= 4f;
					Main.dust[num35].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num4)
				{
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 1f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 12f)
			{
				npc.dontTakeDamage = true;
				npc.chaseable = false;

				if (npc.alpha < 255)
				{
					npc.alpha += 17;
					if (npc.alpha > 255)
						npc.alpha = 255;
				}

				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				if (npc.ai[2] == (float)(num12 / 2))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				if (Main.netMode != 1 && npc.ai[2] == (float)(num12 / 2))
				{
					if (npc.ai[1] == 0f)
						npc.ai[1] = (float)(300 * Math.Sign((vector - player.Center).X));

					Vector2 center = player.Center + new Vector2(-npc.ai[1], -200f);
					vector = (npc.Center = center);

					int num36 = Math.Sign(player.Center.X - vector.X);
					if (num36 != 0)
					{
						if (npc.ai[2] == 0f && num36 != npc.direction)
						{
							npc.rotation += 3.14159274f;

							for (int n = 0; n < npc.oldPos.Length; n++)
								npc.oldPos[n] = Vector2.Zero;
						}

						npc.direction = num36;

						if (npc.spriteDirection != -npc.direction)
							npc.rotation += 3.14159274f;

						npc.spriteDirection = -npc.direction;
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num12)
				{
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 1f;

					if (npc.ai[3] >= 9f)
						npc.ai[3] = 0f;

					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 13f)
			{
				if (npc.ai[2] == 0f)
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				npc.velocity = npc.velocity.RotatedBy((double)(-(double)num15 * (float)npc.direction), default(Vector2));
				npc.rotation -= num15 * (float)npc.direction;

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num13)
				{
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 1f;
					npc.netUpdate = true;
				}
			}
			return false;
		}
		#endregion

		#region Buffed Queen Bee AI
		public static bool BuffedQueenBeeAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);

			// Increase bee spawn rate during bee spawning phase based on number of players near the boss
			int playerNearBoss = 0;
			int num;
			for (int num593 = 0; num593 < 255; num593 = num + 1)
			{
				if (Main.player[num593].active && !Main.player[num593].dead && (npc.Center - Main.player[num593].Center).Length() < 1000f)
					playerNearBoss++;

				num = num593;
			}

			bool enrage = false;
			if (!Main.player[npc.target].ZoneJungle || (double)Main.player[npc.target].position.Y < Main.worldSurface * 16.0 || Main.player[npc.target].position.Y > (float)((Main.maxTilesY - 200) * 16))
				enrage = true;

			// Boost defense and damage as health decreases
			int statBoost = (int)(20f * (1f - (float)npc.life / (float)npc.lifeMax));
			npc.defense = npc.defDefense + statBoost;
			npc.damage = npc.defDamage + statBoost;

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			// Despawn
			if (Main.player[npc.target].dead && Main.expertMode)
			{
				if ((double)npc.position.Y < Main.worldSurface * 16.0 + 2000.0)
					npc.velocity.Y = npc.velocity.Y + 0.04f;
				if (npc.position.X < (float)(Main.maxTilesX * 8))
					npc.velocity.X = npc.velocity.X - 0.04f;
				else
					npc.velocity.X = npc.velocity.X + 0.04f;

				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
					return false;
				}
			}

			// Pick a random phase
			else if (npc.ai[0] == -1f)
			{
				if (Main.netMode != 1)
				{
					float num595 = npc.ai[1];
					int phase;
					do
					{
						if (enrage)
						{
							phase = Main.rand.Next(2);
							if (phase == 1)
								phase = 3;
						}
						else
						{
							phase = Main.rand.Next(3);
							if (phase == 1)
								phase = 2;
							else if (phase == 2)
								phase = 3;
						}
					}
					while ((float)phase == num595);
					npc.ai[0] = (float)phase;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
				}
			}

			// Charging phase
			else if (npc.ai[0] == 0f)
			{
				// Number of charges
				int chargeAmt = 2;
				if (npc.life < npc.lifeMax / 3)
					chargeAmt++;

				// Switch to a random phase if chargeAmt has been exceeded
				if (npc.ai[1] > (float)(2 * chargeAmt) && npc.ai[1] % 2f == 0f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
					return false;
				}
				if (npc.ai[1] % 2f == 0f)
				{
					// Get target and charge
					npc.TargetClosest(true);
					if (Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))) < 20f)
					{
						// Set AI variables and speed
						npc.localAI[0] = 1f;
						npc.ai[1] += 1f;
						npc.ai[2] = 0f;
						float speed = 16f;
						if ((double)npc.life < (double)npc.lifeMax * 0.75)
							speed += 2f;
						if ((double)npc.life < (double)npc.lifeMax * 0.5)
							speed += 2f;
						if ((double)npc.life < (double)npc.lifeMax * 0.25)
							speed += 2f;
						if ((double)npc.life < (double)npc.lifeMax * 0.1)
							speed += 2f;

						// Get target location
						Vector2 vector74 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num599 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector74.X;
						float num600 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector74.Y;
						float num601 = (float)Math.Sqrt((double)(num599 * num599 + num600 * num600));
						num601 = speed / num601;
						npc.velocity.X = num599 * num601;
						npc.velocity.Y = num600 * num601;

						// Face the correct direction and play charge sound
						npc.spriteDirection = npc.direction;
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
						return false;
					}

					// Velocity variables
					npc.localAI[0] = 0f;
					float num602 = 12f;
					float num603 = 0.15f;
					if ((double)npc.life < (double)npc.lifeMax * 0.75)
					{
						num602 += 1f;
						num603 += 0.05f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.5)
					{
						num602 += 1f;
						num603 += 0.05f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.25)
					{
						num602 += 2f;
						num603 += 0.05f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.1)
					{
						num602 += 2f;
						num603 += 0.1f;
					}

					// Velocity calculations
					if (npc.position.Y + (float)(npc.height / 2) < Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))
						npc.velocity.Y = npc.velocity.Y + num603;
					else
						npc.velocity.Y = npc.velocity.Y - num603;

					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -num602;
					if (npc.velocity.Y > 12f)
						npc.velocity.Y = num602;

					if (Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) > 600f)
						npc.velocity.X = npc.velocity.X + 0.15f * (float)npc.direction;
					else if (Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) < 300f)
						npc.velocity.X = npc.velocity.X - 0.15f * (float)npc.direction;
					else
						npc.velocity.X = npc.velocity.X * 0.8f;

					// Limit velocity
					if (npc.velocity.X < -16f)
						npc.velocity.X = -16f;
					if (npc.velocity.X > 16f)
						npc.velocity.X = 16f;

					// Face the correct direction
					npc.spriteDirection = npc.direction;
				}
				else
				{
					// Face the correct direction
					if (npc.velocity.X < 0f)
						npc.direction = -1;
					else
						npc.direction = 1;
					npc.spriteDirection = npc.direction;

					// Charging distance from player
					int num604 = 500;
					if (enrage)
						num604 = 250;
					else if ((double)npc.life < (double)npc.lifeMax * 0.33)
						num604 = 300;
					else if ((double)npc.life < (double)npc.lifeMax * 0.66)
						num604 = 450;

					// Get which side of the player the boss is on
					int num605 = 1;
					if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))
						num605 = -1;

					// If boss is in correct position, slow down, if not, reset
					if (npc.direction == num605 && Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) > (float)num604)
						npc.ai[2] = 1f;
					if (npc.ai[2] != 1f)
					{
						npc.localAI[0] = 1f;
						return false;
					}

					// Get target, face correct direction, and slow down
					npc.TargetClosest(true);
					npc.spriteDirection = npc.direction;
					npc.localAI[0] = 0f;
					npc.velocity *= 0.9f;
					float num606 = 0.1f;
					if (npc.life < npc.lifeMax / 2)
					{
						npc.velocity *= 0.9f;
						num606 += 0.05f;
					}
					if (npc.life < npc.lifeMax / 3)
					{
						npc.velocity *= 0.9f;
						num606 += 0.05f;
					}
					if (npc.life < npc.lifeMax / 5)
					{
						npc.velocity *= 0.9f;
						num606 += 0.05f;
					}

					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num606)
					{
						npc.ai[2] = 0f;
						npc.ai[1] += 1f;
					}
				}
			}

			// Fly above target before bee spawning phase
			else if (npc.ai[0] == 2f)
			{
				// Get target and face the correct direction
				npc.TargetClosest(true);
				npc.spriteDirection = npc.direction;

				// Get target location
				float num608 = 0.1f;
				Vector2 vector75 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num609 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector75.X;
				float num610 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 200f - vector75.Y;
				float num611 = (float)Math.Sqrt((double)(num609 * num609 + num610 * num610));

				// Go to bee spawn phase
				if (num611 < 200f)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.netUpdate = true;
					return false;
				}

				// Velocity calculations
				if (npc.velocity.X < num609)
				{
					npc.velocity.X = npc.velocity.X + num608;
					if (npc.velocity.X < 0f && num609 > 0f)
						npc.velocity.X = npc.velocity.X + num608;
				}
				else if (npc.velocity.X > num609)
				{
					npc.velocity.X = npc.velocity.X - num608;
					if (npc.velocity.X > 0f && num609 < 0f)
						npc.velocity.X = npc.velocity.X - num608;
				}
				if (npc.velocity.Y < num610)
				{
					npc.velocity.Y = npc.velocity.Y + num608;
					if (npc.velocity.Y < 0f && num610 > 0f)
						npc.velocity.Y = npc.velocity.Y + num608;
				}
				else if (npc.velocity.Y > num610)
				{
					npc.velocity.Y = npc.velocity.Y - num608;
					if (npc.velocity.Y > 0f && num610 < 0f)
						npc.velocity.Y = npc.velocity.Y - num608;
				}
			}

			// Bee spawn phase
			else if (npc.ai[0] == 1f)
			{
				// Reset localAI and get target
				npc.localAI[0] = 0f;
				npc.TargetClosest(true);

				// Get target location and spawn bees from ass
				Vector2 vector76 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				Vector2 vector77 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num612 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector77.X;
				float num613 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector77.Y;
				float num614 = (float)Math.Sqrt((double)(num612 * num612 + num613 * num613));

				// Bee spawn timer
				npc.ai[1] += 1f;
				npc.ai[1] += (float)(playerNearBoss / 2);
				if ((double)npc.life < (double)npc.lifeMax * 0.75)
					npc.ai[1] += 0.25f;
				if ((double)npc.life < (double)npc.lifeMax * 0.5)
					npc.ai[1] += 0.25f;
				if ((double)npc.life < (double)npc.lifeMax * 0.25)
					npc.ai[1] += 0.25f;
				if ((double)npc.life < (double)npc.lifeMax * 0.1)
					npc.ai[1] += 0.25f;

				bool spawnBee = false;
				if (npc.ai[1] > 15f)
				{
					npc.ai[1] = 0f;
					npc.ai[2] += 1f;
					spawnBee = true;
				}

				// Spawn bees or hornets
				if (Collision.CanHit(vector76, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && spawnBee)
				{
					Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);
					if (Main.netMode != 1)
					{
						int hornetAmt = CalamityMod.hornetList.Count;
						int spawnType = Main.rand.Next(210, 212);
						int random = CalamityWorld.death ? 5 : 6;
						if (Main.rand.Next(random) == 0)
							spawnType = CalamityMod.hornetList[Main.rand.Next(hornetAmt)];

						int spawn = NPC.NewNPC((int)vector76.X, (int)vector76.Y, spawnType, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[spawn].velocity.X = (float)Main.rand.Next(-200, 201) * 0.002f;
						Main.npc[spawn].velocity.Y = (float)Main.rand.Next(-200, 201) * 0.002f;

						if (!CalamityMod.hornetList.Contains(spawnType))
							Main.npc[spawn].localAI[0] = 60f;

						Main.npc[spawn].netUpdate = true;
					}
				}

				// Velocity calculations if target is too far away
				if (num614 > 400f || !Collision.CanHit(new Vector2(vector76.X, vector76.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					float num617 = 14f;
					float num618 = 0.1f;
					vector77 = vector76;
					num612 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector77.X;
					num613 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector77.Y;
					num614 = (float)Math.Sqrt((double)(num612 * num612 + num613 * num613));
					num614 = num617 / num614;

					if (npc.velocity.X < num612)
					{
						npc.velocity.X = npc.velocity.X + num618;
						if (npc.velocity.X < 0f && num612 > 0f)
							npc.velocity.X = npc.velocity.X + num618;
					}
					else if (npc.velocity.X > num612)
					{
						npc.velocity.X = npc.velocity.X - num618;
						if (npc.velocity.X > 0f && num612 < 0f)
							npc.velocity.X = npc.velocity.X - num618;
					}
					if (npc.velocity.Y < num613)
					{
						npc.velocity.Y = npc.velocity.Y + num618;
						if (npc.velocity.Y < 0f && num613 > 0f)
							npc.velocity.Y = npc.velocity.Y + num618;
					}
					else if (npc.velocity.Y > num613)
					{
						npc.velocity.Y = npc.velocity.Y - num618;
						if (npc.velocity.Y > 0f && num613 < 0f)
							npc.velocity.Y = npc.velocity.Y - num618;
					}
				}
				else
					npc.velocity *= 0.9f;

				// Face the correct direction
				npc.spriteDirection = npc.direction;

				// Go to a random phase
				if (npc.ai[2] > 3f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 1f;
					npc.netUpdate = true;
				}
			}

			// Stinger phase
			else if (npc.ai[0] == 3f)
			{
				// Get target location and shoot from ass
				Vector2 vector78 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				Vector2 vector79 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num621 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector79.X;
				float num622 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 300f - vector79.Y;
				float num623 = (float)Math.Sqrt((double)(num621 * num621 + num622 * num622));

				// Stinger fire counter
				npc.ai[1] += 1f;
				bool shoot = false;
				if ((double)npc.life < (double)npc.lifeMax * 0.33 || enrage)
				{
					if (npc.ai[1] % 15f == 14f)
						shoot = true;
				}
				else if (npc.life < (double)npc.lifeMax * 0.66)
				{
					if (npc.ai[1] % 25f == 24f)
						shoot = true;
				}
				else if (npc.ai[1] % 30f == 29f)
					shoot = true;

				// Fire stingers
				if (shoot && npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y && Collision.CanHit(vector78, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					Main.PlaySound(SoundID.Item17, npc.position);
					if (Main.netMode != 1)
					{
						float num624 = 12f;
						if ((double)npc.life < (double)npc.lifeMax * 0.33)
							num624 += 3f;
						if (CalamityWorld.death)
							num624 += 1f;

						float num625 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector78.X;
						float num626 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector78.Y;
						float num627 = (float)Math.Sqrt((double)(num625 * num625 + num626 * num626));
						num627 = num624 / num627;
						num625 *= num627;
						num626 *= num627;

						int damage = Main.player[npc.target].buffImmune[BuffID.Poisoned] ? 18 : 13;
						int type = ProjectileID.Stinger;
						int projectile = Projectile.NewProjectile(vector78.X, vector78.Y, num625, num626, type, damage, 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[projectile].timeLeft = 300;
					}
				}

				// Movement calculations
				float num619 = 6f;
				float num620 = 0.075f;
				if (!Collision.CanHit(new Vector2(vector78.X, vector78.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					num619 = 14f;
					num620 = 0.1f;
					vector79 = vector78;
					num621 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector79.X;
					num622 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector79.Y;
					num623 = (float)Math.Sqrt((double)(num621 * num621 + num622 * num622));
					num623 = num619 / num623;

					if (npc.velocity.X < num621)
					{
						npc.velocity.X = npc.velocity.X + num620;
						if (npc.velocity.X < 0f && num621 > 0f)
							npc.velocity.X = npc.velocity.X + num620;
					}
					else if (npc.velocity.X > num621)
					{
						npc.velocity.X = npc.velocity.X - num620;
						if (npc.velocity.X > 0f && num621 < 0f)
							npc.velocity.X = npc.velocity.X - num620;
					}
					if (npc.velocity.Y < num622)
					{
						npc.velocity.Y = npc.velocity.Y + num620;
						if (npc.velocity.Y < 0f && num622 > 0f)
							npc.velocity.Y = npc.velocity.Y + num620;
					}
					else if (npc.velocity.Y > num622)
					{
						npc.velocity.Y = npc.velocity.Y - num620;
						if (npc.velocity.Y > 0f && num622 < 0f)
							npc.velocity.Y = npc.velocity.Y - num620;
					}
				}
				else if (num623 > 100f)
				{
					npc.TargetClosest(true);
					npc.spriteDirection = npc.direction;
					num623 = num619 / num623;

					if (npc.velocity.X < num621)
					{
						npc.velocity.X = npc.velocity.X + num620;
						if (npc.velocity.X < 0f && num621 > 0f)
							npc.velocity.X = npc.velocity.X + num620 * 2f;
					}
					else if (npc.velocity.X > num621)
					{
						npc.velocity.X = npc.velocity.X - num620;
						if (npc.velocity.X > 0f && num621 < 0f)
							npc.velocity.X = npc.velocity.X - num620 * 2f;
					}
					if (npc.velocity.Y < num622)
					{
						npc.velocity.Y = npc.velocity.Y + num620;
						if (npc.velocity.Y < 0f && num622 > 0f)
							npc.velocity.Y = npc.velocity.Y + num620 * 2f;
					}
					else if (npc.velocity.Y > num622)
					{
						npc.velocity.Y = npc.velocity.Y - num620;
						if (npc.velocity.Y > 0f && num622 < 0f)
							npc.velocity.Y = npc.velocity.Y - num620 * 2f;
					}
				}

				// Go to a random phase
				if (npc.ai[1] > 800f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 3f;
					npc.netUpdate = true;
				}
			}
			return false;
		}
		#endregion

		#region Buffed Destroyer AI
		public static bool BuffedDestroyerAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);
			bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;

			// Enrage variable if player is flying upside down
			bool targetFloatingUp = Main.player[npc.target].gravDir == -1f;

			// Percent life remaining
			float lifeRatio = (float)npc.life / (float)npc.lifeMax;

			// Phases based on life percentage
			bool phase2 = lifeRatio <= 0.66f;
			bool phase3 = lifeRatio <= 0.33f;

			// Set worm variable for worms
			if (npc.ai[3] > 0f)
				npc.realLife = (int)npc.ai[3];

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
				npc.TargetClosest(true);

			// Dust on spawn and alpha effects
			if (npc.type >= NPCID.TheDestroyer && npc.type <= NPCID.TheDestroyerTail)
			{
				npc.velocity.Length();

				if (npc.type == NPCID.TheDestroyer || (npc.type != NPCID.TheDestroyer && Main.npc[(int)npc.ai[1]].alpha < 128))
				{
					if (npc.alpha != 0)
					{
						for (int i = 0; i < 2; i++)
						{
							int num = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default(Color), 2f);
							Main.dust[num].noGravity = true;
							Main.dust[num].noLight = true;
						}
					}
					npc.alpha -= 42;
					if (npc.alpha < 0)
						npc.alpha = 0;
				}
			}

			// Check if other segments are still alive, if not, die
			if (npc.type > NPCID.TheDestroyer)
			{
				bool flag = false;
				if (npc.ai[1] <= 0f)
					flag = true;
				else if (Main.npc[(int)npc.ai[1]].life <= 0)
					flag = true;

				if (flag)
				{
					npc.life = 0;
					npc.HitEffect(0, 10.0);
					npc.checkDead();
				}
			}

			if (npc.type == NPCID.TheDestroyerBody)
			{
				// Gain more defense as health lowers with a max of 30, lose defense if probe has been launched
				if (npc.ai[2] == 0f)
				{
					int defenseUp = (int)(30f * (1f - lifeRatio));
					npc.defense = npc.defDefense + defenseUp;
				}
				else
					npc.defense = npc.defDefense - 10;

				// Enrage, fire more homing lasers
				if (targetFloatingUp)
				{
					if (calamityGlobalNPC.newAI[2] < 480f)
						calamityGlobalNPC.newAI[2] += 1f;
				}
				else
				{
					if (calamityGlobalNPC.newAI[2] > 0f)
						calamityGlobalNPC.newAI[2] -= 1f;
				}
			}

			if (Main.netMode != 1)
			{
				// Spawn segments from head
				if (npc.ai[0] == 0f && npc.type == NPCID.TheDestroyer)
				{
					npc.ai[3] = (float)npc.whoAmI;
					npc.realLife = npc.whoAmI;
					int num2 = npc.whoAmI;

					int num3 = 80;
					for (int j = 0; j <= num3; j++)
					{
						int num4 = NPCID.TheDestroyerBody;
						if (j == num3)
							num4 = NPCID.TheDestroyerTail;

						int num5 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), num4, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
						Main.npc[num5].ai[3] = (float)npc.whoAmI;
						Main.npc[num5].realLife = npc.whoAmI;
						Main.npc[num5].ai[1] = (float)num2;
						Main.npc[num2].ai[0] = (float)num5;
						NetMessage.SendData(23, -1, -1, null, num5, 0f, 0f, 0f, 0, 0, 0);
						num2 = num5;
					}
				}

				// Fire lasers
				if (npc.type == NPCID.TheDestroyerBody)
				{
					// Laser rate of fire
					int shootTime = 1 + (int)Math.Ceiling((((enraged || configBossRushBoost) ? 7D : 3D) * (double)lifeRatio));
					if (CalamityWorld.bossRushActive)
						shootTime += 1;

					calamityGlobalNPC.newAI[0] += (float)Main.rand.Next(shootTime);
					if (calamityGlobalNPC.newAI[0] >= (float)Main.rand.Next(1400, 26000))
					{
						calamityGlobalNPC.newAI[0] = 0f;
						npc.TargetClosest(true);
						if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
						{
							// Base laser speed on player movement speed
							float laserSpeedBoost = Math.Abs(Main.player[npc.target].velocity.X);
							if (Math.Abs(Main.player[npc.target].velocity.X) < Math.Abs(Main.player[npc.target].velocity.Y))
								laserSpeedBoost = Math.Abs(Main.player[npc.target].velocity.Y);

							// Put limits on laser speed
							float projectileSpeed = 2f + laserSpeedBoost;
							if (projectileSpeed < 7f)
								projectileSpeed = 7f;
							if (projectileSpeed > 10f)
								projectileSpeed = 10f;

							// Increase laser speed as health drops
							if (phase2 || CalamityWorld.bossRushActive)
								projectileSpeed += CalamityWorld.death ? 0.33f : 0.25f;
							if (phase3 || CalamityWorld.bossRushActive)
								projectileSpeed += CalamityWorld.death ? 0.33f : 0.25f;

							// Set projectile damage and type, set projectile to saucer scrap if probe has been launched
							int damage = 28;
							int projectileType = ProjectileID.DeathLaser;
							if (npc.ai[2] == 0f || Main.rand.Next(2) == 0)
							{
								if (phase3 || calamityGlobalNPC.newAI[2] > 0f)
								{
									damage += 3;
									projectileType = mod.ProjectileType("DestroyerHomingLaser");
								}
								else if (phase2)
								{
									damage -= 3;
									projectileType = ProjectileID.FrostBeam;
								}
							}
							else
							{
								damage -= 3;
								projectileType = ProjectileID.SaucerScrap;
							}

							// Get target vector
							Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
							float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X;
							float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y;
							float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
							num8 = projectileSpeed / num8;
							num6 *= num8;
							num7 *= num8;
							vector.X += num6 * 5f;
							vector.Y += num7 * 5f;

							// Shoot projectile and set timeLeft if not a homing laser/metal scrap so lasers don't last for too long
							int proj = Projectile.NewProjectile(vector.X, vector.Y, num6, num7, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
							if (projectileType != mod.ProjectileType("DestroyerHomingLaser") && projectileType != ProjectileID.SaucerScrap)
								Main.projectile[proj].timeLeft = 300;

							npc.netUpdate = true;
						}
					}
				}
			}

			int num12 = (int)(npc.position.X / 16f) - 1;
			int num13 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
			int num14 = (int)(npc.position.Y / 16f) - 1;
			int num15 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;

			if (num12 < 0)
				num12 = 0;
			if (num13 > Main.maxTilesX)
				num13 = Main.maxTilesX;
			if (num14 < 0)
				num14 = 0;
			if (num15 > Main.maxTilesY)
				num15 = Main.maxTilesY;

			// Fly or not
			bool flag2 = false;
			if (!flag2)
			{
				for (int k = num12; k < num13; k++)
				{
					for (int l = num14; l < num15; l++)
					{
						if (Main.tile[k, l] != null && ((Main.tile[k, l].nactive() && (Main.tileSolid[(int)Main.tile[k, l].type] || (Main.tileSolidTop[(int)Main.tile[k, l].type] && Main.tile[k, l].frameY == 0))) || Main.tile[k, l].liquid > 64))
						{
							Vector2 vector2;
							vector2.X = (float)(k * 16);
							vector2.Y = (float)(l * 16);
							if (npc.position.X + (float)npc.width > vector2.X && npc.position.X < vector2.X + 16f && npc.position.Y + (float)npc.height > vector2.Y && npc.position.Y < vector2.Y + 16f)
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
				if (npc.type != NPCID.TheDestroyerBody || npc.ai[2] != 1f)
					Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.3f, 0.1f, 0.05f);

				npc.localAI[1] = 1f;

				if (npc.type == NPCID.TheDestroyer)
				{
					Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
					int num16 = targetFloatingUp ? 50 : 1000;
					int height = 2000 - (int)((targetFloatingUp ? 850f : 700f) * (1f - lifeRatio));
					bool flag3 = true;

					if (npc.position.Y > Main.player[npc.target].position.Y)
					{
						for (int m = 0; m < 255; m++)
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
			float fallSpeed = 16f;
			if (Main.dayTime || Main.player[npc.target].dead)
			{
				flag2 = false;
				npc.velocity.Y = npc.velocity.Y + 1f;

				if ((double)npc.position.Y > Main.worldSurface * 16.0)
				{
					npc.velocity.Y = npc.velocity.Y + 1f;
					fallSpeed = 32f;
				}

				if ((double)npc.position.Y > Main.rockLayer * 16.0)
				{
					for (int n = 0; n < 200; n++)
					{
						if (Main.npc[n].aiStyle == npc.aiStyle)
							Main.npc[n].active = false;
					}
				}
			}
			fallSpeed += 4f * (1f - lifeRatio);

			// Speed and movement
			float speed = 0.1f + ((targetFloatingUp ? 0.2f : 0.1f) * (1f - lifeRatio));
			float turnSpeed = 0.15f + ((targetFloatingUp ? 0.3f : 0.15f) * (1f - lifeRatio));
			Vector2 vector3 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num20 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
			float num21 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
			num20 = (float)((int)(num20 / 16f) * 16);
			num21 = (float)((int)(num21 / 16f) * 16);
			vector3.X = (float)((int)(vector3.X / 16f) * 16);
			vector3.Y = (float)((int)(vector3.Y / 16f) * 16);
			num20 -= vector3.X;
			num21 -= vector3.Y;
			float num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
			if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
			{
				try
				{
					vector3 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num20 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector3.X;
					num21 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector3.Y;
				}
				catch
				{
				}
				npc.rotation = (float)Math.Atan2((double)num21, (double)num20) + 1.57f;
				num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
				int num23 = (int)(44f * npc.scale);
				num22 = (num22 - (float)num23) / num22;
				num20 *= num22;
				num21 *= num22;
				npc.velocity = Vector2.Zero;
				npc.position.X = npc.position.X + num20;
				npc.position.Y = npc.position.Y + num21;
				return false;
			}

			if (!flag2)
			{
				npc.TargetClosest(true);
				npc.velocity.Y = npc.velocity.Y + 0.15f;

				if (npc.velocity.Y > fallSpeed)
					npc.velocity.Y = fallSpeed;

				if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)fallSpeed * 0.4)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X - speed * 1.1f;
					else
						npc.velocity.X = npc.velocity.X + speed * 1.1f;
				}
				else if (npc.velocity.Y == fallSpeed)
				{
					if (npc.velocity.X < num20)
						npc.velocity.X = npc.velocity.X + speed;
					else if (npc.velocity.X > num20)
						npc.velocity.X = npc.velocity.X - speed;
				}
				else if (npc.velocity.Y > 4f)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X + speed * 0.9f;
					else
						npc.velocity.X = npc.velocity.X - speed * 0.9f;
				}
			}
			else
			{
				if (npc.soundDelay == 0)
				{
					float num24 = num22 / 40f;
					if (num24 < 10f)
						num24 = 10f;
					if (num24 > 20f)
						num24 = 20f;

					npc.soundDelay = (int)num24;
					Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);
				}

				num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
				float num25 = Math.Abs(num20);
				float num26 = Math.Abs(num21);
				float num27 = fallSpeed / num22;
				num20 *= num27;
				num21 *= num27;

				if (((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f)) && ((npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f)))
				{
					if (npc.velocity.X < num20)
						npc.velocity.X = npc.velocity.X + turnSpeed;
					else if (npc.velocity.X > num20)
						npc.velocity.X = npc.velocity.X - turnSpeed;
					if (npc.velocity.Y < num21)
						npc.velocity.Y = npc.velocity.Y + turnSpeed;
					else if (npc.velocity.Y > num21)
						npc.velocity.Y = npc.velocity.Y - turnSpeed;
				}

				if ((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f) || (npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f))
				{
					if (npc.velocity.X < num20)
						npc.velocity.X = npc.velocity.X + speed;
					else if (npc.velocity.X > num20)
						npc.velocity.X = npc.velocity.X - speed;
					if (npc.velocity.Y < num21)
						npc.velocity.Y = npc.velocity.Y + speed;
					else if (npc.velocity.Y > num21)
						npc.velocity.Y = npc.velocity.Y - speed;

					if ((double)Math.Abs(num21) < (double)fallSpeed * 0.2 && ((npc.velocity.X > 0f && num20 < 0f) || (npc.velocity.X < 0f && num20 > 0f)))
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y = npc.velocity.Y + speed * 2f;
						else
							npc.velocity.Y = npc.velocity.Y - speed * 2f;
					}
					if ((double)Math.Abs(num20) < (double)fallSpeed * 0.2 && ((npc.velocity.Y > 0f && num21 < 0f) || (npc.velocity.Y < 0f && num21 > 0f)))
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X = npc.velocity.X + speed * 2f;
						else
							npc.velocity.X = npc.velocity.X - speed * 2f;
					}
				}
				else if (num25 > num26)
				{
					if (npc.velocity.X < num20)
						npc.velocity.X = npc.velocity.X + speed * 1.1f;
					else if (npc.velocity.X > num20)
						npc.velocity.X = npc.velocity.X - speed * 1.1f;

					if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)fallSpeed * 0.5)
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y = npc.velocity.Y + speed;
						else
							npc.velocity.Y = npc.velocity.Y - speed;
					}
				}
				else
				{
					if (npc.velocity.Y < num21)
						npc.velocity.Y = npc.velocity.Y + speed * 1.1f;
					else if (npc.velocity.Y > num21)
						npc.velocity.Y = npc.velocity.Y - speed * 1.1f;

					if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)fallSpeed * 0.5)
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X = npc.velocity.X + speed;
						else
							npc.velocity.X = npc.velocity.X - speed;
					}
				}
			}

			npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;

			if (npc.type == NPCID.TheDestroyer)
			{
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
			}
			return false;
		}
		#endregion

		#region Buffed Twins AI
		public static bool BuffedRetinazerAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);
			bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;

			// Enrage variable if player is floating upside down
			bool targetFloatingUp = Main.player[npc.target].gravDir == -1f;

			// Easier to send info to Spazmatism
			CalamityGlobalNPC.laserEye = npc.whoAmI;

			// Check for Spazmatism
			bool spazAlive = false;
			if (CalamityGlobalNPC.fireEye != -1)
				spazAlive = Main.npc[CalamityGlobalNPC.fireEye].active;

			bool enrage = (double)npc.life < (double)npc.lifeMax * 0.25;

			// I'm not commenting this entire fucking thing, already did spaz, I'm not doing ret
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			float num376 = npc.position.X + (float)(npc.width / 2) - Main.player[npc.target].position.X - (float)(Main.player[npc.target].width / 2);
			float num377 = npc.position.Y + (float)npc.height - 59f - Main.player[npc.target].position.Y - (float)(Main.player[npc.target].height / 2);

			float num378 = (float)Math.Atan2((double)num377, (double)num376) + 1.57f;
			if (num378 < 0f)
				num378 += 6.283f;
			else if ((double)num378 > 6.283)
				num378 -= 6.283f;

			float num379 = 0.1f;
			if (npc.rotation < num378)
			{
				if ((double)(num378 - npc.rotation) > 3.1415)
					npc.rotation -= num379;
				else
					npc.rotation += num379;
			}
			else if (npc.rotation > num378)
			{
				if ((double)(npc.rotation - num378) > 3.1415)
					npc.rotation += num379;
				else
					npc.rotation -= num379;
			}

			if (npc.rotation > num378 - num379 && npc.rotation < num378 + num379)
				npc.rotation = num378;

			if (npc.rotation < 0f)
				npc.rotation += 6.283f;
			else if ((double)npc.rotation > 6.283)
				npc.rotation -= 6.283f;

			if (npc.rotation > num378 - num379 && npc.rotation < num378 + num379)
				npc.rotation = num378;

			if (Main.rand.Next(5) == 0)
			{
				int num380 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default(Color), 1f);
				Dust var_9_131D1_cp_0_cp_0 = Main.dust[num380];
				var_9_131D1_cp_0_cp_0.velocity.X = var_9_131D1_cp_0_cp_0.velocity.X * 0.5f;
				Dust var_9_131F5_cp_0_cp_0 = Main.dust[num380];
				var_9_131F5_cp_0_cp_0.velocity.Y = var_9_131F5_cp_0_cp_0.velocity.Y * 0.1f;
			}

			if (Main.netMode != 1 && !Main.dayTime && !Main.player[npc.target].dead && npc.timeLeft < 10)
			{
				int num;
				for (int num381 = 0; num381 < 200; num381 = num + 1)
				{
					if (num381 != npc.whoAmI && Main.npc[num381].active && (Main.npc[num381].type == NPCID.Retinazer || Main.npc[num381].type == NPCID.Spazmatism) && Main.npc[num381].timeLeft - 1 > npc.timeLeft)
						npc.timeLeft = Main.npc[num381].timeLeft - 1;

					num = num381;
				}
			}

			if (Main.dayTime | Main.player[npc.target].dead)
			{
				npc.velocity.Y = npc.velocity.Y - 0.04f;
				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
					return false;
				}
			}

			else if (npc.ai[0] == 0f)
			{
				if (npc.ai[1] == 0f)
				{
					float num382 = 8.25f;
					float num383 = 0.115f;

					int num384 = 1;
					if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
						num384 = -1;

					Vector2 vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num385 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + (float)(num384 * 300) - vector40.X;
					float num386 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 300f - vector40.Y;
					float num387 = (float)Math.Sqrt((double)(num385 * num385 + num386 * num386));
					float num388 = num387;

					num387 = num382 / num387;
					num385 *= num387;
					num386 *= num387;

					if (npc.velocity.X < num385)
					{
						npc.velocity.X = npc.velocity.X + num383;
						if (npc.velocity.X < 0f && num385 > 0f)
							npc.velocity.X = npc.velocity.X + num383;
					}
					else if (npc.velocity.X > num385)
					{
						npc.velocity.X = npc.velocity.X - num383;
						if (npc.velocity.X > 0f && num385 < 0f)
							npc.velocity.X = npc.velocity.X - num383;
					}
					if (npc.velocity.Y < num386)
					{
						npc.velocity.Y = npc.velocity.Y + num383;
						if (npc.velocity.Y < 0f && num386 > 0f)
							npc.velocity.Y = npc.velocity.Y + num383;
					}
					else if (npc.velocity.Y > num386)
					{
						npc.velocity.Y = npc.velocity.Y - num383;
						if (npc.velocity.Y > 0f && num386 < 0f)
							npc.velocity.Y = npc.velocity.Y - num383;
					}

					npc.ai[2] += 1f;
					if (npc.ai[2] >= 450f)
					{
						npc.ai[1] = 1f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.target = 255;
						npc.netUpdate = true;
					}

					else if (npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y && num388 < 400f)
					{
						if (!Main.player[npc.target].dead)
							npc.ai[3] += 1f;

						if (npc.ai[3] >= 30f)
						{
							npc.ai[3] = 0f;
							vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							num385 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector40.X;
							num386 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector40.Y;

							if (Main.netMode != 1)
							{
								float num389 = 10.5f;
								int num390 = 19;
								int num391 = ProjectileID.EyeLaser;

								num387 = (float)Math.Sqrt((double)(num385 * num385 + num386 * num386));
								num387 = num389 / num387;
								num385 *= num387;
								num386 *= num387;
								vector40.X += num385 * 15f;
								vector40.Y += num386 * 15f;

								Projectile.NewProjectile(vector40.X, vector40.Y, num385, num386, num391, num390, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}

				else if (npc.ai[1] == 1f)
				{
					npc.rotation = num378;
					float num393 = 15f;

					Vector2 vector41 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num394 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector41.X;
					float num395 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector41.Y;
					float num396 = (float)Math.Sqrt((double)(num394 * num394 + num395 * num395));
					num396 = num393 / num396;
					npc.velocity.X = num394 * num396;
					npc.velocity.Y = num395 * num396;
					npc.ai[1] = 2f;
				}
				else if (npc.ai[1] == 2f)
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 25f)
					{
						npc.velocity.X = npc.velocity.X * 0.96f;
						npc.velocity.Y = npc.velocity.Y * 0.96f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
						{
							npc.velocity.X = 0f;
						}
						if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
						{
							npc.velocity.Y = 0f;
						}
					}
					else
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

					if (npc.ai[2] >= 70f)
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = num378;
						if (npc.ai[3] >= 3f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
						}
						else
							npc.ai[1] = 1f;
					}
				}

				// Enter phase 2 earlier
				double healthMult = ((CalamityWorld.death || CalamityWorld.bossRushActive) ? 0.8 : 0.7);
				if ((double)npc.life < (double)npc.lifeMax * healthMult || targetFloatingUp)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}

			else if (npc.ai[0] == 1f || npc.ai[0] == 2f)
			{
				if (npc.ai[0] == 1f)
				{
					npc.ai[2] += 0.005f;
					if ((double)npc.ai[2] > 0.5)
						npc.ai[2] = 0.5f;
				}
				else
				{
					npc.ai[2] -= 0.005f;
					if (npc.ai[2] < 0f)
						npc.ai[2] = 0f;
				}

				npc.rotation += npc.ai[2];
				npc.ai[1] += 1f;
				if (npc.ai[1] == 100f)
				{
					npc.ai[0] += 1f;
					npc.ai[1] = 0f;
					if (npc.ai[0] == 3f)
						npc.ai[2] = 0f;
					else
					{
						Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);

						int num;
						for (int num397 = 0; num397 < 2; num397 = num + 1)
						{
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 143, 1f);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
							num = num397;
						}

						for (int num398 = 0; num398 < 20; num398 = num + 1)
						{
							Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);
							num = num398;
						}

						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
					}
				}

				Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);

				npc.velocity.X = npc.velocity.X * 0.98f;
				npc.velocity.Y = npc.velocity.Y * 0.98f;

				if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
					npc.velocity.X = 0f;
				if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
					npc.velocity.Y = 0f;
			}
			else
			{
				// If in phase 2 but Spaz isn't
				bool spazInPhase1 = false;
				if (CalamityGlobalNPC.fireEye != -1)
				{
					if (Main.npc[CalamityGlobalNPC.fireEye].active)
						spazInPhase1 = Main.npc[CalamityGlobalNPC.fireEye].ai[0] == 1f || Main.npc[CalamityGlobalNPC.fireEye].ai[0] == 2f || Main.npc[CalamityGlobalNPC.fireEye].ai[0] == 0f;
				}
				int defenseGain = spazInPhase1 ? 9999 : 20;
				npc.chaseable = !spazInPhase1;

				npc.damage = (int)((double)npc.defDamage * 1.5);
				npc.defense = npc.defDefense + defenseGain;

				npc.HitSound = SoundID.NPCHit4;

				if (npc.ai[1] == 0f)
				{
					float num399 = 9.5f;
					float num400 = 0.175f;

					Vector2 vector42 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num401 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector42.X;
					float num402 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 300f - vector42.Y;
					float num403 = (float)Math.Sqrt((double)(num401 * num401 + num402 * num402));
					num403 = num399 / num403;
					num401 *= num403;
					num402 *= num403;

					if (npc.velocity.X < num401)
					{
						npc.velocity.X = npc.velocity.X + num400;
						if (npc.velocity.X < 0f && num401 > 0f)
							npc.velocity.X = npc.velocity.X + num400;
					}
					else if (npc.velocity.X > num401)
					{
						npc.velocity.X = npc.velocity.X - num400;
						if (npc.velocity.X > 0f && num401 < 0f)
							npc.velocity.X = npc.velocity.X - num400;
					}
					if (npc.velocity.Y < num402)
					{
						npc.velocity.Y = npc.velocity.Y + num400;
						if (npc.velocity.Y < 0f && num402 > 0f)
							npc.velocity.Y = npc.velocity.Y + num400;
					}
					else if (npc.velocity.Y > num402)
					{
						npc.velocity.Y = npc.velocity.Y - num400;
						if (npc.velocity.Y > 0f && num402 < 0f)
							npc.velocity.Y = npc.velocity.Y - num400;
					}

					npc.ai[2] += (spazAlive ? 1f : 1.25f);

					// Enrage
					if (targetFloatingUp)
					{
						npc.ai[2] += 1f;
						npc.localAI[1] += 2f;
					}

					if (npc.ai[2] >= 300f)
					{
						npc.ai[1] = 1f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.TargetClosest(true);
						npc.netUpdate = true;
					}

					vector42 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num401 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector42.X;
					num402 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector42.Y;
					npc.rotation = (float)Math.Atan2((double)num402, (double)num401) - 1.57f;

					if (Main.netMode != 1)
					{
						npc.localAI[1] += 1f;
						if (npc.localAI[1] >= (spazAlive ? 72f : 24f))
						{
							bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
							if (canHit || !spazAlive || enrage)
							{
								npc.localAI[1] = 0f;
								float num404 = 10f;
								int num405 = 23;
								int num406 = ProjectileID.DeathLaser;

								num403 = (float)Math.Sqrt((double)(num401 * num401 + num402 * num402));
								num403 = num404 / num403;
								num401 *= num403;
								num402 *= num403;
								vector42.X += num401 * 15f;
								vector42.Y += num402 * 15f;

								if (canHit)
									Projectile.NewProjectile(vector42.X, vector42.Y, num401, num402, num406, num405, 0f, Main.myPlayer, 0f, 0f);
								else
								{
									int proj = Projectile.NewProjectile(vector42.X, vector42.Y, num401, num402, num406, num405, 0f, Main.myPlayer, 0f, 0f);
									Main.projectile[proj].tileCollide = false;
									Main.projectile[proj].timeLeft = 300;
								}
							}
						}
					}
				}
				else
				{
					if (npc.ai[1] == 1f)
					{
						int num408 = 1;
						if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
							num408 = -1;

						float num409 = 9.5f;
						float num410 = 0.25f;

						Vector2 vector43 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num411 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + (float)(num408 * 340) - vector43.X;
						float num412 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector43.Y;
						float num413 = (float)Math.Sqrt((double)(num411 * num411 + num412 * num412));
						num413 = num409 / num413;
						num411 *= num413;
						num412 *= num413;

						if (npc.velocity.X < num411)
						{
							npc.velocity.X = npc.velocity.X + num410;
							if (npc.velocity.X < 0f && num411 > 0f)
								npc.velocity.X = npc.velocity.X + num410;
						}
						else if (npc.velocity.X > num411)
						{
							npc.velocity.X = npc.velocity.X - num410;
							if (npc.velocity.X > 0f && num411 < 0f)
								npc.velocity.X = npc.velocity.X - num410;
						}
						if (npc.velocity.Y < num412)
						{
							npc.velocity.Y = npc.velocity.Y + num410;
							if (npc.velocity.Y < 0f && num412 > 0f)
								npc.velocity.Y = npc.velocity.Y + num410;
						}
						else if (npc.velocity.Y > num412)
						{
							npc.velocity.Y = npc.velocity.Y - num410;
							if (npc.velocity.Y > 0f && num412 < 0f)
								npc.velocity.Y = npc.velocity.Y - num410;
						}

						vector43 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						num411 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector43.X;
						num412 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector43.Y;
						npc.rotation = (float)Math.Atan2((double)num412, (double)num411) - 1.57f;

						// Enrage
						if (targetFloatingUp)
						{
							npc.ai[2] += 1f;
							npc.localAI[1] += 2f;
						}

						if (Main.netMode != 1)
						{
							npc.localAI[1] += 1f;
							if (npc.localAI[1] > (spazAlive ? 24f : 8f))
							{
								bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
								if (canHit || !spazAlive || enrage)
								{
									npc.localAI[1] = 0f;
									float num414 = 9f;
									int num415 = 17;
									int num416 = ProjectileID.DeathLaser;

									num413 = (float)Math.Sqrt((double)(num411 * num411 + num412 * num412));
									num413 = num414 / num413;
									num411 *= num413;
									num412 *= num413;
									vector43.X += num411 * 15f;
									vector43.Y += num412 * 15f;

									if (canHit)
										Projectile.NewProjectile(vector43.X, vector43.Y, num411, num412, num416, num415, 0f, Main.myPlayer, 0f, 0f);
									else
									{
										int proj = Projectile.NewProjectile(vector43.X, vector43.Y, num411, num412, num416, num415, 0f, Main.myPlayer, 0f, 0f);
										Main.projectile[proj].tileCollide = false;
										Main.projectile[proj].timeLeft = 300;
									}
								}
							}
						}

						npc.ai[2] += (spazAlive ? 1f : 1.5f);
						if (npc.ai[2] >= 180f)
						{
							npc.ai[1] = ((!spazAlive || enrage) ? 4f : 0f);
							npc.ai[2] = 0f;
							npc.ai[3] = 0f;
							npc.TargetClosest(true);
							npc.netUpdate = true;
						}
					}

					// Charge
					else if (npc.ai[1] == 2f)
					{
						// Set rotation and velocity
						npc.rotation = num378;
						float num450 = (CalamityWorld.death ? 24f : 22f);

						if (!spazAlive)
							num450 += 2f;

						// Enrage
						if (targetFloatingUp)
							num450 += 2f;

						Vector2 vector47 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num451 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector47.X;
						float num452 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector47.Y;
						float num453 = (float)Math.Sqrt((double)(num451 * num451 + num452 * num452));
						num453 = num450 / num453;
						npc.velocity.X = num451 * num453;
						npc.velocity.Y = num452 * num453;
						npc.ai[1] = 3f;
					}

					else if (npc.ai[1] == 3f)
					{
						npc.ai[2] += 1f;

						// Enrage
						if (targetFloatingUp)
							npc.ai[2] += 0.25f;

						float chargeTime = (spazAlive ? 110f : 75f);

						// Slow down
						if (npc.ai[2] >= chargeTime)
						{
							npc.velocity *= 0.93f;
							if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
								npc.velocity.X = 0f;
							if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
								npc.velocity.Y = 0f;
						}
						else
						{
							npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

							float fireRate = (spazAlive ? 13f : 9f);
							if (npc.ai[2] % fireRate == 0f)
							{
								Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
								float num349 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
								float num350 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
								float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
								num349 *= num351;
								num350 *= num351;

								if (Main.netMode != 1)
								{
									Main.PlaySound(SoundID.Item33, npc.position);

									float num353 = 6f;
									int num354 = 27;
									int num355 = mod.ProjectileType("ScavengerLaser");

									vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
									num349 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector34.X;
									num350 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector34.Y;
									num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
									num351 = num353 / num351;
									num349 *= num351;
									num350 *= num351;
									vector34.X += num349;
									vector34.Y += num350;

									Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350, num355, num354, 0f, Main.myPlayer, 0f, 0f);
								}
							}
						}

						// Charge once
						if (npc.ai[2] >= chargeTime + 30f)
						{
							npc.ai[2] = 0f;
							npc.target = 255;
							npc.rotation = num378;
							npc.ai[1] = 1f;
						}
					}

					// Get in position for charge
					else if (npc.ai[1] == 4f)
					{
						int num62 = (spazAlive ? 600 : 500);
						float num63 = ((enraged || configBossRushBoost) ? 18f : 12f);
						float num64 = ((enraged || configBossRushBoost) ? 0.45f : 0.3f);

						if (spazAlive)
						{
							num63 *= 0.75f;
							num64 *= 0.75f;
						}

						int num408 = 1;
						if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
							num408 = -1;

						Vector2 vector11 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num65 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + (float)(num62 * num408) - vector11.X;
						float num66 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector11.Y;
						float num67 = (float)Math.Sqrt((double)(num65 * num65 + num66 * num66));

						num67 = num63 / num67;
						num65 *= num67;
						num66 *= num67;

						if (npc.velocity.X < num65)
						{
							npc.velocity.X = npc.velocity.X + num64;
							if (npc.velocity.X < 0f && num65 > 0f)
								npc.velocity.X = npc.velocity.X + num64;
						}
						else if (npc.velocity.X > num65)
						{
							npc.velocity.X = npc.velocity.X - num64;
							if (npc.velocity.X > 0f && num65 < 0f)
								npc.velocity.X = npc.velocity.X - num64;
						}
						if (npc.velocity.Y < num66)
						{
							npc.velocity.Y = npc.velocity.Y + num64;
							if (npc.velocity.Y < 0f && num66 > 0f)
								npc.velocity.Y = npc.velocity.Y + num64;
						}
						else if (npc.velocity.Y > num66)
						{
							npc.velocity.Y = npc.velocity.Y - num64;
							if (npc.velocity.Y > 0f && num66 < 0f)
								npc.velocity.Y = npc.velocity.Y - num64;
						}

						// Take 1.25 or 1 second to get in position, then charge
						npc.ai[2] += 1f;
						if (npc.ai[2] >= (spazAlive ? 75f : 60f))
						{
							npc.TargetClosest(true);
							npc.ai[1] = 2f;
							npc.ai[2] = 0f;
							npc.netUpdate = true;
						}
					}
				}
			}
			return false;
		}

		public static bool BuffedSpazmatismAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);
			bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;

			// Enrage variable if player is floating upside down
			bool targetFloatingUp = Main.player[npc.target].gravDir == -1f;

			// Easier to send info to Retinazer
			CalamityGlobalNPC.fireEye = npc.whoAmI;

			// Check for Retinazer
			bool retAlive = false;
			if (CalamityGlobalNPC.laserEye != -1)
				retAlive = Main.npc[CalamityGlobalNPC.laserEye].active;

			bool enrage = (double)npc.life < (double)npc.lifeMax * 0.25;

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			// Rotation
			float num418 = npc.position.X + (float)(npc.width / 2) - Main.player[npc.target].position.X - (float)(Main.player[npc.target].width / 2);
			float num419 = npc.position.Y + (float)npc.height - 59f - Main.player[npc.target].position.Y - (float)(Main.player[npc.target].height / 2);

			float num420 = (float)Math.Atan2((double)num419, (double)num418) + 1.57f;
			if (num420 < 0f)
				num420 += 6.283f;
			else if ((double)num420 > 6.283)
				num420 -= 6.283f;

			float num421 = 0.15f;
			if (npc.rotation < num420)
			{
				if ((double)(num420 - npc.rotation) > 3.1415)
					npc.rotation -= num421;
				else
					npc.rotation += num421;
			}
			else if (npc.rotation > num420)
			{
				if ((double)(npc.rotation - num420) > 3.1415)
					npc.rotation += num421;
				else
					npc.rotation -= num421;
			}

			if (npc.rotation > num420 - num421 && npc.rotation < num420 + num421)
				npc.rotation = num420;

			if (npc.rotation < 0f)
				npc.rotation += 6.283f;
			else if ((double)npc.rotation > 6.283)
				npc.rotation -= 6.283f;

			if (npc.rotation > num420 - num421 && npc.rotation < num420 + num421)
				npc.rotation = num420;

			// Dust
			if (Main.rand.Next(5) == 0)
			{
				int num422 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default(Color), 1f);
				Dust dust = Main.dust[num422];
				dust.velocity.X = dust.velocity.X * 0.5f;
				dust.velocity.Y = dust.velocity.Y * 0.1f;
			}

			// Despawn Twins at the same time
			if (Main.netMode != 1 && !Main.dayTime && !Main.player[npc.target].dead && npc.timeLeft < 10)
			{
				int num;
				for (int num423 = 0; num423 < 200; num423 = num + 1)
				{
					if (num423 != npc.whoAmI && Main.npc[num423].active && (Main.npc[num423].type == NPCID.Retinazer || Main.npc[num423].type == NPCID.Spazmatism) && Main.npc[num423].timeLeft - 1 > npc.timeLeft)
						npc.timeLeft = Main.npc[num423].timeLeft - 1;

					num = num423;
				}
			}

			// Despawn
			if (Main.dayTime | Main.player[npc.target].dead)
			{
				npc.velocity.Y = npc.velocity.Y - 0.04f;
				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
					return false;
				}
			}

			// Phase 1
			else if (npc.ai[0] == 0f)
			{
				// Cursed fireball phase
				if (npc.ai[1] == 0f)
				{
					// New target
					npc.TargetClosest(true);

					// Velocity
					float num424 = 12f;
					float num425 = 0.4f;

					int num426 = 1;
					if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
						num426 = -1;

					Vector2 vector44 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num427 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + (float)(num426 * 400) - vector44.X;
					float num428 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector44.Y;
					float num429 = (float)Math.Sqrt((double)(num427 * num427 + num428 * num428));

					num429 = num424 / num429;
					num427 *= num429;
					num428 *= num429;

					if (npc.velocity.X < num427)
					{
						npc.velocity.X = npc.velocity.X + num425;
						if (npc.velocity.X < 0f && num427 > 0f)
							npc.velocity.X = npc.velocity.X + num425;
					}
					else if (npc.velocity.X > num427)
					{
						npc.velocity.X = npc.velocity.X - num425;
						if (npc.velocity.X > 0f && num427 < 0f)
							npc.velocity.X = npc.velocity.X - num425;
					}
					if (npc.velocity.Y < num428)
					{
						npc.velocity.Y = npc.velocity.Y + num425;
						if (npc.velocity.Y < 0f && num428 > 0f)
							npc.velocity.Y = npc.velocity.Y + num425;
					}
					else if (npc.velocity.Y > num428)
					{
						npc.velocity.Y = npc.velocity.Y - num425;
						if (npc.velocity.Y > 0f && num428 < 0f)
							npc.velocity.Y = npc.velocity.Y - num425;
					}

					// Fire cursed flames for 5 seconds
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 300f)
					{
						// Reset AI array and go to charging phase
						npc.ai[1] = 1f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.target = 255;
						npc.netUpdate = true;
					}
					else
					{
						// Fire cursed flame every half second
						if (!Main.player[npc.target].dead)
							npc.ai[3] += 1f;

						if (npc.ai[3] >= 30f)
						{
							npc.ai[3] = 0f;
							vector44 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							num427 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector44.X;
							num428 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector44.Y;
							if (Main.netMode != 1)
							{
								float num430 = 15f;
								int num431 = 22;
								int num432 = ProjectileID.CursedFlameHostile;

								num429 = (float)Math.Sqrt((double)(num427 * num427 + num428 * num428));
								num429 = num430 / num429;
								num427 *= num429;
								num428 *= num429;
								num427 += (float)Main.rand.Next(-10, 11) * 0.05f;
								num428 += (float)Main.rand.Next(-10, 11) * 0.05f;
								vector44.X += num427 * 4f;
								vector44.Y += num428 * 4f;

								Projectile.NewProjectile(vector44.X, vector44.Y, num427, num428, num432, num431, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}

				// Charging phase
				else if (npc.ai[1] == 1f)
				{
					// Rotation and velocity
					npc.rotation = num420;
					float num434 = 16f;
					Vector2 vector45 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num435 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector45.X;
					float num436 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector45.Y;
					float num437 = (float)Math.Sqrt((double)(num435 * num435 + num436 * num436));
					num437 = num434 / num437;
					npc.velocity.X = num435 * num437;
					npc.velocity.Y = num436 * num437;
					npc.ai[1] = 2f;
				}
				else if (npc.ai[1] == 2f)
				{
					npc.ai[2] += 1f;

					float timeBeforeSlowDown = CalamityWorld.death ? 15f : 12f;
					if (npc.ai[2] >= timeBeforeSlowDown)
					{
						// Slow down
						npc.velocity.X = npc.velocity.X * 0.9f;
						npc.velocity.Y = npc.velocity.Y * 0.9f;

						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
							npc.velocity.X = 0f;
						if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
							npc.velocity.Y = 0f;
					}
					else
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

					// Charge 8 times
					float chargeTime = CalamityWorld.death ? 35f : 38f;
					if (npc.ai[2] >= chargeTime)
					{
						// Reset AI array and go to cursed fireball phase
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = num420;
						if (npc.ai[3] >= 8f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
						}
						else
							npc.ai[1] = 1f;
					}
				}

				// Enter phase 2 earlier
				double healthMult = ((CalamityWorld.death || CalamityWorld.bossRushActive) ? 0.8 : 0.7);
				if ((double)npc.life < (double)npc.lifeMax * healthMult || targetFloatingUp)
				{
					// Reset AI array and go to transition phase
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}

			// Transition phase
			else if (npc.ai[0] == 1f || npc.ai[0] == 2f)
			{
				// AI timer for rotation
				if (npc.ai[0] == 1f)
				{
					npc.ai[2] += 0.005f;
					if ((double)npc.ai[2] > 0.5)
						npc.ai[2] = 0.5f;
				}
				else
				{
					npc.ai[2] -= 0.005f;
					if (npc.ai[2] < 0f)
						npc.ai[2] = 0f;
				}

				// Spin around like a moron while flinging blood and gore everywhere
				npc.rotation += npc.ai[2];
				npc.ai[1] += 1f;
				if (npc.ai[1] == 100f)
				{
					npc.ai[0] += 1f;
					npc.ai[1] = 0f;

					if (npc.ai[0] == 3f)
						npc.ai[2] = 0f;
					else
					{
						Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);

						int num;
						for (int num438 = 0; num438 < 2; num438 = num + 1)
						{
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 144, 1f);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
							num = num438;
						}

						for (int num439 = 0; num439 < 20; num439 = num + 1)
						{
							Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);
							num = num439;
						}

						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
					}
				}

				Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);

				npc.velocity.X = npc.velocity.X * 0.98f;
				npc.velocity.Y = npc.velocity.Y * 0.98f;

				if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
					npc.velocity.X = 0f;
				if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
					npc.velocity.Y = 0f;
			}

			// Phase 2
			else
			{
				// If in phase 2 but Ret isn't
				bool retInPhase1 = false;
				if (CalamityGlobalNPC.laserEye != -1)
				{
					if (Main.npc[CalamityGlobalNPC.laserEye].active)
						retInPhase1 = Main.npc[CalamityGlobalNPC.laserEye].ai[0] == 1f || Main.npc[CalamityGlobalNPC.laserEye].ai[0] == 2f || Main.npc[CalamityGlobalNPC.laserEye].ai[0] == 0f;
				}
				int defenseGain = retInPhase1 ? 9999 : 30;
				npc.chaseable = !retInPhase1;

				// Increase defense and damage
				npc.damage = (int)((double)npc.defDamage * 1.5);
				npc.defense = npc.defDefense + defenseGain;

				// Change hit sound to metal
				npc.HitSound = SoundID.NPCHit4;

				// Shadowflamethrower phase
				if (npc.ai[1] == 0f)
				{
					float num440 = 6.2f;
					float num441 = 0.1f;

					int num442 = 1;
					if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
						num442 = -1;

					Vector2 vector46 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num443 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + (float)(num442 * 180) - vector46.X;
					float num444 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector46.Y;
					float num445 = (float)Math.Sqrt((double)(num443 * num443 + num444 * num444));

					// Boost speed if too far from target
					if (num445 > 300f)
						num440 += 0.5f;
					if (num445 > 400f)
						num440 += 0.5f;

					// Enrage
					if (targetFloatingUp)
						num440 += 2f;

					// Speed increase
					if (enraged || configBossRushBoost)
						num440 += 2f;

					num445 = num440 / num445;
					num443 *= num445;
					num444 *= num445;

					if (npc.velocity.X < num443)
					{
						npc.velocity.X = npc.velocity.X + num441;
						if (npc.velocity.X < 0f && num443 > 0f)
							npc.velocity.X = npc.velocity.X + num441;
					}
					else if (npc.velocity.X > num443)
					{
						npc.velocity.X = npc.velocity.X - num441;
						if (npc.velocity.X > 0f && num443 < 0f)
							npc.velocity.X = npc.velocity.X - num441;
					}
					if (npc.velocity.Y < num444)
					{
						npc.velocity.Y = npc.velocity.Y + num441;
						if (npc.velocity.Y < 0f && num444 > 0f)
							npc.velocity.Y = npc.velocity.Y + num441;
					}
					else if (npc.velocity.Y > num444)
					{
						npc.velocity.Y = npc.velocity.Y - num441;
						if (npc.velocity.Y > 0f && num444 < 0f)
							npc.velocity.Y = npc.velocity.Y - num441;
					}

					// Fire flamethrower for x seconds
					npc.ai[2] += (retAlive ? 1f : 2f);
					if (npc.ai[2] >= 180f)
					{
						npc.ai[1] = ((!retAlive || enrage) ? 5f : 1f);
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.target = 255;
						npc.netUpdate = true;
					}

					// Fire fireballs and flamethrower
					bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
					if (canHit || !retAlive || enrage)
					{
						// Play flame sound on timer
						npc.localAI[2] += 1f;
						if (npc.localAI[2] > 22f)
						{
							npc.localAI[2] = 0f;
							Main.PlaySound(SoundID.Item34, npc.position);
						}

						if (Main.netMode != 1)
						{
							npc.localAI[1] += 1f;
							if (npc.localAI[1] > 2f)
							{
								npc.localAI[1] = 0f;

								float num446 = 6f;

								int num447 = 27;
								int num448 = mod.ProjectileType("Shadowflamethrower");

								vector46 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
								num443 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector46.X;
								num444 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector46.Y;
								num445 = (float)Math.Sqrt((double)(num443 * num443 + num444 * num444));
								num445 = num446 / num445;
								num443 *= num445;
								num444 *= num445;
								num444 += (float)Main.rand.Next(-10, 11) * 0.01f;
								num443 += (float)Main.rand.Next(-10, 11) * 0.01f;
								num444 += npc.velocity.Y * 0.5f;
								num443 += npc.velocity.X * 0.5f;
								vector46.X -= num443 * 1f;
								vector46.Y -= num444 * 1f;

								if (canHit)
									Projectile.NewProjectile(vector46.X, vector46.Y, num443, num444, num448, num447, 0f, Main.myPlayer, 0f, 0f);
								else
								{
									int proj = Projectile.NewProjectile(vector46.X, vector46.Y, num443, num444, num448, num447, 0f, Main.myPlayer, 0f, 0f);
									Main.projectile[proj].tileCollide = false;
								}
							}
						}
					}
				}

				// Charging phase
				else
				{
					// Charge
					if (npc.ai[1] == 1f)
					{
						// Play charge sound
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

						// Set rotation and velocity
						npc.rotation = num420;
						float num450 = (CalamityWorld.death ? 17f : 16.75f);

						// Enrage
						if (targetFloatingUp)
							num450 += 2f;

						Vector2 vector47 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num451 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector47.X;
						float num452 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector47.Y;
						float num453 = (float)Math.Sqrt((double)(num451 * num451 + num452 * num452));
						num453 = num450 / num453;
						npc.velocity.X = num451 * num453;
						npc.velocity.Y = num452 * num453;
						npc.ai[1] = 2f;
						return false;
					}

					if (npc.ai[1] == 2f)
					{
						npc.ai[2] += (retAlive ? 1f : 1.25f);

						// Enrage
						if (targetFloatingUp)
							npc.ai[2] += 0.25f;

						float chargeTime = 50f;

						// Slow down
						if (npc.ai[2] >= chargeTime)
						{
							npc.velocity *= 0.93f;
							if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
								npc.velocity.X = 0f;
							if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
								npc.velocity.Y = 0f;
						}
						else
							npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

						// Charges 5 times
						if (npc.ai[2] >= chargeTime + 30f)
						{
							npc.ai[3] += 1f;
							npc.ai[2] = 0f;
							npc.target = 255;
							npc.rotation = num420;
							if (npc.ai[3] >= 5f)
							{
								npc.ai[1] = 0f;
								npc.ai[3] = 0f;
								return false;
							}
							npc.ai[1] = 1f;
						}
					}

					// Crazy charge
					else if (npc.ai[1] == 3f)
					{
						// Reset AI array and go to shadowflamethrower phase or fireball phase if ret is dead
						float secondFastCharge = 4f;
						if (npc.ai[3] >= (retAlive ? secondFastCharge : secondFastCharge + 1f))
						{
							npc.TargetClosest(true);
							npc.ai[1] = (retAlive ? 0f : 5f);
							npc.ai[2] = 0f;
							npc.ai[3] = 0f;

							if (npc.ai[1] == 0f)
								npc.localAI[1] = -20f;

							npc.netUpdate = true;

							if (npc.netSpam > 10)
								npc.netSpam = 10;
						}

						// Set charging velocity
						else if (Main.netMode != 1)
						{
							// Get a target
							npc.TargetClosest(true);

							// Velocity
							float num48 = ((enraged || configBossRushBoost) ? 30f : 20f);
							if (npc.ai[2] == -1f || (!retAlive && npc.ai[3] == secondFastCharge))
								num48 *= 1.3f;

							Vector2 vector10 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float num49 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector10.X;
							float num50 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector10.Y;
							float num51 = (float)Math.Sqrt((double)(num49 * num49 + num50 * num50));
							float num52 = num51;

							num51 = num48 / num51;
							npc.velocity.X = num49 * num51;
							npc.velocity.Y = num50 * num51;

							if (retAlive)
							{
								if (num52 < 100f)
								{
									if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
									{
										float num56 = Math.Abs(npc.velocity.X);
										float num57 = Math.Abs(npc.velocity.Y);

										if (npc.Center.X > Main.player[npc.target].Center.X)
											num57 *= -1f;
										if (npc.Center.Y > Main.player[npc.target].Center.Y)
											num56 *= -1f;

										npc.velocity.X = num57;
										npc.velocity.Y = num56;
									}
								}
							}

							npc.ai[1] = 4f;
							npc.netUpdate = true;

							if (npc.netSpam > 10)
								npc.netSpam = 10;
						}
					}

					// Crazy charge
					else if (npc.ai[1] == 4f)
					{
						if (npc.ai[2] == 0f)
							Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

						float num60 = ((!retAlive && npc.ai[3] == 4f) ? 75f : 50f);

						npc.ai[2] += 1f;

						if (npc.ai[2] == num60 && Vector2.Distance(npc.position, Main.player[npc.target].position) < (retAlive ? 200f : 150f))
							npc.ai[2] -= 1f;

						// Slow down
						if (npc.ai[2] >= num60)
						{
							npc.velocity *= 0.93f;
							if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
								npc.velocity.X = 0f;
							if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
								npc.velocity.Y = 0f;
						}
						else
							npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

						// Charge 3 times
						float num61 = num60 + 25f;
						if (npc.ai[2] >= num61)
						{
							npc.netUpdate = true;

							if (npc.netSpam > 10)
								npc.netSpam = 10;

							npc.ai[3] += 1f;
							npc.ai[2] = 0f;
							npc.ai[1] = 3f;
						}
					}

					// Get in position for charge
					else if (npc.ai[1] == 5f)
					{
						float num62 = (retAlive ? 600f : 500f);
						float num63 = ((enraged || configBossRushBoost) ? 24f : 16f);
						float num64 = ((enraged || configBossRushBoost) ? 0.6f : 0.4f);

						if (retAlive)
						{
							num63 *= 0.75f;
							num64 *= 0.75f;
						}

						Vector2 vector11 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num65 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector11.X;
						float num66 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) + num62 - vector11.Y;
						float num67 = (float)Math.Sqrt((double)(num65 * num65 + num66 * num66));

						num67 = num63 / num67;
						num65 *= num67;
						num66 *= num67;

						if (npc.velocity.X < num65)
						{
							npc.velocity.X = npc.velocity.X + num64;
							if (npc.velocity.X < 0f && num65 > 0f)
								npc.velocity.X = npc.velocity.X + num64;
						}
						else if (npc.velocity.X > num65)
						{
							npc.velocity.X = npc.velocity.X - num64;
							if (npc.velocity.X > 0f && num65 < 0f)
								npc.velocity.X = npc.velocity.X - num64;
						}
						if (npc.velocity.Y < num66)
						{
							npc.velocity.Y = npc.velocity.Y + num64;
							if (npc.velocity.Y < 0f && num66 > 0f)
								npc.velocity.Y = npc.velocity.Y + num64;
						}
						else if (npc.velocity.Y > num66)
						{
							npc.velocity.Y = npc.velocity.Y - num64;
							if (npc.velocity.Y > 0f && num66 < 0f)
								npc.velocity.Y = npc.velocity.Y - num64;
						}

						npc.ai[2] += 1f;

						// Fire shadowflames
						float fireRate = (retAlive ? 30f : 20f);
						if (npc.ai[2] % fireRate == 0f)
						{
							Vector2 vector44 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float num427 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector44.X;
							float num428 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector44.Y;
							if (Main.netMode != 1)
							{
								float num430 = 16f;
								int num431 = 26;
								int num432 = mod.ProjectileType("Shadowflame");

								float num429 = (float)Math.Sqrt((double)(num427 * num427 + num428 * num428));
								num429 = num430 / num429;
								num427 *= num429;
								num428 *= num429;
								vector44.X += num427 * 4f;
								vector44.Y += num428 * 4f;

								Projectile.NewProjectile(vector44.X, vector44.Y, num427, num428, num432, num431, 0f, Main.myPlayer, 0f, (retAlive ? 0f : 1f));
							}
						}

						// Take 3 seconds to get in position, then charge
						if (npc.ai[2] >= (retAlive ? 180f : 135f))
						{
							npc.TargetClosest(true);
							npc.ai[1] = 3f;
							npc.ai[2] = -1f;
							npc.netUpdate = true;
						}
					}
				}
			}

			return false;
		}
		#endregion

		#region Buffed Skeletron Prime AI
		public static bool BuffedSkeletronPrimeAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);
			bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;

			// Enrage variable if player is floating upside down
			bool targetFloatingUp = Main.player[npc.target].gravDir == -1f;

			// Spawn arms
			if (npc.ai[0] == 0f && Main.netMode != 1)
			{
				npc.TargetClosest(true);
				npc.ai[0] = 1f;

				int arm = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PrimeCannon, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				Main.npc[arm].ai[0] = -1f;
				Main.npc[arm].ai[1] = (float)npc.whoAmI;
				Main.npc[arm].target = npc.target;
				Main.npc[arm].netUpdate = true;

				arm = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PrimeSaw, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				Main.npc[arm].ai[0] = 1f;
				Main.npc[arm].ai[1] = (float)npc.whoAmI;
				Main.npc[arm].target = npc.target;
				Main.npc[arm].netUpdate = true;

				arm = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PrimeVice, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				Main.npc[arm].ai[0] = -1f;
				Main.npc[arm].ai[1] = (float)npc.whoAmI;
				Main.npc[arm].target = npc.target;
				Main.npc[arm].ai[3] = 150f;
				Main.npc[arm].netUpdate = true;

				arm = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PrimeLaser, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				Main.npc[arm].ai[0] = 1f;
				Main.npc[arm].ai[1] = (float)npc.whoAmI;
				Main.npc[arm].target = npc.target;
				Main.npc[arm].netUpdate = true;
				Main.npc[arm].ai[3] = 150f;
			}

			// Check if arms are alive
			bool cannonAlive = false;
			bool laserAlive = false;
			bool viceAlive = false;
			bool sawAlive = false;
			if (CalamityGlobalNPC.primeCannon != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeCannon].active)
					cannonAlive = true;
			}
			if (CalamityGlobalNPC.primeLaser != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeLaser].active)
					laserAlive = true;
			}
			if (CalamityGlobalNPC.primeVice != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeVice].active)
					viceAlive = true;
			}
			if (CalamityGlobalNPC.primeSaw != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeSaw].active)
					sawAlive = true;
			}
			bool allArmsDead = !cannonAlive && !laserAlive && !viceAlive && !sawAlive;
			npc.chaseable = allArmsDead;

			// Set stats
			if (npc.ai[1] == 5f)
				npc.damage = 0;
			else if (allArmsDead)
			{
				npc.damage = npc.defDamage;
				npc.defense = npc.defDefense;
			}
			else
				npc.defense = 9999;

			// Phases
			bool phase2 = (double)npc.life <= (double)npc.lifeMax * 0.66;
			bool phase3 = (double)npc.life <= (double)npc.lifeMax * 0.33;

			// Despawn
			if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 6000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 6000f)
			{
				npc.TargetClosest(true);
				if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 6000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 6000f)
					npc.ai[1] = 3f;
			}

			// Activate daytime enrage
			if (Main.dayTime && npc.ai[1] != 3f && npc.ai[1] != 2f)
			{
				// Heal
				if (Main.netMode != 1)
				{
					int healAmt = npc.lifeMax - npc.life;
					if (healAmt > 0)
					{
						npc.life += healAmt;
						npc.HealEffect(healAmt, true);
						npc.netUpdate = true;
					}
				}

				npc.ai[1] = 2f;
				Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
			}

			// Float near player
			if (npc.ai[1] == 0f || npc.ai[1] == 4f)
			{
				// Start other phases if arms are dead, start with spinning phase
				if (allArmsDead)
				{
					// Start spin phase after 5 seconds
					npc.ai[2] += (phase3 ? 1.25f : 1f);
					if (npc.ai[2] >= 300f)
					{
						bool shouldSpinAroundTarget = npc.ai[1] == 4f && npc.position.Y < Main.player[npc.target].position.Y - 400f &&
							Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 600f && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 400f;

						if (shouldSpinAroundTarget || npc.ai[1] != 4f)
						{
							if (shouldSpinAroundTarget)
								npc.ai[3] = Vector2.Distance(Main.player[npc.target].Center, npc.Center);

							npc.ai[2] = 0f;
							npc.ai[1] = (shouldSpinAroundTarget ? 5f : 1f);
							npc.TargetClosest(true);
							npc.netUpdate = true;
						}
					}
				}

				if (Main.netMode != 1)
				{
					// Shoot homing skulls
					if (!allArmsDead)
					{
						npc.localAI[0] += 1f;
						if (npc.localAI[0] >= (CalamityWorld.death ? 510f : 540f))
						{
							npc.localAI[0] = 0f;
							Vector2 vector16 = npc.Center;
							if (Collision.CanHit(vector16, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
							{
								float num159 = 7f;
								float num160 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector16.X + (float)Main.rand.Next(-20, 21);
								float num161 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector16.Y + (float)Main.rand.Next(-20, 21);
								float num162 = (float)Math.Sqrt((double)(num160 * num160 + num161 * num161));
								num162 = num159 / num162;
								num160 *= num162;
								num161 *= num162;

								Vector2 value = new Vector2(num160 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f, num161 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f);
								value.Normalize();
								value *= num159;
								value += npc.velocity;
								num160 = value.X;
								num161 = value.Y;

								int num163 = 23;
								int num164 = ProjectileID.Skull;
								vector16 += value * 5f;
								int num165 = Projectile.NewProjectile(vector16.X, vector16.Y, num160, num161, num164, num163, 0f, Main.myPlayer, -1f, 0f);
								Main.projectile[num165].timeLeft = 300;
							}
						}
					}

					// Ring of lasers if laser is dead
					if (!laserAlive)
					{
						npc.localAI[1] += (allArmsDead ? 2f : 1f);
						if (phase3)
							npc.localAI[1] += 1f;

						if (npc.localAI[1] >= (CalamityWorld.death ? 450f : 480f))
						{
							npc.localAI[1] = 0f;
							Vector2 shootFromVector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							int totalProjectiles = 12;
							float spread = MathHelper.ToRadians(30); // 30 degrees in radians = 0.523599
							double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2; // Where the projectiles start spawning at, don't change this
							double deltaAngle = spread / (float)totalProjectiles; // Angle between each projectile, 0.04363325
							double offsetAngle;
							float velocity = 5f;
							int damage = 25;
							int i;
							for (i = 0; i < 6; i++)
							{
								offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i; // Used to be 32
								int proj = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(Math.Sin(offsetAngle) * velocity), (float)(Math.Cos(offsetAngle) * velocity), ProjectileID.DeathLaser, damage, 0f, Main.myPlayer, 0f, 0f);
								int proj2 = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(-Math.Sin(offsetAngle) * velocity), (float)(-Math.Cos(offsetAngle) * velocity), ProjectileID.DeathLaser, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[proj].timeLeft = 300;
								Main.projectile[proj2].timeLeft = 300;
							}
						}
					}

					// Spread of rockets if cannon is dead
					if (!cannonAlive)
					{
						npc.localAI[2] += (allArmsDead ? 2f : 1f);
						if (phase3)
							npc.localAI[2] += 1f;

						if (npc.localAI[2] >= (CalamityWorld.death ? 215f : 240f))
						{
							npc.localAI[2] = 0f;
							float num502 = 8f;
							int num503 = 27;
							int num504 = ProjectileID.RocketSkeleton;
							Vector2 value19 = Main.player[npc.target].Center - npc.Center;
							value19.Normalize();
							value19 *= num502;
							int numProj = 2;
							float rotation = MathHelper.ToRadians(3);
							for (int i = 0; i < numProj + 1; i++)
							{
								Vector2 perturbedSpeed = value19.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
								Projectile.NewProjectile(npc.Center.X, npc.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, num504, num503, 0f, Main.myPlayer, 0f, 1f);
							}
						}
					}
				}

				npc.rotation = npc.velocity.X / 15f;

				if (npc.position.Y > Main.player[npc.target].position.Y - 350f)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y = npc.velocity.Y * 0.98f;

					npc.velocity.Y = npc.velocity.Y - 0.1f;

					if (npc.velocity.Y > 2f)
						npc.velocity.Y = 2f;
				}
				else if (npc.position.Y < Main.player[npc.target].position.Y - 500f)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y = npc.velocity.Y * 0.98f;

					npc.velocity.Y = npc.velocity.Y + 0.1f;

					if (npc.velocity.Y < -2f)
						npc.velocity.Y = -2f;
				}

				if (npc.position.X + (float)(npc.width / 2) > Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + 100f)
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X * 0.98f;

					npc.velocity.X = npc.velocity.X - 0.1f;

					if (npc.velocity.X > 8f)
						npc.velocity.X = 8f;
				}
				if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - 100f)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X * 0.98f;

					npc.velocity.X = npc.velocity.X + 0.1f;

					if (npc.velocity.X < -8f)
						npc.velocity.X = -8f;
				}
			}

			else
			{
				// Spinning
				if (npc.ai[1] == 1f)
				{
					npc.defense *= 2;
					npc.damage *= 2;

					if (phase2)
					{
						npc.localAI[1] += 1f;
						if (npc.localAI[1] >= (CalamityWorld.death ? 39f : 45f))
						{
							npc.localAI[1] = 0f;
							Vector2 shootFromVector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							int totalProjectiles = 12;
							float spread = MathHelper.ToRadians(30); // 30 degrees in radians = 0.523599
							double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2; // Where the projectiles start spawning at, don't change this
							double deltaAngle = spread / (float)totalProjectiles; // Angle between each projectile, 0.04363325
							double offsetAngle;
							float velocity = 5f;
							int damage = 25;
							int i;
							for (i = 0; i < 6; i++)
							{
								offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i; // Used to be 32
								int proj = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(Math.Sin(offsetAngle) * velocity), (float)(Math.Cos(offsetAngle) * velocity), ProjectileID.DeathLaser, damage, 0f, Main.myPlayer, 0f, 0f);
								int proj2 = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(-Math.Sin(offsetAngle) * velocity), (float)(-Math.Cos(offsetAngle) * velocity), ProjectileID.DeathLaser, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[proj].timeLeft = 300;
								Main.projectile[proj2].timeLeft = 300;
							}
						}
					}

					npc.ai[2] += 1f;
					if (npc.ai[2] == 2f)
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

					// Spin for 3 seconds then return to floating phase
					float phaseTimer = (CalamityWorld.death ? 150f : 180f);
					if (phase2 && !phase3)
						phaseTimer += 120f;

					if (npc.ai[2] >= phaseTimer)
					{
						npc.ai[2] = 0f;
						npc.ai[1] = 4f;
					}

					npc.rotation += (float)npc.direction * 0.3f;
					Vector2 vector48 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num455 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector48.X;
					float num456 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector48.Y;
					float num457 = (float)Math.Sqrt((double)(num455 * num455 + num456 * num456));

					float speed = CalamityWorld.bossRushActive ? 7f : 3f;
					if (phase2)
						speed += 1f;
					if (phase3)
						speed += 1f;
					if (enraged || configBossRushBoost)
						speed += 3f;
					if (targetFloatingUp)
						speed += 5f;

					float speedBuff = 3f + (3f * (1f - (float)npc.life / (float)npc.lifeMax));
					float speedBuff2 = 8f + (8f * (1f - (float)npc.life / (float)npc.lifeMax));

					speed += num457 / 100f;
					if (speed < speedBuff)
						speed = speedBuff;
					if (speed > speedBuff2)
						speed = speedBuff2;

					num457 = speed / num457;
					npc.velocity.X = num455 * num457;
					npc.velocity.Y = num456 * num457;
				}

				// Daytime enrage
				if (npc.ai[1] == 2f)
				{
					npc.damage = 1000;
					npc.defense = 9999;

					npc.rotation += (float)npc.direction * 0.3f;

					Vector2 vector49 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num458 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector49.X;
					float num459 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector49.Y;
					float num460 = (float)Math.Sqrt((double)(num458 * num458 + num459 * num459));

					float num461 = 10f;
					num461 += num460 / 100f;
					if (num461 < 8f)
						num461 = 8f;
					if (num461 > 32f)
						num461 = 32f;

					num460 = num461 / num460;
					npc.velocity.X = num458 * num460;
					npc.velocity.Y = num459 * num460;

					if (Main.netMode != 1)
					{
						npc.localAI[0] += 1f;
						if (npc.localAI[0] >= (CalamityWorld.death ? 54f : 60f))
						{
							npc.localAI[0] = 0f;
							Vector2 vector16 = npc.Center;
							if (Collision.CanHit(vector16, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
							{
								float num159 = 7f;
								float num160 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector16.X + (float)Main.rand.Next(-20, 21);
								float num161 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector16.Y + (float)Main.rand.Next(-20, 21);
								float num162 = (float)Math.Sqrt((double)(num160 * num160 + num161 * num161));
								num162 = num159 / num162;
								num160 *= num162;
								num161 *= num162;

								Vector2 value = new Vector2(num160 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f, num161 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f);
								value.Normalize();
								value *= num159;
								value += npc.velocity;
								num160 = value.X;
								num161 = value.Y;

								int num163 = 250;
								int num164 = ProjectileID.Skull;
								vector16 += value * 5f;
								int num165 = Projectile.NewProjectile(vector16.X, vector16.Y, num160, num161, num164, num163, 0f, Main.myPlayer, -1f, 0f);
								Main.projectile[num165].timeLeft = 300;
							}
						}
					}
				}

				// Despawning
				if (npc.ai[1] == 3f)
				{
					npc.velocity.Y = npc.velocity.Y + 0.1f;
					if (npc.velocity.Y < 0f)
						npc.velocity.Y = npc.velocity.Y * 0.95f;

					npc.velocity.X = npc.velocity.X * 0.95f;

					if (npc.timeLeft > 500)
						npc.timeLeft = 500;
				}

				// Fly around target in a circle and spawn probes
				if (npc.ai[1] == 5f)
				{
					npc.ai[2] += 1f;

					npc.rotation = npc.velocity.X / 30f;

					// Spin for 3 seconds
					float spinVelocity = 45f;
					if (npc.ai[2] == 2f)
					{
						// Play angry noise
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

						// Set spin direction
						if (Main.player[npc.target].velocity.X > 0)
							calamityGlobalNPC.newAI[0] = 1f;
						else if (Main.player[npc.target].velocity.X < 0)
							calamityGlobalNPC.newAI[0] = -1f;
						else
							calamityGlobalNPC.newAI[0] = (float)Main.player[npc.target].direction;

						// Set spin velocity
						npc.velocity.X = 3.14159265f * npc.ai[3] / spinVelocity;
						npc.velocity *= -calamityGlobalNPC.newAI[0];
						npc.netUpdate = true;
					}

					// Spawn 6 Probes and maintain velocity, or spit skulls if probes are alive
					else
					{
						npc.velocity = npc.velocity.RotatedBy(3.14159265 / spinVelocity * -calamityGlobalNPC.newAI[0]);
						if (npc.ai[2] % 30f == 0f)
						{
							calamityGlobalNPC.newAI[1] += 1f;

							if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 48f)
							{
								if (Main.netMode != 1)
								{
									if (NPC.CountNPCS(NPCID.Probe) < 6)
										NPC.NewNPC((int)npc.Center.X, (int)(npc.Center.Y + 4f), NPCID.Probe);
									else
									{
										Vector2 vector16 = npc.Center;
										float num159 = 4f;
										float num160 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector16.X + (float)Main.rand.Next(-20, 21);
										float num161 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector16.Y + (float)Main.rand.Next(-20, 21);
										float num162 = (float)Math.Sqrt((double)(num160 * num160 + num161 * num161));
										num162 = num159 / num162;
										num160 *= num162;
										num161 *= num162;

										Vector2 value = new Vector2(num160 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f, num161 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f);
										value.Normalize();
										value *= num159;
										num160 = value.X;
										num161 = value.Y;

										int num163 = 23;
										int num164 = ProjectileID.Skull;
										vector16 += value * 5f;
										int num165 = Projectile.NewProjectile(vector16.X, vector16.Y, num160, num161, num164, num163, 0f, Main.myPlayer, -1f, 0f);
										Main.projectile[num165].timeLeft = 180;
										Main.projectile[num165].tileCollide = false;
									}
								}
							}

							// Go to floating phase, or spinning phase if in phase 2
							if (calamityGlobalNPC.newAI[1] >= 6f)
							{
								npc.velocity.Normalize();

								// Fly overhead and spit missiles if on low health
								npc.ai[1] = (phase3 ? 6f : 1f);
								npc.ai[2] = 0f;
								npc.ai[3] = 0f;
								calamityGlobalNPC.newAI[1] = 0f;
								calamityGlobalNPC.newAI[0] = 0f;
								npc.netUpdate = true;
							}
						}
					}
				}

				// Fly overhead and spit missiles
				if (npc.ai[1] == 6f)
				{
					npc.ai[2] += 1f;

					npc.rotation = npc.velocity.X / 15f;

					float flightVelocity = 10f;
					float fligthAcceleration = 0.12f;

					// Spit homing missiles and then go to floating phase
					Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
					float posX = Main.player[npc.target].Center.X - center.X;
					float posY = Main.player[npc.target].Center.Y - center.Y - 500f;

					float distance = (float)Math.Sqrt((double)(posX * posX + posY * posY));
					if (distance < 20f)
					{
						posX = npc.velocity.X;
						posY = npc.velocity.Y;
					}
					else
					{
						distance = flightVelocity / distance;
						posX *= distance;
						posY *= distance;
					}

					if (npc.velocity.X < posX)
					{
						npc.velocity.X = npc.velocity.X + fligthAcceleration;
						if (npc.velocity.X < 0f && posX > 0f)
							npc.velocity.X = npc.velocity.X + fligthAcceleration;
					}
					else if (npc.velocity.X > posX)
					{
						npc.velocity.X = npc.velocity.X - fligthAcceleration;
						if (npc.velocity.X > 0f && posX < 0f)
							npc.velocity.X = npc.velocity.X - fligthAcceleration;
					}
					if (npc.velocity.Y < posY)
					{
						npc.velocity.Y = npc.velocity.Y + fligthAcceleration;
						if (npc.velocity.Y < 0f && posY > 0f)
							npc.velocity.Y = npc.velocity.Y + fligthAcceleration;
					}
					else if (npc.velocity.Y > posY)
					{
						npc.velocity.Y = npc.velocity.Y - fligthAcceleration;
						if (npc.velocity.Y > 0f && posY < 0f)
							npc.velocity.Y = npc.velocity.Y - fligthAcceleration;
					}

					if (npc.ai[2] % 12f == 0f)
					{
						calamityGlobalNPC.newAI[1] += 1f;

						if (Main.netMode != 1)
						{
							Vector2 velocity = new Vector2(-1f * (float)Main.rand.NextDouble() * 3f, 1f);
							velocity = velocity.RotatedBy((Main.rand.NextDouble() - 0.5) * 0.78539818525314331, default(Vector2));
							velocity *= 5f;
							int damage = Main.expertMode ? 27 : 35;
							float delayBeforeHoming = 45f;
							Projectile.NewProjectile(npc.Center.X + Main.rand.Next(npc.width / 2), npc.Center.Y + 4f, velocity.X, velocity.Y, ProjectileID.SaucerMissile, damage, 0f, Main.myPlayer, 0f, delayBeforeHoming);
						}

						Main.PlaySound(SoundID.Item39, npc.Center);

						if (calamityGlobalNPC.newAI[1] >= 10f)
						{
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
							calamityGlobalNPC.newAI[0] = 0f;
							calamityGlobalNPC.newAI[1] = 0f;
							npc.localAI[1] = -90f;
							npc.localAI[2] = -90f;
							npc.netUpdate = true;
						}
					}
				}
			}
			return false;
		}

		public static bool BuffedPrimeLaserAI(NPC npc, Mod mod)
		{
			// Set direction
			npc.spriteDirection = -(int)npc.ai[0];

			// Despawn if head is gone
			if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 32)
			{
				npc.ai[2] += 10f;
				if (npc.ai[2] > 50f || Main.netMode != 2)
				{
					npc.life = -1;
					npc.HitEffect(0, 10.0);
					npc.active = false;
				}
			}

			CalamityGlobalNPC.primeLaser = npc.whoAmI;

			// Check if arms are alive
			bool cannonAlive = false;
			bool viceAlive = false;
			bool sawAlive = false;
			if (CalamityGlobalNPC.primeCannon != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeCannon].active)
					cannonAlive = true;
			}
			if (CalamityGlobalNPC.primeVice != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeVice].active)
					viceAlive = true;
			}
			if (CalamityGlobalNPC.primeSaw != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeSaw].active)
					sawAlive = true;
			}

			// Phase 1
			if (npc.ai[2] == 0f)
			{
				// Despawn if head is despawning
				if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
					npc.timeLeft = 10;

				// Go to other phase after 13.3 seconds (change this as each arm dies)
				npc.ai[3] += 1f;
				if (!cannonAlive)
					npc.ai[3] += 1f;
				if (!viceAlive)
					npc.ai[3] += 1f;
				if (!sawAlive)
					npc.ai[3] += 1f;

				if (npc.ai[3] >= (CalamityWorld.death ? 680f : 800f))
				{
					npc.localAI[0] = 0f;
					npc.ai[2] = 1f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}

				if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 100f)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y = npc.velocity.Y * 0.96f;

					npc.velocity.Y = npc.velocity.Y - 0.1f;

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
				}
				else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 100f)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y = npc.velocity.Y * 0.96f;

					npc.velocity.Y = npc.velocity.Y + 0.1f;

					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
				}

				if (npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 180f * npc.ai[0])
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X * 0.96f;

					npc.velocity.X = npc.velocity.X - 0.14f;

					if (npc.velocity.X > 8f)
						npc.velocity.X = 8f;
				}
				if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 180f * npc.ai[0])
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X * 0.96f;

					npc.velocity.X = npc.velocity.X + 0.14f;

					if (npc.velocity.X < -8f)
						npc.velocity.X = -8f;
				}

				npc.TargetClosest(true);
				Vector2 vector62 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num506 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector62.X;
				float num507 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector62.Y;
				float num508 = (float)Math.Sqrt((double)(num506 * num506 + num507 * num507));
				npc.rotation = (float)Math.Atan2((double)num507, (double)num506) - 1.57f;

				if (Main.netMode != 1)
				{
					// Fire laser every 1.5 seconds (change this as each arm dies to fire more aggressively)
					npc.localAI[0] += 1f;
					if (!cannonAlive)
						npc.localAI[0] += 1f;
					if (!viceAlive)
						npc.localAI[0] += 1f;
					if (!sawAlive)
						npc.localAI[0] += 1f;

					if (npc.localAI[0] >= 90f)
					{
						npc.localAI[0] = 0f;
						float num509 = 11f;
						int num510 = 25;
						int num511 = ProjectileID.DeathLaser;
						num508 = num509 / num508;
						num506 *= num508;
						num507 *= num508;
						vector62.X += num506 * 8f;
						vector62.Y += num507 * 8f;
						Projectile.NewProjectile(vector62.X, vector62.Y, num506, num507, num511, num510, 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}

			// Other phase, get closer to the player and fire ring of lasers
			else if (npc.ai[2] == 1f)
			{
				// Go to phase 1 after 2 seconds (change this as each arm dies to stay in this phase for longer)
				npc.ai[3] += 1f;

				float timeLimit = (CalamityWorld.death ? 150f : 135f);
				float timeMult = 1.882075f;
				if (!cannonAlive)
					timeLimit *= timeMult;
				if (!viceAlive)
					timeLimit *= timeMult;
				if (!sawAlive)
					timeLimit *= timeMult;

				if (npc.ai[3] >= timeLimit)
				{
					npc.localAI[0] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}

				Vector2 vector63 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num513 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - 350f - vector63.X;
				float num514 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 20f - vector63.Y;
				float num515 = (float)Math.Sqrt((double)(num513 * num513 + num514 * num514));
				num515 = 7f / num515;
				num513 *= num515;
				num514 *= num515;

				if (npc.velocity.X > num513)
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X * 0.9f;
					npc.velocity.X = npc.velocity.X - 0.1f;
				}
				if (npc.velocity.X < num513)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X * 0.9f;
					npc.velocity.X = npc.velocity.X + 0.1f;
				}
				if (npc.velocity.Y > num514)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y = npc.velocity.Y * 0.9f;
					npc.velocity.Y = npc.velocity.Y - 0.03f;
				}
				if (npc.velocity.Y < num514)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y = npc.velocity.Y * 0.9f;
					npc.velocity.Y = npc.velocity.Y + 0.03f;
				}

				npc.TargetClosest(true);
				vector63 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				num513 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector63.X;
				num514 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector63.Y;
				num515 = (float)Math.Sqrt((double)(num513 * num513 + num514 * num514));
				npc.rotation = (float)Math.Atan2((double)num514, (double)num513) - 1.57f;

				if (Main.netMode != 1)
				{
					// Fire laser every 1.5 seconds (change this as each arm dies to fire more aggressively)
					npc.localAI[0] += 1f;
					if (!cannonAlive)
						npc.localAI[0] += 0.5f;
					if (!viceAlive)
						npc.localAI[0] += 0.5f;
					if (!sawAlive)
						npc.localAI[0] += 0.5f;

					if (npc.localAI[0] >= 120f)
					{
						npc.localAI[0] = 0f;
						Vector2 shootFromVector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						int totalProjectiles = 12;
						float spread = MathHelper.ToRadians(30); // 30 degrees in radians = 0.523599
						double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2; // Where the projectiles start spawning at, don't change this
						double deltaAngle = spread / (float)totalProjectiles; // Angle between each projectile, 0.04363325
						double offsetAngle;
						float velocity = 5f;
						int damage = 25;
						int i;
						for (i = 0; i < 6; i++)
						{
							offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i; // Used to be 32
							Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(Math.Sin(offsetAngle) * velocity), (float)(Math.Cos(offsetAngle) * velocity), ProjectileID.DeathLaser, damage, 0f, Main.myPlayer, 0f, 0f);
							Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(-Math.Sin(offsetAngle) * velocity), (float)(-Math.Cos(offsetAngle) * velocity), ProjectileID.DeathLaser, damage, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}
			return false;
		}

		public static bool BuffedPrimeCannonAI(NPC npc, Mod mod)
		{
			npc.spriteDirection = -(int)npc.ai[0];

			if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 32)
			{
				npc.ai[2] += 10f;
				if (npc.ai[2] > 50f || Main.netMode != 2)
				{
					npc.life = -1;
					npc.HitEffect(0, 10.0);
					npc.active = false;
				}
			}

			CalamityGlobalNPC.primeCannon = npc.whoAmI;

			// Check if arms are alive
			bool laserAlive = false;
			bool viceAlive = false;
			bool sawAlive = false;
			if (CalamityGlobalNPC.primeLaser != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeLaser].active)
					laserAlive = true;
			}
			if (CalamityGlobalNPC.primeVice != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeVice].active)
					viceAlive = true;
			}
			if (CalamityGlobalNPC.primeSaw != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeSaw].active)
					sawAlive = true;
			}

			bool fireSlower = false;
			if (laserAlive)
			{
				// If laser is firing ring of lasers
				if (Main.npc[CalamityGlobalNPC.primeLaser].ai[2] == 1f)
					fireSlower = true;
			}
			else
			{
				fireSlower = npc.ai[2] == 0f;

				if (fireSlower)
				{
					// Go to other phase after 13.33 seconds (change this as each arm dies)
					npc.ai[3] += 1f;
					if (!laserAlive)
						npc.ai[3] += 1f;
					if (!viceAlive)
						npc.ai[3] += 1f;
					if (!sawAlive)
						npc.ai[3] += 1f;

					if (npc.ai[3] >= (CalamityWorld.death ? 680f : 800f))
					{
						npc.localAI[0] = 0f;
						npc.ai[2] = 1f;
						fireSlower = false;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
				}
				else
				{
					// Go to phase 1 after 2 seconds (change this as each arm dies to stay in this phase for longer)
					npc.ai[3] += 1f;

					float timeLimit = (CalamityWorld.death ? 145f : 120f);
					float timeMult = 1.882075f;
					if (!laserAlive)
						timeLimit *= timeMult;
					if (!viceAlive)
						timeLimit *= timeMult;
					if (!sawAlive)
						timeLimit *= timeMult;

					if (npc.ai[3] >= timeLimit)
					{
						npc.localAI[0] = 0f;
						npc.ai[2] = 0f;
						fireSlower = true;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
				}
			}

			if (fireSlower)
			{
				if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
					npc.timeLeft = 10;

				if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 150f)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y = npc.velocity.Y * 0.96f;

					npc.velocity.Y = npc.velocity.Y - 0.04f;

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
				}
				else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 150f)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y = npc.velocity.Y * 0.96f;

					npc.velocity.Y = npc.velocity.Y + 0.04f;

					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
				}

				if (npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) + 200f)
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X * 0.96f;

					npc.velocity.X = npc.velocity.X - 0.2f;

					if (npc.velocity.X > 8f)
						npc.velocity.X = 8f;
				}
				if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) + 160f)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X * 0.96f;

					npc.velocity.X = npc.velocity.X + 0.2f;

					if (npc.velocity.X < -8f)
						npc.velocity.X = -8f;
				}

				Vector2 vector60 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num492 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector60.X;
				float num493 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector60.Y;
				float num494 = (float)Math.Sqrt((double)(num492 * num492 + num493 * num493));
				npc.rotation = (float)Math.Atan2((double)num493, (double)num492) - 1.57f;

				if (Main.netMode != 1)
				{
					// Fire rocket every 2 seconds (change this as each arm dies to fire more aggressively)
					npc.localAI[0] += 1f;
					if (!laserAlive)
						npc.localAI[0] += 1f;
					if (!viceAlive)
						npc.localAI[0] += 1f;
					if (!sawAlive)
						npc.localAI[0] += 1f;

					if (npc.localAI[0] >= 120f)
					{
						npc.localAI[0] = 0f;
						float num495 = 8f;
						int num496 = 27;
						int num497 = ProjectileID.RocketSkeleton;
						num494 = num495 / num494;
						num492 *= num494;
						num493 *= num494;
						vector60.X += num492 * 5f;
						vector60.Y += num493 * 5f;
						Projectile.NewProjectile(vector60.X, vector60.Y, num492, num493, num497, num496, 0f, Main.myPlayer, 0f, 1f);
					}
				}
			}

			else
			{
				Vector2 vector61 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num499 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector61.X;
				float num500 = Main.npc[(int)npc.ai[1]].position.Y - vector61.Y;
				num500 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 80f - vector61.Y;
				float num501 = (float)Math.Sqrt((double)(num499 * num499 + num500 * num500));
				num501 = 6f / num501;
				num499 *= num501;
				num500 *= num501;

				if (npc.velocity.X > num499)
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X * 0.9f;
					npc.velocity.X = npc.velocity.X - 0.04f;
				}
				if (npc.velocity.X < num499)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X * 0.9f;
					npc.velocity.X = npc.velocity.X + 0.04f;
				}
				if (npc.velocity.Y > num500)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y = npc.velocity.Y * 0.9f;
					npc.velocity.Y = npc.velocity.Y - 0.08f;
				}
				if (npc.velocity.Y < num500)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y = npc.velocity.Y * 0.9f;
					npc.velocity.Y = npc.velocity.Y + 0.08f;
				}

				npc.TargetClosest(true);
				vector61 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				num499 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector61.X;
				num500 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector61.Y;
				num501 = (float)Math.Sqrt((double)(num499 * num499 + num500 * num500));
				npc.rotation = (float)Math.Atan2((double)num500, (double)num499) - 1.57f;

				if (Main.netMode != 1)
				{
					// Fire rockets every 2 seconds (change this as each arm dies to fire more aggressively)
					npc.localAI[0] += 1f;
					if (!laserAlive)
						npc.localAI[0] += 0.5f;
					if (!viceAlive)
						npc.localAI[0] += 0.5f;
					if (!sawAlive)
						npc.localAI[0] += 0.5f;

					if (npc.localAI[0] >= 180f)
					{
						npc.localAI[0] = 0f;
						float num502 = 8f;
						int num503 = 27;
						int num504 = ProjectileID.RocketSkeleton;
						Vector2 value19 = Main.player[npc.target].Center - npc.Center;
						value19.Normalize();
						value19 *= num502;
						int numProj = 2;
						float rotation = MathHelper.ToRadians(3);
						for (int i = 0; i < numProj + 1; i++)
						{
							Vector2 perturbedSpeed = value19.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
							Projectile.NewProjectile(npc.Center.X, npc.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, num504, num503, 0f, Main.myPlayer, 0f, 1f);
						}
					}
				}
			}
			return false;
		}

		public static bool BuffedPrimeViceAI(NPC npc, Mod mod)
		{
			// Direction
			npc.spriteDirection = -(int)npc.ai[0];

			// Where the vice should be in relation to the head
			Vector2 vector55 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num477 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector55.X;
			float num478 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector55.Y;
			float num479 = (float)Math.Sqrt((double)(num477 * num477 + num478 * num478));

			// Return the vice to its proper location in relation to the head if it's too far away
			if (npc.ai[2] != 99f)
			{
				if (num479 > 800f)
					npc.ai[2] = 99f;
			}
			else if (num479 < 400f)
				npc.ai[2] = 0f;

			// Despawn if head is gone
			if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 32)
			{
				npc.ai[2] += 10f;
				if (npc.ai[2] > 50f || Main.netMode != 2)
				{
					npc.life = -1;
					npc.HitEffect(0, 10.0);
					npc.active = false;
				}
			}

			CalamityGlobalNPC.primeVice = npc.whoAmI;

			// Check if arms are alive
			bool cannonAlive = false;
			bool laserAlive = false;
			bool sawAlive = false;
			if (CalamityGlobalNPC.primeCannon != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeCannon].active)
					cannonAlive = true;
			}
			if (CalamityGlobalNPC.primeLaser != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeLaser].active)
					laserAlive = true;
			}
			if (CalamityGlobalNPC.primeSaw != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeSaw].active)
					sawAlive = true;
			}

			// Return to the head
			if (npc.ai[2] == 99f)
			{
				if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y = npc.velocity.Y * 0.96f;

					npc.velocity.Y = npc.velocity.Y - 0.1f;

					if (npc.velocity.Y > 8f)
						npc.velocity.Y = 8f;
				}
				else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y = npc.velocity.Y * 0.96f;

					npc.velocity.Y = npc.velocity.Y + 0.1f;

					if (npc.velocity.Y < -8f)
						npc.velocity.Y = -8f;
				}

				if (npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2))
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X * 0.96f;

					npc.velocity.X = npc.velocity.X - 0.5f;

					if (npc.velocity.X > 12f)
						npc.velocity.X = 12f;
				}
				if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2))
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X * 0.96f;

					npc.velocity.X = npc.velocity.X + 0.5f;

					if (npc.velocity.X < -12f)
						npc.velocity.X = -12f;
				}
			}

			// Other phases
			else
			{
				// Stay near the head
				if (npc.ai[2] == 0f || npc.ai[2] == 3f)
				{
					// Despawn if head is despawning
					if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
						npc.timeLeft = 10;

					// Start charging after 10 seconds (change this as each arm dies)
					npc.ai[3] += 1f;
					if (!cannonAlive)
						npc.ai[3] += 1f;
					if (!laserAlive)
						npc.ai[3] += 1f;
					if (!sawAlive)
						npc.ai[3] += 1f;

					if (npc.ai[3] >= (CalamityWorld.death ? 540f : 600f))
					{
						npc.ai[2] += 1f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}

					if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 300f)
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y = npc.velocity.Y * 0.96f;

						npc.velocity.Y = npc.velocity.Y - 0.1f;

						if (npc.velocity.Y > 3f)
							npc.velocity.Y = 3f;
					}
					else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 230f)
					{
						if (npc.velocity.Y < 0f)
							npc.velocity.Y = npc.velocity.Y * 0.96f;

						npc.velocity.Y = npc.velocity.Y + 0.1f;

						if (npc.velocity.Y < -3f)
							npc.velocity.Y = -3f;
					}

					if (npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) + 250f)
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X = npc.velocity.X * 0.94f;

						npc.velocity.X = npc.velocity.X - 0.3f;

						if (npc.velocity.X > 9f)
							npc.velocity.X = 9f;
					}
					if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2))
					{
						if (npc.velocity.X < 0f)
							npc.velocity.X = npc.velocity.X * 0.94f;

						npc.velocity.X = npc.velocity.X + 0.2f;

						if (npc.velocity.X < -8f)
							npc.velocity.X = -8f;
					}

					Vector2 vector57 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num483 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector57.X;
					float num484 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector57.Y;
					float num485 = (float)Math.Sqrt((double)(num483 * num483 + num484 * num484));
					npc.rotation = (float)Math.Atan2((double)num484, (double)num483) + 1.57f;
					return false;
				}

				// Charge towards the player
				if (npc.ai[2] == 1f)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y = npc.velocity.Y * 0.9f;

					Vector2 vector58 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num486 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 280f * npc.ai[0] - vector58.X;
					float num487 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector58.Y;
					float num488 = (float)Math.Sqrt((double)(num486 * num486 + num487 * num487));
					npc.rotation = (float)Math.Atan2((double)num487, (double)num486) + 1.57f;

					npc.velocity.X = (npc.velocity.X * 5f + Main.npc[(int)npc.ai[1]].velocity.X) / 6f;
					npc.velocity.X = npc.velocity.X + 0.5f;

					npc.velocity.Y = npc.velocity.Y - 0.5f;
					if (npc.velocity.Y < -9f)
						npc.velocity.Y = -9f;

					if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 280f)
					{
						float chargeVelocity = 20f;
						if (!cannonAlive)
							chargeVelocity += 1.5f;
						if (!laserAlive)
							chargeVelocity += 1.5f;
						if (!sawAlive)
							chargeVelocity += 1.5f;

						npc.TargetClosest(true);
						npc.ai[2] = 2f;
						vector58 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						num486 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector58.X;
						num487 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector58.Y;
						num488 = (float)Math.Sqrt((double)(num486 * num486 + num487 * num487));
						num488 = chargeVelocity / num488;
						npc.velocity.X = num486 * num488;
						npc.velocity.Y = num487 * num488;
						npc.netUpdate = true;
					}
				}

				// Charge 4 times (more if arms are dead)
				else if (npc.ai[2] == 2f)
				{
					if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f)
					{
						float chargeAmt = 4f;
						if (!cannonAlive)
							chargeAmt += 1f;
						if (!laserAlive)
							chargeAmt += 1f;
						if (!sawAlive)
							chargeAmt += 1f;

						if (npc.ai[3] >= chargeAmt)
						{
							// Return to head
							npc.ai[2] = 3f;
							npc.ai[3] = 0f;
							return false;
						}

						npc.ai[2] = 1f;
						npc.ai[3] += 1f;
					}
				}

				// Different type of charge
				else if (npc.ai[2] == 4f)
				{
					Vector2 vector59 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num489 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector59.X;
					float num490 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector59.Y;
					float num491 = (float)Math.Sqrt((double)(num489 * num489 + num490 * num490));
					npc.rotation = (float)Math.Atan2((double)num490, (double)num489) + 1.57f;

					npc.velocity.Y = (npc.velocity.Y * 5f + Main.npc[(int)npc.ai[1]].velocity.Y) / 6f;

					npc.velocity.X = npc.velocity.X + 0.5f;
					if (npc.velocity.X > 12f)
						npc.velocity.X = 12f;

					if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 500f || npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) + 500f)
					{
						float chargeVelocity = 17f;
						if (!cannonAlive)
							chargeVelocity += 1.5f;
						if (!laserAlive)
							chargeVelocity += 1.5f;
						if (!sawAlive)
							chargeVelocity += 1.5f;

						npc.TargetClosest(true);
						npc.ai[2] = 5f;
						vector59 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						num489 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector59.X;
						num490 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector59.Y;
						num491 = (float)Math.Sqrt((double)(num489 * num489 + num490 * num490));
						num491 = chargeVelocity / num491;
						npc.velocity.X = num489 * num491;
						npc.velocity.Y = num490 * num491;
						npc.netUpdate = true;
					}
				}

				// Charge 4 times (more if arms are dead)
				else if (npc.ai[2] == 5f && npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - 100f)
				{
					float chargeAmt = 4f;
					if (!cannonAlive)
						chargeAmt += 1f;
					if (!laserAlive)
						chargeAmt += 1f;
					if (!sawAlive)
						chargeAmt += 1f;

					if (npc.ai[3] >= chargeAmt)
					{
						// Return to head
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						return false;
					}

					npc.ai[2] = 4f;
					npc.ai[3] += 1f;
				}
			}
			return false;
		}

		public static bool BuffedPrimeSawAI(NPC npc, Mod mod)
		{
			Vector2 vector50 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num462 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector50.X;
			float num463 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector50.Y;
			float num464 = (float)Math.Sqrt((double)(num462 * num462 + num463 * num463));

			if (npc.ai[2] != 99f)
			{
				if (num464 > 800f)
					npc.ai[2] = 99f;
			}
			else if (num464 < 400f)
				npc.ai[2] = 0f;

			npc.spriteDirection = -(int)npc.ai[0];

			if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 32)
			{
				npc.ai[2] += 10f;
				if (npc.ai[2] > 50f || Main.netMode != 2)
				{
					npc.life = -1;
					npc.HitEffect(0, 10.0);
					npc.active = false;
				}
			}

			CalamityGlobalNPC.primeSaw = npc.whoAmI;

			// Check if arms are alive
			bool cannonAlive = false;
			bool laserAlive = false;
			bool viceAlive = false;
			if (CalamityGlobalNPC.primeCannon != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeCannon].active)
					cannonAlive = true;
			}
			if (CalamityGlobalNPC.primeLaser != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeLaser].active)
					laserAlive = true;
			}
			if (CalamityGlobalNPC.primeVice != -1)
			{
				if (Main.npc[CalamityGlobalNPC.primeVice].active)
					viceAlive = true;
			}

			if (npc.ai[2] == 99f)
			{
				if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y = npc.velocity.Y * 0.96f;

					npc.velocity.Y = npc.velocity.Y - 0.1f;

					if (npc.velocity.Y > 8f)
						npc.velocity.Y = 8f;
				}
				else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y = npc.velocity.Y * 0.96f;

					npc.velocity.Y = npc.velocity.Y + 0.1f;

					if (npc.velocity.Y < -8f)
						npc.velocity.Y = -8f;
				}

				if (npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2))
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X * 0.96f;

					npc.velocity.X = npc.velocity.X - 0.5f;

					if (npc.velocity.X > 12f)
						npc.velocity.X = 12f;
				}
				if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2))
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X * 0.96f;

					npc.velocity.X = npc.velocity.X + 0.5f;

					if (npc.velocity.X < -12f)
						npc.velocity.X = -12f;
				}
			}
			else
			{
				if (npc.ai[2] == 0f || npc.ai[2] == 3f)
				{
					if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
						npc.timeLeft = 10;

					// Start charging after 10 seconds (change this as each arm dies)
					npc.ai[3] += 1f;
					if (!cannonAlive)
						npc.ai[3] += 1f;
					if (!laserAlive)
						npc.ai[3] += 1f;
					if (!viceAlive)
						npc.ai[3] += 1f;

					if (npc.ai[3] >= (CalamityWorld.death ? 270f : 300f))
					{
						npc.ai[2] += 1f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}

					if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 320f)
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y = npc.velocity.Y * 0.96f;

						npc.velocity.Y = npc.velocity.Y - 0.04f;

						if (npc.velocity.Y > 3f)
							npc.velocity.Y = 3f;
					}
					else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 260f)
					{
						if (npc.velocity.Y < 0f)
							npc.velocity.Y = npc.velocity.Y * 0.96f;

						npc.velocity.Y = npc.velocity.Y + 0.04f;

						if (npc.velocity.Y < -3f)
							npc.velocity.Y = -3f;
					}

					if (npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2))
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X = npc.velocity.X * 0.96f;

						npc.velocity.X = npc.velocity.X - 0.3f;

						if (npc.velocity.X > 12f)
							npc.velocity.X = 12f;
					}
					if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 250f)
					{
						if (npc.velocity.X < 0f)
							npc.velocity.X = npc.velocity.X * 0.96f;

						npc.velocity.X = npc.velocity.X + 0.3f;

						if (npc.velocity.X < -12f)
							npc.velocity.X = -12f;
					}

					Vector2 vector52 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num468 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector52.X;
					float num469 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector52.Y;
					float num470 = (float)Math.Sqrt((double)(num468 * num468 + num469 * num469));
					npc.rotation = (float)Math.Atan2((double)num469, (double)num468) + 1.57f;
					return false;
				}

				if (npc.ai[2] == 1f)
				{
					Vector2 vector53 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num471 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector53.X;
					float num472 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector53.Y;
					float num473 = (float)Math.Sqrt((double)(num471 * num471 + num472 * num472));
					npc.rotation = (float)Math.Atan2((double)num472, (double)num471) + 1.57f;

					npc.velocity.X = npc.velocity.X * 0.95f;
					npc.velocity.Y = npc.velocity.Y - 0.1f;
					if (npc.velocity.Y < -8f)
						npc.velocity.Y = -8f;

					if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 200f)
					{
						float chargeVelocity = 22f;
						if (!cannonAlive)
							chargeVelocity += 1.5f;
						if (!laserAlive)
							chargeVelocity += 1.5f;
						if (!viceAlive)
							chargeVelocity += 1.5f;

						npc.TargetClosest(true);
						npc.ai[2] = 2f;
						vector53 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						num471 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector53.X;
						num472 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector53.Y;
						num473 = (float)Math.Sqrt((double)(num471 * num471 + num472 * num472));
						num473 = chargeVelocity / num473;
						npc.velocity.X = num471 * num473;
						npc.velocity.Y = num472 * num473;
						npc.netUpdate = true;
					}
				}

				else if (npc.ai[2] == 2f)
				{
					if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f)
						npc.ai[2] = 3f;
				}

				else
				{
					if (npc.ai[2] == 4f)
					{
						float chargeVelocity = 8f;
						if (!cannonAlive)
							chargeVelocity += 1.5f;
						if (!laserAlive)
							chargeVelocity += 1.5f;
						if (!viceAlive)
							chargeVelocity += 1.5f;

						npc.TargetClosest(true);
						Vector2 vector54 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num474 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector54.X;
						float num475 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector54.Y;
						float num476 = (float)Math.Sqrt((double)(num474 * num474 + num475 * num475));
						num476 = chargeVelocity / num476;
						num474 *= num476;
						num475 *= num476;

						if (npc.velocity.X > num474)
						{
							if (npc.velocity.X > 0f)
								npc.velocity.X = npc.velocity.X * 0.97f;
							npc.velocity.X = npc.velocity.X - 0.05f;
						}
						if (npc.velocity.X < num474)
						{
							if (npc.velocity.X < 0f)
								npc.velocity.X = npc.velocity.X * 0.97f;
							npc.velocity.X = npc.velocity.X + 0.05f;
						}
						if (npc.velocity.Y > num475)
						{
							if (npc.velocity.Y > 0f)
								npc.velocity.Y = npc.velocity.Y * 0.97f;
							npc.velocity.Y = npc.velocity.Y - 0.05f;
						}
						if (npc.velocity.Y < num475)
						{
							if (npc.velocity.Y < 0f)
								npc.velocity.Y = npc.velocity.Y * 0.97f;
							npc.velocity.Y = npc.velocity.Y + 0.05f;
						}

						npc.ai[3] += 1f;
						if (npc.ai[3] >= 600f)
						{
							npc.ai[2] = 0f;
							npc.ai[3] = 0f;
							npc.netUpdate = true;
						}

						vector54 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						num474 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector54.X;
						num475 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector54.Y;
						num476 = (float)Math.Sqrt((double)(num474 * num474 + num475 * num475));
						npc.rotation = (float)Math.Atan2((double)num475, (double)num474) + 1.57f;
						return false;
					}

					if (npc.ai[2] == 5f && ((npc.velocity.X > 0f && npc.position.X + (float)(npc.width / 2) > Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2)) || (npc.velocity.X < 0f && npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))))
						npc.ai[2] = 0f;
				}
			}
			return false;
		}
		#endregion

		#region Buffed Mothron AI
		public static bool BuffedMothronAI(NPC npc)
		{
			npc.noTileCollide = false;
			npc.noGravity = true;
			npc.knockBackResist = 0.2f * Main.expertKnockBack;
			npc.damage = npc.defDamage;

			if (!Main.eclipse)
				npc.ai[0] = -1f;
			else if (npc.target < 0 || Main.player[npc.target].dead || !Main.player[npc.target].active)
			{
				npc.TargetClosest(true);
				Vector2 vector235 = Main.player[npc.target].Center - npc.Center;
				if (Main.player[npc.target].dead || vector235.Length() > 3000f)
					npc.ai[0] = -1f;
			}
			else
			{
				Vector2 vector236 = Main.player[npc.target].Center - npc.Center;
				if (npc.ai[0] > 1f && vector236.Length() > 1000f)
					npc.ai[0] = 1f;
			}

			if (npc.ai[0] == -1f)
			{
				Vector2 value37 = new Vector2(0f, -8f);
				npc.velocity = (npc.velocity * 22f + value37) / 10f;
				npc.noTileCollide = true;
				npc.dontTakeDamage = true;
				return false;
			}

			if (npc.ai[0] == 0f)
			{
				npc.TargetClosest(true);

				if (npc.Center.X < Main.player[npc.target].Center.X - 2f)
					npc.direction = 1;
				if (npc.Center.X > Main.player[npc.target].Center.X + 2f)
					npc.direction = -1;

				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.025f) / 10f;

				if (npc.collideX)
				{
					npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);

					if (npc.velocity.X > 30f)
						npc.velocity.X = 30f;
					if (npc.velocity.X < -30f)
						npc.velocity.X = -30f;
				}
				if (npc.collideY)
				{
					npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);

					if (npc.velocity.Y > 30f)
						npc.velocity.Y = 30f;
					if (npc.velocity.Y < -30f)
						npc.velocity.Y = -30f;
				}

				Vector2 value38 = Main.player[npc.target].Center - npc.Center;
				value38.Y -= 200f;
				if (value38.Length() > 2000f)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
				}
				else if (value38.Length() > 80f)
				{
					float scaleFactor15 = 13f;
					float num1354 = 30f;
					value38.Normalize();
					value38 *= scaleFactor15;
					npc.velocity = (npc.velocity * (num1354 - 1f) + value38) / num1354;
				}
				else if (npc.velocity.Length() > 2f)
					npc.velocity *= 0.95f;
				else if (npc.velocity.Length() < 1f)
					npc.velocity *= 1.05f;

				npc.ai[1] += 1f;
				if (npc.justHit)
					npc.ai[1] += (float)Main.rand.Next(10, 30);

				if (npc.ai[1] >= 180f && Main.netMode != 1)
				{
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;

					while (npc.ai[0] == 0f)
					{
						int num1355 = Main.rand.Next(3);
						if (num1355 == 0 && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
							npc.ai[0] = 2f;
						else if (num1355 == 1)
							npc.ai[0] = 3f;
						else if (num1355 == 2 && NPC.CountNPCS(478) + NPC.CountNPCS(479) < 3)
							npc.ai[0] = 4f;
					}
				}
			}
			else
			{
				if (npc.ai[0] == 1f)
				{
					npc.collideX = false;
					npc.collideY = false;
					npc.noTileCollide = true;
					npc.knockBackResist = 0f;

					if (npc.target < 0 || !Main.player[npc.target].active || Main.player[npc.target].dead)
						npc.TargetClosest(true);

					if (npc.velocity.X < 0f)
						npc.direction = -1;
					else if (npc.velocity.X > 0f)
						npc.direction = 1;

					npc.spriteDirection = npc.direction;
					npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.02f) / 10f;

					Vector2 value39 = Main.player[npc.target].Center - npc.Center;
					if (value39.Length() < 300f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}

					float scaleFactor16 = 14f + value39.Length() / 100f;
					float num1356 = 25f;
					value39.Normalize();
					value39 *= scaleFactor16;
					npc.velocity = (npc.velocity * (num1356 - 1f) + value39) / num1356;
					return false;
				}

				if (npc.ai[0] == 2f)
				{
					npc.damage = (int)((double)npc.defDamage * 0.75);
					npc.knockBackResist = 0f;

					if (npc.target < 0 || !Main.player[npc.target].active || Main.player[npc.target].dead)
					{
						npc.TargetClosest(true);
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}

					if (Main.player[npc.target].Center.X - 10f < npc.Center.X)
						npc.direction = -1;
					else if (Main.player[npc.target].Center.X + 10f > npc.Center.X)
						npc.direction = 1;

					npc.spriteDirection = npc.direction;
					npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.025f) / 5f;

					if (npc.collideX)
					{
						npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);

						if (npc.velocity.X > 30f)
							npc.velocity.X = 30f;
						if (npc.velocity.X < -30f)
							npc.velocity.X = -30f;
					}
					if (npc.collideY)
					{
						npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);

						if (npc.velocity.Y > 30f)
							npc.velocity.Y = 30f;
						if (npc.velocity.Y < -30f)
							npc.velocity.Y = -30f;
					}

					Vector2 value40 = Main.player[npc.target].Center - npc.Center;
					value40.Y -= 20f;

					npc.ai[2] += 0.0222222228f;
					if (Main.expertMode)
						npc.ai[2] += 0.0166666675f;

					float scaleFactor17 = 13f + npc.ai[2] + value40.Length() / 120f;
					float num1357 = 20f;
					value40.Normalize();
					value40 *= scaleFactor17;
					npc.velocity = (npc.velocity * (num1357 - 1f) + value40) / num1357;

					npc.ai[1] += 1f;
					if (npc.ai[1] > 240f || !Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
				}
				else
				{
					if (npc.ai[0] == 3f)
					{
						npc.knockBackResist = 0f;
						npc.noTileCollide = true;

						if (npc.velocity.X < 0f)
							npc.direction = -1;
						else
							npc.direction = 1;

						npc.spriteDirection = npc.direction;
						npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.0175f) / 5f;

						Vector2 value41 = Main.player[npc.target].Center - npc.Center;
						value41.Y -= 12f;
						if (npc.Center.X > Main.player[npc.target].Center.X)
							value41.X += 400f;
						else
							value41.X -= 400f;

						if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 350f && Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) < 20f)
						{
							npc.ai[0] = 3.1f;
							npc.ai[1] = 0f;
						}

						npc.ai[1] += 0.0333333351f;
						float scaleFactor18 = 20f + npc.ai[1];
						float num1358 = 4f;
						value41.Normalize();
						value41 *= scaleFactor18;
						npc.velocity = (npc.velocity * (num1358 - 1f) + value41) / num1358;
						return false;
					}

					if (npc.ai[0] == 3.1f)
					{
						npc.knockBackResist = 0f;
						npc.noTileCollide = true;
						npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.0175f) / 5f;

						Vector2 vector237 = Main.player[npc.target].Center - npc.Center;
						vector237.Y -= 12f;
						float scaleFactor19 = 30f;
						float num1359 = 8f;
						vector237.Normalize();
						vector237 *= scaleFactor19;
						npc.velocity = (npc.velocity * (num1359 - 1f) + vector237) / num1359;

						if (npc.velocity.X < 0f)
							npc.direction = -1;
						else
							npc.direction = 1;

						npc.spriteDirection = npc.direction;

						npc.ai[1] += 1f;
						if (npc.ai[1] > 10f)
						{
							npc.velocity = vector237;

							if (npc.velocity.X < 0f)
								npc.direction = -1;
							else
								npc.direction = 1;

							npc.ai[0] = 3.2f;
							npc.ai[1] = 0f;
							npc.ai[1] = (float)npc.direction;
						}
					}
					else
					{
						if (npc.ai[0] == 3.2f)
						{
							npc.damage = (int)((double)npc.defDamage * 1.5);
							npc.collideX = false;
							npc.collideY = false;
							npc.knockBackResist = 0f;
							npc.noTileCollide = true;
							npc.ai[2] += 0.0333333351f;
							npc.velocity.X = (20f + npc.ai[2]) * npc.ai[1];

							if ((npc.ai[1] > 0f && npc.Center.X > Main.player[npc.target].Center.X + 260f) || (npc.ai[1] < 0f && npc.Center.X < Main.player[npc.target].Center.X - 260f))
							{
								if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
								{
									npc.ai[0] = 0f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;
								}
								else if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 1600f)
								{
									npc.ai[0] = 1f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;
								}
							}
							npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.0175f) / 5f;
							return false;
						}

						if (npc.ai[0] == 4f)
						{
							npc.ai[0] = 0f;
							npc.TargetClosest(true);

							if (Main.netMode != 1)
							{
								npc.ai[1] = -1f;
								npc.ai[2] = -1f;

								int num;
								for (int num1360 = 0; num1360 < 1000; num1360 = num + 1)
								{
									int num1361 = (int)Main.player[npc.target].Center.X / 16;
									int num1362 = (int)Main.player[npc.target].Center.Y / 16;
									int num1363 = 30 + num1360 / 50;
									int num1364 = 20 + num1360 / 75;

									num1361 += Main.rand.Next(-num1363, num1363 + 1);
									num1362 += Main.rand.Next(-num1364, num1364 + 1);

									if (!WorldGen.SolidTile(num1361, num1362))
									{
										while (!WorldGen.SolidTile(num1361, num1362) && (double)num1362 < Main.worldSurface)
											num1362++;

										if ((new Vector2((float)(num1361 * 16 + 8), (float)(num1362 * 16 + 8)) - Main.player[npc.target].Center).Length() < 2100f)
										{
											npc.ai[0] = 4.1f;
											npc.ai[1] = (float)num1361;
											npc.ai[2] = (float)num1362;
											break;
										}
									}
									num = num1360;
								}
							}
							npc.netUpdate = true;
							return false;
						}

						if (npc.ai[0] == 4.1f)
						{
							if (npc.velocity.X < -2f)
								npc.direction = -1;
							else if (npc.velocity.X > 2f)
								npc.direction = 1;

							npc.spriteDirection = npc.direction;
							npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.025f) / 10f;
							npc.noTileCollide = true;

							int num1365 = (int)npc.ai[1];
							int num1366 = (int)npc.ai[2];
							float x2 = (float)(num1365 * 16 + 8);
							float y2 = (float)(num1366 * 16 - 20);
							Vector2 vector238 = new Vector2(x2, y2);
							vector238 -= npc.Center;
							float num1367 = 6f + vector238.Length() / 150f;

							if (num1367 > 10f)
								num1367 = 10f;

							if (vector238.Length() < 10f)
								npc.ai[0] = 4.2f;

							vector238.Normalize();
							vector238 *= num1367;
							float num1368 = 10f;
							npc.velocity = (npc.velocity * (num1368 - 1f) + vector238) / num1368;
							return false;
						}

						if (npc.ai[0] == 4.2f)
						{
							npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.025f) / 10f;
							npc.knockBackResist = 0f;
							npc.noTileCollide = true;

							int num1369 = (int)npc.ai[1];
							int num1370 = (int)npc.ai[2];
							float x3 = (float)(num1369 * 16 + 8);
							float y3 = (float)(num1370 * 16 - 20);
							Vector2 vector239 = new Vector2(x3, y3);
							vector239 -= npc.Center;
							float num1371 = 4f;
							float num1372 = 2f;

							if (Main.netMode != 1 && vector239.Length() < 4f)
							{
								int num1373 = 70;
								if (Main.expertMode)
									num1373 = (int)((double)num1373 * 0.75);

								npc.ai[3] += 1f;

								if (npc.ai[3] == (float)num1373)
									NPC.NewNPC(num1369 * 16 + 8, num1370 * 16, 478, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
								else if (npc.ai[3] == (float)(num1373 * 2))
								{
									npc.ai[0] = 0f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;

									if (NPC.CountNPCS(478) + NPC.CountNPCS(479) < 3 && Main.rand.Next(3) != 0)
										npc.ai[0] = 4f;
									else if (Collision.SolidCollision(npc.position, npc.width, npc.height))
										npc.ai[0] = 1f;
								}
							}

							if (vector239.Length() > num1371)
							{
								vector239.Normalize();
								vector239 *= num1371;
							}
							npc.velocity = (npc.velocity * (num1372 - 1f) + vector239) / num1372;
						}
					}
				}
			}
			return false;
		}
		#endregion

		#region Buffed Pumpking and Pumpking Blade AI
		public static bool BuffedPumpkingAI(NPC npc)
		{
			npc.localAI[0] += 1f;
			if (npc.localAI[0] > 6f)
			{
				npc.localAI[0] = 0f;
				npc.localAI[1] += 1f;

				if (npc.localAI[1] > 4f)
					npc.localAI[1] = 0f;
			}

			if (Main.netMode != 1)
			{
				npc.localAI[2] += 1f;
				if (npc.localAI[2] > 300f)
				{
					npc.ai[3] = (float)Main.rand.Next(3);
					npc.localAI[2] = 0f;
				}
				else if (npc.ai[3] == 0f && npc.localAI[2] % 30f == 0f && npc.localAI[2] > 30f)
				{
					float num856 = 10f;
					Vector2 vector109 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f + 30f);
					if (!WorldGen.SolidTile((int)vector109.X / 16, (int)vector109.Y / 16))
					{
						float num857 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector109.X;
						float num858 = Main.player[npc.target].position.Y - vector109.Y;
						num857 += (float)Main.rand.Next(-50, 51);
						num858 += (float)Main.rand.Next(50, 201);
						num858 *= 0.2f;
						float num859 = (float)Math.Sqrt((double)(num857 * num857 + num858 * num858));
						num859 = num856 / num859;
						num857 *= num859;
						num858 *= num859;
						num857 *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
						num858 *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
						Projectile.NewProjectile(vector109.X, vector109.Y, num857, num858, Main.rand.Next(326, 329), 60, 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}

			if (npc.ai[0] == 0f && Main.netMode != 1)
			{
				npc.TargetClosest(true);
				npc.ai[0] = 1f;

				int num861 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, 328, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				Main.npc[num861].ai[0] = -1f;
				Main.npc[num861].ai[1] = (float)npc.whoAmI;
				Main.npc[num861].target = npc.target;
				Main.npc[num861].netUpdate = true;

				num861 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, 328, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				Main.npc[num861].ai[0] = 1f;
				Main.npc[num861].ai[1] = (float)npc.whoAmI;
				Main.npc[num861].ai[3] = 150f;
				Main.npc[num861].target = npc.target;
				Main.npc[num861].netUpdate = true;
			}

			if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 2000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 2000f)
			{
				npc.TargetClosest(true);

				if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 2000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 2000f)
					npc.ai[1] = 2f;
			}

			if (Main.dayTime)
			{
				npc.velocity.Y = npc.velocity.Y + 0.3f;
				npc.velocity.X = npc.velocity.X * 0.9f;
			}
			else if (npc.ai[1] == 0f)
			{
				npc.ai[2] += 1f;
				if (npc.ai[2] >= 300f)
				{
					if (npc.ai[3] != 1f)
					{
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
					else
					{
						npc.ai[2] = 0f;
						npc.ai[1] = 1f;
						npc.TargetClosest(true);
						npc.netUpdate = true;
					}
				}

				Vector2 vector110 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num862 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector110.X;
				float num863 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 200f - vector110.Y;
				float num864 = (float)Math.Sqrt((double)(num862 * num862 + num863 * num863));
				float num865 = 8f;

				if (npc.ai[3] == 1f)
				{
					if (num864 > 900f)
						num865 = 14f;
					else if (num864 > 600f)
						num865 = 12f;
					else if (num864 > 300f)
						num865 = 10f;
				}

				if (num864 > 50f)
				{
					num864 = num865 / num864;
					npc.velocity.X = (npc.velocity.X * 14f + num862 * num864) / 15f;
					npc.velocity.Y = (npc.velocity.Y * 14f + num863 * num864) / 15f;
				}
			}
			else if (npc.ai[1] == 1f)
			{
				npc.ai[2] += 1f;
				if (npc.ai[2] >= 600f || npc.ai[3] != 1f)
				{
					npc.ai[2] = 0f;
					npc.ai[1] = 0f;
				}

				Vector2 vector111 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num866 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector111.X;
				float num867 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector111.Y;
				float num868 = (float)Math.Sqrt((double)(num866 * num866 + num867 * num867));
				num868 = 20f / num868;

				npc.velocity.X = (npc.velocity.X * 49f + num866 * num868) / 50f;
				npc.velocity.Y = (npc.velocity.Y * 49f + num867 * num868) / 50f;
			}
			else if (npc.ai[1] == 2f)
			{
				npc.ai[1] = 3f;
				npc.velocity.Y = npc.velocity.Y + 0.1f;

				if (npc.velocity.Y < 0f)
					npc.velocity.Y = npc.velocity.Y * 0.95f;

				npc.velocity.X = npc.velocity.X * 0.95f;

				if (npc.timeLeft > 500)
					npc.timeLeft = 500;
			}
			npc.rotation = npc.velocity.X * -0.02f;
			return false;
		}

		public static bool BuffedPumpkingBladeAI(NPC npc)
		{
			npc.spriteDirection = -(int)npc.ai[0];

			if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 58)
			{
				npc.ai[2] += 10f;
				if (npc.ai[2] > 50f || Main.netMode != 2)
				{
					npc.life = -1;
					npc.HitEffect(0, 10.0);
					npc.active = false;
				}
			}

			if (Main.netMode != 1 && Main.npc[(int)npc.ai[1]].ai[3] == 2f)
			{
				npc.localAI[1] += 1f;
				if (npc.localAI[1] > 30f)
				{
					npc.localAI[1] = 0f;

					float num869 = 0.01f;
					Vector2 vector112 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f + 30f);
					float num870 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector112.X;
					float num871 = Main.player[npc.target].position.Y - vector112.Y;
					float num872 = (float)Math.Sqrt((double)(num870 * num870 + num871 * num871));

					num872 = num869 / num872;
					num870 *= num872;
					num871 *= num872;

					Projectile.NewProjectile(npc.Center.X, npc.Center.Y, num870, num871, 329, 70, 0f, Main.myPlayer, npc.rotation, (float)npc.spriteDirection);
				}
			}

			if (Main.dayTime)
			{
				npc.velocity.Y = npc.velocity.Y + 0.3f;
				npc.velocity.X = npc.velocity.X * 0.9f;
				return false;
			}

			if (npc.ai[2] == 0f || npc.ai[2] == 3f)
			{
				if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
					npc.timeLeft = 10;

				npc.ai[3] += 1f;
				if (npc.ai[3] >= 180f)
				{
					npc.ai[2] += 1f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}

				Vector2 vector113 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num874 = (Main.player[npc.target].Center.X + Main.npc[(int)npc.ai[1]].Center.X) / 2f;
				float num875 = (Main.player[npc.target].Center.Y + Main.npc[(int)npc.ai[1]].Center.Y) / 2f;
				num874 += -170f * npc.ai[0] - vector113.X;
				num875 += 90f - vector113.Y;

				float num876 = Math.Abs(Main.player[npc.target].Center.X - Main.npc[(int)npc.ai[1]].Center.X) + Math.Abs(Main.player[npc.target].Center.Y - Main.npc[(int)npc.ai[1]].Center.Y);
				if (num876 > 700f)
				{
					num874 = Main.npc[(int)npc.ai[1]].Center.X - 170f * npc.ai[0] - vector113.X;
					num875 = Main.npc[(int)npc.ai[1]].Center.Y + 90f - vector113.Y;
				}

				float num877 = (float)Math.Sqrt((double)(num874 * num874 + num875 * num875));
				float num878 = 8f;
				if (num877 > 1000f)
					num878 = 23f;
				else if (num877 > 800f)
					num878 = 20f;
				else if (num877 > 600f)
					num878 = 17f;
				else if (num877 > 400f)
					num878 = 14f;
				else if (num877 > 200f)
					num878 = 11f;

				if (npc.ai[0] < 0f && npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X)
					num874 -= 4f;
				if (npc.ai[0] > 0f && npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X)
					num874 += 4f;

				num877 = num878 / num877;
				npc.velocity.X = (npc.velocity.X * 14f + num874 * num877) / 15f;
				npc.velocity.Y = (npc.velocity.Y * 14f + num875 * num877) / 15f;
				num877 = (float)Math.Sqrt((double)(num874 * num874 + num875 * num875));

				if (num877 > 20f)
					npc.rotation = (float)Math.Atan2((double)num875, (double)num874) + 1.57f;
			}
			else if (npc.ai[2] == 1f)
			{
				Vector2 vector114 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num879 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector114.X;
				float num880 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector114.Y;
				float num881 = (float)Math.Sqrt((double)(num879 * num879 + num880 * num880));

				npc.rotation = (float)Math.Atan2((double)num880, (double)num879) + 1.57f;
				npc.velocity.X = npc.velocity.X * 0.95f;
				npc.velocity.Y = npc.velocity.Y - 0.3f;

				if (npc.velocity.Y < -18f)
					npc.velocity.Y = -18f;

				if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 200f)
				{
					npc.TargetClosest(true);
					npc.ai[2] = 2f;

					vector114 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num879 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector114.X;
					num880 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector114.Y;
					num881 = (float)Math.Sqrt((double)(num879 * num879 + num880 * num880));
					num881 = 24f / num881;

					npc.velocity.X = num879 * num881;
					npc.velocity.Y = num880 * num881;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[2] == 2f)
			{
				float num882 = Math.Abs(npc.Center.X - Main.npc[(int)npc.ai[1]].Center.X) + Math.Abs(npc.Center.Y - Main.npc[(int)npc.ai[1]].Center.Y);

				if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f || num882 > 800f)
					npc.ai[2] = 3f;
			}
			else if (npc.ai[2] == 4f)
			{
				Vector2 vector115 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num883 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector115.X;
				float num884 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector115.Y;
				float num885 = (float)Math.Sqrt((double)(num883 * num883 + num884 * num884));

				npc.rotation = (float)Math.Atan2((double)num884, (double)num883) + 1.57f;
				npc.velocity.Y = npc.velocity.Y * 0.95f;
				npc.velocity.X = npc.velocity.X + 0.3f * -npc.ai[0];

				if (npc.velocity.X < -18f)
					npc.velocity.X = -18f;
				if (npc.velocity.X > 18f)
					npc.velocity.X = 18f;

				if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 500f || npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) + 500f)
				{
					npc.TargetClosest(true);
					npc.ai[2] = 5f;

					vector115 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num883 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector115.X;
					num884 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector115.Y;
					num885 = (float)Math.Sqrt((double)(num883 * num883 + num884 * num884));
					num885 = 17f / num885;

					npc.velocity.X = num883 * num885;
					npc.velocity.Y = num884 * num885;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[2] == 5f)
			{
				float num886 = Math.Abs(npc.Center.X - Main.npc[(int)npc.ai[1]].Center.X) + Math.Abs(npc.Center.Y - Main.npc[(int)npc.ai[1]].Center.Y);

				if ((npc.velocity.X > 0f && npc.position.X + (float)(npc.width / 2) > Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2)) || (npc.velocity.X < 0f && npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2)) || num886 > 800f)
					npc.ai[2] = 0f;
			}
			return false;
		}
		#endregion

		#region Buffed Ice Queen AI
		public static bool BuffedIceQueenAI(NPC npc)
		{
			if (Main.dayTime)
			{
				if (npc.velocity.X > 0f)
					npc.velocity.X = npc.velocity.X + 0.25f;
				else
					npc.velocity.X = npc.velocity.X - 0.25f;

				npc.velocity.Y = npc.velocity.Y - 0.1f;
				npc.rotation = npc.velocity.X * 0.05f;
			}
			else if (npc.ai[0] == 0f)
			{
				if (npc.ai[2] == 0f)
				{
					npc.TargetClosest(true);

					if (npc.Center.X < Main.player[npc.target].Center.X)
						npc.ai[2] = 1f;
					else
						npc.ai[2] = -1f;
				}

				npc.TargetClosest(true);
				int num887 = 800;
				float num888 = Math.Abs(npc.Center.X - Main.player[npc.target].Center.X);

				if (npc.Center.X < Main.player[npc.target].Center.X && npc.ai[2] < 0f && num888 > (float)num887)
					npc.ai[2] = 0f;
				if (npc.Center.X > Main.player[npc.target].Center.X && npc.ai[2] > 0f && num888 > (float)num887)
					npc.ai[2] = 0f;

				float num889 = 0.6f;
				float num890 = 10f;
				if ((double)npc.life < (double)npc.lifeMax * 0.75)
				{
					num889 = 0.7f;
					num890 = 12f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.5)
				{
					num889 = 0.8f;
					num890 = 14f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.25)
				{
					num889 = 0.95f;
					num890 = 16f;
				}

				npc.velocity.X = npc.velocity.X + npc.ai[2] * num889;
				if (npc.velocity.X > num890)
					npc.velocity.X = num890;
				if (npc.velocity.X < -num890)
					npc.velocity.X = -num890;

				float num891 = Main.player[npc.target].position.Y - (npc.position.Y + (float)npc.height);
				if (num891 < 150f)
					npc.velocity.Y = npc.velocity.Y - 0.2f;
				if (num891 > 200f)
					npc.velocity.Y = npc.velocity.Y + 0.2f;
				if (npc.velocity.Y > 9f)
					npc.velocity.Y = 9f;
				if (npc.velocity.Y < -9f)
					npc.velocity.Y = -9f;

				npc.rotation = npc.velocity.X * 0.05f;

				if ((num888 < 500f || npc.ai[3] < 0f) && npc.position.Y < Main.player[npc.target].position.Y)
				{
					npc.ai[3] += 1f;
					int num892 = 8;
					if ((double)npc.life < (double)npc.lifeMax * 0.75)
						num892 = 7;
					if ((double)npc.life < (double)npc.lifeMax * 0.5)
						num892 = 6;
					if ((double)npc.life < (double)npc.lifeMax * 0.25)
						num892 = 5;

					num892++;
					if (npc.ai[3] > (float)num892)
						npc.ai[3] = (float)(-(float)num892);

					if (npc.ai[3] == 0f && Main.netMode != 1)
					{
						Vector2 vector116 = new Vector2(npc.Center.X, npc.Center.Y);
						vector116.X += npc.velocity.X * 7f;
						float num893 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector116.X;
						float num894 = Main.player[npc.target].Center.Y - vector116.Y;
						float num895 = (float)Math.Sqrt((double)(num893 * num893 + num894 * num894));

						float num896 = 8f;
						if ((double)npc.life < (double)npc.lifeMax * 0.75)
							num896 = 9f;
						if ((double)npc.life < (double)npc.lifeMax * 0.5)
							num896 = 10f;
						if ((double)npc.life < (double)npc.lifeMax * 0.25)
							num896 = 11f;

						num895 = num896 / num895;
						num893 *= num895;
						num894 *= num895;

						Projectile.NewProjectile(vector116.X, vector116.Y, num893, num894, 348, 50, 0f, Main.myPlayer, 0f, 0f);
					}
				}
				else if (npc.ai[3] < 0f)
					npc.ai[3] += 1f;

				if (Main.netMode != 1)
				{
					npc.ai[1] += (float)Main.rand.Next(1, 4);

					if (npc.ai[1] > 600f && num888 < 600f)
						npc.ai[0] = -1f;
				}
			}
			else if (npc.ai[0] == 1f)
			{
				npc.TargetClosest(true);

				float num898 = 0.2f;
				float num899 = 10f;
				if ((double)npc.life < (double)npc.lifeMax * 0.75)
				{
					num898 = 0.24f;
					num899 = 12f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.5)
				{
					num898 = 0.28f;
					num899 = 14f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.25)
				{
					num898 = 0.32f;
					num899 = 16f;
				}
				num898 -= 0.05f;
				num899 -= 1f;

				if (npc.Center.X < Main.player[npc.target].Center.X)
				{
					npc.velocity.X = npc.velocity.X + num898;
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X * 0.98f;
				}
				if (npc.Center.X > Main.player[npc.target].Center.X)
				{
					npc.velocity.X = npc.velocity.X - num898;
					if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X * 0.98f;
				}
				if (npc.velocity.X > num899 || npc.velocity.X < -num899)
					npc.velocity.X = npc.velocity.X * 0.95f;

				float num900 = Main.player[npc.target].position.Y - (npc.position.Y + (float)npc.height);
				if (num900 < 180f)
					npc.velocity.Y = npc.velocity.Y - 0.1f;
				if (num900 > 200f)
					npc.velocity.Y = npc.velocity.Y + 0.1f;

				if (npc.velocity.Y > 7f)
					npc.velocity.Y = 7f;
				if (npc.velocity.Y < -7f)
					npc.velocity.Y = -7f;

				npc.rotation = npc.velocity.X * 0.01f;

				if (Main.netMode != 1)
				{
					npc.ai[3] += 1f;
					int num901 = 10;
					if ((double)npc.life < (double)npc.lifeMax * 0.75)
						num901 = 8;
					if ((double)npc.life < (double)npc.lifeMax * 0.5)
						num901 = 6;
					if ((double)npc.life < (double)npc.lifeMax * 0.25)
						num901 = 4;
					if ((double)npc.life < (double)npc.lifeMax * 0.1)
						num901 = 2;

					num901 += 3;
					if (npc.ai[3] >= (float)num901)
					{
						npc.ai[3] = 0f;
						Vector2 vector117 = new Vector2(npc.Center.X, npc.position.Y + (float)npc.height - 14f);
						int i2 = (int)(vector117.X / 16f);
						int j2 = (int)(vector117.Y / 16f);
						if (!WorldGen.SolidTile(i2, j2))
						{
							float num902 = npc.velocity.Y;

							if (num902 < 0f)
								num902 = 0f;

							num902 += 3f;
							float speedX2 = npc.velocity.X * 0.25f;
							Projectile.NewProjectile(vector117.X, vector117.Y, speedX2, num902, 349, 44, 0f, Main.myPlayer, (float)Main.rand.Next(5), 0f);
						}
					}
				}

				if (Main.netMode != 1)
				{
					npc.ai[1] += (float)Main.rand.Next(1, 4);

					if (npc.ai[1] > 450f)
						npc.ai[0] = -1f;
				}
			}
			else if (npc.ai[0] == 2f)
			{
				npc.TargetClosest(true);

				Vector2 vector118 = new Vector2(npc.Center.X, npc.Center.Y - 20f);
				float num904 = (float)Main.rand.Next(-1000, 1001);
				float num905 = (float)Main.rand.Next(-1000, 1001);
				float num906 = (float)Math.Sqrt((double)(num904 * num904 + num905 * num905));
				float num907 = 20f;

				npc.velocity *= 0.95f;
				num906 = num907 / num906;
				num904 *= num906;
				num905 *= num906;
				npc.rotation += 0.2f;
				vector118.X += num904 * 4f;
				vector118.Y += num905 * 4f;

				npc.ai[3] += 1f;
				int num908 = 7;
				if ((double)npc.life < (double)npc.lifeMax * 0.75)
					num908--;
				if ((double)npc.life < (double)npc.lifeMax * 0.5)
					num908 -= 2;
				if ((double)npc.life < (double)npc.lifeMax * 0.25)
					num908 -= 3;
				if ((double)npc.life < (double)npc.lifeMax * 0.1)
					num908 -= 4;

				if (npc.ai[3] > (float)num908)
				{
					npc.ai[3] = 0f;
					int num909 = Projectile.NewProjectile(vector118.X, vector118.Y, num904, num905, 349, 40, 0f, Main.myPlayer, 0f, 0f);
				}

				if (Main.netMode != 1)
				{
					npc.ai[1] += (float)Main.rand.Next(1, 4);

					if (npc.ai[1] > 300f)
						npc.ai[0] = -1f;
				}
			}
			if (npc.ai[0] == -1f)
			{
				int num910 = Main.rand.Next(3);
				npc.TargetClosest(true);

				if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 1000f)
					num910 = 0;

				npc.ai[0] = (float)num910;
				npc.ai[1] = 0f;
				npc.ai[2] = 0f;
				npc.ai[3] = 0f;
			}
			return false;
		}
		#endregion

		#region Buffed Eye of Cthulhu AI
		public static bool BuffedEyeofCthulhuAI(NPC npc, bool enraged)
		{
			bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;
			float num5 = 20f;

			double getMad = 0.5; //0.5
			bool flag2 = false;
			if ((double)npc.life < (double)npc.lifeMax * getMad)
				flag2 = true;

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			bool dead = Main.player[npc.target].dead;
			float num6 = npc.position.X + (float)(npc.width / 2) - Main.player[npc.target].position.X - (float)(Main.player[npc.target].width / 2);
			float num7 = npc.position.Y + (float)npc.height - 59f - Main.player[npc.target].position.Y - (float)(Main.player[npc.target].height / 2);
			float num8 = (float)Math.Atan2((double)num7, (double)num6) + 1.57f;

			if (num8 < 0f)
				num8 += 6.283f;
			else if ((double)num8 > 6.283)
				num8 -= 6.283f;

			float num9 = 0f;
			if (npc.ai[0] == 0f && npc.ai[1] == 0f)
				num9 = 0.02f;
			if (npc.ai[0] == 0f && npc.ai[1] == 2f && npc.ai[2] > 40f)
				num9 = 0.05f;
			if (npc.ai[0] == 3f && npc.ai[1] == 0f)
				num9 = 0.05f;
			if (npc.ai[0] == 3f && npc.ai[1] == 2f && npc.ai[2] > 40f)
				num9 = 0.08f;
			if (npc.ai[0] == 3f && npc.ai[1] == 4f && npc.ai[2] > num5)
				num9 = 0.15f;
			if (npc.ai[0] == 3f && npc.ai[1] == 5f)
				num9 = 0.05f;
			num9 *= 1.5f;

			if (npc.rotation < num8)
			{
				if ((double)(num8 - npc.rotation) > 3.1415)
					npc.rotation -= num9;
				else
					npc.rotation += num9;
			}
			else if (npc.rotation > num8)
			{
				if ((double)(npc.rotation - num8) > 3.1415)
					npc.rotation += num9;
				else
					npc.rotation -= num9;
			}

			if (npc.rotation > num8 - num9 && npc.rotation < num8 + num9)
				npc.rotation = num8;
			if (npc.rotation < 0f)
				npc.rotation += 6.283f;
			else if ((double)npc.rotation > 6.283)
				npc.rotation -= 6.283f;
			if (npc.rotation > num8 - num9 && npc.rotation < num8 + num9)
				npc.rotation = num8;

			if (Main.rand.Next(5) == 0)
			{
				int num10 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default(Color), 1f);
				Dust var_9_825_cp_0_cp_0 = Main.dust[num10];
				var_9_825_cp_0_cp_0.velocity.X = var_9_825_cp_0_cp_0.velocity.X * 0.5f;
				Dust var_9_845_cp_0_cp_0 = Main.dust[num10];
				var_9_845_cp_0_cp_0.velocity.Y = var_9_845_cp_0_cp_0.velocity.Y * 0.1f;
			}

			if (Main.dayTime | dead)
			{
				npc.velocity.Y = npc.velocity.Y - 0.04f;

				if (npc.timeLeft > 10)
					npc.timeLeft = 10;
			}
			else if (npc.ai[0] == 0f)
			{
				if (npc.ai[1] == 0f)
				{
					float num11 = ((enraged || configBossRushBoost) ? 10f : 7f);
					float num12 = ((enraged || configBossRushBoost) ? 0.2f : 0.15f);

					Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num13 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector.X;
					float num14 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 200f - vector.Y;
					float num15 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
					float num16 = num15;

					num15 = num11 / num15;
					num13 *= num15;
					num14 *= num15;

					if (npc.velocity.X < num13)
					{
						npc.velocity.X = npc.velocity.X + num12;
						if (npc.velocity.X < 0f && num13 > 0f)
							npc.velocity.X = npc.velocity.X + num12;
					}
					else if (npc.velocity.X > num13)
					{
						npc.velocity.X = npc.velocity.X - num12;
						if (npc.velocity.X > 0f && num13 < 0f)
							npc.velocity.X = npc.velocity.X - num12;
					}
					if (npc.velocity.Y < num14)
					{
						npc.velocity.Y = npc.velocity.Y + num12;
						if (npc.velocity.Y < 0f && num14 > 0f)
							npc.velocity.Y = npc.velocity.Y + num12;
					}
					else if (npc.velocity.Y > num14)
					{
						npc.velocity.Y = npc.velocity.Y - num12;
						if (npc.velocity.Y > 0f && num14 < 0f)
							npc.velocity.Y = npc.velocity.Y - num12;
					}

					npc.ai[2] += 1f;
					float num17 = 180f;
					if (npc.ai[2] >= num17)
					{
						npc.ai[1] = 1f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.target = 255;
						npc.netUpdate = true;
					}
					else if (num16 < 500f)
					{
						if (!Main.player[npc.target].dead)
							npc.ai[3] += 1f;

						if (npc.ai[3] >= 44f)
						{
							npc.ai[3] = 0f;
							npc.rotation = num8;

							float num19 = 5f;
							float num20 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector.X;
							float num21 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector.Y;
							float num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));

							num22 = num19 / num22;
							Vector2 vector2 = vector;
							Vector2 vector3;
							vector3.X = num20 * num22;
							vector3.Y = num21 * num22;
							vector2.X += vector3.X * 10f;
							vector2.Y += vector3.Y * 10f;

							if (Main.netMode != 1)
							{
								int num23 = NPC.NewNPC((int)vector2.X, (int)vector2.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
								Main.npc[num23].velocity.X = vector3.X;
								Main.npc[num23].velocity.Y = vector3.Y;

								if (Main.netMode == 2 && num23 < 200)
									NetMessage.SendData(23, -1, -1, null, num23, 0f, 0f, 0f, 0, 0, 0);
							}

							Main.PlaySound(3, (int)vector2.X, (int)vector2.Y, 1, 1f, 0f);

							int num;
							for (int m = 0; m < 10; m = num + 1)
							{
								Dust.NewDust(vector2, 20, 20, 5, vector3.X * 0.4f, vector3.Y * 0.4f, 0, default(Color), 1f);
								num = m;
							}
						}
					}
				}
				else if (npc.ai[1] == 1f)
				{
					npc.rotation = num8;
					float num24 = ((enraged || configBossRushBoost) ? 11f : 7.25f);

					Vector2 vector4 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num25 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector4.X;
					float num26 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector4.Y;
					float num27 = (float)Math.Sqrt((double)(num25 * num25 + num26 * num26));

					num27 = num24 / num27;
					npc.velocity.X = num25 * num27;
					npc.velocity.Y = num26 * num27;

					npc.ai[1] = 2f;
					npc.netUpdate = true;

					if (npc.netSpam > 10)
						npc.netSpam = 10;
				}
				else if (npc.ai[1] == 2f)
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 40f)
					{
						npc.velocity *= 0.975f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
							npc.velocity.X = 0f;
						if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
							npc.velocity.Y = 0f;
					}
					else
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

					int num28 = 90;
					if (npc.ai[2] >= (float)num28)
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = num8;

						if (npc.ai[3] >= 3f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
						}
						else
							npc.ai[1] = 1f;
					}
				}

				float num29 = 0.9f;
				if (((float)npc.life < (float)npc.lifeMax * num29) || CalamityWorld.death)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;

					if (npc.netSpam > 10)
						npc.netSpam = 10;
				}
			}
			else if (npc.ai[0] == 1f || npc.ai[0] == 2f)
			{
				if (npc.ai[0] == 1f)
				{
					npc.ai[2] += 0.005f;
					if ((double)npc.ai[2] > 0.5)
						npc.ai[2] = 0.5f;
				}
				else
				{
					npc.ai[2] -= 0.005f;
					if (npc.ai[2] < 0f)
						npc.ai[2] = 0f;
				}

				npc.rotation += npc.ai[2];
				npc.ai[1] += 1f;
				if (npc.ai[1] % 20f == 0f)
				{
					Vector2 vector5 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num31 = (float)Main.rand.Next(-200, 200);
					float num32 = (float)Main.rand.Next(-200, 200);
					float num33 = (float)Math.Sqrt((double)(num31 * num31 + num32 * num32));

					num33 = 5f / num33;
					Vector2 vector6 = vector5;
					Vector2 vector7;
					vector7.X = num31 * num33;
					vector7.Y = num32 * num33;
					vector6.X += vector7.X * 10f;
					vector6.Y += vector7.Y * 10f;

					if (Main.netMode != 1)
					{
						int num34 = NPC.NewNPC((int)vector6.X, (int)vector6.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[num34].velocity.X = vector7.X;
						Main.npc[num34].velocity.Y = vector7.Y;

						if (Main.netMode == 2 && num34 < 200)
							NetMessage.SendData(23, -1, -1, null, num34, 0f, 0f, 0f, 0, 0, 0);
					}

					int num;
					for (int n = 0; n < 10; n = num + 1)
					{
						Dust.NewDust(vector6, 20, 20, 5, vector7.X * 0.4f, vector7.Y * 0.4f, 0, default(Color), 1f);
						num = n;
					}
				}

				if (npc.ai[1] == 100f)
				{
					npc.ai[0] += 1f;
					npc.ai[1] = 0f;

					if (npc.ai[0] == 3f)
						npc.ai[2] = 0f;
					else
					{
						Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);

						int num;
						for (int num35 = 0; num35 < 2; num35 = num + 1)
						{
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 8, 1f);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7, 1f);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 6, 1f);
							num = num35;
						}

						for (int num36 = 0; num36 < 20; num36 = num + 1)
						{
							Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);
							num = num36;
						}

						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
					}
				}
				Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default(Color), 1f);
				npc.velocity.X = npc.velocity.X * 0.98f;
				npc.velocity.Y = npc.velocity.Y * 0.98f;

				if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
					npc.velocity.X = 0f;
				if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
					npc.velocity.Y = 0f;
			}
			else
			{
				if (flag2)
					npc.defense = -5;
				else
					npc.damage = (int)(20f * Main.expertDamage);

				if (npc.ai[1] == 0f & flag2)
					npc.ai[1] = 5f;

				if (npc.ai[1] == 0f)
				{
					float num37 = ((enraged || configBossRushBoost) ? 8f : 5.5f);
					float num38 = ((enraged || configBossRushBoost) ? 0.09f : 0.06f);

					Vector2 vector8 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num39 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector8.X;
					float num40 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 120f - vector8.Y;
					float num41 = (float)Math.Sqrt((double)(num39 * num39 + num40 * num40));

					if (num41 > 400f)
					{
						num37 += 1.25f;
						num38 += 0.075f;
						if (num41 > 600f)
						{
							num37 += 1.25f;
							num38 += 0.075f;
							if (num41 > 800f)
							{
								num37 += 1.25f;
								num38 += 0.075f;
							}
						}
					}

					num41 = num37 / num41;
					num39 *= num41;
					num40 *= num41;

					if (npc.velocity.X < num39)
					{
						npc.velocity.X = npc.velocity.X + num38;
						if (npc.velocity.X < 0f && num39 > 0f)
							npc.velocity.X = npc.velocity.X + num38;
					}
					else if (npc.velocity.X > num39)
					{
						npc.velocity.X = npc.velocity.X - num38;
						if (npc.velocity.X > 0f && num39 < 0f)
							npc.velocity.X = npc.velocity.X - num38;
					}
					if (npc.velocity.Y < num40)
					{
						npc.velocity.Y = npc.velocity.Y + num38;
						if (npc.velocity.Y < 0f && num40 > 0f)
							npc.velocity.Y = npc.velocity.Y + num38;
					}
					else if (npc.velocity.Y > num40)
					{
						npc.velocity.Y = npc.velocity.Y - num38;
						if (npc.velocity.Y > 0f && num40 < 0f)
							npc.velocity.Y = npc.velocity.Y - num38;
					}

					npc.ai[2] += 1f;
					if (npc.ai[2] >= 200f)
					{
						npc.ai[1] = 1f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;

						if ((double)npc.life < (double)npc.lifeMax * 0.4)
							npc.ai[1] = 3f;

						npc.target = 255;
						npc.netUpdate = true;
					}
				}
				else if (npc.ai[1] == 1f)
				{
					Main.PlaySound(36, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
					npc.rotation = num8;

					float num42 = ((enraged || configBossRushBoost) ? 9.5f : 6.2f);
					if (npc.ai[3] == 1f)
						num42 *= 1.15f;
					if (npc.ai[3] == 2f)
						num42 *= 1.3f;

					Vector2 vector9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num43 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector9.X;
					float num44 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector9.Y;
					float num45 = (float)Math.Sqrt((double)(num43 * num43 + num44 * num44));

					num45 = num42 / num45;
					npc.velocity.X = num43 * num45;
					npc.velocity.Y = num44 * num45;
					npc.ai[1] = 2f;
					npc.netUpdate = true;

					if (npc.netSpam > 10)
						npc.netSpam = 10;
				}
				else if (npc.ai[1] == 2f)
				{
					float num46 = CalamityWorld.death ? 70f : 60f;

					npc.ai[2] += 1f;
					if (npc.ai[2] >= num46)
					{
						npc.velocity *= 0.96f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
							npc.velocity.X = 0f;
						if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
							npc.velocity.Y = 0f;
					}
					else
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

					int num47 = CalamityWorld.death ? 70 : 80;
					if (npc.ai[2] >= (float)num47)
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = num8;

						if (npc.ai[3] >= 3f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
							if (Main.netMode != 1 && (double)npc.life < (double)npc.lifeMax * 0.6)
							{
								npc.ai[1] = 3f;
								npc.ai[3] += (float)Main.rand.Next(1, 4);
							}
							npc.netUpdate = true;

							if (npc.netSpam > 10)
								npc.netSpam = 10;
						}
						else
							npc.ai[1] = 1f;
					}
				}
				else if (npc.ai[1] == 3f)
				{
					if ((npc.ai[3] == 4f & flag2) && npc.Center.Y > Main.player[npc.target].Center.Y)
					{
						npc.TargetClosest(true);
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;

						if (npc.netSpam > 10)
							npc.netSpam = 10;
					}
					else if (Main.netMode != 1)
					{
						npc.TargetClosest(true);
						float num48 = ((enraged || configBossRushBoost) ? 26f : 18f);

						Vector2 vector10 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num49 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector10.X;
						float num50 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector10.Y;
						float num51 = Math.Abs(Main.player[npc.target].velocity.X) + Math.Abs(Main.player[npc.target].velocity.Y) / 4f;
						num51 += 10f - num51;

						if (num51 < 5f)
							num51 = 5f;
						if (num51 > 15f)
							num51 = 15f;

						if (npc.ai[2] == -1f)
						{
							num51 *= 4f;
							num48 *= 1.3f;
						}

						num49 -= Main.player[npc.target].velocity.X * num51;
						num50 -= Main.player[npc.target].velocity.Y * num51 / 4f;
						num49 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
						num50 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;

						float num52 = (float)Math.Sqrt((double)(num49 * num49 + num50 * num50));
						float num53 = num52;

						num52 = num48 / num52;
						npc.velocity.X = num49 * num52;
						npc.velocity.Y = num50 * num52;
						npc.velocity.X = npc.velocity.X + (float)Main.rand.Next(-20, 21) * 0.1f;
						npc.velocity.Y = npc.velocity.Y + (float)Main.rand.Next(-20, 21) * 0.1f;

						if (num53 < 100f)
						{
							if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
							{
								float num56 = Math.Abs(npc.velocity.X);
								float num57 = Math.Abs(npc.velocity.Y);

								if (npc.Center.X > Main.player[npc.target].Center.X)
									num57 *= -1f;
								if (npc.Center.Y > Main.player[npc.target].Center.Y)
									num56 *= -1f;

								npc.velocity.X = num57;
								npc.velocity.Y = num56;
							}
						}
						else if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
						{
							float num58 = (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) / 2f;
							float num59 = num58;

							if (npc.Center.X > Main.player[npc.target].Center.X)
								num59 *= -1f;
							if (npc.Center.Y > Main.player[npc.target].Center.Y)
								num58 *= -1f;

							npc.velocity.X = num59;
							npc.velocity.Y = num58;
						}
						npc.ai[1] = 4f;
						npc.netUpdate = true;

						if (npc.netSpam > 10)
							npc.netSpam = 10;
					}
				}
				else if (npc.ai[1] == 4f)
				{
					if (npc.ai[2] == 0f)
						Main.PlaySound(36, (int)npc.position.X, (int)npc.position.Y, -1, 1f, 0f);

					float num60 = num5;
					npc.ai[2] += 1f;

					if (npc.ai[2] == num60 && Vector2.Distance(npc.position, Main.player[npc.target].position) < 200f)
						npc.ai[2] -= 1f;

					if (npc.ai[2] >= num60)
					{
						npc.velocity *= 0.95f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
							npc.velocity.X = 0f;
						if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
							npc.velocity.Y = 0f;
					}
					else
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;

					float num61 = num60 + 13f;
					if (npc.ai[2] >= num61)
					{
						npc.netUpdate = true;

						if (npc.netSpam > 10)
							npc.netSpam = 10;

						npc.ai[3] += 1f;
						npc.ai[2] = 0f;

						if (npc.ai[3] >= 5f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
						}
						else
							npc.ai[1] = 3f;
					}
				}
				else if (npc.ai[1] == 5f)
				{
					float num62 = 600f;
					float num63 = ((enraged || configBossRushBoost) ? 12f : 8f);
					float num64 = ((enraged || configBossRushBoost) ? 0.4f : 0.25f);

					Vector2 vector11 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num65 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector11.X;
					float num66 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) + num62 - vector11.Y;
					float num67 = (float)Math.Sqrt((double)(num65 * num65 + num66 * num66));

					num67 = num63 / num67;
					num65 *= num67;
					num66 *= num67;

					if (npc.velocity.X < num65)
					{
						npc.velocity.X = npc.velocity.X + num64;
						if (npc.velocity.X < 0f && num65 > 0f)
							npc.velocity.X = npc.velocity.X + num64;
					}
					else if (npc.velocity.X > num65)
					{
						npc.velocity.X = npc.velocity.X - num64;
						if (npc.velocity.X > 0f && num65 < 0f)
							npc.velocity.X = npc.velocity.X - num64;
					}
					if (npc.velocity.Y < num66)
					{
						npc.velocity.Y = npc.velocity.Y + num64;
						if (npc.velocity.Y < 0f && num66 > 0f)
							npc.velocity.Y = npc.velocity.Y + num64;
					}
					else if (npc.velocity.Y > num66)
					{
						npc.velocity.Y = npc.velocity.Y - num64;
						if (npc.velocity.Y > 0f && num66 < 0f)
							npc.velocity.Y = npc.velocity.Y - num64;
					}

					npc.ai[2] += 1f;
					if (npc.ai[2] >= 70f)
					{
						npc.TargetClosest(true);
						npc.ai[1] = 3f;
						npc.ai[2] = -1f;
						npc.ai[3] = (float)Main.rand.Next(-3, 1);
						npc.netUpdate = true;
					}
				}
			}
			return false;
		}
		#endregion

		#region Revengeance Moon Lord AI
		public static void RevengeanceMoonLordFreeEyeAI(NPC npc)
		{
			// If there are less than 4 true eyes alive, become immune
			npc.dontTakeDamage = NPC.CountNPCS(npc.type) < 4;
		}

		public static void RevengeanceMoonLordCoreAI(NPC npc)
		{
			// Buffed teleport, teleports happen at a closer range
			if (npc.ai[0] >= 0f && npc.ai[0] < 2f && Main.netMode != 1 && npc.Distance(Main.player[npc.target].Center) > 1800f) //2400
			{
				npc.ai[0] = -2f;
				npc.netUpdate = true;
				Vector2 value8 = Main.player[npc.target].Center - Vector2.UnitY * 150f - npc.Center;
				npc.position += value8;

				if (Main.npc[(int)npc.localAI[0]].active)
				{
					NPC nPC6 = Main.npc[(int)npc.localAI[0]];
					nPC6.position += value8;
					Main.npc[(int)npc.localAI[0]].netUpdate = true;
				}
				if (Main.npc[(int)npc.localAI[1]].active)
				{
					NPC nPC6 = Main.npc[(int)npc.localAI[1]];
					nPC6.position += value8;
					Main.npc[(int)npc.localAI[1]].netUpdate = true;
				}
				if (Main.npc[(int)npc.localAI[2]].active)
				{
					NPC nPC6 = Main.npc[(int)npc.localAI[2]];
					nPC6.position += value8;
					Main.npc[(int)npc.localAI[2]].netUpdate = true;
				}

				int num;
				for (int num1176 = 0; num1176 < 200; num1176 = num + 1)
				{
					NPC nPC7 = Main.npc[num1176];
					if (nPC7.active && nPC7.type == NPCID.MoonLordFreeEye)
					{
						NPC nPC6 = nPC7;
						nPC6.position += value8;
						nPC7.netUpdate = true;
					}
					num = num1176;
				}
			}

			// If there are more than 3 true eyes alive, become immune
			if (npc.ai[0] == 1f)
				npc.dontTakeDamage = NPC.CountNPCS(NPCID.MoonLordFreeEye) > 3;
		}

		public static void RevengeanceMoonLordHandAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);

			// Checks if hands and head are 'dead'
			if (npc.ai[0] == -2f)
			{
				// Spawn a large group of true eyes
				if ((double)Main.npc[(int)npc.ai[3]].life <= (double)Main.npc[(int)npc.ai[3]].lifeMax * 0.75)
				{
					if (calamityGlobalNPC.newAI[2] < 90f)
						calamityGlobalNPC.newAI[2] += 1f;

					if ((int)calamityGlobalNPC.newAI[2] % 60 == 0)
					{
						npc.netUpdate = true;
						if (Main.netMode != 1)
						{
							int num = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, 400, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num].ai[3] = npc.ai[3];
							Main.npc[num].netUpdate = true;
						}
					}
				}
				if ((CalamityWorld.death || CalamityWorld.bossRushActive) && npc.type == NPCID.MoonLordHead)
				{
					// Spawn a large group of true eyes
					if ((double)Main.npc[(int)npc.ai[3]].life <= (double)Main.npc[(int)npc.ai[3]].lifeMax * 0.5)
					{
						if (calamityGlobalNPC.newAI[2] < 150f)
							calamityGlobalNPC.newAI[2] += 1f;

						if ((int)calamityGlobalNPC.newAI[2] % 60 == 0)
						{
							npc.netUpdate = true;
							if (Main.netMode != 1)
							{
								int num = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, 400, 0, 0f, 0f, 0f, 0f, 255);
								Main.npc[num].ai[3] = npc.ai[3];
								Main.npc[num].netUpdate = true;
							}
						}
					}
				}
			}
		}
		#endregion

		#region Revengeance Cultist AI
		public static void RevengeanceCultistAI(NPC npc, bool configBossRushBoost, Mod mod, bool enraged)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);

			// Force enraged vulnerability
			if (npc.buffImmune[mod.BuffType("Enraged")])
				npc.buffImmune[mod.BuffType("Enraged")] = false;

			if ((double)npc.life <= (double)npc.lifeMax * 0.5 || enraged || configBossRushBoost)
			{
				// Spawn Eidolists
				if (!NPC.AnyNPCs(mod.NPCType("Eidolist")))
				{
					if (calamityGlobalNPC.newAI[0] < 120f)
						calamityGlobalNPC.newAI[0] += 1f;

					if (calamityGlobalNPC.newAI[0] >= 120f)
					{
						calamityGlobalNPC.newAI[0] = 0f;
						npc.netUpdate = true;

						if (Main.netMode != 1)
							NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("Eidolist"), 0, 0f, 0f, 0f, 0f, 255);

						calamityGlobalNPC.newAI[0] = -360f;
					}
				}
			}
			else
			{
				// Fire projectiles from the sky
				if (calamityGlobalNPC.CultCountdown == 0)
				{
					if (CalamityWorld.death || CalamityWorld.bossRushActive)
						calamityGlobalNPC.CultCountdown = 75;
					else if ((double)npc.life <= (double)npc.lifeMax * 0.5)
						calamityGlobalNPC.CultCountdown = 150;
					else
						calamityGlobalNPC.CultCountdown = 200;
				}
				if (calamityGlobalNPC.CultCountdown > 0)
				{
					calamityGlobalNPC.CultCountdown--;
					if (calamityGlobalNPC.CultCountdown == 0)
					{
						if (Main.netMode != 1)
						{
							Player player2 = Main.player[npc.target];
							int speed2 = 8;
							float spawnX = Main.rand.Next(1000) - 500 + player2.Center.X;
							float spawnY = -1000 + player2.Center.Y;
							Vector2 baseSpawn = new Vector2(spawnX, spawnY);
							Vector2 baseVelocity = player2.Center - baseSpawn;
							baseVelocity.Normalize();
							baseVelocity = baseVelocity * speed2;
							int damage = 25; //100
							for (int i = 0; i < calamityGlobalNPC.CultProjectiles; i++)
							{
								Vector2 spawn2 = baseSpawn;
								spawn2.X = spawn2.X + i * 30 - (calamityGlobalNPC.CultProjectiles * 15);
								Vector2 velocity = baseVelocity;
								velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-calamityGlobalNPC.CultAngleSpread / 2 + (calamityGlobalNPC.CultAngleSpread * i / (float)calamityGlobalNPC.CultProjectiles)));
								velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
								int projectileType = Main.rand.Next(3);
								switch (projectileType)
								{
									case 0:
										projectileType = ProjectileID.CultistBossFireBall;
										break;
									case 1:
										projectileType = ProjectileID.FrostWave;
										break;
									case 2:
										projectileType = ProjectileID.AncientDoomProjectile;
										break;
									default:
										break;
								}
								int projectileI = Projectile.NewProjectile(spawn2.X, spawn2.Y, velocity.X, velocity.Y, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[projectileI].tileCollide = false;
							}
						}
					}
				}
			}
		}
		#endregion

		#region Revengeance Duke Fishron AI
		public static void RevengeanceDukeFishronAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);

			// Get the damage multiplier Fishron uses in vanilla
			float num = 0.6f * Main.damageMultiplier;

			// Check for phases
			if (npc.ai[0] > 9f)
			{
				// Increase damage and defense
				npc.damage = (int)((float)npc.defDamage * 1.1f * num);
				npc.defense = 38;

				// Fire cthulhunadoes
				calamityGlobalNPC.newAI[0] += 1f;
				if (calamityGlobalNPC.newAI[0] >= 600f)
				{
					calamityGlobalNPC.newAI[0] = 0f;

					if (Main.netMode != 1)
						Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, 385, 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));

					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] > 4f)
			{
				// Increase damage and defense
				npc.damage = (int)((float)npc.defDamage * 1.2f * num);
				npc.defense = (int)((float)npc.defDefense * 1.1f);
			}
			else
			{
				// Normalize damage and defense
				npc.damage = npc.defDamage;
				npc.defense = npc.defDefense;
			}

			// If the player isn't in the ocean biome or Fishron is transitioning between phases, become immune
			if (npc.ai[0] == -1f || npc.ai[0] == 4f || npc.ai[0] == 9f || (Main.player[npc.target].position.Y < 800f ||
				(double)Main.player[npc.target].position.Y > Main.worldSurface * 16.0 || (Main.player[npc.target].position.X > 6400f && Main.player[npc.target].position.X < (float)(Main.maxTilesX * 16 - 6400))))
			{
				npc.dontTakeDamage = true;
			}
			else if (npc.ai[0] <= 8f)
				npc.dontTakeDamage = false;

			// Increase velocity while charging
			if (npc.ai[0] == 1f || npc.ai[0] == 6f || npc.ai[0] == 11f)
				npc.velocity *= 1.01f;

			if (npc.ai[0] == 0f && !Main.player[npc.target].dead)
			{
				// Transition to phase 2 earlier
				if ((double)npc.life <= (double)npc.lifeMax * 0.75)
				{
					npc.ai[0] = 4f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 5f && !Main.player[npc.target].dead)
			{
				// Transition to phase 3 earlier
				if ((double)npc.life <= (double)npc.lifeMax * ((CalamityWorld.death || CalamityWorld.bossRushActive) ? 0.3 : 0.2))
				{
					npc.ai[0] = 9f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}
		}
		#endregion

		#region Revengeance Golem AI
		public static void RevengeanceGolemHeadFreeAI(NPC npc)
		{
			// Enrage if the player isn't inside the temple
			bool enrage = true;
			if ((double)Main.player[npc.target].Center.Y > Main.worldSurface * 16.0)
			{
				int num = (int)Main.player[npc.target].Center.X / 16;
				int num2 = (int)Main.player[npc.target].Center.Y / 16;
				Tile tile = Framing.GetTileSafely(num, num2);

				if (tile.wall == 87)
					enrage = false;
			}

			// Check if Golem body is still alive to avoid index out of bounds errors
			if (NPC.CountNPCS(NPCID.Golem) > 0)
			{
				// Increase laser fire rate if enraged
				if (enrage)
				{
					npc.ai[1] += 3f;
					npc.ai[2] += 3f;
				}

				// Decrease laser and fireball fire rate to make the fight more fair
				npc.ai[1] -= 0.25f; //0.75
				if ((double)Main.npc[NPC.golemBoss].life < (double)Main.npc[NPC.golemBoss].lifeMax * 0.8)
					npc.ai[1] -= 0.75f; //1
				if ((double)Main.npc[NPC.golemBoss].life < (double)Main.npc[NPC.golemBoss].lifeMax * 0.6)
					npc.ai[1] -= 0.75f; //1.25
				if ((double)Main.npc[NPC.golemBoss].life < (double)Main.npc[NPC.golemBoss].lifeMax * 0.2)
					npc.ai[1] -= 0.75f; //1.5
				if ((double)Main.npc[NPC.golemBoss].life < (double)Main.npc[NPC.golemBoss].lifeMax * 0.1)
					npc.ai[1] -= 0.75f; //1.75

				npc.ai[2] -= 0.25f; //0.75
				if (Main.npc[NPC.golemBoss].life < Main.npc[NPC.golemBoss].lifeMax / 2)
					npc.ai[2] -= 0.75f; //1
				if (Main.npc[NPC.golemBoss].life < Main.npc[NPC.golemBoss].lifeMax / 3)
					npc.ai[2] -= 0.75f; //1.25
				if (Main.npc[NPC.golemBoss].life < Main.npc[NPC.golemBoss].lifeMax / 4)
					npc.ai[2] -= 0.5f; //1.75
				if (Main.npc[NPC.golemBoss].life < Main.npc[NPC.golemBoss].lifeMax / 5)
					npc.ai[2] -= 0.5f; //1.25
				if (Main.npc[NPC.golemBoss].life < Main.npc[NPC.golemBoss].lifeMax / 6)
					npc.ai[2] -= 0.25f; //2
			}
		}

		public static void RevengeanceGolemHeadAI(NPC npc)
		{
			// Enrage if the player isn't inside the temple
			bool enrage = true;
			if ((double)Main.player[npc.target].Center.Y > Main.worldSurface * 16.0)
			{
				int num = (int)Main.player[npc.target].Center.X / 16;
				int num2 = (int)Main.player[npc.target].Center.Y / 16;
				Tile tile = Framing.GetTileSafely(num, num2);

				if (tile.wall == 87)
					enrage = false;
			}

			if (npc.ai[0] == 1f)
			{
				// Increase laser fire rate if enraged
				if (enrage)
				{
					npc.ai[1] += 3f;
					npc.ai[2] += 3f;
				}

				// Decrease laser and fireball fire rate to make the fight more fair
				npc.ai[1] -= 0.25f; //0.75
				if ((double)npc.life < (double)npc.lifeMax * 0.4)
					npc.ai[1] -= 0.75f; //1
				if ((double)npc.life < (double)npc.lifeMax * 0.2)
					npc.ai[1] -= 0.75f; //1.25

				npc.ai[2] -= 0.25f; //0.75
				if (npc.life < npc.lifeMax / 3)
					npc.ai[2] -= 0.75f; //1
				if (npc.life < npc.lifeMax / 4)
					npc.ai[2] -= 0.75f; //1.25
				if (npc.life < npc.lifeMax / 5)
					npc.ai[2] -= 0.75f; //1.5
			}
		}
		#endregion

		#region Revengeance Plantera AI
		public static void RevengeancePlanteraAI(NPC npc, bool configBossRushBoost, Mod mod, bool enraged)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);

			// Check for Jungle and remaining Tentacles
			bool jungle = Main.player[npc.target].ZoneJungle;
			bool tentaclesDead = !NPC.AnyNPCs(NPCID.PlanterasTentacle);

			// Increase speed based on HP
			if (npc.life < npc.lifeMax / 8 || CalamityWorld.bossRushActive || tentaclesDead)
			{
				npc.velocity.X *= 1.003f;
				npc.velocity.Y *= 1.003f;
			}
			else if (npc.life < npc.lifeMax / 4)
			{
				npc.velocity.X *= 1.001f;
				npc.velocity.Y *= 1.001f;
			}
			else if (npc.life < npc.lifeMax / 2)
			{
				npc.velocity.X *= 1.0005f;
				npc.velocity.Y *= 1.0005f;
			}

			if (npc.life > npc.lifeMax / 2)
			{
				// Defense increase
				npc.defense = 42;

				// Increase fire rate of Seeds
				if (Main.netMode != 1)
					npc.localAI[1] += ((enraged || configBossRushBoost) ? 3f : 1.5f);
			}
			else
			{
				// Increases fire rate of Spores
				npc.localAI[1] += 1f;

				// Half defense of phase 1
				npc.defense = 22;

				// Shoot Poison Seeds
				if ((double)npc.life < (double)npc.lifeMax * (CalamityWorld.death ? 0.45 : 0.35))
				{
					calamityGlobalNPC.newAI[0] += 1f;
					if (CalamityWorld.bossRushActive)
						calamityGlobalNPC.newAI[0] += 0.5f;
					if (tentaclesDead)
						calamityGlobalNPC.newAI[0] += 3f;

					if (calamityGlobalNPC.newAI[0] >= ((enraged || configBossRushBoost) ? 75f : 120f))
					{
						calamityGlobalNPC.newAI[0] = 0f;
						if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
						{
							Vector2 vector93 = new Vector2(npc.Center.X, npc.Center.Y);
							float num742 = (!jungle ? 32f : 18f);
							float num743 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector93.X;
							float num744 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector93.Y;
							float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));
							num745 = num742 / num745;
							num743 *= num745;
							num744 *= num745;
							vector93.X += num743 * 3f;
							vector93.Y += num744 * 3f;
							if (Main.netMode != 1)
							{
								int num419 = 28;
								int num420 = ProjectileID.PoisonSeedPlantera;
								int proj = Projectile.NewProjectile(vector93.X, vector93.Y, num743, num744, num420, num419, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[proj].timeLeft = 300;
							}
						}
					}
				}

				// Shoot Thorn Balls
				if ((double)npc.life < (double)npc.lifeMax * (CalamityWorld.death ? 0.35 : 0.25) || CalamityWorld.bossRushActive)
				{
					calamityGlobalNPC.newAI[1] += 1f;
					if (CalamityWorld.bossRushActive)
						calamityGlobalNPC.newAI[1] += 0.5f;
					if (tentaclesDead)
						calamityGlobalNPC.newAI[1] += 2f;

					if (calamityGlobalNPC.newAI[1] >= ((enraged || configBossRushBoost) ? 180f : 240f))
					{
						calamityGlobalNPC.newAI[1] = 0f;
						if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
						{
							Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float num349 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
							float num350 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
							float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
							num349 *= num351;
							num350 *= num351;
							if (Main.netMode != 1)
							{
								float num418 = (!jungle ? 24f : 12f);
								int num419 = 32;
								int num420 = ProjectileID.ThornBall;
								num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
								num351 = num418 / num351;
								num349 *= num351;
								num350 *= num351;
								num349 += (float)Main.rand.Next(-10, 11) * 0.05f;
								num350 += (float)Main.rand.Next(-10, 11) * 0.05f;
								vector34.X += num349 * 4f;
								vector34.Y += num350 * 4f;
								Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350, num420, num419, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}
			}
		}

		public static void RevengeancePlanterasTentacleAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);

			// Check for Jungle
			bool jungle = Main.player[npc.target].ZoneJungle;

			// Shoots Spores
			calamityGlobalNPC.newAI[0] += 1f;
			if (CalamityWorld.death || !jungle)
				calamityGlobalNPC.newAI[0] += 1f;

			if (calamityGlobalNPC.newAI[0] >= 480f)
			{
				calamityGlobalNPC.newAI[0] = 0f;
				if (Main.netMode != 1 && NPC.CountNPCS(NPCID.Spore) < 20)
					NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.Spore, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
			}
		}
		#endregion

		#region Revengeance Wall of Flesh AI
		public static void RevengeanceWallofFleshAI(NPC npc, bool configBossRushBoost, bool enraged)
		{
			// Increase speed
			if (enraged || configBossRushBoost)
				npc.velocity.X *= 1.7f;
			else if (CalamityWorld.bossRushActive)
				npc.velocity.X *= 1.3f;
			else if (CalamityWorld.death)
				npc.velocity.X *= 1.18f;
			else
				npc.velocity.X *= 1.12f;
		}

		public static void RevengeanceWallofFleshEyeAI(NPC npc, bool configBossRushBoost, Mod mod, bool enraged)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);

			// Make sure Wall of Flesh mouth is still alive to avoid index out of bounds errors
			if (NPC.CountNPCS(NPCID.WallofFlesh) > 0)
			{
				Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num349 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
				float num350 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
				float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
				num349 *= num351;
				num350 *= num351;

				bool flag30 = true;
				if (npc.direction > 0)
				{
					if (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) < npc.position.X + (float)(npc.width / 2))
						flag30 = false;
				}
				else if (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) > npc.position.X + (float)(npc.width / 2))
					flag30 = false;

				if (Main.netMode != 1)
				{
					// Laser shots per burst
					int num352 = ((enraged || configBossRushBoost) ? 7 : 4);

					// Disable normal laser code
					npc.localAI[1] = 0f;

					// Laser rate of fire
					npc.localAI[3] += ((enraged || configBossRushBoost) ? 4f : 2f);

					// Increase laser shots per burst and rate of fire as health drops
					if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.75)
					{
						npc.localAI[3] += 1f;
						num352++;
					}
					if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.5)
					{
						npc.localAI[3] += 1f;
						num352++;
					}
					if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.25)
					{
						npc.localAI[3] += 1f;
						num352 += 2;
					}
					if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.1 || CalamityWorld.bossRushActive)
					{
						npc.localAI[3] += 2f;
						num352 += 3;
					}

					if (calamityGlobalNPC.newAI[0] == 0f)
					{
						// Pause before next laser burst
						if (npc.localAI[3] > 600f)
						{
							calamityGlobalNPC.newAI[0] = 1f;
							npc.localAI[3] = 0f;
						}
					}
					else if (npc.localAI[3] > 45f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						// Fire laser burst and reset burst variables
						npc.localAI[3] = 0f;

						calamityGlobalNPC.newAI[0] += 1f;
						if (calamityGlobalNPC.newAI[0] >= (float)num352)
							calamityGlobalNPC.newAI[0] = 0f;

						if (flag30)
						{
							// Laser speed
							float num353 = ((enraged || configBossRushBoost) ? 14f : 10f);
							int num354 = 18;
							int num355 = ProjectileID.EyeLaser;

							// Increase laser speed and damage as health drops
							if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.5)
							{
								num354++;
								num353 += 1f;
							}
							if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.25)
							{
								num354++;
								num353 += 1f;
							}
							if ((double)Main.npc[Main.wof].life < (double)Main.npc[Main.wof].lifeMax * 0.1 || CalamityWorld.bossRushActive)
							{
								num354 += 2;
								num353 += 2f;
							}
							if (CalamityWorld.death || CalamityWorld.bossRushActive)
								num353 += 1f;

							vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							num349 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector34.X;
							num350 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector34.Y;
							num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
							num351 = num353 / num351;
							num349 *= num351;
							num350 *= num351;
							vector34.X += num349;
							vector34.Y += num350;
							Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350, num355, num354, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}
		}
		#endregion

		#region Revengeance Skeletron AI
		public static void RevengeanceSkeletronHandAI(NPC npc, bool configBossRushBoost, bool enraged)
		{
			// Increase how aggressive the hands are while in attacking phases
			if (npc.ai[2] == 0f || npc.ai[2] == 3f)
			{
				if (Main.npc[(int)npc.ai[1]].ai[1] == 0f)
				{
					npc.ai[3] += ((enraged || configBossRushBoost) ? 1.5f : 0.5f);
					if (CalamityWorld.death || CalamityWorld.bossRushActive)
						npc.ai[3] += 1f;
				}
			}
		}

		public static void RevengeanceSkeletronAI(NPC npc, bool configBossRushBoost, bool enraged)
		{
			if (npc.ai[1] == 1f)
			{
				if (Main.netMode != 1)
				{
					// Shoot skulls during spinning phase
					npc.localAI[1] += ((enraged || configBossRushBoost) ? 6f : 3f);

					// Increase rate of fire as health drops
					if ((double)npc.life <= (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
						npc.localAI[1] += 3f;
					if ((double)npc.life <= (double)npc.lifeMax * 0.15 || CalamityWorld.bossRushActive)
						npc.localAI[1] += 3f;
					if (CalamityWorld.death || CalamityWorld.bossRushActive)
						npc.localAI[1] += 3f;

					if (npc.localAI[1] >= 500f)
					{
						npc.localAI[1] = 0f;
						Vector2 vector16 = npc.Center;
						if (Collision.CanHit(vector16, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
						{
							float num159 = 5f;
							float num160 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector16.X + (float)Main.rand.Next(-20, 21);
							float num161 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector16.Y + (float)Main.rand.Next(-20, 21);
							float num162 = (float)Math.Sqrt((double)(num160 * num160 + num161 * num161));
							num162 = num159 / num162;
							num160 *= num162;
							num161 *= num162;
							Vector2 value = new Vector2(num160 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f, num161 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f);
							value.Normalize();
							value *= num159;
							value += npc.velocity;
							num160 = value.X;
							num161 = value.Y;
							int num163 = 20;
							int num164 = ProjectileID.Skull;
							vector16 += value * 5f;
							int num165 = Projectile.NewProjectile(vector16.X, vector16.Y, num160, num161, num164, num163, 0f, Main.myPlayer, -1f, 0f);
							Main.projectile[num165].timeLeft = 300;
						}
					}
				}

				Vector2 vector20 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num173 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector20.X;
				float num174 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector20.Y;
				float num175 = (float)Math.Sqrt((double)(num173 * num173 + num174 * num174));

				// Increase speed while charging
				float num176 = CalamityWorld.bossRushActive ? 10f : 5f;
				if (enraged || configBossRushBoost)
					num176 += 3f;

				if (num175 > 150f)
					num176 *= 1.05f;
				if (num175 > 200f)
					num176 *= 1.1f;
				if (num175 > 250f)
					num176 *= 1.1f;
				if (num175 > 300f)
					num176 *= 1.1f;
				if (num175 > 350f)
					num176 *= 1.1f;
				if (num175 > 400f)
					num176 *= 1.1f;
				if (num175 > 450f)
					num176 *= 1.1f;
				if (num175 > 500f)
					num176 *= 1.1f;
				if (num175 > 550f)
					num176 *= 1.1f;
				if (num175 > 600f)
					num176 *= 1.1f;

				num175 = num176 / num175;
				npc.velocity.X = num173 * num175;
				npc.velocity.Y = num174 * num175;
			}
			else if (npc.ai[1] == 2f)
			{
				// Shoot skulls while enraged
				Vector2 vector21 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num177 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector21.X;
				float num178 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector21.Y;
				float num179 = (float)Math.Sqrt((double)(num177 * num177 + num178 * num178));
				num179 = 12f / num179;
				npc.velocity.X = num177 * num179;
				npc.velocity.Y = num178 * num179;
				if (Main.netMode != 1)
				{
					npc.localAI[1] += 1f;
					if (npc.localAI[1] >= 60f)
					{
						npc.localAI[1] = 0f;
						Vector2 vector16 = npc.Center;
						if (Collision.CanHit(vector16, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
						{
							float num159 = 5f;
							float num160 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector16.X + (float)Main.rand.Next(-20, 21);
							float num161 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector16.Y + (float)Main.rand.Next(-20, 21);
							float num162 = (float)Math.Sqrt((double)(num160 * num160 + num161 * num161));
							num162 = num159 / num162;
							num160 *= num162;
							num161 *= num162;
							Vector2 value = new Vector2(num160 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f, num161 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f);
							value.Normalize();
							value *= num159;
							value += npc.velocity;
							num160 = value.X;
							num161 = value.Y;
							int num163 = 2500;
							int num164 = ProjectileID.Skull;
							vector16 += value * 5f;
							int num165 = Projectile.NewProjectile(vector16.X, vector16.Y, num160, num161, num164, num163, 0f, Main.myPlayer, -1f, 0f);
							Main.projectile[num165].timeLeft = 300;
						}
					}
				}
			}
		}
		#endregion

		#region Revengeance Brain of Cthulhu AI
		public static void RevengeanceBrainofCthulhuAI(NPC npc)
		{
			if (npc.ai[0] < 0f)
			{
				// Increase knockback resistance and speed in phase 2
				npc.knockBackResist = (CalamityWorld.death ? 0.04f : 0.06f) * Main.expertKnockBack;

				npc.velocity.X *= (CalamityWorld.death ? 1.009f : 1.006f);
				npc.velocity.Y *= (CalamityWorld.death ? 1.009f : 1.006f);
			}
		}

		public static void RevengeanceCreeperAI(NPC npc)
		{
			if (npc.ai[0] != 0f)
			{
				// Increase charging speed
				Vector2 value = Main.player[npc.target].Center - npc.Center;
				value.Normalize();
				value *= 9f;
				npc.velocity = (npc.velocity * 99f + value) / 97.5f; //100
			}
		}
		#endregion

		#region Revengeance Eater of Worlds AI
		public static void RevengeanceEaterofWorldsAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);

			// Shoot cursed flames from the head
			if (!Main.player[npc.target].dead)
			{
				calamityGlobalNPC.newAI[0] += 1f;
				if (CalamityWorld.death)
					calamityGlobalNPC.newAI[0] += 2f;
			}
			if (calamityGlobalNPC.newAI[0] >= 180f)
			{
				calamityGlobalNPC.newAI[0] = 0f;
				if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num349 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
					float num350 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
					float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
					num349 *= num351;
					num350 *= num351;
					if (Main.netMode != 1)
					{
						float num418 = 12f;
						int num419 = 12;
						int num420 = 96;
						num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
						num351 = num418 / num351;
						num349 *= num351;
						num350 *= num351;
						num349 += (float)Main.rand.Next(-40, 41) * 0.05f;
						num350 += (float)Main.rand.Next(-40, 41) * 0.05f;
						vector34.X += num349 * 4f;
						vector34.Y += num350 * 4f;
						Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350, num420, num419, 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}
		}
		#endregion

		#region Revengeance King Slime AI
		public static void RevengeanceKingSlimeAI(NPC npc)
		{
			// Increases rate of jumps and teleports
			bool move = npc.ai[1] == 5f || npc.ai[1] == 6f;

			if (move)
			{
				npc.ai[0] += 2f;
				if (CalamityWorld.death)
					npc.ai[0] += 1f;
			}

			if (npc.velocity.Y == 0f)
			{
				if (!move)
				{
					npc.ai[0] += 8f;
					if (CalamityWorld.death)
						npc.ai[0] += 4f;
				}
			}
		}
		#endregion

		#region Revengeance Basic NPC AI
		public static void RevengeanceLihzahrdAI(NPC npc)
		{
			// Transform into second state sooner
			if (Main.netMode != 1 && (double)npc.life <= (double)npc.lifeMax * 0.9)
				npc.Transform(NPCID.LihzahrdCrawler);
		}

		public static void RevengeanceIceGolemAI(NPC npc)
		{
			// Increases movement speed
			float num63 = 2f;
			float num64 = 0.14f;
			num63 += (1f - (float)npc.life / (float)npc.lifeMax) * 1.5f;
			num64 += (1f - (float)npc.life / (float)npc.lifeMax) * 0.15f;

			if (npc.velocity.X < -num63 || npc.velocity.X > num63)
				return;
			else if (npc.velocity.X < num63 && npc.direction == 1)
			{
				npc.velocity.X = npc.velocity.X + num64;
				if (npc.velocity.X > num63)
					npc.velocity.X = num63;
			}
			else if (npc.velocity.X > -num63 && npc.direction == -1)
			{
				npc.velocity.X = npc.velocity.X - num64;
				if (npc.velocity.X < -num63)
					npc.velocity.X = -num63;
			}
		}
		#endregion
	}
}
