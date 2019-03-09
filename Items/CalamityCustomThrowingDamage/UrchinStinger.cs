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
	public class UrchinStinger : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Urchin Stinger");
		}

		public override void SafeSetDefaults()
		{
			item.width = 10;
			item.damage = 15;
			item.noMelee = true;
			item.consumable = true;
			item.noUseGraphic = true;
			item.useAnimation = 14;
			item.useStyle = 1;
			item.useTime = 14;
			item.knockBack = 1.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 26;
			item.maxStack = 999;
			item.value = 200;
			item.rare = 1;
			item.shoot = mod.ProjectileType("UrchinStinger");
			item.shootSpeed = 12f;
		}
	}
}
