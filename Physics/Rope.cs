using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Physics;

internal class Rope
{
    /// <summary>
    ///     A 0-1 interpolant which dictates a dampning factor for velocity integration increments.
    /// </summary>
    private float MovementSpeedDampingCoefficient
    {
        get;
        set;
    }

    /// <summary>
    ///     A timer that increments in respond to wind, assuming wind response is enabled in the <see cref="Settings"/>.
    /// </summary>
    private float WindTime
    {
        get;
        set;
    }

    /// <summary>
    ///     The set of segments that compose this rope.
    /// </summary>
    public RopeSegment[] Segments
    {
        get;
        set;
    }

    /// <summary>
    ///     The set of positions that compose this rope.
    /// </summary>
    public Vector2[] SegmentPositions
    {
        get;
        private set;
    }

    /// <summary>
    ///     The desired distance between each segment on the rope.
    /// </summary>
    public float DistancePerSegment
    {
        get;
        set;
    }

    /// <summary>
    ///     The gravity force to exert on the rope when updating.
    /// </summary>
    public Vector2 Gravity
    {
        get;
        set;
    }

    /// <summary>
    ///     The amount of steps to perform when constraining tiles to their desired lengths. Higher values equate to greater accuracy, but reduced performance.
    /// </summary>
    public int ConstraintSteps
    {
        get;
        private set;
    }

    /// <summary>
    ///     Sets of settings that dictate how this rope should behave.
    /// </summary>
    public RopeSettings Settings
    {
        get;
        set;
    }

    /// <summary>
    ///     Whether this rope should collide with tiles or not.
    /// </summary>
    public bool InteractWithTiles => Settings.TileColliderArea is not null;

    public Rope(Vector2 start, Vector2 end, int segmentCount, float distancePerSegment, Vector2 gravity, RopeSettings settings, int constraintSteps = 10)
    {
        Segments = new RopeSegment[segmentCount];
        SegmentPositions = new Vector2[segmentCount];
        for (var i = 0; i < segmentCount; i++)
        {
            var segmentPos = Vector2.Lerp(start, end, i / (segmentCount - 1f));
            Segments[i] = new RopeSegment(segmentPos);
        }

        Segments[0].FixedInPlace = settings.StartIsFixed;
        Segments[^1].FixedInPlace = settings.EndIsFixed;
        RecalculateSegmentPositions();

        DistancePerSegment = distancePerSegment;
        Gravity = gravity;
        ConstraintSteps = constraintSteps;

        Settings = settings;
    }

    /// <summary>
    ///     Recalculates the <see cref="SegmentPositions"/> cache based on <see cref="Segments"/> positions.
    /// </summary>
    private void RecalculateSegmentPositions()
    {
        for (var i = 0; i < Segments.Length; i++)
            SegmentPositions[i] = Segments[i].Position;
    }

    /// <summary>
    ///     Moves a given position around, obeying tile interaction rules if this rope requires them.
    /// </summary>
    /// <param name="position">The position vector to move.</param>
    /// <param name="baseVelocity">The base velocity to consider.</param>
    private void Move(ref Vector2 position, Vector2 baseVelocity)
    {
        // Apply standard Eulerian integration if tile interactions are not required.
        if (!InteractWithTiles || Settings.TileColliderArea is null)
        {
            position += baseVelocity;
            return;
        }

        // If tile interactions *are* required, handle them before moving forward.
        var width = (int)Settings.TileColliderArea.Value.X;
        var height = (int)Settings.TileColliderArea.Value.Y;
        var newVelocity = Collision.noSlopeCollision(position, baseVelocity, width, height + 2, true, true);
        newVelocity = Collision.noSlopeCollision(position, newVelocity, width, height, true, true);
        var finalVelocity = baseVelocity;
        if (Math.Abs(baseVelocity.X) > Math.Abs(newVelocity.X))
            finalVelocity.X = 0f;
        if (Math.Abs(baseVelocity.Y) > Math.Abs(newVelocity.Y))
            finalVelocity.Y = 0f;

        position += finalVelocity;
    }

    /// <summary>
    ///     Updates this rope, making it move around.
    /// </summary>
    public void Update()
    {
        for (var i = 0; i < Segments.Length; i++)
        {
            var movementStep = (Segments[i].Position - Segments[i].OldPosition) * (1f - MovementSpeedDampingCoefficient);
            if (movementStep.Length() < 0.02f)
                movementStep = Vector2.Zero;

            Segments[i].OldPosition = Segments[i].Position;

            if (!Segments[i].FixedInPlace)
                Move(ref Segments[i].Position, movementStep + Gravity);
        }

        for (var i = 0; i < ConstraintSteps; i++)
            Constrain();

        RecalculateSegmentPositions();

        if (Settings.RespondToEntityMovement)
            HandleEntityMovementResponse();
        if (Settings.RespondToWind)
            HandleWindResponse();
    }

    /// <summary>
    ///     Makes this rope respond to the movement of entities.
    /// </summary>
    private void HandleEntityMovementResponse()
    {
        for (var i = 0; i < Segments.Length; i++)
        {
            ref RopeSegment ropeSegment = ref Segments[i];
            if (ropeSegment.FixedInPlace)
                continue;

            foreach (var player in Main.ActivePlayers)
            {
                float playerProximityInterpolant = Utils.GetLerpValue(37f, 10f, player.Distance(ropeSegment.Position), true);
                ropeSegment.Position += player.velocity * playerProximityInterpolant / Settings.Mass * 0.08f;
            }
        }
    }

    /// <summary>
    ///     Makes this entity respond to wind.
    /// </summary>
    private void HandleWindResponse()
    {
        WindTime += Main.windSpeedCurrent / 60f;
        if (MathF.Abs(WindTime) >= 4000f)
            WindTime = 0f;

        var windSpeed = Math.Clamp(Main.WindForVisuals * 2f, -1.3f, 1.3f);
        var windWave = MathF.Cos(WindTime * 3.42f + Segments[0].Position.Length() * 0.06f);
        var wind = Vector2.UnitX * (windWave + Main.windSpeedCurrent) * -0.2f;

        Segments[^1].Position += wind * Utils.GetLerpValue(0.3f, 0.75f, windSpeed, true) / Settings.Mass;
    }

    /// <summary>
    ///     Constrains segments on this rope, conserving their overall length.
    /// </summary>
    public void Constrain()
    {
        for (var i = 0; i < Segments.Length - 1; i++)
        {
            // Determine how much each segment has to move in order to return to its desired resting distance.
            var segmentLength = Segments[i].Position.Distance(Segments[i + 1].Position);
            var distanceFromIdealLength = segmentLength - DistancePerSegment;
            Vector2 correctiveForce = (Segments[i].Position - Segments[i + 1].Position).SafeNormalize(Vector2.Zero) * distanceFromIdealLength;

            var pinned = Segments[i].FixedInPlace;
            var nextPinned = Segments[i + 1].FixedInPlace;
            correctiveForce *= pinned || nextPinned ? 1f : 0.5f;

            if (!pinned)
                Move(ref Segments[i].Position, -correctiveForce);
            if (!nextPinned)
                Move(ref Segments[i + 1].Position, correctiveForce);
        }
    }
}
