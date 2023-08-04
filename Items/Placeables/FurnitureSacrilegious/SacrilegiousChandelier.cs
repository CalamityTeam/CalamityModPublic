using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class SacrilegiousChandelier : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<SacrilegiousChandelierTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
				AddIngredient(ModContent.ItemType<OccultBrickItem>(), 4).
				AddIngredient(ModContent.ItemType<AshesofCalamity>(), 4).
				AddIngredient(ItemID.Chain).
				AddTile(ModContent.TileType<SCalAltar>()).
				Register();
        }
    }
}
