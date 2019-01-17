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
	public class PhantasmalFury : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantasmal Fury");
			Tooltip.SetDefault("Casts a phantasmal bolt that explodes into more bolts");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 363;
	        item.magic = true;
	        item.mana = 20;
	        item.width = 62;
	        item.height = 60;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 7.5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item43;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("PhantasmalFury");
	        item.shootSpeed = 12f;
	    }
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(0, 255, 0);
	            }
	        }
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.SpectreStaff);
	        recipe.AddIngredient(null, "Phantoplasm", 5);
	        recipe.AddIngredient(null, "DarkPlasma");
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}