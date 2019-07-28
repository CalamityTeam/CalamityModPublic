using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureAshen
{
    public class AshenDoor : ModItem
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			item.width = 14;
			item.height = 28;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
            item.rare = 3;
            item.consumable = true;
            item.value = 0;
            item.createTile = mod.TileType("AshenDoorClosed");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SmoothBrimstoneSlag", 6);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "AshenAltar");
            recipe.AddRecipe();
        }
	}
}
