using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
	public class CosmiliteChandelier: ModItem
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 26;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.value = 500;
			item.createTile = mod.TileType("CosmiliteChandelier");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CosmiliteBrick", 4);
            recipe.AddIngredient(ItemID.Torch, 4);
            recipe.AddIngredient(ItemID.Chain);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "DraedonsForge");
            recipe.AddRecipe();
        }
	}
}