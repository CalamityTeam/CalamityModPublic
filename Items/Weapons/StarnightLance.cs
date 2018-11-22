using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class StarnightLance : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Starnight Lance");
	}

	public override void SetDefaults()
	{
		item.width = 72;  //The width of the .png file in pixels divided by 2.
		item.damage = 50;  //Keep this reasonable please.
		item.melee = true;  //Dictates whether this is a melee-class weapon.
		item.noMelee = true;
		item.useTurn = true;
		item.noUseGraphic = true;
		item.useAnimation = 23;
		item.useStyle = 5;
		item.useTime = 23;
		item.knockBack = 6;  //Ranges from 1 to 9.
		item.UseSound = SoundID.Item1;
		item.autoReuse = false;  //Dictates whether the weapon can be "auto-fired".
		item.height = 72;  //The height of the .png file in pixels divided by 2.
		item.maxStack = 1;
		item.value = 325000;  //Value is calculated in copper coins.
		item.rare = 5;  //Ranges from 1 to 11.
		item.shoot = mod.ProjectileType("StarnightLanceProjectile");
		item.shootSpeed = 6f;
	}

	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
		recipe.AddIngredient(null, "VerstaltiteBar", 12);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.SetResult(this);
        recipe.AddRecipe();
	}
}}
