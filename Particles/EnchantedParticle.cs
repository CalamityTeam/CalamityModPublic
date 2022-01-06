using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

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
            Variant = Main.rand.Next(3);
            Lifetime = lifetime;
            EdgeColor = edgeColor;
            CenterColor = centerColor;
            InterpolationSpeed = interpolationSpeed;
            EdgeOffset = edgeOffset;
        }

        public override void Update()
        {
            float distanceToCenter = RelativeOffset.Length();
            Scale = MathHelper.SmoothStep(0.05f, 0.125f, Utils.InverseLerp(EdgeOffset, 6f, distanceToCenter, true));
            Scale *= Utils.InverseLerp(Lifetime, Lifetime - 10f, Time, true);

            if (distanceToCenter > 4.5f)
                RelativeOffset = Vector2.Lerp(RelativeOffset, Vector2.Zero, InterpolationSpeed);

            Color = Color.Lerp(EdgeColor, CenterColor, Utils.InverseLerp(0f, 0.67f, LifetimeCompletion, true));
            Color.A = 50;
        }
    }
}
