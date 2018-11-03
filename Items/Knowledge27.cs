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
	public class Knowledge27 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aquatic Scourge");
			Tooltip.SetDefault("A horror born of pollution and insatiable hunger; based on size alone this was merely a juvenile.\n" +
                "These Scourge creatures are the largest aquatic predators and very rarely do they frequent such shallow waters.");
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