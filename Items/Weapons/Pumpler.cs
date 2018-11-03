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
	public class Pumpler : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pumpler");
			Tooltip.SetDefault("33% chance to not consume ammo");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 9;
	        item.ranged = true;
	        item.width = 50;
	        item.height = 28;
	        item.useTime = 9;
	        item.useAnimation = 9;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 1.25f;
	        item.value = 50000;
	        item.rare = 2;
	        item.UseSound = SoundID.Item11;
	        item.autoReuse = true;
	        item.shoot = 10;
	        item.shootSpeed = 11f;
	        item.useAmmo = 97;
	    }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
		    float SpeedX = speedX + (float) Main.rand.Next(-10, 11) * 0.05f;
		    float SpeedY = speedY + (float) Main.rand.Next(-10, 11) * 0.05f;
		    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
		    return false;
		}
	    
	    public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) <= 33)
	    		return false;
	    	return true;
	    }
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.Pumpkin, 30);
	        recipe.AddIngredient(ItemID.PumpkinSeed, 5);
	        recipe.AddIngredient(ItemID.IllegalGunParts);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}