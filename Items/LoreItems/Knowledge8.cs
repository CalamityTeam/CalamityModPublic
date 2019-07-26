using System;
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
	public class Knowledge8 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Crimson");
			Tooltip.SetDefault("This bloody hell, spawned from a formless mass of flesh that fell from the stars eons ago.\n" +
                "It is now home to many hideous creatures, spawned from the pumping blood and lurching organs deep within.");
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
