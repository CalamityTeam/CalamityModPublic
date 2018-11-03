using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class AzathothProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Azathoth");
		}
    	
        public override void SetDefaults()
        {
        	projectile.CloneDefaults(ProjectileID.Kraken);
            projectile.width = 16;
            projectile.scale = 1.2f;
            projectile.height = 16;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            aiType = 554;
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 1;
        }
        
        public override void AI()
        {
            if (Main.rand.Next(3) == 0)
        	{
            	if (projectile.owner == Main.myPlayer)
            	{
            		Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.35f, projectile.velocity.Y * 0.35f, mod.ProjectileType("CosmicOrb"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            	}
        	}
        }
    }
}