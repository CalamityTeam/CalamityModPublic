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
	public class Knowledge10 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Ocean");
			Tooltip.SetDefault("Take care to not disturb the deep waters of this world.\n" +
                "You may awaken something more terrifying than death itself.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 7;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}