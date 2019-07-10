using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class WhiteWine : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("White Wine");
			Tooltip.SetDefault(@"Boosts magic damage by 10%
Reduces defense by 6 and life regen by 1
I drank a full barrel of this stuff once in one night, I couldn't remember who I was the next day");
		}

		public override void SetDefaults()
		{
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 4;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.healMana = 400;
            item.buffType = mod.BuffType("WhiteWine");
            item.buffTime = 10800; // 3 minutes
            item.value = Item.buyPrice(0, 16, 60, 0);
		}

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(mod.BuffType("WhiteWine"), 10800);
        }
    }
}
