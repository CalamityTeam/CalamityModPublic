using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.TheDevourerofGods
{
	[AutoloadBossHead]
	public class DevourerofGodsHead : ModNPC
	{
		private bool tail = false;
		private const int minLength = 100;
		private const int maxLength = 101;
		private bool halfLife = false;
		private bool halfLife2 = false;
		private int spawnDoGCountdown = 0;
		private int phaseSwitch = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Devourer of Gods");
		}

		public override void SetDefaults()
		{
			npc.damage = 250; //150
			npc.npcSlots = 5f;
			npc.width = 64; //324
			npc.height = 76; //216
			npc.defense = 0;
			npc.lifeMax = CalamityWorld.revenge ? 500000 : 450000; //1000000 960000
			if (CalamityWorld.death)
			{
				npc.lifeMax = 850000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			npc.takenDamageMultiplier = 1.25f;
			npc.aiStyle = 6; //new
			aiType = -1; //new
			animationType = 10; //new
			npc.knockBackResist = 0f;
			npc.scale = 1.4f;
			npc.boss = true;
			npc.value = Item.buyPrice(0, 75, 0, 0);
			npc.alpha = 255;
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.netAlways = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
			if (calamityModMusic != null)
				music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse");
			else
				music = MusicID.Boss3;
			if (Main.expertMode)
			{
				npc.scale = 1.5f;
			}
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(halfLife);
			writer.Write(halfLife2);
			writer.Write(spawnDoGCountdown);
			writer.Write(phaseSwitch);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			halfLife = reader.ReadBoolean();
			halfLife2 = reader.ReadBoolean();
			spawnDoGCountdown = reader.ReadInt32();
			phaseSwitch = reader.ReadInt32();
		}

		public override void AI()
		{
			CalamityGlobalNPC.DoGHead = npc.whoAmI;
			float playerRunAcceleration = Main.player[npc.target].velocity.Y == 0f ? Math.Abs(Main.player[npc.target].moveSpeed * 0.3f) : (Main.player[npc.target].runAcceleration * 0.8f);
			if (playerRunAcceleration <= 1f)
				playerRunAcceleration = 1f;

			if (Main.raining)
				CalamityGlobalNPC.StopRain();

			Vector2 vector = npc.Center;
			bool flies = npc.ai[2] == 0f;
			bool expertMode = Main.expertMode;
			bool speedBoost1 = (double)npc.life <= (double)npc.lifeMax * 0.8; //speed increase
			bool speedBoost2 = (double)npc.life <= (double)npc.lifeMax * 0.6; //speed increase
			bool speedBoost3 = (double)npc.life <= (double)npc.lifeMax * 0.4; //speed increase
			bool speedBoost4 = (double)npc.life <= (double)npc.lifeMax * 0.2; //speed increase
			bool speedBoost5 = (double)npc.life <= (double)npc.lifeMax * 0.1; //speed increase
			if (speedBoost4)
			{
				if (!halfLife)
				{
					if (CalamityWorld.revenge)
					{
						spawnDoGCountdown = 10;
					}
					string key = "Mods.CalamityMod.EdgyBossText";
					Color messageColor = Color.Cyan;
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
				if (spawnDoGCountdown > 0)
				{
					spawnDoGCountdown--;
					if (spawnDoGCountdown == 0 && Main.netMode != 1)
					{
						for (int i = 0; i < 2; i++)
						{
							NPC.SpawnOnPlayer(npc.FindClosestPlayer(), mod.NPCType("DevourerofGodsHead2"));
						}
					}
				}
			}
			else if (speedBoost2)
			{
				if (!halfLife2)
				{
					if (CalamityWorld.revenge)
					{
						spawnDoGCountdown = 10;
					}
					halfLife2 = true;
				}
				if (spawnDoGCountdown > 0)
				{
					spawnDoGCountdown--;
					if (spawnDoGCountdown == 0 && Main.netMode != 1)
					{
						NPC.SpawnOnPlayer(npc.FindClosestPlayer(), mod.NPCType("DevourerofGodsHead2"));
					}
				}
			}
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
			if (npc.ai[3] > 0f)
			{
				npc.realLife = (int)npc.ai[3];
			}
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			npc.velocity.Length();
			if (npc.alpha != 0)
			{
				for (int spawnDust = 0; spawnDust < 2; spawnDust++)
				{
					int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num935].noGravity = true;
					Main.dust[num935].noLight = true;
				}
			}
			npc.alpha -= 12;
			if (npc.alpha < 0)
			{
				npc.alpha = 0;
			}
			if (Main.netMode != 1)
			{
				if (!tail && npc.ai[0] == 0f)
				{
					int Previous = npc.whoAmI;
					for (int segmentSpawn = 0; segmentSpawn < maxLength; segmentSpawn++)
					{
						int segment = 0;
						if (segmentSpawn >= 0 && segmentSpawn < minLength)
						{
							segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("DevourerofGodsBody"), npc.whoAmI);
						}
						else
						{
							segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("DevourerofGodsTail"), npc.whoAmI);
						}
						Main.npc[segment].realLife = npc.whoAmI;
						Main.npc[segment].ai[2] = (float)npc.whoAmI;
						Main.npc[segment].ai[1] = (float)Previous;
						Main.npc[Previous].ai[0] = (float)segment;
						npc.netUpdate = true;
						Previous = segment;
					}
					tail = true;
				}
				if (!npc.active && Main.netMode == 2)
				{
					NetMessage.SendData(28, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
				}
			}
			if (Main.player[npc.target].dead)
			{
				flies = true;
				npc.velocity.Y = npc.velocity.Y - 3f;
				if ((double)npc.position.Y < Main.topWorld + 16f)
				{
					npc.velocity.Y = npc.velocity.Y - 3f;
				}
				if ((double)npc.position.Y < Main.topWorld + 16f)
				{
					for (int a = 0; a < 200; a++)
					{
						if (Main.npc[a].aiStyle == npc.aiStyle)
						{
							Main.npc[a].active = false;
						}
					}
				}
			}
			int num180 = (int)(npc.position.X / 16f) - 1;
			int num181 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
			int num182 = (int)(npc.position.Y / 16f) - 1;
			int num183 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
			if (num180 < 0)
			{
				num180 = 0;
			}
			if (num181 > Main.maxTilesX)
			{
				num181 = Main.maxTilesX;
			}
			if (num182 < 0)
			{
				num182 = 0;
			}
			if (num183 > Main.maxTilesY)
			{
				num183 = Main.maxTilesY;
			}
			if (npc.ai[2] == 0f)
			{
				if (Main.netMode != 2)
				{
					if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < 5600f)
					{
						Main.player[Main.myPlayer].AddBuff(mod.BuffType("Warped"), 2);
					}
				}
				phaseSwitch += 1;
				npc.localAI[1] = 0f;
				float speed = playerRunAcceleration * 15f;
				float turnSpeed = playerRunAcceleration * 0.3f;
				float homingSpeed = playerRunAcceleration * 18f;
				float homingTurnSpeed = playerRunAcceleration * 0.33f;
				if (Vector2.Distance(Main.player[npc.target].Center, vector) > 5600f) //RAGE
				{
					phaseSwitch += 9;
				}
				else if ((expertMode && speedBoost5) || CalamityWorld.death)
				{
					homingSpeed = playerRunAcceleration * 25f;
					homingTurnSpeed = playerRunAcceleration * 0.52f;
				}
				else if (speedBoost4)
				{
					homingSpeed = playerRunAcceleration * 23f;
					homingTurnSpeed = playerRunAcceleration * 0.47f;
				}
				else if (speedBoost3)
				{
					homingSpeed = playerRunAcceleration * 21.5f;
					homingTurnSpeed = playerRunAcceleration * 0.43f;
				}
				else if (speedBoost2)
				{
					homingSpeed = playerRunAcceleration * 20.5f;
					homingTurnSpeed = playerRunAcceleration * 0.39f;
				}
				else if (speedBoost1)
				{
					homingSpeed = playerRunAcceleration * 19f;
					homingTurnSpeed = playerRunAcceleration * 0.36f;
				}
				float num188 = speed;
				float num189 = turnSpeed;
				Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
				float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
				int num42 = -1;
				int num43 = (int)(Main.player[npc.target].Center.X / 16f);
				int num44 = (int)(Main.player[npc.target].Center.Y / 16f);
				for (int num45 = num43 - 2; num45 <= num43 + 2; num45++)
				{
					for (int num46 = num44; num46 <= num44 + 15; num46++)
					{
						if (WorldGen.SolidTile2(num45, num46))
						{
							num42 = num46;
							break;
						}
					}
					if (num42 > 0)
					{
						break;
					}
				}
				if (num42 > 0)
				{
					num42 *= 16;
					float num47 = (float)(num42 - 800);
					if (Main.player[npc.target].position.Y > num47)
					{
						num192 = num47;
						if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 500f)
						{
							if (npc.velocity.X > 0f)
							{
								num191 = Main.player[npc.target].Center.X + 600f;
							}
							else
							{
								num191 = Main.player[npc.target].Center.X - 600f;
							}
						}
					}
				}
				else
				{
					num188 = homingSpeed;
					num189 = homingTurnSpeed;
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
				if (num42 > 0)
				{
					for (int num51 = 0; num51 < 200; num51++)
					{
						if (Main.npc[num51].active && Main.npc[num51].type == npc.type && num51 != npc.whoAmI)
						{
							Vector2 vector3 = Main.npc[num51].Center - npc.Center;
							if (vector3.Length() < 400f)
							{
								vector3.Normalize();
								vector3 *= 1000f;
								num191 -= vector3.X;
								num192 -= vector3.Y;
							}
						}
					}
				}
				else
				{
					for (int num52 = 0; num52 < 200; num52++)
					{
						if (Main.npc[num52].active && Main.npc[num52].type == npc.type && num52 != npc.whoAmI)
						{
							Vector2 vector4 = Main.npc[num52].Center - npc.Center;
							if (vector4.Length() < 60f)
							{
								vector4.Normalize();
								vector4 *= 200f;
								num191 -= vector4.X;
								num192 -= vector4.Y;
							}
						}
					}
				}
				num191 = (float)((int)(num191 / 16f) * 16);
				num192 = (float)((int)(num192 / 16f) * 16);
				vector18.X = (float)((int)(vector18.X / 16f) * 16);
				vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
				num191 -= vector18.X;
				num192 -= vector18.Y;
				float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
				if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
				{
					try
					{
						vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						num191 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
						num192 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
					}
					catch
					{
					}
					npc.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
					num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
					int num194 = npc.width;
					num193 = (num193 - (float)num194) / num193;
					num191 *= num193;
					num192 *= num193;
					npc.velocity = Vector2.Zero;
					npc.position.X = npc.position.X + num191;
					npc.position.Y = npc.position.Y + num192;
				}
				else
				{
					num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
					float num196 = System.Math.Abs(num191);
					float num197 = System.Math.Abs(num192);
					float num198 = num188 / num193;
					num191 *= num198;
					num192 *= num198;
					if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
					{
						if (npc.velocity.X < num191)
						{
							npc.velocity.X = npc.velocity.X + num189;
						}
						else
						{
							if (npc.velocity.X > num191)
							{
								npc.velocity.X = npc.velocity.X - num189;
							}
						}
						if (npc.velocity.Y < num192)
						{
							npc.velocity.Y = npc.velocity.Y + num189;
						}
						else
						{
							if (npc.velocity.Y > num192)
							{
								npc.velocity.Y = npc.velocity.Y - num189;
							}
						}
						if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
						{
							if (npc.velocity.Y > 0f)
							{
								npc.velocity.Y = npc.velocity.Y + num189 * 2f;
							}
							else
							{
								npc.velocity.Y = npc.velocity.Y - num189 * 2f;
							}
						}
						if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
						{
							if (npc.velocity.X > 0f)
							{
								npc.velocity.X = npc.velocity.X + num189 * 2f; //changed from 2
							}
							else
							{
								npc.velocity.X = npc.velocity.X - num189 * 2f; //changed from 2
							}
						}
					}
					else
					{
						if (num196 > num197)
						{
							if (npc.velocity.X < num191)
							{
								npc.velocity.X = npc.velocity.X + num189 * 1.1f; //changed from 1.1
							}
							else if (npc.velocity.X > num191)
							{
								npc.velocity.X = npc.velocity.X - num189 * 1.1f; //changed from 1.1
							}
							if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
							{
								if (npc.velocity.Y > 0f)
								{
									npc.velocity.Y = npc.velocity.Y + num189;
								}
								else
								{
									npc.velocity.Y = npc.velocity.Y - num189;
								}
							}
						}
						else
						{
							if (npc.velocity.Y < num192)
							{
								npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
							}
							else if (npc.velocity.Y > num192)
							{
								npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
							}
							if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
							{
								if (npc.velocity.X > 0f)
								{
									npc.velocity.X = npc.velocity.X + num189;
								}
								else
								{
									npc.velocity.X = npc.velocity.X - num189;
								}
							}
						}
					}
				}
				npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
				if (phaseSwitch > 900)
				{
					npc.ai[2] = 1f;
					phaseSwitch = 0;
					npc.netUpdate = true;
					return;
				}
			}
			else if (npc.ai[2] == 1f)
			{
				if (Main.netMode != 2)
				{
					if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < 5600f)
					{
						Main.player[Main.myPlayer].AddBuff(mod.BuffType("ExtremeGrav"), 2);
					}
				}
				phaseSwitch += 1;
				float speed = playerRunAcceleration * 19f;
				float turnSpeed = playerRunAcceleration * 0.28f;
				if (Vector2.Distance(Main.player[npc.target].Center, vector) > 5600f) //RAGE
				{
					speed = playerRunAcceleration * 80f;
					turnSpeed = playerRunAcceleration * 1f;
				}
				else if ((expertMode && speedBoost5) || CalamityWorld.death)
				{
					speed = playerRunAcceleration * 26f;
					turnSpeed = playerRunAcceleration * 0.45f;
				}
				else if (speedBoost4)
				{
					speed = playerRunAcceleration * 24.5f;
					turnSpeed = playerRunAcceleration * 0.4f;
				}
				else if (speedBoost3)
				{
					speed = playerRunAcceleration * 23f;
					turnSpeed = playerRunAcceleration * 0.36f;
				}
				else if (speedBoost2)
				{
					speed = playerRunAcceleration * 21.5f;
					turnSpeed = playerRunAcceleration * 0.33f;
				}
				else if (speedBoost1)
				{
					speed = playerRunAcceleration * 20f;
					turnSpeed = playerRunAcceleration * 0.3f;
				}
				if (!flies)
				{
					for (int num952 = num180; num952 < num181; num952++)
					{
						for (int num953 = num182; num953 < num183; num953++)
						{
							if (Main.tile[num952, num953] != null && ((Main.tile[num952, num953].nactive() && (Main.tileSolid[(int)Main.tile[num952, num953].type] || (Main.tileSolidTop[(int)Main.tile[num952, num953].type] && Main.tile[num952, num953].frameY == 0))) || Main.tile[num952, num953].liquid > 64))
							{
								Vector2 vector105;
								vector105.X = (float)(num952 * 16);
								vector105.Y = (float)(num953 * 16);
								if (npc.position.X + (float)npc.width > vector105.X && npc.position.X < vector105.X + 16f && npc.position.Y + (float)npc.height > vector105.Y && npc.position.Y < vector105.Y + 16f)
								{
									flies = true;
									break;
								}
							}
						}
					}
				}
				if (!flies)
				{
					npc.localAI[1] = 1f;
					Rectangle rectangle12 = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
					int num954 = 1000;
					bool flag95 = true;
					if (npc.position.Y > Main.player[npc.target].position.Y)
					{
						for (int num955 = 0; num955 < 255; num955++)
						{
							if (Main.player[num955].active)
							{
								Rectangle rectangle13 = new Rectangle((int)Main.player[num955].position.X - num954, (int)Main.player[num955].position.Y - num954, num954 * 2, num954 * 2);
								if (rectangle12.Intersects(rectangle13))
								{
									flag95 = false;
									break;
								}
							}
						}
						if (flag95)
						{
							flies = true;
						}
					}
				}
				else
				{
					npc.localAI[1] = 0f;
				}
				float num188 = speed;
				float num189 = turnSpeed;
				Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
				float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
				num191 = (float)((int)(num191 / 16f) * 16);
				num192 = (float)((int)(num192 / 16f) * 16);
				vector18.X = (float)((int)(vector18.X / 16f) * 16);
				vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
				num191 -= vector18.X;
				num192 -= vector18.Y;
				float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
				if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
				{
					try
					{
						vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						num191 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
						num192 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
					}
					catch
					{
					}
					npc.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
					num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
					int num194 = npc.width;
					num193 = (num193 - (float)num194) / num193;
					num191 *= num193;
					num192 *= num193;
					npc.velocity = Vector2.Zero;
					npc.position.X = npc.position.X + num191;
					npc.position.Y = npc.position.Y + num192;
				}
				else
				{
					if (!flies)
					{
						npc.TargetClosest(true);
						npc.velocity.Y = npc.velocity.Y + turnSpeed; //turnspeed * 0.5f
						if (npc.velocity.Y > num188) //speed
						{
							npc.velocity.Y = num188;
						}
						if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.4)
						{
							if (npc.velocity.X < 0f)
							{
								npc.velocity.X = npc.velocity.X - num189 * 1.1f;
							}
							else
							{
								npc.velocity.X = npc.velocity.X + num189 * 1.1f;
							}
						}
						else if (npc.velocity.Y == num188)
						{
							if (npc.velocity.X < num191)
							{
								npc.velocity.X = npc.velocity.X + num189;
							}
							else if (npc.velocity.X > num191)
							{
								npc.velocity.X = npc.velocity.X - num189;
							}
						}
						else if (npc.velocity.Y > 4f)
						{
							if (npc.velocity.X < 0f)
							{
								npc.velocity.X = npc.velocity.X + num189 * 0.9f;
							}
							else
							{
								npc.velocity.X = npc.velocity.X - num189 * 0.9f;
							}
						}
					}
					else
					{
						if (!flies && npc.behindTiles && npc.soundDelay == 0)
						{
							float num195 = num193 / 40f;
							if (num195 < 10f)
							{
								num195 = 10f;
							}
							if (num195 > 20f)
							{
								num195 = 20f;
							}
							npc.soundDelay = (int)num195;
							Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 1);
						}
						num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
						float num196 = System.Math.Abs(num191);
						float num197 = System.Math.Abs(num192);
						float num198 = num188 / num193;
						num191 *= num198;
						num192 *= num198;
						if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
						{
							if (npc.velocity.X < num191)
							{
								npc.velocity.X = npc.velocity.X + num189;
							}
							else
							{
								if (npc.velocity.X > num191)
								{
									npc.velocity.X = npc.velocity.X - num189;
								}
							}
							if (npc.velocity.Y < num192)
							{
								npc.velocity.Y = npc.velocity.Y + num189;
							}
							else
							{
								if (npc.velocity.Y > num192)
								{
									npc.velocity.Y = npc.velocity.Y - num189;
								}
							}
							if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
							{
								if (npc.velocity.Y > 0f)
								{
									npc.velocity.Y = npc.velocity.Y + num189 * 2f;
								}
								else
								{
									npc.velocity.Y = npc.velocity.Y - num189 * 2f;
								}
							}
							if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
							{
								if (npc.velocity.X > 0f)
								{
									npc.velocity.X = npc.velocity.X + num189 * 2f;
								}
								else
								{
									npc.velocity.X = npc.velocity.X - num189 * 2f;
								}
							}
						}
						else
						{
							if (num196 > num197)
							{
								if (npc.velocity.X < num191)
								{
									npc.velocity.X = npc.velocity.X + num189 * 1.1f;
								}
								else if (npc.velocity.X > num191)
								{
									npc.velocity.X = npc.velocity.X - num189 * 1.1f;
								}
								if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
								{
									if (npc.velocity.Y > 0f)
									{
										npc.velocity.Y = npc.velocity.Y + num189;
									}
									else
									{
										npc.velocity.Y = npc.velocity.Y - num189;
									}
								}
							}
							else
							{
								if (npc.velocity.Y < num192)
								{
									npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
								}
								else if (npc.velocity.Y > num192)
								{
									npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
								}
								if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
								{
									if (npc.velocity.X > 0f)
									{
										npc.velocity.X = npc.velocity.X + num189;
									}
									else
									{
										npc.velocity.X = npc.velocity.X - num189;
									}
								}
							}
						}
					}
				}
				npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
				if (flies)
				{
					if (npc.localAI[0] != 1f)
					{
						npc.netUpdate = true;
					}
					npc.localAI[0] = 1f;
				}
				else
				{
					if (npc.localAI[0] != 0f)
					{
						npc.netUpdate = true;
					}
					npc.localAI[0] = 0f;
				}
				if (phaseSwitch > 900)
				{
					npc.ai[2] = 0f;
					phaseSwitch = 0;
					npc.netUpdate = true;
					return;
				}
				if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
				{
					npc.netUpdate = true;
					return;
				}
			}
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.None;
		}

        // DoG phase 1 does not drop loot, but starts the sentinel phase of the fight.
        public override void NPCLoot()
        {
            // Skip the sentinel phase entirely if DoG has already been killed
            CalamityWorld.DoGSecondStageCountdown = CalamityWorld.downedDoG ? 600 : 21600;

            if (Main.netMode == 2)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                netMessage.Send();
            }

            // Turn off active Rage and Adrenaline mode from all players (why?)
            for (int playerIndex = 0; playerIndex < Main.player.Length; playerIndex++)
            {
                if (Main.player[playerIndex].active)
                {
                    Player player = Main.player[playerIndex];
                    for (int l = 0; l < 22; l++)
                    {
                        int hasBuff = player.buffType[l];
                        if (hasBuff == mod.BuffType("AdrenalineMode"))
                        {
                            player.DelBuff(l);
                            l = -1;
                        }
                        if (hasBuff == mod.BuffType("RageMode"))
                        {
                            player.DelBuff(l);
                            l = -1;
                        }
                    }
                }
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (projectile.type == mod.ProjectileType("SulphuricAcidMist2") || projectile.type == mod.ProjectileType("EidolicWail"))
			{
				damage /= 4;
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			if (damage > npc.lifeMax / 2)
			{
				string key = "Mods.CalamityMod.EdgyBossText2";
				Color messageColor = Color.Cyan;
				if (Main.netMode == 0)
				{
					Main.NewText(Language.GetTextValue(key), messageColor);
				}
				else if (Main.netMode == 2)
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				damage = 0;
				return false;
			}
			return true;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 1.5f;
			return null;
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DoGHead"), 1f);
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 50;
				npc.height = 50;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 15; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 30; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("GodSlayerInferno"), 300, true);
			player.AddBuff(mod.BuffType("WhisperingDeath"), 420, true);
			if (CalamityWorld.death)
			{
				player.KillMe(PlayerDeathReason.ByOther(10), 1000.0, 0, false);
			}
			int num = Main.rand.Next(5);
			string key = "Mods.CalamityMod.EdgyBossText3";
			if (num == 0)
			{
				key = "Mods.CalamityMod.EdgyBossText3";
			}
			else if (num == 1)
			{
				key = "Mods.CalamityMod.EdgyBossText4";
			}
			else if (num == 2)
			{
				key = "Mods.CalamityMod.EdgyBossText5";
			}
			else if (num == 3)
			{
				key = "Mods.CalamityMod.EdgyBossText6";
			}
			else if (num == 4)
			{
				key = "Mods.CalamityMod.EdgyBossText7";
			}
			Color messageColor = Color.Cyan;
			if (Main.netMode == 0)
			{
				Main.NewText(Language.GetTextValue(key), messageColor);
			}
			else if (Main.netMode == 2)
			{
				NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
			}
			player.AddBuff(BuffID.Frostburn, 300, true);
			player.AddBuff(BuffID.Darkness, 300, true);
		}
	}
}
