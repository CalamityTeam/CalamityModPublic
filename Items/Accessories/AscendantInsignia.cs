using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("YharimsInsignia")]
    public class AscendantInsignia : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(CalamityKeybinds.AscendantInsigniaHotKey);
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 52;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.ascendantInsignia = true;
            player.empressBrooch = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EmpressFlightBooster).
                AddIngredient<CoreofSunlight>(5).
                AddIngredient<DivineGeode>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
