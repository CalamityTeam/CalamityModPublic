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
	public class Knowledge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Desert Scourge");
			Tooltip.SetDefault("The great sea worm appears to have survived the extreme heat and has even adapted to it.\n" +
                "What used to be a majestic beast swimming through the water has now become a dried-up and\n" +
                "gluttonous husk on a constant and voracious search for its next meal.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 1;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}