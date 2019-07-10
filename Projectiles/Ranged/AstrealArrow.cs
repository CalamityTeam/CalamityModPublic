using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class AstrealArrow : ModProjectile
    {
    	private int flameTimer = 180;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astreal Arrow");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
		}
        
        public override void AI()
        {
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        	if (projectile.localAI[0] == 0f)
			{
				projectile.scale -= 0.02f;
				projectile.alpha += 30;
				if (projectile.alpha >= 250)
				{
					projectile.alpha = 255;
					projectile.localAI[0] = 1f;
				}
			}
			else if (projectile.localAI[0] == 1f)
			{
				projectile.scale += 0.02f;
				projectile.alpha -= 30;
				if (projectile.alpha <= 0)
				{
					projectile.alpha = 0;
					projectile.localAI[0] = 0f;
				}
			}
			int random = Main.rand.Next(1, 4);
			flameTimer -= random;
        	int choice = Main.rand.Next(2);
        	projectile.velocity.X *= 1.05f;
        	projectile.velocity.Y *= 1.05f;
        	if (choice == 0 && (projectile.velocity.X >= 25f || projectile.velocity.Y >= 25f))
        	{
        		projectile.velocity.X = 0f;
        		projectile.velocity.Y = 10f;
        	}
        	else if (choice == 1 && (projectile.velocity.X >= 25f || projectile.velocity.Y >= 25f))
        	{
        		projectile.velocity.X = 10f;
        		projectile.velocity.Y = 0f;
        	}
        	else if (choice == 0 && (projectile.velocity.X <= -25f || projectile.velocity.Y <= -25f))
        	{
        		projectile.velocity.X = 0f;
        		projectile.velocity.Y = -10f;
        	}
        	else if (choice == 1 && (projectile.velocity.X <= -25f || projectile.velocity.Y <= -25f))
        	{
        		projectile.velocity.X = -10f;
        		projectile.velocity.Y = 0f;
        	}
            if (Main.rand.Next(5) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (flameTimer <= 0)
			{
                float xPos = (Main.rand.Next(2) == 0 ? projectile.position.X + 800 : projectile.position.X - 800);
                Vector2 vector2 = new Vector2(xPos, projectile.position.Y + Main.rand.Next(-800, 801));
                float num80 = xPos;
                float speedX = (float)projectile.position.X - vector2.X;
                float speedY = (float)projectile.position.Y - vector2.Y;
                float dir = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
                dir = 10 / num80;
                speedX *= dir * 150;
                speedY *= dir * 150;
                if (speedX > 15f)
                {
                    speedX = 15f;
                }
                if (speedX < -15f)
                {
                    speedX = -15f;
                }
                if (speedY > 15f)
                {
                    speedY = 15f;
                }
                if (speedY < -15f)
                {
                    speedY = -15f;
                }
                if (projectile.owner == Main.myPlayer)
            	{
					Projectile.NewProjectile(vector2.X, vector2.Y, speedX, speedY, mod.ProjectileType("AstrealFlame"), (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                }
				flameTimer = 180;
			}
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;
			Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)(projectile.Center.X / 16), (int)(projectile.Center.Y / 16));
			Texture2D texture2D3 = Main.projectileTexture[projectile.type];
			int num156 = texture2D3.Height / Main.projFrames[projectile.type];
			int y3 = num156 * projectile.frame;
			Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, y3, texture2D3.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			int num157 = 8;
			int num158 = 2;
			int num159 = 1;
			float num160 = 0f;

			int num161 = num159;
			while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)))
			{
				Microsoft.Xna.Framework.Color color26 = color25;
				color26 = projectile.GetAlpha(color26);
				goto IL_6899;
				IL_6881:
				num161 += num158;
				continue;
				IL_6899:
				float num164 = (float)(num157 - num161);
				if (num158 < 0)
				{
					num164 = (float)(num159 - num161);
				}
				color26 *= num164 / ((float)ProjectileID.Sets.TrailCacheLength[projectile.type] * 1.5f);
				Vector2 value4 = (projectile.oldPos[num161]);
				float num165 = projectile.rotation;
				SpriteEffects effects = spriteEffects;
				Main.spriteBatch.Draw(texture2D3, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + projectile.rotation * num160 * (float)(num161 - 1) * projectile.spriteDirection, origin2, projectile.scale, effects, 0f);
				goto IL_6881;
			}
			Main.spriteBatch.Draw(texture2D3, projectile.position + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, projectile.rotation, origin2, projectile.scale, spriteEffects, 0f);
			return false;
		}

		public override void Kill(int timeLeft)
        {
        	for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(BuffID.ShadowFlame, 360);
        }
    }
}
