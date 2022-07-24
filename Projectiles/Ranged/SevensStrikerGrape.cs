using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SevensStrikerGrape : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grapes");
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.rotation += 0.1f * Projectile.direction;
        }
    }
}
