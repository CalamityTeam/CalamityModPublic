using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
	public class HellionSpike : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spike");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.aiStyle = 27;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.penetrate = 3;
			projectile.timeLeft = 600;
			aiType = ProjectileID.SporeCloud;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, 0f, 0.25f, 0f);
			if (Main.rand.NextBool(3))
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 44, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 44, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (projectile.timeLeft > 595)
				return false;

			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[projectile.owner] = 8;
			target.AddBuff(BuffID.Venom, 300);
			if (crit)
			{
				float xPos = projectile.Center.X + 800 * Main.rand.NextBool(2).ToDirectionInt();
				float yPos = projectile.Center.Y + Main.rand.Next(-800, 801);
				Vector2 spawnPosition = new Vector2(xPos, yPos);
				Vector2 velocity = target.Center - spawnPosition;
				float dir = 10 / spawnPosition.X;
				velocity.X *= dir * 150;
				velocity.Y *= dir * 150;
				velocity.X = MathHelper.Clamp(velocity.X, -15f, 15f);
				velocity.Y = MathHelper.Clamp(velocity.Y, -15f, 15f);
				if (projectile.owner == Main.myPlayer)
				{
					int petal = Projectile.NewProjectile(spawnPosition, velocity, ProjectileID.FlowerPetal, (int)(projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner);
					Main.projectile[petal].Calamity().forceMelee = true;
					Main.projectile[petal].localNPCHitCooldown = -1;
				}
			}
		}
	}
}
