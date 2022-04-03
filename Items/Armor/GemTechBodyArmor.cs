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
            Item.width = 48;
            Item.height = 32;
            Item.defense = 31;
            Item.rare = ItemRarityID.Purple;

            // Exact worth of the armor piece's constituents.
            Item.value = Item.sellPrice(platinum: 9, gold: 79, silver: 80);
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ExoPrism>(), 16).AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5).AddIngredient(ModContent.ItemType<CoreofCalamity>(), 3).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }
    }
}
