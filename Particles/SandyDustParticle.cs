using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class SandyDustParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/SandyDust";
        public override bool UseHalfTransparency => true; //Doesn't actually use half transparency, but guarantees that it gets drawn above the bigger smoke clouds
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;

        private float Spin;
        private float opacity;
        private Vector2 Gravity;
        public Rectangle Frame;

        public SandyDustParticle(Vector2 position, Vector2 velocity, Color color, float scale, int lifeTime, float rotationSpeed = 1f, bool bigSize = false, bool emitsLight = false, Vector2? gravity = null)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Lifetime = lifeTime;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Spin = rotationSpeed;
            Gravity = (Vector2)(gravity == null ? Vector2.Zero : gravity);
            Variant = Main.rand.Next(18);
            Frame = new Rectangle(Variant % 6 * 12, Variant / 6 * 12, 10, 10);
        }

        public override void Update()
        {
            Velocity += Gravity;
            opacity = (float)Math.Sin(LifetimeCompletion * MathHelper.Pi);

            Velocity *= 0.95f;
            Rotation += Spin * ((Velocity.X > 0) ? 1f : -1f);
            Scale *= 0.98f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D dustTexture = GeneralParticleHandler.GetTexture(Type);
            spriteBatch.Draw(dustTexture, Position - Main.screenPosition, Frame, Color * opacity, Rotation, Frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}
