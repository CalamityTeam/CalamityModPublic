using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Items.Placeables.FurnitureBotanic;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class UelibloomBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
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
            Item.createTile = ModContent.TileType<Tiles.UelibloomBrick>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddRecipeGroup("AnyStoneBlock", 100).
                AddIngredient<UelibloomOre>().
                AddTile(TileID.LunarCraftingStation).
                Register();
            CreateRecipe().
                AddIngredient<UelibloomBrickWall>(4).
                AddTile<BotanicPlanter>().
                Register();
            CreateRecipe().
                AddIngredient<BotanicPlatform>(2).
                AddTile<BotanicPlanter>().
                Register();
        }
    }
}
