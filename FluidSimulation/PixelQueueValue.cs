using Microsoft.Xna.Framework;

namespace CalamityMod.FluidSimulation
{
    public struct PixelQueueValue
    {
        public Vector2 Position;

        public Vector4 Value;

        public PixelQueueValue(Vector2 p, Color v)
        {
            Position = p;
            Value = v.ToVector4();
        }

        public PixelQueueValue(Vector2 p, Vector4 v)
        {
            Position = p;
            Value = v;
        }
    }
}
