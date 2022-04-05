using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Particles
{
    public class EnchantedParticle : Particle
    {
        public float RelativePower;
        public override bool SetLifetime => true;

        public float InterpolationSpeed;
        public float EdgeOffset;
        public Color EdgeColor;
        public Color CenterColor;

        public override string Texture => "CalamityMod/Particles/Light";

        public EnchantedParticle(Vector2 relativePosition, int lifetime, float scale, Color edgeColor, Color centerColor, float interpolationSpeed, float edgeOffset)
        {
            RelativeOffset = relativePosition;
            Velocity = Vector2.Zero;
            Scale = scale;
            Lifetime = lifetime;
            EdgeColor = edgeColor;
            CenterColor = centerColor;
            InterpolationSpeed = interpolationSpeed;
            EdgeOffset = edgeOffset;
        }

        public override void Update()
        {
            float distanceToCenter = RelativeOffset.Length();
            Scale = MathHelper.SmoothStep(0.05f, 0.125f, Utils.GetLerpValue(EdgeOffset, 6f, distanceToCenter, true));
            Scale *= Utils.GetLerpValue(Lifetime, Lifetime - 10f, Time, true);

            if (distanceToCenter > 4.5f)
                RelativeOffset = Vector2.Lerp(RelativeOffset, Vector2.Zero, InterpolationSpeed);

            Color = Color.Lerp(EdgeColor, CenterColor, Utils.GetLerpValue(0f, 0.67f, LifetimeCompletion, true));
            Color.A = 50;
        }
    }
}
