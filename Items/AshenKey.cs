using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public class AshenKey : ModItem
	{
		public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used in crafting to lock/unlock ashen chests");
        }

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 99;
			item.value = 100;
			item.rare = 1;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GoldenKey);
			recipe.SetResult(this, 1);
            recipe.AddTile(18);
            recipe.AddRecipe();
        }
	}
}
