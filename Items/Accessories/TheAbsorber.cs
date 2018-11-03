using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
	public class TheAbsorber : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Absorber");
			Tooltip.SetDefault("12% increased movement speed\n" +
				"120% increased jump speed\n" +
				"+20 max life and mana\n" +
				"Standing still boosts life and mana regen\n" +
				"Increased defense and damage reduction when submerged in liquid\n" +
				"Increased movement speed when submerged in liquid\n" +
				"Enemies take damage when they hit you\n" +
				"Taking a hit will make you move very fast for a short time\n" +
				"You emit a mushroom spore and spark explosion when you are hit\n" +
				"6% increased damage reduction\n" +
				"Enemy attacks will have part of their damage absorbed and used to heal you");
		}
		
		public override void SetDefaults()
		{
			item.defense = 6;
			item.width = 20;
			item.height = 24;
			item.value = 3000000;
			item.rare = 10;
			item.accessory = true;
		}
		
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.aSpark = true;
			modPlayer.gShell = true;
			modPlayer.fCarapace = true;
			modPlayer.absorber = true;
			player.statManaMax2 += 20;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "GrandGelatin");
			recipe.AddIngredient(null, "SeaShell");
			recipe.AddIngredient(null, "CrawCarapace");
			recipe.AddIngredient(null, "FungalCarapace");
			recipe.AddIngredient(null, "GiantTortoiseShell");
			recipe.AddIngredient(null, "AmidiasSpark");
            recipe.AddIngredient(null, "DepthCells", 15);
            recipe.AddIngredient(null, "Lumenite", 15);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "GrandGelatin");
			recipe.AddIngredient(null, "SeaShell");
			recipe.AddIngredient(null, "FungalCarapace");
			recipe.AddIngredient(null, "GiantShell");
			recipe.AddIngredient(null, "GiantTortoiseShell");
			recipe.AddIngredient(null, "AmidiasSpark");
            recipe.AddIngredient(null, "DepthCells", 15);
            recipe.AddIngredient(null, "Lumenite", 15);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}