using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class PlagueFang : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fang");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.alpha = 255;
            projectile.penetrate = 9;
            projectile.aiStyle = 1;
            aiType = 355;
        }
        
        public override void AI()
        {
            if (Main.rand.Next(2) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 163, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(0, (int)projectile.position.X, (int)projectile.position.Y, 27);
			for (int num301 = 0; num301 < 15; num301++)
			{
				int num302 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 163, 0f, 0f, 100, default(Color), 1.2f);
				Main.dust[num302].noGravity = true;
				Main.dust[num302].velocity *= 1.2f;
				Main.dust[num302].velocity -= projectile.oldVelocity * 0.3f;
			}
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
	    {
	    	target.AddBuff(mod.BuffType("Plague"), 300);
	    	target.immune[projectile.owner] = 2;
		}
    }
}
