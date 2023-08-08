using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class SnowflakeSparkle : Particle
    {
        public override string Texture => "CalamityMod/Particles/HalfIceStar";

        public override bool UseAdditiveBlend => true;

        public override bool UseCustomDraw => true;

        public override bool SetLifetime => true;

        private float Spin;
        private float opacity;
        private Color Bloom;

        private Color LightColor => Bloom * opacity;
        private float BloomScale;
        private int Spokes;

        public SnowflakeSparkle(Vector2 position, Vector2 velocity, Color color, Color bloom, float scale, int lifeTime, float rotationSpeed = 1f, float bloomScale = 1f, int spokes = 3)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Bloom = bloom;
            Scale = scale;
            Lifetime = lifeTime;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Spin = rotationSpeed;
            BloomScale = bloomScale;
            Spokes = spokes;
        }

        public override void Update()
        {
            opacity = (float)Math.Sin(LifetimeCompletion * MathHelper.Pi);
            Lighting.AddLight(Position, LightColor.R / 255f, LightColor.G / 255f, LightColor.B / 255f);
            Velocity *= 0.95f;
            Rotation += Spin * ((Velocity.X > 0) ? 1f : -1f);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D spokesTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            // Ajust the bloom's texture to be the same size as the star's.
            float properBloomSize = (float)spokesTexture.Height / (float)bloomTexture.Height;
            float halvedOpacity = opacity * 0.5f;

            spriteBatch.Draw(bloomTexture, Position - Main.screenPosition, null, Bloom * halvedOpacity, 0, bloomTexture.Size() / 2f, Scale * BloomScale * properBloomSize, SpriteEffects.None, 0);

            Color spokeColor = Color * halvedOpacity;
            Vector2 origin = spokesTexture.Size() / 2f;
            for (int i = 0; i < Spokes; i++)
            {
                float rotation = Rotation + MathHelper.Lerp(0f, MathHelper.Pi, i / (float)Spokes);
                spriteBatch.Draw(spokesTexture, Position - Main.screenPosition, null, spokeColor, Rotation + rotation, origin, Scale, SpriteEffects.None, 0);
            }
        }
    }
}
