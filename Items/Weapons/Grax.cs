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
	public class Grax : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Grax");
			Tooltip.SetDefault("Hitting an enemy will greatly boost your defense and melee stats for a short time");
		}

		public override void SetDefaults()
		{
			item.width = 60;
			item.damage = 450;
			item.melee = true;
			item.useAnimation = 25;
			item.useStyle = 1;
			item.useTime = 5;
			item.useTurn = true;
			item.axe = 50;
			item.hammer = 200;
			item.knockBack = 8f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 60;
			item.value = 5000000;
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
			recipe.AddIngredient(null, "FellerofEvergreens");
			recipe.AddIngredient(null, "DraedonBar", 5);
			recipe.AddRecipeGroup("LunarAxe");
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			player.AddBuff(mod.BuffType("GraxDefense"), 480);
		}
	}
}
