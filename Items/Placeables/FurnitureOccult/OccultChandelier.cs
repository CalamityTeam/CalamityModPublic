using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureOccult
{
    public class OccultChandelier : ModItem
	{
		public override void SetStaticDefaults()
        {
        }

		public override void SetDefaults()
        {
            item.SetNameOverride("Otherworldly Chandelier");
            item.width = 28;
			item.height = 20;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.createTile = mod.TileType("OccultChandelier");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "OccultStone", 12);
            recipe.AddIngredient(mod.ItemType("CosmiliteBrick"), 4);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "DraedonsForge");
            recipe.AddRecipe();
        }
	}
}
