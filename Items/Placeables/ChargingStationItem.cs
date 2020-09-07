using CalamityMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class ChargingStationItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Charging Station");
            Tooltip.SetDefault("Charges Draedon's Arsenal items using Power Cells\n" +
                "Place both an item and Power Cells into the Charging Station to charge the item");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = item.useTime = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<ChargingStation>();

            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.value = Item.buyPrice(gold: 50);
        }
    }
}
