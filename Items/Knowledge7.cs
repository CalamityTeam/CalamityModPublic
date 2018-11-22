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
	public class Knowledge7 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Underworld");
			Tooltip.SetDefault("These obsidian and hellstone towers were once home to thousands of...'people'.\n" +
                "Unfortunately for them they were twisted by their inner demons until they were beyond saving.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 4;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}