using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
	public class AncientBath : ModItem
	{
		public override void SetStaticDefaults()
        {
        }

		public override void SetDefaults()
        {
            item.width = 28;
			item.height = 20;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.createTile = mod.TileType("AncientBath");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BrimstoneSlag", 14);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "AncientAltar");
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AshenBath", 1);
            recipe.AddIngredient(null, "EssenceofChaos", 1);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "AncientAltar");
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AshenBath", 1);
            recipe.AddIngredient(null, "EssenceofChaos", 1);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "AshenAltar");
            recipe.AddRecipe();
        }
	}
}