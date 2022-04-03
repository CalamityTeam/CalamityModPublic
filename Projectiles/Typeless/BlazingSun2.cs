using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class BlazingSun2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blazing Sun");
        }

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            //Alpha
            if (Projectile.timeLeft > 50)
            {
                if (Projectile.alpha > 50)
                    Projectile.alpha -= 5;
                if (Projectile.alpha < 50)
                    Projectile.alpha = 50;
            }
            else
            {
                if (Projectile.alpha < 255)
                    Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }
            //Rotation
            Projectile.rotation -= 0.025f;

        }
    }
}
