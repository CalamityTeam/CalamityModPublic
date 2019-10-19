using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureEutrophic
{
    public class EutrophicClock : ModItem
    {
        public override void SetStaticDefaults()
        {
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
            item.createTile = ModContent.TileType<Tiles.EutrophicClock>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Navystone>(), 10);
            recipe.AddIngredient(ModContent.ItemType<PrismShard>(), 3);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 6);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<EutrophicCrafting>());
            recipe.AddRecipe();
        }
    }
}
