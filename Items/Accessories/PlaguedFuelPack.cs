using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	public class PlaguedFuelPack : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plagued Fuel Pack");
            Tooltip.SetDefault("5% increased rogue damage\n" +
                "15% increased rogue projectile velocity\n" +
                "TOOLTIP LINE HERE" + 
                "This effect has a 3 second cooldown before it can be used again");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 36;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = 8;
            item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().hasJetpack;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.Calamity().hasJetpack = true;
            player.Calamity().throwingDamage += 0.05f;
            player.Calamity().throwingVelocity += 0.15f;
            player.Calamity().plaguedFuelPack = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.PlaguePackHotKey.TooltipHotkeyString();
            foreach (TooltipLine line in list)
            {
                if (line.mod == "Terraria" && line.Name == "Tooltip2")
                {
                    line.text = "Press " + hotkey + " to consume 25% of your maximum stealth to perform a swift upwards/diagonal dash which leaves a trail of plagued clouds";
                }
            }
        }
    }
}
