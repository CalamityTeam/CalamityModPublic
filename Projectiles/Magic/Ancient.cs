using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Ancient : ModProjectile
    {
    	public int addDustTimer = 0;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ancient");
		}

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.penetrate = 6;
            projectile.extraUpdates = 6;
            projectile.timeLeft = 151;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 2;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, 0.6f, 0.5f, 0f);
        	addDustTimer++;
        	if (addDustTimer >= 30)
        	{
	        	int numProj = 2;
	        	int randomSpread = Main.rand.Next(3, 19);
	            float rotation = MathHelper.ToRadians(randomSpread);
	            if (projectile.owner == Main.myPlayer)
	            {
		            for (int i = 0; i < numProj + 1; i++)
		            {
		                Vector2 perturbedSpeed = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
		                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("Ancient2"), (int)((double)projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
		            }
	            }
	            addDustTimer = 0;
        	}
			if (projectile.timeLeft > 151)
			{
				projectile.timeLeft = 151;
			}
			if (projectile.ai[0] > 4f)
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
					int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num299].noGravity = true;
						Main.dust[num299].scale *= 2f;
						Dust expr_DBEF_cp_0 = Main.dust[num299];
						expr_DBEF_cp_0.velocity.X = expr_DBEF_cp_0.velocity.X * 6f;
						Dust expr_DC0F_cp_0 = Main.dust[num299];
						expr_DC0F_cp_0.velocity.Y = expr_DC0F_cp_0.velocity.Y * 6f;
					}
					else
					{
						Main.dust[num299].scale *= 1.5f;
					}
					Dust expr_DC74_cp_0 = Main.dust[num299];
					expr_DC74_cp_0.velocity.X = expr_DC74_cp_0.velocity.X * 3f;
					Dust expr_DC94_cp_0 = Main.dust[num299];
					expr_DC94_cp_0.velocity.Y = expr_DC94_cp_0.velocity.Y * 3f;
					Main.dust[num299].scale *= num296;
				}
				for (int num298 = 0; num298 < 2; num298++)
				{
					int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
					if (Main.rand.Next(3) == 0)
					{
						Main.dust[num299].noGravity = true;
						Main.dust[num299].scale *= 4f;
						Dust expr_DBEF_cp_0 = Main.dust[num299];
						expr_DBEF_cp_0.velocity.X = expr_DBEF_cp_0.velocity.X * 4f;
						Dust expr_DC0F_cp_0 = Main.dust[num299];
						expr_DC0F_cp_0.velocity.Y = expr_DC0F_cp_0.velocity.Y * 4f;
					}
					else
					{
						Main.dust[num299].scale *= 2.5f;
					}
					Dust expr_DC74_cp_0 = Main.dust[num299];
					expr_DC74_cp_0.velocity.X = expr_DC74_cp_0.velocity.X * 2f;
					Dust expr_DC94_cp_0 = Main.dust[num299];
					expr_DC94_cp_0.velocity.Y = expr_DC94_cp_0.velocity.Y * 2f;
					Main.dust[num299].scale *= num296;
				}
			}
			else
			{
				projectile.ai[0] += 1f;
			}
			projectile.rotation += 0.3f * (float)projectile.direction;
        }
    }
}
