using CalamityMod.CalPlayer;
using CalamityMod.Buffs.DamageOverTime;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class ElysianAegis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elysian Aegis");
            Tooltip.SetDefault("Blessed by the Profaned Flame\n" +
							   "Grants immunity to fire blocks, knockback, and Holy Flames\n" +
                               "+40 max life and increased life regen\n" +
                               "Grants a supreme holy flame dash\n" +
                               "Can be used to ram enemies\n" +
                               "TOOLTIP LINE HERE\n" +
                               "Activating this buff will reduce your movement speed and increase enemy aggro\n" +
                               "Toggle visibility of this accessory to enable/disable the dash");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 42;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.defense = 8;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.AegisHotKey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip5")
                {
                    line2.text = "Press " + hotkey + " to activate buffs to all damage, crit chance, and defense";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (!hideVisual)
            { modPlayer.dashMod = 3; }
            modPlayer.elysianAegis = true;
            player.buffImmune[ModContent.BuffType<HolyFlames>()] = true;
            player.noKnockback = true;
            player.fireWalk = true;
            player.lifeRegen += 2;
            player.statLifeMax2 += 40;
        }
    }
}
