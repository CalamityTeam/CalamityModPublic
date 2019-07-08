using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
	public class HyperiusBullet : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hyperius Bullet");
			Tooltip.SetDefault("Your enemies might have a bad time\n" +
				"Spawns additional bullets on enemy hits");
		}

		public override void SetDefaults()
		{
			item.damage = 21;
			item.ranged = true;
			item.width = 8;
			item.height = 8;
			item.maxStack = 999;
			item.consumable = true;
			item.knockBack = 1.5f;
			item.value = 2000;
			item.rare = 9;
			item.shoot = mod.ProjectileType("HyperiusBullet");
			item.shootSpeed = 16f;
			item.ammo = 97;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.MusketBall, 100);
			recipe.AddIngredient(null, "BarofLife");
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this, 100);
			recipe.AddRecipe();
		}
	}
}