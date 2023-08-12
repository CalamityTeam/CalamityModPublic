using Microsoft.Xna.Framework;
using Terraria;

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
        /// Determines if a typical hitbox rectangle is intersecting a circular hitbox.
        /// </summary>
        /// <param name="centerCheckPosition">The center of the circular hitbox.</param>
        /// <param name="radius">The radius of the circular hitbox.</param>
        /// <param name="targetHitbox">The hitbox of the target to check.</param>
        public static bool CircularHitboxCollision(Vector2 centerCheckPosition, float radius, Rectangle targetHitbox)
        {
            // If the center intersects the hitbox, return true immediately
            Rectangle center = new Rectangle((int)centerCheckPosition.X, (int)centerCheckPosition.Y, 1, 1);
            if (center.Intersects(targetHitbox))
                return true;

            float topLeftDistance = Vector2.Distance(centerCheckPosition, targetHitbox.TopLeft());
            float topRightDistance = Vector2.Distance(centerCheckPosition, targetHitbox.TopRight());
            float bottomLeftDistance = Vector2.Distance(centerCheckPosition, targetHitbox.BottomLeft());
            float bottomRightDistance = Vector2.Distance(centerCheckPosition, targetHitbox.BottomRight());

            float distanceToClosestPoint = topLeftDistance;
            if (topRightDistance < distanceToClosestPoint)
                distanceToClosestPoint = topRightDistance;
            if (bottomLeftDistance < distanceToClosestPoint)
                distanceToClosestPoint = bottomLeftDistance;
            if (bottomRightDistance < distanceToClosestPoint)
                distanceToClosestPoint = bottomRightDistance;

            return distanceToClosestPoint <= radius;
        }

        /// Determines the distance required before a ray in a given direction from a given starting position hits solid tiles. Gives up after a certain quantity of tiles, or when a world border is reached.
        /// </summary>
        /// <param name="startingPoint">The point to check from.</param>
        /// <param name="checkDirection">The direction in which tiles are checked. Will always be a unit vector.</param>
        public static float? DistanceToTileCollisionHit(Vector2 startingPoint, Vector2 checkDirection, int giveUpLimit = 500)
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
                if (WorldGen.SolidTile(tile) || (checkDirection.Y >= 0f && tile.HasTile && Main.tileSolidTop[tile.TileType]))
                    return i;
            }

            return null;
        }
    }
}
