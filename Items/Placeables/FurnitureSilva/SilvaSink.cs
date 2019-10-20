using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSilva
{
    public class SilvaSink : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Counts as a water source");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.FurnitureSilva.SilvaSink>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilvaCrystal>(), 6);
            recipe.AddIngredient(ItemID.WaterBucket);
            recipe.SetResult(this);
            recipe.AddTile(ModContent.TileType<Tiles.FurnitureSilva.SilvaBasin>());
            recipe.AddRecipe();
        }
    }
}
