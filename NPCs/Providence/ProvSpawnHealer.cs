using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Providence
{
	public class ProvSpawnHealer : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("A Profaned Guardian");
			Main.npcFrameCount[npc.type] = 6;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 1f;
			npc.aiStyle = -1;
			npc.damage = 0;
			npc.width = 100; //324
			npc.height = 80; //216
			npc.defense = 30;
			npc.lifeMax = 37500;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 500000 : 400000;
            }
            npc.knockBackResist = 0f;
			npc.noGravity = true;
			npc.noTileCollide = true;
			aiType = -1;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
				npc.buffImmune[BuffID.Ichor] = false;
                npc.buffImmune[BuffID.CursedInferno] = false;
                npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
                npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
                npc.buffImmune[mod.BuffType("DemonFlames")] = false;
                npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
                npc.buffImmune[mod.BuffType("Nightwither")] = false;
                npc.buffImmune[mod.BuffType("Shred")] = false;
                npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
                npc.buffImmune[mod.BuffType("SilvaStun")] = false;
            }
			npc.value = Item.buyPrice(0, 5, 0, 0);
			npc.HitSound = SoundID.NPCHit52;
			npc.DeathSound = SoundID.NPCDeath55;
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
			bool isHoly = Main.player[npc.target].ZoneHoly;
			bool isHell = Main.player[npc.target].ZoneUnderworldHeight;
			npc.defense = (isHoly || isHell || CalamityWorld.bossRushActive) ? 30 : 99999;
			Vector2 vectorCenter = npc.Center;
			Player player = Main.player[npc.target];
			npc.TargetClosest(false);
            if (NPC.CountNPCS(mod.NPCType("Providence")) < 1)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            if (Math.Sign(npc.velocity.X) != 0) 
			{
				npc.spriteDirection = -Math.Sign(npc.velocity.X);
			}
			npc.spriteDirection = Math.Sign(npc.velocity.X);
			int num1009 = (npc.ai[0] == 0f) ? 1 : 2;
			int num1010 = (npc.ai[0] == 0f) ? 60 : 80;
			for (int num1011 = 0; num1011 < 2; num1011++) 
			{
				if (Main.rand.Next(3) < num1009) 
				{
					int dustType = Main.rand.Next(2);
					if (dustType == 0)
					{
						dustType = 244;
					}
					else
					{
						dustType = 107;
					}
					int num1012 = Dust.NewDust(npc.Center - new Vector2((float)num1010), num1010 * 2, num1010 * 2, dustType, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default(Color), 0.5f);
					Main.dust[num1012].noGravity = true;
					Main.dust[num1012].velocity *= 0.2f;
					Main.dust[num1012].fadeIn = 1f;
				}
			}
            if (npc.ai[0] == 0f)
			{
				Vector2 vector96 = new Vector2(npc.Center.X, npc.Center.Y);
				float num784 = Main.npc[CalamityGlobalNPC.holyBoss].Center.X - vector96.X;
				float num785 = Main.npc[CalamityGlobalNPC.holyBoss].Center.Y - vector96.Y;
				float num786 = (float)Math.Sqrt((double)(num784 * num784 + num785 * num785));
				if (num786 > 120f) 
				{
					num786 = 24f / num786; //8f
					num784 *= num786;
					num785 *= num786;
					npc.velocity.X = (npc.velocity.X * 15f + num784) / 16f;
					npc.velocity.Y = (npc.velocity.Y * 15f + num785) / 16f;
					return;
				}
				if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 24f) //8f
				{
					npc.velocity.Y = npc.velocity.Y * 1.15f; //1.05f
					npc.velocity.X = npc.velocity.X * 1.15f; //1.05f
				}
				if (Main.netMode != 1 && ((expertMode && Main.rand.Next(2000) == 0) || Main.rand.Next(1000) == 0)) 
				{
					npc.TargetClosest(true);
					vector96 = new Vector2(npc.Center.X, npc.Center.Y);
					num784 = player.Center.X - vector96.X;
					num785 = player.Center.Y - vector96.Y;
					num786 = (float)Math.Sqrt((double)(num784 * num784 + num785 * num785));
					num786 = 24f / num786; //8f
					npc.velocity.X = num784 * num786;
					npc.velocity.Y = num785 * num786;
					npc.ai[0] = 1f;
					npc.netUpdate = true;
					return;
				}
			}
			else 
			{
				if (expertMode) 
				{
					Vector2 value4 = player.Center - npc.Center;
					value4.Normalize();
					value4 *= 9f; //9f
					npc.velocity = (npc.velocity * 99f + value4) / 98f; //100
				}
				Vector2 vector97 = new Vector2(npc.Center.X, npc.Center.Y);
				float num787 = Main.npc[CalamityGlobalNPC.holyBoss].Center.X - vector97.X;
				float num788 = Main.npc[CalamityGlobalNPC.holyBoss].Center.Y - vector97.Y;
				float num789 = (float)Math.Sqrt((double)(num787 * num787 + num788 * num788));
				if (num789 > 700f || npc.justHit) 
				{
					npc.ai[0] = 0f;
					return;
				}
			}
		}

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.OnFire, 600, true);
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 50; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}