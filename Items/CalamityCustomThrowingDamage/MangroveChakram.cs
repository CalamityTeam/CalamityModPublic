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
	public class MangroveChakram : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mangrove Chakram");
		}

		public override void SafeSetDefaults()
		{
			item.width = 38;
			item.damage = 84;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useAnimation = 14;
			item.useStyle = 1;
			item.useTime = 14;
			item.knockBack = 7.5f;
			item.UseSound = SoundID.Item1;
			item.height = 38;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
			item.shoot = mod.ProjectileType("MangroveChakramProjectile");
			item.shootSpeed = 15.5f;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "DraedonBar", 7);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
