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
	public class Purge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nano Purge");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 70;
	        item.magic = true;
	        item.mana = 6;
	        item.width = 20;
	        item.height = 12;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.noUseGraphic = true;
			item.channel = true;
	        item.knockBack = 3f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("Purge");
	        item.shootSpeed = 24f;
	    }
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.FragmentVortex, 20);
			recipe.AddIngredient(ItemID.LaserMachinegun);
			recipe.AddIngredient(ItemID.Nanites, 100);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Purge"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	}
}