using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Rarities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class ElysianAegis : ModItem
    {
        public const int ShieldSlamIFrames = 12;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Elysian Aegis");
            Tooltip.SetDefault("Blessed by the Profaned Flame\n" +
                               "Grants immunity to knockback and the Burning, On Fire!, and Holy Flames debuffs\n" +
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
            Item.rare = ModContent.RarityType<Turquoise>();
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
            modPlayer.DashID = ElysianAegisDash.ID;
            player.dashType = 0;
            modPlayer.elysianAegis = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[ModContent.BuffType<HolyFlames>()] = true;
            player.noKnockback = true;
            player.fireWalk = true;
            player.statLifeMax2 += 30;
        }
    }
}
