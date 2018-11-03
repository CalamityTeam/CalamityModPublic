using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class Brimblade2 : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimblade");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.thrown = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 300;
            aiType = 52;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 100);
            for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 235, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}