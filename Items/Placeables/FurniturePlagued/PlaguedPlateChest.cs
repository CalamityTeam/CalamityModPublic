using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurniturePlagued
{
    public class PlaguedPlateChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Plagued Chest");
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
            Item.createTile = ModContent.TileType<Tiles.FurniturePlaguedPlate.PlaguedPlateChest>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<PlaguedPlate>(), 8).AddIngredient(ModContent.ItemType<PlagueCellCanister>(), 2).AddIngredient(ItemID.IronBar, 2).AddTile(ModContent.TileType<PlagueInfuser>()).Register();
        }
    }
}
