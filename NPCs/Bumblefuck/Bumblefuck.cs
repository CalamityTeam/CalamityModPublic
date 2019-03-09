using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Bumblefuck
{
	[AutoloadBossHead]
	public class Bumblefuck : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bumblebirb");
			Main.npcFrameCount[npc.type] = 10;
		}

		public override void SetDefaults()
		{
			npc.npcSlots = 32f;
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 160;
			npc.width = 80; //324
			npc.height = 80; //216
			npc.defense = 40;
			npc.lifeMax = CalamityWorld.revenge ? 252500 : 227500;
			if (CalamityWorld.death)
			{
				npc.lifeMax = 302500;
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 1000000 : 900000;
			}
			npc.knockBackResist = 0f;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
				npc.buffImmune[BuffID.Ichor] = false;
				npc.buffImmune[BuffID.CursedInferno] = false;
				npc.buffImmune[mod.BuffType("ExoFreeze")] = false;
				npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
				npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
				npc.buffImmune[mod.BuffType("DemonFlames")] = false;
				npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
				npc.buffImmune[mod.BuffType("Nightwither")] = false;
				npc.buffImmune[mod.BuffType("Shred")] = false;
				npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
				npc.buffImmune[mod.BuffType("SilvaStun")] = false;
			}
			npc.boss = true;
			Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
			if (calamityModMusic != null)
				music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Murderswarm");
			else
				music = MusicID.Boss4;
			npc.lavaImmune = true;
			npc.noGravity = true;
			npc.value = Item.buyPrice(0, 30, 0, 0);
			npc.HitSound = SoundID.NPCHit51;
			npc.DeathSound = SoundID.NPCDeath46;
			bossBag = mod.ItemType("BumblebirbBag");
		}

		public override void AI()
		{
			Player player = Main.player[npc.target];
			bool revenge = CalamityWorld.revenge;
			Vector2 vector = npc.Center;
			int num1305 = revenge ? 6 : 4;
			if (CalamityWorld.death)
			{
				num1305 = 8;
			}
			npc.noTileCollide = false;
			npc.noGravity = true;
			npc.damage = npc.defDamage;
			if (Vector2.Distance(player.Center, vector) > 5600f)
			{
				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
				}
			}
			Vector2 vector205 = player.Center - npc.Center;
			if (npc.ai[0] > 1f && vector205.Length() > 5200f)
			{
				npc.ai[0] = 1f;
			}
			if (npc.ai[0] == 0f)
			{
				npc.TargetClosest(true);
				if (npc.Center.X < player.Center.X - 2f)
				{
					npc.direction = 1;
				}
				if (npc.Center.X > player.Center.X + 2f)
				{
					npc.direction = -1;
				}
				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.05f) / 10f;
				if (npc.collideX)
				{
					npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);
					if (npc.velocity.X > 26f) //4
					{
						npc.velocity.X = 26f; //4
					}
					if (npc.velocity.X < -26f) //4
					{
						npc.velocity.X = -26f; //4
					}
				}
				if (npc.collideY)
				{
					npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);
					if (npc.velocity.Y > 26f) //4
					{
						npc.velocity.Y = 26f; //4
					}
					if (npc.velocity.Y < -26f) //4
					{
						npc.velocity.Y = -26f; //4
					}
				}
				Vector2 value51 = player.Center - npc.Center;
				value51.Y -= 200f;
				if (value51.Length() > 800f) //800 Charge to get closer and spawn birbs
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
				}
				else if (value51.Length() > 80f)
				{
					float scaleFactor15 = 12f; //6
					float num1306 = 30f;
					value51.Normalize();
					value51 *= scaleFactor15;
					npc.velocity = (npc.velocity * (num1306 - 1f) + value51) / num1306;
				}
				else if (npc.velocity.Length() > 18f) //8
				{
					npc.velocity *= 0.95f;
				}
				else if (npc.velocity.Length() < 9f) //4
				{
					npc.velocity *= 1.05f;
				}
				npc.ai[1] += 1f;
				if (npc.justHit)
				{
					npc.ai[1] += (float)Main.rand.Next(10, 30);
				}
				if (npc.ai[1] >= 180f && Main.netMode != 1)
				{
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
					while (npc.ai[0] == 0f)
					{
						int damage = Main.expertMode ? 45 : 60; //180 120
						int num1307 = Main.rand.Next(3);
						if (num1307 == 0 && Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
						{
							npc.ai[0] = 2f;
						}
						else if (num1307 == 1)
						{
							npc.ai[0] = 3f;
						}
						else if (NPC.CountNPCS(mod.NPCType("Bumblefuck2")) < num1305)
						{
							npc.ai[0] = 4f; //summon more birbs
							Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 102);
							for (int num186 = 0; num186 < 6; num186++)
							{
								Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)Main.rand.Next(-10, 10), (float)Main.rand.Next(-5, -2), mod.ProjectileType("RedLightningFeather"), damage, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
					return;
				}
			}
			else
			{
				if (npc.ai[0] == 1f) //move closer and spawn birbs
				{
					npc.collideX = false;
					npc.collideY = false;
					npc.noTileCollide = true;
					if (npc.target < 0 || !player.active || player.dead)
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
					npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.04f) / 10f;
					Vector2 value52 = player.Center - npc.Center;
					if (value52.Length() < 500f && !Collision.SolidCollision(npc.position, npc.width, npc.height)) //500
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
					float scaleFactor16 = 13f + value52.Length() / 100f; //7
					float num1308 = 25f;
					value52.Normalize();
					value52 *= scaleFactor16;
					npc.velocity = (npc.velocity * (num1308 - 1f) + value52) / num1308;
					return;
				}
				if (npc.ai[0] == 2f)
				{
					npc.damage = (int)((double)npc.defDamage * 0.9);
					if (npc.target < 0 || !player.active || player.dead)
					{
						npc.TargetClosest(true);
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
					if (player.Center.X - 10f < npc.Center.X)
					{
						npc.direction = -1;
					}
					else if (player.Center.X + 10f > npc.Center.X)
					{
						npc.direction = 1;
					}
					npc.spriteDirection = npc.direction;
					npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.05f) / 5f;
					if (npc.collideX)
					{
						npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);
						if (npc.velocity.X > 27f) //4
						{
							npc.velocity.X = 27f; //4
						}
						if (npc.velocity.X < -27f) //4
						{
							npc.velocity.X = -27f; //4
						}
					}
					if (npc.collideY)
					{
						npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);
						if (npc.velocity.Y > 27f) //4
						{
							npc.velocity.Y = 27f; //4
						}
						if (npc.velocity.Y < -27f) //4
						{
							npc.velocity.Y = -27f; //4
						}
					}
					Vector2 value53 = player.Center - npc.Center;
					value53.Y -= 20f;
					npc.ai[2] += 0.0222222228f;
					if (Main.expertMode)
					{
						npc.ai[2] += 0.0166666675f;
					}
					float scaleFactor17 = 12f + npc.ai[2] + value53.Length() / 120f; //4
					float num1309 = 20f;
					value53.Normalize();
					value53 *= scaleFactor17;
					npc.velocity = (npc.velocity * (num1309 - 1f) + value53) / num1309;
					npc.ai[1] += 1f;
					if (npc.ai[1] > 180f || !Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						return;
					}
				}
				else
				{
					if (npc.ai[0] == 3f)
					{
						npc.noTileCollide = true;
						if (npc.velocity.X < 0f)
						{
							npc.direction = -1;
						}
						else
						{
							npc.direction = 1;
						}
						npc.spriteDirection = npc.direction;
						npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.035f) / 5f;
						Vector2 value54 = player.Center - npc.Center;
						value54.Y -= 12f;
						if (npc.Center.X > player.Center.X)
						{
							value54.X += 400f;
						}
						else
						{
							value54.X -= 400f;
						}
						if (Math.Abs(npc.Center.X - player.Center.X) > 350f && Math.Abs(npc.Center.Y - player.Center.Y) < 20f)
						{
							npc.ai[0] = 3.1f;
							npc.ai[1] = 0f;
						}
						npc.ai[1] += 0.0333333351f;
						float scaleFactor18 = 18f + npc.ai[1]; //8
						float num1310 = 4f;
						value54.Normalize();
						value54 *= scaleFactor18;
						npc.velocity = (npc.velocity * (num1310 - 1f) + value54) / num1310;
						return;
					}
					if (npc.ai[0] == 3.1f)
					{
						npc.noTileCollide = true;
						npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.035f) / 5f;
						Vector2 vector206 = player.Center - npc.Center;
						vector206.Y -= 12f;
						float scaleFactor19 = 26f; //16
						float num1311 = 8f;
						vector206.Normalize();
						vector206 *= scaleFactor19;
						npc.velocity = (npc.velocity * (num1311 - 1f) + vector206) / num1311;
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
							npc.velocity = vector206;
							if (npc.velocity.X < 0f)
							{
								npc.direction = -1;
							}
							else
							{
								npc.direction = 1;
							}
							npc.ai[0] = 3.2f;
							npc.ai[1] = 0f;
							npc.ai[1] = (float)npc.direction;
							return;
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
							npc.velocity.X = (18f + npc.ai[2]) * npc.ai[1]; //16
							if ((npc.ai[1] > 0f && npc.Center.X > player.Center.X + 400f) || (npc.ai[1] < 0f && npc.Center.X < player.Center.X - 400f))
							{
								if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
								{
									npc.ai[0] = 0f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;
								}
								else if (Math.Abs(npc.Center.X - player.Center.X) > 800f) //800
								{
									npc.ai[0] = 1f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;
								}
							}
							npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.035f) / 5f;
							return;
						}
						if (npc.ai[0] == 4f)
						{
							npc.ai[0] = 0f;
							npc.TargetClosest(true);
							if (Main.netMode != 1)
							{
								npc.ai[1] = -1f;
								npc.ai[2] = -1f;
								for (int num1312 = 0; num1312 < 1000; num1312++)
								{
									int num1313 = (int)player.Center.X / 16;
									int num1314 = (int)player.Center.Y / 16;
									int num1315 = 30 + num1312 / 50;
									int num1316 = 20 + num1312 / 75;
									num1313 += Main.rand.Next(-num1315, num1315 + 1);
									num1314 += Main.rand.Next(-num1316, num1316 + 1);
									if (!WorldGen.SolidTile(num1313, num1314))
									{
										while (!WorldGen.SolidTile(num1313, num1314) && (double)num1314 < Main.worldSurface)
										{
											num1314++;
										}
										if ((new Vector2((float)(num1313 * 16 + 8), (float)(num1314 * 16 + 8)) - player.Center).Length() < 5600f) //600
										{
											npc.ai[0] = 4.1f;
											npc.ai[1] = (float)num1313;
											npc.ai[2] = (float)num1314;
											break;
										}
									}
								}
							}
							npc.netUpdate = true;
							return;
						}
						if (npc.ai[0] == 4.1f)
						{
							if (npc.velocity.X < -2f)
							{
								npc.direction = -1;
							}
							else if (npc.velocity.X > 2f)
							{
								npc.direction = 1;
							}
							npc.spriteDirection = npc.direction;
							npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.05f) / 10f;
							npc.noTileCollide = true;
							int num1317 = (int)npc.ai[1];
							int num1318 = (int)npc.ai[2];
							float x2 = (float)(num1317 * 16 + 8);
							float y2 = (float)(num1318 * 16 - 20);
							Vector2 vector207 = new Vector2(x2, y2);
							vector207 -= npc.Center;
							float num1319 = 6f + vector207.Length() / 150f;
							if (num1319 > 10f)
							{
								num1319 = 10f;
							}
							float num1320 = 10f; //10
							if (vector207.Length() < 100f) //10
							{
								npc.ai[0] = 4.2f;
							}
							vector207.Normalize();
							vector207 *= num1319;
							npc.velocity = (npc.velocity * (num1320 - 1f) + vector207) / num1320;
							return;
						}
						if (npc.ai[0] == 4.2f)
						{
							npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.05f) / 10f;
							npc.noTileCollide = true;
							int num1321 = (int)npc.ai[1];
							int num1322 = (int)npc.ai[2];
							float x3 = (float)(num1321 * 16 + 8);
							float y3 = (float)(num1322 * 16 - 20);
							Vector2 vector208 = new Vector2(x3, y3);
							vector208 -= npc.Center;
							float num1323 = 40f; //4
							float num1324 = 2f; //2
							if (Main.netMode != 1 && vector208.Length() < 40f) //4
							{
								int num1325 = 10;
								if (Main.expertMode)
								{
									num1325 = (int)((double)num1325 * 0.75);
								}
								npc.ai[3] += 1f;
								if (npc.ai[3] == (float)num1325)
								{
									NPC.NewNPC(num1321 * 16 + 8, num1322 * 16, mod.NPCType("Bumblefuck2"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
								}
								else if (npc.ai[3] == (float)(num1325 * 2))
								{
									npc.ai[0] = 0f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;
									if (NPC.CountNPCS(mod.NPCType("Bumblefuck2")) < num1305 && Main.rand.Next(5) != 0)
									{
										npc.ai[0] = 4f;
									}
									else if (Collision.SolidCollision(npc.position, npc.width, npc.height))
									{
										npc.ai[0] = 1f;
									}
								}
							}
							if (vector208.Length() > num1323)
							{
								vector208.Normalize();
								vector208 *= num1323;
							}
							npc.velocity = (npc.velocity * (num1324 - 1f) + vector208) / num1324;
							return;
						}
					}
				}
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += (double)(npc.velocity.Length() / 4f);
			npc.frameCounter += 1.0;
			if (npc.ai[0] < 4f)
			{
				if (npc.frameCounter >= 6.0)
				{
					npc.frameCounter = 0.0;
					npc.frame.Y = npc.frame.Y + frameHeight;
				}
				if (npc.frame.Y / frameHeight > 4)
				{
					npc.frame.Y = 0;
				}
			}
			else
			{
				if (npc.frameCounter >= 6.0)
				{
					npc.frameCounter = 0.0;
					npc.frame.Y = npc.frame.Y + frameHeight;
				}
				if (npc.frame.Y / frameHeight > 9)
				{
					npc.frame.Y = frameHeight * 5;
				}
			}
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			name = "A Bumblebirb";
			potionType = ItemID.SuperHealingPotion;
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 50; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default(Color), 1f);
				}
				float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BumbleHead"), 1f);
				for (int wing = 0; wing < 2; wing++)
				{
					randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
					Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BumbleWing"), 1f);
				}
				for (int leg = 0; leg < 4; leg++)
				{
					randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
					Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BumbleLeg"), 1f);
				}
			}
		}

		public override void NPCLoot()
		{
			if (Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BumblebirbTrophy"));
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
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EffulgentFeather"), Main.rand.Next(6, 12));
				switch (Main.rand.Next(3))
				{
					case 0:
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RougeSlash"));
						break;
					case 1:
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GildedProboscis"));
						break;
					case 2:
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GoldenEagle"));
						break;
				}
			}
		}
	}
}