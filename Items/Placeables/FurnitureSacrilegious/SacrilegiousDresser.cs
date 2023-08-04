using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class SacrilegiousDresser : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<SacrilegiousDresserTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
				AddIngredient(ModContent.ItemType<OccultBrickItem>(), 16).
				AddTile(ModContent.TileType<SCalAltar>()).
				Register();
        }
    }
}
