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
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 3;
            Projectile.timeLeft = 240;
            aiType = ProjectileID.WoodenBoomerang;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Projectile.Calamity().stealthStrike)
                CalamityGlobalProjectile.MagnetSphereHitscan(Projectile, 300f, 10f, 20f, 5, ModContent.ProjectileType<Seashell>());
        }
    }
}
