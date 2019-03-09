using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class Crystalline : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystalline");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 113;
            projectile.timeLeft = 120;
            aiType = 598;
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
        	if (projectile.ai[1] == 30f)
        	{
        		int numProj = 2;
	            float rotation = MathHelper.ToRadians(50);
	            if (projectile.owner == Main.myPlayer)
	            {
		            for (int i = 0; i < numProj + 1; i++)
		            {
		                Vector2 perturbedSpeed = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
		                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("Crystalline2"), (int)((double)projectile.damage * 0.5f), projectile.knockBack, projectile.owner, 0f, 0f);
		            }
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
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 154, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}