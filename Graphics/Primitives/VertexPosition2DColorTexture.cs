using Microsoft.Xna.Framework;
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
        /// The texure-coordinate of the vertex.
        /// </summary>
        public readonly Vector2 TextureCoordinates;

        /// <summary>
        /// The vertex declaration. This declares the order and size of the
        /// </summary>
        public VertexDeclaration VertexDeclaration => _vertexDeclaration;

        private static readonly VertexDeclaration _vertexDeclaration = new(new VertexElement[]
        {
            new(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        });

        public VertexPosition2DColorTexture(Vector2 position, Color color, Vector2 textureCoordinates)
        {
            Position = position;
            Color = color;
            TextureCoordinates = textureCoordinates;
        }
    }
}
