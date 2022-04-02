using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
	public class TerraArrowMain : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Ammo/TerraArrow";

		private bool initialized = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
			projectile.extraUpdates = 1;
			projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
		}

        public override void AI()
        {
			if (!initialized)
			{
				projectile.velocity *= 0.5f;
				initialized = true;
			}
			if (projectile.FinalExtraUpdate() && initialized)
			{
				projectile.velocity *= 1.003f;
				if (projectile.velocity.Length() >= 22f)
				{
					projectile.Kill();
				}
			}
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void Kill(int timeLeft)
        {
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 32);
            Main.PlaySound(SoundID.Item60, projectile.Center);
            for (int d = 0; d < 3; d++)
            {
                int terra = Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, 0f, 0f, 100, default, 2f);
                Main.dust[terra].velocity *= 1.2f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[terra].scale = 0.5f;
                    Main.dust[terra].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            if (projectile.owner == Main.myPlayer)
            {
                for (int a = 0; a < 2; a++)
                {
					Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<TerraArrowSplit>(), (int)(projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
