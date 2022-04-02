using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Tiles.FurnitureCosmilite;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace CalamityMod.Items.Placeables.FurnitureCosmilite
{
    public class CosmiliteBasin : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmilite Basin");
        }

        public override void SetDefaults()
        {
            item.width = 8;
            item.height = 10;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<CosmiliteBasinTile>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBrick>(), 10);
            recipe.AddIngredient(ItemID.IronBar, 5);
            recipe.anyIronBar = true;
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.AddRecipe();
        }
    }
}
