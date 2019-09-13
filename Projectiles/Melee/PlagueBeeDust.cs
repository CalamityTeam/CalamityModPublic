using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PlagueBeeDust : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dust");
		}

		public override void SetDefaults()
		{
			projectile.width = 70;
			projectile.height = 70;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.melee = true;
			projectile.penetrate = -1;
			projectile.extraUpdates = 3;
			projectile.timeLeft = 60;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, 0.05f, 0.4f, 0f);
			if (projectile.ai[0] > 7f)
			{
				float num296 = 1f;
				if (projectile.ai[0] == 8f)
				{
					num296 = 0.25f;
				}
				else if (projectile.ai[0] == 9f)
				{
					num296 = 0.5f;
				}
				else if (projectile.ai[0] == 10f)
				{
					num296 = 0.75f;
				}
				projectile.ai[0] += 1f;
				if (projectile.ai[0] % 2f == 0f)
				{
					int spawnX = (int)(projectile.width / 2);
					int spawnY = (int)(projectile.width / 2);
					int bee = Projectile.NewProjectile(projectile.Center.X + (float)Main.rand.Next(-spawnX, spawnX), projectile.Center.Y + (float)Main.rand.Next(-spawnY, spawnY),
						projectile.velocity.X, projectile.velocity.Y, Main.player[projectile.owner].beeType(),
						Main.player[projectile.owner].beeDamage(projectile.damage / 3), Main.player[projectile.owner].beeKB(0f), projectile.owner, 0f, 0f);
					Main.projectile[bee].penetrate = 1;
				}
				int num297 = 89;
				for (int num298 = 0; num298 < 2; num298++)
				{
					int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
					if (Main.rand.NextBool(3))
					{
						Main.dust[num299].noGravity = true;
						Main.dust[num299].scale *= 2.5f;
						Dust expr_DBEF_cp_0 = Main.dust[num299];
						expr_DBEF_cp_0.velocity.X = expr_DBEF_cp_0.velocity.X * 2f;
						Dust expr_DC0F_cp_0 = Main.dust[num299];
						expr_DC0F_cp_0.velocity.Y = expr_DC0F_cp_0.velocity.Y * 2f;
					}
					else
					{
						Main.dust[num299].scale *= 2f;
					}
					Dust expr_DC74_cp_0 = Main.dust[num299];
					expr_DC74_cp_0.velocity.X = expr_DC74_cp_0.velocity.X * 1.2f;
					Dust expr_DC94_cp_0 = Main.dust[num299];
					expr_DC94_cp_0.velocity.Y = expr_DC94_cp_0.velocity.Y * 1.2f;
					Main.dust[num299].scale *= num296;
				}
			}
			else
			{
				projectile.ai[0] += 1f;
			}
			projectile.rotation += 0.3f * (float)projectile.direction;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[projectile.owner] = 5;
			target.AddBuff(mod.BuffType("Plague"), 180);
		}
	}
}
