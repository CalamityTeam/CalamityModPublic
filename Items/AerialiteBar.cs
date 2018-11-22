using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items {
public class AerialiteBar : ModItem
{
	public override void SetStaticDefaults()
 	{
 		DisplayName.SetDefault("Aerialite Bar");
 	}
	
	public override void SetDefaults()
	{
		item.width = 15;
		item.height = 12;
		item.maxStack = 999;
		item.value = 18750;
		item.rare = 2;
	}
	
	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
		recipe.AddIngredient(null, "AerialiteOre", 4);
		recipe.AddTile(TileID.Furnaces);
		recipe.SetResult(this);
		recipe.AddRecipe();
	}
}}