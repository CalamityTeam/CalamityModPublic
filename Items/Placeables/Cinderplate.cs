using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class Cinderplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinderplate");
            Tooltip.SetDefault("It resonates with otherworldly energy.");
        }

        public override void SetDefaults()
        {
            item.createTile = mod.TileType("Cinderplate");
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
			item.value = Item.buyPrice(0, 0, 10, 0);
			item.rare = 3;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Hellstone, 6);
			recipe.AddIngredient(ItemID.Obsidian, 3);
			recipe.SetResult(this);
			recipe.AddTile(TileID.Anvils);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "CinderplateWall", 4);
			recipe.SetResult(this);
			recipe.AddTile(TileID.Anvils);
			recipe.AddRecipe();
		}
	}
}
