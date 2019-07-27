using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class Pwnagehammer : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hammer");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 120;
            projectile.extraUpdates = 3;
            aiType = 301;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}

        public override void AI()
        {
        	projectile.velocity.X *= 1.06f;
        	projectile.velocity.Y *= 1.06f;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.ai[0] += 0.1f;
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
    }
}
