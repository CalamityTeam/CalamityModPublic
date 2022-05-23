using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurniturePlaguedPlate
{
    public class PlaguedPlateBed : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Plagued 'Bed'");
            Tooltip.SetDefault("Counts as a bed");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurniturePlaguedPlate.PlaguedPlateBed>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<PlaguedPlate>(), 15).AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 5).AddTile(ModContent.TileType<PlagueInfuser>()).Register();
        }
    }
}
