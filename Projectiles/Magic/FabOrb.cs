using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
	public class FabOrb : ModProjectile
    {
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
            projectile.timeLeft = 300;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
			CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 300f, 6f, 24f, 5, ModContent.ProjectileType<FabBolt>());
        }
    }
}
