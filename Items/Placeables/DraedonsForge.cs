using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
	public class DraedonsForge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Draedon's Forge");
			Tooltip.SetDefault("Used to craft uber-tier items");
		}
		
		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 30;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.value = 5000000;
			item.rare = 10;
			item.createTile = mod.TileType("DraedonsForge");
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 20;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.AdamantiteForge);
			recipe.AddIngredient(ItemID.MythrilAnvil);
			recipe.AddIngredient(ItemID.LunarCraftingStation);
			recipe.AddIngredient(ItemID.LunarBar, 5);
			recipe.AddIngredient(null, "CosmiliteBar", 5);
			recipe.AddIngredient(null, "NightmareFuel", 20);
        	recipe.AddIngredient(null, "EndothermicEnergy", 20);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.TitaniumForge);
			recipe.AddIngredient(ItemID.MythrilAnvil);
			recipe.AddIngredient(ItemID.LunarCraftingStation);
			recipe.AddIngredient(ItemID.LunarBar, 5);
			recipe.AddIngredient(null, "CosmiliteBar", 5);
			recipe.AddIngredient(null, "NightmareFuel", 20);
        	recipe.AddIngredient(null, "EndothermicEnergy", 20);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.TitaniumForge);
			recipe.AddIngredient(ItemID.OrichalcumAnvil);
			recipe.AddIngredient(ItemID.LunarCraftingStation);
			recipe.AddIngredient(ItemID.LunarBar, 5);
			recipe.AddIngredient(null, "CosmiliteBar", 5);
			recipe.AddIngredient(null, "NightmareFuel", 20);
        	recipe.AddIngredient(null, "EndothermicEnergy", 20);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.AdamantiteForge);
			recipe.AddIngredient(ItemID.OrichalcumAnvil);
			recipe.AddIngredient(ItemID.LunarCraftingStation);
			recipe.AddIngredient(ItemID.LunarBar, 5);
			recipe.AddIngredient(null, "CosmiliteBar", 5);
			recipe.AddIngredient(null, "NightmareFuel", 20);
        	recipe.AddIngredient(null, "EndothermicEnergy", 20);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}