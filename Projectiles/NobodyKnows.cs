using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class NobodyKnows : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nope");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 1;
        }
    }
}