using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
	public class BloodfireArrow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bloodfire Arrow");
			Tooltip.SetDefault("Heals you a small amount on enemy hits");
		}

		public override void SetDefaults()
		{
			item.damage = 40;
			item.ranged = true;
			item.width = 14;
			item.height = 36;
			item.maxStack = 999;
			item.consumable = true;
			item.knockBack = 3.5f;
			item.value = 2000;
			item.shoot = mod.ProjectileType("BloodfireArrow");
			item.shootSpeed = 10f;
			item.ammo = 40;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "BloodstoneCore");
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this, 250);
			recipe.AddRecipe();
		}
	}
}