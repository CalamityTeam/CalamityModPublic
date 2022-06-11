using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurniturePlaguedPlate
{
    public class PlaguedPlate : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            DisplayName.SetDefault("Plagued Containment Brick");
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
            Item.createTile = ModContent.TileType<Tiles.FurniturePlaguedPlate.PlaguedPlate>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).AddRecipeGroup("AnyStoneBlock", 10).AddIngredient(ModContent.ItemType<PlagueCellCanister>()).AddTile(ModContent.TileType<PlagueInfuser>()).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<PlaguedPlateWall>(), 4).AddTile(ModContent.TileType<PlagueInfuser>()).Register();
            CreateRecipe(1).AddIngredient(ModContent.ItemType<PlaguedPlatePlatform>(), 2).AddTile(ModContent.TileType<PlagueInfuser>()).Register();
        }
    }
}
