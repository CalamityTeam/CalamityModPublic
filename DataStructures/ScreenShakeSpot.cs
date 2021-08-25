using Microsoft.Xna.Framework;

namespace CalamityMod.DataStructures
{
	public struct ScreenShakeSpot
	{
		public float ScreenShakePower;
		public Vector2 Position;
		public ScreenShakeSpot(float screenShakePower, Vector2 position)
		{
			ScreenShakePower = screenShakePower;
			Position = position;
		}
	}
}
