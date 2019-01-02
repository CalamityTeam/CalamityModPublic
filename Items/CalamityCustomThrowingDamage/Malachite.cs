using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
	public class Malachite : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Malachite");
			Tooltip.SetDefault("Legendary Drop\n" +
				"Throws a stream of kunai that stick to enemies and explode\n" +
				"Right click to throw a single kunai that pierces, after piercing an enemy it emits a massive explosion on the next enemy hit\n" +
                "Revengeance drop");
		}

		public override void SafeSetDefaults()
		{
			item.width = 26;
			item.damage = 58;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useTime = 10;
			item.useAnimation = 10;
			item.useStyle = 1;
			item.knockBack = 1.25f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 58;
			item.value = 5000000;
			item.shoot = mod.ProjectileType("Malachite");
			item.shootSpeed = 10f;
		}
		
		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
		
		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				item.useTime = 10;
	    		item.useAnimation = 10;
	        	item.UseSound = SoundID.Item109;
			}
			else
			{
				item.useTime = 5;
	    		item.useAnimation = 5;
	        	item.UseSound = SoundID.Item1;
			}
			return base.CanUseItem(player);
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	if (player.altFunctionUse == 2)
	    	{
	    		Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("MalachiteBolt"), (int)((double)damage * 2.0), knockBack, player.whoAmI, 0.0f, 0.0f);
	    		return false;
	    	}
	    	else
	    	{
	        	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Malachite"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    		return false;
	    	}
		}
	}
}
