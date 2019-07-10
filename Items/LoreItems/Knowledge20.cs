using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items.LoreItems
{
	public class Knowledge20 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Mechanical Bosses");
			Tooltip.SetDefault("I see you have awakened Draedon's old toys.\n" +
                "Once useful tools turned into savage beasts when their AIs went rogue, a mistake that Draedon failed to rectify in time.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 5;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}
