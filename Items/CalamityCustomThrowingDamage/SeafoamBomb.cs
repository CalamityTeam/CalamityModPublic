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
	public class SeafoamBomb : CalamityDamageItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seafoam Bomb");
			Tooltip.SetDefault("Throws a bomb that explodes into a bubble which deals extra damage to enemies");
		}

		public override void SafeSetDefaults()
		{
			item.width = 26;
			item.height = 44;
			item.damage = 18;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 18;
			item.useStyle = 1;
			item.useTime = 18;
			item.knockBack = 8f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.value = Item.buyPrice(0, 2, 0, 0);
			item.rare = 2;
			item.shoot = mod.ProjectileType("SeafoamBomb");
			item.shootSpeed = 8f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Bomb, 25);
	        recipe.AddIngredient(null, "SeaPrism", 10);
			recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
