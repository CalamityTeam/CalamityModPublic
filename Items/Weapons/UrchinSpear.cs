using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class UrchinSpear : ModItem
{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Urchin Spear");
		}

	public override void SetDefaults()
	{
		item.width = 56;  //The width of the .png file in pixels divided by 2.
		item.damage = 17;  //Keep this reasonable please.
		item.melee = true;  //Dictates whether this is a melee-class weapon.
		item.noMelee = true;
		item.useTurn = true;
		item.noUseGraphic = true;
		item.useAnimation = 25;
		item.useStyle = 5;
		item.useTime = 25;
		item.knockBack = 5f;  //Ranges from 1 to 9.
		item.UseSound = SoundID.Item1;
		item.autoReuse = false;  //Dictates whether the weapon can be "auto-fired".
		item.height = 56;  //The height of the .png file in pixels divided by 2.
		item.maxStack = 1;
		item.value = 45000;  //Value is calculated in copper coins.
		item.rare = 2;  //Ranges from 1 to 11.
		item.shoot = mod.ProjectileType("UrchinSpearProjectile");
		item.shootSpeed = 4f;
	}

	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
		recipe.AddIngredient(null, "VictideBar", 4);
        recipe.AddTile(TileID.Anvils);
        recipe.SetResult(this);
        recipe.AddRecipe();
	}
}}
