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
	public class PhantomHeart : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantom Heart");
			Tooltip.SetDefault("Permanently increases maximum mana by 50");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.useAnimation = 30;
			item.useTime = 30;
			item.useStyle = 4;
			item.UseSound = SoundID.Item29;
			item.consumable = true;
		}
		
		public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(0, 255, 0);
	            }
	        }
	    }
		
		public override bool CanUseItem(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (modPlayer.pHeart)
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
				modPlayer.pHeart = true;
			}
			return true;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "Phantoplasm", 100);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}