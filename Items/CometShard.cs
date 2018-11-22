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
	public class CometShard : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Comet Shard");
			Tooltip.SetDefault("Permanently increases maximum mana by 50");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.useAnimation = 30;
			item.rare = 5;
			item.useTime = 30;
			item.useStyle = 4;
			item.UseSound = SoundID.Item29;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (modPlayer.cShard)
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
					player.ManaEffect(50);
				}
				CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
				modPlayer.cShard = true;
			}
			return true;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.MeteoriteBar, 10);
			recipe.AddIngredient(ItemID.FallenStar, 50);
			recipe.AddIngredient(null, "Stardust", 150);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}