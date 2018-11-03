using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons 
{
	public class OldLordOathsword : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Old Lord Oathsword");
			Tooltip.SetDefault("A relic of the ancient underworld");
		}

		public override void SetDefaults()
		{
			item.damage = 31;
			item.width = 78;
			item.height = 78;
			item.melee = true;
			item.useAnimation = 24;
			item.useStyle = 1;
			item.useTime = 24;
			item.useTurn = true;
			item.knockBack = 4.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.value = 120000;
			item.rare = 3;
		}
	}
}
