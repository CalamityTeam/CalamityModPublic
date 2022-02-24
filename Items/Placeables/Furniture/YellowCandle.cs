using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class YellowCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiteful Candle");
            Tooltip.SetDefault("When placed, nearby enemies take 5% more damage.\n" +
                "This extra damage bypasses enemy damage reduction and defense\n" +
                "'Its hateful glow flickers with ire'");
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
            item.createTile = ModContent.TileType<Tiles.Furniture.YellowCandle>();
        }
    }
}
