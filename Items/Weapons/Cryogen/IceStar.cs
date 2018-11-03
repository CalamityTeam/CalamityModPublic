using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Cryogen {
public class IceStar : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Ice Star");
		Tooltip.SetDefault("Ice Stars are too brittle to be recovered after being thrown");
	}

	public override void SetDefaults()
	{
		item.width = 62;  //The width of the .png file in pixels divided by 2.
		item.damage = 35;  //Keep this reasonable please.
		item.thrown = true;  //Dictates whether this is a melee-class weapon.
		item.noMelee = true;
		item.consumable = true;
		item.noUseGraphic = true;
		item.useAnimation = 12;
		item.crit = 7;
		item.useStyle = 1;
		item.useTime = 12;
		item.knockBack = 2.5f;  //Ranges from 1 to 9.
		item.UseSound = SoundID.Item1;
		item.autoReuse = true;  //Dictates whether the weapon can be "auto-fired".
		item.height = 62;  //The height of the .png file in pixels divided by 2.
		item.scale = 0.65f;
		item.maxStack = 999;
		item.value = 3000;  //Value is calculated in copper coins.
		item.rare = 5;  //Ranges from 1 to 11.
		item.shoot = mod.ProjectileType("IceStarProjectile");
		item.shootSpeed = 14f;
	}
	
	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(null, "CryoBar");
        recipe.AddTile(TileID.IceMachine);
        recipe.SetResult(this, 50);
        recipe.AddRecipe();
	}
}}
