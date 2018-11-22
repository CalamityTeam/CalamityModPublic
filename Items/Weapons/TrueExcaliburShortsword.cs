using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons {
public class TrueExcaliburShortsword : ModItem
{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("True Excalibur Shortsword");
			Tooltip.SetDefault("Don't underestimate the power of shortswords");
		}

	public override void SetDefaults()
	{
		item.useStyle = 3;
		item.useTurn = false;
		item.useAnimation = 12;
		item.useTime = 12;  //Ranges from 1 to 55.
		item.width = 50;  //The width of the .png file in pixels divided by 2.
		item.height = 50;  //The height of the .png file in pixels divided by 2.
		item.damage = 70;  //Keep this reasonable please.
		item.melee = true;  //Dictates whether this is a melee-class weapon.
		item.knockBack = 5.75f;  //Ranges from 1 to 9.
		item.UseSound = SoundID.Item1;
		item.useTurn = true;
		item.autoReuse = true;  //Dictates whether the weapon can be "auto-fired".
		item.shoot = mod.ProjectileType("ShortBeam");
		item.shootSpeed = 12f;
		item.value = 1000000;  //Value is calculated in copper coins.
		item.rare = 8;  //Ranges from 1 to 11.
	}

	public override void AddRecipes()
	{
		ModRecipe recipe = new ModRecipe(mod);
		recipe.AddIngredient(null, "ExcaliburShortsword");
		recipe.AddIngredient(ItemID.BrokenHeroSword);
        recipe.AddTile(TileID.MythrilAnvil);	
        recipe.SetResult(this);
        recipe.AddRecipe();
	}

    public override void MeleeEffects(Player player, Rectangle hitbox)
    {
        if (Main.rand.Next(5) == 0)
        {
        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 57);
        }
    }
}}
