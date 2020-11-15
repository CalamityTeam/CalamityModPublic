using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Hybrid
{
    public class SeashellBoomerangProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SeashellBoomerang";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boomerang");
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
            projectile.melee = true;
        }
    }
}
