using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace CalamityMod.DataStructures
{
    public struct Circle
    {
        public float Radius;
        public Vector2 Center;

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        private Vector2 RandomPointUnitCircle() => Main.rand.NextVector2Unit() * (float)Math.Sqrt(Main.rand.NextDouble());

        public Vector2 RandomPointInCircle() => Center + RandomPointUnitCircle() * Radius;

        public Vector2 RandomPointOnCircleEdge()
        {
            Vector2 v = RandomPointUnitCircle();
            // Normalise so point is on edge of unit circle.
            v.Normalize();
            return Center + v * Radius;
        }
    }
}
