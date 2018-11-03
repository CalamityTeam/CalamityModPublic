using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class CalamityAuraType2 : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aura");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 80;
            projectile.melee = true;
        }

        public override void AI()
        {
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 56, 0f, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
    		target.AddBuff(BuffID.Frostburn, 800);
        }
    }
}