using CalamityMod.Tiles.DraedonStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class PowerCellFactoryItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Power Cell Factory");
            Tooltip.SetDefault("Produces Draedon Power Cells over time\n" + "One cell is produced every 15 seconds");
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
            item.createTile = ModContent.TileType<PowerCellFactory>();

            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.value = Item.buyPrice(gold: 50);
        }
    }
}
