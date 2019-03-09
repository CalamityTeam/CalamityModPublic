using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.NormalNPCs
{
	public class ThiccWaifu : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cloud Elemental");
			Main.npcFrameCount[npc.type] = 8;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 3f;
			npc.damage = 38;
			npc.width = 50; //324
			npc.height = 100; //216
			npc.defense = 40;
			npc.lifeMax = 6000;
			npc.knockBackResist = 0.05f;
			npc.value = Item.buyPrice(0, 1, 50, 0);
			npc.HitSound = SoundID.NPCHit23;
			npc.DeathSound = SoundID.NPCDeath39;
			npc.buffImmune[20] = true;
			npc.buffImmune[44] = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.rarity = 2;
			banner = npc.type;
			bannerItem = mod.ItemType("CloudElementalBanner");
		}
		
		public override void AI()
		{
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.375f, 0.5f, 0.625f);
			bool flag111 = false;
			bool flag112 = false;
			bool flag113 = true;
			bool flag114 = false;
			int num1454 = 4;
			int num1455 = 3;
			int num1456 = 0;
			float num1457 = 0.2f;
			float num1458 = 2f;
			float num1459 = -0.2f;
			float num1460 = -4f;
			bool flag115 = true;
			float num1461 = 2f;
			float num1462 = 0.1f;
			float num1463 = 1f;
			float num1464 = 0.04f;
			bool flag116 = false;
			float scaleFactor26 = 0.96f;
			bool flag117 = true;
			if (npc.type == mod.NPCType("ThiccWaifu"))
			{
				flag115 = false;
				npc.rotation = npc.velocity.X * 0.04f;
				npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
				num1456 = 3;
				num1459 = -0.1f;
				num1457 = 0.1f;
				float num1465 = (float)npc.life / (float)npc.lifeMax;
				num1461 += (1f - num1465) * 2f;
				num1462 += (1f - num1465) * 0.02f;
				if (num1465 < 0.5f) 
				{
					npc.knockBackResist = 0f;
				}
				npc.localAI[2] = 0f;
				if (npc.ai[0] < 0f) 
				{
					npc.ai[0] = MathHelper.Min(npc.ai[0] + 1f, 0f);
				}
				if (npc.ai[0] > 0f) 
				{
					flag117 = false;
					flag116 = true;
					npc.ai[0] += 1f;
					if (npc.ai[0] >= 135f) 
					{
						npc.ai[0] = -300f;
						npc.netUpdate = true;
					}
					Vector2 vector = npc.Center;
					vector = Vector2.UnitX * (float)npc.direction * 200f;
					Vector2 vector223 = npc.Center + Vector2.UnitX * (float)npc.direction * 50f - Vector2.UnitY * 6f;
					if (npc.ai[0] == 54f && Main.netMode != 1) 
					{
						List<Point> list4 = new List<Point>();
						Vector2 vec5 = Main.player[npc.target].Center + new Vector2(Main.player[npc.target].velocity.X * 30f, 0f);
						Point point14 = vec5.ToTileCoordinates();
						int num1468 = 0;
						while (num1468 < 1000 && list4.Count < 3) 
						{
							bool flag118 = false;
							int num1469 = Main.rand.Next(point14.X - 30, point14.X + 30 + 1);
							foreach (Point current in list4) 
							{
								if (Math.Abs(current.X - num1469) < 10) 
								{
									flag118 = true;
									break;
								}
							}
							if (!flag118) 
							{
								int startY = point14.Y - 20;
								int num1470;
								int num1471;
								Collision.ExpandVertically(num1469, startY, out num1470, out num1471, 1, 51);
								if (StrayMethods.CanSpawnSandstormHostile(new Vector2((float)num1469, (float)(num1471 - 15)) * 16f, 15, 15)) 
								{
									list4.Add(new Point(num1469, num1471 - 15));
								}
							}
							num1468++;
						}
						foreach (Point current2 in list4) 
						{
							Projectile.NewProjectile((float)(current2.X * 16), (float)(current2.Y * 16), 0f, 0f, mod.ProjectileType("StormMarkHostile"), 0, 0f, Main.myPlayer, 0f, 0f);
						}
					}
					new Vector2(0.9f, 2f);
					if (npc.ai[0] < 114f && npc.ai[0] > 0f) 
					{
						List<Vector2> list5 = new List<Vector2>();
						for (int num1472 = 0; num1472 < 1000; num1472++) 
						{
							Projectile projectile9 = Main.projectile[num1472];
							if (projectile9.active && projectile9.type == mod.ProjectileType("StormMarkHostile")) 
							{
								list5.Add(projectile9.Center);
							}
						}
					}
				}
				if (npc.ai[0] == 0f) 
				{
					npc.ai[0] = 1f;
					npc.netUpdate = true;
					flag116 = true;
				}
			}
			if (npc.justHit) 
			{
				npc.localAI[2] = 0f;
			}
			if (!flag112) 
			{
				if (npc.localAI[2] >= 0f) 
				{
					float num1477 = 16f;
					bool flag119 = false;
					bool flag120 = false;
					if (npc.position.X > npc.localAI[0] - num1477 && npc.position.X < npc.localAI[0] + num1477) 
					{
						flag119 = true;
					} 
					else if ((npc.velocity.X < 0f && npc.direction > 0) || (npc.velocity.X > 0f && npc.direction < 0))
					{
						flag119 = true;
						num1477 += 24f;
					}
					if (npc.position.Y > npc.localAI[1] - num1477 && npc.position.Y < npc.localAI[1] + num1477) 
					{
						flag120 = true;
					}
					if (flag119 && flag120) 
					{
						npc.localAI[2] += 1f;
						if (npc.localAI[2] >= 30f && num1477 == 16f) 
						{
							flag111 = true;
						}
						if (npc.localAI[2] >= 60f) 
						{
							npc.localAI[2] = -180f;
							npc.direction *= -1;
							npc.velocity.X = npc.velocity.X * -1f;
							npc.collideX = false;
						}
					} 
					else 
					{
						npc.localAI[0] = npc.position.X;
						npc.localAI[1] = npc.position.Y;
						npc.localAI[2] = 0f;
					}
					if (flag117) 
					{
						npc.TargetClosest(true);
					}
				} 
				else 
				{
					npc.localAI[2] += 1f;
					npc.direction = ((Main.player[npc.target].Center.X > npc.Center.X) ? 1 : -1);
				}
			}
			int num1478 = (int)((npc.position.X + (float)(npc.width / 2)) / 16f) + npc.direction * 2;
			int num1479 = (int)((npc.position.Y + (float)npc.height) / 16f);
			int num1480 = (int)npc.Bottom.Y / 16;
			int num1481 = (int)npc.Bottom.X / 16;
			if (flag116) 
			{
				npc.velocity *= scaleFactor26;
				return;
			}
			for (int num1482 = num1479; num1482 < num1479 + num1454; num1482++) 
			{
				if (Main.tile[num1478, num1482] == null) 
				{
					Main.tile[num1478, num1482] = new Tile();
				}
				if ((Main.tile[num1478, num1482].nactive() && Main.tileSolid[(int)Main.tile[num1478, num1482].type]) || Main.tile[num1478, num1482].liquid > 0) 
				{
					if (num1482 <= num1479 + 1) 
					{
						flag114 = true;
					}
					flag113 = false;
					break;
				}
			}
			for (int num1483 = num1480; num1483 < num1480 + num1456; num1483++) 
			{
				if (Main.tile[num1481, num1483] == null) 
				{
					Main.tile[num1481, num1483] = new Tile();
				}
				if ((Main.tile[num1481, num1483].nactive() && Main.tileSolid[(int)Main.tile[num1481, num1483].type]) || Main.tile[num1481, num1483].liquid > 0) 
				{
					flag114 = true;
					flag113 = false;
					break;
				}
			}
			if (flag115) 
			{
				for (int num1484 = num1479 - num1455; num1484 < num1479; num1484++) 
				{
					if (Main.tile[num1478, num1484] == null) 
					{
						Main.tile[num1478, num1484] = new Tile();
					}
					if ((Main.tile[num1478, num1484].nactive() && Main.tileSolid[(int)Main.tile[num1478, num1484].type]) || Main.tile[num1478, num1484].liquid > 0) 
					{
						flag114 = false;
						flag111 = true;
						break;
					}
				}
			}
			if (flag111) 
			{
				flag114 = false;
				flag113 = true;
			}
			if (flag113) 
			{
				npc.velocity.Y = npc.velocity.Y + num1457;
				if (npc.velocity.Y > num1458) 
				{
					npc.velocity.Y = num1458;
				}
			} 
			else 
			{
				if ((npc.directionY < 0 && npc.velocity.Y > 0f) || flag114) 
				{
					npc.velocity.Y = npc.velocity.Y + num1459;
				}
				if (npc.velocity.Y < num1460) 
				{
					npc.velocity.Y = num1460;
				}
			}
			if (npc.collideX) 
			{
				npc.velocity.X = npc.oldVelocity.X * -0.4f;
				if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 1f) 
				{
					npc.velocity.X = 1f;
				}
				if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -1f) 
				{
					npc.velocity.X = -1f;
				}
			}
			if (npc.collideY) 
			{
				npc.velocity.Y = npc.oldVelocity.Y * -0.25f;
				if (npc.velocity.Y > 0f && npc.velocity.Y < 1f) 
				{
					npc.velocity.Y = 1f;
				}
				if (npc.velocity.Y < 0f && npc.velocity.Y > -1f) 
				{
					npc.velocity.Y = -1f;
				}
			}
			if (npc.direction == -1 && npc.velocity.X > -num1461) 
			{
				npc.velocity.X = npc.velocity.X - num1462;
				if (npc.velocity.X > num1461) 
				{
					npc.velocity.X = npc.velocity.X - num1462;
				} 
				else if (npc.velocity.X > 0f)
				{
					npc.velocity.X = npc.velocity.X + num1462 / 2f;
				}
				if (npc.velocity.X < -num1461) 
				{
					npc.velocity.X = -num1461;
				}
			} 
			else if (npc.direction == 1 && npc.velocity.X < num1461) 
			{
				npc.velocity.X = npc.velocity.X + num1462;
				if (npc.velocity.X < -num1461) 
				{
					npc.velocity.X = npc.velocity.X + num1462;
				}
				else if (npc.velocity.X < 0f) 
				{
					npc.velocity.X = npc.velocity.X - num1462 / 2f;
				}
				if (npc.velocity.X > num1461) 
				{
					npc.velocity.X = num1461;
				}
			}
			if (npc.directionY == -1 && npc.velocity.Y > -num1463) 
			{
				npc.velocity.Y = npc.velocity.Y - num1464;
				if (npc.velocity.Y > num1463) 
				{
					npc.velocity.Y = npc.velocity.Y - num1464 * 1.25f;
				} 
				else if (npc.velocity.Y > 0f) 
				{
					npc.velocity.Y = npc.velocity.Y + num1464 * 0.75f;
				}
				if (npc.velocity.Y < -num1463) 
				{
					npc.velocity.Y = -num1461;
					return;
				}
			} 
			else if (npc.directionY == 1 && npc.velocity.Y < num1463) 
			{
				npc.velocity.Y = npc.velocity.Y + num1464;
				if (npc.velocity.Y < -num1463) 
				{
					npc.velocity.Y = npc.velocity.Y + num1464 * 1.25f;
				} 
				else if (npc.velocity.Y < 0f)
				{
					npc.velocity.Y = npc.velocity.Y - num1464 * 0.75f;
				}
				if (npc.velocity.Y > num1463) 
				{
					npc.velocity.Y = num1463;
					return;
				}
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            Texture2D texture = mod.GetTexture("NPCs/NormalNPCs/ThiccWaifuAttack");
            if (npc.ai[0] > 0f)
            {
                CalamityMod.DrawTexture(spriteBatch, texture, 0, npc, drawColor);
            }
            else
            {
                CalamityMod.DrawTexture(spriteBatch, Main.npcTexture[npc.type], 0, npc, drawColor);
            }
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter = npc.frameCounter + (double)(npc.velocity.Length() * 0.1f) + 1.0;
            if (npc.frameCounter >= (npc.ai[0] > 0f ? 16.0 : 8.0))
            {
                npc.frame.Y = npc.frame.Y + frameHeight;
                npc.frameCounter = 0.0;
            }
            if (npc.frame.Y >= frameHeight * 8)
            {
                npc.frame.Y = 0;
            }
        }
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !Main.hardMode || !Main.raining)
			{
				return 0f;
			}
			return SpawnCondition.Sky.Chance * 0.1f;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 16, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 50; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 16, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofCinder"), Main.rand.Next(2, 4));
			if (Main.expertMode)
			{
				if (Main.rand.Next(3) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EyeoftheStorm"));
				}
			}
			else if (Main.rand.Next(4) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EyeoftheStorm"));
			}
			if (Main.rand.Next(5) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("StormSaber"));
			}
		}
	}
}