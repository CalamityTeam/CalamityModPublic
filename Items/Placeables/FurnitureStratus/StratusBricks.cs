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
            SacrificeTotal = 100;
            DisplayName.SetDefault("Stratus Brick");
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureStratus.StratusBricks>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(50).AddRecipeGroup("AnyStoneBlock", 50).AddIngredient(ModContent.ItemType<Lumenyl>(), 3).AddIngredient(ModContent.ItemType<RuinousSoul>(), 1).AddIngredient(ModContent.ItemType<ExodiumCluster>(), 1).AddTile(TileID.LunarCraftingStation).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<StratusWall>(), 4).AddTile(TileID.WorkBenches).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<StratusPlatform>(), 2).AddTile(TileID.LunarCraftingStation).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<StratusStarPlatformItem>(), 2).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
