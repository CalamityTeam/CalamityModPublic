using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class TheGodsGambitProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("God's Gambit");
		}
    	
        public override void SetDefaults()
        {
        	projectile.CloneDefaults(ProjectileID.Kraken);
            projectile.width = 16;
            projectile.scale = 1.15f;
            projectile.height = 16;
            projectile.penetrate = 6;
            projectile.melee = true;
            aiType = 554;
        }
        
        public override void AI()
        {
        	if (Main.rand.Next(8) == 0)
        	{
        		if (projectile.owner == Main.myPlayer)
        		{
            		Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, 406, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        		}
        	}
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
	    {
			target.AddBuff(BuffID.Slimed, 200);
		}
    }
}