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
    public class ArcanumoftheVoid : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arcanum of the Void");
            Tooltip.SetDefault("You have a 10% chance to reflect projectiles when they hit you\n" +
                               "If this effect triggers you get healed for the projectile's damage");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = 1500000;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 0);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.projRef = true;
        }
    }
}