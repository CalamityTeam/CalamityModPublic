using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.NPCs;

namespace CalamityMod.Items
{
	public class LivingShard : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Living Shard");
		}

		public override void SetDefaults()
		{
			item.width = 14;
			item.height = 14;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 4, 50, 0);
			item.rare = 7;
		}
	}
}
