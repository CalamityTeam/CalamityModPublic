using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.FabsolStuff
{
	public class StarBeamRye : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Star Beam Rye");
			Tooltip.SetDefault(@"Boosts max mana by 50, magic damage by 8%,
and reduces mana usage by 10%
Reduces defense by 6 and life regen by 3
Made from some stuff I found near the Astral Meteor crash site, don't worry it's safe, trust me");
		}

		public override void SetDefaults()
		{
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 3;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = mod.BuffType("StarBeamRye");
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 13, 30, 0);
		}
    }
}
