using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureStratus
{
    public class StratusChest : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.FurnitureStratus.StratusChest>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StratusBricks>(8).
                AddRecipeGroup("IronBar", 2).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
