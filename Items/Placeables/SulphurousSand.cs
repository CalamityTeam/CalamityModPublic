using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class SulphurousSand : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Sand");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Abyss.SulphurousSandNoWater>();
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddTile(18);
            recipe.AddIngredient(ModContent.ItemType<Walls.SulphurousSandWall>(), 4);
            recipe.SetResult(this);
            recipe.AddRecipe();
            base.AddRecipes();
        }
    }
}
