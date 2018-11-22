using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories {
public class CrimsonFlask : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Crimson Flask");
		Tooltip.SetDefault("7% increased damage reduction and +3 defense while in the crimson");
	}
	
	public override void SetDefaults()
	{
		item.width = 20;
		item.height = 20;
		item.value = 50000;
		item.rare = 2;
		item.accessory = true;
	}
	
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		if (player.ZoneCrimson)
		{
			player.statDefense += 3;
	    	player.endurance += 0.07f;
		}
	}
	
	public override void AddRecipes()
    {
        ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(null, "BloodlettingEssence", 3);
        recipe.AddIngredient(ItemID.Vertebrae, 10);
        recipe.AddTile(TileID.Anvils);
        recipe.SetResult(this);
        recipe.AddRecipe();
    }
}}