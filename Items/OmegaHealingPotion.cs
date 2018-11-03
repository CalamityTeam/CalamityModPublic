using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.Items
{
	public class OmegaHealingPotion : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Omega Healing Potion");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.useTurn = true;
			item.maxStack = 30;
			item.healLife = 300;
			item.useAnimation = 17;
			item.useTime = 17;
			item.useStyle = 2;
			item.UseSound = SoundID.Item3;
			item.consumable = true;
			item.potion = true;
			item.value = 100000;
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

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "SupremeHealingPotion");
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}