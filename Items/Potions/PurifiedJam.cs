using CalamityMod.Buffs.Potions;
using CalamityMod.Events;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class PurifiedJam : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purified Jam");
            Tooltip.SetDefault("Makes you immune to all damage and most debuffs for 10 seconds\n" +
               "Causes potion sickness when consumed\n" +
               "Cannot be consumed while potion sickness is active");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.Orange;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
			if (CalamityWorld.death || BossRushEvent.BossRushActive)
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
					{
						line2.text = "Makes you immune to all damage and most debuffs for 5 seconds";
					}
				}
			}
        }

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override bool UseItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<Invincible>(), (CalamityWorld.death || BossRushEvent.BossRushActive) ? 300 : 600);
            player.AddBuff(BuffID.PotionSickness, player.pStone ? 1500 : 1800);
            return true;
        }
    }
}
