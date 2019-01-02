using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class TequilaSunrise : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tequila Sunrise");
			Tooltip.SetDefault(@"Boosts damage, damage reduction, and knockback by 9%, crit chance by 3%, and defense by 15 during daytime
Reduces life regen by 3
The greatest daytime drink I've ever had");
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
            item.buffType = mod.BuffType("TequilaSunrise");
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 20, 0, 0);
		}
    }
}
