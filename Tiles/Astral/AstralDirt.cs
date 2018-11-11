using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Astral
{
	public class AstralDirt : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][mod.TileType("AstralGrass")] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;

            dustType = mod.DustType("AstralBasic");
			drop = mod.ItemType("AstralDirt");
            
            AddMapEntry(new Color(65, 56, 83));

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            bool forceSameUp = false;
            bool forceSameDown = false;
            bool forceSameLeft = false;
            bool forceSameRight = false;

            Tile up = Main.tile[i, j - 1];
            Tile down = Main.tile[i, j + 1];
            Tile left = Main.tile[i - 1, j];
            Tile right = Main.tile[i + 1, j];

            bool tmp;

            if (up.active() && CustomTileFraming.tileMergeAstralDirt[up.type])
            {
                bool mergedDown;
                CustomTileFraming.FrameTileForCustomMerge(i, j - 1, up.type, Type, out tmp, out tmp, out tmp, out mergedDown, false, false, false, false, false);
                if (mergedDown)
                {
                    forceSameUp = true;
                }
            }
            if (left.active() && CustomTileFraming.tileMergeAstralDirt[left.type])
            {
                bool mergedRight;
                CustomTileFraming.FrameTileForCustomMerge(i - 1, j, left.type, Type, out tmp, out tmp, out mergedRight, out tmp, false, false, false, false, false);
                if (mergedRight)
                {
                    forceSameLeft = true;
                }
            }
            if (right.active() && CustomTileFraming.tileMergeAstralDirt[right.type])
            {
                bool mergedLeft;
                CustomTileFraming.FrameTileForCustomMerge(i + 1, j, right.type, Type, out tmp, out mergedLeft, out tmp, out tmp, false, false, false, false, false);
                if (mergedLeft)
                {
                    forceSameRight = true;
                }
            }
            if (down.active() && CustomTileFraming.tileMergeAstralDirt[down.type])
            {
                bool mergedUp;
                CustomTileFraming.FrameTileForCustomMerge(i, j + 1, down.type, Type, out mergedUp, out tmp, out tmp, out tmp, false, false, false, false, false);
                if (mergedUp)
                {
                    forceSameDown = true;
                }
            }

            CustomTileFraming.FrameTileForCustomMerge(i, j, Type, TileID.Dirt, forceSameDown, forceSameUp, forceSameLeft, forceSameRight);

            return false;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (WorldGen.genRand.Next(3) == 0)
            {
                WorldGen.SpreadGrass(i, j, Type, mod.TileType("AstralGrass"), false);
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
    }
}