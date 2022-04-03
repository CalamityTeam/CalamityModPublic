using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class StrongBloom : Particle
    {
        public override string Texture => "CalamityMod/Particles/BloomCircle";
        public override bool UseAdditiveBlend => true;
        public override bool SetLifetime => true;

        private float opacity;
        private Color BaseColor;

        public StrongBloom(Vector2 position, Vector2 velocity, Color color, float scale, int lifeTime)
        {
            Position = position;
            Velocity = velocity;
            BaseColor = color;
            Scale = scale;
            Lifetime = lifeTime;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void Update()
        {
            opacity = (float)Math.Sin(LifetimeCompletion * MathHelper.Pi);
            Color = BaseColor * opacity;
            Lighting.AddLight(Position, Color.R / 255f, Color.G / 255f, Color.B / 255f);
            Velocity *= 0.95f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = GeneralParticleHandler.Assets.Request<Texture2D>(Type).Value;

            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color * opacity, Rotation, tex.Size() / 2f, Scale, SpriteEffects.None, 0);
        }

        public override void CustomDraw(SpriteBatch spriteBatch, Vector2 relativePosition)
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");

            spriteBatch.Draw(tex, relativePosition - Main.screenPosition, null, Color, Rotation, tex.Size() / 2f, Scale, SpriteEffects.None, 0);
        }
    }
}
