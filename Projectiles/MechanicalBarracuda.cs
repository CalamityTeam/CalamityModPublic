using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class MechanicalBarracuda : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Barracuda");
			Main.projFrames[projectile.type] = 4;
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.aiStyle = 39;
            aiType = 190;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.ranged = true;
        }
    }
}