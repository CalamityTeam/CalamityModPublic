using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Primitives
{
    /// <summary>
    /// This manages rendering primitives in via the provided <see cref="RenderTrail"/> method.<br/><br/>
    /// <b>To use normally, in the draw method of an NPC or Projectile call <see cref="RenderTrail"/>.</b><br/>
    /// <list type="bullet">
    /// <item>The first param should be the positions to use for the trail.</item>
    /// <item>The second is a struct that allows you to choose a desired configuration of settings to use with the trail, to allow for customization.</item>
    /// <item>The third controls how many points are created for the trail, and the fourth determines whether the primitive is subsequently rendered.</item>
    /// </list>
    /// If you wish to use pixelation, you <b>MUST</b> make the NPC/Projectile inherit <see cref="IPixelatedPrimitiveRenderer"/> and use <see cref="IPixelatedPrimitiveRenderer.RenderPixelatedPrimitives"/> instead of predraw..<br/>
    /// You can also optionally specify a render layer with <see cref="IPixelatedPrimitiveRenderer.LayerToRenderTo"/>. It is <see cref="Enums.GeneralDrawLayer.BeforeNPCs"/> by default.
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public sealed class PrimitiveRenderer : ModSystem
    {
        #region Static Members
        private static DynamicVertexBuffer VertexBuffer;

        private static DynamicIndexBuffer IndexBuffer;

        private static PrimitiveSettings MainSettings;

        private static PrimitiveTopology ActiveTopology;

        private static Vector2[] MainPositions;

        private static Vector2[] MainTangents;

        private static Vector2[] MainNormals;

        private static VertexPosition2DColorTexture[] MainVertices;

        private static short[] MainIndices;

        private static VertexPositionColor[] WireframeVertices;

        private static int WireframeVertexCount;

        private static BasicEffect WireframeEffect;

        private static int[] NonSmoothIndexScratch;

        private static short StartCapCenterIndex;

        private static short EndCapCenterIndex;

        private const short MaxPositions = 1000;

        private const short MaxVertices = 3072;

        private const short MaxIndices = 8192;

        private static readonly List<Vector2> ControlPointsCache = new(MaxPositions);

        private static short PositionsIndex;

        private static float[] MainCompletionRatios;

        private static float TotalTrailLength;

        public const float Epsilon = 1e-6f;

        private static short VerticesIndex;

        private static short IndicesIndex;

        public override void OnModLoad()
        {
            Main.QueueMainThreadAction(() =>
            {
                MainPositions = new Vector2[MaxPositions];
                MainVertices = new VertexPosition2DColorTexture[MaxVertices];
                MainIndices = new short[MaxIndices];
                MainCompletionRatios = new float[MaxPositions];
                MainTangents = new Vector2[MaxPositions];
                MainNormals = new Vector2[MaxPositions];
                WireframeVertices = new VertexPositionColor[MaxPositions * 8];
                NonSmoothIndexScratch = new int[MaxPositions];
                VertexBuffer ??= new DynamicVertexBuffer(Main.instance.GraphicsDevice, VertexPosition2DColorTexture.VertexDeclaration2D, MaxVertices, BufferUsage.WriteOnly);
                IndexBuffer ??= new DynamicIndexBuffer(Main.instance.GraphicsDevice, IndexElementSize.SixteenBits, MaxIndices, BufferUsage.WriteOnly);
                WireframeEffect ??= new BasicEffect(Main.instance.GraphicsDevice)
                {
                    VertexColorEnabled = true,
                    TextureEnabled = false,
                    LightingEnabled = false
                };
            });
        }

        public override void OnModUnload()
        {
            Main.QueueMainThreadAction(() =>
            {
                MainPositions = null;
                MainVertices = null;
                MainIndices = null;
                MainCompletionRatios = null;
                MainTangents = null;
                MainNormals = null;
                WireframeVertices = null;
                NonSmoothIndexScratch = null;
                VertexBuffer?.Dispose();
                VertexBuffer = null;
                IndexBuffer?.Dispose();
                IndexBuffer = null;
                WireframeEffect?.Dispose();
                WireframeEffect = null;
            });
        }

        private static void PerformPixelationSafetyChecks(PrimitiveSettings settings)
        {
            // Don't allow accidental screw ups with these.
            if (settings.Pixelate && !PrimitivePixelationSystem.CurrentlyRendering)
                throw new Exception("Error: Primitives using pixelation MUST be prepared/rendered from the IPixelatedPrimitiveRenderer.RenderPixelatedPrimitives method, did you forget to use the interface?");
            else if (!settings.Pixelate && PrimitivePixelationSystem.CurrentlyRendering)
                throw new Exception("Error: Primitives not using pixelation MUST NOT be prepared/rendered from the IPixelatedPrimitiveRenderer.RenderPixelatedPrimitives method.");
        }

        /// <summary>
        /// Renders a primitive trail.
        /// </summary>
        /// <param name="positions">The list of positions to use. Keep in mind that these are expected to be in <b>world position</b>, and <see cref="Main.screenPosition"/> is automatically subtracted from them all.<br/>At least 4 points are required to use smoothing.</param>
        /// <param name="settings">The primitive draw settings to use.</param>
        /// <param name="pointsToCreate">The amount of points to use. More is higher detailed, but less performant. By default, is the number of positions provided. <b>Going above 100 is NOT recommended.</b></param>
        public static void RenderTrail(List<Vector2> positions, PrimitiveSettings settings, int? pointsToCreate = null) => RenderTrail(positions.ToArray(), settings, pointsToCreate);

        /// <summary>
        /// Renders a primitive trail.
        /// </summary>
        /// <param name="positions">The list of positions to use. Keep in mind that these are expected to be in <b>world position</b>, and <see cref="Main.screenPosition"/> is automatically subtracted from them all.<br/>At least 4 points are required to use smoothing.</param>
        /// <param name="settings">The primitive draw settings to use.</param>
        /// <param name="pointsToCreate">The amount of points to use. More is higher detailed, but less performant. By default, is the number of positions provided. <b>Going above 100 is NOT recommended.</b></param>
        public static void RenderTrail(Vector2[] positions, PrimitiveSettings settings, int? pointsToCreate = null)
        {
            PerformPixelationSafetyChecks(settings);

            // Return if not enough to draw anything.
            if (positions.Length <= 2)
                return;

            // Return if too many to draw anything,
            if (positions.Length > MaxPositions)
                return;

            int desiredPointCount = pointsToCreate ?? positions.Length;
            desiredPointCount = Math.Clamp(desiredPointCount, 2, MaxPositions);

            MainSettings = settings;
            ActiveTopology = settings.CapStyle != PrimitiveCapStyle.None ? PrimitiveTopology.TriangleList : settings.Topology;

            // IF this is false, a correct position trail could not be made and rendering should not continue.
            if (!AssignPointsRectangleTrail(positions, settings, desiredPointCount))
                return;

            // A trail with only one point or less has nothing to connect to, and therefore, can't make a trail.
            AssignCompletionData();

            if (PositionsIndex <= 2)
                return;

            AssignVerticesRectangleTrail();
            AssignIndices();

            // Else render without wasting resources creating a set.
            PrivateRender();
            return;
        }

        private static void PrivateRender()
        {
            if (VerticesIndex <= 3)
                return;

            if (ActiveTopology == PrimitiveTopology.TriangleList)
            {
                if (IndicesIndex < 6 || IndicesIndex % 3 != 0)
                    return;
            }
            else if (ActiveTopology == PrimitiveTopology.TriangleStrip && IndicesIndex < 4)
                return;

            // Perform screen culling, for performance reasons.
            Main.instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            Matrix view;
            Matrix projection;
            if (MainSettings.Pixelate || MainSettings.UseUnscaledMatrices)
                CalcuatePixelatedPerspectiveMatrices(out view, out projection);
            else
                CalamityUtils.CalculatePerspectiveMatricies(out view, out projection);

            var shaderToUse = MainSettings.Shader ?? GameShaders.Misc["CalamityMod:StandardPrimitiveShader"];
            shaderToUse.Shader.Parameters["uWorldViewProjection"].SetValue(view * projection);
            shaderToUse.Apply();

            VertexBuffer.SetData(MainVertices, 0, VerticesIndex, SetDataOptions.Discard);
            IndexBuffer.SetData(MainIndices, 0, IndicesIndex, SetDataOptions.Discard);

            Main.instance.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            Main.instance.GraphicsDevice.Indices = IndexBuffer;

            PrimitiveType primitiveType = ActiveTopology == PrimitiveTopology.TriangleStrip ? PrimitiveType.TriangleStrip : PrimitiveType.TriangleList;
            int primitiveCount = primitiveType == PrimitiveType.TriangleStrip ? Math.Max(IndicesIndex - 2, 0) : IndicesIndex / 3;
            Main.instance.GraphicsDevice.DrawIndexedPrimitives(primitiveType, 0, 0, VerticesIndex, 0, primitiveCount);

            if (MainSettings.DebugWireframe && WireframeEffect != null && TryBuildWireframeGeometry(MainSettings.WireframeColor, out int lineCount))
                DrawWireframe(view, projection, lineCount);
        }
        #endregion

        #region Set Preperation
        private static bool AssignPointsRectangleTrail(Vector2[] positions, PrimitiveSettings settings, int pointsToCreate)
        {
            // Don't smoothen the points unless explicitly told do so.
            if (!settings.Smoothen)
            {
                PositionsIndex = 0;

                int validCount = 0;
                for (int i = 0; i < positions.Length; i++)
                {
                    if (positions[i] == Vector2.Zero)
                        continue;

                    NonSmoothIndexScratch[validCount++] = i;
                }

                if (validCount <= 2)
                    return false;

                int lastIndex = validCount - 1;
                float inversePointCount = 1f / (pointsToCreate - 1);
                float lastIndexFloat = lastIndex;

                // Remap the original positions across a certain length without additional allocations.
                for (int i = 0; i < pointsToCreate; i++)
                {
                    float completionRatio = i * inversePointCount;
                    float scaledIndex = completionRatio * lastIndexFloat;
                    int currentIndex = (int)scaledIndex;
                    int nextIndex = Math.Min(currentIndex + 1, lastIndex);
                    float localInterpolant = scaledIndex - currentIndex;

                    Vector2 currentPoint = positions[NonSmoothIndexScratch[currentIndex]];
                    Vector2 nextPoint = positions[NonSmoothIndexScratch[nextIndex]];
                    Vector2 interpolatedWorld = Vector2.Lerp(currentPoint, nextPoint, localInterpolant);
                    Vector2 finalPos = interpolatedWorld - Main.screenPosition;
                    if (settings.OffsetFunction != null)
                        finalPos += settings.OffsetFunction(completionRatio, interpolatedWorld);

                    MainPositions[PositionsIndex++] = finalPos;
                }
                return true;
            }

            PositionsIndex = 0;

            // Create the control points for the spline.
            List<Vector2> controlPoints = ControlPointsCache;
            controlPoints.Clear();
            for (int i = 0; i < positions.Length; i++)
            {
                if (positions[i] == Vector2.Zero)
                    continue;

                float completionRatio = i / (float)positions.Length;
                Vector2 offset = -Main.screenPosition;
                if (settings.OffsetFunction != null)
                    offset += settings.OffsetFunction(completionRatio, positions[i]);
                controlPoints.Add(positions[i] + offset);
            }

            int controlCount = controlPoints.Count;
            if (controlCount <= 1)
            {
                controlPoints.Clear();
                return false;
            }

            int segmentCount = controlCount - 1;

            if (settings.SmoothingSegments > 0)
            {
                int segmentsPerEdge = Math.Max(1, settings.SmoothingSegments);
                PrimitiveSmoothingType smoothingType = settings.SmoothingType;

                for (int segment = 0; segment < segmentCount; segment++)
                {
                    Vector2 p0 = controlPoints[Math.Max(segment - 1, 0)];
                    Vector2 p1 = controlPoints[segment];
                    Vector2 p2 = controlPoints[segment + 1];
                    Vector2 p3 = controlPoints[Math.Min(segment + 2, controlCount - 1)];

                    for (int step = segment == 0 ? 0 : 1; step <= segmentsPerEdge; step++)
                    {
                        if (PositionsIndex >= MaxPositions - 1)
                        {
                            controlPoints.Clear();
                            return true;
                        }

                        float localT = step / (float)segmentsPerEdge;
                        Vector2 point = EvaluateCurve(smoothingType, p0, p1, p2, p3, localT, settings);
                        MainPositions[PositionsIndex++] = point;
                    }
                }

                controlPoints.Clear();
                return true;
            }

            // Legacy behaviour: sample based on requested point count using Catmull-Rom style interpolation.
            PositionsIndex = 0;
            float controlCountMinusOne = controlCount - 1f;
            PrimitiveSmoothingType legacyType = settings.SmoothingType;

            for (int j = 0; j < pointsToCreate; j++)
            {
                if (PositionsIndex >= MaxPositions - 1)
                    break;

                float splineInterpolant = j / (float)pointsToCreate;
                float positionOnCurve = splineInterpolant * controlCountMinusOne;
                int localSplineIndex = (int)positionOnCurve;
                float localSplineInterpolant = positionOnCurve - localSplineIndex;

                Vector2 p0 = controlPoints[Math.Max(localSplineIndex - 1, 0)];
                Vector2 p1 = controlPoints[localSplineIndex];
                Vector2 p2 = controlPoints[Math.Min(localSplineIndex + 1, controlCount - 1)];
                Vector2 p3 = controlPoints[Math.Min(localSplineIndex + 2, controlCount - 1)];

                MainPositions[PositionsIndex] = EvaluateCurve(legacyType, p0, p1, p2, p3, localSplineInterpolant, settings);
                PositionsIndex++;
            }
            MainPositions[PositionsIndex] = controlPoints[controlCount - 1];
            PositionsIndex++;
            controlPoints.Clear();
            return true;
        }

        private static void AssignCompletionData()
        {
            TotalTrailLength = 0f;

            if (PositionsIndex <= 0)
                return;

            MainCompletionRatios[0] = 0f;

            for (int i = 1; i < PositionsIndex; i++)
            {
                float segmentLength = Vector2.Distance(MainPositions[i], MainPositions[i - 1]);
                TotalTrailLength += segmentLength;
                MainCompletionRatios[i] = TotalTrailLength;
            }

            if (PositionsIndex <= 0)
                return;

            if (TotalTrailLength > Epsilon)
            {
                float inverseTotal = 1f / TotalTrailLength;
                int lastIndex = PositionsIndex - 1;

                if (System.Numerics.Vector.IsHardwareAccelerated && PositionsIndex - 1 >= System.Numerics.Vector<float>.Count)
                {
                    var scale = new System.Numerics.Vector<float>(inverseTotal);
                    int i = 1;
                    int upperBound = lastIndex - System.Numerics.Vector<float>.Count + 1;
                    for (; i <= upperBound; i += System.Numerics.Vector<float>.Count)
                    {
                        var values = new System.Numerics.Vector<float>(MainCompletionRatios, i);
                        (values * scale).CopyTo(MainCompletionRatios, i);
                    }

                    for (; i <= lastIndex; i++)
                        MainCompletionRatios[i] *= inverseTotal;
                }
                else
                {
                    for (int i = 1; i < PositionsIndex; i++)
                        MainCompletionRatios[i] *= inverseTotal;
                }

                MainCompletionRatios[PositionsIndex - 1] = 1f;
            }
            else
            {
                for (int i = 1; i < PositionsIndex; i++)
                    MainCompletionRatios[i] = 0f;
            }
        }

        private static void AssignVerticesRectangleTrail()
        {
            VerticesIndex = 0;
            StartCapCenterIndex = -1;
            EndCapCenterIndex = -1;
            ComputeFrameData();
            for (int i = 0; i < PositionsIndex; i++)
            {
                float completionRatio = GetCompletionRatioForIndex(i);
                float widthAtVertex = Math.Max(MainSettings.WidthFunction(completionRatio, MainPositions[i]), 0f);
                Color vertexColor = MainSettings.ColorFunction(completionRatio, MainPositions[i]);
                float textureU = ComputeTextureCoordinateForIndex(i, completionRatio);

                ComputeEdgePositions(i, widthAtVertex, out Vector2 left, out Vector2 right, out float effectiveHalfWidth);

                // Override the initial vertex positions if requested.
                if (i == 0 && MainSettings.InitialVertexPositionsOverride.HasValue && MainSettings.InitialVertexPositionsOverride.Value.Item1 != Vector2.Zero && MainSettings.InitialVertexPositionsOverride.Value.Item2 != Vector2.Zero)
                {
                    left = MainSettings.InitialVertexPositionsOverride.Value.Item1;
                    right = MainSettings.InitialVertexPositionsOverride.Value.Item2;
                    effectiveHalfWidth = Math.Max(Vector2.Distance(left, right) * 0.5f, Epsilon);
                }

                // Guard against degenerate width
                effectiveHalfWidth = Math.Max(effectiveHalfWidth, Epsilon);

                Vector2 leftCurrentTextureCoord = new Vector2(textureU, 0.5f - effectiveHalfWidth * 0.5f);
                Vector2 rightCurrentTextureCoord = new Vector2(textureU, 0.5f + effectiveHalfWidth * 0.5f);

                MainVertices[VerticesIndex] = new VertexPosition2DColorTexture(left, vertexColor, leftCurrentTextureCoord, effectiveHalfWidth);
                VerticesIndex++;
                MainVertices[VerticesIndex] = new VertexPosition2DColorTexture(right, vertexColor, rightCurrentTextureCoord, effectiveHalfWidth);
                VerticesIndex++;
            }

            AddCaps();
        }

        private static void AddCaps()
        {
            if (MainSettings.CapStyle == PrimitiveCapStyle.None || PositionsIndex <= 0)
                return;

            if (ActiveTopology == PrimitiveTopology.TriangleStrip)
                return;

            StartCapCenterIndex = TryCreateCapVertex(0);
            if (PositionsIndex > 1)
                EndCapCenterIndex = TryCreateCapVertex(PositionsIndex - 1);
        }

        private static short TryCreateCapVertex(int positionIndex)
        {
            if (VerticesIndex >= MaxVertices - 1)
                return -1;

            int leftVertexIndex = positionIndex * 2;
            int rightVertexIndex = leftVertexIndex + 1;
            if (rightVertexIndex >= VerticesIndex)
                return -1;

            ref readonly VertexPosition2DColorTexture leftVertex = ref MainVertices[leftVertexIndex];
            ref readonly VertexPosition2DColorTexture rightVertex = ref MainVertices[rightVertexIndex];

            Vector2 centerPosition = MainPositions[positionIndex];
            Color centerColor = Color.Lerp(leftVertex.Color, rightVertex.Color, 0.5f);
            float centerHalfWidth = Math.Max(Math.Max(leftVertex.TextureCoordinates.Z, rightVertex.TextureCoordinates.Z), Epsilon);
            float centerU = (leftVertex.TextureCoordinates.X + rightVertex.TextureCoordinates.X) * 0.5f;
            Vector2 centerTexcoord = new(centerU, 0.5f);

            short newVertexIndex = VerticesIndex;
            MainVertices[VerticesIndex++] = new VertexPosition2DColorTexture(centerPosition, centerColor, centerTexcoord, centerHalfWidth);
            return newVertexIndex;
        }

        private static float GetCompletionRatioForIndex(int index)
        {
            if (PositionsIndex <= 0)
                return 0f;

            if (index <= 0)
                return MainCompletionRatios[0];

            if (index >= PositionsIndex)
                return MainCompletionRatios[PositionsIndex - 1];

            return MainCompletionRatios[index];
        }

        private static float ComputeTextureCoordinateForIndex(int index, float completionRatio)
        {
            float clampedCompletion = MathHelper.Clamp(completionRatio, 0f, 1f);

            if (MainSettings.TextureCoordinateFunction != null)
                return MainSettings.TextureCoordinateFunction(clampedCompletion);

            float cycleLength = MainSettings.TextureCycleLength;
            if (Math.Abs(cycleLength) <= Epsilon)
                cycleLength = cycleLength >= 0f ? 1f : -1f;

            switch (MainSettings.TextureCoordinateMode)
            {
                case PrimitiveTextureMode.Distance:
                    float distance = clampedCompletion * TotalTrailLength + MainSettings.TextureScrollOffset;
                    return distance / cycleLength;

                default:
                    return clampedCompletion * cycleLength + MainSettings.TextureScrollOffset;
            }
        }

        private static void ComputeFrameData()
        {
            if (PositionsIndex <= 0)
                return;

            Vector2 fallbackTangent = Vector2.UnitX;

            for (int i = 0; i < PositionsIndex; i++)
            {
                Vector2 tangent = ComputeTangent(i, fallbackTangent);
                tangent = tangent.SafeNormalize(fallbackTangent.SafeNormalize(Vector2.UnitX));
                MainTangents[i] = tangent;
                fallbackTangent = tangent;
            }

            Vector2 previousNormal = Vector2.Zero;

            for (int i = 0; i < PositionsIndex; i++)
            {
                Vector2 tangent = MainTangents[i];
                if (tangent.LengthSquared() <= Epsilon)
                    tangent = fallbackTangent.SafeNormalize(Vector2.UnitX);

                Vector2 baseNormal = new Vector2(-tangent.Y, tangent.X);
                Vector2 normal;

                if (MainSettings.FrameTransportMode == PrimitiveFrameTransportMode.ParallelTransport && i > 0 && previousNormal.LengthSquared() > Epsilon)
                {
                    Vector2 previousTangent = MainTangents[i - 1];
                    float cosine = MathHelper.Clamp(Vector2.Dot(previousTangent, tangent), -1f, 1f);
                    float sine = Cross(previousTangent, tangent);
                    Vector2 transported = new Vector2(
                        cosine * previousNormal.X - sine * previousNormal.Y,
                        sine * previousNormal.X + cosine * previousNormal.Y
                    );
                    normal = transported;
                }
                else
                    normal = baseNormal;

                if (normal.LengthSquared() <= Epsilon)
                    normal = previousNormal.LengthSquared() > Epsilon ? previousNormal : baseNormal;

                normal = normal.SafeNormalize(previousNormal.LengthSquared() > Epsilon ? previousNormal : Vector2.UnitY);
                MainNormals[i] = normal;
                previousNormal = normal;
            }
        }

        private static Vector2 ComputeTangent(int index, Vector2 fallback)
        {
            int last = PositionsIndex - 1;
            Vector2 tangent;

            if (PositionsIndex <= 1)
            {
                tangent = fallback;
            }
            else if (index <= 0)
            {
                tangent = MainPositions[1] - MainPositions[0];
            }
            else if (index >= last)
            {
                tangent = MainPositions[last] - MainPositions[last - 1];
            }
            else
            {
                Vector2 forward = MainPositions[index + 1] - MainPositions[index];
                Vector2 backward = MainPositions[index] - MainPositions[index - 1];
                tangent = forward + backward;

                if (tangent.LengthSquared() <= Epsilon)
                    tangent = forward.LengthSquared() >= backward.LengthSquared() ? forward : backward;
            }

            if (tangent.LengthSquared() <= Epsilon)
                tangent = fallback.LengthSquared() > Epsilon ? fallback : Vector2.UnitX;

            return tangent;
        }

        private static Vector2 EvaluateCurve(PrimitiveSmoothingType type, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t, PrimitiveSettings settings)
        {
            t = MathHelper.Clamp(t, 0f, 1f);

            switch (type)
            {
                case PrimitiveSmoothingType.Linear:
                    return Vector2.Lerp(p1, p2, t);

                case PrimitiveSmoothingType.Cardinal:
                    {
                        float tension = MathHelper.Clamp(settings.SmoothingTension, -1f, 1f);
                        float scale = (1f - tension) * 0.5f;
                        Vector2 m0 = (p2 - p0) * scale;
                        Vector2 m1 = (p3 - p1) * scale;
                        return EvaluateHermiteSpan(p1, p2, m0, m1, t);
                    }

                case PrimitiveSmoothingType.Hermite:
                    {
                        Vector2 m0 = 0.5f * (p2 - p0);
                        Vector2 m1 = 0.5f * (p3 - p1);
                        return EvaluateHermiteSpan(p1, p2, m0, m1, t);
                    }

                case PrimitiveSmoothingType.CubicBezier:
                    {
                        Vector2 handle1 = p1 + (p2 - p0) / 3f;
                        Vector2 handle2 = p2 - (p3 - p1) / 3f;
                        return EvaluateBezierSpan(p1, handle1, handle2, p2, t);
                    }

                default:
                    return Vector2.CatmullRom(p0, p1, p2, p3, t);
            }
        }

        private static Vector2 EvaluateHermiteSpan(Vector2 start, Vector2 end, Vector2 tangentStart, Vector2 tangentEnd, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            float h00 = 2f * t3 - 3f * t2 + 1f;
            float h10 = t3 - 2f * t2 + t;
            float h01 = -2f * t3 + 3f * t2;
            float h11 = t3 - t2;
            return h00 * start + h10 * tangentStart + h01 * end + h11 * tangentEnd;
        }

        private static Vector2 EvaluateBezierSpan(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1f - t;
            float u2 = u * u;
            float u3 = u2 * u;
            float t2 = t * t;
            float t3 = t2 * t;
            return u3 * p0 + 3f * u2 * t * p1 + 3f * u * t2 * p2 + t3 * p3;
        }

        private static void ComputeEdgePositions(int index, float halfWidth, out Vector2 left, out Vector2 right, out float effectiveHalfWidth)
        {
            Vector2 currentPosition = MainPositions[index];

            if (halfWidth <= 0f)
            {
                left = currentPosition;
                right = currentPosition;
                effectiveHalfWidth = Epsilon;
                return;
            }

            Vector2 defaultNormal = MainNormals[index];
            if (defaultNormal.LengthSquared() <= Epsilon)
                defaultNormal = Vector2.UnitY;

            if (MainSettings.JoinStyle == PrimitiveJoinStyle.Flat || PositionsIndex <= 2 || index == 0 || index == PositionsIndex - 1)
            {
                Vector2 offset = defaultNormal * halfWidth;
                left = currentPosition - offset;
                right = currentPosition + offset;
                effectiveHalfWidth = halfWidth;
                return;
            }

            Vector2 prevNormal = MainNormals[Math.Max(index - 1, 0)];
            if (prevNormal.LengthSquared() <= Epsilon)
                prevNormal = defaultNormal;

            Vector2 nextNormal = MainNormals[Math.Min(index + 1, PositionsIndex - 1)];
            if (nextNormal.LengthSquared() <= Epsilon)
                nextNormal = defaultNormal;

            switch (MainSettings.JoinStyle)
            {
                case PrimitiveJoinStyle.Smooth:
                    {
                        Vector2 averageNormal = (prevNormal + defaultNormal + nextNormal) * (1f / 3f);
                        if (averageNormal.LengthSquared() <= Epsilon)
                            averageNormal = defaultNormal;

                        Vector2 offset = averageNormal.SafeNormalize(defaultNormal) * halfWidth;
                        left = currentPosition - offset;
                        right = currentPosition + offset;
                        effectiveHalfWidth = halfWidth;
                        return;
                    }

                case PrimitiveJoinStyle.Miter:
                    {
                        Vector2 prev = prevNormal.SafeNormalize(defaultNormal);
                        Vector2 next = nextNormal.SafeNormalize(defaultNormal);
                        Vector2 miter = prev + next;
                        if (miter.LengthSquared() <= Epsilon)
                            miter = defaultNormal;

                        miter = miter.SafeNormalize(defaultNormal);
                        float denom = Vector2.Dot(miter, next);
                        if (Math.Abs(denom) < Epsilon)
                            denom = denom >= 0f ? Epsilon : -Epsilon;

                        float miterLength = halfWidth / denom;
                        float maxLength = halfWidth * MainSettings.JoinMiterLimit;
                        miterLength = MathHelper.Clamp(miterLength, -maxLength, maxLength);
                        Vector2 offset = miter * miterLength;
                        left = currentPosition - offset;
                        right = currentPosition + offset;
                        effectiveHalfWidth = Math.Max(Math.Abs(miterLength), Epsilon);
                        return;
                    }

                default:
                    {
                        Vector2 offset = defaultNormal * halfWidth;
                        left = currentPosition - offset;
                        right = currentPosition + offset;
                        effectiveHalfWidth = halfWidth;
                        return;
                    }
            }
        }

        private static bool TryBuildWireframeGeometry(Color lineColor, out int lineCount)
        {
            lineCount = 0;

            if (WireframeVertices == null)
                return false;

            int segments = PositionsIndex - 1;
            if (segments <= 0)
                return false;

            WireframeVertexCount = 0;

            for (int i = 0; i < segments; i++)
            {
                int currentLeft = i * 2;
                int currentRight = currentLeft + 1;
                int nextLeft = currentLeft + 2;
                int nextRight = nextLeft + 1;

                AddWireframeLineFromIndices(currentLeft, nextLeft, lineColor);
                AddWireframeLineFromIndices(currentRight, nextRight, lineColor);
            }

            for (int i = 0; i < PositionsIndex; i++)
            {
                int leftIndex = i * 2;
                AddWireframeLineFromIndices(leftIndex, leftIndex + 1, lineColor);
            }

            lineCount = WireframeVertexCount / 2;
            return lineCount > 0;
        }

        private static void AddWireframeLineFromIndices(int startVertexIndex, int endVertexIndex, Color color)
        {
            if (WireframeVertexCount + 1 >= WireframeVertices.Length)
                return;

            int vertexCount = VerticesIndex;
            if (startVertexIndex >= vertexCount || endVertexIndex >= vertexCount)
                return;

            ref readonly VertexPosition2DColorTexture start = ref MainVertices[startVertexIndex];
            ref readonly VertexPosition2DColorTexture end = ref MainVertices[endVertexIndex];
            WireframeVertices[WireframeVertexCount++] = new VertexPositionColor(new Vector3(start.Position, 0f), color);
            WireframeVertices[WireframeVertexCount++] = new VertexPositionColor(new Vector3(end.Position, 0f), color);
        }

        private static void DrawWireframe(Matrix view, Matrix projection, int lineCount)
        {
            if (lineCount <= 0 || WireframeEffect == null)
                return;

            WireframeEffect.World = Matrix.Identity;
            WireframeEffect.View = view;
            WireframeEffect.Projection = projection;

            var device = Main.instance.GraphicsDevice;
            foreach (EffectPass pass in WireframeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives(PrimitiveType.LineList, WireframeVertices, 0, lineCount);
            }
        }

        private static void AssignIndices()
        {
            IndicesIndex = 0;

            if (ActiveTopology == PrimitiveTopology.TriangleStrip)
            {
                for (short i = 0; i < VerticesIndex && IndicesIndex < MaxIndices; i++)
                    MainIndices[IndicesIndex++] = i;
                return;
            }

            // What this is doing is basically representing each point on the vertices list as
            // indices. These indices should come together to create a tiny rectangle that acts
            // as a segment on the trail. This is achieved here by splitting the indices (or rather, points)
            // into 2 triangles, which requires 6 points.
            // The logic here basically determines which indices are connected together.
            for (short i = 0; i < PositionsIndex - 2 && IndicesIndex + 5 < MaxIndices; i++)
            {
                short connectToIndex = (short)(i * 2);
                MainIndices[IndicesIndex] = connectToIndex;
                IndicesIndex++;

                MainIndices[IndicesIndex] = (short)(connectToIndex + 1);
                IndicesIndex++;

                MainIndices[IndicesIndex] = (short)(connectToIndex + 2);
                IndicesIndex++;

                MainIndices[IndicesIndex] = (short)(connectToIndex + 2);
                IndicesIndex++;

                MainIndices[IndicesIndex] = (short)(connectToIndex + 1);
                IndicesIndex++;

                MainIndices[IndicesIndex] = (short)(connectToIndex + 3);
                IndicesIndex++;
            }

            AppendCapTriangles();
        }

        private static void AppendCapTriangles()
        {
            if (MainSettings.CapStyle == PrimitiveCapStyle.None)
                return;

            if (PositionsIndex <= 0)
                return;

            if (StartCapCenterIndex >= 0)
                AddTriangle((short)(StartCapCenterIndex), 0, 1);

            if (EndCapCenterIndex >= 0)
            {
                short leftIndex = (short)((PositionsIndex - 1) * 2);
                short rightIndex = (short)(leftIndex + 1);
                AddTriangle(leftIndex, rightIndex, EndCapCenterIndex);
            }
        }

        private static void AddTriangle(short i0, short i1, short i2)
        {
            if (IndicesIndex + 2 >= MaxIndices)
                return;

            MainIndices[IndicesIndex++] = i0;
            MainIndices[IndicesIndex++] = i1;
            MainIndices[IndicesIndex++] = i2;
        }

        private static void CalcuatePixelatedPerspectiveMatrices(out Matrix viewMatrix, out Matrix projectionMatrix)
        {
            // Due to the scaling, the normal transformation calcuations do not work with pixelated primitives.
            projectionMatrix = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);
            viewMatrix = Matrix.Identity;
        }

        private static float Cross(Vector2 a, Vector2 b) => a.X * b.Y - a.Y * b.X;
        #endregion
    }
}
