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
	public class EbonianGel : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blighted Gel");
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 14;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 0, 10, 0);
			item.rare = 1;
		}
	}
}