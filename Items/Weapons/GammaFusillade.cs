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
	public class GammaFusillade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Biofusillade");
			Tooltip.SetDefault("Unleashes a concentrated beam of life energy");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 67;
	        item.magic = true;
	        item.mana = 4;
	        item.width = 28;
	        item.height = 30;
	        item.useTime = 3;
	        item.useAnimation = 3;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3;
	        item.value = 1250000;
	        item.UseSound = SoundID.Item33;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("GammaLaser");
	        item.shootSpeed = 20f;
	    }
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(0, 255, 200);
	            }
	        }
	    }
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "UeliaceBar", 8);
	        recipe.AddIngredient(ItemID.SpellTome);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}