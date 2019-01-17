using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.SupremeCalamitas
{
	[AutoloadBossHead]
	public class SupremeCatastrophe : ModNPC
	{
		public float distanceX = 250f;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Catastrophe");
			Main.npcFrameCount[npc.type] = 6;
		}
		
		public override void SetDefaults()
		{
			npc.damage = 0;
			npc.npcSlots = 5f;
			npc.width = 120; //324
			npc.height = 120; //216
			npc.defense = 150;
            npc.lifeMax = CalamityWorld.revenge ? 700000 : 600000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 500000;
            }
            npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.knockBackResist = 0f;
			npc.noGravity = true;
			npc.noTileCollide = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
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
            if (CalamityGlobalNPC.SCal <= 0)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            npc.TargetClosest(true);
			float num676 = 60f;
			float num677 = 1.5f;
			float distanceY = 450f;
			if (npc.localAI[1] <= 1000f)
			{
				npc.localAI[1] += 1f;
				distanceX += 0.5f;
			}
			else if (npc.localAI[1] <= 2000f)
			{
				npc.localAI[1] += 1f;
				distanceX -= 0.5f;
			}
			if (npc.localAI[1] >= 2000f)
			{
				npc.localAI[1] = 0f;
			}
			Vector2 vector83 = new Vector2(npc.Center.X, npc.Center.Y);
			float num678 = Main.player[npc.target].Center.X - vector83.X - distanceX;
			float num679 = Main.player[npc.target].Center.Y - vector83.Y - distanceY;
            float num740 = Main.player[npc.target].Center.X - vector83.X;
            float num741 = Main.player[npc.target].Center.Y - vector83.Y;
            npc.rotation = (float)Math.Atan2((double)num741, (double)num740) - 1.57f;
            float num680 = (float)Math.Sqrt((double)(num678 * num678 + num679 * num679));
			num680 = num676 / num680;
			num678 *= num680;
			num679 *= num680;
			if (npc.velocity.X < num678) 
			{
				npc.velocity.X = npc.velocity.X + num677;
				if (npc.velocity.X < 0f && num678 > 0f) 
				{
					npc.velocity.X = npc.velocity.X + num677;
				}
			} 
			else if (npc.velocity.X > num678) 
			{
				npc.velocity.X = npc.velocity.X - num677;
				if (npc.velocity.X > 0f && num678 < 0f) 
				{
					npc.velocity.X = npc.velocity.X - num677;
				}
			}
			if (npc.velocity.Y < num679) 
			{
				npc.velocity.Y = npc.velocity.Y + num677;
				if (npc.velocity.Y < 0f && num679 > 0f) 
				{
					npc.velocity.Y = npc.velocity.Y + num677;
				}
			} 
			else if (npc.velocity.Y > num679)
			{
				npc.velocity.Y = npc.velocity.Y - num677;
				if (npc.velocity.Y > 0f && num679 < 0f) 
				{
					npc.velocity.Y = npc.velocity.Y - num677;
				}
			}
			npc.ai[1] += 1f;
            if (npc.ai[1] >= 150f)
            {
                npc.ai[1] = 0f;
                Vector2 vector85 = new Vector2(npc.Center.X, npc.Center.Y);
                float num689 = 2f;
                int num690 = expertMode ? 150 : 200; //600 500
                int num691 = mod.ProjectileType("BrimstoneHellblast");
                float num692 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector85.X;
                float num693 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector85.Y;
                float num694 = (float)Math.Sqrt((double)(num692 * num692 + num693 * num693));
                num694 = num689 / num694;
                num692 *= num694;
                num693 *= num694;
                vector85.X += num692 * 3f;
                vector85.Y += num693 * 3f;
                if (Main.netMode != 1)
                {
                    int num695 = Projectile.NewProjectile(vector85.X, vector85.Y, num692, num693, num691, num690, 0f, Main.myPlayer, 0f, 0f);
                }
                return;
            }
		}

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 100;
				npc.height = 100;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 40; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 70; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
	}
}