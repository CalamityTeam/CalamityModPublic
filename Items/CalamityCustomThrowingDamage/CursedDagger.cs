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
	public class CursedDagger : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cursed Dagger");
		}

		public override void SafeSetDefaults()
		{
			item.width = 34;
			item.damage = 34;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 16;
			item.useStyle = 1;
			item.useTime = 16;
			item.knockBack = 4.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 34;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
			item.shoot = mod.ProjectileType("CursedDagger");
			item.shootSpeed = 12f;
		}
	}
}
