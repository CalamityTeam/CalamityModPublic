using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CalamityMod
{
    public class BezierCurve
    {
        public Vector2[] ControlPoints;

        public BezierCurve(params Vector2[] controls)
        {
            ControlPoints = controls;
        }

        public Vector2 Evaluate(float T)
        {
            if (T < 0f)
                T = 0f;
            if (T > 1f)
                T = 1f;

            return PrivateEvaluate(ControlPoints, T);
        }

        public List<Vector2> GetPoints(int amount)
        {
            float perStep = 1f / amount;

            List<Vector2> points = new List<Vector2>();

            for (float step = 0f; step <= 1f; step += perStep)
            {
                points.Add(Evaluate(step));
            }

            return points;
        }

        private Vector2 PrivateEvaluate(Vector2[] points, float T)
        {
            while (points.Length > 2)
            {
                Vector2[] nextPoints = new Vector2[points.Length - 1];
                for (int k = 0; k < points.Length - 1; k++)
                {
                    nextPoints[k] = Vector2.Lerp(points[k], points[k + 1], T);
                }
                points = nextPoints;
            }
            return Vector2.Lerp(points[0], points[1], T);
        }
    }
}
