using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class CrimsonEffigy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Effigy");
            Tooltip.SetDefault("When placed down nearby players have their damage increased by 15% and defense by 10\n" +
                "Nearby players also suffer a 10% decrease to their maximum health");
        }

        public override void SetDefaults()
        {
            item.width = 22;
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
            item.createTile = ModContent.TileType<Tiles.Furniture.CrimsonEffigy>();
        }
    }
}
