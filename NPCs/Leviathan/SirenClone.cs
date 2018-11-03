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
	public class SirenClone : ModNPC
	{
		public int timer = 0;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Siren Clone");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 0;
			npc.width = 120; //324
			npc.height = 120; //216
			npc.defense = 0;
			npc.lifeMax = 3000;
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 0, 0, 0);
			npc.noGravity = true;
			npc.chaseable = false;
			npc.dontTakeDamage = true;
			npc.HitSound = SoundID.NPCHit1;
			npc.alpha = 255;
		}
		
		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.1f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
		
		public override void AI()
		{
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0f, 0.5f, 0.3f);
			if (npc.alpha > 50)
			{
				npc.alpha -= 5;
			}
			npc.TargetClosest(true);
			Vector2 center = npc.Center;
			int num14 = Math.Sign(Main.player[npc.target].Center.X - center.X);
			if (num14 != 0)
			{
				npc.direction = (npc.spriteDirection = num14);
			}
			Vector2 direction = Main.player[npc.target].Center - center;
			direction.Normalize();
			direction *= (CalamityWorld.death ? 15f : 11f); //9
			timer++;
			if (timer > (CalamityWorld.death ? 30 : 60))
			{
				if (Main.netMode != 1)
				{
					int type = mod.ProjectileType("WaterSpear");
                    switch (Main.rand.Next(6))
                    {
                        case 0: type = mod.ProjectileType("FrostMist"); break;
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5: type = mod.ProjectileType("WaterSpear"); break;
                    }
                    int damage = Main.expertMode ? 35 : 48;
					Projectile.NewProjectile(center.X, center.Y, direction.X, direction.Y, type, damage, 1f, npc.target);
				}
				timer = 0;
			}
			if (NPC.CountNPCS(mod.NPCType("Siren")) < 1)
			{
				npc.active = false;
                npc.netUpdate = true;
				return;
			}
		}
		
		public override bool CheckActive()
		{
			return false;
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = 3000;
			npc.damage = 0;
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
					Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}