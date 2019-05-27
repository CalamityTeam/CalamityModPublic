using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;

namespace CalamityMod.Tiles.FurnitureVoid
{
	public class SmoothVoidstone : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			soundType = 21;
			mineResist = 7f;
			minPick = 190;
			drop = mod.ItemType("SmoothVoidstone");
			AddMapEntry(new Color(27, 24, 31));
		}
		int animationFrameWidth = 288;

		public override bool CreateDust(int i, int j, ref int type)
		{
			Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 180, 0f, 0f, 1, new Color(255, 255, 255), 1f);
			return false;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			int uniqueAnimationFrameX = 0;
			int xPos = i % 4;
			int yPos = j % 4;
			switch (xPos)
			{
				case 0:
					switch (yPos)
					{
						case 0:
							uniqueAnimationFrameX = 0;
							break;
						case 1:
							uniqueAnimationFrameX = 0;
							break;
						case 2:
							uniqueAnimationFrameX = 1;
							break;
						case 3:
							uniqueAnimationFrameX = 1;
							break;
						default:
							uniqueAnimationFrameX = 0;
							break;
					}
					break;
				case 1:
					switch (yPos)
					{
						case 0:
							uniqueAnimationFrameX = 1;
							break;
						case 1:
							uniqueAnimationFrameX = 0;
							break;
						case 2:
							uniqueAnimationFrameX = 1;
							break;
						case 3:
							uniqueAnimationFrameX = 1;
							break;
						default:
							uniqueAnimationFrameX = 0;
							break;
					}
					break;
				case 2:
					switch (yPos)
					{
						case 0:
							uniqueAnimationFrameX = 1;
							break;
						case 1:
							uniqueAnimationFrameX = 0;
							break;
						case 2:
							uniqueAnimationFrameX = 0;
							break;
						case 3:
							uniqueAnimationFrameX = 1;
							break;
						default:
							uniqueAnimationFrameX = 0;
							break;
					}
					break;
				case 3:
					switch (yPos)
					{
						case 0:
							uniqueAnimationFrameX = 0;
							break;
						case 1:
							uniqueAnimationFrameX = 1;
							break;
						case 2:
							uniqueAnimationFrameX = 0;
							break;
						case 3:
							uniqueAnimationFrameX = 1;
							break;
						default:
							uniqueAnimationFrameX = 0;
							break;
					}
					break;
			}
			frameXOffset = uniqueAnimationFrameX * animationFrameWidth;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			if (!Lighting.NotRetro)
			{
				return;
			}
			int xPos = Main.tile[i, j].frameX;
			int yPos = Main.tile[i, j].frameY;
			int xOffset = 0;
			int relativeXPos = i % 4;
			int relativeYPos = j % 4;
			switch (relativeXPos)
			{
				case 0:
					switch (relativeYPos)
					{
						case 0:
							xOffset = 0;
							break;
						case 1:
							xOffset = 0;
							break;
						case 2:
							xOffset = 1;
							break;
						case 3:
							xOffset = 1;
							break;
						default:
							xOffset = 0;
							break;
					}
					break;
				case 1:
					switch (relativeYPos)
					{
						case 0:
							xOffset = 1;
							break;
						case 1:
							xOffset = 0;
							break;
						case 2:
							xOffset = 1;
							break;
						case 3:
							xOffset = 1;
							break;
						default:
							xOffset = 0;
							break;
					}
					break;
				case 2:
					switch (relativeYPos)
					{
						case 0:
							xOffset = 1;
							break;
						case 1:
							xOffset = 0;
							break;
						case 2:
							xOffset = 0;
							break;
						case 3:
							xOffset = 1;
							break;
						default:
							xOffset = 0;
							break;
					}
					break;
				case 3:
					switch (relativeYPos)
					{
						case 0:
							xOffset = 0;
							break;
						case 1:
							xOffset = 1;
							break;
						case 2:
							xOffset = 0;
							break;
						case 3:
							xOffset = 1;
							break;
						default:
							xOffset = 0;
							break;
					}
					break;
			}
			xOffset = xOffset * 288;
			xPos += xOffset;
			Texture2D glowmask = mod.GetTexture("Tiles/FurnitureVoid/SmoothVoidstone_Glowmask");
			//Initialize the default draw offset of the post drawn sections, then update it to not have the 4 tile offset if camera mode is enabled
			Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X + GetDrawOffset(), j * 16 - Main.screenPosition.Y + GetDrawOffset());
			if (CaptureManager.Instance.IsCapturing)
			{
				drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y);
			}
			spriteBatch.Draw
							 (
							 glowmask,
							 drawOffset,
							 new Rectangle(xPos, yPos, 18, 18),
							 new Color(50, 50, 50, 50),
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