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
	public class Knowledge37 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moon Lord");
			Tooltip.SetDefault("What a waste.\n" +
				"Had it been fully restored it would have been a force to behold, but what you fought was an empty shell.\n" +
				"However, that doesn't diminish the immense potential locked within it, released upon its death.\n" +
				"Place in your inventory to gain an improved Gravity Globe that gives you an increase to all stats while upside down.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 10;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}

		public override void UpdateInventory(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.moonLordLore = true;
		}
	}
}