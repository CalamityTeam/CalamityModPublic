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
	public class SandDollar : CalamityDamageItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sand Dollar");
			Tooltip.SetDefault("Stacks up to 2");
		}

		public override void SafeSetDefaults()
		{
			item.width = 30;
			item.height = 28;
			item.damage = 30;
			item.thrown = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useTime = 15;
			item.useAnimation = 15;
			item.useStyle = 1;
			item.maxStack = 2;
			item.knockBack = 3.5f;
			item.autoReuse = true;
			item.UseSound = SoundID.Item1;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.rare = 2;
			item.shoot = mod.ProjectileType("SandDollarProj");
			item.shootSpeed = 14f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}

		public override bool CanUseItem(Player player)
		{
			int MAX = item.stack;
		    int launched = 0;
		    foreach (Projectile projectile in Main.projectile)
		    if (projectile.type == item.shoot && projectile.owner == item.owner && projectile.active)
			{
		        launched++;
			}
		    return (launched >= MAX) ? false : true;
		}
	}
}
