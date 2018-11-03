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
	public class CobaltKunai : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cobalt Kunai");
		}

		public override void SetDefaults()
		{
			item.width = 18;
			item.damage = 28;
			item.thrown = true;
			item.noMelee = true;
			item.consumable = true;
			item.noUseGraphic = true;
			item.useAnimation = 12;
			item.scale = 0.75f;
			item.useStyle = 1;
			item.useTime = 12;
			item.knockBack = 2.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 40;
			item.maxStack = 999;
			item.value = 900;
			item.rare = 4;
			item.shoot = mod.ProjectileType("CobaltKunaiProjectile");
			item.shootSpeed = 12f;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.CobaltBar);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this, 30);
	        recipe.AddRecipe();
		}
	}
}
