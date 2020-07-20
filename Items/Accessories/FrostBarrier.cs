using CalamityMod.CalPlayer;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class FrostBarrier : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Barrier");
            Tooltip.SetDefault("You will freeze enemies near you when you are struck\n" +
                               "You are immune to the chilled debuff");
        }

        public override void SetDefaults()
        {
            item.defense = 4;
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = 4;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
			if (CalamityWorld.death)
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
					{
						line2.text = "You are immune to the chilled debuff\n" +
						"Provides heat and cold protection in Death Mode";
					}
				}
			}
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fBarrier = true;
            player.buffImmune[BuffID.Chilled] = true;
        }
    }
}
