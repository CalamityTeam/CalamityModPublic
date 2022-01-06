using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Particles
{
	public class Particle
	{
		/// <summary>
		/// The ID of the particle inside of the general particle handler's array. This is set automatically when the particle is created
		/// </summary>
		public int ID; 
		/// <summary>
		/// The ID of the particle type as registered by the general particle handler. This is set automatically when the particle handler loads
		/// </summary>
		public int Type;
		/// <summary>
		/// The amount of frames this particle has existed for. You shouldn't have to touch this manually.
		/// </summary>
		public int Time;
		/// <summary>
		/// The maximum amount of frames a particle may stay alive when used in a particle set. 
		/// Keep in mind the general particle handler will not automatically clear particles past their lifetime, nor will it care if the Lifetime isnt set at all.
		/// </summary>
		public int Lifetime;

		public int FrameVariants = 1;
		public int Variant = 0;

		/// <summary>
		/// The offset of the particle in relation to the origin of the set it belongs to. This is only used in the context of a Particle Set
		/// </summary>
		public Vector2 RelativeOffset;
		/// <summary>
		/// The inworld position of a particle. Keep in mind this isn't used in the context of a Particle Set, since all the particles work off their relative position to the set's origin
		/// </summary>
		public Vector2 Position;
		public Vector2 Velocity;


		public Vector2 Origin;
		public Color Color;
		public float Rotation;
		public float Scale;

		public virtual string Texture => "";
		/// <summary>
		/// Set this to true to disable default particle drawing, thus calling Particle.CustomDraw() instead.
		/// </summary>
		public virtual bool UseCustomDraw => false;
		/// <summary>
		/// Use this method if you want to handle the particle drawing yourself. Only called if Particle.UseCustomDraw is set to true.
		/// </summary>
		public virtual void CustomDraw(SpriteBatch spriteBatch) { }
		/// <summary>
		/// Called for every update of the particle handler.
		/// The particle's velocity gets automatically added to its position, and its time automatically increases.
		/// </summary>
		public virtual void Update() { }

		/// <summary>
		/// Set this to true to make your particle use additive blending instead of alphablend.
		/// </summary>
		public virtual bool UseAdditiveBlend => false;

		/// <summary>
		/// Removes the particle from the handler
		/// </summary>
		public void Kill() => GeneralParticleHandler.RemoveParticle(ID);

		public Particle(int lifetime, Vector2 relativeOffset, float scale, int variant = 0)
		{
			Time = 0;
			Lifetime = lifetime;
			Variant = variant;
			Scale = scale;
			RelativeOffset = relativeOffset;
		}

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