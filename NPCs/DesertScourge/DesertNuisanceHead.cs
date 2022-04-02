using CalamityMod.Events;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
	public class DesertNuisanceHead : ModNPC
	{
		public bool flies = false;
		public float speed = 0.085f;
		public float turnSpeed = 0.125f;
		public int maxLength = 13;
		bool TailSpawned = false;

		public override void SetStaticDefaults() => DisplayName.SetDefault("A Desert Nuisance");

		public override void SetDefaults()
		{
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.defense = 2;
			npc.width = 60;
			npc.height = 60;
			npc.lifeMax = BossRushEvent.BossRushActive ? 35000 : 800;
			npc.aiStyle = -1;
			aiType = -1;
			npc.knockBackResist = 0f;
			npc.alpha = 255;
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.netAlways = true;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = true;
			npc.Calamity().VulnerableToWater = true;
		}

		public override void AI()
		{
			if (npc.ai[2] > 0f)
				npc.realLife = (int)npc.ai[2];

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			npc.alpha -= 42;
			if (npc.alpha < 0)
				npc.alpha = 0;

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!TailSpawned)
				{
					npc.ai[2] = (float)npc.whoAmI;
					npc.realLife = npc.whoAmI;
					int num2 = npc.whoAmI;
					int num3 = maxLength;
					for (int j = 0; j <= num3; j++)
					{
						int num4 = ModContent.NPCType<DesertNuisanceBody>();
						if (j == num3)
						{
							num4 = ModContent.NPCType<DesertNuisanceTail>();
						}
						int num5 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), num4, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
						Main.npc[num5].ai[2] = (float)npc.whoAmI;
						Main.npc[num5].realLife = npc.whoAmI;
						Main.npc[num5].ai[1] = (float)num2;
						Main.npc[num2].ai[0] = (float)num5;
						NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num5, 0f, 0f, 0f, 0, 0, 0);
						num2 = num5;
					}
					TailSpawned = true;
				}
			}
			int num12 = (int)(npc.position.X / 16f) - 1;
			int num13 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
			int num14 = (int)(npc.position.Y / 16f) - 1;
			int num15 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
			if (num12 < 0)
			{
				num12 = 0;
			}
			if (num13 > Main.maxTilesX)
			{
				num13 = Main.maxTilesX;
			}
			if (num14 < 0)
			{
				num14 = 0;
			}
			if (num15 > Main.maxTilesY)
			{
				num15 = Main.maxTilesY;
			}
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
			if (!flag2)
			{
				npc.localAI[1] = 1f;
				Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
				int num16 = 1000;
				bool flag3 = true;
				if (npc.position.Y > Main.player[npc.target].position.Y)
				{
					for (int m = 0; m < 255; m++)
					{
						if (Main.player[m].active)
						{
							Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X - num16, (int)Main.player[m].position.Y - num16, num16 * 2, num16 * 2);
							if (rectangle.Intersects(rectangle2))
							{
								flag3 = false;
								break;
							}
						}
					}
					if (flag3)
					{
						flag2 = true;
					}
				}
			}
			else
			{
				npc.localAI[1] = 0f;
			}
			float num17 = 16f;
			if (Main.player[npc.target].dead)
			{
				flag2 = false;
				npc.velocity.Y = npc.velocity.Y + 1f;
				if ((double)npc.position.Y > Main.worldSurface * 16.0)
				{
					npc.velocity.Y = npc.velocity.Y + 1f;
					num17 = 32f;
				}
				if ((double)npc.position.Y > Main.rockLayer * 16.0)
				{
					for (int a = 0; a < 200; a++)
					{
						if (Main.npc[a].type == ModContent.NPCType<DesertNuisanceHead>() || Main.npc[a].type == ModContent.NPCType<DesertNuisanceBody>() ||
							Main.npc[a].type == ModContent.NPCType<DesertNuisanceTail>())
						{
							Main.npc[a].active = false;
						}
					}
				}
			}
			float num18 = speed;
			float num19 = turnSpeed;
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
			if (!flag2)
			{
				npc.TargetClosest(true);
				npc.velocity.Y = npc.velocity.Y + 0.15f;
				if (npc.velocity.Y > num17)
				{
					npc.velocity.Y = num17;
				}
				if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num17 * 0.4)
				{
					if (npc.velocity.X < 0f)
					{
						npc.velocity.X = npc.velocity.X - num18 * 1.1f;
					}
					else
					{
						npc.velocity.X = npc.velocity.X + num18 * 1.1f;
					}
				}
				else if (npc.velocity.Y == num17)
				{
					if (npc.velocity.X < num20)
					{
						npc.velocity.X = npc.velocity.X + num18;
					}
					else if (npc.velocity.X > num20)
					{
						npc.velocity.X = npc.velocity.X - num18;
					}
				}
				else if (npc.velocity.Y > 4f)
				{
					if (npc.velocity.X < 0f)
					{
						npc.velocity.X = npc.velocity.X + num18 * 0.9f;
					}
					else
					{
						npc.velocity.X = npc.velocity.X - num18 * 0.9f;
					}
				}
			}
			else
			{
				if (npc.soundDelay == 0)
				{
					float num24 = num22 / 40f;
					if (num24 < 10f)
					{
						num24 = 10f;
					}
					if (num24 > 20f)
					{
						num24 = 20f;
					}
					npc.soundDelay = (int)num24;
					Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);
				}
				num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
				float num25 = Math.Abs(num20);
				float num26 = Math.Abs(num21);
				float num27 = num17 / num22;
				num20 *= num27;
				num21 *= num27;
				if (((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f)) && ((npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f)))
				{
					if (npc.velocity.X < num20)
					{
						npc.velocity.X = npc.velocity.X + num19;
					}
					else if (npc.velocity.X > num20)
					{
						npc.velocity.X = npc.velocity.X - num19;
					}
					if (npc.velocity.Y < num21)
					{
						npc.velocity.Y = npc.velocity.Y + num19;
					}
					else if (npc.velocity.Y > num21)
					{
						npc.velocity.Y = npc.velocity.Y - num19;
					}
				}
				if ((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f) || (npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f))
				{
					if (npc.velocity.X < num20)
					{
						npc.velocity.X = npc.velocity.X + num18;
					}
					else if (npc.velocity.X > num20)
					{
						npc.velocity.X = npc.velocity.X - num18;
					}
					if (npc.velocity.Y < num21)
					{
						npc.velocity.Y = npc.velocity.Y + num18;
					}
					else if (npc.velocity.Y > num21)
					{
						npc.velocity.Y = npc.velocity.Y - num18;
					}
					if ((double)Math.Abs(num21) < (double)num17 * 0.2 && ((npc.velocity.X > 0f && num20 < 0f) || (npc.velocity.X < 0f && num20 > 0f)))
					{
						if (npc.velocity.Y > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num18 * 2f;
						}
						else
						{
							npc.velocity.Y = npc.velocity.Y - num18 * 2f;
						}
					}
					if ((double)Math.Abs(num20) < (double)num17 * 0.2 && ((npc.velocity.Y > 0f && num21 < 0f) || (npc.velocity.Y < 0f && num21 > 0f)))
					{
						if (npc.velocity.X > 0f)
						{
							npc.velocity.X = npc.velocity.X + num18 * 2f;
						}
						else
						{
							npc.velocity.X = npc.velocity.X - num18 * 2f;
						}
					}
				}
				else if (num25 > num26)
				{
					if (npc.velocity.X < num20)
					{
						npc.velocity.X = npc.velocity.X + num18 * 1.1f;
					}
					else if (npc.velocity.X > num20)
					{
						npc.velocity.X = npc.velocity.X - num18 * 1.1f;
					}
					if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num17 * 0.5)
					{
						if (npc.velocity.Y > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num18;
						}
						else
						{
							npc.velocity.Y = npc.velocity.Y - num18;
						}
					}
				}
				else
				{
					if (npc.velocity.Y < num21)
					{
						npc.velocity.Y = npc.velocity.Y + num18 * 1.1f;
					}
					else if (npc.velocity.Y > num21)
					{
						npc.velocity.Y = npc.velocity.Y - num18 * 1.1f;
					}
					if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num17 * 0.5)
					{
						if (npc.velocity.X > 0f)
						{
							npc.velocity.X = npc.velocity.X + num18;
						}
						else
						{
							npc.velocity.X = npc.velocity.X - num18;
						}
					}
				}
			}
			npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
			if (flag2)
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
			if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
			{
				npc.netUpdate = true;
			}
		}

		public override bool PreNPCLoot() => false;

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScourgeHead"), 0.65f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScourgeHead2"), 0.65f);
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
				}
			}
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.7f * bossLifeScale);
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.Bleeding, 180, true);
		}
	}
}
