using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
	public class RadiantStar : CalamityDamageItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Radiant Star");
			Tooltip.SetDefault("Throws daggers that explode and split after a while");
		}

		public override void SafeSetDefaults()
		{
			item.width = 52;
			item.damage = 50; //33
			item.crit += 8;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 12;
			item.useStyle = 1;
			item.useTime = 12;
			item.knockBack = 5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 48;
			item.value = Item.buyPrice(0, 95, 0, 0);
			item.rare = 9;
			item.shoot = mod.ProjectileType("RadiantStar");
			item.shootSpeed = 20f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "Prismalline");
	        recipe.AddIngredient(null, "AstralBar", 10);
			recipe.AddIngredient(null, "Stardust", 15);
			recipe.AddIngredient(ItemID.FallenStar, 10);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
