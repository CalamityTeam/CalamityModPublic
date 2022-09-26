
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralDesert
{
    public class AstralSandstone : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);
            CalamityUtils.MergeAstralTiles(Type);

            DustType = ModContent.DustType<AstralBasic>();
            ItemDrop = ModContent.ItemType<Items.Placeables.AstralSandstone>();

            AddMapEntry(new Color(93, 78, 107));

            TileID.Sets.Conversion.Sandstone[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            // CustomTileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<HardenedAstralSand>(), false, false, false, false, resetFrame);
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<HardenedAstralSand>());
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
