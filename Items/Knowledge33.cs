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
	public class Knowledge33 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ravager");
			Tooltip.SetDefault("One of many flesh golems crafted through forbidden necromancy to counter my unstoppable forces.\n" +
                "Its creators were slaughtered by it moments after its conception. It's for the best that is has been destroyed.");
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