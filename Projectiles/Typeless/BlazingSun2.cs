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
            projectile.width = 130;
            projectile.height = 130;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            //Alpha
            if (projectile.timeLeft > 50)
            {
                if (projectile.alpha > 50)
                    projectile.alpha -= 5;
                if (projectile.alpha < 50)
                    projectile.alpha = 50;
            }
            else
            {
                if (projectile.alpha < 255)
                    projectile.alpha += 5;
                if (projectile.alpha > 255)
                    projectile.alpha = 255;
            }
            //Rotation
            projectile.rotation -= 0.025f;
            
        }
    }
}
