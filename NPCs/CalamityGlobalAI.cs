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

			if (npc.localAI[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
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

				if (Main.netMode != NetmodeID.MultiplayerClient)
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

				if (npc.ai[0] >= 60f && Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.Bottom = new Vector2(npc.localAI[1], npc.localAI[2]);
					npc.ai[1] = 6f;
					npc.ai[0] = 0f;
					npc.netUpdate = true;
				}

				if (Main.netMode == NetmodeID.MultiplayerClient && npc.ai[0] >= 120f)
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

				if (npc.ai[0] >= 30f && Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[1] = 0f;
					npc.ai[0] = 0f;
					npc.netUpdate = true;
					npc.TargetClosest(true);
				}

				if (Main.netMode == NetmodeID.MultiplayerClient && npc.ai[0] >= 60f)
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

				if (Main.netMode != NetmodeID.MultiplayerClient)
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
							if (Main.rand.NextBool(4))
								num254 = 535;

							int num255 = NPC.NewNPC(x, y, num254, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num255].SetDefaults(num254, -1f);
							Main.npc[num255].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
							Main.npc[num255].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
							Main.npc[num255].ai[0] = (float)(-1000 * Main.rand.Next(3));
							Main.npc[num255].ai[1] = 0f;

							if (Main.netMode == NetmodeID.Server && num255 < 200)
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

			if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
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
						Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
						num = num794;
					}

					Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
				}

				calamityGlobalNPC.newAI[0] += 1f;
				if (calamityGlobalNPC.newAI[0] >= 60f)
				{
					calamityGlobalNPC.newAI[0] = 0f;
					if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(NPCID.Creeper) < 15)
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
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.localAI[1] += 1f;
						if (npc.justHit)
							npc.localAI[1] -= (float)Main.rand.Next(5);

						int num799 = 60 + Main.rand.Next(120);
						if (Main.netMode != NetmodeID.SinglePlayer)
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

								if (Main.rand.NextBool(2))
									num801 += Main.rand.Next(7, 13);
								else
									num801 -= Main.rand.Next(7, 13);

								if (Main.rand.NextBool(2))
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

					if (Main.netMode != NetmodeID.SinglePlayer)
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
					if (Main.netMode != NetmodeID.SinglePlayer)
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
					if (Main.netMode != NetmodeID.MultiplayerClient)
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

					if (Main.netMode != NetmodeID.MultiplayerClient)
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
				if (Main.netMode != NetmodeID.MultiplayerClient)
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
					if (Main.netMode != NetmodeID.MultiplayerClient)
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
					if (Main.netMode != NetmodeID.MultiplayerClient)
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

					if (Main.netMode != NetmodeID.MultiplayerClient)
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
				if (Main.netMode != NetmodeID.MultiplayerClient)
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
						Vector2 expr_80F = (Vector2.Normalize(npc.velocity) * new Vector2((float)npc.width / 2f, (float)npc.height) * 0.75f * 0.5f).RotatedBy((double)((float)(i - (num20 / 2 - 1)) * 6.28318548f / (float)num20), default) + npc.Center;
						Vector2 vector2 = expr_80F - npc.Center;
						int num21 = Dust.NewDust(expr_80F + vector2, 0, 0, 172, vector2.X * 2f, vector2.Y * 2f, 100, default, 1.4f);
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
					Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(j - (num24 / 2 - 1)) * 3.1415926535897931 / (double)((float)num24), default) + vector;
					Vector2 vector4 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, 172, vector4.X * 2f, vector4.Y * 2f, 100, default, 1.4f);
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
					if (Main.netMode != NetmodeID.MultiplayerClient)
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

				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == (float)(num9 - 30))
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
					Vector2 arg_1A97_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(k - (num29 / 2 - 1)) * 3.1415926535897931 / (double)((float)num29), default) + vector;
					Vector2 vector9 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num30 = Dust.NewDust(arg_1A97_0 + vector9, 0, 0, 172, vector9.X * 2f, vector9.Y * 2f, 100, default, 1.4f);
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
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector10 = Vector2.Normalize(npc.velocity) * (float)(npc.width + 20) / 2f + vector;
						int num31 = NPC.NewNPC((int)vector10.X, (int)vector10.Y + 45, 371, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[num31].target = npc.target;
						Main.npc[num31].velocity = Vector2.Normalize(npc.velocity).RotatedBy((double)(1.57079637f * (float)npc.direction), default) * scaleFactor3;
						Main.npc[num31].netUpdate = true;
						Main.npc[num31].ai[3] = (float)Main.rand.Next(80, 121) / 100f;
					}
				}

				npc.velocity = npc.velocity.RotatedBy((double)(-(double)num15 * (float)npc.direction), default);
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

				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == (float)(num9 - 30))
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
					Vector2 arg_2444_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(m - (num34 / 2 - 1)) * 3.1415926535897931 / (double)((float)num34), default) + vector;
					Vector2 vector11 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num35 = Dust.NewDust(arg_2444_0 + vector11, 0, 0, 172, vector11.X * 2f, vector11.Y * 2f, 100, default, 1.4f);
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

				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == (float)(num12 / 2))
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

				npc.velocity = npc.velocity.RotatedBy((double)(-(double)num15 * (float)npc.direction), default);
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

		#region Buffed King Slime AI
		public static bool BuffedKingSlimeAI(NPC npc, Mod mod)
		{
			float num234 = 1f;
			bool flag8 = false;
			bool flag9 = false;
			npc.aiAction = 0;

			// Percent life remaining
			float lifeRatio = (float)npc.life / (float)npc.lifeMax;

			// Phases based on life percentage
			bool phase2 = lifeRatio < 0.5f;

			// Spawn crystal in phase 2
			if (phase2 && !NPC.AnyNPCs(mod.NPCType("KingSlimeJewel")))
			{
				Vector2 vector = npc.Center + new Vector2(-40f, (float)(-(float)npc.height / 2));
				for (int num621 = 0; num621 < 20; num621++)
				{
					int num622 = Dust.NewDust(vector, npc.width, npc.height, 90, 0f, 0f, 100, default, 2f);
					Main.dust[num622].velocity *= 2f;
					Main.dust[num622].noGravity = true;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 38);
				NPC.NewNPC((int)vector.X, (int)vector.Y, mod.NPCType("KingSlimeJewel"));
			}

			// Set up health value for spawning slimes
			if (npc.ai[3] == 0f && npc.life > 0)
				npc.ai[3] = (float)npc.lifeMax;

			// Spawn with attack delay
			if (npc.localAI[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
			{
				npc.ai[0] = -100f;
				npc.localAI[3] = 1f;
				npc.TargetClosest(true);
				npc.netUpdate = true;
			}

			// Despawn
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

			// Activate teleport
			if (!Main.player[npc.target].dead && npc.ai[2] >= 300f && npc.ai[1] < 5f && npc.velocity.Y == 0f)
			{
				npc.ai[2] = 0f;
				npc.ai[0] = 0f;
				npc.ai[1] = 5f;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.TargetClosest(false);
					Point point3 = npc.Center.ToTileCoordinates();
					Point point4 = Main.player[npc.target].Center.ToTileCoordinates();
					Vector2 vector30 = Main.player[npc.target].Center - npc.Center;

					int num235 = 10;
					int num236 = 0;
					int num237 = 7;
					int num238 = 0;
					bool flag10 = false;
					if (vector30.Length() > 2000f)
					{
						flag10 = true;
						num238 = 100;
					}

					while (!flag10 && num238 < 100)
					{
						num238++;
						int num239 = Main.rand.Next(point4.X - num235, point4.X + num235 + 1);
						int num240 = Main.rand.Next(point4.Y - num235, point4.Y + 1);

						if ((num240 < point4.Y - num237 || num240 > point4.Y + num237 || num239 < point4.X - num237 || num239 > point4.X + num237) &&
							(num240 < point3.Y - num236 || num240 > point3.Y + num236 || num239 < point3.X - num236 || num239 > point3.X + num236) &&
							!Main.tile[num239, num240].nactive())
						{
							int num241 = num240;
							int num242 = 0;

							bool flag11 = Main.tile[num239, num241].nactive() && Main.tileSolid[(int)Main.tile[num239, num241].type] && !Main.tileSolidTop[(int)Main.tile[num239, num241].type];
							if (flag11)
							{
								num242 = 1;
							}
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

			// Get closer to activating teleport
			if (!Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
				npc.ai[2] += 1f;
			if (Math.Abs(npc.Top.Y - Main.player[npc.target].Bottom.Y) > 320f)
				npc.ai[2] += 1f;

			// Dust variable
			Dust dust;

			// Teleport
			if (npc.ai[1] == 5f)
			{
				flag8 = true;
				npc.aiAction = 1;
				npc.ai[0] += 1f;
				num234 = MathHelper.Clamp((60f - npc.ai[0]) / 60f, 0f, 1f);
				num234 = 0.5f + num234 * 0.5f;

				if (npc.ai[0] >= 60f)
					flag9 = true;

				if (npc.ai[0] == 60f)
					Gore.NewGore(npc.Center + new Vector2(-40f, (float)(-(float)npc.height / 2)), npc.velocity, 734, 1f);

				if (npc.ai[0] >= 60f && Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.Bottom = new Vector2(npc.localAI[1], npc.localAI[2]);
					npc.ai[1] = 6f;
					npc.ai[0] = 0f;
					npc.netUpdate = true;
				}

				if (Main.netMode == NetmodeID.MultiplayerClient && npc.ai[0] >= 120f)
				{
					npc.ai[1] = 6f;
					npc.ai[0] = 0f;
				}

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
			}

			// Post-teleport
			else if (npc.ai[1] == 6f)
			{
				flag8 = true;
				npc.aiAction = 0;
				npc.ai[0] += 1f;
				num234 = MathHelper.Clamp(npc.ai[0] / 30f, 0f, 1f);
				num234 = 0.5f + num234 * 0.5f;

				if (npc.ai[0] >= 30f && Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[1] = 0f;
					npc.ai[0] = 0f;
					npc.netUpdate = true;
					npc.TargetClosest(true);
				}

				if (Main.netMode == NetmodeID.MultiplayerClient && npc.ai[0] >= 60f)
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

			// Don't take damage while teleporting
			npc.dontTakeDamage = (npc.hide = flag9);

			// Jump
			if (npc.velocity.Y == 0f)
			{
				npc.velocity.X = npc.velocity.X * 0.8f;
				if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
					npc.velocity.X = 0f;

				if (!flag8)
				{
					npc.ai[0] += 2f;
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

						float distanceBelowTarget = npc.position.Y - (Main.player[npc.target].position.Y + 80f);
						float speedMult = 1f;
						if (distanceBelowTarget > 0f)
							speedMult += (distanceBelowTarget) * 0.002f;

						if (speedMult > 2f)
							speedMult = 2f;

						// Jump type
						if (npc.ai[1] == 3f)
						{
							npc.velocity.Y = -13f * speedMult;
							npc.velocity.X = npc.velocity.X + (phase2 ? 4f : 3.5f) * (float)npc.direction;
							npc.ai[0] = -200f;
							npc.ai[1] = 0f;
						}
						else if (npc.ai[1] == 2f)
						{
							npc.velocity.Y = -6f * speedMult;
							npc.velocity.X = npc.velocity.X + (phase2 ? 5f : 4.5f) * (float)npc.direction;
							npc.ai[0] = -120f;
							npc.ai[1] += 1f;
						}
						else
						{
							npc.velocity.Y = -8f * speedMult;
							npc.velocity.X = npc.velocity.X + (phase2 ? 4.5f : 4f) * (float)npc.direction;
							npc.ai[0] = -120f;
							npc.ai[1] += 1f;
						}
					}
					else if (npc.ai[0] >= -30f)
						npc.aiAction = 1;
				}
			}

			// Change jump velocity
			else if (npc.target < 255 && ((npc.direction == 1 && npc.velocity.X < 3f) || (npc.direction == -1 && npc.velocity.X > -3f)))
			{
				if ((npc.direction == -1 && (double)npc.velocity.X < 0.1) || (npc.direction == 1 && (double)npc.velocity.X > -0.1))
					npc.velocity.X = npc.velocity.X + 0.2f * (float)npc.direction;
				else
					npc.velocity.X = npc.velocity.X * 0.93f;
			}

			// Spawn dust
			int num249 = Dust.NewDust(npc.position, npc.width, npc.height, 4, npc.velocity.X, npc.velocity.Y, 255, new Color(0, 80, 255, 80), npc.scale * 1.2f);
			Main.dust[num249].noGravity = true;
			dust = Main.dust[num249];
			dust.velocity *= 0.5f;

			if (npc.life > 0)
			{
				// Adjust size npcd on HP
				lifeRatio = lifeRatio * 0.5f + 0.75f;
				lifeRatio *= num234;
				if (lifeRatio != npc.scale)
				{
					npc.position.X = npc.position.X + (float)(npc.width / 2);
					npc.position.Y = npc.position.Y + (float)npc.height;
					npc.scale = lifeRatio;
					npc.width = (int)(98f * npc.scale);
					npc.height = (int)(92f * npc.scale);
					npc.position.X = npc.position.X - (float)(npc.width / 2);
					npc.position.Y = npc.position.Y - (float)npc.height;
				}

				// Slime spawning
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int num251 = (int)((double)npc.lifeMax * 0.05);
					if ((float)(npc.life + num251) < npc.ai[3])
					{
						npc.ai[3] = (float)npc.life;
						int num252 = Main.rand.Next(2, 4);
						int num;
						for (int num253 = 0; num253 < num252; num253 = num + 1)
						{
							int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
							int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));

							int random = phase2 ? 10 : 8;
							int npcType = Main.rand.Next(random);
							switch (npcType)
							{
								case 0:
									npcType = NPCID.BlueSlime;
									break;
								case 1:
									npcType = NPCID.YellowSlime;
									break;
								case 2:
									npcType = NPCID.RedSlime;
									break;
								case 3:
									npcType = NPCID.PurpleSlime;
									break;
								case 4:
									npcType = NPCID.GreenSlime;
									break;
								case 5:
									npcType = NPCID.IceSlime;
									break;
								case 6:
								case 7:
								case 8:
								case 9:
									npcType = NPCID.SlimeSpiked;
									break;
								default:
									break;
							}

							if (Main.raining && Main.rand.NextBool(10))
							{
								npcType = NPCID.UmbrellaSlime;

								if (Main.rand.NextBool(5))
									npcType = NPCID.RainbowSlime;
							}

							if (Main.rand.NextBool(250))
								npcType = NPCID.Pinky;

							int num255 = NPC.NewNPC(x, y, npcType, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num255].SetDefaults(npcType, -1f);
							Main.npc[num255].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
							Main.npc[num255].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
							Main.npc[num255].ai[0] = (float)(-1000 * Main.rand.Next(3));
							Main.npc[num255].ai[1] = 0f;

							if (Main.netMode == NetmodeID.Server && num255 < 200)
								NetMessage.SendData(23, -1, -1, null, num255, 0f, 0f, 0f, 0, 0, 0);

							num = num253;
						}
					}
				}
			}

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}
		#endregion

		#region Buffed Eye of Cthulhu AI
		public static bool BuffedEyeofCthulhuAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);
			bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;
			float num5 = 20f;

			// Percent life remaining
			float lifeRatio = (float)npc.life / (float)npc.lifeMax;

			bool phase2 = lifeRatio < 0.85f;
			bool phase3 = lifeRatio < 0.75f;
			bool phase4 = lifeRatio < 0.65f;
			bool phase5 = lifeRatio < 0.5f;

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

			if (Main.rand.NextBool(5))
			{
				int num10 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default, 1f);
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

						if (npc.ai[3] >= 40f)
						{
							npc.ai[3] = 0f;
							npc.rotation = num8;

							float num19 = 6f;
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

							if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(NPCID.ServantofCthulhu) < 12)
							{
								int num23 = NPC.NewNPC((int)vector2.X, (int)vector2.Y, NPCID.ServantofCthulhu, 0, 0f, 0f, 0f, 0f, 255);
								Main.npc[num23].velocity.X = vector3.X;
								Main.npc[num23].velocity.Y = vector3.Y;

								if (Main.netMode == NetmodeID.Server && num23 < 200)
									NetMessage.SendData(23, -1, -1, null, num23, 0f, 0f, 0f, 0, 0, 0);
							}

							Main.PlaySound(3, (int)vector2.X, (int)vector2.Y, 1, 1f, 0f);

							int num;
							for (int m = 0; m < 10; m = num + 1)
							{
								Dust.NewDust(vector2, 20, 20, 5, vector3.X * 0.4f, vector3.Y * 0.4f, 0, default, 1f);
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

				if (phase2 || CalamityWorld.death)
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

					num33 = 8f / num33;
					Vector2 vector6 = vector5;
					Vector2 vector7;
					vector7.X = num31 * num33;
					vector7.Y = num32 * num33;
					vector6.X += vector7.X * 10f;
					vector6.Y += vector7.Y * 10f;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int num34 = NPC.NewNPC((int)vector6.X, (int)vector6.Y, NPCID.ServantofCthulhu, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[num34].velocity.X = vector7.X;
						Main.npc[num34].velocity.Y = vector7.Y;

						if (Main.netMode == NetmodeID.Server && num34 < 200)
							NetMessage.SendData(23, -1, -1, null, num34, 0f, 0f, 0f, 0, 0, 0);
					}

					int num;
					for (int n = 0; n < 10; n = num + 1)
					{
						Dust.NewDust(vector6, 20, 20, 5, vector7.X * 0.4f, vector7.Y * 0.4f, 0, default, 1f);
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
							Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
							num = num36;
						}

						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
					}
				}
				Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
				npc.velocity.X = npc.velocity.X * 0.98f;
				npc.velocity.Y = npc.velocity.Y * 0.98f;

				if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
					npc.velocity.X = 0f;
				if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
					npc.velocity.Y = 0f;
			}

			else
			{
				npc.defense = 6;
				npc.damage = (int)(20f * Main.expertDamage);

				if (npc.ai[1] == 0f & phase5)
					npc.ai[1] = 5f;

				if (npc.ai[1] == 0f)
				{
					float num37 = ((enraged || configBossRushBoost) ? 8f : 5.5f) + (CalamityWorld.death ? 5f : 3.5f) * (0.85f - lifeRatio);
					float num38 = ((enraged || configBossRushBoost) ? 0.09f : 0.06f) + (CalamityWorld.death ? 0.04f : 0.025f) * (0.85f - lifeRatio);

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

						if (phase4)
							npc.ai[1] = 3f;

						npc.target = 255;
						npc.netUpdate = true;
					}
				}

				else if (npc.ai[1] == 1f)
				{
					Main.PlaySound(36, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
					npc.rotation = num8;

					float num42 = ((enraged || configBossRushBoost) ? 9.5f : 6.2f) + (CalamityWorld.death ? 6f : 4f) * (0.85f - lifeRatio);
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
							if (Main.netMode != NetmodeID.MultiplayerClient && phase3)
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
					if ((npc.ai[3] == 4f & phase5) && npc.Center.Y > Main.player[npc.target].Center.Y)
					{
						npc.TargetClosest(true);
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;

						if (npc.netSpam > 10)
							npc.netSpam = 10;
					}
					else if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.TargetClosest(true);
						float num48 = ((enraged || configBossRushBoost) ? 26f : 18f) + (CalamityWorld.death ? 5f : 3.5f) * (0.7f - lifeRatio);

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
					float num63 = ((enraged || configBossRushBoost) ? 12f : 8f) + (CalamityWorld.death ? 6f : 4f) * (0.4f - lifeRatio);
					float num64 = ((enraged || configBossRushBoost) ? 0.4f : 0.25f) + (CalamityWorld.death ? 0.15f : 0.1f) * (0.4f - lifeRatio);

					Vector2 vector11 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num65 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector11.X;
					float num66 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) + num62 - vector11.Y;

					bool horizontalCharge = calamityGlobalNPC.newAI[0] == 1f || calamityGlobalNPC.newAI[0] == 3f;
					if (horizontalCharge)
					{
						num62 = (calamityGlobalNPC.newAI[0] == 1f ? -500f : 500f);
						num63 *= 1.5f;
						num64 *= 1.5f;

						num65 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + num62 - vector11.X;
						num66 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector11.Y;
					}

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

					if (npc.ai[2] % 45f == 0f)
					{
						float num19 = 6f;
						Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
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

						if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(NPCID.ServantofCthulhu) < 4)
						{
							int num23 = NPC.NewNPC((int)vector2.X, (int)vector2.Y, NPCID.ServantofCthulhu, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num23].velocity.X = vector3.X;
							Main.npc[num23].velocity.Y = vector3.Y;

							if (Main.netMode == NetmodeID.Server && num23 < 200)
								NetMessage.SendData(23, -1, -1, null, num23, 0f, 0f, 0f, 0, 0, 0);
						}

						Main.PlaySound(SoundID.NPCDeath13, npc.position);

						int num;
						for (int m = 0; m < 10; m = num + 1)
						{
							Dust.NewDust(vector2, 20, 20, 5, vector3.X * 0.4f, vector3.Y * 0.4f, 0, default, 1f);
							num = m;
						}
					}

					if (npc.ai[2] >= (horizontalCharge ? 100f : 70f))
					{
						npc.TargetClosest(true);

						switch ((int)calamityGlobalNPC.newAI[0])
						{
							case 0: // Normal Eye behavior
								npc.ai[1] = 3f;
								npc.ai[2] = -1f;
								npc.ai[3] = -1f;
								break;
							case 1: // Charge from the left
								npc.ai[1] = 6f;
								npc.ai[2] = 0f;
								break;
							case 2: // Normal Eye behavior
								npc.ai[1] = 3f;
								npc.ai[2] = -1f;
								break;
							case 3: // Charge from the right
								npc.ai[1] = 6f;
								npc.ai[2] = 0f;
								break;
							default:
								break;
						}

						calamityGlobalNPC.newAI[0] += 1f;
						if (calamityGlobalNPC.newAI[0] > 3f)
							calamityGlobalNPC.newAI[0] = 0f;

						npc.netUpdate = true;
					}
				}

				else if (npc.ai[1] == 6f)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.TargetClosest(true);
						float num48 = ((enraged || configBossRushBoost) ? 26f : 18f) + (CalamityWorld.death ? 6f : 4f) * (0.4f - lifeRatio);

						Vector2 vector10 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num49 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector10.X;
						float num50 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector10.Y;
						float num51 = (float)Math.Sqrt((double)(num49 * num49 + num50 * num50));

						num51 = num48 / num51;
						npc.velocity.X = num49 * num51;
						npc.velocity.Y = num50 * num51;

						npc.ai[1] = 7f;
						npc.netUpdate = true;

						if (npc.netSpam > 10)
							npc.netSpam = 10;
					}
				}

				else if (npc.ai[1] == 7f)
				{
					if (npc.ai[2] == 0f)
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

					float num60 = num5 * 2.5f;
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

						npc.ai[2] = 0f;
						npc.ai[1] = 0f;
					}
				}
			}

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}
		#endregion

		#region Buffed Eater of Worlds AI
		public static bool BuffedEaterofWorldsAI(NPC npc, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);
			bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;

			// Total body segments
			float totalSegments = CalamityWorld.death ? 73f : 63f;

			// Count segments remaining
			float segmentCount = (float)(NPC.CountNPCS(NPCID.EaterofWorldsHead) + NPC.CountNPCS(NPCID.EaterofWorldsBody) + NPC.CountNPCS(NPCID.EaterofWorldsTail)); // 2f to 65f

			// Percent segments remaining, add two to total for head and tail
			float lifeRatio = segmentCount / (totalSegments + 2);

			// Phases
			bool phase2 = lifeRatio < 0.8f; // 52 or less parts left
			bool phase3 = lifeRatio < 0.6f; // 39 or less parts left
			bool phase4 = lifeRatio < 0.4f; // 26 or less parts left
			bool phase5 = lifeRatio < 0.2f; // 13 or less parts left

			// Fire projectiles
			if (Main.netMode != NetmodeID.MultiplayerClient && !phase5)
			{
				// Vile spit
				if (npc.type == NPCID.EaterofWorldsBody)
				{
					if (Main.rand.NextBool(900) && phase2)
					{
						npc.TargetClosest(true);
						if (Collision.CanHitLine(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
							NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2) + npc.velocity.X), (int)(npc.position.Y + (float)(npc.height / 2) + npc.velocity.Y), NPCID.VileSpit, 0, 0f, 1f, 0f, 0f, 255);
					}
				}

				// Cursed flames
				else if (npc.type == NPCID.EaterofWorldsHead)
				{
					calamityGlobalNPC.newAI[0] += 1f;
					float timer = 90f;
					timer += lifeRatio * 180f;

					if (calamityGlobalNPC.newAI[0] >= timer && phase3)
					{
						calamityGlobalNPC.newAI[0] = 0f;

						npc.TargetClosest(true);

						if (Collision.CanHitLine(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
						{
							Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float num349 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
							float num350 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
							float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));

							num349 *= num351;
							num350 *= num351;

							float num418 = 12f;
							int num419 = 12;
							int num420 = ProjectileID.CursedFlameHostile;
							num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
							num351 = num418 / num351;
							num349 *= num351;
							num350 *= num351;
							vector34.X += num349 * 3f;
							vector34.Y += num350 * 3f;

							Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350, num420, num419, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}

			// Worm variable
			npc.realLife = -1;

			// Target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
				npc.TargetClosest(true);

			// Despawn
			if (Main.player[npc.target].dead)
			{
				if (npc.timeLeft > 300)
					npc.timeLeft = 300;
			}

			// Spawn segments
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if ((npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody) && npc.ai[0] == 0f)
				{
					// Spawn entire worm
					if (npc.type == NPCID.EaterofWorldsHead)
					{
						// Length
						npc.ai[2] = totalSegments;

						// Body spawn
						npc.ai[0] = (float)NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), npc.type + 1, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					}
					else if (npc.type == NPCID.EaterofWorldsBody && npc.ai[2] > 0f)
					{
						// Body spawn
						npc.ai[0] = (float)NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), npc.type, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					}
					else
					{
						// Tail spawn
						npc.ai[0] = (float)NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), npc.type + 1, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					}

					// Worm shit
					Main.npc[(int)npc.ai[0]].ai[1] = (float)npc.whoAmI;
					Main.npc[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;
					npc.netUpdate = true;
				}

				// Splitting effect
				if (!Main.npc[(int)npc.ai[1]].active && !Main.npc[(int)npc.ai[0]].active)
				{
					npc.life = 0;
					npc.HitEffect(0, 10.0);
					npc.checkDead();
					npc.active = false;
					NetMessage.SendData(28, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
				}
				if (npc.type == NPCID.EaterofWorldsHead && !Main.npc[(int)npc.ai[0]].active)
				{
					npc.life = 0;
					npc.HitEffect(0, 10.0);
					npc.checkDead();
					npc.active = false;
					NetMessage.SendData(28, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
				}
				if (npc.type == NPCID.EaterofWorldsTail && !Main.npc[(int)npc.ai[1]].active)
				{
					npc.life = 0;
					npc.HitEffect(0, 10.0);
					npc.checkDead();
					npc.active = false;
					NetMessage.SendData(28, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
				}
				if (npc.type == NPCID.EaterofWorldsBody && (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != npc.aiStyle))
				{
					npc.type = NPCID.EaterofWorldsHead;
					int whoAmI = npc.whoAmI;
					float num25 = (float)npc.life / (float)npc.lifeMax;
					float num26 = npc.ai[0];
					npc.SetDefaultsKeepPlayerInteraction(npc.type);
					npc.life = (int)((float)npc.lifeMax * num25);
					npc.ai[0] = num26;
					npc.TargetClosest(true);
					npc.netUpdate = true;
					npc.whoAmI = whoAmI;
				}
				if (npc.type == NPCID.EaterofWorldsBody && (!Main.npc[(int)npc.ai[0]].active || Main.npc[(int)npc.ai[0]].aiStyle != npc.aiStyle))
				{
					int whoAmI2 = npc.whoAmI;
					float num27 = (float)npc.life / (float)npc.lifeMax;
					float num28 = npc.ai[1];
					npc.SetDefaultsKeepPlayerInteraction(npc.type);
					npc.life = (int)((float)npc.lifeMax * num27);
					npc.ai[1] = num28;
					npc.TargetClosest(true);
					npc.netUpdate = true;
					npc.whoAmI = whoAmI2;
				}

				if (!npc.active && Main.netMode == NetmodeID.Server)
					NetMessage.SendData(28, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
			}

			// Movement
			int num29 = (int)(npc.position.X / 16f) - 1;
			int num30 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
			int num31 = (int)(npc.position.Y / 16f) - 1;
			int num32 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
			if (num29 < 0)
				num29 = 0;
			if (num30 > Main.maxTilesX)
				num30 = Main.maxTilesX;
			if (num31 < 0)
				num31 = 0;
			if (num32 > Main.maxTilesY)
				num32 = Main.maxTilesY;

			// Fly or not
			bool flag2 = false;
			if (!flag2)
			{
				for (int num33 = num29; num33 < num30; num33++)
				{
					for (int num34 = num31; num34 < num32; num34++)
					{
						if (Main.tile[num33, num34] != null && ((Main.tile[num33, num34].nactive() && (Main.tileSolid[(int)Main.tile[num33, num34].type] || (Main.tileSolidTop[(int)Main.tile[num33, num34].type] && Main.tile[num33, num34].frameY == 0))) || Main.tile[num33, num34].liquid > 64))
						{
							Vector2 vector;
							vector.X = (float)(num33 * 16);
							vector.Y = (float)(num34 * 16);
							if (npc.position.X + (float)npc.width > vector.X && npc.position.X < vector.X + 16f && npc.position.Y + (float)npc.height > vector.Y && npc.position.Y < vector.Y + 16f)
							{
								flag2 = true;
								if (Main.rand.NextBool(100) && Main.tile[num33, num34].nactive())
								{
									WorldGen.KillTile(num33, num34, true, true, false);
								}
							}
						}
					}
				}
			}
			if (!flag2 && npc.type == NPCID.EaterofWorldsHead)
			{
				Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
				int num35 = (CalamityWorld.death ? 750 : 850);
				bool flag3 = true;
				for (int num36 = 0; num36 < 255; num36++)
				{
					if (Main.player[num36].active)
					{
						Rectangle rectangle2 = new Rectangle((int)Main.player[num36].position.X - num35, (int)Main.player[num36].position.Y - num35, num35 * 2, num35 * 2);
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

			// Velocity and acceleration
			float num37 = (CalamityWorld.death ? 13f : 12f) + 2.4f * (1f - lifeRatio);
			float num38 = (CalamityWorld.death ? 0.16f : 0.15f) + 0.03f * (1f - lifeRatio);

			if (phase5)
			{
				num37 += 2.4f;
				num38 += 0.03f;
			}
			else if (phase4)
			{
				num37 += 1.2f;
				num38 += 0.015f;
			}

			Vector2 vector2 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num39 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
			float num40 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);

			num39 = (float)((int)(num39 / 16f) * 16);
			num40 = (float)((int)(num40 / 16f) * 16);
			vector2.X = (float)((int)(vector2.X / 16f) * 16);
			vector2.Y = (float)((int)(vector2.Y / 16f) * 16);
			num39 -= vector2.X;
			num40 -= vector2.Y;
			float num52 = (float)Math.Sqrt((double)(num39 * num39 + num40 * num40));

			if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
			{
				try
				{
					vector2 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num39 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector2.X;
					num40 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector2.Y;
				}
				catch
				{
				}

				npc.rotation = (float)Math.Atan2((double)num40, (double)num39) + 1.57f;
				num52 = (float)Math.Sqrt((double)(num39 * num39 + num40 * num40));
				int num53 = npc.width;
				num53 = (int)((float)num53 * npc.scale);
				num52 = (num52 - (float)num53) / num52;
				num39 *= num52;
				num40 *= num52;
				npc.velocity = Vector2.Zero;
				npc.position.X = npc.position.X + num39;
				npc.position.Y = npc.position.Y + num40;
			}
			else
			{
				// Prevent new heads from being slowed when they spawn
				if (calamityGlobalNPC.newAI[1] < 3f)
				{
					calamityGlobalNPC.newAI[1] += 1f;

					// Set velocity for when a new head spawns
					npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * (num37 * 0.4f);
				}

				if (!flag2)
				{
					npc.TargetClosest(true);

					npc.velocity.Y = npc.velocity.Y + 0.11f;
					if (npc.velocity.Y > num37)
						npc.velocity.Y = num37;

					if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num37 * 0.4)
					{
						if (npc.velocity.X < 0f)
							npc.velocity.X = npc.velocity.X - num38 * 1.1f;
						else
							npc.velocity.X = npc.velocity.X + num38 * 1.1f;
					}
					else if (npc.velocity.Y == num37)
					{
						if (npc.velocity.X < num39)
							npc.velocity.X = npc.velocity.X + num38;
						else if (npc.velocity.X > num39)
							npc.velocity.X = npc.velocity.X - num38;
					}
					else if (npc.velocity.Y > 4f)
					{
						if (npc.velocity.X < 0f)
							npc.velocity.X = npc.velocity.X + num38 * 0.9f;
						else
							npc.velocity.X = npc.velocity.X - num38 * 0.9f;
					}
				}
				else
				{
					// Sound
					if (npc.soundDelay == 0)
					{
						float num54 = num52 / 40f;
						if (num54 < 10f)
							num54 = 10f;
						if (num54 > 20f)
							num54 = 20f;

						npc.soundDelay = (int)num54;
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);
					}

					num52 = (float)Math.Sqrt((double)(num39 * num39 + num40 * num40));
					float num55 = Math.Abs(num39);
					float num56 = Math.Abs(num40);
					float num57 = num37 / num52;
					num39 *= num57;
					num40 *= num57;

					// Despawn
					bool flag4 = npc.type == NPCID.EaterofWorldsHead && ((!Main.player[npc.target].ZoneCorrupt && !Main.player[npc.target].ZoneCrimson) || Main.player[npc.target].dead);
					if (flag4)
					{
						bool flag5 = true;
						for (int num58 = 0; num58 < 255; num58++)
						{
							if (Main.player[num58].active && !Main.player[num58].dead && Main.player[num58].ZoneCorrupt)
								flag5 = false;
						}

						if (flag5)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient && (double)(npc.position.Y / 16f) > (Main.rockLayer + (double)Main.maxTilesY) / 2.0)
							{
								npc.active = false;
								int num59 = (int)npc.ai[0];

								while (num59 > 0 && num59 < 200 && Main.npc[num59].active && Main.npc[num59].aiStyle == npc.aiStyle)
								{
									int arg_2853_0 = (int)Main.npc[num59].ai[0];
									Main.npc[num59].active = false;
									npc.life = 0;

									if (Main.netMode == NetmodeID.Server)
										NetMessage.SendData(23, -1, -1, null, num59, 0f, 0f, 0f, 0, 0, 0);

									num59 = arg_2853_0;
								}

								if (Main.netMode == NetmodeID.Server)
									NetMessage.SendData(23, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
							}
							num39 = 0f;
							num40 = num37;
						}
					}

					if ((npc.velocity.X > 0f && num39 > 0f) || (npc.velocity.X < 0f && num39 < 0f) || (npc.velocity.Y > 0f && num40 > 0f) || (npc.velocity.Y < 0f && num40 < 0f))
					{
						if (npc.velocity.X < num39)
							npc.velocity.X = npc.velocity.X + num38;
						else if (npc.velocity.X > num39)
							npc.velocity.X = npc.velocity.X - num38;
						if (npc.velocity.Y < num40)
							npc.velocity.Y = npc.velocity.Y + num38;
						else if (npc.velocity.Y > num40)
							npc.velocity.Y = npc.velocity.Y - num38;

						if ((double)Math.Abs(num40) < (double)num37 * 0.2 && ((npc.velocity.X > 0f && num39 < 0f) || (npc.velocity.X < 0f && num39 > 0f)))
						{
							if (npc.velocity.Y > 0f)
								npc.velocity.Y = npc.velocity.Y + num38 * 2f;
							else
								npc.velocity.Y = npc.velocity.Y - num38 * 2f;
						}

						if ((double)Math.Abs(num39) < (double)num37 * 0.2 && ((npc.velocity.Y > 0f && num40 < 0f) || (npc.velocity.Y < 0f && num40 > 0f)))
						{
							if (npc.velocity.X > 0f)
								npc.velocity.X = npc.velocity.X + num38 * 2f;
							else
								npc.velocity.X = npc.velocity.X - num38 * 2f;
						}
					}
					else if (num55 > num56)
					{
						if (npc.velocity.X < num39)
							npc.velocity.X = npc.velocity.X + num38 * 1.1f;
						else if (npc.velocity.X > num39)
							npc.velocity.X = npc.velocity.X - num38 * 1.1f;

						if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num37 * 0.5)
						{
							if (npc.velocity.Y > 0f)
								npc.velocity.Y = npc.velocity.Y + num38;
							else
								npc.velocity.Y = npc.velocity.Y - num38;
						}
					}
					else
					{
						if (npc.velocity.Y < num40)
							npc.velocity.Y = npc.velocity.Y + num38 * 1.1f;
						else if (npc.velocity.Y > num40)
							npc.velocity.Y = npc.velocity.Y - num38 * 1.1f;

						if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num37 * 0.5)
						{
							if (npc.velocity.X > 0f)
								npc.velocity.X = npc.velocity.X + num38;
							else
								npc.velocity.X = npc.velocity.X - num38;
						}
					}
				}

				npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;

				if (npc.type == NPCID.EaterofWorldsHead)
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
			}

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}
		#endregion

		#region Buffed Brain of Cthulhu AI
		public static bool BuffedBrainofCthulhuAI(NPC npc, bool enraged, Mod mod)
		{
			// whoAmI variable
			NPC.crimsonBoss = npc.whoAmI;

			// Spawn Creepers
			if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] == 0f)
			{
				npc.localAI[0] = 1f;
				for (int num789 = 0; num789 < 20; num789++)
				{
					float num790 = npc.Center.X;
					float num791 = npc.Center.Y;
					num790 += (float)Main.rand.Next(-npc.width, npc.width);
					num791 += (float)Main.rand.Next(-npc.height, npc.height);

					int num792 = NPC.NewNPC((int)num790, (int)num791, NPCID.Creeper, 0, 0f, 0f, 0f, 0f, 255);
					Main.npc[num792].velocity = new Vector2((float)Main.rand.Next(-30, 31) * 0.1f, (float)Main.rand.Next(-30, 31) * 0.1f);
					Main.npc[num792].netUpdate = true;
				}
			}

			// Despawn
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				npc.TargetClosest(true);

				if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 6000f)
				{
					npc.active = false;
					npc.life = 0;

					if (Main.netMode == NetmodeID.Server)
						NetMessage.SendData(23, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
				}
			}

			// Phase 2
			if (npc.ai[0] < 0f)
			{
				// Spawn gore
				if (npc.localAI[2] == 0f)
				{
					Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);

					npc.localAI[2] = 1f;

					Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 392, 1f);
					Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 393, 1f);
					Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 394, 1f);
					Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 395, 1f);

					for (int num794 = 0; num794 < 20; num794++)
						Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

					Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
				}

				// Percent life remaining
				float lifeRatio = (float)npc.life / (float)npc.lifeMax;

				// Phases based on HP
				bool phase2 = lifeRatio < 0.85f;
				bool phase3 = lifeRatio < 0.7f;
				bool phase4 = lifeRatio < 0.5f;
				bool phase5 = lifeRatio < 0.3f;
				bool spinning = npc.ai[0] == -4f;

				// Gain defense while spinning
				npc.defense = npc.defDefense + (spinning ? 7 : 0);

				// Take damage
				npc.dontTakeDamage = false;

				// Deal no damage while spinning
				npc.damage = (spinning ? 0 : npc.defDamage);

				// Move towards target
				npc.TargetClosest(true);

				// Target distance X
				float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;

				// Charge
				if (!spinning)
				{
					// Not charging
					if (npc.ai[0] != -6f)
					{
						// Rubber band movement
						Vector2 vector98 = new Vector2(npc.Center.X, npc.Center.Y);
						float num795 = Main.player[npc.target].Center.X - vector98.X;
						float num796 = Main.player[npc.target].Center.Y - vector98.Y;
						float num797 = (float)Math.Sqrt((double)(num795 * num795 + num796 * num796));
						float num798 = 8f + (2f * (1f - lifeRatio));

						if (phase2 && !phase3)
							num798 *= 0.9f;

						num797 = num798 / num797;
						num795 *= num797;
						num796 *= num797;
						npc.velocity.X = (npc.velocity.X * 50f + num795) / 51f;
						npc.velocity.Y = (npc.velocity.Y * 50f + num796) / 51f;
					}

					// Charge, -6
					else
					{
						npc.ai[1] += 1f;

						// Teleport
						if (npc.ai[1] >= 115f)
						{
							npc.knockBackResist = 0.45f - (0.45f * (1f - lifeRatio));

							npc.ai[0] = -7f;
							npc.ai[1] = 0f;
							npc.localAI[1] = 120f;
							npc.netUpdate = true;
						}

						// Charge sound and velocity
						else if (npc.ai[1] == 10f)
						{
							// Sound
							Main.PlaySound(36, (int)npc.position.X, (int)npc.position.Y, -1, 1f, 0f);

							// Velocity
							npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 14f;
						}
					}

					// Rubber band movement, -5
					if (npc.ai[0] == -5f)
					{
						// Spin or teleport
						npc.ai[2] += 1f;
						if (npc.ai[2] >= 180f)
						{
							bool spin = (phase4 ? Main.rand.Next(4) > 0 : Main.rand.NextBool());
							if (phase5)
								spin = true;

							// Velocity and knockback
							if (spin)
							{
								npc.knockBackResist = 0f;
								npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 24f;
							}
							else
								npc.knockBackResist = 0.45f - (0.45f * (1f - lifeRatio));

							npc.ai[0] = (!spin ? -7f : -4f);
							npc.ai[1] = (!spin ? 0f : (playerLocation < 0 ? 1f : -1f));
							npc.ai[2] = 0f;
							npc.ai[3] = (!spin ? 0f : (float)Main.rand.Next(61));
							npc.localAI[1] = (!spin ? 120f : 0f);
							npc.netUpdate = true;
						}
					}
				}

				// Circle around, -4
				if (spinning)
				{
					// Charge sound
					if (npc.ai[2] == 0f)
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

					// Velocity
					int var = 120;
					float velocity = 6.28318548f / ((float)var * 0.75f);
					npc.velocity = npc.velocity.RotatedBy((double)(-(double)velocity * npc.ai[1]), default);

					npc.ai[2] += 1f;

					float timer = 60f + npc.ai[3];

					if (npc.ai[2] >= timer - 5f)
					{
						if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 400f) // 25 tile distance
						{
							npc.ai[2] -= 1f;
							npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * -6f;
						}
					}

					// Charge at target and spit blood, -6 is straight line movement, -5 is rubber band movement
					if (npc.ai[2] >= timer)
					{
						// Complete stop
						npc.velocity *= 0f;

						// Blood spurt
						if (phase5)
						{
							// Blood dust
							for (int num621 = 0; num621 < 6; num621++)
							{
								int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 1f);
								Main.dust[num622].velocity *= 3f;
								if (Main.rand.NextBool(2))
								{
									Main.dust[num622].scale = 0.25f;
									Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
								}
							}
							for (int num623 = 0; num623 < 12; num623++)
							{
								int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 1.5f);
								Main.dust[num624].noGravity = true;
								Main.dust[num624].velocity *= 5f;
								num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 1f);
								Main.dust[num624].velocity *= 2f;
							}

							// Projectile speed and spread
							float num179 = 8f;
							Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float num180 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - value9.X;
							float num181 = Math.Abs(num180) * 0.1f;
							float num182 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - value9.Y - num181;
							float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
							num183 = num179 / num183;
							num180 *= num183;
							num182 *= num183;
							value9.X += num180;
							value9.Y += num182;

							// Fire 10 projectiles in an arc
							for (int num186 = 0; num186 < 5; num186++)
							{
								num180 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - value9.X;
								num182 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - value9.Y;
								num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
								num183 = num179 / num183;
								num180 += (float)Main.rand.Next(-120, 121);
								num180 *= num183;
								Projectile.NewProjectile(value9.X, value9.Y, num180, -5f, mod.ProjectileType("BloodGeyser"), 10, 0f, Main.myPlayer, 0f, 0f);
							}
						}

						bool charge = (phase4 ? Main.rand.Next(4) > 0 : Main.rand.NextBool());
						if (phase5)
							charge = true;

						// Adjust knockback
						if (charge)
							npc.knockBackResist = 0f;
						else
							npc.knockBackResist = 0.45f - (0.45f * (1f - lifeRatio));

						npc.ai[0] = (charge ? -6f : -5f);
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
				}

				// Pick teleport location
				else if (npc.ai[0] == -1f || npc.ai[0] == -7f)
				{
					// Adjust knockback
					if (npc.ai[0] == -1f)
						npc.knockBackResist = 0.45f - (0.45f * (1f - lifeRatio));

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						// Go to phase 3
						if (phase3 && npc.ai[0] == -1f)
						{
							npc.ai[0] = -5f;
							npc.ai[1] = 0f;
							npc.ai[2] = 0f;
							npc.ai[3] = 0f;
							npc.localAI[1] = 0f;
							npc.alpha = 0;
							npc.netUpdate = true;
						}

						npc.localAI[1] += 1f;

						int random = 5 - (int)(5f * (1f - lifeRatio));
						if (npc.justHit)
							npc.localAI[1] -= (float)Main.rand.Next(random);

						float num799 = ((phase2 && !phase3) ? 90f : 120f);
						if (npc.localAI[1] >= num799)
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

								int min = 9;
								int max = ((phase2 && !phase3) ? 11 : 15);

								if (phase3)
								{
									min = 17;
									max = 20;
								}

								if (Main.rand.NextBool(2))
									num801 += Main.rand.Next(min, max);
								else
									num801 -= Main.rand.Next(min, max);

								if (Main.rand.NextBool(2))
									num802 += Main.rand.Next(min, max);
								else
									num802 -= Main.rand.Next(min, max);

								if (!WorldGen.SolidTile(num801, num802))
									break;

								if (num800 > 100)
									goto Block_2784;
							}
							npc.ai[3] = 0f;
							npc.ai[0] = (npc.ai[0] == -7f ? -8f : -2f);
							npc.ai[1] = (float)num801;
							npc.ai[2] = (float)num802;
							npc.netUpdate = true;
							npc.netSpam = 0;
						Block_2784:;
						}
					}
				}

				// Teleport and turn invisible
				else if (npc.ai[0] == -2f || npc.ai[0] == -8f)
				{
					npc.velocity *= 0.9f;

					if (Main.netMode != NetmodeID.SinglePlayer)
						npc.ai[3] += 15f;
					else
						npc.ai[3] += 25f;

					if (npc.ai[3] >= 255f)
					{
						npc.ai[3] = 255f;
						npc.position.X = npc.ai[1] * 16f - (float)(npc.width / 2);
						npc.position.Y = npc.ai[2] * 16f - (float)(npc.height / 2);
						Main.PlaySound(SoundID.Item8, npc.Center);
						npc.ai[0] = (npc.ai[0] == -8f ? -9f : -3f);
						npc.netUpdate = true;
						npc.netSpam = 0;
					}

					npc.alpha = (int)npc.ai[3];
				}

				// Become visible
				else if (npc.ai[0] == -3f || npc.ai[0] == -9f)
				{
					if (Main.netMode != NetmodeID.SinglePlayer)
						npc.ai[3] -= 15f;
					else
						npc.ai[3] -= 25f;

					if (npc.ai[3] <= 0f)
					{
						bool spin = (phase4 ? (Main.rand.NextBool() && npc.ai[0] == -9f) : false);
						if (phase5 && npc.ai[0] == -9f)
							spin = true;

						if (spin)
						{
							// Adjust knockback
							npc.knockBackResist = 0f;

							npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 24f;

							npc.ai[0] = -4f;
							npc.ai[1] = (playerLocation < 0 ? 1f : -1f);
							npc.ai[2] = 0f;
							npc.ai[3] = (float)Main.rand.Next(61);
						}
						else
						{
							npc.ai[3] = 0f;
							npc.ai[2] = 0f;
							npc.ai[1] = 0f;
							npc.ai[0] = (npc.ai[0] == -9f ? -5f : -1f);
						}
						npc.netUpdate = true;
						npc.netSpam = 0;
					}

					npc.alpha = (int)npc.ai[3];
				}
			}

			// Phase 1
			else
			{
				// Creeper count
				int creeperCount = NPC.CountNPCS(NPCID.Creeper);
				int creeperScale = 21 - creeperCount; // 1 to 20
				bool phase2 = creeperCount <= 0;

				// Move towards target
				npc.TargetClosest(true);
				Vector2 vector99 = new Vector2(npc.Center.X, npc.Center.Y);
				float num803 = Main.player[npc.target].Center.X - vector99.X;
				float num804 = Main.player[npc.target].Center.Y - vector99.Y;
				float num805 = (float)Math.Sqrt((double)(num803 * num803 + num804 * num804));
				float num806 = 1f + ((float)creeperScale * 0.05f);
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

				// Pick a teleport location
				if (npc.ai[0] == 0f)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						// Go to phase 2
						if (phase2)
						{
							npc.ai[0] = -1f;
							npc.localAI[1] = 0f;
							npc.alpha = 0;
							npc.netUpdate = true;
						}

						// Teleport location
						npc.localAI[1] += 1f + ((float)creeperScale * 0.05f);
						if (npc.localAI[1] >= 300f)
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

								int min = 18;
								int max = 26;

								if (Main.rand.NextBool(2))
									num810 += Main.rand.Next(min, max);
								else
									num810 -= Main.rand.Next(min, max);

								if (Main.rand.NextBool(2))
									num811 += Main.rand.Next(min, max);
								else
									num811 -= Main.rand.Next(min, max);

								if (!WorldGen.SolidTile(num810, num811) && Collision.CanHit(new Vector2((float)(num810 * 16), (float)(num811 * 16)), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
								{
									break;
								}
								if (num809 > 100)
								{
									goto Block_2801;
								}
							}
							npc.ai[0] = 1f;
							npc.ai[1] = (float)num810;
							npc.ai[2] = (float)num811;
							npc.netUpdate = true;
						Block_2801:;
						}
					}
				}

				// Turn invisible and teleport
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

				// Become visible
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

			// Despawn
			if (Main.player[npc.target].dead || (!Main.player[npc.target].ZoneCrimson && !Main.player[npc.target].ZoneCorrupt))
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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}

		public static bool BuffedCreeperAI(NPC npc, bool enraged, Mod mod)
		{
			// Despawn if Brain is gone
			if (NPC.crimsonBoss < 0)
			{
				npc.active = false;
				npc.netUpdate = true;
				return false;
			}

			// Creeper count, 0 to 20
			int creeperCount = NPC.CountNPCS(NPCID.Creeper);
			bool phase2 = creeperCount <= 10;

			// Adjust knockback
			if (phase2)
				npc.knockBackResist = 0f;

			// Stay near Brain
			if (npc.ai[0] == 0f)
			{
				Vector2 vector100 = new Vector2(npc.Center.X, npc.Center.Y);
				float num812 = Main.npc[NPC.crimsonBoss].Center.X - vector100.X;
				float num813 = Main.npc[NPC.crimsonBoss].Center.Y - vector100.Y;
				float num814 = (float)Math.Sqrt((double)(num812 * num812 + num813 * num813));
				float velocity = CalamityWorld.death ? 10f : 9f;

				// Max distance from Brain
				if (num814 > 90f)
				{
					num814 = velocity / num814;
					num812 *= num814;
					num813 *= num814;
					npc.velocity.X = (npc.velocity.X * 15f + num812) / 16f;
					npc.velocity.Y = (npc.velocity.Y * 15f + num813) / 16f;
					return false;
				}

				// Increase speed
				if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < velocity)
				{
					npc.velocity.Y = npc.velocity.Y * 1.05f;
					npc.velocity.X = npc.velocity.X * 1.05f;
				}

				// Charge at target
				int creeperScale = 21 - creeperCount; // 1 to 20
				npc.ai[1] += 0.5f + ((float)creeperScale * 0.25f);
				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] >= 240f)
				{
					npc.ai[1] = 0f;
					npc.TargetClosest(true);
					vector100 = new Vector2(npc.Center.X, npc.Center.Y);
					num812 = Main.player[npc.target].Center.X - vector100.X;
					num813 = Main.player[npc.target].Center.Y - vector100.Y;
					num814 = (float)Math.Sqrt((double)(num812 * num812 + num813 * num813));
					num814 = velocity / num814;
					npc.velocity.X = num812 * num814;
					npc.velocity.Y = num813 * num814;
					npc.ai[0] = 1f;
					npc.netUpdate = true;
				}
			}

			// Charge
			else
			{
				Vector2 value2 = Main.player[npc.target].Center - npc.Center;
				value2.Normalize();
				value2 *= 9f;
				npc.velocity = (npc.velocity * 99f + value2) / 100f;

				Vector2 vector101 = new Vector2(npc.Center.X, npc.Center.Y);
				float num815 = Main.npc[NPC.crimsonBoss].Center.X - vector101.X;
				float num816 = Main.npc[NPC.crimsonBoss].Center.Y - vector101.Y;
				float num817 = (float)Math.Sqrt((double)(num815 * num815 + num816 * num816));

				// Return to Brain
				if (num817 > 700f || (npc.justHit && !phase2))
					npc.ai[0] = 0f;
			}

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

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
				if (Main.netMode != NetmodeID.MultiplayerClient)
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
					if (CalamityWorld.death)
						num604 -= 25;

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
					if (Main.netMode != NetmodeID.MultiplayerClient)
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
					if (Main.netMode != NetmodeID.MultiplayerClient)
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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}
		#endregion

		#region Buffed Skeletron AI
		public static bool BuffedSkeletronAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);
			bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;

			// Percent life remaining
			float lifeRatio = (float)npc.life / (float)npc.lifeMax;

			// Phases
			bool phase2 = lifeRatio < 0.33f;

			// Set defense
			npc.defense = npc.defDefense;

			// Spawn hands
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (npc.ai[0] == 0f)
				{
					npc.TargetClosest(true);
					npc.ai[0] = 1f;

					int num155 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.SkeletronHand, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					Main.npc[num155].ai[0] = -1f;
					Main.npc[num155].ai[1] = (float)npc.whoAmI;
					Main.npc[num155].target = npc.target;
					Main.npc[num155].netUpdate = true;

					num155 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.SkeletronHand, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					Main.npc[num155].ai[0] = 1f;
					Main.npc[num155].ai[1] = (float)npc.whoAmI;
					Main.npc[num155].ai[3] = 150f;
					Main.npc[num155].target = npc.target;
					Main.npc[num155].netUpdate = true;
				}

				// Respawn hands
				if (phase2 && calamityGlobalNPC.newAI[0] == 0f && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 160f)
				{
					npc.TargetClosest(true);
					calamityGlobalNPC.newAI[0] = 1f;
					Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, -0.25f);

					int num155 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.SkeletronHand, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					Main.npc[num155].ai[0] = -1f;
					Main.npc[num155].ai[1] = (float)npc.whoAmI;
					Main.npc[num155].target = npc.target;
					Main.npc[num155].netUpdate = true;

					num155 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.SkeletronHand, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					Main.npc[num155].ai[0] = 1f;
					Main.npc[num155].ai[1] = (float)npc.whoAmI;
					Main.npc[num155].ai[3] = 150f;
					Main.npc[num155].target = npc.target;
					Main.npc[num155].netUpdate = true;
				}
			}

			// Distance from target
			float distance = Vector2.Distance(Main.player[npc.target].Center, npc.Center);

			// Despawn
			if (Main.player[npc.target].dead || distance > 2000f)
			{
				npc.TargetClosest(true);
				if (Main.player[npc.target].dead || distance > 2000f)
					npc.ai[1] = 3f;
			}

			// Daytime enrage
			if (Main.dayTime && npc.ai[1] != 3f && npc.ai[1] != 2f)
			{
				npc.ai[1] = 2f;
				Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
			}

			// Hand immunity
			int num156 = 0;
			for (int num157 = 0; num157 < 200; num157++)
			{
				if (Main.npc[num157].active && Main.npc[num157].type == npc.type + 1)
					num156++;
			}
			bool handsDead = num156 == 0;
			npc.chaseable = handsDead;
			npc.defense += num156 * 250;

			// Teleport
			if (handsDead || phase2)
			{
				// Post-teleport
				if (npc.ai[3] == -60f)
				{
					npc.ai[3] = 0f;
					Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 66, 1f, -0.25f);
					Main.PlaySound(SoundID.Item66, npc.position);

					Vector2 vector10 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);

					// Fire magic bolt after teleport
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						float num151 = 2f + (distance * 0.005f);
						if (num151 > 5f)
							num151 = 5f;

						int num152 = 17;
						int num153 = ProjectileID.Shadowflames;

						Vector2 value19 = Main.player[npc.target].Center - npc.Center;
						value19.Normalize();
						value19 *= num151;
						int numProj = 2;
						float rotation = MathHelper.ToRadians(15);
						for (int i = 0; i < numProj + 1; i++)
						{
							Vector2 perturbedSpeed = value19.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
							int proj = Projectile.NewProjectile(npc.Center.X, npc.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, num153, num152, 0f, Main.myPlayer, 0f, 0f);
							Main.projectile[proj].timeLeft = 600;
						}
					}

					// Teleport dust
					for (int m = 0; m < 60; m++)
					{
						int num39 = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 200, default, 3f);
						Main.dust[num39].noGravity = true;
						Main.dust[num39].velocity.X = Main.dust[num39].velocity.X * 2f;
					}
				}

				// Teleport after certain number of hits
				if (npc.justHit)
					npc.ai[3] += ((CalamityWorld.death ? 1.5f : 1f) + ((phase2 && handsDead) ? 1f : 0f)) - (handsDead ? 0f : 0.5f);

				// Dust to show teleport
				int ai3 = (int)npc.ai[3]; // 0 to 30, and -60
				bool emitDust = false;

				if (ai3 >= 25)
					emitDust = true;
				else if (ai3 >= 10)
				{
					if (Main.rand.Next(ai3 + 1) >= 10)
						emitDust = true;
				}
				if (emitDust)
				{
					int dust = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 200, default, 1.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 2f;
				}

				// Teleport
				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[3] >= 30f)
				{
					Vector2 value53 = npc.Center + new Vector2((float)(npc.direction * 20), 6f);
					Vector2 vector251 = Main.player[npc.target].Center - value53;
					Point point12 = npc.Center.ToTileCoordinates();
					Point point13 = Main.player[npc.target].Center.ToTileCoordinates();
					int num1453 = 30;
					int num1454 = 4;
					int num1455 = 26;
					int num1456 = 1;
					int num1457 = 0;

					bool flag106 = false;
					if (vector251.Length() > 2000f)
						flag106 = true;

					while (!flag106 && num1457 < 100)
					{
						num1457++;
						int num1458 = Main.rand.Next(point13.X - num1453, point13.X + num1453 + 1);
						int num1459 = Main.rand.Next(point13.Y - num1453, point13.Y + num1453 + 1);
						if ((num1459 < point13.Y - num1455 || num1459 > point13.Y + num1455 || num1458 < point13.X - num1455 || num1458 > point13.X + num1455) && (num1459 < point12.Y - num1454 || num1459 > point12.Y + num1454 || num1458 < point12.X - num1454 || num1458 > point12.X + num1454) && !Main.tile[num1458, num1459].nactive())
						{
							bool flag107 = true;
							if (flag107 && Main.tile[num1458, num1459].lava())
							{
								flag107 = false;
							}
							if (flag107 && Collision.SolidTiles(num1458 - num1456, num1458 + num1456, num1459 - num1456, num1459 + num1456))
							{
								flag107 = false;
							}
							if (flag107)
							{
								// Teleport dust
								for (int m = 0; m < 60; m++)
								{
									int num39 = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 200, default, 3f);
									Main.dust[num39].noGravity = true;
									Main.dust[num39].velocity.X = Main.dust[num39].velocity.X * 2f;
								}

								// New location
								npc.Center = new Vector2((float)(num1458 * 16 - npc.width / 2), (float)(num1459 * 16 - npc.height));
								npc.velocity = Vector2.Zero;
								npc.ai[3] = -60f;
								break;
							}
						}
					}
					npc.netUpdate = true;
				}
			}
			else
				npc.ai[3] = 0f;

			// Skull shooting
			if ((num156 < 2 || lifeRatio < 0.66f) && npc.ai[1] == 0f)
			{
				float num158 = 120f;
				if (handsDead)
					num158 /= 2f;

				if (Main.netMode != NetmodeID.MultiplayerClient && calamityGlobalNPC.newAI[1] % num158 == 0f)
				{
					Vector2 vector18 = npc.Center;
					float num159 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector18.X;
					float num160 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector18.Y;
					float num161 = (float)Math.Sqrt((double)(num159 * num159 + num160 * num160));
					if (Collision.CanHit(vector18, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						float num162 = 3f;
						if (handsDead)
							num162 += 2f;

						float num163 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector18.X + (float)Main.rand.Next(-20, 21);
						float num164 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector18.Y + (float)Main.rand.Next(-20, 21);
						float num165 = (float)Math.Sqrt((double)(num163 * num163 + num164 * num164));
						num165 = num162 / num165;
						num163 *= num165;
						num164 *= num165;
						Vector2 vector19 = new Vector2(num163 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f, num164 * 1f + (float)Main.rand.Next(-50, 51) * 0.01f);
						vector19.Normalize();
						vector19 *= num162;
						vector19 += npc.velocity;
						num163 = vector19.X;
						num164 = vector19.Y;
						int num166 = 17;
						int num167 = ProjectileID.Skull;
						vector18 += vector19 * 5f;
						int num168 = Projectile.NewProjectile(vector18.X, vector18.Y, num163, num164, num167, num166, 0f, Main.myPlayer, -1f, 0f);
						Main.projectile[num168].timeLeft = 300;
					}
				}
			}

			// Float above target
			if (npc.ai[1] == 0f)
			{
				npc.damage = npc.defDamage;

				calamityGlobalNPC.newAI[1] += 1f;
				npc.ai[2] += 1f + (3f * (1f - lifeRatio));
				if (npc.ai[2] >= 600f)
				{
					npc.ai[2] = 0f;
					npc.ai[1] = 1f;
					calamityGlobalNPC.newAI[1] = 0f;
					npc.TargetClosest(true);
					npc.netUpdate = true;
				}

				npc.rotation = npc.velocity.X / 15f;

				float num169 = 0.04f;
				float num170 = 5f;
				float num171 = 0.08f;
				float num172 = 10f;

				if (npc.position.Y > Main.player[npc.target].position.Y - 250f)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y = npc.velocity.Y * 0.98f;
					npc.velocity.Y = npc.velocity.Y - num169;
					if (npc.velocity.Y > num170)
						npc.velocity.Y = num170;
				}
				else if (npc.position.Y < Main.player[npc.target].position.Y - 250f)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y = npc.velocity.Y * 0.98f;
					npc.velocity.Y = npc.velocity.Y + num169;
					if (npc.velocity.Y < -num170)
						npc.velocity.Y = -num170;
				}

				if (npc.position.X + (float)(npc.width / 2) > Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X * 0.98f;
					npc.velocity.X = npc.velocity.X - num171;
					if (npc.velocity.X > num172)
						npc.velocity.X = num172;
				}

				if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X * 0.98f;
					npc.velocity.X = npc.velocity.X + num171;
					if (npc.velocity.X < -num172)
						npc.velocity.X = -num172;
				}
			}

			// Spin charge
			else if (npc.ai[1] == 1f)
			{
				npc.defense -= 10;

				npc.ai[2] += 1f + (0.5f * (1f - lifeRatio));

				calamityGlobalNPC.newAI[1] += 1f;
				if (calamityGlobalNPC.newAI[1] == 2f)
					Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);

				if (npc.ai[2] >= 300f)
				{
					npc.ai[2] = 0f;
					npc.ai[1] = 0f;
					calamityGlobalNPC.newAI[1] = 0f;
				}

				npc.rotation += (float)npc.direction * 0.3f;
				Vector2 vector20 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num173 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector20.X;
				float num174 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector20.Y;
				float num175 = (float)Math.Sqrt((double)(num173 * num173 + num174 * num174));

				// Increase speed while charging
				npc.damage = (int)((double)npc.defDamage * 1.3);
				float num176 = (CalamityWorld.bossRushActive ? 10f : 4.5f) + (1f * (1f - lifeRatio));
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
					num176 *= 1.15f;
				if (num175 > 400f)
					num176 *= 1.15f;
				if (num175 > 450f)
					num176 *= 1.15f;
				if (num175 > 500f)
					num176 *= 1.2f;
				if (num175 > 550f)
					num176 *= 1.2f;
				if (num175 > 600f)
					num176 *= 1.2f;

				num175 = num176 / num175;
				npc.velocity.X = num173 * num175;
				npc.velocity.Y = num174 * num175;
			}

			// Daytime enrage
			else if (npc.ai[1] == 2f)
			{
				npc.damage = 1000;
				npc.defense = 9999;
				npc.rotation += (float)npc.direction * 0.3f;
				Vector2 vector21 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num177 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector21.X;
				float num178 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector21.Y;
				float num179 = (float)Math.Sqrt((double)(num177 * num177 + num178 * num178));
				num179 = 8f / num179;
				npc.velocity.X = num177 * num179;
				npc.velocity.Y = num178 * num179;
			}

			// Despawn
			else if (npc.ai[1] == 3f)
			{
				npc.velocity.Y = npc.velocity.Y + 0.1f;
				if (npc.velocity.Y < 0f)
					npc.velocity.Y = npc.velocity.Y * 0.95f;
				npc.velocity.X = npc.velocity.X * 0.95f;
				if (npc.timeLeft > 50)
					npc.timeLeft = 50;
			}

			// Emit dust
			if (npc.ai[1] != 2f && npc.ai[1] != 3f && num156 != 0)
			{
				int num180 = Dust.NewDust(new Vector2(npc.position.X + (float)(npc.width / 2) - 15f - npc.velocity.X * 5f, npc.position.Y + (float)npc.height - 2f), 30, 10, 5, -npc.velocity.X * 0.2f, 3f, 0, default, 2f);
				Main.dust[num180].noGravity = true;
				Main.dust[num180].velocity.X = Main.dust[num180].velocity.X * 1.3f;
				Main.dust[num180].velocity.X = Main.dust[num180].velocity.X + npc.velocity.X * 0.4f;
				Main.dust[num180].velocity.Y = Main.dust[num180].velocity.Y + (2f + npc.velocity.Y);
				for (int num181 = 0; num181 < 2; num181++)
				{
					num180 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 120f), npc.width, 60, 5, npc.velocity.X, npc.velocity.Y, 0, default, 2f);
					Main.dust[num180].noGravity = true;
					Main.dust[num180].velocity -= npc.velocity;
					Main.dust[num180].velocity.Y = Main.dust[num180].velocity.Y + 5f;
				}
			}

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}

		public static bool BuffedSkeletronHandAI(NPC npc, bool enraged, Mod mod)
		{
			npc.spriteDirection = -(int)npc.ai[0];
			if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 11)
			{
				npc.ai[2] += 10f;
				if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
				{
					npc.life = -1;
					npc.HitEffect(0, 10.0);
					npc.active = false;
				}
			}
			if (npc.ai[2] == 0f || npc.ai[2] == 3f)
			{
				if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
					npc.timeLeft = 10;

				if (Main.npc[(int)npc.ai[1]].ai[1] != 0f)
				{
					if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y - 100f)
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y = npc.velocity.Y * 0.96f;
						npc.velocity.Y = npc.velocity.Y - 0.07f;
						if (npc.velocity.Y > 6f)
							npc.velocity.Y = 6f;
					}
					else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 100f)
					{
						if (npc.velocity.Y < 0f)
							npc.velocity.Y = npc.velocity.Y * 0.96f;
						npc.velocity.Y = npc.velocity.Y + 0.07f;
						if (npc.velocity.Y < -6f)
							npc.velocity.Y = -6f;
					}

					if (npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 120f * npc.ai[0])
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X = npc.velocity.X * 0.96f;
						npc.velocity.X = npc.velocity.X - 0.1f;
						if (npc.velocity.X > 8f)
							npc.velocity.X = 8f;
					}

					if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 120f * npc.ai[0])
					{
						if (npc.velocity.X < 0f)
							npc.velocity.X = npc.velocity.X * 0.96f;
						npc.velocity.X = npc.velocity.X + 0.1f;
						if (npc.velocity.X < -8f)
							npc.velocity.X = -8f;
					}
				}
				else
				{
					npc.ai[3] += 1f;
					if (npc.ai[3] >= 120f)
					{
						npc.ai[2] += 1f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}

					if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 230f)
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y = npc.velocity.Y * 0.96f;
						npc.velocity.Y = npc.velocity.Y - 0.04f;
						if (npc.velocity.Y > 3f)
							npc.velocity.Y = 3f;
					}
					else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 230f)
					{
						if (npc.velocity.Y < 0f)
							npc.velocity.Y = npc.velocity.Y * 0.96f;
						npc.velocity.Y = npc.velocity.Y + 0.04f;
						if (npc.velocity.Y < -3f)
							npc.velocity.Y = -3f;
					}

					if (npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0])
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X = npc.velocity.X * 0.96f;
						npc.velocity.X = npc.velocity.X - 0.07f;
						if (npc.velocity.X > 8f)
							npc.velocity.X = 8f;
					}

					if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0])
					{
						if (npc.velocity.X < 0f)
							npc.velocity.X = npc.velocity.X * 0.96f;
						npc.velocity.X = npc.velocity.X + 0.07f;
						if (npc.velocity.X < -8f)
							npc.velocity.X = -8f;
					}

					if (npc.position.Y > Main.npc[(int)npc.ai[1]].position.Y + 230f)
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y = npc.velocity.Y * 0.96f;
						npc.velocity.Y = npc.velocity.Y - 0.04f;
						if (npc.velocity.Y > 3f)
							npc.velocity.Y = 3f;
					}
					else if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y + 230f)
					{
						if (npc.velocity.Y < 0f)
							npc.velocity.Y = npc.velocity.Y * 0.96f;
						npc.velocity.Y = npc.velocity.Y + 0.04f;
						if (npc.velocity.Y < -3f)
							npc.velocity.Y = -3f;
					}

					if (npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0])
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X = npc.velocity.X * 0.96f;
						npc.velocity.X = npc.velocity.X - 0.07f;
						if (npc.velocity.X > 8f)
							npc.velocity.X = 8f;
					}

					if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0])
					{
						if (npc.velocity.X < 0f)
							npc.velocity.X = npc.velocity.X * 0.96f;
						npc.velocity.X = npc.velocity.X + 0.07f;
						if (npc.velocity.X < -8f)
							npc.velocity.X = -8f;
					}
				}

				Vector2 vector22 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num182 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector22.X;
				float num183 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector22.Y;
				float num184 = (float)Math.Sqrt((double)(num182 * num182 + num183 * num183));
				npc.rotation = (float)Math.Atan2((double)num183, (double)num182) + 1.57f;
				return false;
			}
			if (npc.ai[2] == 1f)
			{
				Vector2 vector23 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num185 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector23.X;
				float num186 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector23.Y;
				float num187 = (float)Math.Sqrt((double)(num185 * num185 + num186 * num186));
				npc.rotation = (float)Math.Atan2((double)num186, (double)num185) + 1.57f;
				npc.velocity.X = npc.velocity.X * 0.95f;
				npc.velocity.Y = npc.velocity.Y - 0.1f;

				npc.velocity.Y = npc.velocity.Y - 0.06f;
				if (npc.velocity.Y < -15f)
					npc.velocity.Y = -15f;

				if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 200f)
				{
					npc.TargetClosest(true);
					npc.ai[2] = 2f;
					vector23 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num185 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector23.X;
					num186 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector23.Y;
					num187 = (float)Math.Sqrt((double)(num185 * num185 + num186 * num186));

					num187 = (CalamityWorld.death ? 25f : 23f) / num187;
					npc.velocity.X = num185 * num187;
					npc.velocity.Y = num186 * num187;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[2] == 2f)
			{
				if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f)
					npc.ai[2] = 3f;
			}
			else if (npc.ai[2] == 4f)
			{
				Vector2 vector24 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num188 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector24.X;
				float num189 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector24.Y;
				float num190 = (float)Math.Sqrt((double)(num188 * num188 + num189 * num189));
				npc.rotation = (float)Math.Atan2((double)num189, (double)num188) + 1.57f;
				npc.velocity.Y = npc.velocity.Y * 0.95f;
				npc.velocity.X = npc.velocity.X + 0.1f * -npc.ai[0];

				npc.velocity.X = npc.velocity.X + 0.07f * -npc.ai[0];
				if (npc.velocity.X < -14f)
					npc.velocity.X = -14f;
				else if (npc.velocity.X > 14f)
					npc.velocity.X = 14f;

				if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 500f || npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) + 500f)
				{
					npc.TargetClosest(true);
					npc.ai[2] = 5f;
					vector24 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num188 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector24.X;
					num189 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector24.Y;
					num190 = (float)Math.Sqrt((double)(num188 * num188 + num189 * num189));

					num190 = (CalamityWorld.death ? 26f : 24f) / num190;
					npc.velocity.X = num188 * num190;
					npc.velocity.Y = num189 * num190;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[2] == 5f && ((npc.velocity.X > 0f && npc.position.X + (float)(npc.width / 2) > Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2)) || (npc.velocity.X < 0f && npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))))
				npc.ai[2] = 0f;

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}
		#endregion

		#region Buffed Wall of Flesh AI
		public static bool BuffedWallofFleshAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);
			bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;

			// Despawn
			if (npc.position.X < 160f || npc.position.X > (float)((Main.maxTilesX - 10) * 16))
				npc.active = false;

			// Set Wall of Flesh variables
			if (npc.localAI[0] == 0f)
			{
				npc.localAI[0] = 1f;
				Main.wofB = -1;
				Main.wofT = -1;
			}

			// Percent life remaining
			float lifeRatio = (float)npc.life / (float)npc.lifeMax;

			// Phases based on HP
			bool phase2 = lifeRatio < 0.66f;
			bool phase3 = lifeRatio < 0.33f;

			// Start leech spawning based on HP
			npc.ai[1] += 1f;
			if (npc.ai[2] == 0f)
			{
				if (phase2)
					npc.ai[1] += 1f;
				if (phase3)
					npc.ai[1] += 1f;

				if (npc.ai[1] > 2700f)
					npc.ai[2] = 1f;
			}

			// Leech spawn
			if (npc.ai[2] > 0f && npc.ai[1] > 60f)
			{
				int num330 = phase3 ? 3 : 2;

				npc.ai[2] += 1f;
				npc.ai[1] = 0f;
				if (npc.ai[2] > (float)num330)
					npc.ai[2] = 0f;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int num331 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)(npc.height / 2) + 20f), NPCID.LeechHead, 1, 0f, 0f, 0f, 0f, 255);
					Main.npc[num331].velocity.X = (float)(npc.direction * 9);
				}
			}

			// Play sound
			npc.localAI[3] += 1f;
			if (npc.localAI[3] >= (float)(600 + Main.rand.Next(1000)))
			{
				npc.localAI[3] = (float)(-(float)Main.rand.Next(200));
				Main.PlaySound(4, (int)npc.position.X, (int)npc.position.Y, 10, 1f, 0f);
			}

			// Set whoAmI variable
			Main.wof = npc.whoAmI;

			// Set eye positions
			int num332 = (int)(npc.position.X / 16f);
			int num333 = (int)((npc.position.X + (float)npc.width) / 16f);
			int num334 = (int)((npc.position.Y + (float)(npc.height / 2)) / 16f);
			int num335 = 0;
			int num336 = num334 + 7;
			while (num335 < 15 && num336 > Main.maxTilesY - 200)
			{
				num336++;
				int num;
				for (int num337 = num332; num337 <= num333; num337 = num + 1)
				{
					try
					{
						if (WorldGen.SolidTile(num337, num336) || Main.tile[num337, num336].liquid > 0)
							num335++;
					}
					catch
					{ num335 += 15; }

					num = num337;
				}
			}
			num336 += 4;
			if (Main.wofB == -1)
				Main.wofB = num336 * 16;
			else if (Main.wofB > num336 * 16)
			{
				Main.wofB--;
				if (Main.wofB < num336 * 16)
					Main.wofB = num336 * 16;
			}
			else if (Main.wofB < num336 * 16)
			{
				Main.wofB++;
				if (Main.wofB > num336 * 16)
					Main.wofB = num336 * 16;
			}

			num335 = 0;
			num336 = num334 - 7;
			while (num335 < 15 && num336 < Main.maxTilesY - 10)
			{
				num336--;
				int num;
				for (int num338 = num332; num338 <= num333; num338 = num + 1)
				{
					try
					{
						if (WorldGen.SolidTile(num338, num336) || Main.tile[num338, num336].liquid > 0)
							num335++;
					}
					catch
					{ num335 += 15; }

					num = num338;
				}
			}
			num336 -= 4;
			if (Main.wofT == -1)
				Main.wofT = num336 * 16;
			else if (Main.wofT > num336 * 16)
			{
				Main.wofT--;
				if (Main.wofT < num336 * 16)
					Main.wofT = num336 * 16;
			}
			else if (Main.wofT < num336 * 16)
			{
				Main.wofT++;
				if (Main.wofT > num336 * 16)
					Main.wofT = num336 * 16;
			}

			// Set Y velocity and position
			float num339 = (float)((Main.wofB + Main.wofT) / 2 - npc.height / 2);

			if (npc.position.Y > num339 + 1f)
				npc.velocity.Y = -1f;
			else if (npc.position.Y < num339 - 1f)
				npc.velocity.Y = 1f;
			npc.velocity.Y = 0f;

			int num340 = (Main.maxTilesY - 180) * 16;
			if (num339 < (float)num340)
				num339 = (float)num340;
			npc.position.Y = num339;

			// Speed up if target is too far or if they're hiding behind tiles, slow down if too close
			float distanceFromTarget = 0f;
			float targetPosition = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
			float npcPosition = npc.position.X + (float)(npc.width / 2);
			if (npc.velocity.X < 0f)
				distanceFromTarget = npcPosition - targetPosition;
			else
				distanceFromTarget = targetPosition - npcPosition;

			float halfAverageScreenWidth = 960f;
			float distanceBeforeSlowingDown = 320f;
			float timeBeforeEnrage = 600f;
			float speedMult = 1f;

			if (calamityGlobalNPC.newAI[0] < timeBeforeEnrage)
			{
				if (distanceFromTarget > halfAverageScreenWidth ||
					!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					speedMult += (distanceFromTarget - halfAverageScreenWidth) * 0.001f;
					calamityGlobalNPC.newAI[0] += 1f;

					// Enrage after 10 seconds of target being off screen
					if (calamityGlobalNPC.newAI[0] >= timeBeforeEnrage)
					{
						calamityGlobalNPC.newAI[1] = 1f;

						// Tell eyes to stop firing lasers
						npc.ai[3] = 1f;

						// Play roar sound on players nearby
						if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, npc.Center) < 2800f)
							Main.PlaySound(4, (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y, 10, 1f, -0.25f);
					}
				}
				else if (distanceFromTarget < distanceBeforeSlowingDown)
					speedMult += (distanceFromTarget - distanceBeforeSlowingDown) * 0.002f;

				if (speedMult > 2f)
					speedMult = 2f;
				if (speedMult < 0.75f)
					speedMult = 0.75f;
			}

			// Enrage if target is off screen for too long
			if (calamityGlobalNPC.newAI[1] == 1f)
			{
				// Triple speed
				speedMult = 3.25f;

				// Return to normal if very close to target
				if (distanceFromTarget < distanceBeforeSlowingDown)
				{
					calamityGlobalNPC.newAI[0] = 0f;
					calamityGlobalNPC.newAI[1] = 0f;
					npc.ai[3] = 0f;
				}
			}

			if (enraged || configBossRushBoost)
				speedMult += 0.7f;
			else if (CalamityWorld.bossRushActive)
				speedMult += 0.3f;
			else if (CalamityWorld.death)
				speedMult += 0.18f;

			// NOTE: Max velocity is 8 in expert mode
			float velocityX = (CalamityWorld.death ? 2.2f : 2f) + (4f * (1f - lifeRatio));
			velocityX *= speedMult;
			// NOTE: Values below are based on Rev Mode only!
			// Max velocity without enrage is 12
			// Min velocity is 1.5
			// Max velocity with enrage is 18

			// Set X velocity
			if (npc.velocity.X == 0f)
			{
				npc.TargetClosest(true);
				npc.velocity.X = (float)npc.direction;
			}
			if (npc.velocity.X < 0f)
			{
				npc.velocity.X = -velocityX;
				npc.direction = -1;
			}
			else
			{
				npc.velocity.X = velocityX;
				npc.direction = 1;
			}

			// Direction
			npc.spriteDirection = npc.direction;
			Vector2 vector37 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num342 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector37.X;
			float num343 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector37.Y;
			float num344 = (float)Math.Sqrt((double)(num342 * num342 + num343 * num343));
			num342 *= num344;
			num343 *= num344;

			// Rotation based on direction
			if (npc.direction > 0)
			{
				if (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) > npc.position.X + (float)(npc.width / 2))
					npc.rotation = (float)Math.Atan2((double)(-(double)num343), (double)(-(double)num342)) + 3.14f;
				else
					npc.rotation = 0f;
			}
			else if (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) < npc.position.X + (float)(npc.width / 2))
				npc.rotation = (float)Math.Atan2((double)num343, (double)num342) + 3.14f;
			else
				npc.rotation = 0f;

			// Expert hungry respawn over time
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				// Range of 1 to 11
				int chance = (int)(1f + lifeRatio * 10f);

				// Range of 1 to 121
				chance *= chance;

				// Range of 20 to 134
				chance = (chance * 19 + 400) / 20;

				// Range of 30 to 59
				if (chance < 60)
					chance = (chance * 3 + 60) / 4;

				// Range of 60 to 268
				chance = (int)((double)chance * 2D);

				if (Main.rand.Next(chance) == 0)
				{
					int num346 = 0;
					float[] array = new float[10];
					int num;
					for (int num347 = 0; num347 < 200; num347 = num + 1)
					{
						if (num346 < 10 && Main.npc[num347].active && Main.npc[num347].type == NPCID.TheHungry)
						{
							array[num346] = Main.npc[num347].ai[0];
							num346++;
						}
						num = num347;
					}

					int maxValue = 1 + num346 * 2;
					if (num346 < 10 && Main.rand.Next(maxValue) <= 1)
					{
						int num348 = -1;
						for (int num349 = 0; num349 < 1000; num349 = num + 1)
						{
							int num350 = Main.rand.Next(10);
							float num351 = (float)num350 * 0.1f - 0.05f;
							bool flag29 = true;
							for (int num352 = 0; num352 < num346; num352 = num + 1)
							{
								if (num351 == array[num352])
								{
									flag29 = false;
									break;
								}
								num = num352;
							}
							if (flag29)
							{
								num348 = num350;
								break;
							}
							num = num349;
						}
						if (num348 >= 0)
						{
							int num353 = NPC.NewNPC((int)npc.position.X, (int)num339, NPCID.TheHungry, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
							Main.npc[num353].ai[0] = (float)num348 * 0.1f - 0.05f;
						}
					}
				}
			}

			// Spawn eyes and hungries
			if (npc.localAI[0] == 1f && Main.netMode != NetmodeID.MultiplayerClient)
			{
				npc.localAI[0] = 2f;

				num339 = (float)((Main.wofB + Main.wofT) / 2);
				num339 = (num339 + (float)Main.wofT) / 2f;
				int num354 = NPC.NewNPC((int)npc.position.X, (int)num339, NPCID.WallofFleshEye, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				Main.npc[num354].ai[0] = 1f;

				num339 = (float)((Main.wofB + Main.wofT) / 2);
				num339 = (num339 + (float)Main.wofB) / 2f;
				num354 = NPC.NewNPC((int)npc.position.X, (int)num339, NPCID.WallofFleshEye, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				Main.npc[num354].ai[0] = -1f;

				num339 = (float)((Main.wofB + Main.wofT) / 2);
				num339 = (num339 + (float)Main.wofB) / 2f;

				int num;
				for (int num355 = 0; num355 < 11; num355 = num + 1)
				{
					num354 = NPC.NewNPC((int)npc.position.X, (int)num339, NPCID.TheHungry, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					Main.npc[num354].ai[0] = (float)num355 * 0.1f - 0.05f;
					num = num355;
				}
			}

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}

		public static bool BuffedWallofFleshEyeAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);
			bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;

			// Despawn
			if (Main.wof < 0)
			{
				npc.active = false;
				return false;
			}

			// Stupid worm variable but for wall of flesh npc time
			npc.realLife = Main.wof;

			// Life and target
			if (Main.npc[Main.wof].life > 0)
				npc.life = Main.npc[Main.wof].life;
			npc.TargetClosest(true);

			// Velocity, direction, and position
			npc.position.X = Main.npc[Main.wof].position.X;
			npc.direction = Main.npc[Main.wof].direction;
			npc.spriteDirection = npc.direction;

			float num356 = (float)((Main.wofB + Main.wofT) / 2);
			if (npc.ai[0] > 0f)
				num356 = (num356 + (float)Main.wofT) / 2f;
			else
				num356 = (num356 + (float)Main.wofB) / 2f;
			num356 -= (float)(npc.height / 2);

			if (npc.position.Y > num356 + 1f)
				npc.velocity.Y = -1f;
			else if (npc.position.Y < num356 - 1f)
				npc.velocity.Y = 1f;
			else
			{
				npc.velocity.Y = 0f;
				npc.position.Y = num356;
			}

			if (npc.velocity.Y > 5f)
				npc.velocity.Y = 5f;
			if (npc.velocity.Y < -5f)
				npc.velocity.Y = -5f;

			Vector2 vector38 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num357 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector38.X;
			float num358 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector38.Y;
			float num359 = (float)Math.Sqrt((double)(num357 * num357 + num358 * num358));
			num357 *= num359;
			num358 *= num359;

			// Rotation based on direction and whether to fire lasers or not
			bool flag30 = true;
			if (npc.direction > 0)
			{
				if (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) > npc.position.X + (float)(npc.width / 2))
					npc.rotation = (float)Math.Atan2((double)(-(double)num358), (double)(-(double)num357)) + 3.14f;
				else
				{
					npc.rotation = 0f;
					flag30 = false;
				}
			}
			else if (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) < npc.position.X + (float)(npc.width / 2))
				npc.rotation = (float)Math.Atan2((double)num358, (double)num357) + 3.14f;
			else
			{
				npc.rotation = 0f;
				flag30 = false;
			}

			// Fire lasers
			if (Main.netMode != NetmodeID.MultiplayerClient && Main.npc[Main.wof].ai[3] == 0f)
			{
				// Percent life remaining
				float lifeRatio = (float)Main.npc[Main.wof].life / (float)Main.npc[Main.wof].lifeMax;

				npc.localAI[1] += 1f + (4f * (1f - lifeRatio));

				if (npc.localAI[2] == 0f)
				{
					if (npc.localAI[1] > 400f)
					{
						npc.localAI[2] = 1f;
						npc.localAI[1] = 0f;
					}
				}
				else if (npc.localAI[1] > (CalamityWorld.death ? 39f : 45f) && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					npc.localAI[1] = 0f;
					npc.localAI[2] += 1f;
					if (npc.localAI[2] >= 4f)
						npc.localAI[2] = 0f;

					if (flag30)
					{
						bool phase2 = lifeRatio < 0.5;
						float velocity = (CalamityWorld.death ? 10f : 9f) + (4f * (1f - lifeRatio));
						int damage = (phase2 ? 17 : 13);
						int projectileType = (phase2 ? ProjectileID.DeathLaser : ProjectileID.EyeLaser);

						vector38 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						num357 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector38.X;
						num358 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector38.Y;
						num359 = (float)Math.Sqrt((double)(num357 * num357 + num358 * num358));
						num359 = velocity / num359;
						num357 *= num359;
						num358 *= num359;
						vector38.X += num357;
						vector38.Y += num358;
						Projectile.NewProjectile(vector38.X, vector38.Y, num357, num358, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}
		#endregion

		#region Buffed Destroyer AI
		public static bool BuffedDestroyerAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);
			bool configBossRushBoost = Config.BossRushXerocCurse && CalamityWorld.bossRushActive;

			// 8 seconds of reistance to prevent spawn killing
			if (calamityGlobalNPC.newAI[1] < 480f)
				calamityGlobalNPC.newAI[1] += 1f;

			// Enrage variable if player is flying upside down
			bool targetFloatingUp = Main.player[npc.target].gravDir == -1f;

			// Percent life remaining
			float lifeRatio = (float)npc.life / (float)npc.lifeMax;

			// Phases based on life percentage
			bool phase2 = lifeRatio < 0.66f;
			bool phase3 = lifeRatio < 0.33f;

			// Set worm variable for worms
			if (npc.ai[3] > 0f)
				npc.realLife = (int)npc.ai[3];

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
				npc.TargetClosest(true);

			// Velocity
			npc.velocity.Length();

			// Dust on spawn and alpha effects
			if (npc.type == NPCID.TheDestroyer || (npc.type != NPCID.TheDestroyer && Main.npc[(int)npc.ai[1]].alpha < 128))
			{
				if (npc.alpha != 0)
				{
					for (int i = 0; i < 2; i++)
					{
						int num = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
						Main.dust[num].noGravity = true;
						Main.dust[num].noLight = true;
					}
				}
				npc.alpha -= 42;
				if (npc.alpha < 0)
					npc.alpha = 0;
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

			if (Main.netMode != NetmodeID.MultiplayerClient)
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
					int randomCount = (CalamityWorld.death ? Main.rand.Next(1260, 23400) : Main.rand.Next(1330, 24700));

					if (calamityGlobalNPC.newAI[0] >= (float)randomCount)
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
							if (npc.ai[2] == 0f || Main.rand.NextBool(2))
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
			fallSpeed += (CalamityWorld.death ? 6f : 4f) * (1f - lifeRatio);

			// Speed and movement
			float speed = (CalamityWorld.death ? 0.11f : 0.1f) + ((targetFloatingUp ? 0.2f : 0.1f) * (1f - lifeRatio));
			float turnSpeed = (CalamityWorld.death ? 0.165f : 0.15f) + ((targetFloatingUp ? 0.3f : 0.15f) * (1f - lifeRatio));
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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

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

			if (Main.rand.NextBool(5))
			{
				int num380 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default, 1f);
				Dust var_9_131D1_cp_0_cp_0 = Main.dust[num380];
				var_9_131D1_cp_0_cp_0.velocity.X = var_9_131D1_cp_0_cp_0.velocity.X * 0.5f;
				Dust var_9_131F5_cp_0_cp_0 = Main.dust[num380];
				var_9_131F5_cp_0_cp_0.velocity.Y = var_9_131F5_cp_0_cp_0.velocity.Y * 0.1f;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient && !Main.dayTime && !Main.player[npc.target].dead && npc.timeLeft < 10)
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

							if (Main.netMode != NetmodeID.MultiplayerClient)
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
							Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
							num = num398;
						}

						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
					}
				}

				Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

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

					if (Main.netMode != NetmodeID.MultiplayerClient)
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

						if (Main.netMode != NetmodeID.MultiplayerClient)
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

								if (Main.netMode != NetmodeID.MultiplayerClient)
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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

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
			if (Main.rand.NextBool(5))
			{
				int num422 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default, 1f);
				Dust dust = Main.dust[num422];
				dust.velocity.X = dust.velocity.X * 0.5f;
				dust.velocity.Y = dust.velocity.Y * 0.1f;
			}

			// Despawn Twins at the same time
			if (Main.netMode != NetmodeID.MultiplayerClient && !Main.dayTime && !Main.player[npc.target].dead && npc.timeLeft < 10)
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
							if (Main.netMode != NetmodeID.MultiplayerClient)
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
							Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);
							num = num439;
						}

						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0, 1f, 0f);
					}
				}

				Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

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

						if (Main.netMode != NetmodeID.MultiplayerClient)
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
						else if (Main.netMode != NetmodeID.MultiplayerClient)
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
							if (Main.netMode != NetmodeID.MultiplayerClient)
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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

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
			if (npc.ai[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
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
			bool phase2 = (double)npc.life < (double)npc.lifeMax * 0.66;
			bool phase3 = (double)npc.life < (double)npc.lifeMax * 0.33;

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
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int healAmt = npc.life - 300;
					if (healAmt < 0)
					{
						int absHeal = Math.Abs(healAmt);
						npc.life += absHeal;
						npc.HealEffect(absHeal, true);
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

				if (Main.netMode != NetmodeID.MultiplayerClient)
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

				float velocity2 = 2f;
				float acceleration = 0.1f;
				if (!cannonAlive)
				{
					velocity2 += 1f;
					acceleration += 0.025f;
				}
				if (!laserAlive)
				{
					velocity2 += 1f;
					acceleration += 0.025f;
				}
				if (!viceAlive)
				{
					velocity2 += 1f;
					acceleration += 0.025f;
				}
				if (!sawAlive)
				{
					velocity2 += 1f;
					acceleration += 0.025f;
				}

				if (npc.position.Y > Main.player[npc.target].position.Y - 350f)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y = npc.velocity.Y * 0.98f;

					npc.velocity.Y = npc.velocity.Y - acceleration;

					if (npc.velocity.Y > velocity2)
						npc.velocity.Y = velocity2;
				}
				else if (npc.position.Y < Main.player[npc.target].position.Y - 500f)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y = npc.velocity.Y * 0.98f;

					npc.velocity.Y = npc.velocity.Y + acceleration;

					if (npc.velocity.Y < -velocity2)
						npc.velocity.Y = -velocity2;
				}

				if (npc.position.X + (float)(npc.width / 2) > Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + 150f)
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X * 0.98f;

					npc.velocity.X = npc.velocity.X - 0.1f;

					if (npc.velocity.X > 8f)
						npc.velocity.X = 8f;
				}
				if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - 150f)
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

					if (Main.netMode != NetmodeID.MultiplayerClient)
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

							if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 64f)
							{
								if (Main.netMode != NetmodeID.MultiplayerClient)
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

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Vector2 velocity = new Vector2(-1f * (float)Main.rand.NextDouble() * 3f, 1f);
							velocity = velocity.RotatedBy((Main.rand.NextDouble() - 0.5) * 0.78539818525314331, default);
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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

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
				if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
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

				if (Main.netMode != NetmodeID.MultiplayerClient)
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
				float num514 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 120f - vector63.Y; // Used to be 20f
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

				if (Main.netMode != NetmodeID.MultiplayerClient)
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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}

		public static bool BuffedPrimeCannonAI(NPC npc, Mod mod)
		{
			npc.spriteDirection = -(int)npc.ai[0];

			if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 32)
			{
				npc.ai[2] += 10f;
				if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
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

				if (Main.netMode != NetmodeID.MultiplayerClient)
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

				if (Main.netMode != NetmodeID.MultiplayerClient)
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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

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
				if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
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
						float chargeVelocity = 16f;
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
						float chargeVelocity = 14f;
						if (!cannonAlive)
							chargeVelocity += 1.15f;
						if (!laserAlive)
							chargeVelocity += 1.15f;
						if (!sawAlive)
							chargeVelocity += 1.15f;

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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

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
				if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
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

					// Start charging after 3 seconds (change this as each arm dies)
					npc.ai[3] += 1f;
					if (!cannonAlive)
						npc.ai[3] += 1f;
					if (!laserAlive)
						npc.ai[3] += 1f;
					if (!viceAlive)
						npc.ai[3] += 1f;

					if (npc.ai[3] >= (CalamityWorld.death ? 165f : 180f))
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
					npc.velocity.Y = npc.velocity.Y - 0.3f;
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
						float chargeVelocity = 11f;
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
						if (npc.justHit)
							npc.ai[3] += 2f;

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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}
		#endregion

		#region Buffed Plantera AI
		public static bool BuffedPlanteraAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);

			// Percent life remaining
			float lifeRatio = (float)npc.life / (float)npc.lifeMax;

			// Phases based on HP
			bool phase2 = lifeRatio <= 0.5f;
			bool phase3 = lifeRatio < 0.25f;

			// Variables and target
			bool enrage = false;
			bool despawn = false;
			npc.TargetClosest(true);

			// Check for Jungle and remaining Tentacles
			bool surface = (double)Main.player[npc.target].position.Y < Main.worldSurface * 16.0;
			int tentacleCount = NPC.CountNPCS(NPCID.PlanterasTentacle);
			bool tentaclesDead = tentacleCount == 0;
			bool speedUp = Vector2.Distance(Main.player[npc.target].Center, npc.Center) > (phase3 ? 480f : 640f); // 30 or 40 tile distance

			// Despawn
			if (Main.player[npc.target].dead)
			{
				despawn = true;
				enrage = true;
			}

			// Despawn if too far from target
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 6000f)
				{
					npc.active = false;
					npc.life = 0;
					if (Main.netMode == NetmodeID.Server)
						NetMessage.SendData(23, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
				}
			}

			// Set whoAmI variable and spawn hooks
			NPC.plantBoss = npc.whoAmI;
			if (npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
			{
				npc.localAI[0] = 1f;
				int num729 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasHook, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				num729 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasHook, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				num729 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasHook, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
			}

			// Find positions of hooks
			int[] array2 = new int[3];
			float num730 = 0f;
			float num731 = 0f;
			int num732 = 0;
			int num;
			for (int num733 = 0; num733 < 200; num733 = num + 1)
			{
				if (Main.npc[num733].active && Main.npc[num733].aiStyle == 52)
				{
					num730 += Main.npc[num733].Center.X;
					num731 += Main.npc[num733].Center.Y;
					array2[num732] = num733;

					num732++;
					if (num732 > 2)
						break;
				}
				num = num733;
			}
			num730 /= (float)num732;
			num731 /= (float)num732;

			// Velocity and acceleration
			float velocity = 6f;
			float acceleration = 0.06f;
			if (phase3)
			{
				velocity = 10f;
				acceleration = 0.1f;
				if (tentaclesDead)
				{
					velocity = 12f;
					acceleration = 0.12f;
				}
			}
			else if (phase2)
			{
				velocity = 8f;
				acceleration = 0.08f;
			}

			// Move faster if inside active tiles
			int radius = 2; // 2 tile radius
			int diameter = radius * 2;
			int npcCenterX = (int)(npc.Center.X / 16f);
			int npcCenterY = (int)(npc.Center.Y / 16f);
			Rectangle area = new Rectangle(npcCenterX - radius, npcCenterY - radius, diameter, diameter);
			bool insideTiles = false;
			for (int x = area.Left; x < area.Right; x++)
			{
				for (int y = area.Top; y < area.Bottom; y++)
				{
					if (Main.tile[x, y] != null)
					{
						if (Main.tile[x, y].active() && Main.tileSolid[Main.tile[x, y].type] && !Main.tileSolidTop[Main.tile[x, y].type] && !TileID.Sets.Platforms[Main.tile[x, y].type])
							insideTiles = true;
					}
				}
			}

			// Slow down if close to target and not inside tiles
			if (!speedUp && !insideTiles)
			{
				velocity = 4f;
				acceleration = 0.04f;
			}

			// Enrage if target is on the surface
			if (surface || Main.player[npc.target].position.Y > (float)((Main.maxTilesY - 200) * 16))
			{
				enrage = true;
				velocity += 8f;
				acceleration = 0.15f;
			}

			if (CalamityWorld.death)
			{
				velocity += 1f;
				acceleration += 0.01f;
			}

			// Detect active tiles around Plantera
			radius = 20; // 20 tile radius
			diameter = radius * 2;
			area = new Rectangle(npcCenterX - radius, npcCenterY - radius, diameter, diameter);
			int nearbyActiveTiles = 0; // 0 to 1600
			for (int x = area.Left; x < area.Right; x++)
			{
				for (int y = area.Top; y < area.Bottom; y++)
				{
					if (Main.tile[x, y] != null)
					{
						if (Main.tile[x, y].active() && !Main.tile[x, y].actuator() && Main.tileSolid[Main.tile[x, y].type] && !Main.tileSolidTop[Main.tile[x, y].type] && !TileID.Sets.Platforms[Main.tile[x, y].type])
							nearbyActiveTiles++;
					}
				}
			}

			// Scale multiplier based on nearby active tiles
			float tileEnrageMult = 1f;
			if (nearbyActiveTiles < 800)
				tileEnrageMult += (float)(800 - nearbyActiveTiles) * 0.0015f; // Ranges from 1f to 2.2f

			// Movement relative to the target and hook positions
			Vector2 vector91 = new Vector2(num730, num731);
			float num736 = Main.player[npc.target].Center.X - vector91.X;
			float num737 = Main.player[npc.target].Center.Y - vector91.Y;
			if (despawn)
			{
				num737 *= -1f;
				num736 *= -1f;
				velocity += 8f;
			}
			float num738 = (float)Math.Sqrt((double)(num736 * num736 + num737 * num737));

			// Increase speed based on nearby active tiles
			velocity *= tileEnrageMult;
			acceleration *= tileEnrageMult;

			// Velocity ranges from 4 to 7.2, Acceleration ranges from 0.04 to 0.072, non-enraged phase 1
			// Velocity ranges from 7 to 12.6, Acceleration ranges from 0.07 to 0.126, non-enraged phase 2
			// Velocity ranges from 9 to 16.2, Acceleration ranges from 0.07 to 0.126, non-enraged phase 3
			// Velocity ranges from 17 to 30.6, Acceleration ranges from 0.15 to 0.27, enraged phase 3

			// Distance Plantera can travel from her hooks
			float maxDistanceFromHooks = enrage ? 850f : 550f;
			if (phase3)
				maxDistanceFromHooks += 150f;
			if (CalamityWorld.death)
				maxDistanceFromHooks += 50f;

			if (num738 >= maxDistanceFromHooks)
			{
				num738 = maxDistanceFromHooks / num738;
				num736 *= num738;
				num737 *= num738;
			}

			num730 += num736;
			num731 += num737;
			vector91 = new Vector2(npc.Center.X, npc.Center.Y);
			num736 = num730 - vector91.X;
			num737 = num731 - vector91.Y;
			num738 = (float)Math.Sqrt((double)(num736 * num736 + num737 * num737));

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
				npc.velocity.X = npc.velocity.X + acceleration;
				if (npc.velocity.X < 0f && num736 > 0f)
					npc.velocity.X = npc.velocity.X + acceleration * 2f;
			}
			else if (npc.velocity.X > num736)
			{
				npc.velocity.X = npc.velocity.X - acceleration;
				if (npc.velocity.X > 0f && num736 < 0f)
					npc.velocity.X = npc.velocity.X - acceleration * 2f;
			}
			if (npc.velocity.Y < num737)
			{
				npc.velocity.Y = npc.velocity.Y + acceleration;
				if (npc.velocity.Y < 0f && num737 > 0f)
					npc.velocity.Y = npc.velocity.Y + acceleration * 2f;
			}
			else if (npc.velocity.Y > num737)
			{
				npc.velocity.Y = npc.velocity.Y - acceleration;
				if (npc.velocity.Y > 0f && num737 < 0f)
					npc.velocity.Y = npc.velocity.Y - acceleration * 2f;
			}

			// Slow down considerably if near player
			if (!speedUp && nearbyActiveTiles > 800 && !insideTiles)
			{
				if (npc.velocity.Length() > velocity)
					npc.velocity *= 0.97f;
			}

			// Rotation
			Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
			float num740 = Main.player[npc.target].Center.X - vector92.X;
			float num741 = Main.player[npc.target].Center.Y - vector92.Y;
			npc.rotation = (float)Math.Atan2((double)num741, (double)num740) + 1.57f;

			// Phase 1
			if (!phase2)
			{
				// Adjust stats
				npc.defense = 42;
				npc.damage = (int)(50f * Main.damageMultiplier);
				if (enrage)
				{
					npc.defense *= 2;
					npc.damage *= 2;
				}

				// Fire projectiles
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[1] += 1f + (8f * (1f - lifeRatio));

					if (enrage)
					{
						npc.localAI[1] += 3f;

						// If hit, fire projectiles even if target is behind tiles
						if (npc.justHit && Main.rand.NextBool(2))
							npc.localAI[3] = 1f;
					}

					if (npc.localAI[1] >= (CalamityWorld.death ? 65f : 75f))
					{
						npc.localAI[1] = 0f;
						bool canHit = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
						if (npc.localAI[3] > 0f)
						{
							canHit = true;
							npc.localAI[3] = 0f;
						}
						if (canHit)
						{
							Vector2 vector93 = new Vector2(npc.Center.X, npc.Center.Y);
							float num742 = 18f;
							float num743 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector93.X;
							float num744 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector93.Y;
							float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));
							num745 = num742 / num745;
							num743 *= num745;
							num744 *= num745;

							int damage = 22;
							int projectileType = ProjectileID.SeedPlantera;
							if (lifeRatio < 0.9f && Main.rand.NextBool(2))
							{
								damage = 27;
								npc.localAI[1] = -30f;
								projectileType = ProjectileID.PoisonSeedPlantera;
							}
							else if (lifeRatio < 0.8f && Main.rand.NextBool(4))
							{
								int thornBallCount = 0;
								for (int i = 0; i < 1000; i++)
								{
									if (Main.projectile[i].active && Main.projectile[i].type == ProjectileID.ThornBall)
										thornBallCount++;

									if (thornBallCount > 2)
										break;
								}
								if (thornBallCount < 3)
								{
									damage = 31;
									npc.localAI[1] = -120f;
									projectileType = ProjectileID.ThornBall;
								}
							}

							if (enrage)
								damage *= 2;

							vector93.X += num743 * 3f;
							vector93.Y += num744 * 3f;
							int num748 = Projectile.NewProjectile(vector93.X, vector93.Y, num743, num744, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
							if (projectileType == ProjectileID.SeedPlantera)
								Main.projectile[num748].timeLeft = 300;
						}
					}
				}
			}

			// Phase 2
			else
			{
				// Adjust stats
				npc.defense = 21;
				npc.damage = (int)(70f * Main.damageMultiplier);
				if (enrage)
				{
					npc.defense *= 4;
					npc.damage *= 2;
				}

				// Spawn tentacles
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					if (npc.localAI[0] == 1f)
					{
						npc.localAI[0] = 2f;
						for (int num749 = 0; num749 < 8; num749 = num + 1)
						{
							int num750 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.PlanterasTentacle, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
							num = num749;
						}
					}
				}

				// If tentacles are alive, gain high defense
				if (!tentaclesDead)
					npc.defense = 9999;
				npc.chaseable = tentaclesDead;

				// Spawn gore
				if (npc.localAI[2] == 0f)
				{
					Gore.NewGore(new Vector2(npc.position.X + (float)Main.rand.Next(npc.width), npc.position.Y + (float)Main.rand.Next(npc.height)), npc.velocity, 378, npc.scale);
					Gore.NewGore(new Vector2(npc.position.X + (float)Main.rand.Next(npc.width), npc.position.Y + (float)Main.rand.Next(npc.height)), npc.velocity, 379, npc.scale);
					Gore.NewGore(new Vector2(npc.position.X + (float)Main.rand.Next(npc.width), npc.position.Y + (float)Main.rand.Next(npc.height)), npc.velocity, 380, npc.scale);
					npc.localAI[2] = 1f;
				}

				// Spawn spores
				npc.localAI[1] += 1f + (8f * (0.5f - lifeRatio));

				if (npc.localAI[1] >= (CalamityWorld.death ? 300f : 360f))
				{
					float num757 = 8f;
					Vector2 vector94 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num758 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector94.X + (float)Main.rand.Next(-10, 11);
					float num759 = Math.Abs(num758 * 0.2f);

					float num760 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector94.Y + (float)Main.rand.Next(-10, 11);
					if (num760 > 0f)
						num759 = 0f;

					num760 -= num759;
					float num761 = (float)Math.Sqrt((double)(num758 * num758 + num760 * num760));
					num761 = num757 / num761;
					num758 *= num761;
					num760 *= num761;

					int num762 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.Spore, 0, 0f, 0f, 0f, 0f, 255);
					Main.npc[num762].velocity.X = num758;
					Main.npc[num762].velocity.Y = num760;
					Main.npc[num762].netUpdate = true;
					npc.localAI[1] = 0f;
				}

				// Fire spread of poison seeds
				if (tentacleCount < 8)
				{
					int tentacleScale = 8 - tentacleCount; // 1 to 8

					if (nearbyActiveTiles > 600)
						npc.localAI[3] += 0.5f + ((float)(tentacleScale - 1) * 0.5f);
					else
						npc.localAI[3] += (nearbyActiveTiles > 300 ? 1f : 5f) + (float)(tentacleScale - 1);

					if (npc.localAI[3] >= (CalamityWorld.death ? 330f : 360f))
					{
						Vector2 vector93 = new Vector2(npc.Center.X, npc.Center.Y);

						float num742 = 10f - ((float)tentacleScale * 0.5f); // 9.5f to 6f, slower projectiles are harder to avoid
						if (nearbyActiveTiles <= 300)
							num742 = 10f;

						float num743 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector93.X;
						float num744 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector93.Y;
						float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));
						num745 = num742 / num745;
						num743 *= num745;
						num744 *= num745;
						vector93.X += num743 * 3f;
						vector93.Y += num744 * 3f;

						int damage = 24;
						int numProj = 2;

						int spread = 2 + tentacleScale; // 3 to 10, wider spread is harder to avoid
						if (nearbyActiveTiles <= 300)
							spread = (Main.rand.NextBool(2) ? 3 : 6) + (tentacleScale / 2);

						float rotation = MathHelper.ToRadians(spread);
						for (int i = 0; i < numProj + 1; i++)
						{
							Vector2 perturbedSpeed = new Vector2(num743, num744).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
							Projectile.NewProjectile(vector93.X, vector93.Y, perturbedSpeed.X, perturbedSpeed.Y, ProjectileID.PoisonSeedPlantera, damage, 0f, Main.myPlayer, 0f, 0f);
						}
						npc.localAI[3] = 0f;
					}
				}

				// Fire spread of spore clouds
				if (tentaclesDead)
				{
					calamityGlobalNPC.newAI[0] += 1f + (2f * (0.5f - lifeRatio));

					if (calamityGlobalNPC.newAI[0] >= (CalamityWorld.death ? 270f : 300f))
					{
						Main.PlaySound(SoundID.Item20, npc.position);

						Vector2 vector93 = new Vector2(npc.Center.X, npc.Center.Y);
						float num742 = 7f;
						float num743 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector93.X;
						float num744 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector93.Y;
						float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));
						num745 = num742 / num745;
						num743 *= num745;
						num744 *= num745;
						vector93.X += num743 * 3f;
						vector93.Y += num744 * 3f;

						int damage = 26;
						int numProj = 4;

						int spread = 30;
						if (nearbyActiveTiles <= 300)
							spread = (Main.rand.NextBool(2) ? 30 : 45);

						float rotation = MathHelper.ToRadians(spread);
						float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
						double startAngle = Math.Atan2(num743, num744) - rotation / 2;
						double deltaAngle = rotation / (float)numProj;
						double offsetAngle;

						for (int i = 0; i < numProj; i++)
						{
							offsetAngle = startAngle + deltaAngle * i;
							float ai0 = (float)Main.rand.Next(3);
							Projectile.NewProjectile(vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), mod.ProjectileType("SporeGasPlantera"), damage, 0f, Main.myPlayer, ai0, 0f);
						}

						calamityGlobalNPC.newAI[0] = 0f;
					}
				}
			}

			// Heal if on surface and it's daytime, else, gain defense
			if (surface)
			{
				if (Main.dayTime)
				{
					if (Main.rand.NextBool(3))
					{
						int dust = Dust.NewDust(npc.position, npc.width, npc.height, 55, 0f, 0f, 200, default, 0.5f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 0.75f;
						Main.dust[dust].fadeIn = 1.3f;
						Vector2 vector = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
						vector.Normalize();
						vector *= (float)Main.rand.Next(50, 100) * 0.04f;
						Main.dust[dust].velocity = vector;
						vector.Normalize();
						vector *= 86f;
						Main.dust[dust].position = npc.Center - vector;
					}

					// Heal, 25 seconds to reach full HP from 0
					calamityGlobalNPC.newAI[1] += 1f;
					if (calamityGlobalNPC.newAI[1] >= 15f)
					{
						calamityGlobalNPC.newAI[1] = 0f;
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							int healAmt = npc.lifeMax / (CalamityWorld.bossRushActive ? 400 : 100);
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
				else
				{
					npc.defense += 250;
					if (npc.defense > 9999)
						npc.defense = 9999;
				}
			}

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}

		public static bool BuffedPlanterasHookAI(NPC npc, Mod mod)
		{
			// Variables
			bool enrage = false;
			bool despawn = false;

			// Percent life remaining, Plantera
			float lifeRatio = (float)Main.npc[NPC.plantBoss].life / (float)Main.npc[NPC.plantBoss].lifeMax;

			// Despawn if Plantera is gone
			if (NPC.plantBoss < 0 || !Main.npc[NPC.plantBoss].active)
			{
				npc.StrikeNPCNoInteraction(9999, 0f, 0, false, false, false);
				npc.netUpdate = true;
				return false;
			}

			// Despawn if Plantera's target is dead
			if (Main.player[Main.npc[NPC.plantBoss].target].dead)
				despawn = true;

			// Enrage if Plantera's target is on the surface
			if (((double)Main.player[Main.npc[NPC.plantBoss].target].position.Y < Main.worldSurface * 16.0 || Main.player[Main.npc[NPC.plantBoss].target].position.Y > (float)((Main.maxTilesY - 200) * 16)) | despawn)
			{
				npc.localAI[0] -= 4f;
				enrage = true;
			}

			// Set centers for movement
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (npc.ai[0] == 0f)
					npc.ai[0] = (float)((int)(npc.Center.X / 16f));
				if (npc.ai[1] == 0f)
					npc.ai[1] = (float)((int)(npc.Center.X / 16f));
			}

			// Find new spot to move to after set time has passed
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				// Move immediately
				if (npc.ai[0] == 0f || npc.ai[1] == 0f)
					npc.localAI[0] = 0f;

				// Timer dictating whether to pick a new location or not
				npc.localAI[0] -= 1f + (2f * (1f - lifeRatio));
				if (enrage)
					npc.localAI[0] -= 6f;
				if (CalamityWorld.death)
					npc.localAI[0] -= 0.5f;

				// Set timer to new amount if a different hook is currently moving
				if (!despawn && npc.localAI[0] <= 0f && npc.ai[0] != 0f)
				{
					int num;
					for (int num763 = 0; num763 < 200; num763 = num + 1)
					{
						if (num763 != npc.whoAmI && Main.npc[num763].active && Main.npc[num763].type == npc.type && (Main.npc[num763].velocity.X != 0f || Main.npc[num763].velocity.Y != 0f))
							npc.localAI[0] = (float)Main.rand.Next(60, 300);

						num = num763;
					}
				}

				// Pick a location to move to
				if (npc.localAI[0] <= 0f)
				{
					// Reset timer
					npc.localAI[0] = (float)Main.rand.Next(300, 600);

					// Pick location
					bool flag50 = false;
					int num764 = 0;
					while (!flag50 && num764 <= 1000)
					{
						num764++;

						int num765 = (int)(Main.player[Main.npc[NPC.plantBoss].target].Center.X / 16f);
						int num766 = (int)(Main.player[Main.npc[NPC.plantBoss].target].Center.Y / 16f);

						if (npc.ai[0] == 0f)
						{
							num765 = (int)((Main.player[Main.npc[NPC.plantBoss].target].Center.X + Main.npc[NPC.plantBoss].Center.X) / 32f);
							num766 = (int)((Main.player[Main.npc[NPC.plantBoss].target].Center.Y + Main.npc[NPC.plantBoss].Center.Y) / 32f);
						}

						if (despawn)
						{
							num765 = (int)Main.npc[NPC.plantBoss].position.X / 16;
							num766 = (int)(Main.npc[NPC.plantBoss].position.Y + 400f) / 16;
						}

						int num767 = 20;
						num767 += (int)(100f * ((float)num764 / 1000f));
						int num768 = num765 + Main.rand.Next(-num767, num767 + 1);
						int num769 = num766 + Main.rand.Next(-num767, num767 + 1);

						try
						{
							if (WorldGen.SolidTile(num768, num769) || (Main.tile[num768, num769].wall > 0 && (num764 > 500 || lifeRatio < 0.5f)))
							{
								flag50 = true;
								npc.ai[0] = (float)num768;
								npc.ai[1] = (float)num769;
								npc.netUpdate = true;
							}
						}
						catch
						{
						}
					}
				}
			}

			// Movement to new location
			if (npc.ai[0] > 0f && npc.ai[1] > 0f)
			{
				// Hook movement velocity
				float velocity = 7f + (3f * (1f - lifeRatio));
				if (CalamityWorld.death)
					velocity += 1f;
				if (enrage)
					velocity *= 2f;
				if (despawn)
					velocity *= 2f;

				// Moving to new location
				Vector2 vector95 = new Vector2(npc.Center.X, npc.Center.Y);
				float num773 = npc.ai[0] * 16f - 8f - vector95.X;
				float num774 = npc.ai[1] * 16f - 8f - vector95.Y;
				float num775 = (float)Math.Sqrt((double)(num773 * num773 + num774 * num774));
				if (num775 < 12f + velocity)
				{
					npc.velocity.X = num773;
					npc.velocity.Y = num774;
				}
				else
				{
					num775 = velocity / num775;
					npc.velocity.X = num773 * num775;
					npc.velocity.Y = num774 * num775;
				}

				// Rotation
				Vector2 vector96 = new Vector2(npc.Center.X, npc.Center.Y);
				float num776 = Main.npc[NPC.plantBoss].Center.X - vector96.X;
				float num777 = Main.npc[NPC.plantBoss].Center.Y - vector96.Y;
				npc.rotation = (float)Math.Atan2((double)num777, (double)num776) - 1.57f;
			}
			return false;
		}

		public static bool BuffedPlanterasTentacleAI(NPC npc, Mod mod)
		{
			// Despawn if Plantera is gone
			if (NPC.plantBoss < 0 || !Main.npc[NPC.plantBoss].active)
			{
				npc.StrikeNPCNoInteraction(9999, 0f, 0, false, false, false);
				npc.netUpdate = true;
				return false;
			}

			// Set Plantera to a variable
			int num778 = NPC.plantBoss;
			if (npc.ai[3] > 0f)
				num778 = (int)npc.ai[3] - 1;

			// Movement variables
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				npc.localAI[0] -= 1f;
				if (npc.localAI[0] <= 0f)
				{
					npc.localAI[0] = (float)Main.rand.Next(120, 480);
					npc.ai[0] = (float)Main.rand.Next(-100, 101);
					npc.ai[1] = (float)Main.rand.Next(-100, 101);
					npc.netUpdate = true;
				}
			}

			// Target
			npc.TargetClosest(true);

			// Velocity and acceleration
			float num779 = 0.5f;
			float num781 = 1f - (float)npc.life / (float)npc.lifeMax;
			float num780 = 200f + (num781 * 50f);
			if (CalamityWorld.death)
				num780 += 25f;

			// Despawn if Plantera is gone
			if (!Main.npc[num778].active || NPC.plantBoss < 0)
			{
				npc.active = false;
				return false;
			}

			// Movement
			float num782 = Main.npc[num778].position.X + (float)(Main.npc[num778].width / 2);
			float num783 = Main.npc[num778].position.Y + (float)(Main.npc[num778].height / 2);
			Vector2 vector97 = new Vector2(num782, num783);
			float num784 = num782 + npc.ai[0];
			float num785 = num783 + npc.ai[1];
			float num786 = num784 - vector97.X;
			float num787 = num785 - vector97.Y;
			float num788 = (float)Math.Sqrt((double)(num786 * num786 + num787 * num787));
			num788 = num780 / num788;
			num786 *= num788;
			num787 *= num788;
			if (npc.position.X < num782 + num786)
			{
				npc.velocity.X = npc.velocity.X + num779;
				if (npc.velocity.X < 0f && num786 > 0f)
					npc.velocity.X = npc.velocity.X * 0.9f;
			}
			else if (npc.position.X > num782 + num786)
			{
				npc.velocity.X = npc.velocity.X - num779;
				if (npc.velocity.X > 0f && num786 < 0f)
					npc.velocity.X = npc.velocity.X * 0.9f;
			}
			if (npc.position.Y < num783 + num787)
			{
				npc.velocity.Y = npc.velocity.Y + num779;
				if (npc.velocity.Y < 0f && num787 > 0f)
					npc.velocity.Y = npc.velocity.Y * 0.9f;
			}
			else if (npc.position.Y > num783 + num787)
			{
				npc.velocity.Y = npc.velocity.Y - num779;
				if (npc.velocity.Y > 0f && num787 < 0f)
					npc.velocity.Y = npc.velocity.Y * 0.9f;
			}
			if (npc.velocity.X > 8f)
				npc.velocity.X = 8f;
			if (npc.velocity.X < -8f)
				npc.velocity.X = -8f;
			if (npc.velocity.Y > 8f)
				npc.velocity.Y = 8f;
			if (npc.velocity.Y < -8f)
				npc.velocity.Y = -8f;

			// Direction and rotation
			if (num786 > 0f)
			{
				npc.spriteDirection = 1;
				npc.rotation = (float)Math.Atan2((double)num787, (double)num786);
			}
			if (num786 < 0f)
			{
				npc.spriteDirection = -1;
				npc.rotation = (float)Math.Atan2((double)num787, (double)num786) + 3.14f;
			}
			return false;
		}
        #endregion

        #region Buffed Golem AI
        public static bool BuffedGolemAI(NPC npc, bool enraged, Mod mod)
        {
            // whoAmI variable
            NPC.golemBoss = npc.whoAmI;

            // Percent life remaining
            float lifeRatio = (float)npc.life / (float)npc.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.75f;
            bool phase3 = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < 0.25f;

            // Spawn parts
            if (npc.localAI[0] == 0f && Main.netMode != 1)
            {
                npc.localAI[0] = 1f;
                NPC.NewNPC((int)npc.Center.X - 84, (int)npc.Center.Y - 9, NPCID.GolemFistLeft, 0, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)npc.Center.X + 78, (int)npc.Center.Y - 9, NPCID.GolemFistRight, 0, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)npc.Center.X - 3, (int)npc.Center.Y - 57, NPCID.GolemHead, 0, 0f, 0f, 0f, 0f, 255);
            }

            // Despawn
            if (npc.target >= 0 && Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
                if (Main.player[npc.target].dead)
                    npc.noTileCollide = true;
            }

            // Enrage if the target isn't inside the temple
            bool enrage = true;
            if ((double)Main.player[npc.target].Center.Y > Main.worldSurface * 16.0)
            {
                int num = (int)Main.player[npc.target].Center.X / 16;
                int num2 = (int)Main.player[npc.target].Center.Y / 16;

                Tile tile = Framing.GetTileSafely(num, num2);
                if (tile.wall == WallID.LihzahrdBrickUnsafe)
                    enrage = false;
            }

            // Alpha
            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                npc.ai[1] = 0f;
            }

            // Check for body parts
            bool flag40 = NPC.AnyNPCs(NPCID.GolemHead);
            bool flag41 = NPC.AnyNPCs(NPCID.GolemFistLeft);
            bool flag42 = NPC.AnyNPCs(NPCID.GolemFistRight);
            npc.dontTakeDamage = flag40 || flag41 || flag42;

            // Spawn arm dust
            if (!flag41)
            {
                int num642 = Dust.NewDust(new Vector2(npc.Center.X - 80f, npc.Center.Y - 9f), 8, 8, 31, 0f, 0f, 100, default(Color), 1f);
                Dust dust = Main.dust[num642];
                dust.alpha += Main.rand.Next(100);
                dust.velocity *= 0.2f;
                dust.velocity.Y = dust.velocity.Y - (0.5f + (float)Main.rand.Next(10) * 0.1f);
                dust.fadeIn = 0.5f + (float)Main.rand.Next(10) * 0.1f;

                if (Main.rand.Next(10) == 0)
                {
                    num642 = Dust.NewDust(new Vector2(npc.Center.X - 80f, npc.Center.Y - 9f), 8, 8, 6, 0f, 0f, 0, default(Color), 1f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[num642].noGravity = true;
                        dust = Main.dust[num642];
                        dust.scale *= 1f + (float)Main.rand.Next(10) * 0.1f;
                        dust.velocity.Y = dust.velocity.Y - 1f;
                    }
                }
            }
            if (!flag42)
            {
                int num643 = Dust.NewDust(new Vector2(npc.Center.X + 62f, npc.Center.Y - 9f), 8, 8, 31, 0f, 0f, 100, default(Color), 1f);
                Dust dust = Main.dust[num643];
                dust.alpha += Main.rand.Next(100);
                dust.velocity *= 0.2f;
                dust.velocity.Y = dust.velocity.Y - (0.5f + (float)Main.rand.Next(10) * 0.1f);
                dust.fadeIn = 0.5f + (float)Main.rand.Next(10) * 0.1f;

                if (Main.rand.Next(10) == 0)
                {
                    num643 = Dust.NewDust(new Vector2(npc.Center.X + 62f, npc.Center.Y - 9f), 8, 8, 6, 0f, 0f, 0, default(Color), 1f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[num643].noGravity = true;
                        dust = Main.dust[num643];
                        dust.scale *= 1f + (float)Main.rand.Next(10) * 0.1f;
                        dust.velocity.Y = dust.velocity.Y - 1f;
                    }
                }
            }

            // Jump
            if (npc.ai[0] == 0f)
            {
                npc.noTileCollide = false;

                if (npc.velocity.Y == 0f)
                {
                    // Laser fire when head is dead
                    if (Main.netMode != 1 && !flag40)
                    {
                        npc.localAI[1] += 1f;

                        float divisor = 14f -
                            (phase2 ? 7f : 0f) -
                            (phase3 ? 3f : 0f) -
                            (phase4 ? 1f : 0f);

                        if (enrage)
                            divisor = 3f;

                        if (npc.localAI[1] % divisor == 0f && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 160f)
                        {
                            Vector2 vector82 = new Vector2(npc.Center.X, npc.Center.Y - 40f);
                            float num673 = enrage ? 12f : 6f;
                            float num674 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector82.X;
                            float num675 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector82.Y;
                            float num676 = (float)Math.Sqrt((double)(num674 * num674 + num675 * num675));

                            num676 = num673 / num676;
                            num674 *= num676;
                            num675 *= num676;
                            vector82.X += num674 * 3f;
                            vector82.Y += num675 * 3f;

                            if (Main.netMode != 1)
                            {
                                int num677 = Projectile.NewProjectile(vector82.X, vector82.Y, num674, num675, ProjectileID.EyeBeam, 35, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[num677].timeLeft = 480;
                            }
                        }

                        if (npc.localAI[1] >= 15f)
                            npc.localAI[1] = 0f;
                    }

                    // Slow down
                    npc.velocity.X = npc.velocity.X * 0.8f;

                    // Delay before jumping
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 0f)
                    {
                        npc.ai[1] += 1f;
                        if (enrage)
                            npc.ai[1] += 18f;
                        else
                        {
                            if (!flag41)
                                npc.ai[1] += 6f;
                            if (!flag42)
                                npc.ai[1] += 6f;
                            if (!flag40)
                                npc.ai[1] += 6f;
                        }
                    }
                    if (npc.ai[1] >= 300f)
                    {
                        npc.ai[1] = -20f;
                        npc.frameCounter = 0.0;
                    }
                    else if (npc.ai[1] == -1f)
                    {
                        // Set jump velocity
                        npc.TargetClosest(true);

                        float velocityX = 4f + (8f * (1f - lifeRatio));
                        if (enrage)
                            velocityX *= 1.5f;

                        npc.velocity.X = velocityX * (float)npc.direction;
                        npc.velocity.Y = -12.1f;

                        if (npc.target >= 0 && Main.player[npc.target].position.Y < npc.position.Y + (float)npc.height)
                            npc.noTileCollide = true;

                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                    }
                }
            }

            // Fall down
            else if (npc.ai[0] == 1f)
            {
                if (npc.velocity.Y == 0f)
                {
                    // Play sound
                    Main.PlaySound(SoundID.Item14, npc.position);

                    npc.ai[0] = 0f;

                    // Dust and gore
                    for (int num644 = (int)npc.position.X - 20; num644 < (int)npc.position.X + npc.width + 40; num644 += 20)
                    {
                        for (int num645 = 0; num645 < 4; num645++)
                        {
                            int num646 = Dust.NewDust(new Vector2(npc.position.X - 20f, npc.position.Y + (float)npc.height), npc.width + 20, 4, 31, 0f, 0f, 100, default(Color), 1.5f);
                            Dust dust = Main.dust[num646];
                            dust.velocity *= 0.2f;
                        }
                        int num647 = Gore.NewGore(new Vector2((float)(num644 - 20), npc.position.Y + (float)npc.height - 8f), default(Vector2), Main.rand.Next(61, 64), 1f);
                        Gore gore = Main.gore[num647];
                        gore.velocity *= 0.4f;
                    }

                    // Fireball explosion when head is dead
                    if (Main.netMode != 1 && !flag40)
                    {
                        for (int num621 = 0; num621 < 10; num621++)
                        {
                            int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default(Color), 2f);
                            Main.dust[num622].velocity.Y *= 6f;
                            Main.dust[num622].velocity.X *= 3f;
                            if (Main.rand.Next(2) == 0)
                            {
                                Main.dust[num622].scale = 0.5f;
                                Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int num623 = 0; num623 < 20; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default(Color), 3f);
                            Main.dust[num624].noGravity = true;
                            Main.dust[num624].velocity.Y *= 10f;
                            num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default(Color), 2f);
                            Main.dust[num624].velocity.X *= 2f;
                        }

                        int spawnX = (int)(npc.width / 2);
                        for (int i = 0; i < 5; i++)
                        {
                            float velocityX = Main.rand.NextBool() ? (float)Main.rand.Next(5, 7) : (float)Main.rand.Next(-6, -4);
                            float velocityY = (float)Main.rand.Next(-2, 3);

                            if (enrage)
                            {
                                velocityX *= 2f;
                                velocityY *= 2f;
                            }

                            int proj = Projectile.NewProjectile(npc.Center.X + (float)Main.rand.Next(-spawnX, spawnX), npc.Center.Y + (float)(npc.width / 2) * 0.8f,
                                velocityX, velocityY, ProjectileID.Fireball, 32, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[proj].timeLeft = 240;
                        }
                    }
                }
                else
                {
                    npc.TargetClosest(true);

                    // Fall through
                    if (npc.target >= 0 &&
                        ((Main.player[npc.target].position.Y > npc.position.Y + (float)npc.height && npc.velocity.Y > 0f) || (Main.player[npc.target].position.Y < npc.position.Y + (float)npc.height && npc.velocity.Y < 0f)))
                        npc.noTileCollide = true;
                    else
                        npc.noTileCollide = false;

                    // Velocity when falling
                    if (npc.position.X < Main.player[npc.target].position.X && npc.position.X + (float)npc.width > Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
                    {
                        npc.velocity.X = npc.velocity.X * 0.9f;

                        if (Main.player[npc.target].position.Y > npc.position.Y + (float)npc.height)
                        {
                            float fallSpeed = 0.2f + (0.8f * (1f - lifeRatio));
                            if (enrage)
                                fallSpeed *= 1.5f;

                            npc.velocity.Y = npc.velocity.Y + fallSpeed;
                        }
                    }
                    else
                    {
                        if (npc.direction < 0)
                            npc.velocity.X = npc.velocity.X - 0.2f;
                        else if (npc.direction > 0)
                            npc.velocity.X = npc.velocity.X + 0.2f;

                        float num648 = 3f + (6f * (1f - lifeRatio));
                        if (enrage)
                            num648 *= 1.5f;

                        if (npc.velocity.X < -num648)
                            npc.velocity.X = -num648;
                        if (npc.velocity.X > num648)
                            npc.velocity.X = num648;
                    }
                }
            }

            // Target
            if (npc.target <= 0 || npc.target == 255 || Main.player[npc.target].dead)
                npc.TargetClosest(true);

            // Despawn
            int num649 = 3000;
            if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) + Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > (float)num649)
            {
                npc.TargetClosest(true);

                if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) + Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > (float)num649)
                    npc.active = false;
            }

            return false;
        }

        public static bool BuffedGolemHeadAI(NPC npc, bool enraged, Mod mod)
        {
            // Don't collide
            npc.noTileCollide = true;

            // Die if body is gone
            if (NPC.golemBoss < 0)
            {
                npc.StrikeNPCNoInteraction(9999, 0f, 0, false, false, false);
                return false;
            }

            // Percent life remaining
            float lifeRatio = (float)npc.life / (float)npc.lifeMax;

            // Count body parts
            bool flag41 = NPC.AnyNPCs(NPCID.GolemFistLeft);
            bool flag42 = NPC.AnyNPCs(NPCID.GolemFistRight);
            npc.dontTakeDamage = flag41 || flag42;

            // Stay in position on top of body
            float num650 = 12f;
            Vector2 vector80 = new Vector2(npc.Center.X, npc.Center.Y);
            float num651 = Main.npc[NPC.golemBoss].Center.X - vector80.X;
            float num652 = Main.npc[NPC.golemBoss].Center.Y - vector80.Y;
            num652 -= 57f;
            num651 -= 3f;
            float num653 = (float)Math.Sqrt((double)(num651 * num651 + num652 * num652));
            if (num653 < 20f)
            {
                npc.rotation = 0f;
                npc.velocity.X = num651;
                npc.velocity.Y = num652;
            }
            else
            {
                num653 = num650 / num653;
                npc.velocity.X = num651 * num653;
                npc.velocity.Y = num652 * num653;
                npc.rotation = npc.velocity.X * 0.1f;
            }

            // Enrage if the target isn't inside the temple
            bool enrage = true;
            if ((double)Main.player[npc.target].Center.Y > Main.worldSurface * 16.0)
            {
                int num = (int)Main.player[npc.target].Center.X / 16;
                int num2 = (int)Main.player[npc.target].Center.Y / 16;

                Tile tile = Framing.GetTileSafely(num, num2);
                if (tile.wall == WallID.LihzahrdBrickUnsafe)
                    enrage = false;
            }

            // Alpha
            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                npc.ai[1] = 30f;
            }

            // Spit fireballs if arms are alive
            if (npc.ai[0] == 0f)
            {
                npc.ai[1] += 1f;
                int num654 = 300;
                if (npc.ai[1] < 20f || npc.ai[1] > (float)(num654 - 20))
                    npc.localAI[0] = 1f;
                else
                    npc.localAI[0] = 0f;

                if (npc.ai[1] >= (float)num654)
                {
                    npc.TargetClosest(true);

                    npc.ai[1] = 0f;

                    Vector2 vector81 = new Vector2(npc.Center.X, npc.Center.Y + 10f);
                    float num655 = enrage ? 16f : 8f;
                    float num656 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector81.X;
                    float num657 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector81.Y;
                    float num658 = (float)Math.Sqrt((double)(num656 * num656 + num657 * num657));

                    num658 = num655 / num658;
                    num656 *= num658;
                    num657 *= num658;

                    if (Main.netMode != 1)
                        Projectile.NewProjectile(vector81.X, vector81.Y, num656, num657, ProjectileID.Fireball, 30, 0f, Main.myPlayer, 0f, 0f);
                }
            }

            // Shoot lasers and fireballs if arms are dead
            else if (npc.ai[0] == 1f)
            {
                npc.TargetClosest(true);

                // Fire projectiles from eye positions
                Vector2 vector82 = new Vector2(npc.Center.X, npc.Center.Y + 10f);
                if (Main.player[npc.target].Center.X < npc.Center.X - (float)npc.width)
                {
                    npc.localAI[1] = -1f;
                    vector82.X -= 40f;
                }
                else if (Main.player[npc.target].Center.X > npc.Center.X + (float)npc.width)
                {
                    npc.localAI[1] = 1f;
                    vector82.X += 40f;
                }
                else
                    npc.localAI[1] = 0f;

                // Fireballs
                npc.ai[1] += 1f + (2f * (1f - lifeRatio));

                int num662 = 300;
                if (npc.ai[1] < 20f || npc.ai[1] > (float)(num662 - 20))
                    npc.localAI[0] = 1f;
                else
                    npc.localAI[0] = 0f;

                if (npc.ai[1] >= (float)num662)
                {
                    npc.TargetClosest(true);

                    npc.ai[1] = 0f;

                    float num663 = enrage ? 16f : 8f;
                    float num664 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector82.X;
                    float num665 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector82.Y;
                    float num666 = (float)Math.Sqrt((double)(num664 * num664 + num665 * num665));

                    num666 = num663 / num666;
                    num664 *= num666;
                    num665 *= num666;

                    if (Main.netMode != 1)
                        Projectile.NewProjectile(vector82.X, vector82.Y, num664, num665, ProjectileID.Fireball, 32, 0f, Main.myPlayer, 0f, 0f);
                }

                // Lasers
                npc.ai[2] += 1f + (3f * (1f - lifeRatio));
                if (enrage)
                    npc.ai[2] += 4f;
                if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    npc.ai[2] += 8f;

                if (npc.ai[2] >= 150f)
                {
                    npc.ai[2] = 0f;

                    int num670 = 35;
                    int num671 = ProjectileID.EyeBeam;

                    if (npc.localAI[1] == 0f)
                    {
                        for (int num672 = 0; num672 < 2; num672++)
                        {
                            vector82 = new Vector2(npc.Center.X, npc.Center.Y - 22f);
                            if (num672 == 0)
                                vector82.X -= 18f;
                            else
                                vector82.X += 18f;

                            float num673 = 9f;
                            if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                                num673 = 14f;

                            float num674 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector82.X;
                            float num675 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector82.Y;
                            float num676 = (float)Math.Sqrt((double)(num674 * num674 + num675 * num675));

                            num676 = num673 / num676;
                            num674 *= num676;
                            num675 *= num676;
                            vector82.X += num674 * 3f;
                            vector82.Y += num675 * 3f;

                            if (Main.netMode != 1)
                            {
                                int num677 = Projectile.NewProjectile(vector82.X, vector82.Y, num674, num675, num671, num670, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[num677].timeLeft = (enrage ? 480 : 300);
                            }
                        }
                    }
                    else if (npc.localAI[1] != 0f)
                    {
                        vector82 = new Vector2(npc.Center.X, npc.Center.Y - 22f);
                        if (npc.localAI[1] == -1f)
                            vector82.X -= 30f;
                        else if (npc.localAI[1] == 1f)
                            vector82.X += 30f;

                        float num678 = 9f;
                        if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                            num678 = 14f;

                        float num679 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector82.X;
                        float num680 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector82.Y;
                        float num681 = (float)Math.Sqrt((double)(num679 * num679 + num680 * num680));

                        num681 = num678 / num681;
                        num679 *= num681;
                        num680 *= num681;
                        vector82.X += num679 * 3f;
                        vector82.Y += num680 * 3f;

                        if (Main.netMode != 1)
                        {
                            int num682 = Projectile.NewProjectile(vector82.X, vector82.Y, num679, num680, num671, num670, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[num682].timeLeft = (enrage ? 480 : 300);
                        }
                    }
                }
            }

            // Laser fire if arms are dead
            if (!flag41 && !flag42)
            {
                npc.ai[0] = 1f;
                return false;
            }
            npc.ai[0] = 0f;

            return false;
        }

        public static bool BuffedGolemHeadFreeAI(NPC npc, bool enraged, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);

            // Die if body is gone
            if (NPC.golemBoss < 0)
            {
                npc.StrikeNPCNoInteraction(9999, 0f, 0, false, false, false);
                return false;
            }

            // Percent life remaining
            float lifeRatio = (float)npc.life / (float)npc.lifeMax;
            float golemLifeRatio = (float)Main.npc[NPC.golemBoss].life / (float)Main.npc[NPC.golemBoss].lifeMax;
            float combinedRatio = lifeRatio + golemLifeRatio;

            // Phases
            bool phase2 = lifeRatio < 0.7f || golemLifeRatio < 0.85f;
            bool phase3 = lifeRatio < 0.4f || golemLifeRatio < 0.7f;
            bool phase4 = lifeRatio < 0.1f || golemLifeRatio < 0.55f;

            // Float through tiles or not
            bool flag44 = false;
            if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1) || phase3)
            {
                npc.noTileCollide = true;
                flag44 = true;
            }
            else
                npc.noTileCollide = false;

            // Target
            npc.TargetClosest(true);

            // Enrage if the target isn't inside the temple
            bool enrage = true;
            if ((double)Main.player[npc.target].Center.Y > Main.worldSurface * 16.0)
            {
                int num = (int)Main.player[npc.target].Center.X / 16;
                int num2 = (int)Main.player[npc.target].Center.Y / 16;

                Tile tile = Framing.GetTileSafely(num, num2);
                if (tile.wall == WallID.LihzahrdBrickUnsafe)
                    enrage = false;
            }

            // Move to new location
            if (npc.ai[3] <= 0f)
            {
                npc.ai[3] = 300f;

                float maxDistance = 300f;

                // Four corners around target
                if (phase3)
                {
                    if (calamityGlobalNPC.newAI[1] == -maxDistance)
                    {
                        switch ((int)calamityGlobalNPC.newAI[0])
                        {
                            case 0:
                            case 300:
                                calamityGlobalNPC.newAI[0] = -maxDistance;
                                break;
                            case -300:
                                calamityGlobalNPC.newAI[1] = maxDistance;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch ((int)calamityGlobalNPC.newAI[0])
                        {
                            case 0:
                            case -300:
                                calamityGlobalNPC.newAI[0] = maxDistance;
                                break;
                            case 300:
                                calamityGlobalNPC.newAI[1] = -maxDistance;
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Above target
                else if (phase2)
                {
                    switch ((int)calamityGlobalNPC.newAI[0])
                    {
                        case 0:
                            calamityGlobalNPC.newAI[0] = maxDistance;
                            break;
                        case 300:
                            calamityGlobalNPC.newAI[0] = -maxDistance;
                            break;
                        case -300:
                            calamityGlobalNPC.newAI[0] = 0f;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = -maxDistance;
                }
            }

            npc.ai[3] -= 1f +
                (phase2 ? 1f : 0f) +
                (phase3 ? 1f : 0f) +
                (phase4 ? 2f : 0f);

            float offsetX = calamityGlobalNPC.newAI[0];
            float offsetY = calamityGlobalNPC.newAI[1];

            // Velocity and acceleration
            float num700 = 7f +
                (phase2 ? 4f : 0f) +
                (phase3 ? 4f : 0f);
            if (enrage)
                num700 = 15f;

            Vector2 vector87 = new Vector2(npc.Center.X, npc.Center.Y);
            float num702 = Main.player[npc.target].Center.X - vector87.X + offsetX;
            float num703 = Main.player[npc.target].Center.Y - vector87.Y + offsetY;
            float num704 = (float)Math.Sqrt((double)(num702 * num702 + num703 * num703));

            // Static movement
            if (phase3)
            {
                if (num704 < num700)
                {
                    npc.velocity.X = num702;
                    npc.velocity.Y = num703;
                }
                else
                {
                    num704 = num700 / num704;
                    npc.velocity.X = num702 * num704;
                    npc.velocity.Y = num703 * num704;
                }
            }

            // Rubber band movement
            else
            {
                float num701 = 0.1f + (phase2 ? 0.1f : 0f);
                if (enrage)
                    num701 = 0.2f;

                num704 = num700 / num704;
                num702 *= num704;
                num703 *= num704;

                if (npc.velocity.X < num702)
                {
                    npc.velocity.X = npc.velocity.X + num701;
                    if (npc.velocity.X < 0f && num702 > 0f)
                        npc.velocity.X = npc.velocity.X + num701;
                }
                else if (npc.velocity.X > num702)
                {
                    npc.velocity.X = npc.velocity.X - num701;
                    if (npc.velocity.X > 0f && num702 < 0f)
                        npc.velocity.X = npc.velocity.X - num701;
                }
                if (npc.velocity.Y < num703)
                {
                    npc.velocity.Y = npc.velocity.Y + num701;
                    if (npc.velocity.Y < 0f && num703 > 0f)
                        npc.velocity.Y = npc.velocity.Y + num701;
                }
                else if (npc.velocity.Y > num703)
                {
                    npc.velocity.Y = npc.velocity.Y - num701;
                    if (npc.velocity.Y > 0f && num703 < 0f)
                        npc.velocity.Y = npc.velocity.Y - num701;
                }
            }

            // Fireballs
            npc.ai[1] += 1f + (2f * (2f - (lifeRatio + golemLifeRatio)));

            int num705 = 360;
            if (npc.ai[1] < 20f || npc.ai[1] > (float)(num705 - 20))
                npc.localAI[0] = 1f;
            else
                npc.localAI[0] = 0f;

            if (flag44 && !phase3)
                npc.ai[1] = 20f;

            if (npc.ai[1] >= (float)num705 && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 160f)
            {
                npc.TargetClosest(true);

                npc.ai[1] = 0f;

                Vector2 vector88 = new Vector2(npc.Center.X, npc.Center.Y - 10f);
                float num706 = enrage ? 16f : 8f;
                float num709 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector88.X;
                float num710 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector88.Y;
                float num711 = (float)Math.Sqrt((double)(num709 * num709 + num710 * num710));

                num711 = num706 / num711;
                num709 *= num711;
                num710 *= num711;

                int projectileType = phase3 ? ProjectileID.InfernoHostileBolt : ProjectileID.Fireball;
                if (Main.netMode != 1)
                {
                    int proj = Projectile.NewProjectile(vector88.X, vector88.Y, num709, num710, projectileType, 32, 0f, Main.myPlayer, 0f, 0f);
                    if (projectileType == ProjectileID.InfernoHostileBolt)
                    {
                        Main.projectile[proj].timeLeft = 300;
                        Main.projectile[proj].ai[0] = Main.player[npc.target].Center.X;
                        Main.projectile[proj].ai[1] = Main.player[npc.target].Center.Y;
                        Main.projectile[proj].netUpdate = true;
                    }
                }
            }

            // Lasers
            npc.ai[2] += 1f + (5f * (2f - (lifeRatio + golemLifeRatio)));
            if (!Collision.CanHit(Main.npc[NPC.golemBoss].Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                npc.ai[2] += 8f;

            if (npc.ai[2] >= 150f && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 160f)
            {
                npc.ai[2] = 0f;

                for (int num713 = 0; num713 < 2; num713++)
                {
                    Vector2 vector89 = new Vector2(npc.Center.X, npc.Center.Y - 50f);
                    if (num713 == 0)
                        vector89.X -= 14f;
                    else if (num713 == 1)
                        vector89.X += 14f;

                    float num714 = 9f + (2f * (2f - (lifeRatio + golemLifeRatio)));
                    float num717 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector89.X;
                    float num718 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector89.Y;
                    float num719 = (float)Math.Sqrt((double)(num717 * num717 + num718 * num718));

                    num719 = num714 / num719;
                    num717 *= num719;
                    num718 *= num719;
                    vector89.X += num717 * 3f;
                    vector89.Y += num718 * 3f;

                    if (Main.netMode != 1)
                    {
                        int num720 = Projectile.NewProjectile(vector89.X, vector89.Y, num717, num718, ProjectileID.EyeBeam, 35, 0f, Main.myPlayer, 0f, 0f);
                        Main.projectile[num720].timeLeft = (enrage ? 480 : 300);
                    }
                }
            }

            return false;
        }
        #endregion

        #region Buffed Duke Fishron AI
        public static bool BuffedDukeFishronAI(NPC npc, bool enraged, Mod mod)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.GetGlobalNPC<CalamityGlobalNPC>(mod);

			// Variables
			float num = 0.6f * Main.damageMultiplier;
			bool phase2 = (double)npc.life <= (double)npc.lifeMax * 0.66;
			bool phase3 = (double)npc.life <= (double)npc.lifeMax * ((CalamityWorld.death || CalamityWorld.bossRushActive) ? 0.33 : 0.2);
			bool phase2AI = npc.ai[0] > 4f;
			bool phase3AI = npc.ai[0] > 9f;
			bool charging = npc.ai[3] < 10f;

			// Adjust stats
			if (phase3AI)
			{
				npc.damage = (int)((float)npc.defDamage * 1.1f * num);
				npc.defense = (int)((float)npc.defDefense * 0.75f);
			}
			else if (phase2AI)
			{
				npc.damage = (int)((float)npc.defDamage * 1.2f * num);
				npc.defense = npc.defDefense;
			}
			else
			{
				npc.damage = npc.defDamage;
				npc.defense = (int)((float)npc.defDefense * 1.2f);
			}

			int num2 = 30;
			float num3 = 0.55f;
			float scaleFactor = 8.5f;
			if (phase3AI)
			{
				num3 = 0.7f;
				scaleFactor = 12f;
			}
			else if (phase2AI & charging)
			{
				num3 = 0.6f;
				scaleFactor = 10f;
			}

			int chargeTime = 28;
			float chargeVelocity = 17f;
			if (phase3AI)
			{
				chargeTime = 25;
				chargeVelocity = 27f;
			}
			else if (charging & phase2AI)
			{
				chargeTime = 27;
				chargeVelocity = 21f;
			}

			// Variables
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
			Vector2 vector = npc.Center;
			Player player = Main.player[npc.target];

			// Get target
			if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
			{
				npc.TargetClosest(true);
				player = Main.player[npc.target];
				npc.netUpdate = true;
			}

			// Despawn
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

			// Enrage variable
			bool enrage = player.position.Y < 800f || (double)player.position.Y > Main.worldSurface * 16.0 || (player.position.X > 6400f && player.position.X < (float)(Main.maxTilesX * 16 - 6400));

			// If the player isn't in the ocean biome or Fishron is transitioning between phases, become immune
			if (!phase3AI)
				npc.dontTakeDamage = npc.ai[0] == -1f || npc.ai[0] == 4f || npc.ai[0] == 9f || enrage;

			// Enrage
			if (enrage)
			{
				num2 = 20;
				npc.damage = npc.defDamage * 2;
				npc.defense = npc.defDefense * 2;
				npc.ai[3] = 0f;
				chargeVelocity += 8f;
			}

			// Spawn cthulhunadoes in phase 3
			if (phase3AI)
			{
				calamityGlobalNPC.newAI[0] += 1f;
				if (calamityGlobalNPC.newAI[0] >= 600f)
				{
					calamityGlobalNPC.newAI[0] = 0f;

					if (Main.netMode != NetmodeID.MultiplayerClient)
						Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));

					npc.netUpdate = true;
				}
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

			// Alpha adjustments
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

			// Spawn effects
			if (npc.ai[0] == -1f)
			{
				// Velocity
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
				if (npc.ai[2] == (float)(num9 - 30))
				{
					int num20 = 36;
					for (int i = 0; i < num20; i++)
					{
						Vector2 expr_80F = (Vector2.Normalize(npc.velocity) * new Vector2((float)npc.width / 2f, (float)npc.height) * 0.75f * 0.5f).RotatedBy((double)((float)(i - (num20 / 2 - 1)) * 6.28318548f / (float)num20), default) + npc.Center;
						Vector2 vector2 = expr_80F - npc.Center;
						int num21 = Dust.NewDust(expr_80F + vector2, 0, 0, 172, vector2.X * 2f, vector2.Y * 2f, 100, default, 1.4f);
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

			// Phase 1
			else if (npc.ai[0] == 0f && !player.dead)
			{
				// Velocity
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

				// Rotation and direction
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

				// Phase switch
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

					if (phase2)
						num23 = 4;

					// Set velocity for charge
					if (num23 == 1)
					{
						npc.ai[0] = 1f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;

						// Velocity
						npc.velocity = Vector2.Normalize(player.Center - vector) * chargeVelocity;
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

						// Direction
						if (num22 != 0)
						{
							npc.direction = num22;

							if (npc.spriteDirection == 1)
								npc.rotation += 3.14159274f;

							npc.spriteDirection = -npc.direction;
						}
					}

					// Bubbles
					else if (num23 == 2)
					{
						npc.ai[0] = 2f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}

					// Spawn sharknadoes
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

			// Charge
			else if (npc.ai[0] == 1f)
			{
				// Accelerate
				npc.velocity *= 1.01f;

				// Spawn dust
				int num24 = 7;
				for (int j = 0; j < num24; j++)
				{
					Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(j - (num24 / 2 - 1)) * 3.1415926535897931 / (double)((float)num24), default) + vector;
					Vector2 vector4 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, 172, vector4.X * 2f, vector4.Y * 2f, 100, default, 1.4f);
					Main.dust[num25].noGravity = true;
					Main.dust[num25].noLight = true;
					Main.dust[num25].velocity /= 4f;
					Main.dust[num25].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)chargeTime)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 2f;
					npc.netUpdate = true;
				}
			}

			// Bubble belch
			else if (npc.ai[0] == 2f)
			{
				// Velocity
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

				// Play sounds and spawn bubbles
				if (npc.ai[2] == 0f)
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				if (npc.ai[2] % (float)num7 == 0f)
				{
					Main.PlaySound(4, (int)npc.Center.X, (int)npc.Center.Y, 19, 1f, 0f);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector6 = Vector2.Normalize(player.Center - vector) * (float)(npc.width + 20) / 2f + vector;
						NPC.NewNPC((int)vector6.X, (int)vector6.Y + 45, NPCID.DetonatingBubble, 0, 0f, 0f, 0f, 0f, 255);
					}
				}

				// Direction
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

			// Sharknado spawn
			else if (npc.ai[0] == 3f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Play sound and spawn sharknadoes
				if (npc.ai[2] == (float)(num9 - 30))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 9, 1f, 0f);

				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == (float)(num9 - 30))
				{
					Vector2 vector7 = npc.rotation.ToRotationVector2() * (Vector2.UnitX * (float)npc.direction) * (float)(npc.width + 20) / 2f + vector;
					bool normal = Main.rand.NextBool();
					float velocityY = normal ? 8f : -4f;
					float ai1 = normal ? 0f : -1f;

					Projectile.NewProjectile(vector7.X, vector7.Y, (float)(npc.direction * 3), velocityY, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 0f, ai1);
					Projectile.NewProjectile(vector7.X, vector7.Y, (float)(-(float)npc.direction * 3), velocityY, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 0f, ai1);

					velocityY = normal ? -4f : 8f;
					ai1 = normal ? -1f : 0f;
					Projectile.NewProjectile(vector7.X, vector7.Y, 0f, velocityY, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 0f, ai1);
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

			// Transition to phase 2
			else if (npc.ai[0] == 4f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Sound
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

			// Phase 2
			else if (npc.ai[0] == 5f && !player.dead)
			{
				// Velocity
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

				// Direction and rotation
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

				// Phase switch
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

					if (phase3)
						num28 = 4;

					// Set velocity for charge
					if (num28 == 1)
					{
						npc.ai[0] = 6f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;

						// Velocity and rotation
						npc.velocity = Vector2.Normalize(player.Center - vector) * chargeVelocity;
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

						// Direction
						if (num27 != 0)
						{
							npc.direction = num27;

							if (npc.spriteDirection == 1)
								npc.rotation += 3.14159274f;

							npc.spriteDirection = -npc.direction;
						}
					}

					// Set velocity for spin
					else if (num28 == 2)
					{
						// Velocity and rotation
						npc.velocity = Vector2.Normalize(player.Center - vector) * scaleFactor4;
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

						// Direction
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

					// Spawn cthulhunado
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

			// Charge
			else if (npc.ai[0] == 6f)
			{
				// Accelerate
				npc.velocity *= 1.01f;

				// Spawn dust
				int num29 = 7;
				for (int k = 0; k < num29; k++)
				{
					Vector2 arg_1A97_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(k - (num29 / 2 - 1)) * 3.1415926535897931 / (double)((float)num29), default) + vector;
					Vector2 vector9 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num30 = Dust.NewDust(arg_1A97_0 + vector9, 0, 0, 172, vector9.X * 2f, vector9.Y * 2f, 100, default, 1.4f);
					Main.dust[num30].noGravity = true;
					Main.dust[num30].noLight = true;
					Main.dust[num30].velocity /= 4f;
					Main.dust[num30].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)chargeTime)
				{
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 2f;
					npc.netUpdate = true;
				}
			}

			// Bubble spin
			else if (npc.ai[0] == 7f)
			{
				// Play sounds and spawn bubbles
				if (npc.ai[2] == 0f)
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				if (npc.ai[2] % (float)num14 == 0f)
				{
					Main.PlaySound(4, (int)npc.Center.X, (int)npc.Center.Y, 19, 1f, 0f);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 vector10 = Vector2.Normalize(npc.velocity) * (float)(npc.width + 20) / 2f + vector;
						int num31 = NPC.NewNPC((int)vector10.X, (int)vector10.Y + 45, NPCID.DetonatingBubble, 0, 0f, 0f, 0f, 0f, 255);
						Main.npc[num31].target = npc.target;
						Main.npc[num31].velocity = Vector2.Normalize(npc.velocity).RotatedBy((double)(1.57079637f * (float)npc.direction), default) * scaleFactor3;
						Main.npc[num31].netUpdate = true;
						Main.npc[num31].ai[3] = (float)Main.rand.Next(80, 121) / 100f;

						if (npc.ai[2] % (float)(num14 * 5) == 0f)
						{
							int npc2 = NPC.NewNPC((int)vector10.X, (int)vector10.Y + 45, NPCID.Sharkron2, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[npc2].ai[1] = 89f;
						}
					}
				}

				// Velocity and rotation
				npc.velocity = npc.velocity.RotatedBy((double)(-(double)num15 * (float)npc.direction), default);
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

			// Spawn cthulhunado
			else if (npc.ai[0] == 8f)
			{
				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Play sound and spawn cthulhunado
				if (npc.ai[2] == (float)(num9 - 30))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == (float)(num9 - 30))
					Projectile.NewProjectile(vector.X, vector.Y, 0f, 0f, ProjectileID.SharknadoBolt, 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)num9)
				{
					npc.ai[0] = 5f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}
			}

			// Transition to phase 3
			else if (npc.ai[0] == 9f)
			{
				// Alpha adjustments
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

				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Play sound
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

			// Phase 3
			else if (npc.ai[0] == 10f && !player.dead)
			{
				npc.dontTakeDamage = false;
				npc.chaseable = false;

				// Alpha
				if (npc.alpha < 255)
				{
					npc.alpha += 25;
					if (npc.alpha > 255)
						npc.alpha = 255;
				}

				// Teleport location
				if (npc.ai[1] == 0f)
					npc.ai[1] = (float)(360 * Math.Sign((vector - player.Center).X));

				Vector2 desiredVelocity = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * scaleFactor;
				npc.SimpleFlyMovement(desiredVelocity, num3);

				// Rotation and direction
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

				// Phase switch
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

					// Set velocity for charge
					if (num33 == 1)
					{
						npc.ai[0] = 11f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;

						// Velocity and rotation
						npc.velocity = Vector2.Normalize(player.Center - vector) * chargeVelocity;
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

						// Direction
						if (num32 != 0)
						{
							npc.direction = num32;

							if (npc.spriteDirection == 1)
								npc.rotation += 3.14159274f;

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

					npc.netUpdate = true;
				}
			}

			// Charge
			else if (npc.ai[0] == 11f)
			{
				npc.dontTakeDamage = false;
				npc.chaseable = true;

				// Accelerate
				npc.velocity *= 1.01f;

				// Alpha
				npc.alpha -= 25;
				if (npc.alpha < 0)
					npc.alpha = 0;

				// Spawn dust
				int num34 = 7;
				for (int m = 0; m < num34; m++)
				{
					Vector2 arg_2444_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(m - (num34 / 2 - 1)) * 3.1415926535897931 / (double)((float)num34), default) + vector;
					Vector2 vector11 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num35 = Dust.NewDust(arg_2444_0 + vector11, 0, 0, 172, vector11.X * 2f, vector11.Y * 2f, 100, default, 1.4f);
					Main.dust[num35].noGravity = true;
					Main.dust[num35].noLight = true;
					Main.dust[num35].velocity /= 4f;
					Main.dust[num35].velocity -= npc.velocity;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (float)chargeTime)
				{
					npc.ai[0] = 10f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] += 1f;
					npc.netUpdate = true;
				}
			}

			// Pause before teleport
			else if (npc.ai[0] == 12f)
			{
				npc.dontTakeDamage = true;
				npc.chaseable = false;

				// Alpha
				if (npc.alpha < 255)
				{
					npc.alpha += 17;
					if (npc.alpha > 255)
						npc.alpha = 255;
				}

				// Velocity
				npc.velocity *= 0.98f;
				npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

				// Play sound
				if (npc.ai[2] == (float)(num12 / 2))
					Main.PlaySound(29, (int)vector.X, (int)vector.Y, 20, 1f, 0f);

				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == (float)(num12 / 2))
				{
					// Teleport location
					if (npc.ai[1] == 0f)
						npc.ai[1] = (float)(300 * Math.Sign((vector - player.Center).X));

					// Rotation and direction
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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}
		#endregion

		#region Buffed Mothron AI
		public static bool BuffedMothronAI(NPC npc)
		{
			npc.noTileCollide = false;
			npc.noGravity = true;
			npc.knockBackResist = 0f;
			npc.damage = npc.defDamage;

			if (!Main.eclipse)
				npc.ai[0] = -1f;
			else if (npc.target < 0 || Main.player[npc.target].dead || !Main.player[npc.target].active)
			{
				npc.TargetClosest(true);
				Vector2 vector235 = Main.player[npc.target].Center - npc.Center;
				if (Main.player[npc.target].dead || vector235.Length() > 4000f)
					npc.ai[0] = -1f;
			}
			else
			{
				Vector2 vector236 = Main.player[npc.target].Center - npc.Center;
				if (npc.ai[0] > 1f && vector236.Length() > 3000f)
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

					if (npc.velocity.X > 4f)
						npc.velocity.X = 4f;
					if (npc.velocity.X < -4f)
						npc.velocity.X = -4f;
				}
				if (npc.collideY)
				{
					npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);

					if (npc.velocity.Y > 4f)
						npc.velocity.Y = 4f;
					if (npc.velocity.Y < -4f)
						npc.velocity.Y = -4f;
				}

				Vector2 value38 = Main.player[npc.target].Center - npc.Center;
				value38.Y -= 200f;
				if (value38.Length() > 3000f)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
				}
				else if (value38.Length() > 240f)
				{
					float scaleFactor15 = 15f;
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

				if (npc.ai[1] >= 20f && Main.netMode != NetmodeID.MultiplayerClient)
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
						else if (num1355 == 2 && NPC.CountNPCS(478) + NPC.CountNPCS(479) < 2)
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

					if (npc.target < 0 || !Main.player[npc.target].active || Main.player[npc.target].dead)
						npc.TargetClosest(true);

					if (npc.velocity.X < 0f)
						npc.direction = -1;
					else if (npc.velocity.X > 0f)
						npc.direction = 1;

					npc.spriteDirection = npc.direction;
					npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.02f) / 10f;

					Vector2 value39 = Main.player[npc.target].Center - npc.Center;
					if (value39.Length() < 800f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}

					float scaleFactor16 = 18f + value39.Length() / 100f;
					float num1356 = 25f;
					value39.Normalize();
					value39 *= scaleFactor16;
					npc.velocity = (npc.velocity * (num1356 - 1f) + value39) / num1356;
					return false;
				}

				if (npc.ai[0] == 2f)
				{
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

						if (npc.velocity.X > 4f)
							npc.velocity.X = 4f;
						if (npc.velocity.X < -4f)
							npc.velocity.X = -4f;
					}
					if (npc.collideY)
					{
						npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);

						if (npc.velocity.Y > 4f)
							npc.velocity.Y = 4f;
						if (npc.velocity.Y < -4f)
							npc.velocity.Y = -4f;
					}

					Vector2 value40 = Main.player[npc.target].Center - npc.Center;
					value40.Y -= 20f;

					npc.ai[2] += 0.0222222228f;
					if (Main.expertMode)
						npc.ai[2] += 0.0166666675f;

					float scaleFactor17 = 12f + npc.ai[2] + value40.Length() / 120f;
					float num1357 = 20f;
					value40.Normalize();
					value40 *= scaleFactor17;
					npc.velocity = (npc.velocity * (num1357 - 1f) + value40) / num1357;

					npc.ai[1] += 1f;
					if (npc.ai[1] >= 120f || !Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
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
							value41.X += 600f;
						else
							value41.X -= 600f;

						if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 550f && Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) < 20f)
						{
							npc.ai[0] = 3.1f;
							npc.ai[1] = 0f;
						}

						npc.ai[1] += 0.0333333351f;
						float scaleFactor18 = 24f + npc.ai[1];
						float num1358 = 4f;
						value41.Normalize();
						value41 *= scaleFactor18;
						npc.velocity = (npc.velocity * (num1358 - 1f) + value41) / num1358;
						return false;
					}

					if (npc.ai[0] == 3.1f)
					{
						npc.noTileCollide = true;
						npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.0175f) / 5f;

						Vector2 vector237 = Main.player[npc.target].Center - npc.Center;
						vector237.Y -= 12f;
						float scaleFactor19 = 32f;
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
							npc.noTileCollide = true;
							npc.ai[2] += 0.0333333351f;
							npc.velocity.X = (32f + npc.ai[2]) * npc.ai[1];

							if ((npc.ai[1] > 0f && npc.Center.X > Main.player[npc.target].Center.X + 460f) || (npc.ai[1] < 0f && npc.Center.X < Main.player[npc.target].Center.X - 460f))
							{
								if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
								{
									npc.ai[0] = 0f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;
								}
								else if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 800f)
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

							if (Main.netMode != NetmodeID.MultiplayerClient)
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

										if ((new Vector2((float)(num1361 * 16 + 8), (float)(num1362 * 16 + 8)) - Main.player[npc.target].Center).Length() < 3600f)
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
							float num1367 = 12f + vector238.Length() / 150f;

							if (num1367 > 20f)
								num1367 = 20f;

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
							npc.noTileCollide = true;

							int num1369 = (int)npc.ai[1];
							int num1370 = (int)npc.ai[2];
							float x3 = (float)(num1369 * 16 + 8);
							float y3 = (float)(num1370 * 16 - 20);
							Vector2 vector239 = new Vector2(x3, y3);
							vector239 -= npc.Center;
							float num1371 = 4f;
							float num1372 = 2f;

							if (Main.netMode != NetmodeID.MultiplayerClient && vector239.Length() < 4f)
							{
								int num1373 = 20;
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

			if (Main.netMode != NetmodeID.MultiplayerClient)
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

			if (npc.ai[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

			return false;
		}

		public static bool BuffedPumpkingBladeAI(NPC npc)
		{
			npc.spriteDirection = -(int)npc.ai[0];

			if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 58)
			{
				npc.ai[2] += 10f;
				if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
				{
					npc.life = -1;
					npc.HitEffect(0, 10.0);
					npc.active = false;
				}
			}

			if (Main.netMode != NetmodeID.MultiplayerClient && Main.npc[(int)npc.ai[1]].ai[3] == 2f)
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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

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

					if (npc.ai[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
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

				if (Main.netMode != NetmodeID.MultiplayerClient)
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

				if (Main.netMode != NetmodeID.MultiplayerClient)
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

				if (Main.netMode != NetmodeID.MultiplayerClient)
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

				if (Main.netMode != NetmodeID.MultiplayerClient)
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

			if (ModLoader.GetMod("FargowiltasSouls") != null)
				ModLoader.GetMod("FargowiltasSouls").Call("FargoSoulsAI", npc.whoAmI);

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
			if (npc.ai[0] >= 0f && npc.ai[0] < 2f && Main.netMode != NetmodeID.MultiplayerClient && npc.Distance(Main.player[npc.target].Center) > 1800f) //2400
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
						if (Main.netMode != NetmodeID.MultiplayerClient)
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
							if (Main.netMode != NetmodeID.MultiplayerClient)
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

						if (Main.netMode != NetmodeID.MultiplayerClient)
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
						if (Main.netMode != NetmodeID.MultiplayerClient)
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

        #region Revengeance Dungeon Guardian AI
        public static void RevengeanceDungeonGuardianAI(NPC npc, bool configBossRushBoost, bool enraged)
		{
			Vector2 vector21 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num177 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector21.X;
			float num178 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector21.Y;
			float num179 = (float)Math.Sqrt((double)(num177 * num177 + num178 * num178));
			num179 = 12f / num179;
			npc.velocity.X = num177 * num179;
			npc.velocity.Y = num178 * num179;
			if (Main.netMode != NetmodeID.MultiplayerClient)
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
		#endregion

		#region Revengeance Basic NPC AI
		public static void RevengeanceLihzahrdAI(NPC npc)
		{
			// Transform into second state sooner
			if (Main.netMode != NetmodeID.MultiplayerClient && (double)npc.life <= (double)npc.lifeMax * 0.9)
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
