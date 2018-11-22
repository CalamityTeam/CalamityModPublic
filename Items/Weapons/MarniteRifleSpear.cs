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
	public class MarniteRifleSpear : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Marnite Rifle Spear");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 15;
	        item.ranged = true;
	        item.width = 72;
	        item.height = 20;
	        item.useTime = 28;
	        item.useAnimation = 28;
	        item.useStyle = 5;
	        item.knockBack = 2.25f;
	        item.value = 40000;
	        item.rare = 2;
	        item.UseSound = SoundID.Item41;
	        item.autoReuse = true;
	        item.shootSpeed = 22f;
	        item.useAmmo = 97;
	        item.shoot = 10;
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.PlatinumBar, 7);
			recipe.AddIngredient(ItemID.Granite, 5);
			recipe.AddIngredient(ItemID.Marble, 5);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GoldBar, 7);
			recipe.AddIngredient(ItemID.Granite, 5);
			recipe.AddIngredient(ItemID.Marble, 5);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}