using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class BlueCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Weightless Candle");
            Tooltip.SetDefault("When placed, nearby players gain 10% movement speed, 10% wing time and 5% acceleration\n" +
                "'The floating flame seems to uplift your very spirit'");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 40;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 25, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.createTile = ModContent.TileType<Tiles.Furniture.BlueCandle>();
        }
    }
}
