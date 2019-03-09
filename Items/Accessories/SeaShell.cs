using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
	public class SeaShell : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sea Shell");
			Tooltip.SetDefault("Increased defense and damage reduction when submerged in liquid\n" +
				"Increased movement speed when submerged in liquid");
		}
		
		public override void SetDefaults()
		{
			item.defense = 2;
			item.width = 20;
			item.height = 24;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = 1;
			item.accessory = true;
		}
		
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
			{
				player.statDefense += 3;
				player.endurance += 0.05f;
				player.moveSpeed += 0.15f;
				player.ignoreWater = true;
			}
		}
	}
}