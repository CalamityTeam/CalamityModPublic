using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Particles
{
    public class ChargeUpLineVFX : Particle
    {
        public override string Texture => "CalamityMod/Particles/Light";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;
        public override bool Important => Telegraph;

        public float BaseOpacity;
        public float BaseScale;
        public float Opacity;
        public float LineDirection;
        public float LineLenght = 0f;
        private Vector2 PrevOffset = Vector2.Zero;
        private Vector2 BasePosition;
        public bool Telegraph; //Denotes if the line is used as an enemy telegraph. In that case, it'll be marked as important
        public float FullFadeInPoint;
        public float MinDistanceFromOrigin;

        public ChargeUpLineVFX(Vector2 startPoint, float lineDirection, float thickness, Color color, int lifetime, float opacity = 1f, bool telegraph = true, float fullFadeInPoint = 0.5f, float minDistanceFromOrigin = 8f)
        {
            RelativeOffset = startPoint;
            BasePosition = startPoint;
            LineDirection = lineDirection + MathHelper.Pi;
            Scale = thickness;
            BaseScale = thickness;
            Color = color;
            BaseOpacity = opacity;
            Telegraph = telegraph;
            FullFadeInPoint = fullFadeInPoint;
            MinDistanceFromOrigin = minDistanceFromOrigin;
            Velocity = Vector2.Zero;
            Rotation = 0;
            Lifetime = lifetime;
        }


        public CurveSegment goBack = new CurveSegment(EasingType.SineInOut, 0f, 1f, 0.25f);
        public CurveSegment goForward = new CurveSegment(EasingType.SineIn, 0.35f, 1.25f, -1.25f);
        public float offsetPosition() => PiecewiseAnimation(LifetimeCompletion, new CurveSegment[] { goBack , goForward } );

        public CurveSegment noSquish = new CurveSegment(EasingType.Linear, 0f, 1f, 0f);
        public CurveSegment squishSpeed = new CurveSegment(EasingType.SineIn, 0.35f, 1f, 0.8f);
        public float Squish() => PiecewiseAnimation(LifetimeCompletion, new CurveSegment[] { noSquish, squishSpeed });


        public override void Update()
        {
            Opacity = LifetimeCompletion > FullFadeInPoint ? BaseOpacity : (float)Math.Sin(Time / (FullFadeInPoint * Lifetime) * MathHelper.PiOver2) * BaseOpacity;

            float offsetMove = LifetimeCompletion == 0 ? 0 : LifetimeCompletion == 1 ? 1 : LifetimeCompletion < 0.5 ? (float)Math.Pow(2f, 20f * LifetimeCompletion - 10) / 2f : -((float)Math.Cos(MathHelper.Pi * LifetimeCompletion) - 1) / 2f;

            Scale = BaseScale - (float)Math.Sin(LifetimeCompletion * MathHelper.Pi) * BaseScale * 0.5f;

            RelativeOffset = offsetPosition() * LineDirection.ToRotationVector2() * BasePosition.Length();

            LineLenght = (BasePosition - RelativeOffset).Length();

            RelativeOffset = RelativeOffset.RotatedBy(MathHelper.PiOver4 / 16f);
            //LineDirection -= MathHelper.PiOver4 / 16f;

            if (RelativeOffset.Length() < MinDistanceFromOrigin)
                RelativeOffset = LineDirection.ToRotationVector2() * MinDistanceFromOrigin;
        }

        public override void CustomDraw(SpriteBatch spriteBatch, Vector2 basePosition)
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Particles/Light").Value;

            float rot = LineDirection + MathHelper.PiOver2;
            Vector2 origin = new Vector2(tex.Width / 2f, tex.Height);
            Vector2 scale = new Vector2(Scale - Scale * Squish() * 0.3f, Scale * Squish());

            Vector2 drawPosition = basePosition - Main.screenPosition + RelativeOffset;

            //Main.spriteBatch.Draw(tex, drawPosition, null, Color * Opacity * 0.8f, rot, origin, scale * 1.1f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(tex, drawPosition, null, Color.White * Opacity, rot, origin, scale, SpriteEffects.None, 0f);



            Main.spriteBatch.Draw(tex, drawPosition, null, Color * Opacity * 0.8f, rot, origin, scale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(tex, drawPosition, null, Color.White * Opacity, rot, origin, scale, SpriteEffects.None, 0f);

        }
    }
}
