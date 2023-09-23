using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureWulfrum;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureWulfrum
{
    [LegacyName("WulfrumWorkbench")]
    public class WulfrumWorkBench : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 18;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<WulfrumWorkBenchTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumPlating>(10).
                AddTile<WulfrumLabstation>().
                Register();
        }
    }
}
