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
	public class CoreofCinder : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Core of Sunlight");
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 16;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 5, 0, 0);
			item.rare = 8;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			maxFallSpeed = 0f;
			float num = (float)Main.rand.Next(90, 111) * 0.01f;
			num *= Main.essScale;
			Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.3f * num, 0.3f * num, 0.05f * num);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "EssenceofCinder");
			recipe.AddIngredient(ItemID.Ectoplasm);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this, 3);
			recipe.AddRecipe();
		}
	}
}