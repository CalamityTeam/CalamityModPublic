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
	public class Cnidarian : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cnidarian");
			Tooltip.SetDefault("Fires a seashell when enemies are near");
		}

	    public override void SetDefaults()
	    {
	    	item.CloneDefaults(ItemID.CorruptYoyo);
	        item.damage = 13;
	        item.useTime = 25;
	        item.useAnimation = 25;
	        item.useStyle = 5;
	        item.channel = true;
	        item.melee = true;
	        item.knockBack = 3f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("CnidarianProjectile");
	    }
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "VictideBar", 2);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}