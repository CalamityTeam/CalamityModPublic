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
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using CalamityMod.Tiles;
using CalamityMod.World;

namespace CalamityMod.NPCs.PlaguebringerGoliath
{
	[AutoloadBossHead]
	public class PlaguebringerGoliath : ModNPC
	{
		private const float MissileAngleSpread = 90;
		private const int MissileProjectiles = 5;
		private int MissileCountdown = 0;
		private int despawnTimer = 600;
		private int chargeDistance = 0;
		private bool charging = false;
		private bool halfLife = false;
		private bool canDespawn = false;
		private bool flyingFrame2 = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Plaguebringer Goliath");
			Main.npcFrameCount[npc.type] = 4;
		}

		public override void SetDefaults()
		{
			npc.damage = 100; //150
			npc.npcSlots = 64f;
			npc.width = 198; //324
			npc.height = 198; //216
			npc.defense = 55;
			npc.lifeMax = CalamityWorld.revenge ? 77275 : 58500;
			if (CalamityWorld.death)
			{
				npc.lifeMax = 110000;
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 4000000 : 3700000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			npc.knockBackResist = 0f;
			npc.aiStyle = -1; //new
			aiType = -1; //new
			npc.boss = true;
			npc.value = Item.buyPrice(0, 25, 0, 0);
			NPCID.Sets.TrailCacheLength[npc.type] = 8;
			NPCID.Sets.TrailingMode[npc.type] = 1;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.buffImmune[BuffID.Ichor] = false;
			npc.buffImmune[BuffID.CursedInferno] = false;
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
			Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
			if (calamityModMusic != null)
				music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/PlaguebringerGoliath");
			else
				music = MusicID.Boss3;
			bossBag = mod.ItemType("PlaguebringerGoliathBag");
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(halfLife);
			writer.Write(canDespawn);
			writer.Write(flyingFrame2);
			writer.Write(MissileCountdown);
			writer.Write(despawnTimer);
			writer.Write(chargeDistance);
			writer.Write(charging);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			halfLife = reader.ReadBoolean();
			canDespawn = reader.ReadBoolean();
			flyingFrame2 = reader.ReadBoolean();
			MissileCountdown = reader.ReadInt32();
			despawnTimer = reader.ReadInt32();
			chargeDistance = reader.ReadInt32();
			charging = reader.ReadBoolean();
		}

		public override void AI()
		{
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.15f, 0.35f, 0.05f);
			if (!halfLife && ((double)npc.life <= (double)npc.lifeMax * 0.5 || CalamityWorld.death || CalamityWorld.bossRushActive))
			{
				string key = "Mods.CalamityMod.PlagueBossText";
				Color messageColor = Color.Lime;
				if (Main.netMode == 0)
				{
					Main.NewText(Language.GetTextValue(key), messageColor);
				}
				else if (Main.netMode == 2)
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				halfLife = true;
			}
			if (halfLife && MissileCountdown == 0)
			{
				MissileCountdown = 600;
			}
			if (MissileCountdown > 1)
			{
				MissileCountdown--;
			}
			int num1038 = 0;
			for (int num1039 = 0; num1039 < 255; num1039++)
			{
				if (Main.player[num1039].active && !Main.player[num1039].dead && (npc.Center - Main.player[num1039].Center).Length() < 1000f)
				{
					num1038++;
				}
			}
			if (expertMode)
			{
				int num1040 = (int)(50f * (1f - (float)npc.life / (float)npc.lifeMax));
				npc.defense = npc.defDefense + num1040;
			}
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
			{
				npc.TargetClosest(true);
			}
			Vector2 distFromPlayer = Main.player[npc.target].Center - npc.Center;
			bool aboveGroundEnrage = ((double)Main.player[npc.target].position.Y < Main.worldSurface * 16.0 ||
				Main.player[npc.target].position.Y > (float)((Main.maxTilesY - 200) * 16)) && !CalamityWorld.bossRushActive;
			bool jungleEnrage = false;
			if (!Main.player[npc.target].ZoneJungle && !CalamityWorld.bossRushActive)
			{
				jungleEnrage = true;
			}
			if (jungleEnrage || Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 5600f)
			{
				if (despawnTimer > 0)
				{
					despawnTimer--;
				}
			}
			else
			{
				despawnTimer = 600;
			}
			canDespawn = (despawnTimer <= 0);
			if (canDespawn)
			{
				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
				}
			}
			if (npc.ai[0] == -1f)
			{
				if (Main.netMode != 1)
				{
					float num595 = npc.ai[1];
					int num596;
					do
					{
						num596 = Main.rand.Next(3);
						if (MissileCountdown == 1)
						{
							num596 = 4;
						}
						else if (num596 == 1)
						{
							num596 = 2;
						}
						else if (num596 == 2)
						{
							num596 = 3;
						}
					}
					while ((float)num596 == num595);
					if (num596 == 0 && ((double)npc.life <= (double)npc.lifeMax * 0.8 || CalamityWorld.death) && distFromPlayer.Length() < 1800f)
					{
						switch (Main.rand.Next(3))
						{
							case 0: chargeDistance = 0; break;
							case 1: chargeDistance = 400; break;
							case 2: chargeDistance = -400; break;
						}
					}
					npc.ai[0] = (float)num596;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
				}
			}
			else if (npc.ai[0] == 0f)
			{
				int num1043 = 2; //2
				if ((npc.ai[1] > (float)(2 * num1043) && npc.ai[1] % 2f == 0f) || distFromPlayer.Length() > 1800f)
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
					float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
					if (Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - (float)chargeDistance)) < 20f)
					{
						switch (Main.rand.Next(3))
						{
							case 0: chargeDistance = 0; break;
							case 1: chargeDistance = 400; break;
							case 2: chargeDistance = -400; break;
						}
						charging = true;
						npc.ai[1] += 1f;
						npc.ai[2] = 0f;
						float num1044 = revenge ? 24f : 22f; //16
						if (aboveGroundEnrage)
						{
							num1044 += 6f;
						}
						if ((double)npc.life < (double)npc.lifeMax * 0.75)
						{
							num1044 += 2f; //2 not a prob
						}
						if ((double)npc.life < (double)npc.lifeMax * 0.5)
						{
							num1044 += 2f; //2 not a prob
						}
						if ((double)npc.life < (double)npc.lifeMax * 0.25)
						{
							num1044 += 2f; //2 not a prob
						}
						if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
						{
							num1044 += 2f; //2 not a prob
						}
						if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
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
						npc.direction = (playerLocation < 0 ? 1 : -1);
						npc.spriteDirection = npc.direction;
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0);
						return;
					}
					charging = false;
					float num1048 = revenge ? 10f : 9f; //12 not a prob
					float num1049 = revenge ? 0.15f : 0.13f; //0.15 not a prob
					if ((double)npc.life < (double)npc.lifeMax * 0.75)
					{
						num1048 += 1f; //1 not a prob
						num1049 += 0.05f; //0.05 not a prob
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.5)
					{
						num1048 += 1f; //1 not a prob
						num1049 += 0.05f; //0.05 not a prob
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.25)
					{
						num1048 += 2f; //2 not a prob
						num1049 += 0.05f; //0.05 not a prob
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
					{
						num1048 += 2f; //2 not a prob
						num1049 += 0.1f; //0.1 not a prob
					}
					if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
					{
						num1048 += 2f;
						num1049 += 0.1f;
					}
					if (npc.position.Y + (float)(npc.height / 2) < (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - (float)chargeDistance))
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
					npc.direction = (playerLocation < 0 ? 1 : -1);
					npc.spriteDirection = npc.direction;
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
					int num1050 = 600; //600 not a prob
					if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
					{
						num1050 = 250;
					}
					else if (aboveGroundEnrage)
					{
						num1050 = 350;
					}
					else if (CalamityWorld.death || CalamityWorld.bossRushActive)
					{
						num1050 = 400;
					}
					else if ((double)npc.life < (double)npc.lifeMax * 0.1)
					{
						num1050 = revenge ? 425 : 450; //300 not a prob
					}
					else if ((double)npc.life < (double)npc.lifeMax * 0.25)
					{
						num1050 = revenge ? 450 : 475; //450 not a prob
					}
					else if ((double)npc.life < (double)npc.lifeMax * 0.5)
					{
						num1050 = 500; //500 not a prob
					}
					else if ((double)npc.life < (double)npc.lifeMax * 0.75)
					{
						num1050 = 550; //550 not a prob
					}
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
						charging = true;
						return;
					}
					npc.TargetClosest(true);
					npc.spriteDirection = npc.direction;
					charging = false;
					npc.velocity *= 0.9f;
					float num1052 = revenge ? 0.13f : 0.115f; //0.1
					if (npc.life < npc.lifeMax / 2)
					{
						npc.velocity *= 0.9f;
						num1052 += 0.05f; //0.05
					}
					if (npc.life < npc.lifeMax / 3)
					{
						npc.velocity *= 0.9f;
						num1052 += 0.05f; //0.05
					}
					if (npc.life < npc.lifeMax / 5 || CalamityWorld.bossRushActive)
					{
						npc.velocity *= 0.9f;
						num1052 += 0.05f; //0.05
					}
					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num1052)
					{
						npc.ai[2] = 0f;
						npc.ai[1] += 1f;
					}
				}
			}
			else if (npc.ai[0] == 2f)
			{
				npc.TargetClosest(true);
				float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
				npc.direction = (playerLocation < 0 ? 1 : -1);
				npc.spriteDirection = npc.direction;
				float num1053 = 12f;
				float num1054 = 0.2f;
				float num10542 = 0.1f;
				Vector2 vector118 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num1055 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector118.X;
				float num1056 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 200f - vector118.Y;
				float num1057 = (float)Math.Sqrt((double)(num1055 * num1055 + num1056 * num1056));
				if (num1057 < 800f)
				{
					npc.ai[0] = (((double)npc.life <= (double)npc.lifeMax * 0.66 || CalamityWorld.death) ? 5f : 1f);
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
					npc.velocity.Y = npc.velocity.Y + num10542;
					if (npc.velocity.Y < 0f && num1056 > 0f)
					{
						npc.velocity.Y = npc.velocity.Y + num10542;
					}
				}
				else if (npc.velocity.Y > num1056)
				{
					npc.velocity.Y = npc.velocity.Y - num10542;
					if (npc.velocity.Y > 0f && num1056 < 0f)
					{
						npc.velocity.Y = npc.velocity.Y - num10542;
					}
				}
			}
			else if (npc.ai[0] == 1f)
			{
				charging = false;
				npc.TargetClosest(true);
				Vector2 vector119 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				Vector2 vector120 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num1058 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector120.X;
				float num1059 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector120.Y;
				float num1060 = (float)Math.Sqrt((double)(num1058 * num1058 + num1059 * num1059));
				npc.ai[1] += 1f;
				npc.ai[1] += (float)(num1038 / 2);
				if ((double)npc.life < (double)npc.lifeMax * 0.75 || CalamityWorld.bossRushActive)
				{
					npc.ai[1] += 0.25f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
				{
					npc.ai[1] += 0.25f;
				}
				bool flag103 = false;
				if (npc.ai[1] > 40f)
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
						int randomAmt = expertMode ? 2 : 4;
						int num1061;
						if (Main.rand.Next(randomAmt) == 0)
						{
							num1061 = mod.NPCType("PlagueBeeLargeG");
						}
						else
						{
							num1061 = mod.NPCType("PlagueBeeG");
						}
						if (expertMode && NPC.CountNPCS(mod.NPCType("PlagueMine")) < (aboveGroundEnrage ? 4 : 2))
						{
							NPC.NewNPC((int)vector119.X, (int)vector119.Y, mod.NPCType("PlagueMine"), 0, 0f, 0f, 0f, 0f, 255);
						}
						if (revenge && NPC.CountNPCS(mod.NPCType("PlaguebringerShade")) < (aboveGroundEnrage ? 2 : 1))
						{
							NPC.NewNPC((int)vector119.X, (int)vector119.Y, mod.NPCType("PlaguebringerShade"), 0, 0f, 0f, 0f, 0f, 255);
						}
						if (NPC.CountNPCS(mod.NPCType("PlagueBeeLargeG")) < 2)
						{
							int num1062 = NPC.NewNPC((int)vector119.X, (int)vector119.Y, num1061, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num1062].velocity.X = (float)Main.rand.Next(-200, 201) * 0.02f;
							Main.npc[num1062].velocity.Y = (float)Main.rand.Next(-200, 201) * 0.02f;
							Main.npc[num1062].localAI[0] = 60f;
							Main.npc[num1062].netUpdate = true;
						}
					}
				}
				if (num1060 > 800f || !Collision.CanHit(new Vector2(vector119.X, vector119.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					float num1063 = 14f; //changed from 14 not a prob
					float num1064 = 0.2f; //changed from 0.1 not a prob
					float num10642 = 0.07f;
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
						npc.velocity.Y = npc.velocity.Y + num10642;
						if (npc.velocity.Y < 0f && num1059 > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num10642;
						}
					}
					else if (npc.velocity.Y > num1059)
					{
						npc.velocity.Y = npc.velocity.Y - num10642;
						if (npc.velocity.Y > 0f && num1059 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - num10642;
						}
					}
				}
				else
				{
					npc.velocity *= 0.9f;
				}
				float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
				npc.direction = (playerLocation < 0 ? 1 : -1);
				npc.spriteDirection = npc.direction;
				if (npc.ai[2] > 3f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 1f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 5f)
			{
				charging = false;
				npc.TargetClosest(true);
				Vector2 vector119 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				Vector2 vector120 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num1058 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector120.X;
				float num1059 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector120.Y;
				float num1060 = (float)Math.Sqrt((double)(num1058 * num1058 + num1059 * num1059));
				npc.ai[1] += 1f;
				npc.ai[1] += (float)(num1038 / 2);
				bool flag103 = false;
				if ((double)npc.life < (double)npc.lifeMax * 0.25 || CalamityWorld.bossRushActive)
				{
					npc.ai[1] += 0.25f; //0.25 not a prob
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
				{
					npc.ai[1] += 0.25f; //0.25 not a prob
				}
				if (npc.ai[1] > 40f) //changed from 40 not a prob
				{
					npc.ai[1] = 0f;
					npc.ai[2] += 1f;
					flag103 = true;
				}
				if (Collision.CanHit(vector119, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && flag103)
				{
					Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 88);
					if (Main.netMode != 1)
					{
						if (expertMode && NPC.CountNPCS(mod.NPCType("PlagueMine")) < (aboveGroundEnrage ? 6 : 4))
						{
							NPC.NewNPC((int)vector119.X, (int)vector119.Y, mod.NPCType("PlagueMine"), 0, 0f, 0f, 0f, 0f, 255);
						}
						if (revenge && NPC.CountNPCS(mod.NPCType("PlaguebringerShade")) < 1 && aboveGroundEnrage)
						{
							NPC.NewNPC((int)vector119.X, (int)vector119.Y, mod.NPCType("PlaguebringerShade"), 0, 0f, 0f, 0f, 0f, 255);
						}
						float projectileSpeed = revenge ? 6f : 5f;
						float num1071 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector119.X + (float)Main.rand.Next(-80, 81);
						float num1072 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector119.Y + (float)Main.rand.Next(-40, 41);
						float num1073 = (float)Math.Sqrt((double)(num1071 * num1071 + num1072 * num1072));
						num1073 = projectileSpeed / num1073;
						num1071 *= num1073;
						num1072 *= num1073;
						if (NPC.CountNPCS(mod.NPCType("PlagueHomingMissile")) < (aboveGroundEnrage ? 8 : 5))
						{
							int num1062 = NPC.NewNPC((int)vector119.X, (int)vector119.Y, mod.NPCType("PlagueHomingMissile"), 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num1062].velocity.X = num1071;
							Main.npc[num1062].velocity.Y = num1072;
							Main.npc[num1062].netUpdate = true;
						}
					}
				}
				if (num1060 > 800f || !Collision.CanHit(new Vector2(vector119.X, vector119.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					float num1063 = 14f; //changed from 14 not a prob
					float num1064 = 0.2f; //changed from 0.1 not a prob
					float num10642 = 0.07f;
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
						npc.velocity.Y = npc.velocity.Y + num10642;
						if (npc.velocity.Y < 0f && num1059 > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num10642;
						}
					}
					else if (npc.velocity.Y > num1059)
					{
						npc.velocity.Y = npc.velocity.Y - num10642;
						if (npc.velocity.Y > 0f && num1059 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - num10642;
						}
					}
				}
				else
				{
					npc.velocity *= 0.9f;
				}
				float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
				npc.direction = (playerLocation < 0 ? 1 : -1);
				npc.spriteDirection = npc.direction;
				if (npc.ai[2] > 3f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 1f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 3f)
			{
				Vector2 vector121 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				npc.ai[1] += 1f;
				bool flag104 = false;
				if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
				{
					if (npc.ai[1] % 10f == 9f)
					{
						flag104 = true;
					}
				}
				else if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.death || CalamityWorld.bossRushActive || aboveGroundEnrage)
				{
					if (npc.ai[1] % 20f == 19f)
					{
						flag104 = true;
					}
				}
				else if ((double)npc.life < (double)npc.lifeMax * 0.5)
				{
					if (npc.ai[1] % 25f == 24f)
					{
						flag104 = true;
					}
				}
				else if (npc.ai[1] % 30f == 29f)
				{
					flag104 = true;
				}
				if (flag104 && npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y && Collision.CanHit(vector121, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 42);
					if (Main.netMode != 1)
					{
						float projectileSpeed = revenge ? 6.5f : 6f;
						if (jungleEnrage || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
						{
							projectileSpeed += 10f;
						}
						float num1071 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector121.X + (float)Main.rand.Next(-80, 81);
						float num1072 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector121.Y + (float)Main.rand.Next(-40, 41);
						float num1073 = (float)Math.Sqrt((double)(num1071 * num1071 + num1072 * num1072));
						num1073 = projectileSpeed / num1073;
						num1071 *= num1073;
						num1072 *= num1073;
						int num1074 = 40; //projectile damage
						int num1075 = (Main.rand.Next(2) == 0 ? mod.ProjectileType("PlagueStingerGoliath") : mod.ProjectileType("PlagueStingerGoliathV2"));
						if (expertMode)
						{
							num1074 = 28; //112
							int damageBoost = (int)(6f * (1f - (float)npc.life / (float)npc.lifeMax));
							num1074 += damageBoost; //112 to 136
							if (Main.rand.Next(6) == 0)
							{
								num1074 += 8; //144 to 168
								num1075 = mod.ProjectileType("HiveBombGoliath");
							}
						}
						else
						{
							if (Main.rand.Next(9) == 0)
							{
								num1074 = 50;
								num1075 = mod.ProjectileType("HiveBombGoliath");
							}
						}
						Projectile.NewProjectile(vector121.X, vector121.Y, num1071, num1072, num1075, num1074, 0f, Main.myPlayer, -1f, 0f);
					}
				}
				if (npc.position.Y > Main.player[npc.target].position.Y - 200f) //200
				{
					if (npc.velocity.Y > 0f)
					{
						npc.velocity.Y = npc.velocity.Y * 0.98f;
					}
					npc.velocity.Y = npc.velocity.Y - 0.1f;
					if (npc.velocity.Y > 2f)
					{
						npc.velocity.Y = 2f;
					}
				}
				else if (npc.position.Y < Main.player[npc.target].position.Y - 500f) //500
				{
					if (npc.velocity.Y < 0f)
					{
						npc.velocity.Y = npc.velocity.Y * 0.98f;
					}
					npc.velocity.Y = npc.velocity.Y + 0.1f;
					if (npc.velocity.Y < -2f)
					{
						npc.velocity.Y = -2f;
					}
				}
				if (npc.position.X + (float)(npc.width / 2) > Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + 100f)
				{
					if (npc.velocity.X > 0f)
					{
						npc.velocity.X = npc.velocity.X * 0.98f;
					}
					npc.velocity.X = npc.velocity.X - 0.1f;
					if (npc.velocity.X > 8f)
					{
						npc.velocity.X = 8f;
					}
				}
				if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - 100f)
				{
					if (npc.velocity.X < 0f)
					{
						npc.velocity.X = npc.velocity.X * 0.98f;
					}
					npc.velocity.X = npc.velocity.X + 0.1f;
					if (npc.velocity.X < -8f)
					{
						npc.velocity.X = -8f;
					}
				}
				float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
				npc.direction = (playerLocation < 0 ? 1 : -1);
				npc.spriteDirection = npc.direction;
				if (npc.ai[1] > 300f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 3f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 4f)
			{
				int num1043 = 2; //2
				if (npc.ai[1] > (float)(2 * num1043) && npc.ai[1] % 2f == 0f)
				{
					MissileCountdown = 0;
					npc.ai[0] = -1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.netUpdate = true;
					return;
				}
				if (npc.ai[1] % 2f == 0f)
				{
					npc.TargetClosest(true);
					float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
					if (Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 500f)) < 20f)
					{
						if (MissileCountdown == 1)
						{
							Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 116);
							if (Main.netMode != 1)
							{
								int speed = revenge ? 6 : 5;
								float spawnX = Main.rand.Next(1000) - 500 + npc.Center.X;
								float spawnY = npc.Center.Y;
								Vector2 baseSpawn = new Vector2(spawnX, spawnY);
								Vector2 baseVelocity = Main.player[npc.target].Center - baseSpawn;
								baseVelocity.Normalize();
								baseVelocity = baseVelocity * speed;
								int damage = expertMode ? 42 : 57;
								for (int i = 0; i < MissileProjectiles; i++)
								{
									Vector2 spawn = baseSpawn;
									spawn.X = spawn.X + i * 30 - (MissileProjectiles * 15);
									Vector2 velocity = baseVelocity;
									velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-MissileAngleSpread / 2 + (MissileAngleSpread * i / (float)MissileProjectiles)));
									velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
									Projectile.NewProjectile(spawn.X, spawn.Y, velocity.X, velocity.Y, mod.ProjectileType("HiveBombGoliath"), damage, 10f, Main.myPlayer, 0f, Main.player[npc.target].position.Y);
								}
							}
						}
						charging = true;
						npc.ai[1] += 1f;
						npc.ai[2] = 0f;
						float num1044 = revenge ? 28f : 26f; //16
						Vector2 vector117 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num1045 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector117.X;
						float num1046 = (Main.player[npc.target].position.Y - 500f) + (float)(Main.player[npc.target].height / 2) - vector117.Y;
						float num1047 = (float)Math.Sqrt((double)(num1045 * num1045 + num1046 * num1046));
						num1047 = num1044 / num1047;
						npc.velocity.X = num1045 * num1047;
						npc.velocity.Y = num1046 * num1047;
						npc.direction = (playerLocation < 0 ? 1 : -1);
						npc.spriteDirection = npc.direction;
						return;
					}
					charging = false;
					float num1048 = 12f; //12 not a prob
					float num1049 = 0.15f; //0.15 not a prob
					if (npc.position.Y + (float)(npc.height / 2) < (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 500f))
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
					npc.direction = (playerLocation < 0 ? 1 : -1);
					npc.spriteDirection = npc.direction;
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
					int num1050 = 600; //600 not a prob
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
						charging = true;
						return;
					}
					npc.TargetClosest(true);
					npc.spriteDirection = npc.direction;
					charging = false;
					npc.velocity *= 0.9f;
					float num1052 = revenge ? 0.13f : 0.115f; //0.1
					if (npc.life < npc.lifeMax / 2)
					{
						npc.velocity *= 0.9f;
						num1052 += 0.05f; //0.05
					}
					if (npc.life < npc.lifeMax / 3)
					{
						npc.velocity *= 0.9f;
						num1052 += 0.05f; //0.05
					}
					if (npc.life < npc.lifeMax / 5 || CalamityWorld.bossRushActive)
					{
						npc.velocity *= 0.9f;
						num1052 += 0.05f; //0.05
					}
					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num1052)
					{
						npc.ai[2] = 0f;
						npc.ai[1] += 1f;
					}
				}
			}
		}

		public override bool CheckActive()
		{
			return canDespawn;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 2; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Pbg"), 2f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Pbg2"), 2f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Pbg3"), 2f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Pbg4"), 2f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Pbg5"), 2f);
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

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Mod mod = ModLoader.GetMod("CalamityMod");
			Texture2D texture = Main.npcTexture[npc.type];
			if (charging)
			{
				texture = mod.GetTexture("NPCs/PlaguebringerGoliath/PlaguebringerGoliathChargeTex");
			}
			else
			{
				if (!flyingFrame2)
				{
					texture = Main.npcTexture[npc.type];
				}
				else
				{
					texture = mod.GetTexture("NPCs/PlaguebringerGoliath/PlaguebringerGoliathAltTex");
				}
			}
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Microsoft.Xna.Framework.Color color24 = npc.GetAlpha(drawColor);
			Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
			int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
			int y3 = num156 * (int)npc.frameCounter;
			Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, y3, texture.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			int num157 = 8;
			int num158 = 2;
			int num159 = 1;
			float num160 = 0f;
			int num161 = num159;
			while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)) && Lighting.NotRetro)
			{
				Microsoft.Xna.Framework.Color color26 = npc.GetAlpha(color25);
				{
					goto IL_6899;
				}
			IL_6881:
				num161 += num158;
				continue;
			IL_6899:
				float num164 = (float)(num157 - num161);
				if (num158 < 0)
				{
					num164 = (float)(num159 - num161);
				}
				color26 *= num164 / ((float)NPCID.Sets.TrailCacheLength[npc.type] * 1.5f);
				Vector2 value4 = (npc.oldPos[num161]);
				float num165 = npc.rotation;
				Main.spriteBatch.Draw(texture, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, spriteEffects, 0f);
				goto IL_6881;
			}
			var something = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, color24, npc.rotation, npc.frame.Size() / 2, npc.scale, something, 0);
			return false;
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += 1.0;
			if (npc.frameCounter > 4.0)
			{
				npc.frame.Y = npc.frame.Y + frameHeight;
				npc.frameCounter = 0.0;
			}
			if (npc.frame.Y >= frameHeight * 4)
			{
				npc.frame.Y = 0;
				if (!charging)
				{
					flyingFrame2 = !flyingFrame2;
				}
			}
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.GreaterHealingPotion;
		}

		public override void NPCLoot()
		{
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, mod.ItemType("PlaguebringerGoliathTrophy"), 10);
            DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge32"), true, !CalamityWorld.downedPlaguebringer);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedPlaguebringer, 4, 2, 1);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
			{
                // Materials
                DropHelper.DropItemSpray(npc, mod.ItemType("PlagueCellCluster"), 10, 14);

                // Weapons
                DropHelper.DropItemChance(npc, mod.ItemType("VirulentKatana"), 4); // Virulence
                DropHelper.DropItemChance(npc, mod.ItemType("DiseasedPike"), 4);
                DropHelper.DropItemChance(npc, mod.ItemType("ThePlaguebringer"), 4); // Pandemic
                DropHelper.DropItemChance(npc, mod.ItemType("Malevolence"), 4);
                DropHelper.DropItemChance(npc, mod.ItemType("PestilentDefiler"), 4);
                DropHelper.DropItemChance(npc, mod.ItemType("TheHive"), 4);
                DropHelper.DropItemChance(npc, mod.ItemType("MepheticSprayer"), 4); // Blight Spewer
                DropHelper.DropItemChance(npc, mod.ItemType("PlagueStaff"), 4);

                // Equipment
                DropHelper.DropItemChance(npc, mod.ItemType("BloomStone"), 10);

                // Vanity
                DropHelper.DropItemChance(npc, mod.ItemType("PlaguebringerGoliathMask"), 7);
			}

            // Mark PBG as dead
            CalamityWorld.downedPlaguebringer = true;
            CalamityGlobalNPC.UpdateServerBoolean();
        }

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("Plague"), 300, true);
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 180, true);
				player.AddBuff(mod.BuffType("MarkedforDeath"), 180);
			}
		}
	}
}
