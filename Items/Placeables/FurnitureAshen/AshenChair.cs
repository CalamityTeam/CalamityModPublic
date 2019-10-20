using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureAshen
{
    public class AshenChair : ModItem
    {
        public override void SetStaticDefaults()
        {
            //Tooltip.SetDefault("This is a modded chair.");
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 30;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.rare = 3;
            item.useStyle = 1;
            item.consumable = true;
            item.value = 0;
            item.createTile = ModContent.TileType<Tiles.FurnitureAshen.AshenChair>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SmoothBrimstoneSlag>(), 4);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<Tiles.FurnitureAshen.AshenAltar>());
            recipe.AddRecipe();
        }
    }
}
