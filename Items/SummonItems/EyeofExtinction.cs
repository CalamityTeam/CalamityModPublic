using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class EyeofExtinction : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ceremonial Urn");
            Tooltip.SetDefault("Use at the Altar of the Accursed to summon Supreme Calamitas\n" + "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 54;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CalamitousEssence>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>(), 15);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
