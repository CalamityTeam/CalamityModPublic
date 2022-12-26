using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.GemTech
{
    [AutoloadEquip(EquipType.Body)]
    public class GemTechBodyArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Gem Tech Body Armor");
            Tooltip.SetDefault("You sunk low enough for me to reach.");
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 32;
            Item.defense = 31;

            // Exact worth of the armor piece's constituents.
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.Calamity().donorItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => GemTechHeadgear.ModifySetTooltips(this, tooltips);
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ExoPrism>(16)
                .AddIngredient<GalacticaSingularity>(5)
                .AddIngredient<CoreofCalamity>(2)
                .AddTile<DraedonsForge>()
                .Register();
        }
    }
}
