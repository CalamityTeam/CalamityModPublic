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
	public class ExecutionersBlade : CalamityDamageItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Executioner's Blade");
			Tooltip.SetDefault("Throws a stream of homing blades");
		}

		public override void SafeSetDefaults()
		{
			item.width = 50;
			item.damage = 550;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useTime = 3;
			item.useAnimation = 9;
			item.useStyle = 1;
			item.knockBack = 6.75f;
			item.UseSound = SoundID.Item73;
			item.autoReuse = true;
			item.height = 50;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("ExecutionersBlade");
			item.shootSpeed = 26f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).rogue = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}
		
		public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "CosmiliteBar", 11);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
