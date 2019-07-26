using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureSilva
{
	public class SilvaChandelier : ModItem
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
			item.createTile = mod.TileType("SilvaChandelier");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SilvaCrystal", 4);
            recipe.AddIngredient(null, "EffulgentFeather", 4);
            recipe.AddIngredient(ItemID.GoldBar);
            recipe.SetResult(this);
            recipe.AddTile(null, "SilvaBasin");
            recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "SilvaCrystal", 4);
			recipe.AddIngredient(null, "EffulgentFeather", 4);
			recipe.AddIngredient(ItemID.PlatinumBar);
			recipe.SetResult(this);
			recipe.AddTile(null, "SilvaBasin");
			recipe.AddRecipe();
		}
	}
}
