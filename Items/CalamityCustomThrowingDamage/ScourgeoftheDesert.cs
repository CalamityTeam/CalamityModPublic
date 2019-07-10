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
	public class ScourgeoftheDesert : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scourge of the Desert");
			Tooltip.SetDefault("Gains velocity over time");
		}

		public override void SafeSetDefaults()
		{
			item.width = 44;
			item.damage = 16;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 20;
			item.useStyle = 1;
			item.useTime = 20;
			item.knockBack = 3.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 44;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
			item.shoot = mod.ProjectileType("ScourgeoftheDesert");
			item.shootSpeed = 12f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}
	}
}
