using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class AstralClay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Clay");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Astral.AstralClay>();
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
            recipe.AddIngredient(this, 2);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(ItemID.Bowl);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(this, 2);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(ItemID.ClayPot);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(this, 2);
            recipe.AddTile(TileID.Furnaces);
            recipe.SetResult(ItemID.PinkVase);
            recipe.AddRecipe();
        }
    }
}
