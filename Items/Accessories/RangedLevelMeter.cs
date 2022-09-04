using CalamityMod.CalPlayer;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Linq;

namespace CalamityMod.Items.Accessories
{
    public class RangedLevelMeter : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Ranged Level Meter");
            Tooltip.SetDefault("Tells you how high your ranged proficiency is");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            int level = Main.player[Main.myPlayer].Calamity().rangedLevel;
            int exactLevel = Main.player[Main.myPlayer].Calamity().exactRangedLevel;
            int damageGain = 0;
            int moveSpeed = 0;
            int critGain = 0;
            switch (exactLevel)
            {
                case 0:
                    break;
                case 1:
                    damageGain = 1;
                    break;
                case 2:
                    damageGain = 2;
                    break;
                case 3:
                    damageGain = 2;
                    critGain = 1;
                    break;
                case 4:
                    damageGain = 2;
                    moveSpeed = 1;
                    critGain = 1;
                    break;
                case 5:
                    damageGain = 2;
                    moveSpeed = 2;
                    critGain = 1;
                    break;
                case 6:
                    damageGain = 2;
                    moveSpeed = 3;
                    critGain = 1;
                    break;
                case 7:
                    damageGain = 2;
                    moveSpeed = 3;
                    critGain = 2;
                    break;
                case 8:
                    damageGain = 3;
                    moveSpeed = 3;
                    critGain = 2;
                    break;
                case 9:
                    damageGain = 3;
                    moveSpeed = 4;
                    critGain = 2;
                    break;
                case 10:
                    damageGain = 4;
                    moveSpeed = 4;
                    critGain = 2;
                    break;
                case 11:
                    damageGain = 4;
                    moveSpeed = 4;
                    critGain = 3;
                    break;
                case 12:
                    damageGain = 4;
                    moveSpeed = 5;
                    critGain = 3;
                    break;
                case 13:
                    damageGain = 5;
                    moveSpeed = 5;
                    critGain = 3;
                    break;
                case 14:
                    damageGain = 5;
                    moveSpeed = 6;
                    critGain = 3;
                    break;
                case 15:
                    damageGain = 6;
                    moveSpeed = 6;
                    critGain = 3;
                    break;
            }

            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");

            if (line != null)
                line.Text = "Tells you how high your ranged proficiency is\n" +
                "While equipped you will gain ranged proficiency faster\n" +
                "The higher your ranged level the higher your ranged damage, critical strike chance, and movement speed\n" +
                "Ranged proficiency (max of 12500): " + (level - (level > 12500 ? 1 : 0)) + "\n" +
                "Ranged level (max of 15): " + exactLevel + "\n" +
                "Ranged damage increase: " + damageGain + "%\n" +
                "Movement speed increase: " + moveSpeed + "%\n" +
                "Ranged crit increase: " + critGain + "%";
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fasterRangedLevel = true;
        }
    }
}
