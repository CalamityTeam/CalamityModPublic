using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class SparkInfernal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spark");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 6, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f);
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<InfernadoMarkFriendly>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        }
    }
}
