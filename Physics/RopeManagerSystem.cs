using System;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using BitOperations = System.Numerics.BitOperations;

namespace CalamityMod.Physics;

public sealed class RopeManagerSystem : ModSystem
{
    /// <summary>
    ///     The set of all maintained ropes in the world.
    /// </summary>
    internal readonly Rope[] Ropes = new Rope[MaxRopeCount];

    /// <summary>
    ///     The internal binary mappings that determine whether ropes are active or not.
    /// </summary>
    internal readonly ulong[] ActivityBitChunks = new ulong[(int)Math.Ceiling((double)MaxRopeCount / BitsPerChunk)];

    /// <summary>
    ///     The maximum amount of ropes to maintain across the world.
    /// </summary>
    public const int MaxRopeCount = 2048;

    /// <summary>
    ///     The amount of bits contained within each chunk in the <see cref="ActivityBitChunks"/> array.
    /// </summary>
    public const int BitsPerChunk = sizeof(ulong) * 8;

    public override void ClearWorld()
    {
        for (var i = 0; i < ActivityBitChunks.Length; i++)
            ActivityBitChunks[i] = 0uL;
    }

    public override void PostUpdateWorld()
    {
        for (var i = 0; i < Ropes.Length; i++)
        {
            var bitIndex = i % BitsPerChunk;
            var active = (ActivityBitChunks[i / BitsPerChunk] >> bitIndex & 1) == 1;
            if (active)
                Ropes[i].Update();
        }
    }

    /// <summary>
    ///     Toggles a given activity index.
    /// </summary>
    /// <param name="chunkIndex">The index of the <see cref="ActivityBitChunks"/> to toggle.</param>
    /// <param name="bitIndex">The bit index in the chunk to toggle.</param>
    internal void ToggleActivityIndex(int chunkIndex, int bitIndex) => ActivityBitChunks[chunkIndex] ^= 1uL << bitIndex;

    /// <summary>
    ///     Attempts to find and return the first available index for a new rope.
    /// </summary>
    private int? SelectFirstAvailableIndex()
    {
        for (var i = 0; i < ActivityBitChunks.Length; i++)
        {
            // This comment can be deleted later if it's deemed a bit too verbose. I'm half doing it for myself to test my understanding, half doing it for any future readers who might
            // not immediately understand what the bit operations do here. -Lucille

            // As a simplified example, assume the following for the activity bits, where zero means inactive and one means active:
            // 0110 1111

            // This means the first four indices are occupied, but the fifth index is free. Note that in this case bits are counted from right to left.
            // In order to find the first new index, we simply need to count the amount of ones until the first zero.
            // Conveniently, BitOperations.TrailingZeroCount exists for this purpose. We just need to invert the binary in order to convert the ones into zeroes.
            var offset = BitOperations.TrailingZeroCount(~ActivityBitChunks[i]);

            // Check if the index offset is equal to the amount of bits in the chunk.
            // If so, that means that every single bit is a one, and that there's no available index in the chunk to use.
            var allBitsAreOccupied = offset == BitsPerChunk;
            if (allBitsAreOccupied)
                continue;

            return offset + i * BitsPerChunk;
        }

        // No valid index found across the activity bit chunks. Return null.
        return null;
    }

    /// <summary>
    ///     Requests a new rope, returning a handle to it, or null if for some reason the rope couldn't be created.
    /// </summary>
    public RopeHandle? RequestNew(Vector2 start, Vector2 end, int segmentCount, float distancePerSegment, Vector2 gravity, RopeSettings settings, int constraintSteps = 10)
    {
        var index = SelectFirstAvailableIndex();
        if (index is null)
            return null;

        // Mark the newly selected index as active by toggling its activity state on.
        ToggleActivityIndex(index.Value / BitsPerChunk, index.Value % BitsPerChunk);

        Ropes[index.Value] = new Rope(start, end, segmentCount, distancePerSegment, gravity, settings, constraintSteps);

        return new RopeHandle(index.Value);
    }

    /// <summary>
    ///     Calculates the overall segment length of a rope based on the horizontal span between its two end points and a desired sag distance.
    /// </summary>
    public static float CalculateSegmentLength(float ropeSpan, float sag, int iterations = 12)
    {
        // A rope at rest is defined via a catenary curve, which exists in the following mathematical form:
        // y(x) = a * cosh(x / a)

        // Furthermore, the length of a rope, given the horizontal width w for a rope, is defined as follows:
        // L = 2a * sinh(w / 2a)

        // In order to use the above equation, the value of a must be determined for the catenary that this rope will form.
        // To do so, a numerical solution will need to be found based on the known width and sag values.

        // Suppose the two supports are at equal height at distances -w/2 and w/2.
        // From this, sag (which will be denoted with h) can be defined in the following way: h = y(w/2) - y(0)
        // Reducing this results in the following equation:

        // h = a(cosh(w / 2a) - 1)
        // a(cosh(w / 2a) - 1) - h = 0
        // This can be used to numerically find a.
        var initialGuessA = sag;
        var a = (float)CalamityUtils.IterativelySearchForRoot(x =>
        {
            return x * (Math.Cosh(ropeSpan / (x * 2D)) - 1D) - sag;
        }, initialGuessA, iterations);

        // Now that a is known, it's just a matter of plugging it back into the original equation to find L.
        return MathF.Sinh(ropeSpan / a * 0.5f) * a * 2f;
    }
}
