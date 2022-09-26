using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace CalamityMod.Particles
{
    public class ThanatosSmokeParticle : Particle
    {
        public float RelativePower;
        public float BaseMoveRotation;
        public override bool SetLifetime => true;
        public override int FrameVariants => 3;

        public override string Texture => "CalamityMod/Particles/MediumSmoke";

        public ThanatosSmokeParticle(Vector2 relativePosition, int lifetime, float scale, float relativePower, float baseMoveRotation)
        {
            RelativeOffset = relativePosition;
            Velocity = Vector2.Zero;
            Scale = scale;
            Variant = Main.rand.Next(3);
            Lifetime = lifetime;
            RelativePower = relativePower;
            BaseMoveRotation = baseMoveRotation;
        }

        public override void Update()
        {
            if (Scale < 1.25f)
                Scale += RelativePower * 0.04f;
            RelativeOffset -= (BaseMoveRotation + Main.rand.NextFloat(-0.18f, 0.18f)).ToRotationVector2() * RelativePower * 4.5f;

            Color = Color.DarkRed;
            Color = Color.Lerp(Color, new Color(154, 139, 138), (float)Math.Pow(Utils.GetLerpValue(0f, 0.57f, LifetimeCompletion, true), 2D) * 0.5f + 0.5f);
            Color.A = 92;

            float opacity = Utils.GetLerpValue(1f, 0.85f, LifetimeCompletion, true);
            Color *= opacity;
        }

    }
}
