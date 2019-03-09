using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Astral
{
    public class RadiantStar : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Radiant Star");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.penetrate = 4;
            projectile.timeLeft = 300;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}
        
        public override void AI()
        {
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
        	if(projectile.spriteDirection == -1)
        	{
        		projectile.rotation -= 1.57f;
        	}
        	projectile.ai[1] += 1f;
        	if (projectile.ai[1] == 25f)
        	{
        		int numProj = 2;
	            float rotation = MathHelper.ToRadians(50);
	            if (projectile.owner == Main.myPlayer)
	            {
		            for (int i = 0; i < numProj + 1; i++)
		            {
		                Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
						while (speed.X == 0f && speed.Y == 0f)
						{
							speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
						}
						speed.Normalize();
						speed *= ((float)Main.rand.Next(30, 61) * 0.1f) * 2.5f;
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, mod.ProjectileType("RadiantStar2"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
					}
					Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("RadiantExplosion"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
					projectile.active = false;
					return;
				}
        	}
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        
        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 27);
        	for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, mod.DustType("AstralBlue"), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}