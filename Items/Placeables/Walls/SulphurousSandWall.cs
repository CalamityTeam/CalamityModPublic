using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class SulphurousSandWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Sand Wall");
        }

        public override void SetDefaults()
        {
            item.createWall = ModContent.WallType<WallTiles.SulphurousSandWallSafe>();
            item.useStyle = ItemUseStyleID.SwingThrow;
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
            recipe.AddTile(
                TileID.WorkBenches);
            recipe.AddIngredient(ModContent.ItemType<SulphurousSand>());
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
            base.AddRecipes();
        }
    }
}
