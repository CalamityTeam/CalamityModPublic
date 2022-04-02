using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
	public class VividLaser2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vivid Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.alpha = 255;
			projectile.ignoreWater = true;
			projectile.timeLeft = 120;
            projectile.penetrate = 1;
            projectile.magic = true;
        }

        public override void AI()
        {
            Vector2 value7 = new Vector2(5f, 10f);
            projectile.ai[1] += 1f;
            for (int dust = 0; dust < 2; dust++)
            {
				Vector2 value8 = Vector2.UnitX * -12f;
				value8 = -Vector2.UnitY.RotatedBy((double)(projectile.ai[1] * 0.1308997f + (float)dust * 3.14159274f), default) * value7 - projectile.rotation.ToRotationVector2() * 10f;
				int num42 = Dust.NewDust(projectile.Center, 0, 0, 66, 0f, 0f, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
				Main.dust[num42].scale = 0.75f;
				Main.dust[num42].noGravity = true;
				Main.dust[num42].position = projectile.Center + value8;
				Main.dust[num42].velocity = projectile.velocity;
            }

			if (projectile.timeLeft < 110)
				projectile.ai[0] = 1f;

			if (projectile.ai[0] >= 1f)
				CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 400f, 12f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
					107,
					234,
					269
                });
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			target.ExoDebuffs();
        }

		// Cannot deal damage for the first several frames of existence.
        public override bool? CanHitNPC(NPC target)
		{
			if (projectile.timeLeft >= 110)
			{
				return false;
			}
			return null;
		}

		public override bool CanHitPvp(Player target) => projectile.timeLeft < 110;
    }
}
