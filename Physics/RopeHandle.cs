using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Physics;

// Yes, I was silently reading over the Nightshade dev channels when learning about this.
// So, thanks Tomat and 1-3 for the idea! -Lucille
/// <summary>
///     A handle that contains a reference to a given globally managed rope instance.
/// </summary>
public readonly struct RopeHandle
{
    /// <summary>
    ///     The value used to identify this handle's associated rope managed by the central system.
    /// </summary>
    private readonly int Identifier;

    /// <summary>
    ///     The rope associated with this handle.
    /// </summary>
    private readonly Rope Rope => ModContent.GetInstance<RopeManagerSystem>().Ropes[Identifier]!;

    /// <summary>
    ///     The set of all positions maintained by the underlying rope.
    /// </summary>
    public readonly IEnumerable<Vector2> Positions => Rope.SegmentPositions;

    /// <summary>
    ///     The amount of segments this rope has.
    /// </summary>
    public int SegmentCount => Rope.Segments.Length;

    /// <summary>
    ///     The starting position of the underlying rope.
    /// </summary>
    public ref Vector2 Start => ref Rope.Segments[0].Position;

    /// <summary>
    ///     The ending position of the underlying rope.
    /// </summary>
    public ref Vector2 End => ref Rope.Segments[^1].Position;

    /// <summary>
    ///     The gravity associated with the rope.
    /// </summary>
    public Vector2 Gravity
    {
        get => Rope.Gravity;
        set => Rope.Gravity = value;
    }

    internal RopeHandle(int identifier) => Identifier = identifier;

    /// <summary>
    ///     Forces this rope to settle by performing a series of constrained updates.
    /// </summary>
    public void Settle()
    {
        for (var i = 0; i < 20; i++)
            Rope.Update();
    }

    /// <summary>
    ///     Indicates that the rope associated with this handle should be returned back to the pool.
    /// </summary>
    public void Dispose()
    {
        var chunkIndex = Identifier / RopeManagerSystem.BitsPerChunk;
        var bitIndex = Identifier % RopeManagerSystem.BitsPerChunk;
        ModContent.GetInstance<RopeManagerSystem>().ToggleActivityIndex(chunkIndex, bitIndex);
    }
}
