using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class ShadecrystalProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystal");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.alpha = 50;
            projectile.scale = 1.2f;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.magic = true;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.15f) / 255f, ((255 - projectile.alpha) * 0.01f) / 255f, ((255 - projectile.alpha) * 0.15f) / 255f);
			projectile.rotation += projectile.velocity.X * 0.2f;
			projectile.ai[1] += 1f;
			if (Main.rand.Next(4) == 0)
			{
				int num300 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 70, 0f, 0f, 0, default(Color), 1f);
				Main.dust[num300].noGravity = true;
				Main.dust[num300].velocity *= 0.5f;
				Main.dust[num300].scale *= 0.9f;
			}
			projectile.velocity *= 0.985f;
			if (projectile.ai[1] > 130f)
			{
				projectile.scale -= 0.05f;
				if ((double)projectile.scale <= 0.2)
				{
					projectile.scale = 0.2f;
					projectile.Kill();
					return;
				}
			}
        }
        
        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 70, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(BuffID.Frostburn, 100);
        }
    }
}