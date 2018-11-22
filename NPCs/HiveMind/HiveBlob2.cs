using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.HiveMind
{
	public class HiveBlob2 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hive Blob");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 0.1f;
			npc.aiStyle = -1;
			npc.damage = 0;
			npc.width = 25; //324
			npc.height = 25; //216
			npc.lifeMax = 75;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 13000;
            }
            aiType = -1;
			npc.value = Item.buyPrice(0, 0, 0, 0);
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
		}
		
		public override void AI()
		{
			bool expertMode = Main.expertMode;
			bool revenge = CalamityWorld.revenge;
			if (CalamityGlobalNPC.hiveMind2 < 0) 
			{
				npc.StrikeNPCNoInteraction(9999, 0f, 0, false, false, false);
				npc.netUpdate = true;
				return;
			}
			int num750 = CalamityGlobalNPC.hiveMind2;
			if (npc.ai[3] > 0f) 
			{
				num750 = (int)npc.ai[3] - 1;
			}
			if (Main.netMode != 1) 
			{
				npc.localAI[0] -= 1f;
				if (npc.localAI[0] <= 0f) 
				{
					npc.localAI[0] = (float)Main.rand.Next(120, 480);
					npc.ai[0] = (float)Main.rand.Next(-100, 101);
					npc.ai[1] = (float)Main.rand.Next(-100, 101);
					npc.netUpdate = true;
				}
			}
			npc.TargetClosest(true);
			float num751 = 0.02f;
			float num752 = 300f;
			if ((double)Main.npc[CalamityGlobalNPC.hiveMind2].life < (double)Main.npc[CalamityGlobalNPC.hiveMind2].lifeMax * 0.25) 
			{
				num752 += 30f;
			}
			if ((double)Main.npc[CalamityGlobalNPC.hiveMind2].life < (double)Main.npc[CalamityGlobalNPC.hiveMind2].lifeMax * 0.1) 
			{
				num752 += 60f;
			}
			if (expertMode) 
			{
				float num753 = 1f - (float)npc.life / (float)npc.lifeMax;
				num752 += num753 * 100f;
				num751 += 0.03f;
			}
			if (revenge)
			{
				num751 += 0.1f;
			}
			if (!Main.npc[num750].active || CalamityGlobalNPC.hiveMind2 < 0) 
			{
				npc.active = false;
				return;
			}
			Vector2 vector22 = new Vector2(npc.ai[0] * 16f + 8f, npc.ai[1] * 16f + 8f);
			float num189 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - (float)(npc.width / 2) - vector22.X;
			float num190 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - (float)(npc.height / 2) - vector22.Y;
			float num191 = (float)Math.Sqrt((double)(num189 * num189 + num190 * num190));
			float num754 = Main.npc[num750].position.X + (float)(Main.npc[num750].width / 2);
			float num755 = Main.npc[num750].position.Y + (float)(Main.npc[num750].height / 2);
			Vector2 vector93 = new Vector2(num754, num755);
			float num756 = num754 + npc.ai[0];
			float num757 = num755 + npc.ai[1];
			float num758 = num756 - vector93.X;
			float num759 = num757 - vector93.Y;
			float num760 = (float)Math.Sqrt((double)(num758 * num758 + num759 * num759));
			num760 = num752 / num760;
			num758 *= num760;
			num759 *= num760;
			if (npc.position.X < num754 + num758) 
			{
				npc.velocity.X = npc.velocity.X + num751;
				if (npc.velocity.X < 0f && num758 > 0f) 
				{
					npc.velocity.X = npc.velocity.X * 0.9f;
				}
			} 
			else if (npc.position.X > num754 + num758) 
			{
				npc.velocity.X = npc.velocity.X - num751;
				if (npc.velocity.X > 0f && num758 < 0f) 
				{
					npc.velocity.X = npc.velocity.X * 0.9f;
				}
			}
			if (npc.position.Y < num755 + num759) 
			{
				npc.velocity.Y = npc.velocity.Y + num751;
				if (npc.velocity.Y < 0f && num759 > 0f) 
				{
					npc.velocity.Y = npc.velocity.Y * 0.9f;
				}
			} 
			else if (npc.position.Y > num755 + num759) 
			{
				npc.velocity.Y = npc.velocity.Y - num751;
				if (npc.velocity.Y > 0f && num759 < 0f) 
				{
					npc.velocity.Y = npc.velocity.Y * 0.9f;
				}
			}
			if (npc.velocity.X > 8f) 
			{
				npc.velocity.X = 8f;
			}
			if (npc.velocity.X < -8f) 
			{
				npc.velocity.X = -8f;
			}
			if (npc.velocity.Y > 8f) 
			{
				npc.velocity.Y = 8f;
			}
			if (npc.velocity.Y < -8f) 
			{
				npc.velocity.Y = -8f;
			}
		}
		
		public override bool CheckActive()
		{
			return false;
		}
		
		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.1f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}