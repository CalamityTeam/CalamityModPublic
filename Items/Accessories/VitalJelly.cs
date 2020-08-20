using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class VitalJelly : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vital Jelly");
            Tooltip.SetDefault("20% increased movement speed\n" +
                "24% increased jump speed");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = 3;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            bool autoJump = Main.player[Main.myPlayer].autoJump;
			string jumpAmt = autoJump ? "6" : "24";
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                {
                    line2.text = jumpAmt + "% increased jump speed";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.2f;
            player.jumpSpeedBoost += player.autoJump ? 0.3f : 1.2f;
        }
    }
}
