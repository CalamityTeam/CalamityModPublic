using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Crabulon
{
	public class Mycoroot : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mycoroot");
			Tooltip.SetDefault("Fires a stream of short-range fungal roots");
		}

		public override void SetDefaults()
		{
			item.width = 32;  //The width of the .png file in pixels divided by 2.
			item.damage = 10;  //Keep this reasonable please.
			item.thrown = true;  //Dictates whether this is a melee-class weapon.
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useTime = 5;
			item.useAnimation = 5;
			item.useStyle = 1;
			item.knockBack = 1.75f;  //Ranges from 1 to 9.
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;  //Dictates whether the weapon can be "auto-fired".
			item.height = 32;  //The height of the .png file in pixels divided by 2.
			item.maxStack = 1;
			item.rare = 2;
			item.value = 40000;  //Value is calculated in copper coins.
			item.shoot = mod.ProjectileType("Mycoroot");
			item.shootSpeed = 20f;
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
		    float SpeedX = speedX + (float) Main.rand.Next(-30, 31) * 0.05f;
		    float SpeedY = speedY + (float) Main.rand.Next(-30, 31) * 0.05f;
		    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
		    return false;
		}
	}
}
