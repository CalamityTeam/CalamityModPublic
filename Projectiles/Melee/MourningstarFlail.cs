using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee
{
    public class MourningstarFlail : BaseWhipProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mourningstar");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Calamity().trueMelee = true;
        }
    }
}
