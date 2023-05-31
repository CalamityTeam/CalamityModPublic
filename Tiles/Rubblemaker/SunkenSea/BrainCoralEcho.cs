using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Rubblemaker.SunkenSea
{
    public class BrainCoralEcho : ModTile
    {
        public override string Texture => "CalamityMod/Tiles/SunkenSea/BrainCoral";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(36, 61, 111));
            RegisterItemDrop(ModContent.ItemType<Navystone>());
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<Navystone>(), Type, 0);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
