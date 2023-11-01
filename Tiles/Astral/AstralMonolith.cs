
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Astral
{
    public class AstralMonolith : ModTile
    {
        private static int sheetWidth = 216;
        private static int sheetHeight = 72;

        public byte[,] tileAdjacency;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Wood"]);

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeAstralTiles(Type);
            CalamityUtils.SetMerge(Type, TileID.LeafBlock);
            CalamityUtils.SetMerge(Type, TileID.LivingMahoganyLeaves);
            CalamityUtils.SetMerge(Type, TileID.LivingWood);
            CalamityUtils.SetMerge(Type, TileID.LivingMahogany);

            AddMapEntry(new Color(45, 36, 63));

            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<AstralDirt>(), out tileAdjacency);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<AstralBasic>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int xPos = i % 4;
            int yPos = j % 4;

            if ((xPos == 0 && yPos == 2) || (xPos == 1 && yPos == 3) || (xPos == 3 && yPos == 1))
                Main.tile[i, j].Get<TileWallWireStateData>().TileFrameNumber = Main.tile[i, j - 1].TileFrameNumber;
            else if (xPos == 2 && yPos == 2)
                Main.tile[i, j].Get<TileWallWireStateData>().TileFrameNumber = Main.tile[i - 1, j].TileFrameNumber;
            else if (xPos == 2 && yPos == 3)
                Main.tile[i, j].Get<TileWallWireStateData>().TileFrameNumber = Main.tile[i - 1, j - 1].TileFrameNumber;

            GetDrawSpecifics(i, j, ref xPos, ref yPos);

            frameXOffset = xPos * sheetWidth;
            frameYOffset = yPos * sheetHeight;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            int xOffset = i % 4;
            int yOffset = j % 4;

            GetDrawSpecifics(i, j, ref xOffset, ref yOffset);

            xOffset *= sheetWidth;
            yOffset *= sheetHeight;

            int xPos = tile.TileFrameX;
            int yPos = tile.TileFrameY;
            xPos += xOffset;
            yPos += yOffset;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Tiles/Astral/AstralMonolithGlow").Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
            Color drawColour = GetDrawColour(i, j, new Color(50, 50, 50, 50));
            if (!tile.IsHalfBlock && tile.Slope == 0)
            {
                Main.spriteBatch.Draw(glowmask, drawOffset, new Rectangle?(new Rectangle(xPos, yPos, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else if (tile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(glowmask, drawOffset + new Vector2(0f, 8f), new Rectangle?(new Rectangle(xPos, yPos, 18, 8)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }

            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/AstralDirtMerge");
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {

            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<AstralDirt>(), out tileAdjacency[i, j]);
            TileFraming.CompactFraming(i, j, resetFrame);
            return false;
        }

        private void GetDrawSpecifics(int i, int j, ref int xPos, ref int yPos)
        {
            Tile tile = Main.tile[i, j];
            // Make sure the features work as intended
            if (tile.TileFrameNumber == 1)
            {
                if (xPos == 0 && (yPos == 1 || yPos == 2))
                    yPos += 3;
                else if ((xPos == 1 || xPos == 2) && (yPos == 2 || yPos == 3))
                    yPos += 2;
                else if (xPos == 3 && (yPos == 0 || yPos == 1))
                    yPos += 4;
            }
            else if (tile.TileFrameNumber == 2)
            {
                if (xPos == 1 && yPos == 0)
                {
                    xPos = 0;
                    yPos = 6;
                }
                else if (xPos == 2 && yPos == 1)
                {
                    xPos = 1;
                    yPos = 6;
                }
                else if (xPos == 1 && yPos == 3)
                {
                    xPos = 2;
                    yPos = 6;
                }
                else if (xPos == 3 && yPos == 3)
                {
                    xPos = 3;
                    yPos = 6;
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
        }

        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = Color.Cyan;
            return true;
        }
    }
}
