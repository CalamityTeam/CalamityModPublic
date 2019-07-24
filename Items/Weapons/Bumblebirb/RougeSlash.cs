using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Bumblebirb
{
	public class RougeSlash : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rouge Slash");
			Tooltip.SetDefault("Fires a wave of 3 rouge air slashes");
		}

	    public override void SetDefaults()
	    {
			item.damage = 90;
			item.magic = true;
			item.mana = 30;
			item.width = 28;
			item.height = 32;
			item.useTime = 19;
			item.useAnimation = 19;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 7.5f;
			item.UseSound = SoundID.Item91;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
			item.shoot = mod.ProjectileType("RougeSlashLarge");
			item.shootSpeed = 24f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	        Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("RougeSlashLarge"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	        Projectile.NewProjectile(position.X, position.Y, speedX * 0.8f, speedY * 0.8f, mod.ProjectileType("RougeSlashMedium"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	        Projectile.NewProjectile(position.X, position.Y, speedX * 0.6f, speedY * 0.6f, mod.ProjectileType("RougeSlashSmall"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	}
}