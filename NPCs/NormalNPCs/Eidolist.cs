using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
using CalamityMod.World;

namespace CalamityMod.NPCs.NormalNPCs
{
	public class Eidolist : ModNPC
	{
		public bool hasBeenHit = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eidolist");
			Main.npcFrameCount[npc.type] = 6;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 0;
			npc.width = 60; //324
			npc.height = 80; //216
			npc.defense = 0;
			npc.lifeMax = 10000;
			if (CalamityWorld.death)
			{
				npc.lifeMax = 13000;
			}
			npc.knockBackResist = 0f;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.value = Item.buyPrice(0, 1, 0, 0);
			npc.alpha = 50;
			npc.noGravity = true;
			npc.noTileCollide = false;
			npc.HitSound = SoundID.NPCHit13;
			npc.DeathSound = SoundID.NPCDeath59;
			npc.timeLeft = NPC.activeTime * 2;
			banner = npc.type;
			bannerItem = mod.ItemType("EidolistBanner");
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(hasBeenHit);
			writer.Write(npc.chaseable);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			hasBeenHit = reader.ReadBoolean();
			npc.chaseable = reader.ReadBoolean();
		}

		public override void AI()
		{
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0f, 0.4f, 0.5f);
			if (npc.justHit || CalamityGlobalNPC.AnyBossNPCS())
			{
				hasBeenHit = true;
			}
			npc.chaseable = hasBeenHit;
			if (!hasBeenHit)
			{
				if (npc.collideX)
				{
					npc.velocity.X = npc.velocity.X * -1f;
					npc.direction *= -1;
					npc.netUpdate = true;
				}
				if (npc.collideY)
				{
					npc.netUpdate = true;
					if (npc.velocity.Y > 0f)
					{
						npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
						npc.directionY = -1;
						npc.localAI[2] = -1f;
					}
					else if (npc.velocity.Y < 0f)
					{
						npc.velocity.Y = Math.Abs(npc.velocity.Y);
						npc.directionY = 1;
						npc.localAI[2] = 1f;
					}
				}
				npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
				if (npc.velocity.X < -2f || npc.velocity.X > 2f)
				{
					npc.velocity.X = npc.velocity.X * 0.95f;
				}
				if (npc.localAI[2] == -1f)
				{
					npc.velocity.Y = npc.velocity.Y - 0.01f;
					if ((double)npc.velocity.Y < -0.3)
					{
						npc.localAI[2] = 1f;
					}
				}
				else
				{
					npc.velocity.Y = npc.velocity.Y + 0.01f;
					if ((double)npc.velocity.Y > 0.3)
					{
						npc.localAI[2] = -1f;
					}
				}
				if ((double)npc.velocity.Y > 0.4 || (double)npc.velocity.Y < -0.4)
				{
					npc.velocity.Y = npc.velocity.Y * 0.95f;
				}
				return;
			}
			npc.noTileCollide = true;
			float num1446 = 7f;
			int num1447 = 480;
			float num244;
			if (npc.localAI[1] == 1f)
			{
				npc.localAI[1] = 0f;
				if (Main.rand.Next(4) == 0)
				{
					npc.ai[0] = (float)num1447;
				}
			}
			npc.TargetClosest(true);
			npc.rotation = Math.Abs(npc.velocity.X) * (float)npc.direction * 0.1f;
			npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
			Vector2 value53 = npc.Center + new Vector2((float)(npc.direction * 20), 6f);
			Vector2 vector251 = Main.player[npc.target].Center - value53;
			bool flag104 = Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1);
			npc.localAI[0] += 1f;
			if (Main.netMode != 1 && npc.localAI[0] >= 300f)
			{
				npc.localAI[0] = 0f;
				if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					float speed = 5f;
					Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
					float num6 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X + (float)Main.rand.Next(-10, 11);
					float num7 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-10, 11);
					float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
					num8 = speed / num8;
					num6 *= num8;
					num7 *= num8;
					int damage = 40;
					if (Main.expertMode)
					{
						damage = 30;
					}
					int random = Main.rand.Next(2);
					if (random == 0)
					{
						Projectile.NewProjectile(npc.Center.X, npc.Center.Y, num6, num7, 465, damage, 0f, Main.myPlayer, 0f, 0f);
					}
					else
					{
						Vector2 vec = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);
						vec = Vector2.Normalize(Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 20f);
						if (vec.HasNaNs())
						{
							vec = new Vector2((float)npc.direction, 0f);
						}
						for (int n = 0; n < 1; n++)
						{
							Vector2 vector4 = vec * 4f;
							Projectile.NewProjectile(npc.Center.X, npc.Center.Y, vector4.X, vector4.Y, 464, damage, 0f, Main.myPlayer, 0f, 1f);
						}
					}
				}
			}
			if (vector251.Length() > 400f || !flag104)
			{
				Vector2 value54 = vector251;
				if (value54.Length() > num1446)
				{
					value54.Normalize();
					value54 *= num1446;
				}
				int num1448 = 30;
				npc.velocity = (npc.velocity * (float)(num1448 - 1) + value54) / (float)num1448;
			}
			else
			{
				npc.velocity *= 0.98f;
			}
			if (npc.ai[2] != 0f && npc.ai[3] != 0f)
			{
				Main.PlaySound(SoundID.Item8, npc.Center);
				int num;
				for (int num1449 = 0; num1449 < 20; num1449 = num + 1)
				{
					int num1450 = Dust.NewDust(npc.position, npc.width, npc.height, 20, 0f, 0f, 100, Color.Transparent, 1f);
					Dust dust = Main.dust[num1450];
					dust.velocity *= 3f;
					Main.dust[num1450].noGravity = true;
					Main.dust[num1450].scale = 2.5f;
					num = num1449;
				}
				npc.Center = new Vector2(npc.ai[2] * 16f, npc.ai[3] * 16f);
				npc.velocity = Vector2.Zero;
				npc.ai[2] = 0f;
				npc.ai[3] = 0f;
				Main.PlaySound(SoundID.Item8, npc.Center);
				for (int num1451 = 0; num1451 < 20; num1451 = num + 1)
				{
					int num1452 = Dust.NewDust(npc.position, npc.width, npc.height, 20, 0f, 0f, 100, Color.Transparent, 1f);
					Dust dust = Main.dust[num1452];
					dust.velocity *= 3f;
					Main.dust[num1452].noGravity = true;
					Main.dust[num1452].scale = 2.5f;
					num = num1451;
				}
			}
			float[] var_9_48E3C_cp_0 = npc.ai;
			int var_9_48E3C_cp_1 = 0;
			num244 = var_9_48E3C_cp_0[var_9_48E3C_cp_1];
			var_9_48E3C_cp_0[var_9_48E3C_cp_1] = num244 + 1f;
			if (npc.ai[0] >= (float)num1447 && Main.netMode != 1)
			{
				npc.ai[0] = 0f;
				Point point12 = npc.Center.ToTileCoordinates();
				Point point13 = Main.player[npc.target].Center.ToTileCoordinates();
				int num1453 = 20;
				int num1454 = 3;
				int num1455 = 10;
				int num1456 = 1;
				int num1457 = 0;
				bool flag106 = false;
				if (vector251.Length() > 2000f)
				{
					flag106 = true;
				}
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
							npc.ai[2] = (float)num1458;
							npc.ai[3] = (float)num1459;
							break;
						}
					}
				}
				npc.netUpdate = true;
			}
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			if (projectile.minion)
			{
				return hasBeenHit;
			}
			return null;
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += (hasBeenHit ? 0.15f : 0.075f);
			npc.frameCounter %= Main.npcFrameCount[npc.type];
			int frame = (int)npc.frameCounter;
			npc.frame.Y = frame * frameHeight;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (!Main.hardMode)
			{
				return 0f;
			}
			if (spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyssLayer3 && spawnInfo.water)
			{
				return 0.05f;
			}
			return spawnInfo.player.ZoneDungeon ? 0.04f : 0f;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}

		public override void NPCLoot()
		{
			if (Main.rand.Next(4) == 0)
			{
				if (!NPC.LunarApocalypseIsUp)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EidolonTablet"));
				}
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.BlueLunaticHood);
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.BlueLunaticRobe);
			}
			if (NPC.downedPlantBoss || CalamityWorld.downedCalamitas)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Ectoplasm, Main.rand.Next(3, 6));
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Lumenite"), Main.rand.Next(8, 11));
				if (Main.expertMode && Main.rand.Next(2) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Lumenite"), Main.rand.Next(2, 5));
				}
			}
		}
	}
}