using CalamityMod.Utilities;
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

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeAstralTiles(Type);
            TileMerge.MergeOreTiles(Type);

            dustType = mod.DustType("AstralBasic");
            drop = mod.ItemType("AstralDirt");

            AddMapEntry(new Color(65, 56, 83));

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CustomTileFraming.FrameTileForCustomMergeFrom(i, j, Type);
            return false;
        }

        public override void RandomUpdate(int i, int j)
        {
            //Make sure that astral grass only spreads to adjacent tiles, as opposed to appearing out of thin air
            Tile up = Main.tile[i, j - 1];
            Tile down = Main.tile[i, j + 1];
            Tile left = Main.tile[i - 1, j];
            Tile right = Main.tile[i + 1, j];
            if (WorldGen.genRand.Next(3) == 0 && (up.type == mod.TileType("AstralGrass") || down.type == mod.TileType("AstralGrass") || left.type == mod.TileType("AstralGrass") || right.type == mod.TileType("AstralGrass")))
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
