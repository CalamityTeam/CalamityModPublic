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
	public class Knowledge21 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Destroyer");
			Tooltip.SetDefault("A machine brought to life by the mighty souls of warriors and built to excavate massive tunnels in planets to gather resources.\n" +
                "Could have proven useful if he didn't have an obsession with turning everything into a tool of destruction.");
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