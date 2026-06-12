using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ChaosStone : ModItem, ILocalizedModType
    {
        public static float LostRegenPer100Mana => 8;
        public static float DamageMultPer100Mana => 0.05f;
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            var regenAmount = Main.LocalPlayer.statManaMax2 == 0 ? 0 : (int)((Main.LocalPlayer.statManaMax2 / 100f) * LostRegenPer100Mana) * 0.5f;
            var dmgAmount = Main.LocalPlayer.statManaMax2 == 0 ? 0 : ((Main.LocalPlayer.statManaMax2 / 100f) * DamageMultPer100Mana * 100);
            tooltips.FindAndReplaceAll("[REGEN]", regenAmount.ToString("0.#"));
            tooltips.FindAndReplaceAll("[DAMAGE]", dmgAmount.ToString("0.#"));
        }
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 7));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) => player.Calamity().ChaosStone = true;
    }
}
