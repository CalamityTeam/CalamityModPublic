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
	public class SHPC : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("SHPC");
			Tooltip.SetDefault("Legendary Drop\n" +
				"Fires plasma orbs that linger and emit massive explosions\n" +
				"Right click to fire powerful energy beams\n" +
                "Revengeance drop");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 18;
	        item.magic = true;
	        item.mana = 20;
	        item.width = 96;
	        item.height = 34;
	        item.useTime = 50;
	        item.useAnimation = 50;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3.25f;
            item.rare = 5;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.UseSound = SoundID.Item92;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("SHPB");
	        item.shootSpeed = 20f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 17;
		}
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-25, 0);
		}
	    
	    public override bool AltFunctionUse(Player player)
		{
			return true;
		}
	    
	    public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.useTime = 7;
	    		item.useAnimation = 7;
	    		item.mana = 6;
	        	item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
			}
			else
			{
				item.useTime = 50;
	        	item.useAnimation = 50;
	        	item.mana = 20;
	        	item.UseSound = SoundID.Item92;
			}
			return base.CanUseItem(player);
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	if (player.altFunctionUse == 2)
	    	{
	        	for (int shootAmt = 0; shootAmt < 3; shootAmt++)
	        	{
	        		float SpeedX = speedX + (float) Main.rand.Next(-20, 21) * 0.05f;
		    		float SpeedY = speedY + (float) Main.rand.Next(-20, 21) * 0.05f;
	        		Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("SHPL"), damage, (knockBack * 0.5f), player.whoAmI, 0.0f, 0.0f);
	        	}
	    		return false;
	    	}
	    	else
	    	{
	        	for (int shootAmt = 0; shootAmt < 3; shootAmt++)
	        	{
	        		float SpeedX = speedX + (float) Main.rand.Next(-40, 41) * 0.05f;
		    		float SpeedY = speedY + (float) Main.rand.Next(-40, 41) * 0.05f;
	        		Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("SHPB"), (int)((double)damage * 1.1), knockBack, player.whoAmI, 0.0f, 0.0f);
	        	}
	    		return false;
	    	}
		}
	}
}