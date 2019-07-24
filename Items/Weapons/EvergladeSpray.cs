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
	public class EvergladeSpray : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Everglade Spray");
			Tooltip.SetDefault("Fires a stream of burning green ichor");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 28;
	        item.magic = true;
	        item.mana = 8;
	        item.width = 30;
	        item.height = 30;
	        item.useTime = 6;
	        item.useAnimation = 18;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 4.5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
	        item.UseSound = SoundID.Item13;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("EvergladeSprayProjectile");
	        item.shootSpeed = 10f;
	    }
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Ichor, 20);
			recipe.AddIngredient(null, "DraedonBar", 3);
	        recipe.AddTile(TileID.Bookcases);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}