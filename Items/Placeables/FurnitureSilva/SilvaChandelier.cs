using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureSilva
{
    public class SilvaChandelier : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.FurnitureSilva.SilvaChandelier>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilvaCrystal>(), 4);
            recipe.AddIngredient(ModContent.ItemType<EffulgentFeather>(), 4);
            recipe.AddRecipeGroup("AnyGoldBar");
            recipe.SetResult(this);
            recipe.AddTile(ModContent.TileType<SilvaBasin>());
            recipe.AddRecipe();
        }
    }
}
