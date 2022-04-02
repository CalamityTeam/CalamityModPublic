using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailGasCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Alchemist's Gas");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;

            // Only one gas cloud can hit at once.
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 9;
        }

        public override void AI()
        {
            projectile.ai[1] += 1f;
            if (projectile.ai[1] > 60f)
            {
                projectile.ai[0] += 10f;
            }
            if (projectile.ai[0] > 255f)
            {
                projectile.Kill();
                projectile.ai[0] = 255f;
            }
            projectile.alpha = (int)(100.0 + (double)projectile.ai[0] * 0.7);
            projectile.rotation += projectile.velocity.X * 0.1f;
            projectile.rotation += (float)projectile.direction * 0.003f;
            projectile.velocity *= 0.96f;
        }
    }
}
