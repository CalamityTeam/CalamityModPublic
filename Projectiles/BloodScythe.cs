using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class BloodScythe : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood Scythe");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.scale = 0.7f;
            projectile.tileCollide = true;
            projectile.penetrate = 1;
        }
        
        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.35f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f, ((255 - projectile.alpha) * 0.075f) / 255f);
            if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 16f && projectile.timeLeft < 420)
            {
                projectile.velocity *= 1.05f;
            }
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.1f;
            if (Main.rand.Next(5) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 5, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}