using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.UI.Chat;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.GameContent.UI;

namespace CalamityMod
{
	public class CalamityCollision
	{
		public static bool HotWetCollision(Vector2 Position, int Width, int Height)
		{
			Vector2 vector = new Vector2(Position.X + (float)(Width / 2), Position.Y + (float)(Height / 2));
			int num = 10;
			int num2 = Height / 2;
			if (num > Width)
			{
				num = Width;
			}
			if (num2 > Height)
			{
				num2 = Height;
			}
			vector = new Vector2(vector.X - (float)(num / 2), vector.Y - (float)(num2 / 2));
			int arg_A3_0 = (int)(Position.X / 16f) - 1;
			int num3 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num4 = (int)(Position.Y / 16f) - 1;
			int num5 = (int)((Position.Y + (float)Height) / 16f) + 2;
			int arg_DB_0 = Utils.Clamp<int>(arg_A3_0, 0, Main.maxTilesX - 1);
			num3 = Utils.Clamp<int>(num3, 0, Main.maxTilesX - 1);
			num4 = Utils.Clamp<int>(num4, 0, Main.maxTilesY - 1);
			num5 = Utils.Clamp<int>(num5, 0, Main.maxTilesY - 1);
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
								vector2.X = (float)(i * 16);
								vector2.Y = (float)(j * 16);
								int num6 = 16;
								float num7 = (float)(256 - (int)Main.tile[i, j].liquid);
								num7 /= 32f;
								vector2.Y += num7 * 2f;
								num6 -= (int)(num7 * 2f);
								if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num6)
								{
									return true;
								}
							}
							else if (Main.tile[i, j].active() && Main.tile[i, j].slope() != 0 && j > 0 && Main.tile[i, j - 1] != null && Main.tile[i, j - 1].liquid > 0)
							{
								Vector2 vector2;
								vector2.X = (float)(i * 16);
								vector2.Y = (float)(j * 16);
								int num8 = 16;
								if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num8)
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