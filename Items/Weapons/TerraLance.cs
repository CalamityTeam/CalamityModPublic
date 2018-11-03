using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class TerraLance : ModItem
{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Terra Lance");
		}

	public override void SetDefaults()
	{
		item.width = 44;  //The width of the .png file in pixels divided by 2.
		item.damage = 100;  //Keep this reasonable please.
		item.melee = true;  //Dictates whether this is a melee-class weapon.
		item.noMelee = true;
		item.useTurn = true;
		item.noUseGraphic = true;
		item.useAnimation = 17;
		item.useStyle = 5;
		item.useTime = 17;
		item.knockBack = 8.5f;  //Ranges from 1 to 9.
		item.UseSound = SoundID.Item1;
		item.autoReuse = true;  //Dictates whether the weapon can be "auto-fired".
		item.height = 44;  //The height of the .png file in pixels divided by 2.
		item.maxStack = 1;
		item.value = 885000;  //Value is calculated in copper coins.
		item.rare = 8;  //Ranges from 1 to 11.
		item.shoot = mod.ProjectileType("TerraLanceProjectile");
		item.shootSpeed = 11f;
	}
	
	public override bool CanUseItem(Player player)
    {
        for (int i = 0; i < 1000; ++i)
        {
            if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
            {
                return false;
            }
        }
        return true;
    }

	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
		recipe.AddIngredient(null, "StarnightLance");
		recipe.AddIngredient(null, "HellionFlowerSpear");
		recipe.AddIngredient(null, "ExsanguinationLance");
		recipe.AddIngredient(null, "LivingShard", 5);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.SetResult(this);
        recipe.AddRecipe();
	}
}}
