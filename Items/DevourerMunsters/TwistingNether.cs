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
	public class TwistingNether : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Twisting Nether");
		}

		public override void SetDefaults()
		{
			item.width = 15;
			item.height = 12;
			item.maxStack = 999;
			item.rare = 10;
			item.value = Item.buyPrice(0, 7, 0, 0);
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			maxFallSpeed = 0f;
			float num = (float)Main.rand.Next(90, 111) * 0.01f;
			num *= Main.essScale;
			Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.5f * num, 0.1f * num, 0.7f * num);
		}
	}
}
