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
	public class Knowledge26 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Elemental");
			Tooltip.SetDefault("The most powerful of the elementals, bent on exacting revenge upon the bloody inferno that demolished her home.\n" +
                "Finally put to rest, she will suffer no longer from the grief caused by the deaths of her people.");
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