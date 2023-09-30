using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TalonSmallProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        private static float SineFrequency = 0.05f;
        private static float SineAmplitude = 0.008f;
        private static float RotationIncrement = 0.26f;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = RogueDamageClass.Instance;

            SineFrequency = 0.15f;
            SineAmplitude = 0.06f;
            RotationIncrement = 0.26f;
        }

        public override void AI()
        {
            // On-spawn effects
            if (Projectile.ai[0] == 0)
            {
                // Store the X and Y of the spawn velocity so it can be used for trig calculations
                Projectile.localAI[0] = Projectile.velocity.X;
                Projectile.localAI[1] = Projectile.velocity.Y;
            }

            // Doesn't collide with tiles for the first 2 frames
            Projectile.tileCollide = Projectile.ai[0] > 2f;

            // Apply fancy sine movement, then slight gravity, then cap velocity.
            // Original velocity is reconstructed so that it can be used in the calculation
            Vector2 originalVelocity = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
            ApplySineVelocity(originalVelocity);
            // projectile.velocity.Y += 0.08f;
            float currentSpeed = Projectile.velocity.Length();
            float maxSpeed = 1.4f * originalVelocity.Length();
            if (currentSpeed > maxSpeed)
                Projectile.velocity *= maxSpeed / currentSpeed;

            // spawn dust
            if (Main.rand.Next(5) == 0)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 85, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }

            // freely spins instead of facing forwards
            Projectile.rotation += RotationIncrement;
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.ai[0]++;
        }

        private void ApplySineVelocity(Vector2 baseVelocity)
        {
            float radians = Projectile.ai[1] * (-MathHelper.PiOver2 + SineFrequency * Projectile.ai[0]);
            Projectile.velocity += SineAmplitude * baseVelocity.RotatedBy(radians);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 5; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 85, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
