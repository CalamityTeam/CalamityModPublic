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
            Item.width = 28;
            Item.height = 26;
            Item.defense = 24;
            Item.rare = ItemRarityID.Purple;

            // Exact worth of the armor piece's constituents.
            Item.value = Item.sellPrice(platinum: 7, gold: 35, silver: 84);
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ExoPrism>(), 12).AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 4).AddIngredient(ModContent.ItemType<CoreofCalamity>(), 3).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }
    }
}
