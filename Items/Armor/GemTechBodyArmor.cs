using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ExoPrism>(16)
                .AddIngredient<GalacticaSingularity>(5)
                .AddIngredient<CoreofCalamity>(3)
                .AddTile<DraedonsForge>()
                .Register();
        }
    }
}
