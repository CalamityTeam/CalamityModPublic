using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
	public class EyeofExtinction : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye of Extinction");
            Tooltip.SetDefault("Summons Supreme Calamitas at the altar but is not consumed");
        }

        public override void SetDefaults()
        {
            item.width = 54;
            item.height = 42;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CalamitousEssence>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>(), 15);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
