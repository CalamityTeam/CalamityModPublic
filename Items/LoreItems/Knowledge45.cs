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
	public class Knowledge45 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Calamitas");
			Tooltip.SetDefault("The witch unrivaled. Perhaps the only one amongst my cohorts to have ever given me cause for doubt.\n" +
				"Now that you have defeated her your destiny is clear.\n" +
				"Come now, face me.\n" +
				"Place in your inventory to die instantly from every hit.");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 10;
			item.consumable = false;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 15;
		}
		
		public override bool CanUseItem(Player player)
		{
			return false;
		}

		public override void UpdateInventory(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.SCalLore = true;
		}
	}
}