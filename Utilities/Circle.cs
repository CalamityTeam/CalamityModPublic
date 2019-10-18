using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace CalamityMod
{
    public class Circle
    {
        public float Radius;
        public Vector2 Center;

        public Circle(Vector2 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        private Vector2 RandomPointUnitCircle()
        {
            double r = Math.Sqrt(Main.rand.NextDouble());
            double t = Main.rand.NextDouble() * MathHelper.TwoPi;
            return new Vector2((float)(r * Math.Cos(t)), (float)(r * Math.Sin(t)));
        }

        public Vector2 RandomPointInCircle()
        {
            return Center + RandomPointUnitCircle() * Radius;
        }

        public Vector2 RandomPointOnCircleEdge()
        {
            Vector2 v = RandomPointUnitCircle();
            //Normalise so point is on edge of unit circle
            v.Normalize();
            return Center + v * Radius;
        }
    }
}
