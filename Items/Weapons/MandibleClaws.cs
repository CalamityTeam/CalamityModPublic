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
	public class MandibleClaws : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mandible Claws");
		}

		public override void SetDefaults()
		{
			item.width = 22;
			item.damage = 14;
			item.melee = true;
			item.useAnimation = 6;
			item.useStyle = 1;
			item.useTime = 6;
			item.useTurn = true;
			item.knockBack = 3.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 18;
			item.value = 10000;
			item.rare = 1;
		}
	}
}
