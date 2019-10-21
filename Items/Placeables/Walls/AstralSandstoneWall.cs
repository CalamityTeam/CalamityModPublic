using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class AstralSandstoneWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Sandstone Wall");
        }

        public override void SetDefaults()
        {
            item.createWall = ModContent.WallType<WallTiles.AstralSandstoneWall>();
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
            recipe.AddIngredient(ModContent.ItemType<AstralSandstone>());
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
            base.AddRecipes();
        }
    }
}
