using Microsoft.Xna.Framework;
using Terraria;


namespace CalamityMod.Particles
{
    public class SeaFoamParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/SeaFoam";
        public override int FrameVariants => 3;
        public override bool UseAdditiveBlend => true;


        public float Opacity;
        private Color ColorStart;
        private Color ColorFade;
        private float Spin;
        private float BaseScale;

        public SeaFoamParticle(Vector2 position, Vector2 velocity, Color colorStart, Color colorFade, float scale, float opacity, float rotationSpeed = 1f)
        {
            Position = position;
            Velocity = velocity;
            ColorStart = colorStart;
            ColorFade = colorFade;
            BaseScale = scale;
            Opacity = opacity;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Spin = rotationSpeed;
            Variant = Main.rand.Next(3);
        }

        public override void Update()
        {
            Velocity *= 0.85f;
            Rotation += Spin * ((Velocity.X > 0) ? 1f : -1f);
            Opacity -= 10;
            Scale = BaseScale + Opacity / 255f;
            if (Opacity <= 0)
                Kill();
            Color = Color.Lerp(ColorStart, ColorFade, MathHelper.Clamp((float)((255 -Opacity) - 100) / 80, 0f, 1f)) * (Opacity / 255f);
        }
    }
}
