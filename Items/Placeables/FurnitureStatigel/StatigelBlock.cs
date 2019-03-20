using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace CalamityMod.Items.Placeables.FurnitureStatigel
{
	public class StatigelBlock : ModItem
	{
		public override void SetStaticDefaults()
		{
        }

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
            item.consumable = true;
			item.createTile = mod.TileType("StatigelBlock");
		}

		public override void AddRecipes()
		{
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PurifiedGel");
            recipe.SetResult(this);
            recipe.AddTile(null, "StaticRefiner");
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "StatigelPlatform", 2);
            recipe.SetResult(this);
            recipe.AddTile(null, "StaticRefiner");
            recipe.AddRecipe();
        }
	}
}
