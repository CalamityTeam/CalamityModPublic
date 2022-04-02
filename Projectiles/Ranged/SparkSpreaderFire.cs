using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class SparkSpreaderFire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 12;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 3;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 60;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.05f / 255f, (255 - projectile.alpha) * 0.45f / 255f, (255 - projectile.alpha) * 0.05f / 255f);

            if (projectile.wet && !projectile.lavaWet)
            {
                projectile.Kill();
            }

            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 35, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 80, default, 0.75f);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 50, default, 0.75f);
            }

            if (projectile.ai[0]++ > 7f)
            {
                float dustScaleSize = 1f;
                if (projectile.ai[0] == 8f)
                {
                    dustScaleSize = 0.25f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    dustScaleSize = 0.5f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    dustScaleSize = 0.75f;
                }
                projectile.ai[0] += 1f;
                int dustType = DustID.Fire;
				for (int i = 0; i < 2; i++)
				{
					int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 10, default, 0.75f);
					Dust dust = Main.dust[fire];
					if (Main.rand.Next(3) == 0)
					{
						dust.noGravity = true;
						dust.scale *= 1.75f;
						dust.velocity.X *= 2f;
						dust.velocity.Y *= 2f;
					}
					else
					{
						dust.noGravity = true;
						dust.scale *= 0.5f;
					}
					dust.velocity.X *= 1.2f;
					dust.velocity.Y *= 1.2f;
					dust.scale *= dustScaleSize;
					dust.velocity += projectile.velocity;
                }
            }

            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(1, 4));
        }

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
            target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(1, 4));
        }
    }
}
