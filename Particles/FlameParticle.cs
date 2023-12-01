using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Particles
{
    public class FlameParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/Flames";
        public override bool UseAdditiveBlend => true;
        public override bool SetLifetime => true;
        public float RelativePower;
        
        

        public Color BrightColor;
        public Color DarkColor;
        

        public FlameParticle(Vector2 position, int lifetime, float scale, float relativePower, Color brightColor, Color darkColor)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Velocity.X = (Main.rand.NextFloat(1f, -1f));
            Scale = scale;
            Variant = Main.rand.Next(3);
            Lifetime = lifetime;
            RelativePower = relativePower;
            BrightColor = brightColor;
            DarkColor = darkColor;
        }

        public override void Update()
        {
            Scale += RelativePower * 0.01f;
            Position.Y -= RelativePower * 1.25f;
            Scale *= 0.97f;

            Color = Color.Lerp(BrightColor, DarkColor, LifetimeCompletion);
            Color = Color.Lerp(Color, Color.White, Utils.GetLerpValue(0.1f, 0.25f, LifetimeCompletion, true) * Utils.GetLerpValue(0.4f, 0.25f, LifetimeCompletion, true) * 0.7f);
            Color *= Utils.GetLerpValue(0f, 0.15f, LifetimeCompletion, true) * Utils.GetLerpValue(1f, 0.8f, LifetimeCompletion, true) * 0.6f;
            Color *= 1.5f;
            Color.A = 50;
        }
    }
}
