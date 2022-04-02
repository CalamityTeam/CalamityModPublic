using Microsoft.Xna.Framework;

namespace CalamityMod.Particles
{
	public class CircularSmearVFX : Particle //Also check out Split mod!
    {
        public override string Texture => "CalamityMod/Particles/CircularSmear";
        public override bool UseAdditiveBlend => true;

        public override bool SetLifetime => true;
        public float opacity;

        public CircularSmearVFX(Vector2 position, Color color, float rotation, float scale)
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
