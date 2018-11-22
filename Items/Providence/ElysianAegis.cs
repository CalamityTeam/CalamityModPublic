using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Providence
{
    [AutoloadEquip(EquipType.Shield)]
    public class ElysianAegis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elysian Aegis");
            Tooltip.SetDefault("Grants immunity to fire blocks and knockback\n" +
                               "+40 max life and increased life regen\n" +
                               "Grants a supreme holy flame dash\n" +
                               "Can be used to ram enemies\n" +
                               "Press N to activate buffs to all damage, crit chance, and defense\n" +
                               "Activating this buff will reduce your movement speed and increase enemy aggro\n" +
                               "Toggle visibility of this accessory to enable/disable the dash");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 42;
            item.value = 10000000;
            item.expert = true;
            item.rare = 9;
            item.defense = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if (!hideVisual) { modPlayer.dashMod = 3; }
            modPlayer.elysianAegis = true;
            player.noKnockback = true;
            player.fireWalk = true;
            player.lifeRegen += 3;
            player.statLifeMax2 += 40;
        }
    }
}