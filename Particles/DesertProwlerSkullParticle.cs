using Microsoft.Xna.Framework;
using Terraria;


namespace CalamityMod.Particles
{
    public class DesertProwlerSkullParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/DesertProwlerSkull";

        private float Opacity;
        private Color ColorStart;
        private Color ColorFade;

        public DesertProwlerSkullParticle(Vector2 position, Vector2 velocity, Color colorStart, Color colorFade, float scale, float opacity)
        {
            Position = position;
            Velocity = velocity;
            ColorStart = colorStart;
            ColorFade = colorFade;
            Scale = scale;
            Opacity = opacity;
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
    }
}
