using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.AstrumDeus
{
	public class Nebulash : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nebulash");
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 16;
			item.damage = 54;
            item.rare = 7;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.channel = true;
			item.autoReuse = true;
			item.melee = true;
			item.useAnimation = 18;
			item.useTime = 18;
			item.useStyle = 5;
			item.knockBack = 2f;
			item.UseSound = SoundID.Item117;
			item.value = 500000;
			item.shootSpeed = 24f;
			item.shoot = mod.ProjectileType("Nebulash");
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	float ai3 = (Main.rand.NextFloat() - 0.5f) * 0.7853982f; //0.5
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, ai3);
	    	return false;
		}
	}
}
