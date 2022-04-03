using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class PinkCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vigorous Candle");
            Tooltip.SetDefault("When placed, nearby players regenerate 0.4% of their maximum health per second\n" +
                "This regeneration is at full power even while moving and bypasses Revengeance Mode caps\n" +
                "'Its brilliant light suffuses those nearby with hope'");
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
            Item.createTile = ModContent.TileType<Tiles.Furniture.PinkCandle>();
        }
    }
}
