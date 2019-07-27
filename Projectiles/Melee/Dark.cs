using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
	public class Dark : ModProjectile
	{
		private int speedTimer = 120;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dark");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.timeLeft = 300;
		}

		public override void AI()
		{
			projectile.rotation += 0.5f;
			speedTimer--;
			if (speedTimer > 60)
			{
				projectile.velocity.X = 10f;
				projectile.velocity.Y = 0f;
			}
			else if (speedTimer <= 60)
			{
				projectile.velocity.X = -10f;
				projectile.velocity.Y = 0f;
			}
			if (speedTimer <= 0)
			{
				speedTimer = 120;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
			return false;
		}
	}
}
