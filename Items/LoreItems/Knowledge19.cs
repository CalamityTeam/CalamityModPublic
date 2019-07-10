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
	public class Knowledge19 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cryogen");
			Tooltip.SetDefault("The archmage's prison.\n" +
                "I am unsure if it has grown weaker over the decades of imprisonment.\n" +
				"Place in your inventory to gain a frost dash that freezes enemies, at the cost of slightly reduced defense.");
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

		public override void UpdateInventory(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.dashMod = 6;
			player.statDefense -= 10;
		}
	}
}
