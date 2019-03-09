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
	public class FeatherKnife : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Feather Knife");
		}

		public override void SafeSetDefaults()
		{
			item.width = 18;
			item.damage = 20;
			item.noMelee = true;
			item.consumable = true;
			item.noUseGraphic = true;
			item.useAnimation = 13;
			item.useStyle = 1;
			item.useTime = 13;
			item.knockBack = 2f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 32;
			item.maxStack = 999;
			item.value = 300;
			item.rare = 3;
			item.shoot = mod.ProjectileType("FeatherKnifeProjectile");
			item.shootSpeed = 12f;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "AerialiteBar");
	        recipe.AddTile(TileID.SkyMill);
	        recipe.SetResult(this, 30);
	        recipe.AddRecipe();
		}
	}
}
