using CalamityMod.CalPlayer;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class FrostFlare : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Flare");
            Tooltip.SetDefault("All melee attacks and projectiles inflict frostburn\n" +
                "Immunity to frostburn, chilled, and frozen\n" +
                "Resistant to cold attacks and +1 life regen\n" +
                "Being above 75% life grants the player 10% increased damage\n" +
                "Being below 25% life grants the player 10 defense and 15% increased max movement speed and acceleration\n" +
				"Revengeance drop");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 4));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 24;
            item.lifeRegen = 1;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = 5;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
			if (CalamityWorld.death)
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip5")
					{
						line2.text = "Provides heat and cold protection in Death Mode\n" +
						"Revengeance drop";
					}
				}
			}
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.frostFlare = true;
        }
    }
}
