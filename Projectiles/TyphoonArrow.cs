using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class TyphoonArrow : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Arrow");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 1;
            projectile.timeLeft = 600;
            aiType = 1;
        }

        public override void Kill(int timeLeft)
        {
        	if (projectile.owner == Main.myPlayer)
        	{
        		Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("TyphoonBubble"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        	}
        }
    }
}