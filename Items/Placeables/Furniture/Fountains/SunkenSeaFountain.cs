using Terraria;
using Terraria.ModLoader;
using CalamityMod.Tiles.Furniture.Fountains;

namespace CalamityMod.Items.Placeables.Furniture.Fountains
{
    public class SunkenSeaFountain : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunken Water Fountain");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 32;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 0;
            item.createTile = ModContent.TileType<SunkenSeaFountainTile>();
        }
    }
}
