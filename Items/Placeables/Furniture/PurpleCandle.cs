using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class PurpleCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Resilient Candle");
            Tooltip.SetDefault("When placed, nearby players' defense blocks 5% more damage\n" +
                "'Neither rain nor wind can snuff its undying flame'");
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
            item.createTile = ModContent.TileType<Tiles.Furniture.PurpleCandle>();
        }
    }
}
