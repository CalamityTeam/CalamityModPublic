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
	public class Knowledge19 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cryogen");
			Tooltip.SetDefault("You managed to bring down what dozens of sorcerers long ago could not.\n" +
                "I am unsure if it has grown weaker over the decades of imprisonment.");
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