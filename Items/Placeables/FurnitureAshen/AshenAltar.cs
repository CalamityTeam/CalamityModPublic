using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureAshen
{
	public class AshenAltar : ModItem
	{
		public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used for special crafting") ;
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
            item.rare = 3;
            item.value = 0;
            item.consumable = true;
			item.createTile = mod.TileType("AshenAltar");
		}

		public override void AddRecipes()
		{
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SmoothBrimstoneSlag", 10);
            recipe.AddIngredient(null, "UnholyCore", 10);
            recipe.SetResult(this, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.AddRecipe();
        }
	}
}