using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class BrimstoneLaserFriendly : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laser");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 2;
            projectile.aiStyle = 1;
            aiType = 100;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.scale = 2f;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.alpha = 120;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f);
        	projectile.velocity.X *= 1.05f;
        	projectile.velocity.Y *= 1.05f;
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            projectile.velocity.Y += projectile.ai[0];
            if (Main.rand.Next(2) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 235, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 235, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 60);
        }
    }
}