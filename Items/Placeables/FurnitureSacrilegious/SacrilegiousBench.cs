using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class SacrilegiousBench : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sacrilegious Bench");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<SacrilegiousBenchTile>();
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
				AddIngredient(ModContent.ItemType<OccultBrickItem>(), 5).
				AddIngredient(ItemID.Silk, 2).
				AddTile(ModContent.TileType<CosmicAnvil>()).
				Register();
        }
    }
}
