using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SquirrelSquireAcorn : ModProjectile
    {
        public const float Gravity = 0.5f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acorn");
            ProjectileID.Sets.SentryShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 180;
            projectile.MaxUpdates = 2;
        }

        public override void AI()
        {
            projectile.velocity.Y += Gravity;
            projectile.rotation += (projectile.velocity.X > 0f).ToDirectionInt() * 0.3f;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 7, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
        }
    }
}
