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
	public class BarofLife : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bar of Life");
		}

		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 24;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 5, 0, 0);
			item.rare = 8;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "VerstaltiteBar");
			recipe.AddIngredient(null, "DraedonBar");
			recipe.AddIngredient(null, "CruptixBar");
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}