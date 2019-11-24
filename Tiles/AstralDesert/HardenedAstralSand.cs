
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralDesert
{
    public class HardenedAstralSand : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeDesertTiles(Type);
            TileMerge.MergeAstralTiles(Type);


            dustType = 108;
            drop = ModContent.ItemType<Items.Placeables.HardenedAstralSand>();

            AddMapEntry(new Color(128, 128, 158));

            TileID.Sets.Conversion.HardenedSand[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            // CustomTileFraming.FrameTileForCustomMerge(i, j, Type, ModContent.TileType<AstralSand>(), false, false, false, false, resetFrame);
            CustomTileFraming.FrameTileForCustomMergeFrom(i, j, Type, ModContent.TileType<AstralSand>());
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
