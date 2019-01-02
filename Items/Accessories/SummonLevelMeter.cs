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
    public class SummonLevelMeter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summon Level Meter");
            Tooltip.SetDefault("Tells you how high your summon proficiency is");
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
            int level = Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>(mod).summonLevel;
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                {
                    line2.text = "Tells you how high your summon proficiency is\n" +
                "While equipped you will gain summon proficiency faster\n" +
                "The higher your summon level the higher your summon damage, knockback, and minion slots\n" +
                "Summon proficiency (max of 12500): " + (level - (level > 12500 ? 1 : 0)) + "\n" +
                "Summon level (max of 15): " + Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>(mod).exactSummonLevel;
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.fasterSummonLevel = true;
        }
    }
}