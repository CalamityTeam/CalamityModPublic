
using CalamityMod.Dusts.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles
{
    public class UelibloomBrick : ModTile
    {
        private int extraFrameHeight = 36;
        private int extraFrameWidth = 90;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeDecorativeTiles(Type);

            MineResist = 2f;
            SoundType = SoundID.Tink;
            ItemDrop = ModContent.ItemType<Items.Placeables.UelibloomBrick>();
            AddMapEntry(new Color(174, 108, 46));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<BloomTileGold>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<BloomTileLeaves>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
        {
            if ((Main.tile[i - 1, j - 1].TileType != Type || Main.tile[i, j - 1].TileType != Type || Main.tile[i + 1, j - 1].TileType != Type ||
                Main.tile[i - 1, j - 2].TileType != Type || Main.tile[i, j - 2].TileType != Type || Main.tile[i + 1, j - 2].TileType != Type) &&
                nextSpecialDrawIndex < Main.specX.Length)
            {
                Main.specX[nextSpecialDrawIndex] = i;
                Main.specY[nextSpecialDrawIndex] = j;
                nextSpecialDrawIndex++;
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return TileFraming.BrimstoneFraming(i, j, resetFrame);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Color drawColour = GetDrawColour(i, j);
            Texture2D leaves = ModContent.Request<Texture2D>("CalamityMod/Tiles/UelibloomBrick_Leaves");

            DrawExtraTop(i, j, leaves, drawOffset, drawColour);
            DrawExtraWallEnds(i, j, leaves, drawOffset, drawColour);
            DrawExtraDrapes(i, j, leaves, drawOffset, drawColour);
        }

        #region 'Extra Drapes' Drawing
        private void DrawExtraTop(int i, int j, Texture2D extras, Vector2 drawOffset, Color drawColour)
        {
            /*
                If the tile directly above this tile is not otherworldly stone, or if it is, there is air to both sides of that tile, draw the Extra surface
            */
            if (
                CheckTile(Type, false, 0, 1, i, j) ||
                (CheckTile(Type, true, 0, 1, i, j) && CheckTile(Type, false, 1, 1, i, j) && CheckTile(Type, false, -1, 1, i, j) && CheckTile(Type, true, 1, 0, i, j) && CheckTile(Type, true, -1, 0, i, j))
                )
            {
                Main.spriteBatch.Draw(extras, drawOffset, new Rectangle?(new Rectangle(GetExtraState("middle") + GetExtraVariant(i, j), GetExtraPattern(i), 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                Main.spriteBatch.Draw(extras, drawOffset + new Vector2(0f, 16f), new Rectangle?(new Rectangle(GetExtraState("middle") + GetExtraVariant(i, j), GetExtraPattern(i) + 18, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);

                DrawExtraOverhang(i, j, extras, drawOffset, drawColour);
            }
        }

        private void DrawExtraWallEnds(int i, int j, Texture2D extras, Vector2 drawOffset, Color drawColour)
        {
            /*
                Ending the Extra when a wall is reached
            */

            //Left
            if (
                CheckTile(Type, true, 1, 0, i, j) && CheckTile(Type, false, 1, 1, i, j) && CheckTile(Type, true, 0, 1, i, j) &&
                (CheckTile(Type, true, -1, 1, i, j) || CheckTile(Type, false, -1, 0, i, j))
                )
            {
                Main.spriteBatch.Draw(extras, drawOffset, new Rectangle?(new Rectangle(GetExtraState("wallEndLeft") + GetExtraVariant(i + 1, j), GetExtraPattern(i), 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                Main.spriteBatch.Draw(extras, drawOffset + new Vector2(0f, 16f), new Rectangle?(new Rectangle(GetExtraState("wallEndLeft") + GetExtraVariant(i + 1, j), GetExtraPattern(i) + 18, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            //Right
            if (
                CheckTile(Type, true, -1, 0, i, j) && CheckTile(Type, false, -1, 1, i, j) && CheckTile(Type, true, 0, 1, i, j) &&
                (CheckTile(Type, true, 1, 1, i, j) || CheckTile(Type, false, 1, 0, i, j))
                )
            {
                Main.spriteBatch.Draw(extras, drawOffset, new Rectangle?(new Rectangle(GetExtraState("wallEndRight") + GetExtraVariant(i - 1, j), GetExtraPattern(i), 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                Main.spriteBatch.Draw(extras, drawOffset + new Vector2(0f, 16f), new Rectangle?(new Rectangle(GetExtraState("wallEndRight") + GetExtraVariant(i - 1, j), GetExtraPattern(i) + 18, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
        }

        private void DrawExtraOverhang(int i, int j, Texture2D extras, Vector2 drawOffset, Color drawColour)
        {
            /*
                Called from DrawExtraTop(). Ending the Extra when the edge of the tile is reached
            */

            //Left
            if (
                CheckTile(Type, false, -1, 0, i, j)
                )
            {
                Main.spriteBatch.Draw(extras, drawOffset + new Vector2(-16f, 0f), new Rectangle?(new Rectangle(GetExtraState("overhangLeft") + GetExtraVariant(i, j), GetExtraPattern(i - 1), 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                Main.spriteBatch.Draw(extras, drawOffset + new Vector2(-16f, 16f), new Rectangle?(new Rectangle(GetExtraState("overhangLeft") + GetExtraVariant(i, j), GetExtraPattern(i - 1) + 18, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            //Right
            if (
                CheckTile(Type, false, 1, 0, i, j)
                )
            {
                Main.spriteBatch.Draw(extras, drawOffset + new Vector2(16f, 0f), new Rectangle?(new Rectangle(GetExtraState("overhangRight") + GetExtraVariant(i, j), GetExtraPattern(i + 1), 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                Main.spriteBatch.Draw(extras, drawOffset + new Vector2(16f, 16f), new Rectangle?(new Rectangle(GetExtraState("overhangRight") + GetExtraVariant(i, j), GetExtraPattern(i + 1) + 18, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
        }

        private void DrawExtraDrapes(int i, int j, Texture2D extras, Vector2 drawOffset, Color drawColour)
        {
            /*
                Hanging 'drapes' of the extra element
            */

            //Base
            if (
                (CheckTile(Type, true, 0, 1, i, j) && CheckTile(Type, false, 0, 2, i, j)) ||
                (CheckTile(Type, true, 0, 2, i, j) && CheckTile(Type, false, 1, 2, i, j) && CheckTile(Type, false, -1, 2, i, j) && CheckTile(Type, true, 1, 1, i, j) && CheckTile(Type, true, -1, 1, i, j))
                )
            {
                Main.spriteBatch.Draw(extras, drawOffset, new Rectangle?(new Rectangle(GetExtraState("middle") + GetExtraVariant(i, j - 1), GetExtraPattern(i) + 18, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            //Left Wall
            if (
                CheckTile(Type, true, 1, 1, i, j) && CheckTile(Type, false, 1, 2, i, j) && CheckTile(Type, true, 0, 2, i, j) &&
                (CheckTile(Type, true, -1, 2, i, j) || CheckTile(Type, false, -1, 1, i, j))
                )
            {
                Main.spriteBatch.Draw(extras, drawOffset, new Rectangle?(new Rectangle(GetExtraState("wallEndLeft") + GetExtraVariant(i + 1, j - 1), GetExtraPattern(i) + 18, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            //Right Wall
            if (
                CheckTile(Type, true, -1, 1, i, j) && CheckTile(Type, false, -1, 2, i, j) && CheckTile(Type, true, 0, 2, i, j) &&
                (CheckTile(Type, true, 1, 2, i, j) || CheckTile(Type, false, 1, 1, i, j))
                )
            {
                Main.spriteBatch.Draw(extras, drawOffset, new Rectangle?(new Rectangle(GetExtraState("wallEndRight") + GetExtraVariant(i - 1, j - 1), GetExtraPattern(i) + 18, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            //Left Overhang
            if (
                CheckTile(Type, true, 1, 1, i, j) && CheckTile(Type, false, 0, 1, i, j) && CheckTile(Type, false, 0, 2, i, j) && CheckTile(Type, false, 1, 2, i, j)
                )
            {
                Main.spriteBatch.Draw(extras, drawOffset, new Rectangle?(new Rectangle(GetExtraState("overhangLeft") + GetExtraVariant(i + 1, j - 1), GetExtraPattern(i) + 18, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            //Right Overhang
            if (
                CheckTile(Type, true, -1, 1, i, j) && CheckTile(Type, false, 0, 1, i, j) && CheckTile(Type, false, 0, 2, i, j) && CheckTile(Type, false, -1, 2, i, j)
                )
            {
                Main.spriteBatch.Draw(extras, drawOffset, new Rectangle?(new Rectangle(GetExtraState("overhangRight") + GetExtraVariant(i - 1, j - 1), GetExtraPattern(i) + 18, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
        }
        #endregion

        #region Tile Data
        private bool CheckTile(int type, bool equal, int x, int y, int i, int j)
        {
            //Subtract y so that y is vertical for ease of readability
            return Main.tile[i + x, j - y].TileType == type == equal;
        }

        private Color GetDrawColour(int i, int j)
        {
            int colType = Main.tile[i, j].TileColor;
            Color paintCol = WorldGen.paintColor(colType);
            if (colType < 13)
            {
                paintCol.R = (byte)((paintCol.R / 2f) + 128);
                paintCol.G = (byte)((paintCol.G / 2f) + 128);
                paintCol.B = (byte)((paintCol.B / 2f) + 128);
            }
            if (colType == 29)
            {
                paintCol = Color.Black;
            }
            Color col = Lighting.GetColor(i, j);
            col.R = (byte)(paintCol.R / 255f * col.R);
            col.G = (byte)(paintCol.G / 255f * col.G);
            col.B = (byte)(paintCol.B / 255f * col.B);
            return col;
        }

        private int GetExtraState(string type)
        {
            switch (type)
            {
                case "middle":
                    return 36;
                case "overhangLeft":
                    return 18;
                case "overhangRight":
                    return 54;
                case "wallEndLeft":
                    return 0;
                case "wallEndRight":
                    return 72;
                default:
                    Main.NewText(type.ToString() + " is not a valid Extra sheet state");
                    return 0;
            }
        }

        private int GetExtraPattern(int i)
        {
            return i % 3 * extraFrameHeight;
        }

        private int GetExtraVariant(int i, int j)
        {
            return Main.tile[i, j].frameNumber() * extraFrameWidth;
        }

        /*
        private int GetTileVariant(int i, int j)
        {
            int variant = 0; //Default to using variant 1
            Tile sourceTile = Main.tile[i, j];
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
            return (variant);
        }
        */
        #endregion
    }
}
