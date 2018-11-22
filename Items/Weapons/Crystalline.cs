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
	public class Crystalline : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crystalline");
		}

		public override void SetDefaults()
		{
			item.width = 44;
			item.damage = 18;
			item.crit += 4;
			item.thrown = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 18;
			item.useStyle = 1;
			item.useTime = 18;
			item.knockBack = 3f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 44;
			item.maxStack = 1;
			item.value = 52000;
			item.rare = 2;
			item.shoot = mod.ProjectileType("Crystalline");
			item.shootSpeed = 10f;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.ThrowingKnife, 50);
	        recipe.AddIngredient(ItemID.Diamond, 3);
	        recipe.AddIngredient(ItemID.FallenStar, 3);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
