using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class SlashRound : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Round");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.light = 0.5f;
            projectile.alpha = 255;
			projectile.extraUpdates = 7;
			projectile.scale = 1.18f;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = 242;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	if (Main.rand.Next(10) == 0)
        	{
        		target.AddBuff(mod.BuffType("Shred"), 360);
        	}
        }
    }
}