using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Armor 
{
	[AutoloadEquip(EquipType.Legs)]
	public class DemonshadeGreaves : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonshade Greaves");
            Tooltip.SetDefault("Shadow speed");
        }

	    public override void SetDefaults()
	    {
	        item.width = 18;
	        item.height = 18;
	        item.value = 10000000;
	        item.defense = 50; //15
	    }
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(255, 0, 255);
	            }
	        }
	    }
	
	    public override void UpdateEquip(Player player)
	    {
	    	CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
    		modPlayer.shadowSpeed = true;
	        player.moveSpeed += 1f;
	    }
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "ShadowspecBar", 11);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}