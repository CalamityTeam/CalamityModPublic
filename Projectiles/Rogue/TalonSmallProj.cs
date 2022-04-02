using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TalonSmallProj : ModProjectile
    {
        private static float SineFrequency = 0.05f;
        private static float SineAmplitude = 0.008f;
        private static float RotationIncrement = 0.26f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terror Talon");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;

            SineFrequency = 0.15f;
            SineAmplitude = 0.06f;
            RotationIncrement = 0.26f;
        }

        public override void AI()
        {
            // On-spawn effects
            if (projectile.ai[0] == 0)
            {
                // Store the X and Y of the spawn velocity so it can be used for trig calculations
                projectile.localAI[0] = projectile.velocity.X;
                projectile.localAI[1] = projectile.velocity.Y;
            }

            // Doesn't collide with tiles for the first 2 frames
            projectile.tileCollide = projectile.ai[0] > 2f;

            // Apply fancy sine movement, then slight gravity, then cap velocity.
            // Original velocity is reconstructed so that it can be used in the calculation
            Vector2 originalVelocity = new Vector2(projectile.localAI[0], projectile.localAI[1]);
            ApplySineVelocity(originalVelocity);
            // projectile.velocity.Y += 0.08f;
            float currentSpeed = projectile.velocity.Length();
            float maxSpeed = 1.4f * originalVelocity.Length();
            if (currentSpeed > maxSpeed)
                projectile.velocity *= maxSpeed / currentSpeed;

            // spawn dust
            if (Main.rand.Next(5) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 85, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }

            // freely spins instead of facing forwards
            projectile.rotation += RotationIncrement;
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.ai[0]++;
        }

        private void ApplySineVelocity(Vector2 baseVelocity)
        {
            float radians = projectile.ai[1] * (-MathHelper.PiOver2 + SineFrequency * projectile.ai[0]);
            projectile.velocity += SineAmplitude * baseVelocity.RotatedBy(radians);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 5; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 85, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
