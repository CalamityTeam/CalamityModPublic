using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class CorruptionEffigy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corruption Effigy");
            Tooltip.SetDefault("When placed down, nearby players have their movement speed and crit chance increased by 10%\n" +
                "Nearby players also suffer a 5% decrease to their damage reduction");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 32;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 3;
            item.createTile = ModContent.TileType<Tiles.Furniture.CorruptionEffigy>();
        }
    }
}
