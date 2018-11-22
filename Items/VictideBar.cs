using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items {
public class VictideBar : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Victide Bar");
	}
		
	public override void SetDefaults()
	{
		item.width = 15;
		item.height = 12;
		item.maxStack = 999;
		item.value = 15750;
		item.rare = 2;
	}
	
	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
		recipe.AddIngredient(null, "VictoryShard");
		recipe.AddIngredient(ItemID.Coral);
		recipe.AddIngredient(ItemID.Starfish);
		recipe.AddIngredient(ItemID.Seashell);
		recipe.AddTile(TileID.Furnaces);
		recipe.SetResult(this);
		recipe.AddRecipe();
	}
}}