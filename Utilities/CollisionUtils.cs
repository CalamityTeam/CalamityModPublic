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
		public static bool RotatingHitboxCollision(this Entity entity, Vector2 targetTopLeft, Vector2 targetHitboxDimensions, Vector2? directionOverride = null)
		{
			Vector2 lineDirection = directionOverride ?? entity.velocity;

			// Ensure that the line direction is a unit vector.
			lineDirection = lineDirection.SafeNormalize(Vector2.UnitY);
			Vector2 start = entity.Center - lineDirection * entity.height * 0.5f;
			Vector2 end = entity.Center + lineDirection * entity.height * 0.5f;

			float _ = 0f;
			return Collision.CheckAABBvLineCollision(targetTopLeft, targetHitboxDimensions, start, end, entity.width, ref _);
		}

		/// <summary>
		/// Determines if a typical hitbox rectangle is intersecting a circular hitbox.
		/// </summary>
		/// <param name="centerCheckPosition">The center of the circular hitbox.</param>
		/// <param name="radius">The radius of the circular hitbox.</param>
		/// <param name="targetHitbox">The hitbox of the target to check.</param>
		public static bool CircularHitboxCollision(Vector2 centerCheckPosition, float radius, Rectangle targetHitbox)
		{
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
	}
}
