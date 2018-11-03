using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
// If you are using c# 6, you can use: "using static Terraria.Localization.GameCulture;" which would mean you could just write "DisplayName.AddTranslation(German, "");"
using Terraria.Localization;

namespace CalamityMod.Items.Placeables
{
	public class VoidstoneSlab : ModItem
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
			item.createTile = mod.TileType("VoidstoneSlab");
		}

		public override void AddRecipes()
		{
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SmoothVoidstone");
            recipe.SetResult(this);
            recipe.AddTile(null, "VoidCondenser");
            recipe.AddRecipe();
        }
	}
}
