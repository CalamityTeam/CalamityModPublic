﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Graphics.Primitives
{
    /// <summary>
    /// A custom vertex type with a position using Vector2 instead of Vector4, as Terraria is only a 2D game.
    /// </summary>
    /// <remarks>This represents a vertex that will be rendered by the GPU.</remarks>
    public readonly struct VertexPosition2DColorTexture : IVertexType
    {
        /// <summary>
        /// The position of the vertex.
        /// </summary>
        public readonly Vector2 Position;

        /// <summary>
        /// The color of the vertex.
        /// </summary>
        public readonly Color Color;

        /// <summary>
        /// The texture-coordinate of the vertex.
        /// </summary>
        /// <remarks>
        /// The Z component isn't actually related to 3D, it holds the width of the vertex at the given point, since arbitrary data cannot be saved on a per-vertex basis and needs to be contained within some pre-defined format.
        /// </remarks>
        public readonly Vector3 TextureCoordinates;

        /// <summary>
        /// The vertex declaration. This declares the layout and size of the data in the vertex shader.
        /// </summary>
        public VertexDeclaration VertexDeclaration => VertexDeclaration2D;

        public static readonly VertexDeclaration VertexDeclaration2D = new(new VertexElement[]
        {
            new(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
        });

        public VertexPosition2DColorTexture(Vector2 position, Color color, Vector2 textureCoordinates, float widthCorrectionFactor)
        {
            Position = position;
            Color = color;
            TextureCoordinates = new(textureCoordinates, widthCorrectionFactor);
        }
    }
}
