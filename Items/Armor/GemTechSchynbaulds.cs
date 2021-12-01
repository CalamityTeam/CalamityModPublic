using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class GemTechSchynbaulds : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gem Tech Schynbaulds");
            Tooltip.SetDefault("If they hurt you, kick them down.");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 26;
            item.defense = 24;
            item.rare = ItemRarityID.Purple;

            // Exact worth of the armor piece's constituents.
            item.value = Item.sellPrice(platinum: 7, gold: 35, silver: 84);
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ExoPrism>(), 12);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 4);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 3);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
