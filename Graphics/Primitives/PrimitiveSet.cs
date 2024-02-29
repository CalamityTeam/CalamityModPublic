using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Graphics.Primitives
{
    /// <summary>
    /// This both acts as an instanceable struct containing primitive trail data, and as a manager that creates and renders said instances. <br/><br/>
    /// <b>To use normally, in the draw method of an NPC or Projectile call <see cref="Prepare"/>.</b><br/>
    /// <list type="bullet">
    /// <item>The first param should be the positions to use for the trail.</item>
    /// <item>The second is a struct that allows you to choose a desired configuration of settings to use with the trail, to allow for customization.</item>
    /// <item>The third controls how many points are created for the trail, and the fourth determines whether the primitive is subsequently rendered.</item>
    /// <item>If you do not render in <see cref="Prepare"/>, you <b>MUST</b> call <see cref="Render"/> manually on the created instance to make it actually appear.</item>
    /// </list>
    /// If you wish to use pixelation, you <b>MUST</b> make the NPC/Projectile inherit <see cref="IPixelatedPrimitiveRenderer"/> and use <see cref="IPixelatedPrimitiveRenderer.RenderPixelatedPrimitives"/> instead.<br/>
    /// You can also optionally specify a render layer with <see cref="IPixelatedPrimitiveRenderer.LayerToRenderTo"/>. It is <see cref="PixelationPrimitiveLayer.BeforeNPCs"/> by default.
    /// </summary>
    public readonly struct PrimitiveSet
    {
        #region Instance Members
        /// <summary>
        /// The vertices of the set.
        /// </summary>
        public readonly VertexPosition2DColorTexture[] Vertices;

        /// <summary>
        /// The indices of the set.
        /// </summary>
        public readonly int[] Indices;

        /// <summary>
        /// The settings of the set.
        /// </summary>
        public readonly PrimitiveSettings Settings;

        private PrimitiveSet(VertexPosition2DColorTexture[] vertices, int[] indices, PrimitiveSettings settings)
        {
            Vertices = vertices;
            Indices = indices;
            Settings = settings;
        }
        #endregion

        #region Static Members
        private static void PerformPixelationSafetyChecks(PrimitiveSettings settings)
        {
            // Don't allow accidental screw ups with these.
            if (settings.Pixelate && !PrimitivePixelationSystem.CurrentlyRendering)
                throw new Exception("Error: Primitives using pixelation MUST be prepared/rendered from the IPixelatedPrimitiveRenderer.RenderPixelatedPrimitives method, did you forget to use the interface?");
            else if (!settings.Pixelate && PrimitivePixelationSystem.CurrentlyRendering)
                throw new Exception("Error: Primitives not using pixelation MUST NOT be prepared/rendered from the IPixelatedPrimitiveRenderer.RenderPixelatedPrimitives method.");
        }

        /// <summary>
        /// Prepares a <see cref="PrimitiveSet"/> based on the provided information, and optionally renders it.
        /// </summary>
        /// <param name="positions">The list of positions to use. Keep in mind that these are expected to be in <b>world position</b>, and <see cref="Main.screenPosition"/> is automatically subtracted from them all.</param>
        /// <param name="settings">The primitive draw settings to use.</param>
        /// <param name="pointsToCreate">The amount of points to use. More is higher detailed, but less performant. By default, is the number of positions provided. <b>Going above 100 is NOT recommended.</b></param>
        /// <param name="render">Whether to render the set.</param>
        /// <returns>The prepared set.</returns>
        public static PrimitiveSet? Prepare(IEnumerable<Vector2> positions, PrimitiveSettings settings, int? pointsToCreate = null, bool render = true)
        {
            PerformPixelationSafetyChecks(settings);

            int positionCount = positions.Count();

            // Return if not enough to draw anything.
            if (positionCount <= 2)
                return null;

            // Cull out any zeroed points.
            positions = positions.Where(p => p != Vector2.Zero);
            List<Vector2> trailPoints = PreparePointsRectangleTrail(positions, settings, pointsToCreate ?? positionCount).ToList();

            // A trail with only one point or less has nothing to connect to, and therefore, can't make a trail.
            if (trailPoints.Count <= 2)
                return null;

            // If the trail point has any NaN positions, don't draw anything.
            if (trailPoints.Any(point => point.HasNaNs()))
                return null;

            // If the trail points are all equal, don't draw anything.
            if (trailPoints.All(point => point == trailPoints[0]))
                return null;

            IEnumerable<VertexPosition2DColorTexture> vertices = CreateVerticesRectangleTrail(trailPoints.ToList(), settings);
            int[] indices = CreateIndicesRectangleTrail(trailPoints.Count);

            // Create the set, and just return it if not asked to render.
            PrimitiveSet set = new(vertices.ToArray(), indices, settings);
            if (!render)
                return set;

            // Else render the set and return it.
            return set.Render();
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Renders the provided primitive set to the screen.
        /// </summary>
        /// <returns>The provided set.</returns>
        public PrimitiveSet Render()
        {
            PerformPixelationSafetyChecks(Settings);

            if (Indices.Length % 6 != 0 || Vertices.Length <= 3)
                return this;

            // Perform screen culling, for performance reasons.
            Main.instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            Matrix view;
            Matrix projection;
            if (Settings.Pixelate)
                CalcuatePixelatedPerspectiveMatrices(out view, out projection);
            else
                CalamityUtils.CalculatePerspectiveMatricies(out view, out projection);

            var shaderToUse = Settings.Shader ?? GameShaders.Misc["CalamityMod:StandardPrimitiveShader"];
            shaderToUse.Shader.Parameters["uWorldViewProjection"].SetValue(view * projection);
            shaderToUse.Apply();

            // Render the set.
            Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, Indices, 0, Indices.Length / 3);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            return this;
        }
        #endregion

        #region Set Preperation
        private static IEnumerable<Vector2> PreparePointsRectangleTrail(IEnumerable<Vector2> positions, PrimitiveSettings settings, int pointsToCreate)
        {
            // Don't smoothen the points unless explicitly told do so.
            if (!settings.Smoothen)
            {
                List<Vector2> basePoints = positions.Where(originalPosition => originalPosition != Vector2.Zero).ToList();
                List<Vector2> endPoints = new();

                if (basePoints.Count <= 2)
                    return endPoints;

                // Remap the original positions across a certain length.
                for (int i = 0; i < pointsToCreate; i++)
                {
                    float completionRatio = i / (float)(pointsToCreate - 1f);
                    int currentIndex = (int)(completionRatio * (basePoints.Count - 1));
                    Vector2 currentPoint = basePoints[currentIndex];
                    Vector2 nextPoint = basePoints[(currentIndex + 1) % basePoints.Count];
                    endPoints.Add(Vector2.Lerp(currentPoint, nextPoint, completionRatio * (basePoints.Count - 1) % 0.99999f) - Main.screenPosition);
                }
                return endPoints;
            }

            List<Vector2> controlPoints = new();
            for (int i = 0; i < positions.Count(); i++)
            {
                // Don't incorporate points that are zeroed out.
                // They are almost certainly a result of incomplete oldPos arrays.
                if (positions.ElementAt(i) == Vector2.Zero)
                    continue;

                float completionRatio = i / (float)positions.Count();
                Vector2 offset = -Main.screenPosition;
                if (settings.OffsetFunction != null)
                    offset += settings.OffsetFunction(completionRatio);
                controlPoints.Add(positions.ElementAt(i) + offset);
            }
            List<Vector2> points = new();

            // Avoid stupid index errors.
            if (controlPoints.Count <= 4)
                return controlPoints;

            for (int j = 0; j < pointsToCreate; j++)
            {
                float splineInterpolant = j / (float)pointsToCreate;
                float localSplineInterpolant = splineInterpolant * (controlPoints.Count - 1f) % 1f;
                int localSplineIndex = (int)(splineInterpolant * (controlPoints.Count - 1f));

                Vector2 farLeft;
                Vector2 left = controlPoints[localSplineIndex];
                Vector2 right = controlPoints[localSplineIndex + 1];
                Vector2 farRight;

                // Special case: If the spline attempts to access the previous/next index but the index is already at the very beginning/end, simply
                // cheat a little bit by creating a phantom point that's mirrored from the previous one.
                if (localSplineIndex <= 0)
                {
                    Vector2 mirrored = left * 2f - right;
                    farLeft = mirrored;
                }
                else
                    farLeft = controlPoints[localSplineIndex - 1];

                if (localSplineIndex >= controlPoints.Count - 2)
                {
                    Vector2 mirrored = right * 2f - left;
                    farRight = mirrored;
                }
                else
                    farRight = controlPoints[localSplineIndex + 2];

                points.Add(Vector2.CatmullRom(farLeft, left, right, farRight, localSplineInterpolant));
            }

            // Manually insert the front and end points.
            points.Insert(0, controlPoints.First());
            points.Add(controlPoints.Last());

            return points;
        }

        private static IEnumerable<VertexPosition2DColorTexture> CreateVerticesRectangleTrail(List<Vector2> positions, PrimitiveSettings settings)
        {
            List<VertexPosition2DColorTexture> vertices = new();

            for (int i = 0; i < positions.Count; i++)
            {
                float completionRatio = i / (float)(positions.Count - 1);
                float widthAtVertex = settings.WidthFunction(completionRatio);
                Color vertexColor = settings.ColorFunction(completionRatio);
                Vector2 currentPosition = positions[i];
                Vector2 directionToAhead = i == positions.Count - 1 ? (positions[i] - positions[i - 1]).SafeNormalize(Vector2.Zero) : (positions[i + 1] - positions[i]).SafeNormalize(Vector2.Zero);

                Vector2 leftCurrentTextureCoord = new(completionRatio, 0f);
                Vector2 rightCurrentTextureCoord = new(completionRatio, 1f);

                // Point 90 degrees away from the direction towards the next point, and use it to mark the edges of the rectangle.
                // This doesn't use RotatedBy for the sake of performance (there can potentially be a lot of trail points).
                Vector2 sideDirection = new(-directionToAhead.Y, directionToAhead.X);

                Vector2 left = currentPosition - sideDirection * widthAtVertex;
                Vector2 right = currentPosition + sideDirection * widthAtVertex;

                // Override the initial vertex positions if requested.
                if (i == 0 && settings.InitialVertexPositionsOverride.HasValue && settings.InitialVertexPositionsOverride.Value.Item1 != Vector2.Zero && settings.InitialVertexPositionsOverride.Value.Item2 != Vector2.Zero)
                {
                    left = settings.InitialVertexPositionsOverride.Value.Item1;
                    right = settings.InitialVertexPositionsOverride.Value.Item2;
                }

                // What this is doing, at its core, is defining a rectangle based on two triangles.
                // These triangles are defined based on the width of the strip at that point.
                // The resulting rectangles combined are what make the trail itself.
                vertices.Add(new VertexPosition2DColorTexture(left, vertexColor, leftCurrentTextureCoord));
                vertices.Add(new VertexPosition2DColorTexture(right, vertexColor, rightCurrentTextureCoord));
            }

            return vertices;
        }

        private static int[] CreateIndicesRectangleTrail(int pointCount)
        {
            // What this is doing is basically representing each point on the vertices list as
            // indices. These indices should come together to create a tiny rectangle that acts
            // as a segment on the trail. This is achieved here by splitting the indices (or rather, points)
            // into 2 triangles, which requires 6 points.
            // The logic here basically determines which indices are connected together.
            int totalIndices = (pointCount - 1) * 6;
            int[] indices = new int[totalIndices];

            for (int i = 0; i < pointCount - 2; i++)
            {
                int startingTriangleIndex = i * 6;
                int connectToIndex = i * 2;
                indices[startingTriangleIndex] = connectToIndex;
                indices[startingTriangleIndex + 1] = connectToIndex + 1;
                indices[startingTriangleIndex + 2] = connectToIndex + 2;
                indices[startingTriangleIndex + 3] = connectToIndex + 2;
                indices[startingTriangleIndex + 4] = connectToIndex + 1;
                indices[startingTriangleIndex + 5] = connectToIndex + 3;
            }

            return indices;
        }

        private static void CalcuatePixelatedPerspectiveMatrices(out Matrix viewMatrix, out Matrix projectionMatrix)
        {
            // Due to the scaling, the normal transformation calcuations do not work with pixelated primitives.
            projectionMatrix = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);
            viewMatrix = Matrix.Identity;
        }
        #endregion
    }
}
