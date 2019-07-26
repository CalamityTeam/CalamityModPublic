using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.DesertScourge
{
	public class StormSpray : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Storm Spray");
			Tooltip.SetDefault("Fires a spray of water that drips extra trails of water");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 18;
	        item.magic = true;
	        item.mana = 8;
	        item.width = 42;
	        item.height = 42;
	        item.useTime = 23;
	        item.useAnimation = 23;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 2f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
	        item.UseSound = SoundID.Item13;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("WaterStream");
	        item.shootSpeed = 10f;
	    }

	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
			return false;
		}
	}
}
