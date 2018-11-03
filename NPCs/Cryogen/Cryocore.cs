using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Cryogen
{
	public class Cryocore : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cryocore");
            Main.npcFrameCount[npc.type] = 5;
        }
		
		public override void SetDefaults()
		{
			npc.damage = 35;
			npc.width = 40; //324
			npc.height = 40; //216
			npc.defense = 5;
			npc.lifeMax = 220;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 30000;
            }
            npc.aiStyle = -1; //new
            aiType = -1; //new
            animationType = 10; //new
			npc.knockBackResist = 0.75f;
			npc.value = Item.buyPrice(0, 0, 0, 0);
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit5;
			npc.DeathSound = SoundID.NPCDeath15;
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
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.01f, 0.25f, 0.25f);
			npc.TargetClosest(true);
			float num1372 = revenge ? 12f : 11f;
			Vector2 vector167 = new Vector2(npc.Center.X + (float)(npc.direction * 20), npc.Center.Y + 6f);
			float num1373 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector167.X;
			float num1374 = Main.player[npc.target].Center.Y - vector167.Y;
			float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
			float num1376 = num1372 / num1375;
			num1373 *= num1376;
			num1374 *= num1376;
			npc.ai[0] -= 1f;
			if (num1375 < 200f || npc.ai[0] > 0f)
			{
				if (num1375 < 200f)
				{
					npc.ai[0] = 20f;
				}
				if (npc.velocity.X < 0f)
				{
					npc.direction = -1;
				}
				else
				{
					npc.direction = 1;
				}
				npc.rotation += (float)npc.direction * 0.3f;
				return;
			}
			npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
			npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
			if (num1375 < 350f)
			{
				npc.velocity.X = (npc.velocity.X * 10f + num1373) / 11f;
				npc.velocity.Y = (npc.velocity.Y * 10f + num1374) / 11f;
			}
			if (num1375 < 300f)
			{
				npc.velocity.X = (npc.velocity.X * 7f + num1373) / 8f;
				npc.velocity.Y = (npc.velocity.Y * 7f + num1374) / 8f;
			}
			npc.rotation = npc.velocity.X * 0.15f;
			return;
		}
		
		public override bool PreNPCLoot()
		{
			return false;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}