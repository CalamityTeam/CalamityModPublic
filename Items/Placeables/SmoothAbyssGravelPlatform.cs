using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
	public class SmoothAbyssGravelPlatform : ModItem
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			item.width = 8;
			item.height = 10;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.createTile = mod.TileType("SmoothAbyssGravelPlatform");
		}

		public override void AddRecipes()
		{
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SmoothAbyssGravel");
            recipe.SetResult(this, 2);
            recipe.AddTile(null, "VoidCondenser");
            recipe.AddRecipe();
        }
	}
}