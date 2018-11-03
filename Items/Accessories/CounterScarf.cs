using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    public class CounterScarf : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Counter Scarf");
            Tooltip.SetDefault("Melee weapons that don't fire projectiles are granted 20% more damage\n" +
                "Grants the ability to dash; dashing into an attack will cause you to dodge it\n" +
                "After a dodge you will be granted a buff to melee damage, speed, and crit chance for a short time\n" +
                "After a successful dodge you must wait 15 seconds before you can dodge again\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = 50000;
            item.rare = 5;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.dodgeScarf = true;
            modPlayer.dashMod = 1;
        }
    }
}