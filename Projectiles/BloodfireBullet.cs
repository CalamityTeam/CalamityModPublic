using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class BloodfireBullet : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bloodfire Bullet");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ranged = true;
            projectile.extraUpdates = 1;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 2; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 235, projectile.oldVelocity.X * 0.15f, projectile.oldVelocity.Y * 0.15f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	if (target.type == NPCID.TargetDummy)
			{
				return;
			}
        	Player player = Main.player[projectile.owner];
			player.statLife += 1;
    		player.HealEffect(1);
    		target.AddBuff(mod.BuffType("BrimstoneFlames"), 360);
        }
    }
}