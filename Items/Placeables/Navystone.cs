using CalamityMod.Items.Placeables.FurnitureEutrophic;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class Navystone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Navystone");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.SunkenSea.Navystone>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<NavystoneWallSafe>(), 4);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<NavystoneWall>(), 4);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<EutrophicPlatform>(), 2);
            recipe.SetResult(this);
            recipe.AddTile(ModContent.TileType<EutrophicCrafting>());
            recipe.AddRecipe();
        }
    }
}
