using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.GemTech
{
    [AutoloadEquip(EquipType.Legs)]
    public class GemTechSchynbaulds : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Gem Tech Schynbaulds");
            Tooltip.SetDefault("If they hurt you, kick them down.");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 26;
            Item.defense = 24;

            // Exact worth of the armor piece's constituents.
            Item.value = Item.sellPrice(platinum: 7, gold: 35, silver: 84);
            Item.rare = ModContent.RarityType<Violet>();
            Item.Calamity().donorItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => GemTechHeadgear.ModifySetTooltips(this, tooltips);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ExoPrism>(12).
                AddIngredient<GalacticaSingularity>(4).
                AddIngredient<CoreofCalamity>(2).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
