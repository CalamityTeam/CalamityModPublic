using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
	public class FrostBarrier : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frost Barrier");
			Tooltip.SetDefault("You will freeze enemies near you when you are struck\n" +
			                   "You are immune to the chilled debuff");
		}
		
		public override void SetDefaults()
		{
			item.defense = 4;
			item.width = 20;
			item.height = 24;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 3;
			item.accessory = true;
		}
		
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.fBarrier = true;
			player.buffImmune[46] = true;
		}
	}
}
