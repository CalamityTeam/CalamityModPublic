
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using Terraria.ID;

namespace CalamityMod.Tiles
{
    public class AstralStone : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeAstralTiles(Type);
            TileMerge.MergeOreTiles(Type);

            dustType = ModContent.DustType<AstralBasic>();
            drop = ModContent.ItemType<Items.AstralStone>();

            soundType = 21;

            AddMapEntry(new Color(45, 36, 63));

            TileID.Sets.Stone[Type] = true;
            TileID.Sets.Conversion.Stone[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CustomTileFraming.FrameTileForCustomMerge(i, j, Type, ModContent.TileType<AstralDirt>());
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
