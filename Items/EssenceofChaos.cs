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
	public class EssenceofChaos : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Essence of Chaos");
			Tooltip.SetDefault("The essence of chaotic creatures");
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			maxFallSpeed = 0f;
			float num = (float)Main.rand.Next(90, 111) * 0.01f;
			num *= Main.essScale;
			Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.5f * num, 0.3f * num, 0.05f * num);
		}

		public override void SetDefaults()
		{
			item.width = 8;
			item.height = 10;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 3, 0, 0);
			item.rare = 5;
		}
	}
}