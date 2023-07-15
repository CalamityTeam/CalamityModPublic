using Microsoft.Xna.Framework;
using Terraria;


namespace CalamityMod.Particles
{
    public class MediumMistParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/MediumMist";
        public override int FrameVariants => 3;
        public override bool UseAdditiveBlend => true;


        private float Opacity;
        private Color ColorFire;
        private Color ColorFade;
        private float Spin;

        public MediumMistParticle(Vector2 position, Vector2 velocity, Color colorFire, Color colorFade, float scale, float opacity, float rotationSpeed = 0f)
        {
            Position = position;
            Velocity = velocity;
            ColorFire = colorFire;
            ColorFade = colorFade;
            Scale = scale;
            Opacity = opacity;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Spin = rotationSpeed;
            Variant = Main.rand.Next(3);
        }

        public override void Update()
        {
            Rotation += Spin * ((Velocity.X > 0) ? 1f : -1f);
            Velocity *= 0.85f;

            if (Opacity > 90)
            {
                Lighting.AddLight(Position, Color.ToVector3() * 0.1f);
                Scale += 0.01f;
                Opacity -= 3;
            }
            else
            {
                Scale *= 0.975f;
                Opacity -= 2;
            }

            if (Opacity < 0)
                Kill();

            Color = Color.Lerp(ColorFire, ColorFade, MathHelper.Clamp((float)((255 - Opacity) - 100) / 80, 0f, 1f)) * (Opacity / 255f);

        }
    }
}
