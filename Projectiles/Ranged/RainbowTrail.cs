using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class RainbowTrail : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/RainbowFront";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 46;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.ranged = true;
            projectile.alpha = 255;
            projectile.light = 0.3f;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1.25f;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
    }
}
