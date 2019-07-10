using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.SunkenSea
{
	public class SerpentineHead : ModProjectile
	{
		
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Serpentine");
		}
    	
		public override void SetDefaults()
		{
			projectile.width = 14;
			projectile.height = 16;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.netImportant = true;
			projectile.penetrate = 5;
			projectile.timeLeft = 600;
			projectile.magic = true;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 20;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.55f) / 255f, ((255 - projectile.alpha) * 0.55f) / 255f);
			if (projectile.alpha > 0)
			{
				projectile.alpha -= 40;
			}
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}
            int num114 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 68, 0f, 0f, 100, default(Color), 1.25f);
            Dust dust = Main.dust[num114];
            dust.velocity *= 0.3f;
            Main.dust[num114].position.X = projectile.position.X + (float)(projectile.width / 2) + 4f + (float)Main.rand.Next(-4, 5);
            Main.dust[num114].position.Y = projectile.position.Y + (float)(projectile.height / 2) + (float)Main.rand.Next(-4, 5);
            Main.dust[num114].noGravity = true;
			projectile.rotation = projectile.velocity.ToRotation() + 1.57079637f;
			int direction = projectile.direction;
			projectile.direction = (projectile.spriteDirection = ((projectile.velocity.X > 0f) ? 1 : -1));
			if (direction != projectile.direction)
			{
				projectile.netUpdate = true;
			}
			float num1061 = MathHelper.Clamp(projectile.localAI[0], 0f, 50f);
			projectile.position = projectile.Center;
			projectile.scale = 1f + num1061 * 0.01f;
			projectile.width = (projectile.height = (int)(10 * projectile.scale));
			projectile.Center = projectile.position;
			
			projectile.ai[0] += 1f;
			if (projectile.ai[0] >= 20f && projectile.ai[0] < 40f)
			{
				projectile.velocity.Y = projectile.velocity.Y + 0.3f;
			}
			else if (projectile.ai[0] >= 40f && projectile.ai[0] < 60f)
			{
				projectile.velocity.Y = projectile.velocity.Y - 0.3f;
			}
			else if (projectile.ai[0] >= 60f)
			{
				projectile.ai[0] = 0f;
			}
			if (Main.myPlayer == projectile.owner && projectile.ai[0] <= 0f)
            {
                if (Main.player[projectile.owner].channel)
                {
                    float num115 = 18f;
                    Vector2 vector10 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num116 = (float)Main.mouseX + Main.screenPosition.X - vector10.X;
                    float num117 = (float)Main.mouseY + Main.screenPosition.Y - vector10.Y;
                    if (Main.player[projectile.owner].gravDir == -1f)
                    {
                        num117 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector10.Y;
                    }
                    float num118 = (float)Math.Sqrt((double)(num116 * num116 + num117 * num117));
                    num118 = (float)Math.Sqrt((double)(num116 * num116 + num117 * num117));
                    if (num118 > num115)
                    {
                        num118 = num115 / num118;
                        num116 *= num118;
                        num117 *= num118;
                        int num119 = (int)(num116 * 1000f);
                        int num120 = (int)(projectile.velocity.X * 1000f);
                        int num121 = (int)(num117 * 1000f);
                        int num122 = (int)(projectile.velocity.Y * 1000f);
                        if (num119 != num120 || num121 != num122)
                        {
                            projectile.netUpdate = true;
                        }
                        projectile.velocity.X = num116;
                        projectile.velocity.Y = num117;
                    }
                    else
                    {
                        int num123 = (int)(num116 * 1000f);
                        int num124 = (int)(projectile.velocity.X * 1000f);
                        int num125 = (int)(num117 * 1000f);
                        int num126 = (int)(projectile.velocity.Y * 1000f);
                        if (num123 != num124 || num125 != num126)
                        {
                            projectile.netUpdate = true;
                        }
                        projectile.velocity.X = num116;
                        projectile.velocity.Y = num117;
                    }
                }
                else if (projectile.ai[0] <= 0f)
                {
                    projectile.netUpdate = true;
                    float num127 = 12f;
                    Vector2 vector11 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num128 = (float)Main.mouseX + Main.screenPosition.X - vector11.X;
                    float num129 = (float)Main.mouseY + Main.screenPosition.Y - vector11.Y;
                    if (Main.player[projectile.owner].gravDir == -1f)
                    {
                        num129 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector11.Y;
                    }
                    float num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                    if (num130 == 0f || projectile.ai[0] < 0f)
                    {
                        vector11 = new Vector2(Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2), Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2));
                        num128 = projectile.position.X + (float)projectile.width * 0.5f - vector11.X;
                        num129 = projectile.position.Y + (float)projectile.height * 0.5f - vector11.Y;
                        num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                    }
                    num130 = num127 / num130;
                    num128 *= num130;
                    num129 *= num130;
                    projectile.velocity.X = num128;
                    projectile.velocity.Y = num129;
                    if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                    {
                        projectile.Kill();
                    }
                    projectile.ai[0] = 1f;
                }
            }
            if (projectile.velocity.X != 0f || projectile.velocity.Y != 0f)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            }
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
		}
		
		public override void Kill(int timeLeft)
        {
        	Main.PlaySound(SoundID.Item10, projectile.position);
        	for (int k = 0; k < 8; k++)
            {
            	int num114 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 68, 0f, 0f, 100, default(Color), 1.25f);
				Dust dust = Main.dust[num114];
				dust.velocity *= 0.3f;
				Main.dust[num114].position.X = projectile.position.X + (float)(projectile.width / 2) + 4f + (float)Main.rand.Next(-4, 5);
				Main.dust[num114].position.Y = projectile.position.Y + (float)(projectile.height / 2) + (float)Main.rand.Next(-4, 5);
				Main.dust[num114].noGravity = true;
            }
        }
	}
}
