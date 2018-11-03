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
	public class Knowledge11 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Brain of Cthulhu");
			Tooltip.SetDefault("An eye and now a brain.\n" +
                "Most likely another abomination spawned from this inchoate mass of flesh.");
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