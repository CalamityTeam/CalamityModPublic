using System.Collections.Generic;
using CalamityMod.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace CalamityMod.FluidSimulation
{
    // Calculations for these are done primarily on the GPU in shaders for performance-sake.
    public class FluidFieldState
    {
        public ManagedRenderTarget PreviousState;

        public ManagedRenderTarget NextState;

        public Queue<PixelQueueValue> PendingChanges = new();

        public readonly int Size;

        public readonly SurfaceFormat FieldContents;

        public void SwapState() => Utils.Swap(ref PreviousState, ref NextState);

        public RenderTarget2D FieldColorCreateCondition(int screen, int height) =>
            new(Main.instance.GraphicsDevice, Size, Size, true, FieldContents, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

        public FluidFieldState(int size, SurfaceFormat fieldContents = SurfaceFormat.Color)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            Size = size;
            FieldContents = fieldContents;

            PreviousState = new(false, FieldColorCreateCondition, false);
            NextState = new(false, FieldColorCreateCondition, false);
        }
    }
}
