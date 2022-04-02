using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class SandCloak : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Cloak");
            Tooltip.SetDefault("+1 defense and 5% increased movement speed\n" +
                "TOOLTIP LINE HERE\n" + 
                "This effect has a 30 second cooldown before it can be used again");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 44;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = ItemRarityID.Green;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.SandCloakHotkey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                {
                    line2.text = "Press " + hotkey + " to consume 25% of your maximum stealth to create a protective dust veil which provides +6 defense and +2 life regen";
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 1;
            player.moveSpeed += 0.05f;
            player.Calamity().sandCloak = true;
        }
    }
}
