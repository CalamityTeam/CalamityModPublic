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
	public class Pwnagehammer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pwnagehammer");
		}

		public override void SetDefaults()
		{
			item.width = 68;
			item.damage = 90;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useStyle = 1;
			item.useTime = 15;
			item.knockBack = 10f;
			item.UseSound = SoundID.Item1;
			item.thrown = true;
			item.height = 68;
			item.value = 300000;
			item.rare = 6;
			item.shoot = mod.ProjectileType("Pwnagehammer");
			item.shootSpeed = 12f;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Pwnhammer);
			recipe.AddIngredient(ItemID.HallowedBar, 7);
            recipe.AddIngredient(ItemID.SoulofMight, 3);
            recipe.AddIngredient(ItemID.SoulofSight, 3);
            recipe.AddIngredient(ItemID.SoulofFright, 3);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
