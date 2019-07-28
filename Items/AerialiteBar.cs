using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class AerialiteBar : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aerialite Bar");
		}

		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 24;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 2, 0, 0);
			item.rare = 3;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "AerialiteOre", 4);
			recipe.AddTile(TileID.Furnaces);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
