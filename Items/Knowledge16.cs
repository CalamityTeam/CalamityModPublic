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
	public class Knowledge16 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Queen Bee");
			Tooltip.SetDefault("As crude as the giant insects are they can prove useful in certain situations.\n" +
                "...given the ability to control them.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 3;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}