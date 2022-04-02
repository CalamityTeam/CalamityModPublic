using CalamityMod.CalPlayer;
using CalamityMod.Buffs.DamageOverTime;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class ElysianAegis : ModItem
    {
        public const int ShieldSlamIFrames = 6;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elysian Aegis");
            Tooltip.SetDefault("Blessed by the Profaned Flame\n" +
                               "Grants immunity to knockback, and the Burning, On Fire!, and Holy Flames debuffs\n" +
                               "+30 max life\n" +
                               "Grants a supreme holy flame dash\n" +
                               "Can be used to ram enemies\n" +
                               "TOOLTIP LINE HERE\n" +
                               "Activating this buff will reduce your movement speed and increase enemy aggro");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 42;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.defense = 18;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.AegisHotKey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip5")
                {
                    line2.text = "Press " + hotkey + " to activate buffs to all damage, crit chance and defense";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dashMod = 3;
            player.dash = 0;
            modPlayer.elysianAegis = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[ModContent.BuffType<HolyFlames>()] = true;
            player.noKnockback = true;
            player.fireWalk = true;
            player.statLifeMax2 += 30;
        }
    }
}
