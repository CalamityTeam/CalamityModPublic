using CalamityMod.Items.Placeables.FurniturePlaguedPlate;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class PlaguedPlateWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 400;
            DisplayName.SetDefault("Plagued Containment Wall");
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<WallTiles.PlaguedPlateWall>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).AddIngredient(ModContent.ItemType<PlaguedPlate>()).AddTile(ModContent.TileType<PlagueInfuser>()).Register();
        }
    }
}
