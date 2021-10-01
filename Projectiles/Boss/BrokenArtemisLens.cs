using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BrokenArtemisLens : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public override void SetStaticDefaults() => DisplayName.SetDefault("Lens");

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 480;
        }

        public override void AI()
        {
            if (Time < 5f)
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            else
                projectile.rotation += (projectile.velocity.X > 0f).ToDirectionInt() * projectile.velocity.Length() * 0.018f;

            if (projectile.timeLeft < 90)
                projectile.Opacity = projectile.timeLeft / 90f;
            projectile.velocity.Y = MathHelper.Clamp(projectile.velocity.Y + 0.325f, -25f, 25f);

            Time++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Stop bouncing after a certain degree of slowness has been reached.
            if (projectile.velocity.Length() < 3f)
            {
                projectile.velocity = Vector2.Zero;
                return false;
            }

            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;

            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y * 0.75f;
            projectile.velocity *= 0.8f;

            return false;
        }
    }
}
