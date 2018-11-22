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
	public class GreatswordofBlah : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Greatsword of Blah");
			Tooltip.SetDefault("A pale white sword from a forgotten land\n" +
			                   "You can hear faint yet comforting whispers emanating from the blade\n" +
					            "'No matter where you may be you are never alone.\n" +
					            "I shall always be at your side, my lord'");
		}

		public override void SetDefaults()
		{
			item.width = 110;
			item.damage = 410;
			item.melee = true;
			item.useAnimation = 18;
			item.useStyle = 1;
			item.useTime = 18;
			item.useTurn = true;
			item.knockBack = 7f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 110;
			item.value = 10000000;
			item.shoot = mod.ProjectileType("JudgementBlah");
			item.shootSpeed = 12f;
		}
		
		public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(108, 45, 199);
	            }
	        }
	    }
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "GreatswordofJudgement");
			recipe.AddIngredient(null, "NightmareFuel", 10);
        	recipe.AddIngredient(null, "EndothermicEnergy", 10);
			recipe.AddIngredient(null, "DarksunFragment", 10);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
