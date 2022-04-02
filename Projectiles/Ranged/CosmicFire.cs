using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class CosmicFire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 100;
        }

        public override void AI()
        {
            if (projectile.scale <= 1.5f)
            {
                projectile.scale *= 1.01f;
            }
            Lighting.AddLight(projectile.Center, 0.25f, 0f, 0.1f);
            if (projectile.ai[0] > 7f)
            {
                float scalar = 1f;
                if (projectile.ai[0] == 8f)
                {
                    scalar = 0.25f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    scalar = 0.5f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    scalar = 0.75f;
                }
                projectile.ai[0] += 1f;
                int dustType = Main.rand.NextBool(4) ? 61 : 62;
                if (Main.rand.NextBool(2))
                {
					int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
					Dust dust = Main.dust[fire];
					if (Main.rand.NextBool(3))
					{
						dust.noGravity = true;
						dust.scale *= 3f;
						dust.velocity.X *= 2f;
						dust.velocity.Y *= 2f;
					}
					else
					{
						dust.scale *= 1.5f;
					}
					dust.velocity.X *= 1.2f;
					dust.velocity.Y *= 1.2f;
					dust.scale *= scalar;
					dust.velocity += projectile.velocity;
					if (!dust.noGravity)
					{
						dust.velocity *= 0.5f;
					}
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
            target.immune[projectile.owner] = 3;
        }
    }
}
