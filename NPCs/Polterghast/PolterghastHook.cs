using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Polterghast
{
	public class PolterghastHook : ModNPC
	{
        public int despawnTimer = 300;
        public bool phase2 = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Polterghast Hook");
			Main.npcFrameCount[npc.type] = 2;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 120;
			npc.width = 40; //324
			npc.height = 40; //216
			npc.defense = 50;
            npc.lifeMax = CalamityWorld.revenge ? 60000 : 50000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 100000;
            }
            npc.dontTakeDamage = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.HitSound = SoundID.NPCHit34;
			npc.DeathSound = SoundID.NPCDeath39;
		}

        public override void AI()
        {
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.3f, 1f, 1f);
            bool expertMode = Main.expertMode;
            bool revenge = CalamityWorld.revenge;
            bool speedBoost1 = false;
            bool despawnBoost = false;
            if (CalamityGlobalNPC.ghostBoss <= 0)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            if (CalamityGlobalNPC.ghostBoss != -1 && !Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].ZoneDungeon && !CalamityWorld.bossRushActive)
            {
                despawnTimer--;
                if (despawnTimer <= 0) { despawnBoost = true; }
                npc.localAI[0] -= 6f;
                speedBoost1 = true;
            }
            else { despawnTimer = 300; }
            if ((double)Main.npc[CalamityGlobalNPC.ghostBoss].life <= (double)Main.npc[CalamityGlobalNPC.ghostBoss].lifeMax * 0.75)
            {
                if (!phase2)
                {
                    npc.damage = 0;
                    phase2 = true;
                }
                npc.TargetClosest(true);
                if (Main.netMode == 1)
                {
                    if (npc.ai[0] == 0f)
                    {
                        npc.ai[0] = (float)((int)(npc.Center.X / 16f));
                    }
                    if (npc.ai[1] == 0f)
                    {
                        npc.ai[1] = (float)((int)(npc.Center.X / 16f));
                    }
                }
                if (Main.netMode != 1)
                {
                    if (npc.ai[0] == 0f || npc.ai[1] == 0f)
                    {
                        npc.localAI[0] = 0f;
                    }
                    npc.localAI[0] -= 5f;
                    if (speedBoost1)
                    {
                        npc.localAI[0] -= 10f;
                    }
                    if (!despawnBoost && npc.localAI[0] <= 0f && npc.ai[0] != 0f)
                    {
                        int num;
                        for (int num763 = 0; num763 < 200; num763 = num + 1)
                        {
                            if (num763 != npc.whoAmI && Main.npc[num763].active && Main.npc[num763].type == npc.type && (Main.npc[num763].velocity.X != 0f || Main.npc[num763].velocity.Y != 0f))
                            {
                                npc.localAI[0] = (float)Main.rand.Next(60, 300);
                            }
                            num = num763;
                        }
                    }
                    if (npc.localAI[0] <= 0f)
                    {
                        npc.localAI[0] = (float)Main.rand.Next(300, 600);
                        bool flag50 = false;
                        int num764 = 0;
                        while (!flag50 && num764 <= 1000)
                        {
                            num764++;
                            int num765 = (int)(Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center.X / 16f);
                            int num766 = (int)(Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center.Y / 16f);
                            if (npc.ai[0] == 0f)
                            {
                                num765 = (int)((Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center.X + Main.npc[CalamityGlobalNPC.ghostBoss].Center.X) / 32f);
                                num766 = (int)((Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center.Y + Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y) / 32f);
                            }
                            if (despawnBoost)
                            {
                                num765 = (int)Main.npc[CalamityGlobalNPC.ghostBoss].position.X / 16;
                                num766 = (int)(Main.npc[CalamityGlobalNPC.ghostBoss].position.Y + 400f) / 16;
                            }
                            int num767 = 20;
                            num767 += (int)(100f * ((float)num764 / 1000f));
                            int num768 = num765 + Main.rand.Next(-num767, num767 + 1);
                            int num769 = num766 + Main.rand.Next(-num767, num767 + 1);
                            try
                            {
                                if (WorldGen.SolidTile(num768, num769) || (Main.tile[num768, num769].wall > 0 && (num764 > 500 || Main.npc[CalamityGlobalNPC.ghostBoss].life < Main.npc[CalamityGlobalNPC.ghostBoss].lifeMax / 2)))
                                {
                                    flag50 = true;
                                    npc.ai[0] = (float)num768;
                                    npc.ai[1] = (float)num769;
                                    npc.netUpdate = true;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                if (npc.ai[0] > 0f && npc.ai[1] > 0f)
                {
                    float num772 = 8f;
                    if (expertMode)
                    {
                        num772 += 1f;
                    }
                    if (speedBoost1)
                    {
                        num772 *= 2f;
                    }
                    if (despawnBoost)
                    {
                        num772 *= 2f;
                    }
                    Vector2 vector95 = new Vector2(npc.Center.X, npc.Center.Y);
                    float num773 = npc.ai[0] * 16f - 8f - vector95.X;
                    float num774 = npc.ai[1] * 16f - 8f - vector95.Y;
                    float num775 = (float)Math.Sqrt((double)(num773 * num773 + num774 * num774));
                    if (num775 < 12f + num772)
                    {
                        npc.velocity.X = num773;
                        npc.velocity.Y = num774;
                    }
                    else
                    {
                        num775 = num772 / num775;
                        npc.velocity.X = num773 * num775;
                        npc.velocity.Y = num774 * num775;
                    }
                }
                Vector2 vector102 = new Vector2(npc.Center.X, npc.Center.Y);
                float num818 = Main.player[npc.target].Center.X - vector102.X;
                float num819 = Main.player[npc.target].Center.Y - vector102.Y;
                float num820 = (float)Math.Sqrt((double)(num818 * num818 + num819 * num819));
                float num821 = 10f;
                num820 = num821 / num820;
                num818 *= num820;
                num819 *= num820;
                npc.rotation = (float)Math.Atan2((double)num819, (double)num818) + 1.57f;
                Vector2 vector17 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num147 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector17.X;
                float num148 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector17.Y;
                float num149 = (float)Math.Sqrt((double)(num147 * num147 + num148 * num148));
                num149 = 4f / num149;
                num147 *= num149;
                num148 *= num149;
                vector17 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                num147 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector17.X;
                num148 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector17.Y;
                num149 = (float)Math.Sqrt((double)(num147 * num147 + num148 * num148));
                if (num149 > 800f)
                {
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    return;
                }
                npc.ai[2] += 1f;
                if (npc.ai[3] == 0f)
                {
                    if (npc.ai[2] > 120f)
                    {
                        npc.ai[2] = 0f;
                        npc.ai[3] = 1f;
                        npc.netUpdate = true;
                        return;
                    }
                }
                else
                {
                    if (npc.ai[2] > 40f)
                    {
                        npc.ai[3] = 0f;
                    }
                    if (Main.netMode != 1 && npc.ai[2] == 20f)
                    {
                        float num151 = 5f;
                        int num152 = 60;
                        int num153 = 299;
                        num149 = num151 / num149;
                        num147 *= num149;
                        num148 *= num149;
                        int num154 = Projectile.NewProjectile(vector17.X, vector17.Y, num147, num148, num153, num152, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                return;
            }
            if (Main.netMode == 1)
            {
                if (npc.ai[0] == 0f)
                {
                    npc.ai[0] = (float)((int)(npc.Center.X / 16f));
                }
                if (npc.ai[1] == 0f)
                {
                    npc.ai[1] = (float)((int)(npc.Center.X / 16f));
                }
            }
            if (Main.netMode != 1)
            {
                if (npc.ai[0] == 0f || npc.ai[1] == 0f)
                {
                    npc.localAI[0] = 0f;
                }
                npc.localAI[0] -= 5f;
                if (speedBoost1)
                {
                    npc.localAI[0] -= 10f;
                }
                if (!despawnBoost && npc.localAI[0] <= 0f && npc.ai[0] != 0f)
                {
                    int num;
                    for (int num763 = 0; num763 < 200; num763 = num + 1)
                    {
                        if (num763 != npc.whoAmI && Main.npc[num763].active && Main.npc[num763].type == npc.type && (Main.npc[num763].velocity.X != 0f || Main.npc[num763].velocity.Y != 0f))
                        {
                            npc.localAI[0] = (float)Main.rand.Next(60, 300);
                        }
                        num = num763;
                    }
                }
                if (npc.localAI[0] <= 0f)
                {
                    npc.localAI[0] = (float)Main.rand.Next(300, 600);
                    bool flag50 = false;
                    int num764 = 0;
                    while (!flag50 && num764 <= 1000)
                    {
                        num764++;
                        int num765 = (int)(Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center.X / 16f);
                        int num766 = (int)(Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center.Y / 16f);
                        if (npc.ai[0] == 0f)
                        {
                            num765 = (int)((Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center.X + Main.npc[CalamityGlobalNPC.ghostBoss].Center.X) / 32f);
                            num766 = (int)((Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center.Y + Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y) / 32f);
                        }
                        if (despawnBoost)
                        {
                            num765 = (int)Main.npc[CalamityGlobalNPC.ghostBoss].position.X / 16;
                            num766 = (int)(Main.npc[CalamityGlobalNPC.ghostBoss].position.Y + 400f) / 16;
                        }
                        int num767 = 20;
                        num767 += (int)(100f * ((float)num764 / 1000f));
                        int num768 = num765 + Main.rand.Next(-num767, num767 + 1);
                        int num769 = num766 + Main.rand.Next(-num767, num767 + 1);
                        try
                        {
                            if (WorldGen.SolidTile(num768, num769) || (Main.tile[num768, num769].wall > 0 && (num764 > 500 || Main.npc[CalamityGlobalNPC.ghostBoss].life < Main.npc[CalamityGlobalNPC.ghostBoss].lifeMax / 2)))
                            {
                                flag50 = true;
                                npc.ai[0] = (float)num768;
                                npc.ai[1] = (float)num769;
                                npc.netUpdate = true;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            if (npc.ai[0] > 0f && npc.ai[1] > 0f)
            {
                float num772 = 11f;
                if (expertMode)
                {
                    num772 += 1f;
                }
                if (speedBoost1)
                {
                    num772 *= 2f;
                }
                if (despawnBoost)
                {
                    num772 *= 2f;
                }
                Vector2 vector95 = new Vector2(npc.Center.X, npc.Center.Y);
                float num773 = npc.ai[0] * 16f - 8f - vector95.X;
                float num774 = npc.ai[1] * 16f - 8f - vector95.Y;
                float num775 = (float)Math.Sqrt((double)(num773 * num773 + num774 * num774));
                if (num775 < 12f + num772)
                {
                    npc.velocity.X = num773;
                    npc.velocity.Y = num774;
                }
                else
                {
                    num775 = num772 / num775;
                    npc.velocity.X = num773 * num775;
                    npc.velocity.Y = num774 * num775;
                }
                Vector2 vector96 = new Vector2(npc.Center.X, npc.Center.Y);
                float num776 = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - vector96.X;
                float num777 = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - vector96.Y;
                npc.rotation = (float)Math.Atan2((double)num777, (double)num776) - 1.57f;
            }
        }
		
		public override void FindFrame(int frameHeight)
		{
            if (phase2)
            {
                if (npc.ai[3] == 0f)
                {
                    if (npc.frame.Y < 1)
                    {
                        npc.frameCounter += 1.0;
                        if (npc.frameCounter > 4.0)
                        {
                            npc.frameCounter = 0.0;
                            npc.frame.Y = npc.frame.Y + frameHeight;
                        }
                    }
                }
                else if (npc.frame.Y > 0)
                {
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 4.0)
                    {
                        npc.frameCounter = 0.0;
                        npc.frame.Y = npc.frame.Y - frameHeight;
                    }
                }
                return;
            }
            if (npc.velocity.X == 0f && npc.velocity.Y == 0f)
			{
				if (npc.frame.Y < 1)
				{
					npc.frameCounter += 1.0;
					if (npc.frameCounter > 4.0)
					{
						npc.frameCounter = 0.0;
						npc.frame.Y = npc.frame.Y + frameHeight;
					}
				}
			}
			else if (npc.frame.Y > 0)
			{
				npc.frameCounter += 1.0;
				if (npc.frameCounter > 4.0)
				{
					npc.frameCounter = 0.0;
					npc.frame.Y = npc.frame.Y - frameHeight;
				}
			}
		}
		
		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			if (CalamityGlobalNPC.ghostBoss >= 0 && !phase2) 
			{
				Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
				float bossCenterX = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - center.X;
				float bossCenterY = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - center.Y;
				float rotation2 = (float)Math.Atan2((double)bossCenterY, (double)bossCenterX) - 1.57f;
				bool draw = true;
				while (draw) 
				{
					int chainWidth = 20; //16 24
					int chainHeight = 52; //32 16
					float num10 = (float)Math.Sqrt((double)(bossCenterX * bossCenterX + bossCenterY * bossCenterY));
					if (num10 < (float)chainHeight) 
					{
						chainWidth = (int)num10 - chainHeight + chainWidth;
						draw = false;
					}
					num10 = (float)chainWidth / num10;
					bossCenterX *= num10;
					bossCenterY *= num10;
					center.X += bossCenterX;
					center.Y += bossCenterY;
					bossCenterX = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - center.X;
					bossCenterY = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - center.Y;
					Microsoft.Xna.Framework.Color color2 = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
					Main.spriteBatch.Draw(mod.GetTexture("NPCs/Polterghast/PolterghastChain"), new Vector2(center.X - Main.screenPosition.X, center.Y - Main.screenPosition.Y), 
						new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, mod.GetTexture("NPCs/Polterghast/PolterghastChain").Width, chainWidth)), color2, rotation2, 
						new Vector2((float)mod.GetTexture("NPCs/Polterghast/PolterghastChain").Width * 0.5f, (float)mod.GetTexture("NPCs/Polterghast/PolterghastChain").Height * 0.5f), 1f, SpriteEffects.None, 0f);
				}
			}
			return true;
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
			player.AddBuff(mod.BuffType("Horror"), 180, true);
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.5f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.7f);
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 180, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 180, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}