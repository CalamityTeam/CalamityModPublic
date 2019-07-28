using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureAshen
{
    public class AshenSlab : ModItem
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
            item.rare = 3;
            item.consumable = true;
			item.createTile = mod.TileType("AshenSlab");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.GetItem("SmoothBrimstoneSlag"), 4);
            recipe.AddIngredient(mod.GetItem("UnholyCore"), 1);
            recipe.SetResult(this, 5);
            recipe.AddTile(null, "AshenAltar");
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AshenSlabWall", 4);
            recipe.SetResult(this);
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddRecipe();
        }
    }
}
