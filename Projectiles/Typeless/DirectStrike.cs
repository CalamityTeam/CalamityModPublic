using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class DirectStrike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nondescript Damaging Entity");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.penetrate = 1;
            projectile.extraUpdates = 0;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.timeLeft = 2;
        }
    }
}
