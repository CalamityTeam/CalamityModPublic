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
	public class MeteorFist : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Meteor Fist");
		}

		public override void SetDefaults()
		{
			item.width = 22;
			item.damage = 15;
			item.melee = true;
			item.noMelee = true;
			item.useTurn = true;
			item.noUseGraphic = true;
			item.useAnimation = 30;
			item.useStyle = 5;
			item.useTime = 30;
			item.knockBack = 5.75f;
			item.UseSound = SoundID.Item20;
			item.autoReuse = true;
			item.height = 28;
			item.value = 75000;
			item.rare = 2;
			item.shoot = mod.ProjectileType("MeteorFist");
			item.shootSpeed = 10f;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.MeteoriteBar, 10);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
