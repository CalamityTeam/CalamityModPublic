using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureAshen
{
	public class LockedAshenChest : ModItem
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
            item.rare = 3;
            item.value = 0;
            item.createTile = mod.TileType("AshenChestLocked");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SmoothBrimstoneSlag", 8);
            recipe.AddIngredient(ItemID.IronBar, 2);
            recipe.AddIngredient(mod.GetItem("AshenKey"), 1);
            recipe.anyIronBar = true;
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "AshenAltar");
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.GetItem("AshenChest"), 1);
            recipe.AddIngredient(mod.GetItem("AshenKey"), 1);
            recipe.anyIronBar = true;
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "AshenAltar");
            recipe.AddRecipe();
        }
    }
}