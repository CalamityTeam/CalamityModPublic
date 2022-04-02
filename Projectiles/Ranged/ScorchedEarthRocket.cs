using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
	public class ScorchedEarthRocket : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rocket");
			Main.projFrames[projectile.type] = 10;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 22;
			projectile.height = 22;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 300;
			projectile.ranged = true;
			projectile.tileCollide = false;
			projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			//Lighting
            Lighting.AddLight(projectile.Center, 1f, 0.79f, 0.3f);

			//Animation
			projectile.frameCounter++;
			if (projectile.frameCounter > (projectile.frame == 1 ? 10 : 7))
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame >= Main.projFrames[projectile.type])
			{
				projectile.frame = 6;
			}

			CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 400f, 5f, 30f);

			//Rotation
			projectile.rotation = projectile.velocity.ToRotation();

			float xVel = projectile.velocity.X * 0.5f;
			float yVel = projectile.velocity.Y * 0.5f;
			int d = Dust.NewDust(new Vector2(projectile.position.X + 3f + xVel, projectile.position.Y + 3f + yVel) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 244, 0f, 0f, 100, default, 1f);
			Main.dust[d].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
			Main.dust[d].velocity *= 0.2f;
			Main.dust[d].noGravity = true;
			d = Dust.NewDust(new Vector2(projectile.position.X + 3f + xVel, projectile.position.Y + 3f + yVel) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 244, 0f, 0f, 100, default, 0.5f);
			Main.dust[d].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
			Main.dust[d].velocity *= 0.05f;
		}

		public override void Kill(int timeLeft)
		{
			if (projectile.owner == Main.myPlayer)
			{
				CalamityGlobalProjectile.ExpandHitboxBy(projectile, 300);
				Main.PlaySound(SoundID.Item14, projectile.position);
				CalamityUtils.ExplosionGores(projectile.Center, 10);

				Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ScorchedEarthBlast>(), projectile.damage, projectile.knockBack * 2f, projectile.owner);
				for (int j = 0; j < 5; j++)
				{
					Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(8f, 10f);
					Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<ScorchedEarthClusterBomb>(), (int)(projectile.damage * 0.25), projectile.knockBack * 0.25f, projectile.owner);
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
			return false;
		}
	}
}
