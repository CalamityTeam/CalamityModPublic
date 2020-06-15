using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
	public class TerraOrb2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
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
        }

        public override void AI()
        {
			CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 300f, 6f, 18f, 5, ModContent.ProjectileType<TerraBolt2>());
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
        }
    }
}
