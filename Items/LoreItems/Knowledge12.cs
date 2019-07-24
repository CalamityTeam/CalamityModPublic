﻿using System;
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
	public class Knowledge12 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Eater of Worlds");
			Tooltip.SetDefault("Perhaps it was just a giant worm infected by the microbe, given centuries to feed and grow its festering body.\n" +
                "Seems likely, given the origins of this place.\n" +
				"Deadly microbes spawn around you while this is placed in your inventory.");
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

		public override void UpdateInventory(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.eaterOfWorldsLore = true;
		}
	}
}