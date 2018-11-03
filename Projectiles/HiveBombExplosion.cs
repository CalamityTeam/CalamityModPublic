using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class HiveBombExplosion : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Explosion");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 5;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("Plague"), 600);
        }
    }
}