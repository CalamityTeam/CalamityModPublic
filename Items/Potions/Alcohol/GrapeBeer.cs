using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class GrapeBeer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grape Beer");
            Tooltip.SetDefault(@"Restores 100 mana
Reduces defense by 2 and movement speed by 5%
This crap is abhorrent but you might like it");
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
            item.healLife = 100;
            item.consumable = true;
            item.potion = true;
            item.value = Item.buyPrice(0, 0, 65, 0);
        }

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override void OnConsumeItem(Player player)
        {
            player.statLife += 100;
            player.statMana += 100;
            if (player.statLife > player.statLifeMax2)
            {
                player.statLife = player.statLifeMax2;
            }
            if (player.statMana > player.statManaMax2)
            {
                player.statMana = player.statManaMax2;
            }
            player.AddBuff(BuffID.ManaSickness, Player.manaSickTime, true);
            if (Main.myPlayer == player.whoAmI)
            {
                player.HealEffect(100, true);
                player.ManaEffect(100);
            }
            player.AddBuff(ModContent.BuffType<Buffs.Alcohol.GrapeBeer>(), 3600);
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
