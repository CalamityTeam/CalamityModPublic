using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class RavagerFlame : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flame");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.aiStyle = 1;
        }

        public override void AI()
        {
            for (int num468 = 0; num468 < 4; num468++)
            {
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 135, 0f, 0f, 100, default(Color), 2f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
            }
        }
        
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
        	target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}