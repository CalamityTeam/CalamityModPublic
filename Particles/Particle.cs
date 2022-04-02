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
        /// Set this to true if you NEED the particle to render even if the particle cap is reached.
        /// </summary>
        public virtual bool Important => false;

        /// <summary>
        /// Set this to true if you want your particle to automatically get removed when its time reaches its maximum lifetime
        /// </summary>
        public virtual bool SetLifetime => false;
        /// <summary>
        /// The maximum amount of frames a particle may stay alive if Particle.SetLifeTime is set to true
        /// </summary>
        public int Lifetime = 0;

        public float LifetimeCompletion => Lifetime != 0 ? Time / (float)Lifetime : 0;

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

        public virtual int FrameVariants => 1;
        public int Variant = 0;
        public virtual string Texture => "";
        /// <summary>
        /// Set this to true to disable default particle drawing, thus calling Particle.CustomDraw() instead.
        /// </summary>
        public virtual bool UseCustomDraw => false;
        /// <summary>
        /// Use this method if you want to handle the particle drawing yourself. Only called if Particle.UseCustomDraw is set to true.
        /// </summary>
        public virtual void CustomDraw(SpriteBatch spriteBatch) { }

        public virtual void CustomDraw(SpriteBatch spriteBatch, Vector2 basePosition) { }
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
        /// Set this to true to make your particles work with semi transparent pixels. Is overriden if UseAdditiveBlend is set to true.
        /// </summary>
        public virtual bool UseHalfTransparency => false;

        /// <summary>
        /// Removes the particle from the handler
        /// </summary>
        public void Kill() => GeneralParticleHandler.RemoveParticle(this);

    }
}
