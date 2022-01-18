using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.World.Generation;

namespace CalamityMod.World
{
    public static class CustomShapes
    {
        public class DistortedCircle : GenShape
        {
            private readonly int baseRadius;
            private readonly float distortionFactor;

            public DistortedCircle(int radius, float distortionFactor)
            {
                baseRadius = radius;
                this.distortionFactor = distortionFactor;
            }

            public override bool Perform(Point origin, GenAction action)
            {
                float offsetAngle = WorldGen.genRand.NextFloat(-10f, 10f);
                for (float angle = 0f; angle < MathHelper.TwoPi; angle += MathHelper.Pi * 0.0084f)
                {
                    float distortionQuantity = CalamityUtils.AperiodicSin(angle, offsetAngle, MathHelper.PiOver2, MathHelper.E * 0.5f) * distortionFactor;
                    int currentRadius = (int)(baseRadius - distortionQuantity * baseRadius);
                    if (currentRadius <= 0)
                        continue;

                    int horizontalOffset = (int)(Math.Cos(angle) * currentRadius);
                    int verticalOffset = (int)(Math.Sin(angle) * currentRadius);
                    for (int dx = 0; dx != horizontalOffset; dx += Math.Sign(horizontalOffset))
                    {
                        for (int dy = 0; dy != verticalOffset; dy += Math.Sign(verticalOffset))
                            UnitApply(action, origin, origin.X + dx, origin.Y + dy, new object[0]);
                    }
                }
                return true;
            }
        }
    }
}
