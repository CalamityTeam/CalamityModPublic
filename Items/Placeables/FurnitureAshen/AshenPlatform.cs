using Terraria.ModLoader;
namespace CalamityMod.Items
{
    public class AshenPlatform : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 8;
            item.height = 10;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.rare = 3;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.AshenPlatform>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SmoothBrimstoneSlag");
            recipe.SetResult(this, 2);
            recipe.AddTile(null, "AshenAltar");
            recipe.AddRecipe();
        }
    }
}
