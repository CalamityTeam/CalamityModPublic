using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace CalamityMod.Particles
{
	public class StoneDebrisParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/StoneDebris";
        public override bool SetLifetime => true;
        public override int FrameVariants => 5;

        private float Spin;
        private float opacity;
        Color originalColor;

        public StoneDebrisParticle(Vector2 position, Vector2 velocity, Color color, float scale, int lifeTime, float rotationSpeed = 1f)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            originalColor = color;
            Scale = scale;
            Lifetime = lifeTime;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Spin = rotationSpeed;
            Variant = Main.rand.Next(3);
        }

        public override void Update()
        {
            opacity = LifetimeCompletion < 0.5f ? 1f : (float)Math.Sin(LifetimeCompletion * MathHelper.Pi);
            Color = originalColor * opacity;
            Velocity = Velocity * 0.95f + Vector2.UnitY * 0.1f;
            Rotation += Spin * ((Velocity.X > 0) ? 1f : -1f);
        }
    }
}
