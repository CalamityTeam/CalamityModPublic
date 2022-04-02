using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureStratus
{
    public class StratusBricks : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stratus Brick");
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.FurnitureStratus.StratusBricks>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyStoneBlock", 50);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 3);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 1);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 1);
            recipe.SetResult(this, 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StratusWall>(), 4);
            recipe.SetResult(this, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StratusPlatform>(), 2);
            recipe.SetResult(this);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<StratusStarPlatformItem>(), 2);
            recipe.SetResult(this);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.AddRecipe();
        }
    }
}
