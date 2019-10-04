using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Utilities;

namespace CalamityMod.Tiles.AstralDesert
{
	public class AstralSandstone : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeDesertTiles(Type);
            TileMerge.MergeAstralTiles(Type);

            dustType = mod.DustType("AstralBasic");
			drop = mod.ItemType("AstralSandstone");

            AddMapEntry(new Color(79, 61, 97));

            TileID.Sets.Conversion.Sandstone[Type] = true;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CustomTileFraming.FrameTileForCustomMerge(i, j, Type, mod.TileType("HardenedAstralSand"), false, false, false, false, resetFrame);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
    }
}
