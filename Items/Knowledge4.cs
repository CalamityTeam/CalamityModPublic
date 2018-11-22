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
	public class Knowledge4 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lunatic Cultist");
			Tooltip.SetDefault("The gifted one that terminated my grand summoning so long ago with his uncanny powers over the arcane.\n" +
                "Someone I once held in such contempt for his actions is now...deceased, his sealing ritual undone...prepare for the end.\n" +
                "Your impending doom approaches...");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 9;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}