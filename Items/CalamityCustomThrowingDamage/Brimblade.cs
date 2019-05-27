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
	public class Brimblade : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimblade");
		}

		public override void SafeSetDefaults()
		{
			item.width = 26;
			item.damage = 32;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.autoReuse = true;
			item.useAnimation = 18;
			item.useStyle = 1;
			item.useTime = 18;
			item.knockBack = 6.5f;
			item.UseSound = SoundID.Item1;
			item.height = 26;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 6;
			item.shoot = mod.ProjectileType("Brimblade");
			item.shootSpeed = 12f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "UnholyCore", 4);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
