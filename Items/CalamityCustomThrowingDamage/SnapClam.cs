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
	public class SnapClam : CalamityDamageItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Snap Clam");
			Tooltip.SetDefault("Can latch on enemies and deal damage over time");
		}

		public override void SafeSetDefaults()
		{
			item.width = 26;
			item.height = 16;
			item.damage = 15;
			item.thrown = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useTime = 25;
			item.useAnimation = 25;
			item.useStyle = 1;
			item.knockBack = 3f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.value = Item.buyPrice(0, 2, 0, 0);
			item.rare = 2;
			item.shoot = mod.ProjectileType("SnapClamProj");
			item.shootSpeed = 12f;
		}
	}
}
