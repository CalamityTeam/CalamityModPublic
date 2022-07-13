using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class MarniteObliteratorProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Obliterator");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 20;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
        }
    }
}
