using Microsoft.Xna.Framework;

namespace CalamityMod.Physics;

/// <summary>
///     A representation of a rope segment, containing physical data such as position, velocity, etc., as well as a value which determines whether the rope is fixed in place and not subject to standard physics.
/// </summary>
public struct RopeSegment
{
    /// <summary>
    ///     The current position of this segment.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    ///     The previous position of this segment.
    /// </summary>
    public Vector2 OldPosition;

    /// <summary>
    ///     Whether this segment is fixed in place and not subject to standard physics, such as gravity.
    /// </summary>
    public bool FixedInPlace;

    public RopeSegment(Vector2 position)
    {
        Position = position;
        OldPosition = position;
    }
}
