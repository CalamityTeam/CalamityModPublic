using CalamityMod.Tiles.FurnitureAcidwood;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.Items.Placeables.FurnitureAcidwood
{
    public class AcidwoodChest : ModItem, ILocalizedModType
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
            Item.createTile = ModContent.TileType<AcidwoodChestTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Acidwood>(8).
                AddRecipeGroup("IronBar", 2).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
