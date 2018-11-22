using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class GrapeBeer : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Grape Beer");
			Tooltip.SetDefault(@"Reduces life regen by 1, defense by 2, and movement speed by 5%
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
            item.consumable = true;
            item.healLife = 100;
            item.healMana = 100;
            item.buffType = mod.BuffType("GrapeBeer");
            item.buffTime = 3600; //3600 = 1 minute
            item.value = Item.buyPrice(0, 0, 65, 0);
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
