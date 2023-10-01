using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class BlunderBooster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }

        // TODO -- Check if its trying to replace the other rogue jetpack. If its the case, return true.
        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().hasJetpack;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().hasJetpack = true;
            player.GetDamage<ThrowingDamageClass>() += 0.12f;
            player.Calamity().rogueVelocity += 0.15f;
            player.Calamity().blunderBooster = true;
            player.Calamity().stealthGenStandstill += 0.1f;
            player.Calamity().stealthGenMoving += 0.1f;
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(CalamityKeybinds.BoosterDashHotKey);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PlaguedFuelPack>().
                AddIngredient<EffulgentFeather>(8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
