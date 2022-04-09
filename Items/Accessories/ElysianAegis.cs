using CalamityMod.CalPlayer;
using CalamityMod.Buffs.DamageOverTime;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

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
            Item.width = 48;
            Item.height = 42;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.defense = 18;
            Item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityKeybinds.AegisHotKey.TooltipHotkeyString();
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip5");

            if (line != null)
                line.Text = "Press " + hotkey + " to activate buffs to all damage, crit chance and defense";
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
