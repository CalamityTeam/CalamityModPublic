using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class PlasmaBolt : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bolt");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 200;
            projectile.light = 0.05f;
        }

        public override void AI()
        {
			for (int num121 = 0; num121 < 5; num121++)
			{
				Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 1f)];
				dust.velocity = Vector2.Zero;
				dust.position -= projectile.velocity / 5f * (float)num121;
				dust.noGravity = true;
				dust.scale = 0.5f;
				dust.noLight = true;
			}
        }
    }
}