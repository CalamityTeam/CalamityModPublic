using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureAbyss
{
    public class AbyssChair : ModItem
    {
        public override void SetStaticDefaults()
        {
            //Tooltip.SetDefault("This is a modded chair.");
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 30;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.value = 0;
            item.createTile = ModContent.TileType<Tiles.FurnitureAbyss.AbyssChair>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SmoothAbyssGravel>(), 4);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<VoidCondenser>());
            recipe.AddRecipe();
        }
    }
}
