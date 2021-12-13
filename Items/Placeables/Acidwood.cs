using CalamityMod.Items.Placeables.FurnitureAcidwood;
using CalamityMod.Tiles.Abyss;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class Acidwood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acidwood");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 22;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<AcidwoodTile>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AcidwoodPlatform>(), 2);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
