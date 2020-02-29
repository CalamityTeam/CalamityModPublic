using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class SeashellBoomerangProjectileMelee : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boomerang");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 240;
            aiType = ProjectileID.WoodenBoomerang;
        }
    }
}
