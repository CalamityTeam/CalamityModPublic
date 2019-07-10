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
	public class Stardust : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stardust");
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 18;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 3, 0, 0);
			item.rare = 5;
		}
	}
}
