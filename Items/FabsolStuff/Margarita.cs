using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class Margarita : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Margarita");
			Tooltip.SetDefault(@"Restores 200 mana
Makes you immune to most debuffs
Reduces defense by 6 and life regen by 1
One of the best drinks ever created, enjoy it while it lasts");
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
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.potion = true;
            item.healLife = 200;
            item.buffType = mod.BuffType("Margarita");
            item.buffTime = 10800;
            item.value = Item.buyPrice(0, 23, 30, 0);
		}

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override void OnConsumeItem(Player player)
        {
            player.statLife += 200;
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
                player.HealEffect(200, true);
                player.ManaEffect(200);
            }
            player.AddBuff(mod.BuffType("Margarita"), 10800);
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
