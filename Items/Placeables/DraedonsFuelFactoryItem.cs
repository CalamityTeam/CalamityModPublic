using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Tiles;

namespace CalamityMod.Items.Placeables
{
    public class DraedonsFuelFactoryItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon's Fuel Factory");
        }

        public override void SetDefaults()
        {
            item.rare = 10;
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = item.useTime = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<DraedonFuelFactory>();
        }
    }
}
