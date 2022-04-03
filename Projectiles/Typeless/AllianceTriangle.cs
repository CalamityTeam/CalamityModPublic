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
            Projectile.width = 154;
            Projectile.height = 134;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 254;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, new Vector3(240, 185, 7) * (3f / 255));
            Projectile.Center = player.Center;

            Projectile.ai[0]++;
            if (Projectile.ai[0] <= 5f)
            {
                Projectile.alpha -= 75;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            else
            {
                Projectile.scale *= 1.06f;
                Projectile.alpha += 10;
            }

            if (Projectile.alpha >= 255 || player is null || player.dead)
            {
                Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.alpha <= 0)
                return new Color(200, 200, 200, 200);
            return null;
        }

        public override bool CanDamage() => false;
    }
}
