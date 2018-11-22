using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.StormWeaver
{
	public class StasisProbe : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stasis Probe");
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.npcSlots = 10f;
			npc.damage = 70;
			npc.width = 30; //324
			npc.height = 30; //216
			npc.defense = 100;
			npc.lifeMax = 1200;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 25000;
            }
            npc.knockBackResist = 0.9f;
			npc.value = Item.buyPrice(0, 0, 0, 0);
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.buffImmune[24] = true;
		}
		
		public override void AI()
		{
			bool revenge = CalamityWorld.revenge;
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			float num = revenge ? 12f : 9f;
			float num2 = revenge ? 0.15f : 0.12f;
			Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num4 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
			float num5 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
			num4 = (float)((int)(num4 / 8f) * 8);
			num5 = (float)((int)(num5 / 8f) * 8);
			vector.X = (float)((int)(vector.X / 8f) * 8);
			vector.Y = (float)((int)(vector.Y / 8f) * 8);
			num4 -= vector.X;
			num5 -= vector.Y;
			float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
			float num7 = num6;
			bool flag = false;
			if (num6 > 600f)
			{
				flag = true;
			}
			if (num6 == 0f)
			{
				num4 = npc.velocity.X;
				num5 = npc.velocity.Y;
			}
			else
			{
				num6 = num / num6;
				num4 *= num6;
				num5 *= num6;
			}
			if (num7 > 100f)
			{
				npc.ai[0] += 1f;
				if (npc.ai[0] > 0f)
				{
					npc.velocity.Y = npc.velocity.Y + 0.023f;
				}
				else
				{
					npc.velocity.Y = npc.velocity.Y - 0.023f;
				}
				if (npc.ai[0] < -100f || npc.ai[0] > 100f)
				{
					npc.velocity.X = npc.velocity.X + 0.023f;
				}
				else
				{
					npc.velocity.X = npc.velocity.X - 0.023f;
				}
				if (npc.ai[0] > 200f)
				{
					npc.ai[0] = -200f;
				}
			}
			if (Main.player[npc.target].dead)
			{
				num4 = (float)npc.direction * num / 2f;
				num5 = -num / 2f;
			}
			if (npc.velocity.X < num4)
			{
				npc.velocity.X = npc.velocity.X + num2;
			}
			else if (npc.velocity.X > num4)
			{
				npc.velocity.X = npc.velocity.X - num2;
			}
			if (npc.velocity.Y < num5)
			{
				npc.velocity.Y = npc.velocity.Y + num2;
			}
			else if (npc.velocity.Y > num5)
			{
				npc.velocity.Y = npc.velocity.Y - num2;
			}
			npc.localAI[0] += 1f;
            if (npc.justHit)
            {
                npc.localAI[0] = 0f;
            }
            if (Main.netMode != 1 && npc.localAI[0] >= 90f)
			{
				npc.localAI[0] = 0f;
				if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					int num8 = 70;
					if (Main.expertMode)
					{
						num8 = 50;
					}
					int num9 = 84;
					int beam = Projectile.NewProjectile(vector.X, vector.Y, num4 * 0.75f, num5 * 0.75f, num9, num8, 0f, Main.myPlayer, 0f, 0f);
					Main.projectile[beam].timeLeft = 200;
				}
			}
			int num10 = (int)npc.position.X + npc.width / 2;
			int num11 = (int)npc.position.Y + npc.height / 2;
			num10 /= 16;
			num11 /= 16;
			if (!WorldGen.SolidTile(num10, num11))
			{
				Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.3f, 0f, 0.25f);
			}
			if (num4 > 0f)
			{
				npc.spriteDirection = 1;
				npc.rotation = (float)Math.Atan2((double)num5, (double)num4);
			}
			if (num4 < 0f)
			{
				npc.spriteDirection = -1;
				npc.rotation = (float)Math.Atan2((double)num5, (double)num4) + 3.14f;
			}
			float num12 = 0.7f;
			if (npc.collideX)
			{
				npc.netUpdate = true;
				npc.velocity.X = npc.oldVelocity.X * -num12;
				if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
				{
					npc.velocity.X = 2f;
				}
				if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
				{
					npc.velocity.X = -2f;
				}
			}
			if (npc.collideY)
			{
				npc.netUpdate = true;
				npc.velocity.Y = npc.oldVelocity.Y * -num12;
				if (npc.velocity.Y > 0f && (double)npc.velocity.Y < 1.5)
				{
					npc.velocity.Y = 2f;
				}
				if (npc.velocity.Y < 0f && (double)npc.velocity.Y > -1.5)
				{
					npc.velocity.Y = -2f;
				}
			}
			if (flag)
			{
				if ((npc.velocity.X > 0f && num4 > 0f) || (npc.velocity.X < 0f && num4 < 0f))
				{
					if (Math.Abs(npc.velocity.X) < 12f)
					{
						npc.velocity.X = npc.velocity.X * 1.05f;
					}
				}
				else
				{
					npc.velocity.X = npc.velocity.X * 0.9f;
				}
			}
			if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
			{
				npc.netUpdate = true;
			}
		}
		
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 30;
				npc.height = 30;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 5; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
				for (int num625 = 0; num625 < 3; num625++)
				{
					float scaleFactor10 = 0.33f;
					if (num625 == 1)
					{
						scaleFactor10 = 0.66f;
					}
					if (num625 == 2)
					{
						scaleFactor10 = 1f;
					}
					int num626 = Gore.NewGore(new Vector2(npc.position.X + (float)(npc.width / 2) - 24f, npc.position.Y + (float)(npc.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
					Main.gore[num626].velocity *= scaleFactor10;
					Gore expr_13AB6_cp_0 = Main.gore[num626];
					expr_13AB6_cp_0.velocity.X = expr_13AB6_cp_0.velocity.X + 1f;
					Gore expr_13AD6_cp_0 = Main.gore[num626];
					expr_13AD6_cp_0.velocity.Y = expr_13AD6_cp_0.velocity.Y + 1f;
					num626 = Gore.NewGore(new Vector2(npc.position.X + (float)(npc.width / 2) - 24f, npc.position.Y + (float)(npc.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
					Main.gore[num626].velocity *= scaleFactor10;
					Gore expr_13B79_cp_0 = Main.gore[num626];
					expr_13B79_cp_0.velocity.X = expr_13B79_cp_0.velocity.X - 1f;
					Gore expr_13B99_cp_0 = Main.gore[num626];
					expr_13B99_cp_0.velocity.Y = expr_13B99_cp_0.velocity.Y + 1f;
					num626 = Gore.NewGore(new Vector2(npc.position.X + (float)(npc.width / 2) - 24f, npc.position.Y + (float)(npc.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
					Main.gore[num626].velocity *= scaleFactor10;
					Gore expr_13C3C_cp_0 = Main.gore[num626];
					expr_13C3C_cp_0.velocity.X = expr_13C3C_cp_0.velocity.X + 1f;
					Gore expr_13C5C_cp_0 = Main.gore[num626];
					expr_13C5C_cp_0.velocity.Y = expr_13C5C_cp_0.velocity.Y - 1f;
					num626 = Gore.NewGore(new Vector2(npc.position.X + (float)(npc.width / 2) - 24f, npc.position.Y + (float)(npc.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
					Main.gore[num626].velocity *= scaleFactor10;
					Gore expr_13CFF_cp_0 = Main.gore[num626];
					expr_13CFF_cp_0.velocity.X = expr_13CFF_cp_0.velocity.X - 1f;
					Gore expr_13D1F_cp_0 = Main.gore[num626];
					expr_13D1F_cp_0.velocity.Y = expr_13D1F_cp_0.velocity.Y - 1f;
				}
			}
		}
		
		public override bool PreNPCLoot()
		{
			return false;
		}
	}
}