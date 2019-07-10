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
	public class CarnageRay : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Carnage Ray");
			Tooltip.SetDefault("Fires a blood ray that splits if enemies are near it");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 35;
	        item.magic = true;
	        item.mana = 10;
	        item.width = 46;
	        item.height = 46;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 3.25f;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 4;
	        item.UseSound = SoundID.Item72;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("BloodRay");
	        item.shootSpeed = 6f;
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.WandofSparking);
	        recipe.AddIngredient(ItemID.CrimsonRod);
	        recipe.AddIngredient(ItemID.AmberStaff);
	        recipe.AddIngredient(ItemID.MagicMissile);
	        recipe.AddIngredient(null, "BloodSample", 15);
	        recipe.AddIngredient(null, "PurifiedGel", 10);
	        recipe.AddTile(TileID.DemonAltar);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
