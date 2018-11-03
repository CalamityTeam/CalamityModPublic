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
	public class FlameScythe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flame Scythe");
		}

		public override void SetDefaults()
		{
			item.width = 50;
			item.damage = 130;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.autoReuse = true;
			item.useAnimation = 19;
			item.useStyle = 1;
			item.useTime = 19;
			item.knockBack = 8.5f;
			item.UseSound = SoundID.Item1;
			item.thrown = true;
			item.height = 48;
			item.value = 800000;
			item.rare = 8;
			item.shoot = mod.ProjectileType("FlameScytheProjectile");
			item.shootSpeed = 16f;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "CruptixBar", 9);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
