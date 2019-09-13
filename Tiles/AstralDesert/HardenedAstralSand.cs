using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Utilities;

namespace CalamityMod.Tiles.AstralDesert
{
	public class HardenedAstralSand : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[mod.TileType("AstralSand")][Type] = true;

            dustType = 108;
			drop = mod.ItemType("HardenedAstralSand");

            AddMapEntry(new Color(45, 36, 63));

            TileID.Sets.Conversion.HardenedSand[Type] = true;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CustomTileFraming.FrameTileForCustomMerge(i, j, Type, mod.TileType("AstralSand"), false, false, false, false, resetFrame);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
    }
}
