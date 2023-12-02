using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {

        internal static readonly List<Vector2> Directions = new List<Vector2>()
        {
            new Vector2(-1f, -1f),
            new Vector2(1f, -1f),
            new Vector2(-1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, -1f),
            new Vector2(-1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 0f),
        };

        /// <summary>
        /// Computes 2-dimensional Perlin Noise, which gives "random" but continuous values.
        /// </summary>
        /// <param name="x">The X position on the map.</param>
        /// <param name="y">The Y position on the map.</param>
        /// <param name="octaves">A metric of "instability" of the noise. The higher this is, the more unstable. Lower of bounds of 2-3 are preferable.</param>
        /// <param name="seed">The seed for the noise.</param>
        public static float PerlinNoise2D(float x, float y, int octaves, int seed)
        {
            float SmoothFunction(float n) => 3f * n * n - 2f * n * n * n;
            float NoiseGradient(int s, int noiseX, int noiseY, float xd, float yd)
            {
                int hash = s;
                hash ^= 1619 * noiseX;
                hash ^= 31337 * noiseY;

                hash = hash * hash * hash * 60493;
                hash = (hash >> 13) ^ hash;

                Vector2 g = Directions[hash & 7];

                return xd * g.X + yd * g.Y;
            }

            int frequency = (int)Math.Pow(2D, octaves);
            x *= frequency;
            y *= frequency;

            int flooredX = (int)x;
            int flooredY = (int)y;
            int ceilingX = flooredX + 1;
            int ceilingY = flooredY + 1;
            float interpolatedX = x - flooredX;
            float interpolatedY = y - flooredY;
            float interpolatedX2 = interpolatedX - 1;
            float interpolatedY2 = interpolatedY - 1;

            float fadeX = SmoothFunction(interpolatedX);
            float fadeY = SmoothFunction(interpolatedY);

            float smoothX = MathHelper.Lerp(NoiseGradient(seed, flooredX, flooredY, interpolatedX, interpolatedY), NoiseGradient(seed, ceilingX, flooredY, interpolatedX2, interpolatedY), fadeX);
            float smoothY = MathHelper.Lerp(NoiseGradient(seed, flooredX, ceilingY, interpolatedX, interpolatedY2), NoiseGradient(seed, ceilingX, ceilingY, interpolatedX2, interpolatedY2), fadeX);
            return MathHelper.Lerp(smoothX, smoothY, fadeY);
        }

        // When two periodic functions are summed, the resulting function is periodic if the
        // ratio of the b/a is rational, given periodic functions f and g:
        // f(a * x) + g(b * x). However, if the ratio is irrational, then the result has no period.
        // This is desirable for somewhat random wavy fluctuations.
        // In this case, pi/1 (or simply pi) is used, which is indeed an irrational number.
        /// <summary>
        /// Calculates an aperiodic sine. This function only achieves this if <paramref name="a"/> and <paramref name="b"/> are irrational numbers.
        /// </summary>
        /// <param name="x">The input value.</param>
        /// <param name="a">The first irrational coefficient.</param>
        /// <param name="b">The second irrational coefficient.</param>
        public static float AperiodicSin(float x, float dx = 0f, float a = MathHelper.Pi, float b = MathHelper.E)
        {
            return (float)(Math.Sin(x * a + dx) + Math.Sin(x * b + dx)) * 0.5f;
        }

        /// <summary>
        /// Computes the Manhattan Distance between two points. This is typically used as a cheaper alternative to Euclidean Distance.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        public static float ManhattanDistance(this Vector2 a, Vector2 b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

        /// <summary>
        /// Wraps an angle between -90 and 90 degrees. If an angle goes past this range it'll go back to the other end.
        /// </summary>
        /// <param name="theta"></param>
        /// <returns></returns>
        public static float WrapAngle90Degrees(float theta)
        {
            // Ensure that the angle starts off in the -180 to 180 degree range instead of the 0 to 360 degree range.
            if (theta > MathHelper.Pi)
                theta -= MathHelper.Pi;

            if (theta > MathHelper.PiOver2)
                theta -= MathHelper.Pi;
            if (theta < -MathHelper.PiOver2)
                theta += MathHelper.Pi;

            return theta;
        }

        /// <summary>
        /// Determines the angular distance between two vectors based on dot product comparisons. This method ensures underlying normalization is performed safely.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        public static float AngleBetween(this Vector2 v1, Vector2 v2) => (float)Math.Acos(Vector2.Dot(v1.SafeNormalize(Vector2.Zero), v2.SafeNormalize(Vector2.Zero)));

        // NOTE: A similar function to this one exists in 1.4, but it is not the same underlying function. Check Turn01ToCyclic010 in Utils.cs to see the effect.

        /// <summary>
        /// Converts a 0-1 bound to a 0-1-0 bump. This function automatically clamps the value to the necessary 0-1 range.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static float Convert01To010(float value) => (float)Math.Sin(MathHelper.Pi * MathHelper.Clamp(value, 0f, 1f));

        /// <summary>
        /// Uses a rewritten horizontal range formula to determine the direction to fire a projectile in order for it to hit a destination. Falls back on a certain value if no such direction can exist. If no fallback is provided, a clamp is used.
        /// </summary>
        /// <param name="shootingPosition">The starting position of the projectile.</param>
        /// <param name="destination">The destination for the projectile to land at.</param>
        /// <param name="gravity">The gravity of the projectile.</param>
        /// <param name="shootSpeed">The magnitude </param>
        /// <param name="nanFallback">The direction to fall back to if the calculations result in any NaNs. If nothing is specified, a clamp is performed to prevent any chance of NaNs at all.</param>
        public static Vector2 GetProjectilePhysicsFiringVelocity(Vector2 shootingPosition, Vector2 destination, float gravity, float shootSpeed, Vector2? nanFallback = null)
        {
            // Ensure that the gravity has the right sign for Terraria's coordinate system.
            gravity = -Math.Abs(gravity);

            float horizontalRange = MathHelper.Distance(shootingPosition.X, destination.X);
            float fireAngleSine = gravity * horizontalRange / (float)Math.Pow(shootSpeed, 2);

            // Clamp the sine if no fallback is provided.
            if (nanFallback is null)
                fireAngleSine = MathHelper.Clamp(fireAngleSine, -1f, 1f);

            float fireAngle = (float)Math.Asin(fireAngleSine) * 0.5f;

            // Get out of here if no valid firing angle exists. This can only happen if a fallback does indeed exist.
            if (float.IsNaN(fireAngle))
                return nanFallback.Value * shootSpeed;

            Vector2 fireVelocity = new Vector2(0f, -shootSpeed).RotatedBy(fireAngle);
            fireVelocity.X *= (destination.X - shootingPosition.X < 0).ToDirectionInt();
            return fireVelocity;
        }

        /// <summary>
        /// Calculates the shortest distance between a point and a line that passes through 2 specified points
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <returns></returns>
        public static float ShortestDistanceToLine(this Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2 lineVector = lineEnd - lineStart;
            Vector2 perpendicular = lineVector.RotatedBy(MathHelper.PiOver2);
            Vector2 pointToOrigin = point - lineStart;

            return (float)Math.Abs((pointToOrigin.X * perpendicular.X + pointToOrigin.Y * perpendicular.Y)) / perpendicular.Length();
        }

        /// <summary>
        /// Gets the closest point on a line from a point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <returns></returns>
        public static Vector2 ClosestPointOnLine(this Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {

            Vector2 perpendicular = (lineEnd - lineStart).RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
            float distanceToLine = point.ShortestDistanceToLine(lineStart, lineEnd);
            float lineSide = Math.Sign((point.X - lineStart.X) * ( -lineEnd.Y + lineStart.Y) + (point.Y - lineStart.Y) * (lineEnd.X - lineStart.X));

            return point + distanceToLine * lineSide * perpendicular;
        }

        /// <summary>
        /// Gives the *real* modulo of a divided by a divisor.
        /// This method is necessary because the % operator in c# keeps the sign of the dividend, making it Fake as Fuck.
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        public static float Modulo(this float dividend, float divisor)
        {
            return dividend - (float)Math.Floor(dividend / divisor) * divisor;
        }

        /// <summary>
        /// Clamps the magnitude of a vector via safe normalization.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="min">The minimum magnitude.</param>
        /// <param name="max">The maximum magnitude.</param>
        public static Vector2 ClampMagnitude(this Vector2 v, float min, float max) => v.SafeNormalize(Vector2.UnitY) * MathHelper.Clamp(v.Length(), min, max);

        /// <summary>
        /// Gives the angle in radians between two other angles
        /// This function exists for vectors but somehow is missing for floats
        /// </summary>
        /// <param name="angle">Your source angle</param>
        /// <param name="otherAngle">The target angle</param>
        /// <returns></returns>
        public static float AngleBetween(this float angle, float otherAngle) => ((otherAngle - angle) + MathHelper.Pi).Modulo(MathHelper.TwoPi) - MathHelper.Pi;

        /// <summary>
        /// Gets the sign of the number, but without the zero case. If 0 is inputted into this method, 1 is returned/
        /// </summary>
        /// <param name="x">The input value.</param>
        public static int DirectionalSign(this float x) => (x > 0f).ToDirectionInt();

        #region Easings
        /// <summary>
        /// Gets a value from 0 to 1 and returns an eased value.
        /// </summary>
        /// <param name="amount">How far along the easing are we</param>
        /// <returns></returns>
        public delegate float EasingFunction(float amount, int degree);

        public static float LinearEasing(float amount, int degree) => amount;
        //Sines
        public static float SineInEasing(float amount, int degree) => 1f - (float)Math.Cos(amount * MathHelper.Pi / 2f);
        public static float SineOutEasing(float amount, int degree) => (float)Math.Sin(amount * MathHelper.Pi / 2f);
        public static float SineInOutEasing(float amount, int degree) => -((float)Math.Cos(amount * MathHelper.Pi) - 1) / 2f;
        public static float SineBumpEasing(float amount, int degree) => (float)Math.Sin(amount * MathHelper.Pi);
        //Polynomials
        public static float PolyInEasing(float amount, int degree) => (float)Math.Pow(amount, degree);
        public static float PolyOutEasing(float amount, int degree) => 1f - (float)Math.Pow(1f - amount, degree);
        public static float PolyInOutEasing(float amount, int degree) => amount < 0.5f ? (float)Math.Pow(2, degree - 1) * (float)Math.Pow(amount, degree) : 1f - (float)Math.Pow(-2 * amount + 2, degree) / 2f;
        //Exponential
        public static float ExpInEasing(float amount, int degree) => amount == 0f ? 0f : (float)Math.Pow(2, 10f * amount - 10f);
        public static float ExpOutEasing(float amount, int degree) => amount == 1f ? 1f : 1f - (float)Math.Pow(2, -10f * amount);
        public static float ExpInOutEasing(float amount, int degree) => amount == 0f ? 0f : amount == 1f ? 1f : amount < 0.5f ? (float)Math.Pow(2, 20f * amount - 10f) / 2f : (2f - (float)Math.Pow(2, -20f * amount - 10f)) / 2f;
        //circular
        public static float CircInEasing(float amount, int degree) => (1f - (float)Math.Sqrt(1 - Math.Pow(amount, 2f)));
        public static float CircOutEasing(float amount, int degree) => (float)Math.Sqrt(1 - Math.Pow(amount - 1f, 2f));
        public static float CircInOutEasing(float amount, int degree) => amount < 0.5 ? (1f - (float)Math.Sqrt(1 - Math.Pow(2 * amount, 2f))) / 2f : ((float)Math.Sqrt(1 - Math.Pow(-2f * amount - 2f, 2f)) + 1f) / 2f;


        public enum EasingType //Potion seller. I need your strongest ease ins
        {
            Linear,
            SineIn, SineOut, SineInOut, SineBump,
            PolyIn, PolyOut, PolyInOut,
            ExpIn, ExpOut, ExpInOut,
            CircIn, CircOut, CircInOut
        }

        private static readonly EasingFunction[] EasingTypeToFunction = new EasingFunction[] { LinearEasing, SineInEasing, SineOutEasing, SineInOutEasing, SineBumpEasing, PolyInEasing, PolyOutEasing, PolyInOutEasing, ExpInEasing, ExpOutEasing, ExpInOutEasing, CircInEasing, CircOutEasing, CircInOutEasing};

        /// <summary>
        /// This represents a part of a piecewise function.
        /// </summary>
        public struct CurveSegment
        {
            /// <summary>
            /// This is the type of easing used in the segment
            /// </summary>
            public EasingFunction easing;
            /// <summary>
            /// This indicates when the segment starts on the animation
            /// </summary>
            public float startingX;
            /// <summary>
            /// This indicates what the starting height of the segment is
            /// </summary>
            public float startingHeight;
            /// <summary>
            /// This represents the elevation shift that will happen during the segment. Set this to 0 to turn the segment into a flat line.
            /// Usually this elevation shift is fully applied at the end of a segment, but the sinebump easing type makes it be reached at the apex of its curve.
            /// </summary>
            public float elevationShift;
            /// <summary>
            /// This is the degree of the polynomial, if the easing mode chosen is a polynomial one
            /// </summary>
            public int degree;

            /// <summary>
            /// The height of the segment after the elevation shift is taken into account.
            /// </summary>
            public float EndingHeight => startingHeight + elevationShift;

            /// <summary>
            /// Legacy constructor
            /// </summary>
            public CurveSegment(EasingType MODE, float startX, float startHeight, float elevationShift, int degree = 1) :
                this(EasingTypeToFunction[(int)MODE], startX, startHeight, elevationShift, degree)
            { }

            public CurveSegment(EasingFunction MODE, float startX, float startHeight, float elevationShift, int degree = 1)
            {
                easing = MODE;
                startingX = startX;
                startingHeight = startHeight;
                this.elevationShift = elevationShift;
                this.degree = degree;
            }
        }

        /// <summary>
        /// This gives you the height of a custom piecewise function for any given X value, so you may create your own complex animation curves easily.
        /// The X value is automatically clamped between 0 and 1, but the height of the function may go beyond the 0 - 1 range
        /// </summary>
        /// <param name="progress">How far along the curve you are. Automatically clamped between 0 and 1</param>
        /// <param name="segments">An array of curve segments making up the full animation curve</param>
        /// <returns></returns>
        public static float PiecewiseAnimation(float progress, params CurveSegment[] segments)
        {
            if (segments.Length == 0)
                return 0f;

            if (segments[0].startingX != 0) //If for whatever reason you try to not play by the rules, get fucked
                segments[0].startingX = 0;

            progress = MathHelper.Clamp(progress, 0f, 1f); //Clamp the progress
            float ratio = 0f;

            for (int i = 0; i <= segments.Length - 1; i++)
            {
                CurveSegment segment = segments[i];
                float startPoint = segment.startingX;
                float endPoint = 1f;

                if (progress < segment.startingX) //Too early. This should never get reached, since by the time you'd have gotten there you'd have found the appropriate segment and broken out of the for loop
                    continue;

                if (i < segments.Length - 1)
                {
                    if (segments[i + 1].startingX <= progress) //Too late
                        continue;
                    endPoint = segments[i + 1].startingX;
                }

                float segmentLength = endPoint - startPoint;
                float segmentProgress = (progress - segment.startingX) / segmentLength; //How far along the specific segment
                ratio = segment.startingHeight;

                //Failsafe because somehow it can fail? what
                if (segment.easing != null)
                    ratio += segment.easing(segmentProgress, segment.degree) * segment.elevationShift;

                else
                    ratio += LinearEasing(segmentProgress, segment.degree) * segment.elevationShift;

                break;
            }
            return ratio;
        }

        #endregion
    }
}
