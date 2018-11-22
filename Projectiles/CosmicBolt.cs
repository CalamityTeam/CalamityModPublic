using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class CosmicBolt : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bolt");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.extraUpdates = 100;
            projectile.friendly = true;
            projectile.timeLeft = 180;
            projectile.melee = true;
        }

        public override void AI()
        {
        	projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 4f)
			{
				for (int num441 = 0; num441 < 2; num441++)
				{
					Vector2 vector30 = projectile.position;
					vector30 -= projectile.velocity * ((float)num441 * 0.25f);
					projectile.alpha = 255;
					int num442 = Dust.NewDust(vector30, 1, 1, 107, 0f, 0f, 0, default(Color), 1.3f);
					Main.dust[num442].position = vector30;
					Dust expr_13A3E_cp_0 = Main.dust[num442];
					expr_13A3E_cp_0.position.X = expr_13A3E_cp_0.position.X + (float)(projectile.width / 2);
					Dust expr_13A62_cp_0 = Main.dust[num442];
					expr_13A62_cp_0.position.Y = expr_13A62_cp_0.position.Y + (float)(projectile.height / 2);
					Main.dust[num442].scale = (float)Main.rand.Next(70, 110) * 0.007f;
					Main.dust[num442].velocity *= 0.2f;
				}
				return;
			}
        }
    }
}