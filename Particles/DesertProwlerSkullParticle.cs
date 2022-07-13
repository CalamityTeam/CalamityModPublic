using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Particles
{
    public class DesertProwlerSkullParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/DesertProwlerSkull";
        public override bool UseCustomDraw => true;
        public override bool UseHalfTransparency => true;

        private float Opacity;
        private Color ColorStart;
        private Color ColorFade;
        private float BaseOpacity;

        public DesertProwlerSkullParticle(Vector2 position, Vector2 velocity, Color colorStart, Color colorFade, float scale, float opacity)
        {
            Position = position;
            Velocity = velocity;
            ColorStart = colorStart;
            ColorFade = colorFade;
            Scale = scale;
            Opacity = opacity;
            BaseOpacity = opacity;  
            Rotation = Main.rand.NextFloat(MathHelper.PiOver4 * 0.5f) - MathHelper.PiOver4 * 0.25f;
        }

        public override void Update()
        {

            Velocity *= 0.97f;

            if (Opacity > 90)
            {
                Lighting.AddLight(Position, Color.ToVector3() * 0.1f);
                Scale += 0.01f;
                Opacity -= 3;
            }
            else
            {
                Scale *= 1.01f;
                Opacity -= 2f;
            }
            if (Opacity < 0)
                Kill();

            Color = Color.Lerp(ColorStart, ColorFade, MathHelper.Clamp((float)((255 - Opacity) - 100) / 80, 0f, 1f)) * (Opacity / 255f);
        }

        public CurveSegment UpsquashSegment = new CurveSegment(PolyOutEasing, 0f, 1f, 0.2f, 2);
        public CurveSegment DownsquashSegment = new CurveSegment(PolyInEasing, 0.2f, 1.2f, -0.3f, 2);
        public CurveSegment BumpSquashSegment = new CurveSegment(SineOutEasing, 0.3f, 0.9f, -0.05f);
        public CurveSegment BumpSquash2Segment = new CurveSegment(SineInEasing, 0.6f, 0.85f, 0.15f);
        public CurveSegment BumpSquash3Segment = new CurveSegment(SineBumpEasing, 0.76f, 1f, 0.05f);
        public CurveSegment StaySegment = new CurveSegment(LinearEasing, 0.9f, 1f, 0f);
        internal float Squash => PiecewiseAnimation(1 - Opacity / BaseOpacity, new CurveSegment[] { UpsquashSegment, DownsquashSegment, BumpSquashSegment, BumpSquash2Segment, BumpSquash3Segment, StaySegment });

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D sprite = GeneralParticleHandler.GetTexture(Type);
            Vector2 size = Scale * new Vector2(1 - (Squash - 1) * 0.8f, 1 + (Squash - 1) * 0.8f);
            spriteBatch.Draw(sprite, Position - Main.screenPosition, null, Color, Rotation, sprite.Size() / 2f, size, 0, 0);
        }
    }
}
