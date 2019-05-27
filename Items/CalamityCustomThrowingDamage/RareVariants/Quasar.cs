using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.CalamityCustomThrowingDamage.RareVariants
{
	public class Quasar : CalamityDamageItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Quasar");
			Tooltip.SetDefault("Succ");
		}

		public override void SafeSetDefaults()
		{
			item.width = 52;
			item.damage = 50;
			item.crit += 8;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 12;
			item.useStyle = 1;
			item.useTime = 12;
			item.knockBack = 0f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 48;
			item.value = Item.buyPrice(0, 60, 0, 0);
			item.rare = 7;
			item.shoot = mod.ProjectileType("RadiantStar");
			item.shootSpeed = 20f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 22;
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 1f, 0f);
			return false;
		}
	}
}
