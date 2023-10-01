using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureOtherworldly
{
    [LegacyName("OccultBed")]
    public class OtherworldlyBed : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureOtherworldly.OtherworldlyBed>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<OtherworldlyStone>(15).
                AddIngredient(ItemID.Silk, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
