using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class GreenDust : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dust");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.melee = true;
            projectile.timeLeft = 3600;
        }

        public override void AI()
        {
        	projectile.rotation += projectile.velocity.X * 0.02f;
			if (projectile.velocity.X < 0f)
			{
				projectile.rotation -= Math.Abs(projectile.velocity.Y) * 0.02f;
			}
			else
			{
				projectile.rotation += Math.Abs(projectile.velocity.Y) * 0.02f;
			}
			projectile.velocity *= 0.98f;
			projectile.ai[0] += 1f;
			if (projectile.ai[0] >= 60f)
			{
				if (projectile.alpha < 255)
				{
					projectile.alpha += 5;
					if (projectile.alpha > 255)
					{
						projectile.alpha = 255;
					}
				}
				else if (projectile.owner == Main.myPlayer)
				{
					projectile.Kill();
				}
			}
			else if (projectile.alpha > 80)
			{
				projectile.alpha -= 30;
				if (projectile.alpha < 80)
				{
					projectile.alpha = 80;
				}
			}
        }
    }
}
