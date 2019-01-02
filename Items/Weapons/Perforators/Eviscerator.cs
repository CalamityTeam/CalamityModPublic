using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Perforators
{
	public class Eviscerator : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eviscerator");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 65;
	        item.ranged = true;
	        item.width = 58;
	        item.height = 22;
	        item.crit += 25;
	        item.useTime = 60;
	        item.useAnimation = 60;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 7.5f;
	        item.value = 80000;
	        item.rare = 3;
	        item.UseSound = SoundID.Item40;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("BloodClotFriendly");
	        item.shootSpeed = 22f;
	        item.useAmmo = 97;
	    }
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("BloodClotFriendly"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "BloodSample", 8);
	        recipe.AddIngredient(ItemID.Vertebrae, 4);
	        recipe.AddIngredient(ItemID.CrimtaneBar, 4);
	        recipe.AddTile(TileID.DemonAltar);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}