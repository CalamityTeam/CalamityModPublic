using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class BlazingSun2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            //Alpha
            if (Projectile.timeLeft < 18)
            {
                Projectile.scale -= 0.05f;
                if (Projectile.alpha < 255)
                    Projectile.alpha += 10;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }

            if (Projectile.timeLeft >= 30)
            {
                Projectile.scale += 0.6f;
            }

            //Rotation
            Projectile.rotation -= 0.025f;
        }

        public override bool? CanDamage() => false;
    }
}
