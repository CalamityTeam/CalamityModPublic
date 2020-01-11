using CalamityMod.Buffs.Alcohol;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class RedWine : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Wine");
            Tooltip.SetDefault(@"Reduces life regen by 1
Too dry for my taste");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 1;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.healLife = 200;
            item.consumable = true;
            item.potion = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override void OnConsumeItem(Player player)
        {
            player.statLife += (player.Calamity().baguette ? 250 : 200);
            if (player.statLife > player.statLifeMax2)
            {
                player.statLife = player.statLifeMax2;
            }
            if (Main.myPlayer == player.whoAmI)
            {
                player.HealEffect((player.Calamity().baguette ? 250 : 200), true);
            }
            player.AddBuff(ModContent.BuffType<RedWineBuff>(), 900);
        }

        // Zeroes out the hardcoded healing function from having a healLife value. The item still heals in the UseItem hook.
        public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
        {
            healValue = 0;
        }

        // Forces the "Restores X life" tooltip to display the actual life restored instead of zero (due to the previous function).
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Find(line => line.Name == "HealLife").text = "Restores " + item.healLife + " life";
        }
    }
}