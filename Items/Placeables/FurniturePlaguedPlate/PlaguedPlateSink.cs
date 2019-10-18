using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurniturePlaguedPlate
{
    public class PlaguedPlateSink : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Counts as a water source");
        }

        public override void SetDefaults()
        {
            item.width = 8;
            item.height = 10;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.PlaguedPlateSink>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PlaguedPlate", 6);
            recipe.AddIngredient(mod.GetItem("PlagueCellCluster"), 2);
            recipe.AddIngredient(ItemID.WaterBucket, 1);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "PlagueInfuser");
            recipe.AddRecipe();
        }
    }
}
