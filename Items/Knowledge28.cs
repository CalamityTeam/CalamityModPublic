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
	public class Knowledge28 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Leviathan and Siren");
			Tooltip.SetDefault("An odd pair of creatures; one seeking companionship and the other seeking sustenance.\n" +
                "Perhaps two genetic misfits outcast from their homes that found comfort in assisting one another.");
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