using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        /// <summary>
        /// Performs collision based a rotating hitbox for an entity by treating the hitbox as a line. By default uses the velocity of the entity as a direction. This can be overriden.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="targetTopLeft">The top left coordinates of the target to check.</param>
        /// <param name="targetHitboxDimensions">The hitbox size of the target to check.</param>
        /// <param name="directionOverride">An optional direction override</param>
        public static bool RotatingHitboxCollision(this Entity entity, Vector2 targetTopLeft, Vector2 targetHitboxDimensions, Vector2? directionOverride = null, float scale = 1f)
        {
            Vector2 lineDirection = directionOverride ?? entity.velocity;

            // Ensure that the line direction is a unit vector.
            lineDirection = lineDirection.SafeNormalize(Vector2.UnitY);
            Vector2 start = entity.Center - lineDirection * entity.height * 0.5f * scale;
            Vector2 end = entity.Center + lineDirection * entity.height * 0.5f * scale;

            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetTopLeft, targetHitboxDimensions, start, end, entity.width * scale, ref _);
        }

        /// <summary>
        /// Shortcut used to make projectiles have rotating hitbox collision.
        /// </summary>
        /// <param name="projectile">The projectile.</param>
        /// <param name="targetHitbox">The rectangle for the target hitbox.</param>
        public static bool RotatingHitboxCollision(this Projectile proj, Rectangle targetHitbox) => RotatingHitboxCollision(proj, targetHitbox.TopLeft(), targetHitbox.Size(), (proj.rotation + MathHelper.PiOver2).ToRotationVector2(), proj.scale);

        /// <summary>
        /// Determines if a typical hitbox rectangle is intersecting a circular hitbox.
        /// </summary>
        /// <param name="centerCheckPosition">The center of the circular hitbox.</param>
        /// <param name="radius">The radius of the circular hitbox.</param>
        /// <param name="targetHitbox">The hitbox of the target to check.</param>
        public static bool CircularHitboxCollision(Vector2 centerCheckPosition, float radius, Rectangle targetHitbox)
        {

            if (radius <= 0f)
                return false;

            float closestX = MathHelper.Clamp(centerCheckPosition.X, targetHitbox.Left, targetHitbox.Right);
            float closestY = MathHelper.Clamp(centerCheckPosition.Y, targetHitbox.Top, targetHitbox.Bottom);

            float dx = centerCheckPosition.X - closestX;
            float dy = centerCheckPosition.Y - closestY;

            return (dx * dx + dy * dy) <= (radius * radius);
        }

        /// <summary>
        /// Creates an arc around an entity and returns true if a specific collision check is reached for a specific point along the arc.
        /// <br>
        /// By default, this will check if any individual point along the arc is stuck inside of a solid tile.
        /// </br>
        /// </summary>
        /// <param name="minRadians">The minimum angle of the arc in radians.</param>
        /// <param name="maxRadians">The maximum angle of the arc in radians.</param>
        /// <param name="radiansIncrement">By how many radians should the loop increment by when looping between the two angles. This will affect how many points the arc has.</param>
        /// <param name="basePosition">The base position the arc should be scribed from. Defaults to the entity's center.</param>
        /// <param name="arcDirection">The direction of the arc. Defaults to the entity's velocity.</param>
        /// <param name="arcRadius">The radius of the arc from the base position in pixels. Defaults to 32.</param>
        /// <param name="optionalCollisionCheckOverride">An optional delegate parameter of two Vector2 inputs, the base position and the individual arc point, which can be used to
        /// override the default collision check logic.</param>
        public static bool ArcCollisionCheck(this Entity entity, float minRadians, float maxRadians, float radiansIncrement, Vector2? basePosition = null, Vector2? arcDirection = null, float arcRadius = 32f, Func<Vector2, Vector2, bool> optionalCollisionCheckOverride = null)
        {
            for (float i = minRadians; i < maxRadians; i += radiansIncrement)
            {
                Vector2 arcPointBase = basePosition ?? entity.Center;
                Vector2 checkDirection = arcDirection ?? entity.velocity;
                Vector2 arcPoint = arcPointBase + checkDirection.SafeNormalize(Vector2.Zero).RotatedBy(i) * arcRadius;

                bool collisionCheck = !Collision.CanHitLine(arcPointBase, 1, 1, arcPoint, 1, 1);
                if (optionalCollisionCheckOverride is not null)
                    collisionCheck = optionalCollisionCheckOverride.Invoke(arcPointBase, arcPoint);

                if (collisionCheck)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines the distance required before a ray in a given direction from a given starting position hits solid tiles. Gives up after a certain quantity of tiles, or when a world border is reached.
        /// </summary>
        /// <param name="startingPoint">The point to check from.</param>
        /// <param name="checkDirection">The direction in which tiles are checked. Will always be a unit vector.</param>
        /// <param name="optionalTileCheckOverride">An optional delegate function for overriding the base tile check logic with your own custom logic.</param>
        public static float? DistanceToTileCollisionHit(Vector2 startingPoint, Vector2 checkDirection, int giveUpLimit = 500, Func<Tile, bool> optionalTileCheckOverride = null)
        {
            // Ensure that the check direction is normalized.
            checkDirection = checkDirection.SafeNormalize(Vector2.Zero);

            for (int i = 1; i < giveUpLimit; i++)
            {
                Point checkPosition = startingPoint.ToTileCoordinates();
                checkPosition.X += (int)(checkDirection.X * i);
                checkPosition.Y += (int)(checkDirection.Y * i);

                // Don't bother checking any further if the check has left the world.
                if (!WorldGen.InWorld(checkPosition.X, checkPosition.Y, 2))
                    return null;

                // If a solid tile is hit, return the distance.
                // Since Terraria's tile coordinate system is discrete and does not care for more advanced concepts,
                // the amount of tiles searched such far is a sufficient answer.
                Tile tile = ParanoidTileRetrieval(checkPosition.X, checkPosition.Y);
                bool validTileHasBeenHit = WorldGen.SolidTile(tile) || (checkDirection.Y >= 0f && tile.HasTile && Main.tileSolidTop[tile.TileType]);
                if (optionalTileCheckOverride is not null)
                    validTileHasBeenHit = optionalTileCheckOverride.Invoke(tile);

                if (validTileHasBeenHit)
                    return i;
            }

            return null;
        }

        /// <summary>
        /// Determines the distance required before a ray in a given direction from a given starting position hits solid tiles, taking slopes into account.
        /// </summary>
        /// <param name="start">The point to check from.</param>
        /// <param name="rotation">The direction in which tiles are checked.</param>
        /// <param name="length">How far in the direction that will be checked.</param>
        /// <param name="step">How many units moved forward each loop. Greater = less precise.</param>
        /// <returns>The length until the first collision detected. Returns input length if no collision occurs.</returns>
        public static float PreciseDistanceToTileCollisionHit(Vector2 start, float rotation, float length, float step = 1)
        {
            Vector2 unitVect = rotation.ToRotationVector2();
            Vector2 end = unitVect * length;

            if (length < 1f)
            {
                Point endWorldPos = end.ToTileCoordinates();
                return ParanoidTileRetrieval(endWorldPos.X, endWorldPos.Y).IsTileSolid() ? 0 : length;
            }

            Vector2 currentPos = start;
            Point lastAirPos = new Point(-1, -1);
            for (float i = 0; i < length; i += step)
            {
                currentPos += unitVect * step;

                Point tilePos = currentPos.ToTileCoordinates();

                if (tilePos == lastAirPos)
                    continue;

                if (!WorldGen.InWorld(tilePos.X, tilePos.Y))
                    continue;

                Tile tile = Main.tile[tilePos.X, tilePos.Y];
                if (!tile.IsTileSolid())
                {
                    lastAirPos = tilePos;
                    continue;
                }

                if (tile.Slope == SlopeType.Solid && !tile.IsHalfBlock)
                    return (currentPos - start).Length();

                Vector2 tileWorldPos = new Vector2(tilePos.X * 16, tilePos.Y * 16);
                Vector2 currentPosInTile = currentPos - tileWorldPos;
                if (tile.IsHalfBlock)
                {
                    if (currentPosInTile.Y >= 8f)
                        return (currentPos - start).Length();
                }
                else if (tile.Slope == SlopeType.SlopeDownLeft)
                {
                    if (currentPosInTile.X <= currentPosInTile.Y)
                        return (currentPos - start).Length();
                }
                else if (tile.Slope == SlopeType.SlopeDownRight)
                {
                    if ((16 - currentPosInTile.X) <= currentPosInTile.Y)
                        return (currentPos - start).Length();
                }
                else if (tile.Slope == SlopeType.SlopeUpLeft)
                {
                    if (currentPosInTile.X <= (16 - currentPosInTile.Y))
                        return (currentPos - start).Length();
                }
                else if (tile.Slope == SlopeType.SlopeUpRight)
                {
                    if (currentPosInTile.X >= currentPosInTile.Y)
                        return (currentPos - start).Length();
                }
            }

            return length;
        }
        /// <summary>
        /// Determines the distance required before a ray in a given direction from a given starting position hits solid tiles, taking slopes into account.
        /// </summary>
        /// <param name="start">The point to check from.</param>
        /// <param name="rotation">The direction in which tiles are checked.</param>
        /// <param name="length">How far in the direction that will be checked.</param>
        /// <param name="step">How many units moved forward each loop. Greater = less precise.</param>
        /// <returns>False if there is a tile collision before checking the full length of the line.</returns>
        public static bool PreciseCanHitInLine(Vector2 start, float rotation, float length, float step = 1)
        {
            Vector2 unitVect = rotation.ToRotationVector2();
            Vector2 end = unitVect * length;

            if (length < 1f)
            {
                Point endWorldPos = end.ToTileCoordinates();
                return !ParanoidTileRetrieval(endWorldPos.X, endWorldPos.Y).IsTileSolid();
            }

            Vector2 currentPos = start;
            Point lastAirPos = new Point(-1, -1);
            for (float i = 0; i < length; i += step)
            {
                currentPos += unitVect * step;

                Point tilePos = currentPos.ToTileCoordinates();

                if (tilePos == lastAirPos)
                    continue;

                if (!WorldGen.InWorld(tilePos.X, tilePos.Y))
                    continue;

                Tile tile = Main.tile[tilePos.X, tilePos.Y];
                if (!tile.IsTileSolid())
                {
                    lastAirPos = tilePos;
                    continue;
                }

                if (tile.Slope == SlopeType.Solid && !tile.IsHalfBlock)
                    return false;

                Vector2 tileWorldPos = new Vector2(tilePos.X * 16, tilePos.Y * 16);
                Vector2 currentPosInTile = currentPos - tileWorldPos;
                if (tile.IsHalfBlock)
                {
                    if (currentPosInTile.Y >= 8f)
                        return false;
                }
                else if (tile.Slope == SlopeType.SlopeDownLeft)
                {
                    if (currentPosInTile.X <= currentPosInTile.Y)
                        return false;
                }
                else if (tile.Slope == SlopeType.SlopeDownRight)
                {
                    if ((16 - currentPosInTile.X) <= currentPosInTile.Y)
                        return false;
                }
                else if (tile.Slope == SlopeType.SlopeUpLeft)
                {
                    if (currentPosInTile.X <= (16 - currentPosInTile.Y))
                        return false;
                }
                else if (tile.Slope == SlopeType.SlopeUpRight)
                {
                    if (currentPosInTile.X >= currentPosInTile.Y)
                        return false;
                }
            }

            return true;
        }
    }
}
