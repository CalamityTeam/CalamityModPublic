using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.CeaselessVoid
{
	public class DarkEnergy : ModNPC
	{
        public int invinceTime = 180;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dark Energy");
			Main.npcFrameCount[npc.type] = 6;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			npc.damage = 120;
			npc.width = 80; //324
			npc.height = 80; //216
			npc.defense = 68;
			npc.lifeMax = 3000;
            if (CalamityGlobalNPC.DoGSecondStageCountdown <= 0)
            {
                npc.lifeMax = 12000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 30000;
            }
            npc.knockBackResist = 0.25f;
			npc.noGravity = true;
			npc.noTileCollide = true;
			aiType = -1;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.value = Item.buyPrice(0, 0, 0, 0);
			npc.HitSound = SoundID.NPCHit53;
			npc.DeathSound = SoundID.NPCDeath44;
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
			bool expertMode = Main.expertMode;
            if (invinceTime > 0)
            {
                invinceTime--;
                npc.damage = 0;
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.damage = expertMode ? 240 : 120;
                npc.dontTakeDamage = false;
            }
            Vector2 vectorCenter = npc.Center;
			Player player = Main.player[npc.target];
			npc.TargetClosest(true);
            if ((double)npc.life < (double)npc.lifeMax * 0.5)
            {
                npc.knockBackResist = 0f;
            }
			if (npc.ai[1] == 0f)
			{
				npc.scale -= 0.02f;
				npc.alpha += 30;
				if (npc.alpha >= 250)
				{
					npc.alpha = 255;
					npc.ai[1] = 1f;
				}
			}
			else if (npc.ai[1] == 1f)
			{
				npc.scale += 0.02f;
				npc.alpha -= 30;
				if (npc.alpha <= 0)
				{
					npc.alpha = 0;
					npc.ai[1] = 0f;
				}
			}
			int num1009 = (npc.ai[0] == 0f) ? 1 : 2;
			int num1010 = (npc.ai[0] == 0f) ? 60 : 80;
			for (int num1011 = 0; num1011 < 2; num1011++) 
			{
				if (Main.rand.Next(3) < num1009) 
				{
					int num1012 = Dust.NewDust(npc.Center - new Vector2((float)num1010), num1010 * 2, num1010 * 2, 173, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default(Color), 1.5f);
					Main.dust[num1012].noGravity = true;
					Main.dust[num1012].velocity *= 0.2f;
					Main.dust[num1012].fadeIn = 1f;
				}
			}
            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    npc.velocity = new Vector2(0f, -10f);
                    CalamityGlobalNPC.DoGSecondStageCountdown = 0;
                    if (npc.timeLeft > 150)
                    {
                        npc.timeLeft = 150;
                    }
                    return;
                }
            }
            else if (npc.timeLeft < 2400)
            {
                npc.timeLeft = 2400;
            }
            if (npc.ai[0] == 0f) 
			{
				Vector2 vector96 = new Vector2(npc.Center.X, npc.Center.Y);
				float num784 = Main.npc[CalamityGlobalNPC.voidBoss].Center.X - vector96.X;
				float num785 = Main.npc[CalamityGlobalNPC.voidBoss].Center.Y - vector96.Y;
				float num786 = (float)Math.Sqrt((double)(num784 * num784 + num785 * num785));
				if (num786 > 90f) 
				{
					num786 = 8f / num786; //8f
					num784 *= num786;
					num785 *= num786;
					npc.velocity.X = (npc.velocity.X * 15f + num784) / 16f;
					npc.velocity.Y = (npc.velocity.Y * 15f + num785) / 16f;
					return;
				}
				if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 8f) //8f
				{
					npc.velocity.Y = npc.velocity.Y * 1.05f; //1.05f
					npc.velocity.X = npc.velocity.X * 1.05f; //1.05f
				}
				if (Main.netMode != 1 && ((expertMode && Main.rand.Next(50) == 0) || Main.rand.Next(100) == 0)) 
				{
					npc.TargetClosest(true);
					vector96 = new Vector2(npc.Center.X, npc.Center.Y);
					num784 = player.Center.X - vector96.X;
					num785 = player.Center.Y - vector96.Y;
					num786 = (float)Math.Sqrt((double)(num784 * num784 + num785 * num785));
					num786 = 8f / num786; //8f
					npc.velocity.X = num784 * num786;
					npc.velocity.Y = num785 * num786;
					npc.ai[0] = 1f;
					npc.netUpdate = true;
					return;
				}
			} 
			else 
			{
				Vector2 value4 = player.Center - npc.Center;
				value4.Normalize();
				value4 *= 9f; //9f
				npc.velocity = (npc.velocity * 99f + value4) / 100f;
				Vector2 vector97 = new Vector2(npc.Center.X, npc.Center.Y);
				float num787 = Main.npc[CalamityGlobalNPC.voidBoss].Center.X - vector97.X;
				float num788 = Main.npc[CalamityGlobalNPC.voidBoss].Center.Y - vector97.Y;
				float num789 = (float)Math.Sqrt((double)(num787 * num787 + num788 * num788));
				if (num789 > 1400f) 
				{
					npc.ai[0] = 0f;
					return;
				}
			}
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 600, true);
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
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}