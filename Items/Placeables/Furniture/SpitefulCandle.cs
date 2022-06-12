using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    [LegacyName("YellowCandle")]
    public class SpitefulCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Spiteful Candle");
            Tooltip.SetDefault("When placed, nearby enemies take 5% more damage.\n" +
                "This extra damage bypasses enemy damage reduction and defense\n" +
                "'Its hateful glow flickers with ire'");
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
            Item.createTile = ModContent.TileType<Tiles.Furniture.YellowCandle>();
        }
    }
}
