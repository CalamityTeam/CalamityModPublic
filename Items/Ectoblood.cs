using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items
{
	public class Ectoblood : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ectoblood");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 32;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.rare = 8;
		}
	}
}
