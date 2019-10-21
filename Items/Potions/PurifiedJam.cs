using CalamityMod.Buffs.Potions;
using CalamityMod.World;
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
            Tooltip.SetDefault("Makes you immune to all damage and most debuffs for 10 seconds (5 seconds in Death Mode)\n" +
               "Causes potion sickness when consumed\n" +
               "Cannot be consumed while potion sickness is active\n" +
               "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 999;
            item.rare = 3;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override bool UseItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<Invincible>(), CalamityWorld.death ? 300 : 600);
            player.AddBuff(BuffID.PotionSickness, player.pStone ? 1500 : 1800);
            return true;
        }
    }
}
