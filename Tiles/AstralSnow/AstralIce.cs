using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Utilities;

namespace CalamityMod.Tiles.AstralSnow
{
	public class AstralIce : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeSnowTiles(Type);
            TileMerge.MergeAstralTiles(Type);

            dustType = 173;
			drop = mod.ItemType("AstralIce");

            soundType = 2;
            soundStyle = 50;

            AddMapEntry(new Color(153, 143, 168));

            TileID.Sets.Ices[Type] = true;
            TileID.Sets.IcesSlush[Type] = true;
            TileID.Sets.IcesSnow[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.Conversion.Ice[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

        public override void FloorVisuals(Player player)
        {
            player.slippy = true;
            base.FloorVisuals(player);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CustomTileFraming.FrameTileForCustomMerge(i, j, Type, mod.TileType("AstralDirt"), false, false, false, false, resetFrame);
            return false;
        }
    }
}
