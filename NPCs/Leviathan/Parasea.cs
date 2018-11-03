using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Leviathan
{
	public class Parasea : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Parasea");
			Main.npcFrameCount[npc.type] = 6;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.damage = 34;
			npc.width = 90; //324
			npc.height = 20; //216
			npc.defense = 9;
			npc.lifeMax = 650;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 50000;
            }
            npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 0, 0, 0);
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
		}

        public override void AI()
		{
			bool revenge = CalamityWorld.revenge;
			npc.TargetClosest(true);
			Vector2 vector145 = new Vector2(npc.Center.X, npc.Center.Y);
			float num1258 = Main.player[npc.target].Center.X - vector145.X;
			float num1259 = Main.player[npc.target].Center.Y - vector145.Y;
			float num1260 = (float)Math.Sqrt((double)(num1258 * num1258 + num1259 * num1259));
			float num1261 = revenge ? 16f : 13f;
			num1260 = num1261 / num1260;
			num1258 *= num1260;
			num1259 *= num1260;
			npc.velocity.X = (npc.velocity.X * 100f + num1258) / 101f;
			npc.velocity.Y = (npc.velocity.Y * 100f + num1259) / 101f;
			npc.rotation = (float)Math.Atan2((double)num1259, (double)num1258) + 3.14f; //1.57
			return;
		}
		
		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.Wet, 60, true);
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}