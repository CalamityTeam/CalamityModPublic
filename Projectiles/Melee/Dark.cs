using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class Dark : ModProjectile
    {
    	public int speedTimer = 120;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dark");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
        	projectile.rotation += 0.5f;
        	speedTimer--;
        	if (speedTimer > 60)
        	{
        		projectile.velocity.X = 10f;
        		projectile.velocity.Y = 0f;
        	}
        	else if (speedTimer <= 60)
        	{
        		projectile.velocity.X = -10f;
        		projectile.velocity.Y = 0f;
        	}
        	if (speedTimer <= 0)
        	{
        		speedTimer = 120;
        	}
        }
    }
}