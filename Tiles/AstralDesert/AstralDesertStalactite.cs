
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralDesert
{
    public class AstralDesertStalactite : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileBlockLight[Type] = true;

            dustType = mod.DustType("AstralBasic");

            AddMapEntry(new Color(79, 61, 97));

            base.SetDefaults();
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            WorldGen.CheckTight(i, j);
            return false;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameY <= 18 || tile.frameY == 72)
            {
                offsetY = -2;
            }
            else if ((tile.frameY >= 36 && tile.frameY <= 54) || tile.frameY == 90)
            {
                offsetY = 2;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 4;
        }
    }
}
