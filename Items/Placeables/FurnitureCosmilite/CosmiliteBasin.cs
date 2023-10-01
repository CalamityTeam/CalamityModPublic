using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Tiles.FurnitureCosmilite;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace CalamityMod.Items.Placeables.FurnitureCosmilite
{
    public class CosmiliteBasin : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 10;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<CosmiliteBasinTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CosmiliteBrick>(10).
                AddRecipeGroup("IronBar", 5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
