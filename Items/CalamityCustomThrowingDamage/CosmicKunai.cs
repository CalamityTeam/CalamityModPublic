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
	public class CosmicKunai : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmic Kunai");
			Tooltip.SetDefault("Fires a stream of short-range kunai");
		}

		public override void SafeSetDefaults()
		{
			item.width = 26;
			item.damage = 200;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useTime = 1;
			item.useAnimation = 5;
			item.useStyle = 1;
			item.knockBack = 5f;
			item.UseSound = SoundID.Item109;
			item.autoReuse = true;
			item.height = 48;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("CosmicKunai");
			item.shootSpeed = 28f;
			item.rare = 9;
		}
	}
}
