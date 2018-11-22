using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class CnidarianProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cnidarian");
		}
    	
        public override void SetDefaults()
        {
        	projectile.CloneDefaults(ProjectileID.CorruptYoyo);
            projectile.width = 16;
            projectile.scale = 1.15f;
            projectile.height = 16;
            projectile.penetrate = 6;
            projectile.melee = true;
            aiType = 542;
        }
    }
}