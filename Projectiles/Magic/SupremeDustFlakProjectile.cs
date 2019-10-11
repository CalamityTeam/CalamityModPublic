using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SupremeDustFlakProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dust");
		}

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.penetrate = 2;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 100;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.35f) / 255f, ((255 - projectile.alpha) * 0.3f) / 255f, ((255 - projectile.alpha) * 0.01f) / 255f);
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
				int num297 = 32;
				for (int num298 = 0; num298 < 2; num298++)
				{
					int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
					if (Main.rand.NextBool(3))
					{
						Main.dust[num299].noGravity = false;
						Main.dust[num299].scale *= 1f;
						Dust expr_DBEF_cp_0 = Main.dust[num299];
						expr_DBEF_cp_0.velocity.X *= 2f;
						Dust expr_DC0F_cp_0 = Main.dust[num299];
						expr_DC0F_cp_0.velocity.Y *= 2f;
					}
					else
					{
						Main.dust[num299].scale *= 0.5f;
					}
					Dust expr_DC74_cp_0 = Main.dust[num299];
					expr_DC74_cp_0.velocity.X *= 1.2f;
					Dust expr_DC94_cp_0 = Main.dust[num299];
					expr_DC94_cp_0.velocity.Y *= 1.2f;
					Main.dust[num299].scale *= num296;
				}
				for (int num298 = 0; num298 < 2; num298++)
				{
					int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
					if (Main.rand.NextBool(3))
					{
						Main.dust[num299].noGravity = false;
						Main.dust[num299].scale *= 1.5f;
						Dust expr_DBEF_cp_0 = Main.dust[num299];
						expr_DBEF_cp_0.velocity.X *= 2f;
						Dust expr_DC0F_cp_0 = Main.dust[num299];
						expr_DC0F_cp_0.velocity.Y *= 2f;
					}
					else
					{
						Main.dust[num299].scale *= 0.25f;
					}
					Dust expr_DC74_cp_0 = Main.dust[num299];
					expr_DC74_cp_0.velocity.X *= 1.2f;
					Dust expr_DC94_cp_0 = Main.dust[num299];
					expr_DC94_cp_0.velocity.Y *= 1.2f;
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
        	target.immune[projectile.owner] = 6;
        }
    }
}
