using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class PlaguedFuelPack : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plagued Fuel Pack");
            Tooltip.SetDefault("5% increased rogue damage and 15% increased rogue projectile velocity\n" +
                "Stealth generates 10% faster\n" +
                "TOOLTIP LINE HERE" +
                "This effect has a 1 second cooldown before it can be used again");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 36;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = ItemRarityID.Yellow;
            item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().hasJetpack;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().hasJetpack = true;
            player.Calamity().throwingDamage += 0.05f;
            player.Calamity().throwingVelocity += 0.15f;
            player.Calamity().plaguedFuelPack = true;
            player.Calamity().stealthGenStandstill += 0.1f;
            player.Calamity().stealthGenMoving += 0.1f;
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
