using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;

namespace CalamityMod.Tiles.FurnitureAshen
{
	public class AshenSlab : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
			drop = mod.ItemType("AshenSlab");
			soundType = 21;
			mineResist = 5f;
			minPick = 180;
			AddMapEntry(new Color(40, 24, 48));
			animationFrameHeight = 90;
		}
		int animationFrameWidth = 234;

		public override bool CreateDust(int i, int j, ref int type)
		{
			Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
			Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
			return false;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			//Main.NewText(GetTileVariant(i, j));
			int uniqueAnimationFrameX = Main.tileFrame[Type] + i;
			int uniqueAnimationFrameY = Main.tileFrame[Type] + j;
			int xOffset = 0; //Used to shift the pattern tiles patterns
			int xPos = i % 2;
			int yPos = j % 3;
			int xPattern = (i % 20) / 2;
			int yPattern = (j % 30) / 3;
			switch (xPattern)
			{
				case 0:
					switch (yPattern)
					{
						case 1:
							xOffset = 1;
							break;
						case 5:
							xOffset = 0;
							break;
						case 8:
							xOffset = 2;
							break;
						case 9:
							xOffset = 1;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 1:
					switch (yPattern)
					{
						case 0:
							xOffset = 2;
							break;
						case 3:
							xOffset = 0;
							break;
						case 8:
							xOffset = 1;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 2:
					switch (yPattern)
					{
						case 2:
							xOffset = 0;
							break;
						case 7:
							xOffset = 1;
							break;
						case 8:
							xOffset = 0;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 3:
					switch (yPattern)
					{
						case 2:
							xOffset = 2;
							break;
						case 5:
							xOffset = 2;
							break;
						case 7:
							xOffset = 1;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 4:
					switch (yPattern)
					{
						case 1:
							xOffset = 0;
							break;
						case 4:
							xOffset = 0;
							break;
						case 9:
							xOffset = 2;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 5:
					switch (yPattern)
					{
						case 0:
							xOffset = 1;
							break;
						case 7:
							xOffset = 0;
							break;
						case 8:
							xOffset = 1;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 6:
					switch (yPattern)
					{
						case 5:
							xOffset = 2;
							break;
						case 8:
							xOffset = 1;
							break;
						case 12:
							xOffset = 0;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 7:
					switch (yPattern)
					{
						case 2:
							xOffset = 0;
							break;
						case 6:
							xOffset = 0;
							break;
						case 7:
							xOffset = 1;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 8:
					switch (yPattern)
					{
						case 3:
							xOffset = 0;
							break;
						case 4:
							xOffset = 0;
							break;
						case 9:
							xOffset = 0;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 9:
					switch (yPattern)
					{
						case 1:
							xOffset = 0;
							break;
						case 7:
							xOffset = 0;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				default:
					xOffset = -1;
					break;
			}
			if (yPos < 2)
			{
				if (xOffset != -1)
				{
					uniqueAnimationFrameX = GetTileVariant(i, j);
					if (uniqueAnimationFrameX != 0)
					{
						uniqueAnimationFrameX += xOffset;
					}
				}
				else
				{
					uniqueAnimationFrameX = 0;
				}
			}
			switch (yPos)
			{
				case 0:
					switch (xPos)
					{
						case 0:
							uniqueAnimationFrameY = 0;
							break;
						case 1:
							uniqueAnimationFrameY = 2;
							break;
					}
					break;
				case 1:
					switch (xPos)
					{
						case 0:
							uniqueAnimationFrameY = 1;
							break;
						case 1:
							uniqueAnimationFrameY = 3;
							break;
					}
					break;
				case 2:
					uniqueAnimationFrameY = 4 + xPos;
					uniqueAnimationFrameX = 0;
					break;
				default:
					uniqueAnimationFrameY = 0;
					break;
			}
			frameXOffset = uniqueAnimationFrameX * animationFrameWidth;
			frameYOffset = uniqueAnimationFrameY * animationFrameHeight;
		}

		/// <summary>
		/// Gets the tile variant to use. Returns -1 by default
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <returns></returns>
		public int GetTileVariant(int i, int j)
		{
			int variant = 0; //Default to using variant 1
			if (j % 3 < 2)
			{
				//We need to get the particular tile to refer to. This is the top left tile in each iteration of the pattern (large slab above the small rune bricks
				Tile sourceTile = Main.tile[i - (i % 2), j - (j % 3)]; //Gets the tile for this iteration by taking the current position of the tile minus this tile's position in the pattern
																	   //Now to get the particular 'variant group' to use, which is used to take the frameX/frameY of the tile and convert it to the variant the tile is using
				int frameX = sourceTile.frameX / 18;
				int frameY = sourceTile.frameY / 18;
				if (frameY < 3 && !(frameX >= 6 && frameX < 9))
				{
					int group = 0;
					int[] group1XPos = new int[] { 1, 2, 3 };
					int[] group2XPos = new int[] { 0, 4, 5, 9, 10, 11, 12 };
					foreach (int k in group1XPos)
					{
						if (frameX < k + 1 && frameX >= k)
						{
							group = 1;
						}
					}
					foreach (int k in group2XPos)
					{
						if (frameX < k + 1 && frameX >= k)
						{
							group = 2;
						}
					}
					if (group == 1)
					{
						if (frameX < 2)
						{
							variant = 0;
						}
						else if (frameX < 3)
						{
							variant = 1;
						}
						else
						{
							variant = 2;
						}
					}
					else if (group == 2)
					{
						if (frameY < 1)
						{
							variant = 0;
						}
						else if (frameY < 2)
						{
							variant = 1;
						}
						else
						{
							variant = 2;
						}
					}
				}
				else if (frameX < 6 && frameY >= 3)
				{
					if (frameX < 2)
					{
						variant = 0;
					}
					else if (frameX < 4)
					{
						variant = 1;
					}
					else
					{
						variant = 2;
					}
				}
				else if (frameX >= 6 && frameX < 9)
				{
					if (frameX < 7)
					{
						variant = 0;
					}
					else if (frameX < 8)
					{
						variant = 1;
					}
					else
					{
						variant = 2;
					}
				}
				else if (frameX >= 9 && frameY >= 3)
				{
					if (frameX < 10)
					{
						variant = 0;
					}
					else if (frameX < 11)
					{
						variant = 1;
					}
					else
					{
						variant = 2;
					}
				}
			}
			return (variant);
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			int uniqueAnimationFrameX = Main.tileFrame[Type] + i;
			int uniqueAnimationFrameY = Main.tileFrame[Type] + j;
			int xOffset = 0; //Used to shift the pattern tiles patterns
			int xPos = i % 2;
			int yPos = j % 3;
			int xPattern = (i % 20) / 2;
			int yPattern = (j % 30) / 3;
			switch (xPattern)
			{
				case 0:
					switch (yPattern)
					{
						case 1:
							xOffset = 1;
							break;
						case 5:
							xOffset = 0;
							break;
						case 8:
							xOffset = 2;
							break;
						case 9:
							xOffset = 1;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 1:
					switch (yPattern)
					{
						case 0:
							xOffset = 2;
							break;
						case 3:
							xOffset = 0;
							break;
						case 8:
							xOffset = 1;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 2:
					switch (yPattern)
					{
						case 2:
							xOffset = 0;
							break;
						case 7:
							xOffset = 1;
							break;
						case 8:
							xOffset = 0;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 3:
					switch (yPattern)
					{
						case 2:
							xOffset = 2;
							break;
						case 5:
							xOffset = 2;
							break;
						case 7:
							xOffset = 1;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 4:
					switch (yPattern)
					{
						case 1:
							xOffset = 0;
							break;
						case 4:
							xOffset = 0;
							break;
						case 9:
							xOffset = 2;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 5:
					switch (yPattern)
					{
						case 0:
							xOffset = 1;
							break;
						case 7:
							xOffset = 0;
							break;
						case 8:
							xOffset = 1;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 6:
					switch (yPattern)
					{
						case 5:
							xOffset = 2;
							break;
						case 8:
							xOffset = 1;
							break;
						case 12:
							xOffset = 0;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 7:
					switch (yPattern)
					{
						case 2:
							xOffset = 0;
							break;
						case 6:
							xOffset = 0;
							break;
						case 7:
							xOffset = 1;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 8:
					switch (yPattern)
					{
						case 3:
							xOffset = 0;
							break;
						case 4:
							xOffset = 0;
							break;
						case 9:
							xOffset = 0;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				case 9:
					switch (yPattern)
					{
						case 1:
							xOffset = 0;
							break;
						case 7:
							xOffset = 0;
							break;
						default:
							xOffset = -1;
							break;
					}
					break;
				default:
					xOffset = -1;
					break;
			}
			if (yPos < 2)
			{
				if (xOffset != -1)
				{
					uniqueAnimationFrameX = GetTileVariant(i, j);
					if (uniqueAnimationFrameX != 0)
					{
						uniqueAnimationFrameX += xOffset;
					}
				}
				else
				{
					uniqueAnimationFrameX = 0;
				}
			}
			switch (yPos)
			{
				case 0:
					switch (xPos)
					{
						case 0:
							uniqueAnimationFrameY = 0;
							break;
						case 1:
							uniqueAnimationFrameY = 2;
							break;
					}
					break;
				case 1:
					switch (xPos)
					{
						case 0:
							uniqueAnimationFrameY = 1;
							break;
						case 1:
							uniqueAnimationFrameY = 3;
							break;
					}
					break;
				case 2:
					uniqueAnimationFrameY = 4 + xPos;
					uniqueAnimationFrameX = 0;
					break;
				default:
					uniqueAnimationFrameY = 0;
					break;
			}
			int animationFrameHeight = 90;
			int animationFrameWidth = 234;
			int xDrawPos = Main.tile[i, j].frameX + (uniqueAnimationFrameX * animationFrameWidth);
			int yDrawPos = Main.tile[i, j].frameY + (uniqueAnimationFrameY * animationFrameHeight);
			//Initialize the default draw offset of the post drawn sections, then update it to not have the 4 tile offset if camera mode is enabled
			Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X + GetDrawOffset(), j * 16 - Main.screenPosition.Y + GetDrawOffset());
			if (CaptureManager.Instance.IsCapturing)
			{
				drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y);
			}
			Texture2D glowmask = mod.GetTexture("Tiles/FurnitureAshen/AshenSlab_Glowmask");
			spriteBatch.Draw
							 (
							 glowmask,
							 drawOffset,
							 new Rectangle(xDrawPos, yDrawPos, 18, 18),
							 new Color(128, 128, 128, 128),
							 0,
							 new Vector2(0f, 0f),
							 1,
							 SpriteEffects.None,
							 0f
							 );
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
			return (drawOffset - 1);
		}
	}
}