using Microsoft.Xna.Framework;

using Terraria;

namespace CalamityMod.Utilities
{
    public class CalamityCollision
	{
		public static bool HotWetCollision(Vector2 position, int width, int height)
		{
			Vector2 vector = new Vector2(position.X + width / 2, position.Y + height / 2);
			int num = 10;
			int num2 = height / 2;
			if (num > width)
			{
				num = width;
			}
			if (num2 > height)
			{
				num2 = height;
			}
			vector = new Vector2(vector.X - num / 2, vector.Y - num2 / 2);
			int arg_A3_0 = (int)(position.X / 16f) - 1;
			int num3 = (int)((position.X + width) / 16f) + 2;
			int num4 = (int)(position.Y / 16f) - 1;
			int num5 = (int)((position.Y + height) / 16f) + 2;
			int arg_DB_0 = Utils.Clamp(arg_A3_0, 0, Main.maxTilesX - 1);
			num3 = Utils.Clamp(num3, 0, Main.maxTilesX - 1);
			num4 = Utils.Clamp(num4, 0, Main.maxTilesY - 1);
			num5 = Utils.Clamp(num5, 0, Main.maxTilesY - 1);
			for (int i = arg_DB_0; i < num3; i++)
			{
				for (int j = num4; j < num5; j++)
				{
					if (Main.tile[i, j] != null)
					{
						if (Main.tile[i, j + 2].lava() ||
							Main.tile[i, j + 3].lava() ||
							Main.tile[i, j + 4].lava())
						{
							if (Main.tile[i, j].liquid > 0)
							{
								Vector2 vector2;
								vector2.X = i * 16;
								vector2.Y = j * 16;
								int num6 = 16;
								float num7 = 256 - Main.tile[i, j].liquid;
								num7 /= 32f;
								vector2.Y += num7 * 2f;
								num6 -= (int)(num7 * 2f);
								if (vector.X + num > vector2.X && vector.X < vector2.X + 16f && vector.Y + num2 > vector2.Y && vector.Y < vector2.Y + num6)
								{
									return true;
								}
							}
							else if (Main.tile[i, j].active() && Main.tile[i, j].slope() != 0 && j > 0 && Main.tile[i, j - 1] != null && Main.tile[i, j - 1].liquid > 0)
							{
								Vector2 vector2;
								vector2.X = i * 16;
								vector2.Y = j * 16;
								int num8 = 16;
								if (vector.X + num > vector2.X && vector.X < vector2.X + 16f && vector.Y + num2 > vector2.Y && vector.Y < vector2.Y + num8)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}
	}
}
