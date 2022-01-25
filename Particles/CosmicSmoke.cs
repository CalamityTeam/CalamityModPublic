using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace CalamityMod.Particles { 
    public class CosmicSmokeParticle : Particle 
    {
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;
        public override string Texture => "CalamityMod/Particles/SmallSmoke";

        private float Opacity;
        private Color BackColor;
        private float ColorHueShift;
        private float Spin;

        public CosmicSmokeParticle(Vector2 position, Vector2 velocity, Color color, Color backColor, float scale, float opacity, int lifetime, float rotationSpeed = 0f)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            BackColor = backColor;
            Scale = scale;
            Opacity = opacity;
            Lifetime = lifetime;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Spin = rotationSpeed;
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

        }
    }
}