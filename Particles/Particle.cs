using Microsoft.Xna.Framework;

namespace CalamityMod.Particles
{
	public class Particle
	{
		public int Time;
		public int Lifetime;
		public int Variant;
		public float Scale;
		public Vector2 RelativeOffset;
		public virtual string Texture => "";

		public Particle(int lifetime, Vector2 relativeOffset, float scale, int variant = 0)
		{
			Time = 0;
			Lifetime = lifetime;
			Variant = variant;
			Scale = scale;
			RelativeOffset = relativeOffset;
		}
	}
}