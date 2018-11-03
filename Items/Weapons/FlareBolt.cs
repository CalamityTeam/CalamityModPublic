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
	public class FlareBolt : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flare Bolt");
			Tooltip.SetDefault("Casts a slow-moving ball of flame");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 27;
	        item.magic = true;
	        item.mana = 12;
	        item.width = 28;
	        item.height = 30;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5.5f;
	        item.value = 90000;
	        item.rare = 3;
	        item.UseSound = SoundID.Item20;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("FlareBoltProjectile");
	        item.shootSpeed = 7.5f;
	    }
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.HellstoneBar, 6);
			recipe.AddIngredient(ItemID.Obsidian, 9);
			recipe.AddIngredient(ItemID.Fireblossom, 2);
			recipe.AddIngredient(ItemID.LavaBucket);
	        recipe.AddTile(TileID.Bookcases);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}