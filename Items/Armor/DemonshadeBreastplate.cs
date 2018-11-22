using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;

namespace CalamityMod.Items.Armor 
{
	[AutoloadEquip(EquipType.Body)]
	public class DemonshadeBreastplate : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonshade Breastplate");
            Tooltip.SetDefault("20% increased melee speed, 20% increased damage and critical strike chance\n" +
                       "Enemies take ungodly damage when they touch you\n" +
                       "Increased max life and mana by 200\n" +
                       "Standing still lets you absorb the shadows and boost your life regen");
        }

	    public override void SetDefaults()
	    {
	        item.width = 18;
	        item.height = 18;
	        item.value = 10000000;
	        item.defense = 50;
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
	    	modPlayer.shadeRegen = true;
	    	player.thorns = 100f;
	    	player.statLifeMax2 += 200;
	        player.statManaMax2 += 200;
	        player.magicCrit += 20;
			player.magicDamage += 0.2f;
			player.meleeCrit += 20;
			player.meleeDamage += 0.2f;
			player.thrownCrit += 20;
			player.thrownDamage += 0.2f;
			player.rangedCrit += 20;
			player.rangedDamage += 0.2f;
			player.minionDamage += 0.2f;
			player.meleeSpeed += 0.2f;
	    }
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "ShadowspecBar", 15);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}