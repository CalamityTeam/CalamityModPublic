using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.CosmicWraith
{
	public class CosmicLantern : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmic Lantern");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 100;
			npc.width = 25; //324
			npc.height = 25; //216
			npc.defense = 85;
			npc.lifeMax = 25;
            npc.alpha = 255;
			npc.knockBackResist = 0.85f;
			npc.noGravity = true;
            npc.dontTakeDamage = true;
            npc.chaseable = false;
			npc.canGhostHeal = false;
			npc.noTileCollide = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
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
            npc.alpha -= 3;
            if (npc.alpha <= 0)
            {
                npc.alpha = 0;
                int num1262 = Dust.NewDust(npc.position, npc.width, npc.height, 204, 0f, 0f, 0, default(Color), 0.25f);
                Main.dust[num1262].velocity *= 0.1f;
                Main.dust[num1262].noGravity = true;
            }
            npc.rotation = npc.velocity.X * 0.08f;
            npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
            npc.TargetClosest(true);
            Vector2 vector145 = new Vector2(npc.Center.X, npc.Center.Y);
            float num1258 = Main.player[npc.target].Center.X - vector145.X;
            float num1259 = Main.player[npc.target].Center.Y - vector145.Y;
            float num1260 = (float)Math.Sqrt((double)(num1258 * num1258 + num1259 * num1259));
            bool revenge = CalamityWorld.revenge;
            float num1261 = revenge ? 27f : 25f;
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
            {
                num1261 = 30f;
            }
            if (npc.localAI[0] < 85f)
            {
                num1261 = 0.1f;
                num1260 = num1261 / num1260;
                num1258 *= num1260;
                num1259 *= num1260;
                npc.velocity.X = (npc.velocity.X * 100f + num1258) / 101f;
                npc.velocity.Y = (npc.velocity.Y * 100f + num1259) / 101f;
                npc.localAI[0] += 1f;
                return;
            }
            npc.dontTakeDamage = false;
            npc.chaseable = true;
			num1260 = num1261 / num1260;
			num1258 *= num1260;
			num1259 *= num1260;
			npc.velocity.X = (npc.velocity.X * 100f + num1258) / 101f;
			npc.velocity.Y = (npc.velocity.Y * 100f + num1259) / 101f;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("MarkedforDeath"), 180);
			}
		}
		
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 0;
			return true;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 204, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}