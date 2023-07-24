using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class FlatGlow : Particle
    {
        public override string Texture => "CalamityMod/Particles/FlatShape";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;

        private Vector2 OriginalScale;
        private Vector2 FinalScale;
        public Vector2 CurrentScale;
        private float opacity;
        private Color BaseColor;

        public FlatGlow(Vector2 position, Vector2 velocity, Color color, float rotation, Vector2 originalScale, Vector2 finalScale, int lifeTime)
        {
            Position = position;
            Velocity = velocity;
            BaseColor = color;
            OriginalScale = originalScale;
            FinalScale = finalScale;
            CurrentScale = originalScale;
            Lifetime = lifeTime;
            Rotation = rotation;
        }

        public override void Update()
        {
            opacity = (float)Math.Sin(LifetimeCompletion * MathHelper.Pi);
            CurrentScale = Vector2.Lerp(OriginalScale, FinalScale, LifetimeCompletion);
            Color = BaseColor * opacity;
            Lighting.AddLight(Position, Color.R / 255f, Color.G / 255f, Color.B / 255f);
            Velocity *= 0.95f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color * opacity, Rotation, tex.Size() / 2f, CurrentScale, SpriteEffects.None, 0);
        }
    }
}
