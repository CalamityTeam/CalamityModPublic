using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class GooglyEye : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public ref float Time => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 480;
        }

        public override void AI()
        {
            if (Time < 5f)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            else
                Projectile.rotation += (Projectile.velocity.X > 0f).ToDirectionInt() * Projectile.velocity.Length() * 0.018f;

            if (Projectile.timeLeft < 90)
                Projectile.Opacity = Projectile.timeLeft / 90f;
            Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y + 0.325f, -25f, 25f);

            Time++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Stop bouncing after a certain degree of slowness has been reached.
            if (Projectile.velocity.Length() < 3f)
            {
                Projectile.velocity = Vector2.Zero;
                return false;
            }

            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.75f;
            Projectile.velocity *= 0.8f;

            return false;
        }
    }
}
