using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class RedWine : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Red Wine");
			Tooltip.SetDefault(@"Reduces life regen by 1
Too dry for my taste
Restores 200 life");
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
            item.consumable = true;
            item.buffType = mod.BuffType("RedWine");
            item.buffTime = 900;
            item.value = Item.buyPrice(0, 2, 0, 0);
		}

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override bool ConsumeItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override void OnConsumeItem(Player player)
        {
            player.statLife += 200;
            if (player.statLife > player.statLifeMax2)
            {
                player.statLife = player.statLifeMax2;
            }
            if (Main.myPlayer == player.whoAmI)
            {
                player.HealEffect(200, true);
            }
            player.AddBuff(mod.BuffType("RedWine"), 900);
            player.AddBuff(BuffID.PotionSickness, (player.pStone ? 2700 : 3600));
        }
    }
}
