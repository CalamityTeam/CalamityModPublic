using CalamityMod.CalPlayer;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Linq;

namespace CalamityMod.Items.Accessories
{
    public class MagicLevelMeter : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Magic Level Meter");
            Tooltip.SetDefault("Tells you how high your magic proficiency is");
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
            int level = Main.player[Main.myPlayer].Calamity().magicLevel;
            int exactLevel = Main.player[Main.myPlayer].Calamity().exactMagicLevel;
            int damageGain = 0;
            int manaUsage = 0;
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
                    manaUsage = 1;
                    break;
                case 4:
                    damageGain = 2;
                    manaUsage = 1;
                    critGain = 1;
                    break;
                case 5:
                    damageGain = 2;
                    manaUsage = 2;
                    critGain = 1;
                    break;
                case 6:
                    damageGain = 3;
                    manaUsage = 2;
                    critGain = 1;
                    break;
                case 7:
                    damageGain = 3;
                    manaUsage = 2;
                    critGain = 2;
                    break;
                case 8:
                    damageGain = 3;
                    manaUsage = 3;
                    critGain = 2;
                    break;
                case 9:
                    damageGain = 4;
                    manaUsage = 3;
                    critGain = 2;
                    break;
                case 10:
                    damageGain = 4;
                    manaUsage = 4;
                    critGain = 2;
                    break;
                case 11:
                    damageGain = 4;
                    manaUsage = 4;
                    critGain = 3;
                    break;
                case 12:
                    damageGain = 4;
                    manaUsage = 5;
                    critGain = 3;
                    break;
                case 13:
                    damageGain = 5;
                    manaUsage = 5;
                    critGain = 3;
                    break;
                case 14:
                    damageGain = 5;
                    manaUsage = 6;
                    critGain = 3;
                    break;
                case 15:
                    damageGain = 6;
                    manaUsage = 6;
                    critGain = 3;
                    break;
            }
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");

            if (line != null)
                line.Text = "Tells you how high your magic proficiency is\n" +
                "While equipped you will gain magic proficiency faster\n" +
                "The higher your magic level the higher your magic damage, critical strike chance, and the lower your mana cost\n" +
                "Magic proficiency (max of 12500): " + (level - (level > 12500 ? 1 : 0)) + "\n" +
                "Magic level (max of 15): " + exactLevel + "\n" +
                "Magic damage increase: " + damageGain + "%\n" +
                "Mana usage decrease: " + manaUsage + "%\n" +
                "Magic crit increase: " + critGain + "%";
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fasterMagicLevel = true;
        }
    }
}
