using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class Abaddon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static float BrimstoneFlamesReduction = 0.5f;
        public static float critScaling = 3f; // How effective crit chance is at increasing debuff damage
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().abaddon = true;
            player.Calamity().abaddonEffectVisual = !hideVisual;
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (Main.LocalPlayer != null)
                list.FindAndReplace("[DAMAGELINE]", Main.LocalPlayer.Calamity().abaddon ? this.GetLocalization("Equipped").Format(((int)(Bane.debuffData.EnemyLostRegen / 2 * Main.LocalPlayer.Calamity().playerBaneDebuffDamage)).ToString())
                : this.GetLocalizedValue("Unequipped"));
        }
    }
}
