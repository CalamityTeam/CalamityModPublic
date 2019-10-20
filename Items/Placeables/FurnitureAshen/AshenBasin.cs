using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureAshen
{
    public class AshenBasin : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.rare = 3;
            item.value = 0;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.FurnitureAshen.AshenBasin>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SmoothBrimstoneSlag>(), 10);
            recipe.AddIngredient(ModContent.ItemType<UnholyCore>(), 5);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<AshenAltar>());
            recipe.AddRecipe();
        }
    }
}
