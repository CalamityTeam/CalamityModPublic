using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
	public class BlushieStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Staff of Blushie");
			Tooltip.SetDefault("Hold your mouse, wait, wait, wait, and put your trust in the power of blue magic");
		}

		public override void SetDefaults()
		{
			item.width = 38;
			item.height = 38;
			item.useStyle = 4;
			item.useAnimation = 30;
			item.useTime = 30;
			item.channel = true;
			item.noMelee = true;
			item.damage = 1;
			item.knockBack = 1f;
			item.autoReuse = false;
			item.useTurn = false;
			item.rare = 11;
			item.magic = true;
			item.value = 100000000;
			item.UseSound = SoundID.Item1;
			item.shoot = mod.ProjectileType("BlushieStaffProj");
			item.mana = 200;
			item.shootSpeed = 0f;
		}
		
		public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(0, 0, 255);
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