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
	public class Knowledge29 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astrum Deus");
			Tooltip.SetDefault("God of the stars and spreader of the astral infection.\n" +
                "Though it was not the progenitor of the infection, that scaleless beast is still out there, devouring celestial bodies by the dozens.");
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