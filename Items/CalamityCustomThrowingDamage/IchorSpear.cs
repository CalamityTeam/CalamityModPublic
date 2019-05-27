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
	public class IchorSpear : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ichor Spear");
		}

		public override void SafeSetDefaults()
		{
			item.width = 52;
			item.damage = 40;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 20;
			item.useStyle = 1;
			item.useTime = 20;
			item.knockBack = 6f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 52;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
			item.shoot = mod.ProjectileType("IchorSpear");
			item.shootSpeed = 20f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}
	}
}
