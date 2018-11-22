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
	public class Cnidrion : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cnidrion");
			Main.npcFrameCount[npc.type] = 10;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 3f;
			npc.aiStyle = -1;
			npc.damage = 20;
			npc.width = 160; //324
			npc.height = 80; //216
			npc.defense = 10;
			npc.lifeMax = 400;
			npc.knockBackResist = 0.05f;
			aiType = -1;
			npc.value = Item.buyPrice(0, 0, 50, 0);
			npc.HitSound = SoundID.NPCHit12;
			npc.DeathSound = SoundID.NPCDeath18;
			npc.rarity = 2;
		}
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe)
			{
				return 0f;
			}
			return SpawnCondition.DesertCave.Chance * 0.0175f;
		}
		
		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.1f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
		
		public override void AI()
		{
			Player player = Main.player[npc.target];
			bool expertMode = Main.expertMode;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
			float num823 = 1f;
			npc.TargetClosest(true);
			bool flag51 = false;
			if ((double)npc.life < (double)npc.lifeMax * 0.33) 
			{
				num823 = 2f;
			}
			if ((double)npc.life < (double)npc.lifeMax * 0.1) 
			{
				num823 = 4f;
			}
			if (npc.ai[0] == 0f)
			{
				npc.ai[1] += 1f;
				if ((double)npc.life < (double)npc.lifeMax * 0.33) 
				{
					npc.ai[1] += 1f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.1) 
				{
					npc.ai[1] += 1f;
				}
				if (npc.ai[1] >= 300f && Main.netMode != 1) 
				{
					npc.ai[1] = 0f;
					if ((double)npc.life < (double)npc.lifeMax * 0.1) 
					{
						npc.ai[0] = (float)Main.rand.Next(3, 5);
					} 
					else
					{
						npc.ai[0] = (float)Main.rand.Next(1, 3);
					}
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 1f)
			{
				flag51 = true;
				npc.ai[1] += 1f;
				if (npc.ai[1] % 15f == 0f) 
				{
					Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 20f);
					vector18.X += (float)(10 * npc.direction);
					float num829 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector18.X;
					float num830 = Main.player[npc.target].position.Y - vector18.Y;
					float num831 = (float)Math.Sqrt((double)(num829 * num829 + num830 * num830));
					float num832 = 6f;
					num831 = num832 / num831;
					num829 *= num831;
					num830 *= num831;
					num829 *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
					num830 *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
					int num833 = Projectile.NewProjectile(vector18.X, vector18.Y, num829, num830, mod.ProjectileType("HorsWaterBlast"), (expertMode ? 6 : 9), 0f, Main.myPlayer, 0f, 0f);
				}
				if (npc.ai[1] >= 120f) 
				{
					npc.ai[1] = 0f;
					npc.ai[0] = 0f;
				}
			}
			else if (npc.ai[0] == 2f)
			{
				flag51 = true;
				npc.ai[1] += 1f;
				if (npc.ai[1] > 60f && npc.ai[1] < 240f && npc.ai[1] % 8f == 0f) 
				{
					Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 20f);
					vector18.X += (float)(10 * npc.direction);
					float num829 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector18.X;
					float num830 = Main.player[npc.target].position.Y - vector18.Y;
					float num831 = (float)Math.Sqrt((double)(num829 * num829 + num830 * num830));
					float num832 = 8f;
					num831 = num832 / num831;
					num829 *= num831;
					num830 *= num831;
					num829 *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
					num830 *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
					int num833 = Projectile.NewProjectile(vector18.X, vector18.Y, num829, num830, mod.ProjectileType("HorsWaterBlast"), (expertMode ? 7 : 10), 0f, Main.myPlayer, 0f, 0f);
				}
				if (npc.ai[1] >= 300f) 
				{
					npc.ai[1] = 0f;
					npc.ai[0] = 0f;
				}
			} 
			else if (npc.ai[0] == 3f)
			{
				num823 = 4f;
				npc.ai[1] += 1f;
				if (npc.ai[1] % 30f == 0f) 
				{
					Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 20f);
					vector18.X += (float)(10 * npc.direction);
					float num844 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector18.X;
					float num845 = Main.player[npc.target].position.Y - vector18.Y;
					float num846 = (float)Math.Sqrt((double)(num844 * num844 + num845 * num845));
					float num847 = 10f;
					num846 = num847 / num846;
					num844 *= num846;
					num845 *= num846;
					num844 *= 1f + (float)Main.rand.Next(-20, 21) * 0.001f;
					num845 *= 1f + (float)Main.rand.Next(-20, 21) * 0.001f;
					int num848 = Projectile.NewProjectile(vector18.X, vector18.Y, num844, num845, mod.ProjectileType("HorsWaterBlast"), (expertMode ? 9 : 12), 0f, Main.myPlayer, 0f, 0f);
				}
				if (npc.ai[1] >= 120f) 
				{
					npc.ai[1] = 0f;
					npc.ai[0] = 0f;
				}
			} 
			else if (npc.ai[0] == 4f)
			{
				num823 = 4f;
				npc.ai[1] += 1f;
				if (npc.ai[1] % 10f == 0f) 
				{
					Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 20f);
					vector18.X += (float)(10 * npc.direction);
					float num829 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector18.X;
					float num830 = Main.player[npc.target].position.Y - vector18.Y;
					float num831 = (float)Math.Sqrt((double)(num829 * num829 + num830 * num830));
					float num832 = 11f;
					num831 = num832 / num831;
					num829 *= num831;
					num830 *= num831;
					num829 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
					num830 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
					int num833 = Projectile.NewProjectile(vector18.X, vector18.Y, num829, num830, mod.ProjectileType("HorsWaterBlast"), (expertMode ? 11 : 15), 0f, Main.myPlayer, 0f, 0f);
				}
				if (npc.ai[1] >= 240f) 
				{
					npc.ai[1] = 0f;
					npc.ai[0] = 0f;
				}
			}
			if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 50f) 
			{
				flag51 = true;
			}
			if (flag51) 
			{
				npc.velocity.X = npc.velocity.X * 0.9f;
				if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1) 
				{
					npc.velocity.X = 0f;
				}
			} 
			else
			{
				if (npc.direction > 0) 
				{
					npc.velocity.X = (npc.velocity.X * 20f + num823) / 21f;
				}
				if (npc.direction < 0) 
				{
					npc.velocity.X = (npc.velocity.X * 20f - num823) / 21f;
				}
			}
			int num854 = 80;
			int num855 = 20;
			Vector2 position2 = new Vector2(npc.Center.X - (float)(num854 / 2), npc.position.Y + (float)npc.height - (float)num855);
			bool flag52 = false;
			if (npc.position.X < Main.player[npc.target].position.X && npc.position.X + (float)npc.width > Main.player[npc.target].position.X + (float)Main.player[npc.target].width && npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y + (float)Main.player[npc.target].height - 16f) 
			{
				flag52 = true;
			}
			if (flag52) 
			{
				npc.velocity.Y = npc.velocity.Y + 0.5f;
			} 
			else if (Collision.SolidCollision(position2, num854, num855))
			{
				if (npc.velocity.Y > 0f) 
				{
					npc.velocity.Y = 0f;
				}
				if ((double)npc.velocity.Y > -0.2) 
				{
					npc.velocity.Y = npc.velocity.Y - 0.025f;
				} 
				else
				{
					npc.velocity.Y = npc.velocity.Y - 0.2f;
				}
				if (npc.velocity.Y < -4f) 
				{
					npc.velocity.Y = -4f;
				}
			} 
			else
			{
				if (npc.velocity.Y < 0f) 
				{
					npc.velocity.Y = 0f;
				}
				if ((double)npc.velocity.Y < 0.1) 
				{
					npc.velocity.Y = npc.velocity.Y + 0.025f;
				} 
				else
				{
					npc.velocity.Y = npc.velocity.Y + 0.5f;
				}
			}
			if (npc.velocity.Y > 10f) 
			{
				npc.velocity.Y = 10f;
				return;
			}
			float num116 = (float)Main.rand.Next(30, 900);
			num116 *= (float)npc.life / (float)npc.lifeMax;
			num116 += 30f;
			if (Main.netMode != 1 && npc.ai[2] >= num116 && npc.velocity.Y == 0f && !player.dead && ((npc.direction > 0 && npc.Center.X < player.Center.X) || (npc.direction < 0 && npc.Center.X > player.Center.X)) && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
			{
				float num117 = 13f;
				Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 20f);
				vector18.X += (float)(10 * npc.direction);
				float num118 = player.position.X + (float)player.width * 0.5f - vector18.X;
				float num119 = player.position.Y + (float)player.height * 0.5f - vector18.Y;
				num118 += (float)Main.rand.Next(-40, 41);
				num119 += (float)Main.rand.Next(-40, 41);
				float num120 = (float)Math.Sqrt((double)(num118 * num118 + num119 * num119));
				npc.netUpdate = true;
				num120 = num117 / num120;
				num118 *= num120;
				num119 *= num120;
				int num121 = expertMode ? 8 : 12;
				int num122 = mod.ProjectileType("HorsWaterBlast");
				vector18.X += num118 * 3f;
				vector18.Y += num119 * 3f;
				Projectile.NewProjectile(vector18.X, vector18.Y, num118, num119, num122, num121, 0f, Main.myPlayer, 0f, 0f);
				npc.ai[2] = 0f;
			}
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 40; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 2f);
				}
			}
		}
		
		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Coral, Main.rand.Next(1, 4));
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Starfish, Main.rand.Next(1, 4));
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Seashell, Main.rand.Next(1, 4));
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("VictoryShard"), Main.rand.Next(1, 4));
			if (Main.rand.Next(4) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AmidiasSpark"));
			}
		}
	}
}