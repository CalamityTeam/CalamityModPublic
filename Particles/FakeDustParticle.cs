using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class FakeGlowDust : Particle
    {
        public override string Texture => "CalamityMod/Particles/FakeDust";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;

        private float Spin;
        private float opacity;
        private bool Big;
        private bool EmitsLight;
        private Vector2 Gravity;

        public FakeGlowDust(Vector2 position, Vector2 velocity, Color color, float scale, int lifeTime, float rotationSpeed = 1f, bool bigSize = false, bool emitsLight = false, Vector2? gravity = null)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Lifetime = lifeTime;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Spin = rotationSpeed;
            Big = bigSize;
            EmitsLight = emitsLight;
            Gravity = (Vector2)(gravity == null ? Vector2.Zero : gravity);
            Variant = Main.rand.Next(3);
        }

        public override void Update()
        {
            Velocity += Gravity;
            opacity = (float)Math.Sin(LifetimeCompletion * MathHelper.Pi);

            if (EmitsLight)
                Lighting.AddLight(Position, opacity * Color.R / 255f, opacity * Color.G / 255f, opacity * Color.B / 255f);

            Velocity *= 0.95f;
            Rotation += Spin * ((Velocity.X > 0) ? 1f : -1f);
            Scale *= 0.98f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D dustTexture = Big ? ModContent.Request<Texture2D>("CalamityMod/Particles/FakeDustBig").Value : ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = new Rectangle(0, (Big ? 8 : 6) * Variant, (Big ? 8 : 6), (Big ? 8 : 6));
            spriteBatch.Draw(dustTexture, Position - Main.screenPosition, frame, Color * opacity, Rotation, frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}
