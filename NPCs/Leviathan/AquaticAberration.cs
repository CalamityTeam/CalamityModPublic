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

namespace CalamityMod.NPCs.Leviathan
{
	public class AquaticAberration : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aquatic Aberration");
			Main.npcFrameCount[npc.type] = 9;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			npc.damage = 60;
			npc.width = 70; //324
			npc.height = 40; //216
			npc.defense = 18;
			npc.lifeMax = CalamityWorld.death ? 2200 : 1100;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 100000;
            }
            npc.knockBackResist = 0f;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			aiType = -1;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			banner = npc.type;
			bannerItem = mod.ItemType("AquaticAberrationBanner");
		}
		
		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
		
		public override void AI()
		{
			bool revenge = CalamityWorld.revenge;
			if (CalamityGlobalNPC.leviathan < 0 || !Main.npc[CalamityGlobalNPC.leviathan].active)
			{
				npc.active = false;
				npc.netUpdate = true;
				return;
			}
			npc.TargetClosest(false);
			npc.rotation = npc.velocity.ToRotation();
			if (Math.Sign(npc.velocity.X) != 0) 
			{
				npc.spriteDirection = -Math.Sign(npc.velocity.X);
			}
			if (npc.rotation < -1.57079637f) 
			{
				npc.rotation += 3.14159274f;
			}
			if (npc.rotation > 1.57079637f) 
			{
				npc.rotation -= 3.14159274f;
			}
			npc.spriteDirection = Math.Sign(npc.velocity.X);
			float num998 = 8f;
			float scaleFactor3 = 300f;
			float num999 = 800f;
			float num1000 = 60f;
			float num1001 = 5f;
			float scaleFactor4 = 0.8f;
			int num1002 = 0;
			float scaleFactor5 = 10f;
			float num1003 = 30f;
			float num1004 = 150f;
			float num1005 = 60f;
			float num1006 = 0.333333343f;
			float num1007 = 8f;
			num1006 *= num1005;
			int num1009 = (npc.ai[0] == 2f) ? 2 : 1;
			int num1010 = (npc.ai[0] == 2f) ? 30 : 20;
			for (int num1011 = 0; num1011 < 2; num1011++) 
			{
				if (Main.rand.Next(3) < num1009) 
				{
					int num1012 = Dust.NewDust(npc.Center - new Vector2((float)num1010), num1010 * 2, num1010 * 2, 33, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default(Color), 1.5f);
					Main.dust[num1012].noGravity = true;
					Main.dust[num1012].velocity *= 0.2f;
					Main.dust[num1012].fadeIn = 1f;
				}
			}
			if (npc.ai[0] == 0f) 
			{
				float scaleFactor6 = num998;
				Vector2 center4 = npc.Center;
				Vector2 center5 = Main.player[npc.target].Center;
				Vector2 vector126 = center5 - center4;
				Vector2 vector127 = vector126 - Vector2.UnitY * scaleFactor3;
				float num1013 = vector126.Length();
				vector126 = Vector2.Normalize(vector126) * scaleFactor6;
				vector127 = Vector2.Normalize(vector127) * scaleFactor6;
				bool flag64 = Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1);
				if (npc.ai[3] >= 120f) 
				{
					flag64 = true;
				}
				float num1014 = 8f;
				flag64 = (flag64 && vector126.ToRotation() > 3.14159274f / num1014 && vector126.ToRotation() < 3.14159274f - 3.14159274f / num1014);
				if (num1013 > num999 || !flag64) 
				{
					npc.velocity.X = (npc.velocity.X * (num1000 - 1f) + vector127.X) / num1000;
					npc.velocity.Y = (npc.velocity.Y * (num1000 - 1f) + vector127.Y) / num1000;
					if (!flag64) 
					{
						npc.ai[3] += 1f;
						if (npc.ai[3] == 120f) 
						{
							npc.netUpdate = true;
						}
					} 
					else
					{
						npc.ai[3] = 0f;
					}
				} 
				else 
				{
					npc.ai[0] = 1f;
					npc.ai[2] = vector126.X;
					npc.ai[3] = vector126.Y;
					npc.netUpdate = true;
				}
			} 
			else if (npc.ai[0] == 1f) 
			{
				npc.velocity *= scaleFactor4;
				npc.ai[1] += 1f;
				if (npc.ai[1] >= num1001) 
				{
					npc.ai[0] = 2f;
					npc.ai[1] = 0f;
					npc.netUpdate = true;
					Vector2 velocity = new Vector2(npc.ai[2], npc.ai[3]) + new Vector2((float)Main.rand.Next(-num1002, num1002 + 1), (float)Main.rand.Next(-num1002, num1002 + 1)) * 0.04f;
					velocity.Normalize();
					velocity *= scaleFactor5;
					npc.velocity = velocity;
				}
			} 
			else if (npc.ai[0] == 2f) 
			{
				float num1016 = num1003;
				npc.ai[1] += 1f;
				bool flag65 = Vector2.Distance(npc.Center, Main.player[npc.target].Center) > num1004 && npc.Center.Y > Main.player[npc.target].Center.Y;
				if ((npc.ai[1] >= num1016 && flag65) || npc.velocity.Length() < num1007) 
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.velocity /= 2f;
					npc.netUpdate = true;
					npc.ai[1] = 45f;
					npc.ai[0] = 4f;
				} 
				else 
				{
					Vector2 center6 = npc.Center;
					Vector2 center7 = Main.player[npc.target].Center;
					Vector2 vec2 = center7 - center6;
					vec2.Normalize();
					if (vec2.HasNaNs()) 
					{
						vec2 = new Vector2((float)npc.direction, 0f);
					}
					npc.velocity = (npc.velocity * (num1005 - 1f) + vec2 * (npc.velocity.Length() + num1006)) / num1005;
				}
			} 
			else if (npc.ai[0] == 4f) 
			{
				npc.ai[1] -= 3f;
				if (npc.ai[1] <= 0f) 
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.netUpdate = true;
				}
				npc.velocity *= 0.95f;
			}
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.Wet, 120, true);
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("MarkedforDeath"), 120);
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
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}