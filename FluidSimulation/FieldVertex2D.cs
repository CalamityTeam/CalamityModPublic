using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.FluidSimulation
{
    public struct FieldVertex2D : IVertexType
    {
        public Vector2 Position;
        public Vector4 Color;
        public Vector2 TextureCoordinates;
        public VertexDeclaration VertexDeclaration => _vertexDeclaration;

        private static readonly VertexDeclaration _vertexDeclaration = new(new VertexElement[]
        {
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        });
        public FieldVertex2D(Vector2 position, Vector4 color, Vector2 textureCoordinates)
        {
            Position = position;
            Color = color;
            TextureCoordinates = textureCoordinates;
        }
    }
}
