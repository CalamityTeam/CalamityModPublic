using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons 
{
	public class ScourgeoftheDesert : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scourge of the Desert");
            Tooltip.SetDefault("Revengeance drop");
        }

		public override void SetDefaults()
		{
			item.width = 44;
			item.damage = 18;
			item.thrown = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 20;
			item.useStyle = 1;
			item.useTime = 20;
			item.knockBack = 3.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 44;
			item.value = 15000;
			item.rare = 2;
			item.shoot = mod.ProjectileType("ScourgeoftheDesert");
			item.shootSpeed = 12f;
		}
	}
}
