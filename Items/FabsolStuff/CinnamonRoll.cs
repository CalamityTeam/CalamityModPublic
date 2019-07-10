using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class CinnamonRoll : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cinnamon Roll");
			Tooltip.SetDefault(@"Boosts mana regeneration rate and all fire-based weapon damage by 15%
Cursed flame, shadowflame, god slayer inferno, brimstone flame, and frostburn weapons will not receive this benefit
The weapon must be more fire-related than anything else
Reduces defense by 12
A great-tasting cinnamon whiskey with a touch of cream soda");
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
            item.buffType = mod.BuffType("CinnamonRoll");
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 16, 60, 0);
		}
    }
}
