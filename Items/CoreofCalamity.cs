using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items {
public class CoreofCalamity : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Core of Calamity");
	}
	
	public override void SetDefaults()
	{
		item.width = 40;
		item.height = 40;
		item.maxStack = 69;
		item.value = 500000;
		item.rare = 10;
	}
	
	public override void Update(ref float gravity, ref float maxFallSpeed)
    {
		maxFallSpeed *= 0f;
	}
	
	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
		recipe.AddIngredient(null, "CoreofCinder", 5);
        recipe.AddIngredient(null, "CoreofEleum", 5);
        recipe.AddIngredient(null, "CoreofChaos", 5);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.SetResult(this);
        recipe.AddRecipe();
	}
}}