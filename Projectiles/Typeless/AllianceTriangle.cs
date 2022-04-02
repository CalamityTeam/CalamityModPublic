using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class AllianceTriangle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Alliance Triangle");
        }

        public override void SetDefaults()
        {
            projectile.width = 154;
            projectile.height = 134;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
			projectile.alpha = 254;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Lighting.AddLight(projectile.Center, new Vector3(240, 185, 7) * (3f / 255));
            projectile.Center = player.Center;

			projectile.ai[0]++;
			if (projectile.ai[0] <= 5f)
			{
				projectile.alpha -= 75;
				if (projectile.alpha < 0)
					projectile.alpha = 0;
			}
			else
			{
				projectile.scale *= 1.06f;
				projectile.alpha += 10;
			}

			if (projectile.alpha >= 255 || player is null || player.dead)
			{
				projectile.Kill();
			}
        }

		public override Color? GetAlpha(Color lightColor)
		{
			if (projectile.alpha <= 0)
				return new Color(200, 200, 200, 200);
			return null;
		}

		public override bool CanDamage() => false;
    }
}
