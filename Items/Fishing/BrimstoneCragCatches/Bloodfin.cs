using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.Potions;
using System.Collections.Generic;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class Bloodfin : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodfin");
            Tooltip.SetDefault(@"Grants a buff that boosts life regen for 10 seconds
The life regen boost is stronger if below 75% health
Angiogenesis");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.height = 36;
            item.maxStack = 999;
            item.useTurn = true;
            item.value = Item.sellPrice(gold: 5);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 13;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.healLife = 240;
            item.potion = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override void OnConsumeItem(Player player)
        {
            player.statLife += 240;
            if (player.statLife > player.statLifeMax2)
            {
                player.statLife = player.statLifeMax2;
            }
            if (Main.myPlayer == player.whoAmI)
            {
                player.HealEffect(240, true);
            }
            player.AddBuff(ModContent.BuffType<BloodfinBoost>(), 600);

			//So you can't just spam with quick buff
            player.ClearBuff(BuffID.PotionSickness);
            player.AddBuff(BuffID.PotionSickness, player.pStone ? 2700 : 3600);
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
