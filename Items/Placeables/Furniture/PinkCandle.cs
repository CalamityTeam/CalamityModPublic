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
            item.createTile = ModContent.TileType<Tiles.Furniture.PinkCandle>();
        }
    }
}
