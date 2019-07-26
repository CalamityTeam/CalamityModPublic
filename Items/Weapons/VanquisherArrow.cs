using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
	public class VanquisherArrow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vanquisher Arrow");
			Tooltip.SetDefault("Pierces through tiles\n" +
                "Spawns extra homing arrows as it travels");
		}

		public override void SetDefaults()
		{
			item.damage = 33;
			item.ranged = true;
			item.width = 14;
			item.height = 36;
			item.maxStack = 999;
			item.consumable = true;
			item.knockBack = 3.5f;
			item.value = 2250;
			item.shoot = mod.ProjectileType("VanquisherArrow");
			item.shootSpeed = 10f;
			item.ammo = 40;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "CosmiliteBar");
			recipe.AddTile(null, "DraedonsForge");
			recipe.SetResult(this, 250);
			recipe.AddRecipe();
		}
	}
}
