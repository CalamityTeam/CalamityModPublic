using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class AortaProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aorta");
		}
    	
        public override void SetDefaults()
        {
        	projectile.CloneDefaults(ProjectileID.Valor);
            projectile.width = 16;
            projectile.height = 16;
            projectile.penetrate = 7;
            projectile.extraUpdates = 1;
            projectile.melee = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            aiType = ProjectileID.Valor;
        }
    }
}