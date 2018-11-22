using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class LaceratorProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lacerator");
		}
    	
        public override void SetDefaults()
        {
        	projectile.CloneDefaults(ProjectileID.TheEyeOfCthulhu);
            projectile.width = 16;
            projectile.scale = 1.1f;
            projectile.height = 16;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            aiType = 555;
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 5;
        }
        
        public override void AI()
        {
        	if (projectile.owner == Main.myPlayer)
        	{
            	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, 476, (int)((double)projectile.damage * 0.66f), projectile.knockBack, projectile.owner, 0f, 0f);
        	}
        }
    }
}