using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.Astrageldon
{
    public class AstrageldonSlime : ModNPC
	{
		private bool boostDR = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aureus Spawn");
			Main.npcFrameCount[npc.type] = 4;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 0;
			npc.width = 90; //324
			npc.height = 60; //216
			npc.defense = 0;
			npc.alpha = 255;
			npc.lifeMax = Main.expertMode ? 1007 : 1012;
			if (CalamityWorld.death)
			{
				npc.lifeMax = 1002;
			}
			npc.knockBackResist = 0f;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(boostDR);
			writer.Write(npc.dontTakeDamage);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			boostDR = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
		}

		public override void AI()
		{
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.6f, 0.25f, 0f);
			npc.rotation = Math.Abs(npc.velocity.X) * (float)npc.direction * 0.04f;
			npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
			if (npc.alpha > 0)
			{
				npc.alpha -= 5;
				int num;
				for (int num245 = 0; num245 < 10; num245 = num + 1)
				{
					int dust = Dust.NewDust(npc.position, npc.width, npc.height, mod.DustType("AstralOrange"), npc.velocity.X, npc.velocity.Y, 255, default, 0.8f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 0.5f;
					num = num245;
				}
			}
			npc.TargetClosest(true);
			if (npc.life <= 1000 || Main.dayTime)
			{
				npc.dontTakeDamage = true;
				Vector2 vector = Main.player[npc.target].Center - npc.Center;
				Point point15 = npc.Center.ToTileCoordinates();
				Tile tileSafely = Framing.GetTileSafely(point15);
				bool flag121 = (tileSafely.nactive() && Main.tileSolid[tileSafely.type] && !Main.tileSolidTop[tileSafely.type] && !TileID.Sets.Platforms[tileSafely.type]);
				if (vector.Length() < 60f || flag121)
				{
					npc.dontTakeDamage = false;
					CheckDead();
					npc.life = 0;
					return;
				}
				float num1372 = 18f;
				Vector2 vector167 = new Vector2(npc.Center.X + (float)(npc.direction * 20), npc.Center.Y + 6f);
				float num1373 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector167.X;
				float num1374 = Main.player[npc.target].Center.Y - vector167.Y;
				float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
				float num1376 = num1372 / num1375;
				num1373 *= num1376;
				num1374 *= num1376;
				npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
				npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
				return;
			}
			float num1446 = 7f;
			int num1447 = 480;
			float num244;
			if (npc.localAI[1] == 1f)
			{
				npc.localAI[1] = 0f;
				if (Main.rand.NextBool(4))
				{
					npc.ai[0] = (float)num1447;
				}
			}
			Vector2 value53 = npc.Center + new Vector2((float)(npc.direction * 20), 6f);
			Vector2 vector251 = Main.player[npc.target].Center - value53;
			bool flag104 = Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1);
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
					int num1450 = Dust.NewDust(npc.position, npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, Color.Transparent, 0.6f);
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
					int num1452 = Dust.NewDust(npc.position, npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, Color.Transparent, 0.6f);
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
			if (npc.ai[0] >= (float)num1447 && Main.netMode != NetmodeID.MultiplayerClient)
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

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			damage = (crit ? 2 : 1);
			return false;
		}

		public override Color? GetAlpha(Color drawColor)
		{
			return new Color(200, 200, 200, npc.alpha);
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += 0.15f;
			npc.frameCounter %= Main.npcFrameCount[npc.type];
			int frame = (int)npc.frameCounter;
			npc.frame.Y = frame * frameHeight;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default, 1f);
				}
			}
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("AstralInfectionDebuff"), 120, true);
		}

		public override bool CheckDead()
		{
			Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 14);
			npc.position.X = npc.position.X + (float)(npc.width / 2);
			npc.position.Y = npc.position.Y + (float)(npc.height / 2);
			npc.damage = 150;
			npc.width = (npc.height = 216);
			npc.position.X = npc.position.X - (float)(npc.width / 2);
			npc.position.Y = npc.position.Y - (float)(npc.height / 2);
			for (int num621 = 0; num621 < 15; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default, 1f);
				Main.dust[num622].velocity *= 3f;
				if (Main.rand.NextBool(2))
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
				Main.dust[num622].noGravity = true;
			}
			for (int num623 = 0; num623 < 30; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 5f;
				num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default, 1f);
				Main.dust[num624].velocity *= 2f;
				Main.dust[num624].noGravity = true;
			}
			return true;
		}
	}
}
