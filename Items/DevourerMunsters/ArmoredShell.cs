using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.DevourerMunsters
{
	public class ArmoredShell : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Armored Shell");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 30;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 7, 0, 0);
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}
	}
}