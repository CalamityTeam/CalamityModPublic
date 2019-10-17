using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles
{
    public class FlamingPumpkin : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkin");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 14;
            projectile.penetrate = 2;
            projectile.timeLeft = 200;
            projectile.magic = true;
            projectile.friendly = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 64, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }
    }
}
