using System;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Graphics.Primitives
{
    /// <summary>
    /// Controls how U texture coordinates are generated for primitive ribbons.
    /// </summary>
    public enum PrimitiveTextureMode
    {
        /// <summary>
        /// U coordinates run from 0 to 1 across the full trail length. This matches the legacy behavior.
        /// </summary>
        Normalized,

        /// <summary>
        /// U coordinates advance according to distance along the trail. One full texture cycle occurs every <see cref="PrimitiveSettings.TextureCycleLength"/> pixels.
        /// </summary>
        Distance
    }

    /// <summary>
    /// Controls how adjacent segments are joined together when expanding points into quads.
    /// </summary>
    public enum PrimitiveJoinStyle
    {
        /// <summary>
        /// Legacy behavior. Each segment is expanded using its forward direction only.
        /// </summary>
        Flat,

        /// <summary>
        /// Uses the averaged tangent of neighbouring segments to smooth corners.
        /// </summary>
        Smooth,

        /// <summary>
        /// Uses a mitered join to keep width continuous around sharp corners.
        /// </summary>
        Miter
    }

    /// <summary>
    /// Determines how intermediate sample points are generated for smoothing.
    /// </summary>
    public enum PrimitiveSmoothingType
    {
        /// <summary>
        /// Classic Catmull-Rom interpolation between points.
        /// </summary>
        CatmullRom,

        /// <summary>
        /// Cardinal spline interpolation with configurable tension.
        /// </summary>
        Cardinal,

        /// <summary>
        /// Straight-line interpolation between neighbouring points.
        /// </summary>
        Linear,

        /// <summary>
        /// Cubic Hermite interpolation using central difference tangents.
        /// </summary>
        Hermite,

        /// <summary>
        /// Cubic Bezier interpolation using Catmull-Rom handles.
        /// </summary>
        CubicBezier
    }

    /// <summary>
    /// Selects how orientation frames propagate along the trail.
    /// </summary>
    public enum PrimitiveFrameTransportMode
    {
        /// <summary>
        /// Derives the normal from the instantaneous tangent (legacy behaviour).
        /// </summary>
        Basic,

        /// <summary>
        /// Uses parallel transport to minimise twisting.
        /// </summary>
        ParallelTransport
    }

    /// <summary>
    /// Determines the topology used when submitting geometry to the GPU.
    /// </summary>
    public enum PrimitiveTopology
    {
        /// <summary>
        /// Expands geometry into independent triangles (legacy behaviour).
        /// </summary>
        TriangleList,

        /// <summary>
        /// Connects geometry using a continuous triangle strip.
        /// </summary>
        TriangleStrip
    }

    /// <summary>
    /// Controls how the start and end of the primitive are capped.
    /// </summary>
    public enum PrimitiveCapStyle
    {
        /// <summary>
        /// Leaves the ribbon open (legacy behaviour).
        /// </summary>
        None,

        /// <summary>
        /// Adds a flat triangular cap across the ribbon ends.
        /// </summary>
        Flat
    }

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
        public delegate float VertexWidthFunction(float trailLengthInterpolant, Vector2 vertexPosition);

        /// <summary>
        /// A delegate to dynamically determine the color of the trail at each position.
        /// </summary>
        /// <param name="trailLengthInterpolant">The current position along the trail as a 0-1 interlopant value.</param>
        /// <returns>The color for the current point.</returns>
        public delegate Color VertexColorFunction(float trailLengthInterpolant, Vector2 vertexPosition);

        /// <summary>
        /// A delegate to dynamically determine the offset of the trail at each position.
        /// </summary>
        /// <param name="trailLengthInterpolant">The current position along the trail as a 0-1 interlopant value.</param>
        /// <returns>The offset for the current point.</returns>
        public delegate Vector2 VertexOffsetFunction(float trailLengthInterpolant, Vector2 vertexPosition);
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
        /// Whether to use unscaled perspective matrices when rendering the primitives. Recommended only when the system is being used outside of its typical context
        /// i.e. rendering primitives in the background via a CustomSky.
        /// </summary>
        public readonly bool UseUnscaledMatrices;

        /// <summary>
        /// The shader to apply when rendering.
        /// </summary>
        public readonly MiscShaderData Shader;

        /// <summary>
        /// An optional override to force the trail to use the provided positions as the side positions of the initial vertex. They are the left and right positions respectively.
        /// </summary>
        public readonly (Vector2, Vector2)? InitialVertexPositionsOverride;

        /// <summary>
        /// Determines how U texture coordinates are generated.
        /// </summary>
        public readonly PrimitiveTextureMode TextureCoordinateMode;

        /// <summary>
        /// Controls the scale of generated U coordinates. In <see cref="PrimitiveTextureMode.Normalized"/> this is a multiplier, in <see cref="PrimitiveTextureMode.Distance"/> this is the distance (in pixels) for one full texture cycle.
        /// </summary>
        public readonly float TextureCycleLength;

        /// <summary>
        /// Adds an additional offset to the generated U coordinate. In distance mode this value is interpreted in pixels.
        /// </summary>
        public readonly float TextureScrollOffset;

        /// <summary>
        /// Allows overriding the automatically generated U coordinate with a custom function. Receives the normalized completion ratio (0-1) and should return the desired U coordinate.
        /// </summary>
        public readonly Func<float, float> TextureCoordinateFunction;

        /// <summary>
        /// Determines how consecutive segments are joined together when expanding into quads.
        /// </summary>
        public readonly PrimitiveJoinStyle JoinStyle;

        /// <summary>
        /// Maximum multiplier applied to miter joins to keep spikes under control.
        /// </summary>
        public readonly float JoinMiterLimit;

        /// <summary>
        /// If true, a debug wireframe will be rendered on top of the primitive.
        /// </summary>
        public readonly bool DebugWireframe;

        /// <summary>
        /// The color used for the optional wireframe overlay.
        /// </summary>
        public readonly Color WireframeColor;

        /// <summary>
        /// Determines the interpolation style when smoothing the provided point list.
        /// </summary>
        public readonly PrimitiveSmoothingType SmoothingType;

        /// <summary>
        /// Controls how many intermediate subdivisions are generated per control segment when smoothing. Set to 0 for automatic distribution.
        /// </summary>
        public readonly int SmoothingSegments;

        /// <summary>
        /// Additional tension used by smoothing functions that support it (e.g. Cardinal splines).
        /// </summary>
        public readonly float SmoothingTension;

        /// <summary>
        /// Determines how normals are transported along the curve.
        /// </summary>
        public readonly PrimitiveFrameTransportMode FrameTransportMode;

        /// <summary>
        /// Chooses the primitive topology when submitting geometry to the GPU.
        /// </summary>
        public readonly PrimitiveTopology Topology;

        /// <summary>
        /// Determines how the ribbon is capped at its extremities.
        /// </summary>
        public readonly PrimitiveCapStyle CapStyle;
        #endregion

        /// <summary>
        /// Contains all the various options to use when creating a primitive trail.
        /// </summary>
        /// <param name="widthFunction">Used to determine the width of each vertex.</param>
        /// <param name="colorFunction">Used to determine the color of each vertex.</param>
        /// <param name="offsetFunction">Used to offset each position when generating them.</param>
        /// <param name="smoothen">Whether to use smoothening when generating the vertex positions from the provided ones. Recommended to be enabled.</param>
        /// <param name="pixelate">Whether to pixelate the primitives. Recommended to be enabled if the effect was designed with this in mind.</param>
        /// <param name="shader">The shader to apply when rendering the primitives.</param>
        /// <param name="useUnscaledMatrices">Whether to use unscaled perspective matrices when rendering primitives. Recommended when using this system outside of its typical context; i.e. rendering primitives in the background via a CustomSky.</param>
        /// <param name="initialVertexPositionsOverride">An optional override to force the trail to use the provided positions as the side positions of the initial vertex. They are the left and right positions respectively and should be in screen space.</param>
        /// <param name="textureCoordinateMode">Determines how U texture coordinates are generated.</param>
        /// <param name="textureCycleLength">Controls how fast U coordinates advance. Acts as a multiplier in normalized mode, or the distance per cycle in distance mode.</param>
        /// <param name="textureScrollOffset">Adds an offset to generated U coordinates. Interpreted in pixels when using distance mode.</param>
        /// <param name="textureCoordinateFunction">Optional override that receives the completion ratio and returns a custom U coordinate.</param>
        /// <param name="joinStyle">Determines how neighbouring segments are joined when expanding into quads.</param>
        /// <param name="joinMiterLimit">Maximum multiplier applied to miter joins to prevent spikes. Only used when <paramref name="joinStyle"/> is <see cref="PrimitiveJoinStyle.Miter"/>.</param>
        /// <param name="debugWireframe">When true, draws a wireframe overlay useful for debugging primitive geometry.</param>
        /// <param name="wireframeColor">Optional override for the wireframe color. If null, a vivid green is used.</param>
        /// <param name="smoothingType">Determines the interpolation style used when smoothing the control points.</param>
        /// <param name="smoothingSegments">When greater than zero, specifies the number of subdivisions created per control edge.</param>
        /// <param name="smoothingTension">Optional tension parameter used by certain smoothing types such as cardinal splines.</param>
        /// <param name="frameTransportMode">Controls how frame orientation is propagated along the curve.</param>
        /// <param name="topology">Specifies the primitive topology used when submitting geometry.</param>
        /// <param name="capStyle">Selects how the ribbon's start and end are capped.</param>
        public PrimitiveSettings(VertexWidthFunction widthFunction, VertexColorFunction colorFunction, VertexOffsetFunction offsetFunction = null, bool smoothen = true, bool pixelate = false, MiscShaderData shader = null, bool useUnscaledMatrices = false, (Vector2, Vector2)? initialVertexPositionsOverride = null, PrimitiveTextureMode textureCoordinateMode = PrimitiveTextureMode.Normalized, float textureCycleLength = 1f, float textureScrollOffset = 0f, Func<float, float> textureCoordinateFunction = null, PrimitiveJoinStyle joinStyle = PrimitiveJoinStyle.Smooth, float joinMiterLimit = 4f, bool debugWireframe = false, Color? wireframeColor = null, PrimitiveSmoothingType smoothingType = PrimitiveSmoothingType.CatmullRom, int smoothingSegments = 0, float smoothingTension = 0f, PrimitiveFrameTransportMode frameTransportMode = PrimitiveFrameTransportMode.ParallelTransport, PrimitiveTopology topology = PrimitiveTopology.TriangleStrip, PrimitiveCapStyle capStyle = PrimitiveCapStyle.None)
        {
            WidthFunction = widthFunction;
            ColorFunction = colorFunction;
            OffsetFunction = offsetFunction;
            Smoothen = smoothen;
            Pixelate = pixelate;
            Shader = shader;
            UseUnscaledMatrices = useUnscaledMatrices;
            InitialVertexPositionsOverride = initialVertexPositionsOverride;
            TextureCoordinateMode = textureCoordinateMode;
            TextureCycleLength = Math.Abs(textureCycleLength) <= PrimitiveRenderer.Epsilon ? 1f : textureCycleLength;
            TextureScrollOffset = textureScrollOffset;
            TextureCoordinateFunction = textureCoordinateFunction;
            JoinStyle = joinStyle;
            JoinMiterLimit = Math.Max(joinMiterLimit, 1f);
            DebugWireframe = debugWireframe;
            WireframeColor = wireframeColor ?? Color.LimeGreen;
            SmoothingType = smoothingType;
            SmoothingSegments = Math.Max(smoothingSegments, 0);
            SmoothingTension = smoothingTension;
            FrameTransportMode = frameTransportMode;
            Topology = topology;
            CapStyle = capStyle;
        }
    }
}
