using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.AbyssWeapons 
{
	public class Lionfish : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lionfish");
		}

		public override void SetDefaults()
		{
			item.width = 40;
			item.damage = 54;
			item.thrown = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 22;
			item.useStyle = 1;
			item.useTime = 22;
			item.knockBack = 2.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 40;
			item.value = 50000;
			item.rare = 3;
			item.shoot = mod.ProjectileType("Lionfish");
			item.shootSpeed = 12f;
		}
	}
}
