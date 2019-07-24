using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureVoid
{
	public class VoidChest : ModItem
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
			item.createTile = mod.TileType("VoidChest");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SmoothVoidstone", 8);
            recipe.AddIngredient(ItemID.IronBar, 2);
            recipe.anyIronBar = true;
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "VoidCondenser");
            recipe.AddRecipe();
        }
    }
}