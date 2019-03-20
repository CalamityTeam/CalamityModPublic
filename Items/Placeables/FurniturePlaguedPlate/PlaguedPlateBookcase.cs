using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurniturePlaguedPlate
{
	public class PlaguedPlateBookcase : ModItem
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
			item.createTile = mod.TileType("PlaguedPlateBookcase");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PlaguedPlate", 20);
            recipe.AddIngredient(mod.GetItem("PlagueCellCluster"), 2);
            recipe.AddIngredient(ItemID.Book);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "PlagueInfuser");
            recipe.AddRecipe();
        }
	}
}