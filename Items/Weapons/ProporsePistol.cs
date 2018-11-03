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
	public class ProporsePistol : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Proporse Pistol");
			Tooltip.SetDefault("Fires a blue energy blast that bounces on tile hits");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 45;
	        item.ranged = true;
	        item.width = 36;
	        item.height = 20;
	        item.useTime = 25;
	        item.useAnimation = 25;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3.5f;
	        item.value = 100000;
	        item.rare = 5;
	        item.UseSound = SoundID.Item33;
	        item.autoReuse = true;
	        item.shootSpeed = 20f;
	        item.shoot = mod.ProjectileType("ProBolt");
	        item.useAmmo = 97;
	    }
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("ProBolt"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	}
}