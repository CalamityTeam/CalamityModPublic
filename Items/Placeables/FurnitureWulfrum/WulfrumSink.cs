using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureWulfrum
{
    public class WulfrumSink : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureWulfrum.WulfrumSink>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumPlating>(6).
                AddIngredient(ItemID.WaterBucket).
                AddTile<WulfrumLabstation>().
                Register();
        }
    }
}
