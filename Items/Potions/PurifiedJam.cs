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
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (CalamityWorld.death || BossRushEvent.BossRushActive)
            {
                foreach (TooltipLine line2 in list)
                {
                    if (line2.Mod == "Terraria" && line2.Name == "Tooltip0")
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

        public override bool? UseItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<Invincible>(), (CalamityWorld.death || BossRushEvent.BossRushActive) ? 300 : 600);
            player.AddBuff(BuffID.PotionSickness, player.pStone ? 1500 : 1800);
            return true;
        }
    }
}
