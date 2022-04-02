using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class GemTechBodyArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gem Tech Body Armor");
            Tooltip.SetDefault("You sunk low enough for me to reach.");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 32;
            item.defense = 31;
            item.rare = ItemRarityID.Purple;

            // Exact worth of the armor piece's constituents.
            item.value = Item.sellPrice(platinum: 9, gold: 79, silver: 80);
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ExoPrism>(), 16);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 3);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
