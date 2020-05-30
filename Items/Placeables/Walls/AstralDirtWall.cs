using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Walls
{
    public class AstralDirtWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Dirt Wall");
        }

        public override void SetDefaults()
        {
            item.createWall = ModContent.WallType<WallTiles.AstralDirtWallSafe>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 7;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddIngredient(ModContent.ItemType<AstralDirt>());
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
            base.AddRecipes();
        }
    }
}
