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
	public class ElementalRay : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Elemental Ray");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 178;
	        item.magic = true;
	        item.mana = 18;
	        item.width = 62;
	        item.height = 62;
	        item.useTime = 16;
	        item.useAnimation = 16;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 7.5f;
	        item.value = 10000000;
	        item.UseSound = SoundID.Item60;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("ElementRay");
	        item.shootSpeed = 6f;
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
	        recipe.AddIngredient(null, "GalacticaSingularity", 5);
	        recipe.AddIngredient(ItemID.LunarBar, 5);
	        recipe.AddIngredient(null, "TerraRay");
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}