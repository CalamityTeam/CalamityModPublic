using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.HiveMind
{
	public class TrueShadowScale : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("True Shadow Scale");
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 22;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 0, 50, 0);
			item.rare = 2;
		}
	}
}