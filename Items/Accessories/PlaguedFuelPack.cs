using CalamityMod.CalPlayer;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
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
                "Press Q to consume 25% of your maximum stealth to perform a swift upwards/diagonal dash which leaves a trail of plagued clouds\n" + 
                "This effect has a 3 second cooldown before it can be used again");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 36;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().throwingDamage += 0.05f;
            player.Calamity().throwingVelocity += 0.15f;
            player.Calamity().plaguedFuelPack = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (CalamityMod.PlaguePackHotKey.GetAssignedKeys().Count > 0)
            {
                foreach (TooltipLine line in tooltips)
                {
                    if (line.mod == "Terraria" && line.Name == "Tooltip2")
                    {
                        line.text = "Press " + CalamityMod.PlaguePackHotKey.GetAssignedKeys()[0] + " to consume 25% of your maximum stealth to perform a swift upwards/diagonal dash which leaves a trail of plagued clouds";
                    }
                }
            }
        }
    }
}
