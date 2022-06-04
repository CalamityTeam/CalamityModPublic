using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class CuteManaStarParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/CuteStars";
        public override bool UseCustomDraw => false;
        public override bool SetLifetime => true;
        public override int FrameVariants => 2;

        public float Opacity;

        public CuteManaStarParticle(Vector2 position, Vector2 speed, float scale = 1f, float opacity = 1f, int lifetime = 20)
        {
            Position = position;
            Scale = scale;
            Color = Color.White;
            Opacity = opacity;
            Velocity = speed;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Lifetime = lifetime;
            Variant = Main.rand.Next(2);
        }

        public override void Update()
        {
            Opacity *= 0.96f;
            Color = Color.White * Opacity;
            Lighting.AddLight(Position, Color.DodgerBlue.ToVector3());
            Velocity *= 0.9f;
            if (Velocity.Length() <= 0.01f)
                GeneralParticleHandler.RemoveParticle(this);
        }
    }
}
