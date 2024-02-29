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
    /// This manages rendering primitives in via the provided <see cref="RenderTrail"/> method.<br/><br/>
    /// <b>To use normally, in the draw method of an NPC or Projectile call <see cref="RenderTrail"/>.</b><br/>
    /// <list type="bullet">
    /// <item>The first param should be the positions to use for the trail.</item>
    /// <item>The second is a struct that allows you to choose a desired configuration of settings to use with the trail, to allow for customization.</item>
    /// <item>The third controls how many points are created for the trail, and the fourth determines whether the primitive is subsequently rendered.</item>
    /// </list>
    /// If you wish to use pixelation, you <b>MUST</b> make the NPC/Projectile inherit <see cref="IPixelatedPrimitiveRenderer"/> and use <see cref="IPixelatedPrimitiveRenderer.RenderPixelatedPrimitives"/> instead of predraw..<br/>
    /// You can also optionally specify a render layer with <see cref="IPixelatedPrimitiveRenderer.LayerToRenderTo"/>. It is <see cref="PixelationPrimitiveLayer.BeforeNPCs"/> by default.
    /// </summary>
    public static class PrimitiveRenderer
    {
        #region Static Members
        private static DynamicVertexBuffer VertexBuffer;

        private static DynamicIndexBuffer IndexBuffer;

        private static PrimitiveSettings MainSettings;

        private static Vector2[] MainPositions;

        private static VertexPosition2DColorTexture[] MainVertices;

        private static short[] MainIndices;

        private const short MaxPositions = 1000;

        private const short MaxVertices = 3072;

        private const short MaxIndices = 8192;

        private static short PositionsIndex;

        private static short VerticesIndex;

        private static short IndicesIndex;

        static PrimitiveRenderer()
        {
            Main.QueueMainThreadAction(() =>
            {
                MainPositions = new Vector2[MaxPositions];
                MainVertices = new VertexPosition2DColorTexture[MaxVertices];
                MainIndices = new short[MaxIndices];
                VertexBuffer ??= new DynamicVertexBuffer(Main.instance.GraphicsDevice, VertexPosition2DColorTexture.VertexDeclaration2D, MaxVertices, BufferUsage.WriteOnly);
                IndexBuffer ??= new DynamicIndexBuffer(Main.instance.GraphicsDevice, IndexElementSize.SixteenBits, MaxIndices, BufferUsage.WriteOnly);
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
            if (positions.Length >= MaxPositions)
                return;

            // IF this is false, a correct position trail could not be made and rendering should not continue.
            if (!AssignPointsRectangleTrail(positions, settings, pointsToCreate ?? positions.Length))
                return;

            // A trail with only one point or less has nothing to connect to, and therefore, can't make a trail.
            if (MainPositions.Length <= 2)
                return;

            MainSettings = settings;

            AssignVerticesRectangleTrail();
            AssignIndicesRectangleTrail();

            // Else render without wasting resources creating a set.
            PrivateRender();
            return;
        }

        private static void PrivateRender()
        {
            if (IndicesIndex % 6 != 0 || VerticesIndex <= 3)
                return;

            // Perform screen culling, for performance reasons.
            Main.instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            Main.instance.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

            Matrix view;
            Matrix projection;
            if (MainSettings.Pixelate)
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
            Main.instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VerticesIndex, 0, IndicesIndex / 3);
        }
        #endregion

        #region Set Preperation
        private static bool AssignPointsRectangleTrail(Vector2[] positions, PrimitiveSettings settings, int pointsToCreate)
        {
            // Don't smoothen the points unless explicitly told do so.
            if (!settings.Smoothen)
            {
                PositionsIndex = 0;

                // Would like to remove this, but unsure how else to properly ensure that none are zero.
                positions = positions.Where(originalPosition => originalPosition != Vector2.Zero).ToArray();

                if (positions.Length <= 2)
                    return false;

                // Remap the original positions across a certain length.
                for (int i = 0; i < pointsToCreate; i++)
                {
                    float completionRatio = i / (float)(pointsToCreate - 1f);
                    int currentIndex = (int)(completionRatio * (positions.Length - 1));
                    Vector2 currentPoint = positions[currentIndex];
                    Vector2 nextPoint = positions[(currentIndex + 1) % positions.Length];
                    MainPositions[PositionsIndex] = Vector2.Lerp(currentPoint, nextPoint, completionRatio * (positions.Length - 1) % 0.99999f) - Main.screenPosition;
                    PositionsIndex++;
                }
                return true;
            }

            // Due to the first point being manually added, points should be added starting at the second position instead of the first.
            PositionsIndex = 1;

            // Create the control points for the spline.
            List<Vector2> controlPoints = new();
            for (int i = 0; i < positions.Length; i++)
            {
                // Don't incorporate points that are zeroed out.
                // They are almost certainly a result of incomplete oldPos arrays.
                if (positions[i] == Vector2.Zero)
                    continue;

                float completionRatio = i / (float)positions.Length;
                Vector2 offset = -Main.screenPosition;
                if (settings.OffsetFunction != null)
                    offset += settings.OffsetFunction(completionRatio);
                controlPoints.Add(positions[i] + offset);
            }

            // Avoid stupid index errors.
            if (controlPoints.Count <= 4)
                return false;

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

                MainPositions[PositionsIndex] = Vector2.CatmullRom(farLeft, left, right, farRight, localSplineInterpolant);
                PositionsIndex++;
            }

            // Manually insert the front and end points.
            MainPositions[0] = controlPoints.First();
            MainPositions[PositionsIndex] = controlPoints.Last();
            PositionsIndex++;
            return true;
        }

        private static void AssignVerticesRectangleTrail()
        {
            VerticesIndex = 0;
            for (int i = 0; i < PositionsIndex; i++)
            {
                float completionRatio = i / (float)(PositionsIndex - 1);
                float widthAtVertex = MainSettings.WidthFunction(completionRatio);
                Color vertexColor = MainSettings.ColorFunction(completionRatio);
                Vector2 currentPosition = MainPositions[i];
                Vector2 directionToAhead = i == PositionsIndex - 1 ? (MainPositions[i] - MainPositions[i - 1]).SafeNormalize(Vector2.Zero) : (MainPositions[i + 1] - MainPositions[i]).SafeNormalize(Vector2.Zero);

                Vector2 leftCurrentTextureCoord = new(completionRatio, 0f);
                Vector2 rightCurrentTextureCoord = new(completionRatio, 1f);

                // Point 90 degrees away from the direction towards the next point, and use it to mark the edges of the rectangle.
                // This doesn't use RotatedBy for the sake of performance (there can potentially be a lot of trail points).
                Vector2 sideDirection = new(-directionToAhead.Y, directionToAhead.X);

                Vector2 left = currentPosition - sideDirection * widthAtVertex;
                Vector2 right = currentPosition + sideDirection * widthAtVertex;

                // Override the initial vertex positions if requested.
                if (i == 0 && MainSettings.InitialVertexPositionsOverride.HasValue && MainSettings.InitialVertexPositionsOverride.Value.Item1 != Vector2.Zero && MainSettings.InitialVertexPositionsOverride.Value.Item2 != Vector2.Zero)
                {
                    left = MainSettings.InitialVertexPositionsOverride.Value.Item1;
                    right = MainSettings.InitialVertexPositionsOverride.Value.Item2;
                }

                // What this is doing, at its core, is defining a rectangle based on two triangles.
                // These triangles are defined based on the width of the strip at that point.
                // The resulting rectangles combined are what make the trail itself.
                MainVertices[VerticesIndex] = new VertexPosition2DColorTexture(left, vertexColor, leftCurrentTextureCoord);
                VerticesIndex++;
                MainVertices[VerticesIndex] = new VertexPosition2DColorTexture(right, vertexColor, rightCurrentTextureCoord);
                VerticesIndex++;
            }
        }

        private static void AssignIndicesRectangleTrail()
        {
            // What this is doing is basically representing each point on the vertices list as
            // indices. These indices should come together to create a tiny rectangle that acts
            // as a segment on the trail. This is achieved here by splitting the indices (or rather, points)
            // into 2 triangles, which requires 6 points.
            // The logic here basically determines which indices are connected together.
            IndicesIndex = 0;
            for (short i = 0; i < PositionsIndex - 2; i++)
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
