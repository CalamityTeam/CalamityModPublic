using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Graphics.Primitives
{
    /// <summary>
    /// Contains all the various options to use when creating a primitive trail. New members can be added freely without breaking existing implementations.
    /// </summary>
    public readonly struct PrimitiveSettings
    {
        #region Delegates
        /// <summary>
        /// A delegate to dynamically determine the width of the trail at each position.
        /// </summary>
        /// <param name="trailLengthInterpolant">The current position along the trail as a 0-1 interlopant value.</param>
        /// <returns>The width for the current point.</returns>
        public delegate float VertexWidthFunction(float trailLengthInterpolant);

        /// <summary>
        /// A delegate to dynamically determine the color of the trail at each position.
        /// </summary>
        /// <param name="trailLengthInterpolant">The current position along the trail as a 0-1 interlopant value.</param>
        /// <returns>The color for the current point.</returns>
        public delegate Color VertexColorFunction(float trailLengthInterpolant);

        /// <summary>
        /// A delegate to dynamically determine the offset of the trail at each position.
        /// </summary>
        /// <param name="trailLengthInterpolant">The current position along the trail as a 0-1 interlopant value.</param>
        /// <returns>The offset for the current point.</returns>
        public delegate Vector2 VertexOffsetFunction(float trailLengthInterpolant);
        #endregion

        #region Instance Fields
        /// <summary>
        /// Used to determine the width of each vertex.
        /// </summary>
        public readonly VertexWidthFunction WidthFunction;

        /// <summary>
        /// Used to determine the color of each vertex. 
        /// </summary>
        public readonly VertexColorFunction ColorFunction;

        /// <summary>
        /// Used to offset each position when generating them.
        /// </summary>
        public readonly VertexOffsetFunction OffsetFunction;

        /// <summary>
        /// Whether to use smoothening when generating the vertex positions from the provided ones Recommended to be on by default.
        /// </summary>
        public readonly bool Smoothen;

        /// <summary>
        /// Whether to pixelate the primitives. Recommended to be on by default.
        /// </summary>
        public readonly bool Pixelate;

        /// <summary>
        /// The shader to apply when rendering.
        /// </summary>
        public readonly MiscShaderData Shader;
        #endregion

        /// <summary>
        /// Contains all the various options to use when creating a primitive trail.
        /// </summary>
        /// <param name="widthFunction">Used to determine the width of each vertex.</param>
        /// <param name="colorFunction">Used to determine the color of each vertex.</param>
        /// <param name="offsetFunction">Used to offset each position when generating them.</param>
        /// <param name="smoothen">Whether to use smoothening when generating the vertex positions from the provided ones Recommended to be on by default.</param>
        /// <param name="pixelate">Whether to pixelate the primitives. Recommended to be on.</param>
        /// <param name="shader">The shader to apply when rendering.</param>
        public PrimitiveSettings(VertexWidthFunction widthFunction, VertexColorFunction colorFunction, VertexOffsetFunction offsetFunction = null, bool smoothen = true, bool pixelate = false, MiscShaderData shader = null)
        {
            WidthFunction = widthFunction;
            ColorFunction = colorFunction;
            OffsetFunction = offsetFunction;
            Smoothen = smoothen;
            Pixelate = pixelate;
            Shader = shader;
        }
    }
}
