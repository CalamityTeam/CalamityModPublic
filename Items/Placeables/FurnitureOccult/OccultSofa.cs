using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureOccult
{
	public class OccultSofa : ModItem
	{
		public override void SetStaticDefaults()
        {
        }

		public override void SetDefaults()
        {
            item.SetNameOverride("Otherworldly Sofa");
            item.width = 28;
			item.height = 20;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.createTile = mod.TileType("OccultSofa");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "OccultStone", 5);
            recipe.AddIngredient(ItemID.Silk, 2);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "DraedonsForge");
            recipe.AddRecipe();
        }
	}
}