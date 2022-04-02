using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class VividOrb : ModProjectile
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
            projectile.timeLeft = 60;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 300f, 6f, 24f, 5, ModContent.ProjectileType<VividBolt>(), 1D, true);
        }
    }
}
