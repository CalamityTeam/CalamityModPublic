using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace CalamityMod.FluidSimulation
{
    // Calculations for these are done primarily on the GPU in shaders for performance-sake.
    public class FluidFieldState : IDisposable
    {
        public RenderTarget2D PreviousState;

        public RenderTarget2D NextState;

        public void Dispose()
        {
            PreviousState?.Dispose();
            NextState?.Dispose();
        }

        public void SwapState() => Utils.Swap(ref PreviousState, ref NextState);

        public FluidFieldState(int size, SurfaceFormat fieldContents = SurfaceFormat.Color)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            PreviousState = new(Main.instance.GraphicsDevice, size, size, true, fieldContents, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            NextState = new(Main.instance.GraphicsDevice, size, size, true, fieldContents, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
        }
    }
}
