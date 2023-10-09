using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using CalamityMod.Items.Placeables.Ores;

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
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<PureGreen>();
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
                AddIngredient<Lumenyl>(5).
                AddIngredient<ExodiumCluster>(12).
                AddIngredient<RuinousSoul>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
