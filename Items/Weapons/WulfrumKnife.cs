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
	public class WulfrumKnife : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wulfrum Knife");
		}

		public override void SetDefaults()
		{
			item.width = 22;
			item.damage = 8;
			item.thrown = true;
			item.noMelee = true;
			item.consumable = true;
			item.noUseGraphic = true;
			item.useAnimation = 15;
			item.useStyle = 1;
			item.useTime = 15;
			item.knockBack = 1f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 38;
			item.maxStack = 999;
			item.value = 100;
			item.rare = 1;
			item.shoot = mod.ProjectileType("WulfrumKnife");
			item.shootSpeed = 12f;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "WulfrumShard");
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this, 50);
	        recipe.AddRecipe();
		}
	}
}
