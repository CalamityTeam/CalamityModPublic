using Microsoft.Xna.Framework;
using Terraria;
using System;

namespace CalamityMod.Particles
{
    public class TimedSmokeParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/SmallSmoke";
        public override bool SetLifetime => true;

        private float Opacity;
        private Color ColorStart;
        private Color ColorFade;
        private float Spin;

        public TimedSmokeParticle(Vector2 position, Vector2 velocity, Color colorStart, Color colorFade, float scale, float opacity, int timeleft, float rotationSpeed = 0f)
        {
            Position = position;
            Velocity = velocity;
            ColorStart = colorStart;
            ColorFade = colorFade;
            Scale = scale;
            Opacity = opacity;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Spin = rotationSpeed;
            Lifetime = timeleft;
        }

        public override void Update()
        {
            Rotation += Spin * ((Velocity.X > 0) ? 1f : -1f);
            Velocity *= 0.85f;

            if (LifetimeCompletion < 0.84f)
            {
                Scale += 0.01f;
            }
            else
            {
                Scale *= 0.975f;
            }

            Velocity -= Vector2.UnitY * 0.08f;

            float opacityMult = 1 - (float)Math.Pow(LifetimeCompletion, 2);
            Color = Color.Lerp(ColorStart, ColorFade, opacityMult) * (Opacity * opacityMult);
        }
    }
}
