using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureWulfrum
{
    public class WulfrumTable : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureWulfrum.WulfrumTable>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumPlating>(8).
                AddTile<WulfrumLabstation>().
                Register();
        }
    }
}
