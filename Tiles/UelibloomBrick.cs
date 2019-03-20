using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ObjectData;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;

namespace CalamityMod.Tiles
{
	public class UelibloomBrick : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
			soundType = 21;
			minPick = 200;
			drop = mod.ItemType("UelibloomBrick");
			AddMapEntry(new Color(174, 108, 46));
			animationFrameHeight = 90;
		}
		int animationFrameWidth = 234;

		public override bool CreateDust(int i, int j, ref int type)
		{
			Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 39, 0, 0, 1, new Color(255, 255, 255), 1f);
			Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 10, 0, 0, 1, new Color(255, 255, 255), 1f);
			return false;
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Main.tile[i, j].color(0);
			return true;
		}

		public override bool Slope(int i, int j)
		{
			return false;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			//I'm so sorry about this section
			//This selects the particular pattern to draw, instead of based on just position also being based on relation to an edge.
			int uniqueAnimationFrameX = i % 3;
			int uniqueAnimationFrameY = Main.tileFrame[Type] + j;
			if (Main.tile[i, j - 1].type != mod.TileType("UelibloomBrick"))
			{
				//Row 1 and Row 10
				if ((Main.tile[i, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j - 2].type != mod.TileType("UelibloomBrick")) ||
				(Main.tile[i, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j - 2].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 2, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 2, j - 2].type != mod.TileType("UelibloomBrick")))
				{
					//Row 10
					uniqueAnimationFrameY = 9;
				}
				else
				{
					//Row 1
					uniqueAnimationFrameY = 0;
				}
			}
			else if ((Main.tile[i - 1, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type != mod.TileType("UelibloomBrick")) ||
				(Main.tile[i - 1, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type == mod.TileType("UelibloomBrick")) ||
				(Main.tile[i - 1, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type == mod.TileType("UelibloomBrick")) ||
				(Main.tile[i - 1, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type == mod.TileType("UelibloomBrick")))
			{
				//Row 1 and Row 7
				if (Main.tile[i, j - 1].type != mod.TileType("UelibloomBrick") ||
					((Main.tile[i - 1, j].type == mod.TileType("UelibloomBrick") ||
					Main.tile[i + 1, j].type == mod.TileType("UelibloomBrick")) &&
					Main.tile[i + 1, j - 1].type != mod.TileType("UelibloomBrick") &&
					Main.tile[i, j - 2].type == mod.TileType("UelibloomBrick") &&
					!(Main.tile[i - 1, j - 2].type != mod.TileType("UelibloomBrick") &&
					Main.tile[i - 1, j - 1].type == mod.TileType("UelibloomBrick"))) ||
					((Main.tile[i - 1, j].type == mod.TileType("UelibloomBrick") ||
					Main.tile[i + 1, j].type == mod.TileType("UelibloomBrick")) &&
					Main.tile[i - 1, j - 1].type != mod.TileType("UelibloomBrick") &&
					Main.tile[i, j - 2].type == mod.TileType("UelibloomBrick") &&
					!(Main.tile[i + 1, j - 2].type != mod.TileType("UelibloomBrick") &&
					Main.tile[i + 1, j - 1].type == mod.TileType("UelibloomBrick"))))
				{
					//Row 1
					uniqueAnimationFrameY = 0;
				}
				else
				{
					//Row 7
					uniqueAnimationFrameY = 6;
				}
			}
			else if ((Main.tile[i - 1, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick")))
			{
				//Row 3 and Row 8
				if (Main.tile[i, j - 2].type == mod.TileType("UelibloomBrick") &&
					Main.tile[i + 1, j - 2].type == mod.TileType("UelibloomBrick"))
				{
					//Row 3
					uniqueAnimationFrameY = 2;
				}
				else
				{
					//Row 8
					uniqueAnimationFrameY = 7;
				}
			}
			else if ((Main.tile[i + 1, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick")))
			{
				//Row 5 and Row 9
				if (Main.tile[i, j - 2].type == mod.TileType("UelibloomBrick") &&
					Main.tile[i - 1, j - 2].type == mod.TileType("UelibloomBrick"))
				{
					//Row 5
					uniqueAnimationFrameY = 4;
				}
				else
				{
					//Row 9
					uniqueAnimationFrameY = 8;
				}
			}
			else if ((Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 2].type != mod.TileType("UelibloomBrick")) ||

				(((Main.tile[i - 1, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j - 2].type != mod.TileType("UelibloomBrick") &&
				(Main.tile[i + 1, j - 2].type != mod.TileType("UelibloomBrick") ||
				Main.tile[i + 1, j - 1].type != mod.TileType("UelibloomBrick"))) ||
				(Main.tile[i + 1, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 2].type != mod.TileType("UelibloomBrick")) &&
				(Main.tile[i - 1, j - 2].type != mod.TileType("UelibloomBrick") ||
				Main.tile[i - 1, j - 1].type != mod.TileType("UelibloomBrick")))) ||

				(Main.tile[i - 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j - 2].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 2].type != mod.TileType("UelibloomBrick")))
			{
				//Row 2
				uniqueAnimationFrameY = 1;
			}
			else if ((Main.tile[i - 1, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j - 2].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 2].type == mod.TileType("UelibloomBrick")))
			{
				//Row 4
				uniqueAnimationFrameY = 3;
			}
			else if ((Main.tile[i + 1, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 2].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 2].type == mod.TileType("UelibloomBrick")))
			{
				//Row 6
				uniqueAnimationFrameY = 5;
			}
			else
			{
				uniqueAnimationFrameX = 1;
				uniqueAnimationFrameY = 1;
			}

			frameXOffset = uniqueAnimationFrameX * animationFrameWidth;
			frameYOffset = uniqueAnimationFrameY * animationFrameHeight;
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
		{
			Main.specX[nextSpecialDrawIndex] = i;
			Main.specY[nextSpecialDrawIndex] = j;
			nextSpecialDrawIndex++;
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			//Initialize the default draw offset of the post drawn sections, then update it to not have the 4 tile offset if camera mode is enabled
			//Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X + (Main.screenWidth / 30), j * 16 - Main.screenPosition.Y + (Main.screenWidth / 30));
			Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X + GetDrawOffset(), j * 16 - Main.screenPosition.Y + GetDrawOffset());
			//string debug = "Main.screenWidth: " + Main.screenWidth.ToString();
			//string debug2 = "Tile Position: (" + (i * 16).ToString() + ", " + (j * 16).ToString() + ")";
			//string debug3 = "Draw Position: (" + (i * 16 - Main.screenPosition.X + (Main.screenWidth / 30)).ToString() + ", " + (j * 16 - Main.screenPosition.Y + (Main.screenWidth / 30)).ToString() + ")";
			//string debug5 = "Draw Position to screen width: (" + ((i * 16 - Main.screenPosition.X + (Main.screenWidth / 30)) / Main.screenWidth).ToString() + ", " + ((j * 16 - Main.screenPosition.Y + (Main.screenWidth / 30)) / Main.screenWidth).ToString() + ")";
			//string debug4 = "Tile Screen Position: (" + (i * 16 - Main.screenPosition.X).ToString() + ", " + (j * 16 - Main.screenPosition.Y).ToString() + ")";
			//Main.NewText(debug, Color.White);
			//Main.NewText(debug3, Color.White);
			//Main.NewText(debug5, Color.White);

			if (CaptureManager.Instance.IsCapturing)
			{
				drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y);
			}
			Texture2D cloth = mod.GetTexture("Tiles/UelibloomBrick_Leaves");
			//Left
			if ((Main.tile[i - 1, j].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type != mod.TileType("OccultStone")) ||
				(Main.tile[i, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type != mod.TileType("OccultStone") &&
				Main.tile[i - 1, j].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 1].type != mod.TileType("UelibloomBrick")))
			{
				//xPos must be based on the occult stone tile pos, not this tile
				int xPos = i % 3;
				//We also want to make sure that these sections reflect the colour of the light being cast upon the tile
				Color lightColor = GetDrawColour(i, j);
				switch (xPos)
				{
					case 0:
						spriteBatch.Draw
							(
							cloth,
							drawOffset - new Vector2(16, 0),
							new Rectangle(0, 0, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
					case 1:
						spriteBatch.Draw
							(
							cloth,
							drawOffset - new Vector2(16, 0),
							new Rectangle(0, 36, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
					case 2:
						spriteBatch.Draw
							(
							cloth,
							drawOffset - new Vector2(16, 0),
							new Rectangle(0, 72, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
				}
			}
			//Right
			if ((Main.tile[i + 1, j].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type != mod.TileType("OccultStone")) ||
				(Main.tile[i, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type != mod.TileType("OccultStone") &&
				Main.tile[i + 1, j].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j - 1].type != mod.TileType("UelibloomBrick")))
			{
				//xPos must be based on the occult stone tile pos, not this tile
				int xPos = i % 3;
				//We also want to make sure that these sections reflect the colour of the light being cast upon the tile
				Color lightColor = GetDrawColour(i, j);
				switch (xPos)
				{
					case 0:
						spriteBatch.Draw
							(
							cloth,
							drawOffset + new Vector2(16, 0),
							new Rectangle(36, 0, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
					case 1:
						spriteBatch.Draw
							(
							cloth,
							drawOffset + new Vector2(16, 0),
							new Rectangle(36, 36, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
					case 2:
						spriteBatch.Draw
							(
							cloth,
							drawOffset + new Vector2(16, 0),
							new Rectangle(36, 72, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
				}
			}
			//Bottom Left
			if (((Main.tile[i, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type != mod.TileType("UelibloomBrick")) ||
				(Main.tile[i, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type != mod.TileType("UelibloomBrick")) ||
				(Main.tile[i - 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i - 2, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 2, j - 1].type == mod.TileType("UelibloomBrick"))) &&
				Main.tile[i + 1, j - 1].type != mod.TileType("OccultStone"))
			{
				//xPos must be based on the occult stone tile pos, not this tile
				int xPos = i % 3;
				//We also want to make sure that these sections reflect the colour of the light being cast upon the tile
				Color lightColor = GetDrawColour(i, j);
				switch (xPos)
				{
					case 0:
						spriteBatch.Draw
							(
							cloth,
							drawOffset + new Vector2(-16, 16),
							new Rectangle(0, 18, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
					case 1:
						spriteBatch.Draw
							(
							cloth,
							drawOffset + new Vector2(-16, 16),
							new Rectangle(0, 54, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
					case 2:
						spriteBatch.Draw
							(
							cloth,
							drawOffset + new Vector2(-16, 16),
							new Rectangle(0, 90, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
				}
			}
			//Bottom Right
			if (((Main.tile[i, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type != mod.TileType("UelibloomBrick")) ||
				(Main.tile[i, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type != mod.TileType("UelibloomBrick")) ||
				(Main.tile[i + 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 2, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 2, j - 1].type == mod.TileType("UelibloomBrick"))) &&
				Main.tile[i - 1, j - 1].type != mod.TileType("OccultStone"))
			{
				//xPos must be based on the occult stone tile pos, not this tile
				int xPos = i % 3;
				//We also want to make sure that these sections reflect the colour of the light being cast upon the tile
				Color lightColor = GetDrawColour(i, j);
				switch (xPos)
				{
					case 0:
						spriteBatch.Draw
							(
							cloth,
							drawOffset + new Vector2(16, 16),
							new Rectangle(36, 18, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
					case 1:
						spriteBatch.Draw
							(
							cloth,
							drawOffset + new Vector2(16, 16),
							new Rectangle(36, 54, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
					case 2:
						spriteBatch.Draw
							(
							cloth,
							drawOffset + new Vector2(16, 16),
							new Rectangle(36, 90, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
				}
			}
			//Bottom
			if ((Main.tile[i, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type != mod.TileType("UelibloomBrick")) ||
				(Main.tile[i, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 1].type != mod.TileType("UelibloomBrick")) ||
				(Main.tile[i, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i - 1, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 1].type == mod.TileType("UelibloomBrick")) ||
				(Main.tile[i, j].type == mod.TileType("UelibloomBrick") &&
				Main.tile[i, j - 1].type == mod.TileType("UelibloomBrick") &&
				(Main.tile[i - 1, j].type == mod.TileType("UelibloomBrick") ||
				Main.tile[i + 1, j].type == mod.TileType("UelibloomBrick")) &&
				Main.tile[i - 1, j - 1].type != mod.TileType("UelibloomBrick") &&
				Main.tile[i + 1, j - 1].type != mod.TileType("UelibloomBrick")))
			{
				//xPos must be based on the occult stone tile pos, not this tile
				int xPos = i % 3;
				//We also want to make sure that these sections reflect the colour of the light being cast upon the tile
				Color lightColor = GetDrawColour(i, j);
				switch (xPos)
				{
					case 0:
						spriteBatch.Draw
							(
							cloth,
							drawOffset + new Vector2(0, 16),
							new Rectangle(18, 18, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
					case 1:
						spriteBatch.Draw
							(
							cloth,
							drawOffset + new Vector2(0, 16),
							new Rectangle(18, 54, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
					case 2:
						spriteBatch.Draw
							(
							cloth,
							drawOffset + new Vector2(0, 16),
							new Rectangle(18, 90, 16, 16),
							lightColor,
							0,
							new Vector2(0f, 0f),
							1,
							SpriteEffects.None,
							0f
							);
						break;
				}
			}
		}

		/// <summary>
		/// Gets the colour that should be used for drawing the additions to the tile
		/// </summary>
		/// <param name="i">The x index of the tile</param>
		/// <param name="j">The y index of the tile</param>
		/// <returns></returns>
		private Color GetDrawColour(int i, int j)
		{
			Color lightColour = Lighting.GetColor(i, j);
			//Color paintColour = WorldGen.paintColor(Main.tile[i, j].color());
			//Color drawColour = Color.White;
			//drawColour = lightColour;
			//drawColour.R = (byte)((paintColour.R / 255) * lightColour.R);
			//drawColour.G = (byte)((paintColour.G / 255) * lightColour.G);
			//drawColour.B = (byte)((paintColour.B / 255) * lightColour.B);
			return (lightColour);
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