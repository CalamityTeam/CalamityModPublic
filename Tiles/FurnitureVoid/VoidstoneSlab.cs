using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;

namespace CalamityMod.Tiles.FurnitureVoid
{
	public class VoidstoneSlab : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			soundType = 21;
			mineResist = 7f;
			minPick = 190;
			drop = mod.ItemType("VoidstoneSlab");
			AddMapEntry(new Color(27, 24, 31));
			animationFrameHeight = 270;
		}

		public override bool CreateDust(int i, int j, ref int type)
		{
			Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 180, 0f, 0f, 1, new Color(255, 255, 255), 1f);
			return false;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			int uniqueAnimationFrame = Main.tileFrame[Type] + i;
			int xPos = i % 3;
			int yPos = j % 3;
			switch (xPos)
			{
				case 0:
					switch (yPos)
					{
						case 0:
							uniqueAnimationFrame = 0;
							break;
						case 1:
							uniqueAnimationFrame = 1;
							break;
						case 2:
							uniqueAnimationFrame = 2;
							break;
						default:
							uniqueAnimationFrame = 0;
							break;
					}
					break;
				case 1:
					switch (yPos)
					{
						case 0:
							uniqueAnimationFrame = 2;
							break;
						case 1:
							uniqueAnimationFrame = 3;
							break;
						case 2:
							uniqueAnimationFrame = 4;
							break;
						default:
							uniqueAnimationFrame = 0;
							break;
					}
					break;
				case 2:
					switch (yPos)
					{
						case 0:
							uniqueAnimationFrame = 4;
							break;
						case 1:
							uniqueAnimationFrame = 0;
							break;
						case 2:
							uniqueAnimationFrame = 1;
							break;
						default:
							uniqueAnimationFrame = 0;
							break;
					}
					break;
			}

			frameYOffset = uniqueAnimationFrame * animationFrameHeight;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			int xPos = Main.tile[i, j].frameX;
			int yPos = Main.tile[i, j].frameY;
			int yOffset = 0;
			int relavtiveXPos = i % 3;
			int relavtiveYPos = j % 3;
			switch (relavtiveXPos)
			{
				case 0:
					switch (relavtiveYPos)
					{
						case 0:
							yOffset = 0;
							break;
						case 1:
							yOffset = 1;
							break;
						case 2:
							yOffset = 2;
							break;
						default:
							yOffset = 0;
							break;
					}
					break;
				case 1:
					switch (relavtiveYPos)
					{
						case 0:
							yOffset = 2;
							break;
						case 1:
							yOffset = 3;
							break;
						case 2:
							yOffset = 4;
							break;
						default:
							yOffset = 0;
							break;
					}
					break;
				case 2:
					switch (relavtiveYPos)
					{
						case 0:
							yOffset = 4;
							break;
						case 1:
							yOffset = 0;
							break;
						case 2:
							yOffset = 1;
							break;
						default:
							yOffset = 0;
							break;
					}
					break;
			}
			yOffset = yOffset * 270;
			yPos += yOffset;
			Texture2D glowmask = mod.GetTexture("Tiles/FurnitureVoid/VoidstoneSlab_Glowmask");
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
							 new Color(75, 75, 75, 75),
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