using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class PlaguedFuelPack : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 36;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        // TODO -- Check if the slot contains the other rogue jetpack, in which case, let the player swap accs
        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().hasJetpack;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().hasJetpack = true;
            player.GetDamage<ThrowingDamageClass>() += 0.08f;
            player.Calamity().rogueVelocity += 0.15f;
            player.Calamity().plaguedFuelPack = true;
            player.Calamity().stealthGenStandstill += 0.1f;
            player.Calamity().stealthGenMoving += 0.1f;
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(CalamityKeybinds.PlaguePackHotKey);
    }
}
