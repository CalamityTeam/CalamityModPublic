using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Graphics.Capture;

namespace CalamityMod.Tiles.FurnitureAshen
{
	public class AshenMonolith : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileLighted[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.Origin = new Point16(0, 4);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			AddMapEntry(new Color(191, 142, 111), name);
			name.SetDefault("Ashen Monolith");
			dustType = mod.DustType("Pixel");
			adjTiles = new int[] { TileID.GrandfatherClocks };
		}
		int animationFrameWidth = 36;

		public override bool HasSmartInteract()
		{
			return true;
		}

		public override bool CreateDust(int i, int j, ref int type)
		{
			Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
			Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
			return false;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 1f;
			g = 0.5f;
			b = 0.5f;
		}

		//public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		//{
		//    int uniqueAnimationFrame = Main.tileFrame[Type] + i;
		//    uniqueAnimationFrame = (uniqueAnimationFrame % 54) + 1;

		//    frameXOffset = uniqueAnimationFrame * animationFrameWidth;
		//}

		//public override void AnimateTile(ref int frame, ref int frameCounter)
		//{
		//    frameCounter++;
		//    if (frameCounter > 6)
		//    {
		//        frameCounter = 0;
		//        frame++;
		//        if (frame > 54)
		//        {
		//            frame = 1;
		//        }
		//    }
		//}

		public override void RightClick(int x, int y)
		{
			{
				string text = "AM";
				//Get current weird time
				double time = Main.time;
				if (!Main.dayTime)
				{
					//if it's night add this number
					time += 54000.0;
				}
				//Divide by seconds in a day * 24
				time = time / 86400.0 * 24.0;
				//Dunno why we're taking 19.5. Something about hour formatting
				time = time - 7.5 - 12.0;
				//Format in readable time
				if (time < 0.0)
				{
					time += 24.0;
				}
				if (time >= 12.0)
				{
					text = "PM";
				}
				int intTime = (int)time;
				//Get the decimal points of time.
				double deltaTime = time - intTime;
				//multiply them by 60. Minutes, probably
				deltaTime = ((int)(deltaTime * 60.0));
				//This could easily be replaced by deltaTime.ToString()
				string text2 = string.Concat(deltaTime);
				if (deltaTime < 10.0)
				{
					//if deltaTime is eg "1" (which would cause time to display as HH:M instead of HH:MM)
					text2 = "0" + text2;
				}
				if (intTime > 12)
				{
					//This is for AM/PM time rather than 24hour time
					intTime -= 12;
				}
				if (intTime == 0)
				{
					//0AM = 12AM
					intTime = 12;
				}
				//Whack it all together to get a HH:MM format
				var newText = string.Concat("Time: ", intTime, ":", text2, " ", text);
				Main.NewText(newText, 255, 240, 20);
			}
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (closer)
			{
				Main.clock = true;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 48, 32, mod.ItemType("AshenMonolith"));
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			if (!Lighting.NotRetro)
			{
				return;
			}
			Tile currentTile = Main.tile[i, j];
			if (currentTile.type == mod.TileType("AshenMonolith"))
			{
				//This is used to draw the eye, where the frame is changed depending on the player's position relative to the eye's centre.
				//To make sure that each section draws correctly, if the tile is the left side of the eye then add 8 to the point compared to the player's position, and if it's on the right then subtract 8
				//Likewise, if frameY = 1 then add 8, if it's 0 then subtract 8
				Vector2 eyeCentre = new Vector2(i * 16, j * 16);
				switch (currentTile.frameX)
				{
					case 0:
						eyeCentre += new Vector2(8f, 0f);
						break;
					case 1:
						eyeCentre -= new Vector2(8f, 0f);
						break;
					default:
						break;
				}
				switch (currentTile.frameY)
				{
					case 0:
						eyeCentre += new Vector2(0f, 8f);
						break;
					case 1:
						eyeCentre -= new Vector2(0f, 8f);
						break;
					default:
						break;
				}
				//The eye should track the closest player
				Vector2 playerPos = eyeCentre;
				float shortestDistance = 9999f;
				for (int x = 0; x < Main.player.Length; x++)
				{
					if ((Main.player[x].position - eyeCentre).Length() < shortestDistance)
					{
						playerPos = Main.player[x].position;
						shortestDistance = (Main.player[x].position - eyeCentre).Length();
					}
				}
				//Use this to determine which eye frame to draw
				//Default positions for the eye (This is for the pupil being centred)
				int frameX = 5;
				int frameY = 2;
				float differenceX = eyeCentre.X - playerPos.X;
				float differenceY = eyeCentre.Y - playerPos.Y;
				int frameXPositive = 1;
				int frameYPositive = 1;
				if (differenceX < 0)
				{
					frameXPositive = -1;
				}
				if (differenceY < 0)
				{
					frameYPositive = -1;
				}
				int frameDifferenceX = 0;
				int frameDifferenceY = 0;
				if (differenceX < -160 || differenceX > 160)
				{
					frameDifferenceX = 5 * frameXPositive;
				}
				else if (differenceX < -92 || differenceX > 92)
				{
					frameDifferenceX = 4 * frameXPositive;
				}
				else if (differenceX < -64 || differenceX > 64)
				{
					frameDifferenceX = 3 * frameXPositive;
				}
				else if (differenceX < -32 || differenceX > 32)
				{
					frameDifferenceX = 2 * frameXPositive;
				}
				else if (differenceX < -16 || differenceX > 16)
				{
					frameDifferenceX = frameXPositive;
				}
				if (differenceY < -64 || differenceY > 64)
				{
					frameDifferenceY = 2 * frameYPositive;
				}
				else if (differenceY < -16 || differenceY > 16)
				{
					frameDifferenceY = frameYPositive;
				}
				frameX -= frameDifferenceX;
				frameY -= frameDifferenceY;
				frameX = frameX * 36;
				frameY = frameY * 90;
				//Initialize the default draw offset of the post drawn sections, then update it to not have the 4 tile offset if camera mode is enabled
				Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X + GetDrawOffset(), j * 16 - Main.screenPosition.Y + GetDrawOffset());
				if (CaptureManager.Instance.IsCapturing)
				{
					drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y);
				}
				Texture2D eyeSheet = mod.GetTexture("Tiles/FurnitureAshen/AshenMonolith_Eye");
				spriteBatch.Draw
				(
					eyeSheet,
					drawOffset,
					new Rectangle(frameX + currentTile.frameX, frameY + currentTile.frameY, 16, 16),
					new Color(255, 255, 255, 255),
					0,
					new Vector2(0f, 0f),
					1,
					SpriteEffects.None,
					0f
				);
			}
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