using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static CalamityMod.CalamityUtils;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class CrackParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/Crack";
        public override bool UseAdditiveBlend => true;
        public override bool SetLifetime => true;
        public override bool UseCustomDraw => true;

        private float OriginalScale;
        private float FinalScale;
        private Vector2 Squish;
        private Color BaseColor;
        public float opacity;
        public int Stop = 0;

        public CrackParticle(Vector2 position, Vector2 velocity, Color color, Vector2 squish, float rotation, float originalScale, float finalScale, int lifeTime)
        {
            Position = position;
            Velocity = velocity;
            BaseColor = color;
            OriginalScale = originalScale;
            FinalScale = finalScale;
            Scale = originalScale;
            Lifetime = lifeTime;
            Squish = squish;
            Rotation = rotation;
        }

        public override void Update()
        {
            if (Velocity != Vector2.Zero)
                Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
            ++Stop;
            float pulseProgress = PiecewiseAnimation(LifetimeCompletion, new CurveSegment[] { new CurveSegment(EasingType.PolyOut, 0f, 0f, 1f, 4) });
            Scale = MathHelper.Lerp(OriginalScale, FinalScale, pulseProgress);
            if (Stop > 1)
                Velocity *= 0;
            Color = BaseColor;

            opacity = (float)Math.Sin(MathHelper.PiOver2 + LifetimeCompletion * MathHelper.PiOver2);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color * opacity, Rotation, tex.Size() / 2f, Scale * Squish, SpriteEffects.None, 0);
        }

    }
}
