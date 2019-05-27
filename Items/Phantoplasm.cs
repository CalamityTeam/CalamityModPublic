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
	public class Phantoplasm : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantoplasm");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 5));
		}

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.rare = 10;
			item.value = Item.buyPrice(0, 7, 0, 0);
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(200, 200, 200, 0);
		}
	}
}