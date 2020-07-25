using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class PermafrostsConcoction : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Permafrost's Concoction");
            Tooltip.SetDefault(@"Increases maximum mana by 50 and reduces mana cost by 15%
Increases life regen as life decreases
Increases life regen when afflicted with Poison, On Fire, or Brimstone Flames
You will survive fatal damage and revive with 30% life on a 3 minute cooldown
You are encased in an ice barrier for 3 seconds when revived");
        }

        public override void SetDefaults()
        {
            item.accessory = true;
            item.width = 36;
            item.height = 34;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = 5;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
			if (CalamityWorld.death)
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip4")
					{
						line2.text = "You are encased in an ice barrier for 3 seconds when revived\n" +
						"Provides heat and cold protection in Death Mode";
					}
				}
			}
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().permafrostsConcoction = true;
        }
    }
}
