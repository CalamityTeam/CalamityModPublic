using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class ProBolt : ModProjectile
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
            projectile.ranged = true;
            projectile.penetrate = 3;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
			for (int num134 = 0; num134 < 10; num134++)
			{
				float x = projectile.position.X - projectile.velocity.X / 10f * (float)num134;
				float y = projectile.position.Y - projectile.velocity.Y / 10f * (float)num134;
				int num135 = Dust.NewDust(new Vector2(x, y), 1, 1, 160, 0f, 0f, 0, default(Color), 1.25f);
				Main.dust[num135].alpha = projectile.alpha;
				Main.dust[num135].position.X = x;
				Main.dust[num135].position.Y = y;
				Main.dust[num135].velocity *= 0f;
				Main.dust[num135].noGravity = true;
			}
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                projectile.ai[0] += 0.1f;
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 10; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 160, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f); //206 160 226
            }
        }
    }
}