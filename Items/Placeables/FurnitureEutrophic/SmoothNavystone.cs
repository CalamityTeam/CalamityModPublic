using Terraria.ID;
using Terraria.ModLoader;
// If you are using c# 6, you can use: "using static Terraria.Localization.GameCulture;" which would mean you could just write "DisplayName.AddTranslation(German, "");"

namespace CalamityMod.Items.Placeables.FurnitureEutrophic
{
    public class SmoothNavystone : ModItem
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
			item.createTile = mod.TileType("SmoothNavystone");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("Navystone"), 1);
            recipe.SetResult(this);
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SmoothNavystoneWall", 4);
            recipe.SetResult(this);
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddRecipe();
        }
    }
}
