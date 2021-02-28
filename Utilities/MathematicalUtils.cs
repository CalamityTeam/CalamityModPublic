using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace CalamityMod
{
	public static partial class CalamityUtils
	{
		/// <summary>
		/// Computes the Manhattan Distance between two points. This is typically used as a cheaper alternative to Euclidean Distance.
		/// </summary>
		/// <param name="a">The first point.</param>
		/// <param name="b">The second point.</param>
		public static float ManhattanDistance(this Vector2 a, Vector2 b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
		
		/// <summary>
		/// Determines the angular distance between two vectors based on dot product comparisons. This method ensures underlying normalization is performed safely.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		public static float AngleBetween(this Vector2 v1, Vector2 v2) => (float)Math.Acos(Vector2.Dot(v1.SafeNormalize(Vector2.Zero), v2.SafeNormalize(Vector2.Zero)));

		/// <summary>
		/// Uses a rewritten horizontal range formula to determine the direction to fire a projectile in order for it to hit a destination. Falls back on a certain value if no such direction can exist. If no fallback is provided, a clamp is used.
		/// </summary>
		/// <param name="shootingPosition">The starting position of the projectile.</param>
		/// <param name="destination">The destination for the projectile to land at.</param>
		/// <param name="gravity">The gravity of the projectile.</param>
		/// <param name="shootSpeed">The magnitude </param>
		/// <param name="nanFallback">The direction to fall back to if the calculations result in any NaNs. If nothing is specified, a clamp is performed to prevent any chance of NaNs at all.</param>
		public static Vector2 GetProjectilePhysicsFiringVelocity(Vector2 shootingPosition, Vector2 destination, float gravity, float shootSpeed, Vector2? nanFallback = null)
		{
			// Reset the NaN fallback to its default.
			if (nanFallback is null)
				nanFallback = Vector2.UnitY;

			// Ensure that the gravity has the right sign for Terraria's coordinate system.
			gravity = -Math.Abs(gravity);

			float horizontalRange = MathHelper.Distance(shootingPosition.X, destination.X);
			float fireAngleSine = gravity * horizontalRange / (float)Math.Pow(shootSpeed, 2);

			// Clamp the sine if no fallback is provided.
			if (nanFallback is null)
				fireAngleSine = MathHelper.Clamp(fireAngleSine, -1f, 1f);

			float fireAngle = (float)Math.Asin(fireAngleSine) * 0.5f;

			// Get out of here if no valid firing angle exists. This can only happen if a fallback does indeed exist.
			if (float.IsNaN(fireAngle))
				return nanFallback.Value * shootSpeed;

			Vector2 fireVelocity = new Vector2(0f, -shootSpeed).RotatedBy(fireAngle);
			fireVelocity.X *= (destination.X - shootingPosition.X < 0).ToDirectionInt();
			return fireVelocity;
		}

		// REMOVE THIS IN CALAMITY 1.4, it's a 1.4 World.cs function.
		// Due to its temporary state, this method will not receive an XML documentation comment.
		public static Rectangle ClampToWorld(Rectangle tileRectangle)
		{
			int num = Math.Max(0, Math.Min(tileRectangle.Left, Main.maxTilesX));
			int num2 = Math.Max(0, Math.Min(tileRectangle.Top, Main.maxTilesY));
			int num3 = Math.Max(0, Math.Min(tileRectangle.Right, Main.maxTilesX));
			int num4 = Math.Max(0, Math.Min(tileRectangle.Bottom, Main.maxTilesY));
			return new Rectangle(num, num2, num3 - num, num4 - num2);
		}
	}
}
