using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class Vodka : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vodka");
			Tooltip.SetDefault(@"Boosts damage by 6% and critical strike chance by 2%
Reduces life regen by 1 and defense by 4
The number one alcohol for creating great mixed drinks");
		}

		public override void SetDefaults()
		{
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 2;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = mod.BuffType("Vodka");
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 3, 30, 0);
		}
    }
}
