using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SandBlast : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sand");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 0f)
            {
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                projectile.ai[1] = 1f;
                Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 21);
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                projectile.alpha -= 50;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default(Color), 1f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
        	for (int dust = 0; dust <= 10; dust++)
        	{
        		float num463 = (float)Main.rand.Next(-10, 11);
				float num464 = (float)Main.rand.Next(-10, 11);
				float num465 = (float)Main.rand.Next(3, 9);
				float num466 = (float)Math.Sqrt((double)(num463 * num463 + num464 * num464));
				num466 = num465 / num466;
				num463 *= num466;
				num464 *= num466;
        		int num467 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default(Color), 1.2f);
        		Main.dust[num467].noGravity = true;
				Main.dust[num467].position.X = projectile.Center.X;
				Main.dust[num467].position.Y = projectile.Center.Y;
				Dust expr_149DF_cp_0 = Main.dust[num467];
				expr_149DF_cp_0.position.X = expr_149DF_cp_0.position.X + (float)Main.rand.Next(-10, 11);
				Dust expr_14A09_cp_0 = Main.dust[num467];
				expr_14A09_cp_0.position.Y = expr_14A09_cp_0.position.Y + (float)Main.rand.Next(-10, 11);
				Main.dust[num467].velocity.X = num463;
				Main.dust[num467].velocity.Y = num464;
        	}
        }
    }
}