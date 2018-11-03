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
	public class AirSpinner : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Air Spinner");
		}

	    public override void SetDefaults()
	    {
	    	item.CloneDefaults(ItemID.Valor);
	        item.damage = 21;
	        item.useTime = 22;
	        item.useAnimation = 22;
	        item.useStyle = 5;
	        item.channel = true;
	        item.melee = true;
	        item.knockBack = 4;
	        item.value = 50000;
	        item.rare = 3;
	        item.autoReuse = false;
	        item.shoot = mod.ProjectileType("AirSpinnerProjectile");
	    }
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "AerialiteBar", 6);
	        recipe.AddTile(TileID.SkyMill);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}