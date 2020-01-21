using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureAstral
{
    public class MonolithBench : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.SetNameOverride("Monolith Bench");
            item.width = 28;
            item.height = 20;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.FurnitureAstral.MonolithBench>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AstralMonolith>(), 5);
            recipe.AddIngredient(ItemID.Silk, 2);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<MonolithCrafting>());
            recipe.AddRecipe();
        }
    }
}
