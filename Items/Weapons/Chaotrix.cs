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
	public class Chaotrix : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chaotrix");
			Tooltip.SetDefault("Explodes on enemy hits");
		}

	    public override void SetDefaults()
	    {
	    	item.CloneDefaults(ItemID.Yelets);
	        item.damage = 110;
	        item.useTime = 22;
	        item.useAnimation = 22;
	        item.useStyle = 5;
	        item.channel = true;
	        item.melee = true;
	        item.knockBack = 4f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("ChaotrixProjectile");
	    }
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "CruptixBar", 6);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}