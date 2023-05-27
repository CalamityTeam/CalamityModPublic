using CalamityMod.Rarities;
using CalamityMod.Tiles.DraedonStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.DraedonStructures
{
    public class PowerCellFactoryItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<PowerCellFactory>();

            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.value = Item.buyPrice(gold: 50);
        }
    }
}
