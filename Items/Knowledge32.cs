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
	public class Knowledge32 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Plaguebringer Goliath");
			Tooltip.SetDefault("A beautiful amalgam of steel, flesh, and infection.  Capable of destroying an entire civilization in just one onslaught.\n" +
                "Its plague nuke barrage can leave an entire area uninhabitable for months...at least concerning certain species.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 8;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}