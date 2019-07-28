using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class UnholyCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unholy Core");
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
			item.value = Item.buyPrice(0, 4, 0, 0);
			item.rare = 6;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CharredOre", 4);
            recipe.AddIngredient(ItemID.Hellstone, 4);
            recipe.AddTile(TileID.AdamantiteForge);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
