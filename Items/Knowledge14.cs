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
	public class Knowledge14 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Hive Mind");
			Tooltip.SetDefault("A hive of clustered microbial-infected flesh.\n" +
                "Sadly I do not believe killing it will lessen the corruption here.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 3;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}