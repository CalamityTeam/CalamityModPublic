using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
	public class AstralBrickWall : ModItem
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
			item.useTime = 7;
			item.useStyle = 1;
			item.consumable = true;
			item.createWall = mod.WallType("AstralBrickWall");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("AstralBrick"));
            recipe.SetResult(this, 4);
            recipe.AddTile(18);
            recipe.AddRecipe();
        }
	}
}