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
			NPCID.Sets.TrailCacheLength[npc.type] = 8;
			NPCID.Sets.TrailingMode[npc.type] = 1;
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
            if (!Main.npc[CalamityGlobalNPC.SCal].active)
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
            }
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			Microsoft.Xna.Framework.Color color24 = npc.GetAlpha(drawColor);
			Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
			Texture2D texture2D3 = Main.npcTexture[npc.type];
			int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
			int y3 = num156 * (int)npc.frameCounter;
			Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, y3, texture2D3.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			int num157 = 8;
			int num158 = 2;
			int num159 = 1;
			float num160 = 0f;
			int num161 = num159;
			while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)) && Lighting.NotRetro)
			{
				Microsoft.Xna.Framework.Color color26 = npc.GetAlpha(color25);
				{
					goto IL_6899;
				}
				IL_6881:
				num161 += num158;
				continue;
				IL_6899:
				float num164 = (float)(num157 - num161);
				if (num158 < 0)
				{
					num164 = (float)(num159 - num161);
				}
				color26 *= num164 / ((float)NPCID.Sets.TrailCacheLength[npc.type] * 1.5f);
				Vector2 value4 = (npc.oldPos[num161]);
				float num165 = npc.rotation;
				Main.spriteBatch.Draw(texture2D3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, spriteEffects, 0f);
				goto IL_6881;
			}
			var something = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(texture2D3, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, color24, npc.rotation, npc.frame.Size() / 2, npc.scale, something, 0);
			return false;
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