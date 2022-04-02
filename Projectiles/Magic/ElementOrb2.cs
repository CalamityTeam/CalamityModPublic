using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class ElementOrb2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 30;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 300f, 6f, 24f, 5, ModContent.ProjectileType<ElementBolt2>());
        }
    }
}
