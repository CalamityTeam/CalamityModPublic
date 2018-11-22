using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class TitaniumShuriken : ModItem
{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Titanium Shuriken");
		}

	public override void SetDefaults()
	{
		item.width = 38;  //The width of the .png file in pixels divided by 2.
		item.damage = 31;  //Keep this reasonable please.
		item.thrown = true;  //Dictates whether this is a melee-class weapon.
		item.noMelee = true;
		item.consumable = true;
		item.noUseGraphic = true;
		item.useAnimation = 9;
		item.scale = 0.75f;
		item.crit = 10;
		item.useStyle = 1;
		item.useTime = 9;
		item.knockBack = 3f;  //Ranges from 1 to 9.
		item.UseSound = SoundID.Item1;
		item.autoReuse = true;  //Dictates whether the weapon can be "auto-fired".
		item.height = 38;  //The height of the .png file in pixels divided by 2.
		item.maxStack = 999;
		item.value = 2000;  //Value is calculated in copper coins.
		item.rare = 4;  //Ranges from 1 to 11.
		item.shoot = mod.ProjectileType("TitaniumShurikenProjectile");
		item.shootSpeed = 16f;
	}
	
	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(ItemID.TitaniumBar);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.SetResult(this, 30);
        recipe.AddRecipe();
	}
}}
