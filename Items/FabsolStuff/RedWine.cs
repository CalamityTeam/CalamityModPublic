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
			Tooltip.SetDefault(@"Reduces life regen by 2
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
            item.consumable = true;
            item.healLife = 200;
            item.buffType = mod.BuffType("RedWine");
            item.buffTime = 900; //3600 = 1 minute
            item.value = Item.buyPrice(0, 2, 0, 0);
		}

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override bool UseItem(Player player)
        {
            player.AddBuff(BuffID.PotionSickness, (player.pStone ? 2700 : 3600));
            return true;
        }
    }
}
