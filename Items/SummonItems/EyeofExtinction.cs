using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class EyeofExtinction : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ceremonial Urn");
            Tooltip.SetDefault("Use at the Altar of the Accursed to summon Supreme Calamitas\n" + "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 54;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CalamitousEssence>(), 5).AddIngredient(ModContent.ItemType<CalamityDust>(), 15).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
