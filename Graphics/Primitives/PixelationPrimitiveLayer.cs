using System;

namespace CalamityMod.Graphics.Primitives
{
    /// <summary>
    /// Controls what layer the <see cref="IPixelatedPrimitiveRenderer.RenderPixelatedPrimitives"/> renders to.
    /// </summary>
    [Flags]
    public enum PixelationPrimitiveLayer
    {
        BeforeNPCs = 1,
        AfterNPCs = 2,
        BeforeProjectiles = 4,
        AfterProjectiles = 8,
        AfterPlayers = 16
    }
}
