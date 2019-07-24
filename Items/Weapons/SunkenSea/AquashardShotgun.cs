using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.SunkenSea
{
	public class AquashardShotgun : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aquashard Shotgun");
			Tooltip.SetDefault("Shoots aquashards which split upon hitting an enemy");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 12;
	        item.ranged = true;
	        item.width = 62;
	        item.height = 26;
	        item.crit += 6;
	        item.useTime = 25;
	        item.useAnimation = 25;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5.5f;
			item.value = Item.buyPrice(0, 2, 0, 0);
			item.rare = 2;
	        item.UseSound = SoundID.Item61;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("Aquashard");
	        item.shootSpeed = 22f;
	    }
		
		public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	int num6 = Main.rand.Next(2, 4);
			for (int index = 0; index < num6; ++index)
			{
			    float SpeedX = speedX + (float) Main.rand.Next(-40, 41) * 0.05f;
			    float SpeedY = speedY + (float) Main.rand.Next(-40, 41) * 0.05f;
			    int projectile = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
			    Main.projectile[projectile].timeLeft = 200;
			}
			return false;
		}
	
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Boomstick);
	        recipe.AddIngredient(null, "SeaPrism", 5);
			recipe.AddIngredient(null, "PrismShard", 5);
			recipe.AddIngredient(null, "Navystone", 25);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}