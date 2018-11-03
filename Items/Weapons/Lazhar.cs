using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons 
{
	public class Lazhar : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lazhar");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 75;
	        item.magic = true;
	        item.mana = 4;
	        item.width = 42;
	        item.height = 20;
	        item.useTime = 7;
	        item.useAnimation = 7;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5f;
	        item.value = 1050000;
	        item.rare = 9;
	        item.UseSound = SoundID.Item12;
	        item.autoReuse = true;
	        item.shootSpeed = 15f;
	        item.shoot = mod.ProjectileType("SolarBeam2");
	    }
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	float SpeedX = speedX + (float) Main.rand.Next(-15, 16) * 0.05f;
		    float SpeedY = speedY + (float) Main.rand.Next(-15, 16) * 0.05f;
		    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.HeatRay);
	        recipe.AddIngredient(ItemID.FragmentSolar, 10);
	        recipe.AddIngredient(ItemID.ChlorophyteBar, 6);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}