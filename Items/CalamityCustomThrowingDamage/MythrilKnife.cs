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
	public class MythrilKnife : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mythril Knife");
		}

		public override void SafeSetDefaults()
		{
			item.width = 12;
			item.damage = 32;
			item.noMelee = true;
			item.consumable = true;
			item.noUseGraphic = true;
			item.useAnimation = 10;
			item.useStyle = 1;
			item.useTime = 10;
			item.knockBack = 1.75f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 30;
			item.maxStack = 999;
			item.value = 1100;
			item.rare = 4;
			item.shoot = mod.ProjectileType("MythrilKnifeProjectile");
			item.shootSpeed = 12f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.MythrilBar);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this, 40);
	        recipe.AddRecipe();
		}
	}
}
