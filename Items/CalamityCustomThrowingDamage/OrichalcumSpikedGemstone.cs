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
	public class OrichalcumSpikedGemstone : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Orichalcum Spiked Gemstone");
		}

		public override void SafeSetDefaults()
		{
			item.width = 14;
			item.damage = 25;
			item.noMelee = true;
			item.consumable = true;
			item.noUseGraphic = true;
			item.useAnimation = 13;
			item.useStyle = 1;
			item.useTime = 13;
			item.knockBack = 2f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 24;
			item.shoot = 330;
			item.maxStack = 999;
			item.value = 1200;
			item.rare = 4;
			item.shoot = mod.ProjectileType("OrichalcumSpikedGemstoneProjectile");
			item.shootSpeed = 12f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.OrichalcumBar);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this, 50);
	        recipe.AddRecipe();
		}
	}
}
