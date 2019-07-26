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
	public class Knowledge33 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ravager");
			Tooltip.SetDefault("The flesh golem constructed using twisted necromancy during the time of my conquest to counter my unstoppable forces.\n" +
				"Its creators were slaughtered by it moments after its conception. It is for the best that it has been destroyed.\n" +
				"Place in your inventory to gain an increase to all damage but reduced wing flight time.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 8;
			item.consumable = false;
		}

		public override bool CanUseItem(Player player)
		{
			return false;
		}

		public override void UpdateInventory(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.ravagerLore = true;
		}
	}
}
