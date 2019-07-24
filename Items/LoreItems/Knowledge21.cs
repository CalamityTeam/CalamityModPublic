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
	public class Knowledge21 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Destroyer");
			Tooltip.SetDefault("A machine brought to life by the mighty souls of warriors, and built to excavate massive tunnels in planets to gather resources.\n" +
                "Could have proven useful if Draedon didn't have an obsession with turning everything into a tool of destruction.\n" +
				"Place in your inventory to gain a boost to your pick speed.");
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
			modPlayer.destroyerLore = true;
		}
	}
}