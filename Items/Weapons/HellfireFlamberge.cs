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
	public class HellfireFlamberge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hellfire Flamberge");
			Tooltip.SetDefault("Fires a spread of fireballs");
		}

		public override void SetDefaults()
		{
			item.width = 60;
			item.damage = 102;
			item.melee = true;
			item.useAnimation = 20;
			item.useStyle = 1;
			item.useTime = 20;
			item.useTurn = true;
			item.knockBack = 7.75f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 60;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
			item.shoot = mod.ProjectileType("ChaosFlameSmall");
			item.shootSpeed = 20f;
		}
		
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	float SpeedA = speedX;
	   		float SpeedB = speedY;
	        int num6 = Main.rand.Next(3, 5);
	        for (int index = 0; index < num6; ++index)
	        {
	      	 	float num7 = speedX;
	            float num8 = speedY;
	            float SpeedX = speedX + (float) Main.rand.Next(-40, 41) * 0.05f;
	            float SpeedY = speedY + (float) Main.rand.Next(-40, 41) * 0.05f;
	    		switch (Main.rand.Next(3))
				{
	    			case 0: type = mod.ProjectileType("ChaosFlameSmall"); break;
	    			case 1: type = mod.ProjectileType("ChaosFlameMedium"); break;
	    			case 2: type = mod.ProjectileType("ChaosFlameLarge"); break;
	    			default: break;
				}
	            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)((double)damage * 0.75), knockBack, player.whoAmI, 0.0f, 0.0f);
	    	}
	    	return false;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "CruptixBar", 15);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 174);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			target.AddBuff(BuffID.OnFire, 300);
		}
	}
}
