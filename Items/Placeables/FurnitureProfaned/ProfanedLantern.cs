using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
	public class ProfanedLantern: ModItem
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 26;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.createTile = mod.TileType("ProfanedLantern");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ProfanedRock", 6);
            recipe.AddIngredient(ItemID.Torch);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "ProfanedBasin");
            recipe.AddRecipe();
        }
	}
}