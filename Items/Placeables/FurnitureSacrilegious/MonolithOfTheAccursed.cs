using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class MonolithOfTheAccursed : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monolith of the Accursed");
            Tooltip.SetDefault("Let the sky burn a blazing red");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<MonolithOfTheAccursedTile>();
            Item.rare = ModContent.RarityType<Violet>();
			Item.placeStyle = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
				AddIngredient(ModContent.ItemType<OccultBrickItem>(), 15).
				AddTile(ModContent.TileType<SCalAltar>()).
				Register();
        }
    }
}
