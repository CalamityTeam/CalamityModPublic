using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityMod
{
    public class PrimitiveTrail
    {
        public struct VertexPosition2DColor : IVertexType
        {
            public Vector2 Position;
            public Color Color;
            public Vector2 TextureCoordinates;
            public VertexDeclaration VertexDeclaration => _vertexDeclaration;

            private static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[]
            {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            });
            public VertexPosition2DColor(Vector2 position, Color color, Vector2 textureCoordinates)
            {
                Position = position;
                Color = color;
                TextureCoordinates = textureCoordinates;
            }
        }

        public delegate float VertexWidthFunction(float completionRatio);
        public delegate Color VertexColorFunction(float completionRatio);

        public int DegreeOfBezierCurveCornerSmoothening;
        public VertexWidthFunction WidthFunction;
        public VertexColorFunction ColorFunction;

        public static BasicEffect BaseEffect;
        public MiscShaderData SpecialShader;
        public TrailPointRetrievalFunction TrailPointFunction;

        public Vector2 OverridingStickPointStart = Vector2.Zero;
        public Vector2 OverridingStickPointEnd = Vector2.Zero;

        public delegate List<Vector2> TrailPointRetrievalFunction(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int totalTrailPoints, IEnumerable<float> originalRotations = null);

        public PrimitiveTrail(VertexWidthFunction widthFunction, VertexColorFunction colorFunction, TrailPointRetrievalFunction pointFunction = null, MiscShaderData specialShader = null)
        {
            if (widthFunction is null || colorFunction is null)
                throw new NullReferenceException($"In order to create a primitive trail, a non-null {(widthFunction is null ? "width" : "color")} function must be specified.");
            WidthFunction = widthFunction;
            ColorFunction = colorFunction;

            // Default to bezier smoothening if nothing else is inputted.
            if (pointFunction is null)
                pointFunction = SmoothBezierPointRetreivalFunction;

            TrailPointFunction = pointFunction;

            if (specialShader != null)
                SpecialShader = specialShader;

            if (BaseEffect is null)
                BaseEffect = new BasicEffect(Main.instance.GraphicsDevice)
                {
                    VertexColorEnabled = true,
                    TextureEnabled = false
                };
            UpdateBaseEffect(out _, out _);
        }

        public void UpdateBaseEffect(out Matrix effectProjection, out Matrix effectView)
        {
            CalamityUtils.CalculatePerspectiveMatricies(out effectView, out effectProjection);

            BaseEffect.View = effectView;
            BaseEffect.Projection = effectProjection;
        }

        public static List<Vector2> RigidPointRetreivalFunction(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int totalTrailPoints, IEnumerable<float> _ = null)
        {
            List<Vector2> basePoints = originalPositions.Where(originalPosition => originalPosition != Vector2.Zero).ToList();
            List<Vector2> endPoints = new List<Vector2>();

            if (basePoints.Count < 2)
            {
                for (int i = 0; i < basePoints.Count; i++)
                    basePoints[i] += generalOffset;
                return basePoints;
            }

            // Remap the original positions across a certain length.
            for (int i = 0; i < totalTrailPoints; i++)
            {
                float completionRatio = i / (float)(totalTrailPoints - 1f);
                int currentIndex = (int)(completionRatio * (basePoints.Count - 1));
                Vector2 currentPoint = basePoints[currentIndex];
                Vector2 nextPoint = basePoints[(currentIndex + 1) % basePoints.Count];
                endPoints.Add(Vector2.Lerp(currentPoint, nextPoint, completionRatio * (basePoints.Count - 1) % 0.99999f) + generalOffset);
            }
            endPoints.Add(basePoints.Last() + generalOffset);
            return endPoints;
        }

        // NOTE: Beziers can be laggy when a lot of control points are used, since our implementation
        // uses a recursive Lerp that gets more computationally expensive the more original indices.
        // n(n + 1)/2 linear interpolations to be precise, where n is the amount of original indices.
        public List<Vector2> SmoothBezierPointRetreivalFunction(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int totalTrailPoints, IEnumerable<float> originalRotations = null)
        {
            List<Vector2> controlPoints = new List<Vector2>();
            for (int i = 0; i < originalPositions.Count(); i++)
            {
                // Don't incorporate points that are zeroed out.
                // They are almost certainly a result of incomplete oldPos arrays.
                if (originalPositions.ElementAt(i) == Vector2.Zero)
                    continue;
                controlPoints.Add(originalPositions.ElementAt(i) + generalOffset);
            }

            bool pointCountCondition = DegreeOfBezierCurveCornerSmoothening <= 0 || controlPoints.Count < DegreeOfBezierCurveCornerSmoothening * 3;
            int effectivePointCount = DegreeOfBezierCurveCornerSmoothening > 0 ? totalTrailPoints * DegreeOfBezierCurveCornerSmoothening : totalTrailPoints;
            BezierCurve bezierCurve = new BezierCurve(controlPoints.Where((_, i) =>
            {
                return DegreeOfBezierCurveCornerSmoothening <= 0 ||
                    i % DegreeOfBezierCurveCornerSmoothening == 0 ||
                    i == 0 ||
                    i == controlPoints.Count - 1 ||
                    pointCountCondition;
            }).ToArray());
            return controlPoints.Count <= 1 ? controlPoints : bezierCurve.GetPoints(effectivePointCount);
        }

        // Using this method requires oldRot be supplied to the Draw method for the sake of determining how many points need to be made at a given time step.
        // Furthermore, since the point count is dynamic, the point count int parameter has no purpose. It can be supplied simply with zero.
        public static List<Vector2> SmoothCatmullRomPointRetreivalFunction(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int _, IEnumerable<float> originalRotations)
        {
            List<Vector2> smoothenedPoints = new List<Vector2>();
            for (int i = 0; i < originalPositions.Count() - 1; i++)
            {
                Vector2 currentPosition = originalPositions.ElementAt(i);
                Vector2 aheadPosition = originalPositions.ElementAt(i + 1);

                // Don't incorporate points that are zeroed out.
                // They are almost certainly a result of incomplete oldPos arrays.
                if (currentPosition == Vector2.Zero || aheadPosition == Vector2.Zero)
                    continue;

                float currentRotation = MathHelper.WrapAngle(originalRotations.ElementAt(i));
                float aheadRotation = MathHelper.WrapAngle(originalRotations.ElementAt(i + 1));

                // Determine the amount of extra points to draw based on the rotational offset between the two time steps.
                int pointsToAdd = (int)Math.Round(Math.Abs(MathHelper.WrapAngle(aheadRotation - currentRotation)) * 8f / MathHelper.Pi);

                // Add a base point.
                smoothenedPoints.Add(currentPosition + generalOffset);

                // If no new points are needed, skip this.
                if (pointsToAdd == 0)
                    continue;

                float segmentLength = Vector2.Distance(currentPosition, aheadPosition);
                float increment = 1f / (pointsToAdd + 2);
                Vector2 backEnd = currentPosition + currentRotation.ToRotationVector2() * segmentLength;
                Vector2 frontEnd = aheadPosition + aheadRotation.ToRotationVector2() * -segmentLength;

                // Create smoothened points based on a Catmull-Rom spline.
                for (float j = increment; j < 1f; j += increment)
                    smoothenedPoints.Add(Vector2.CatmullRom(backEnd, currentPosition, aheadPosition, frontEnd, j) + generalOffset);
            }
            return smoothenedPoints;
        }

        public VertexPosition2DColor[] GetVerticesFromTrailPoints(List<Vector2> trailPoints)
        {
            VertexPosition2DColor[] vertices = new VertexPosition2DColor[trailPoints.Count * 2 - 2];
            for (int i = 0; i < trailPoints.Count - 1; i++)
            {
                float completionRatio = i / (float)trailPoints.Count;
                float widthAtVertex = WidthFunction(completionRatio);
                Color vertexColor = ColorFunction(completionRatio);

                Vector2 currentPosition = trailPoints[i];
                Vector2 positionAhead = trailPoints[i + 1];
                Vector2 directionToAhead = (positionAhead - trailPoints[i]).SafeNormalize(Vector2.Zero);

                Vector2 leftCurrentTextureCoord = new Vector2(completionRatio, 0f);
                Vector2 rightCurrentTextureCoord = new Vector2(completionRatio, 1f);

                // Point 90 degrees away from the direction towards the next point, and use it to mark the edges of the rectangle.
                // This doesn't use RotatedBy for the sake of performance (there can potentially be a lot of trail points).
                Vector2 sideDirection = new Vector2(-directionToAhead.Y, directionToAhead.X);

                Vector2 left = currentPosition - sideDirection * widthAtVertex;
                Vector2 right = currentPosition + sideDirection * widthAtVertex;
                if (i == 0 && OverridingStickPointStart != Vector2.Zero)
                {
                    left = OverridingStickPointStart;
                    right = OverridingStickPointEnd;
                }

                // What this is doing, at its core, is defining a rectangle based on two triangles.
                // These triangles are defined based on the width of the strip at that point.
                // The resulting rectangles combined are what make the trail itself.
                vertices[i * 2] = new VertexPosition2DColor(left, vertexColor, leftCurrentTextureCoord);
                vertices[i * 2 + 1] = new VertexPosition2DColor(right, vertexColor, rightCurrentTextureCoord);
            }

            return vertices.ToArray();
        }

        public short[] GetIndicesFromTrailPoints(int pointCount)
        {
            // What this is doing is basically representing each point on the vertices list as
            // indices. These indices should come together to create a tiny rectangle that acts
            // as a segment on the trail. This is achieved here by splitting the indices (or rather, points)
            // into 2 triangles, which requires 6 points.
            // The logic here basically determines which indices are connected together.
            int totalIndices = (pointCount - 1) * 6;
            short[] indices = new short[totalIndices];

            for (int i = 0; i < pointCount - 2; i++)
            {
                int startingTriangleIndex = i * 6;
                int connectToIndex = i * 2;
                indices[startingTriangleIndex] = (short)connectToIndex;
                indices[startingTriangleIndex + 1] = (short)(connectToIndex + 1);
                indices[startingTriangleIndex + 2] = (short)(connectToIndex + 2);
                indices[startingTriangleIndex + 3] = (short)(connectToIndex + 2);
                indices[startingTriangleIndex + 4] = (short)(connectToIndex + 1);
                indices[startingTriangleIndex + 5] = (short)(connectToIndex + 3);
            }

            return indices;
        }

        public void Draw(IEnumerable<Vector2> originalPositions, Vector2 generalOffset, int totalTrailPoints, IEnumerable<float> originalRotations = null)
        {
            List<Vector2> trailPoints = TrailPointFunction(originalPositions, generalOffset, totalTrailPoints, originalRotations);

            // A trail with only one point or less has nothing to connect to, and therefore, can't make a trail.
            if (trailPoints.Count < 2)
                return;

            // If the trail points have any NaN positions, don't draw anything.
            if (trailPoints.Any(point => point.HasNaNs()))
                return;

            // If the trail points are all equal, don't draw anything.
            if (trailPoints.All(point => point == trailPoints[0]))
                return;

            UpdateBaseEffect(out Matrix projection, out Matrix view);
            VertexPosition2DColor[] vertices = GetVerticesFromTrailPoints(trailPoints);

            short[] triangleIndices = GetIndicesFromTrailPoints(trailPoints.Count);

            // Don't draw anything if the indicies/vertices are in any way invalid.
            // If they are, the graphics engine, along with the entire game, will crash.
            if (triangleIndices.Length % 6 != 0 || vertices.Length % 2 != 0)
                return;

            Main.instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight); //offscreen culling

            if (SpecialShader != null)
            {
                SpecialShader.Shader.Parameters["uWorldViewProjection"].SetValue(view * projection);
                SpecialShader.Apply();
            }
            else
                BaseEffect.CurrentTechnique.Passes[0].Apply();

            Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, triangleIndices, 0, triangleIndices.Length / 3);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
}
