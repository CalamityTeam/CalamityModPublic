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
	public class Knowledge23 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skeletron Prime");
			Tooltip.SetDefault("What a silly and pointless contraption for something created with the essence of pure terror.\n" +
                "Draedon obviously took several liberties with his design of this thing...I am not impressed.  Better off destroyed.");
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