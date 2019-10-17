
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Tiles
{
    public class ProfanedCrystal : ModTile
    {
        int subsheetWidth = 324;
        int subsheetHeight = 90;
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeDecorativeTiles(Type);
            TileMerge.MergeSmoothTiles(Type);

            soundType = 13;
            mineResist = 1f;
            minPick = 225;
            drop = ModContent.ItemType<Items.ProfanedCrystal>();
            AddMapEntry(new Color(181, 136, 177));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return CustomTileFraming.BetterGemsparkFraming(i, j, resetFrame);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 205, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 2 * subsheetWidth;
            frameYOffset = j % 2 * subsheetHeight;
        }
    }
}
