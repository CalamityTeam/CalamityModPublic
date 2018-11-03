using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
	public class StatigelClock: ModItem
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 22;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.value = 500;
			item.createTile = mod.TileType("StatigelClock");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.IronBar, 3);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ItemID.Glass, 6);
            recipe.AddIngredient(null, "StatigelBlock", 10);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "StaticRefiner");
            recipe.AddRecipe();
        }
	}
}