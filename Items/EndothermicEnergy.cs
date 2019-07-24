using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;
using CalamityMod.Items;

namespace CalamityMod.Items
{
	public class EndothermicEnergy : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Endothermic Energy");
			Tooltip.SetDefault("Great for preventing heat stroke");
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			maxFallSpeed = 0f;
			float num = (float)Main.rand.Next(90, 111) * 0.01f;
			num *= Main.essScale;
			Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0f * num, 0f * num, 1.2f * num);
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 999;
			item.rare = 10;
			item.value = Item.buyPrice(0, 7, 50, 0);
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}
	}
}