using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
	public class AncientAltar : ModItem
	{
		public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Capable of creating furniture from the ashes of the Brimstone Crags\nUsed to craft the Ashen and Ancient furniture sets, as well as brimstone tiles");
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
            item.consumable = true;
			item.createTile = mod.TileType("AncientAltar");
		}

		public override void AddRecipes()
		{
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BrimstoneSlag", 10);
            recipe.AddIngredient(null, "CharredOre", 10);
            recipe.SetResult(this, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.AddRecipe();
        }
    }
}