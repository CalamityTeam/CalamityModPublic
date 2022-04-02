using CalamityMod.Projectiles.Typeless;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SeashellBoomerangProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SeashellBoomerang";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seashell Boomerang");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 240;
            aiType = ProjectileID.WoodenBoomerang;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.Calamity().stealthStrike)
                CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 300f, 10f, 20f, 5, ModContent.ProjectileType<Seashell>());
        }
    }
}
