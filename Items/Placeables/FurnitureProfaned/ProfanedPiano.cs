using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureProfaned
{
	public class ProfanedPiano: ModItem
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
			item.createTile = mod.TileType("ProfanedPiano");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bone, 4);
            recipe.AddIngredient(null, "ProfanedRock", 15);
            recipe.AddIngredient(ItemID.Book);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "ProfanedBasin");
            recipe.AddRecipe();
        }
	}
}