using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class SacrilegiousSink : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<SacrilegiousSinkTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
				AddIngredient<OccultBrickItem>(6).
				AddIngredient(ItemID.WaterBucket).
				AddIngredient(ItemID.HoneyBucket).
				AddIngredient(ItemID.LavaBucket).
				AddTile<SCalAltar>().
				Register();
        }
    }
}
