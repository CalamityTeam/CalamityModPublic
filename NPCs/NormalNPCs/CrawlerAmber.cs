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
	public class CrawlerAmber : ModNPC
	{
		private bool detected = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Amber Crawler");
			Main.npcFrameCount[npc.type] = 5;
		}

		public override void SetDefaults()
		{
			npc.npcSlots = 0.3f;
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 25;
			npc.width = 44; //324
			npc.height = 34; //216
			npc.defense = 18;
			npc.lifeMax = 90;
			npc.knockBackResist = 0.3f;
			npc.value = Item.buyPrice(0, 0, 0, 80);
			npc.HitSound = SoundID.NPCHit33;
			npc.DeathSound = SoundID.NPCDeath36;
			banner = npc.type;
			bannerItem = mod.ItemType("AmberCrawlerBanner");
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(detected);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			detected = reader.ReadBoolean();
		}

		public override void FindFrame(int frameHeight)
		{
			if (!detected)
			{
				npc.frame.Y = frameHeight * 4;
				npc.frameCounter = 0.0;
				return;
			}
			npc.spriteDirection = -npc.direction;
			npc.frameCounter += (double)(npc.velocity.Length() / 8f);
			if (npc.frameCounter > 2.0)
			{
				npc.frame.Y = npc.frame.Y + frameHeight;
				npc.frameCounter = 0.0;
			}
			if (npc.frame.Y >= frameHeight * 3)
			{
				npc.frame.Y = 0;
			}
		}

		public override void AI()
		{
			if (!detected)
				npc.TargetClosest(true);
			if (((Main.player[npc.target].Center - npc.Center).Length() < 100f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position,
					Main.player[npc.target].width, Main.player[npc.target].height)) || npc.justHit)
				detected = true;
			if (!detected)
				return;
			int num19 = 30;
			int num20 = 10;
			bool flag19 = false;
			bool flag20 = false;
			bool flag30 = false;
			if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction > 0) || (npc.velocity.X < 0f && npc.direction < 0)))
			{
				flag20 = true;
				npc.ai[3] += 1f;
			}
			if ((npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num19) | flag20)
			{
				npc.ai[3] += 1f;
				flag30 = true;
			}
			else if (npc.ai[3] > 0f)
			{
				npc.ai[3] -= 1f;
			}
			if (npc.ai[3] > (float)(num19 * num20))
			{
				npc.ai[3] = 0f;
			}
			if (npc.justHit)
			{
				npc.ai[3] = 0f;
			}
			if (npc.ai[3] == (float)num19)
			{
				npc.netUpdate = true;
			}
			Vector2 vector19 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float arg_31B_0 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector19.X;
			float num30 = Main.player[npc.target].position.Y - vector19.Y;
			float num40 = (float)Math.Sqrt((double)(arg_31B_0 * arg_31B_0 + num30 * num30));
			if (num40 < 200f && !flag30)
			{
				npc.ai[3] = 0f;
			}
			if (npc.ai[3] < (float)num19)
			{
				npc.TargetClosest(true);
			}
			else
			{
				if (npc.velocity.X == 0f)
				{
					if (npc.velocity.Y == 0f)
					{
						npc.ai[0] += 1f;
						if (npc.ai[0] >= 2f)
						{
							npc.direction *= -1;
							npc.spriteDirection = -npc.direction;
							npc.ai[0] = 0f;
						}
					}
				}
				else
				{
					npc.ai[0] = 0f;
				}
				npc.directionY = -1;
				if (npc.direction == 0)
				{
					npc.direction = 1;
				}
			}
			float num6 = 5f; //5
			float num70 = 0.05f; //0.05
			if (!flag19 && (npc.velocity.Y == 0f || npc.wet || (npc.velocity.X <= 0f && npc.direction > 0) || (npc.velocity.X >= 0f && npc.direction < 0)))
			{
				if (npc.velocity.X < -num6 || npc.velocity.X > num6)
				{
					if (npc.velocity.Y == 0f)
					{
						npc.velocity *= 0.8f;
					}
				}
				else if (npc.velocity.X < num6 && npc.direction == -1)
				{
					npc.velocity.X = npc.velocity.X + num70;
					if (npc.velocity.X > num6)
					{
						npc.velocity.X = num6;
					}
				}
				else if (npc.velocity.X > -num6 && npc.direction == 1)
				{
					npc.velocity.X = npc.velocity.X - num70;
					if (npc.velocity.X < -num6)
					{
						npc.velocity.X = -num6;
					}
				}
			}
			if (npc.velocity.Y >= 0f)
			{
				int num9 = 0;
				if (npc.velocity.X < 0f)
				{
					num9 = -1;
				}
				if (npc.velocity.X > 0f)
				{
					num9 = 1;
				}
				Vector2 position = npc.position;
				position.X += npc.velocity.X;
				int num10 = (int)((position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num9)) / 16f);
				int num11 = (int)((position.Y + (float)npc.height - 1f) / 16f);
				if (Main.tile[num10, num11] == null)
				{
					Main.tile[num10, num11] = new Tile();
				}
				if (Main.tile[num10, num11 - 1] == null)
				{
					Main.tile[num10, num11 - 1] = new Tile();
				}
				if (Main.tile[num10, num11 - 2] == null)
				{
					Main.tile[num10, num11 - 2] = new Tile();
				}
				if (Main.tile[num10, num11 - 3] == null)
				{
					Main.tile[num10, num11 - 3] = new Tile();
				}
				if (Main.tile[num10, num11 + 1] == null)
				{
					Main.tile[num10, num11 + 1] = new Tile();
				}
				if ((float)(num10 * 16) < position.X + (float)npc.width && (float)(num10 * 16 + 16) > position.X && ((Main.tile[num10, num11].nactive() && !Main.tile[num10, num11].topSlope() && !Main.tile[num10, num11 - 1].topSlope() && Main.tileSolid[(int)Main.tile[num10, num11].type] && !Main.tileSolidTop[(int)Main.tile[num10, num11].type]) || (Main.tile[num10, num11 - 1].halfBrick() && Main.tile[num10, num11 - 1].nactive())) && (!Main.tile[num10, num11 - 1].nactive() || !Main.tileSolid[(int)Main.tile[num10, num11 - 1].type] || Main.tileSolidTop[(int)Main.tile[num10, num11 - 1].type] || (Main.tile[num10, num11 - 1].halfBrick() && (!Main.tile[num10, num11 - 4].nactive() || !Main.tileSolid[(int)Main.tile[num10, num11 - 4].type] || Main.tileSolidTop[(int)Main.tile[num10, num11 - 4].type]))) && (!Main.tile[num10, num11 - 2].nactive() || !Main.tileSolid[(int)Main.tile[num10, num11 - 2].type] || Main.tileSolidTop[(int)Main.tile[num10, num11 - 2].type]) && (!Main.tile[num10, num11 - 3].nactive() || !Main.tileSolid[(int)Main.tile[num10, num11 - 3].type] || Main.tileSolidTop[(int)Main.tile[num10, num11 - 3].type]) && (!Main.tile[num10 - num9, num11 - 3].nactive() || !Main.tileSolid[(int)Main.tile[num10 - num9, num11 - 3].type]))
				{
					float num12 = (float)(num11 * 16);
					if (Main.tile[num10, num11].halfBrick())
					{
						num12 += 8f;
					}
					if (Main.tile[num10, num11 - 1].halfBrick())
					{
						num12 -= 8f;
					}
					if (num12 < position.Y + (float)npc.height)
					{
						float num13 = position.Y + (float)npc.height - num12;
						if ((double)num13 <= 16.1)
						{
							npc.gfxOffY += npc.position.Y + (float)npc.height - num12;
							npc.position.Y = num12 - (float)npc.height;
							if (num13 < 9f)
							{
								npc.stepSpeed = 1f;
							}
							else
							{
								npc.stepSpeed = 2f;
							}
						}
					}
				}
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSunkenSea)
			{
				return 0f;
			}
			return SpawnCondition.DesertCave.Chance * 0.0175f;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 32, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 32, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}

		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Amber, Main.rand.Next(2, 5));
		}
	}
}
