using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Astral
{
    public class AstralDirt : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Dirt"]);

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeAstralTiles(Type);
            CalamityUtils.MergeWithOres(Type);
            CalamityUtils.SetMerge(Type, TileID.Grass);
            CalamityUtils.SetMerge(Type, TileID.CorruptGrass);
            CalamityUtils.SetMerge(Type, TileID.HallowedGrass);
            CalamityUtils.SetMerge(Type, TileID.CrimsonGrass);

            DustType = ModContent.DustType<AstralBasic>();
            ItemDrop = ModContent.ItemType<Items.Placeables.AstralDirt>();

            AddMapEntry(new Color(59, 50, 77));

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, TileID.Dirt, false, false, false);
            return false;
        }

        public override void RandomUpdate(int i, int j)
        {
            //Make sure that astral grass only spreads to adjacent tiles, as opposed to appearing out of thin air
            Tile up = Main.tile[i, j - 1];
            Tile down = Main.tile[i, j + 1];
            Tile left = Main.tile[i - 1, j];
            Tile right = Main.tile[i + 1, j];
            if (WorldGen.genRand.NextBool(3) && (up.TileType == ModContent.TileType<AstralGrass>() || down.TileType == ModContent.TileType<AstralGrass>() || left.TileType == ModContent.TileType<AstralGrass>() || right.TileType == ModContent.TileType<AstralGrass>()))
            {
                WorldGen.SpreadGrass(i, j, Type, ModContent.TileType<AstralGrass>(), false);
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
