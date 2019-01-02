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
    public class MagicLevelMeter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Level Meter");
            Tooltip.SetDefault("Tells you how high your magic proficiency is");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 6, 0, 0);
            item.rare = 1;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            int level = Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>(mod).magicLevel;
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                {
                    line2.text = "Tells you how high your magic proficiency is\n" +
                "While equipped you will gain magic proficiency faster\n" +
                "The higher your magic level the higher your magic damage, critical strike chance, and the lower your mana cost\n" +
                "Magic proficiency (max of 12500): " + (level - (level > 12500 ? 1 : 0)) + "\n" +
                "Magic level (max of 15): " + Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>(mod).exactMagicLevel;
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.fasterMagicLevel = true;
        }
    }
}