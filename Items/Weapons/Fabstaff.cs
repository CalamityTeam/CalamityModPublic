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
	public class Fabstaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fabstaff");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 330;
	        item.magic = true;
	        item.mana = 50;
	        item.width = 84;
	        item.height = 84;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5f;
	        item.value = 100000000;
	        item.UseSound = SoundID.Item60;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("FabRay");
	        item.shootSpeed = 6f;
	    }
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(Main.DiscoR, 100, 255);
	            }
	        }
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "Phantoplasm", 100);
	        recipe.AddIngredient(null, "ShadowspecBar", 50);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}