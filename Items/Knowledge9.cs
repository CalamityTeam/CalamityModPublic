using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items
{
	public class Knowledge9 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Corruption");
			Tooltip.SetDefault("Ah, the rotten and forever-deteriorating landscape of infected life, brought upon by a deadly microbe long ago.\n" +
                "It is rumored that this microbe was created by a race predating the Terrarians.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 2;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}