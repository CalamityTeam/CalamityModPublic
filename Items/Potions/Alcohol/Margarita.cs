using CalamityMod.Buffs.Alcohol;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class Margarita : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Margarita");
            Tooltip.SetDefault(@"Restores 200 mana
One of the best drinks ever created, enjoy it while it lasts
Reduces the duration of most debuffs
Reduces defense by 6 and life regen by 1");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 5;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.potion = true;
            item.healLife = 200;
            item.buffType = ModContent.BuffType<MargaritaBuff>();
            item.buffTime = 10800;
            item.value = Item.buyPrice(0, 23, 30, 0);
        }

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override void OnConsumeItem(Player player)
        {
			int healAmt = CalamityWorld.ironHeart ? 0 : 200;
			if (player.Calamity().bloodPactBuffTimer > 0)
				healAmt = (int)(healValue * 1.5);
            player.statLife += healAmt;
            player.statMana += 200;
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
				if (!CalamityWorld.ironHeart)
					player.HealEffect(healAmt, true);
                player.ManaEffect(200);
            }
            player.AddBuff(ModContent.BuffType<MargaritaBuff>(), 10800);
        }

        // Zeroes out the hardcoded healing function from having a healLife value. The item still heals in the UseItem hook.
        public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
        {
            healValue = 0;
        }

        // Forces the "Restores X life" tooltip to display the actual life restored instead of zero (due to the previous function).
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
			float healMult = 1f;
			if (Main.player[Main.myPlayer].Calamity().bloodPactBuffTimer > 0)
				healMult = 1.5f;
            tooltips.Find(line => line.Name == "HealLife").text = "Restores " + (CalamityWorld.ironHeart ? 0 : (int)(item.healLife * healMult)) + " life";
        }
    }
}
