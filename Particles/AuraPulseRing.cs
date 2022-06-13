using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static CalamityMod.CalamityUtils;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class AuraPulseRing : Particle
    {
        public override string Texture => "CalamityMod/Particles/HollowCircleHardEdge";
        public override bool UseAdditiveBlend => true;
        public override bool SetLifetime => true;

        private Vector2 OriginalScale;
        private Vector2 FinalScale;
        public Vector2 CurrentScale;
        private float opacity;
        private Color BaseColor;
        public NPC StuckTo;

        public AuraPulseRing(Color color, Vector2 originalScale, Vector2 finalScale, int lifeTime, NPC stuckTo)
        {
            BaseColor = color;
            OriginalScale = originalScale;
            FinalScale = finalScale;
            CurrentScale = originalScale;
            Lifetime = lifeTime;
            StuckTo = stuckTo;
        }

        public override void Update()
        {
            float pulseProgress = PiecewiseAnimation(LifetimeCompletion, new CurveSegment[] { new CurveSegment(SineBumpEasing, 0f, 0f, 1f)});
            
            float heightProgress = PiecewiseAnimation(LifetimeCompletion, new CurveSegment[] { new CurveSegment(SineInOutEasing, 0f, 0f, 1f) });

            opacity = (float)Math.Sin(LifetimeCompletion * MathHelper.Pi) * 0.8f + 0.2f;

            Color = BaseColor * opacity;
            Lighting.AddLight(Position, Color.R / 255f, Color.G / 255f, Color.B / 255f);

            if (StuckTo != null && StuckTo.active)
            {
                CurrentScale = Vector2.Lerp(OriginalScale, FinalScale, pulseProgress) * StuckTo.scale;
                RelativeOffset = - Vector2.UnitY.RotatedBy(StuckTo.rotation) * StuckTo.height / 2f + Vector2.UnitY.RotatedBy(StuckTo.rotation) * StuckTo.height * heightProgress;
                Rotation = StuckTo.rotation;
            }
        }

        public override void CustomDraw(SpriteBatch spriteBatch, Vector2 basePosition)
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Particles/HollowCircleHardEdge").Value; //somehow wont work for set particles and defaults to the first particle it has (blood)

            spriteBatch.Draw(tex, basePosition + RelativeOffset - Main.screenPosition, null, Color * opacity, Rotation, tex.Size() / 2f, CurrentScale, SpriteEffects.None, 0);
        }
    }
}
