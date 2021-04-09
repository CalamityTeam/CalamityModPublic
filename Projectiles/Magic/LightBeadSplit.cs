using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class LightBeadSplit : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/LightBead";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light Bead");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.alpha = 50;
            projectile.scale = 0.6f;
            projectile.penetrate = 1;
            projectile.timeLeft = 400;
            projectile.magic = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.5f / 255f);
            projectile.rotation += projectile.velocity.X * 0.2f;
            projectile.velocity *= 0.985f;

        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 212, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
