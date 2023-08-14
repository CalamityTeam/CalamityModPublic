using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Typeless
{
    public class BlazingSun : ModProjectile, ILocalizedModType
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
            Projectile.rotation += 0.025f;
            //Light is produced only when parry is available
            if (Projectile.timeLeft >= 18)
                Lighting.AddLight(Projectile.Center, new Vector3(240, 185, 7) * (3f / 255));
        }

        public override bool? CanDamage() => false;
    }
}
