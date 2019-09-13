using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class HolyFireBullet : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bullet");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.extraUpdates = 5;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ranged = true;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.65f) / 255f, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
        	projectile.localAI[0] += 1f;
        	if (projectile.localAI[0] >= 4f)
        	{
	        	for (int num92 = 0; num92 < 2; num92++)
				{
					float num93 = projectile.velocity.X / 3f * (float)num92;
					float num94 = projectile.velocity.Y / 3f * (float)num92;
					int num95 = 4;
					int num96 = Dust.NewDust(new Vector2(projectile.position.X + (float)num95, projectile.position.Y + (float)num95), projectile.width - num95 * 2, projectile.height - num95 * 2, 244, 0f, 0f, 100, default, 2f);
					Main.dust[num96].noGravity = true;
					Main.dust[num96].velocity *= 0.1f;
					Main.dust[num96].velocity += projectile.velocity * 0.1f;
					Dust expr_47FA_cp_0 = Main.dust[num96];
					expr_47FA_cp_0.position.X = expr_47FA_cp_0.position.X - num93;
					Dust expr_4815_cp_0 = Main.dust[num96];
					expr_4815_cp_0.position.Y = expr_4815_cp_0.position.Y - num94;
				}
				if (Main.rand.NextBool(10))
				{
					int num97 = 4;
					int num98 = Dust.NewDust(new Vector2(projectile.position.X + (float)num97, projectile.position.Y + (float)num97), projectile.width - num97 * 2, projectile.height - num97 * 2, 244, 0f, 0f, 100, default, 0.6f);
					Main.dust[num98].velocity *= 0.25f;
					Main.dust[num98].velocity += projectile.velocity * 0.5f;
				}
        	}
        }

        public override void Kill(int timeLeft)
        {
        	if (projectile.owner == Main.myPlayer)
        	{
        		int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, 612, (int)((double)projectile.damage * 0.85), projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
				Main.projectile[proj].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceRanged = true;
			}
            for (int k = 0; k < 10; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("HolyLight"), 300);
        }
    }
}
