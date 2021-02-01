using Terraria;
using Terraria.ID;
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
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.White;
            item.createTile = ModContent.TileType<SunkenSeaFountainTile>();
        }
    }
}
