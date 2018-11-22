using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class Whiskey : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Whiskey");
			Tooltip.SetDefault(@"Boosts damage, critical strike chance, and knockback by 3%
Reduces defense by 8
The burning sensation makes it tastier");
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
            item.buffType = mod.BuffType("Whiskey");
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 3, 30, 0);
		}
    }
}
