using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("AstralArcanum")]
    public class Purity : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.defense = 8;
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.purity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<InfectedJewel>().
                AddIngredient<AmbrosialAmpoule>().
                AddIngredient<CoreofCalamity>(1).
                AddIngredient<DivineGeode>(8).
                AddIngredient<SeaPrism>(8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
