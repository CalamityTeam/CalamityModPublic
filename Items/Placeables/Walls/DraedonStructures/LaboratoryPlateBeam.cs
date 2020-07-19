using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls.DraedonStructures;
using TileItems = CalamityMod.Items.Placeables.DraedonStructures;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Walls.DraedonStructures
{
    public class LaboratoryPlateBeam : ModItem
    {
        public override void SetStaticDefaults()
        {
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
            item.createWall = ModContent.WallType<WallTiles.LaboratoryPlateBeam>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<TileItems.LaboratoryPlating>());
            recipe.SetResult(this, 4);
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddRecipe();
        }
    }
}
