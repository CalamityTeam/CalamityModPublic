using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
	public class VeriumBullet : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Verium Bullet");
			Tooltip.SetDefault("There is no escape!");
		}
		
		public override void SetDefaults()
		{
			item.damage = 8;
			item.ranged = true;
			item.width = 8;
			item.height = 8;
			item.maxStack = 999;
			item.consumable = true;
			item.knockBack = 1.25f;
			item.value = 500;
			item.rare = 3;
			item.shoot = mod.ProjectileType("VeriumBullet");
			item.shootSpeed = 16f;
			item.ammo = 97;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.MusketBall, 100);
			recipe.AddIngredient(null, "VerstaltiteBar");
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this, 100);
			recipe.AddRecipe();
		}
	}
}