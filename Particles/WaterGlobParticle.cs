using Microsoft.Xna.Framework;
using Terraria;


namespace CalamityMod.Particles
{
    public class WaterGlobParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/WaterGlob";

        public override bool SetLifetime => true;

        public override bool Important => true;

        private float Spin;

        public WaterGlobParticle(Vector2 position, Vector2 velocity, float scale, float rotationSpeed = 0f, int lifeTime = 50)
        {
            Position = position;
            Velocity = velocity;
            Color = Color.White * 0.5f;
            Scale = scale;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Spin = rotationSpeed;
            Lifetime = lifeTime;
        }

        public override void Update()
        {

            Color *= 0.985f;
            Rotation += Spin * ((Velocity.X > 0) ? 1f : -1f);
            Velocity *= 0.85f;
            Scale *= 0.975f;

        }
    }
}
