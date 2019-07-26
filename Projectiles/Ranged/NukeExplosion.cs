using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class NukeExplosion : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Explosion");
		}

        public override void SetDefaults()
        {
            projectile.width = 200;
            projectile.height = 200;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
			projectile.ranged = true;
			projectile.penetrate = -1;
            projectile.timeLeft = 5;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 2;
        }
    }
}
