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
	public class BurningSea : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burning Sea");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 79;
	        item.magic = true;
	        item.mana = 15;
	        item.width = 28;
	        item.height = 30;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 6.5f;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 6;
	        item.UseSound = SoundID.Item20;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("BrimstoneFireball");
	        item.shootSpeed = 15f;
	    }
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "UnholyCore", 5);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}