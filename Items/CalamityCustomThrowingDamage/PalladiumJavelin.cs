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
	public class PalladiumJavelin : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Palladium Javelin");
		}

		public override void SafeSetDefaults()
		{
			item.width = 44;
			item.damage = 41;
			item.noMelee = true;
			item.consumable = true;
			item.noUseGraphic = true;
			item.useAnimation = 19;
			item.useStyle = 1;
			item.useTime = 19;
			item.knockBack = 5.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 44;
			item.shoot = 330;
			item.maxStack = 999;
			item.value = 1200;
			item.rare = 4;
			item.shoot = mod.ProjectileType("PalladiumJavelinProjectile");
			item.shootSpeed = 16f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.PalladiumBar);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this, 20);
	        recipe.AddRecipe();
		}
	}
}
