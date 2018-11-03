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
	public class VitalJelly : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vital Jelly");
			Tooltip.SetDefault("10% increased movement speed\n" +
				"100% increased jump speed");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 24;
			item.value = 30000;
			item.rare = 5;
			item.accessory = true;
		}
		
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.moveSpeed += 0.1f;
        	player.jumpSpeedBoost += 1.0f;
		}
	}
}