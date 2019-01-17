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
	public class Meowthrower : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Meowthrower");
			Tooltip.SetDefault("Consumes gel at a 50% chance");
		}

	    public override void SetDefaults()
	    {
			item.damage = 28;
			item.ranged = true;
			item.width = 74;
			item.height = 24;
			item.useTime = 10;
			item.useAnimation = 30;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 1.25f;
			item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 4;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("MeowFire");
			item.shootSpeed = 5.5f;
			item.useAmmo = 23;
		}
	    
	    public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) <= 50)
	    		return false;
	    	return true;
	    }
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	float SpeedA = speedX;
	   		float SpeedB = speedY;
	        int num6 = Main.rand.Next(1, 3);
	        for (int index = 0; index < num6; ++index)
	        {
	      	 	float num7 = speedX;
	            float num8 = speedY;
	            float SpeedX = speedX + (float) Main.rand.Next(-15, 16) * 0.05f;
	            float SpeedY = speedY + (float) Main.rand.Next(-15, 16) * 0.05f;
	    		switch (Main.rand.Next(3))
				{
	    			case 1: type = mod.ProjectileType("MeowFire"); break;
	    			case 2: type = mod.ProjectileType("MeowFire2"); break;
	    			default: break;
				}
	            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	}
	    	return false;
		}
	}
}