using CalamityMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class Rock : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rock");
            Tooltip.SetDefault("The first object Xeroc ever created");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 0, 0, 1);
            item.createTile = ModContent.TileType<PlacedRock>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
        }
    }
}
