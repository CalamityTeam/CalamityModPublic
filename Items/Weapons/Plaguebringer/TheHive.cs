using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Plaguebringer
{
	public class TheHive : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Hive");
		}

	    public override void SetDefaults()
	    {
			item.damage = 70;
			item.ranged = true;
			item.width = 62;
			item.height = 30;
			item.useTime = 21;
			item.useAnimation = 21;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
			item.UseSound = SoundID.Item61;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("BeeRPG");
			item.shootSpeed = 13f;
			item.useAmmo = 771;
		}
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	switch (Main.rand.Next(4))
			{
	    		case 0: type = mod.ProjectileType("GoliathRocket"); break;
	    		case 1: type = mod.ProjectileType("HiveMissile"); break;
	    		case 2: type = mod.ProjectileType("HiveBomb"); break;
	    		case 3: type = mod.ProjectileType("BeeRPG"); break;
	    		default: break;
			}
	        Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	}
}