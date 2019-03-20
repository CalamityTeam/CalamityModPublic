using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;

namespace CalamityMod.Tiles
{
	public class Voidstone : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
            soundType = 21;
            mineResist = 10f;
            minPick = 189;
			dustType = 187;
			drop = mod.ItemType("Voidstone");
			AddMapEntry(new Color(10, 10, 10));
        }

		public override bool CanExplode(int i, int j)
		{
			return false;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (!closer && j < Main.maxTilesY - 205)
			{
				if (Main.tile[i, j].liquid <= 0)
				{
					Main.tile[i, j].liquid = 255;
					Main.tile[i, j].lava(false);
				}
			}
		}

		public override void RandomUpdate(int i, int j)
		{
			if (Main.rand.Next(20) == 0)
			{
				if (NPC.downedPlantBoss || CalamityWorld.downedCalamitas)
				{
					int random = WorldGen.genRand.Next(4);
					if (random == 0)
					{
						i++;
					}
					if (random == 1)
					{
						i--;
					}
					if (random == 2)
					{
						j++;
					}
					if (random == 3)
					{
						j--;
					}
					if (Main.tile[i, j] != null)
					{
						if (!Main.tile[i, j].active())
						{
							if (!Main.tile[i, j].lava() && Main.tile[i, j].slope() == 0 && !Main.tile[i, j].halfBrick())
							{
								Main.tile[i, j].type = (ushort)mod.TileType("LumenylCrystals");
								Main.tile[i, j].active(true);
								if (Main.tile[i, j + 1].active() && Main.tileSolid[Main.tile[i, j + 1].type] && Main.tile[i, j + 1].slope() == 0 && !Main.tile[i, j + 1].halfBrick())
								{
									Main.tile[i, j].frameY = (short)(0 * 18);
								}
								else if (Main.tile[i, j - 1].active() && Main.tileSolid[Main.tile[i, j - 1].type] && Main.tile[i, j - 1].slope() == 0 && !Main.tile[i, j - 1].halfBrick())
								{
									Main.tile[i, j].frameY = (short)(1 * 18);
								}
								else if (Main.tile[i + 1, j].active() && Main.tileSolid[Main.tile[i + 1, j].type] && Main.tile[i + 1, j].slope() == 0 && !Main.tile[i + 1, j].halfBrick())
								{
									Main.tile[i, j].frameY = (short)(2 * 18);
								}
								else if (Main.tile[i - 1, j].active() && Main.tileSolid[Main.tile[i - 1, j].type] && Main.tile[i - 1, j].slope() == 0 && !Main.tile[i - 1, j].halfBrick())
								{
									Main.tile[i, j].frameY = (short)(3 * 18);
								}
								Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(18) * 18);
								WorldGen.SquareTileFrame(i, j, true);
								if (Main.netMode == 2)
								{
									NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
								}
							}
						}
					}
				}
			}
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
                            uniqueAnimationFrameX = 2;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 1:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 2:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 3:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
            }
            frameXOffset = uniqueAnimationFrameX * 288;
        }

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
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
							xOffset = 2;
							break;
						case 2:
							xOffset = 1;
							break;
						case 3:
							xOffset = 2;
							break;
						default:
							xOffset = 2;
							break;
					}
					break;
				case 1:
					switch (relativeYPos)
					{
						case 0:
							xOffset = 2;
							break;
						case 1:
							xOffset = 0;
							break;
						case 2:
							xOffset = 2;
							break;
						case 3:
							xOffset = 2;
							break;
						default:
							xOffset = 2;
							break;
					}
					break;
				case 2:
					switch (relativeYPos)
					{
						case 0:
							xOffset = 2;
							break;
						case 1:
							xOffset = 0;
							break;
						case 2:
							xOffset = 1;
							break;
						case 3:
							xOffset = 2;
							break;
						default:
							xOffset = 2;
							break;
					}
					break;
				case 3:
					switch (relativeYPos)
					{
						case 0:
							xOffset = 1;
							break;
						case 1:
							xOffset = 2;
							break;
						case 2:
							xOffset = 0;
							break;
						case 3:
							xOffset = 2;
							break;
						default:
							xOffset = 2;
							break;
					}
					break;
			}
			xOffset = xOffset * 288;
			xPos += xOffset;
			Texture2D glowmask = mod.GetTexture("Tiles/Voidstone_Glowmask");
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
							 new Color(25, 25, 25, 25),
							 0,
							 new Vector2(0f, 0f),
							 1,
							 SpriteEffects.None,
							 0f
							 );
		}

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