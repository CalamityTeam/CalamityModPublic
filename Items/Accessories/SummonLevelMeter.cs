using CalamityMod.CalPlayer;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Linq;

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
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 6, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            int level = Main.player[Main.myPlayer].Calamity().summonLevel;
            int exactLevel = Main.player[Main.myPlayer].Calamity().exactSummonLevel;
            int damageGain = 0;
            int minionKB = 0;
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
                    minionKB = 30;
                    break;
                case 4:
                    damageGain = 3;
                    minionKB = 30;
                    break;
                case 5:
                    damageGain = 3;
                    minionKB = 60;
                    break;
                case 6:
                    damageGain = 4;
                    minionKB = 90;
                    break;
                case 7:
                    damageGain = 4;
                    minionKB = 120;
                    break;
                case 8:
                    damageGain = 5;
                    minionKB = 150;
                    break;
                case 9:
                    damageGain = 6;
                    minionKB = 180;
                    break;
                case 10:
                    damageGain = 7;
                    minionKB = 180;
                    break;
                case 11:
                    damageGain = 7;
                    minionKB = 210;
                    break;
                case 12:
                    damageGain = 8;
                    minionKB = 240;
                    break;
                case 13:
                    damageGain = 9;
                    minionKB = 270;
                    break;
                case 14:
                    damageGain = 10;
                    minionKB = 300;
                    break;
                case 15:
                    damageGain = 12;
                    minionKB = 300;
                    break;
            }
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");

            if (line != null)
                line.Text = "Tells you how high your summon proficiency is\n" +
                "While equipped you will gain summon proficiency faster\n" +
                "The higher your summon level the higher your minion damage and knockback\n" +
                "Summon proficiency (max of 12500): " + (level - (level > 12500 ? 1 : 0)) + "\n" +
                "Summon level (max of 15): " + exactLevel + "\n" +
                "Minion damage increase: " + damageGain + "%\n" +
                "Minion knockback increase: " + minionKB;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fasterSummonLevel = true;
        }
    }
}
