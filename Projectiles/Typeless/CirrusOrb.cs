using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
	public class CirrusOrb : ModProjectile
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
            projectile.timeLeft = 30;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
			projectile.npcProj = true;
		}

        public override void AI()
        {
			CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 150f, 6f, 6f, 2, ModContent.ProjectileType<CirrusBolt>());
        }
    }
}
