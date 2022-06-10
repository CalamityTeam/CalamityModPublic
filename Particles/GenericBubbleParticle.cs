using Microsoft.Xna.Framework;
using Terraria;


namespace CalamityMod.Particles
{
    //This is mostly so i can easily get it to draw over the reed blowgun.
    public class GenericBubbleParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/Bubble";
        public override bool UseHalfTransparency => true;
        public override bool SetLifetime => true;
        public override bool Important => true;

        public GenericBubbleParticle(Vector2 position, Vector2 velocity, float scale, float rotation, int lifeTime = 50)
        {
            Position = position;
            Velocity = velocity;
            Color = Color.White;
            Scale = scale;
            Rotation = rotation;
            Lifetime = lifeTime;
        }
    }
}
