using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurniturePlagued
{
    public class PlaguedPlateClock : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Plagued Clock");
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 10;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurniturePlaguedPlate.PlaguedPlateClock>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ModContent.ItemType<PlaguedContainmentBrick>(), 10).
                AddIngredient(ModContent.ItemType<PlagueCellCanister>(), 2).
                AddIngredient(ItemID.Glass, 6).
                AddRecipeGroup("IronBar", 3).
                AddTile(ModContent.TileType<PlagueInfuser>()).
                Register();
        }
    }
}
