using CalamityMod.Items.Placeables.Walls;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class AstralDirt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Dirt");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Astral.AstralDirt>();
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddTile(18);
            recipe.AddIngredient(ModContent.ItemType<AstralDirtWall>(), 4);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
            base.AddRecipes();
        }
    }
}
