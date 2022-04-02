using Microsoft.Xna.Framework;

namespace CalamityMod.Particles
{
    public class CircularSmearSmokeyVFX : Particle //Also check out random noise generation!
    {
        public override string Texture => "CalamityMod/Particles/CircularSmearSmokey";
        public override bool UseAdditiveBlend => true;

        public override bool SetLifetime => true;
        public float opacity;

        public CircularSmearSmokeyVFX(Vector2 position, Color color, float rotation, float scale)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Color = color;
            Scale = scale;
            Rotation = rotation;
            Lifetime = 2;
        }
    }
}
