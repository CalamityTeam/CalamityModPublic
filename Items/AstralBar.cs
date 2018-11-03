using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items {
public class AstralBar : ModItem
{
	public override void SetStaticDefaults()
 	{
 		DisplayName.SetDefault("Astral Bar");
 	}
	
	public override void SetDefaults()
	{
		item.width = 15;
		item.height = 12;
		item.maxStack = 999;
		item.value = 58750;
		item.rare = 7;
	}
	
	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
		recipe.AddIngredient(null, "Stardust", 12);
		recipe.AddIngredient(null, "AstralOre", 8);
		recipe.AddTile(TileID.AdamantiteForge);
		recipe.SetResult(this, 4);
		recipe.AddRecipe();
	}
}}