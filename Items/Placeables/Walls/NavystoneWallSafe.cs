using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
using Terraria.ID;
namespace CalamityMod.Items.Placeables.Walls
{
    public class NavystoneWallSafe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Navystone Wall");
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 7;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createWall = ModContent.WallType<WallTiles.NavystoneWallSafe>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Navystone>());
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
        }
    }
}
