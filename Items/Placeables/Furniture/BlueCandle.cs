using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class BlueCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Weightless Candle");
            Tooltip.SetDefault("When placed, nearby players gain 10% movement speed, 10% wing time and 5% acceleration\n" +
                "'The floating flame seems to uplift your very spirit'");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 40;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = Item.buyPrice(0, 25, 0, 0);
            item.rare = ItemRarityID.LightRed;
            item.createTile = ModContent.TileType<Tiles.Furniture.BlueCandle>();
        }
    }
}
