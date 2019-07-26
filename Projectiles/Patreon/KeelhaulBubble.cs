using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Patreon
{
    public class KeelhaulBubble : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bubble");
		}

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
        }

        public override void Kill(int timeLeft)
        {
        	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 80f, 0f, 0f, mod.ProjectileType("KeelhaulGeyserBottom"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
			Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 80f, 0f, 0f, mod.ProjectileType("KeelhaulGeyserTop"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        }
    }
}
