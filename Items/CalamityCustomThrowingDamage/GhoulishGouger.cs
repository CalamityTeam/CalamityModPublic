using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
	public class GhoulishGouger : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ghoulish Gouger");
			Tooltip.SetDefault("Throws a ghoulish scythe");
		}

		public override void SafeSetDefaults()
		{
			item.width = 68;
			item.damage = 160;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 12;
			item.useTime = 12;
			item.useStyle = 1;
			item.knockBack = 7.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 60;
			item.value = 1000000;
			item.shoot = mod.ProjectileType("GhoulishGouger");
			item.shootSpeed = 20f;
		}
	}
}
