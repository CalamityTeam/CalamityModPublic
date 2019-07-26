using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Amidias
{
	public class Waywasher : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Waywasher");
			Tooltip.SetDefault("Casts inaccurate water bolts");
		}

		public override void SetDefaults()
		{
			item.damage = 10;
			item.magic = true;
			item.mana = 4;
			item.width = 30;
			item.height = 30;
			item.useTime = 12;
			item.useAnimation = 12;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 2.5f;
			item.value = Item.buyPrice(0, 2, 0, 0);
			item.rare = 2;
			item.UseSound = SoundID.Item8;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("WaywasherProj");
			item.shootSpeed = 8f;
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float SpeedX = speedX + (float)Main.rand.Next(-25, 26) * 0.05f;
			float SpeedY = speedY + (float)Main.rand.Next(-25, 26) * 0.05f;
			Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, mod.ProjectileType("WaywasherProj"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
			return false;
		}

	}
}
