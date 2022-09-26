using CalamityMod.Tiles.Astral;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralSnow
{
    public class AstralSnow : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Snow"]);

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithSnow(Type);
            CalamityUtils.MergeAstralTiles(Type);

            DustType = 173;
            ItemDrop = ModContent.ItemType<Items.Placeables.AstralSnow>();

            HitSound = SoundID.Item48;

            AddMapEntry(new Color(189, 211, 221));

            TileID.Sets.Snow[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            // CustomTileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AstralDirt>(), false, false, false, false, resetFrame);
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AstralDirt>());
            return false;
        }
    }
}
