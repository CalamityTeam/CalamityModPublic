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
	public class Mariana : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mariana");
			Tooltip.SetDefault("Tropical and deadly");
		}

		public override void SetDefaults()
		{
			item.damage = 75;
			item.width = 54;
			item.height = 62;
			item.melee = true;
			item.useAnimation = 24;
			item.useStyle = 1;
			item.useTime = 24;
			item.useTurn = true;
			item.knockBack = 6.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.value = 500000;
			item.rare = 7;
			item.shoot = mod.ProjectileType("MarianaProjectile");
			item.shootSpeed = 16f;
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
			float SpeedA = speedX;
	   		float SpeedB = speedY;
	        int num6 = Main.rand.Next(4, 6);
	        for (int index = 0; index < num6; ++index)
	        {
	      	 	float num7 = speedX;
	            float num8 = speedY;
	            float SpeedX = speedX + (float) Main.rand.Next(-40, 41) * 0.05f;
	            float SpeedY = speedY + (float) Main.rand.Next(-40, 41) * 0.05f;
	            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)((double)damage * 0.5f), knockBack, player.whoAmI, 0.0f, 0.0f);
	        }
	        return false;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ChlorophyteClaymore);
	        recipe.AddIngredient(ItemID.Coral, 3);
	        recipe.AddIngredient(ItemID.Starfish, 3);
	        recipe.AddIngredient(ItemID.Seashell, 3);
            recipe.AddIngredient(null, "DepthCells", 10);
            recipe.AddIngredient(null, "Lumenite", 10);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
		
		public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if(Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 59);
	        }
	    }
	}
}
