using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons 
{
	public class ExecutionersBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Executioner's Blade");
		}

		public override void SetDefaults()
		{
			item.width = 50;
			item.damage = 550;
			item.thrown = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.useTime = 3;
			item.useAnimation = 9;
			item.useStyle = 1;
			item.knockBack = 6.75f;
			item.UseSound = SoundID.Item73;
			item.autoReuse = true;
			item.height = 50;
			item.value = 1350000;
			item.shoot = mod.ProjectileType("ExecutionersBlade");
			item.shootSpeed = 26f;
		}
		
		public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(43, 96, 222);
	            }
	        }
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
