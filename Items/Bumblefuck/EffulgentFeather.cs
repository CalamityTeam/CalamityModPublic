using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Bumblefuck
{
	public class EffulgentFeather : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Effulgent Feather");
			Tooltip.SetDefault("It vibrates with fluffy golden energy");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(3, 12));
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 24;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 7, 50, 0);
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}
	}
}