using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Tiles.Crags
{
    public class BrimstoneSlag : ModTile
    {
        private const short subsheetWidth = 450;
        private const short subsheetHeight = 198;
        public byte[,] TileAdjacency;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithHell(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<InfernalSuevite>());

            HitSound = SoundID.Tink;
            MineResist = 2f;
            MinPick = 100;
            AddMapEntry(new Color(53, 33, 56));
            TileFraming.SetUpUniversalMerge(Type, TileID.Ash, out TileAdjacency);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 60, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 2 * subsheetWidth;
            frameYOffset = j % 2 * subsheetHeight;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, TileAdjacency, "CalamityMod/Tiles/Merges/AshMerge");
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Ash, out TileAdjacency[i, j]);
            return TileFraming.BrimstoneFraming(i, j, resetFrame);
        }

        /*public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Texture2D sprite = ModContent.GetTexture("CalamityMod/Tiles/Crags/BrimstoneSlagGlow");
            int frameXOffset = i % 2 * subsheetWidth;
            int frameYOffset = j % 2 * subsheetHeight;
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            Color drawColour = GetDrawColour(i, j, new Color(50, 50, 50, 50));
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            if (tile.Slope == (byte)0 && !tile.IsHalfBlock)
                Main.spriteBatch.Draw(sprite, new Vector2((float)(i * 16 - (int)Main.screenPosition.X), (float)(j * 16 - (int)Main.screenPosition.Y)) + zero, new Rectangle(tile.frameX + frameXOffset, tile.frameY + frameYOffset, 16, 16), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            else if (tile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(sprite, new Vector2((float)(i * 16 - (int)Main.screenPosition.X), (float)(j * 16 - (int)Main.screenPosition.Y + 10) + 8) + zero, new Rectangle(tile.frameX + frameXOffset, tile.frameY + frameYOffset, 16, 8), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else
            {
                byte num9 = tile.Slope;
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
                    Main.spriteBatch.Draw(sprite, new Vector2((float)(i * 16 - (int)Main.screenPosition.X) + (float)num10, (float)(j * 16 - (int)Main.screenPosition.Y + index4 * 2)) + zero, drawRectangle, drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
            }
        }

        private Color GetDrawColour(int i, int j, Color colour)
        {
            int colType = Main.tile[i, j].TileColor;
            Color paintCol = WorldGen.paintColor(colType);
            if (colType >= 13 && colType <= 24)
            {
                colour.R = (byte)(paintCol.R / 255f * colour.R);
                colour.G = (byte)(paintCol.G / 255f * colour.G);
                colour.B = (byte)(paintCol.B / 255f * colour.B);
            }
            return colour;
        }*/
    }
}
