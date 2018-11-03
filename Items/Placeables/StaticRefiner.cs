using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
	public class StaticRefiner : ModItem
	{
		public override void SetStaticDefaults()
        {
        }

		public override void SetDefaults()
        {
            item.width = 26;
			item.height = 26;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.createTile = mod.TileType("StaticRefiner");
		}

        //The idea is that this would be a drop from slime god, like the solidifier is from king slime.
        //If this idea is rejected, uncomment the code below, which makes this craftable with 25 purified gel
        //and a solidifier.

		//public override void AddRecipes()
  //      {
  //          ModRecipe recipe = new ModRecipe(mod);
  //          recipe.AddIngredient(null, "PurifiedGel", 25);
  //          recipe.AddIngredient(ItemID.Solidifier);
  //          recipe.SetResult(this, 1);
  //          recipe.AddTile(TileID.Anvils);
  //          recipe.AddRecipe();
  //      }
	}
}