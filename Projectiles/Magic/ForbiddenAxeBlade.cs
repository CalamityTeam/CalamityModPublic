using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ForbiddenAxeBlade : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blade");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 300;
            projectile.magic = true;
        }

        public override void AI()
        {
        	projectile.alpha -= 3;
        	projectile.rotation += 0.75f;
        	projectile.ai[1] += 1f;
        	if (projectile.ai[1] <= 20f)
        	{
        		projectile.velocity.X *= 0.85f;
        		projectile.velocity.Y *= 0.85f;
        	}
            else if (projectile.ai[1] > 20f && projectile.ai[1] <= 39f)
        	{
            	projectile.velocity.X *= 1.25f;
        		projectile.velocity.Y *= 1.25f;
        	}
            else if (projectile.ai[1] == 40f)
            {
            	projectile.ai[1] = 0f;
            }
            if (Main.rand.Next(4) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 159, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 159, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}