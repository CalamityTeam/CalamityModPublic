using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureStratus
{
    public class StratusCandelabra : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.StratusCandelabra>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StratusBricks>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 3);
            recipe.SetResult(this, 1);
            recipe.AddTile(412);
            recipe.AddRecipe();
        }
    }
}
