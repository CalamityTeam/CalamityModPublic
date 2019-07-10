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
	public class Knowledge2 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Duke Fishron");
			Tooltip.SetDefault("The mutant terror of the sea was once the trusted companion of an old king; he tamed it using its favorite treat.\n" +
                "Long ago, the creature flew in desperation from the raging bloody inferno consuming its home, ultimately finding its way to the ocean.\n" +
				"Place in your inventory for an increase to all damage, crit, and movement speed while submerged in liquid.");
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
			if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
			{
				CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
				modPlayer.dukeFishronLore = true;
			}
		}
	}
}
