using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class RedtideSword : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Redtide Sword");
	}

	public override void SetDefaults()
	{
		item.width = 42;
		item.damage = 23;
		item.melee = true;
		item.useAnimation = 19;
		item.useStyle = 1;
		item.useTime = 19;
		item.useTurn = true;
		item.knockBack = 4;
		item.UseSound = SoundID.Item1;
		item.autoReuse = true;
		item.height = 42;
		item.maxStack = 1;
		item.value = 50000;
		item.rare = 2;
	}
	
	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(null, "VictideBar", 3);
        recipe.AddTile(TileID.Anvils);
        recipe.SetResult(this);
        recipe.AddRecipe();
	}
}}
