using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ForbiddenAxeBlade : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blade");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 26;
			projectile.height = 26;
			projectile.friendly = true;
			projectile.penetrate = 1;
			projectile.alpha = 255;
			projectile.timeLeft = 300;
			projectile.magic = true;
		}

		public override void AI()
		{
			projectile.alpha -= 3;
			projectile.rotation += 0.75f;
			projectile.ai[1] += 1f;
			if (projectile.ai[1] <= 20f)
			{
				projectile.velocity.X *= 0.85f;
				projectile.velocity.Y *= 0.85f;
			}
			else if (projectile.ai[1] > 20f && projectile.ai[1] <= 39f)
			{
				projectile.velocity.X *= 1.25f;
				projectile.velocity.Y *= 1.25f;
				float num472 = projectile.Center.X;
				float num473 = projectile.Center.Y;
				float num474 = 300f;
				bool flag17 = false;
				for (int num475 = 0; num475 < 200; num475++)
				{
					if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
					{
						float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
						float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
						float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
						if (num478 < num474)
						{
							num474 = num478;
							num472 = num476;
							num473 = num477;
							flag17 = true;
						}
					}
				}
				if (flag17)
				{
					float num483 = 10f;
					Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
					float num484 = num472 - vector35.X;
					float num485 = num473 - vector35.Y;
					float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
					num486 = num483 / num486;
					num484 *= num486;
					num485 *= num486;
					projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
					projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
				}
			}
			else if (projectile.ai[1] == 40f)
			{
				projectile.ai[1] = 0f;
			}
			if (Main.rand.NextBool(8))
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 159, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f);
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 159, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}
	}
}
