using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.CalPlayer;

namespace CalamityMod.NPCs.CalamityBiomeNPCs
{
    public class DespairStone : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Despair Stone");
		}

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 40;
			npc.width = 72; //324
			npc.height = 72; //216
			npc.defense = 38;
			npc.lifeMax = 120;
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 0, 5, 0);
			npc.HitSound = SoundID.NPCHit41;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.behindTiles = true;
			npc.lavaImmune = true;
			if (CalamityWorld.downedProvidence)
			{
				npc.damage = 190;
				npc.defense = 185;
				npc.lifeMax = 5000;
				npc.value = Item.buyPrice(0, 0, 50, 0);
			}
			banner = npc.type;
			bannerItem = mod.ItemType("DespairStoneBanner");
		}

		public override void AI()
		{
			int num = 30;
			int num2 = 10;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
			{
				flag2 = true;
				npc.ai[3] += 1f;
			}
			num2 = 4;
			bool flag4 = npc.velocity.Y == 0f;
			for (int i = 0; i < 200; i++)
			{
				if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type && Math.Abs(npc.position.X - Main.npc[i].position.X) + Math.Abs(npc.position.Y - Main.npc[i].position.Y) < (float)npc.width)
				{
					if (npc.position.X < Main.npc[i].position.X)
					{
						npc.velocity.X = npc.velocity.X - 0.05f;
					}
					else
					{
						npc.velocity.X = npc.velocity.X + 0.05f;
					}
					if (npc.position.Y < Main.npc[i].position.Y)
					{
						npc.velocity.Y = npc.velocity.Y - 0.05f;
					}
					else
					{
						npc.velocity.Y = npc.velocity.Y + 0.05f;
					}
				}
			}
			if (flag4)
			{
				npc.velocity.Y = 0f;
			}
			if (npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num || flag2)
			{
				npc.ai[3] += 1f;
				flag3 = true;
			}
			else if (npc.ai[3] > 0f)
			{
				npc.ai[3] -= 1f;
			}
			if (npc.ai[3] > (float)(num * num2))
			{
				npc.ai[3] = 0f;
			}
			if (npc.justHit)
			{
				npc.ai[3] = 0f;
			}
			if (npc.ai[3] == (float)num)
			{
				npc.netUpdate = true;
			}
			Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num3 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X;
			float num4 = Main.player[npc.target].position.Y - vector.Y;
			float num5 = (float)Math.Sqrt((double)(num3 * num3 + num4 * num4));
			if (num5 < 200f && !flag3)
			{
				npc.ai[3] = 0f;
			}
			if (npc.velocity.Y == 0f && Math.Abs(npc.velocity.X) > 3f && ((npc.Center.X < Main.player[npc.target].Center.X && npc.velocity.X > 0f) || (npc.Center.X > Main.player[npc.target].Center.X && npc.velocity.X < 0f)))
			{
				npc.velocity.Y = npc.velocity.Y - 4f;
				Main.PlaySound(2, npc.Center, 14);
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 235, 0f, -1f, 0, default, 1f);
				}
			}
			if (npc.ai[3] < (float)num)
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
							npc.spriteDirection = npc.direction;
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
			float num7 = 6f;
			float num8 = 0.07f;
			if (!flag && (npc.velocity.Y == 0f || npc.wet || (npc.velocity.X <= 0f && npc.direction < 0) || (npc.velocity.X >= 0f && npc.direction > 0)))
			{
				if (Math.Sign(npc.velocity.X) != npc.direction)
				{
					npc.velocity.X = npc.velocity.X * 0.92f;
				}
				float num9 = MathHelper.Lerp(0.6f, 1f, Math.Abs(Main.windSpeedSet)) * (float)Math.Sign(Main.windSpeedSet);
				if (!Main.player[npc.target].ZoneSandstorm)
				{
					num9 = 0f;
				}
				num7 = 5f + num9 * (float)npc.direction * 4f;
				num8 = 0.2f;
				if (npc.velocity.X < -num7 || npc.velocity.X > num7)
				{
					if (npc.velocity.Y == 0f)
					{
						npc.velocity *= 0.8f;
					}
				}
				else if (npc.velocity.X < num7 && npc.direction == 1)
				{
					npc.velocity.X = npc.velocity.X + num8;
					if (npc.velocity.X > num7)
					{
						npc.velocity.X = num7;
					}
				}
				else if (npc.velocity.X > -num7 && npc.direction == -1)
				{
					npc.velocity.X = npc.velocity.X - num8;
					if (npc.velocity.X < -num7)
					{
						npc.velocity.X = -num7;
					}
				}
			}
			if (npc.velocity.Y >= 0f)
			{
				int num10 = 0;
				if (npc.velocity.X < 0f)
				{
					num10 = -1;
				}
				if (npc.velocity.X > 0f)
				{
					num10 = 1;
				}
				Vector2 position = npc.position;
				position.X += npc.velocity.X;
				int num11 = (int)((position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num10)) / 16f);
				int num12 = (int)((position.Y + (float)npc.height - 1f) / 16f);
				if (Main.tile[num11, num12] == null)
				{
					Main.tile[num11, num12] = new Tile();
				}
				if (Main.tile[num11, num12 - 1] == null)
				{
					Main.tile[num11, num12 - 1] = new Tile();
				}
				if (Main.tile[num11, num12 - 2] == null)
				{
					Main.tile[num11, num12 - 2] = new Tile();
				}
				if (Main.tile[num11, num12 - 3] == null)
				{
					Main.tile[num11, num12 - 3] = new Tile();
				}
				if (Main.tile[num11, num12 + 1] == null)
				{
					Main.tile[num11, num12 + 1] = new Tile();
				}
				if ((float)(num11 * 16) < position.X + (float)npc.width && (float)(num11 * 16 + 16) > position.X && ((Main.tile[num11, num12].nactive() && !Main.tile[num11, num12].topSlope() && !Main.tile[num11, num12 - 1].topSlope() && Main.tileSolid[(int)Main.tile[num11, num12].type] && !Main.tileSolidTop[(int)Main.tile[num11, num12].type]) || (Main.tile[num11, num12 - 1].halfBrick() && Main.tile[num11, num12 - 1].nactive())) && (!Main.tile[num11, num12 - 1].nactive() || !Main.tileSolid[(int)Main.tile[num11, num12 - 1].type] || Main.tileSolidTop[(int)Main.tile[num11, num12 - 1].type] || (Main.tile[num11, num12 - 1].halfBrick() && (!Main.tile[num11, num12 - 4].nactive() || !Main.tileSolid[(int)Main.tile[num11, num12 - 4].type] || Main.tileSolidTop[(int)Main.tile[num11, num12 - 4].type]))) && (!Main.tile[num11, num12 - 2].nactive() || !Main.tileSolid[(int)Main.tile[num11, num12 - 2].type] || Main.tileSolidTop[(int)Main.tile[num11, num12 - 2].type]) && (!Main.tile[num11, num12 - 3].nactive() || !Main.tileSolid[(int)Main.tile[num11, num12 - 3].type] || Main.tileSolidTop[(int)Main.tile[num11, num12 - 3].type]) && (!Main.tile[num11 - num10, num12 - 3].nactive() || !Main.tileSolid[(int)Main.tile[num11 - num10, num12 - 3].type]))
				{
					float num13 = (float)(num12 * 16);
					if (Main.tile[num11, num12].halfBrick())
					{
						num13 += 8f;
					}
					if (Main.tile[num11, num12 - 1].halfBrick())
					{
						num13 -= 8f;
					}
					if (num13 < position.Y + (float)npc.height)
					{
						float num14 = position.Y + (float)npc.height - num13;
						if ((double)num14 <= 16.1)
						{
							npc.gfxOffY += npc.position.Y + (float)npc.height - num13;
							npc.position.Y = num13 - (float)npc.height;
							if (num14 < 9f)
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
			if (npc.velocity.Y == 0f)
			{
				int num15 = (int)((npc.position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 2) * npc.direction) + npc.velocity.X * 5f) / 16f);
				int num16 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);
				if (Main.tile[num15, num16] == null)
				{
					Main.tile[num15, num16] = new Tile();
				}
				if (Main.tile[num15, num16 - 1] == null)
				{
					Main.tile[num15, num16 - 1] = new Tile();
				}
				if (Main.tile[num15, num16 - 2] == null)
				{
					Main.tile[num15, num16 - 2] = new Tile();
				}
				if (Main.tile[num15, num16 - 3] == null)
				{
					Main.tile[num15, num16 - 3] = new Tile();
				}
				if (Main.tile[num15, num16 + 1] == null)
				{
					Main.tile[num15, num16 + 1] = new Tile();
				}
				if (Main.tile[num15 + npc.direction, num16 - 1] == null)
				{
					Main.tile[num15 + npc.direction, num16 - 1] = new Tile();
				}
				if (Main.tile[num15 + npc.direction, num16 + 1] == null)
				{
					Main.tile[num15 + npc.direction, num16 + 1] = new Tile();
				}
				if (Main.tile[num15 - npc.direction, num16 + 1] == null)
				{
					Main.tile[num15 - npc.direction, num16 + 1] = new Tile();
				}
				int num17 = npc.spriteDirection;
				num17 *= -1;
				if ((npc.velocity.X < 0f && num17 == -1) || (npc.velocity.X > 0f && num17 == 1))
				{
					bool flag6 = npc.type == 410 || npc.type == 423;
					float num18 = 3f;
					if (Main.tile[num15, num16 - 2].nactive() && Main.tileSolid[(int)Main.tile[num15, num16 - 2].type])
					{
						if (Main.tile[num15, num16 - 3].nactive() && Main.tileSolid[(int)Main.tile[num15, num16 - 3].type])
						{
							npc.velocity.Y = -8.5f;
							npc.netUpdate = true;
						}
						else
						{
							npc.velocity.Y = -7.5f;
							npc.netUpdate = true;
						}
					}
					else if (Main.tile[num15, num16 - 1].nactive() && !Main.tile[num15, num16 - 1].topSlope() && Main.tileSolid[(int)Main.tile[num15, num16 - 1].type])
					{
						npc.velocity.Y = -7f;
						npc.netUpdate = true;
					}
					else if (npc.position.Y + (float)npc.height - (float)(num16 * 16) > 20f && Main.tile[num15, num16].nactive() && !Main.tile[num15, num16].topSlope() && Main.tileSolid[(int)Main.tile[num15, num16].type])
					{
						npc.velocity.Y = -6f;
						npc.netUpdate = true;
					}
					else if ((npc.directionY < 0 || Math.Abs(npc.velocity.X) > num18) && (!flag6 || !Main.tile[num15, num16 + 1].nactive() || !Main.tileSolid[(int)Main.tile[num15, num16 + 1].type]) && (!Main.tile[num15, num16 + 2].nactive() || !Main.tileSolid[(int)Main.tile[num15, num16 + 2].type]) && (!Main.tile[num15 + npc.direction, num16 + 3].nactive() || !Main.tileSolid[(int)Main.tile[num15 + npc.direction, num16 + 3].type]))
					{
						npc.velocity.Y = -8f;
						npc.netUpdate = true;
					}
				}
			}
			npc.rotation += npc.velocity.X * 0.05f;
			npc.spriteDirection = -npc.direction;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 180, true);
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
			return spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneCalamity ? 0.25f : 0f;
        }

		public override void NPCLoot()
		{
			if (CalamityWorld.downedProvidence && Main.rand.NextBool(2))
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Bloodstone"));
			}
			if (Main.hardMode && Main.rand.NextBool(3))
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofChaos"));
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 40; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default, 1f);
				}
			}
		}
	}
}
