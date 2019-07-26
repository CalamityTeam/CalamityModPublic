using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items
{
	public class MiracleFruit : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Miracle Fruit");
			Tooltip.SetDefault("Permanently increases maximum life by 25\n" +
			                   "Can only be used if the max amount of life fruit has been consumed");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.useAnimation = 30;
			item.rare = 7;
			item.useTime = 30;
			item.useStyle = 4;
			item.UseSound = SoundID.Item4;
			item.consumable = true;
		}

		public override bool CanUseItem(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (modPlayer.mFruit || player.statLifeMax < 500)
			{
				return false;
			}
			return true;
		}

		public override bool UseItem(Player player)
		{
			if (player.itemAnimation > 0 && player.itemTime == 0)
			{
				player.itemTime = item.useTime;
				if (Main.myPlayer == player.whoAmI)
				{
					player.HealEffect(25);
				}
				CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
				modPlayer.mFruit = true;
			}
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LifeFruit, 5);
			recipe.AddIngredient(null, "AstralJelly", 5);
			recipe.AddIngredient(null, "LivingShard", 10);
			recipe.AddIngredient(null, "Stardust", 20);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
