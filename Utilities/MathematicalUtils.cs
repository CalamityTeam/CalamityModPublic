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

		// NOTE: A similar function to this one exists in 1.4, but it is not the same underlying function. Check Turn01ToCyclic010 in Utils.cs to see the effect.

		/// <summary>
		/// Converts a 0-1 bound to a 0-1-0 bump. This function automatically clamps the value to the necessary 0-1 range.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		public static float Convert01To010(float value) => (float)Math.Sin(MathHelper.Pi * MathHelper.Clamp(value, 0f, 1f));

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
