using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SevensStrikerCherrySplit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 150;
        }

        public override void AI()
        {
            Projectile.rotation += 0.4f * Projectile.direction;
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }
    }
}
