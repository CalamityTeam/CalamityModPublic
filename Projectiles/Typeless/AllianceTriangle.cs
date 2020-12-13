using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
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
			if (projectile.ai[0] <= 30f)
			{
				projectile.alpha -= 75;
				if (projectile.alpha < 0)
					projectile.alpha = 0;
			}
			else if (projectile.ai[0] > 120f)
				projectile.alpha += 50;

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
