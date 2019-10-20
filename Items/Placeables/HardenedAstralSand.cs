using CalamityMod.Items.Placeables.Walls;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class HardenedAstralSand : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hardened Astral Sand");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.AstralDesert.HardenedAstralSand>();
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
            recipe.AddIngredient(ModContent.ItemType<HardenedAstralSandWall>(), 4);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
            base.AddRecipes();
        }
    }
}
