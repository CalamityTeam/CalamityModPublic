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
	public class LifeJelly : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Life Jelly");
			Tooltip.SetDefault("+20 max life\n" +
				"Standing still boosts life regen");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 24;
            item.value = Item.buyPrice(0, 6, 0, 0);
            item.rare = 1;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statLifeMax2 += 20;
			if ((double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
			{
				player.lifeRegen += 2;
			}
		}
	}
}
