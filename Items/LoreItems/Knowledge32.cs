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
	public class Knowledge32 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Plaguebringer Goliath");
			Tooltip.SetDefault("A horrific amalgam of steel, flesh, and infection, capable of destroying an entire civilization in just one onslaught.\n" +
				"Its plague nuke barrage can leave an entire area uninhabitable for months. A shame that it came to this but the plague must be contained.\n" +
				"Place in your inventory to gain increased wing flight time but at the cost of reduced life regen.");
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
			modPlayer.plaguebringerGoliathLore = true;
		}
	}
}
