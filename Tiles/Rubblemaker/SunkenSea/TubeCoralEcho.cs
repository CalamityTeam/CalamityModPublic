using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Rubblemaker.SunkenSea
{
    public class TubeCoralEcho : ModTile
    {
        public override string Texture => "CalamityMod/Tiles/SunkenSea/TubeCoral";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new[]
            {
                16,
                16,
                16
            };
            TileObjectData.addTile(Type);
            DustType = 253;
            AddMapEntry(new Color(36, 61, 111));
            RegisterItemDrop(ModContent.ItemType<Navystone>());
            FlexibleTileWand.RubblePlacementMedium.AddVariations(ModContent.ItemType<Navystone>(), Type, 0);

            base.SetStaticDefaults();
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
