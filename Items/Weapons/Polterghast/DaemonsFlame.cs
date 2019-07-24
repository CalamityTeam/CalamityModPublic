using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Polterghast
{
	public class DaemonsFlame : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Daemon's Flame");
			Tooltip.SetDefault("Shoots daemon flame arrows as well as regular arrows");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 160;
	        item.width = 20;
	        item.height = 12;
	        item.useTime = 12;
	        item.useAnimation = 12;
	        item.useStyle = 5;
	        item.knockBack = 4f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item5;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.ranged = true;
			item.channel = true;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("DaemonsFlame");
	        item.shootSpeed = 20f;
	        item.useAmmo = 40;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("DaemonsFlame"), damage, knockBack, player.whoAmI, 0f, 0f);
	    	return false;
	    }
	}
}