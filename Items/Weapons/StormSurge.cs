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
	public class StormSurge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Storm Surge");
			Tooltip.SetDefault("Fear the storm\nDoes not consume ammo");
		}

	    public override void SetDefaults()
	    {
			item.damage = 22;
			item.ranged = true;
			item.width = 58;
			item.height = 22;
			item.useTime = 15;
			item.useAnimation = 15;
			item.useStyle = 5;
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 5f;
			item.UseSound = SoundID.Item122;
			item.value = 10000;
			item.rare = 2;
			item.autoReuse = false;
			item.shoot = mod.ProjectileType("StormSurge"); //idk why but all the guns in the vanilla source have this
			item.shootSpeed = 10f;
		}
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "StormlionMandible");
			recipe.AddIngredient(null, "VictideBar", 2);
			recipe.AddIngredient(null, "AerialiteBar", 2);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}