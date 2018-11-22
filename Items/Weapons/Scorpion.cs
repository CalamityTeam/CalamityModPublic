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
	public class Scorpion : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scorpio");
			Tooltip.SetDefault("Rockets\nRight click to change modes");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 95;
	        item.ranged = true;
	        item.width = 58;
	        item.height = 26;
	        item.useTime = 13;
	        item.useAnimation = 13;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 6.5f;
	        item.value = 3050000;
	        item.rare = 9;
	        item.UseSound = SoundID.Item11;
	        item.autoReuse = true;
	        item.shootSpeed = 20f;
	        item.shoot = mod.ProjectileType("MiniRocket");
	        item.useAmmo = 771;
	    }
	    
	    public override bool AltFunctionUse(Player player)
		{
			return true;
		}
	    
	    public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.useTime = 39;
	        	item.useAnimation = 39;
			}
			else
			{
				item.useTime = 13;
	        	item.useAnimation = 13;
			}
			return base.CanUseItem(player);
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	if (player.altFunctionUse == 2)
	    	{
	    		Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("BigNuke"), (int)((double)damage * 3f), knockBack, player.whoAmI, 0.0f, 0.0f);
	    		return false;
	    	}
	    	else
	    	{
	    		Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("MiniRocket"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    		return false;
	    	}
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.SnowmanCannon);
	        recipe.AddIngredient(ItemID.GrenadeLauncher);
	        recipe.AddIngredient(ItemID.RocketLauncher);
	        recipe.AddIngredient(ItemID.FragmentVortex, 20);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}