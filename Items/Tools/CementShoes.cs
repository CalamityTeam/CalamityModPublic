using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class CementShoes : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cement Shoes");
            Tooltip.SetDefault("So heavy...\n" +
                "Favorite this item to disable all dashing.");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 30;
            item.rare = 1;
        }

        public override bool CanUseItem(Player player) => false;

        public override void UpdateInventory(Player player) => player.Calamity().cementShoes = true;

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("SiltGroup", 20);
            recipe.AddIngredient(ItemID.Stone, 20);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
