using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    
    public class EutrophicGlass : ModTile
    {
        private static int sheetWidth = 216;
        private static int sheetHeight = 72;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = false;
            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeSmoothTiles(Type);
            CalamityUtils.MergeDecorativeTiles(Type);
            Main.tileLighted[Type] = true;
            Main.tileShine2[Type] = false;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.WallsMergeWith[Type] = true;
            DustType = 108;
            AddMapEntry(new Color(197, 220, 220));
            HitSound = SoundID.Shatter;
            MinPick = 55;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            float transparency = 0.6f;

            // Must be set here 
            TileID.Sets.DrawsWalls[Type] = true;
            Main.tileNoSunLight[Type] = false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture + "_Tile").Value;

            Tile tile = Main.tile[i, j];
            int xPos = i % 10;
            int yPos = j % 10;
            int frameXOffset = xPos * sheetWidth;
            int frameYOffset = yPos * sheetHeight;
            Rectangle frame = new Rectangle(tile.TileFrameX + frameXOffset, tile.TileFrameY + frameYOffset, 16, 16);

            Color color = Lighting.GetColor(i, j) * transparency;
            Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;
            TileFraming.SlopedGlowmask(i, j, tile.TileType, tex, drawPos, frame, GetDrawColour(i, j, color), default);
        }

        private Color GetDrawColour(int i, int j, Color colour)
        {
            int colType = Main.tile[i, j].TileColor;
            Color paintCol = WorldGen.paintColor(colType);
            if (colType >= 0 && colType <= 30)
            {
                colour.R = (byte)(paintCol.R / 255f * colour.R);
                colour.G = (byte)(paintCol.G / 255f * colour.G);
                colour.B = (byte)(paintCol.B / 255f * colour.B);
            }
            return colour;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CompactFraming(i, j, resetFrame);
            return false;
        }
    }
}
