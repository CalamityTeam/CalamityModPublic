using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Amidias
{
	public class SmallCoral : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Small Coral");
		}

		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 22;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.penetrate = 1;
			projectile.aiStyle = 1;
		}

		public override void AI()
		{
			projectile.velocity.X *= 0.9995f;
			projectile.velocity.Y = projectile.velocity.Y + 0.01f;
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
			projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = 16;
			projectile.height = 22;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num621 = 0; num621 < 5; num621++)
			{
				int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 51, projectile.oldVelocity.X / 4, projectile.oldVelocity.Y / 4, 0, new Color(234, 183, 100), 1f);
				Main.dust[num195].noGravity = true;
				Main.dust[num195].velocity *= 2f;
			}
		}
	}
}