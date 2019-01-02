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
	public class CrawCarapace : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Craw Carapace");
			Tooltip.SetDefault("5% increased damage reduction\n" +
				"Enemies take damage when they touch you");
		}
		
		public override void SetDefaults()
		{
			item.defense = 3;
			item.width = 20;
			item.height = 24;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = 1;
			item.accessory = true;
		}
		
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.endurance += 0.05f;
			player.thorns = 0.25f;
		}
	}
}