using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Walls
{
    public class PlagueContainmentCellsWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plagueplate Wall");
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
            item.createWall = ModContent.WallType<WallTiles.PlagueContainmentCellsWall>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Plates.PlagueContainmentCells>());
            recipe.SetResult(this, 4);
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddRecipe();
        }
    }
}
