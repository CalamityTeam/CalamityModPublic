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
	}
}
