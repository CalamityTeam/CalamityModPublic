using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Particles
{
    public class FlareShine : Particle
    {
        public override string Texture => "CalamityMod/Particles/ThinSparkle";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;

        private float Spin;
        private float opacity;
        private Color Bloom;
        private Color LightColor => Bloom * opacity;
        private float BloomScale;
        private float HueShift;
        private Vector2 OriginalScale;
        private Vector2 FinalScale;
        private int SpawnDelay;

        public FlareShine(Vector2 position, Vector2 velocity, Color color, Color bloom, float angle, Vector2 scale, Vector2 finalScale, int lifeTime, float rotationSpeed = 0f, float bloomScale = 1f, float hueShift = 0f, int spawnDelay = 0)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Bloom = bloom;

            OriginalScale = scale;
            FinalScale = finalScale;

            Scale = 1f;
            Lifetime = lifeTime;
            Rotation = angle % MathHelper.Pi;
            Spin = rotationSpeed;
            BloomScale = bloomScale;
            HueShift = hueShift;
            SpawnDelay = spawnDelay;
        }

        public override void Update()
        {
            if (SpawnDelay > 0) //Prevent the particle from existing
            {
                Time--;
                Position -= Velocity;
                SpawnDelay--;
                return;
            }

            opacity = (float)Math.Sin(MathHelper.PiOver2 + LifetimeCompletion * MathHelper.PiOver2);
            Velocity *= 0.80f;
            Rotation += Spin * ((Velocity.X > 0) ? 1f : -1f) * (LifetimeCompletion > 0.5 ? 1f : 0.5f);

            Color = Main.hslToRgb((Main.rgbToHsl(Color).X + HueShift) % 1, Main.rgbToHsl(Color).Y, Main.rgbToHsl(Color).Z);
            Bloom = Main.hslToRgb((Main.rgbToHsl(Bloom).X + HueShift) % 1, Main.rgbToHsl(Bloom).Y, Main.rgbToHsl(Bloom).Z);


            Lighting.AddLight(Position, LightColor.R / 255f, LightColor.G / 255f, LightColor.B / 255f);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            if (SpawnDelay > 0)
                return;
            Texture2D sparkTexture = GeneralParticleHandler.GetTexture(Type);
            Texture2D bloomTexture = ModContent.GetTexture("CalamityMod/Particles/BloomCircle");
            //Ajust the bloom's texture to be the same size as the star's
            float properBloomSize = (float)sparkTexture.Height / (float)bloomTexture.Height;
            Vector2 squish = Vector2.Lerp(OriginalScale, FinalScale, PiecewiseAnimation(LifetimeCompletion, new CurveSegment[] { new CurveSegment(EasingType.PolyOut, 0f, 0f, 1f, 4) }));

            spriteBatch.Draw(bloomTexture, Position - Main.screenPosition, null, Bloom * opacity * 0.5f, 0, bloomTexture.Size() / 2f, squish * BloomScale * properBloomSize, SpriteEffects.None, 0);
            spriteBatch.Draw(sparkTexture, Position - Main.screenPosition, null, Color * opacity, Rotation, sparkTexture.Size() / 2f, squish, SpriteEffects.None, 0);
        }
    }
}
