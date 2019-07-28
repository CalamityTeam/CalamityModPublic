using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.PlaguebringerShade
{
    public class PlaguebringerShade : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plaguebringer");
			Main.npcFrameCount[npc.type] = 12;
		}

		public override void SetDefaults()
		{
			npc.damage = 80; //150
			npc.npcSlots = 8f;
			npc.width = 66; //324
			npc.height = 66; //216
			npc.defense = 30;
			npc.lifeMax = CalamityWorld.death ? 4000 : 3000; //250000
			npc.value = Item.buyPrice(0, 1, 50, 0);
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = 200000;
			}
			npc.knockBackResist = 0f;
			npc.aiStyle = -1; //new
			aiType = -1; //new
			animationType = NPCID.QueenBee;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.buffImmune[BuffID.Ichor] = false;
			npc.buffImmune[BuffID.CursedInferno] = false;
			npc.buffImmune[mod.BuffType("MarkedforDeath")] = false;
			npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
			npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
			npc.buffImmune[mod.BuffType("DemonFlames")] = false;
			npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
			npc.buffImmune[mod.BuffType("HolyLight")] = false;
			npc.buffImmune[mod.BuffType("Nightwither")] = false;
			npc.buffImmune[mod.BuffType("Shred")] = false;
			npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
			npc.buffImmune[mod.BuffType("SilvaStun")] = false;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			banner = npc.type;
			bannerItem = mod.ItemType("PlaguebringerBanner");
		}

		public override void AI()
		{
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.05f, 0.15f, 0.025f);
			if (Main.expertMode)
			{
				int num1041 = (int)(30f * (1f - (float)npc.life / (float)npc.lifeMax));
				npc.damage = npc.defDamage - num1041;
			}
			bool flag113 = false;
			if (!Main.player[npc.target].ZoneJungle && !CalamityWorld.bossRushActive)
			{
				flag113 = true;
				if (npc.timeLeft > 150)
				{
					npc.timeLeft = 150;
				}
			}
			else
			{
				if (npc.timeLeft < 750)
				{
					npc.timeLeft = 750;
				}
			}
			int num1038 = 0;
			for (int num1039 = 0; num1039 < 255; num1039++)
			{
				if (Main.player[num1039].active && !Main.player[num1039].dead && (npc.Center - Main.player[num1039].Center).Length() < 1000f)
				{
					num1038++;
				}
			}
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
			{
				npc.TargetClosest(true);
			}
			bool dead4 = Main.player[npc.target].dead;
			if (dead4 && Main.expertMode)
			{
				if ((double)npc.position.Y < Main.worldSurface * 16.0 + 2000.0)
				{
					npc.velocity.Y = npc.velocity.Y + 0.04f;
				}
				if (npc.position.X < (float)(Main.maxTilesX * 8))
				{
					npc.velocity.X = npc.velocity.X - 0.04f;
				}
				else
				{
					npc.velocity.X = npc.velocity.X + 0.04f;
				}
				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
					return;
				}
			}
			else if (npc.ai[0] == -1f)
			{
				if (Main.netMode != 1)
				{
					float num1041 = npc.ai[1];
					int num1042;
					do
					{
						num1042 = Main.rand.Next(3);
						if (num1042 == 1)
						{
							num1042 = 2;
						}
						else if (num1042 == 2)
						{
							num1042 = 3;
						}
					}
					while ((float)num1042 == num1041);
					npc.ai[0] = (float)num1042;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					return;
				}
			}
			else if (npc.ai[0] == 0f)
			{
				int num1043 = 2; //2
				if (flag113)
				{
					num1043 += 1;
				}
				if (npc.ai[1] > (float)(2 * num1043) && npc.ai[1] % 2f == 0f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
					return;
				}
				if (npc.ai[1] % 2f == 0f)
				{
					npc.TargetClosest(true);
					if (Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))) < 20f)
					{
						npc.localAI[0] = 1f;
						npc.ai[1] += 1f;
						npc.ai[2] = 0f;
						float num1044 = 15f; //12
						if (flag113)
						{
							num1044 += 2f;
						}
						Vector2 vector117 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num1045 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector117.X;
						float num1046 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector117.Y;
						float num1047 = (float)Math.Sqrt((double)(num1045 * num1045 + num1046 * num1046));
						num1047 = num1044 / num1047;
						npc.velocity.X = num1045 * num1047;
						npc.velocity.Y = num1046 * num1047;
						npc.spriteDirection = npc.direction;
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0);
						return;
					}
					npc.localAI[0] = 0f;
					float num1048 = 12.25f; //12
					float num1049 = 0.155f; //0.15
					if (flag113)
					{
						num1048 += 1f; //2
						num1049 += 0.075f; //0.1
					}
					if (npc.position.Y + (float)(npc.height / 2) < Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))
					{
						npc.velocity.Y = npc.velocity.Y + num1049;
					}
					else
					{
						npc.velocity.Y = npc.velocity.Y - num1049;
					}
					if (npc.velocity.Y < -12f)
					{
						npc.velocity.Y = -num1048;
					}
					if (npc.velocity.Y > 12f)
					{
						npc.velocity.Y = num1048;
					}
					if (Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) > 600f)
					{
						npc.velocity.X = npc.velocity.X + 0.15f * (float)npc.direction;
					}
					else if (Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) < 300f)
					{
						npc.velocity.X = npc.velocity.X - 0.15f * (float)npc.direction;
					}
					else
					{
						npc.velocity.X = npc.velocity.X * 0.8f;
					}
					if (npc.velocity.X < -16f)
					{
						npc.velocity.X = -16f;
					}
					if (npc.velocity.X > 16f)
					{
						npc.velocity.X = 16f;
					}
					npc.spriteDirection = npc.direction;
					return;
				}
				else
				{
					if (npc.velocity.X < 0f)
					{
						npc.direction = -1;
					}
					else
					{
						npc.direction = 1;
					}
					npc.spriteDirection = npc.direction;
					int num1050 = 500; //600
					int num1051 = 1;
					if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))
					{
						num1051 = -1;
					}
					if (npc.direction == num1051 && Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) > (float)num1050)
					{
						npc.ai[2] = 1f;
					}
					if (npc.ai[2] != 1f)
					{
						npc.localAI[0] = 1f;
						return;
					}
					npc.TargetClosest(true);
					npc.spriteDirection = npc.direction;
					npc.localAI[0] = 0f;
					npc.velocity *= 0.9f;
					float num1052 = 0.105f; //0.1
					if (flag113)
					{
						npc.velocity *= 0.9f;
						num1052 += 0.075f;
					}
					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num1052)
					{
						npc.ai[2] = 0f;
						npc.ai[1] += 1f;
						return;
					}
				}
			}
			else if (npc.ai[0] == 2f)
			{
				npc.TargetClosest(true);
				npc.spriteDirection = npc.direction;
				float num1053 = 12f; //12
				float num1054 = 0.1f; //0.07
				if (flag113)
				{
					num1054 = 0.12f;
				}
				Vector2 vector118 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num1055 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector118.X;
				float num1056 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 200f - vector118.Y;
				float num1057 = (float)Math.Sqrt((double)(num1055 * num1055 + num1056 * num1056));
				if (num1057 < 400f)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.netUpdate = true;
					return;
				}
				num1057 = num1053 / num1057;
				if (npc.velocity.X < num1055)
				{
					npc.velocity.X = npc.velocity.X + num1054;
					if (npc.velocity.X < 0f && num1055 > 0f)
					{
						npc.velocity.X = npc.velocity.X + num1054;
					}
				}
				else if (npc.velocity.X > num1055)
				{
					npc.velocity.X = npc.velocity.X - num1054;
					if (npc.velocity.X > 0f && num1055 < 0f)
					{
						npc.velocity.X = npc.velocity.X - num1054;
					}
				}
				if (npc.velocity.Y < num1056)
				{
					npc.velocity.Y = npc.velocity.Y + num1054;
					if (npc.velocity.Y < 0f && num1056 > 0f)
					{
						npc.velocity.Y = npc.velocity.Y + num1054;
						return;
					}
				}
				else if (npc.velocity.Y > num1056)
				{
					npc.velocity.Y = npc.velocity.Y - num1054;
					if (npc.velocity.Y > 0f && num1056 < 0f)
					{
						npc.velocity.Y = npc.velocity.Y - num1054;
						return;
					}
				}
			}
			else if (npc.ai[0] == 1f)
			{
				npc.localAI[0] = 0f;
				npc.TargetClosest(true);
				Vector2 vector119 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				Vector2 vector120 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num1058 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector120.X;
				float num1059 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector120.Y;
				float num1060 = (float)Math.Sqrt((double)(num1058 * num1058 + num1059 * num1059));
				npc.ai[1] += 1f;
				npc.ai[1] += (float)(num1038 / 2);
				bool flag103 = false;
				if (npc.ai[1] > 10f)
				{
					npc.ai[1] = 0f;
					npc.ai[2] += 1f;
					flag103 = true;
				}
				if (Collision.CanHit(vector119, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && flag103)
				{
					Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 8);
					if (Main.netMode != 1)
					{
						int num1061;
						if (Main.rand.Next(4) == 0)
						{
							num1061 = mod.NPCType("PlagueBeeLargeG");
						}
						else
						{
							num1061 = mod.NPCType("PlagueBeeG");
						}
						if (NPC.CountNPCS(mod.NPCType("PlagueBeeG")) < 3)
						{
							int num1062 = NPC.NewNPC((int)vector119.X, (int)vector119.Y, num1061, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num1062].velocity.X = (float)Main.rand.Next(-200, 201) * 0.005f;
							Main.npc[num1062].velocity.Y = (float)Main.rand.Next(-200, 201) * 0.005f;
							Main.npc[num1062].localAI[0] = 60f;
							Main.npc[num1062].netUpdate = true;
						}
					}
				}
				if (num1060 > 400f || !Collision.CanHit(new Vector2(vector119.X, vector119.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					float num1063 = 14.5f; //changed from 14
					float num1064 = 0.105f; //changed from 0.1
					vector120 = vector119;
					num1058 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector120.X;
					num1059 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector120.Y;
					num1060 = (float)Math.Sqrt((double)(num1058 * num1058 + num1059 * num1059));
					num1060 = num1063 / num1060;
					if (npc.velocity.X < num1058)
					{
						npc.velocity.X = npc.velocity.X + num1064;
						if (npc.velocity.X < 0f && num1058 > 0f)
						{
							npc.velocity.X = npc.velocity.X + num1064;
						}
					}
					else if (npc.velocity.X > num1058)
					{
						npc.velocity.X = npc.velocity.X - num1064;
						if (npc.velocity.X > 0f && num1058 < 0f)
						{
							npc.velocity.X = npc.velocity.X - num1064;
						}
					}
					if (npc.velocity.Y < num1059)
					{
						npc.velocity.Y = npc.velocity.Y + num1064;
						if (npc.velocity.Y < 0f && num1059 > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num1064;
						}
					}
					else if (npc.velocity.Y > num1059)
					{
						npc.velocity.Y = npc.velocity.Y - num1064;
						if (npc.velocity.Y > 0f && num1059 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - num1064;
						}
					}
				}
				else
				{
					npc.velocity *= 0.9f; //changed from 0.9
				}
				npc.spriteDirection = npc.direction;
				if (npc.ai[2] > 2f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 1f;
					npc.netUpdate = true;
					return;
				}
			}
			else if (npc.ai[0] == 3f)
			{
				float num1065 = 7f; //changed from 4
				float num1066 = 0.075f; //changed from 0.05
				if (flag113)
				{
					num1066 = 0.09f;
					num1065 = 8f;
				}
				Vector2 vector121 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				Vector2 vector122 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num1067 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector122.X;
				float num1068 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 300f - vector122.Y;
				float num1069 = (float)Math.Sqrt((double)(num1067 * num1067 + num1068 * num1068));
				npc.ai[1] += 1f;
				bool flag104 = false;
				if (npc.ai[1] % 35f == 34f)
				{
					flag104 = true;
				}
				if (flag104 && npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y && Collision.CanHit(vector121, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 42);
					if (Main.netMode != 1)
					{
						float num1070 = 6f; //changed from 8
						if (flag113)
						{
							num1070 += 2f;
						}
						float num1071 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector121.X + (float)Main.rand.Next(-80, 81);
						float num1072 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector121.Y + (float)Main.rand.Next(-40, 41);
						float num1073 = (float)Math.Sqrt((double)(num1071 * num1071 + num1072 * num1072));
						num1073 = num1070 / num1073;
						num1071 *= num1073;
						num1072 *= num1073;
						int num1074 = 20; //projectile damage
						int num1075 = mod.ProjectileType("PlagueStingerGoliathV2"); //projectile type
						if (Main.rand.Next(15) == 0)
						{
							num1074 = 25;
							num1075 = mod.ProjectileType("HiveBombGoliath");
						}
						if (CalamityWorld.death)
						{
							num1074 += 20;
						}
						int num1076 = Projectile.NewProjectile(vector121.X, vector121.Y, num1071, num1072, num1075, num1074, 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[num1076].timeLeft = 300;
					}
				}
				if (!Collision.CanHit(new Vector2(vector121.X, vector121.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					num1065 = 14.5f; //changed from 14
					num1066 = 0.105f; //changed from 0.1
					vector122 = vector121;
					num1067 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector122.X;
					num1068 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector122.Y;
					num1069 = (float)Math.Sqrt((double)(num1067 * num1067 + num1068 * num1068));
					num1069 = num1065 / num1069;
					if (npc.velocity.X < num1067)
					{
						npc.velocity.X = npc.velocity.X + num1066;
						if (npc.velocity.X < 0f && num1067 > 0f)
						{
							npc.velocity.X = npc.velocity.X + num1066;
						}
					}
					else if (npc.velocity.X > num1067)
					{
						npc.velocity.X = npc.velocity.X - num1066;
						if (npc.velocity.X > 0f && num1067 < 0f)
						{
							npc.velocity.X = npc.velocity.X - num1066;
						}
					}
					if (npc.velocity.Y < num1068)
					{
						npc.velocity.Y = npc.velocity.Y + num1066;
						if (npc.velocity.Y < 0f && num1068 > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num1066;
						}
					}
					else if (npc.velocity.Y > num1068)
					{
						npc.velocity.Y = npc.velocity.Y - num1066;
						if (npc.velocity.Y > 0f && num1068 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - num1066;
						}
					}
				}
				else if (num1069 > 100f)
				{
					npc.TargetClosest(true);
					npc.spriteDirection = npc.direction;
					num1069 = num1065 / num1069;
					if (npc.velocity.X < num1067)
					{
						npc.velocity.X = npc.velocity.X + num1066;
						if (npc.velocity.X < 0f && num1067 > 0f)
						{
							npc.velocity.X = npc.velocity.X + num1066 * 2f;
						}
					}
					else if (npc.velocity.X > num1067)
					{
						npc.velocity.X = npc.velocity.X - num1066;
						if (npc.velocity.X > 0f && num1067 < 0f)
						{
							npc.velocity.X = npc.velocity.X - num1066 * 2f;
						}
					}
					if (npc.velocity.Y < num1068)
					{
						npc.velocity.Y = npc.velocity.Y + num1066;
						if (npc.velocity.Y < 0f && num1068 > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num1066 * 2f;
						}
					}
					else if (npc.velocity.Y > num1068)
					{
						npc.velocity.Y = npc.velocity.Y - num1066;
						if (npc.velocity.Y > 0f && num1068 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - num1066 * 2f;
						}
					}
				}
				if (npc.ai[1] > 600f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 3f;
					npc.netUpdate = true;
					return;
				}
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !NPC.downedGolemBoss)
			{
				return 0f;
			}
			return SpawnCondition.HardmodeJungle.Chance * 0.02f;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Pbg"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Pbg2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Pbg3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Pbg4"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Pbg5"), 1f);
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 100;
				npc.height = 100;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 40; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 46, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 70; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 46, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 46, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}

		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PlagueCellCluster"), Main.rand.Next(8, 13));
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("Plague"), 120, true);
		}
	}
}
