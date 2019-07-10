using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;

namespace CalamityMod.Walls
{
	public class VoidstoneSlabWall : ModWall
	{
		public override void SetDefaults()
		{
			Main.wallHouse[Type] = true;
			dustType = mod.DustType("Sparkle");
			drop = mod.ItemType("VoidstoneSlabWall");
			AddMapEntry(new Color(19, 17, 22));
		}

		public override bool CreateDust(int i, int j, ref int type)
		{
			Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 180, 0f, 0f, 1, new Color(255, 255, 255), 1f);
			return false;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			bool drawWall = true;
			if (Main.tile[i, j].wall == mod.WallType("VoidstoneSlabWall"))
			{
				drawWall = false;
				Texture2D sprite = mod.GetTexture("Walls/VoidstoneSlabWall");
				Color lightColor = Lighting.GetColor(i, j);
				//Initialize the default draw offset of the post drawn sections, then update it to not have the 4 tile offset if camera mode is enabled
				Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X + GetDrawOffset(), j * 16 - Main.screenPosition.Y + GetDrawOffset());
				if (CaptureManager.Instance.IsCapturing)
				{
					drawOffset = new Vector2(i * 16 - Main.screenPosition.X - 8, j * 16 - Main.screenPosition.Y - 8);
				}
				int[] sheetOffset = CreatePattern(i, j);
				spriteBatch.Draw
					(
						sprite,
						drawOffset,
						new Rectangle(sheetOffset[0] + Main.tile[i, j].wallFrameX(), sheetOffset[1] + Main.tile[i, j].wallFrameY(), 32, 32),
						lightColor,
						0,
						new Vector2(0f, 0f),
						1,
						SpriteEffects.None,
						0f
					);
			}
			return drawWall;
		}

		public int[] CreatePattern(int i, int j)
		{
			int[] sheetOffset = new int[2] { i % 3, j % 3 };
			int xPos = i % 3;
			int yPos = j % 3;
			switch (yPos)
			{
				case 0:
					switch (xPos)
					{
						case 0:
							sheetOffset = new int[2] { 0, 0 };
							break;
						case 1:
							sheetOffset = new int[2] { 0, 1 };
							break;
						case 2:
							sheetOffset = new int[2] { 0, 2 };
							break;
					}
					break;
				case 1:
					switch (xPos)
					{
						case 0:
							sheetOffset = new int[2] { 0, 3 };
							break;
						case 1:
							sheetOffset = new int[2] { 0, 4 };
							break;
						case 2:
							sheetOffset = new int[2] { 0, 0 };
							break;
					}
					break;
				case 2:
					switch (xPos)
					{
						case 0:
							sheetOffset = new int[2] { 0, 1 };
							break;
						case 1:
							sheetOffset = new int[2] { 0, 2 };
							break;
						case 2:
							sheetOffset = new int[2] { 0, 3 };
							break;
					}
					break;
			}
			sheetOffset[0] = sheetOffset[0] * 468;
			sheetOffset[1] = sheetOffset[1] * 180;
			return (sheetOffset);
		}

		/// <summary>
		/// Gets the offset in both axes that should be used for drawing the additions to the tile
		/// </summary>
		/// <returns>The pixel draw offset of the postdrawn sprite in both axes</returns>
		private int GetDrawOffset()
		{
			int drawOffset = 0;
			if (Main.screenWidth < 1664f)
			{
				drawOffset = 193;
			}
			else
			{
				drawOffset = (int)(-0.5f * (float)Main.screenWidth + 1025f);
			}
			return (drawOffset - 9);
		}
	}
}
