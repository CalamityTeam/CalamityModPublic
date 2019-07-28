using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureSilva
{
    public class SilvaCandelabra : ModItem
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
			item.createTile = mod.TileType("SilvaCandelabra");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SilvaCrystal", 5);
            recipe.AddIngredient(mod.ItemType("EffulgentFeather"), 3);
            recipe.SetResult(this);
            recipe.AddTile(null, "SilvaBasin");
            recipe.AddRecipe();
        }
	}
}
