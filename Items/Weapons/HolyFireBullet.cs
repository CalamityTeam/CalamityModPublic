using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
	public class HolyFireBullet : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Holy Fire Bullet");
			Tooltip.SetDefault("Explosive holy bullets");
		}

		public override void SetDefaults()
		{
			item.damage = 27;
			item.ranged = true;
			item.width = 8;
			item.height = 8;
			item.maxStack = 999;
			item.consumable = true;
			item.knockBack = 2f;
			item.value = 2000;
			item.rare = 10;
			item.shoot = mod.ProjectileType("HolyFireBullet");
			item.shootSpeed = 12f;
			item.ammo = 97;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ExplodingBullet, 100);
			recipe.AddIngredient(null, "UnholyEssence");
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this, 100);
			recipe.AddRecipe();
		}
	}
}