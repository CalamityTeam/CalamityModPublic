using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneLaserSplit : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laser");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.hostile = true;
            projectile.scale = 2f;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.alpha = 120;
        }

        public override void AI()
        {
        	int num103 = (int)Player.FindClosest(projectile.Center, 1, 1);
			projectile.ai[1] += 1f;
			if (projectile.ai[1] < 210f && projectile.ai[1] > 90f)
			{
				float scaleFactor2 = projectile.velocity.Length();
				Vector2 vector11 = Main.player[num103].Center - projectile.Center;
				vector11.Normalize();
				vector11 *= scaleFactor2;
				projectile.velocity = (projectile.velocity * 24f + vector11) / 25f;
				projectile.velocity.Normalize();
				projectile.velocity *= scaleFactor2;
			}
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 50, 50, projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
        }
    }
}