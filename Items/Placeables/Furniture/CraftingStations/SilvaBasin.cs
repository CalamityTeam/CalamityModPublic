using CalamityMod.Items.Placeables.FurnitureSilva;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture.CraftingStations
{
    public class SilvaBasin : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used for special crafting");
        }

        public override void SetDefaults()
        {
            item.SetNameOverride("Effulgent Manipulator");
            item.width = 28;
            item.height = 20;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.Furniture.CraftingStations.SilvaBasin>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilvaCrystal>(), 10);
            recipe.AddRecipeGroup("AnyGoldBar", 5);
            recipe.SetResult(this);
            recipe.AddTile(ModContent.TileType<Tiles.Furniture.CraftingStations.DraedonsForge>());
            recipe.AddRecipe();
        }
    }
}
