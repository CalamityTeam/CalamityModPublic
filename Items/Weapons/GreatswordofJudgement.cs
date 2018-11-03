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
	public class GreatswordofJudgement : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Greatsword of Judgement");
			Tooltip.SetDefault("A pale white sword from a forgotten land\n" +
			                   "You can hear faint yet comforting whispers emanating from the blade\n" +
					            "'No matter where you may be you are never alone.\n" +
					            "I shall always be at your side, my lord'");
		}

		public override void SetDefaults()
		{
			item.width = 70;
			item.damage = 58;
			item.melee = true;
			item.useAnimation = 18;
			item.useStyle = 1;
			item.useTime = 18;
			item.useTurn = true;
			item.knockBack = 7f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 72;
			item.value = 5000000;
			item.shoot = mod.ProjectileType("Judgement");
			item.shootSpeed = 10f;
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
			recipe.AddIngredient(ItemID.LunarBar, 11);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
