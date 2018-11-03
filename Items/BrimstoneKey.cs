using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public class BrimstoneKey : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Grants access to Locked Ashen Chests");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 1;
			item.value = 100;
			item.rare = 1;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ShadowKey);
			recipe.SetResult(this, 1);
            recipe.AddTile(null, "AshenAltar");
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ShadowKey);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "AncientAltar");
            recipe.AddRecipe();
        }
	}
}
