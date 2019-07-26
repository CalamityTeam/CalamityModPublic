using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class BrimstoneSlag : ModTile
	{
        private const short subsheetWidth = 288;
        private const short subsheetHeight = 396;

		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            soundType = 21;
            mineResist = 3f;
            minPick = 180;
            drop = mod.ItemType("BrimstoneSlag");
			AddMapEntry(new Color(53, 33, 56));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }
        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = (i % 2) * subsheetWidth;
            frameYOffset = (j % 2) * subsheetHeight;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Texture2D sprite = mod.GetTexture("Tiles/BrimstoneSlag_Glowmask");
            int frameXOffset = (i % 2) * subsheetWidth;
            int frameYOffset = (j % 2) * subsheetHeight;
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            if (tile.slope() == (byte)0 && !tile.halfBrick())
                Main.spriteBatch.Draw(sprite, new Vector2((float)(i * 16 - (int)Main.screenPosition.X), (float)(j * 16 - (int)Main.screenPosition.Y)) + zero, new Rectangle(tile.frameX + frameXOffset, tile.frameY + frameYOffset, 16, 16), new Color(50, 50, 50, 50), 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            else if (tile.halfBrick())
            {
                Main.spriteBatch.Draw(sprite, new Vector2((float)(i * 16 - (int)Main.screenPosition.X), (float)(j * 16 - (int)Main.screenPosition.Y + 10) + 8) + zero, new Rectangle(tile.frameX + frameXOffset, tile.frameY + frameYOffset, 16, 8), new Color(50, 50, 50, 50), 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else
            {
                byte num9 = tile.slope();
                for (int index4 = 0; index4 < 8; ++index4)
                {
                    int width2 = index4 << 1;
                    Rectangle drawRectangle = new Rectangle(tile.frameX + frameXOffset, tile.frameY + frameYOffset + index4 * 2, width2, 2);
                    int num10 = 0;
                    switch (num9)
                    {
                        case 2:
                            drawRectangle.X = 16 - width2;
                            num10 = 16 - width2;
                            break;
                        case 3:
                            drawRectangle.Width = 16 - width2;
                            break;
                        case 4:
                            drawRectangle.Width = 14 - width2;
                            drawRectangle.X = width2 + 2;
                            num10 = width2 + 2;
                            break;
                    }
                    Main.spriteBatch.Draw(sprite, new Vector2((float)(i * 16 - (int)Main.screenPosition.X) + (float)num10, (float)(j * 16 - (int)Main.screenPosition.Y + index4 * 2)) + zero, drawRectangle, new Color(50, 50, 50, 50), 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
            }
        }
    }
}
